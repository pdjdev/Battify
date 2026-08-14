using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Battify
{
    /// <summary>
    /// Uses the platform startup-task registration for MSIX installations and
    /// the current-user Run key for ordinary executable launches.
    /// </summary>
    internal static class MsixStartupSetter
    {
        private const string TaskId = "BattifyStartup";
        private const int ErrorInsufficientBuffer = 122;

        public static string? LastError { get; private set; }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, System.Text.StringBuilder? packageFullName);

        private static bool IsPackaged()
        {
            var length = 0;
            var result = GetCurrentPackageFullName(ref length, null);
            return result == ErrorInsufficientBuffer;
        }

        public static async Task<bool> IsStartupEnabledAsync()
        {
            LastError = null;

            if (!IsPackaged())
                return StartupSetter.CheckStartup();

            try
            {
                var startupTask = await StartupTask.GetAsync(TaskId);
                return startupTask.State == StartupTaskState.Enabled;
            }
            catch (Exception ex)
            {
                LastError = $"MSIX 시작 프로그램 상태를 확인하지 못했습니다: {ex.Message}";
                return false;
            }
        }

        public static async Task<bool> SetStartupAsync(bool enable)
        {
            LastError = null;

            if (!IsPackaged())
            {
                try
                {
                    var enabled = StartupSetter.SetStartup(enable);
                    return enable ? enabled : !enabled;
                }
                catch (Exception ex)
                {
                    LastError = $"시작 프로그램 레지스트리를 변경하지 못했습니다: {ex.Message}";
                    return false;
                }
            }

            try
            {
                var startupTask = await StartupTask.GetAsync(TaskId);

                if (!enable)
                {
                    if (startupTask.State == StartupTaskState.Enabled)
                        startupTask.Disable();

                    return startupTask.State != StartupTaskState.Enabled;
                }

                switch (startupTask.State)
                {
                    case StartupTaskState.Enabled:
                        return true;
                    case StartupTaskState.Disabled:
                        return await startupTask.RequestEnableAsync() == StartupTaskState.Enabled;
                    case StartupTaskState.DisabledByUser:
                        LastError = "작업 관리자에서 사용자가 시작 프로그램을 해제했습니다. 작업 관리자의 시작 앱 탭에서 Battify를 다시 활성화해주세요.";
                        return false;
                    case StartupTaskState.DisabledByPolicy:
                        LastError = "조직 또는 Windows 정책에 의해 시작 프로그램이 비활성화되어 있습니다.";
                        return false;
                    default:
                        LastError = $"알 수 없는 시작 프로그램 상태입니다: {startupTask.State}";
                        return false;
                }
            }
            catch (Exception ex)
            {
                LastError = $"MSIX 시작 프로그램을 변경하지 못했습니다: {ex.Message}";
                return false;
            }
        }
    }
}
