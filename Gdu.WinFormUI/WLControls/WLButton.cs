/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{      
    public class WLButton
    {
        #region private var

        private Control _owner;
        private GMButtonState _state;        
        private EventHandler _click;
        private PaintEventHandler _paint;
        private bool _capture;
        private Rectangle _bounds;

        #endregion

        #region properties

        /// <summary>
        /// 是否在这个按钮上按下了鼠标未释放
        /// </summary>
        public bool Capture
        { get { return _capture; } }

        public Size BtnSize
        {
            get { return Bounds.Size; }
        }

        public Point Location
        {
            get { return Bounds.Location; }
        }

        public ButtonForePathGetter ForePathGetter { get; set; }

        public event EventHandler Click
        {
            add { _click = value; }
            remove { _click = null; }
        }

        public event PaintEventHandler Paint
        {
            add { _paint = value; }
            remove { _paint = null; }
        }

        public GMButtonState State
        {
            get { return _state; }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    _owner.Invalidate(Bounds);
                }
            }
        }

        public bool Visible { get; set; }
        public Rectangle Bounds 
        {
            get { return _bounds; }
            set
            {
                if (_bounds != value)
                {
                    _bounds = value;
                }
            }
        }        
        public Font ForeFont { get; set; }
        public string Text { get; set; }
        public Size ForePathSize { get; set; }

        public ButtonColorTable ColorTable { get; set; }
        public ButtonBorderType BorderType { get; set; }
        public bool DrawLightGlass { get; set; }
        /// <summary>
        /// 画两次可以加深颜色
        /// </summary>
        public bool DrawForePathTwice { get; set; }

        /// <summary>
        /// 用于在click事件中传回数据
        /// </summary>
        public object ClickSendBackOject { get; set; }        

        public Image BackImageNormal { get; set; }
        public Image BackImageHover { get; set; }
        public Image BackImagePressed { get; set; }
        public Image BackImageDisabled { get; set; }

        public bool Enabled { get; set; }
        public ForePathRatoteDirection RotateDirection { get; set; }
        public ForePathRenderMode HowForePathRender { get; set; }

        /// <summary>
        /// 获取或设置是否将绘制完全限制在指定的区域内
        /// </summary>
        public bool RestrictedBounds { get; set; }

        public int Radius { get; set; }
        public RoundStyle RoundedType { get; set; }

        #endregion

        #region private render method

        private void RenderNormal(Graphics g)
        {            
            RenderInternal(g, BackImageNormal, ColorTable.BorderColorNormal, 
                ColorTable.BackColorNormal, ColorTable.ForeColorNormal);            
        }

        private void RenderHover(Graphics g)
        {
            RenderInternal(g, BackImageHover, ColorTable.BorderColorHover, 
                ColorTable.BackColorHover, ColorTable.ForeColorHover);
        }

        private void RenderPressed(Graphics g)
        {            
            RenderInternal(g, BackImagePressed, ColorTable.BorderColorPressed, 
                ColorTable.BackColorPressed, ColorTable.ForeColorPressed);            
        }

        private void RenderDisabled(Graphics g)
        {
            RenderInternal(g, BackImageDisabled, ColorTable.BorderColorDisabled,
                ColorTable.BackColorDisabled, ColorTable.ForeColorDisabled);
        }

        private void RenderInternal(Graphics g, Image backImage, Color borderColor,
            Color backColor, Color foreColor)
        {
            Region oldClip = g.Clip;
            Region newClip = null;

            if (RestrictedBounds)
            {
                newClip = new Region(Bounds);
                g.Clip = newClip;
            }

            if (backImage != null)
            {
                g.DrawImage(backImage, Bounds);
            }
            else
            {
                FillInBackground(g, backColor);                
                RenderForePathAndText(g, foreColor);
                DrawBorder(g, borderColor);
            }

            if (RestrictedBounds)
            {
                g.Clip = oldClip;                
                newClip.Dispose();
            }
        }

        private void FillInBackground(Graphics g, Color backColor)
        {
            BasicBlockPainter.RenderFlatBackground(
                g,
                Bounds,
                backColor,
                BorderType,
                Radius,
                RoundedType);
            
            if (BorderType == ButtonBorderType.Ellipse && DrawLightGlass)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    Rectangle rect = Bounds;
                    rect.Height--;
                    rect.Width--;        
                    gp.AddEllipse(rect);
                    using (PathGradientBrush pb = new PathGradientBrush(gp))
                    {
                        pb.CenterPoint = new PointF(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 4.0f);
                        pb.CenterColor = Color.FromArgb(180, Color.White);
                        pb.SurroundColors = new Color[] { Color.FromArgb(40, Color.White) };
                        g.FillPath(pb, gp);
                    }
                }
            }            
        }

        private void DrawBorder(Graphics g, Color borderColor)
        {
            BasicBlockPainter.RenderBorder(
                g,
                Bounds,
                borderColor,
                BorderType,
                Radius,
                RoundedType);            
        }

        private void RenderForePathAndText(Graphics g, Color foreColor)
        {
            if (ForePathGetter != null)
            {
                if(string.IsNullOrEmpty(Text))
                    PathOnly(g,foreColor);
                else
                    PathAndText(g,foreColor);
            }
            else if(!string.IsNullOrEmpty(Text))
                TextOnly(g,foreColor);
        }

        private void PathOnly(Graphics g, Color foreColor)
        {
            using (GraphicsPath path = ForePathGetter(Bounds))
            {                
                PathWithRotate(g, path, foreColor, Bounds);
                if (DrawForePathTwice)                        
                    PathWithRotate(g, path, foreColor, Bounds);                
            }
        }

        private void TextOnly(Graphics g, Color foreColor)
        {
            Size size = TextRenderer.MeasureText(Text, ForeFont);
            int x = Bounds.Left + (Bounds.Width - size.Width) / 2;
            int y = Bounds.Top + (Bounds.Height - size.Height) / 2;
            Rectangle textRect = new Rectangle(new Point(x, y), size);
            TextRenderer.DrawText(
                g,
                Text,
                ForeFont,
                textRect,
                foreColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter);
        }

        private void PathAndText(Graphics g, Color foreColor)
        {
            Size size = TextRenderer.MeasureText(Text, ForeFont);            
            Rectangle rect = Bounds;
            rect.Offset(-size.Width / 2 + 2, 0);
            using (GraphicsPath path = ForePathGetter(rect))
            {
                PathWithRotate(g, path, foreColor, rect);                
                int x = rect.Left + rect.Width / 2 + ForePathSize.Width / 2 + 1;
                int y = rect.Top + (rect.Height - size.Height) / 2;
                Rectangle textRect = new Rectangle(new Point(x, y), size);
                TextRenderer.DrawText(
                    g,
                    Text,
                    ForeFont,
                    textRect,
                    foreColor,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter);
            }
        }

        private void PathWithRotate(Graphics g, GraphicsPath path, Color foreColor, Rectangle pathRect)
        {
            Pen p = new Pen(foreColor);
            SolidBrush sb = new SolidBrush(foreColor);

            if (RotateDirection == ForePathRatoteDirection.Down)
            {
                if (HowForePathRender == ForePathRenderMode.Draw)
                    g.DrawPath(p, path);
                else
                    g.FillPath(sb, path);
            }
            else
            {
                int dx = 0, dy = 0, angle = 0;
                switch (RotateDirection)
                {
                    case ForePathRatoteDirection.Left:
                        dx = ForePathSize.Width - 1;
                        dy = 0;
                        angle = 90;
                        break;
                    case ForePathRatoteDirection.Up:
                        dx = ForePathSize.Width - 1;
                        dy = ForePathSize.Height - 1;
                        angle = 180;
                        break;
                    case ForePathRatoteDirection.Right:
                        dx = 0;
                        dy = ForePathSize.Height - 1;
                        angle = 270;
                        break;
                }
                int pathX = pathRect.Left + (pathRect.Width - ForePathSize.Width) / 2;
                int pathY = pathRect.Top + (pathRect.Height - ForePathSize.Height) / 2;
                g.TranslateTransform(pathX, pathY);
                g.TranslateTransform(dx, dy);
                g.RotateTransform(angle);
                using (GraphicsPath newPath = ForePathGetter(new Rectangle(Point.Empty, ForePathSize)))
                {
                    if (HowForePathRender == ForePathRenderMode.Draw)
                        g.DrawPath(p, newPath);
                    else
                        g.FillPath(sb, newPath);
                }
                g.ResetTransform();
            }
            p.Dispose();
            sb.Dispose();
        }

        #region mouse operation

        private void MouseDown(Point location)
        {
            if (Bounds.Contains(location))
            {
                State = GMButtonState.Pressed;
                _capture = true;
            }
            else
            {
                _capture = false;
            }
        }

        private void MouseMove(Point location)
        {
            if (Bounds.Contains(location))
            {                
                if (State == GMButtonState.Normal)
                {
                    // 没有在窗体其他地方按下按钮
                    if (!_owner.Capture) 
                    {
                        State = GMButtonState.Hover;
                    }
                }
                else if (State == GMButtonState.PressLeave)
                {
                    State = GMButtonState.Pressed;
                }
                
            }
            else
            {
                if (_capture)
                {
                    State = GMButtonState.PressLeave;
                }
                else
                {
                    State = GMButtonState.Normal;
                }
            }
        }

        private void MouseLeave(Point location)
        {
            State = GMButtonState.Normal;
            _capture = false;
        }

        private void MouseUp(Point location)
        {
            
            if (Bounds.Contains(location))
            {
                State = GMButtonState.Hover;
                if (_capture)
                    OnClick(EventArgs.Empty);                
            }
            else
            {
                State = GMButtonState.Normal;
            }
            _capture = false;
        }

        #endregion

        #endregion

        #region event

        protected virtual void OnClick(EventArgs e)
        {
            if (_click != null)
            {
                object obj = (ClickSendBackOject != null) ? ClickSendBackOject : this;
                _click(obj, e);
            }
        }

        protected virtual void OnPaint(PaintEventArgs e)
        {
            if (_paint != null)
            {
                _paint(this, e);
            }
        }

        #endregion

        #region public method

        public WLButton(Control owner)
        {
            _owner = owner;
            _state = GMButtonState.Normal;
            Visible = true;
            Enabled = true;
            BorderType = ButtonBorderType.Rectangle;
            DrawLightGlass = false;
            RotateDirection = ForePathRatoteDirection.Down;
            HowForePathRender = ForePathRenderMode.Draw;
            RestrictedBounds = true;
            Radius = 0;
            RoundedType = RoundStyle.None;
        }

        public void DrawButton(Graphics g)
        {
            if (!Visible)
                return;

            if (Enabled)
            {
                switch (State)
                {
                    case GMButtonState.Hover:
                        RenderHover(g);
                        break;
                    case GMButtonState.Pressed:
                        RenderPressed(g);
                        break;
                    default:
                        RenderNormal(g);
                        break;
                }
            }
            else
            {
                RenderDisabled(g);
            }

            OnPaint(new PaintEventArgs(g, Bounds));
        }

        public void MouseOperation(Point location, MouseOperationType type)
        {
            if (!Visible)
                return;

            switch (type)
            {
                case MouseOperationType.Move:
                    MouseMove(location);
                    break;

                case MouseOperationType.Down:
                    MouseDown(location);
                    break;

                case MouseOperationType.Up:
                    MouseUp(location);
                    break;

                case MouseOperationType.Leave:
                    MouseLeave(location);
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
