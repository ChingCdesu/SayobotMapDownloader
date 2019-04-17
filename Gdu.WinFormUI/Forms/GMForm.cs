/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请先与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Gdu.WinFormUI.Win32;
using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    /// <summary>
    /// a form that supports custom theme
    /// </summary>
    public partial class GMForm : Form
    {

        #region private variable

        ControlBoxManager controlBoxManager;
        GMShadow _shawdowForm;
        Form _lastClosedMdiChild;
        List<Form> _listClosedMdiChild;
        MdiBarController _mdiBarController;

        #endregion

        #region mdi event support

        EventHandler _mdiNewTabButtonClick;
        EventHandler _mdiTabCloseButtonClick;
        EventHandler _mdiBarCreated;

        public event EventHandler MdiNewTabButtonClick
        {
            add
            {
                _mdiNewTabButtonClick = value;
            }
            remove
            {
                _mdiNewTabButtonClick = null;
            }
        }

        public event EventHandler MdiTabCloseButtonClick
        {
            add
            {
                _mdiTabCloseButtonClick = value;
            }
            remove
            {
                _mdiTabCloseButtonClick = null;
            }
        }

        public event EventHandler MdiBarCreated
        {
            add { _mdiBarCreated = value; }
            remove { _mdiBarCreated = null; }
        }

        private void OnMdiNewTabButtonClick(object sender, EventArgs e)
        {
            if (_mdiNewTabButtonClick != null)
                _mdiNewTabButtonClick(sender, e);
        }

        private void OnMdiTabCloseButtonClick(object sender, EventArgs e)
        {
            if (_mdiTabCloseButtonClick != null)
                _mdiTabCloseButtonClick(sender, e);
        }

        protected virtual void OnMdiBarCreated(object sender, EventArgs e)
        {
            if (_mdiBarCreated != null)
                _mdiBarCreated(sender, e);
        }

        #endregion

        #region property

        #region property private var

        bool _resizable = true;             // not with theme
        Padding _padding = new Padding(0);  // not with theme

        ThemeFormBase _myTheme;

        #endregion
        
        [DefaultValue(typeof(Padding), "0")]
        public new Padding Padding
        {
            get { return _padding; }
            set
            {
                _padding = value;
                base.Padding = new Padding(
                    BorderWidth + _padding.Left,
                    CaptionHeight + BorderWidth + _padding.Top,
                    BorderWidth + _padding.Right,
                    BorderWidth + _padding.Bottom);
            }
        }

        protected override Padding DefaultPadding
        {
            get
            {
                return new Padding(
                    BorderWidth,
                    BorderWidth + CaptionHeight,
                    BorderWidth,
                    BorderWidth);
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                Invalidate(TextRect);
            }
        }

        [Browsable(false)]
        public ThemeFormBase XTheme
        {
            get
            {
                if (_myTheme == null)
                    _myTheme = new ThemeFormBase();
                return _myTheme;
            }
            set
            {
                _myTheme = value;
                PrepareForNewTheme();
                Invalidate();
            }
        }

        #region editable gmform properties

        [Category("GMForm")]
        [Description("是否可拖动改变窗体大小")]
        [DefaultValue(true)]
        public bool Resizable
        {
            get { return _resizable; }
            set
            {
                _resizable = value;
            }
        }

        [Category("GMForm")]
        [Description("窗体边界大小，鼠标移动到该边界将变成指针形状，拖动可改变窗体大小")]
        [DefaultValue(6)]
        public int SideResizeWidth
        {
            get { return XTheme.SideResizeWidth; }
            set
            {
                if (value != XTheme.SideResizeWidth)
                {
                    XTheme.SideResizeWidth = value;
                }
            }
        }

        [Category("GMForm")]
        [Description("窗体边框大小")]
        [DefaultValue(6)]
        public int BorderWidth
        { 
            get { return XTheme.BorderWidth; }
            set
            {
                if (value != XTheme.BorderWidth)
                {
                    XTheme.BorderWidth = value;
                    Invalidate();
                }
            }
        }

        [Category("GMForm")]
        [Description("标题栏高度")]
        [DefaultValue(30)]
        public int CaptionHeight
        { 
            get { return XTheme.CaptionHeight; }
            set
            {
                if (value != XTheme.CaptionHeight)
                {
                    XTheme.CaptionHeight = value;
                    Invalidate();
                }
            }
        }

        [Category("GMForm")]
        [DefaultValue(8)]
        public int Radius
        {
            get { return XTheme.Radius; }
            set
            {
                if (value != XTheme.Radius)
                {
                    XTheme.Radius = (value < 0 ? 0 : value);
                    Invalidate();
                }
            }
        }

        [Category("GMForm")]
        [DefaultValue(typeof(RoundStyle), "2")]
        public RoundStyle Round
        {
            get { return XTheme.RoundedStyle; }
            set
            {
                if (value != XTheme.RoundedStyle)
                {
                    XTheme.RoundedStyle = value;
                    Invalidate();
                }
            }
        }

        [Category("GMForm")]
        [Description("控制按钮相对于右上角的偏移量")]
        [DefaultValue(typeof(Point),"8, 8")]
        public Point ControlBoxOffset
        {
            get { return XTheme.ControlBoxOffset;}
            set
            {
                XTheme.ControlBoxOffset = value;
                Invalidate();
            }
        }

        [Category("GMForm")]
        [DefaultValue(0)]
        public int ControlBoxSpace
        {
            get { return XTheme.ControlBoxSpace; }
            set
            {
                XTheme.ControlBoxSpace = value;
                Invalidate();
            }
        }

        [Category("GMForm")]
        [DefaultValue(typeof(Size), "16,16")]
        public Size IconSize
        {
            get 
            {
                if (ShowIcon)
                    return XTheme.IconSize;
                else
                    return System.Drawing.Size.Empty;
            }
            set
            {
                XTheme.IconSize = value;
                Invalidate();
            }
        }

        [Category("GMForm")]
        [DefaultValue(2)]
        public int IconLeftMargin
        {
            get { return (this.ShowIcon ? XTheme.IconLeftMargin : 0); }
            set
            {
                XTheme.IconLeftMargin = value;
                Invalidate();
            }
        }

        [Category("GMForm")]
        [DefaultValue(2)]
        public int TextLeftMargin
        {
            get { return XTheme.TextLeftMargin; }
            set
            {
                XTheme.TextLeftMargin = value;
                Invalidate();
            }
        }

        [Category("GMForm")]
        [DefaultValue(typeof(Size), "37, 17")]
        public Size CloseBoxSize
        {
            get { return XTheme.CloseBoxSize; }
            set
            {
                XTheme.CloseBoxSize = value;
                Invalidate();
            }
        }

        [Category("GMForm")]
        [DefaultValue(typeof(Size), "25, 17")]
        public Size MaxBoxSize
        {
            get { return XTheme.MaxBoxSize; }
            set
            {
                XTheme.MaxBoxSize = value;
                Invalidate();
            }
        }

        [Category("GMForm")]
        [DefaultValue(typeof(Size), "25, 17")]
        public Size MinBoxSize
        {
            get { return XTheme.MinBoxSize; }
            set
            {
                XTheme.MinBoxSize = value;
                Invalidate();
            }
        }

        #endregion

        #region form shadow about

        [Category("Form Shadow")]
        public bool ShowShadow
        {
            get { return XTheme.ShowShadow; }
            set
            {
                if (value != XTheme.ShowShadow)
                {
                    XTheme.ShowShadow = value;                    
                }
            }
        }

        [Category("Form Shadow")]
        public int ShadowWidth
        {
            get { return XTheme.ShadowWidth; }
            set
            {
                XTheme.ShadowWidth = value;
            }
        }

        [Category("Form Shadow")]
        public bool UseShadowToResize
        {
            get { return XTheme.UseShadowToResize; }
            set
            {
                XTheme.UseShadowToResize = value;
            }
        }

        [Category("Form Shadow")]
        public Color ShadowColor
        {
            get { return XTheme.ShadowColor; }
            set
            {
                XTheme.ShadowColor = value;
            }
        }

        [Category("Form Shadow")]
        public int ShadowAValueDark
        {
            get { return XTheme.ShadowAValueDark; }
            set
            {
                XTheme.ShadowAValueDark = value;
            }
        }

        [Category("Form Shadow")]
        public int ShadowAValueLight
        {
            get { return XTheme.ShadowAValueLight; }
            set
            {
                XTheme.ShadowAValueLight = value;
            }
        }

        #endregion

        #region form resize region, internal readonly

        internal Rectangle TopLeftRect
        {
            get
            {
                return new Rectangle(0, 0, SideResizeWidth, SideResizeWidth);
            }
        }

        internal Rectangle TopRect
        {
            get
            {
                return new Rectangle(
                    SideResizeWidth,
                    0,
                    this.Size.Width - SideResizeWidth * 2,
                    SideResizeWidth);
            }
        }

        internal Rectangle TopRightRect
        {
            get
            {
                return new Rectangle(
                    this.Size.Width - SideResizeWidth,
                    0,
                    SideResizeWidth,
                    SideResizeWidth);
            }
        }

        internal Rectangle LeftRect
        {
            get
            {
                return new Rectangle(
                    0,
                    SideResizeWidth,
                    SideResizeWidth,
                    this.Size.Height - SideResizeWidth * 2);
            }
        }

        internal Rectangle RightRect
        {
            get
            {
                return new Rectangle(
                    this.Size.Width - SideResizeWidth,
                    SideResizeWidth,
                    SideResizeWidth,
                    this.Size.Height - SideResizeWidth * 2);
            }
        }

        internal Rectangle BottomLeftRect
        {
            get
            {
                return new Rectangle(
                    0,
                    this.Size.Height - SideResizeWidth,
                    SideResizeWidth,
                    SideResizeWidth);
            }
        }

        internal Rectangle BottomRect
        {
            get
            {
                return new Rectangle(
                    SideResizeWidth,
                    this.Size.Height - SideResizeWidth,
                    this.Size.Width - SideResizeWidth * 2,
                    SideResizeWidth);
            }
        }

        internal Rectangle BottomRightRect
        {
            get
            {
                return new Rectangle(
                    this.Size.Width - SideResizeWidth,
                    this.Size.Height - SideResizeWidth,
                    SideResizeWidth,
                    SideResizeWidth);
            }
        }

        #endregion


        #region calculated rect

        internal Rectangle CaptionRect
        {
            get
            {
                return new Rectangle(
                    BorderWidth,
                    BorderWidth,
                    this.ClientSize.Width - BorderWidth * 2,
                    CaptionHeight);
            }
        }

        internal Rectangle CaptionRectToDraw
        {
            get
            {
                return new Rectangle(
                    0,
                    0,
                    this.ClientSize.Width,
                    CaptionHeight + BorderWidth);
            }
        }

        internal Rectangle CloseBoxRect
        {
            get
            {
                if (ControlBox)
                {
                    int x = ClientSize.Width - ControlBoxOffset.X - CloseBoxSize.Width;
                    return new Rectangle(
                        new Point(x, ControlBoxOffset.Y),
                        CloseBoxSize);
                }
                else
                    return Rectangle.Empty;
            }
        }

        internal Rectangle MaxBoxRect
        {
            get
            {
                if (ControlBox && MaximizeBox)
                {
                    int x = CloseBoxRect.Left - ControlBoxSpace - MaxBoxSize.Width;
                    return new Rectangle(
                        new Point(x, ControlBoxOffset.Y),
                        MaxBoxSize);
                }
                else
                    return Rectangle.Empty;
            }
        }

        internal Rectangle MinBoxRect
        {
            get
            {
                if (ControlBox && MinimizeBox)
                {
                    int x;
                    if (MaximizeBox)
                        x = MaxBoxRect.Left - ControlBoxSpace - MinBoxSize.Width;
                    else
                        x = CloseBoxRect.Left - ControlBoxSpace - MinBoxSize.Width;
                    return new Rectangle(
                        new Point(x, ControlBoxOffset.Y),
                        MinBoxSize);
                }
                else
                    return Rectangle.Empty;
            }
        }

        internal Rectangle IconRect
        {
            get
            {
                if (ControlBox && ShowIcon)
                {
                    int x = BorderWidth + IconLeftMargin;
                    int y = BorderWidth + (CaptionHeight - IconSize.Height) / 2;
                    return new Rectangle(new Point(x, y), IconSize);
                }
                else
                    return new Rectangle(BorderWidth, BorderWidth, 0, 0);
            }
        }

        internal Rectangle TextRect
        {
            get
            {
                int x = IconRect.Right + TextLeftMargin;
                int y = BorderWidth;
                int height = CaptionHeight;
                int right = this.ClientSize.Width - x;
                if (ControlBox)
                {
                    right = CloseBoxRect.Left;
                    if (MinimizeBox)
                    {
                        right = MinBoxRect.Left;
                    }
                    else if (MaximizeBox)
                    {
                        right = MaxBoxRect.Left;
                    }
                }
                int width = right - x;
                return new Rectangle(x, y, width, height);
            }
        }

        #endregion

        /// <summary>
        /// 表示去掉自画的边框及标题栏后，剩下的可用的客户区区域，坐标相对于窗体左上角
        /// </summary>
        [Browsable(false)]        
        public Rectangle UserClientBounds
        {
            get
            {
                return new Rectangle(
                    BorderWidth,
                    BorderWidth + CaptionHeight,
                    ClientSize.Width - BorderWidth * 2,
                    ClientSize.Height - BorderWidth * 2 - CaptionHeight);
            }
        }

        /// <summary>
        /// 表示去掉自画的边框及标题栏后，剩下的可用的客户区大小
        /// </summary>
        [Browsable(false)]        
        public Size UserClientSize
        {
            get
            {
                return new Size(                    
                    ClientSize.Width - BorderWidth * 2,
                    ClientSize.Height - BorderWidth * 2 - CaptionHeight);
            }
        }

        /// <summary>
        /// 表示MDI标签栏的区域，包括bottom-region
        /// </summary>
        [Browsable(false)]
        public Rectangle MdiBarBounds
        {
            get
            {
                if (!IsMdiContainer || _mdiBarController == null)
                    return Rectangle.Empty;
                return _mdiBarController.Bounds;
            }
        }

        #endregion

        #region constructor & initialize

        public GMForm()
            :base()
        {
            InitializeComponent();
            FormIni();
            
            // 下面这个条件是永远不为true的，如果不把此类直接设置成mdicontainer
            //if (this.IsMdiContainer)
            //    SetMdiClient();
        }

        private void FormIni()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            this.UpdateStyles();
            base.Padding = DefaultPadding;

            if (ControlBox)
                controlBoxManager = new ControlBoxManager(this);
        }

        #region MDI-Support

        public MdiClient GetMdiClient()
        {
            MdiClient mdiclient = null;
            foreach (Control ctl in Controls)
            {
                if ((mdiclient = ctl as MdiClient) != null)                
                    break;                
            }
            return mdiclient;
        }

        /// <summary>
        /// 通过sendmessage方式激活子窗体以避免闪烁
        /// </summary>
        /// <param name="childToActivate"></param>
        public void ActivateMdiChildForm(Form childToActivate)
        {
            MdiClient mdi = GetMdiClient();
            if (mdi == null)
                return;
            if (base.ActiveMdiChild == childToActivate)
                return;
            if (base.MdiChildren.Length < 2)
                return;
            Control form = null;
            bool isNext = false;
            int pos = mdi.Controls.IndexOf(childToActivate);
            if (pos < 0)
                return;
            if (pos == 0)
            {
                form = mdi.Controls[pos + 1];
                isNext = true;
            }
            else
            {
                form = mdi.Controls[pos - 1];
                isNext = false;
            }
            IntPtr next = (isNext ? WinAPI.TRUE : WinAPI.FALSE);
            WinAPI.SendMessage(mdi.Handle, 
                (int)WinAPI.WindowMessages.WM_MDINEXT, form.Handle, next);            
        }

        private void SetMdiClient()
        {
            if (!IsMdiContainer)
                return;            

            MdiClient mdi = GetMdiClient();            
            if (mdi != null)
            {
                SetMdiStyles(mdi);
                UpdateMdiStyles(mdi);
                SetMdiClientLocation(mdi);                    
            }            
        }

        private void SetMdiStyles(MdiClient mdi)
        {
            // remove the border

            int style = WinAPI.GetWindowLong(mdi.Handle, (int)WinAPI.GWLPara.GWL_STYLE);
            int exStyle = WinAPI.GetWindowLong(mdi.Handle, (int)WinAPI.GWLPara.GWL_EXSTYLE);

            style &= ~(int)WinAPI.WindowStyle.WS_BORDER;
            exStyle &= ~(int)WinAPI.WindowStyleEx.WS_EX_CLIENTEDGE;

            WinAPI.SetWindowLong(mdi.Handle, (int)WinAPI.GWLPara.GWL_STYLE, style);
            WinAPI.SetWindowLong(mdi.Handle, (int)WinAPI.GWLPara.GWL_EXSTYLE, exStyle);

            WinAPI.ShowScrollBar(mdi.Handle, (int)WinAPI.ScrollBar.SB_BOTH, 0 /*false*/);
        }

        private void UpdateMdiStyles(MdiClient mdi)
        {
            // To show style changes, the non-client area must be repainted. Using the
            // control's Invalidate method does not affect the non-client area.
            // Instead use a Win32 call to signal the style has changed.

            WinAPI.SetWindowPos(mdi.Handle, IntPtr.Zero, 0, 0, 0, 0,
                (uint)WinAPI.SWPPara.SWP_NOACTIVATE |
                (uint)WinAPI.SWPPara.SWP_NOMOVE |
                (uint)WinAPI.SWPPara.SWP_NOSIZE |
                (uint)WinAPI.SWPPara.SWP_NOZORDER |
                (uint)WinAPI.SWPPara.SWP_NOOWNERZORDER |
                (uint)WinAPI.SWPPara.SWP_FRAMECHANGED);
        }

        private void SetMdiClientLocation(MdiClient mdi)
        {
            mdi.BackColor = Color.White;
            //mdi.Margin = new Padding(10);
            //mdi.Dock = DockStyle.None;

            //mdi.Location = new Point(30, 40);
            //mdi.Size = new Size(400, 260);

        }

        // used for child mdi form to notify its parent form that it's closed
        protected void MdiChildClosed(object sender, FormClosedEventArgs e)
        {            
            Form child = sender as Form;
            if (child != null)
            {
                _lastClosedMdiChild = child;
                _listClosedMdiChild.Add(child);
            }            
        }

        // for mdi child form
        protected void MdiChildVisibleChange(object sender, EventArgs e)
        {
            base.Invalidate();
        }

        public List<Form> GetCurrentMdiChildren()
        {
            List<Form> list = new List<Form>();
            if (_listClosedMdiChild != null)
            {
                foreach (Form f in MdiChildren)
                {
                    if (!_listClosedMdiChild.Contains(f) && f.Visible)
                        list.Add(f);
                }
            }
            return list;
        }

        #endregion

        #endregion

        #region private method

        private void SetFormMinimizeSize()
        {
            int minW = 160;
            int minH = 60;

            int w = BorderWidth * 2 + IconLeftMargin + IconSize.Width
                + TextLeftMargin + MinBoxSize.Width + MaxBoxSize.Width
                + CloseBoxSize.Width + ControlBoxSpace * 2
                + ControlBoxOffset.X + 12;
            if (w < minW)
                w = minW;
            int h = BorderWidth * 2 + CaptionHeight + 8;
            if (h < minH)
                h = minH;
            base.MinimumSize = new Size(w, h);
        }

        private void SetShadowFormSize()
        {
            if (_shawdowForm == null || _shawdowForm.IsDisposed)
                return;
            if (base.WindowState == FormWindowState.Normal)
            {
                var size = base.Size;
                size.Width += XTheme.ShadowWidth * 2;
                size.Height += XTheme.ShadowWidth * 2;
                if (_shawdowForm.Size != size)
                {
                    _shawdowForm.Size = size;
                    // update size -- api: movewindow....
                }
            }
        }

        private void SetShadowFormLocation()
        {
            if (_shawdowForm == null || _shawdowForm.IsDisposed)
                return;
            if (base.WindowState == FormWindowState.Normal)
            {
                Point p = base.Location;
                p.Offset(-XTheme.ShadowWidth, -XTheme.ShadowWidth);
                if (_shawdowForm.Location != p)
                {
                    _shawdowForm.Location = p;
                    // update location -- api: movewindow....
                }
            }
        }

        private void PrepareForNewTheme()
        {
            if (base.Region != null)
                base.Region.Dispose();
            base.Region = null;

            if (ControlBox)
			{
                controlBoxManager.ResetBoxColor();
				controlBoxManager.FormResize();
			}
            SetFormRegion();
            //Padding = new Padding(0);
            base.BackColor = XTheme.FormBackColor;
            base.OnSizeChanged(EventArgs.Empty);
            SetFormMinimizeSize();
            if (XTheme.ShowShadow)
            {
                if (_shawdowForm == null || _shawdowForm.IsDisposed)
                {
                    OnActivated(EventArgs.Empty);
                }
            }
            else
            {
                if (_shawdowForm != null)
                {
                    _shawdowForm.Close();
                    _shawdowForm = null;
                }
            }
        }

        private void SetFormRegion()
        {
            if (base.Region != null)
                base.Region.Dispose();

            Rectangle rect = new Rectangle(Point.Empty, base.Size);
            GraphicsPath path;

            if (XTheme.UseDefaultTopRoundingFormRegion)
                path = GraphicsPathHelper.CreateTopRoundedPathForFormRegion(rect);
            else
                path = GraphicsPathHelper.CreateRoundedRect(rect, Radius, Round, false);
            
            this.Region = new Region(path);
        }

        //private GraphicsPath CreateRoundedFormRect(bool correction)
        //{
        //    Rectangle rect = new Rectangle(Point.Empty, this.Size);
        //    return GraphicsPathHelper.CreateRoundedRect(rect, Radius, Round, correction);
        //}

        /// <summary>
        /// 判断所接收到的 wm_nc-calc-size 消息是否指示窗体即将最小化
        /// </summary>        
        private bool IsAboutToMinimize(WinAPI.RECT rect)
        {
            if(rect.Left == -32000 && rect.Top == -32000)
                return true;
            else
                return false;
        }        

        /// <summary>
        /// 判断所接收到的 wm_nc-calc-size 消息是否指示窗体即将最大化
        /// </summary>        
        private bool IsAboutToMaximize(WinAPI.RECT rect)
        {
            /*
             * 判断的方法是，只要窗体的左右、上下都延伸到了屏幕工作区之外，
             * 并且左和右、上和下都延伸相同的量，就认为窗体是要进行最大化
             */

            int left = rect.Left;
            int top = rect.Top;
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            if (left < 0 && top < 0)
            {
                Rectangle workingArea = Screen.GetWorkingArea(this);
                if (width == (workingArea.Width + (-left) * 2) 
                    && height == (workingArea.Height + (-top) * 2))
                    return true;
            }            
            return false;
        }

        private void DrawFormBackground(Graphics g)
        {
            SmoothingMode oldMode = g.SmoothingMode;
            if (Round != RoundStyle.None)
                g.SmoothingMode = SmoothingMode.AntiAlias;

            using (SolidBrush sb = new SolidBrush(XTheme.FormBackColor))
            {
                using (GraphicsPath path = GraphicsPathHelper.CreateRoundedRect(
                    ClientRectangle, Radius, Round, false))
                {                    
                    g.FillPath(sb, path);
                }
            }
            g.SmoothingMode = oldMode;
        }

        private void DrawCaptionBackground(Graphics g)
        {            
            using (LinearGradientBrush lb = new LinearGradientBrush(
                 CaptionRectToDraw,
                 XTheme.CaptionBackColorTop,
                 XTheme.CaptionBackColorBottom,
                 LinearGradientMode.Vertical))
            {
                g.FillRectangle(lb, CaptionRectToDraw);
            }
        }

        private void DrawFormIconAndText(Graphics g)
        {
            if (ShowIcon && Icon != null && XTheme.DrawCaptionIcon)
            {
                g.DrawIcon(this.Icon, IconRect);
            }

            if (!string.IsNullOrEmpty(Text) && XTheme.DrawCaptionText)
            {
                TextRenderer.DrawText(
                    g,
                    this.Text,
                    SystemFonts.CaptionFont,
                    TextRect,
                    XTheme.CaptionTextColor,
                    (XTheme.CaptionTextCenter ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left) |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis);
            }
        }

        private void DrawFormBorder(Graphics g)
        {
            int width = BorderWidth;
            Rectangle rect = ClientRectangle;

            SmoothingMode oldMode = g.SmoothingMode;
            if (Round != RoundStyle.None)
                g.SmoothingMode = SmoothingMode.AntiAlias;

            // outter border
            if (width > 0)
            {
                using (Pen p = new Pen(XTheme.FormBorderOutterColor))
                {
                    using (GraphicsPath path = GraphicsPathHelper.CreateRoundedRect(
                        rect, Radius, Round, true))
                    {
                        g.DrawPath(p, path);
                    }
                }
            }
            width--;
            
            // inner border
            if (width > 0)
            {
                using (Pen p = new Pen(XTheme.FormBorderInnerColor))
                {
                    rect.Inflate(-1, -1);
                    using (GraphicsPath path = GraphicsPathHelper.CreateRoundedRect(
                        rect, Radius, Round, true))
                    {
                        g.DrawPath(p, path);
                    }
                }
            }
            width--;

            g.SmoothingMode = oldMode;

            // other inside border
            using (Pen p = new Pen(XTheme.FormBorderInmostColor))
            {
                while (width > 0)
                {
                    rect.Inflate(-1, -1);
                    g.DrawRectangle(p, rect);
                    width--;
                }
            }
        }

        /// <summary>
        /// to make the client area to  have 3D view
        /// </summary>        
        private void DrawInsetClientRect(Graphics g)
        {
            int x = BorderWidth;
            int y = BorderWidth + CaptionHeight;
            int w = ClientSize.Width - BorderWidth * 2;
            int h = ClientSize.Height - BorderWidth * 2 - CaptionHeight;
            Rectangle clientRect = new Rectangle(x, y, w, h);
            clientRect.Width--;
            clientRect.Height--;
                        
            Color inner = ColorHelper.GetDarkerColor(this.BackColor, 20);
            clientRect.Inflate(1, 1);
            using (Pen p1 = new Pen(inner))
            {
                g.DrawRectangle(p1, clientRect);
            }
            
            Color outter = Color.FromArgb(80,255, 255, 255);
            clientRect.Inflate(1, 1);
            using (Pen p2 = new Pen(outter))
            {
                g.DrawRectangle(p2, clientRect);
            }
        }

        #endregion

        #region your Win-Message handler method

        private bool WmNcActivate(ref Message m)
        {
            // something here
            m.Result = WinAPI.TRUE;
            return true;
        }

        private bool WmNcCalcSize(ref Message m)
        {
            if (m.WParam == new IntPtr(1))
            {
                WinAPI.NCCALCSIZE_PARAMS info = (WinAPI.NCCALCSIZE_PARAMS)
                    Marshal.PtrToStructure(m.LParam, typeof(WinAPI.NCCALCSIZE_PARAMS));
                if (IsAboutToMaximize(info.rectNewForm))
                {
                    Rectangle workingRect = Screen.GetWorkingArea(this);
                    info.rectNewForm.Left = workingRect.Left - BorderWidth;
                    info.rectNewForm.Top = workingRect.Top - BorderWidth;
                    info.rectNewForm.Right = workingRect.Right + BorderWidth;
                    info.rectNewForm.Bottom = workingRect.Bottom + BorderWidth;
                    Marshal.StructureToPtr(info, m.LParam, false);
                }
            }
            return true;
        }

        private bool WmNcHitTest(ref Message m)
        {
            int para = m.LParam.ToInt32();
            int x0 = WinAPI.LOWORD(para);
            int y0 = WinAPI.HIWORD(para);
            Point p = PointToClient(new Point(x0, y0));

            if (Resizable)
            {
                if (TopLeftRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTTOPLEFT);
                    return true;
                }

                if (TopRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTTOP);
                    return true;
                }

                if (TopRightRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTTOPRIGHT);
                    return true;
                }

                if (LeftRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTLEFT);
                    return true;
                }

                if (RightRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTRIGHT);
                    return true;
                }

                if (BottomLeftRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTBOTTOMLEFT);
                    return true;
                }

                if (BottomRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTBOTTOM);
                    return true;
                }

                if (BottomRightRect.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTBOTTOMRIGHT);
                    return true;
                }
            }

            if (IconRect.Contains(p))
            {
                m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTSYSMENU);
                return true;
            }

            if (CloseBoxRect.Contains(p) || MaxBoxRect.Contains(p) || MinBoxRect.Contains(p))
            {
                m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTCLIENT);
                return true;
            }

            if (IsMdiContainer && _mdiBarController != null)
            {
                if(_mdiBarController.HitTestBounds.Contains(p))
                {
                    m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTCLIENT);
                    return true;
                }
            }

            if (CaptionRect.Contains(p))
            {
                m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTCAPTION);
                return true;
            }

            m.Result = new IntPtr((int)WinAPI.NCHITTEST.HTCLIENT);
            return true;
        }

        #endregion

        #region override method

        protected override void WndProc(ref Message m)
        {
            bool alreadyHandled = false;

            switch (m.Msg)
            {
                case (int)WinAPI.WindowMessages.WM_NCCALCSIZE:
                    alreadyHandled = WmNcCalcSize(ref m);
                    break;

                case (int)WinAPI.WindowMessages.WM_NCHITTEST:
                    alreadyHandled = WmNcHitTest(ref m);
                    break;

                case (int)WinAPI.WindowMessages.WM_NCACTIVATE:
                    alreadyHandled = WmNcActivate(ref m);
                    break;

                case (int)WinAPI.WindowMessages.WM_NCPAINT:
                    alreadyHandled = true;
                    break;

                default:
                    break;
            }

            if (!alreadyHandled)
                base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);            

            DrawFormBackground(e.Graphics);
            DrawCaptionBackground(e.Graphics);
            DrawFormBorder(e.Graphics);
            DrawFormIconAndText(e.Graphics);

            if (XTheme.SetClientInset)
                DrawInsetClientRect(e.Graphics);

            if (ControlBox)
                controlBoxManager.DrawBoxes(e.Graphics);

            if (IsMdiContainer && _mdiBarController != null)
                _mdiBarController.RenderTheBar(e.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        /// <summary>
        /// 重写该方法解决窗体每次还原都会变大的问题
        /// </summary>        
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (base.WindowState == FormWindowState.Normal)
            {
                if (this.Size == this.ClientSize)
                {
                    //if (width == (this.Size.Width + SystemInformation.FrameBorderSize.Width * 2))
                    if (width == (this.Size.Width + 4 * 2) || width == (this.Size.Width + 8 * 2))
                    {
                        width = this.Size.Width;
                        height = this.Size.Height;
                    }
                }
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        /// 重写该方法解决在VS设计器中，每次保存一个新的尺寸，再打开尺寸会变大的问题
        /// </summary>        
        protected override void SetClientSizeCore(int x, int y)
        {
            //MessageBox.Show("before SetClientSizeCore,size:" +
            //    base.Size.ToString() + ", clisize:" + base.ClientSize.ToString()
            //    + ", x:" + x.ToString() + ", y:" + y.ToString());

            //if (base.WindowState == FormWindowState.Normal)
            //{
            //    if (base.Size != base.ClientSize)
            //    {                    
            //        int diffx = Size.Width - ClientSize.Width;
            //        int diffy = Size.Height - ClientSize.Height;
            //        if (diffx == 4 * 2 || diffx == 8 * 2 || DesignMode)
            //        {
            //            x -= diffx;
            //            y -= diffy;
            //        }
            //    }
            //}
            base.SetClientSizeCore(x, y);

            //MessageBox.Show(base.SizeFromClientSize(new Size(x,y)).ToString());

            //MessageBox.Show("after SetClientSizeCore,size:" +
            //    base.Size.ToString() + ", clisize:" + base.ClientSize.ToString()
            //    + ", x:" + x.ToString() + ", y:" + y.ToString());

        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            SetFormRegion();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (_shawdowForm != null)
                _shawdowForm.Visible = base.Visible;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetFormRegion();

            if (ControlBox)
                controlBoxManager.FormResize();

            if (XTheme.ShowShadow && _shawdowForm != null
                && !_shawdowForm.IsDisposed)
            {
                if (base.WindowState == FormWindowState.Normal)
                {
                    _shawdowForm.Visible = true;
                    SetShadowFormSize();
                }
                else
                {
                    _shawdowForm.Visible = false;
                }
            }

        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            if (XTheme.ShowShadow && _shawdowForm != null 
                && !_shawdowForm.IsDisposed && _shawdowForm.Visible)
                SetShadowFormLocation();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (XTheme.ShowShadow && !base.IsMdiChild)
            {
                if (_shawdowForm == null)
                {
                    _shawdowForm = new GMShadow(this);
                    SetShadowFormSize();
                    SetShadowFormLocation();
                    _shawdowForm.Show(this);
                    SetShadowFormSize();
                    SetShadowFormLocation();

                    _shawdowForm.TopMost = base.TopMost;

                    var size = base.MinimumSize;
                    size.Width += ShadowWidth * 2;
                    size.Height += ShadowWidth * 2;
                    _shawdowForm.MinimumSize = size;

                    if (XTheme.UseShadowToResize)
                        _shawdowForm.BeginToResizeOwner();
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_shawdowForm != null && !_shawdowForm.IsDisposed)
                _shawdowForm.Close();

            base.OnFormClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (base.IsMdiContainer)
            {
                SetMdiClient();
                _listClosedMdiChild = new List<Form>();
                _mdiBarController = new MdiBarController(this);
                _mdiBarController.TabCloseButtonClick += new EventHandler(OnMdiTabCloseButtonClick);
                _mdiBarController.NewTabButtonClick +=new EventHandler(OnMdiNewTabButtonClick);
                OnMdiBarCreated(this, EventArgs.Empty);
            }
        }

        protected override void OnMdiChildActivate(EventArgs e)
        {
            base.OnMdiChildActivate(e);
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (ControlBox)
                controlBoxManager.MouseOperation(e.Location, MouseOperationType.Move);
            if (IsMdiContainer && _mdiBarController != null)
                _mdiBarController.MouseOperation(e.Location, MouseOperationType.Move);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (ControlBox)
                controlBoxManager.MouseOperation(e.Location, MouseOperationType.Down);
            if (IsMdiContainer && _mdiBarController != null)
                _mdiBarController.MouseOperation(e.Location, MouseOperationType.Down);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (e.Clicks > 1)
                return;

            if (ControlBox)
                controlBoxManager.MouseOperation(e.Location, MouseOperationType.Up);
            if (IsMdiContainer && _mdiBarController != null)
                _mdiBarController.MouseOperation(e.Location, MouseOperationType.Up);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (ControlBox)
                controlBoxManager.MouseOperation(Point.Empty, MouseOperationType.Leave);
            if (IsMdiContainer && _mdiBarController != null)
                _mdiBarController.MouseOperation(Point.Empty, MouseOperationType.Leave);
        }

        #endregion

    }
}
