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
            Task.Run(async () =>
            {
                await Task.Delay(3000);

                // MsixStartupSetter가 이미 폴백 로직을 처리하므로 하나만 체크
                bool startupEnabled = await MsixStartupSetter.IsStartupEnabledAsync();

                // 설정되어 있지 않은 경우에만 권장 메시지 표시
                if (!startupEnabled)
                {
                    // WPF Dispatcher 사용
                    System.Windows.Application.Current.Dispatcher.Invoke(() => StartupSuggestBalloonShow());
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
            // 자동 -> 라이트 -> 다크 -> 자동 순서로 토글
            switch (Settings.Default.theme)
            {
                case "auto":
                    Settings.Default.theme = "light";
                    break;
                case "light":
                    Settings.Default.theme = "dark";
                    break;
                case "dark":
                    Settings.Default.theme = "auto";
                    break;
                default:
                    Settings.Default.theme = "auto";
                    break;
            }
            
            mainWindow.loadTheme();
            Settings.Default.Save();
        }

        private void changeTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 자동 -> 흰색 -> 검은색 -> 자동 순서로 토글
            switch (Settings.Default.traytheme)
            {
                case "auto":
                    Settings.Default.traytheme = "white";
                    break;
                case "white":
                    Settings.Default.traytheme = "black";
                    break;
                case "black":
                    Settings.Default.traytheme = "auto";
                    break;
                default:
                    Settings.Default.traytheme = "auto";
                    break;
            }
            
            mainWindow.UpdateIcon(mainWindow.percentage);
            Settings.Default.Save();
        }

        private void showBattInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // BatteryInfoWindow (WPF) 실행

            // 이미 실행 중이라면
            var existingWindow = System.Windows.Application.Current.Windows.OfType<BatteryInfoWindow>().FirstOrDefault();
            if (existingWindow != null)
            {
                existingWindow.Activate();
                existingWindow.Focus();
                return;
            }
            
            BatteryInfoWindow batteryInfoWindow = new BatteryInfoWindow();
            batteryInfoWindow.Show();
        }

        public void StartupSuggestBalloonShow()
        {
            trayIcon.BalloonTipTitle = "시작 프로그램 설정 안됨";
            trayIcon.BalloonTipText = "여기를 눌러 시작프로그램으로 설정하세요.";
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;

            // balloontip 클릭시 - 비동기로 처리
            trayIcon.BalloonTipClicked += async (sender, e) =>
            {
                bool success = await MsixStartupSetter.SetStartupAsync(true);
                if (success)
                {
                    trayIcon.ShowBalloonTip(2000, "설정 완료", "시작 프로그램으로 설정되었습니다.", ToolTipIcon.Info);
                }
                else
                {
                    trayIcon.ShowBalloonTip(2000, "설정 실패", "시작 프로그램 설정에 실패했습니다.", ToolTipIcon.Error);
                }
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
            // 음소거 메뉴 텍스트 업데이트
            muteToolStripMenuItem.Text = "음소거 " + (Settings.Default.mute ? "해제" : "설정");
            
            // 팝업 색상 메뉴 텍스트 업데이트
            string popupThemeText = Settings.Default.theme switch
            {
                "auto" => "팝업 색: 자동",
                "light" => "팝업 색: 라이트",
                "dark" => "팝업 색: 다크",
                _ => "팝업 색: 자동"
            };
            changeThemeToolStripMenuItem.Text = popupThemeText;
            
            // 아이콘 색상 메뉴 텍스트 업데이트
            string iconThemeText = Settings.Default.traytheme switch
            {
                "auto" => "아이콘 색: 자동",
                "white" => "아이콘 색: 라이트",
                "black" => "아이콘 색: 다크",
                _ => "아이콘 색: 자동"
            };
            changeTrayToolStripMenuItem.Text = iconThemeText;
        }
    }
}
