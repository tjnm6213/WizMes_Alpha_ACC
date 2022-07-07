using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WizMes_Alpha_JA;
using WizMes_Alpha_JA.PopUP;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// Win_com_Person_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_com_Person_U : UserControl
    {
        string strFlag = string.Empty;
        int rowNum = 0;
        Win_com_Person_U_CodeView2 PersonCodeView = new Win_com_Person_U_CodeView2();
        PersonProcessMachineCodeView2 PersonMachineCodeView = new PersonProcessMachineCodeView2();
        ObservableCollection<PersonMenu2> ovcPersonMenu2 = new ObservableCollection<PersonMenu2>();
        public List<PersonMenu2> lstPersonMenu2 = new List<PersonMenu2>();

        public Win_com_Person_U()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            SetComboBox();
        }

        //ComboBox 전체 세팅
        private void SetComboBox()
        {
            //공급유형(조회, 입력)
            ObservableCollection<CodeView> ovcDepartSrh = ComboBoxUtil.Instance.GetCode_SetComboBoxPlusAll("Depart", null);
            cboDepartSrh.ItemsSource = ovcDepartSrh;
            cboDepartSrh.DisplayMemberPath = "code_name";
            cboDepartSrh.SelectedValuePath = "code_id";

            ObservableCollection<CodeView> ovcDepart = ComboBoxUtil.Instance.GetCode_SetComboBox("Depart", null);
            cboDepart.ItemsSource = ovcDepart;
            cboDepart.DisplayMemberPath = "code_name";
            cboDepart.SelectedValuePath = "code_id";

            //ObservableCollection<CodeView> ovcResably = ComboBoxUtil.Instance.GetCode_SetComboBox("Resably", null);
            //cboResably.ItemsSource = ovcResably;
            //cboResably.DisplayMemberPath = "code_name";
            //cboResably.SelectedValuePath = "code_id";

            ObservableCollection<CodeView> ovcDuty = ComboBoxUtil.Instance.GetCode_SetComboBox("Duty", null);
            cboDuty.ItemsSource = ovcDuty;
            cboDuty.DisplayMemberPath = "code_name";
            cboDuty.SelectedValuePath = "code_id";

            List<string> strValue = new List<string>();
            strValue.Add("양력");
            strValue.Add("음력");

            ObservableCollection<CodeView> ovcSolar = ComboBoxUtil.Instance.Direct_SetComboBox(strValue);
            this.cboSolarClss.ItemsSource = ovcSolar;
            this.cboSolarClss.DisplayMemberPath = "code_name";
            this.cboSolarClss.SelectedValuePath = "code_id";

            ObservableCollection<CodeView> ovcTeam = ComboBoxUtil.Instance.GetCode_SetComboBox("Team", null);
            cboTeam.ItemsSource = ovcTeam;
            cboTeam.DisplayMemberPath = "code_name";
            cboTeam.SelectedValuePath = "code_id";
        }

        //취소, 저장 후
        private void CanBtnControl()
        {
            Lib.Instance.UiButtonEnableChange_IUControl(this);
            grdInput.IsHitTestVisible = false;
            dgdMain.IsEnabled = true;
        }

        //추가, 수정 클릭 시
        private void CantBtnControl()
        {
            Lib.Instance.UiButtonEnableChange_SCControl(this);
            grdInput.IsHitTestVisible = true;
            dgdMain.IsEnabled = false;
        }

        #region Header 부분 - 검색조건

        // 검색조건 - 부서명 검색
        private void lblDepartSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkDepartSrh.IsChecked == true)
            {
                chkDepartSrh.IsChecked = false;
            }
            else
            {
                chkDepartSrh.IsChecked = true;
            }
        }
        private void chkDepartSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkDepartSrh.IsChecked = true;
            cboDepartSrh.IsEnabled = true;
        }
        private void chkDepartSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkDepartSrh.IsChecked = false;
            cboDepartSrh.IsEnabled = false;
        }

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

        // 검색조건 - 퇴사자 포함
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
        private void chkUseClssSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkUseClssSrh.IsChecked = true;
        }
        private void chkUseClssSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkUseClssSrh.IsChecked = false;
        }

        #endregion // Header 부분 - 검색조건

        // 퇴사일자 라벨 클릭 이벤트
        private void lblEndDate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkEndDate.IsChecked == true)
            {
                chkEndDate.IsChecked = false;
            }
            else
            {
                chkEndDate.IsChecked = true;
            }
        }
        // 퇴사일자 체크박스 이벤트
        private void chkEndDate_Checked(object sender, RoutedEventArgs e)
        {
            chkEndDate.IsChecked = true;


            if (dtpEndDate.SelectedDate == null)
            {
                dtpEndDate.SelectedDate = DateTime.Today;
            }

        }
        private void chkEndDate_Unchecked(object sender, RoutedEventArgs e)
        {
            chkEndDate.IsChecked = false;
        }

        //추가
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            strFlag = "I";
            this.DataContext = null;
            CantBtnControl();

            if (dgdProcess.Items.Count > 0)
            {
                dgdProcess.Items.Clear();
            }

            dtpStartDate.SelectedDate = DateTime.Today;
            dtpEndDate.SelectedDate = DateTime.Today;
            tbkMsg.Text = "자료 입력 중";
            MakeMenu();
            PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            usingPersonMenu2(mainMenu);
            rowNum = 0;

            // 퇴사일자 체크박스 해제하기
            chkEndDate.IsChecked = false;

            txtName.Focus();
        }

        //수정
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //int numIndex = dgdMain.SelectedIndex;
            //PersonCodeView = dgdMain.Items[numIndex] as Win_com_Person_U_CodeView2;

            PersonCodeView = dgdMain.SelectedItem as Win_com_Person_U_CodeView2;

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

                PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
                usingPersonMenu2(mainMenu);

                txtName.Focus();
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
            PersonCodeView = dgdMain.SelectedItem as Win_com_Person_U_CodeView2;

            if (PersonCodeView == null)
            {
                MessageBox.Show("삭제할 데이터가 지정되지 않았습니다. 삭제데이터를 지정하고 눌러주세요");
            }
            else
            {
                // admin, master 계정 수정 유효성  체크
                if (checkUserID(PersonCodeView, "D") == false)
                    return;

                if (MessageBox.Show("선택하신 항목을 삭제하시겠습니까?", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (dgdMain.Items.Count > 0 && dgdMain.SelectedItem != null)
                    {
                        rowNum = dgdMain.SelectedIndex;
                    }

                    if (DeleteData(PersonCodeView.PersonID))
                    {
                        rowNum -= 1;
                        re_Search(rowNum);
                    }
                }
            }
        }

        // 수정 - admin, master 면 본인것만 수정되도록
        // 삭제 - admin, master 계정은 삭제가 안되도록 다 막아버리기.
        private bool checkUserID(Win_com_Person_U_CodeView2 PersonCodeView, string superFlag)
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

            if (SaveData(strFlag, txtCode.Text))
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

            string[] lst = new string[4];
            lst[0] = "사원 목록";
            lst[1] = "사원별 공정 목록";
            lst[2] = dgdMain.Name;
            lst[3] = dgdProcess.Name;

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
                else if (ExpExc.choice.Equals(dgdProcess.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdProcess);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdProcess);

                    Name = dgdProcess.Name;

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

        //조회
        private void FillGrid()
        {
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Clear();
                sqlParameter.Add("nChkDepartID", chkDepartSrh.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sDepartID", chkDepartSrh.IsChecked == true && cboDepartSrh.SelectedValue != null ? cboDepartSrh.SelectedValue.ToString() : "");
                sqlParameter.Add("sName", chkNameSrh.IsChecked == true && !txtNameSrh.Text.Trim().Equals("") ? txtNameSrh.Text : "");
                //sqlParameter.Add("sUseClss", chkUseClssSrh.IsChecked == true ? 1 : 0);
                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Person_sPerson", sqlParameter, false);

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
                            var PersonView = new Win_com_Person_U_CodeView2()
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
                                //CustomID = dr["CustomID"].ToString(),
                                //KCustom = dr["KCustom"].ToString(),
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

        //행선택 시
        private void dgdMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PersonCodeView = dgdMain.SelectedItem as Win_com_Person_U_CodeView2;

            if (PersonCodeView != null)
            {
                this.DataContext = PersonCodeView;
                FillGridSub(PersonCodeView.PersonID);
                MakeMenu();

                // 2019.08.29 곽동운 추가
                // 생일 콤보박스 세팅 : 음력 1, 양력 0 선택
                if (PersonCodeView.SolarClss == null || PersonCodeView.SolarClss.Equals(""))
                    cboSolarClss.SelectedIndex = -1;
                else if (PersonCodeView.SolarClss.Equals("0"))
                    cboSolarClss.SelectedIndex = 0;
                else if (PersonCodeView.SolarClss.Equals("1"))
                    cboSolarClss.SelectedIndex = 1;

                // 작업조 콤보박스 세팅 : 주간(01) 교대A(02) 교대B(03) 야간(04)
                if (PersonCodeView.TeamID == null || PersonCodeView.TeamID.Equals(""))
                    cboTeam.SelectedIndex = -1;
                else if (PersonCodeView.TeamID.Equals("01"))
                    cboTeam.SelectedIndex = 0;
                else if (PersonCodeView.TeamID.Equals("02"))
                    cboTeam.SelectedIndex = 1;
                else if (PersonCodeView.TeamID.Equals("03"))
                    cboTeam.SelectedIndex = 2;
                else if (PersonCodeView.TeamID.Equals("04"))
                    cboTeam.SelectedIndex = 3;

                // 퇴사자면, 퇴사일자 체크박스 체크
                if (PersonCodeView.EndDate != null && !PersonCodeView.EndDate.Equals(""))
                    chkEndDate.IsChecked = true;
                else
                    chkEndDate.IsChecked = false;
            }

        }

        //더블 클릭시 수정모드로
        private void dgdMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        //
        private void FillGridSub(string strID)
        {
            if (dgdProcess.Items.Count > 0)
            {
                dgdProcess.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("PersonID", strID);
                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Person_sPersonMachine", sqlParameter, false);

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
                            var PersonProcessView = new PersonProcessMachineCodeView2()
                            {
                                Num = i,
                                PersonID = dr["PersonID"].ToString(),
                                ProcessID = dr["ProcessID"].ToString(),
                                Process = dr["Process"].ToString(),
                                MachineID = dr["MachineID"].ToString(),
                                Machine = dr["Machine"].ToString(),
                                MachineNO = dr["MachineNO"].ToString()
                            };

                            dgdProcess.Items.Add(PersonProcessView);
                        }
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

        private void MakeMenu()
        {
            if (ovcPersonMenu2.Count > 0)
            {
                ovcPersonMenu2.Clear();
            }
            if (lstPersonMenu2.Count > 0)
            {
                lstPersonMenu2.Clear();
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
                    sqlParameter.Add("sUserID", "20100501");
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
                                var user = new PersonMenu2()
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
                                    lstPersonMenu2.Add(user);
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

                        PersonMenu2 person = new PersonMenu2();
                        person.Menu = "메뉴목록";
                        person.MenuID = "0";
                        person.ParentID = "11";
                        person.Level = "A";
                        person.Children = new List<PersonMenu2>();
                        //lstPersonMenu2.Add(person);
                        //TreeViewItems = new TreeViewItem() { Header = person, Tag = person, IsExpanded = true };
                        int k = 0;
                        int j = 0;


                        foreach (DataRow dr in drc)
                        {
                            i++;
                            var PMenu = new PersonMenu2()
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
                                Children = new List<PersonMenu2>(),
                                ChkCount = 0
                            };

                            bool forFlag = true;
                            if (!strFlag.Equals("I"))
                            {
                                foreach (PersonMenu2 user in lstPersonMenu2)
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
                            else if (PMenu.ParentID.Trim().Length == 4)
                            {
                                PMenu.Level = "3";
                                person.Children[k - 1].Children[j - 1].Children.Add(PMenu);
                            }
                            else
                            {
                                PMenu.Level = "0";
                                person.Children.Add(PMenu);
                                k++;
                                j = 0;
                            }
                        }
                        ovcPersonMenu2.Add(person);
                        tlvMenuSetting.ItemsSource = ovcPersonMenu2;
                    }
                }  
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        // 사용구분 체크 이벤트 → 한줄 전체 체크
        private void chkGubun_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;

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
                //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
                //chkLstPersonMenu2Zero(mainMenu, true);
            }

        }
        // 사용구분 체크해제 이벤트 → 한줄 체크해제 체크
        private void chkGubun_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;

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

            //lstPersonMenu2.RemoveAll(lamda => lamda.MenuID == senderTreeViewItem.MenuID);

            //// 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);
        }

        // 전체를 체크하면서, 체크 된것들만 다 처리하기 → 체크할때마다 반복
        private void chkLstPersonMenu2Zero(PersonMenu2 senderTreeViewItem, bool firstFlag)
        {
            // 처음 메서드가 시작할때만, lstPersonMenu2 를 초기화 시켜주기 위해서 firstFlag 를 추가
            if (firstFlag == true)
                lstPersonMenu2.Clear();

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
                        // if (lstPersonMenu2.Contain(senderTreeViewItem.Children[i]) == false)
                        lstPersonMenu2.Add(senderTreeViewItem.Children[i]);
                    }

                    // 만약에 하위 노드가 존재한다면 없을때까지 무한 반복
                    if (senderTreeViewItem.Children[i].Children.Count > 0)
                    {
                        chkLstPersonMenu2Zero(senderTreeViewItem.Children[i], false);
                    }
                }
            } // 1 끝
        }

        // 추가, 수정 상태가 아닐때 체크가 되지 않도록 막아놓은 상태임. 
        // → 수정, 추가 일때 다시 체크가 되도록 변경 (isEnabled = true 로 변경) 
        private void usingPersonMenu2(PersonMenu2 senderTreeViewItem)
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
                        usingPersonMenu2(senderTreeViewItem.Children[i]);
                    }
                }
            } // 1 끝
        }

        // 사원메뉴 조회 체크
        private void chkSearch_Checked(object sender, RoutedEventArgs e)
        {

            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;
            senderTreeViewItem.SelectClss = "*";

            senderTreeViewItem.ChkCount++;
            if (lstPersonMenu2.Contains(senderTreeViewItem) == false)
            {
                lstPersonMenu2.Add(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);


        }
        // 사원메뉴 조회 체크해제
        private void chkSearch_Unchecked(object sender, RoutedEventArgs e)
        {

            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;
            senderTreeViewItem.SelectClss = "";

            senderTreeViewItem.ChkCount--;
            if (senderTreeViewItem.ChkCount == 0)
            {
                lstPersonMenu2.Remove(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);
        }


        // 사원메뉴 추가 체크
        private void chkAdd_Checked(object sender, RoutedEventArgs e)
        {

            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;
            senderTreeViewItem.AddNewClss = "*";

            senderTreeViewItem.ChkCount++;
            if (lstPersonMenu2.Contains(senderTreeViewItem) == false)
            {
                lstPersonMenu2.Add(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);

        }
        // 사원메뉴 추가 체크해제
        private void chkAdd_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;
            senderTreeViewItem.AddNewClss = "";

            senderTreeViewItem.ChkCount--;
            if (senderTreeViewItem.ChkCount == 0)
            {
                lstPersonMenu2.Remove(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);

        }


        // 사원메뉴 수정 체크
        private void chkUpdate_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;
            senderTreeViewItem.UpdateClss = "*";

            senderTreeViewItem.ChkCount++;
            if (lstPersonMenu2.Contains(senderTreeViewItem) == false)
            {
                lstPersonMenu2.Add(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);

        }
        // 사원메뉴 수정 체크해제
        private void chkUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;
            senderTreeViewItem.UpdateClss = "";

            senderTreeViewItem.ChkCount--;
            if (senderTreeViewItem.ChkCount == 0)
            {
                lstPersonMenu2.Remove(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);

        }

        // 사원메뉴 삭제 체크
        private void chkDelete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;
            senderTreeViewItem.DeleteClss = "*";

            senderTreeViewItem.ChkCount++;
            if (lstPersonMenu2.Contains(senderTreeViewItem) == false)
            {
                lstPersonMenu2.Add(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);

        }
        // 사원메뉴 삭제 체크해제
        private void chkDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;
            senderTreeViewItem.DeleteClss = "";

            senderTreeViewItem.ChkCount--;
            if (senderTreeViewItem.ChkCount == 0)
            {
                lstPersonMenu2.Remove(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);

        }


        // 사원메뉴 출력 체크
        private void chkPrint_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;
            senderTreeViewItem.PrintClss = "*";

            senderTreeViewItem.ChkCount++;
            if (lstPersonMenu2.Contains(senderTreeViewItem) == false)
            {
                lstPersonMenu2.Add(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);

        }
        // 사원메뉴 출력 체크해제
        private void chkPrint_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            PersonMenu2 senderTreeViewItem = chkSender.DataContext as PersonMenu2;
            senderTreeViewItem.PrintClss = "";

            senderTreeViewItem.ChkCount--;
            if (senderTreeViewItem.ChkCount == 0)
            {
                lstPersonMenu2.Remove(senderTreeViewItem);
            }

            // 이건 아마 메뉴(전체선택 행)일 것이여
            //PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            //chkLstPersonMenu2Zero(mainMenu, true);

        }

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

        //
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
            PersonMenu2 mainMenu = tlvMenuSetting.Items[0] as PersonMenu2;
            chkLstPersonMenu2Zero(mainMenu, true);

            try
            {
                if (CheckData())
                {
                    int Seq = 0;
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();

                    sqlParameter.Add("sPersonID", strID.Trim());
                    sqlParameter.Add("sUserID", txtUserID.Text);
                    sqlParameter.Add("sPassword", txtPassWord.Text);
                    sqlParameter.Add("sName", txtName.Text);
                    sqlParameter.Add("sDepartID", cboDepart.SelectedValue == null ? "" : cboDepart.SelectedValue.ToString());

                    sqlParameter.Add("sResablyID", cboResably.SelectedValue == null ? "" : cboResably.SelectedValue.ToString());
                    sqlParameter.Add("sDutyID", cboDuty.SelectedValue == null ? "" : cboDuty.SelectedValue.ToString());
                    sqlParameter.Add("sStartDate", dtpStartDate.SelectedDate == null ?
                        "" : dtpStartDate.SelectedDate.Value.ToString("yyyyMMdd"));
                    sqlParameter.Add("sEndDate", chkEndDate.IsChecked == true ?
                        dtpEndDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                    sqlParameter.Add("sRegistID", txtRegistID.Text);

                    sqlParameter.Add("sHandPhone", txtHandPhone.Text);
                    sqlParameter.Add("sPhone", txtTelePhone.Text);
                    sqlParameter.Add("sBirthDay", txtBirthday.Text);
                    sqlParameter.Add("sSolarClss", cboSolarClss.SelectedValue == null ?
                        "" : cboSolarClss.SelectedValue.ToString());
                    sqlParameter.Add("sZipCode", txtZipCode.Text);

                    sqlParameter.Add("sOldNNewClss", rbnDoro.IsChecked == true ? 0 : 1);
                    sqlParameter.Add("sGunMoolMngNo", txtGunMoolMngNo.Text);
                    sqlParameter.Add("sAddress1", txtAddress1.Text);
                    sqlParameter.Add("sAddress2", txtAddress2.Text);
                    sqlParameter.Add("sAddressAssist", txtAddressAssist.Text);

                    sqlParameter.Add("sAddressJiBun1", txtAddressJ1.Text);
                    sqlParameter.Add("sAddressJiBun2", txtAddressJ2.Text);
                    sqlParameter.Add("sEMail", txtEMail.Text);
                    sqlParameter.Add("sRemark", txtRemark.Text);
                    sqlParameter.Add("TeamID", cboTeam.SelectedValue == null ? "" : cboTeam.SelectedValue.ToString());

                    sqlParameter.Add("CustomID", txtCustom.Tag == null ? "" : txtCustom.Tag.ToString());
                    sqlParameter.Add("Bank", txtBank.Text);
                    //sqlParameter.Add("sPgGubun", "8");

                    #region 추가

                    if (strFlag.Equals("I"))
                    {
                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro1 = new Procedure();
                        pro1.Name = "xp_Person_iPerson";
                        pro1.OutputUseYN = "Y";
                        pro1.OutputName = "sPersonID";
                        pro1.OutputLength = "8";

                        Prolist.Add(pro1);
                        ListParameter.Add(sqlParameter);

                        for (int i = 0; i < lstPersonMenu2.Count; i++)
                        {
                            PersonMenu2 PersonMenu2 = lstPersonMenu2[i] as PersonMenu2;
                            if (lstPersonMenu2[i].Level != null &&
                               !(lstPersonMenu2[i].Level.Equals("A")))  // !lstPersonMenu2[i].Level.Equals("A")    lstPersonMenu2[i].UseClss != null  && lstPersonMenu2[i].UseClss.Equals("*")
                            {
                                Seq++;
                                sqlParameter = new Dictionary<string, object>();
                                sqlParameter.Clear();

                                sqlParameter.Add("sPersonID", strID);
                                sqlParameter.Add("sPgGubun", "8");
                                sqlParameter.Add("sMenuID", PersonMenu2.MenuID);
                                sqlParameter.Add("nSeq", Seq);
                                sqlParameter.Add("nLevel", PersonMenu2.Level);
                                sqlParameter.Add("sParentID", PersonMenu2.ParentID);
                                sqlParameter.Add("sSelectClss", PersonMenu2.SelectClss);
                                sqlParameter.Add("sAddNewClss", PersonMenu2.AddNewClss);
                                sqlParameter.Add("sUpdateClss", PersonMenu2.UpdateClss);
                                sqlParameter.Add("sDeleteClss", PersonMenu2.DeleteClss);
                                sqlParameter.Add("sPrintClss", PersonMenu2.PrintClss);
                                sqlParameter.Add("sCreateUserID", MainWindow.CurrentUser);

                                Procedure pro3 = new Procedure();
                                pro3.Name = "xp_Menu_iUserMenu";
                                pro3.OutputUseYN = "N";
                                pro3.OutputName = "sPersonID";
                                pro3.OutputLength = "8";

                                Prolist.Add(pro3);
                                ListParameter.Add(sqlParameter);
                            }
                        }

                        List<KeyValue> list_Result = new List<KeyValue>();
                        list_Result = DataStore.Instance.ExecuteAllProcedureOutputGetCS(Prolist, ListParameter);
                        string sGetID = string.Empty;

                        if (list_Result[0].key.ToLower() == "success")
                        {
                            list_Result.RemoveAt(0);
                            for (int i = 0; i < list_Result.Count; i++)
                            {
                                KeyValue kv = list_Result[i];
                                if (kv.key == "sPersonID")
                                {
                                    sGetID = kv.value;
                                    flag = true;
                                }
                            }

                            if (flag)
                            {
                                if (InsertPersonMachine(sGetID))
                                {
                                    flag = true;
                                }
                                else
                                {
                                    MessageBox.Show("해당 직원의 작업공정 입력 실패");
                                    flag = false;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("[저장실패]\r\n" + list_Result[0].value.ToString());
                            flag = false;
                            //return false;
                        }
                    }

                    #endregion

                    #region 수정

                    else if (strFlag.Equals("U"))
                    {
                        sqlParameter.Add("LastUpdateUserID", MainWindow.CurrentUser);

                        Procedure pro1 = new Procedure();
                        pro1.Name = "xp_Person_uPerson";
                        pro1.OutputUseYN = "N";
                        pro1.OutputName = "sPersonID";
                        pro1.OutputLength = "8";

                        Prolist.Add(pro1);
                        ListParameter.Add(sqlParameter);

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", strID);

                        Procedure pro5 = new Procedure();
                        pro5.Name = "xp_Person_dPersonMachine";
                        pro5.OutputUseYN = "N";
                        pro5.OutputName = "PersonID";
                        pro5.OutputLength = "8";

                        Prolist.Add(pro5);
                        ListParameter.Add(sqlParameter);

                        for (int i = 0; i < dgdProcess.Items.Count; i++)
                        {
                            var PersonMachine = dgdProcess.Items[i] as PersonProcessMachineCodeView2;
                            sqlParameter = new Dictionary<string, object>();
                            sqlParameter.Clear();
                            sqlParameter.Add("PersonID", strID);
                            sqlParameter.Add("ProcessID", PersonMachine.ProcessID);
                            sqlParameter.Add("MachineID", PersonMachine.MachineID);
                            sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                            Procedure pro2 = new Procedure();
                            pro2.Name = "xp_Person_iPersonMachine";
                            pro2.OutputUseYN = "N";
                            pro2.OutputName = "sPersonID";
                            pro2.OutputLength = "8";

                            Prolist.Add(pro2);
                            ListParameter.Add(sqlParameter);
                        }

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

                        // 테스트 : lstPersonMenu2.Count
                        for (int i = 0; i < lstPersonMenu2.Count; i++)
                        {
                            if (lstPersonMenu2[i].Level != null &&
                                !(lstPersonMenu2[i].Level.Equals("A")))  // !lstPersonMenu2[i].Level.Equals("A")    lstPersonMenu2[i].UseClss != null  && lstPersonMenu2[i].UseClss.Equals("*")
                            {
                                Seq++;
                                sqlParameter = new Dictionary<string, object>();
                                sqlParameter.Clear();
                                sqlParameter.Add("sPersonID", strID);
                                sqlParameter.Add("sPgGubun", "8");
                                sqlParameter.Add("sMenuID", lstPersonMenu2[i].MenuID);
                                sqlParameter.Add("nSeq", Seq);
                                sqlParameter.Add("nLevel", lstPersonMenu2[i].Level);
                                sqlParameter.Add("sParentID", lstPersonMenu2[i].ParentID);
                                sqlParameter.Add("sSelectClss", lstPersonMenu2[i].SelectClss);
                                sqlParameter.Add("sAddNewClss", lstPersonMenu2[i].AddNewClss);
                                sqlParameter.Add("sUpdateClss", lstPersonMenu2[i].UpdateClss);
                                sqlParameter.Add("sDeleteClss", lstPersonMenu2[i].DeleteClss);
                                sqlParameter.Add("sPrintClss", lstPersonMenu2[i].PrintClss);
                                sqlParameter.Add("sCreateUserID", MainWindow.CurrentUser);

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

                    #endregion
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

        /// <summary>
        /// 입력사항 체크
        /// </summary>
        /// <returns></returns>
        private bool CheckData()
        {
            bool flag = true;

            if (txtName.Text.Length <= 0 || txtName.Text.Equals(""))
            {
                MessageBox.Show("성명이 입력되지 않았습니다.");
                flag = false;
                return flag;
            }

            //if (txtUserID.Text.Length <= 0 || txtUserID.Text.Equals(""))
            //{
            //    MessageBox.Show("아이디가 입력되지 않았습니다.");
            //    flag = false;
            //    return flag;
            //}

            if (txtPassWord.Text.Length <= 0 || txtPassWord.Text.Equals(""))
            {
                MessageBox.Show("비밀번호가 입력되지 않았습니다.");
                flag = false;
                return flag;
            }

            // 2019.08.19 곽동운 : 부서가 입력되지 않았을때 에러 발생 -> 입력 될수 있도록 추가
            if (cboDepart.Text.Length <= 0 || cboDepart.Text.Trim().Equals(""))
            {
                MessageBox.Show("부서가 입력되지 않았습니다.");
                flag = false;
                return flag;
            }

            // 2019.08.19 곽동운 : 추가시 회원 아이디가 중복이 되지 않도록 조회 및 검사
            if (strFlag == "I")
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("UserID", txtUserID.Text);
                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Person_sCheckUserID", sqlParameter, false);
                DataTable dt = ds.Tables[0];
                DataRow dr = dt.Rows[0];
                int count = Convert.ToInt32(dr["count"].ToString());

                if (count > 0) // 회원 아이디 개수가 0보다 클경우 중복 메시지 팝업
                {
                    MessageBox.Show("회원 아이디가 이미 존재합니다.");
                    flag = false;
                    return flag;
                }
            }

            // 퇴사일자가 체크 일때는 반드시 날짜를 입력하도록
            if (strFlag.Equals("I") || strFlag.Equals("U"))
            {
                if (chkEndDate.IsChecked == true)
                {
                    if (dtpEndDate.SelectedDate == null)
                    {
                        MessageBox.Show("퇴사일자를 선택해주세요.");
                        flag = false;
                        return flag;
                    }
                }
            }

            // 사원의 해당 공정에 입력되지 않은 부분이 있을 경우
            for (int i = 0; i < dgdProcess.Items.Count; i++)
            {
                var PersonMachine = dgdProcess.Items[i] as PersonProcessMachineCodeView2;

                if ((PersonMachine.ProcessID.Trim().Equals("") || PersonMachine.ProcessID == null)
                        || (PersonMachine.MachineID.Trim().Equals("") || PersonMachine.MachineID == null))
                {
                    MessageBox.Show("사원의 해당 공정에 입력되지 않은 부분이 있습니다.");
                    flag = false;
                    return flag;
                }
            }


            //if (cboMcInsCycleGbn.SelectedValue == null)
            //{
            //    MessageBox.Show("정기검사구분이 선택되지 않았습니다.");
            //    flag = false;
            //    return flag;
            //}

            return flag;
        }

        //
        private bool InsertPersonMachine(string strPersonID)
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                for (int i = 0; i < dgdProcess.Items.Count; i++)
                {
                    var PersonMachine = dgdProcess.Items[i] as PersonProcessMachineCodeView2;
                    sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();
                    sqlParameter.Add("PersonID", strPersonID);
                    sqlParameter.Add("ProcessID", PersonMachine.ProcessID);
                    sqlParameter.Add("MachineID", PersonMachine.MachineID);
                    sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                    Procedure pro2 = new Procedure();
                    pro2.Name = "xp_Person_iPersonMachine";
                    pro2.OutputUseYN = "N";
                    pro2.OutputName = "sPersonID";
                    pro2.OutputLength = "8";

                    Prolist.Add(pro2);
                    ListParameter.Add(sqlParameter);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return flag;
        }

        private void txtZipCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PopUp.Win_Zip_Address ZipPopUp = new PopUp.Win_Zip_Address();
                ZipPopUp.ShowDialog();

                if (ZipPopUp.DialogResult == true)
                {
                    if (ZipPopUp.strGubun.Equals("0"))
                    {
                        txtAddress1.Text = ZipPopUp.Juso;
                        txtAddress2.Text = ZipPopUp.Detail1;
                        txtAddressAssist.Text = ZipPopUp.Detail2;
                        txtZipCode.Text = ZipPopUp.ZipCode;
                        txtGunMoolMngNo.Text = ZipPopUp.GunMoolMngNo;
                    }
                    else if (ZipPopUp.strGubun.Equals("1"))
                    {
                        txtAddressJ1.Text = ZipPopUp.Juso;
                        txtZipCode.Text = ZipPopUp.ZipCode;
                    }
                }
            }
        }

        private void btnPfZipCode_Click(object sender, RoutedEventArgs e)
        {
            PopUp.Win_Zip_Address ZipPopUp = new PopUp.Win_Zip_Address();
            ZipPopUp.ShowDialog();

            if (ZipPopUp.DialogResult == true)
            {
                if (ZipPopUp.strGubun.Equals("0"))
                {
                    txtAddress1.Text = ZipPopUp.Juso;
                    txtAddress2.Text = ZipPopUp.Detail1;
                    txtAddressAssist.Text = ZipPopUp.Detail2;
                    txtZipCode.Text = ZipPopUp.ZipCode;
                    txtGunMoolMngNo.Text = ZipPopUp.GunMoolMngNo;
                }
                else if (ZipPopUp.strGubun.Equals("1"))
                {
                    txtAddressJ1.Text = ZipPopUp.Juso;
                    txtZipCode.Text = ZipPopUp.ZipCode;
                }
            }
        }

        private void txtCustom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
            }
        }

        private void btnPfCustom_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
        }

        private void btnSubAdd_Click(object sender, RoutedEventArgs e)
        {
            int count = dgdProcess.Items.Count + 1;

            var PersonMe = new PersonProcessMachineCodeView2()
            {
                PersonID = txtCode.Text,
                Machine = "",
                Process = "",
                MachineID = "",
                MachineNO = "",
                ProcessID = "",
                Num = count
            };

            dgdProcess.Items.Add(PersonMe);
        }

        private void btnSubDel_Click(object sender, RoutedEventArgs e)
        {
            if (dgdProcess.Items.Count > 0)
            {
                if (dgdProcess.SelectedItem != null)
                {
                    dgdProcess.Items.Remove(dgdProcess.SelectedItem as PersonProcessMachineCodeView2);
                }
                dgdProcess.Refresh();
            }
        }


        #region 서브 데이터그리드 입력 이벤트

        // 2019.08.27 PreviewKeyDown 는 key 다운과 같은것 같음
        private void DataGird_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
                {
                    DataGird_KeyDown(sender, e);
                }
            }
            catch (Exception ex)
            {

            }
        }
        // KeyDown 이벤트
        private void DataGird_KeyDown(object sender, KeyEventArgs e)
        {
            int currRow = dgdProcess.Items.IndexOf(dgdProcess.CurrentItem);
            int currCol = dgdProcess.Columns.IndexOf(dgdProcess.CurrentCell.Column);
            int startCol = 1;
            int endCol = 2;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                // 마지막 열, 마지막 행 아님
                if (endCol == currCol && dgdProcess.Items.Count - 1 > currRow)
                {
                    dgdProcess.SelectedIndex = currRow + 1; // 이건 한줄 파란색으로 활성화 된 걸 조정하는 것입니다.
                    dgdProcess.CurrentCell = new DataGridCellInfo(dgdProcess.Items[currRow + 1], dgdProcess.Columns[startCol]);

                } // 마지막 열 아님
                else if (endCol > currCol && dgdProcess.Items.Count - 1 >= currRow)
                {
                    dgdProcess.CurrentCell = new DataGridCellInfo(dgdProcess.Items[currRow], dgdProcess.Columns[currCol + 1]);
                } // 마지막 열, 마지막 행
                else if (endCol == currCol && dgdProcess.Items.Count - 1 == currRow)
                {
                    //btnSave.Focus();
                }
                else
                {
                    MessageBox.Show("나머지가 있나..");
                }
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                // 마지막 행 아님
                if (dgdProcess.Items.Count - 1 > currRow)
                {
                    dgdProcess.SelectedIndex = currRow + 1;
                    dgdProcess.CurrentCell = new DataGridCellInfo(dgdProcess.Items[currRow + 1], dgdProcess.Columns[currCol]);
                } // 마지막 행일때
                else if (dgdProcess.Items.Count - 1 == currRow)
                {
                    if (endCol > currCol) // 마지막 열이 아닌 경우, 열을 오른쪽으로 이동
                    {
                        //dgdProcess.SelectedIndex = 0;
                        dgdProcess.CurrentCell = new DataGridCellInfo(dgdProcess.Items[currRow], dgdProcess.Columns[currCol + 1]);
                    }
                    else
                    {
                        //btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                // 첫행 아님
                if (currRow > 0)
                {
                    dgdProcess.SelectedIndex = currRow - 1;
                    dgdProcess.CurrentCell = new DataGridCellInfo(dgdProcess.Items[currRow - 1], dgdProcess.Columns[currCol]);
                } // 첫 행
                else if (dgdProcess.Items.Count - 1 == currRow)
                {
                    if (0 < currCol) // 첫 열이 아닌 경우, 열을 왼쪽으로 이동
                    {
                        //dgdProcess.SelectedIndex = 0;
                        dgdProcess.CurrentCell = new DataGridCellInfo(dgdProcess.Items[currRow], dgdProcess.Columns[currCol - 1]);
                    }
                    else
                    {
                        //btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Left)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (startCol < currCol)
                {
                    dgdProcess.CurrentCell = new DataGridCellInfo(dgdProcess.Items[currRow], dgdProcess.Columns[currCol - 1]);
                }
                else if (startCol == currCol)
                {
                    if (0 < currRow)
                    {
                        dgdProcess.SelectedIndex = currRow - 1;
                        dgdProcess.CurrentCell = new DataGridCellInfo(dgdProcess.Items[currRow - 1], dgdProcess.Columns[endCol]);
                    }
                    else
                    {
                        //btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Right)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (endCol > currCol)
                {

                    dgdProcess.CurrentCell = new DataGridCellInfo(dgdProcess.Items[currRow], dgdProcess.Columns[currCol + 1]);
                }
                else if (endCol == currCol)
                {
                    if (dgdProcess.Items.Count - 1 > currRow)
                    {
                        dgdProcess.SelectedIndex = currRow + 1;
                        dgdProcess.CurrentCell = new DataGridCellInfo(dgdProcess.Items[currRow + 1], dgdProcess.Columns[startCol]);
                    }
                    else
                    {
                        //btnSave.Focus();
                    }
                }
            }
        }
        // KeyUp 이벤트
        private void DatagridIn_TextFocus(object sender, KeyEventArgs e)
        {
            // 엔터 → 포커스 = true → cell != null → 해당 텍스트박스가 null이 아니라면 
            // → 해당 텍스트박스가 포커스가 안되있음 SelectAll() or 포커스
            Lib.Instance.DataGridINTextBoxFocus(sender, e);
        }
        // GotFocus 이벤트
        private void DataGridCell_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                DataGridCell cell = sender as DataGridCell;
                cell.IsEditing = true;
            }
        }
        // 2019.08.27 MouseUp 이벤트
        private void DataGridCell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Lib.Instance.DataGridINTextBoxFocusByMouseUP(sender, e);
        }
        // 2019.08.27 데이터그리드 Process 플러스파인더 검색
        private void TextBoxProcessName_KeyDown(object sender, KeyEventArgs e)
        {
            // 설계 : 공정명(Process, ProcessID)을 수정했을 경우, 해당 공정명에 따라 설비명, 호기가 달라지기 때문에, 설비명과 호기를 비워줘야 합니다.
            //          → 처음에 있던 ProcessID 를 가져와서, 변경된 ProcessID와 비교하여 달라졌을 경우 설비명과 호기를 비워주는 걸로 구현

            // ProcessID 초기값
            string initialProcessID = string.Empty;

            PersonMachineCodeView = dgdProcess.CurrentItem as PersonProcessMachineCodeView2;
            if (PersonMachineCodeView.ProcessID != null)
            {
                initialProcessID = PersonMachineCodeView.ProcessID;
            }

            if (lblMsg.Visibility == Visibility.Visible)
            {
                if (e.Key == Key.Enter)
                {
                    TextBox tb1 = sender as TextBox;

                    MainWindow.pf.ReturnCode(tb1, 71, ""); // 2019.08.29 곽동운 71번(Machine이 존재하는 Process 리스트) 추가함

                    if (tb1.Tag != null)
                    {
                        PersonMachineCodeView.Process = tb1.Text;
                        PersonMachineCodeView.ProcessID = tb1.Tag.ToString();

                        // 초기값이 존재한다면 ProcessID 초기값과 변경된 값 비교 → 변경됬으면 설비명, 호기 초기화
                        if (initialProcessID != string.Empty)
                        {
                            if (!initialProcessID.Equals(PersonMachineCodeView.ProcessID))
                            {
                                // 테스트
                                //MessageBox.Show("여기까지 왔음");
                                PersonMachineCodeView.Machine = string.Empty;
                                PersonMachineCodeView.MachineID = string.Empty;
                                PersonMachineCodeView.MachineNO = string.Empty;
                            }
                        }
                    }
                }
            }
        }

        // 2019.08.27 데이터그리드 Machine 플러스파인더 검색 > 검색 후 선택한 MachineID 를 가지고 호기도 자동으로 입력되도록 구현
        private void TextBoxMachineName_KeyDown(object sender, KeyEventArgs e)
        {
            PersonMachineCodeView = dgdProcess.CurrentItem as PersonProcessMachineCodeView2;

            // Machine(설비명), MachineID, MachineNo(호기) 을 가져와야 하는데
            // MachineNo 를 검색해서 가져오기 위해서는 추가적으로 ProcessID 가 필요 하기 때문에, 
            // ProcessID가 입력되지 않았다면 사용못하도록 막아놓음
            if (PersonMachineCodeView.ProcessID != null && !PersonMachineCodeView.ProcessID.Trim().Equals(""))
            {
                if (lblMsg.Visibility == Visibility.Visible)
                {
                    if (e.Key == Key.Enter)
                    {
                        if (PersonMachineCodeView != null)
                        {
                            TextBox tb1 = sender as TextBox;
                            MainWindow.pf.ReturnCode(tb1, 66, PersonMachineCodeView.ProcessID);

                            if (tb1.Tag != null)
                            {
                                PersonMachineCodeView.MachineID = tb1.Tag.ToString();
                                PersonMachineCodeView.Machine = tb1.Text;

                                string[] result = new string[2];
                                string sql = "SELECT MachineNo FROM mt_Machine"
                                     + " WHERE ProcessID = " + PersonMachineCodeView.ProcessID
                                     + " AND MachineID = " + PersonMachineCodeView.MachineID;
                                result = DataStore.Instance.ExecuteQuery(sql, false);

                                if (result[0].Equals("success"))
                                {
                                    PersonMachineCodeView.MachineNO = result[1];
                                }
                                else
                                {
                                    // 2019.08.29 곽동운
                                    // 문제 : Process 는 있는데, 해당 설비(Machine) 가 없는경우, 
                                    //  입력되는 테이블 mt_PersonMachine 이 MacineID 가 외래키로 묶여 있기 때문에, 필수로 입력 해주지 않으면 에러가 발생.
                                    //  그래서 해당 공정의 Machine 이 검색되지 않는 경우는 입력할수 없도록 막아놔야 하는데, 여기서 막겠음.
                                    // → 해결 : Machine 이 존재하는 Process 만 나오도록 프로시저(xp_Common_PlusFinder, 71번)  추가 및 소스 수정(1441줄) 
                                    // → 이건 봉인
                                    //MessageBox.Show("해당 공정에 설비가 존재하지 않습니다.");
                                    //PersonMachineCodeView.ProcessID = "";
                                    //PersonMachineCodeView.Process = "";
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else // ProcessID가 입력되지 않았다면 사용 불가
            {
                MessageBox.Show("공정명을 먼저 입력해주세요.");
                return;
            }
        }


        #endregion

        // 테스트
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            string message = "리스트 갯수 : " + lstPersonMenu2.Count;

            int i = 0;
            foreach (PersonMenu2 pm in lstPersonMenu2)
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
        // 테스트2
        private void btnTest2_Click(object sender, RoutedEventArgs e)
        {
            var Person = dgdMain.SelectedItem as Win_com_Person_U_CodeView2;

            try
            {
                string sql = "DELETE mt_Person WHERE PersonID = '" + Person.PersonID + "'";

                DataStore.Instance.ExecuteQuery(sql, false);

                rowNum = 0;
                re_Search(rowNum);
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생 : " + ex.ToString() + "\r혹여나 서브그리드나 사용자 메뉴에 데이터가 남아있다면, 지우고 할것!");
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }

        #region 텍스트 박스 엔터 → 다음 텍스트 박스

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                cboDepart.Focus();
            }
        }

        private void cboDepart_DropDownClosed(object sender, EventArgs e)
        {
            cboResably.Focus();
        }

        private void cboResably_DropDownClosed(object sender, EventArgs e)
        {
            cboDuty.Focus();
        }

        private void cboDuty_DropDownClosed(object sender, EventArgs e)
        {
            txtUserID.Focus();
        }

        private void txtUserID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPassWord.Focus();
            }
        }

        private void txtPassWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dtpStartDate.Focus();
            }
        }

        private void dtpStartDate_CalendarClosed(object sender, RoutedEventArgs e)
        {
            txtRegistID.Focus();
        }

        private void txtRegistID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtHandPhone.Focus();
            }
        }

        private void txtHandPhone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtBirthday.Focus();
            }
        }

        private void txtBirthday_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                cboSolarClss.Focus();
            }
        }

        private void cboSolarClss_DropDownClosed(object sender, EventArgs e)
        {
            txtTelePhone.Focus();
        }

        private void txtTelePhone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtZipCode.Focus();
            }
        }

        private void txtAddressAssist_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtRemark.Focus();
            }
        }

        private void txtAddressJ2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtRemark.Focus();
            }
        }

        private void txtRemark_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtEMail.Focus();
            }
        }

        private void txtEMail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtCustom.Focus();
            }
        }

        private void txtBank_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                cboTeam.Focus();
            }
        }

        private void txtCustom_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtBank.Focus();
        }


        #endregion // 텍스트 박스 엔터 → 다음 텍스트 박스


    }

    class Win_com_Person_U_CodeView2 : BaseView
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

    class ProcessMachineCodeView2 : BaseView
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

    class PersonProcessMachineCodeView2 : BaseView
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

    public class PersonMenu2 : INotifyPropertyChanged
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

        public List<PersonMenu2> Children { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName == "SelectChk")
            {
                foreach (PersonMenu2 child in this.Children)
                    child.SelectChk = this.SelectChk;
            }

            if (propertyName == "AddNewChk")
            {
                foreach (PersonMenu2 child in this.Children)
                    child.AddNewChk = this.AddNewChk;
            }

            if (propertyName == "UpdateChk")
            {
                foreach (PersonMenu2 child in this.Children)
                    child.UpdateChk = this.UpdateChk;
            }

            if (propertyName == "DeleteChk")
            {
                foreach (PersonMenu2 child in this.Children)
                    child.DeleteChk = this.DeleteChk;
            }

            if (propertyName == "PrintChk")
            {
                foreach (PersonMenu2 child in this.Children)
                    child.PrintChk = this.PrintChk;
            }

            if (propertyName == "UseChk")
            {
                foreach (PersonMenu2 child in this.Children)
                    child.UseChk = this.UseChk;
            }
        }
    }
}
