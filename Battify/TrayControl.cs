namespace Battify
{
    public partial class TrayControl : UserControl
    {
        public MainWindow mainWindow;

        public TrayControl(MainWindow w)
        {
            InitializeComponent();
            mainWindow = w;

            // 3초간 비동기 대기후 시작프로그램 체크
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                if (!StartupSetter.CheckStartup())
                {
                    StartupSuggestBalloonShow();
                }
            });
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Mainwindow의 ShowPopup 실행
            mainWindow.ShowPopup();
        }

        private void closeAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void changeThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.Default.theme == "dark")
            {
                Settings.Default.theme = "light";
            }
            else if (Settings.Default.theme == "light")
            {
                Settings.Default.theme = "dark";
            }
            mainWindow.loadTheme();
            Settings.Default.Save();
        }

        private void changeTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.Default.traytheme == "white")
            {
                Settings.Default.traytheme = "black";
            }
            else if (Settings.Default.traytheme == "black")
            {
                Settings.Default.traytheme = "white";
            }
            mainWindow.UpdateIcon(mainWindow.percentage);
            Settings.Default.Save();
        }

        private void showBattInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // BatteryInfoForm 실행

            // 이미 실행 중이라면
            if (Application.OpenForms.OfType<BatteryInfoForm>().Count() == 1)
            {
                Application.OpenForms.OfType<BatteryInfoForm>().First().BringToFront();
                return;
            }
            BatteryInfoForm batteryInfoForm = new BatteryInfoForm();
            batteryInfoForm.Show();
        }

        public void StartupSuggestBalloonShow()
        {
            trayIcon.BalloonTipTitle = "시작 프로그램 설정 안됨";
            trayIcon.BalloonTipText = "여기를 눌러 시작프로그램으로 설정하세요.";
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;


            // balloontip 클릭시
            trayIcon.BalloonTipClicked += (sender, e) =>
            {
                StartupSetter.SetStartup(true);
            };

            trayIcon.ShowBalloonTip(3000);
        }

        private void muteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.mute = !Settings.Default.mute;
            Settings.Default.Save();
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            muteToolStripMenuItem.Text = "음소거 " + (Settings.Default.mute ? "해제" : "설정");
        }
    }
}
