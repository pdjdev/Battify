using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Battify
{
    /// <summary>
    /// TrayGuideWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TrayGuideWindow : Window
    {
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        public TrayGuideWindow()
        {
            InitializeComponent();

            // 창이 로드된 후 DWM 속성 설정
            this.Loaded += TrayGuideWindow_Loaded;
        }

        private void TrayGuideWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // DWM 다크 모드 속성 설정 (WPF용)
            SetDarkModeAttribute();

            // 주 화면의 우측 하단에 표시
            var workArea = SystemParameters.WorkArea;
            this.Left = workArea.Right - this.Width;
            this.Top = workArea.Bottom - this.Height;

            // 가이드 이미지 로드
            LoadGuideImage();

            // 현재 배터리 퍼센트 표시
            UpdateBatteryPercent();
        }

        // 가이드 이미지 로드
        private void LoadGuideImage()
        {
            try
            {
                // resx에서 이미지 바이트 배열 가져오기
                byte[] imageBytes = ImageResources.tray_guide_image;

                if (imageBytes != null && imageBytes.Length > 0)
                {
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        bitmap.Freeze(); // UI 스레드 외부에서 사용 가능하도록

                        GuideImage.Source = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"가이드 이미지 로드 실패: {ex.Message}");
            }
        }

        private void SetDarkModeAttribute()
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                if (hwnd != IntPtr.Zero)
                {
                    if (DwmSetWindowAttribute(hwnd, 19, new[] { 1 }, 4) != 0)
                        DwmSetWindowAttribute(hwnd, 20, new[] { 1 }, 4);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DWM 속성 설정 실패: {ex.Message}");
            }
        }

        private void UpdateBatteryPercent()
        {
            try
            {
                // MainWindow에서 배터리 상태 가져오기
                MainWindow.SYSTEM_POWER_STATUS status;
                if (MainWindow.GetSystemPowerStatus(out status))
                {
                    CurrentBatteryPercentText.Text = status.BatteryLifePercent.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"배터리 퍼센트 업데이트 실패: {ex.Message}");
                CurrentBatteryPercentText.Text = "80"; // 기본값
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // "확인했습니다" 버튼 클릭 시 설정값 저장
            Settings.Default.CheckedTrayGuide = true;
            Settings.Default.Save();

            Debug.WriteLine("트레이 가이드 확인 완료: CheckedTrayGuide = true");

            this.Close();
        }
    }
}
