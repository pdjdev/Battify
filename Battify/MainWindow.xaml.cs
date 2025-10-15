using System.Diagnostics;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Battify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_POWER_STATUS
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public uint BatteryLifeTime;
            public uint BatteryFullLifeTime;
        }

        [DllImport("kernel32.dll")]
        public static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS systemPowerStatus);
        public int percentage = 80;
        public bool plugged = false;

        public bool isShown = false;

        // NotifyIcon
        /* public NotifyIcon battIcon = new NotifyIcon(); */

        // 알림음
        SoundPlayer snd = new SoundPlayer(DefAlertSound.plug);

        // ContextMenu 생성
        ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

        // 트레이 컨트롤
        TrayControl trayControl;

        public MainWindow()
        {
            System.Windows.Forms.Application.EnableVisualStyles();

            InitializeComponent();
            trayControl = new TrayControl(this);

            this.Hide();
            
            // 프로그램 시작 시 자동 테마 체크 및 적용
            CheckAndApplyAutoTheme();
            loadTheme();
            InitBattTimer();

            this.Loaded += async (s, e) =>
            {
                Debug.WriteLine("Battify WPF loaded");
                //PerformPopup();
            };

            
        }

        /// <summary>
        /// auto 설정일 때 현재 Windows 테마를 체크하고 적용합니다.
        /// </summary>
        public void CheckAndApplyAutoTheme()
        {
            // 팝업 테마가 auto인 경우에만 체크
            if (Settings.Default.theme == "auto")
            {
                // 현재 테마 다시 로드
                loadTheme();
            }
            
            // 트레이 아이콘 테마가 auto인 경우에만 체크
            if (Settings.Default.traytheme == "auto")
            {
                // 아이콘 업데이트
                UpdateIcon(percentage);
            }
        }

        private void InitBattTimer()
        {
            // DispatcherTimer 생성
            DispatcherTimer timer = new DispatcherTimer();

            // 타이머의 간격 설정 (예: 1초)
            timer.Interval = TimeSpan.FromSeconds(1);

            // 이전 배터리 상태 저장
            byte previousACLineStatus = 255;
            byte previousBatteryLife = 255;

            // 이벤트 핸들러 추가
            timer.Tick += (sender, e) =>
            {
                // 현재 배터리 상태 가져오기
                SYSTEM_POWER_STATUS status;
                if (GetSystemPowerStatus(out status))
                {
                    // 배터리 충전 상태가 변경되었는지 확인
                    if (status.ACLineStatus != previousACLineStatus)
                    {
                        Debug.WriteLine("Power status changed!");
                        previousACLineStatus = status.ACLineStatus;

                        if (status.ACLineStatus == 1)
                        {
                            // 충전 중
                            Debug.WriteLine("Charging");

                            // 충전음 재생 - 음소거 아닐 시에만!
                            if (Settings.Default.mute == false) snd.Play();

                            plugged = true;
                        }
                        else if (status.ACLineStatus == 0)
                        {
                            // 배터리 사용 중
                            Debug.WriteLine("Discharging");
                            plugged = false;
                        }

                        PerformPopup();
                        DrawBattery(percentage, plugged);
                    }

                    // 배터리 잔량이 변경되었는지 확인
                    if (status.BatteryLifePercent != previousBatteryLife)
                    {
                        Console.WriteLine("Battery life changed!");
                        previousBatteryLife = status.BatteryLifePercent;

                        percentage = status.BatteryLifePercent;
                        
                        // 퍼센트 변경 시 자동 테마 체크 (트레이 아이콘 갱신)
                        CheckAndApplyAutoTheme();
                        UpdateIcon(percentage);
                        // DrawBattery(percentage, plugged);
                        UpdateValue();
                    }
                }

                trayControl.trayIcon.Text = percentage.ToString() + "%";

                if (plugged)
                {
                    trayControl.trayIcon.Text += ", 충전중";
                }
                else
                {
                    trayControl.trayIcon.Text += " 남음";
                }
            };

            // 타이머 시작
            timer.Start();
        }

        // 팝업 표시
        public void ShowPopup()
        {
            if (isShown == true)
            {
                return;
            }
            isShown = true;

            // 팝업이 뜰 때 자동 테마 체크
            CheckAndApplyAutoTheme();

            const double margin = 10;
            var dpi = VisualTreeHelper.GetDpi(this);
            var left = SystemParameters.WorkArea.Width - this.Width - margin * dpi.DpiScaleX;
            var top = SystemParameters.WorkArea.Height - this.Height - margin * dpi.DpiScaleY;
            this.Left = left > 0 ? left : 0;
            this.Top = SystemParameters.WorkArea.Height; // 창을 화면 밖으로 이동

            var animation = new DoubleAnimation
            {
                From = SystemParameters.WorkArea.Height,
                To = top > 0 ? top : 0,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                BeginTime = TimeSpan.FromSeconds(0.1),
                EasingFunction = new ExponentialEase
                { EasingMode = EasingMode.EaseOut, Exponent = 6 }
            };
            animation.Completed += (s, e) => this.Topmost = true;

            this.BeginAnimation(Window.TopProperty, animation);
            this.Show();

        }

        // 팝업 숨기기
        public void ClosePopup()
        {
            if (isShown == false)
            {
                return;
            }
            isShown = false;
            // 아래로 슬라이딩하여 사라지는 애니메이션
            var animation = new DoubleAnimation
            {
                From = this.Top,
                To = SystemParameters.WorkArea.Height,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                EasingFunction = new ExponentialEase
                { EasingMode = EasingMode.EaseIn, Exponent = 3 }
            };

            animation.Completed += (s, e) => this.Hide();
            Topmost = false;
            this.BeginAnimation(Window.TopProperty, animation);
        }

        private CancellationTokenSource cts = new CancellationTokenSource();

        // 팝업 수행 (표시 후 5초 동안 대기 후 숨김)
        public async void PerformPopup()
        {
            cts.Cancel(); // 이전 작업 취소
            cts = new CancellationTokenSource(); // 새로운 CancellationTokenSource 생성

            try
            {
                ShowPopup(); // 팝업 표시
                await Task.Delay(TimeSpan.FromSeconds(5), cts.Token); // 5초 동안 대기
                ClosePopup(); // 팝업 숨기기
            }
            catch (TaskCanceledException)
            {
                // 작업이 취소되면 아무것도 하지 않음
            }
        }


        // 테마 로드
        public void loadTheme()
        {
            // 실제 적용할 테마 가져오기 (auto인 경우 시스템 테마 감지)
            string effectiveTheme = WindowsInfoGetter.GetEffectiveTheme(Settings.Default.theme);
            
            if (effectiveTheme == "light")
            {
                // 배경색 변경
                PopupBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(250, 250, 250));
                // 텍스트 색 변경
                this.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(20, 20, 20));
            }
            else if (effectiveTheme == "dark")
            {
                // 배경색 변경
                PopupBorder.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(10, 10, 10));
                // 텍스트 색 변경
                this.Foreground = new SolidColorBrush(Colors.White);
            }

            UpdateValue();
        }

        public void UpdateValue()
        {
            GetSystemPowerStatus(out SYSTEM_POWER_STATUS status);
            percentage = status.BatteryLifePercent;
            PercentLabel.Text = percentage.ToString() + "%";
            DrawBattery(percentage, plugged);
        }

        public void DrawBattery(int percentage, bool plugged)
        {
            // BatteryCanvas 내용 지우기
            BatteryCanvas.Children.Clear();

            // 실제 적용할 테마 가져오기 (auto인 경우 시스템 테마 감지)
            string effectiveTheme = WindowsInfoGetter.GetEffectiveTheme(Settings.Default.theme);
            
            // 브러시 색상 설정
            System.Windows.Media.Brush brush = System.Windows.Media.Brushes.Black;
            if (effectiveTheme == "dark")
            {
                brush = System.Windows.Media.Brushes.White;
            }

            // 좌여백값
            double LeftMargin = 0.2;

            // 충전 중일시
            if (plugged)
            {
                // 좌여백 0으로 변경
                LeftMargin = 0;

                // 번개 아이콘 그리기
                System.Windows.Shapes.Path lightningPath = new System.Windows.Shapes.Path
                {
                    Stroke = brush,
                    StrokeThickness = 0.08,
                    Fill = brush,
                    StrokeLineJoin = PenLineJoin.Round
                };

                lightningPath.SetValue(Canvas.LeftProperty, 0.7);
                lightningPath.SetValue(Canvas.TopProperty, 0.4);

                PathGeometry lpathGeometry = new PathGeometry();
                PathFigure lpathFigure = new PathFigure
                {
                    StartPoint = new System.Windows.Point(0.25, 0),
                    IsClosed = true
                };

                lpathFigure.Segments.Add(new LineSegment(new System.Windows.Point(0.05, 0.35), true));
                lpathFigure.Segments.Add(new LineSegment(new System.Windows.Point(0.2, 0.35), true));
                lpathFigure.Segments.Add(new LineSegment(new System.Windows.Point(0.15, 0.6), true));
                lpathFigure.Segments.Add(new LineSegment(new System.Windows.Point(0.35, 0.25), true));
                lpathFigure.Segments.Add(new LineSegment(new System.Windows.Point(0.2, 0.25), true));

                lpathGeometry.Figures.Add(lpathFigure);

                lightningPath.Data = lpathGeometry;
                BatteryCanvas.Children.Add(lightningPath);
            }



            // 배터리 틀 그리기
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path
            {
                Stroke = brush,
                StrokeThickness = 0.1,
                StrokeLineJoin = PenLineJoin.Round,
            };

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure
            {
                StartPoint = new System.Windows.Point(0.05, 0.2),
                IsClosed = true
            };

            pathFigure.Segments.Add(new LineSegment(new System.Windows.Point(0.6, 0.2), true));
            pathFigure.Segments.Add(new LineSegment(new System.Windows.Point(0.6, 1), true));
            pathFigure.Segments.Add(new LineSegment(new System.Windows.Point(0.05, 1), true));

            pathGeometry.Figures.Add(pathFigure);
            path.Data = pathGeometry;
            Canvas.SetLeft(path, LeftMargin);


            // 배터리 꼭지 그리기
            System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
            {
                Fill = brush,
                Width = 0.25,
                Height = 0.1
            };
            Canvas.SetLeft(rectangle, LeftMargin + 0.2);
            Canvas.SetTop(rectangle, 0.0);


            // 잔량 그리기
            System.Windows.Shapes.Rectangle remain = new System.Windows.Shapes.Rectangle
            {
                Fill = brush,
                Width = 0.35,
                Height = 0.6 * (percentage / 100.0) // 0.6은 배터리의 높이
            };

            Canvas.SetLeft(remain, LeftMargin + 0.15);
            Canvas.SetTop(remain, 0.3 + (1 - percentage / 100.0) * 0.6);


            // Path와 Rectangle, remain을 Canvas에 추가
            BatteryCanvas.Children.Add(path);
            BatteryCanvas.Children.Add(rectangle);
            BatteryCanvas.Children.Add(remain);
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            cts.Cancel(); // 타임아웃 작업 취소
            ClosePopup();
        }

        public void UpdateIcon(int percentage)
        {
            Icon icon = null;

            // 실제 적용할 트레이 테마 가져오기 (auto인 경우 시스템 테마 감지)
            string effectiveTrayTheme = WindowsInfoGetter.GetEffectiveTrayTheme(Settings.Default.traytheme);

            switch (effectiveTrayTheme)
            {
                case "white":
                    icon = BattIconWhite.ResourceManager.GetObject("_" + percentage.ToString()) as Icon;
                    break;
                case "black":
                    icon = BattIconBlack.ResourceManager.GetObject("_" + percentage.ToString()) as Icon;
                    break;
                default:
                    icon = BattIconWhite.ResourceManager.GetObject("_" + percentage.ToString()) as Icon;
                    break;
            }

            trayControl.trayIcon.Icon = icon;
            trayControl.trayIcon.Visible = true;
        }
    }


}