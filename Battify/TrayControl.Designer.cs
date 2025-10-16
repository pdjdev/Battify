namespace Battify
{
    partial class TrayControl
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
            components = new System.ComponentModel.Container();
            contextMenuStrip1 = new ContextMenuStrip(components);
            closeAppToolStripMenuItem = new ToolStripMenuItem();
            changeThemeToolStripMenuItem = new ToolStripMenuItem();
            changeTrayToolStripMenuItem = new ToolStripMenuItem();
            muteToolStripMenuItem = new ToolStripMenuItem();
            showBattInfoToolStripMenuItem = new ToolStripMenuItem();
            infoItemToolStripMenuItem = new ToolStripMenuItem();
            trayIcon = new NotifyIcon(components);
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.BackColor = Color.FromArgb(30, 30, 30);
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { closeAppToolStripMenuItem, changeThemeToolStripMenuItem, changeTrayToolStripMenuItem, muteToolStripMenuItem, showBattInfoToolStripMenuItem, infoItemToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.RenderMode = ToolStripRenderMode.System;
            contextMenuStrip1.ShowImageMargin = false;
            contextMenuStrip1.Size = new Size(186, 216);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // closeAppToolStripMenuItem
            // 
            closeAppToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            closeAppToolStripMenuItem.Font = new Font("맑은 고딕", 9F);
            closeAppToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            closeAppToolStripMenuItem.Margin = new Padding(0, 5, 0, 5);
            closeAppToolStripMenuItem.Name = "closeAppToolStripMenuItem";
            closeAppToolStripMenuItem.Padding = new Padding(0);
            closeAppToolStripMenuItem.Size = new Size(185, 22);
            closeAppToolStripMenuItem.Text = "Battify 종료";
            closeAppToolStripMenuItem.Click += closeAppToolStripMenuItem_Click;
            // 
            // changeThemeToolStripMenuItem
            // 
            changeThemeToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            changeThemeToolStripMenuItem.Font = new Font("맑은 고딕", 9F);
            changeThemeToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            changeThemeToolStripMenuItem.Margin = new Padding(0, 5, 0, 5);
            changeThemeToolStripMenuItem.Name = "changeThemeToolStripMenuItem";
            changeThemeToolStripMenuItem.Padding = new Padding(0);
            changeThemeToolStripMenuItem.Size = new Size(185, 22);
            changeThemeToolStripMenuItem.Text = "팝업 색상 변경";
            changeThemeToolStripMenuItem.Click += changeThemeToolStripMenuItem_Click;
            // 
            // changeTrayToolStripMenuItem
            // 
            changeTrayToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            changeTrayToolStripMenuItem.Font = new Font("맑은 고딕", 9F);
            changeTrayToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            changeTrayToolStripMenuItem.Margin = new Padding(0, 5, 0, 5);
            changeTrayToolStripMenuItem.Name = "changeTrayToolStripMenuItem";
            changeTrayToolStripMenuItem.Padding = new Padding(0);
            changeTrayToolStripMenuItem.Size = new Size(185, 22);
            changeTrayToolStripMenuItem.Text = "아이콘 색상 변경";
            changeTrayToolStripMenuItem.Click += changeTrayToolStripMenuItem_Click;
            // 
            // muteToolStripMenuItem
            // 
            muteToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            muteToolStripMenuItem.Font = new Font("맑은 고딕", 9F);
            muteToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            muteToolStripMenuItem.Margin = new Padding(0, 5, 0, 5);
            muteToolStripMenuItem.Name = "muteToolStripMenuItem";
            muteToolStripMenuItem.Padding = new Padding(0);
            muteToolStripMenuItem.Size = new Size(185, 22);
            muteToolStripMenuItem.Text = "알림음 끄기";
            muteToolStripMenuItem.Click += muteToolStripMenuItem_Click;
            // 
            // showBattInfoToolStripMenuItem
            // 
            showBattInfoToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            showBattInfoToolStripMenuItem.Font = new Font("맑은 고딕", 9F);
            showBattInfoToolStripMenuItem.ForeColor = Color.White;
            showBattInfoToolStripMenuItem.Margin = new Padding(0, 5, 0, 5);
            showBattInfoToolStripMenuItem.Name = "showBattInfoToolStripMenuItem";
            showBattInfoToolStripMenuItem.Padding = new Padding(0);
            showBattInfoToolStripMenuItem.Size = new Size(185, 22);
            showBattInfoToolStripMenuItem.Text = "정보 (시작 설정)";
            showBattInfoToolStripMenuItem.Click += showBattInfoToolStripMenuItem_Click;
            // 
            // infoItemToolStripMenuItem
            // 
            infoItemToolStripMenuItem.Enabled = false;
            infoItemToolStripMenuItem.Font = new Font("맑은 고딕", 7.8F, FontStyle.Italic, GraphicsUnit.Point, 0);
            infoItemToolStripMenuItem.ForeColor = Color.Silver;
            infoItemToolStripMenuItem.Name = "infoItemToolStripMenuItem";
            infoItemToolStripMenuItem.Size = new Size(185, 24);
            infoItemToolStripMenuItem.Text = "Battify (1.0.0)";
            // 
            // trayIcon
            // 
            trayIcon.ContextMenuStrip = contextMenuStrip1;
            trayIcon.Text = "Battify";
            trayIcon.Visible = true;
            trayIcon.MouseDoubleClick += trayIcon_MouseDoubleClick;
            // 
            // TrayControl
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Name = "TrayControl";
            Size = new Size(174, 162);
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        public ContextMenuStrip contextMenuStrip1;
        public NotifyIcon trayIcon;
        private ToolStripMenuItem closeAppToolStripMenuItem;
        private ToolStripMenuItem changeThemeToolStripMenuItem;
        private ToolStripMenuItem changeTrayToolStripMenuItem;
        private ToolStripMenuItem showBattInfoToolStripMenuItem;
        private ToolStripMenuItem infoItemToolStripMenuItem;
        private ToolStripMenuItem muteToolStripMenuItem;
    }
}
