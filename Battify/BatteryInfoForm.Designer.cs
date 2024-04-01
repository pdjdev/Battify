using System.Resources;

namespace Battify
{
    partial class BatteryInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatteryInfoForm));
            updateBt = new Button();
            StatusTextBox = new TextBox();
            SetStartupChk = new CheckBox();
            SuspendLayout();
            // 
            // updateBt
            // 
            updateBt.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            updateBt.BackColor = Color.FromArgb(40, 40, 40);
            updateBt.FlatAppearance.BorderSize = 0;
            updateBt.FlatStyle = FlatStyle.Flat;
            updateBt.ForeColor = Color.White;
            updateBt.Location = new Point(293, 289);
            updateBt.Name = "updateBt";
            updateBt.Size = new Size(134, 42);
            updateBt.TabIndex = 0;
            updateBt.Text = "새로 고침";
            updateBt.UseVisualStyleBackColor = false;
            updateBt.Click += updateBt_Click;
            // 
            // StatusTextBox
            // 
            StatusTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            StatusTextBox.BackColor = Color.FromArgb(32, 32, 32);
            StatusTextBox.BorderStyle = BorderStyle.FixedSingle;
            StatusTextBox.ForeColor = Color.White;
            StatusTextBox.Location = new Point(12, 12);
            StatusTextBox.Multiline = true;
            StatusTextBox.Name = "StatusTextBox";
            StatusTextBox.ReadOnly = true;
            StatusTextBox.ScrollBars = ScrollBars.Vertical;
            StatusTextBox.Size = new Size(415, 271);
            StatusTextBox.TabIndex = 1;
            // 
            // SetStartupChk
            // 
            SetStartupChk.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            SetStartupChk.AutoSize = true;
            SetStartupChk.ForeColor = Color.White;
            SetStartupChk.Location = new Point(12, 307);
            SetStartupChk.Name = "SetStartupChk";
            SetStartupChk.Size = new Size(156, 24);
            SetStartupChk.TabIndex = 2;
            SetStartupChk.Text = "시작프로그램 설정";
            SetStartupChk.UseVisualStyleBackColor = true;
            SetStartupChk.CheckedChanged += SetStartupChk_CheckedChanged;
            // 
            // BatteryInfoForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(27, 27, 27);
            ClientSize = new Size(439, 343);
            Controls.Add(SetStartupChk);
            Controls.Add(StatusTextBox);
            Controls.Add(updateBt);
            Icon = (Icon)AppIcon.ResourceManager.GetObject("icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BatteryInfoForm";
            StartPosition = FormStartPosition.Manual;
            Text = "배터리 정보";
            Load += BatteryInfoForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button updateBt;
        private TextBox StatusTextBox;
        private CheckBox SetStartupChk;
    }
}