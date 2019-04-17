/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Gdu.WinFormUI
{
    [ToolboxItem(false), DefaultEvent("ValueChanged")]
    public abstract class GMScrollBarBase: GMBarControlBase, IGMControl
    {
        #region 类内部专用变量

        const int MIN_MIDDLEBUTTON_LENGTH = 10;

        const int TIMER_INTERVAL_SLOW = 400;
        const int TIMER_INTERVAL_FAST = 50;
        bool isMouseDownInMiddleButton;
        CutePointAndValuePresenter presenter;

        bool isMouseDownInSideButton;
        LocationResult sideButtonDownWhere;
        Point sideButtonDownPoint;
        Point middleButtonMovePoint;

        Timer mouseDownTimer;
        
        bool middleButtonVisible;

        WLButton wlSideButton1;
        WLButton wlSideButton2;
        WLButton wlMiddleButton;

        private static readonly object EVENT_VALUECHANGED;
        private static readonly object EVENT_THEMECHANGED;

        #endregion

        #region 内部专用属性

        private int BlankSpaceLength
        {
            get
            {
                int sideBtnLen = ShowSideButtons ? SideButtonLength * 2 : 0;
                return ScrollBarLength - InnerPaddingWidth * 2 - sideBtnLen -
                    MiddleButtonOutterSpace1 * 2;
            }
        }

        private int ActualMovableSpaceLength
        {
            get
            {
                return BlankSpaceLength - MiddleButtonLength + 1;
            }
        }        

        #endregion

        #region 内部私有方法

        private LocationResult CheckLocation(Point p)
        {
            if(SideButton1Rect.Contains(p))
            {
                return LocationResult.SideButton1;
            }
            if(SideButton2Rect.Contains(p))
            {
                return LocationResult.SideButton2;
            }
            if(MiddleButtonRect.Contains(p) && middleButtonVisible)
            {
                return LocationResult.MiddleButton;
            }
            if(BeforeMdlBtnRect.Contains(p))
            {
                return LocationResult.BeforeMiddleButton;
            }
            if (AfterMdlBtnRect.Contains(p))
            {
                return LocationResult.AfterMiddleButton;
            }
            return LocationResult.NoWhere;
        }

        private void DoOnMouseDown(Point p)
        {
            LocationResult where = CheckLocation(p);
            if (where == LocationResult.MiddleButton)
            {
                isMouseDownInMiddleButton = true;
                middleButtonMovePoint = p;
                wlMiddleButton.State = GMButtonState.Pressed;
            }
            else if (where != LocationResult.NoWhere)
            {                
                isMouseDownInSideButton = true;
                sideButtonDownWhere = where;
                sideButtonDownPoint = p;
                MouseDownSideButton(where);
                mouseDownTimer.Enabled = true;
            }
        }

        private void MouseDownSideButton(LocationResult where)
        {
            int delta = 0;
            switch (where)
            {
                case LocationResult.SideButton1:
                    delta = - SmallChange;
                    wlSideButton1.State = GMButtonState.Pressed;
                    break;
                case LocationResult.SideButton2:
                    wlSideButton2.State = GMButtonState.Pressed;
                    delta = SmallChange;
                    break;
                case LocationResult.AfterMiddleButton:
                    delta = LargeChange;
                    break;
                case LocationResult.BeforeMiddleButton:
                    delta  = - LargeChange;
                    break;
            }
            if (delta != 0)
            {
                ValueAdd(delta);
            }
        }

        private void DealMouseMoveWhenDownInSideButton(Point p)
        {
            sideButtonDownPoint = p;
        }

        private void DealMouseMoveWhenDownInMiddleButton(Point p)
        {
            if (IsVerticalBar)
            {
                if (p.Y < MiddleButtonBeginPositionDot || p.Y > (MiddleButtonMaxPositionDot + MiddleButtonLength))
                    return;
                int delta = p.Y - middleButtonMovePoint.Y;
                if (delta != 0)
                    this.DealMiddleButtonMove(delta);
                middleButtonMovePoint = p;
            }
            else
            {
                if (p.X < MiddleButtonBeginPositionDot || p.X > (MiddleButtonMaxPositionDot + MiddleButtonLength))
                    return;
                int delta = p.X - middleButtonMovePoint.X;
                if (delta != 0)
                    this.DealMiddleButtonMove(delta);
                middleButtonMovePoint = p;
            }
        }

        private void MouseDownTimerHandler(object sender, EventArgs e)
        {
            if (mouseDownTimer.Interval != TIMER_INTERVAL_FAST)
                mouseDownTimer.Interval = TIMER_INTERVAL_FAST;
            if (sideButtonDownWhere == CheckLocation(sideButtonDownPoint))
            {
                MouseDownSideButton(sideButtonDownWhere);
            }
        }

        private void ValueAdd(int amount)
        {
            int value = Value;
            value += amount;
            if (value < Minimum)
                value = Minimum;
            if (value > Maximum)
                value = Maximum;
            Value = value;
        }

        private void UpdateScrollInfo()
        {
            if (presenter == null)
                return;

            int valueCount = Maximum - Minimum + 1;
            int pointCount = ActualMovableSpaceLength;
            if (valueCount != presenter.ValueCount || pointCount != presenter.PointCount)
            {                
                presenter.SetPointAndValueCount(pointCount, valueCount);
                ResetMiddleButtonPosition();
                base.Invalidate();
            }
        }

        private void ResetMiddleButtonPosition()
        {
            int beginDot = MiddleButtonBeginPositionDot;
            int p1, p2;
            presenter.GetPointIndexFromValueIndex(Value - Minimum, out p1, out p2);
            p1 += beginDot;
            p2 += beginDot;
            if (MiddleButtonCurrentPositionDot >= p1 && MiddleButtonCurrentPositionDot <= p2)
                return;
            if (Value == Maximum)
                MiddleButtonCurrentPositionDot = p2;
            else
                MiddleButtonCurrentPositionDot = p1;
            Invalidate();
        }

        private bool HasEnoughRoomForMiddleButton()
        {
            int lenForMBtn = ScrollBarLength - InnerPaddingWidth * 2 -
                MiddleButtonOutterSpace1 * 2;
            if (ShowSideButtons)
                lenForMBtn -= SideButtonLength * 2;
            return (lenForMBtn > MIN_MIDDLEBUTTON_LENGTH);
        }

        private void DoOnResize()
        {

            middleButtonVisible = HasEnoughRoomForMiddleButton() && base.Enabled;

            wlSideButton1.Bounds = SideButton1Rect;
            wlSideButton2.Bounds = SideButton2Rect;
            wlMiddleButton.Bounds = MiddleButtonRect;

            this.UpdateScrollInfo();          
        }

        private void UpdateInfoToSideMiddleButton()
        {
            SetSideMdlBtnInfo();
            DoOnResize();
            Invalidate();
        }

        private void SetSideMdlBtnInfo()
        {
            wlSideButton1.ColorTable = SideButtonColorTable;
            wlSideButton2.ColorTable = SideButtonColorTable;
            wlMiddleButton.ColorTable = MiddleButtonColorTable;
            wlSideButton1.ForePathGetter = SideButtonForePathGetter;
            wlSideButton2.ForePathGetter = SideButtonForePathGetter;
            wlSideButton1.ForePathSize = SideButtonForePathSize;
            wlSideButton2.ForePathSize = SideButtonForePathSize;

            wlSideButton1.HowForePathRender = HowSideButtonForePathDraw;
            wlSideButton2.HowForePathRender = HowSideButtonForePathDraw;

            wlSideButton1.BorderType = wlSideButton2.BorderType =
                (_xtheme == null ? ButtonBorderType.Rectangle : _xtheme.SideButtonBorderType);
            wlSideButton1.Radius = wlSideButton2.Radius =
                (_xtheme == null ? 0 : _xtheme.SideButtonRadius);
            wlMiddleButton.Radius = (_xtheme == null ? 0 : _xtheme.MiddleButtonRadius);
        }

        private void SetButtonState(Point p, GMButtonState newState)
        {
            wlMiddleButton.State = wlSideButton2.State = wlSideButton1.State = GMButtonState.Normal;
            switch (CheckLocation(p))
            {
                case LocationResult.MiddleButton:
                    wlMiddleButton.State = newState;
                    break;
                case LocationResult.SideButton1:
                    wlSideButton1.State = newState;
                    break;
                case LocationResult.SideButton2:
                    wlSideButton2.State = newState;
                    break;
            }
        }

        private void DealMiddleButtonMove(int moveDelta)
        {
            MiddleButtonCurrentPositionDot += moveDelta;
            int v1, v2;
            int locIndex = MiddleButtonCurrentPositionDot - MiddleButtonBeginPositionDot;
            presenter.GetValueIndexFromPointIndex(locIndex, out v1, out v2);
            if (MiddleButtonCurrentPositionDot == MiddleButtonMaxPositionDot)
            {
                Value = Minimum + v2;
            }
            else if (MiddleButtonCurrentPositionDot == MiddleButtonBeginPositionDot)
            {
                Value = Minimum + v1;
            }
            else
            {
                Value = Minimum + v1 + (v2 - v1) / 2;
            }
        }

        #endregion

        #region 内部绘图

        private void PaintScrollBar(Graphics g)
        {
            if (DrawBackground)
            {
                RenderBackground(g);
            }

            if (middleButtonVisible)
            {
                wlMiddleButton.Bounds = MiddleButtonRect;
                wlMiddleButton.DrawButton(g);             
            }           

            if (ShowSideButtons)
            {
                wlSideButton1.DrawButton(g);
                wlSideButton2.DrawButton(g);
            }

            if (DrawBorder)
            {
                RenderBorders(g);
            }                      
        }

        private void RenderBackground(Graphics g)
        {
            using (SolidBrush sb = new SolidBrush(GMBackColor))
            {
                g.FillRectangle(sb, ClientRectangle);
            }
        }

        private void RenderMiddleButton(Graphics g)
        {
            Rectangle rect = MiddleButtonRect;
            rect.Width--; rect.Height--;
            g.DrawRectangle(Pens.Black, rect);
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
        }

        private void MiddleButtonExtraPaint(object sender, PaintEventArgs e)
        {
            if (!DrawLinesInMiddleButton)
                return;
            int linesLen = 8;
            if (MiddleButtonLength < (linesLen + MiddleBtnLineOutterSpace1 * 2))
                return;

            Pen p1 = new Pen(MiddleButtonLine1Color);
            Pen p2 = new Pen(MiddleButtonLine2Color);
            Rectangle rect = e.ClipRectangle;

            if (IsVerticalBar)
            {
                int x1 = rect.Left + MiddleBtnLineOutterSpace2;
                int x2 = rect.Right - MiddleBtnLineOutterSpace2 - 1;
                int y = rect.Top + (rect.Height - linesLen) / 2;
                for (int i = 0; i < 3; i++)
                {
                    e.Graphics.DrawLine(p1, x1, y, x2, y);
                    e.Graphics.DrawLine(p2, x1, y+1, x2, y+1);

                    y += 3;
                }
            }
            else
            {
                int x = rect.Left + (rect.Width - linesLen) / 2;
                int y1 = rect.Top + MiddleBtnLineOutterSpace2;
                int y2 = rect.Bottom - MiddleBtnLineOutterSpace2 - 1;
                for (int i = 0; i < 3; i++)
                {
                    e.Graphics.DrawLine(p1, x, y1, x, y2);
                    e.Graphics.DrawLine(p2, x+1, y1, x+1, y2);
                    x += 3;
                }
            }

            p1.Dispose();
            p2.Dispose();
        }

        #endregion

        #region 可通过XTheme配置的属性

        protected int InnerPaddingWidth
        {
            get
            {
                if (_xtheme == null)
                    return 0;
                else
                    return _xtheme.InnerPaddingWidth;
            }
        }

        protected int MiddleButtonOutterSpace1
        {
            get
            {
                if (_xtheme == null)
                    return 1;
                else
                    return _xtheme.MiddleButtonOutterSpace1;
            }
        }

        protected int MiddleButtonOutterSpace2
        {
            get
            {
                if (_xtheme == null)
                    return 0;
                else
                    return _xtheme.MiddleButtonOutterSpace2;
            }
        }

        protected int SideButtonLength
        {
            get
            {
                if (_xtheme == null)
                    return 16;
                else
                    return _xtheme.SideButtonLength;
            }
        }

        protected bool DrawBackground
        {
            get
            {
                if (_xtheme == null)
                    return true;
                else
                    return _xtheme.DrawBackground;
            }
        }

        protected bool DrawBorder
        {
            get
            {
                if (_xtheme == null)
                    return false;
                else
                    return _xtheme.DrawBorder;
            }
        }

        protected bool DrawInnerBorder
        {
            get
            {
                if (_xtheme == null)
                    return false;
                else
                    return _xtheme.DrawInnerBorder;
            }
        }

        protected bool ShowSideButtons
        {
            get
            {
                if (_xtheme == null)
                    return true;
                else
                    return _xtheme.ShowSideButtons;
            }
        }

        protected Color GMBackColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.FromArgb(227, 227, 227);
                else
                    return _xtheme.BackColor;
            }
        }

        protected Color BorderColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.FromArgb(248, 248, 248);
                else
                    return _xtheme.BorderColor;
            }
        }

        protected Color InnerBorderColor
        {
            get
            {
                if (_xtheme == null)
                    return Color.Empty;
                else
                    return _xtheme.InnerBorderColor;
            }
        }

        protected Size SideButtonForePathSize
        {
            get
            {
                if (_xtheme == null)
                    return new Size(7, 7);
                else
                    return _xtheme.SideButtonForePathSize;
            }
        }

        protected ButtonForePathGetter SideButtonForePathGetter
        {
            get
            {
                if (_xtheme == null)
                    return new ButtonForePathGetter(
                        Gdu.WinFormUI.MyGraphics.GraphicsPathHelper.Create7x4In7x7DownTriangleFlag);
                else
                    return _xtheme.SideButtonForePathGetter;
            }
        }

        protected ButtonColorTable SideButtonColorTable
        {
            get
            {
                if (_xtheme == null)
                    return GetDefaultSideMdlBtnColor();
                else
                    return _xtheme.SideButtonColorTable;
            }
        }

        protected ButtonColorTable MiddleButtonColorTable
        {
            get
            {
                if (_xtheme == null)
                    return GetDefaultSideMdlBtnColor();
                else
                    return _xtheme.MiddleButtonColorTable;
            }
        }

        protected ForePathRenderMode HowSideButtonForePathDraw
        {
            get
            {
                if (_xtheme == null)
                    return ForePathRenderMode.Draw ;
                else
                    return _xtheme.HowSideButtonForePathDraw;
            }
        }

        protected bool DrawLinesInMiddleButton
        {
            get
            {
                if (_xtheme == null)
                    return true;
                else
                    return _xtheme.DrawLinesInMiddleButton;
            }
        }

        protected Color MiddleButtonLine1Color
        {
            get
            {
                if (_xtheme == null)
                    return Color.FromArgb(89, 89, 89);
                else
                    return _xtheme.MiddleButtonLine1Color;
            }
        }

        protected Color MiddleButtonLine2Color
        {
            get
            {
                if (_xtheme == null)
                    return Color.FromArgb(182, 182, 182);
                else
                    return _xtheme.MiddleButtonLine2Color;
            }
        }

        protected int MiddleBtnLineOutterSpace1
        {
            get
            {
                if (_xtheme == null)
                    return 4;
                else
                    return _xtheme.MiddleBtnLineOutterSpace1;
            }
        }

        protected int MiddleBtnLineOutterSpace2
        {
            get
            {
                if (_xtheme == null)
                    return 4;
                else
                    return _xtheme.MiddleBtnLineOutterSpace2;
            }
        }

        private ButtonColorTable GetDefaultSideMdlBtnColor()
        {
            ButtonColorTable table = new ButtonColorTable();

            table.BorderColorNormal = Color.FromArgb(151, 151, 151);
            table.BorderColorHover = Color.FromArgb(53, 111, 155);
            table.BorderColorPressed = Color.FromArgb(60, 127, 177);

            table.BackColorNormal = Color.FromArgb(217, 218, 219);
            table.BackColorHover = Color.FromArgb(169, 219, 246);
            table.BackColorPressed = Color.FromArgb(111, 202, 240);

            table.ForeColorNormal = Color.FromArgb(73, 73, 73);
            table.ForeColorHover = Color.FromArgb(32, 106, 145);
            table.ForeColorPressed = Color.FromArgb(15, 38, 50);
            table.ForeColorDisabled = SystemColors.ControlDarkDark;

            table.BackColorDisabled = SystemColors.ControlDark;

            return table;
        }

        #endregion

        #region 子类可访问的Protected属性, 可重写的protected方法

        protected int MiddleButtonLength
        {
            get
            {
                return Math.Max(MIN_MIDDLEBUTTON_LENGTH,
                    BlankSpaceLength * MiddleButtonLengthPercentage / 100);
            }
        }

        protected int MiddleButtonMaxPositionDot
        {
            get
            {
                return MiddleButtonBeginPositionDot + ActualMovableSpaceLength - 1;
            }
        }

        int _middleButtonCurrentPositionDot;
        protected int MiddleButtonCurrentPositionDot
        {
            get
            {
                return _middleButtonCurrentPositionDot;
            }
            set
            {
                if (_middleButtonCurrentPositionDot == value)
                    return;
                if (value > MiddleButtonMaxPositionDot)
                    _middleButtonCurrentPositionDot = MiddleButtonMaxPositionDot;
                else if (value < MiddleButtonBeginPositionDot)
                    _middleButtonCurrentPositionDot = MiddleButtonBeginPositionDot;
                else
                    _middleButtonCurrentPositionDot = value;
                
                Invalidate();
            }
        }

        /// <summary>
        /// 引发 ValueChanged 事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValueChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)base.Events[EVENT_VALUECHANGED];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 引发 ThemeChanged 事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnThemeChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)base.Events[EVENT_THEMECHANGED];
            if (handler != null)
            {
                handler(this, e);
            }
        }        

        #endregion

        #region 留给子类(HScrollBar, VScrollBar)重写的属性

        protected abstract int ScrollBarLength { get; }        
        protected abstract int MiddleButtonBeginPositionDot { get; }        
        protected abstract Rectangle SideButton1Rect { get; }        
        protected abstract Rectangle SideButton2Rect { get; }        
        protected abstract Rectangle MiddleButtonRect { get; }        
        protected abstract Rectangle BeforeMdlBtnRect { get; }        
        protected abstract Rectangle AfterMdlBtnRect { get; }
        protected abstract ForePathRatoteDirection SideButton1RotateInfo { get; }
        protected abstract ForePathRatoteDirection SideButton2RotateInfo { get; }
        protected abstract bool IsVerticalBar { get; }

        #endregion

        #region IGMControl接口实现

        [Browsable(false),EditorBrowsable(EditorBrowsableState.Never)]
        public GMControlType ControlType
        {
            get { return GMControlType.ScrollBar; }
        }

        #endregion

        #region 控件事件

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

        public event EventHandler ThemeChanged
        {
            add
            {
                base.Events.AddHandler(EVENT_THEMECHANGED, value);
            }
            remove
            {
                base.Events.RemoveHandler(EVENT_THEMECHANGED, value);
            }
        }

        #endregion

        #region 辅助公开属性的内部变量

        int _maximum = 100;
        int _minimum = 0;
        int _value = 0;
        int _smallChange = 1;
        int _largeChange = 10;
        int _middleButtonLengthPercentage = 10;
        GMScrollBarThemeBase _xtheme;

        #endregion

        #region 公开属性

        [Browsable(false),EditorBrowsable(EditorBrowsableState.Never)]
        public GMScrollBarThemeBase XTheme
        {
            get
            {                
                return this._xtheme;
            }
            set
            {
                if (this._xtheme == value)
                    return;
                this._xtheme = value;
                UpdateInfoToSideMiddleButton();
                OnThemeChanged(EventArgs.Empty);                
            }
        }

        [DefaultValue(0)]
        public int Value
        {
            get 
            {
                return this._value;
            }
            set
            {
                if (this._value == value)
                    return;
                if (value < Minimum || value > Maximum)
                    throw new ArgumentOutOfRangeException("Value");

                this._value = value;
                ResetMiddleButtonPosition();
                OnValueChanged(EventArgs.Empty);
            }
        }

        [DefaultValue(0), RefreshProperties(System.ComponentModel.RefreshProperties.Repaint)]
        public int Minimum
        {
            get
            {
                return this._minimum;
            }
            set
            {
                if (this._minimum == value)
                    return;

                if (value > this._maximum)
                    this._maximum = value;

                if (this.Value < value)
                    this.Value = value;
                
                this._minimum = value;
                this.UpdateScrollInfo();
            }
        }

        [DefaultValue(100), RefreshProperties(System.ComponentModel.RefreshProperties.Repaint)]
        public int Maximum
        {
            get
            {
                return this._maximum;
            }
            set
            {
                if (this._maximum == value)
                    return;
                
                if (value < this._minimum)
                    this._minimum = value;
                
                if (this.Value > value)
                    this.Value = value;

                this._maximum = value;
                this.UpdateScrollInfo();
            }
        }

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

        public int MiddleButtonLengthPercentage
        {
            get
            {
                return this._middleButtonLengthPercentage;
            }
            set
            {
                if (this._middleButtonLengthPercentage == value)
                    return;
                if (value >= 5 && value <= 90)
                {
                    this._middleButtonLengthPercentage = value;
                }
                else
                {
                    if (value < 5)
                        this._middleButtonLengthPercentage = 5;
                    if (value > 90)
                        this._middleButtonLengthPercentage = 90;
                }
                this.UpdateScrollInfo();
            }
        }

        #endregion

        #region 构造函数及初始化

        static GMScrollBarBase()
        {
            EVENT_VALUECHANGED = new object();
            EVENT_THEMECHANGED = new object();
        }

        public GMScrollBarBase()
            : base()
        {
            presenter = new CutePointAndValuePresenter();            
            ButtonsIni();
            mouseDownTimer = new Timer();
            mouseDownTimer.Enabled = false;
            mouseDownTimer.Interval = TIMER_INTERVAL_SLOW;
            mouseDownTimer.Tick += new EventHandler(MouseDownTimerHandler);
        }       

        private void ButtonsIni()
        {
            wlSideButton1 = new WLButton(this);
            wlSideButton2 = new WLButton(this);
            wlMiddleButton = new WLButton(this);

            wlSideButton1.RotateDirection = SideButton1RotateInfo;
            wlSideButton2.RotateDirection = SideButton2RotateInfo;

            wlSideButton1.RoundedType = wlSideButton2.RoundedType = MyGraphics.RoundStyle.All;
            wlMiddleButton.RoundedType = MyGraphics.RoundStyle.All;

            SetSideMdlBtnInfo();

            wlMiddleButton.Paint += new PaintEventHandler(MiddleButtonExtraPaint);
        }

        #endregion

        #region 重写方法

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            PaintScrollBar(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            DoOnResize();  
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                DoOnMouseDown(e.Location);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                isMouseDownInMiddleButton = false;
                isMouseDownInSideButton = false;
                sideButtonDownWhere = LocationResult.NoWhere;
                mouseDownTimer.Enabled = false;
                mouseDownTimer.Interval = TIMER_INTERVAL_SLOW;

                SetButtonState(e.Location, GMButtonState.Hover);
            }
        }       

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isMouseDownInSideButton)
            {
                DealMouseMoveWhenDownInSideButton(e.Location);
            }
            if (isMouseDownInMiddleButton)
            {
                DealMouseMoveWhenDownInMiddleButton(e.Location);
            }
            if (!isMouseDownInMiddleButton && !isMouseDownInSideButton)
            {
                SetButtonState(e.Location, GMButtonState.Hover);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            wlMiddleButton.State = wlSideButton2.State = wlSideButton1.State = GMButtonState.Normal;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            wlSideButton2.Enabled = base.Enabled;
            wlSideButton1.Enabled = base.Enabled;

            middleButtonVisible = HasEnoughRoomForMiddleButton() && base.Enabled;            
            Invalidate();
        }

        #endregion

        #region debug

        //debuggggggggggggggggggggggggggggggggg
        //ccccccccccccccccccccccccccccccccccccc
        public void DebugEntry()
        {
            Console.WriteLine("BlankSpaceLength: " + BlankSpaceLength.ToString());
            Console.WriteLine("MiddleButtonLength: " + MiddleButtonLength.ToString());
            Console.WriteLine("MiddleButtonRect: " + MiddleButtonRect.ToString());
            Console.WriteLine("ActualMovableSpaceLength: " + ActualMovableSpaceLength.ToString());

            //XTheme.MiddleButtonOutterSpace1 = 2;

            Console.WriteLine("MiddleButtonOutterSpace1: " + XTheme.MiddleButtonOutterSpace1.ToString());

            Console.WriteLine("InnerPaddingWidth: " + XTheme.InnerPaddingWidth.ToString());

        }

        //debuggggggggggggggggggggggggggggggggggggggggggg
        public CutePointAndValuePresenter GetPresenter()
        {
            return presenter;
        }

        #endregion

        /// <summary>
        /// 指示鼠标位于哪个区域
        /// </summary>
        private enum LocationResult
        {
            NoWhere,
            SideButton1,
            SideButton2,
            MiddleButton,
            BeforeMiddleButton,
            AfterMiddleButton
        }

    }   
}
