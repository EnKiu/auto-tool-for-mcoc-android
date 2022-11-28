using KAutoHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Threading;
using System.Data.Common;
using System.Timers;
using AndroidAutoMCOC.Model;
using Newtonsoft.Json;
using System.IO;

namespace AndroidAutoMCOC
{
    /// <summary>
    /// Interaction logic for AutoConfig.xaml
    /// </summary>
    public partial class AutoConfig : Window
    {
        #region Declaration

        string gameAppName = "com.kabam.marvelbattle";

        //Cách tìm:
        //adb shell dumpsys package > dumpsys.txt
        //Sau đó vào file dumpsys.txt để tìm với AppName tương ứng
        string gameStartActivity = "com.explodingbarrel.Activity";

        bool isRunning;

        GameMode gameMode = GameMode.HOME;
        List<ArenaCode> RunnedArena = new List<ArenaCode>();
        ArenaCode currentArena;
        int countGame = 0;
        #endregion

        #region Property   

        /// <summary>
        /// Lấy Device ID
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// Lấy ArenaLocation từ bên ngoài
        /// </summary>
        public int ArenaLocation { get; set; }

        /// <summary>
        /// Số lần đấm đá để kết thúc 1 màn game trước khi tiến hành kiểm tra xem đấm đá xong chưa
        /// </summary>
        /// NQMinh 13.11.2022
        public int NumberOfCombo { get; set; }

        /// <summary>
        /// Lấy ScreenResolution 1 lần duy nhất để tránh bị lấy nhiều lần
        /// </summary>
        public System.Drawing.Point ScreenResolution { get; set; }

        /// <summary>
        /// Ghép config từ device id
        /// </summary>
        /// NQMinh 13.11.2022
        public string ConfigPath { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo thì gửi kèm thông tin DeviceID và ArenaLocation
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="arenaLocation"></param>
        /// NQMinh 13.11.2022
        public AutoConfig(string deviceID, int arenaLocation, int numberOfCombo)
        {
            InitializeComponent();

            InitFormData(deviceID, arenaLocation, numberOfCombo);
            LoadLastConfig();
        }
        #endregion

        #region Event

        /// <summary>
        /// Bắt đầu chạy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateBeforeStart())
            {
                return;
            }

            //Gán cờ bắt đầu auto
            isRunning = true;

            SetFormTitle();

            //Đang chạy thì đóng lại chạy lại từ đầu
            CommonFunction.ExecuteCMD(string.Format("adb shell am force-stop {0}", gameAppName));

            //Mở lại game
            CommonFunction.ExecuteCMD(string.Format("adb shell am start -n {0}/{1}", gameAppName, gameStartActivity));

            AutoRunningGame();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {

            //Gán giá trị default
            isRunning = true;
            gameMode = GameMode.FIGHT;

            OnArenaAction();
        }

        /// <summary>
        /// Dừng mọi hoạt động
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false;
        }

        /// <summary>
        /// Event trước khi đóng form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// NQMinh 13.11.2022
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveLastConfig();
        }
        #endregion

        #region Method

