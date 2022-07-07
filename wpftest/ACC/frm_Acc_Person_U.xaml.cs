using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WizMes_Alpha_JA.PopUP;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// frm_Acc_Person_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class frm_Acc_Person_U : UserControl
    {
        string strFlag = string.Empty;
        int rowNum = 0;
        frm_Acc_Person_U_CodeView PersonCodeView = new frm_Acc_Person_U_CodeView();
        PersonProcessMachineCodeView PersonMachineCodeView = new PersonProcessMachineCodeView();
        ObservableCollection<PersonMenu> ovcPersonMenu = new ObservableCollection<PersonMenu>();
        public List<PersonMenu> lstPersonMenu = new List<PersonMenu>();

        public frm_Acc_Person_U()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            SetComboBox();
        }

        #region 콤보박스

        //ComboBox 전체 세팅
        private void SetComboBox()
        {
            //공급유형(조회, 입력)
            ObservableCollection<CodeView> ovcDepartSrh = ComboBoxUtil.Instance.GetCode_SetComboBoxPlusAll("Depart", null);
            cboDepartSrh.ItemsSource = ovcDepartSrh;
            cboDepartSrh.DisplayMemberPath = "code_name";
            cboDepartSrh.SelectedValuePath = "code_id";

            //ObservableCollection<CodeView> ovcDepart = ComboBoxUtil.Instance.GetCode_SetComboBox("Depart", null);
            //cboDepart.ItemsSource = ovcDepart;
            //cboDepart.DisplayMemberPath = "code_name";
            //cboDepart.SelectedValuePath = "code_id";

            //ObservableCollection<CodeView> ovcResably = ComboBoxUtil.Instance.GetCode_SetComboBox("Resably", null);
            //cboResably.ItemsSource = ovcResably;
            //cboResably.DisplayMemberPath = "code_name";
            //cboResably.SelectedValuePath = "code_id";

            //ObservableCollection<CodeView> ovcDuty = ComboBoxUtil.Instance.GetCode_SetComboBox("Duty", null);
            //cboDuty.ItemsSource = ovcDuty;
            //cboDuty.DisplayMemberPath = "code_name";
            //cboDuty.SelectedValuePath = "code_id";

            //List<string> strValue = new List<string>();
            //strValue.Add("양력");
            //strValue.Add("음력");

            //ObservableCollection<CodeView> ovcSolar = ComboBoxUtil.Instance.Direct_SetComboBox(strValue);
            //this.cboSolarClss.ItemsSource = ovcSolar;
            //this.cboSolarClss.DisplayMemberPath = "code_name";
            //this.cboSolarClss.SelectedValuePath = "code_id";

            //ObservableCollection<CodeView> ovcTeam = ComboBoxUtil.Instance.GetCode_SetComboBox("Team", null);
            //cboTeam.ItemsSource = ovcTeam;
            //cboTeam.DisplayMemberPath = "code_name";
            //cboTeam.SelectedValuePath = "code_id";
        }

        #endregion 콤보박스

        #region 버튼 컨트롤

        //취소, 저장 후
        private void CanBtnControl()
        {
            Lib.Instance.UiButtonEnableChange_IUControl(this);
            dgdMain.IsEnabled = true;
            //tabItemSetMenu.IsEnabled = false;
            tlvMenuSetting.IsEnabled = true;
            //btnSave.Visibility = Visibility.Hidden;
        }

        //추가, 수정 클릭 시
        private void CantBtnControl()
        {
            Lib.Instance.UiButtonEnableChange_SCControl(this);
            dgdMain.IsEnabled = false;
            //tabItemSetMenu.IsEnabled = true;
            tlvMenuSetting.IsEnabled = true;
            //btnSave.Visibility = Visibility.Visible;
        }

        #endregion 버튼 컨트롤

        #region 상단 왼쪽 조건부 

        // 검색조건 - 사원명 검색
        private void lblNameSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkNameSrh.IsChecked == true)
            {
                chkNameSrh.IsChecked = false;
            }
            else
            {
                chkNameSrh.IsChecked = true;
            }
        }

        private void chkNameSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkNameSrh.IsChecked = true;
            txtNameSrh.IsEnabled = true;
        }

        private void chkNameSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkNameSrh.IsChecked = false;
            txtNameSrh.IsEnabled = false;
        }

        //퇴사자 라벨 클릭 이벤트
        private void lblUseClssSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkUseClssSrh.IsChecked == true)
            {
                chkUseClssSrh.IsChecked = false;
            }
            else
            {
                chkUseClssSrh.IsChecked = true;
            }
        }

        //퇴사자 체크박스 체크 이벤트
        private void chkUseClssSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkUseClssSrh.IsChecked = true;
        }

        //퇴사자 체크박스 해제 이벤트
        private void chkUseClssSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkUseClssSrh.IsChecked = false;
        }

        #endregion 상단 왼쪽 조건부

        #region CRUD 버튼들

        //추가 : 추가는 아마 필요가 없지 않을까 싶어서 Hidden
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            strFlag = "I";
            this.DataContext = null;
            CantBtnControl();
            tlvMenuSetting.IsEnabled = true;

            //if (dgdProcess.Items.Count > 0)
            //{
            //    dgdProcess.Items.Clear();
            //}

            //dtpStartDate.SelectedDate = DateTime.Today;
            //dtpEndDate.SelectedDate = DateTime.Today;
            tbkMsg.Text = "자료 입력 중";
            MakeMenu();
            rowNum = dgdMain.SelectedIndex;
        }

        //수정
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            PersonCodeView = dgdMain.SelectedItem as frm_Acc_Person_U_CodeView;

            if (PersonCodeView != null)
            {
                // admin, master 계정 수정 유효성  체크
                if (checkUserID(PersonCodeView, "U") == false)
                    return;

                rowNum = dgdMain.SelectedIndex;
                dgdMain.IsEnabled = false;
                tbkMsg.Text = "자료 수정 중";
                strFlag = "U";
                CantBtnControl();

                PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
                usingPersonMenu(mainMenu);

            }
            else
            {
                MessageBox.Show("수정할 데이터를 선택해주세요.");
                return;
            }
        }

        //삭제
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //PersonCodeView = dgdMain.SelectedItem as frm_Acc_Person_U_CodeView;

            //if (PersonCodeView == null)
            //{
            //    MessageBox.Show("삭제할 데이터가 지정되지 않았습니다. 삭제데이터를 지정하고 눌러주세요");
            //}
            //else
            //{
            //    if (MessageBox.Show("선택하신 항목을 삭제하시겠습니까?", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //    {
            //        if (dgdMain.Items.Count > 0 && dgdMain.SelectedItem != null)
            //        {
            //            rowNum = dgdMain.SelectedIndex;
            //        }

            //        if (DeleteData(PersonCodeView.PersonID))
            //        {
            //            rowNum -= 1;
            //            re_Search(rowNum);
            //        }
            //    }
            //}
        }

        //닫기
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        //조회
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            rowNum = 0;
            re_Search(rowNum);
        }

        //저장
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var Person = dgdMain.SelectedItem as frm_Acc_Person_U_CodeView;

            if (Person != null)
            {
                if (SaveData(strFlag, Person.PersonID))
                {
                    CanBtnControl();
                    lblMsg.Visibility = Visibility.Hidden;
                    if (!strFlag.Trim().Equals("U"))
                    {
                        rowNum = 0;
                    }
                    dgdMain.IsEnabled = true;
                    strFlag = string.Empty;
                    re_Search(rowNum);
                }
            }
        }

        //취소
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CanBtnControl();

            if (!strFlag.Equals(string.Empty))
            {
                if (!strFlag.Trim().Equals("U"))
                {
                    rowNum = 0;
                }
                strFlag = string.Empty;
                re_Search(rowNum);
            }

            dgdMain.IsEnabled = true;
        }

        //엑셀
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] lst = new string[2];
            lst[0] = "사원 목록";
            lst[1] = dgdMain.Name;

            ExportExcelxaml ExpExc = new ExportExcelxaml(lst);

            ExpExc.ShowDialog();

            if (ExpExc.DialogResult.HasValue)
            {
                if (ExpExc.choice.Equals(dgdMain.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdMain);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdMain);

                    Name = dgdMain.Name;

                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else
                {
                    if (dt != null)
                    {
                        dt.Clear();
                    }
                }
            }
        }


        //Tag
        private void btnBarCode_Click(object sender, RoutedEventArgs e)
        {

        }

        //인쇄
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion CRUD 버튼들

        #region 메뉴 만들기 메서드

        //둘리
        private void MakeMenu()
        {
            if (ovcPersonMenu.Count > 0)
            {
                ovcPersonMenu.Clear();
            }
            if (lstPersonMenu.Count > 0)
            {
                lstPersonMenu.Clear();
            }
            if (tlvMenuSetting.Items.Count > 0)
            {
                tlvMenuSetting.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("sPgGubun", "8");
                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Code_sMenu", sqlParameter, false);

                if (!strFlag.Equals("I"))
                {
                    sqlParameter.Clear();
                    sqlParameter.Add("sUserID", PersonCodeView.UserID);
                    sqlParameter.Add("sPgGubun", "8");
                    DataSet dst = DataStore.Instance.ProcedureToDataSet("xp_Menu_sUserMenu", sqlParameter, false);

                    if (dst != null && dst.Tables.Count > 0)
                    {
                        DataTable dtt = dst.Tables[0];

                        if (dtt.Rows.Count > 0)
                        {
                            DataRowCollection drct = dtt.Rows;

                            foreach (DataRow drt in drct)
                            {
                                var user = new PersonMenu()
                                {
                                    Menu = drt["Menu"].ToString().Replace(" ", ""),
                                    MenuID = drt["MenuID"].ToString().Replace(" ", ""),
                                    Level = drt["Level"].ToString().Replace(" ", ""),
                                    ParentID = drt["ParentID"].ToString().Replace(" ", ""),
                                    AddNewClss = drt["AddNewClss"].ToString().Replace(" ", ""),
                                    UpdateClss = drt["UpdateClss"].ToString().Replace(" ", ""),
                                    DeleteClss = drt["DeleteClss"].ToString().Replace(" ", ""),
                                    SelectClss = drt["SelectClss"].ToString().Replace(" ", ""),
                                    PrintClss = drt["PrintClss"].ToString().Replace(" ", ""),
                                    Seq = drt["Seq"].ToString().Replace(" ", ""),
                                    ChkCount = 0
                                };

                                // 곽동운 추가 - 테스트
                                if (user.SelectClss.Equals("*"))
                                    user.ChkCount++;
                                if (user.AddNewClss.Equals("*"))
                                    user.ChkCount++;
                                if (user.UpdateClss.Equals("*"))
                                    user.ChkCount++;
                                if (user.DeleteClss.Equals("*"))
                                    user.ChkCount++;
                                if (user.PrintClss.Equals("*"))
                                    user.ChkCount++;

                                if (user.ChkCount != 0)
                                    lstPersonMenu.Add(user);
                            }
                        }
                    }
                }

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        //TreeViewItem TreeViewItems = null;
                        DataRowCollection drc = dt.Rows;
                        int i = 0;

                        PersonMenu person = new PersonMenu();
                        person.Menu = "메뉴목록";
                        person.MenuID = "0";
                        person.ParentID = "11";
                        person.Level = "A";
                        person.Children = new List<PersonMenu>();
                        //lstPersonMenu.Add(person);
                        //TreeViewItems = new TreeViewItem() { Header = person, Tag = person, IsExpanded = true };
                        int k = 0;
                        int j = 0;
                        int z = 0;


                        foreach (DataRow dr in drc)
                        {
                            i++;
                            var PMenu = new PersonMenu()
                            {
                                Num = i,
                                Menu = dr["Menu"].ToString().Replace(" ", ""),
                                MenuID = dr["MenuID"].ToString().Replace(" ", ""),
                                ParentID = dr["ParentID"].ToString().Replace(" ", ""),
                                AddNewChk = false,
                                UpdateChk = false,
                                DeleteChk = false,
                                SelectChk = false,
                                PrintChk = false,
                                UseChk = false,
                                Children = new List<PersonMenu>(),
                                ChkCount = 0,
                                ProgramID = dr["ProgramID"].ToString().Replace(" ", ""),
                            };

                            bool forFlag = true;
                            if (!strFlag.Equals("I"))
                            {
                                foreach (PersonMenu user in lstPersonMenu)
                                {
                                    if (PMenu.MenuID.Equals(user.MenuID))
                                    {
                                        PMenu.SelectClss = user.SelectClss;
                                        PMenu.AddNewClss = user.AddNewClss;
                                        PMenu.UpdateClss = user.UpdateClss;
                                        PMenu.DeleteClss = user.DeleteClss;
                                        PMenu.PrintClss = user.PrintClss;

                                        PMenu.SelectChk = user.SelectChk;
                                        PMenu.AddNewChk = user.AddNewChk;
                                        PMenu.UpdateChk = user.UpdateChk;
                                        PMenu.DeleteChk = user.DeleteChk;
                                        PMenu.PrintChk = user.PrintChk;

                                        PMenu.Seq = user.Seq;
                                        PMenu.UseChk = false;

                                        if (user.SelectClss.Equals("*"))
                                        {
                                            PMenu.SelectChk = true;
                                            PMenu.ChkCount++;
                                        }
                                        else
                                            PMenu.SelectChk = false;

                                        if (user.AddNewClss.Equals("*"))
                                        {
                                            PMenu.AddNewChk = true;
                                            PMenu.ChkCount++;
                                        }
                                        else
                                            PMenu.AddNewChk = false;

                                        if (user.UpdateClss.Equals("*"))
                                        {
                                            PMenu.UpdateChk = true;
                                            PMenu.ChkCount++;
                                        }
                                        else
                                            PMenu.UpdateChk = false;

                                        if (user.DeleteClss.Equals("*"))
                                        {
                                            PMenu.DeleteChk = true;
                                            PMenu.ChkCount++;
                                        }
                                        else
                                            PMenu.DeleteChk = false;

                                        if (user.PrintClss.Equals("*"))
                                        {
                                            PMenu.PrintChk = true;
                                            PMenu.ChkCount++;
                                        }
                                        else
                                            PMenu.PrintChk = false;

                                        if (PMenu.SelectClss.Equals("*") && PMenu.AddNewClss.Equals("*") &&
                                            PMenu.UpdateClss.Equals("*") && PMenu.DeleteClss.Equals("*") &&
                                            PMenu.PrintClss.Equals("*"))
                                        {
                                            PMenu.UseClss = "*";
                                            PMenu.UseChk = true;
                                        }

                                        forFlag = false;
                                        break;
                                    }
                                }
                            }

                            if (forFlag)
                            {
                                PMenu.SelectClss = "";
                                PMenu.AddNewClss = "";
                                PMenu.UpdateClss = "";
                                PMenu.DeleteClss = "";
                                PMenu.PrintClss = "";
                                PMenu.UseClss = "";
                            }

                            if (PMenu.ParentID.Trim().Length == 3)
                            {
                                PMenu.Level = "1";
                                person.Children[k - 1].Children.Add(PMenu);
                                j++;
                            }
                            // 진호염직에서 기초코드 등록이라는 상위 폴더를 만들어달라고 했다. .......................
                            else if (PMenu.ParentID.Trim().Length == 4)
                            {
                                if (PMenu.ProgramID == "")
                                {
                                    PMenu.Level = "2";
                                    person.Children[k - 1].Children[j - 1].Children.Add(PMenu);
                                    z++;
                                }
                                else
                                {       
                                    if (PMenu.ParentID == "9501" || PMenu.ParentID == "9539" || PMenu.ParentID == "9569")       //구분자가 없어서 하드코딩 함 몰라. 더 이상 만들지마 쓰지마.
                                    {
                                        PMenu.Level = "3";
                                        person.Children[k - 1].Children[j - 1].Children[z - 1].Children.Add(PMenu);
                                    }
                                    else
                                    {
                                        PMenu.Level = "2";
                                        person.Children[k - 1].Children[j - 1].Children.Add(PMenu);
                                        z = 0;
                                    }
                                }
                            }
                            // 진호염직에서 기초코드 등록이라는 상위 폴더를 만들어달라고 했다. .......................
                            else
                            {
                                PMenu.Level = "0";
                                person.Children.Add(PMenu);
                                k++;
                                j = 0;
                            }
                        }

                        ovcPersonMenu.Add(person);
                        tlvMenuSetting.ItemsSource = ovcPersonMenu;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion // 메뉴 만들기 메서드

        #region 권한쪽 체크 이벤트

        // 사용구분 클릭 이벤트 → 한줄 전체 클릭
        private void chkGubun_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;

            if (chkSender.IsChecked == true)
            {
                senderTreeViewItem.AddNewChk = true;
                senderTreeViewItem.AddNewClss = "*";

                senderTreeViewItem.DeleteChk = true;
                senderTreeViewItem.DeleteClss = "*";

                senderTreeViewItem.PrintChk = true;
                senderTreeViewItem.PrintClss = "*";

                senderTreeViewItem.SelectChk = true;
                senderTreeViewItem.SelectClss = "*";

                senderTreeViewItem.UpdateChk = true;
                senderTreeViewItem.UpdateClss = "*";

                //// 이건 아마 메뉴(전체선택 행)일 것이여
                //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
                //chkLstPersonMenuZero(mainMenu, true);
            }
        }

        // 사용구분 체크해제 이벤트 → 한줄 체크해제 체크
        private void chkGubun_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;

            senderTreeViewItem.AddNewChk = false;
            senderTreeViewItem.AddNewClss = "";
            senderTreeViewItem.DeleteChk = false;
            senderTreeViewItem.AddNewClss = "";
            senderTreeViewItem.PrintChk = false;
            senderTreeViewItem.AddNewClss = "";
            senderTreeViewItem.SelectChk = false;
            senderTreeViewItem.AddNewClss = "";
            senderTreeViewItem.UpdateChk = false;
            senderTreeViewItem.AddNewClss = "";

            //lstPersonMenu.RemoveAll(lamda => lamda.MenuID == senderTreeViewItem.MenuID);

            //// 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }

        // 전체를 체크하면서, 체크 된것들만 다 처리하기 → 체크할때마다 반복
        private void chkLstPersonMenuZero(PersonMenu senderTreeViewItem, bool firstFlag)
        {
            // 처음 메서드가 시작할때만, lstPersonMenu 를 초기화 시켜주기 위해서 firstFlag 를 추가
            if (firstFlag == true)
                lstPersonMenu.Clear();

            if (senderTreeViewItem.Children.Count > 0)
            {
                for (int i = 0; i < senderTreeViewItem.Children.Count; i++)
                {
                    senderTreeViewItem.Children[i].ChkCount = 0;
                    if (senderTreeViewItem.Children[i].SelectChk == true)
                    {
                        senderTreeViewItem.Children[i].ChkCount++;
                        senderTreeViewItem.Children[i].SelectClss = "*";
                    }
                    else
                        senderTreeViewItem.Children[i].SelectClss = "";

                    if (senderTreeViewItem.Children[i].AddNewChk == true)
                    {
                        senderTreeViewItem.Children[i].ChkCount++;
                        senderTreeViewItem.Children[i].AddNewClss = "*";
                    }
                    else
                        senderTreeViewItem.Children[i].AddNewClss = "";

                    if (senderTreeViewItem.Children[i].UpdateChk == true)
                    {
                        senderTreeViewItem.Children[i].ChkCount++;
                        senderTreeViewItem.Children[i].UpdateClss = "*";
                    }
                    else
                        senderTreeViewItem.Children[i].UpdateClss = "";

                    if (senderTreeViewItem.Children[i].DeleteChk == true)
                    {
                        senderTreeViewItem.Children[i].ChkCount++;
                        senderTreeViewItem.Children[i].DeleteClss = "*";
                    }
                    else
                        senderTreeViewItem.Children[i].DeleteClss = "";

                    if (senderTreeViewItem.Children[i].PrintChk == true)
                    {
                        senderTreeViewItem.Children[i].ChkCount++;
                        senderTreeViewItem.Children[i].PrintClss = "*";
                    }
                    else
                        senderTreeViewItem.Children[i].PrintClss = "";

                    if (senderTreeViewItem.Children[i].ChkCount != 0)
                    {
                        // if (lstPersonMenu.Contain(senderTreeViewItem.Children[i]) == false)
                        lstPersonMenu.Add(senderTreeViewItem.Children[i]);
                    }

                    // 만약에 하위 노드가 존재한다면 없을때까지 무한 반복
                    if (senderTreeViewItem.Children[i].Children.Count > 0)
                    {
                        chkLstPersonMenuZero(senderTreeViewItem.Children[i], false);
                    }
                }
            } // 1 끝
        }

        // 추가, 수정 상태가 아닐때 체크가 되지 않도록 막아놓은 상태임. 
        // → 수정, 추가 일때 다시 체크가 되도록 변경 (isEnabled = true 로 변경) 
        private void usingPersonMenu(PersonMenu senderTreeViewItem)
        {
            senderTreeViewItem.isEnabled = true;
            if (senderTreeViewItem.Children.Count > 0)
            {
                for (int i = 0; i < senderTreeViewItem.Children.Count; i++)
                {
                    // 입력해랑
                    senderTreeViewItem.Children[i].isEnabled = true;

                    // 만약에 하위 노드가 존재한다면 없을때까지 무한 반복
                    if (senderTreeViewItem.Children[i].Children.Count > 0)
                    {
                        usingPersonMenu(senderTreeViewItem.Children[i]);
                    }
                }
            } // 1 끝
        }


        // 사원메뉴 조회 체크
        private void chkSearch_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;
            senderTreeViewItem.SelectClss = "*";

            senderTreeViewItem.ChkCount++;
            if (lstPersonMenu.Contains(senderTreeViewItem) == false)
            {
                lstPersonMenu.Add(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }

        // 사원메뉴 조회 체크해제
        private void chkSearch_Unchecked(object sender, RoutedEventArgs e)
        {

            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;
            senderTreeViewItem.SelectClss = "";

            senderTreeViewItem.ChkCount--;
            if (senderTreeViewItem.ChkCount == 0)
            {
                lstPersonMenu.Remove(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }


        //추가
        private void chkAdd_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;
            senderTreeViewItem.AddNewClss = "*";

            senderTreeViewItem.ChkCount++;
            if (lstPersonMenu.Contains(senderTreeViewItem) == false)
            {
                lstPersonMenu.Add(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }

        // 사원메뉴 추가 체크 해제
        private void chkAdd_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;
            senderTreeViewItem.AddNewClss = "";

            senderTreeViewItem.ChkCount--;
            if (senderTreeViewItem.ChkCount == 0)
            {
                lstPersonMenu.Remove(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }


        // 사원메뉴 수정 체크
        private void chkUpdate_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;
            senderTreeViewItem.UpdateClss = "*";

            senderTreeViewItem.ChkCount++;
            if (lstPersonMenu.Contains(senderTreeViewItem) == false)
            {
                lstPersonMenu.Add(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }

        // 사원메뉴 수정 체크해제
        private void chkUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;
            senderTreeViewItem.UpdateClss = "";

            senderTreeViewItem.ChkCount--;
            if (senderTreeViewItem.ChkCount == 0)
            {
                lstPersonMenu.Remove(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }


        // 사원메뉴 삭제 체크
        private void chkDelete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;
            senderTreeViewItem.DeleteClss = "*";

            senderTreeViewItem.ChkCount++;
            if (lstPersonMenu.Contains(senderTreeViewItem) == false)
            {
                lstPersonMenu.Add(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }

        // 사원메뉴 삭제 체크해제
        private void chkDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;
            senderTreeViewItem.DeleteClss = "";

            senderTreeViewItem.ChkCount--;
            if (senderTreeViewItem.ChkCount == 0)
            {
                lstPersonMenu.Remove(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }


        //출력
        private void chkPrint_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;
            senderTreeViewItem.PrintClss = "*";

            senderTreeViewItem.ChkCount++;
            if (lstPersonMenu.Contains(senderTreeViewItem) == false)
            {
                lstPersonMenu.Add(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }
        //사원메뉴 출력 체크해제
        private void chkPrint_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu senderTreeViewItem = chkSender.DataContext as PersonMenu;
            senderTreeViewItem.PrintClss = "";

            senderTreeViewItem.ChkCount--;
            if (senderTreeViewItem.ChkCount == 0)
            {
                lstPersonMenu.Remove(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            //chkLstPersonMenuZero(mainMenu, true);
        }

        #endregion // 권한쪽 체크 이벤트

        #region 저장 SaveData

        /// <summary>
        /// 저장
        /// </summary>
        /// <param name="strFlag"></param>
        /// <param name="strID"></param>
        /// <returns></returns>
        private bool SaveData(string strFlag, string strID)
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            // 이건 아마 메뉴(전체선택 행)일 것이여
            PersonMenu mainMenu = tlvMenuSetting.Items[0] as PersonMenu;
            chkLstPersonMenuZero(mainMenu, true);

            Dictionary<string, object> sqlParameter = null;

            try
            {
                if (CheckData())
                {
                    int Seq = 0;
                    sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();
                    sqlParameter.Add("sPersonID", strID);
                    sqlParameter.Add("sPgGubun", "8");

                    Procedure pro4 = new Procedure();
                    pro4.Name = "xp_Menu_dUserMenu";
                    pro4.OutputUseYN = "N";
                    pro4.OutputName = "sPersonID";
                    pro4.OutputLength = "8";

                    Prolist.Add(pro4);
                    ListParameter.Add(sqlParameter);

                    // 테스트 : lstPersonMenu.Count
                    for (int i = 0; i < lstPersonMenu.Count; i++)
                    {
                        if (lstPersonMenu[i].Level != null &&
                            !(lstPersonMenu[i].Level.Equals("A")))  // !lstPersonMenu[i].Level.Equals("A")    lstPersonMenu[i].UseClss != null  && lstPersonMenu[i].UseClss.Equals("*")
                        {
                            Seq++;
                            sqlParameter = new Dictionary<string, object>();
                            sqlParameter.Clear();

                            sqlParameter.Add("sPersonID", strID);
                            sqlParameter.Add("sPgGubun", "8");
                            sqlParameter.Add("sMenuID", lstPersonMenu[i].MenuID);
                            sqlParameter.Add("nSeq", Seq);
                            sqlParameter.Add("nLevel", lstPersonMenu[i].Level);

                            sqlParameter.Add("sParentID", lstPersonMenu[i].ParentID);
                            sqlParameter.Add("sSelectClss", lstPersonMenu[i].SelectClss);
                            sqlParameter.Add("sAddNewClss", lstPersonMenu[i].AddNewClss);
                            sqlParameter.Add("sUpdateClss", lstPersonMenu[i].UpdateClss);
                            sqlParameter.Add("sDeleteClss", lstPersonMenu[i].DeleteClss);

                            sqlParameter.Add("sPrintClss", lstPersonMenu[i].PrintClss);
                            //sqlParameter.Add("sCreateUserID", MainWindow.CurrentUser);

                            Procedure pro3 = new Procedure();
                            pro3.Name = "xp_Menu_iUserMenu";
                            pro3.OutputUseYN = "N";
                            pro3.OutputName = "sPersonID";
                            pro3.OutputLength = "8";

                            Prolist.Add(pro3);
                            ListParameter.Add(sqlParameter);
                        }
                    }

                    string[] Confirm = new string[2];
                    Confirm = DataStore.Instance.ExecuteAllProcedureOutputNew(Prolist, ListParameter);
                    if (Confirm[0] != "success")
                    {
                        MessageBox.Show("[저장실패]\r\n" + Confirm[1].ToString());
                        flag = false;
                        //return false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return flag;
        }

        #endregion // 저장 SaveData

        #region CheckDate (체크데이터)

        /// <summary>
        /// 입력사항 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckData()
        {
            bool flag = true;

            return flag;
        }

        #endregion CheckDate (체크데이터)

        #region 기타 메서드

        // 수정 - admin, master 면 본인것만 수정되도록
        // 삭제 - admin, master 계정은 삭제가 안되도록 다 막아버리기.
        private bool checkUserID(frm_Acc_Person_U_CodeView PersonCodeView, string superFlag)
        {
            bool flag = true;

            string You = MainWindow.CurrentUser;

            if (PersonCodeView.PersonID.Trim().Equals("20191003") || PersonCodeView.PersonID.Trim().Equals("admin"))
            {
                if (superFlag.Equals("U")) // 수정의 경우
                {

                    if (!You.Trim().Equals("admin"))
                    {
                        if (!You.Equals(PersonCodeView.UserID))
                        {
                            MessageBox.Show("해당 계정의 수정 권한이 없습니다.");
                            flag = false;
                        }
                    }

                }
                else if (superFlag.Equals("D")) // 삭제의 경우
                {
                    if (!You.Trim().Equals("admin"))
                    {
                        MessageBox.Show("해당 계정은 삭제가 불가능 합니다.");
                        flag = false;
                    }
                }
            }

            return flag;
        }

        #endregion 기타 메서드

        #region 사원 삭제가 필요한가?? 뭐 어차피 사용안함 처리긴 한데

        /// <summary>
        /// 실삭제
        /// </summary>
        /// <param name="strID"></param>
        /// <returns></returns>
        private bool DeleteData(string strID)
        {
            bool flag = false;

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("PersonID", strID);
                sqlParameter.Add("EndDate", DateTime.Today.ToString("yyyyMMdd"));

                string[] result = DataStore.Instance.ExecuteProcedure("xp_Person_dPerson", sqlParameter, false);

                if (result[0].Equals("success"))
                {
                    //MessageBox.Show("성공 *^^*");
                    //flag = true;
                    if (DeleteUserMenu(strID))
                    {
                        flag = true;
                    }
                    else
                    {
                        MessageBox.Show("해당 아이디의 권한삭제 실패");
                        flag = false;
                    }
                }
                else
                {
                    MessageBox.Show("해당 아이디 삭제 실패");
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }

            return flag;
        }

        #endregion // 사원 삭제가 필요한가??

        #region 권한 삭제 메서드 DeleteUserMenu

        private bool DeleteUserMenu(string strPersonID)
        {
            bool flag = false;

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("sPersonID", strPersonID);
                sqlParameter.Add("sPgGubun", "8");

                string[] result = DataStore.Instance.ExecuteProcedure("xp_Menu_dUserMenu", sqlParameter, false);

                if (result[0].Equals("success"))
                {
                    //MessageBox.Show("성공 *^^*");
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }

            return flag;
        }

        #endregion  권한 삭제 메서드 DeleteUserMenu

        #region re_Search 

        //재조회
        private void re_Search(int selectedIndex)
        {
            FillGrid();

            if (dgdMain.Items.Count > 0)
            {
                dgdMain.SelectedIndex = selectedIndex;
            }
            else
            {
                this.DataContext = null;
            }
        }

        #endregion re_Search 

        #region 조회 FillGrid

        private void FillGrid()
        {
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                // 아무것도 선택하지 않았을때는 전체 선택
                if (cboDepartSrh.SelectedIndex == -1)
                    cboDepartSrh.SelectedIndex = 0;

                sqlParameter.Clear();
                sqlParameter.Add("nChkDepartID", cboDepartSrh.SelectedValue.ToString() == "" ? 0 : 1 );
                sqlParameter.Add("sDepartID", cboDepartSrh.SelectedValue != null ? cboDepartSrh.SelectedValue.ToString() : "");
                sqlParameter.Add("sName", chkNameSrh.IsChecked == true && !txtNameSrh.Text.Trim().Equals("") ? txtNameSrh.Text : "");
                sqlParameter.Add("sUseClss", chkUseClssSrh.IsChecked == true ? 1 : 0);
                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Person_sPerson_WPF", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        int i = 0;
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow dr in drc)
                        {
                            i++;
                            var PersonView = new frm_Acc_Person_U_CodeView()
                            {
                                Num = i,
                                PersonID = dr["PersonID"].ToString(),
                                Name = dr["Name"].ToString(),
                                UserID = dr["UserID"].ToString(),
                                PassWord = dr["PassWord"].ToString(),
                                DepartID = dr["DepartID"].ToString(),
                                Depart = dr["Depart"].ToString(),
                                DutyID = dr["DutyID"].ToString(),
                                Duty = dr["Duty"].ToString(),
                                StartDate = dr["StartDate"].ToString(),
                                EndDate = dr["EndDate"].ToString(),
                                RegistID = dr["RegistID"].ToString(),
                                HandPhone = dr["HandPhone"].ToString(),
                                Phone = dr["Phone"].ToString(),
                                BirthDay = dr["BirthDay"].ToString(),
                                SolarClss = dr["SolarClss"].ToString(),
                                ZipCode = dr["ZipCode"].ToString(),
                                OldNNewClss = dr["OldNNewClss"].ToString(),
                                GunMoolMngNo = dr["GunMoolMngNo"].ToString(),
                                Address1 = dr["Address1"].ToString(),
                                Address2 = dr["Address2"].ToString(),
                                AddressAssist = dr["AddressAssist"].ToString(),
                                AddressJiBun1 = dr["AddressJiBun1"].ToString(),
                                AddressJiBun2 = dr["AddressJiBun2"].ToString(),
                                EMail = dr["EMail"].ToString(),
                                Remark = dr["Remark"].ToString(),
                                TeamID = dr["TeamID"].ToString(),
                                Team = dr["Team"].ToString(),
                                //ResablyID = dr["ResablyID"].ToString(),
                                //Resably = dr["Resably"].ToString(),
                                CustomID = dr["CustomID"].ToString(),
                                KCustom = dr["KCustom"].ToString(),
                                //Bank = dr["Bank"].ToString()
                            };

                            if (PersonView != null && PersonView.PersonID != null)
                            {
                                // admin, master 계정일 경우, 
                                // 접속한 아이디가 둘중 하나 본인인것을 제외한, 나머지는 비밀번호가 ****로 표시되도록 → 수정, 삭제도 막아야 함
                                if (PersonView.PersonID.Trim().Equals("20191003") || PersonView.PersonID.Trim().Equals("admin"))
                                {
                                    // 접속한 유저 아이디랑 비교. 본인이 아니면, **** 처리
                                    string You = MainWindow.CurrentUser;

                                    if (!You.Trim().Equals("admin"))
                                    {
                                        // 본인 것이 아니면
                                        if (!You.Equals(PersonView.UserID.Trim()))
                                        {
                                            string Password = new string('*', PersonView.PassWord.Length);
                                            PersonView.PassWord = Password;
                                        }
                                    }

                                }
                            }

                            if (PersonView.StartDate.Length > 0)
                            {
                                PersonView.StartDate_CV = Lib.Instance.StrDateTimeBar(PersonView.StartDate);
                            }

                            if (PersonView.EndDate.Length > 0)
                            {
                                PersonView.EndDate_CV = Lib.Instance.StrDateTimeBar(PersonView.EndDate);
                            }

                            dgdMain.Items.Add(PersonView);
                        }

                        // 2019.08.28 검색결과에 갯수 추가
                        sPersonCount.Text = "▶검색 결과 : " + i + "건";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }

        #endregion // 조회 FillGrid




        // 테스트
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            string message = "리스트 갯수 : " + lstPersonMenu.Count;

            int i = 0;
            foreach (PersonMenu pm in lstPersonMenu)
            {
                i++;
                message += ", " + pm.Menu + "(" + pm.ChkCount + " : " + pm.SelectClss + pm.AddNewClss + pm.UpdateClss + pm.DeleteClss + pm.PrintClss + ")";

                if (i % 3 == 0)
                {
                    message += " \r";
                }
            }

            MessageBox.Show(message);
        }

        //더블 클릭시 수정모드로
        private void dgdMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        // 메인 그리드 선택 시
        private void dgdMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PersonCodeView = dgdMain.SelectedItem as frm_Acc_Person_U_CodeView;

            if (PersonCodeView != null)
            {
                MakeMenu();
            }
        }

        //조회
        private void btnSearchClick(object sender, RoutedEventArgs e)
        {
            rowNum = 0;
            re_Search(rowNum);
        }
    }



    class frm_Acc_Person_U_CodeView : BaseView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }
        public int Num { get; set; }
        public string PersonID { get; set; }
        public string Name { get; set; }
        public string UserID { get; set; }
        public string PassWord { get; set; }
        public string DepartID { get; set; }
        public string Depart { get; set; }
        public string DutyID { get; set; }
        public string Duty { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string RegistID { get; set; }
        public string HandPhone { get; set; }
        public string Phone { get; set; }
        public string BirthDay { get; set; }
        public string SolarClss { get; set; }
        public string ZipCode { get; set; }
        public string OldNNewClss { get; set; }
        public string GunMoolMngNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string AddressAssist { get; set; }
        public string AddressJiBun1 { get; set; }
        public string AddressJiBun2 { get; set; }
        public string EMail { get; set; }
        public string Remark { get; set; }
        public string TeamID { get; set; }
        public string Team { get; set; }
        public string ResablyID { get; set; }
        public string Resably { get; set; }
        public string CustomID { get; set; }
        public string KCustom { get; set; }
        public string Bank { get; set; }

        public string StartDate_CV { get; set; }
        public string EndDate_CV { get; set; }
        //public string useclss { get; set; }
        //public string WorkLevelID { get; set; }
        //public string WorkLevelName { get; set; }
    }

    class ProcessMachineCodeView : BaseView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public string MachineID { get; set; }
        public string Machine { get; set; }
        public string MachineNO { get; set; }
        public string SetHitCount { get; set; }
        public string ProductLocID { get; set; }
        public string ProcessID { get; set; }
        public string Process { get; set; }
        public string TdGbn { get; set; }
        public string TdCycle { get; set; }
        public string CommStationNo { get; set; }
        public string TdDate { get; set; }
        public string TdTime { get; set; }
        public string TdExchange { get; set; }
    }

    class PersonProcessMachineCodeView : BaseView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }
        public string PersonID { get; set; }
        public string ProcessID { get; set; }
        public string Process { get; set; }
        public string MachineID { get; set; }
        public string Machine { get; set; }
        public string MachineNO { get; set; }
    }

    public class PersonMenu : INotifyPropertyChanged
    {
        public int Num { get; set; }
        public string MenuID { get; set; }
        public string Menu { get; set; }
        public string ParentID { get; set; }
        public string Level { get; set; }
        public string SelectClss { get; set; }
        public string AddNewClss { get; set; }
        public string UpdateClss { get; set; }
        public string DeleteClss { get; set; }
        public string PrintClss { get; set; }
        public string Seq { get; set; }
        public string ProgramID { get; set; }
        public string UseClss { get; set; }

        public int ChkCount { get; set; }
        public bool isEnabled { get; set; }

        public bool SelectChk { get; set; }
        public bool AddNewChk { get; set; }
        public bool UpdateChk { get; set; }
        public bool DeleteChk { get; set; }
        public bool PrintChk { get; set; }
        public bool UseChk { get; set; }

        public List<PersonMenu> Children { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == "SelectChk")
            {
                foreach (PersonMenu child in this.Children)
                    child.SelectChk = this.SelectChk;
            }

            if (propertyName == "AddNewChk")
            {
                foreach (PersonMenu child in this.Children)
                    child.AddNewChk = this.AddNewChk;
            }

            if (propertyName == "UpdateChk")
            {
                foreach (PersonMenu child in this.Children)
                    child.UpdateChk = this.UpdateChk;
            }

            if (propertyName == "DeleteChk")
            {
                foreach (PersonMenu child in this.Children)
                    child.DeleteChk = this.DeleteChk;
            }

            if (propertyName == "PrintChk")
            {
                foreach (PersonMenu child in this.Children)
                    child.PrintChk = this.PrintChk;
            }

            if (propertyName == "UseChk")
            {
                foreach (PersonMenu child in this.Children)
                    child.UseChk = this.UseChk;
            }
        }
    }
}
