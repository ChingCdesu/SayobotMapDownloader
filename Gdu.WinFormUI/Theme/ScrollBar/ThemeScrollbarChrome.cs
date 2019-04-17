using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    public class ThemeScrollbarChrome : GMScrollBarThemeBase
    {
        public ThemeScrollbarChrome()
        {
            BackColor = Color.FromArgb(241, 241, 241);
            DrawBorder = false;
            DrawBackground = true;

            InnerPaddingWidth = 0;
            MiddleButtonOutterSpace1 = 0;
            MiddleButtonOutterSpace2 = 2;

            ShowSideButtons = true;
            //SideButtonCanDisabled = true;
            SideButtonLength = 17;
            BestUndirectLen = 17;

            SideButtonColorTable = GetSideButtonColorTable();
            MiddleButtonColorTable = GetMiddleButtonColorTable();

            SideButtonForePathSize = new Size(7, 7);
            SideButtonForePathGetter = new ButtonForePathGetter(
                GraphicsPathHelper.Create7x4In7x7DownTriangleFlag);
            HowSideButtonForePathDraw = ForePathRenderMode.Draw;

            DrawLinesInMiddleButton = false;
        }        

        private ButtonColorTable GetSideButtonColorTable()
        {
            ButtonColorTable table = new ButtonColorTable();

            table.ForeColorHover = table.ForeColorNormal = Color.FromArgb(80, 80, 80);
            table.ForeColorPressed = Color.White;
            table.ForeColorDisabled = Color.FromArgb(163, 163, 163);

            table.BackColorHover = Color.FromArgb(210, 210, 210);
            table.BackColorPressed = Color.FromArgb(120, 120, 120);

            return table;
        }

        private ButtonColorTable GetMiddleButtonColorTable()
        {
            ButtonColorTable table = new ButtonColorTable();

            table.BorderColorNormal = Color.FromArgb(168, 168, 168);
            table.BorderColorHover = Color.FromArgb(154, 154, 154);
            table.BorderColorPressed = Color.FromArgb(120, 120, 120);

            table.BackColorNormal = Color.FromArgb(188, 188, 188);
            table.BackColorHover = Color.FromArgb(170, 170, 170);
            table.BackColorPressed = Color.FromArgb(141, 141, 141);

            return table;
        }
    }
}
