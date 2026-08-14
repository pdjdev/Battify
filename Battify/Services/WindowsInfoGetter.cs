using Microsoft.Win32;

namespace Battify
{
    internal class WindowsInfoGetter
    {
        public static bool IsWindowsDarkMode()
        {
            const string RegistryKeyPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string RegistryValueName = "AppsUseLightTheme";

            try
            {
                // Read the registry value
                object value = Registry.GetValue(RegistryKeyPath, RegistryValueName, null);

                // If the value is 0, dark mode is enabled; if 1, light mode is enabled.
                // If the value is null or not an integer, assume light mode as a fallback.
                if (value is int intValue && intValue == 0)
                {
                    return true; // Dark mode is enabled
                }
                else
                {
                    return false; // Light mode is enabled or value is unexpected
                }
            }
            catch (Exception ex)
            {
                // Handle potential exceptions (e.g., registry key not found, permissions)
                Console.WriteLine($"Error reading registry: {ex.Message}");
                return false; // Assume light mode in case of error
            }
        }

        /// <summary>
        /// 현재 설정에 따라 실제 사용할 테마를 반환합니다.
        /// auto인 경우 Windows 시스템 테마를 따릅니다.
        /// </summary>
        /// <param name="settingValue">설정값 (auto, light, dark)</param>
        /// <returns>실제 적용할 테마 (light 또는 dark)</returns>
        public static string GetEffectiveTheme(string settingValue)
        {
            if (settingValue == "auto")
            {
                return IsWindowsDarkMode() ? "dark" : "light";
            }
            return settingValue;
        }

        /// <summary>
        /// 현재 설정에 따라 실제 사용할 트레이 테마를 반환합니다.
        /// auto인 경우 Windows 시스템 테마를 따릅니다.
        /// </summary>
        /// <param name="settingValue">설정값 (auto, white, black)</param>
        /// <returns>실제 적용할 트레이 테마 (white 또는 black)</returns>
        public static string GetEffectiveTrayTheme(string settingValue)
        {
            if (settingValue == "auto")
            {
                return IsWindowsDarkMode() ? "white" : "black";
            }
            return settingValue;
        }
    }
}