        /// <summary>
        /// Validate khi nhấn bắt đầu
        /// </summary>
        /// <returns></returns>
        bool ValidateBeforeStart()
        {
            //Validate trước khi chạy
            if (!chkSixStarBasic.IsChecked.Value && !chkSixStarFeatured.IsChecked.Value && !chkTrail.IsChecked.Value)
            {
                MessageBox.Show("Chưa chọn loại Arena để đánh (Tích vào các checkbox).");
                return false;
            }


            //Nếu điền Change Arena minute thì cần điền đúng định dạng
            if (numChangeArea.Value.HasValue && numChangeArea.Value < 0)
            {
                MessageBox.Show("Bỏ trống hoặc set = 0 để không change");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gán giá trị cho các property khi được khởi tạo form
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="arenaLocation"></param>
        /// <param name="numberOfCombo"></param>
        /// NQMinh 13.11.2022
        void InitFormData(string deviceID, int arenaLocation, int numberOfCombo)
        {
            DeviceID = deviceID;
            ArenaLocation = arenaLocation;
            NumberOfCombo = numberOfCombo;
            ConfigPath = string.Format("Config/{0}.json", DeviceID);
            ScreenResolution = CommonFunction.GetScreenResolution(deviceID);
        }

        /// <summary>
        /// Load config từ json và bind vào các control trên form
        /// </summary>
        /// NQMinh 13.11.2022
        void LoadLastConfig()
        {

            if (File.Exists(ConfigPath))
            {
                string deviceDetailConfig = File.ReadAllText(ConfigPath);
                var result = JsonConvert.DeserializeObject<DeviceDetailConfig>(deviceDetailConfig);

                //Gán giá trị cho các control trên form
                chkSixStarBasic.IsChecked = result.Basic;
                chkSixStarFeatured.IsChecked = result.Featured;
                chkTrail.IsChecked = result.Trial;
                numChangeArea.Value = result.ChangeArenaTime;
            }
            else
            {
                //Gán default cho các control trên form
                chkSixStarBasic.IsChecked = true;
                chkSixStarFeatured.IsChecked = true;
                chkTrail.IsChecked = true;
                numChangeArea.Value = 100;
            }
        }

        /// <summary>
        /// Save config từ vào json
        /// </summary>
        /// NQMinh 13.11.2022
        void SaveLastConfig()
        {
            var oConfig = new DeviceDetailConfig();
            //Gán giá trị cho các control trên form
            oConfig.Basic = chkSixStarBasic.IsChecked;
            oConfig.Featured = chkSixStarFeatured.IsChecked;
            oConfig.Trial = chkTrail.IsChecked;
            oConfig.ChangeArenaTime = numChangeArea.Value;

            var strConfig = JsonConvert.SerializeObject(oConfig);

            if (!Directory.Exists("Config"))
            {
                Directory.CreateDirectory("Config");
            }
            File.WriteAllText(ConfigPath, strConfig);
        }

        /// <summary>
        /// Set lại form title
        /// Do có cả việc set trong thread khác nên cứ đặt như này cho dễ
        /// </summary>        
        void SetFormTitle()
        {
            string title;
            if (IsAutoRunning())
            {
                title = string.Format("{0} on {1}", DeviceID, "Running");
            }
            else
            {
                title = string.Format("{0} is {1}", DeviceID, "Stopped");
            }
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate { Title = title; });
        }

        /// <summary>
        /// Hàm kiểm tra xem có nên chạy tiếp hay không
        /// </summary>
        /// <returns></returns>
        bool IsAutoRunning()
        {
            return isRunning;
        }

        /// <summary>
        /// Tự động chạy game
        /// </summary>
        /// NQMinh 13.11.2022
        /// Chỉ chạy theo 1 kịch bản từ đầu đến cuối
        void AutoRunningGame()
        {
            Task a = new Task(() =>
            {
                //Gán cờ bắt đầu auto
                isRunning = true;

                //Chạy đến khi vào được CHOOSEARENA
                while (gameMode == GameMode.HOME)
                {
                    DetectScreenAndActionForHome();
                }

                if (gameMode == GameMode.CHOOSEARENA)
                {
                    ChooseArenaAction();
                }

                if (gameMode == GameMode.FIGHT)
                {
                    OnArenaAction();
                }
            });

            a.Start();
        }

        /// <summary>
        /// Phán đoán màn hình và action tương ứng với màn hình
        /// </summary>
        void DetectScreenAndActionForHome()
        {
            if (!IsAutoRunning()) return;

            switch (DetectScreenForHome())
            {
                case ScreenCode.TODAY_REWARD:
                    TodayRewardAction();
                    break;

                case ScreenCode.HOME:
                    HomeAction();
                    break;

                case ScreenCode.UNDEFINED:
                    //Vào màn hình không rõ thì làm gì?
                    //Đóng game chạy lại chăng?
                    break;
            }
        }


        /// <summary>
        /// Detect ra screen để làm việc
        /// </summary>
        /// <returns></returns>
        ScreenCode DetectScreenForHome()
        {
            var returnValue = ScreenCode.UNDEFINED;

            //Chụp ảnh màn hình hiện tại
            var phonescreen = CommonFunction.ScreenShoot(DeviceID);

            //Check có phải màn hình Today reward
            var imageToCheck = (Bitmap)Bitmap.FromFile("images//today-view-calendar.png");
            var pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
            if (pointToCheck.HasValue)
            {
                returnValue = ScreenCode.TODAY_REWARD;
                return returnValue;
            }

            //Check có phải màn hình Home
            var settingButton = (Bitmap)Bitmap.FromFile("images//home-setting.png");
            var whatnewbutton = (Bitmap)Bitmap.FromFile("images//home-button-what-new.png");
            var actionButton = (Bitmap)Bitmap.FromFile("images//home-action.png");
            var settingPoint = ImageScanOpenCV.FindOutPoint(phonescreen, settingButton);
            var whatnewPoint = ImageScanOpenCV.FindOutPoint(phonescreen, whatnewbutton);
            var actionPoint = ImageScanOpenCV.FindOutPoint(phonescreen, actionButton);

            if (settingPoint.HasValue && whatnewPoint.HasValue && actionPoint.HasValue)
            {
                //Kiểm tra nút Fight có nằm ở đó không
                var fightImage = (Bitmap)Bitmap.FromFile("images//fight-icon.png");
                var fightPoint = ImageScanOpenCV.FindOutPoint(phonescreen, fightImage);
                if (fightPoint.HasValue)
                {
                    returnValue = ScreenCode.HOME;
                    return returnValue;
                }
                else
                {
                    returnValue = ScreenCode.HOME_WITHOUT_FIGHT;
                    return returnValue;
                }
            }

            //Check có phải màn hình Multiverse arena
            imageToCheck = (Bitmap)Bitmap.FromFile("images//multiverse-arena-button.png");
            pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
            if (pointToCheck.HasValue)
            {
                returnValue = ScreenCode.CHOSE_ARENA;
                return returnValue;
            }

            return returnValue;
        }

        /// <summary>
        /// Hành động khi gặp màn hình today reward
        /// </summary>
        void TodayRewardAction()
        {
            //Đóng today reward
            if (!IsAutoRunning()) return;
            CommonFunction.TapByPercent(DeviceID, ScreenResolution, 50, 90);
        }

        /// <summary>
        /// Hành động khi gặp màn hình home
        /// Nhấn vào Fight và chọn Multiverse arena
        /// </summary>
        void HomeAction()
        {
            //Nhấn vào Fight
            if (!IsAutoRunning()) return;
            Delay(1);

            var fightImage = (Bitmap)Bitmap.FromFile("images//fight-icon.png");
            var phonescreen = CommonFunction.ScreenShoot(DeviceID);
            var fightPoint = ImageScanOpenCV.FindOutPoint(phonescreen, fightImage);

            if (fightPoint.HasValue)
            {
                CommonFunction.Tap(DeviceID, fightPoint.Value.X + 50, fightPoint.Value.Y + 50);
            }

            //Do màn hình Fight cần vào chọn Multiverse arena nhưng không có action nào ở đó nên code luôn đoạn chọn vào step này
            if (!IsAutoRunning()) return;
            Delay(2);
            //Lướt sang phải 3 lần để đảm bảo đến cuối
            if (!IsAutoRunning()) return;
            for (int i = 0; i < 3; i++)
            {
                CommonFunction.SwipeByPercent(DeviceID, ScreenResolution, 60, 50, 20, 50, 200);
                Delay(1);
            }

            //Chọn vào Multiverse Arena theo config từ màn hình chính
            if (!IsAutoRunning()) return;
            Delay(1);
            double x = 0, y = 0;

            switch (ArenaLocation)
            {
                //Row 1 column 1
                case 0:
                    x = 90;
                    y = 35;
                    break;
                //Row 1 column 2
                case 1:
                    x = 90;
                    y = 75;
                    break;
                //Row 2 column 1
                case 2:
                    x = 75;
                    y = 35;
                    break;
                //Row 2 column 2
                case 3:
                    x = 75;
                    y = 75;
                    break;
                //Row 3 column 1
                case 4:
                    x = 50;
                    y = 35;
                    break;
                //Row 3 column 2
                case 5:
                    x = 60;
                    y = 70;
                    break;
                //Row 4 column 1
                case 6:
                    x = 45;
                    y = 35;
                    break;
                //Row 4 column 2
                case 7:
                    x = 45;
                    y = 70;
                    break;
            }
            if (x != 0 && y != 0)
            {
                //Nhấn vào Arena                
                CommonFunction.TapByPercent(DeviceID, ScreenResolution, x, y);
                //Chuyển game mode sang Fight để kết thúc vòng lặp bước sang phần fight
                gameMode = GameMode.CHOOSEARENA;
            }
        }

        /// <summary>
        /// Tiến hành nghỉ 1 chút
        /// </summary>
        /// <param name="secToDelay">số giây nghỉ</param>
        /// NQMinh 13.11.2022
        void Delay(int secToDelay)
        {
            Thread.Sleep(secToDelay * 1000);
        }

        /// <summary>
        /// Màn hình chọn arena để chiến
        /// //Chọn đúng loại arena được config theo thứ tự từ trái sang phải
        /// </summary>
        void ChooseArenaAction()
        {
            if (!IsAutoRunning()) return;
            Delay(1);
            Bitmap imageToCheck = null;

            //Nếu check vào 6Star basic và chưa được chạy
            bool basicChecked = false, featuredChecked = false, trailChecked = false;
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                basicChecked = chkSixStarBasic.IsChecked.Value;
                featuredChecked = chkSixStarFeatured.IsChecked.Value;
                trailChecked = chkTrail.IsChecked.Value;
            });

