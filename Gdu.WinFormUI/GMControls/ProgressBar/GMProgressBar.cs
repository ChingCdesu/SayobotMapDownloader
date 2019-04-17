/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    [ToolboxItem(true)]
    public class GMProgressBar : GMBarControlBase, IGMControl
    {

        #region 构造函数及初始化

        public GMProgressBar()
        {
            
        }        

        #endregion

        #region private var

        string text = "0%";

        #endregion

        #region IGMControl实现

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public GMControlType ControlType
        {
            get { return GMControlType.ProgressBar; }
        }

        #endregion

        #region 新增的公开属性

        int _percentage = 0;
        bool _showPercentageText = true;
        GMProgessBarThemeBase _xtheme;
        ProgressBarShapeStyle _shap = ProgressBarShapeStyle.Rectangle;

        [DefaultValue(0), Description("0-100之间的整数值，表示当前进度")]
        public int Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                if (_percentage != value)
                {
                    if (value < 0)
                        value = 0;
                    if (value > 100)
                        value = 100;
                    _percentage = value;
                    text = _percentage.ToString() + "%";
                    Invalidate();
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowPercentageText
        {
            get
            {
                return _showPercentageText;
            }
            set
            {
                if (_showPercentageText != value)
                {
                    _showPercentageText = value;
                    Invalidate();
                }
            }
        }

        [Description("表示进度条是长方形的还是圆形的"),DefaultValue(typeof(ProgressBarShapeStyle),"0")]
        public ProgressBarShapeStyle Shape
        {
            get
            {
                return _shap;
            }
            set
            {
                if (_shap != value)
                {
                    _shap = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public GMProgessBarThemeBase XTheme
        {
            get
            {
                return _xtheme;
            }
            set
            {
                _xtheme = value;
                Invalidate();
            }
        }

        #endregion

        #region 可用XTheme配置的属性

        protected virtual Color GMBackColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.White;
                else
                    return _xtheme.BackColor;
            }
        }

        protected virtual Color InnerBorderColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.White;
                else
                    return _xtheme.InnerBorderColor;
            }
        }

        protected virtual Color BorderColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.FromArgb(171, 171, 171);
                else
                    return _xtheme.BorderColor;
            }
        }

        protected virtual Color CoveredColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.FromArgb(0, 114, 198);
                else
                    return _xtheme.CoveredColor;
            }
        }

        protected virtual Color LeadingEdgeColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.Empty;
                else
                    return _xtheme.LeadingEdgeColor;
            }
        }

        protected virtual bool DrawInnerBorder
        {
            get
            {
                if (_xtheme == null)
                    return true;
                else
                    return _xtheme.DrawInnerBorder;
            }
        }

        protected virtual bool DrawLeadingEdge
        {
            get
            {
                if (_xtheme == null)
                    return false;
                else
                    return _xtheme.DrawLeadingEdge;
            }
        }

        protected virtual bool DrawBackColorGlass
        {
            get
            {
                if (_xtheme == null)
                    return false;
                else
                    return _xtheme.DrawBackColorGlass;
            }
        }

        protected virtual bool DrawCoveredColorGlass
        {
            get
            {
                if (_xtheme == null)
                    return false;
                else
                    return _xtheme.DrawCoveredColorGlass;
            }
        }

        protected virtual int BorderRadius
        {
            get
            {
                if (_xtheme == null)
                    return 0;
                else
                    return _xtheme.BorderRadius;
            }
        }

        protected virtual Color GMForeColor
        {
            get
            {
                if (_xtheme == null)
                    return base.ForeColor;
                else
                    return _xtheme.ForeColor;
            }
        }

        protected virtual Font ForeFont
        {
            get
            {
                if (_xtheme == null)
                    return base.Font;
                else
                    return _xtheme.ForeFont;
            }
        }

        #endregion

        #region 计算出的各元素区域

        private Rectangle CoveredRect
        {
            get
            {
                Point location = ClientRectangle.Location;
                int maxWidth, height, width;
                if (DrawInnerBorder)
                {
                    maxWidth = base.Width - 4;
                    height = base.Height - 4;
                    location.Offset(2, 2);
                }
                else
                {
                    maxWidth = base.Width - 2;
                    height = base.Height - 2;
                    location.Offset(1, 1);
                }
                width = (int)((float)maxWidth * (float)Percentage / 100f);
                return new Rectangle(location, new Size(width, height));
            }
        }

        #endregion

        #region 内部绘图

        protected virtual void PaintThisBar(Graphics g)
        {                        
            BasicBlockPainter.RenderFlatBackground(g, ClientRectangle, GMBackColor, 
                ButtonBorderType.Rectangle, BorderRadius, RoundStyle.All);
            if (DrawBackColorGlass)
                BasicBlockPainter.RenderRectangleGlass(g, ClientRectangle, BorderRadius, RoundStyle.All,
                    RectangleGlassPosition.Bottom, 270f, 0.5f, Color.White, 100, 20);           
            Rectangle rectCover = CoveredRect;
            if (rectCover.Width > 0)
            {
                rectCover.Inflate(1, 1);
                BasicBlockPainter.RenderFlatBackground(g, rectCover, CoveredColor,
                    ButtonBorderType.Rectangle, BorderRadius, RoundStyle.All);
                if (DrawCoveredColorGlass)
                    BasicBlockPainter.RenderRectangleGlass(g, rectCover, BorderRadius, RoundStyle.All,
                        RectangleGlassPosition.Top, 90.001f);
            }            
            if (DrawLeadingEdge && Percentage != 100)
            {
                rectCover.Inflate(-1, -1);
                Point p1 = new Point(rectCover.Right - 1 + 1, rectCover.Y);
                Point p2 = new Point(rectCover.Right - 1 + 1, rectCover.Bottom - 1);
                using (Pen p = new Pen(LeadingEdgeColor))
                {
                    g.DrawLine(p, p1, p2);
                }
            }                        
            if (ShowPercentageText)
            {
                TextRenderer.DrawText(g, text, ForeFont, ClientRectangle, ForeColor,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);
            }            
            Rectangle rectBorder = ClientRectangle;
            BasicBlockPainter.RenderBorder(g, rectBorder, BorderColor, ButtonBorderType.Rectangle,
                BorderRadius, RoundStyle.All);
            if (DrawInnerBorder)
            {
                rectBorder.Inflate(-1, -1);
                BasicBlockPainter.RenderBorder(g, rectBorder, InnerBorderColor, ButtonBorderType.Rectangle,
                BorderRadius, RoundStyle.All);
            }
        }

        protected virtual void PaintThisBarInCircleShape(Graphics g)
        {
            Rectangle rect;

            if (base.Width < base.Height)
            {
                rect = new Rectangle(0, (Height - Width) / 2, Width, Width);
            }
            else
            {
                rect = new Rectangle((Width - Height) / 2, 0, Height, Height);
            }
            
            g.FillRectangle(Brushes.Transparent, ClientRectangle);

            SimpleObjectPainter.RenderCircleProgressBar(
                g,
                rect,
                CoveredColor,
                BorderColor,
                GMBackColor,
                DrawInnerBorder,
                270,
                Percentage,
                ShowPercentageText,
                ForeFont);
        }

        #endregion

        #region 重写基类方法

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Shape == ProgressBarShapeStyle.Circle)
            {
                PaintThisBarInCircleShape(e.Graphics);
            }
            else
            {
                PaintThisBar(e.Graphics);
            }
        }

        [Browsable(true)]
        public override System.Drawing.Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        [Browsable(true)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        #endregion

    }
}
