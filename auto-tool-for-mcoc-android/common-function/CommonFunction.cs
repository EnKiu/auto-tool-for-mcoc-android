using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidAutoMCOC
{
    public static class CommonFunction
    {

        #region Declaration

        private static string ADB_FOLDER_PATH = "";
        private static string LIST_DEVICES = "adb devices";
        private static string GET_SCREEN_RESOLUTION = "adb -s {0} shell dumpsys display | Find \"{1}\"";
        private static string SWIPE_DEVICES = "adb -s {0} shell input swipe {1} {2} {3} {4} {5}";
        private static string TAP_DEVICES = "adb -s {0} shell input tap {1} {2}";
        private static string CAPTURE_SCREEN_TO_DEVICES = "adb -s {0} shell screencap -p \"{1}\"";
        private static string PULL_SCREEN_FROM_DEVICES = "adb -s {0} pull \"{1}\"";
        private static string REMOVE_SCREEN_FROM_DEVICES = "adb -s {0} shell rm -f \"{1}\"";
        #endregion

        #region Property

        public static Dictionary<string, Point> ResolutionByDeviceID { get; set; } = new Dictionary<string, Point>();
        #endregion

        public static string ExecuteCMD(string cmdCommand)
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.WorkingDirectory = ADB_FOLDER_PATH;
                processStartInfo.FileName = "cmd.exe";
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;
                process.StartInfo = processStartInfo;
                process.Start();
                process.StandardInput.WriteLine(cmdCommand);
                process.StandardInput.Flush();
                process.StandardInput.Close();
                process.WaitForExit();
                return process.StandardOutput.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }

        public static List<string> GetDevices()
        {
            List<string> list = new List<string>();
            string input = ExecuteCMD(LIST_DEVICES);
            string pathRoot = Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory);
            pathRoot = pathRoot.Replace("\\", "");
            string pattern = "(?<=List of devices attached ).*?(?=" + pathRoot + ")";
            MatchCollection matchCollection = Regex.Matches(input, pattern, RegexOptions.Singleline);
            if (matchCollection != null && matchCollection.Count > 0)
            {
                foreach (object item2 in matchCollection)
                {
                    string text = item2.ToString();
                    string[] array = text.Split(new string[1] { "device" }, StringSplitOptions.None);
                    for (int i = 0; i < array.Length - 1; i++)
                    {
                        string text2 = array[i];
                        string item = text2.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                        list.Add(item);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Sửa lại hàm của thằng host vì dòng của mình nó ưu tiên mCurrentLayerStackRect trước rồi mới đến mCurrentDisplayRect
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public static Point GetScreenResolution(string deviceID)
        {
            Point screenResolution;
            if (ResolutionByDeviceID.ContainsKey(deviceID))
            {
                screenResolution = ResolutionByDeviceID[deviceID];
            }
            else
            {
                string cmdCommand = string.Format(GET_SCREEN_RESOLUTION, deviceID, "mCurrentLayerStackRect");
                string text = ExecuteCMD(cmdCommand);

                if (string.IsNullOrWhiteSpace(text))
                {
                    cmdCommand = string.Format(GET_SCREEN_RESOLUTION, deviceID, "mCurrentDisplayRect");
                    text = ExecuteCMD(cmdCommand);
                }
                text = text.Substring(text.IndexOf("- "));
                text = text.Substring(text.IndexOf(' '), text.IndexOf(')') - text.IndexOf(' '));
                string[] array = text.Split(',');
                int x = Convert.ToInt32(array[0].Trim());
                int y = Convert.ToInt32(array[1].Trim());
                screenResolution = new Point(x, y);

                //Add vào dictionary để sau lấy cho dễ
                ResolutionByDeviceID.Add(deviceID, screenResolution);
            }

            return screenResolution;
        }

        /// <summary>
        /// Do hàm mặc định của ADBHelper dành cho Vertical (máy dựng thẳng) nên cần có hàm riêng cho Horizontal
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="duration"></param>
        public static void SwipeByPercent(string deviceID, Point screenResolution, double x1, double y1, double x2, double y2, int duration = 100)
        {
            //Point screenResolution = GetScreenResolution(deviceID);
            int num = (int)(x1 * ((double)screenResolution.X * 1.0 / 100.0));
            int num2 = (int)(y1 * ((double)screenResolution.Y * 1.0 / 100.0));
            int num3 = (int)(x2 * ((double)screenResolution.X * 1.0 / 100.0));
            int num4 = (int)(y2 * ((double)screenResolution.Y * 1.0 / 100.0));
            string cmdCommand = string.Format(SWIPE_DEVICES, deviceID, num, num2, num3, num4, duration);
            string text = ExecuteCMD(cmdCommand);
        }

        public static void Swipe(string deviceID, int x1, int y1, int x2, int y2, int duration = 100)
        {
            string cmdCommand = string.Format(SWIPE_DEVICES, deviceID, x1, y1, x2, y2, duration);
            string text = ExecuteCMD(cmdCommand);
        }

        public static void Tap(string deviceID, int x, int y)
        {
            string cmdCommand = string.Format(TAP_DEVICES, deviceID, x, y);
            string text = ExecuteCMD(cmdCommand);
        }

        public static void TapByPercent(string deviceID, Point screenResolution, double x, double y)
        {
            //Point screenResolution = GetScreenResolution(deviceID);
            int num = (int)(x * ((double)screenResolution.X * 1.0 / 100.0));
            int num2 = (int)(y * ((double)screenResolution.Y * 1.0 / 100.0));
            string cmdCommand = string.Format(TAP_DEVICES, deviceID, num, num2);
            string text = ExecuteCMD(cmdCommand);
        }

        public static Bitmap ScreenShoot(string deviceID = null, bool isDeleteImageAfterCapture = true, string fileName = "screenShoot.png")
        {
            //Không truyền device thì mặc định lấy device đầu tiên đang connect
            if (string.IsNullOrEmpty(deviceID))
            {
                List<string> devices = GetDevices();
                if (devices == null || devices.Count <= 0)
                {
                    return null;
                }

                deviceID = devices.First();
            }

            string text = deviceID;
            try
            {
                text = deviceID.Split(':')[1];
            }
            catch
            {
            }

            //Sinh ra file name mới đảm bảo không trùng
            string text2 = String.Format("{0}{1}{2}{3}", Path.GetFileNameWithoutExtension(fileName), text, Guid.NewGuid().ToString(), Path.GetExtension(fileName));
            
            //Sinh ra câu command để chụp ảnh màn hình lưu vào sdcard
            string text3 = string.Format(CAPTURE_SCREEN_TO_DEVICES, deviceID, "/sdcard/" + text2);
            text3 = text3 + Environment.NewLine + string.Format(PULL_SCREEN_FROM_DEVICES, deviceID, "/sdcard/" + text2);
            
            string text4 = ExecuteCMD(text3);
            Bitmap result;
            using (Bitmap original = new Bitmap(text2))
            {
                result = new Bitmap(original);
            }

            if (isDeleteImageAfterCapture)
            {
                try
                {
                    text3 = string.Format(REMOVE_SCREEN_FROM_DEVICES, deviceID, "/sdcard/" + text2) + Environment.NewLine;
                    ExecuteCMD(text3);
                    File.Delete(text2);
                }
                catch
                {
                }
            }

            return result;
        }
    }

    public enum ScreenCode
    {
        /// <summary>
        /// Chưa đoán được
        /// </summary>
        UNDEFINED,
        /// <summary>
        /// HOME DEFAULT (Fight hiện ra)
        /// </summary>
        HOME,

        /// <summary>
        /// Màn hình quà tặng hàng ngày
        /// </summary>
        TODAY_REWARD,
        /// <summary>
        /// Vẫn là HOME nhưng màn hình fight bị ẩn -> click vào Menu để hiện ra trước
        /// </summary>
        HOME_WITHOUT_FIGHT,

        /// <summary>
        /// Màn hình chọn các loại arena để đánh
        /// basic, featured, trail
        /// </summary>
        CHOSE_ARENA,

        /// <summary>
        /// Hero quickfill
        /// </summary>
        QUICK_FILL,

        /// <summary>
        /// Sau khi fill đủ hero thì tìm trận
        /// </summary>
        FIND_MATCH,

        /// <summary>
        /// Màn hình xem lại đội để tìm đối mới hoặc chiến
        /// </summary>
        FIGHT_PREVIEW_1,

        /// <summary>
        /// Màn hình show ra hero nào sẽ đập nhau với hero nào
        /// </summary>
        SET_LINEUP,

        /// <summary>
        /// Màn hình show ra hero nào sẽ đập nhau với hero nào và có thêm chữ next fight!
        /// </summary>
        FIGHT_PREVIEW_2,

        ON_FIGHT,

        FINISH_FIGHT_1,

        FIGHT_PREVIEW_3,

        FIGHT_REWARD,

        NEXT_SERIES,

    }

    public enum ArenaCode
    {
        /// <summary>
        /// Chưa đoán được
        /// </summary>
        UNDEFINED,
        /// <summary>
        /// 
        /// </summary>
        SIX_BASIC,
        /// <summary>
        /// 
        /// </summary>
        SIX_FEATURED,
        /// <summary>
        /// 
        /// </summary>
        SUMMONER_TRIAL,
    }

    public enum GameMode { 
        /// <summary>
        /// Vẫn đang ở các màn hình HOME
        /// </summary>
        HOME,

        /// <summary>
        /// Chọn loại Arena (trial, featured, basic)
        /// </summary>
        CHOOSEARENA,
        /// <summary>
        /// Tiến vào vòng lặp đánh, next fight liên tục
        /// </summary>
        FIGHT
    }
}