            //Nguyên tắc cắt ảnh:
            //Lấy từ đoạn 3vs3 bên trên đến hết 6 sao bên dưới
            if (basicChecked && !RunnedArena.Contains(ArenaCode.SIX_BASIC))
            {
                currentArena = ArenaCode.SIX_BASIC;
                RunnedArena.Add(currentArena);


                //Tìm vị trí thằng six basic rồi bấm                
                imageToCheck = (Bitmap)Bitmap.FromFile("images//6-star-basic.png");
            }
            else if (featuredChecked && !RunnedArena.Contains(ArenaCode.SIX_FEATURED))
            {
                currentArena = ArenaCode.SIX_FEATURED;
                RunnedArena.Add(currentArena);

                //Tìm vị trí thằng six featured rồi bấm                
                imageToCheck = (Bitmap)Bitmap.FromFile("images//6-star-featured.png");
            }
            else if (trailChecked && !RunnedArena.Contains(ArenaCode.SUMMONER_TRIAL))
            {
                currentArena = ArenaCode.SUMMONER_TRIAL;
                RunnedArena.Add(currentArena);

                //Tìm vị trí thằng trails rồi bấm                
                imageToCheck = (Bitmap)Bitmap.FromFile("images//common-trial.png");
            }
            else
            {
                //Chạy đủ 1 vòng từ trái qua phải thì clear danh sách đã chạy quay lại chạy từ đầu
                RunnedArena.Clear();
                ChooseArenaAction();
                return;
            }

