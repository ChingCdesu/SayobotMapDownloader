using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    public class ThemeScrollbarXMPBlack : GMScrollBarThemeBase
    {
        public ThemeScrollbarXMPBlack()
        {
            BackColor = Color.FromArgb(64, 64, 64);
            BorderColor = Color.FromArgb(17, 17, 17);
            DrawBackground = true;
            DrawBorder = true;

            InnerPaddingWidth = 1;
            MiddleButtonOutterSpace1 = 0;
            MiddleButtonOutterSpace2 = 1;
            SideButtonLength = 14;
            BestUndirectLen = 14;

            SideButtonForePathSize = new Size(7, 7);
            SideButtonForePathGetter = new ButtonForePathGetter(
                GraphicsPathHelper.Create7x4In7x7DownTriangleFlag);

            MiddleButtonColorTable = GetMiddleButtonColorTable();
            SideButtonColorTable = GetSideButtonColorTable();

            DrawLinesInMiddleButton = true;
            MiddleButtonLine1Color = Color.FromArgb(42, 42, 42);
            MiddleButtonLine2Color = Color.FromArgb(95, 95, 95);

            MiddleBtnLineOutterSpace1 = 2;
            MiddleBtnLineOutterSpace2 = 2;
        }

        private ButtonColorTable GetMiddleButtonColorTable()
        {
            ButtonColorTable table = new ButtonColorTable();

            table.BackColorNormal = Color.FromArgb(117, 117, 117);
            table.BackColorHover = Color.FromArgb(129, 129, 129);
            table.BackColorPressed = Color.FromArgb(140, 140, 140);

            return table;
        }

        private ButtonColorTable GetSideButtonColorTable()
        {
            ButtonColorTable table = new ButtonColorTable();

            table.ForeColorNormal = Color.FromArgb(120, 120, 120);
            table.ForeColorHover = Color.FromArgb(140, 140, 140);
            table.ForeColorPressed = Color.FromArgb(160, 160, 160);
            table.ForeColorDisabled = Color.FromArgb(89, 89, 89);

            return table;
        }
    }
}
