using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WizMes_Alpha_JA.PopUP;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// frm_Acc_BS_Item_Code_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class frm_Acc_BS_Item_Code_U : UserControl
    {
        string strFlag = string.Empty;
        int rowNum = 0;
        Lib lib = new Lib();
        frm_Acc_BS_Item_Code_U_CodeView BSItemView = new frm_Acc_BS_Item_Code_U_CodeView();

        int BsCountNum = 0;

        ////rowNum에 필요한 변수(추가할 때)
        //string BSItemCode = string.Empty;

        public frm_Acc_BS_Item_Code_U()
        {
            InitializeComponent();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            SetComboBox();

            btnSales.IsChecked = true;

        }

        #region 콤보박스 셋팅 
        private void SetComboBox()
        {
            //Main항목여부 콤보박스 목록 지정
            List<string> cboMainList = new List<string>();
            cboMainList.Add("N");
            cboMainList.Add("Y");

            //Main항목여부
            ObservableCollection<CodeView> cboMainlist = ComboBoxUtil.Instance.Direct_SetComboBox(cboMainList); //스트링배열 cboMainList 넣고 
            //this.cboMain.ItemsSource = cboMainlist;
            //this.cboMain.DisplayMemberPath = "code_name";
            //this.cboMain.SelectedValuePath = "code_id";

            //대분류 목록 가져오기
            ObservableCollection<CodeView> cboLargeList = Direct_SetComboBoxLarge();
            //중분류탭의 대분류 콤보박스
            this.cboLarge.ItemsSource = cboLargeList;
            this.cboLarge.DisplayMemberPath = "code_name";
            this.cboLarge.SelectedValuePath = "code_id";

            ObservableCollection<CodeView> cboLargeList2 = Direct_SetComboBoxLarge();
            //항목탭의 대분류 콤보박스
            this.cboLarge2.ItemsSource = cboLargeList2;
            this.cboLarge2.DisplayMemberPath = "code_name";
            this.cboLarge2.SelectedValuePath = "code_id";

            // 항목탭의 중분류 콤보박스 초기화
            this.cboMiddle.SelectedIndex = -1;


        }

        #endregion

        #region 우측 상단 버튼들 

        //추가
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = tabAll.SelectedItem as TabItem;

            if (btnSales.IsChecked == false && btnBuy.IsChecked == false)
            {
                MessageBox.Show("매출 혹은 매입 버튼을 먼저 선택하십시오.");

            }
            else if (ti.Header.ToString().Equals("중분류") && cboLarge.SelectedItem == null)
            {
                MessageBox.Show("대분류를 선택 후 추가를 진행 해주세요.");

                //콤보박스에 포커스가 맞춰지도록
                cboLarge.Focus();
                lib.SendK(Key.Enter, this);
                cboLarge.IsDropDownOpen = true;

            }
            else if ((ti.Header.ToString().Equals("항목") && cboLarge2.SelectedItem == null) || (ti.Header.ToString().Equals("항목") && cboMiddle.SelectedItem == null))
            {
                MessageBox.Show("대분류, 중분류를 선택 후 추가를 진행 해주세요.");

                if (cboLarge2.SelectedItem == null)
                {
                    //대분류 콤보박스에 포커스가 맞춰지도록
                    cboLarge2.Focus();
                    lib.SendK(Key.Enter, this);
                    cboLarge2.IsDropDownOpen = true;
                }
                else if (cboLarge.SelectedItem != null)
                {
                    //콤보박스에 포커스가 맞춰지도록
                    cboMiddle.Focus();
                    lib.SendK(Key.Enter, this);
                    cboMiddle.IsDropDownOpen = true;

                    //끝나고 나서는 strFlag 비우기
                    strFlag = string.Empty;
                }
            }
            else
            {
                strFlag = "I";
                tbkMsg.Text = "자료 입력 중";
                this.DataContext = null; //텍스트박스 비우기
                chkYes.IsChecked = true; //추가일 때는 사용여부가 Yes 체크 되어있도록.
                chkNo.IsChecked = false; //체크박스 비우기
                CantBtnControl();


                //추가 버튼 클릭시 은행명에 커서가 이동되도록
                txtKName.Focus();

                //Main항목여부 기본값 설정 N이 되게 
                //cboMain.SelectedIndex = 0;

                if (ti.Header.ToString().Equals("대분류"))
                {
                    if (dgdLarge.SelectedItem != null) // 선택된 행이 있다면
                    {
                        rowNum = dgdLarge.SelectedIndex; // rowNum에 행번호를 기억, 취소시 재검색시 rowNum의 자료를 보여줌

                    }
                }
                else if (ti.Header.ToString().Equals("중분류"))
                {
                    if (dgdMiddle.SelectedItem != null)
                    {
                        rowNum = dgdMiddle.SelectedIndex;
                    }
                }
                else if (ti.Header.ToString().Equals("항목"))
                {
                    //항목에서는 Y,N 변경 가능하도록
                    //cboMain.IsEnabled = true;

                    if (dgdList.SelectedItem != null)
                    {
                        rowNum = dgdList.SelectedIndex;
                    }
                }
            }
        }


        //수정
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = tabAll.SelectedItem as TabItem;

            if (ti.Header.ToString().Equals("대분류"))
            {
                BSItemView = dgdLarge.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("중분류"))
            {
                BSItemView = dgdMiddle.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("항목"))
            {
                BSItemView = dgdList.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;
            }

            if (BSItemView != null)
            {
                if (ti.Header.ToString().Equals("대분류"))
                {
                    rowNum = dgdLarge.SelectedIndex;
                }
                else if (ti.Header.ToString().Equals("중분류"))
                {
                    rowNum = dgdMiddle.SelectedIndex;
                }
                else if (ti.Header.ToString().Equals("항목"))
                {
                    rowNum = dgdList.SelectedIndex;
                }

                strFlag = "U";
                tbkMsg.Text = "자료 수정 중";
                if (chkYes.IsChecked == true)
                {
                    CantBtnControl();
                }
                else //사용여부(N)인 것은 수정이 되지 않게 하기 위해, 사용여부 Y,N만 체크할 수 있도록 
                {
                    Lib.Instance.UiButtonEnableChange_SCControl(this);

                    txtKName.IsEnabled = false; //한글명
                    txtEName.IsEnabled = false; //영문명
                    txtOrder.IsEnabled = false; //관리순서
                    chkYes.IsEnabled = true; //사용여부 Yes
                    chkNo.IsEnabled = false; //사용여부 No
                    txtComments.IsEnabled = false; //비고
                    //cboMain.IsEnabled = false; //Main항목여부 

                    dgdLarge.IsEnabled = false; //대분류그리드
                    dgdMiddle.IsEnabled = false; //중분류그리드
                    dgdList.IsEnabled = false; //항목그리드

                    btnSales.IsHitTestVisible = false; //매출 버튼
                    btnBuy.IsHitTestVisible = false; //매입 버튼

                    tabLarge.IsEnabled = false; //대분류탭
                    tabMiddle.IsEnabled = false; //중분류탭
                    tabList.IsEnabled = false; //항목탭

                    cboLarge.IsEnabled = false; //대분류 콤보박스
                    cboLarge2.IsEnabled = false; //대분류2 콤보박스
                    cboMiddle.IsEnabled = false; //중분류 콤보박스
                }

            }
            else
            {
                MessageBox.Show("수정할 자료를 선택하고 눌러주십시오.");
            }

        }

        //삭제
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            string sql = string.Empty;

            TabItem ti = tabAll.SelectedItem as TabItem;

            if (ti.Header.ToString().Equals("대분류"))
            {
                BSItemView = dgdLarge.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("중분류"))
            {
                BSItemView = dgdMiddle.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("항목"))
            {
                BSItemView = dgdList.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;
            }

            if (BSItemView == null)
            {
                MessageBox.Show("삭제할 데이터가 없습니다. 선택 후 눌러주세요.");
            }
            else
            {
                //대분류에서 삭제할 때
                if (ti.Header.ToString().Equals("대분류"))
                {
                    sql = "select BSItemMCode, BSItemSCode from Acc_BSItem_Code";
                    sql += " where BSItemLCode =" + BSItemView.BSItemLCode;
                    sql += " and UseYN = 'Y'";
                }
                //중분류에서 삭제할 때
                else if (ti.Header.ToString().Equals("중분류"))
                {
                    sql = "select BSItemSCode from Acc_BSItem_Code ";
                    sql += "where BSItemLCode =" + BSItemView.BSItemLCode;
                    sql += " and BSItemMCode =" + BSItemView.BSItemMCode;
                    sql += " and UseYN = 'Y'";
                }
                //항목에서 삭제할 때
                else if (ti.Header.ToString().Equals("항목"))
                {

                    if (MessageBox.Show("선택하신 항목을 삭제하시겠습니까? ", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (ti.Header.ToString().Equals("대분류"))
                        {
                            if (dgdLarge.Items.Count > 0 && dgdLarge.SelectedItem != null)
                            {
                                rowNum = dgdLarge.SelectedIndex;
                            }

                        }
                        else if (ti.Header.ToString().Equals("중분류"))
                        {
                            if (dgdMiddle.Items.Count > 0 && dgdMiddle.SelectedItem != null)
                            {
                                rowNum = dgdMiddle.SelectedIndex;
                            }
                        }
                        else if (ti.Header.ToString().Equals("항목"))
                        {
                            if (dgdList.Items.Count > 0 && dgdList.SelectedItem != null)
                            {
                                rowNum = dgdList.SelectedIndex;
                            }
                        }

                        //항목에서 삭제할 때 
                        if (Procedure.Instance.DeleteData(BSItemView.BSItemCode.ToString(), MainWindow.CurrentUser, "sItemCode", "UserID", "xp_Acc_BS_dItemCode"))
                        {
                            rowNum -= 1;
                            re_Search(rowNum);

                            return;
                        }
                    }
                }

                DataSet ds = DataStore.Instance.QueryToDataSet(sql);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 1) // L, M, S코드 3개 나올 꺼니까 1보다 크면이고,
                    {
                        if (MessageBox.Show("해당 코드의 사용중인 하위 코드(중분류, 항목)가 존재합니다. \n 삭제시 모두 삭제 됩니다. 삭제하시겠습니까? ", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            //sql = "update Acc_BSItem_Code ";
                            //sql += "set UseYN = 'N', UpdateUserID =" + MainWindow.CurrentUser + ", UpdateDate = "+ DateTime.Today.ToString();
                            //sql += " BSItemCode like " + BSItemView.BSItemCode.ToString().Trim() + "_%";

                            //MessageBox.Show(sql);
                        }
                    }
                    else if (dt.Rows.Count < 2) // L코드 1개 나올 꺼니까 2보다 작으면으로 조건 지정함.
                    {
                        if (MessageBox.Show("선택하신 항목을 삭제하시겠습니까? ", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            if (Procedure.Instance.DeleteData(BSItemView.BSItemCode.ToString(), MainWindow.CurrentUser, "sItemCode", "UserID", "xp_Acc_BS_dItemCode"))
                            {

                            }
                        }
                    }

                    if (ti.Header.ToString().Equals("대분류"))
                    {
                        if (dgdLarge.Items.Count > 0 && dgdLarge.SelectedItem != null)
                        {
                            rowNum = dgdLarge.SelectedIndex;
                        }

                    }
                    else if (ti.Header.ToString().Equals("중분류"))
                    {
                        if (dgdMiddle.Items.Count > 0 && dgdMiddle.SelectedItem != null)
                        {
                            rowNum = dgdMiddle.SelectedIndex;
                        }
                    }
                    else if (ti.Header.ToString().Equals("항목"))
                    {
                        if (dgdList.Items.Count > 0 && dgdList.SelectedItem != null)
                        {
                            rowNum = dgdList.SelectedIndex;
                        }
                    }

                    //하위항목있을 경우 삭제 
                    if (Procedure.Instance.DeleteData(BSItemView.BSItemCode.ToString().Trim(), MainWindow.CurrentUser, "sItemCode", "UserID", "xp_Acc_BS_dItemCode"))
                    {
                        rowNum -= 1;
                        re_Search(rowNum);
                    }
                }
            }
        }


        //닫기
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        //검색(조회)
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (btnSales.IsChecked == false && btnBuy.IsChecked == false)
            {
                MessageBox.Show("매출 혹은 매입 버튼을 먼저 선택하십시오.");
            }
            else
            {
                rowNum = 0;
                re_Search(rowNum);
            }
        }

        //저장
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = tabAll.SelectedItem as TabItem;

            if (ti.Header.ToString().Equals("대분류"))
            {
                BSItemView = dgdLarge.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("중분류"))
            {
                BSItemView = dgdMiddle.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("항목"))
            {
                BSItemView = dgdList.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;
            }

            if (SaveData(txtCode.Text, strFlag))
            {
                CanBtnControl();

                //2020.02.06 장가빈, 대분류, 중분류 콤보박스가 저장 후 선택된 값 그대로 보여지게 하기 위해 주석처리. 
                //SetComboBox();

                if (strFlag.Equals("U"))
                {
                    re_Search(rowNum);
                }
                else if (strFlag.Equals("I"))
                {

                    //일단 다 보여줌
                    rowNum = 0;
                    re_Search(rowNum);

                    if (ti.Header.ToString().Equals("대분류"))
                    {
                        rowNum = dgdLarge.Items.Count - 1;
                        re_Search(rowNum);
                    }
                    else if (ti.Header.ToString().Equals("중분류"))
                    {
                        rowNum = dgdMiddle.Items.Count - 1;
                        re_Search(rowNum);
                    }
                    else if (ti.Header.ToString().Equals("항목"))
                    {
                        rowNum = dgdList.Items.Count - 1;
                        re_Search(rowNum);
                    }
                }
                strFlag = string.Empty;
            }



            ////저장한 후 다시 콤보박스가 셋팅 되도록(전체 항목이 보이게 대분류, 중분류 콤보박스 초기화)
            //if (ti.Header.ToString().Equals("중분류"))
            //{
            //    //중분류 탭일 때 
            //    cboLarge.SelectedIndex = -1;
            //}
            //else if (ti.Header.ToString().Equals("항목"))
            //{
            //    //항목 탭일 때 
            //    cboLarge2.SelectedIndex = -1;
            //    cboMiddle.SelectedIndex = -1;
            //}

        }

        //취소
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CanBtnControl();
            strFlag = string.Empty;
            
        }

        //엑셀
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] lst = new string[6];
            lst[0] = "대분류코드";
            lst[1] = "중분류코드";
            lst[2] = "항목코드";
            lst[3] = dgdLarge.Name;
            lst[4] = dgdMiddle.Name;
            lst[5] = dgdList.Name;

            ExportExcelxaml ExpExc = new ExportExcelxaml(lst);

            ExpExc.ShowDialog();

            if (ExpExc.DialogResult.HasValue)
            {
                if (ExpExc.choice.Equals(dgdLarge.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdLarge);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdLarge);

                    Name = dgdLarge.Name;

                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else if (ExpExc.choice.Equals(dgdMiddle.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdMiddle);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdMiddle);

                    Name = dgdMiddle.Name;

                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else if (ExpExc.choice.Equals(dgdList.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdList);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdList);

                    Name = dgdList.Name;

                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
            }
        }

        #endregion

        #region 기타 메서드들

        //취소, 저장 후
        private void CanBtnControl()
        {
            Lib.Instance.UiButtonEnableChange_IUControl(this);

            txtKName.IsEnabled = false; //한글명
            txtEName.IsEnabled = false; //영문명
            txtOrder.IsEnabled = false; //관리순서
            chkYes.IsEnabled = false; //사용여부 Yes
            chkNo.IsEnabled = false; //사용여부 No
            txtComments.IsEnabled = false; //비고
            //cboMain.IsEnabled = false; //Main항목여부 

            dgdLarge.IsEnabled = true; //대분류그리드
            dgdMiddle.IsEnabled = true; //중분류그리드
            dgdList.IsEnabled = true; //항목그리드

            btnSales.IsHitTestVisible = true;
            btnBuy.IsHitTestVisible = true;

            tabLarge.IsEnabled = true; //대분류탭
            tabMiddle.IsEnabled = true; //중분류탭
            tabList.IsEnabled = true; //항목탭

            cboLarge.IsEnabled = true; //대분류 콤보박스
            cboLarge2.IsEnabled = true; //대분류2 콤보박스
            cboMiddle.IsEnabled = true; //중분류 콤보박스
        }

        //추가, 수정 클릭 시
        private void CantBtnControl()
        {

            Lib.Instance.UiButtonEnableChange_SCControl(this);

            txtKName.IsEnabled = true; //한글명
            txtEName.IsEnabled = true; //영문명
            txtOrder.IsEnabled = true; //관리순서
            chkYes.IsEnabled = true; //사용여부 Yes
            chkNo.IsEnabled = false; //사용여부 No, 삭제를 통해서만 아니오가 되게.
            txtComments.IsEnabled = true; //비고

            dgdLarge.IsEnabled = false; //대분류그리드
            dgdMiddle.IsEnabled = false; //중분류그리드
            dgdList.IsEnabled = false; //항목그리드

            btnSales.IsHitTestVisible = false; //매출 버튼
            btnBuy.IsHitTestVisible = false; //매입 버튼

            tabLarge.IsEnabled = false; //대분류탭
            tabMiddle.IsEnabled = false; //중분류탭
            tabList.IsEnabled = false; //항목탭

            //중분류, 항목의 분류를 변경할 때에는 삭제 후 추가를 하도록 안내. 안그럼 BSItemCode가 엉켜 힘들어..
            cboLarge.IsEnabled = false; //대분류 콤보박스
            cboLarge2.IsEnabled = false; //대분류2 콤보박스
            cboMiddle.IsEnabled = false; //중분류 콤보박스

            TabItem ti = tabAll.SelectedItem as TabItem;

            if (ti.Header.ToString().Equals("대분류"))
            {
                //대분류일 때는 선택할 수 없도록
                //cboMain.IsEnabled = false;
            }
            else if (ti.Header.ToString().Equals("중분류"))
            {
                //중분류일 때는 선택할 수 없도록
                //cboMain.IsEnabled = false;
            }
            else if (ti.Header.ToString().Equals("항목"))
            {
                //중분류일 때는 선택할 수 없도록
                //cboMain.IsEnabled = true;
            }
        }

        //재조회
        private void re_Search(int selectedIndex)
        {
            FillGrid();

            if (dgdLarge.Items.Count > 0)
            {
                dgdLarge.SelectedIndex = selectedIndex;
            }
            else if (dgdMiddle.Items.Count > 0)
            {
                dgdMiddle.SelectedIndex = selectedIndex;
            }
            else if (dgdList.Items.Count > 0)
            {
                dgdList.SelectedIndex = selectedIndex;
            }
            else
            {
                this.DataContext = null;
            }

        }

        //조회
        private void FillGrid ()
        {

            if (dgdLarge.Items.Count > 0)
            {
                dgdLarge.Items.Clear();
            }
            else if (dgdMiddle.Items.Count > 0)
            {
                dgdMiddle.Items.Clear();
            }
            else if (dgdList.Items.Count > 0)
            {
                dgdList.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                TabItem ti = tabAll.SelectedItem as TabItem;

                sqlParameter.Clear();
                if (ti.Header.ToString().Equals("중분류") && cboLarge.SelectedValue != null && !cboLarge.SelectedValue.ToString().Equals(""))
                {
                    sqlParameter.Add("LCode", cboLarge.SelectedValue.ToString());
                    sqlParameter.Add("MCode", "");
                }

                else if (ti.Header.ToString().Equals("항목") && cboMiddle.SelectedValue != null && !cboMiddle.SelectedValue.ToString().Equals(""))
                {
                    sqlParameter.Add("LCode", cboLarge2.SelectedValue.ToString());
                    sqlParameter.Add("MCode", cboMiddle.SelectedValue.ToString());
                }

                else
                {
                    sqlParameter.Add("LCode", "");
                    sqlParameter.Add("MCode", "");
                }


                sqlParameter.Add("SCode", "");
                sqlParameter.Add("BSItemName", "");
                sqlParameter.Add("bsGbnID", btnSales.IsChecked == true ? "2" : "1");
                sqlParameter.Add("UseYN", chkNotUseSrh.IsChecked == true ? "" : "Y");

                if (ti.Header.ToString().Equals("대분류"))
                {
                    sqlParameter.Add("LMS", "L");
                }
                else if (ti.Header.ToString().Equals("중분류"))
                {
                    sqlParameter.Add("LMS", "M");
                }
                else
                {
                    sqlParameter.Add("LMS", "S");
                }


                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_BS_sItemCode_WPF", sqlParameter, false);

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
                            var BSItemView = new frm_Acc_BS_Item_Code_U_CodeView()
                            {
                                Num = i,
                                BSItemCode = dr["BSItemCode"].ToString(), //코드번호
                                BSItemLCode = dr["BSItemLCode"].ToString(), //대분류
                                BSItemMCode = dr["BSItemMCode"].ToString(), //중분류
                                BSItemSCode = dr["BSItemSCode"].ToString(), //항목
                                BSItemName = dr["BSItemName"].ToString(), //한글명
                                BSGbn = dr["BSGbn"].ToString(), //매입/매출 구분
                                UseYN = dr["UseYN"].ToString(), //사용여부
                                BSItemNameEng = dr["BSItemNameEng"].ToString(), //영문명
                                Comments = dr["Comments"].ToString(), //비고
                                Seq = dr["Seq"].ToString(), //관리순서

                                LargeName = dr["BSItemLName"].ToString(), //대분류명
                                MiddleName = dr["BSItemMName"].ToString(),  //중분류명 
                                ListName = dr["BSItemSName"].ToString(), //항목명

                            };

                            if (ti.Header.ToString().Equals("대분류"))
                            {
                                dgdLarge.Items.Add(BSItemView);
                            }
                            else if (ti.Header.ToString().Equals("중분류"))
                            {
                                dgdMiddle.Items.Add(BSItemView);
                            }
                            else if (ti.Header.ToString().Equals("항목"))
                            {
                                dgdList.Items.Add(BSItemView);
                            }
                        }
                    } else if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("조회 된 결과가 없습니다");
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


        //저장
        private bool SaveData(string strCode, string strFlag)
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                if (CheckData())
                {
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    TabItem ti = tabAll.SelectedItem as TabItem;

                    sqlParameter.Add("BSItemName", txtKName.Text);
                    //sqlParameter.Add("MainItemGbn", cboMain.SelectedValue.ToString());

                    sqlParameter.Add("BSGBN", btnSales.IsChecked == true ? 2 : 1);
                    sqlParameter.Add("Seq", txtOrder.Text);
                    sqlParameter.Add("Comments", txtComments.Text);
                    sqlParameter.Add("BSItemNameEng", txtEName.Text);

                    sqlParameter.Add("RtnMsg", "");

                    //추가일 때
                    if (strFlag.Equals("I"))
                    {
                        if (ti.Header.ToString().Equals("대분류"))
                        {
                            sqlParameter.Add("BSItemLCode", "");
                            sqlParameter.Add("BSItemMCode", "");
                        }
                        if (ti.Header.ToString().Equals("중분류"))
                        {
                            sqlParameter.Add("BSItemLCode", cboLarge.SelectedValue.ToString());
                            sqlParameter.Add("BSItemMCode", "");
                        }
                        if (ti.Header.ToString().Equals("항목"))
                        {
                            sqlParameter.Add("BSItemLCode", cboLarge2.SelectedValue.ToString());
                            sqlParameter.Add("BSItemMCode", cboMiddle.SelectedValue.ToString());
                        }

                        sqlParameter.Add("UseYN", chkYes.IsChecked == true ? "Y" : "N");
                        sqlParameter.Add("BSItemSCode", "");
                        sqlParameter.Add("UserID", MainWindow.CurrentUser);

                        Procedure pro1 = new Procedure();
                        pro1.Name = "xp_Acc_BS_iItemCode_WPF";
                        pro1.OutputUseYN = "N";
                        pro1.OutputName = "RtnMsg";
                        pro1.OutputLength = "10";

                        Prolist.Add(pro1);
                        ListParameter.Add(sqlParameter);

                        string[] Confirm = new string[2];
                        Confirm = DataStore.Instance.ExecuteAllProcedureOutputNew(Prolist, ListParameter);

                        if (Confirm[0] != "success")
                        {
                            MessageBox.Show("[저장실패]\r\n" + Confirm[1].ToString());
                            flag = false;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    //수정일 때 
                    else if (strFlag.Equals("U"))
                    {
                        if (ti.Header.ToString().Equals("대분류"))
                        {
                            sqlParameter.Add("BSItemLCode", BSItemView.BSItemLCode);
                            sqlParameter.Add("BSItemMCode", "");
                        }
                        if (ti.Header.ToString().Equals("중분류"))
                        {
                            sqlParameter.Add("BSItemLCode", cboLarge.SelectedValue != null ? cboLarge.SelectedValue.ToString() : BSItemView.BSItemLCode.Trim().ToString());
                            sqlParameter.Add("BSItemMCode", BSItemView.BSItemMCode);
                        }
                        if (ti.Header.ToString().Equals("항목"))
                        {
                            sqlParameter.Add("BSItemLCode", cboLarge2.SelectedValue != null ? cboLarge2.SelectedValue.ToString() : BSItemView.BSItemLCode.Trim().ToString());
                            sqlParameter.Add("BSItemMCode", cboMiddle.SelectedValue != null ? cboMiddle.SelectedValue.ToString() : BSItemView.BSItemMCode.Trim().ToString());
                        }

                        sqlParameter.Add("UseYN", chkYes.IsChecked == true ? "Y" : "N");
                        sqlParameter.Add("BSItemSCode", BSItemView.BSItemSCode);
                        sqlParameter.Add("sItemCode", BSItemView.BSItemCode);
                        sqlParameter.Add("UserID", MainWindow.CurrentUser);

                        Procedure pro1 = new Procedure();
                        pro1.Name = "xp_Acc_BS_uItemCode_WPF";
                        pro1.OutputUseYN = "N";
                        pro1.OutputName = "RtnMsg";
                        pro1.OutputLength = "10";

                        Prolist.Add(pro1);
                        ListParameter.Add(sqlParameter);

                        string[] Confirm = new string[2];
                        Confirm = DataStore.Instance.ExecuteAllProcedureOutputNew(Prolist, ListParameter);

                        if (Confirm[0] != "success")
                        {
                            MessageBox.Show("[저장실패]\r\n" + Confirm[1].ToString());
                            flag = false;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                }
                else
                {
                    flag = false;
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

            return flag;
        }


        //데이터 체크
        private bool CheckData()
        {
            bool flag = true;

            if (txtKName.Text.Length <= 0 || txtKName.Text.Equals(""))
            {
                MessageBox.Show("한글명이 입력되지 않았습니다.");

                txtKName.Focus();

                flag = false;
                return flag;
            }

            //if (cboMain.SelectedValue == null)
            //{
            //    MessageBox.Show("Main항목여부가 선택되지 않았습니다.");

            //    flag = false;
            //    return flag;

            //}

            if (chkYes.IsChecked == false && chkNo.IsChecked == false)
            {
                MessageBox.Show("사용여부가 선택되지 않았습니다.");
                flag = false;
                return flag;
            }
            else if (chkYes.IsChecked == true && chkNo.IsChecked == true)
            {
                MessageBox.Show("예(Y) 혹은 아니오(N) 중 하나만 선택하십시오.");
                flag = false;
                return flag;
            }

            //Main항목여부가 Y, 1일 경우 체크(Main은 1개만 존재해야 함) * UseYN이 Y인 경우만
            //if (cboMain.SelectedItem != null && cboMain.SelectedValue.ToString().Equals("1")) //콤보메인이 널이 아니고 1인경우 (0이 N, 1은 Y)
            //{
            //    string sql = "select COUNT(*) from Acc_BSItem_Code where MainItemGbn = 1 and UseYN = 'Y'";

            //    if (strFlag == "U")
            //    {
            //        sql += " and BSItemCode !=" + BSItemView.BSItemCode.ToString().Trim();
            //    }
            //    else if (strFlag == "I")
            //    {
            //        //추가 sql없이 실행 
            //    }

            //    // 매출 매입 비교 구분자
            //    if (btnSales.IsChecked == true)
            //    {
            //        sql += " and BSGbn = 2";  //매출
            //    }
            //    else
            //    {
            //        sql += " and BSGbn = 1";  //매입
            //    }

            //    try
            //    {
            //        DataSet ds = DataStore.Instance.QueryToDataSet(sql);
            //        if (ds != null && ds.Tables.Count > 0)
            //        {
            //            DataTable dt = ds.Tables[0];
            //            if (dt.Rows.Count == 0)
            //            {
            //            }
            //            else
            //            {
            //                DataRowCollection drc = dt.Rows;

            //                foreach (DataRow item in drc)
            //                {
            //                    var BsCount = new BSCount();
            //                    {
            //                        //Main항목 1 존재 여부 갯수가 1이면 이미 Main이 있다는 뜻임
            //                        BsCountNum = Convert.ToInt32(item[0].ToString().Trim());
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show("콤보박스 생성 중 오류 발생 : " + ex.ToString());

            //    }
            //    finally
            //    {
            //        DataStore.Instance.CloseConnection();
            //    }


            //    //Main항목의 갯수가 0이 아니면 
            //    if (!BsCountNum.ToString().Equals("0"))
            //    {
            //        MessageBox.Show("이미 Main인 항목이 존재합니다.");

            //        //cboMain이 N으로 설정되도록
            //        //cboMain.SelectedIndex = 0;

            //        flag = false;
            //        return flag;
            //    }
            //}

            // 추가, 수정일때만 데이터 체크
            //if (!strFlag.Equals(string.Empty))
            //{
            //// 중분류 탭이 활성화 됐을때
            //if (ti.Header.ToString().Equals("중분류") == true)
            //{
            //    if (cboLarge.SelectedItem == null)
            //    {
            //        MessageBox.Show("대분류가 선택되지 않았습니다.");
            //        flag = false;
            //        return flag;
            //    }
            //}

            // 항목 탭 활성화 됐을때
            //if (ti.Header.ToString().Equals("항목") == true)
            //{
            //    if (cboLarge2.SelectedItem == null || cboMiddle.SelectedItem == null)
            //    {
            //        MessageBox.Show("대분류 혹은 중분류가 선택되지 않았습니다.");
            //        flag = false;
            //        return flag;
            //    }
            //}

            return flag;
        }


        #endregion

        #region 왼쪽 상단 매입 매출 구분 버튼

        //매입
        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            btnSales.IsChecked = false;
            btnBuy.IsChecked = true;

            SetComboBox(); // 매입의 분류만 나오게 다시 셋팅

            rowNum = 0;
            re_Search(rowNum);

        }

        //매출
        private void btnSales_Click(object sender, RoutedEventArgs e)
        {
            btnSales.IsChecked = true;
            btnBuy.IsChecked = false;

            SetComboBox(); // 매출의 분류만 나오게 다시 셋팅

            rowNum = 0;
            re_Search(rowNum);
        }

        #endregion

        #region 데이터그리드 SelectionChanged

        //대분류 이벤트 
        private void dgdLarge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BSItemView = dgdLarge.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;

            //데이터가 널이 아닐 때만 작동
            if (BSItemView != null)
            {

                //사용여부 체크박스  
                if (BSItemView.UseYN.Equals("") || BSItemView.UseYN == null)
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
                else if (BSItemView.UseYN.Equals("Y"))
                {
                    chkYes.IsChecked = true;
                    chkNo.IsChecked = false;
                }
                else
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
            }
            this.DataContext = BSItemView;
        }

        //중분류 이벤트
        private void dgdMiddle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BSItemView = dgdMiddle.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;

            //데이터가 널이 아닐 때만 작동
            if (BSItemView != null)
            {
                BSItemView.BSItemName = BSItemView.MiddleName;

                //사용여부 체크박스  
                if (BSItemView.UseYN.Equals("") || BSItemView.UseYN == null)
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
                else if (BSItemView.UseYN.Equals("Y"))
                {
                    chkYes.IsChecked = true;
                    chkNo.IsChecked = false;
                }
                else
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
            }
            this.DataContext = BSItemView;
        }

        //항목 이벤트
        private void dgdList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BSItemView = dgdList.SelectedItem as frm_Acc_BS_Item_Code_U_CodeView;

            //데이터가 널이 아닐 때만 작동
            if (BSItemView != null)
            {
                BSItemView.BSItemName = BSItemView.ListName;

                //사용여부 체크박스  
                if (BSItemView.UseYN.Equals("") || BSItemView.UseYN == null)
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
                else if (BSItemView.UseYN.Equals("Y"))
                {
                    chkYes.IsChecked = true;
                    chkNo.IsChecked = false;
                }
                else
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
            }

            this.DataContext = BSItemView;
        }
        #endregion

        //탭 이동시 발생 이벤트
        private void tabControl_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // → 미친듯이 발생해서 쓰레드 에러 발생함
            //rowNum = 0;
            //re_Search(rowNum);
        }

        // 위 문제 대체로 탭 클릭 이벤트 둘리
        // 대분류 탭 클릭 이벤트
        private void tabLarge_Click(object sender, MouseButtonEventArgs e)
        {

            //Main 항목여부 라벨 가려주기
            //lblMain.Visibility = Visibility.Hidden;
            //Main 항목여부 콤보박스 가려주기
            //cboMain.Visibility = Visibility.Hidden;
            //Main 항목여부 설명 텍스트 블럭 가려주기
            tbMain.Visibility = Visibility.Hidden;

            if (Equals(sender, e.Source))
            {
                rowNum = 0;
                re_Search(rowNum);
            }

        }
        // 중분류 탭 클릭 이벤트
        private void tabMiddle_Click(object sender, MouseButtonEventArgs e)
        {
            if (Equals(sender, e.Source))
            {

                //Main 항목여부 라벨 가려주기
                //lblMain.Visibility = Visibility.Hidden;
                //Main 항목여부 콤보박스 가려주기
                //cboMain.Visibility = Visibility.Hidden;
                //Main 항목여부 설명 텍스트 블럭 가려주기
                tbMain.Visibility = Visibility.Hidden;


                //대분류 탭에서 대분류 항목을 추가하고 중분류 탭으로 넘어온다면, 콤보박스 재셋팅 2020.02.22, 장가빈
                //대분류 목록 가져오기
                ObservableCollection<CodeView> cboLargeList = Direct_SetComboBoxLarge();
                //중분류탭의 대분류 콤보박스
                this.cboLarge.ItemsSource = cboLargeList;
                this.cboLarge.DisplayMemberPath = "code_name";
                this.cboLarge.SelectedValuePath = "code_id";
                
                rowNum = 0;
                re_Search(rowNum);
            }
        }
        // 항목 탭 클릭 이벤트
        private void tabList_Click(object sender, MouseButtonEventArgs e)
        {
            if (Equals(sender, e.Source))
            {
                //Main 항목여부 라벨 보여주기
                //lblMain.Visibility = Visibility.Visible;
                //Main 항목여부 콤보박스 보여주기
                //cboMain.Visibility = Visibility.Visible;
                //Main 항목여부 설명 텍스트 블럭 보여주기
                tbMain.Visibility = Visibility.Visible;


                //대분류와 중분류 탭에서 분류를 추가하고 항목 탭으로 넘어온다면, 콤보박스 재셋팅 2020.02.22, 장가빈
                ObservableCollection<CodeView> cboLargeList2 = Direct_SetComboBoxLarge();
                //항목탭의 대분류 콤보박스
                this.cboLarge2.ItemsSource = cboLargeList2;
                this.cboLarge2.DisplayMemberPath = "code_name";
                this.cboLarge2.SelectedValuePath = "code_id";

                //대분류가 재샛팅 되어도 중분류 콤보박스에는 값이 남아있어서 초기화를 시켜주기로 함. 2020.02.22, 장가빈
                cboMiddle.SelectedIndex = -1;

                rowNum = 0;
                re_Search(rowNum);
            }
        }

        // 대분류 목록 가져오기 둘리
        private ObservableCollection<CodeView> Direct_SetComboBoxLarge()
        {
            ObservableCollection<CodeView> retunCollection = new ObservableCollection<CodeView>();
            string sql = " select BSItemLCode, BSItemName ";
            sql += " from Acc_BSItem_Code ";
            sql += " where UseYN = 'Y' and ISNULL(BSItemLCode, '') != '' and ISNULL(BSItemMCode, '') = ''and ISNULL(BSItemSCode, '') = '' ";

            if (btnSales.IsChecked == true && btnBuy.IsChecked == false)
            {
                sql += " and BsGbn = 2 ";
            }
            else if (btnBuy.IsChecked == true && btnSales.IsChecked == false)
            {
                sql += " and BsGbn = 1 ";
            }

            try
            {
                DataSet ds = DataStore.Instance.QueryToDataSet(sql);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow item in drc)
                        {

                            CodeView mCodeView = new CodeView()
                            {
                                code_id = item[0].ToString().Trim(),
                                code_name = item[1].ToString().Trim()
                            };
                            retunCollection.Add(mCodeView);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("콤보박스 생성 중 오류 발생 : " + ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return retunCollection;
        }

        //중분류 목록 가져오기  
        private ObservableCollection<CodeView> Direct_SetComboBoxMiddle(string BSItemLCode)
        {
            ObservableCollection<CodeView> retunCollection = new ObservableCollection<CodeView>();
            string sql = " select BSItemMCode, BSItemName ";
            sql += " from Acc_BSItem_Code ";
            sql += " where UseYN = 'Y' and ISNULL(BSItemLCode, '') != '' and ISNULL(BSItemMCode, '') != ''and ISNULL(BSItemSCode, '') = ''";

            if (BSItemLCode != null && !BSItemLCode.Equals(""))
            {
                sql += " and BSItemLCode = " + BSItemLCode;
            }

            if (btnSales.IsChecked == true)
            {
                sql += " and BsGbn = 2 ";
            }
            else if (btnBuy.IsChecked == true)
            {
                sql += " and BsGbn = 1 ";
            }


            try
            {
                DataSet ds = DataStore.Instance.QueryToDataSet(sql);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow item in drc)
                        {

                            CodeView mCodeView = new CodeView()
                            {
                                code_id = item[0].ToString().Trim(),
                                code_name = item[1].ToString().Trim()
                            };
                            retunCollection.Add(mCodeView);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("콤보박스 생성 중 오류 발생 : " + ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return retunCollection;
        }

        #region 생성자

        class frm_Acc_BS_Item_Code_U_CodeView : BaseView
        {
            public int Num { get; set; }
            public string BSItemCode { get; set; }
            public string BSGbn { get; set; }
            public string Seq { get; set; }
            public string BSItemLCode { get; set; }
            public string BSItemMCode { get; set; }
            public string BSItemSCode { get; set; }
            public string BSItemName { get; set; }
            public string BSItemNameEng { get; set; }
            public string MainItemGbn { get; set; }
            public string UseYN { get; set; }
            public string Comments { get; set; }
            public string createDate { get; set; }
            public string createUserID { get; set; }
            public string UpdateDate { get; set; }
            public string UpdateUserID { get; set; }

            public string LargeName { get; set; } //대분류명
            public string MiddleName { get; set; } //중분류명
            public string ListName { get; set; } //항목명 

        }

        class BSCount
        {
            int BsCountNum { get; set; } // Main항목 존재여부 확인
        }


        #endregion

        // 중분류탭 : 대분류 콤보박스 선택 이벤트
        private void cboLarge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboLarge.SelectedValue != null && !cboLarge.SelectedValue.ToString().Equals("")
                    && strFlag.Equals("U") == false)
            {
                rowNum = 0;
                re_Search(rowNum);
            }
        }

        // 항목탭 : 대분류 값 선택시 항목 값 자동 설정
        private void cboLarge2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboLarge2.SelectedValue != null && !cboLarge2.SelectedValue.ToString().Equals(""))
            {
                ObservableCollection<CodeView> cboMiddleList = Direct_SetComboBoxMiddle(cboLarge2.SelectedValue.ToString());
                //항목탭의 중분류 콤보박스
                this.cboMiddle.ItemsSource = cboMiddleList;
                this.cboMiddle.DisplayMemberPath = "code_name";
                this.cboMiddle.SelectedValuePath = "code_id";
            }


            if (cboLarge2.SelectedValue != null && !cboLarge2.SelectedValue.ToString().Equals(""))
            {
                //대분류 선택 후 중분류 콤보박스에 포커스가 맞춰지도록
                cboMiddle.Focus();
                lib.SendK(Key.Enter, this);
                cboMiddle.IsDropDownOpen = true;
            }
        }

        // 항목탭 : 중분류 콤보박스 선택 이벤트
        private void cboMiddle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMiddle.SelectedValue != null && !cboMiddle.SelectedValue.ToString().Equals("")
                    && strFlag.Equals("U") == false)
            {
                rowNum = 0;
                re_Search(rowNum);
            }
        }

        //한글명 key.enter
        private void TxtKName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtOrder.Focus();
            }
        }

        //관리순서 key.enter
        private void TxtOrder_KeyDown(object sender, KeyEventArgs e)

        {
            if (e.Key == Key.Enter)
            {
                txtComments.Focus();
            }
        }

        //비고 key.enter
        private void TxtComments_KeyDown(object sender, KeyEventArgs e)
        {
            TabItem ti = tabAll.SelectedItem as TabItem;

            if (ti.Header.ToString().Equals("대분류") || ti.Header.ToString().Equals("중분류"))
            {
                if (e.Key == Key.Enter)
                {
                    txtEName.Focus();
                }
            }
            else if (ti.Header.ToString().Equals("항목"))
            {
                if (e.Key == Key.Enter)
                {
                    //cboMain.Focus();
                    lib.SendK(Key.Enter, this);
                    //cboMain.IsDropDownOpen = true;
                }
            }
        }

        //Main항목여부
        private void CboMain_DropDownClosed(object sender, EventArgs e) //콤보박스 목록이 닫길 때 나타나는 이벤트
        {
            //탭이 항목일 때만 cboMain을 탈 꺼니까
            TabItem ti = tabAll.SelectedItem as TabItem;

            if (ti.Header.ToString().Equals("항목"))
            {
                txtEName.Focus();
            }
        }

        //영문명 key.enter
        private void TxtEName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSave.Focus();
                btnSave.Focus();
            }
        }

        //사용안함 포함 라벨 클릭 시
        private void LblNotUseSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkNotUseSrh.IsChecked == true)
            {
                chkNotUseSrh.IsChecked = false;
            }
            else
            {
                chkNotUseSrh.IsChecked = true;
            }
        }
    }
}
