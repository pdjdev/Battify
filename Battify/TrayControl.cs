using System.Reflection;

namespace Battify
{
    public partial class TrayControl : UserControl
    {
        public MainWindow mainWindow;
        private bool balloonHandlerRegistered = false;

        public TrayControl(MainWindow w)
        {
            InitializeComponent();
            mainWindow = w;

            // 커스텀 렌더러 적용
            contextMenuStrip1.Renderer = new DarkMenuRenderer();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version != null)
            {
                // Major.Minor.Build 형식으로 (Revision 제외)
                infoItemToolStripMenuItem.Text = $"Battify ({version.Major}.{version.Minor}.{version.Build})";
            }
            else
            {
                infoItemToolStripMenuItem.Text = "Battify";
            }

            // BalloonTipClicked 이벤트를 한 번만 등록
            trayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;
            balloonHandlerRegistered = true;

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

        private bool isProcessingBalloonClick = false;

        private async void TrayIcon_BalloonTipClicked(object? sender, EventArgs e)
        {
            // 이미 처리 중이면 무시 (중복 실행 방지)
            if (isProcessingBalloonClick) return;

            isProcessingBalloonClick = true;

            try
            {
                // 먼저 현재 상태 확인 - 이미 활성화되어 있으면 다시 설정하지 않음
                bool alreadyEnabled = await MsixStartupSetter.IsStartupEnabledAsync();

                if (alreadyEnabled)
                {
                    // 이미 활성화되어 있으면 메시지만 표시
                    // trayIcon.ShowBalloonTip(2000, "이미 설정됨", "시작 프로그램이 이미 설정되어 있습니다.", ToolTipIcon.Info);
                    return;
                }

                bool success = await MsixStartupSetter.SetStartupAsync(true);
                if (success)
                {
                    trayIcon.ShowBalloonTip(2000, "설정 완료", "시작 프로그램으로 설정되었습니다.", ToolTipIcon.Info);
                }
                else
                {
                    trayIcon.ShowBalloonTip(2000, "설정 실패", "시작 프로그램 설정에 실패했습니다.", ToolTipIcon.Error);
                }
            }
            finally
            {
                isProcessingBalloonClick = false;
            }
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

            trayIcon.ShowBalloonTip(3000);
        }

        private void muteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.mute = !Settings.Default.mute;
            Settings.Default.Save();
        }

        private void togglePopupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.nopopup = !Settings.Default.nopopup;
            Settings.Default.Save();
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 알림음 메뉴 텍스트 업데이트
            muteToolStripMenuItem.Text = Settings.Default.mute ? "알림음 켜기" : "알림음 끄기";

            // 팝업 메뉴 텍스트 업데이트
            togglePopupToolStripMenuItem.Text = Settings.Default.nopopup ? "팝업 켜기" : "팝업 끄기";

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
