/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    [ToolboxItem(true), DefaultEvent("ValueChanged")]
    public class GMTrackBar : GMBarControlBase, IGMControl
    {

        #region 构造函数及初始化

        static GMTrackBar()
        {
            EVENT_VALUECHANGED = new object();
            EVENT_PAINTBUTTON = new object();
            EVENT_PAINTMAINLINE = new object();
        }

        public GMTrackBar()
        {            
            ThumeButtonIni();
            presenter = new CutePointAndValuePresenter();
            base.TabStop = true;
        }        

        private void ThumeButtonIni()
        {
            thumbButton = new WLButton(this);
            thumbButton.RestrictedBounds = false;
            SetThumbButtonInfo();
        }

        private void SetThumbButtonInfo()
        {
            thumbButton.ColorTable = ThumbButtonColorTable;
            thumbButton.BorderType = ThumbButtonBorderType;
        }

        #endregion

        #region 控件事件相关

        private static readonly object EVENT_VALUECHANGED;
        private static readonly object EVENT_PAINTMAINLINE;
        private static readonly object EVENT_PAINTBUTTON;

        /// <summary>
        /// 控件Value值发生变化后引发
        /// </summary>
        [Description("控件Value值发生变化后引发")]
        public event EventHandler ValueChanged
        {
            add
            {
                base.Events.AddHandler(EVENT_VALUECHANGED, value);
            }
            remove
            {
                base.Events.RemoveHandler(EVENT_VALUECHANGED, value);
            }
        }

        /// <summary>
        /// 控件绘制MainLine前引发, 可以设置Cancel参数来取消默认的绘制
        /// </summary>
        [Description("控件绘制MainLine前引发, 可以设置Cancel参数来取消默认的绘制, 请把参数转化为 GMTrackBarPaintEventArgs")]
        public event PaintEventHandler BeforePaintMainLine
        {
            add
            {
                base.Events.AddHandler(EVENT_PAINTMAINLINE, value);
            }
            remove
            {
                base.Events.RemoveHandler(EVENT_PAINTMAINLINE, value);
            }
        }

        /// <summary>
        /// 控件绘制Button前引发
        /// </summary>
        [Description("控件绘制ThumbButton前引发, 可以设置Cancel参数来取消默认的绘制, 请把参数转化为 GMTrackBarPaintEventArgs")]
        public event PaintEventHandler BeforePaintButton
        {
            add
            {
                base.Events.AddHandler(EVENT_PAINTBUTTON, value);
            }
            remove
            {
                base.Events.RemoveHandler(EVENT_PAINTBUTTON, value);
            }
        }


        /// <summary>
        /// 引发 ValueChanged 事件
        /// </summary>
        protected virtual void OnValueChanged(EventArgs e)
        {
            EventHandler handle = (EventHandler)base.Events[EVENT_VALUECHANGED];
            if (handle != null)
            {
                handle(this, e);
            }
        }

        /// <summary>
        /// 引发 BeforePaintMainLine 事件
        /// </summary>
        protected virtual void OnBeforePaintMainLine(PaintEventArgs e)
        {
            PaintEventHandler handle = (PaintEventHandler)base.Events[EVENT_PAINTMAINLINE];
            if (handle != null)
            {
                handle(this, e);
            }
        }

        /// <summary>
        /// 引发 BeforePaintButton 事件
        /// </summary>
        protected virtual void OnBeforePaintButton(PaintEventArgs e)
        {
            PaintEventHandler handle = (PaintEventHandler)base.Events[EVENT_PAINTBUTTON];
            if (handle != null)
            {
                handle(this, e);
            }
        }

        #endregion

        #region 公开的属性

        int _value = 0;
        int _minimum = 0;
        int _maximum = 10;
        int _smallChange = 1;
        int _largeChange = 5;
        int _tickFrequency = 1;
        TickStyle _tickSide = TickStyle.None;
        Orientation _barOrientation = Orientation.Horizontal;
        GMTrackBarThemeBase _xtheme = null;
        bool _drawFocusRect = false;
        bool _autoSize = true;

        [DefaultValue(0)]
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    if (value > _maximum)
                        value = _maximum;
                    if (value < _minimum)
                        value = _minimum;

                    _value = value;
                    ResetButtonPositionFromValue();
                    OnValueChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(0)]
        public int Minimum
        {
            get
            {
                return _minimum;
            }
            set
            {
                if (_minimum != value)
                {
                    if (value > _maximum)
                        _maximum = value;
                    if (_value < value)
                        _value = value;
                    _minimum = value;
                    UpdatePresenterAndRedraw();
                }
            }
        }

        [DefaultValue(10)]
        public int Maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                if (_maximum != value)
                {
                    if (value < _minimum)
                        _minimum = value;
                    if (_value > value)
                        _value = value;
                    _maximum = value;
                    UpdatePresenterAndRedraw();
                }
            }
        }

        [DefaultValue(1)]
        public int SmallChange
        {
            get
            {
                return _smallChange;
            }
            set
            {
                _smallChange = value < 1 ? 1 : value;
            }
        }

        [DefaultValue(5)]
        public int LargeChange
        {
            get
            {
                return _largeChange;
            }
            set
            {
                _largeChange = value < 1 ? 1 : value;
            }
        }

        [DefaultValue(1)]
        public int TickFrequency
        {
            get
            {
                return _tickFrequency;
            }
            set
            {
                if (_tickFrequency != value)
                {
                    _tickFrequency = value < 1 ? 1 : value;
                    if (_tickSide != TickStyle.None)
                        Invalidate();
                }
            }
        }

        [DefaultValue(0)]
        public TickStyle TickSide
        {
            get
            {
                return _tickSide;
            }
            set
            {
                if (_tickSide != value)
                {
                    _tickSide = value;
                    DoWhenResize();                    
                    Invalidate();
                }
            }
        }

        [DefaultValue(0)]
        public Orientation BarOrientation
        {
            get
            {
                return _barOrientation;
            }
            set
            {
                if (_barOrientation != value)
                {
                    _barOrientation = value;
                    DoWhenBarOrientationChanged();
                }
            }
        }

        [DefaultValue(true), Browsable(true)]
        public override bool AutoSize
        {
            get
            {
                return _autoSize;
            }
            set
            {
                if (_autoSize != value)
                {
                    _autoSize = value;

                    if (_barOrientation == Orientation.Horizontal)
                    {
                        base.SetStyle(ControlStyles.FixedHeight, _autoSize);
                        base.SetStyle(ControlStyles.FixedWidth, false);
                    }
                    else
                    {
                        base.SetStyle(ControlStyles.FixedWidth, _autoSize);
                        base.SetStyle(ControlStyles.FixedHeight, false);
                    }

                    DoWhenAutoSizeChanged();
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public GMTrackBarThemeBase XTheme
        {
            get
            {
                return _xtheme;
            }
            set
            {
                if (_xtheme != value)
                {
                    _xtheme = value;
                    DoWhenXThemeChanged();
                }
            }
        }

        [DefaultValue(false)]
        public bool DrawFocusRect
        {
            get
            {
                return _drawFocusRect;
            }
            set
            {
                _drawFocusRect = value;
                Invalidate();
            }
        }

        #endregion

        #region 公开的方法

        /// <summary>
        /// 将Value值加一个LargeChange量
        /// </summary>
        public void DoLargeUp()
        {
            ValueAdd(LargeChange);
        }

        /// <summary>
        /// 将Value值减一个LargeChange量
        /// </summary>
        public void DoLargeDown()
        {
            ValueAdd(-LargeChange);
        }

        /// <summary>
        /// 将Value值加一个SmallChange量
        /// </summary>
        public void DoSmallUp()
        {
            ValueAdd(SmallChange);
        }

        /// <summary>
        /// 将Value值减一个SmallChange量
        /// </summary>
        public void DoSmallDown()
        {
            ValueAdd(-SmallChange);
        }

        #endregion

        #region 可通过XTheme配置的内部 look and feel 属性

        protected virtual int ButtonLength1
        {
            get
            {
                if (_xtheme == null)
                    return 8;
                else
                    return _xtheme.ButtonLength1;
            }
        }

        protected virtual int ButtonLength2
        {
            get
            {
                if (_xtheme == null)
                    return 18;
                else
                    return _xtheme.ButtonLength2;
            }
        }

        protected virtual int ButtonOutterSpace1
        {
            get
            {
                if (_xtheme == null)
                    return 4;
                else
                    return _xtheme.ButtonOutterSpace1;
            }
        }

        protected virtual int ButtonOutterSpace2
        {
            get
            {
                if (_xtheme == null)
                    return 2;
                else
                    return _xtheme.ButtonOutterSpace2;
            }
        }

        protected virtual int MainLineLength
        {
            get
            {
                if (_xtheme == null)
                    return 4;
                else
                    return _xtheme.MainLineLength;
            }
        }

        protected virtual int TickLineLength
        {
            get
            {
                if (_xtheme == null)
                    return 3;
                else
                    return _xtheme.TickLineLength;
            }
        }

        protected virtual int TickLineSpaceWithButton
        {
            get
            {
                if (_xtheme == null)
                    return 2;
                else
                    return _xtheme.TickLineSpaceWithButton;
            }
        }

        protected virtual int TickLineSpaceWithBorder
        {
            get
            {
                if (_xtheme == null)
                    return 6;
                else
                    return _xtheme.TickLineSpaceWithBorder;
            }
        }

        protected virtual int BorderWidth
        {
            get
            {
                if (_xtheme == null)
                    return 1;
                else
                    return _xtheme.BorderWidth;
            }
        }

        protected virtual bool DrawBackground
        {
            get
            {
                if (_xtheme == null)
                    return true;
                else
                    return _xtheme.DrawBackground;
            }
        }

        protected virtual bool DrawBorder
        {
            get
            {
                if (_xtheme == null)
                    return false;
                else
                    return _xtheme.DrawBorder;
            }
        }

        protected virtual bool DrawInnerBorder
        {
            get
            {
                if (_xtheme == null)
                    return false;
                else
                    return _xtheme.DrawInnerBorder;
            }
        }

        protected virtual Color GMBackColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.Transparent;
                else
                    return _xtheme.BackColor;
            }
        }

        protected virtual Color BorderColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.LightGray;
                else
                    return _xtheme.BorderColor;
            }
        }

        protected virtual Color InnerBorderColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.Empty;
                else
                    return _xtheme.InnerBorderColor;
            }
        }        

        protected virtual Color TickLineColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.FromArgb(185,185,185);
                else
                    return _xtheme.TickLineColor;
            }
        }

        protected virtual Color MainLineRange1BackColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.LightGray;
                else
                    return _xtheme.MainLineRange1BackColor;
            }
        }

        protected virtual Color MainLineRange2BackColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.White;
                else
                    return _xtheme.MainLineRange2BackColor;
            }
        }

        protected virtual Color MainLineBorderColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.FromArgb(0,114,198);
                else
                    return _xtheme.MainLineBorderColor;
            }
        }

        protected virtual bool MainLineDrawBorder
        {
            get
            {
                if (_xtheme == null)
                    return true;
                else
                    return _xtheme.MainLineDrawBorder;
            }
        }

        protected virtual int MainLineRadius
        {
            get
            {
                if (_xtheme == null)
                    return 0;
                else
                    return _xtheme.MainLineRadius;
            }
        }

        protected virtual ButtonColorTable ThumbButtonColorTable
        {
            get
            {
                if (_xtheme == null)
                    return DefaultThumbColor();
                else
                    return _xtheme.ThumbButtonColorTable;
            }
        }

        protected virtual ButtonBorderType ThumbButtonBorderType
        {
            get
            {
                if (_xtheme == null)
                    return ButtonBorderType.Rectangle;
                else
                    return _xtheme.ThumbButtonBorderType;
            }
        }       

        protected virtual ButtonColorTable DefaultThumbColor()
        {
            ButtonColorTable table = new ButtonColorTable();
            table.BorderColorNormal = table.BorderColorHover = table.BorderColorPressed =
                Color.FromArgb(0, 114, 198);
            table.BackColorNormal = Color.White;
            table.BackColorHover = Color.FromArgb(177, 214, 240);
            table.BackColorPressed = ColorHelper.GetDarkerColor(table.BackColorHover, 16);
            return table;
        }

        #endregion

        #region 内部使用的变量

        CutePointAndValuePresenter presenter;

        bool isMouseDownInButton;

        WLButton thumbButton;

        #endregion

        #region 内部函数, 属性

        int _buttonCurrentPositionDot;

        private int ButtonCurrentPositionDot
        {
            get
            {
                return _buttonCurrentPositionDot;
            }
            set
            {
                if (_buttonCurrentPositionDot != value)
                {
                    if (_barOrientation == Orientation.Horizontal)
                    {
                        if (value < BBDot)
                            _buttonCurrentPositionDot = BBDot;
                        else if (value > BMDot)
                            _buttonCurrentPositionDot = BMDot;
                        else
                            _buttonCurrentPositionDot = value;
                    }
                    else
                    {
                        if (value > BBDot)
                            _buttonCurrentPositionDot = BBDot;
                        else if (value < BMDot)
                            _buttonCurrentPositionDot = BMDot;
                        else
                            _buttonCurrentPositionDot = value;
                    }
                    if (thumbButton != null)
                        thumbButton.Bounds = BTR;
                }
            }
        }

        private void DoWhenResize()
        {
            SetFittedSize();
            thumbButton.Bounds = BTR;
            UpdatePresenterAndRedraw();
        }

        private void DoWhenMouseDownInMainLine(Point p)
        {            
            DoWhenMouseMoveInButton(p);
        }

        private void DoWhenMouseMoveInButton(Point p)
        {
            int begin = BBDot;
            int max = BMDot;

            if (_barOrientation == Orientation.Horizontal)
                p.Offset(-ButtonLength1 / 2, 0);
            else
                p.Offset(0, -ButtonLength1 / 2);

            if (_barOrientation == Orientation.Horizontal)
            {
                int pos = p.X;
                if (pos < (begin - 100) || pos > (max + 100))
                    return;
                if (pos < begin)
                    pos = begin;
                if (pos > max)
                    pos = max;

                int v1, v2;
                int locIndex = pos - begin;
                presenter.GetValueIndexFromPointIndex(locIndex, out v1, out v2);
                if (_value < v1 || _value > v2)
                {
                    if (pos == begin)
                        Value = Minimum + v1;
                    else if (pos == max)
                        Value = Minimum + v2;
                    else
                        Value = Minimum + (v1 + v2) / 2;
                }
            }
            else
            {
                int pos = p.Y;
                if (pos < (max - 100) || pos > (begin + 100))
                    return;
                if (pos < max)
                    pos = max;
                if (pos > begin)
                    pos = begin;
                pos = (begin + max) - pos;
                int v1, v2;
                int locIndex = pos - max;
                presenter.GetValueIndexFromPointIndex(locIndex, out v1, out v2);
                if (_value < v1 || _value > v2)
                {
                    if (pos == begin)
                        Value = Minimum + v2;
                    else if (pos == max)
                        Value = Minimum + v1;
                    else
                        Value = Minimum + (v1 + v2) / 2;
                }
            }
        }

        private void UpdatePresenterAndRedraw()
        {
            int pointCount = RemainPointCount;
            int valueCount = Maximum - Minimum + 1;
            if (pointCount != presenter.PointCount || valueCount != presenter.ValueCount)
            {                
                presenter.SetPointAndValueCount(pointCount, valueCount);

                ResetButtonPositionFromValue();
                Invalidate();
            }
        }

        private void ResetButtonPositionFromValue()
        {
            int newPos = GetDotFromValue(Value);
            if (newPos != ButtonCurrentPositionDot)
            {
                ButtonCurrentPositionDot = newPos;
                Invalidate();
            }
        }

        private int GetDotFromValue(int value)
        {
            int p1, p2;
            int valueIndex = value - Minimum;
            presenter.GetPointIndexFromValueIndex(valueIndex, out p1, out p2);

            int targetPos;
            if (value == Minimum)
                targetPos = p1;
            else if (value == Maximum)
                targetPos = p2;
            else
                targetPos = (p1 + p2) / 2;
            
            if (_barOrientation == Orientation.Horizontal)
                targetPos += BBDot;
            else
            {
                targetPos += BMDot;
                targetPos = (BMDot + BBDot) - targetPos;
            }
            return targetPos;
        }

        private void ValueAdd(int amount)
        {            
            if (Value == Minimum && amount <= 0)
                return;
            if (Value == Maximum && amount >= 0)
                return;
            int v = Value;
            v += amount;
            if (v < Minimum)
                v = Minimum;
            if (v > Maximum)
                v = Maximum;
            Value = v;
        }

        private void SetFittedSize()
        {
            if (AutoSize)
            {
                int bestLen = BestFittedLength;
                if (_barOrientation == Orientation.Horizontal)
                {
                    if (base.Height != bestLen)
                        base.Height = bestLen;
                }
                else
                {
                    if (base.Width != bestLen)
                        base.Width = bestLen;
                }
            }
        }

        private void DoWhenBarOrientationChanged()
        {
            var newSize = new Size(base.Height, base.Width);

            base.Size = newSize;            
            ResetButtonPositionFromValue();
            UpdatePresenterAndRedraw();
            Invalidate();
        }

        private void DoWhenXThemeChanged()
        {
            SetThumbButtonInfo();
            DoWhenResize();
            Invalidate();
        }

        private void DoWhenAutoSizeChanged()
        {
            SetFittedSize();       
        }

        #endregion

        #region 内部绘图

        protected virtual void OnPaintGMTrackbar(Graphics g)
        {
            if (DrawBackground)
                RenderBackground(g);
            if (TickSide != TickStyle.None)
                RenderTickLines(g);
            RenderMainLine(g);
            RenderButton(g);
            if (DrawBorder)
                RenderBorders(g);
            RenderFocusRect(g); 
        }

        private void RenderBackground(Graphics g)
        {
            using (SolidBrush sb = new SolidBrush(GMBackColor))
            {
                g.FillRectangle(sb, ClientRectangle);
            }
        }

        private void RenderFocusRect(Graphics g)
        {
            if (DrawFocusRect && base.Focused)
            {
                Rectangle rect = ClientRectangle;
                rect.Width--;
                rect.Height--;
                rect.Inflate(-1, -1);
                using (Pen p = new Pen(Color.Black))
                {
                    p.DashStyle = DashStyle.Dot;
                    g.DrawRectangle(p, rect);
                }
            }
        }

        private void RenderBorders(Graphics g)
        {
            Rectangle rect = ClientRectangle;
            rect.Width--;
            rect.Height--;
            using (Pen p = new Pen(BorderColor))
            {
                g.DrawRectangle(p, rect);
            }
            if (DrawInnerBorder)
            {
                rect.Inflate(-1, -1);
                using (Pen p2 = new Pen(InnerBorderColor))
                {
                    g.DrawRectangle(p2, rect);
                }
            }
        }

        private void RenderMainLine(Graphics g)
        {
            GMTrackBarPaintEventArgs e = new GMTrackBarPaintEventArgs(g,
                MainLineRectToDraw, MainLineRange1Rect, MainLineRange2Rect);
            OnBeforePaintMainLine(e);

            if (!e.Cancel)
            {
                RenderMainLine(g, MainLineRange1Rect, MainLineRange1BackColor);
                RenderMainLine(g, MainLineRange2Rect, MainLineRange2BackColor);
            }
            e.Dispose();
        }

        private void RenderMainLine(Graphics g, Rectangle rect, Color backColor)
        {
            SmoothingMode newMode = (MainLineRadius > 0) ? SmoothingMode.AntiAlias : SmoothingMode.HighSpeed;
            RoundStyle style = (MainLineRadius > 0) ? RoundStyle.All : RoundStyle.None;

            using (NewSmoothModeGraphics ng = new NewSmoothModeGraphics(g, newMode))
            {
                using (SolidBrush sb = new SolidBrush(backColor))
                {
                    using (GraphicsPath path = GraphicsPathHelper.CreateRoundedRect(
                        rect, MainLineRadius, style, false))
                    {
                        g.FillPath(sb, path);
                    }
                }
                if (MainLineDrawBorder)
                {
                    using (Pen p = new Pen(MainLineBorderColor))
                    {
                        using (GraphicsPath path = GraphicsPathHelper.CreateRoundedRect(
                            rect, MainLineRadius, style, true))
                        {
                            g.DrawPath(p, path);
                        }
                    }
                }
            }
        }

        private void RenderButton(Graphics g)
        {
            GMTrackBarPaintEventArgs e = new GMTrackBarPaintEventArgs(g, BTR);
            e.BtnState = thumbButton.State;
            OnBeforePaintButton(e);

            if (!e.Cancel)
                thumbButton.DrawButton(g);
            e.Dispose();
        }

        private void RenderTickLines(Graphics g)
        {
            Pen p = new Pen(TickLineColor);
            
            if (TickSide == TickStyle.Both || TickSide == TickStyle.TopLeft)
            {
                if (_barOrientation == Orientation.Horizontal)
                    RenderTickLinesTopBottom(g, p, TickLinesRect1, true);
                else
                    RenderTickLinesLeftRight(g, p, TickLinesRect1, true);
            }
            if (TickSide == TickStyle.Both || TickSide == TickStyle.BottomRight)
            {
                if (_barOrientation == Orientation.Horizontal)
                    RenderTickLinesTopBottom(g, p, TickLinesRect2, false);
                else
                    RenderTickLinesLeftRight(g, p,TickLinesRect2, false);
            }

            p.Dispose();
        }

        private void RenderTickLinesTopBottom(Graphics g, Pen p, Rectangle rect, bool isTop)
        {            
            int y1 = rect.Top;
            int y2 = rect.Bottom - 1;
            int prex = -1;
            int addExtra = ButtonLength1 / 2;

            int xfirst = GetDotFromValue(Minimum) + addExtra;
            int xlast = GetDotFromValue(Maximum) + addExtra;            

            for (int i = Minimum; i <= Maximum; i += TickFrequency)
            {
                int x = GetDotFromValue(i) + addExtra;

                if (x == xfirst || x == (xfirst + 1) || x == xlast || x == (xlast - 1))
                    continue;
                if (x == prex || x == (prex + 1))
                    continue;

                g.DrawLine(p, x, y1, x, y2);
                prex = x;
            }
            
            // render the first and last points with a little longer line
            if (isTop)
                y1 -= 1;
            else
                y2 += 1;
            g.DrawLine(p, xfirst, y1, xfirst, y2);
            g.DrawLine(p, xlast, y1, xlast, y2);
        }        

        private void RenderTickLinesLeftRight(Graphics g, Pen p, Rectangle rect, bool isLeft)
        {            
            int x1 = rect.Left;
            int x2 = rect.Right - 1;
            int prey = -1;
            int addExtra = ButtonLength1 / 2;

            int yfirst = GetDotFromValue(Minimum) + addExtra;
            int ylast = GetDotFromValue(Maximum) + addExtra;

            for (int i = Minimum; i <= Maximum; i += TickFrequency)
            {
                int y = GetDotFromValue(i) + addExtra;

                if (y == yfirst || y == yfirst - 1 || y == ylast || y == ylast + 1)
                    continue;
                if (y == prey || y == (prey - 1))
                    continue;

                g.DrawLine(p, x1, y, x2, y);
                prey = y;
            }

            // render the first and last points
            if (isLeft)
                x1 -= 1;
            else
                x2 += 1;
            g.DrawLine(p, x1, yfirst, x2, yfirst);
            g.DrawLine(p, x1, ylast, x2, ylast);
        }        

        #endregion

        #region 计算而得的内部只读属性

        private int RemainPointCount
        {
            get
            {
                int count = 0;

                if (_barOrientation == Orientation.Horizontal)
                {
                    count = Size.Width - BorderWidth * 2 - ButtonOutterSpace1 * 2
                        - ButtonLength1 + 1;
                }
                else
                {
                    count = Size.Height - BorderWidth * 2 - ButtonOutterSpace1 * 2
                        - ButtonLength1 + 1;
                }

                return count;
            }
        }

        private int BBDot
        {
            get
            {
                if (_barOrientation == Orientation.Horizontal)
                    return BorderWidth + ButtonOutterSpace1;
                else
                    return Size.Height - 1 - BorderWidth - ButtonOutterSpace1 - ButtonLength1 + 1;
            }
        }

        private int BMDot
        {
            get
            {
                if (_barOrientation == Orientation.Horizontal)
                    return (Size.Width - 1) - BorderWidth - ButtonOutterSpace1 - ButtonLength1 + 1;
                else
                    return BorderWidth + ButtonOutterSpace1;
            }
        }

        private int MainLineBegionDot
        {
            get
            {
                return BBDot + ButtonLength1 / 2;
            }
        }

        private int MainLineEndDot
        {
            get
            {
                return BMDot + ButtonLength1 / 2;
            }
        }

        private Rectangle BTR
        {
            get
            {

                if (_barOrientation == Orientation.Horizontal)
                {
                    int x = ButtonCurrentPositionDot;
                    int y = 0;
                    if (TickSide == TickStyle.Both || TickSide == TickStyle.TopLeft)
                        y = BorderWidth + TickLineSpaceWithBorder + TickLineLength +
                            TickLineSpaceWithButton + ButtonOutterSpace2;
                    else
                        y = BorderWidth + ButtonOutterSpace2;
                    return new Rectangle(x, y, ButtonLength1, ButtonLength2);
                }
                else
                {
                    int x = 0;
                    if (TickSide == TickStyle.Both || TickSide == TickStyle.TopLeft)
                        x = BorderWidth + TickLineSpaceWithBorder + TickLineLength +
                            TickLineSpaceWithButton + ButtonOutterSpace2;
                    else
                        x = BorderWidth + ButtonOutterSpace2;
                    int y = ButtonCurrentPositionDot;
                    return new Rectangle(x, y, ButtonLength2, ButtonLength1);
                }                
            }
        }

        private Rectangle MainLineRect
        {
            get
            {
                Rectangle btnRect = BTR;

                if (_barOrientation == Orientation.Horizontal)
                {
                    int h = MainLineLength;
                    int x = MainLineBegionDot;
                    int y = btnRect.Y + (btnRect.Height - h) / 2;
                    if ((btnRect.Height - h) % 2 != 0)
                        y++;
                    int w = MainLineEndDot - MainLineBegionDot + 1;
                    return new Rectangle(x, y, w, h);
                }
                else
                {
                    int w = MainLineLength;
                    int x = btnRect.X + (btnRect.Width - w) / 2;
                    if ((btnRect.Width - w) % 2 != 0)
                        x++;
                    int y = MainLineEndDot;
                    int h = MainLineBegionDot - y + 1;
                    return new Rectangle(x, y, w, h);
                }
            }
        }

        private Rectangle MainLineRectToDraw
        {
            get
            {
                Rectangle rect = MainLineRect;

                if (_barOrientation == Orientation.Horizontal)
                    rect.Inflate(2, 0);
                else
                    rect.Inflate(0, 2);

                return rect;
            }
        }

        private Rectangle MainLineRectHitTest
        {
            get
            {
                Rectangle rect = MainLineRect;
                int diff = (ButtonLength2 - MainLineLength) / 2;
                if (_barOrientation == Orientation.Horizontal)
                    rect.Inflate(0, diff);
                else
                    rect.Inflate(diff, 0);
                return rect;
            }
        }

        private Rectangle TickLinesRect1
        {
            get
            {
                int lineBegin = MainLineBegionDot;
                int lineEnd  = MainLineEndDot;

                if (_barOrientation == Orientation.Horizontal)
                {
                    int x = lineBegin;
                    int y = BorderWidth + TickLineSpaceWithBorder;
                    int w = lineEnd - lineBegin + 1;
                    int h = 0;
                    if (TickSide == TickStyle.Both || TickSide == TickStyle.TopLeft)
                        h = TickLineLength;
                    return new Rectangle(x, y, w, h);
                }
                else
                {
                    int x = BorderWidth + TickLineSpaceWithBorder;
                    int y = lineEnd;
                    int h = lineBegin - lineEnd + 1;
                    int w = 0;
                    if (TickSide == TickStyle.Both || TickSide == TickStyle.TopLeft)
                        w = TickLineLength;
                    return new Rectangle(x, y, w, h);
                }                
            }
        }

        private Rectangle TickLinesRect2
        {
            get
            {
                Rectangle btnRect = BTR;

                if (_barOrientation == Orientation.Horizontal)
                {
                    int x = MainLineBegionDot;
                    int y = btnRect.Bottom + ButtonOutterSpace2 + TickLineSpaceWithButton;
                    int w = MainLineEndDot - MainLineBegionDot + 1;
                    int h = 0;
                    if(TickSide == TickStyle.Both || TickSide == TickStyle.BottomRight)
                        h = TickLineLength;
                    return new Rectangle(x, y, w, h);
                }
                else
                {
                    int x = btnRect.Right + ButtonOutterSpace2 + TickLineSpaceWithButton;
                    int y = MainLineEndDot;
                    int h = MainLineBegionDot - MainLineEndDot + 1;
                    int w = 0;
                    if (TickSide == TickStyle.Both || TickSide == TickStyle.BottomRight)
                        w = TickLineLength;
                    return new Rectangle(x, y, w, h);
                }                
            }
        }

        private Rectangle MainLineRange1Rect
        {
            get
            {
                Rectangle rect = MainLineRectToDraw;
                int pos = ButtonCurrentPositionDot + ButtonLength1 / 2;

                if (_barOrientation == Orientation.Horizontal)
                {
                    rect.Width = pos - 1 - rect.X + 1;
                    return rect;
                }
                else
                {
                    return new Rectangle(rect.X, pos + 1,
                        rect.Width, rect.Bottom - pos - 1);
                }                
            }
        }

        private Rectangle MainLineRange2Rect
        {
            get
            {
                Rectangle rect = MainLineRectToDraw;
                int pos = ButtonCurrentPositionDot + ButtonLength1 / 2;

                if (_barOrientation == Orientation.Horizontal)
                {
                    return new Rectangle(pos + 1, rect.Y, rect.Right - pos - 1, rect.Height);
                }
                else
                {
                    rect.Height = pos - 1 - rect.Y + 1;
                    return rect;
                }                
            }
        }

        private int BestFittedLength
        {
            get
            {
                int value = BorderWidth * 2 + ButtonOutterSpace2 * 2 + ButtonLength2;
                if (TickSide == TickStyle.Both || TickSide == TickStyle.TopLeft)
                    value += TickLineSpaceWithBorder + TickLineLength + TickLineSpaceWithButton;
                if (TickSide == TickStyle.Both || TickSide == TickStyle.BottomRight)
                    value += TickLineSpaceWithBorder + TickLineLength + TickLineSpaceWithButton;
                return value;
            }
        }

        #endregion
        
        #region IGMControl实现

        [Browsable(false),EditorBrowsable(EditorBrowsableState.Never)]
        public GMControlType ControlType
        {
            get { return GMControlType.TrackBar; }
        }

        #endregion

        #region 重写基类方法

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            OnPaintGMTrackbar(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            DoWhenResize();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);            

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (TabStop)
                    base.Focus();

                if (BTR.Contains(e.Location))
                {
                    isMouseDownInButton = true;
                    thumbButton.State = GMButtonState.Pressed;
                }
                else if(MainLineRectHitTest.Contains(e.Location))
                {
                    DoWhenMouseDownInMainLine(e.Location);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                isMouseDownInButton = false;
                if (BTR.Contains(e.Location))
                    thumbButton.State = GMButtonState.Hover;
                else
                    thumbButton.State = GMButtonState.Normal;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            thumbButton.State = GMButtonState.Normal;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isMouseDownInButton)
            {
                DoWhenMouseMoveInButton(e.Location);
            }
            if (!base.Capture)
            {
                if(BTR.Contains(e.Location))
                    thumbButton.State = GMButtonState.Hover;
                else
                    thumbButton.State = GMButtonState.Normal;
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                return false;
            }
            switch ((keyData & Keys.KeyCode))
            {
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.End:
                case Keys.Home:

                case Keys.Down:
                case Keys.Up:
                case Keys.Left:
                case Keys.Right:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (DrawFocusRect)
                base.Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (DrawFocusRect)
                base.Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {            
            switch (e.KeyData)
            {
                case Keys.Left:
                case Keys.Up:
                    if (_barOrientation == Orientation.Horizontal)
                        DoSmallDown();
                    else
                        DoSmallUp();
                    break;
                case Keys.Down:
                case Keys.Right:
                    if (_barOrientation == Orientation.Horizontal)
                        DoSmallUp();
                    else
                        DoSmallDown();
                    break;
                case Keys.PageUp:
                    if (_barOrientation == Orientation.Horizontal)
                        DoLargeDown();
                    else
                        DoLargeUp();
                    break;
                case Keys.PageDown:
                    if (_barOrientation == Orientation.Horizontal)
                        DoLargeUp();
                    else
                        DoLargeDown();
                    break;
                case Keys.Home:
                    if (_barOrientation == Orientation.Horizontal)
                        Value = Minimum;
                    else
                        Value = Maximum;
                    break;
                case Keys.End:
                    if (_barOrientation == Orientation.Horizontal)
                        Value = Maximum;
                    else
                        Value = Minimum;
                    break;
            }
            base.OnKeyDown(e);
        }

        #endregion

        #region 内部类

        public class GMTrackBarPaintEventArgs : PaintEventArgs
        {
            public Rectangle MainLineRange1Rect { get; private set; }
            public Rectangle MainLineRange2Rect { get; private set; }
            public bool Cancel { get; set; }
            public GMButtonState BtnState { get; set; }

            public GMTrackBarPaintEventArgs(Graphics g, Rectangle clipRect)
                : this(g, clipRect, Rectangle.Empty, Rectangle.Empty)
            {

            }

            public GMTrackBarPaintEventArgs(Graphics g, Rectangle mainLineRect,
                Rectangle range1Rect, Rectangle range2Rect) : base(g, mainLineRect)
            {
                MainLineRange1Rect = range1Rect;
                MainLineRange2Rect = range2Rect;
                Cancel = false;
                BtnState = GMButtonState.Normal;
            }
        }

        #endregion

    }
}
