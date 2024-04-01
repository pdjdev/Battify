using System.ComponentModel;

namespace Battify
{
    partial class OptionTabBt
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            buttonPanel = new Panel();
            buttonTextLabel = new Label();
            buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // buttonPanel
            // 
            buttonPanel.BackColor = Color.Transparent;
            buttonPanel.Controls.Add(buttonTextLabel);
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.Location = new Point(10, 10);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Size = new Size(251, 75);
            buttonPanel.TabIndex = 0;
            buttonPanel.Paint += buttonPanel_Paint;
            // 
            // buttonTextLabel
            // 
            buttonTextLabel.Dock = DockStyle.Fill;
            buttonTextLabel.Font = new Font("맑은 고딕", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonTextLabel.ForeColor = Color.Black;
            buttonTextLabel.Location = new Point(0, 0);
            buttonTextLabel.Name = "buttonTextLabel";
            buttonTextLabel.Size = new Size(251, 75);
            buttonTextLabel.TabIndex = 0;
            buttonTextLabel.Text = "Button Text";
            buttonTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // OptionTabBt
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Transparent;
            Controls.Add(buttonPanel);
            Name = "OptionTabBt";
            Padding = new Padding(10);
            Size = new Size(271, 95);
            buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel buttonPanel;
        private Label buttonTextLabel;

        [Description("Test text displayed in the textbox"), Category("Data")]
        public string ButtonText
        {
            get => buttonTextLabel.Text;
            set => buttonTextLabel.Text = value;
        }

    }

    
}
