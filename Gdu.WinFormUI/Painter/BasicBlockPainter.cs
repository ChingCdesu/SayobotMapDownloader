/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    public class BasicBlockPainter
    {

        #region static method

        public static void RenderFlatBackground(Graphics g, Rectangle rect, Color backColor,
            ButtonBorderType borderType, int radius, RoundStyle roundType)
        {
            SmoothingMode newMode;
            bool simpleRect = (borderType == ButtonBorderType.Rectangle && (roundType == RoundStyle.None || radius < 2));
            if (simpleRect)
            {
                newMode = SmoothingMode.HighSpeed;
            }
            else
            {
                newMode = SmoothingMode.AntiAlias;
                rect.Width--;
                rect.Height--;
            }
            using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, newMode))
            {
                using (SolidBrush sb = new SolidBrush(backColor))
                {
                    if (simpleRect)
                    {
                        g.FillRectangle(sb, rect);
                    }
                    else if (borderType == ButtonBorderType.Ellipse)
                    {
                        g.FillEllipse(sb, rect);
                    }
                    else
                    {
                        using (GraphicsPath path = GraphicsPathHelper.CreateRoundedRect(rect, radius, roundType, false))
                        {
                            g.FillPath(sb, path);
                        }
                    }
                }
            }
        }

        public static void RenderBorder(Graphics g, Rectangle rect, Color borderColor,
            ButtonBorderType borderType, int radius, RoundStyle roundType)
        {
            rect.Width--;
            rect.Height--;
            
            bool simpleRect = (borderType == ButtonBorderType.Rectangle && (roundType == RoundStyle.None || radius < 2));
            SmoothingMode newMode = simpleRect ? SmoothingMode.HighSpeed : SmoothingMode.AntiAlias;

            using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, newMode))
            {
                using (Pen p = new Pen(borderColor))
                {
                    if (simpleRect)
                    {
                        g.DrawRectangle(p, rect);
                    }
                    else if (borderType == ButtonBorderType.Ellipse)
                    {
                        g.DrawEllipse(p, rect);
                    }
                    else
                    {
                        using (GraphicsPath path = GraphicsPathHelper.CreateRoundedRect(rect, radius, roundType, false))
                        {
                            g.DrawPath(p, path);
                        }
                    }
                }
            }
        }

        #region rectangle glass

        public static void RenderRectangleGlass(Graphics g, Rectangle ownerRect,
            int ownerRadius, RoundStyle ownerRoundTye, RectangleGlassPosition position,
            float angle, float glassLengthFactor, Color glassColor, int alpha1, int alpha2)
        {
            if (!(glassLengthFactor > 0 && glassLengthFactor < 1))
                throw new ArgumentException("glassLengthFactor must be between 0 and 1, but not include 0 and 1. ", 
                    "glassLengthFactor");

            Rectangle rect = CalcGlassRect(ownerRect, position, glassLengthFactor);
            RoundStyle round = CalcRoundStyle(position, ownerRadius, ownerRoundTye);

            //if (angle == 0 || angle == 90 || angle == 180 || angle == 270)
            //    angle++;

            bool simpleRect = (round == RoundStyle.None);
            SmoothingMode newMode;
            if (simpleRect)
            {
                newMode = SmoothingMode.HighSpeed;
            }
            else
            {
                newMode = SmoothingMode.AntiAlias;
                rect.Width--;
                rect.Height--;
            }

            if (rect.Width < 1 || rect.Height < 1)
                return;

            using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, newMode))
            {
                using (LinearGradientBrush lb = new LinearGradientBrush(rect,
                    Color.FromArgb(alpha1, glassColor), Color.FromArgb(alpha2, glassColor), angle))
                {
                    if (simpleRect)
                    {
                        g.FillRectangle(lb, rect);
                    }
                    else
                    {
                        using (GraphicsPath path = GraphicsPathHelper.CreateRoundedRect(rect,
                            ownerRadius, round, false))
                        {
                            g.FillPath(lb, path);
                        }
                    }
                }
            }
        }

        private static Rectangle CalcGlassRect(Rectangle ownerRect, RectangleGlassPosition pos, float factor)
        {
            Point location = ownerRect.Location;
            int width = ownerRect.Width;
            int height = ownerRect.Height;

            switch (pos)
            {
                case RectangleGlassPosition.Fill:
                    break;
                case RectangleGlassPosition.Top:
                    height = (int)(ownerRect.Height * factor);
                    break;
                case RectangleGlassPosition.Bottom:
                    height = (int)(ownerRect.Height * factor);
                    location.Offset(0, ownerRect.Height - height);
                    break;
                case RectangleGlassPosition.Left:
                    width = (int)(ownerRect.Width * factor);
                    break;
                case RectangleGlassPosition.Right:
                    width = (int)(ownerRect.Width * factor);
                    location.Offset(ownerRect.Width - width, 0);
                    break;
            }
            return new Rectangle(location, new Size(width, height));
        }

        private static RoundStyle CalcRoundStyle(RectangleGlassPosition pos, int radius, RoundStyle ownerStyle)
        {            
            if (radius < 2 || ownerStyle == RoundStyle.None)
                return RoundStyle.None;
            switch (pos)
            {
                case RectangleGlassPosition.Fill:
                    return ownerStyle;
                case RectangleGlassPosition.Top:
                    if (ownerStyle == RoundStyle.All || ownerStyle == RoundStyle.Top)
                        return RoundStyle.Top;
                    else
                        return RoundStyle.None;
                case RectangleGlassPosition.Bottom:
                    if (ownerStyle == RoundStyle.All || ownerStyle == RoundStyle.Bottom)
                        return RoundStyle.Bottom;
                    else
                        return RoundStyle.None;
                case RectangleGlassPosition.Left:
                    if (ownerStyle == RoundStyle.All || ownerStyle == RoundStyle.Left)
                        return RoundStyle.Left;
                    else
                        return RoundStyle.None;
                case RectangleGlassPosition.Right:
                    if (ownerStyle == RoundStyle.All || ownerStyle == RoundStyle.Right)
                        return RoundStyle.Right;
                    else
                        return RoundStyle.None;
                default:
                    return RoundStyle.None;
            }
        }

        public static void RenderRectangleGlass(Graphics g, Rectangle ownerRect, int ownerRadius, RoundStyle ownerRoundTye)
        {
            RenderRectangleGlass(g, ownerRect, ownerRadius, ownerRoundTye, RectangleGlassPosition.Top, 90f);
        }

        public static void RenderRectangleGlass(Graphics g, Rectangle ownerRect, int ownerRadius, RoundStyle ownerRoundTye,
            RectangleGlassPosition position, float angle)
        {
            RenderRectangleGlass(g, ownerRect, ownerRadius, ownerRoundTye, position, angle, 0.5f);
        }

        public static void RenderRectangleGlass(Graphics g, Rectangle ownerRect, int ownerRadius, RoundStyle ownerRoundTye,
            RectangleGlassPosition position, float angle, float glassLengthFactor)
        {
            RenderRectangleGlass(g, ownerRect, ownerRadius, ownerRoundTye, 
                position, angle, glassLengthFactor, Color.White, 220, 60);
        }

        #endregion

        #endregion

    }
}
