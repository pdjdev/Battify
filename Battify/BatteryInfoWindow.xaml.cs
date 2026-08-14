using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

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

            // 창이 로드된 후 DWM 속성 설정
            this.Loaded += BatteryInfoWindow_Loaded;

            LanguageSelector.Items.Add(new System.Windows.Controls.ComboBoxItem
            {
                Tag = "system",
                Content = Localizer.Get("Language.System")
            });

            foreach (var language in Localizer.GetAvailableLanguages())
            {
                var culture = System.Globalization.CultureInfo.GetCultureInfo(language);
                LanguageSelector.Items.Add(new System.Windows.Controls.ComboBoxItem
                {
                    Tag = language,
                    Content = culture.NativeName
                });
            }

            foreach (var item in LanguageSelector.Items.OfType<System.Windows.Controls.ComboBoxItem>())
            {
                if ((string)item.Tag == Settings.Default.language)
                {
                    LanguageSelector.SelectedItem = item;
                    break;
                }
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
            AppVersionLabel.Content = Localizer.Format("Battery.AppVersion", assmblyVersion);


            // StatusTextBox를 비활성화하고 로딩 메시지 표시
            StatusTextBox.IsEnabled = false;
            StatusTextBox.Text = Localizer.Get("Common.Loading");

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
            StatusTextBox.Text = Localizer.Get("Common.Loading");

            await LoadBatteryInfoAsync();
        }

        private void UpdateText()
        {
            string resultString = "";

            try
            {

                // voltage
                string voltage = BatteryInfoGetter.Get("Voltage");
                resultString += Localizer.Format("Battery.Voltage", voltage) + Environment.NewLine;

                // DesignVoltage
                string designVoltage = BatteryInfoGetter.Get("DesignVoltage");
                resultString += Localizer.Format("Battery.DesignVoltage", designVoltage) + Environment.NewLine;

                // ChargeRate
                string chargeRate = BatteryInfoGetter.Get("ChargeRate");
                resultString += Localizer.Format("Battery.ChargeRate", chargeRate) + Environment.NewLine;

                // DischargeRate
                string dischargeRate = BatteryInfoGetter.Get("DischargeRate");
                resultString += Localizer.Format("Battery.DischargeRate", dischargeRate) + Environment.NewLine;

                // DesignCapacity
                string designCapacity = BatteryInfoGetter.Get("DesignCapacity");
                resultString += Localizer.Format("Battery.DesignCapacity", designCapacity) + Environment.NewLine;

                // MaxCapacity
                uint maxCapacity = BatteryInfoGetter.MaxCapacity();
                resultString += Localizer.Format("Battery.MaxCapacity", maxCapacity) + Environment.NewLine;

                // RemainingCapacity
                string remainingCapacity = BatteryInfoGetter.Get("RemainingCapacity");
                uint remainingCapacityUint = BatteryInfoGetter.RemainingCapacity();
                resultString += Localizer.Format("Battery.RemainingCapacity", remainingCapacity, remainingCapacityUint) + Environment.NewLine;

                // Name
                string name = BatteryInfoGetter.Get("Name");
                resultString += Localizer.Format("Battery.ModelName", name) + Environment.NewLine;

                // EstimatedChargeRemaining
                string estimatedChargeRemaining = BatteryInfoGetter.Get("EstimatedChargeRemaining");

                if (int.TryParse(estimatedChargeRemaining, out int estimatedChargeRemainingInt))
                {
                    int hours = estimatedChargeRemainingInt / 3600;
                    int minutes = estimatedChargeRemainingInt % 3600 / 60;
                    estimatedChargeRemaining = hours > 0
                        ? Localizer.Format("Battery.TimeHoursMinutes", hours, minutes)
                        : Localizer.Format("Battery.TimeMinutes", minutes);
                }

                resultString += Localizer.Format("Battery.EstimatedChargeTime", estimatedChargeRemaining) + Environment.NewLine;

                resultString += Localizer.Format("Battery.LegacyEstimatedTime", BatteryInfoGetter.EstimatedTime()) + Environment.NewLine;

                // 계산

                // 남은 용량이 숫자로 변환 가능한 경우
                if (int.TryParse(remainingCapacity, out int remainingCapacityInt))
                {
                    // 충전 퍼센트 계산
                    double percentage = (double)remainingCapacityInt / maxCapacity * 100;
                    resultString += Localizer.Format("Battery.ChargePercentage", percentage) + Environment.NewLine;

                    // 지정 용량이 숫자로 변환 가능한 경우
                    if (int.TryParse(designCapacity, out int designCapacityInt))
                    {
                        // 웨어율 계산
                        double wear = (double)(designCapacityInt - maxCapacity) / designCapacityInt * 100;
                        resultString += Localizer.Format("Battery.WearRate", wear) + Environment.NewLine;
                    }
                }

                // PowerOnline
                string powerOnline = BatteryInfoGetter.Get("PowerOnline");
                resultString += Localizer.Format("Battery.PowerOnline", powerOnline);

            }
            catch (Exception ex)
            {
                resultString = Localizer.Format("Battery.LoadFailed", ex.Message);
            }

            // 이제 프로그램 정보 덧대기

            resultString += Environment.NewLine + Environment.NewLine;

            // 프로그램 정보
            resultString += Localizer.Format("Battery.AppVersion", assmblyVersion) + Environment.NewLine;
            resultString += Localizer.Get("Battery.Author") + Environment.NewLine;
            resultString += "Source: https://github.com/pdjdev/Battify" + Environment.NewLine;
            resultString += Localizer.Get("Battery.License") + Environment.NewLine;

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

        private bool isUpdatingStartupSetting;

        private async Task HandleStartupSettingChange(bool isChecked)
        {
            // 초기화 또는 코드에 의한 UI 동기화 중에는 설정을 변경하지 않습니다.
            if (!loaded || isUpdatingStartupSetting) return;

            try
            {
                bool success = await MsixStartupSetter.SetStartupAsync(isChecked);
                if (!success)
                {
                    SetStartupCheckbox(!isChecked);
                    var errorMessage = MsixStartupSetter.LastError
                        ?? Localizer.Get("Startup.ChangeFailed");

                    global::System.Windows.MessageBox.Show(errorMessage, "Battify", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Windows가 반영한 실제 상태로 UI를 맞춥니다.
                SetStartupCheckbox(await MsixStartupSetter.IsStartupEnabledAsync());
            }
            catch (Exception ex)
            {
                SetStartupCheckbox(!isChecked);
                Debug.WriteLine($"SetStartupChk 변경 예외: {ex}");

                global::System.Windows.MessageBox.Show(Localizer.Format("Startup.ChangeError", ex.Message),
                                "Battify", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetStartupCheckbox(bool isChecked)
        {
            isUpdatingStartupSetting = true;
            try
            {
                SetStartupChk.IsChecked = isChecked;
            }
            finally
            {
                isUpdatingStartupSetting = false;
            }
        }

        private void LanguageSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LanguageSelector.SelectedValue is not string language || language == Settings.Default.language)
                return;

            Settings.Default.language = language;
            Settings.Default.Save();
            global::System.Windows.MessageBox.Show(Localizer.Get("Language.RestartRequired"),
                Localizer.Get("Language.RestartTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
