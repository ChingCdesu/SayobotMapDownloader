using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Gdu.WinFormUI
{
    public class GMScrollBarThemeBase : IDisposable
    {
        public int InnerPaddingWidth { get; set; }
        public int MiddleButtonOutterSpace1 { get; set; }
        public int MiddleButtonOutterSpace2 { get; set; }
        public int SideButtonLength { get; set; }
        public int BestUndirectLen { get; set; }
        public bool DrawBackground { get; set; }
        public bool DrawBorder { get; set; }
        public bool DrawInnerBorder { get; set; }
        public bool ShowSideButtons { get; set; }
        public Color BackColor { get; set; }
        public Color BorderColor { get; set; }
        public Color InnerBorderColor { get; set; }
        public Size SideButtonForePathSize { get; set; }
        public ButtonForePathGetter SideButtonForePathGetter { get; set; }
        public ButtonColorTable SideButtonColorTable { get; set; }
        public ButtonColorTable MiddleButtonColorTable { get; set; }
        public ForePathRenderMode HowSideButtonForePathDraw { get; set; }

        public bool DrawLinesInMiddleButton { get; set; }
        public Color MiddleButtonLine1Color { get; set; }
        public Color MiddleButtonLine2Color { get; set; }
        public int MiddleBtnLineOutterSpace1 { get; set; }
        public int MiddleBtnLineOutterSpace2 { get; set; }

        public int SideButtonRadius { get; set; }
        public int MiddleButtonRadius { get; set; }
        public ButtonBorderType SideButtonBorderType { get; set; }

        private void SetDefaultValue()
        {
            InnerPaddingWidth = 0;
            MiddleButtonOutterSpace1 = 1;
            MiddleButtonOutterSpace2 = 0;
            SideButtonLength = 16;
            BestUndirectLen = 15;
            DrawBackground = true;
            DrawBorder = false;
            DrawInnerBorder = false;
            ShowSideButtons = true;
            //SideButtonCanDisabled = false;
            BackColor = Color.FromArgb(227,227,227);
            BorderColor = Color.FromArgb(248, 248, 248);

            SideButtonForePathSize = new Size(10, 9);
            SideButtonForePathGetter = new ButtonForePathGetter(
                Gdu.WinFormUI.MyGraphics.GraphicsPathHelper.Create7x4DownTriangleFlag);

            SideButtonColorTable = SideBtnColor();
            MiddleButtonColorTable = MdlBtnColor();            

            HowSideButtonForePathDraw = ForePathRenderMode.Draw;

            DrawLinesInMiddleButton = true;
            MiddleBtnLineOutterSpace1 = 4;
            MiddleBtnLineOutterSpace2 = 4;
            MiddleButtonLine1Color = Color.FromArgb(89, 89, 89);
            MiddleButtonLine2Color = Color.FromArgb(182, 182, 182);

            SideButtonRadius = MiddleButtonRadius = 0;
            SideButtonBorderType = ButtonBorderType.Rectangle;
        }

        private ButtonColorTable SideBtnColor()
        {
            ButtonColorTable table = MdlBtnColor();

            table.ForeColorNormal = Color.FromArgb(73, 73, 73);
            table.ForeColorHover = Color.FromArgb(32, 106, 145);
            table.ForeColorPressed = Color.FromArgb(15, 38, 50);
            table.ForeColorDisabled = SystemColors.ControlDarkDark;

            table.BackColorDisabled = SystemColors.ControlDark;

            return table;
        }

        private ButtonColorTable MdlBtnColor()
        {
            ButtonColorTable table = new ButtonColorTable();

            table.BorderColorNormal = Color.FromArgb(151, 151, 151);
            table.BorderColorHover = Color.FromArgb(53, 111, 155);
            table.BorderColorPressed = Color.FromArgb(60, 127, 177);

            table.BackColorNormal = Color.FromArgb(217, 218, 219);
            table.BackColorHover = Color.FromArgb(169, 219, 246);
            table.BackColorPressed = Color.FromArgb(111, 202, 240);

            return table;
        }

        public GMScrollBarThemeBase()
        {
            SetDefaultValue();
        }

        #region IDisposable

        public void Dispose()
        {
            //if (SideButtonForePath != null)
            //    SideButtonForePath.Dispose();
        }

        #endregion
    }
}
