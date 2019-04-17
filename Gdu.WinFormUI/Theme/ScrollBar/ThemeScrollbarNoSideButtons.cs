using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    public class ThemeScrollbarNoSideButtons : GMScrollBarThemeBase
    {
        public ThemeScrollbarNoSideButtons()
        {
            BackColor = Color.FromArgb(228, 237, 243);
            DrawBorder = false;
            DrawBackground = true;

            InnerPaddingWidth = 0;
            MiddleButtonOutterSpace1 = 0;
            MiddleButtonOutterSpace2 = 0;

            DrawLinesInMiddleButton = false;
            ShowSideButtons = false;

            MiddleButtonColorTable = GetColorTable();

            BestUndirectLen = 12;
        }

        private ButtonColorTable GetColorTable()
        {
            ButtonColorTable table = new ButtonColorTable();

            table.BackColorNormal = Color.FromArgb(190, 199, 209);
            table.BackColorHover = Color.FromArgb(163, 176, 189);
            table.BackColorPressed = Color.FromArgb(146, 162, 178);
            table.BackColorDisabled = Color.FromArgb(210, 210, 210);

            return table;
        }
    }    
}
