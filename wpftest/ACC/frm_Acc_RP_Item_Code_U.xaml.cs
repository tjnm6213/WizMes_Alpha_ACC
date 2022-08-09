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
    /// frm_Acc_RP_Item_Code_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class frm_Acc_RP_Item_Code_U : UserControl
    {

        string strFlag = string.Empty;
        int rowNum = 0;

        Lib lib = new Lib();
        frm_Acc_RP_Item_Code_U_CodeView RPItemView = new frm_Acc_RP_Item_Code_U_CodeView();


        public frm_Acc_RP_Item_Code_U()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            SetComboBox();

            btndeposit.IsChecked = true;
        }

        #region 콤보박스 셋팅 
        private void SetComboBox()
        {
            //현금항목여부 콤보박스 목록 지정
            List<string> cboCashList = new List<string>();
            cboCashList.Add("N");
            cboCashList.Add("Y");
            
            //현금항목여부
            //ObservableCollection<CodeView> cboCashlist = ComboBoxUtil.Instance.Direct_SetComboBox(cboCashList);
            //this.cboCash.ItemsSource = cboCashlist;
            //this.cboCash.DisplayMemberPath = "code_name";
            //this.cboCash.SelectedValuePath = "code_id";

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

        #region 상단 왼쪽 입금/출금 버튼

            //입금
            private void btndeposit_Click(object sender, RoutedEventArgs e)
        {
            btndeposit.IsChecked = true;
            btnwithdraw.IsChecked = false;


            SetComboBox(); // 입금의 분류만 나오게 다시 셋팅

            rowNum = 0;
            re_Search(rowNum);

        }

        //출금
        private void btnwithdraw_Click(object sender, RoutedEventArgs e)
        {
            btndeposit.IsChecked = false;
            btnwithdraw.IsChecked = true;

            SetComboBox(); // 출금의 분류만 나오게 다시 셋팅

            rowNum = 0;
            re_Search(rowNum);

        }

        #endregion

        #region 우측 상단 버튼

        //추가
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

            TabItem ti = tabAll.SelectedItem as TabItem;

            if (btndeposit.IsChecked == false && btnwithdraw.IsChecked == false)
            {
                MessageBox.Show("입금 혹은 출근 버튼을 먼저 선택하십시오.");
            }
            else if(ti.Header.ToString().Equals("중분류") && cboLarge.SelectedItem == null)
            {
                MessageBox.Show("대분류를 선택 후 추가를 진행 해주세요.");

                //콤보박스에 포커스가 맞춰지도록
                cboLarge.Focus();
                lib.SendK(Key.Enter, this);
                cboLarge.IsDropDownOpen = true;
            }
            else if((ti.Header.ToString().Equals("항목") && cboLarge2.SelectedItem == null) || (ti.Header.ToString().Equals("항목") && cboMiddle.SelectedItem == null))
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
                chkYes.IsChecked = true; //사용여부 체크박스는 기본값이 Yes가 되도록.
                chkNo.IsChecked = false; //체크박스 비우기
                chkProduct.IsChecked = true;
                chkBuyOrSales.IsChecked = true;
                CantBtnControl();

                txtKName.Focus(); //추가버튼 클릭시 한글명 텍스트박스에 커서가 가도록 

                chkCashYes.IsChecked= true; //현금항목여부에 Y로 기본값 셋팅되도록 설정
                                       
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
                RPItemView = dgdLarge.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("중분류"))
            {
                RPItemView = dgdMiddle.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("항목"))
            {
                RPItemView = dgdList.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;
            }

            if (RPItemView != null)
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
                    chkCashYes.IsEnabled = false; //Main항목여부 

                    dgdLarge.IsEnabled = false; //대분류그리드
                    dgdMiddle.IsEnabled = false; //중분류그리드
                    dgdList.IsEnabled = false; //항목그리드

                    btndeposit.IsHitTestVisible = false; //입금 버튼
                    btnwithdraw.IsHitTestVisible = false; //출금 버튼

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
                RPItemView = dgdLarge.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("중분류"))
            {
                RPItemView = dgdMiddle.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("항목"))
            {
                RPItemView = dgdList.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;
            }

            if (RPItemView == null)
            {
                MessageBox.Show("삭제할 데이터가 없습니다. 선택 후 눌러주세요.");
            }
            else
            {
                //대분류에서 삭제할 때
                if (ti.Header.ToString().Equals("대분류"))
                {
                    sql = "select RPItemMCode, RPItemSCode from Acc_RPItem_Code";
                    sql += " where RPItemLCode =" + RPItemView.RPItemLCode;
                    sql += " and UseYN = 'Y'";
                }
                //중분류에서 삭제할 때
                else if (ti.Header.ToString().Equals("중분류"))
                {
                    sql = "select RPItemSCode from Acc_RPItem_Code ";
                    sql += "where RPItemLCode =" + RPItemView.RPItemLCode;
                    sql += " and RPItemMCode =" + RPItemView.RPItemMCode;
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
                        if (Procedure.Instance.DeleteData(RPItemView.RPItemCode.ToString(), MainWindow.CurrentUser, "sItemCode", "UserID", "xp_Acc_BS_dRPItemCode"))
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
                    if (dt.Rows.Count > 1) //데이터가 2개이상이면(하위항목이 존재한다는 말)
                    {
                        if (MessageBox.Show("해당 코드의 사용중인 하위 코드(중분류, 항목)가 존재합니다. \n 삭제시 모두 삭제 됩니다. 삭제하시겠습니까? ", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {

                        }
                    }
                    else if (dt.Rows.Count < 2) // 데이터가 1개이하이면 (하위항목이 없다는 말)
                    {
                        if (MessageBox.Show("선택하신 항목을 삭제하시겠습니까? ", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            if (Procedure.Instance.DeleteData(RPItemView.RPItemCode.ToString(), MainWindow.CurrentUser, "sItemCode", "UserID", "xp_Acc_BS_dRPItemCode"))
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
                    if (Procedure.Instance.DeleteData(RPItemView.RPItemCode.ToString().Trim(), MainWindow.CurrentUser, "sItemCode", "UserID", "xp_Acc_BS_dRPItemCode"))
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

        //저장
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            TabItem ti = tabAll.SelectedItem as TabItem;

            if (ti.Header.ToString().Equals("대분류"))
            {
                RPItemView = dgdLarge.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("중분류"))
            {
                RPItemView = dgdMiddle.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;
            }
            else if (ti.Header.ToString().Equals("항목"))
            {
                RPItemView = dgdList.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;
            }

            if (SaveData(txtCode.Text, strFlag))
            {
                //2020.02.22 장가빈, 대분류, 중분류 콤보박스가 저장 후 선택된 값 그대로 보여지게 하기 위해 주석처리. 
                //SetComboBox();

                CanBtnControl();

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
        }

        //검색
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (btndeposit.IsChecked == false && btnwithdraw.IsChecked == false)
            {
                MessageBox.Show("입금 혹은 출근 버튼을 먼저 선택하십시오.");
            }
            else
            {
                rowNum = 0;
                re_Search(rowNum);
            }
        }

        //취소
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CanBtnControl();
            strFlag = string.Empty;
            re_Search(rowNum);
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

        #region 탭 이벤트 

        // 대분류 탭 클릭 이벤트
        private void tabLarge_Click(object sender, MouseButtonEventArgs e)
        {
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
                // 중분류 탭 클릭시 중분류 안의 콤보박스 초기화
                cboLarge.SelectedIndex = -1;
                rowNum = 0;
                re_Search(rowNum);
            }
        }

        // 항목 탭 클릭 이벤트
        private void tabList_Click(object sender, MouseButtonEventArgs e)
        {
            lblProduct.Visibility = Visibility.Visible;
            chkProduct.Visibility = Visibility.Visible;
            chkBuyOrSales.Visibility = Visibility.Visible;
            lblBuyOrSales.Visibility = Visibility.Visible;

            if (Equals(sender, e.Source))
            {
                cboLarge2.SelectedIndex = -1;
                cboMiddle.SelectedIndex = -1;
                rowNum = 0;
                re_Search(rowNum);

            }
        }

        #endregion



        #region SelectionChaned 

        //대분류 
        private void dgdLarge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RPItemView = dgdLarge.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;

            //데이터가 널이 아닐 때만 작동
            if (RPItemView != null)
            {

                //사용여부 체크박스  
                if (RPItemView.UseYN.Equals("") || RPItemView.UseYN == null)
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
                else if (RPItemView.UseYN.Equals("Y"))
                {
                    chkYes.IsChecked = true;
                    chkNo.IsChecked = false;
                }
                else
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }

                //매출품여부
                if (RPItemView.ProductWongaYN.Equals("") || RPItemView.UseYN == null)
                {
                    chkProduct.IsChecked = false;
                    chkProduct.IsChecked = false;
                }
                else if (RPItemView.ProductWongaYN.Equals("Y"))
                {
                    chkProduct.IsChecked = true;
                    chkProduct.IsChecked = false;
                }
                else
                {
                    chkProduct.IsChecked = false;
                    chkProduct.IsChecked = false;
                }
                //제조원가
                if (RPItemView.BuySaleYN.Equals("") || RPItemView.UseYN == null)
                {
                    chkBuyOrSales.IsChecked = false;
                    chkBuyOrSales.IsChecked = false;
                }
                else if (RPItemView.BuySaleYN.Equals("Y"))
                {
                    chkBuyOrSales.IsChecked = true;
                    chkBuyOrSales.IsChecked = false;
                }
                else
                {
                    chkBuyOrSales.IsChecked = false;
                    chkBuyOrSales.IsChecked = false;
                }
            }
            this.DataContext = RPItemView;
        }


        //중분류 
        private void dgdMiddle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RPItemView = dgdMiddle.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;

            //데이터가 널이 아닐 때만 작동
            if (RPItemView != null)
            {
                RPItemView.RPItemName = RPItemView.MiddleName;

                //사용여부 체크박스  
                if (RPItemView.UseYN.Equals("") || RPItemView.UseYN == null)
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
                else if (RPItemView.UseYN.Equals("Y"))
                {
                    chkYes.IsChecked = true;
                    chkNo.IsChecked = false;
                }
                else
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
                //매출품여부
                if (RPItemView.ProductWongaYN.Equals("") || RPItemView.UseYN == null)
                {
                    chkProduct.IsChecked = false;
                    chkProduct.IsChecked = false;
                }
                else if (RPItemView.ProductWongaYN.Equals("Y"))
                {
                    chkProduct.IsChecked = true;
                    chkProduct.IsChecked = false;
                }
                else
                {
                    chkProduct.IsChecked = false;
                    chkProduct.IsChecked = false;
                }
                //제조원가
                if (RPItemView.BuySaleYN.Equals("") || RPItemView.UseYN == null)
                {
                    chkBuyOrSales.IsChecked = false;
                    chkBuyOrSales.IsChecked = false;
                }
                else if (RPItemView.BuySaleYN.Equals("Y"))
                {
                    chkBuyOrSales.IsChecked = true;
                    chkBuyOrSales.IsChecked = false;
                }
                else
                {
                    chkBuyOrSales.IsChecked = false;
                    chkBuyOrSales.IsChecked = false;
                }
            }
            this.DataContext = RPItemView;
        }

        //항목
        private void dgdList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RPItemView = dgdList.SelectedItem as frm_Acc_RP_Item_Code_U_CodeView;

            //데이터가 널이 아닐 때만 작동
            if (RPItemView != null)
            {
                RPItemView.RPItemName = RPItemView.ListName;

                //사용여부 체크박스  
                if (RPItemView.UseYN.Equals("") || RPItemView.UseYN == null)
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
                else if (RPItemView.UseYN.Equals("Y"))
                {
                    chkYes.IsChecked = true;
                    chkNo.IsChecked = false;
                }
                else
                {
                    chkYes.IsChecked = false;
                    chkNo.IsChecked = false;
                }
                //매출품여부
                if (RPItemView.ProductWongaYN.Equals("") || RPItemView.UseYN == null)
                {
                    chkProduct.IsChecked = false;
                    chkProduct.IsChecked = false;
                }
                else if (RPItemView.ProductWongaYN.Equals("Y"))
                {
                    chkProduct.IsChecked = true;
                  
                }
                else
                {
                    chkProduct.IsChecked = false;
                   
                }
                //제조원가
                if (RPItemView.BuySaleYN.Equals("") || RPItemView.UseYN == null)
                {
                    chkBuyOrSales.IsChecked = false;
                    chkBuyOrSales.IsChecked = false;
                }
                else if (RPItemView.BuySaleYN.Equals("Y"))
                {
                    chkBuyOrSales.IsChecked = true;
                }
                else
                {
                    chkBuyOrSales.IsChecked = false;
                }
            }

            this.DataContext = RPItemView;
        }

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

        // 항목탭 : 대분류 값 선택시 항목 값 자동 설정
        private void cboMiddle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMiddle.SelectedValue != null && !cboMiddle.SelectedValue.ToString().Equals("")
                    && strFlag.Equals("U") == false)
            {
                rowNum = 0;
                re_Search(rowNum);
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
            chkCashYes.IsEnabled = false; //Main항목여부 

            dgdLarge.IsEnabled = true; //대분류그리드
            dgdMiddle.IsEnabled = true; //중분류그리드
            dgdList.IsEnabled = true; //항목그리드

            btndeposit.IsHitTestVisible = true;
            btnwithdraw.IsHitTestVisible = true;

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
            chkNo.IsEnabled = false; //사용여부 No
            txtComments.IsEnabled = true; //비고
            chkCashYes.IsEnabled = true; //Main항목여부 
            chkProduct.IsEnabled = true;
            chkBuyOrSales.IsEnabled = true;

            dgdLarge.IsEnabled = false; //대분류그리드
            dgdMiddle.IsEnabled = false; //중분류그리드
            dgdList.IsEnabled = false; //항목그리드

            btndeposit.IsHitTestVisible = false; //입금 버튼
            btnwithdraw.IsHitTestVisible = false; //출금 버튼

            tabLarge.IsEnabled = false; //대분류탭
            tabMiddle.IsEnabled = false; //중분류탭
            tabList.IsEnabled = false; //항목탭

            //중분류, 항목의 분류를 변경할 때에는 삭제 후 추가를 하도록 안내. 안그럼 RPItemCode가 엉켜 힘들어..
            cboLarge.IsEnabled = false; //대분류 콤보박스
            cboLarge2.IsEnabled = false; //대분류2 콤보박스
            cboMiddle.IsEnabled = false; //중분류 콤보박스
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
        private void FillGrid()
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
                sqlParameter.Add("RPItemName", "");
                sqlParameter.Add("ProductWongaYN", "");
                sqlParameter.Add("bsGbnID", btndeposit.IsChecked == true ? "2" : "1");
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


                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_RP_sItemCode_WPF", sqlParameter, false);

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
                            var RPItemView = new frm_Acc_RP_Item_Code_U_CodeView()
                            {
                                Num = i,
                                RPItemCode = dr["RPItemCode"].ToString(), //코드번호
                                RPItemLCode = dr["RPItemLCode"].ToString(), //대분류
                                RPItemMCode = dr["RPItemMCode"].ToString(), //중분류
                                RPItemSCode = dr["RPItemSCode"].ToString(), //항목
                                RPItemName = dr["RPItemName"].ToString(), //한글명
                                RPItemNameEng = dr["RPItemNameEng"].ToString(), //영문명
                                CashAccountYN = dr["CashAccountYN"].ToString(), //현금항목여부
                                ProductWongaYN = dr["ProductWongaYN"].ToString(), //제조원가항목여부
                                BuySaleYN = dr["BuySaleYN"].ToString(), //매입/매출품 여부
                                RPGBN = dr["RPGBN"].ToString(), //입금/출금 구분
                                UseYN = dr["UseYN"].ToString(), //사용여부
                                Comments = dr["Comments"].ToString(), //비고
                                Seq = dr["Seq"].ToString(), //관리순서

                                LargeName = dr["RPItemLName"].ToString(), //대분류명
                                MiddleName = dr["RPItemMName"].ToString(),  //중분류명 
                                ListName = dr["RPItemSName"].ToString(), //항목명

                            };

                            if (ti.Header.ToString().Equals("대분류"))
                            {
                                dgdLarge.Items.Add(RPItemView);
                            }
                            else if (ti.Header.ToString().Equals("중분류"))
                            {
                                dgdMiddle.Items.Add(RPItemView);
                            }
                            else if (ti.Header.ToString().Equals("항목"))
                            {
                                dgdList.Items.Add(RPItemView);
                            }
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

                    sqlParameter.Add("RPItemName", txtKName.Text);
                    sqlParameter.Add("CashAccountYN", chkCashYes.IsChecked == true ? "Y" : "N");
                    sqlParameter.Add("RPGBN", btndeposit.IsChecked == true ? 2 : 1);
                    sqlParameter.Add("Seq", txtOrder.Text);
                    sqlParameter.Add("Comments", txtComments.Text);
                    sqlParameter.Add("RPItemNameEng", txtEName.Text);
                    sqlParameter.Add("ProductWongaYN", chkProduct.IsChecked == true ? "Y" : "N");
                    sqlParameter.Add("BuySaleYN", chkBuyOrSales.IsChecked == true ? "Y" : "N");

                    sqlParameter.Add("RtnMsg", "");

                    //추가일 때
                    if (strFlag.Equals("I"))
                    {
                        if (ti.Header.ToString().Equals("대분류"))
                        {
                            sqlParameter.Add("RPItemLCode", "");
                            sqlParameter.Add("RPItemMCode", "");
                        }
                        if (ti.Header.ToString().Equals("중분류"))
                        {
                            sqlParameter.Add("RPItemLCode", cboLarge.SelectedValue.ToString());
                            sqlParameter.Add("RPItemMCode", "");
                        }
                        if (ti.Header.ToString().Equals("항목"))
                        {
                            sqlParameter.Add("RPItemLCode", cboLarge2.SelectedValue.ToString());
                            sqlParameter.Add("RPItemMCode", cboMiddle.SelectedValue.ToString());
                        }

                       
                        sqlParameter.Add("UseYN", chkYes.IsChecked == true ? "Y" : "N");
                        sqlParameter.Add("RPItemSCode", "");
                        sqlParameter.Add("UserID", MainWindow.CurrentUser);

                        Procedure pro1 = new Procedure();
                        pro1.Name = "xp_Acc_RP_iRPItemCode_WPF";
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
                            sqlParameter.Add("RPItemLCode", RPItemView.RPItemLCode);
                            sqlParameter.Add("RPItemMCode", "");
                        }
                        if (ti.Header.ToString().Equals("중분류"))
                        {
                            sqlParameter.Add("RPItemLCode", cboLarge.SelectedValue != null ? cboLarge.SelectedValue.ToString() : RPItemView.RPItemLCode.Trim().ToString());
                            sqlParameter.Add("RPItemMCode", RPItemView.RPItemMCode);
                        }
                        if (ti.Header.ToString().Equals("항목"))
                        {
                            sqlParameter.Add("RPItemLCode", cboLarge2.SelectedValue != null ? cboLarge2.SelectedValue.ToString() : RPItemView.RPItemLCode.Trim().ToString());
                            sqlParameter.Add("RPItemMCode", cboMiddle.SelectedValue != null ? cboMiddle.SelectedValue.ToString() : RPItemView.RPItemMCode.Trim().ToString());
                        }

                        sqlParameter.Add("UseYN", chkYes.IsChecked == true ? "Y" : "N");
                        sqlParameter.Add("RPItemSCode", RPItemView.RPItemSCode);
                        sqlParameter.Add("sItemCode", RPItemView.RPItemCode);
                        sqlParameter.Add("UserID", MainWindow.CurrentUser);

                        Procedure pro1 = new Procedure();
                        pro1.Name = "xp_Acc_RP_uRPItemCode_WPF";
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

            if (chkCashYes.IsChecked == false && chkCashNo.IsChecked == false)
            {
                MessageBox.Show("현금 항목여부가 선택되지 않았습니다.");
                flag = false;
                return flag;
            }

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

            TabItem ti = tabAll.SelectedItem as TabItem;

            // 추가, 수정일때만 데이터 체크
            if (!strFlag.Equals(string.Empty))
            {
                // 중분류 탭이 활성화 됐을때
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
            }
            return flag;
        }




        #endregion


        #region 콤보박스 설정 메서드

        // 대분류 목록 가져오기 둘리
        private ObservableCollection<CodeView> Direct_SetComboBoxLarge()
        {
            ObservableCollection<CodeView> retunCollection = new ObservableCollection<CodeView>();
            string sql = " select RPItemLCode, RPItemName ";
            sql += " from Acc_RPItem_Code ";
            sql += " where UseYN = 'Y' and ISNULL(RPItemLCode, '') != '' and ISNULL(RPItemMCode, '') = ''and ISNULL(RPItemSCode, '') = '' ";

            if (btndeposit.IsChecked == true && btnwithdraw.IsChecked == false)
            {
                sql += " and RPGBN = 2 ";
            }
            else if (btnwithdraw.IsChecked == true && btndeposit.IsChecked == false)
            {
                sql += " and RPGBN = 1 ";
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
        private ObservableCollection<CodeView> Direct_SetComboBoxMiddle(string RPItemLCode)
        {
            ObservableCollection<CodeView> retunCollection = new ObservableCollection<CodeView>();
            string sql = " select RPItemMCode, RPItemName ";
            sql += " from Acc_RPItem_Code ";
            sql += " where UseYN = 'Y' and ISNULL(RPItemLCode, '') != '' and ISNULL(RPItemMCode, '') != ''and ISNULL(RPItemSCode, '') = ''";

            if (RPItemLCode != null && !RPItemLCode.Equals(""))
            {
                sql += " and RPItemLCode = " + RPItemLCode;
            }

            if (btndeposit.IsChecked == true)
            {
                sql += " and RPGBN = 2 ";
            }
            else if (btnwithdraw.IsChecked == true)
            {
                sql += " and RPGBN = 1 ";
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

        #endregion

        #region 생성자

        class frm_Acc_RP_Item_Code_U_CodeView : BaseView
        {
            public int Num { get; set; }
            public string RPItemCode { get; set; }
            public string RPItemLCode { get; set; }
            public string RPItemMCode { get; set; }
            public string RPItemSCode { get; set; }
            public string RPItemName { get; set; }
            public string RPItemNameEng { get; set; }
            public string ProductWongaYN { get; set; }
            public string BuySaleYN { get; set; }
            public string RPGBN { get; set; }
            public string UseYN { get; set; }
            public string Seq { get; set; }
            public string Comments { get; set; }
            public string CashAccountYN { get; set; }
            public string createDate { get; set; }
            public string createUserID { get; set; }
            public string LastUpdateDate { get; set; }
            public string LastUpdateUserID { get; set; }

            public string LargeName { get; set; }
            public string MiddleName { get; set; }
            public string ListName { get; set; }
        }

        #endregion


        #region key.enter 이벤트

        //한글명 key.enter
        private void TxtKName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
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
            if (e.Key == Key.Enter)
            {
                lib.SendK(Key.Tab, this);
                //cboCash.IsDropDownOpen = true;
            }
        }
        
        //현금항목여부 key.enter
        private void CboCash_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
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
            }
        }



        #endregion


        //사용안함 포함 라벨 클릭시 
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