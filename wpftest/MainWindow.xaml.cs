using System.Collections.Generic;
using System.Windows;
using WPF.MDI;
using System.Windows.Controls;
using System;
using System.Data;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<MenuViewModel> mMenulist = new List<MenuViewModel>();
        public static MdiContainer MainMdiContainer = new MdiContainer();
        public static string CurrentUser = string.Empty;
        public static string CompanyID = string.Empty;
        public static PlusFinder pf = new PlusFinder();

        public string mfont { get; set; }
        Lib lib = new Lib();
        public string[] strFavorites = null;

        public static List<MenuViewModel> listFavorites = new List<MenuViewModel>();
        public MenuViewModel currentMenuViewModel = null;
        public static string CurrentPerson = string.Empty;
        public static string CurrentPersonID = string.Empty;

        public int TheFont { get; set; }
        public double TheHeight { get; set; }
        public double TheWidth { get; set; }

        public static string TriggerApp = "";

        public static List<string> tempContent = new List<string>();

        #region 생성자

        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            menuLoad();

            this.Height = SystemParameters.WorkArea.Height;
            this.Width = SystemParameters.WorkArea.Width;

            mdiPanel.Height = SystemParameters.WorkArea.Height;
            mdiPanel.Children.Add(MainMdiContainer);

            uiScaleSlider.MouseDoubleClick += new MouseButtonEventHandler(RestoreScalingFactor);
            uiScaleSliderChild.MouseDoubleClick += new MouseButtonEventHandler(RestoreScalingFactor);

            // 2019.12.31 내가 추가함여 - 결재처리 화면 바로이동으로 이동시에만 결재화면 띄움
            if (TriggerApp.Trim().Equals("Approval"))
            {
                //MessageBox.Show(TriggerApp);
                gogo_Approval();
            }
            else if (TriggerApp.Trim().Equals("ApprovalReq"))
            {
                gogo_ApprovalReq();
            }
        }

        #endregion

        #region 결재처리 화면으로 이동하는 메소등

        // 재고 조회 페이지로 이동하는 버튼 이벤트
        private void gogo_Approval()
        {
            // 재고현황(제품포함)
            int i = 0;
            foreach (MenuViewModel mvm in MainWindow.mMenulist)
            {
                if (mvm.Menu.Equals("결재처리"))
                {
                    break;
                }
                i++;
            }
            try
            {
                if (MainWindow.MainMdiContainer.Children.Contains(MainWindow.mMenulist[i].subProgramID as MdiChild))
                {
                    (MainWindow.mMenulist[i].subProgramID as MdiChild).Focus();
                }
                else
                {
                    Type type = Type.GetType("WizMes_Alpha_JA." + MainWindow.mMenulist[i].ProgramID.Trim(), true);
                    object uie = Activator.CreateInstance(type);

                    MainWindow.mMenulist[i].subProgramID = new MdiChild()
                    {
                        Title = "Alpha [" + MainWindow.mMenulist[i].MenuID.Trim() + "] " + MainWindow.mMenulist[i].Menu.Trim() +
                                " (→" + MainWindow.mMenulist[i].ProgramID + ")",
                        Height = SystemParameters.PrimaryScreenHeight * 0.8,
                        MaxHeight = SystemParameters.PrimaryScreenHeight * 0.85,
                        Width = SystemParameters.WorkArea.Width * 0.85,
                        MaxWidth = SystemParameters.WorkArea.Width,
                        Content = uie as UIElement,
                        Tag = MainWindow.mMenulist[i]
                    };
                    Lib.Instance.AllMenuLogInsert(MainWindow.mMenulist[i].MenuID, MainWindow.mMenulist[i].Menu, MainWindow.mMenulist[i].subProgramID);
                    MainWindow.MainMdiContainer.Children.Add(MainWindow.mMenulist[i].subProgramID as MdiChild);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("해당 화면이 존재하지 않습니다.");
            }
        }

        #endregion // 결재처리 화면으로 이동하는 메소드

        #region 결재등록 화면으로 이동하는 메소등

        // 재고 조회 페이지로 이동하는 버튼 이벤트
        private void gogo_ApprovalReq()
        {
            // 재고현황(제품포함)
            int i = 0;
            foreach (MenuViewModel mvm in MainWindow.mMenulist)
            {
                if (mvm.Menu.Equals("결재등록"))
                {
                    break;
                }
                i++;
            }
            try
            {
                if (MainWindow.MainMdiContainer.Children.Contains(MainWindow.mMenulist[i].subProgramID as MdiChild))
                {
                    (MainWindow.mMenulist[i].subProgramID as MdiChild).Focus();
                }
                else
                {
                    Type type = Type.GetType("WizMes_Alpha_JA." + MainWindow.mMenulist[i].ProgramID.Trim(), true);
                    object uie = Activator.CreateInstance(type);

                    MainWindow.mMenulist[i].subProgramID = new MdiChild()
                    {
                        Title = "Alpha [" + MainWindow.mMenulist[i].MenuID.Trim() + "] " + MainWindow.mMenulist[i].Menu.Trim() +
                                " (→" + MainWindow.mMenulist[i].ProgramID + ")",
                        Height = SystemParameters.PrimaryScreenHeight * 0.8,
                        MaxHeight = SystemParameters.PrimaryScreenHeight * 0.85,
                        Width = SystemParameters.WorkArea.Width * 0.85,
                        MaxWidth = SystemParameters.WorkArea.Width,
                        Content = uie as UIElement,
                        Tag = MainWindow.mMenulist[i]
                    };
                    Lib.Instance.AllMenuLogInsert(MainWindow.mMenulist[i].MenuID, MainWindow.mMenulist[i].Menu, MainWindow.mMenulist[i].subProgramID);
                    MainWindow.MainMdiContainer.Children.Add(MainWindow.mMenulist[i].subProgramID as MdiChild);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("해당 화면이 존재하지 않습니다.");
            }
        }

        #endregion // 결재처리 화면으로 이동하는 메소드

        void RestoreScalingFactor(object sender, MouseButtonEventArgs args)
        {
            ((Slider)sender).Value = 1.0;
        }

        #region menuLoad

        private void menuLoad()
        {
            SettingINI setting = new SettingINI();
            setting.GetSettingINI();
            this.FontSize = setting.setFontSize;
            this.FontFamily = setting.setFontFamily;
            this.FontStyle = setting.setFontStyle;
            this.FontWeight = setting.setFontWeight;
            uiScaleSlider.Value = setting.setMainScale;
            uiScaleSliderChild.Value = setting.setChildScale;
            TheHeight = SystemParameters.WorkArea.Height;
            TheWidth = SystemParameters.WorkArea.Width;
            TheFont = (int)this.FontSize;
            setMenuList();
            setTreeMenu();
            setMainmenu();

            string[] Person = lib.SetPerson();
            CurrentPerson = Person[0];
            CurrentPersonID = Person[1];

            MainMenu.FontSize = setting.setFontSize;
            MainMenu.FontFamily = setting.setFontFamily;
            MainMenu.FontStyle = setting.setFontStyle;
            MainMenu.FontWeight = setting.setFontWeight;

            BookMarkINI bookMarkINI = new BookMarkINI();
            bookMarkINI.GetBookMarkINI();
            strFavorites = bookMarkINI.strBookMarkMenu;

            if (strFavorites != null && strFavorites.Length > 0)
            {
                SetBookMarkListBox(strFavorites);
            }
        }

        private void SetBookMarkListBox(string[] FavoritesArray)
        {
            foreach (MenuViewModel mvm in mMenulist)
            {
                foreach (string str in FavoritesArray)
                {
                    if (mvm.Menu.Trim().Equals(str))
                    {
                        listFavorites.Add(mvm);
                        ListBoxItem lbi = new ListBoxItem();
                        lbi = SetLBItem(mvm, mvm.Menu.Trim());

                        listBookMark.Items.Add(lbi);
                        break;
                    }
                }
            }
        }

        private void setMainmenu()
        {
            MenuItem mMenuItem0 = null;
            MenuItem mMenuItem1 = null;
            MenuItem mMenuItem2 = null;
            MenuItem mMenuItem3 = null;

            foreach (MenuViewModel mvm in mMenulist)
            {
                //상위 메뉴가 이상하여 확인결과  메뉴를 먼저 추가하고 다음위치 지정해야한다.
                //이전은 위치 먼저잡고 메뉴를 추가하고 있었다.
                if (mvm.Level == 0)
                {
                    mMenuItem0 = new MenuItem() { Header = mvm.Menu, Tag = mvm };
                    if (mMenuItem0 != null)
                    {
                        if (!Lib.Instance.Right(mvm.MenuID.Replace(" ", ""), 1).Equals("0"))
                        {
                            mMenuItem0.Header = mvm.MenuID + "." + mvm.Menu;
                            MainMenu.Items.Add(mMenuItem0);
                            mMenuItem0.MouseLeftButtonUp += fmenu_click;
                        }
                        else
                        {
                            MainMenu.Items.Add(mMenuItem0);
                        }
                    };

                    //mMenuItem0 = new MenuItem() { Header = mvm.Menu, Tag = mvm };
                    //if (mMenuItem0 != null) { MainMenu.Items.Add(mMenuItem0); };
                    //mMenuItem0.Click += (s, e) => { fmenu_click(s, null); };
                }
                else if (mvm.Level == 1)
                {
                    mMenuItem1 = new MenuItem() { Header = mvm.Menu, Tag = mvm };
                    if (mMenuItem1 != null) { mMenuItem0.Items.Add(mMenuItem1); };
                }
                else if (mvm.Level == 2)
                {
                    mMenuItem2 = new MenuItem() { Header = mvm.Menu, Tag = mvm };
                    if (mMenuItem2 != null) { mMenuItem1.Items.Add(mMenuItem2); };
                }
                else if (mvm.Level == 3)
                {
                    mMenuItem3 = new MenuItem() { Header = mvm.Menu + "(" + mvm.MenuID + ")", Tag = mvm };
                    if (mMenuItem3 != null) { mMenuItem2.Items.Add(mMenuItem3); };
                    mMenuItem3.Click += (s, e) => { fmenu_click(s, null); };
                }
            }

        }

        private void setTreeMenu()
        {
            TreeViewItem mTreeViewItem0 = null;
            TreeViewItem mTreeViewItem1 = null;
            TreeViewItem mTreeViewItem2 = null;
            TreeViewItem mTreeViewItem3 = null;

            foreach (MenuViewModel mvm in mMenulist)
            {
                if (mvm.Level == 0)
                {
                    mTreeViewItem0 = new TreeViewItem() { Header = mvm.Menu, Tag = mvm, IsExpanded = true };
                    mTreeViewItem0.Template = (ControlTemplate)FindResource("ImageTreeViewItemEx");
                    if (mTreeViewItem0 != null)
                    {
                        if (!Lib.Instance.Right(mvm.MenuID.Replace(" ", ""), 1).Equals("0"))
                        {
                            mTreeViewItem0.Header = mvm.MenuID + "." + mvm.Menu;
                            mTree.Items.Add(mTreeViewItem0);
                            mTreeViewItem0.MouseLeftButtonUp += fmenu_click;
                        }
                        else
                        {
                            mTree.Items.Add(mTreeViewItem0);
                        }
                    };
                    //mTreeViewItem0 = new TreeViewItem() { Header = mvm.Menu, Tag = mvm };
                    //if (mTreeViewItem0 != null)
                    //{
                    //    mTree.Items.Add(mTreeViewItem0);
                    //};
                }
                else if (mvm.Level == 1)
                {
                    mTreeViewItem1 = new TreeViewItem() { Header = mvm.Menu, Tag = mvm };
                    mTreeViewItem1.Template = (ControlTemplate)FindResource("ImageTreeViewItemEx");
                    if (mTreeViewItem1 != null) { mTreeViewItem0.Items.Add(mTreeViewItem1); };
                }

                //진호염직 기초코드등록 폴더 추가요청, level 2로 추가함.
                else if (mvm.Level == 2)
                {
                    if (mvm.ProgramID == "")
                    {
                        mTreeViewItem2 = new TreeViewItem() { Header = mvm.Menu, Tag = mvm };
                        mTreeViewItem2.Template = (ControlTemplate)FindResource("ImageTreeViewItemEx");
                        if (mTreeViewItem2 != null) { mTreeViewItem1.Items.Add(mTreeViewItem2); };
                    }
                    else
                    {
                        mTreeViewItem2 = new TreeViewItem() { Header = mvm.Menu + "(" + mvm.MenuID + ")", Tag = mvm };
                        mTreeViewItem2.Template = (ControlTemplate)FindResource("ImageTreeViewItem");
                        if (mTreeViewItem2 != null) { mTreeViewItem1.Items.Add(mTreeViewItem2); };
                        mTreeViewItem2.MouseLeftButtonUp += fmenu_click;
                    }
                }
                //진호염직 기초코드등록 폴더 추가요청, level 2로 추가함. 


                else if (mvm.Level == 3)
                {
                    mTreeViewItem3 = new TreeViewItem() { Header = mvm.Menu + "(" + mvm.MenuID + ")", Tag = mvm };
                    mTreeViewItem3.Template = (ControlTemplate)FindResource("ImageTreeViewItem");
                    if (mTreeViewItem3 != null) { mTreeViewItem2.Items.Add(mTreeViewItem3); };
                    mTreeViewItem3.MouseLeftButtonUp += fmenu_click;
                }
            }
        }



        private void fmenu_click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string window_name = "";
            Type tt1 = null;
            MenuViewModel MenuViewModel = null;
            MdiChild mdiChild = null;
            object uie = null;

            if (sender is MenuItem)
            {
                MenuViewModel = (sender as MenuItem).Tag as MenuViewModel;
                window_name = MenuViewModel.MenuID;
                mdiChild = MenuViewModel.subProgramID as MdiChild;
            }
            if (sender is TreeViewItem)
            {
                MenuViewModel = (sender as TreeViewItem).Tag as MenuViewModel;
                window_name = MenuViewModel.MenuID;
                mdiChild = MenuViewModel.subProgramID as MdiChild;
            }

            if (sender is ListBoxItem)
            {
                MenuViewModel = (sender as ListBoxItem).Tag as MenuViewModel;
                window_name = MenuViewModel.MenuID;
                mdiChild = MenuViewModel.subProgramID as MdiChild;
            }

            if (MainMdiContainer.Children.Contains(mdiChild))
            {
                if (mdiChild.WindowState == WindowState.Minimized)
                {
                    mdiChild.WindowState = WindowState.Normal;
                }
                mdiChild.Focus();
            }
            else
            {
                try
                {
                    tt1 = Type.GetType("WizMes_Alpha_JA." + MenuViewModel.ProgramID.Trim(), true);
                    uie = Activator.CreateInstance(tt1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("화면로드에 실패하였습니다." + ex.ToString());
                    return;
                }

                MenuViewModel.subProgramID = new MdiChild()
                {
                    Title = "Alpha [" + MenuViewModel.MenuID + "] " + MenuViewModel.Menu + " (→" + MenuViewModel.ProgramID.Trim() + ")",
                    Height = SystemParameters.PrimaryScreenHeight * 0.8,
                    MaxHeight = SystemParameters.PrimaryScreenHeight * 0.85,
                    Width = SystemParameters.WorkArea.Width * 0.85,
                    MaxWidth = SystemParameters.WorkArea.Width,
                    Content = uie as UIElement,
                    Tag = MenuViewModel
                };

                //Lib.Instance.AllMenuLogInsert(MenuViewModel.MenuID, MenuViewModel.Menu, MenuViewModel.ProgramID);
                MainMdiContainer.Children.Add(MenuViewModel.subProgramID as MdiChild);
            }
        }

        private void CommonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            string strPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);//+ "\\WizMes_Alpha_JA.exe";
            strPath = strPath + "\\WizMes_Alpha_JA2.exe";
            startInfo.FileName = strPath;
            startInfo.Arguments = CurrentUser;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            System.Diagnostics.Process processTemp = new System.Diagnostics.Process();
            processTemp.StartInfo = startInfo;
            //processTemp.EnableRaisingEvents = true;
            try
            {
                processTemp.Start();
                //Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void setMenuList()
        {
            string str = string.Empty;


            try
            {
                string[] arg = Environment.GetCommandLineArgs();
                string[] getProcessValue = arg[1].Split('.');
                CurrentUser = getProcessValue[0];
                //CurrentUser = "20090601";

                if (getProcessValue.Length > 1)
                {
                    TriggerApp = getProcessValue[1];
                }
            }
            catch (Exception ee7)
            {
                CurrentUser = "admin";
                //CurrentUser = "20150330";
            }

            //CompanyID = Lib.Instance.LogCompany(CurrentUser);

            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Add("@sUserID", CurrentUser);
            //sqlParameter.Add("@sPgGubun", "9");
            //sqlParameter.Add("@sUserID", "admin");
            sqlParameter.Add("@sPgGubun", "8");

            DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Menu_sUserMenu", sqlParameter, false);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("조회된 데이터가 없습니다.");
                }
                else
                {
                    //사원 아이디 변경
                    var mModel = new MenuViewModel()
                    {
                        MenuID = "001",
                        Menu = "사원 변경",
                        Level = 0,
                        ParentID = "0",
                        SelectClss = "*",
                        AddNewClss = "*",
                        UpdateClss = "*",
                        DeleteClss = "*",
                        PrintClss = "*",
                        ProgramID = "ChangePerson",
                        subProgramID = "ChangePerson"
                    };

                    //str = mModel.Menu.Trim();
                    //mMenulist.Add(mModel);

                    //// AppReq
                    //var mModel1 = new MenuViewModel()
                    //{
                    //    MenuID = "002",
                    //    Menu = "결재 새로운 디자인",
                    //    Level = 0,
                    //    ParentID = "0",
                    //    SelectClss = "*",
                    //    AddNewClss = "*",
                    //    UpdateClss = "*",
                    //    DeleteClss = "*",
                    //    PrintClss = "*",
                    //    ProgramID = "Win_App_Approval_U2",
                    //    subProgramID = "Win_App_Approval_U2"
                    //};

                    //str = mModel1.Menu.Trim();
                    //mMenulist.Add(mModel1);

                    //// Win_com_Person_U
                    //var mModel2 = new MenuViewModel()
                    //{
                    //    MenuID = "003",
                    //    Menu = "사원 메뉴 차이",
                    //    Level = 0,
                    //    ParentID = "0",
                    //    SelectClss = "*",
                    //    AddNewClss = "*",
                    //    UpdateClss = "*",
                    //    DeleteClss = "*",
                    //    PrintClss = "*",
                    //    ProgramID = "Win_com_Person_U",
                    //    subProgramID = "Win_com_Person_U"
                    //};

                    //str = mModel2.Menu.Trim();
                    //mMenulist.Add(mModel2);

                    //// frm_Gte_PersonBasic_U
                    //var mModel3 = new MenuViewModel()
                    //{
                    //    MenuID = "004",
                    //    Menu = "근태 - 인사관리",
                    //    Level = 0,
                    //    ParentID = "0",
                    //    SelectClss = "*",
                    //    AddNewClss = "*",
                    //    UpdateClss = "*",
                    //    DeleteClss = "*",
                    //    PrintClss = "*",
                    //    ProgramID = "frm_Gte_PersonBasic_U",
                    //    subProgramID = "frm_Gte_PersonBasic_U"
                    //};

                    //str = mModel3.Menu.Trim();
                    //mMenulist.Add(mModel3);

                    //// frm_Gte_DailyGte_U
                    //var mModel4 = new MenuViewModel()
                    //{
                    //    MenuID = "005",
                    //    Menu = "근태 - 숫자 테스트",
                    //    Level = 0,
                    //    ParentID = "0",
                    //    SelectClss = "*",
                    //    AddNewClss = "*",
                    //    UpdateClss = "*",
                    //    DeleteClss = "*",
                    //    PrintClss = "*",
                    //    ProgramID = "frm_Gte_DailyGte_U",
                    //    subProgramID = "frm_Gte_DailyGte_U"
                    //};

                    //str = mModel4.Menu.Trim();
                    //mMenulist.Add(mModel4);

                    //// frm_Acc_Person_U
                    //var mModel5 = new MenuViewModel()
                    //{
                    //    MenuID = "005",
                    //    Menu = "사원 권한",
                    //    Level = 0,
                    //    ParentID = "0",
                    //    SelectClss = "*",
                    //    AddNewClss = "*",
                    //    UpdateClss = "*",
                    //    DeleteClss = "*",
                    //    PrintClss = "*",
                    //    ProgramID = "frm_Acc_Person_U",
                    //    subProgramID = "frm_Acc_Person_U"
                    //};

                    //str = mModel5.Menu.Trim();
                    //mMenulist.Add(mModel5);

                    DataRowCollection drc = dt.Rows;

                    foreach (DataRow item in drc)
                    {
                        var mMenuviewModel = new MenuViewModel()
                        {
                            MenuID = item["MenuID"] as string,
                            Menu = item["Menu"] as string,
                            Level = Convert.ToInt32(item["Level"]),
                            ParentID = item["ParentID"] as string,
                            SelectClss = item["SelectClss"] as string,
                            AddNewClss = item["AddNewClss"] as string,
                            UpdateClss = item["UpdateClss"] as string,
                            DeleteClss = item["DeleteClss"] as string,
                            PrintClss = item["PrintClss"] as string,
                            ////Remark = "WizMes_Alpha_JA." + item["Remark"].ToString(),
                            ////subRemark = item["Remark"] as object,
                            ProgramID = item["ProgramID"] as string,
                            subProgramID = item["ProgramID"] as object
                        };

                        //if ((mMenuviewModel.MenuID.Substring(0, 1)).Equals("0"))
                        //{
                        //    str = mMenuviewModel.Menu.Trim();
                        //    mMenulist.Add(mMenuviewModel);
                        //}
                        //else if ((mMenuviewModel.MenuID.Substring(0, 1)).Equals("1"))
                        //{
                        //    str = mMenuviewModel.Menu.Trim();
                        //    mMenulist.Add(mMenuviewModel);
                        //}
                        //else if ((mMenuviewModel.MenuID.Substring(0, 1)).Equals("3"))
                        //{
                        //    str = mMenuviewModel.Menu.Trim();
                        //    mMenulist.Add(mMenuviewModel);
                        //}
                        //else if ((mMenuviewModel.MenuID.Substring(0, 1)).Equals("5"))
                        //{
                        //    str = mMenuviewModel.Menu.Trim();
                        //    mMenulist.Add(mMenuviewModel);
                        //}
                        if ((mMenuviewModel.MenuID.Trim()).Equals("900"))
                        {
                            str = mMenuviewModel.Menu.Trim();
                            mMenulist.Add(mMenuviewModel);
                        }
                        else if ((mMenuviewModel.MenuID.Substring(0, 2)).Equals("91")
                            || (mMenuviewModel.MenuID.Substring(0, 2)).Equals("92"))
                        {
                            str = mMenuviewModel.Menu.Trim();
                            mMenulist.Add(mMenuviewModel);
                        }
                        //{
                        //if (!(mMenuviewModel.MenuID.Substring(0, 2)).Equals("31") && !(mMenuviewModel.MenuID.Substring(0, 2)).Equals("32"))
                        //{
                        //    //str = mMenuviewModel.Menu.Replace(" ", "");
                        //    str = mMenuviewModel.Menu.Trim();
                        //    mMenulist.Add(mMenuviewModel);
                        //}                            
                        //}
                    }
                }
            }
        }

        #endregion

        private void mmClick(object sender, RoutedEventArgs e)
        {
            if (this.mMenuWidth.Width != new GridLength(0))
            {
                this.mMenuWidth.Width = new GridLength(0);
            }
            else
            {
                this.mMenuWidth.Width = new GridLength(150);
            }
        }

        #region 메인화면 닫을시 종료
        protected override void OnClosing(CancelEventArgs e1)
        {
            if (MessageBox.Show("WizMes_Alpha_JA를 종료하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else
            {
                e1.Cancel = true;
            }
        }
        void _HideThisWindow()
        {
            this.Hide();
        }

        #endregion

        private void OnClosing(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("WizMes_Alpha_JA를 종료하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SaveFontSetting();
                Environment.Exit(0);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        private void btnBefore_Click(object sender, RoutedEventArgs e)
        {
            int index = -1;
            for (int i = 0; i < MainMdiContainer.Children.Count; i++)
            {
                if (MainMdiContainer.Children[i].Equals(MainMdiContainer.ActiveMdiChild))
                {
                    index = i;
                    break;
                }
            }

            if (MainMdiContainer.Children.Count > 1)
            {
                if (index > 0)
                {
                    MainMdiContainer.Children[index - 1].Focus();
                }
                else if (index == 0)
                {
                    MainMdiContainer.Children[MainMdiContainer.Children.Count - 1].Focus();
                }
            }
        }

        private void btnAfter_Click(object sender, RoutedEventArgs e)
        {
            int index = -1;
            for (int i = 0; i < MainMdiContainer.Children.Count; i++)
            {
                if (MainMdiContainer.Children[i].Equals(MainMdiContainer.ActiveMdiChild))
                {
                    index = i;
                    break;
                }
            }

            if (MainMdiContainer.Children.Count > 0)
            {
                if (index == MainMdiContainer.Children.Count - 1)
                {
                    MainMdiContainer.Children[0].Focus();
                }
                else if (index >= 0)
                {
                    MainMdiContainer.Children[index + 1].Focus();
                }
            }
        }

        //글꼴 설정
        private void SetMySetting(object sender, RoutedEventArgs e)
        {
            TheFont = (int)this.FontSize;
            PopUp.FontPopUP fontPop = new PopUp.FontPopUP(MainMenu);
            fontPop.ShowDialog();

            if (fontPop.DialogResult == true)
            {
                this.FontFamily = fontPop.ResultFontFamily;
                TheFont = (int)fontPop.ResultFontSize;
                this.FontSize = TheFont;
                this.FontStyle = fontPop.ResultTypeFace.Style;
                this.FontWeight = fontPop.ResultTypeFace.Weight;
                MainMenu.FontFamily = fontPop.ResultFontFamily;
                MainMenu.FontSize = TheFont;
                MainMenu.FontStyle = fontPop.ResultTypeFace.Style;
                MainMenu.FontWeight = fontPop.ResultTypeFace.Weight;

                SaveFontSetting();
            }
        }

        private void SaveFontSetting()
        {
            SettingINI setting = new SettingINI();
            SettingINI.myFontSize.Clear();
            SettingINI.myFontFamily.Clear();
            SettingINI.myFontStyle.Clear();
            SettingINI.myFontWeight.Clear();
            SettingINI.myFontSize.Append(MainMenu.FontSize.ToString());
            SettingINI.myFontFamily.Append(MainMenu.FontFamily.ToString());
            SettingINI.myFontStyle.Append(MainMenu.FontStyle.ToString());
            SettingINI.myFontWeight.Append(MainMenu.FontWeight.ToString());

            SettingINI.myMainScale.Clear();
            SettingINI.myChildScale.Clear();
            SettingINI.myMainScale.Append(uiScaleSlider.Value.ToString());
            SettingINI.myChildScale.Append(uiScaleSliderChild.Value.ToString());
            setting.WriteSettingINI();
        }

        private void SaveBookMark()
        {
            BookMarkINI bookMarkINI = new BookMarkINI();
            BookMarkINI.myBookMarkMenu.Clear();

            for (int i = 0; i < listBookMark.Items.Count; i++)
            {
                var mvm = (listBookMark.Items[i] as ListBoxItem).Tag as MenuViewModel;
                if (i == listBookMark.Items.Count - 1)
                {
                    BookMarkINI.myBookMarkMenu.Append(mvm.Menu.Trim());
                }
                else
                {
                    BookMarkINI.myBookMarkMenu.Append(mvm.Menu.Trim());
                    BookMarkINI.myBookMarkMenu.Append("/");
                }
            }
            bookMarkINI.WriteBookMarkINI();
        }

        //모두닫기
        private void btnAllClose_Click(object sender, RoutedEventArgs e)
        {
            MainMdiContainer.Children.Clear();
        }

        ///// <summary>
        /// 메인 컨테이너 사이즈 조절(휠로)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                uiScaleSliderChild.Value += e.Delta * 0.0001;
            }
        }


        private void btnFavoriteAddtion_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = btnFavoriteAddtion.ContextMenu;
            menu.StaysOpen = true;
            menu.IsOpen = true;
        }


        private void ShowMenuAdd_Click(object sender, RoutedEventArgs e)
        {
            listBookMark.SelectedIndex = -1;
            //현재 즐겨찾기 메뉴 추가
            if (listFavorites.Count > 0)
            {
                listFavorites.Clear();
            }

            if (listBookMark.Items.Count > 0)
            {
                foreach (ListBoxItem listItem in listBookMark.Items)
                {
                    var Compare = listItem.Tag as MenuViewModel;
                    listFavorites.Add(Compare);
                }
            }

            PopUp.FavoriterAddtionPopUP BookMarkPopUp = new PopUp.FavoriterAddtionPopUP(mMenulist, listFavorites);
            BookMarkPopUp.ShowDialog();
            if (BookMarkPopUp.DialogResult == true)
            {
                //listFavorites.Clear();
                listFavorites = BookMarkPopUp.listBMMenu;

                listBookMark.Items.Clear();
                foreach (MenuViewModel mvm in listFavorites)
                {
                    ListBoxItem lbi = new ListBoxItem();
                    lbi = SetLBItem(mvm, mvm.Menu.Trim());

                    listBookMark.Items.Add(lbi);
                }
                SaveBookMark();
            }
        }


        /// <summary>
        /// 현재 포커싱된 화면을 즐겨찾기에 추가한다
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentBookMark_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;

            for (int i = 0; i < MainMdiContainer.Children.Count; i++)
            {
                //munu번호와 menu 네임을 받는다
                if (MainMdiContainer.Children[i].Focused == true)
                {
                    currentMenuViewModel = (MainMdiContainer.Children[i].Tag as MenuViewModel);
                }
            }

            foreach (ListBoxItem listItem in listBookMark.Items)
            {

                var Compare = listItem.Tag as MenuViewModel;
                if (currentMenuViewModel == Compare)
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                ListBoxItem lbi = new ListBoxItem();
                lbi = SetLBItem(currentMenuViewModel, currentMenuViewModel.Menu.Trim());

                listBookMark.Items.Add(lbi);
                //listBookMark.Items.Add(str);
                SaveBookMark();
            }
            else
            {
                MessageBox.Show("이미 같은 이름의 메뉴가 추가되어 있습니다.");
            }
        }

        /// <summary>
        /// 메뉴번호를 tag로 저장하고 content를 네임으로 저장한다
        /// 리스트 아이템으로 contextMenu도 추가해준다
        /// </summary>
        /// <param name="strTag"></param>
        /// <param name="strItem"></param>
        /// <returns></returns>
        private ListBoxItem SetLBItem(MenuViewModel mvm, string strItem)
        {
            ListBoxItem listBoxItem = new ListBoxItem();
            ContextMenu contextMenu = new ContextMenu();

            MenuItem menuOne = new MenuItem();
            menuOne.Header = "선택화면으로 이동";
            menuOne.Tag = strItem;
            menuOne.Click += new RoutedEventHandler(btnOneMenuClick);

            MenuItem menuTwo = new MenuItem();
            menuTwo.Header = "선택화면삭제";
            menuTwo.Tag = strItem;
            menuTwo.Click += new RoutedEventHandler(btnTwoMenuClick);

            contextMenu.Items.Add(menuOne);
            contextMenu.Items.Add(menuTwo);
            listBoxItem.ContextMenu = contextMenu;
            listBoxItem.MouseRightButtonUp += new MouseButtonEventHandler(btnFavoritesMenu);
            //lbi.Click += new RoutedEventHandler(btnFavoritesMenuSee);
            listBoxItem.MouseLeftButtonUp += new MouseButtonEventHandler(btnFavoritesMenuSee);
            listBoxItem.Content = strItem;
            listBoxItem.Tag = mvm;

            return listBoxItem;
        }

        //보이는 메뉴 클릭
        private void btnFavoritesMenuSee(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbxSend = listBookMark.SelectedItem as ListBoxItem;
            fmenu_click(lbxSend, null);
        }

        //보이는 메뉴 클릭
        private void btnFavoritesMenuSee(object sender, MouseButtonEventArgs e)
        {
            var strSend = (listBookMark.SelectedItem as ListBoxItem);
            fmenu_click(strSend, null);
        }

        //선택화면의 메뉴 보이기
        private void btnFavoritesMenu(object sender, MouseButtonEventArgs e)
        {
            ContextMenu menu = (sender as ListBoxItem).ContextMenu;
            menu.StaysOpen = true;
            menu.IsOpen = true;
        }

        private void btnOneMenuClick(object sender, RoutedEventArgs e)
        {
            var strSend = (listBookMark.SelectedItem as ListBoxItem);
            fmenu_click(strSend, null);
        }

        private void btnTwoMenuClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("선택하신 항목을 즐겨찾기에서 삭제하시겠습니까?", "즐겨찾기 목록 편집", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                listBookMark.Items.Remove(listBookMark.SelectedItem);
            }
        }

        private void ChildbtnSearchEvent(object sender, ExecutedRoutedEventArgs e)
        {
            Object obj = MainMdiContainer.ActiveMdiChild.Content;

            if (obj != null)
            {
                UserControl CurrentUserControl = obj as UserControl;

                if (CurrentUserControl != null)
                {
                    object objSearch = CurrentUserControl.FindName("btnSearch");

                    if (objSearch != null)
                    {
                        if ((objSearch as Button).IsEnabled == true)
                        {
                            (objSearch as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                }
            }
        }

        private void ChildbtnCloseEvent(object sender, ExecutedRoutedEventArgs e)
        {
            Object obj = MainMdiContainer.ActiveMdiChild.Content;

            if (obj != null)
            {
                UserControl CurrentUserControl = obj as UserControl;

                if (CurrentUserControl != null)
                {
                    object objClose = CurrentUserControl.FindName("btnClose");

                    if (objClose != null)
                    {
                        if ((objClose as Button).IsEnabled == true)
                        {
                            (objClose as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                }
            }
        }

        private void ChildbtnCancelEvent(object sender, ExecutedRoutedEventArgs e)
        {
            Object obj = MainMdiContainer.ActiveMdiChild.Content;

            if (obj != null)
            {
                UserControl CurrentUserControl = obj as UserControl;

                if (CurrentUserControl != null)
                {
                    object objCancel = CurrentUserControl.FindName("btnCancel");

                    if (objCancel != null)
                    {
                        if ((objCancel as Button).Visibility == Visibility.Visible)
                        {
                            (objCancel as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                }
            }
        }

        private void ChildbtnAddEvent(object sender, ExecutedRoutedEventArgs e)
        {
            Object obj = MainMdiContainer.ActiveMdiChild.Content;

            if (obj != null)
            {
                UserControl CurrentUserControl = obj as UserControl;

                if (CurrentUserControl != null)
                {
                    object objAdd = CurrentUserControl.FindName("btnAdd");

                    if (objAdd != null)
                    {
                        if ((objAdd as Button).IsEnabled == true)
                        {
                            (objAdd as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                }
            }
        }

        private void ChildbtnUpdateEvent(object sender, ExecutedRoutedEventArgs e)
        {
            Object obj = MainMdiContainer.ActiveMdiChild.Content;

            if (obj != null)
            {
                UserControl CurrentUserControl = obj as UserControl;

                if (CurrentUserControl != null)
                {
                    object objUpdate = CurrentUserControl.FindName("btnUpdate");
                    object objEdit = CurrentUserControl.FindName("btnEdit");

                    if (objUpdate != null)
                    {
                        if ((objUpdate as Button).IsEnabled == true)
                        {
                            (objUpdate as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                    else if (objEdit != null)
                    {
                        if ((objEdit as Button).IsEnabled == true)
                        {
                            (objEdit as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                }
            }
        }

        private void ChildbtnDeleteEvent(object sender, ExecutedRoutedEventArgs e)
        {
            Object obj = MainMdiContainer.ActiveMdiChild.Content;

            if (obj != null)
            {
                UserControl CurrentUserControl = obj as UserControl;

                if (CurrentUserControl != null)
                {
                    object objDelete = CurrentUserControl.FindName("btnDelete");

                    if (objDelete != null)
                    {
                        if ((objDelete as Button).IsEnabled == true)
                        {
                            (objDelete as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                }
            }
        }

        private void ChildbtnSaveEvent(object sender, ExecutedRoutedEventArgs e)
        {
            Object obj = MainMdiContainer.ActiveMdiChild.Content;

            if (obj != null)
            {
                UserControl CurrentUserControl = obj as UserControl;

                if (CurrentUserControl != null)
                {
                    object objSave = CurrentUserControl.FindName("btnSave");

                    if (objSave != null)
                    {
                        if ((objSave as Button).Visibility == Visibility.Visible)
                        {
                            (objSave as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                }
            }
        }

        private void ChildbtnExcelEvent(object sender, ExecutedRoutedEventArgs e)
        {
            Object obj = MainMdiContainer.ActiveMdiChild.Content;

            if (obj != null)
            {
                UserControl CurrentUserControl = obj as UserControl;

                if (CurrentUserControl != null)
                {
                    object objExcel = CurrentUserControl.FindName("btnExcel");

                    if (objExcel != null)
                    {
                        if ((objExcel as Button).IsEnabled == true)
                        {
                            (objExcel as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                }
            }
        }

        private void ChildbtnPrintEvent(object sender, ExecutedRoutedEventArgs e)
        {
            Object obj = MainMdiContainer.ActiveMdiChild.Content;

            if (obj != null)
            {
                UserControl CurrentUserControl = obj as UserControl;

                if (CurrentUserControl != null)
                {
                    object objPrint = CurrentUserControl.FindName("btnPrint");

                    if (objPrint != null)
                    {
                        if ((objPrint as Button).IsEnabled == true)
                        {
                            (objPrint as Button).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                    }
                }
            }
        }

        private void btnUpAndDown_Click(object sender, RoutedEventArgs e)
        {
            if (bdrFavorite.Width < 20)
            {
                btnUpAndDown.Content = "-";
                bdrFavorite.Width = 250;
                listBookMark.Visibility = Visibility.Visible;
                tbkFavorite.Text = "즐겨찾기 접기"
;
            }
            else if (bdrFavorite.Width > 200)
            {
                btnUpAndDown.Content = "+";
                bdrFavorite.Width = 15;
                listBookMark.Visibility = Visibility.Hidden;
                tbkFavorite.Text = "즐겨찾기 펼치기";
            }
        }

        public bool inactivated = false;

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            //if (inactivated == false)
            //{
            //    btnFavorite.IsEnabled = false;
            //    inactivated = true;
            //}
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            //Console.Write("안호잇");
            //btnFavorite.IsEnabled = true;
        }
    }

    public class MenuViewModel
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public string MenuID { get; set; }
        public string Menu { get; set; }
        public int Level { get; set; }
        public string ParentID { get; set; }
        public string SelectClss { get; set; }
        public string AddNewClss { get; set; }
        public string UpdateClss { get; set; }
        public string DeleteClss { get; set; }
        public string PrintClss { get; set; }
        public string seq { get; set; }
        public string Remark { get; set; }
        public object subRemark { get; set; }
        public string ProgramID { get; set; }
        public object subProgramID { get; set; }
    }
}
