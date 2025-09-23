using Microsoft.Win32;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Battify
{

    public partial class BatteryInfoForm : Form
    {
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }

        bool loaded = false;

        public BatteryInfoForm()
        {
            InitializeComponent();
        }

        private async void BatteryInfoForm_Load(object sender, EventArgs e)
        {
            // 주 화면의 우측 하단에 표시
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            
            // StatusTextBox를 비활성화하고 로딩 메시지 표시
            StatusTextBox.Enabled = false;
            StatusTextBox.Text = "로드 중...";
            
            SetStartupChk.Checked = StartupSetter.CheckStartup();
            
            // 배터리 정보를 비동기적으로 로드
            await LoadBatteryInfoAsync();
            
            loaded = true;
        }

        private async Task LoadBatteryInfoAsync()
        {
            await Task.Run(() => 
            {
                BatteryInfoGetter.Load();
            });
            
            // UI 스레드에서 텍스트 업데이트 및 StatusTextBox 활성화
            StatusTextBox.Enabled = true;
            updateText();
        }

        private void startTimer_Click(object sender, EventArgs e)
        {
            BatteryInfoGetter.StartChecking();
        }

        private void endTimer_Click(object sender, EventArgs e)
        {
            BatteryInfoGetter.StopChecking();
        }

        private void refreshBt_Click(object sender, EventArgs e)
        {
            updateText();
        }

        private void updateText()
        {
            string resultString = "";

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

            // StatusTextBox 출력
            StatusTextBox.Text = resultString;
        }

        private async void updateBt_Click(object sender, EventArgs e)
        {
            // 업데이트 버튼도 비동기로 처리
            StatusTextBox.Enabled = false;
            StatusTextBox.Text = "로드 중...";
            
            await LoadBatteryInfoAsync();
        }

        private void SetStartupChk_CheckedChanged(object sender, EventArgs e)
        {
            if (!loaded) return;
           
            try
            {
                StartupSetter.SetStartup(SetStartupChk.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show("시작 프로그램 설정에 실패했습니다." + Environment.NewLine + ex.Message,
                                "Battify", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (SetStartupChk.Checked)
            {
                MessageBox.Show("시작 프로그램으로 설정되었습니다.", "Battify", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("시작 프로그램 설정이 해제되었습니다.", "Battify", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
