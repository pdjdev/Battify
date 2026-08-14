namespace Battify
{
    // 개선된 다크 메뉴 렌더러
    public class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer() : base(new DarkMenuColorTable())
        {
            RoundedEdges = false; // 각진 모서리로 모던한 느낌
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected)
            {
                base.OnRenderMenuItemBackground(e);
            }
            else
            {
                // 선택된 항목의 배경을 그라디언트 없이 단색으로
                Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 50, 50)), rect);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            // 텍스트 색상 설정
            e.TextColor = e.Item.Enabled ? Color.FromArgb(240, 240, 240) : Color.FromArgb(128, 128, 128);
            
            // 텍스트 렌더링 영역 조정 - 수직 중앙 정렬을 위해
            Rectangle textRect = e.TextRectangle;
            
            // 텍스트를 수직 중앙에 배치
            textRect.Y = (e.Item.Height - e.TextRectangle.Height) / 2;
            e.TextRectangle = textRect;
            
            e.TextFormat = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            // 구분선을 더 얇고 모던하게
            if (e.Vertical)
            {
                base.OnRenderSeparator(e);
            }
            else
            {
                Rectangle rect = new Rectangle(3, 3, e.Item.Width - 6, 1);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 60)), rect);
            }
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            // 화살표 색상 개선
            e.ArrowColor = Color.FromArgb(200, 200, 200);
            base.OnRenderArrow(e);
        }
    }

    public class DarkMenuColorTable : ProfessionalColorTable
    {
        // 메뉴 배경색
        public override Color ToolStripDropDownBackground => Color.FromArgb(32, 32, 32);

        // 선택된 항목 배경색
        public override Color MenuItemSelected => Color.FromArgb(50, 50, 50);

        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(50, 50, 50);

        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(50, 50, 50);

        // 메뉴 테두리
        public override Color MenuBorder => Color.FromArgb(60, 60, 60);

        public override Color MenuItemBorder => Color.FromArgb(60, 60, 60);

        // 이미지 여백
        public override Color ImageMarginGradientBegin => Color.FromArgb(32, 32, 32);

        public override Color ImageMarginGradientMiddle => Color.FromArgb(32, 32, 32);

        public override Color ImageMarginGradientEnd => Color.FromArgb(32, 32, 32);

        // 눌린 항목
        public override Color MenuItemPressedGradientBegin => Color.FromArgb(40, 40, 40);

        public override Color MenuItemPressedGradientMiddle => Color.FromArgb(40, 40, 40);

        public override Color MenuItemPressedGradientEnd => Color.FromArgb(40, 40, 40);

        // 구분선
        public override Color SeparatorDark => Color.FromArgb(60, 60, 60);

        public override Color SeparatorLight => Color.FromArgb(60, 60, 60);
    }
}