            //Chụp ảnh màn hình hiện tại
            var phonescreen = CommonFunction.ScreenShoot(DeviceID);
            var pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);

            while (!pointToCheck.HasValue)
            {
                if (!IsAutoRunning()) return;
                Delay(1);
                
                phonescreen = CommonFunction.ScreenShoot(DeviceID);
                pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
                //Chưa tìm thấy thằng six basic thì nó nằm ở trang 2
                //Slide nhẹ sang rồi bấm vào
                CommonFunction.SwipeByPercent(DeviceID, ScreenResolution, 85, 50, 50, 50, 1000);
            }

            //X vào 9%, Y vào 50%
            if (pointToCheck.HasValue)
            {
                if (!IsAutoRunning()) return;
                Delay(1);
                CommonFunction.Tap(DeviceID, pointToCheck.Value.X + (ScreenResolution.X * 9 / 100), pointToCheck.Value.Y + (ScreenResolution.Y * 50 / 100));

                //Đã vào đến fight
                gameMode = GameMode.FIGHT;
            }
        }

        /// <summary>
        /// Vào đấm nhau
        /// </summary>
        void OnFightAction()
        {
            if (!IsAutoRunning()) return;
            //Thằng này ko có delay vì cần đấm nhanh
            bool keepFighting = true;
            //Default gía trị là 3
            int count = NumberOfCombo;

            while (count > 0 && keepFighting)
            {
                //trượt vào
                CommonFunction.SwipeByPercent(DeviceID, ScreenResolution, 10, 50, 40, 5);

                //đấm 3 phát
                CommonFunction.TapByPercent(DeviceID, ScreenResolution, 90, 50);
                CommonFunction.TapByPercent(DeviceID, ScreenResolution, 91, 51);
                CommonFunction.TapByPercent(DeviceID, ScreenResolution, 89, 49);
                CommonFunction.TapByPercent(DeviceID, ScreenResolution, 90, 50);
                CommonFunction.TapByPercent(DeviceID, ScreenResolution, 91, 51);

                //trượt ra
                CommonFunction.SwipeByPercent(DeviceID, ScreenResolution, 40, 50, 10, 5);

                count--;
                if (count == 0)
                {
                    var pausePng = (Bitmap)Bitmap.FromFile("images//pause-on-fighting.png");
                    var phonescreen = CommonFunction.ScreenShoot(DeviceID);
                    var pausePoint = ImageScanOpenCV.FindOutPoint(phonescreen, pausePng);

                    //Nếu không còn thấy màn hình Pause nữa thì dừng đấm đá
                    if (!pausePoint.HasValue)
                    {
                        keepFighting = false;
                    }
                    else
                    {
                        count = NumberOfCombo;
                    }
                }
            }

        }

        /// <summary>
        /// Hoạt động trong chuỗi đấm đá
        /// </summary>
        void OnArenaAction()
        {
            Task a = new Task(() =>
            {
                if (gameMode == GameMode.FIGHT)
                {
                    int numberOfGame = 100;
                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        numberOfGame = numChangeArea.Value.HasValue ? numChangeArea.Value.Value : 100;
                    });

                    //Bắt đầu bước vào fight
                    while (countGame < numberOfGame)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                        {
                            txtLog.Text += String.Format("{1} {2} {4} {3}{0}", Environment.NewLine, DateTime.Now.ToLongTimeString(), "Game:", countGame.ToString(), currentArena.ToString());
                        });

                        //TODO: Check sao tự nhiên bị màn hình khóa từ                        
                        //QUICK FILL
                        if (!IsAutoRunning()) return;
                        Delay(2);
                        var phonescreen = CommonFunction.ScreenShoot(DeviceID);
                        var imageToCheck = (Bitmap)Bitmap.FromFile("images//quick-fill-hero.png");
                        var pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);

                        while (!pointToCheck.HasValue)
                        {
                            if (!IsAutoRunning()) return;
                            Delay(1);
                            phonescreen = CommonFunction.ScreenShoot(DeviceID);
                            pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
                        }
                        //Click vào point to check
                        CommonFunction.Tap(DeviceID, pointToCheck.Value.X + 100, pointToCheck.Value.Y + 100);

                        //FIND MATCH
                        if (!IsAutoRunning()) return;
                        Delay(2);
                        phonescreen = CommonFunction.ScreenShoot(DeviceID);
                        imageToCheck = (Bitmap)Bitmap.FromFile("images//find-match.png");
                        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);

                        while (!pointToCheck.HasValue)
                        {
                            if (!IsAutoRunning()) return;
                            Delay(1);
                            phonescreen = CommonFunction.ScreenShoot(DeviceID);
                            pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
                        }
                        CommonFunction.TapByPercent(DeviceID, ScreenResolution, 8, 90);

                        //CONTINUE
                        if (!IsAutoRunning()) return;
                        Delay(2);
                        phonescreen = CommonFunction.ScreenShoot(DeviceID);
                        imageToCheck = (Bitmap)Bitmap.FromFile("images//continue-button.png");
                        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);

                        while (!pointToCheck.HasValue)
                        {
                            if (!IsAutoRunning()) return;
                            Delay(1);
                            phonescreen = CommonFunction.ScreenShoot(DeviceID);
                            pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
                        }
                        CommonFunction.Tap(DeviceID, pointToCheck.Value.X + 100, pointToCheck.Value.Y + 100);

                        //ACCEPT
                        if (!IsAutoRunning()) return;
                        Delay(2);
                        phonescreen = CommonFunction.ScreenShoot(DeviceID);
                        imageToCheck = (Bitmap)Bitmap.FromFile("images//accept-button.png");
                        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);

                        while (!pointToCheck.HasValue)
                        {
                            if (!IsAutoRunning()) return;
                            Delay(1);
                            phonescreen = CommonFunction.ScreenShoot(DeviceID);
                            pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
                        }
                        CommonFunction.Tap(DeviceID, pointToCheck.Value.X + 100, pointToCheck.Value.Y + 100);
                        //CommonFunction.TapByPercent(DeviceID, ScreenResolution, 90, 93);

                        //CONTINUE
                        if (!IsAutoRunning()) return;
                        Delay(2);
                        phonescreen = CommonFunction.ScreenShoot(DeviceID);
                        imageToCheck = (Bitmap)Bitmap.FromFile("images//continue-button.png");
                        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);

                        while (!pointToCheck.HasValue)
                        {
                            if (!IsAutoRunning()) return;
                            Delay(1);
                            phonescreen = CommonFunction.ScreenShoot(DeviceID);
                            pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
                        }
                        CommonFunction.Tap(DeviceID, pointToCheck.Value.X + 100, pointToCheck.Value.Y + 100);
                        //CommonFunction.TapByPercent(DeviceID, ScreenResolution, 90, 93);

                        //Đấm lần 1
                        OnFightAction();

                        //NEXT FIGHT
                        //Tìm và bấm vào next fight
                        phonescreen = CommonFunction.ScreenShoot(DeviceID);
                        imageToCheck = (Bitmap)Bitmap.FromFile("images//next-fight-button.png");
                        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);

                        while (!pointToCheck.HasValue)
                        {
                            if (!IsAutoRunning()) return;
                            Delay(1);
                            phonescreen = CommonFunction.ScreenShoot(DeviceID);
                            pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
                        }
                        //Click vào point to check
                        CommonFunction.Tap(DeviceID, pointToCheck.Value.X + 100, pointToCheck.Value.Y + 100);

                        //Đấm lần 2
                        OnFightAction();

                        //FINAL FIGHT
                        phonescreen = CommonFunction.ScreenShoot(DeviceID);
                        imageToCheck = (Bitmap)Bitmap.FromFile("images//final-fight-button.png");
                        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);

                        while (!pointToCheck.HasValue)
                        {
                            if (!IsAutoRunning()) return;
                            Delay(1);
                            phonescreen = CommonFunction.ScreenShoot(DeviceID);
                            pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
                        }
                        //Click vào point to check
                        CommonFunction.Tap(DeviceID, pointToCheck.Value.X + 100, pointToCheck.Value.Y + 100);

                        //Đấm lần 3
                        OnFightAction();

                        //TAP ANYWHERE TO CONTINUE
                        Delay(5);
                        CommonFunction.TapByPercent(DeviceID, ScreenResolution, 68, 90);

                        //Tìm vào nút NEXT SERIES để bấm
                        Delay(10);
                        phonescreen = CommonFunction.ScreenShoot(DeviceID);
                        imageToCheck = (Bitmap)Bitmap.FromFile("images//next-series-button.png");
                        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);

                        while (!pointToCheck.HasValue)
                        {
                            if (!IsAutoRunning()) return;
                            Delay(1);
                            phonescreen = CommonFunction.ScreenShoot(DeviceID);
                            pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
                            //Tap bừa 1 phát vì có thể đang struck ở màn trước (Tap anywhere to continue)
                            CommonFunction.TapByPercent(DeviceID, ScreenResolution, 68, 90);
                        }
                        //Click vào point to check
                        CommonFunction.Tap(DeviceID, pointToCheck.Value.X, pointToCheck.Value.Y);

                        //VÒNG LẠI TỪ QUICK FILL
                        countGame++;
                    }

                    //Chuyển lại mode Chọn Arena
                    countGame = 0;
                    BackToChangeArena();
                    return;
                }
            });
            a.Start();
        }

        /// <summary>
        /// Quay lại màn hình chọn arena đã
        /// </summary>
        /// NQMinh 13.11.2022
        void BackToChangeArena()
        {
            //Nhấn nút back           

            //Click vào point to check
            if (!IsAutoRunning()) return;
            Delay(3);
            CommonFunction.TapByPercent(DeviceID, ScreenResolution, 6, 4);

            gameMode = GameMode.CHOOSEARENA;
            AutoRunningGame();
        }
        #endregion
    }
}
