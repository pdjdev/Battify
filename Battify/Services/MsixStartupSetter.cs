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
                LastError = Localizer.Format("Startup.StatusCheckFailed", ex.Message);
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
                    LastError = Localizer.Format("Startup.RegistryChangeFailed", ex.Message);
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
                        LastError = Localizer.Get("Startup.DisabledByUser");
                        return false;
                    case StartupTaskState.DisabledByPolicy:
                        LastError = Localizer.Get("Startup.DisabledByPolicy");
                        return false;
                    default:
                        LastError = Localizer.Format("Startup.UnknownState", startupTask.State);
                        return false;
                }
            }
            catch (Exception ex)
            {
                LastError = Localizer.Format("Startup.MsixChangeFailed", ex.Message);
                return false;
            }
        }
    }
}
