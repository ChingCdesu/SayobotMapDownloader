/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Text;
using System.Drawing;

namespace Gdu.WinFormUI.MyGraphics
{
    public static class ColorHelper
    {
        public static Color GetLighterColor(Color baseColor, int percentage)
        {
            int r0 = baseColor.R;
            int g0 = baseColor.G;
            int b0 = baseColor.B;

            int r = r0 + (int)((255 - r0) * (percentage / 100f));
            int g = g0 + (int)((255 - g0) * (percentage / 100f));
            int b = b0 + (int)((255 - b0) * (percentage / 100f));

            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;

            return Color.FromArgb(baseColor.A, r, g, b);
        }

        public static Color GetDarkerColor(Color baseColor, int percentage)
        {
            int r0 = baseColor.R;
            int g0 = baseColor.G;
            int b0 = baseColor.B;

            int r = r0 - (int)(r0 * (percentage / 100f));
            int g = g0 - (int)(g0 * (percentage / 100f));
            int b = b0 - (int)(b0 * (percentage / 100f));

            if (r < 0)
                r = 0;
            if (g < 0)
                g = 0;
            if (b < 0)
                b = 0;

            return Color.FromArgb(baseColor.A, r, g, b);
        }

        public static Color[] GetLighterArrayColors(Color baseColor, int arrayLength, float maxPercentage)
        {
            if (maxPercentage < 2)
                maxPercentage = 2f;
            if (maxPercentage > 100)
                maxPercentage = 100f;

            Color[] arrc = new Color[arrayLength];
            float average = maxPercentage / arrayLength;
            for (int i = 0; i < arrayLength; i++)
            {
                arrc[arrayLength - i - 1] = GetLighterColor(baseColor, (int)(average * i));
            }
            return arrc;
        }

        public static Color[] GetLighterArrayColors(Color baseColor, int arrayLength)
        {
            return GetLighterArrayColors(baseColor, arrayLength, 100f);
        }
    }
}
