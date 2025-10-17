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
            appearanceToolStripMenuItem = new ToolStripMenuItem();
            changeTrayToolStripMenuItem = new ToolStripMenuItem();
            changeThemeToolStripMenuItem = new ToolStripMenuItem();
            notificationToolStripMenuItem = new ToolStripMenuItem();
            muteToolStripMenuItem = new ToolStripMenuItem();
            togglePopupToolStripMenuItem = new ToolStripMenuItem();
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
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { closeAppToolStripMenuItem, appearanceToolStripMenuItem, notificationToolStripMenuItem, showBattInfoToolStripMenuItem, infoItemToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.RenderMode = ToolStripRenderMode.System;
            contextMenuStrip1.ShowImageMargin = false;
            contextMenuStrip1.Size = new Size(143, 184);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // closeAppToolStripMenuItem
            // 
            closeAppToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            closeAppToolStripMenuItem.Font = new Font("맑은 고딕", 10F);
            closeAppToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            closeAppToolStripMenuItem.Name = "closeAppToolStripMenuItem";
            closeAppToolStripMenuItem.Padding = new Padding(4, 6, 4, 6);
            closeAppToolStripMenuItem.Size = new Size(150, 38);
            closeAppToolStripMenuItem.Text = "Battify 종료";
            closeAppToolStripMenuItem.Click += closeAppToolStripMenuItem_Click;
            // 
            // appearanceToolStripMenuItem
            // 
            appearanceToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            appearanceToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { changeTrayToolStripMenuItem, changeThemeToolStripMenuItem });
            appearanceToolStripMenuItem.Font = new Font("맑은 고딕", 10F);
            appearanceToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            appearanceToolStripMenuItem.Name = "appearanceToolStripMenuItem";
            appearanceToolStripMenuItem.Padding = new Padding(4, 6, 4, 6);
            appearanceToolStripMenuItem.Size = new Size(150, 38);
            appearanceToolStripMenuItem.Text = "모양";
            // 
            // changeTrayToolStripMenuItem
            // 
            changeTrayToolStripMenuItem.BackColor = Color.FromArgb(30, 30, 30);
            changeTrayToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            changeTrayToolStripMenuItem.Font = new Font("맑은 고딕", 9F);
            changeTrayToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            changeTrayToolStripMenuItem.Name = "changeTrayToolStripMenuItem";
            changeTrayToolStripMenuItem.Padding = new Padding(4, 6, 4, 6);
            changeTrayToolStripMenuItem.Size = new Size(204, 36);
            changeTrayToolStripMenuItem.Text = "아이콘 색: 자동";
            changeTrayToolStripMenuItem.Click += changeTrayToolStripMenuItem_Click;
            // 
            // changeThemeToolStripMenuItem
            // 
            changeThemeToolStripMenuItem.BackColor = Color.FromArgb(30, 30, 30);
            changeThemeToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            changeThemeToolStripMenuItem.Font = new Font("맑은 고딕", 9F);
            changeThemeToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            changeThemeToolStripMenuItem.Name = "changeThemeToolStripMenuItem";
            changeThemeToolStripMenuItem.Padding = new Padding(4, 6, 4, 6);
            changeThemeToolStripMenuItem.Size = new Size(204, 36);
            changeThemeToolStripMenuItem.Text = "팝업 색: 자동";
            changeThemeToolStripMenuItem.Click += changeThemeToolStripMenuItem_Click;
            // 
            // notificationToolStripMenuItem
            // 
            notificationToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            notificationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { muteToolStripMenuItem, togglePopupToolStripMenuItem });
            notificationToolStripMenuItem.Font = new Font("맑은 고딕", 10F);
            notificationToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            notificationToolStripMenuItem.Name = "notificationToolStripMenuItem";
            notificationToolStripMenuItem.Padding = new Padding(4, 6, 4, 6);
            notificationToolStripMenuItem.Size = new Size(150, 38);
            notificationToolStripMenuItem.Text = "알림";
            // 
            // muteToolStripMenuItem
            // 
            muteToolStripMenuItem.BackColor = Color.FromArgb(30, 30, 30);
            muteToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            muteToolStripMenuItem.Font = new Font("맑은 고딕", 9F);
            muteToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            muteToolStripMenuItem.Name = "muteToolStripMenuItem";
            muteToolStripMenuItem.Padding = new Padding(4, 6, 4, 6);
            muteToolStripMenuItem.Size = new Size(181, 36);
            muteToolStripMenuItem.Text = "알림음 끄기";
            muteToolStripMenuItem.Click += muteToolStripMenuItem_Click;
            // 
            // togglePopupToolStripMenuItem
            // 
            togglePopupToolStripMenuItem.BackColor = Color.FromArgb(30, 30, 30);
            togglePopupToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            togglePopupToolStripMenuItem.Font = new Font("맑은 고딕", 9F);
            togglePopupToolStripMenuItem.ForeColor = Color.FromArgb(224, 224, 224);
            togglePopupToolStripMenuItem.Name = "togglePopupToolStripMenuItem";
            togglePopupToolStripMenuItem.Padding = new Padding(4, 6, 4, 6);
            togglePopupToolStripMenuItem.Size = new Size(181, 36);
            togglePopupToolStripMenuItem.Text = "팝업 끄기";
            togglePopupToolStripMenuItem.Click += togglePopupToolStripMenuItem_Click;
            // 
            // showBattInfoToolStripMenuItem
            // 
            showBattInfoToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            showBattInfoToolStripMenuItem.Font = new Font("맑은 고딕", 10F);
            showBattInfoToolStripMenuItem.ForeColor = Color.White;
            showBattInfoToolStripMenuItem.Name = "showBattInfoToolStripMenuItem";
            showBattInfoToolStripMenuItem.Padding = new Padding(4, 6, 4, 6);
            showBattInfoToolStripMenuItem.Size = new Size(150, 38);
            showBattInfoToolStripMenuItem.Text = "정보 / 설정";
            showBattInfoToolStripMenuItem.Click += showBattInfoToolStripMenuItem_Click;
            // 
            // infoItemToolStripMenuItem
            // 
            infoItemToolStripMenuItem.Enabled = false;
            infoItemToolStripMenuItem.Font = new Font("맑은 고딕", 7.8F, FontStyle.Italic, GraphicsUnit.Point, 0);
            infoItemToolStripMenuItem.ForeColor = Color.Silver;
            infoItemToolStripMenuItem.Name = "infoItemToolStripMenuItem";
            infoItemToolStripMenuItem.Size = new Size(142, 28);
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
        private ToolStripMenuItem appearanceToolStripMenuItem;
        private ToolStripMenuItem changeThemeToolStripMenuItem;
        private ToolStripMenuItem changeTrayToolStripMenuItem;
        private ToolStripMenuItem notificationToolStripMenuItem;
        private ToolStripMenuItem muteToolStripMenuItem;
        private ToolStripMenuItem togglePopupToolStripMenuItem;
        private ToolStripMenuItem showBattInfoToolStripMenuItem;
        private ToolStripMenuItem infoItemToolStripMenuItem;
    }
}
