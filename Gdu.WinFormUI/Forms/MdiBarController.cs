/*
 * 本代码受中华人民共和国著作权法保护，作者仅授权下载代码之人在学习和交流范围内
 * 自由使用与修改代码；欲将代码用于商业用途的，请与作者联系。
 * 使用本代码请保留此处信息。作者联系方式：ping3108@163.com, 欢迎进行技术交流
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Gdu.WinFormUI.MyGraphics;

namespace Gdu.WinFormUI
{
    /// <summary>
    /// 该类处理mdi-bar的绘制及鼠标事件管理
    /// </summary>
    internal class MdiBarController
    {
        #region private var

        GMForm _owner;
        List<MdiGMTabItem> _listTabItems;
        
        // 有时候可能有很多tab,不能全部显示，那么就用两个变量来标识
        // 是第几个到第几个tab被显示出来了。
        int _beginShowIndex;
        int _endShowIndex;

        int _activeTabIndex;

        // 有多少tab被隐藏了
        int _hiddenTabsCount;

        Point _lastMouseUpLocation;

        WLButton _newTabBtn;
        WLButton _listAllBtn;

        EventHandler _newTabBtnClick;
        EventHandler _tabCloseBtnClick;

        ContextMenuStrip _menuPopup;

        Rectangle _hitTestBounds;

        #endregion

        #region event & event handler

        public event EventHandler NewTabButtonClick
        {
            add { _newTabBtnClick = value; }
            remove { _newTabBtnClick = null; }
        }

        public event EventHandler TabCloseButtonClick
        {
            add { _tabCloseBtnClick = value; }
            remove { _tabCloseBtnClick = null; }
        }

        private void OnNewTabBtnClick(object sender, EventArgs e)
        {           
            if (_newTabBtnClick != null)
                _newTabBtnClick(this, EventArgs.Empty);
        }

        private void OnTabCloseBtnClick(object sender, EventArgs e)
        {
            Form f = sender as Form;
            if (f != null)
            {                
                if (_tabCloseBtnClick != null)
                {
                    _tabCloseBtnClick(f, EventArgs.Empty);
                }
            }
        }

        private void OnListAllBtnClick(object sender, EventArgs e)
        {
            if (_listTabItems.Count < 1)
                return;
            
            foreach (MdiGMTabItem tab in _listTabItems)
            {
                if (tab.IsFormActive)
                {
                    tab.MenuItemPop.Checked = true;
                    tab.MenuItemPop.Image = null;
                }
                else
                {
                    tab.MenuItemPop.Checked = false;
                    tab.MenuItemPop.Image = tab.form.Icon.ToBitmap();
                }
                tab.MenuItemPop.Text = tab.form.Text;
                
                if (!tab.IsHidden)
                    tab.MenuItemPop.Font = new Font(tab.MenuItemPop.Font, FontStyle.Bold);
                else
                    tab.MenuItemPop.Font = new Font(tab.MenuItemPop.Font, FontStyle.Regular);
            }            
            _menuPopup.Show(_owner, ListAllBtnBounds.Left, ListAllBtnBounds.Bottom);
        }

        private void OnActivateChild(Form child)
        {
            if (child != _owner.ActiveMdiChild)
            {
                if (_owner.XTheme.Mdi_UseMsgToActivateChild)
                    _owner.ActivateMdiChildForm(child);
                else
                    child.Activate();                
            }
        }

        private void OnPopMenuItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem ctl = sender as ToolStripMenuItem;
            if (ctl != null)
            {
                Form f = ctl.Tag as Form;
                if (f != null)
                {
                    OnActivateChild(f);
                }
            }
        }

        #endregion

        #region Constructor & initial

        public MdiBarController(GMForm owner)
        {
            _owner = owner;
            _listTabItems = new List<MdiGMTabItem>();
            _newTabBtn = new WLButton(owner);
            _listAllBtn = new WLButton(owner);

            // ini _newtabbtn
            _newTabBtn.ColorTable = NewTabBtnColor;              
            _newTabBtn.Click += new EventHandler(OnNewTabBtnClick);
            _newTabBtn.ForePathGetter = new ButtonForePathGetter(
                Gdu.WinFormUI.MyGraphics.GraphicsPathHelper.CreatePlusFlag);

            _listAllBtn.ColorTable = ListAllBtnColor;
            _listAllBtn.Click +=new EventHandler(OnListAllBtnClick);
            _listAllBtn.ForePathGetter = new ButtonForePathGetter(
                Gdu.WinFormUI.MyGraphics.GraphicsPathHelper.CreateDownTriangleFlag);
            _listAllBtn.ForePathSize = new Size(10, 9);
            _listAllBtn.ForeFont = new Font("微软雅黑", 8);

            _menuPopup = new ContextMenuStrip();
        }

        #endregion

        #region editable properties from thmem info

        /// <summary>
        /// use left,top,right to location the bar in owner form,
        /// the margin is from Form.ClientRectangle
        /// </summary>
        public Padding Margin
        {
            get { return _owner.XTheme.Mdi_BarMargin; }
        }

        /// <summary>
        /// the whole height of the bar, including bottom region, tab, tab-top-space
        /// </summary>
        public int BarHeight
        {
            get { return BarBottomRegionHeight + Math.Max(TabHeight, TabHeightActive) + TabTopSpace; }
        }

        /// <summary>
        /// the region under the tabs
        /// </summary>
        public int BarBottomRegionHeight
        {
            get { return _owner.XTheme.Mdi_BarBottomRegionHeight; }
        }

        public bool DrawBarBorder
        {
            get { return _owner.XTheme.Mdi_DrawBarBorder; }
        }

        public bool DrawBarBackgound
        {
            get { return _owner.XTheme.Mdi_DrawBarBackground; }
        }

        /// <summary>
        /// the bar should have inner padding left & right
        /// </summary>
        public int BarLeftPadding
        {
            get { return _owner.XTheme.Mdi_BarLeftPadding; }
        }
        public int BarRightPadding
        {
            get { return _owner.XTheme.Mdi_BarRightPadding; }
        }

        public int TabHeight
        {
            get { return _owner.XTheme.Mdi_TabHeight; }
        }

        public int TabHeightActive
        {
            get { return _owner.XTheme.Mdi_TabHeightActive; }
        }

        /// <summary>
        /// tab顶部与bar顶部的空间,这个值用来计算 BarHeight
        /// </summary>
        public int TabTopSpace
        {
            get { return _owner.XTheme.Mdi_TabTopSpace; }
        }

        /// <summary>
        /// 每个标签的左右边界可以不是垂直的，而是都有一个斜度
        /// </summary>
        public int TabSlopeWidth
        {
            get { return _owner.XTheme.Mdi_TabSlopeWidth; }
        }

        /// <summary>
        /// tab与tab之间的距离，用负值可以使tab有重叠的效果
        /// </summary>
        public int TabAndTabSpace
        {
            get { return _owner.XTheme.Mdi_TabAndTabSpace; }            
        }

        public bool ShowTabIcon
        {
            get { return _owner.XTheme.Mdi_ShowTabIcon; }
        }

        public Size IconSize
        {
            get { return ShowTabIcon ? new Size(16, 16) : Size.Empty; }
        }
        
        public int IconLeftSpace
        {
            get { return TabSlopeWidth + (ShowTabIcon ? 2 : 0); }
        }

        public int TextLeftSpace
        {
            get { return 2; }
        }

        public Size TabCloseBtnSize
        {
            get { return new Size(16, 16); }
        }

        public int TabCloseBtnRightSpace
        {
            get { return TabSlopeWidth + 2; }
        }

        public bool ShowNewTabBtn
        {
            get { return _owner.XTheme.Mdi_ShowNewTabBtn; }
        }

        public Size NewTabBtnSize
        {
            get { return _owner.XTheme.Mdi_NewTabBtnSize; }
        }

        public Rectangle NewTabBtnBounds
        {
            get;
            set;
        }

        public int NewTabBtnLeftSpace
        {
            get { return _owner.XTheme.Mdi_NewTabBtnLeftSpace; }
        }

        /// <summary>
        /// 按钮与Bar底部的距离
        /// </summary>
        public int NewTabBtnBottomSpace
        {
            get { return _owner.XTheme.Mdi_NewTabBtnBottomSpace; }
        }

        public Rectangle ListAllBtnBounds { get; set; }

        public Size ListAllBtnSize
        {
            get { return _owner.XTheme.Mdi_ListAllBtnSize; }
        }

        public int ListAllBtnLeftSpace
        {
            get { return _owner.XTheme.Mdi_ListAllBtnLeftSpace; }
        }

        public int ListAllBtnBottomSpace
        {
            get { return _owner.XTheme.Mdi_ListAllBtnBottomSpace; }
        }

        public BarButtonAlignmentType ListAllBtnAlign
        {
            get { return _owner.XTheme.Mdi_ListAllBtnAlign; }
        }

        public bool AlwaysShowListAllBtn
        { get { return _owner.XTheme.Mdi_AlwaysShowListAllBtn; } }

        /// <summary>
        /// 一个tab允许的最小宽度
        /// </summary>
        public int TabMinWidth
        {
            get 
            {
                return Math.Max(TabSlopeWidth * 2 + IconLeftSpace + IconSize.Width
                    + TabCloseBtnRightSpace + TabCloseBtnSize.Width + 36,
                    _owner.XTheme.Mdi_TabMinWidth);
            }
        }

        /// <summary>
        /// 一个tab允许的最大显示宽度
        /// </summary>
        public int TabMaxWidth
        {
            get { return _owner.XTheme.Mdi_TabMaxWidth; }
        }

        public int TabNormalWidth
        {
            get { return _owner.XTheme.Mdi_TabNormalWidth; }
        }

        #region bar & tab color

        public Color BarBorderColor
        {
            get { return _owner.XTheme.Mdi_BarBorderColor; }
        }

        public Color BarBackColor
        {
            get { return _owner.XTheme.Mdi_BarBackColor; }
        }        

        public Color TabOutterBorderColor
        {
            get { return _owner.XTheme.Mdi_TabOutterBorderColor; }
        }

        public Color TabInnerBorderColor
        {
            get { return _owner.XTheme.Mdi_TabInnerBorderColor; }
        }

        public Color TabActiveBackColorTop
        {
            get { return _owner.XTheme.Mdi_TabActiveBackColorTop; }
        }

        public Color TabActiveBackColorBottom
        {
            get { return _owner.XTheme.Mdi_TabActiveBackColorBottom; }
        }

        public Color TabDeactiveBackColorTop
        {
            get { return _owner.XTheme.Mdi_TabDeactiveBackColorTop; }
        }

        public Color TabDeactiveBackColorBottom
        {
            get { return _owner.XTheme.Mdi_TabDeactiveBackColorBottom; }
        }

        public Color BarBottomRegionBackColor
        {
            get { return _owner.XTheme.Mdi_BarBottomRegionBackColor; }
        }

        #endregion

        #region tab btn color

        public ButtonColorTable NewTabBtnColor
        {
            get { return _owner.XTheme.Mdi_NewTabBtnColor; }
        }                

        public ButtonColorTable ListAllBtnColor
        {
            get { return _owner.XTheme.Mdi_ListAllBtnColor; }
        }

        public ButtonColorTable TabCloseBtnColor
        {
            get { return ButtonColorTable.GetDefaultCloseBtnColor(); }
        }

        #endregion

        #endregion

        #region Calculated Properties

        public int BarWidth
        {
            get { return _owner.ClientSize.Width - Margin.Left - Margin.Right; }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    _owner.ClientRectangle.Left + Margin.Left,
                    _owner.ClientRectangle.Top + Margin.Top,
                    BarWidth,
                    BarHeight);
            }
        }

        public Rectangle BarBottomRegionBounds
        {
            get
            {
                return new Rectangle(
                    Bounds.Left,
                    Bounds.Top + (Bounds.Height - BarBottomRegionHeight),
                    BarWidth,
                    BarBottomRegionHeight);
            }
        }

        public Rectangle HitTestBounds
        {
            get { return _hitTestBounds; }
        }

        #endregion

        #region private method        

        private MdiGMTabItem CreateNewTab(Form f)
        {
            MdiGMTabItem tab = new MdiGMTabItem(this);
            tab.form = f;
            tab.CloseBtn = new WLButton(_owner);
            tab.CloseBtn.ColorTable = TabCloseBtnColor;
            tab.CloseBtn.BorderType = ButtonBorderType.Ellipse;
            tab.CloseBtn.DrawForePathTwice = true;
            //tab.CloseBtn.DrawLightGlass = true;
            tab.CloseBtn.ClickSendBackOject = f;
            tab.CloseBtn.ForePathGetter = new
                ButtonForePathGetter(Gdu.WinFormUI.MyGraphics.GraphicsPathHelper.CreateSingleLineCloseFlag);
            tab.CloseBtn.Click += new EventHandler(OnTabCloseBtnClick);

            // menu item
            tab.MenuItemPop = new ToolStripMenuItem();
            tab.MenuItemPop.Tag = f;
            tab.MenuItemPop.Click += new EventHandler(OnPopMenuItemClick);
            _menuPopup.Items.Add(tab.MenuItemPop);
            return tab;
        }

        /// <summary>
        /// 同步_listTabItems与mdi主窗体实际存在的子form
        /// </summary>
        private void CheckTabItemsWithOwnerMdiForm()
        {
            List<Form> listCurrent = _owner.GetCurrentMdiChildren();
            List<Form> newForm = new List<Form>();

            foreach (MdiGMTabItem tab in _listTabItems)
                tab.IsStillValid = false;

            foreach (Form f in listCurrent)
            {
                MdiGMTabItem tab;
                if (IsTabItemsContaining(f, out tab))
                    tab.IsStillValid = true;
                else
                    newForm.Add(f);
            }

            // remove the closed form tab
            for (int i = _listTabItems.Count - 1; i >= 0; i--)
                if (!_listTabItems[i].IsStillValid)
                {
                    _menuPopup.Items.Remove(_listTabItems[i].MenuItemPop);
                    _listTabItems[i].MenuItemPop.Dispose();
                    _listTabItems.RemoveAt(i);
                }

            // create tabs for newly added forms
            foreach (Form f in newForm)
            {                
                _listTabItems.Add(CreateNewTab(f));
            }

            // find the active tab
            int j = 0;
            foreach (MdiGMTabItem tab in _listTabItems)
            {
                if (tab.form == _owner.ActiveMdiChild)
                {
                    _activeTabIndex = j;
                    tab.IsFormActive = true;
                }
                else
                    tab.IsFormActive = false;
                j++;
            }
        }

        private bool IsTabItemsContaining(Form f, out MdiGMTabItem theTab)
        {
            theTab = null;
            foreach (MdiGMTabItem tab in _listTabItems)
            {
                if (f == tab.form)
                {
                    theTab = tab;
                    return true;
                }
            }
            return false;
        }        

        /// <summary>
        /// 为每个tab分配空间，如果tab数目太多，则有些会被隐藏
        /// </summary>
        private void CalculateSpaceForEachTab()
        {
            int beginOld = _beginShowIndex;
            int endOld = _endShowIndex;
            _beginShowIndex = 0;
            _endShowIndex = _listTabItems.Count - 1;
            _hiddenTabsCount = 0;

            int availableWidth = BarWidth - BarLeftPadding - BarRightPadding;
            if (ShowNewTabBtn)
                availableWidth -= (NewTabBtnSize.Width + NewTabBtnLeftSpace);
            if (AlwaysShowListAllBtn)
                availableWidth -= (ListAllBtnSize.Width + ListAllBtnLeftSpace * 2);
            //int tabCount = _listTabItems.Count;

            int widthAll = 0;
            int tabShorterThanNormalCount = 0;
            int tabLongerThanMinWidthCount = 0;
            int index = 0;
            foreach (MdiGMTabItem tab in _listTabItems)
            {
                tab.FinalWidth = tab.GetFullShowTabWidth();
                widthAll += (tab.FinalWidth + (index > 0 ? TabAndTabSpace : 0));
                if (tab.FinalWidth < TabNormalWidth)
                    tabShorterThanNormalCount++;
                if (tab.FinalWidth > TabMinWidth && index != _activeTabIndex)
                    tabLongerThanMinWidthCount++;
                index++;
            }

            if (widthAll <= availableWidth)
            {
                #region 1. 能以全文本方法显示所有
                int extraWidth = availableWidth - widthAll;

                //把剩余的空间分配给那些还没有达到normal宽度的tab
                index=0;
                while (extraWidth > 0 && tabShorterThanNormalCount > 0)
                {
                    if (_listTabItems[index].FinalWidth < TabNormalWidth)
                    {
                        if (extraWidth > 2)
                        {
                            _listTabItems[index].FinalWidth += 2;
                            extraWidth -= 2;
                        }
                        else
                        {
                            _listTabItems[index].FinalWidth += extraWidth;
                            extraWidth = 0;
                        }
                        if (_listTabItems[index].FinalWidth >= TabNormalWidth)
                            tabShorterThanNormalCount--;
                    }
                    index++;
                    if (index >= _listTabItems.Count)
                        index = 0;
                }
                return;
                #endregion
            }
            

            // 空间有限不能以全文本方式显示所有tab，则统一缩小非active的tab
            int neededWidth = widthAll - availableWidth;

            #region 缩小非active tab,直到其达到tabminwidth
            // 统一缩小非active-tab，看是否能在所有tab达到min宽度前腾出空间
            index = 0;
            while (neededWidth > 0 && tabLongerThanMinWidthCount > 0)
            {
                if (_listTabItems[index].FinalWidth > TabMinWidth && index != _activeTabIndex)
                {
                    int diff = _listTabItems[index].FinalWidth - TabMinWidth;
                    if (diff > 2)
                    {
                        _listTabItems[index].FinalWidth -= 2;
                        neededWidth -= 2;
                    }
                    else
                    {
                        _listTabItems[index].FinalWidth = TabMinWidth;
                        neededWidth -= diff;
                    }
                    if (_listTabItems[index].FinalWidth <= TabMinWidth)
                        tabLongerThanMinWidthCount--;
                }
                index++;
                if (index >= _listTabItems.Count)
                    index = 0;
            }
            #endregion

            if (neededWidth <= 0)
            {
                // 2. 缩小非actvie-tab后，可以全部显示
                return;
            }

            if ((_listTabItems[_activeTabIndex].FinalWidth > TabNormalWidth))
            {
                // 若active-tab的宽度大于normal，则逐渐减小到normal                
                int width = _listTabItems[_activeTabIndex].FinalWidth;
                while (width > TabNormalWidth && neededWidth > 0)
                {
                    width--;
                    neededWidth--;
                }
                _listTabItems[_activeTabIndex].FinalWidth = width;
                if (neededWidth <= 0)
                {
                    // 3. 缩小active-tab达到normal宽度后，可以显示全部标签
                    return;
                }

            }
            
            #region 4. 隐藏相应的tab
            if (!AlwaysShowListAllBtn)
            {
                availableWidth -= (ListAllBtnSize.Width + ListAllBtnLeftSpace * 2);
                neededWidth += (ListAllBtnSize.Width + ListAllBtnLeftSpace * 2);
            }
            if (availableWidth < _listTabItems[_activeTabIndex].FinalWidth)
            {
                _listTabItems[_activeTabIndex].FinalWidth = availableWidth;
                if (availableWidth < (TabCloseBtnSize.Width + TabSlopeWidth * 2))
                    _listTabItems[_activeTabIndex].FinalWidth = TabCloseBtnSize.Width + TabSlopeWidth * 2;
            }
            _hiddenTabsCount = neededWidth / (TabMinWidth + TabAndTabSpace);
            if ((neededWidth % (TabMinWidth + TabAndTabSpace)) != 0)
                _hiddenTabsCount++;
            if (_hiddenTabsCount >= _listTabItems.Count)
                _hiddenTabsCount = _listTabItems.Count - 1;

            //先看一下之前所隐藏的tab数是否与将要隐藏的tab数一致，如果一致，
            //且不影响active的显示，则不需要改变之前所隐藏的tab
            //若不行，则简单点,先隐藏右边，不够的再隐藏左边
            #region 设置哪些tabs被隐藏
            int showTabCount = endOld - beginOld + 1;
            if ((_listTabItems.Count - _hiddenTabsCount) == showTabCount)
            {
                if (endOld > _listTabItems.Count - 1)
                {
                    endOld = _listTabItems.Count - 1;
                    beginOld = endOld - showTabCount + 1;
                }

                if (beginOld <= _activeTabIndex && endOld >= _activeTabIndex)
                {
                    _beginShowIndex = beginOld;
                    _endShowIndex = endOld;
                }
                else
                {
                    //平移一下，使_actvieTabIndex包含其中
                    if (_activeTabIndex < beginOld)
                    {
                        _beginShowIndex = _activeTabIndex;
                        _endShowIndex = _activeTabIndex + showTabCount - 1;
                    }
                    else
                    {
                        _endShowIndex = _activeTabIndex;
                        _beginShowIndex = _activeTabIndex - showTabCount + 1;
                    }
                }
            }
            else
            {
                //先隐藏右边，不够的再隐藏左边
                int tabCntAfterActive = _listTabItems.Count - (_activeTabIndex + 1);
                if (tabCntAfterActive >= _hiddenTabsCount)
                {
                    _beginShowIndex = 0;
                    _endShowIndex = _listTabItems.Count - 1 - _hiddenTabsCount;
                }
                else
                {
                    _beginShowIndex = _hiddenTabsCount - tabCntAfterActive;
                    _endShowIndex = _activeTabIndex;
                }
            }
            #endregion

            // 再次把多出的空间重新分配给能显示的tab
            int remainWidth = _hiddenTabsCount * (TabMinWidth + TabAndTabSpace) - neededWidth;
            if (remainWidth > (_endShowIndex - _beginShowIndex + 1) * 3)
            {
                int average = remainWidth / (_endShowIndex - _beginShowIndex + 1);
                for (int i = _beginShowIndex; i <= _endShowIndex; i++)
                    _listTabItems[i].FinalWidth += average;
            }

            #endregion            
        }

        private void SetTabsVisibility()
        {
            for (int i = 0; i < _listTabItems.Count; i++)
            {
                if (i >= _beginShowIndex && i <= _endShowIndex)
                    _listTabItems[i].IsHidden = false;
                else
                    _listTabItems[i].IsHidden = true;
            }
        }

        private void SetTabsAndBarButtonBounds()
        {
            if (ListAllBtnAlign == BarButtonAlignmentType.Left)
            {
                SetListAllBtnBounds();
                SetEachTabBounds();
                SetNewTabBtnBounds();
            }
            else
            {
                SetEachTabBounds();                
                SetNewTabBtnBounds();
                SetListAllBtnBounds();
            }
            
            SetHitTestBounds();
        }

        private void SetHitTestBounds()
        {
            int right = Bounds.Right - BarRightPadding;

            if(ListAllBtnAlign != BarButtonAlignmentType.Left && 
                (AlwaysShowListAllBtn || _hiddenTabsCount>0))
            {
                right = ListAllBtnBounds.Right;
            }
            else if (ShowNewTabBtn)
            {
                right = NewTabBtnBounds.Right;
            }
            else if (_listTabItems.Count > 0 && _endShowIndex < _listTabItems.Count)
            {
                right = _listTabItems[_endShowIndex].Bounds.Right;
            }

            int left = Bounds.Left + BarLeftPadding;
            int width = right - left;

            _hitTestBounds = new Rectangle(left, Bounds.Top, width, Bounds.Height);
        }

        private void SetEachTabBounds()
        {
            int x = Bounds.X + BarLeftPadding;
            if (ListAllBtnAlign == BarButtonAlignmentType.Left)
            {
                if (AlwaysShowListAllBtn || _hiddenTabsCount > 0)
                {
                    x = ListAllBtnBounds.Right + ListAllBtnLeftSpace;
                }
            }
            if (_listTabItems.Count > 0)
            {
                for (int i = _beginShowIndex; i <= _endShowIndex; i++)
                {
                    int height = _listTabItems[i].IsFormActive ? TabHeightActive : TabHeight;                    
                    int y = Bounds.Bottom - BarBottomRegionHeight - height; 
                    int w = _listTabItems[i].FinalWidth;
                    _listTabItems[i].Bounds = new Rectangle(x, y, w, height);                    
                    x += (w + TabAndTabSpace);
                }
            }            
        }

        private void SetNewTabBtnBounds()
        {
            int tabsCount = _listTabItems.Count;
            int x;
            if (tabsCount > 0 && _endShowIndex < tabsCount)
            {
                x = _listTabItems[_endShowIndex].Bounds.Right + NewTabBtnLeftSpace;
                if (x > (Bounds.Right - BarRightPadding - NewTabBtnSize.Width))
                    x = Bounds.Right - BarRightPadding - NewTabBtnSize.Width;
                if (x < Bounds.Left + BarLeftPadding)
                    x = Bounds.Left + BarLeftPadding;
            }
            else
            {
                x = Bounds.Left + BarLeftPadding;
                if (ListAllBtnAlign == BarButtonAlignmentType.Left)
                    x = ListAllBtnBounds.Right + ListAllBtnLeftSpace;
            }            
            int y = Bounds.Bottom - NewTabBtnBottomSpace - NewTabBtnSize.Height;
            var size = ShowNewTabBtn ? NewTabBtnSize : Size.Empty;
            if (size.Width > Bounds.Width)
                size.Width = Bounds.Width;
            NewTabBtnBounds = new Rectangle(new Point(x, y), size);
        }

        private void SetListAllBtnBounds()
        {
            int x;
            if (ListAllBtnAlign == BarButtonAlignmentType.Left)
            {
                x = Bounds.X + BarLeftPadding;
            }
            else
            {
                x = NewTabBtnBounds.Right;
                if (ShowNewTabBtn || _listTabItems.Count > 0)
                    x += ListAllBtnLeftSpace;
                if (x > (Bounds.Right - BarRightPadding - ListAllBtnSize.Width))
                    x = Bounds.Right - BarRightPadding - ListAllBtnSize.Width;
            }
            int y = Bounds.Bottom - ListAllBtnBottomSpace - ListAllBtnSize.Height;
            var size = ((_hiddenTabsCount>0 || AlwaysShowListAllBtn) ? ListAllBtnSize : Size.Empty);
            if (size.Width > Bounds.Width)
                size.Width = Bounds.Width;
            ListAllBtnBounds = new Rectangle(new Point(x, y), size);
        }

        private void RenderEachBarItem(Graphics g)
        {
            Region oldClip = g.Clip;
            Region newClip = new Region(Bounds);
            g.Clip = newClip;
            SmoothingMode oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawBarBorderAndBackground(g);
            DrawEachTabItem(g);
            DrawOtherBarButtons(g);
            DrawBarBottomRegion(g);

            g.SmoothingMode = oldMode;
            g.Clip = oldClip;
            newClip.Dispose();
        }

        private void DrawBarBorderAndBackground(Graphics g)
        {
            Rectangle rect = Bounds;
            if (DrawBarBackgound)
            {
                using (SolidBrush sb = new SolidBrush(BarBackColor))
                {
                    g.FillRectangle(sb, rect);
                }
            }
            if (DrawBarBorder)
            {                
                rect.Width--;
                rect.Height--;
                using (Pen p = new Pen(BarBorderColor))
                {
                    g.DrawRectangle(p, rect);
                }
            }
        }

        private void DrawBarBottomRegion(Graphics g)
        {
            // bottom region
            using (SolidBrush sb = new SolidBrush(BarBottomRegionBackColor))
            {
                using (NewSmoothModeGraphics newGP = new NewSmoothModeGraphics(g, SmoothingMode.HighSpeed))
                {
                    g.FillRectangle(sb, BarBottomRegionBounds);
                }
            }            
        }

        private void DrawOtherBarButtons(Graphics g)
        {
            // new-tab-btn
            if (ShowNewTabBtn)
            {
                _newTabBtn.Bounds = NewTabBtnBounds;
                _newTabBtn.DrawButton(g);
            }

            // list-all-btn
            if (_hiddenTabsCount > 0 || AlwaysShowListAllBtn)
            {
                _listAllBtn.Bounds = ListAllBtnBounds;
                if (_hiddenTabsCount == 0)
                    _listAllBtn.Text = string.Empty;
                else
                    _listAllBtn.Text = _hiddenTabsCount.ToString();
                _listAllBtn.DrawButton(g);                
            }            
        }        

        private void DrawEachTabItem(Graphics g)
        {            
            foreach (MdiGMTabItem tab in _listTabItems)
            {
                if (!tab.IsHidden && !tab.IsFormActive)
                    RenderTabItem(g, tab);
            }
            // active-tab last
            if (_listTabItems.Count > 0)
                RenderTabItem(g, _listTabItems[_activeTabIndex]);    
        }

        private void RenderTabItem(Graphics g, MdiGMTabItem tab)
        {
            Rectangle rect = tab.Bounds;
            Region oldRegion = g.Clip;
            Region newRegion = new Region(rect);
            g.Clip = newRegion;

            // fill backgroung
            Color backTop,backBottom;
            if (tab.IsFormActive)
            {
                backTop = TabActiveBackColorTop;
                backBottom = TabActiveBackColorBottom;
            }
            else
            {
                backTop = TabDeactiveBackColorTop;
                backBottom = TabDeactiveBackColorBottom;
            }

            // background
            using (LinearGradientBrush lb = new LinearGradientBrush(
                rect, backTop, backBottom, LinearGradientMode.Vertical))
            {
                using (GraphicsPath path = PathGetter.GetTabBorderPath(rect, TabSlopeWidth))
                {
                    g.FillPath(lb, path);
                }
            }

            // icon
            if (ShowTabIcon && tab.form.Icon != null)
            {
                if (tab.IconRect.Right < tab.Bounds.Right)
                    g.DrawIcon(tab.form.Icon, tab.IconRect);
            }

            // text
            TextRenderer.DrawText(g, tab.form.Text, tab.form.Font, tab.TextRect, tab.form.ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // outter border
            rect.Width--;
            rect.Height--;
            using (Pen p = new Pen(TabOutterBorderColor))
            {
                if (tab.IsFormActive)
                {
                    Rectangle rectTab = tab.Bounds;
                    Rectangle rectBottom = BarBottomRegionBounds;
                    Point p1 = new Point(rectBottom.X, rectBottom.Y - 1);
                    Point p2 = new Point(rectTab.X, rectTab.Bottom - 1);
                    Point p3 = new Point(rectTab.X + TabSlopeWidth, rectTab.Y);
                    Point p4 = new Point(rectTab.Right - 1 - TabSlopeWidth, rectTab.Y);
                    Point p5 = new Point(rectTab.Right - 1, rectTab.Bottom - 1);
                    Point p6 = new Point(rectBottom.Right - 1, rectBottom.Y - 1);
                    g.Clip = oldRegion;
                    g.DrawLines(p, new Point[] { p1, p2, p3, p4, p5, p6 });
                    g.Clip = newRegion;
                }
                else
                {
                    using (GraphicsPath path = PathGetter.GetTabBorderPath(rect, TabSlopeWidth))
                    {
                        g.DrawPath(p, path);
                    }
                }
            }

            // inner border
            rect.Inflate(-1, -1);
            rect.Height++;  // 为了使bar-bottom-region 与 active-tab 的连线能覆盖非 active-tab 的底部的线
            using (Pen p = new Pen(TabInnerBorderColor))
            {
                using (GraphicsPath path = PathGetter.GetTabBorderPath(rect, TabSlopeWidth))
                {
                    g.DrawPath(p, path);
                }
            }

            // do not let the active-tab to have bottom-line
            if (tab.IsFormActive)
            {
                Rectangle rtmp = tab.Bounds;
                Point p1 = new Point(rtmp.Left + 1, rtmp.Bottom - 1);
                Point p2 = new Point(rtmp.Right - 2, rtmp.Bottom - 1);
                using (Pen pen = new Pen(backBottom))
                {
                    g.DrawLine(pen, p1, p2);
                }
            }

            //close btn            
            tab.CloseBtn.Bounds = tab.CloseBtnRect;
            tab.CloseBtn.DrawButton(g);

            g.Clip = oldRegion;
            newRegion.Dispose();
        }

        #region mouse operation

        private void MouseOperationDown(Point location)
        {            
            foreach (MdiGMTabItem tab in _listTabItems)
            {
                if (!tab.IsHidden)
                {
                    if (tab.Bounds.Contains(location) && !tab.CloseBtnRect.Contains(location))
                    {
                        tab.Capture = true;
                        break;
                    }
                }
            }
        }

        private void MouseOperationUp(Point location)
        {
            foreach (MdiGMTabItem tab in _listTabItems)
            {
                if (!tab.IsHidden)
                {
                    if (tab.Capture && tab.Bounds.Contains(location))
                    {
                        tab.Capture = false;
                        OnActivateChild(tab.form);
                        //break;
                    }
                }
                tab.Capture = false;
            }
        }

        #endregion

        #endregion

        #region public method

        /// <summary>
        /// 重绘整个标签栏
        /// </summary>
        /// <param name="g"></param>
        public void RenderTheBar(Graphics g)
        {
            // 先把已关闭的form对应的tab删除，对新加的form则为其创建相应tab
            CheckTabItemsWithOwnerMdiForm();

            if (_listTabItems.Count > 0)
            {
                // 按照指定方式为每个tab分配空间
                CalculateSpaceForEachTab();
                SetTabsVisibility();
            }

            SetTabsAndBarButtonBounds();
            
            RenderEachBarItem(g);

            if (ShowNewTabBtn)
            {
                if (_lastMouseUpLocation != Point.Empty)
                {
                    //gdu debug: currently we need this to set the btn back to normal
                    if (!NewTabBtnBounds.Contains(_lastMouseUpLocation)
                        && _newTabBtn.State == GMButtonState.Hover)
                        _newTabBtn.MouseOperation(_lastMouseUpLocation, MouseOperationType.Move);
                }
            }
        }

        /// <summary>
        /// 处理鼠标事件。该方法会触发一些click事件，重刷一些按钮等
        /// </summary>
        /// <param name="location"></param>
        /// <param name="type"></param>
        public void MouseOperation(Point location, MouseOperationType type)
        {
            //if (type == MouseOperationType.Up)
                _lastMouseUpLocation = location;
            //else
            //    _lastMouseUpLocation = Point.Empty;

            if (ShowNewTabBtn)
                _newTabBtn.MouseOperation(location, type);
            if (_hiddenTabsCount > 0 || AlwaysShowListAllBtn)
                _listAllBtn.MouseOperation(location, type);
            foreach (MdiGMTabItem tab in _listTabItems)
            {
                if (!tab.IsHidden)
                    tab.CloseBtn.MouseOperation(location, type);
            }

            switch (type)
            {
                case MouseOperationType.Down:
                    MouseOperationDown(location);
                    break;

                case MouseOperationType.Up:
                    MouseOperationUp(location);                    
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// 该类表示一个单独的标签项
    /// </summary>
    internal class MdiGMTabItem
    {

        #region 内部变量        

        #endregion

        // mdi bar which this tab item is belong to
        MdiBarController _barContainer;

        public MdiGMTabItem(MdiBarController barContainer)
        {
            _barContainer = barContainer;
            IsStillValid = true;
        }

        /// <summary>
        /// 该标签所对应的窗体
        /// </summary>
        public Form form { get; set; }

        public bool IsFormActive { get; set; }

        /// <summary>
        /// 最终分配到的整个tab的宽度
        /// </summary>
        public int FinalWidth { get; set; }

        /// <summary>
        /// 表示所对应的form是否仍未被关闭
        /// </summary>
        public bool IsStillValid { get; set; }

        public Rectangle Bounds { get; set; }

        public WLButton CloseBtn { get; set; }

        public bool Capture { get; set; }

        /// <summary>
        /// 是否因空间不足而被隐藏
        /// </summary>
        public bool IsHidden { get; set; }

        public ToolStripMenuItem MenuItemPop { get; set; }

        public Rectangle IconRect
        {
            get 
            {
                return new Rectangle(
                    Bounds.Left + _barContainer.IconLeftSpace,
                    Bounds.Top + (Bounds.Height - _barContainer.IconSize.Height) / 2,
                    _barContainer.IconSize.Width,
                    _barContainer.IconSize.Height);
            }
        }

        public Rectangle TextRect
        {
            get 
            {
                int x = IconRect.Right + _barContainer.TextLeftSpace;
                int w = CloseBtnRect.Left - x;
                if (w < 0)
                    w = 0;
                return new Rectangle(x, Bounds.Top, w, Bounds.Height); 
            }
        }

        public Rectangle CloseBtnRect
        {
            get 
            {
                return new Rectangle(
                    Bounds.Right - _barContainer.TabCloseBtnRightSpace - _barContainer.TabCloseBtnSize.Width,
                    Bounds.Top + (Bounds.Height - _barContainer.TabCloseBtnSize.Height) / 2,
                    _barContainer.TabCloseBtnSize.Width,
                    _barContainer.TabCloseBtnSize.Height);
            }
        }

        /// <summary>
        /// 返回全部显示该标签文本所需的长度
        /// </summary>
        /// <returns></returns>
        private int GetFullTextLength()
        {
            if (string.IsNullOrEmpty(form.Text))
                return 0;
            return TextRenderer.MeasureText(form.Text, form.Font).Width;
        }

        /// <summary>
        /// 返回显示全部文本时该tab所需的宽度,该宽度不超过bar-container
        /// 规定的tabmaxwidth
        /// </summary>
        /// <returns></returns>
        public int GetFullShowTabWidth()
        {
            int w = _barContainer.TabSlopeWidth * 2 + _barContainer.IconSize.Width
                + _barContainer.IconLeftSpace + _barContainer.TabCloseBtnSize.Width
                + _barContainer.TabCloseBtnRightSpace + GetFullTextLength();
            if (w > _barContainer.TabMaxWidth)
                w = _barContainer.TabMaxWidth;
            return w;
        }
    }

    public enum BarButtonAlignmentType
    {
        Left,
        //Right,
        AfterLastTab,
    }

    internal static class PathGetter
    {
        public static GraphicsPath GetTabBorderPath(Rectangle rect, int slopeWidth)
        {
            GraphicsPath path = new GraphicsPath();

            Point p1 = new Point(rect.X + slopeWidth, rect.Y);
            Point p2 = new Point(rect.Right - slopeWidth, rect.Y);
            Point p3 = new Point(rect.Right, rect.Bottom);
            Point p4 = new Point(rect.X, rect.Bottom);

            path.AddLine(p1, p2);
            path.AddLine(p3, p4);
            path.CloseFigure();

            return path;
        }
    }
}
