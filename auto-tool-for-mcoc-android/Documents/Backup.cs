using KAutoHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace AndroidAutoMCOC.Documents
{
    /// <summary>
    /// Interaction logic for AutoConfig.xaml
    /// </summary>
    public partial class AutoConfig1 : Window
    {
    //    #region Declaration

    //    string gameAppName = "com.kabam.marvelbattle";

    //    //Cách tìm:
    //    //adb shell dumpsys package > dumpsys.txt
    //    //Sau đó vào file dumpsys.txt để tìm với AppName tương ứng
    //    string gameStartActivity = "com.explodingbarrel.Activity";

    //    bool isRunning = false;

    //    List<ArenaCode> RunnedArena = new List<ArenaCode>();

    //    DispatcherTimer runningTimer = new DispatcherTimer();
    //    #endregion

    //    #region Property   

    //    public string DeviceID { get; set; }

    //    public int ArenaLocation { get; set; }
    //    #endregion

    //    #region Constructor

    //    public AutoConfig(string deviceID, int arenaLocation)
    //    {
    //        InitializeComponent();
    //        DeviceID = deviceID;
    //        ArenaLocation = arenaLocation;
    //        runningTimer.Interval = new TimeSpan(0, 0, 1);
    //    }
    //    #endregion

    //    #region Event

    //    /// <summary>
    //    /// Bắt đầu chạy
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void btnStart_Click(object sender, RoutedEventArgs e)
    //    {
    //        //Validate trước khi chạy
    //        if (!chkSixStarBasic.IsChecked.Value && !chkSixStarFeatured.IsChecked.Value && !chkTrail.IsChecked.Value)
    //        {
    //            MessageBox.Show("Chưa chọn loại Arena để đánh (Tích vào các checkbox).");
    //            return;
    //        }

    //        int iChangeArea;
    //        //Nếu điền Change Arena minute thì cần điền đúng định dạng
    //        if (!string.IsNullOrWhiteSpace(numChangeArea.Text) && !int.TryParse(numChangeArea.Text, out iChangeArea))
    //        {
    //            MessageBox.Show("Thông tin Change area không đúng định dạng số. Bỏ trống hoặc set = 0 để không change");
    //            return;
    //        }

    //        //Gán cờ bắt đầu auto
    //        isRunning = true;

    //        SetFormTitle();

    //        //Đang chạy thì đóng lại chạy lại từ đầu
    //        CommonFunction.ExecuteCMD(string.Format("adb shell am force-stop {0}", gameAppName));

    //        //Mở lại game
    //        CommonFunction.ExecuteCMD(string.Format("adb shell am start -n {0}/{1}", gameAppName, gameStartActivity));

    //        AutoRunningGame();
    //    }

    //    private void btnTest_Click(object sender, RoutedEventArgs e)
    //    {
    //        AutoRunningGame();
    //    }

    //    /// <summary>
    //    /// Dừng mọi hoạt động
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private void btnStop_Click(object sender, RoutedEventArgs e)
    //    {
    //        //Gán cờ dừng auto
    //        isRunning = false;

    //        //Set lại title form
    //        SetFormTitle();
    //    }
    //    #endregion

    //    #region Method

    //    void AutoRunningGame()
    //    {
    //        Task a = new Task(() =>
    //        {
    //            //Gán cờ bắt đầu auto
    //            isRunning = true;

    //            runningTimer.Start();

    //            while (IsAutoRunning())
    //            {
    //                Task t = new Task(() =>
    //                {
    //                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
    //                    {
    //                        txtLog.Text += String.Format("{0}{1} {2}", Environment.NewLine, DateTime.Now.ToLongTimeString(), "Một thread mới được chạy, detect screen và action tương ứng với màn hình");
    //                    });
    //                    DetectScreenAndAction();
    //                });
    //                t.Start();

    //                //Cứ 1s thì tiếp tục vòng white
    //                Thread.Sleep(1000);
    //            }
    //        });

    //        a.Start();
    //    }

    //    /// <summary>
    //    /// Set lại form title
    //    /// Do có cả việc set trong thread khác nên cứ đặt như này cho dễ
    //    /// </summary>        
    //    void SetFormTitle()
    //    {
    //        string title;
    //        if (IsAutoRunning())
    //        {
    //            title = string.Format("{0} on {1}", DeviceID, "Running");
    //        }
    //        else
    //        {
    //            title = string.Format("{0} is {1}", DeviceID, "Stopped");
    //        }
    //        System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate { Title = title; });
    //    }

    //    /// <summary>
    //    /// Phán đoán màn hình và action tương ứng với màn hình
    //    /// </summary>
    //    void DetectScreenAndAction()
    //    {
    //        if (!IsAutoRunning()) return;

    //        switch (DetectScreen())
    //        {
    //            case ScreenCode.ON_FIGHT:
    //                OnFightAction();
    //                break;

    //            case ScreenCode.TODAY_REWARD:
    //                TodayRewardAction();
    //                break;

    //            case ScreenCode.HOME:
    //                HomeAction();
    //                break;

    //            case ScreenCode.HOME_WITHOUT_FIGHT:
    //                break;

    //            case ScreenCode.CHOSE_ARENA:
    //                ChooseArenaAction();
    //                break;

    //            case ScreenCode.QUICK_FILL:
    //                QuickFillAction();
    //                break;

    //            case ScreenCode.FIND_MATCH:
    //                FindMatchAction();
    //                break;

    //            case ScreenCode.FIGHT_PREVIEW_1:
    //                FightPreview1Action();
    //                break;

    //            case ScreenCode.SET_LINEUP:
    //                SetLineupAction();
    //                break;

    //            case ScreenCode.FIGHT_PREVIEW_2:
    //            case ScreenCode.FIGHT_PREVIEW_3:
    //                FightPreview2Action();
    //                break;

    //            case ScreenCode.FINISH_FIGHT_1:
    //                FinishFight1Action();
    //                break;

    //            case ScreenCode.FIGHT_REWARD:
    //                FightRewardAction();
    //                break;

    //            case ScreenCode.UNDEFINED:
    //                //Vào màn hình không rõ thì làm gì?
    //                //Đóng game chạy lại chăng?
    //                break;
    //        }
    //    }

    //    /// <summary>
    //    /// Hành động khi gặp màn hình today reward
    //    /// </summary>
    //    void TodayRewardAction()
    //    {
    //        //Đóng today reward
    //        if (!IsAutoRunning()) return;
    //        CommonFunction.TapByPercent(DeviceID, 50, 90);
    //    }

    //    /// <summary>
    //    /// Hành động khi gặp màn hình home
    //    /// Nhấn vào Fight và chọn Multiverse arena
    //    /// </summary>
    //    void HomeAction()
    //    {
    //        //Nhấn vào Fight
    //        if (!IsAutoRunning()) return;
    //        Delay(1);

    //        var fightImage = (Bitmap)Bitmap.FromFile("images//fight-icon.png");
    //        var phonescreen = CommonFunction.ScreenShoot(DeviceID);
    //        var fightPoint = ImageScanOpenCV.FindOutPoint(phonescreen, fightImage);

    //        if (fightPoint.HasValue)
    //        {
    //            CommonFunction.Tap(DeviceID, fightPoint.Value.X + 50, fightPoint.Value.Y + 50);
    //        }

    //        //Do màn hình Fight cần vào chọn Multiverse arena nhưng không có action nào ở đó nên code luôn đoạn chọn vào step này
    //        if (!IsAutoRunning()) return;
    //        Delay(2);
    //        //Lướt sang phải 3 lần để đảm bảo đến cuối
    //        if (!IsAutoRunning()) return;
    //        for (int i = 0; i < 3; i++)
    //        {
    //            CommonFunction.SwipeByPercent(DeviceID, 60, 50, 20, 50, 200);
    //            Delay(1);
    //        }

    //        //Chọn vào Multiverse Arena theo config từ màn hình chính
    //        if (!IsAutoRunning()) return;
    //        Delay(1);
    //        double x = 0, y = 0;

    //        switch (ArenaLocation)
    //        {
    //            //Row 1 column 1
    //            case 0:
    //                x = 90;
    //                y = 35;
    //                break;
    //            //Row 1 column 2
    //            case 1:
    //                x = 90;
    //                y = 75;
    //                break;
    //            //Row 2 column 1
    //            case 2:
    //                x = 75;
    //                y = 35;
    //                break;
    //            //Row 2 column 2
    //            case 3:
    //                x = 75;
    //                y = 75;
    //                break;
    //            //Row 3 column 1
    //            case 4:
    //                x = 50;
    //                y = 35;
    //                break;
    //            //Row 3 column 2
    //            case 5:
    //                x = 60;
    //                y = 70;
    //                break;
    //            //Row 4 column 1
    //            case 6:
    //                x = 45;
    //                y = 35;
    //                break;
    //            //Row 4 column 2
    //            case 7:
    //                x = 45;
    //                y = 70;
    //                break;
    //        }
    //        if (x != 0 && y != 0)
    //        {
    //            //Nhấn vào Arena                
    //            CommonFunction.TapByPercent(DeviceID, x, y);
    //        }
    //    }

    //    /// <summary>
    //    /// Màn hình chọn arena để chiến
    //    /// //Chọn đúng loại arena được config theo thứ tự từ trái sang phải
    //    /// </summary>
    //    void ChooseArenaAction()
    //    {
    //        if (!IsAutoRunning()) return;
    //        Delay(1);
    //        Bitmap imageToCheck = null;

    //        //Nếu check vào 6Star basic và chưa được chạy
    //        bool basicChecked = false, featuredChecked = false, trailChecked = false;
    //        System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
    //        {
    //            basicChecked = chkSixStarBasic.IsChecked.Value;
    //            featuredChecked = chkSixStarFeatured.IsChecked.Value;
    //            trailChecked = chkTrail.IsChecked.Value;
    //        });

    //        if (basicChecked && !RunnedArena.Contains(ArenaCode.SIX_BASIC))
    //        {
    //            RunnedArena.Add(ArenaCode.SIX_BASIC);

    //            //Tìm vị trí thằng six basic rồi bấm                
    //            imageToCheck = (Bitmap)Bitmap.FromFile("images//6-star-basic.png");
    //        }
    //        else if (featuredChecked && !RunnedArena.Contains(ArenaCode.SIX_FEATURED))
    //        {
    //            RunnedArena.Add(ArenaCode.SIX_FEATURED);

    //            //Tìm vị trí thằng six featured rồi bấm                
    //            imageToCheck = (Bitmap)Bitmap.FromFile("images//6-star-featured.png");
    //        }
    //        else if (trailChecked && !RunnedArena.Contains(ArenaCode.SUMMONER_TRIAL))
    //        {
    //            RunnedArena.Add(ArenaCode.SUMMONER_TRIAL);

    //            //Tìm vị trí thằng trails rồi bấm                
    //            imageToCheck = (Bitmap)Bitmap.FromFile("images//common-trial.png");
    //        }
    //        else
    //        {
    //            //Chạy đủ 1 vòng từ trái qua phải thì clear danh sách đã chạy quay lại chạy từ đầu
    //            RunnedArena.Clear();
    //            ChooseArenaAction();
    //            return;
    //        }

    //        //Chụp ảnh màn hình hiện tại
    //        var phonescreen = CommonFunction.ScreenShoot(DeviceID);
    //        var pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);

    //        while (!pointToCheck.HasValue)
    //        {
    //            if (!IsAutoRunning()) return;
    //            Delay(1);
    //            //Chưa tìm thấy thằng six basic thì nó nằm ở trang 2
    //            //Slide nhẹ sang rồi bấm vào
    //            CommonFunction.SwipeByPercent(DeviceID, 85, 50, 50, 50, 1000);
    //            phonescreen = CommonFunction.ScreenShoot(DeviceID);
    //            pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        }

    //        //X giữ nguyên, Y vào 50%
    //        if (pointToCheck.HasValue)
    //        {
    //            if (!IsAutoRunning()) return;
    //            Delay(1);
    //            var resolution = CommonFunction.GetScreenResolution(DeviceID);
    //            CommonFunction.Tap(DeviceID, pointToCheck.Value.X, pointToCheck.Value.Y + (resolution.Y * 50 / 100));
    //        }
    //    }

    //    /// <summary>
    //    /// Màn hình quick fill hero trước khi vào trận
    //    /// </summary>
    //    void QuickFillAction()
    //    {
    //        if (!IsAutoRunning()) return;
    //        Delay(2);
    //        CommonFunction.TapByPercent(DeviceID, 8, 90);
    //    }

    //    /// <summary>
    //    /// Màn hình find match
    //    /// </summary>
    //    void FindMatchAction()
    //    {
    //        if (!IsAutoRunning()) return;
    //        Delay(2);
    //        CommonFunction.TapByPercent(DeviceID, 8, 90);
    //    }

    //    void FightPreview1Action()
    //    {
    //        if (!IsAutoRunning()) return;
    //        Delay(2);
    //        CommonFunction.TapByPercent(DeviceID, 90, 90);
    //    }

    //    void SetLineupAction()
    //    {
    //        if (!IsAutoRunning()) return;
    //        Delay(2);
    //        CommonFunction.TapByPercent(DeviceID, 90, 90);
    //    }

    //    void FightPreview2Action()
    //    {
    //        if (!IsAutoRunning()) return;
    //        Delay(2);
    //        CommonFunction.TapByPercent(DeviceID, 90, 90);
    //    }

    //    void OnFightAction()
    //    {
    //        if (!IsAutoRunning()) return;
    //        //Thằng này ko có delay vì cần đấm nhanh
    //        bool keepFighting = true;
    //        int count = 20;

    //        while (count > 0 && keepFighting)
    //        {
    //            //trượt vào
    //            CommonFunction.SwipeByPercent(DeviceID, 10, 50, 40, 50);

    //            //đấm 3 phát
    //            CommonFunction.TapByPercent(DeviceID, 90, 50);
    //            CommonFunction.TapByPercent(DeviceID, 91, 51);
    //            CommonFunction.TapByPercent(DeviceID, 89, 49);

    //            //trượt ra
    //            CommonFunction.SwipeByPercent(DeviceID, 40, 50, 10, 50);

    //            count--;
    //            if (count == 0)
    //            {
    //                var pausePng = (Bitmap)Bitmap.FromFile("images//pause-on-fighting.png");
    //                var phonescreen = CommonFunction.ScreenShoot(DeviceID);
    //                var pausePoint = ImageScanOpenCV.FindOutPoint(phonescreen, pausePng);

    //                //Nếu không còn thấy màn hình Pause nữa thì dừng đấm đá
    //                if (!pausePoint.HasValue)
    //                {
    //                    keepFighting = false;
    //                }
    //                else
    //                {
    //                    count = 20;
    //                }
    //            }
    //        }

    //    }

    //    void FinishFight1Action()
    //    {
    //        if (!IsAutoRunning()) return;
    //        Delay(2);
    //        //Thua thì ở vị trí này
    //        CommonFunction.TapByPercent(DeviceID, 60, 60);
    //        //Thắng thì ở vị trí này
    //        CommonFunction.TapByPercent(DeviceID, 60, 80);
    //    }

    //    void FightRewardAction()
    //    {
    //        if (!IsAutoRunning()) return;
    //        var imageToCheck = (Bitmap)Bitmap.FromFile("images//next-series-button.png");
    //        var phonescreen = CommonFunction.ScreenShoot(DeviceID);
    //        var pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            CommonFunction.Tap(DeviceID, pointToCheck.Value.X, pointToCheck.Value.Y);
    //        }

    //    }

    //    /// <summary>
    //    /// Detect ra screen để làm việc
    //    /// </summary>
    //    /// <returns></returns>
    //    ScreenCode DetectScreen()
    //    {
    //        var returnValue = ScreenCode.UNDEFINED;

    //        //Chụp ảnh màn hình hiện tại
    //        var phonescreen = CommonFunction.ScreenShoot(DeviceID);

    //        //Màn hình on fight là màn hình cần nhiều action liên tục nhất nên cứ để trên đầu
    //        var imageToCheck = (Bitmap)Bitmap.FromFile("images//pause-on-fighting.png");
    //        var pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.ON_FIGHT;
    //            return returnValue;
    //        }

    //        //Check có phải màn hình Today reward
    //        imageToCheck = (Bitmap)Bitmap.FromFile("images//today-view-calendar.png");
    //        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.TODAY_REWARD;
    //            return returnValue;
    //        }

    //        //Check có phải màn hình Home
    //        var settingButton = (Bitmap)Bitmap.FromFile("images//home-setting.png");
    //        var whatnewbutton = (Bitmap)Bitmap.FromFile("images//home-button-what-new.png");
    //        var actionButton = (Bitmap)Bitmap.FromFile("images//home-action.png");
    //        var settingPoint = ImageScanOpenCV.FindOutPoint(phonescreen, settingButton);
    //        var whatnewPoint = ImageScanOpenCV.FindOutPoint(phonescreen, whatnewbutton);
    //        var actionPoint = ImageScanOpenCV.FindOutPoint(phonescreen, actionButton);

    //        if (settingPoint.HasValue && whatnewPoint.HasValue && actionPoint.HasValue)
    //        {
    //            //Kiểm tra nút Fight có nằm ở đó không
    //            var fightImage = (Bitmap)Bitmap.FromFile("images//fight-icon.png");
    //            var fightPoint = ImageScanOpenCV.FindOutPoint(phonescreen, fightImage);
    //            if (fightPoint.HasValue)
    //            {
    //                returnValue = ScreenCode.HOME;
    //                return returnValue;
    //            }
    //            else
    //            {
    //                returnValue = ScreenCode.HOME_WITHOUT_FIGHT;
    //                return returnValue;
    //            }
    //        }

    //        //Check có phải màn hình Multiverse arena
    //        imageToCheck = (Bitmap)Bitmap.FromFile("images//multiverse-arena-button.png");
    //        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.CHOSE_ARENA;
    //            return returnValue;
    //        }

    //        //Có phải màn hình hero quick fill            
    //        imageToCheck = (Bitmap)Bitmap.FromFile("images//quick-fill-hero.png");
    //        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.QUICK_FILL;
    //            return returnValue;
    //        }

    //        //Có phải màn hình find match            
    //        imageToCheck = (Bitmap)Bitmap.FromFile("images//find-match.png");
    //        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.FIND_MATCH;
    //            return returnValue;
    //        }

    //        //FIGHT_PREVIEW_1
    //        imageToCheck = (Bitmap)Bitmap.FromFile("images//find-new-match-and-continue-button.png");
    //        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.FIGHT_PREVIEW_1;
    //            return returnValue;
    //        }

    //        //SET_LINEUP
    //        imageToCheck = (Bitmap)Bitmap.FromFile("images//set-lineup-accept-button.png");
    //        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.SET_LINEUP;
    //            return returnValue;
    //        }

    //        //FIGHT_PREVIEW_2
    //        imageToCheck = (Bitmap)Bitmap.FromFile("images//next-fight-screen.png");
    //        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.FIGHT_PREVIEW_2;
    //            return returnValue;
    //        }

    //        //FINISH_FIGHT_1
    //        imageToCheck = (Bitmap)Bitmap.FromFile("images//finish-fight-1.png");
    //        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.FINISH_FIGHT_1;
    //            return returnValue;
    //        }

    //        //FIGHT_PREVIEW_3
    //        imageToCheck = (Bitmap)Bitmap.FromFile("images//final-fight-scren.png");
    //        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.FIGHT_PREVIEW_3;
    //            return returnValue;
    //        }

    //        //FIGHT_REWARD
    //        imageToCheck = (Bitmap)Bitmap.FromFile("images//next-series-button.png");
    //        pointToCheck = ImageScanOpenCV.FindOutPoint(phonescreen, imageToCheck);
    //        if (pointToCheck.HasValue)
    //        {
    //            returnValue = ScreenCode.FIGHT_REWARD;
    //            return returnValue;
    //        }

    //        return returnValue;
    //    }

    //    /// <summary>
    //    /// Hàm kiểm tra xem có nên chạy tiếp hay không
    //    /// </summary>
    //    /// <returns></returns>
    //    bool IsAutoRunning()
    //    {
    //        return isRunning;
    //    }

    //    void Delay(int secToDelay)
    //    {

    //    }
    //    #endregion       
    }
}
