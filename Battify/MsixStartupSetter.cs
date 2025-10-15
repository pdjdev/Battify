using System;
using System.Threading.Tasks;
using Microsoft.Win32;
using Windows.ApplicationModel;

namespace Battify
{
    internal class MsixStartupSetter
    {
        // Package.appxmanifest에 정의한 TaskId와 일치해야 합니다.
        private const string TaskId = "BattifyStartup";

        // MSIX 패키지인지 확인
        private static bool IsMsixPackage()
        {
            try
            {
                // 1. 환경 변수 확인
                var familyName = Environment.GetEnvironmentVariable("PACKAGE_FAMILY_NAME");
                if (!string.IsNullOrEmpty(familyName))
                    return true;

                // 2. 실행 파일 경로 확인
                var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                if (location.Contains("WindowsApps") || location.Contains("Microsoft.WindowsStore"))
                    return true;

                // 3. Windows Runtime 패키지 API 직접 확인
                try
                {
                    var package = Package.Current;
                    if (package != null && !string.IsNullOrEmpty(package.Id.FamilyName))
                        return true;
                }
                catch
                {
                    // Windows Runtime API 사용 실패 시 무시
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> IsStartupEnabledAsync()
        {
            if (IsMsixPackage())
            {
                try
                {
                    // MSIX 환경에서 StartupTask API 직접 사용
                    var startupTask = await Windows.ApplicationModel.StartupTask.GetAsync(TaskId);
                    if (startupTask != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"현재 StartupTask 상태: {startupTask.State}");
                        
                        // Enabled만 활성 상태로 간주
                        return startupTask.State == Windows.ApplicationModel.StartupTaskState.Enabled;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"MSIX StartupTask 확인 실패: {ex.Message}");
                }
            }

            // MSIX가 아니거나 StartupTask 사용 실패 시 Registry 방식 사용
            return StartupSetter.CheckStartup();
        }

        public static async Task<bool> SetStartupAsync(bool enable)
        {
            if (IsMsixPackage())
            {
                try
                {
                    // MSIX 환경에서 StartupTask API 직접 사용 - Microsoft 문서와 정확히 동일
                    var startupTask = await Windows.ApplicationModel.StartupTask.GetAsync(TaskId);
                    if (startupTask != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"설정 전 StartupTask 상태: {startupTask.State}");
                        
                        if (enable)
                        {
                            // Microsoft 문서와 동일한 switch 구조 사용
                            switch (startupTask.State)
                            {
                                case Windows.ApplicationModel.StartupTaskState.Disabled:
                                    // Task is disabled but can be enabled.
                                    var newState = await startupTask.RequestEnableAsync();
                                    System.Diagnostics.Debug.WriteLine($"Request to enable startup, result = {newState}");
                                    return newState == Windows.ApplicationModel.StartupTaskState.Enabled;

                                case Windows.ApplicationModel.StartupTaskState.DisabledByUser:
                                    // Task is disabled and user must enable it manually.
                                    System.Diagnostics.Debug.WriteLine("You have disabled this app's ability to run as soon as you sign in, but if you change your mind, you can enable this in the Startup tab in Task Manager.");
                                    return false;

                                case Windows.ApplicationModel.StartupTaskState.DisabledByPolicy:
                                    System.Diagnostics.Debug.WriteLine("Startup disabled by group policy, or not supported on this device");
                                    return false;

                                case Windows.ApplicationModel.StartupTaskState.Enabled:
                                    System.Diagnostics.Debug.WriteLine("Startup is enabled.");
                                    return true;

                                default:
                                    System.Diagnostics.Debug.WriteLine($"Unknown state: {startupTask.State}");
                                    return false;
                            }
                        }
                        else
                        {
                            // 비활성화
                            if (startupTask.State == Windows.ApplicationModel.StartupTaskState.Enabled)
                            {
                                startupTask.Disable();
                                
                                // 잠시 대기 후 상태 재확인
                                await Task.Delay(100);
                                
                                // 상태 재확인을 위해 다시 가져오기
                                var updatedTask = await Windows.ApplicationModel.StartupTask.GetAsync(TaskId);
                                System.Diagnostics.Debug.WriteLine($"Disable 후 상태: {updatedTask.State}");
                                
                                return updatedTask.State == Windows.ApplicationModel.StartupTaskState.Disabled;
                            }
                            else
                            {
                                // 이미 비활성화됨
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"MSIX StartupTask 설정 실패: {ex.Message}");
                    return false;
                }
            }

            // MSIX가 아닌 환경에서는 Registry 방식 사용
            try
            {
                StartupSetter.SetStartup(enable);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 디버깅을 위한 추가 메서드
        public static async Task<string> GetCurrentStateAsync()
        {
            if (IsMsixPackage())
            {
                try
                {
                    var startupTask = await Windows.ApplicationModel.StartupTask.GetAsync(TaskId);
                    if (startupTask != null)
                    {
                        return startupTask.State.ToString();
                    }
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
            
            return "Not MSIX Package";
        }

        // 사용자에게 수동 활성화 메시지를 표시하는 메서드
        public static async Task<bool> HandleDisabledByUserAsync()
        {
            if (IsMsixPackage())
            {
                try
                {
                    var startupTask = await Windows.ApplicationModel.StartupTask.GetAsync(TaskId);
                    if (startupTask != null)
                    {
                        return startupTask.State == Windows.ApplicationModel.StartupTaskState.DisabledByUser;
                    }
                }
                catch
                {
                    // 무시
                }
            }
            return false;
        }
    }
}