using AndroidAutoMCOC.Model;
using KAutoHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AndroidAutoMCOC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Declaration       

        public Dictionary<string, AutoConfig> DicRunningDevice { get; set; } = new Dictionary<string, AutoConfig>();
        string configPath = "Config/main-config.json";
        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            InitFormData();

            LoadLastConfig();
        }
        #endregion

        #region Event

        /// <summary>
        /// Sự kiện khi bấm nút Refresh list device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefreshList_Click(object sender, RoutedEventArgs e)
        {
            LoadDevice();
        }

        /// <summary>
        /// Sự kiện khi nhấn nút Slect device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectDevice_Click(object sender, RoutedEventArgs e)
        {
            var objSelectedDevice = cboDevices.SelectedValue;
            if (objSelectedDevice != null)
            {
                var selDeviceID = objSelectedDevice.ToString();
                AutoConfig frmAutoConfig;

                if (DicRunningDevice.ContainsKey(selDeviceID))
                {
                    frmAutoConfig = DicRunningDevice[selDeviceID];
                    frmAutoConfig.Focus();
                }
                else
                {
                    frmAutoConfig = new AutoConfig(selDeviceID, cboArenaLocation.SelectedIndex, numNumberOfCombo.Value.Value);
                    frmAutoConfig.Title = selDeviceID;
                    frmAutoConfig.Closed += FrmAutoConfig_Closed;

                    DicRunningDevice.Add(selDeviceID, frmAutoConfig);
                    DisplayRunningDevice();
                    frmAutoConfig.Show();
                }
            }
            else
            {
                MessageBox.Show("Chưa chọn device nào");
            }
        }

        private void FrmAutoConfig_Closed(object sender, EventArgs e)
        {
            var frmAutoConfig = (AutoConfig)sender;
            frmAutoConfig.Closed -= FrmAutoConfig_Closed;
            DicRunningDevice.Remove(frmAutoConfig.DeviceID);
            DisplayRunningDevice();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveLastConfig();
        }
        #endregion

        #region Method

        /// <summary>
        /// Khởi tạo các giá trị mặc định cho form
        /// </summary>
        void InitFormData()
        {
            LoadDevice();
            DisplayRunningDevice();
        }

        /// <summary>
        /// Load config từ json và bind vào các control trên form
        /// </summary>
        /// NQMinh 13.11.2022
        void LoadLastConfig()
        {
            if (File.Exists(configPath))
            {
                string mainconfig = File.ReadAllText(configPath);
                var result = JsonConvert.DeserializeObject<MainConfig>(mainconfig);

                //Gán giá trị cho combo Arena location theo lần chọn gần nhất
                cboArenaLocation.SelectedIndex = result.ArenaLocationIndex;
                numNumberOfCombo.Value = result.NumberOfCombo;
            }
            else
            {
                //Gán default value cho control
                cboArenaLocation.SelectedIndex = 1;
                numNumberOfCombo.Value = 3;
            }
        }

        /// <summary>
        /// Load danh sách các thiết bị được kết nối
        /// </summary>
        void LoadDevice()
        {
            var lstDevices = CommonFunction.GetDevices();
            cboDevices.ItemsSource = lstDevices;
            cboDevices.SelectedIndex = 0;
        }

        void DisplayRunningDevice()
        {
            if (DicRunningDevice.Count > 0)
            {
                txtRunningDevices.Text = string.Join(Environment.NewLine, DicRunningDevice.Keys);
            }
            else
            {
                txtRunningDevices.Text = "There is no running device yet.";
            }
        }

        /// <summary>
        /// Save config từ vào json
        /// </summary>
        /// NQMinh 13.11.2022
        void SaveLastConfig()
        {
            var oConfig = new MainConfig();
            oConfig.ArenaLocationIndex = cboArenaLocation.SelectedIndex;
            oConfig.NumberOfCombo = numNumberOfCombo.Value.HasValue ? numNumberOfCombo.Value.Value : 3;

            var strConfig = JsonConvert.SerializeObject(oConfig);

            if (!Directory.Exists("Config"))
            {
                Directory.CreateDirectory("Config");
            }
            File.WriteAllText(configPath, strConfig);
        }
        #endregion

    }
}
