using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return !(rk.GetValue("Battify") == null);
        }
    }
}
