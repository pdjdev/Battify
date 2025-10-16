using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Battify
{
    /// <summary>
    /// BatteryInfoWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BatteryInfoWindow : Window
    {
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        private bool loaded = false;
        private string assmblyVersion = "";

        public BatteryInfoWindow()
        {
            InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version != null)
            {
                // Major.Minor.Build 형식으로 (Revision 제외)
                assmblyVersion = $"{version.Major}.{version.Minor}.{version.Build}";
            }
            else
            {
                assmblyVersion = "1.0.0";
            }

            // 배너 이미지 로드
            LoadBannerImage();

            // 창이 로드된 후 DWM 속성 설정
            this.Loaded += BatteryInfoWindow_Loaded;
        }

        private void LoadBannerImage()
        {
            try
            {
                // resx에서 이미지 바이트 배열 가져오기
                byte[] imageBytes = ImageResources.battify_info_banner;
                
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
                        
                        BannerImage.Source = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"배너 이미지 로드 실패: {ex.Message}");
            }
        }

        private async void BatteryInfoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // DWM 다크 모드 속성 설정 (WPF용)
            SetDarkModeAttribute();

            // 주 화면의 우측 하단에 표시
            var workArea = SystemParameters.WorkArea;
            this.Left = workArea.Right - this.Width;
            this.Top = workArea.Bottom - this.Height;

            // 버전 정보 갱신해서 표시
            AppVersionLabel.Content = "배티파이, v" + assmblyVersion;


            // StatusTextBox를 비활성화하고 로딩 메시지 표시
            StatusTextBox.IsEnabled = false;
            StatusTextBox.Text = "로드 중...";

            // 시작프로그램 설정 확인 및 디버깅 정보 출력
            SetStartupChk.IsChecked = await MsixStartupSetter.IsStartupEnabledAsync();

            loaded = true;

            // 배터리 정보를 비동기적으로 로드
            await LoadBatteryInfoAsync();
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

        private async Task LoadBatteryInfoAsync()
        {
            await Task.Run(() =>
            {
                BatteryInfoGetter.Load();
            });

            // UI 스레드에서 텍스트 업데이트 및 StatusTextBox 활성화
            Dispatcher.Invoke(() =>
            {
                StatusTextBox.IsEnabled = true;
                UpdateText();
            });
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // 업데이트 버튼도 비동기로 처리
            StatusTextBox.IsEnabled = false;
            StatusTextBox.Text = "로드 중...";

            await LoadBatteryInfoAsync();
        }

        private void UpdateText()
        {
            string resultString = "";

            try
            {

                // voltage
                string voltage = BatteryInfoGetter.Get("Voltage");
                resultString += "전압: " + voltage + " mV" + Environment.NewLine;

                // DesignVoltage
                string designVoltage = BatteryInfoGetter.Get("DesignVoltage");
                resultString += "지정 전압: " + designVoltage + " mV" + Environment.NewLine;

                // ChargeRate
                string chargeRate = BatteryInfoGetter.Get("ChargeRate");
                resultString += "충전율: " + chargeRate + " mW" + Environment.NewLine;

                // DischargeRate
                string dischargeRate = BatteryInfoGetter.Get("DischargeRate");
                resultString += "방전율: " + dischargeRate + " mW" + Environment.NewLine;

                // DesignCapacity
                string designCapacity = BatteryInfoGetter.Get("DesignCapacity");
                resultString += "지정 용량: " + designCapacity + " mWh" + Environment.NewLine;

                // MaxCapacity
                uint maxCapacity = BatteryInfoGetter.MaxCapacity();
                resultString += "완충 용량: " + maxCapacity + " mWh" + Environment.NewLine;

                // RemainingCapacity
                string remainingCapacity = BatteryInfoGetter.Get("RemainingCapacity");
                uint remainingCapacityUint = BatteryInfoGetter.RemainingCapacity();
                resultString += "남은 용량 (레거시): " + remainingCapacity + "(" + remainingCapacityUint.ToString() + ") mWh" + Environment.NewLine;

                // Name
                string name = BatteryInfoGetter.Get("Name");
                resultString += "모델명: " + name + Environment.NewLine;

                // EstimatedChargeRemaining
                string estimatedChargeRemaining = BatteryInfoGetter.Get("EstimatedChargeRemaining");

                if (int.TryParse(estimatedChargeRemaining, out int estimatedChargeRemainingInt))
                {
                    int hours = estimatedChargeRemainingInt / 3600;
                    int minutes = estimatedChargeRemainingInt % 3600 / 60;
                    estimatedChargeRemaining = "";

                    if (hours > 0)
                    {
                        estimatedChargeRemaining = hours + "시간 ";
                    }

                    estimatedChargeRemaining += minutes + "분";
                }

                resultString += "충전 예상 시간: " + estimatedChargeRemaining + Environment.NewLine;

                resultString += "레거시 예상 시간: " + BatteryInfoGetter.EstimatedTime().ToString() + Environment.NewLine;

                // 계산

                // 남은 용량이 숫자로 변환 가능한 경우
                if (int.TryParse(remainingCapacity, out int remainingCapacityInt))
                {
                    // 충전 퍼센트 계산
                    double percentage = (double)remainingCapacityInt / maxCapacity * 100;
                    resultString += "충전 퍼센트: " + percentage.ToString("0.00") + "%" + Environment.NewLine;

                    // 지정 용량이 숫자로 변환 가능한 경우
                    if (int.TryParse(designCapacity, out int designCapacityInt))
                    {
                        // 웨어율 계산
                        double wear = (double)(designCapacityInt - maxCapacity) / designCapacityInt * 100;
                        resultString += "웨어율: " + wear.ToString("0.00") + "%" + Environment.NewLine;
                    }
                }

                // PowerOnline
                string powerOnline = BatteryInfoGetter.Get("PowerOnline");
                resultString += "전원 연결: " + powerOnline;

            }
            catch (Exception ex)
            {
                resultString = "배터리 정보 로드 실패: " + ex.Message;
            }

            // 이제 프로그램 정보 덧대기

            resultString += Environment.NewLine + Environment.NewLine;

            // 프로그램 정보
            resultString += "Battify v" + assmblyVersion + Environment.NewLine;
            resultString += "Made by PBJSoftware (박동준)" + Environment.NewLine;
            resultString += "Source: https://github.com/pdjdev/Battify" + Environment.NewLine;
            resultString += "본 프로그램은 MIT License 하에 자유롭게 이용이 가능합니다." + Environment.NewLine;

            // StatusTextBox 출력
            StatusTextBox.Text = resultString;
        }

        private async void SetStartupChk_Checked(object sender, RoutedEventArgs e)
        {
            await HandleStartupSettingChange(true);
        }

        private async void SetStartupChk_Unchecked(object sender, RoutedEventArgs e)
        {
            await HandleStartupSettingChange(false);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenUpdatePageButton_Click(object sender, RoutedEventArgs e)
        {
            // battify-latest-store.pbj.kr 로 접속
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://link.pbj.kr/battify-store",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"업데이트 페이지 열기 실패: {ex.Message}");
            }
        }

        private void DevSiteButton_Click(object sender, RoutedEventArgs e)
        {
            // battify-latest-store.pbj.kr 로 접속
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://pbj.kr",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"업데이트 페이지 열기 실패: {ex.Message}");
            }
        }

        private async Task HandleStartupSettingChange(bool isChecked)
        {
            // 로드된 상태가 아니면 무시 (초기화 중 이벤트 방지)
            if (!loaded) return;

            try
            {
                // 디버깅: 설정 시도 전 상태 확인
                if (IsMsixEnvironment())
                {
                    var beforeState = await MsixStartupSetter.GetCurrentStateAsync();
                    Debug.WriteLine($"설정 시도 전 상태: {beforeState}");
                }

                bool success = await MsixStartupSetter.SetStartupAsync(isChecked);

                // 디버깅: 설정 시도 후 상태 확인
                if (IsMsixEnvironment())
                {
                    var afterState = await MsixStartupSetter.GetCurrentStateAsync();
                    Debug.WriteLine($"설정 시도 후 상태: {afterState}");
                }

                if (!success)
                {
                    // 설정 실패 시 체크박스 상태를 원래대로 되돌림
                    SetStartupChk.IsChecked = !isChecked;

                    // DisabledByUser 상태인지 확인
                    bool isDisabledByUser = await MsixStartupSetter.HandleDisabledByUserAsync();

                    string errorMessage;
                    if (isDisabledByUser)
                    {
                        errorMessage = "시작 프로그램 설정이 사용자에 의해 비활성화되었습니다.\n\n" +
                                      "다시 활성화하려면:\n" +
                                      "• 작업 관리자 > 시작프로그램 탭에서 'Battify'를 찾아 '사용'으로 설정하거나\n" +
                                      "• Windows 설정 > 앱 > 시작프로그램에서 'Battify'를 켜기로 설정하세요.";
                    }
                    else if (IsMsixEnvironment())
                    {
                        errorMessage = "시작 프로그램 설정에 실패했습니다.\n\n" +
                                      "Microsoft Store에서 설치된 앱의 경우:\n" +
                                      "• Windows 설정 > 앱 > 시작프로그램에서 직접 설정할 수 있습니다.\n" +
                                      "• 또는 작업 관리자 > 시작프로그램 탭에서 관리할 수 있습니다.";
                    }
                    else
                    {
                        errorMessage = "시작 프로그램 설정에 실패했습니다.";
                    }

                    global::System.Windows.MessageBox.Show(errorMessage, "Battify", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // 성공 후 실제 상태를 다시 확인하여 UI 동기화
                bool actualState = await MsixStartupSetter.IsStartupEnabledAsync();
                if (SetStartupChk.IsChecked != actualState)
                {
                    SetStartupChk.IsChecked = actualState;
                }
            }
            catch (Exception ex)
            {
                // 설정 실패 시 체크박스 상태를 원래대로 되돌림
                SetStartupChk.IsChecked = !isChecked;

                Debug.WriteLine($"SetStartupChk 변경 예외: {ex}");

                global::System.Windows.MessageBox.Show("시작 프로그램 설정 중 오류가 발생했습니다." + Environment.NewLine + ex.Message,
                                "Battify", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (isChecked)
            {
                // global::System.Windows.MessageBox.Show("시작 프로그램으로 설정되었습니다.", "Battify", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // global::System.Windows.MessageBox.Show("시작 프로그램 설정이 해제되었습니다.", "Battify", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool IsMsixEnvironment()
        {
            try
            {
                var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return location.Contains("WindowsApps") || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PACKAGE_FAMILY_NAME"));
            }
            catch
            {
                return false;
            }
        }
    }
}