using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Battify
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemBatteryState
    {
        public byte AcOnLine;
        public byte BatteryPresent;
        public byte Charging;
        public byte Discharging;
        public byte Spare1;
        public byte Spare2;
        public byte Spare3;
        public byte Spare4;
        public uint MaxCapacity;
        public uint RemainingCapacity;
        public uint Rate;
        public uint EstimatedTime;
        public uint DefaultAlert1;
        public uint DefaultAlert2;
    }

    public class BatteryInfoGetter
    {
        [DllImport("PowrProf.dll")]
        public static extern uint CallNtPowerInformation(
            int InformationLevel,
            IntPtr lpInputBuffer,
            uint nInputBufferSize,
            out SystemBatteryState lpOutputBuffer,
            uint nOutputBufferSize
        );

        private static Dictionary<string, string> batteryInfo = new Dictionary<string, string>();
        private static System.Timers.Timer? timer;
        private static bool isChecking = false;
        private static SystemBatteryState batteryState;

        public static void Load()
        {
            string command = "";
            command += "gwmi -Class batterystatus -Namespace root\\wmi ;";
            command += "Get-CimInstance -Namespace root/cimv2 -ClassName \"Win32_Battery\" ;";
            command += "Get-CimInstance -Namespace root/cimv2 -ClassName \"Win32_PortableBattery\"";

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"" + command + "\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var lines = output.Split('\n');
            foreach (var line in lines)
            {
                // lf line hasn't a colon
                if (!line.Contains(':')) continue;

                var parts = line.Split(':');
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    // if key is already in the dictionary, and not null, update the value
                    if (batteryInfo.ContainsKey(key))
                    {
                        if (value != "")
                        {
                            batteryInfo[key] = value;
                        }
                    }
                    else
                    {
                        batteryInfo.Add(key, value);
                    }
                }
            }

            // SystemSounds.Beep.Play();

            // 레거시 API 업데이트
            CallNtPowerInformation(5, IntPtr.Zero, 0, out batteryState, (uint)Marshal.SizeOf(typeof(SystemBatteryState)));
        }

        public static string Get(string parameter)
        {
            if (batteryInfo.ContainsKey(parameter))
            {
                return batteryInfo[parameter];
            }
            else
            {
                return null;
            }
        }

        public static void StartChecking(int interval = 5000)
        {
            if (isChecking) return;

            isChecking = true;
            timer = new System.Timers.Timer(interval);
            timer.Elapsed += (sender, e) => Load();
            timer.Start();

        }

        public static void StopChecking()
        {
            if (!isChecking) return;

            timer.Stop();
            isChecking = false;
        }

        public static uint MaxCapacity()
        {
            return batteryState.MaxCapacity;
        }

        public static uint RemainingCapacity()
        {
            return batteryState.RemainingCapacity;
        }

        public static uint EstimatedTime()
        {
            return batteryState.EstimatedTime;
        }
    }
}
