using Microsoft.Win32;
using System.Diagnostics;

namespace Battify
{
    internal static class StartupSetter
    {
        private const string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "Battify";

        public static bool SetStartup(bool enable)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
                ?? throw new InvalidOperationException(Localizer.Get("Startup.RegistryKeyFailed"));

            if (enable)
                key.SetValue(ValueName, QuoteExecutablePath(GetExecutablePath()), RegistryValueKind.String);
            else
                key.DeleteValue(ValueName, throwOnMissingValue: false);

            return CheckStartup();
        }

        public static bool CheckStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            var value = key?.GetValue(ValueName) as string;

            if (string.IsNullOrWhiteSpace(value))
                return false;

            return string.Equals(value.Trim(), QuoteExecutablePath(GetExecutablePath()), StringComparison.OrdinalIgnoreCase);
        }

        private static string GetExecutablePath()
        {
            return Process.GetCurrentProcess().MainModule?.FileName
                ?? Application.ExecutablePath;
        }

        private static string QuoteExecutablePath(string executablePath)
        {
            return $"\"{executablePath}\"";
        }
    }
}
