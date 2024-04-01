namespace Battify
{
    partial class OptionForm
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
            panel1 = new Panel();
            optionTabBt1 = new OptionTabBt();
            panel2 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(optionTabBt1);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(10);
            panel1.Size = new Size(210, 359);
            panel1.TabIndex = 0;
            // 
            // optionTabBt1
            // 
            optionTabBt1.BackColor = Color.Transparent;
            optionTabBt1.ButtonText = "Welcome!";
            optionTabBt1.Dock = DockStyle.Top;
            optionTabBt1.Location = new Point(10, 10);
            optionTabBt1.Name = "optionTabBt1";
            optionTabBt1.Padding = new Padding(10);
            optionTabBt1.Size = new Size(190, 71);
            optionTabBt1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(210, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(457, 359);
            panel2.TabIndex = 1;
            // 
            // OptionForm
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(27, 27, 27);
            ClientSize = new Size(667, 359);
            Controls.Add(panel2);
            Controls.Add(panel1);
            ForeColor = Color.White;
            Name = "OptionForm";
            StartPosition = FormStartPosition.Manual;
            Text = "OptionForm";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private OptionTabBt optionTabBt1;
    }
}