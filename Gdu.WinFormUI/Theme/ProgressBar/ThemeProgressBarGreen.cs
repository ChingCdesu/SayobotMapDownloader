using System;
using System.Drawing;

namespace Gdu.WinFormUI
{
    public class ThemeProgressBarGreen : GMProgessBarThemeBase
    {
        public ThemeProgressBarGreen()
        {
            DrawInnerBorder = false;
            DrawCoveredColorGlass = true;
            DrawLeadingEdge = true;
            DrawBackColorGlass = true;

            CoveredColor = Color.FromArgb(176, 229, 124);
            BorderColor = Color.FromArgb(165, 178, 152);
            LeadingEdgeColor = Color.FromArgb(188, 203, 173);
            BackColor = Color.FromArgb(211, 220, 226);

            BorderRadius = 2;
        }
    }
}
