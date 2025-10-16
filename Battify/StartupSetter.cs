using Microsoft.Win32;

namespace Battify
{
    internal class StartupSetter
    {
        public static void SetStartup(bool set)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (set)
                rk.SetValue("Battify", Application.ExecutablePath);
            else
                rk.DeleteValue("Battify", false);

        }

        public static bool CheckStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            var registryValue = rk.GetValue("Battify") as string;
            return registryValue != null && registryValue.Equals(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase);
        }
    }
}
