using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using WPF.MDI;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// Win_Acc_Receive_Pay_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Acc_Receive_Pay_U : UserControl
    {
        private Microsoft.Office.Interop.Excel.Application excelapp;
        private Microsoft.Office.Interop.Excel.Workbook workbook;
        private Microsoft.Office.Interop.Excel.Worksheet worksheet;
        private Microsoft.Office.Interop.Excel.Range workrange;
        private Microsoft.Office.Interop.Excel.Worksheet copysheet;
        private Microsoft.Office.Interop.Excel.Worksheet pastesheet;
        // 엑셀 활용 용도 (프린트)

        WizMes_Alpha_JA.PopUp.NoticeMessage msg = new PopUp.NoticeMessage();
        //(기다림 알림 메시지창)

        private string InsertOrUpdate = string.Empty;       // I / U 변수.

        int Wh_Ar_SelectedLastIndex = 0;        // 그리드 마지막 선택 줄 임시저장 그릇

        public Win_Acc_Receive_Pay_U()
        {
            InitializeComponent();
        }

        //날짜를 담는 변수?
        string SelectDate = string.Empty;


        // 로드 이벤트.
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            btnToday_Click(null, null);
            chkPeriod.IsChecked = true;
            SetComboBox();
            CanBtnControl();

            tbnReceive.IsChecked = true;    // 로드시 입금버튼 기본선택.
            txtgrbBsItem.IsEnabled = false;
            txtgrbTotalAmount.IsEnabled = false;
            
            cboCompany.SelectedValue = 0;
            cboCurrencyUnit.SelectedValue = 0;
            chkCompany.IsChecked = true;
            cboCompany2.SelectedValue = 0;
        }


        #region (상단 조회조건 체크박스 enable 모음)

        // 입금 / 출금 토글버튼
        private void tbnReceive_Checked(object sender, RoutedEventArgs e)
        {
            tbnPay.IsChecked = false;
            tbnReceive.IsChecked = true;
 

            // 입금버튼 클릭. > 명칭변경 및 항목, 그리드 체인지.
            tbnReceive_CheckedChange();
        }
        // 입금 / 출금 토글버튼
        private void tbnReceive_Unchecked(object sender, RoutedEventArgs e)
        {
            tbnReceive.IsChecked = false;
            tbnPay.IsChecked = true;
         
        }
        // 입금 / 출금 토글버튼
        private void tbnPay_Checked(object sender, RoutedEventArgs e)
        {
            tbnReceive.IsChecked = false;
            tbnPay.IsChecked = true;

            // 출금버튼 클릭. > 명칭변경 및 항목, 그리드 체인지.
            tbnPay_CheckedChange();
        }
        // 입금 / 출금 토글버튼
        private void tbnPay_Unchecked(object sender, RoutedEventArgs e)
        {
            tbnPay.IsChecked = false;
            tbnReceive.IsChecked = true;
        }


        // 기간
        private void lblPeriod_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkPeriod.IsChecked == true) { chkPeriod.IsChecked = false; }
            else { chkPeriod.IsChecked = true; }
        }
        // 기간
        private void chkPeriod_Checked(object sender, RoutedEventArgs e)
        {
            dtpSDate.IsEnabled = true;
            dtpEDate.IsEnabled = true;
        }
        // 기간
        private void chkPeriod_Unchecked(object sender, RoutedEventArgs e)
        {
            dtpSDate.IsEnabled = false;
            dtpEDate.IsEnabled = false;
        }


        #region (상단 조회 일자변경 버튼 이벤트)
        //금일
        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = DateTime.Today;
            dtpEDate.SelectedDate = DateTime.Today;
        }
        //금월
        private void btnThisMonth_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[0];
            dtpEDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[1];
        }
        //전월
        private void btnLastMonth_Click(object sender, RoutedEventArgs e)
        {
            //dtpSDate.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[0];
            //dtpEDate.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[1];

            if (dtpSDate.SelectedDate != null)
            {
                DateTime ThatMonth1 = dtpSDate.SelectedDate.Value.AddDays(-(dtpSDate.SelectedDate.Value.Day - 1)); // 선택한 일자 달의 1일!

                DateTime LastMonth1 = ThatMonth1.AddMonths(-1); // 저번달 1일
                DateTime LastMonth31 = ThatMonth1.AddDays(-1); // 저번달 말일

                dtpSDate.SelectedDate = LastMonth1;
                dtpEDate.SelectedDate = LastMonth31;
            }
            else
            {
                DateTime ThisMonth1 = DateTime.Today.AddDays(-(DateTime.Today.Day - 1)); // 이번달 1일

                DateTime LastMonth1 = ThisMonth1.AddMonths(-1); // 저번달 1일
                DateTime LastMonth31 = ThisMonth1.AddDays(-1); // 저번달 말일

                dtpSDate.SelectedDate = LastMonth1;
                dtpEDate.SelectedDate = LastMonth31;
            }


        }
        //전일
        private void btnYesterday_Click(object sender, RoutedEventArgs e)
        {
            //dtpSDate.SelectedDate = DateTime.Today.AddDays(-1);
            //dtpEDate.SelectedDate = DateTime.Today.AddDays(-1);


            if (dtpSDate.SelectedDate != null)
            {
                dtpSDate.SelectedDate = dtpSDate.SelectedDate.Value.AddDays(-1);
                dtpEDate.SelectedDate = dtpSDate.SelectedDate;
            }
            else
            {
                dtpSDate.SelectedDate = DateTime.Today.AddDays(-1);
                dtpEDate.SelectedDate = DateTime.Today.AddDays(-1);
            }
        }

        #endregion

        // 매출사업장
        private void lblCompany_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkCompany.IsChecked == true) { chkCompany.IsChecked = false; }
            else { chkCompany.IsChecked = true; }
        }
        // 매출사업장
        private void chkCompany_Checked(object sender, RoutedEventArgs e)
        {
            cboCompany.IsEnabled = true;
            cboCompany.Focus();
        }
        // 매출사업장
        private void chkCompany_Unchecked(object sender, RoutedEventArgs e)
        {
            cboCompany.IsEnabled = false;
        }
        // 수금처
        private void lblCustom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkCustom.IsChecked == true) { chkCustom.IsChecked = false; }
            else { chkCustom.IsChecked = true; }
        }
        // 수금처
        private void chkCustom_Checked(object sender, RoutedEventArgs e)
        {
            txtCustom.IsEnabled = true;
            btnpfCustom.IsEnabled = true;
            txtCustom.Focus();
        }
        // 수금처
        private void chkCustom_Unchecked(object sender, RoutedEventArgs e)
        {
            txtCustom.IsEnabled = false;
            btnpfCustom.IsEnabled = false;
        }
        // 계정과목
        private void lblBSItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkBSItem.IsChecked == true) { chkBSItem.IsChecked = false; }
            else { chkBSItem.IsChecked = true; }
        }
        // 계정과목
        private void chkBSItem_Checked(object sender, RoutedEventArgs e)
        {
            txtBSItem.IsEnabled = true;
            btnPfBSItem.IsEnabled = true;
            txtBSItem.Focus();
        }
        // 계정과목
        private void chkBSItem_Unchecked(object sender, RoutedEventArgs e)
        {
            txtBSItem.IsEnabled = false;
            btnPfBSItem.IsEnabled = false;
        }

        // 영업사원
        private void lblSalesCharge_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkSalesCharge.IsChecked == true) { chkSalesCharge.IsChecked = false; }
            else { chkSalesCharge.IsChecked = true; }
        }
        // 영업사원
        private void chkSalesCharge_Checked(object sender, RoutedEventArgs e)
        {
            txtSalesCharge.IsEnabled = true;
            btnPfSalesCharge.IsEnabled = true;
            txtSalesCharge.Focus();
        }
        // 영업사원
        private void chkSalesCharge_Unchecked(object sender, RoutedEventArgs e)
        {
            txtSalesCharge.IsEnabled = false;
            btnPfSalesCharge.IsEnabled = false;
        }
        //화폐
        private void lblMoney_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkMoney.IsChecked == true) { chkMoney.IsChecked = false; }
            else { chkMoney.IsChecked = true; }
        }
        // 화폐
        private void chkMoney_Checked(object sender, RoutedEventArgs e)
        {
            cboMoney.IsEnabled = true;
            cboMoney.Focus();
        }
        // 화폐
        private void chkMoney_Unchecked(object sender, RoutedEventArgs e)
        {
            cboMoney.IsEnabled = false;
        }
        //은행
        private void lblBank_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkBank.IsChecked == true) { chkBank.IsChecked = false; }
            else { chkBank.IsChecked = true; }
        }
        // 은행
        private void chkBank_Checked(object sender, RoutedEventArgs e)
        {
            cboBank.IsEnabled = true;
            cboBank.Focus();
        }
        // 은행
        private void chkBank_Unchecked(object sender, RoutedEventArgs e)
        {
            cboMoney.IsEnabled = false;
        }
        //당일수금분
        private void lblNowDate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkNowDate.IsChecked == true) { chkNowDate.IsChecked = false; }
            else { chkNowDate.IsChecked = true; }
        }
        // 당일수금분
        private void chkNowDate_Checked(object sender, RoutedEventArgs e)
        {
            cboNowDate.IsEnabled = true;
            cboNowDate.Focus();
        }

        // 당일수금분
        private void chkNowDate_Unchecked(object sender, RoutedEventArgs e)
        {
            cboNowDate.IsEnabled = false;
        }
        //입금자명
        private void lblName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkNowDate.IsChecked == true) { chkNowDate.IsChecked = false; }
            else { chkNowDate.IsChecked = true; }
        }
        // 입금자명
        private void chkName_Checked(object sender, RoutedEventArgs e)
        {
            txtName.IsEnabled = true;
            txtName.Focus();
        }
        // 입금자명
        private void chkName_Unchecked(object sender, RoutedEventArgs e)
        {
            txtName.IsEnabled = false;
        }
        #endregion

        #region (콤보박스 세팅) SetComboBox
        private void SetComboBox()
        {
            //매출거래처
            List<string[]> listCompany = new List<string[]>();
            string[] Company01 = new string[] { "0001", "(주)알파신소재" };
            listCompany.Add(Company01);

            ObservableCollection<CodeView> ovcCompany = ComboBoxUtil.Instance.Direct_SetComboBox(listCompany);
            this.cboCompany.ItemsSource = ovcCompany;
            this.cboCompany.DisplayMemberPath = "code_name";
            this.cboCompany.SelectedValuePath = "code_id";

            ObservableCollection<CodeView> ovcCompany2 = ComboBoxUtil.Instance.Direct_SetComboBox(listCompany);
            this.cboCompany2.ItemsSource = ovcCompany2;
            this.cboCompany2.DisplayMemberPath = "code_name";
            this.cboCompany2.SelectedValuePath = "code_id";

            //매입,매출 화폐단위(입력)
            List<string[]> listPrice = new List<string[]>();
            string[] Price01 = new string[] { "0", "₩" };
            string[] Price02 = new string[] { "1", "$" };
            string[] Price03 = new string[] { "2", "EUR" };
            string[] Price04 = new string[] { "3", "ALL" };
            listPrice.Add(Price01);
            listPrice.Add(Price02);
            listPrice.Add(Price03);
            listPrice.Add(Price04);

            ObservableCollection<CodeView> ovcPrice = ComboBoxUtil.Instance.Direct_SetComboBox(listPrice);
            this.cboMoney.ItemsSource = ovcPrice;
            this.cboMoney.DisplayMemberPath = "code_name";
            this.cboMoney.SelectedValuePath = "code_id";

            ObservableCollection<CodeView> ovcPrice2 = ComboBoxUtil.Instance.Direct_SetComboBox(listPrice);
            this.cboCurrencyUnit.ItemsSource = ovcPrice;
            this.cboCurrencyUnit.DisplayMemberPath = "code_name";
            this.cboCurrencyUnit.SelectedValuePath = "code_id";


            // 은행
            ObservableCollection<CodeView> ovcBank = ComboBoxUtil.Instance.GetBankList();
            this.cboBank.ItemsSource = ovcBank;
            this.cboBank.DisplayMemberPath = "code_name";
            this.cboBank.SelectedValuePath = "code_id";

            ObservableCollection<CodeView> ovcBank2 = ComboBoxUtil.Instance.GetBankList();
            this.cbogrbBank.ItemsSource = ovcBank;
            this.cbogrbBank.DisplayMemberPath = "code_name";
            this.cbogrbBank.SelectedValuePath = "code_id";

            //당일수금분
            List<string[]> listToday = new List<string[]>();
            string[] Today01 = new string[] { "0", "N" };
            string[] Today02 = new string[] { "1", "Y" };
            string[] Today03 = new string[] { "2", "ALL" };
            listPrice.Add(Today01);
            listPrice.Add(Today02);
            listPrice.Add(Today03);

            ObservableCollection<CodeView> ovcToday = ComboBoxUtil.Instance.Direct_SetComboBox(listToday);
            this.cboNowDate.ItemsSource = ovcPrice;
            this.cboNowDate.DisplayMemberPath = "code_name";
            this.cboNowDate.SelectedValuePath = "code_id";

            //대체계정여부
            List<string[]> listRefYN = new List<string[]>();
            string[] RefYN01 = new string[] { "0", "N" };
            string[] RefYN02 = new string[] { "1", "Y" };
            listRefYN.Add(RefYN01);
            listRefYN.Add(RefYN02);


            ObservableCollection<CodeView> ovcRefYN = ComboBoxUtil.Instance.Direct_SetComboBox(listRefYN);
            this.cboRefYN.ItemsSource = ovcRefYN;
            this.cboRefYN.DisplayMemberPath = "code_name";
            this.cboRefYN.SelectedValuePath = "code_id";

        }

        #endregion

        #region (쓸 수 있어, 쓸 수 없어) CanBtnControl / CantBtnControl
        // (추가,수정,삭제버튼 류) 쓸 수 있어.
        private void CanBtnControl()
        {
            btnAdd.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSearch.IsEnabled = true;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;
            btnExcel.Visibility = Visibility.Visible;
            grbAddItems.IsHitTestVisible = false;
            dgdReceiveGrid.IsHitTestVisible = true;

            InsertOrUpdate = string.Empty;
            //lblMsg.Visibility = Visibility.Hidden;


            //취소 버튼 눌렀을 때는 입금, 출금 버튼이 눌리게
            tbnReceive.IsHitTestVisible = true;
            tbnPay.IsHitTestVisible = true;
        }


        // (추가,수정,삭제버튼 류) 쓸 수 없어.
        private void CantBtnControl()
        {
            btnAdd.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSearch.IsEnabled = false;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            btnExcel.Visibility = Visibility.Hidden;
            grbAddItems.IsHitTestVisible = true;
            //lblMsg.Visibility = Visibility.Visible;
            dgdReceiveGrid.IsHitTestVisible = false;

            //입출금번호 - 쓸수 없음.
            txtgrbBsItem.IsEnabled = false;
            //합계금액 - 쓸수 없음.(자동산출)
            txtgrbTotalAmount.IsEnabled = false;

            //추가, 수정 버튼 눌렀을 때는 입금, 출금 버튼이 눌리지 않게
            tbnReceive.IsHitTestVisible = false;
            tbnPay.IsHitTestVisible = false;
        }

        #endregion

        #region (플러스파인더 호출묶음) PlusFinder

        // 플러스파인더 거래처
        private void txtCustom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
            }
        }
        // 플러스파인더 >> 거래처.
        private void btnPfCustom_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
        }
        // 플러스파인더 그룹박스 거래처
        private void txtgrbCustom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtgrbCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
            }
        }
        // 플러스파인더 그룹박스 거래처
        private void btngrbpfCustom_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtgrbCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
        }



        // 플러스파인더 계정과목
        private void txtBSItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbnReceive.IsChecked == true)
                {

                    MainWindow.pf.ReturnCode(txtBSItem, 80, "In");
                }
                else
                {
                    MainWindow.pf.ReturnCode(txtBSItem, 80, "Out");
                }
            }
        }
        // 플러스파인더 계정과목
        private void btnPfBSItem_Click(object sender, RoutedEventArgs e)
        {
            if (tbnReceive.IsChecked == true)
            {

                MainWindow.pf.ReturnCode(txtBSItem, 80, "In");
            }
            else
            {
                MainWindow.pf.ReturnCode(txtBSItem, 80, "Out");
            }
        }


        // 플러스파인더 영업사원
        private void txtSalesCharge_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtSalesCharge, (int)Defind_CodeFind.DCF_SalesCharge, "");
            }
        }
        // 플러스파인더 영업사원
        private void btnPfSalesCharge_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtSalesCharge, (int)Defind_CodeFind.DCF_SalesCharge, "");
        }
        // 플러스파인더 그룹박스 영업사원
        private void txtgrbSalesCharge_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtSalesCharge, (int)Defind_CodeFind.DCF_SalesCharge, "");
            }
        }
        // 플러스파인더 그룹박스 영업사원
        private void btngrbSalesCharge_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtSalesCharge, (int)Defind_CodeFind.DCF_SalesCharge, "");
        }

        // 플러스파인더 오더번호
        private void txtOrderNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 4번.
                MainWindow.pf.ReturnCode(txtOrderNo, (int)Defind_CodeFind.DCF_ORDER, "");
            }
        }
        // 플러스파인더 오더번호
        private void btnPfOrderNo_Click(object sender, RoutedEventArgs e)
        {
            // 4번.
            MainWindow.pf.ReturnCode(txtOrderNo, (int)Defind_CodeFind.DCF_ORDER, "");
        }

        // 플러스파인더 그룹박스 계정과목
        private void txtgrbReceivePayItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbnReceive.IsChecked == true)
                {
                    // 33번. 입금 계정만 보이게
                    MainWindow.pf.ReturnCode(txtgrbReceivePayItems, 33, "S");
                    txtgrbCustom.Focus();
                }
                else if (tbnPay.IsChecked == true)
                {
                    // 32번. 출금 계정만 보이게
                    MainWindow.pf.ReturnCode(txtgrbReceivePayItems, 32, "S");
                    txtgrbCustom.Focus();
                }

            }
        }

        // 플러스파인더 그룹박스 계정과목
        private void btngrbpftxtgrbReceivePayItems_Click(object sender, RoutedEventArgs e)
        {
            if (tbnReceive.IsChecked == true)
            {
                // 33번. 입금 계정만 보이게
                MainWindow.pf.ReturnCode(txtgrbReceivePayItems, 33, "S");
                txtgrbCustom.Focus();
            }
            else if (tbnPay.IsChecked == true)
            {
                // 32번. 출금 계정만 보이게
                MainWindow.pf.ReturnCode(txtgrbReceivePayItems, 32, "S");
                txtgrbCustom.Focus();
            }
            //// 32번.
            //MainWindow.pf.ReturnCode(txtgrbReceivePayItems, (int)Defind_CodeFind.DCF_ACCRCVITEM, "S");
            //txtgrbCustom.Focus();
        }

        //플러스파인더 그룹박스 대체계정
        private void txtgrbRefBSItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbnReceive.IsChecked == true)
                {
                    // 33번. 입금 계정만 보이게
                    MainWindow.pf.ReturnCode(txtgrbReceivePayItems, 33, "S");
                    txtgrbCustom.Focus();
                }
                else if (tbnPay.IsChecked == true)
                {
                    // 32번. 출금 계정만 보이게
                    MainWindow.pf.ReturnCode(txtgrbReceivePayItems, 32, "S");
                    txtgrbCustom.Focus();
                }

            }
        }
        //플러스파인더 그룹박스 대체계정 
        private void btngrbpfRefBSItem_Click(object sender, RoutedEventArgs e)
        {
            if (tbnReceive.IsChecked == true)
            {
                // 33번. 입금 계정만 보이게
                MainWindow.pf.ReturnCode(txtgrbReceivePayItems, 33, "S");
                txtgrbCustom.Focus();
            }
            else if (tbnPay.IsChecked == true)
            {
                // 32번. 출금 계정만 보이게
                MainWindow.pf.ReturnCode(txtgrbReceivePayItems, 32, "S");
                txtgrbCustom.Focus();
            }
        }
        #endregion

        #region (토글버튼 체크 체인지 이벤트) CheckedChange
        // 입금 클릭.
        private void tbnReceive_CheckedChange()
        {
            this.DataContext = null;

            //txtgrbReceivePayItems.Text = "계정과목";
            lbrgrbKCustom.Content = "거래처";
            lblgrbBSITEM.Content = "입출금번호";


            SetComboBox();

        }

        // 출금 클릭.
        private void tbnPay_CheckedChange()
        {
            this.DataContext = null;

            //txtgrbReceivePayItems.Text = "계정과목";
            lbrgrbKCustom.Content = "거래처";
            lblgrbBSITEM.Content = "입출금번호";

            SetComboBox();

        }

        #endregion



        // 검색버튼 클릭.
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Wh_Ar_SelectedLastIndex = 0;
            re_Search(Wh_Ar_SelectedLastIndex);

            //}
        }

        /// <summary>
        /// 재검색(수정,삭제,추가 저장후에 자동 재검색)
        /// </summary>
        /// <param name="selectedIndex"></param>
        private void re_Search(int selectedIndex)
        {

            FillGrid_ReceiveGrid();

            if (dgdReceiveGrid.Items.Count > 0)
            {
                dgdReceiveGrid.SelectedIndex = selectedIndex;
            }

        }


        #region (입금용 그리드 채우기) FillGrid_ReceiveGrid
        // 입금용 그리드 채우기.
        private void FillGrid_ReceiveGrid()
        {
            if (dgdReceiveGrid.Items.Count > 0)
            {
                dgdReceiveGrid.Items.Clear();
            }

            //합계변수
            var SumIn = new Win_Acc_Receive_Pay_U_CodeView_Sum();


            try
            {
                // 매입 / 매출 토글박스 구분.
               
                string RPGBN = string.Empty;

                if (tbnReceive.IsChecked == true) { RPGBN = "2"; }
                else if (tbnPay.IsChecked == true) { RPGBN = "1"; }

                // 기간 체크여부 yn.
                int sBSDate = 0;
                if (chkPeriod.IsChecked == true) { sBSDate = 1; }



                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("RPGBN", RPGBN);
                sqlParameter.Add("sBSDate", sBSDate);
                sqlParameter.Add("sDate", sBSDate == 1 ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("eDate", sBSDate == 1 ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("CustomID", chkCustom.IsChecked == true && txtCustom.Tag != null ? txtCustom.Tag.ToString() : "");
                sqlParameter.Add("BSItemCode", chkBSItem.IsChecked == true && txtBSItem.Tag != null ? txtBSItem.Tag.ToString() : "");
                //sqlParameter.Add("ArticleID", "");
                //sqlParameter.Add("Article", "");
                //sqlParameter.Add("OrderNo", "");

                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_RP_sReceivePay_WPF", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("조회된 데이터가 없습니다.");

                        SearchCount.Text = "검색건수 :";

                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;
                        foreach (DataRow dr in drc)
                        {
                            var WinAccBuySale = new Win_Acc_Receive_Pay_U_CodeView()
                            {
                                Num = i + 1,
                               

                                RPNo = dr["RPNo"].ToString(),
                                CurrencyUnit = dr["CurrencyUnit"].ToString(),
                                KCustom = dr["KCustom"].ToString(),
                                BSItem = dr["BSItem"].ToString(),
                                RPDate = dr["RPDate"].ToString(),
                                CompanyID = dr["CompanyID"].ToString(),

                                SalesCharge = dr["SalesCharge"].ToString(),
                                CashAmount = dr["CashAmount"].ToString(),
                                BillAmount = dr["BillAmount"].ToString(),
                                BankAmount = dr["BankAmount"].ToString(),
                                SumAmount = dr["SumAmount"].ToString(),
                                DCAmount = dr["DCAmount"].ToString(),

                                Comments = dr["Comments"].ToString(),
                                BankName = dr["BankName"].ToString(),
                                KCustomName = dr["KCustomName"].ToString(),
                                BillNo = dr["BillNo"].ToString(),
                                RPItemCode = dr["RPItemCode"].ToString(),
                                ReceivePersonName = dr["ReceivePersonName"].ToString(),

                                RefBSNO = dr["RefBSNO"].ToString(),
                                RefRPItemCode = dr["RefRPItemCode"].ToString(),
                                RefComments = dr["RefComments"].ToString(),
                                RefAccountYN = dr["RefAccountYN"].ToString(),
                                RefAmount = dr["RefAmount"].ToString(),
                                RPGBN = dr["RPGBN"].ToString(),
                                BankID = dr["BankID"].ToString(),
                                
                            };

                            SumIn.SumCash += ConvertDouble(WinAccBuySale.CashAmount);
                            SumIn.SumBill += ConvertDouble(WinAccBuySale.BillAmount);
                            SumIn.SumBank += ConvertDouble(WinAccBuySale.BankAmount);
                            SumIn.SumDC += ConvertDouble(WinAccBuySale.DCAmount);
                            SumIn.SumAmount = ConvertDouble(WinAccBuySale.SumAmount);


                            ////거래종류
                            //if (WinAccBuySale.ReceiveNowDateYN.Trim().Equals("Y") && WinAccBuySale.CurrencyUnit.ToString().Equals("0"))
                            //{
                            //    WinAccBuySale.ReceiveNowDateYN = "현금입금";
                            //    WinAccBuySale.cboReceiveNowDateYN = "4";
                            //}
                            ////else if(WinAccBuySale.ReceiveNowDateYN.Trim().Equals("Y") && WinAccBuySale.CurrencyUnit.ToString().Equals("1"))
                            ////{
                            ////    WinAccBuySale.ReceiveNowDateYN = "외화입금";
                            ////    WinAccBuySale.cboReceiveNowDateYN = "6";
                            ////}
                            //else
                            //{
                            //    WinAccBuySale.ReceiveNowDateYN = "어음입금";
                            //    WinAccBuySale.cboReceiveNowDateYN = "5";
                            //}

                            // 화폐단위
                            if (WinAccBuySale.CurrencyUnit.Trim().Equals("0"))
                            {
                                WinAccBuySale.CurrencyUnit = "₩";
                            }
                            else
                            {
                                WinAccBuySale.CurrencyUnit = "$";
                            }

                            // 콤마입히기 > 현금
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.CashAmount))
                            {
                                WinAccBuySale.CashAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.CashAmount);
                            }
                            // 콤마입히기 > 어음
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.BillAmount))
                            {
                                WinAccBuySale.BillAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.BillAmount);
                            }
                            // 콤마입히기 > 은행
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.BankAmount))
                            {
                                WinAccBuySale.BankAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.BankAmount);
                            }
                            
                           
                            // 콤마입히기 > 감액
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.DCAmount))
                            {
                                WinAccBuySale.DCAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.DCAmount);
                            }
                          

                            dgdReceiveGrid.Items.Add(WinAccBuySale);
                            i++;
                        }
                        
                        SearchCount.Text = " 검색건수 : " + i.ToString() + "건"; ;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion


        // 추가버튼 클릭.
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            CantBtnControl();

            //2020.09.28, 장가빈, 추가버튼 클릭시 날짜가 금일로 초기화 되지 않도록 요청,
            dtpDate2.SelectedDate = DateTime.Today;

            //if (SelectDate != "")
            //{
            //    dtpDate2.SelectedDate = DateTime.Parse(SelectDate.Substring(0, 4) + "-" + SelectDate.Substring(4, 2) + "-" + SelectDate.Substring(6, 2));
            //}


            cboCurrencyUnit.SelectedIndex = 0;
            cboCompany2.SelectedIndex = 0;


            InsertOrUpdate = "I";

                if (dgdReceiveGrid.Items.Count > 0)
                {
                    Wh_Ar_SelectedLastIndex = dgdReceiveGrid.SelectedIndex;
                }
                else
                {
                    Wh_Ar_SelectedLastIndex = 0;
                }
            
            
        }

        // 수정버튼 클릭.
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
             if (dgdReceiveGrid.Items.Count < 1)
                {
                    MessageBox.Show("먼저 검색해 주세요.");
                    return;
                }
                var OBJ = dgdReceiveGrid.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;
                if (OBJ == null)
                {
                    MessageBox.Show("수정할 항목이 정확히 선택되지 않았습니다.");
                    return;
                }
            
            CantBtnControl();
            InsertOrUpdate = "U";

              Wh_Ar_SelectedLastIndex = dgdReceiveGrid.SelectedIndex;

        }

        #region (텍스트 박스 숫자만 들어가게끔) PreviewTextInput        
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 현금 )
        private void txtgrbCash_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 은행 )
        private void txtgrbBankPay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 카드 )
        private void txtgrbCard_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 어음 )
        private void txtgrbBankPaper_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 감액 )
        private void txtgrbDiscount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 토탈합계 )
        private void txtgrbTotalAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }


        #endregion

        #region 합계금액 자동도출 (textchacnged)
        // (현금) - 합계금액 자동도출.
        private void txtgrbCash_TextChanged(object sender, TextChangedEventArgs e)
        {
            double a = 0;       //(현금)
            double b = 0;       //(은행)
            double c = 0;       //(카드)
            double d = 0;       //(어음)
            double ee = 0;       //(감액)

            double f = 0;       //(합계)

            double.TryParse(txtgrbCash.Text, out a);
            double.TryParse(txtgrbBankPay.Text, out b);
            double.TryParse(txtgrbCard.Text, out c);
            double.TryParse(txtgrbBankPaper.Text, out d);
            double.TryParse(txtgrbDiscount.Text, out ee);

            f = (a + b + c + d) - ee;
            txtgrbTotalAmount.Text = string.Format("{0:N0}", f); // 2020.01.16 천단위 자리로 나오도록 수정
        }
        // (은행) - 합계금액 자동도출.
        private void txtgrbBankPay_TextChanged(object sender, TextChangedEventArgs e)
        {
            double a = 0;       //(현금)
            double b = 0;       //(은행)
            double c = 0;       //(카드)
            double d = 0;       //(어음)
            double ee = 0;       //(감액)

            double f = 0;       //(합계)

            double.TryParse(txtgrbCash.Text, out a);
            double.TryParse(txtgrbBankPay.Text, out b);
            double.TryParse(txtgrbCard.Text, out c);
            double.TryParse(txtgrbBankPaper.Text, out d);
            double.TryParse(txtgrbDiscount.Text, out ee);

            f = (a + b + c + d) - ee;
            txtgrbTotalAmount.Text = string.Format("{0:N0}", f);
        }
        // (카드) - 합계금액 자동도출.
        private void txtgrbCard_TextChanged(object sender, TextChangedEventArgs e)
        {
            double a = 0;       //(현금)
            double b = 0;       //(은행)
            double c = 0;       //(카드)
            double d = 0;       //(어음)
            double ee = 0;       //(감액)

            double f = 0;       //(합계)

            double.TryParse(txtgrbCash.Text, out a);
            double.TryParse(txtgrbBankPay.Text, out b);
            double.TryParse(txtgrbCard.Text, out c);
            double.TryParse(txtgrbBankPaper.Text, out d);
            double.TryParse(txtgrbDiscount.Text, out ee);

            f = (a + b + c + d) - ee;
            txtgrbTotalAmount.Text = string.Format("{0:N0}", f);
        }
        // (어음) - 합계금액 자동도출.
        private void txtgrbBankPaper_TextChanged(object sender, TextChangedEventArgs e)
        {
            double a = 0;       //(현금)
            double b = 0;       //(은행)
            double c = 0;       //(카드)
            double d = 0;       //(어음)
            double ee = 0;       //(감액)

            double f = 0;       //(합계)

            double.TryParse(txtgrbCash.Text, out a);
            double.TryParse(txtgrbBankPay.Text, out b);
            double.TryParse(txtgrbCard.Text, out c);
            double.TryParse(txtgrbBankPaper.Text, out d);
            double.TryParse(txtgrbDiscount.Text, out ee);

            f = (a + b + c + d) - ee;
            txtgrbTotalAmount.Text = string.Format("{0:N0}", f);
        }
        // (감액) - 합계금액 자동도출.
        private void txtgrbDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            double a = 0;       //(현금)
            double b = 0;       //(은행)
            double c = 0;       //(카드)
            double d = 0;       //(어음)
            double ee = 0;       //(감액)

            double f = 0;       //(합계)

            double.TryParse(txtgrbCash.Text, out a);
            double.TryParse(txtgrbBankPay.Text, out b);
            double.TryParse(txtgrbCard.Text, out c);
            double.TryParse(txtgrbBankPaper.Text, out d);
            double.TryParse(txtgrbDiscount.Text, out ee);

            f = (a + b + c + d) - ee;
            txtgrbTotalAmount.Text = string.Format("{0:N0}", f);
        }

        #endregion



        // 삭제버튼 클릭.
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int D_Check = 0;
           
                foreach (Win_Acc_Receive_Pay_U_CodeView Win_Acc_Buy_Receive in dgdReceiveGrid.Items)
                {
                    if (Win_Acc_Buy_Receive != null)
                    {
                        
                            D_Check++;
                        
                    }
                }
            
           


            if (D_Check == 0)
            {
                MessageBox.Show("삭제할 항목이 없습니다. \r\n " +
                    "삭제할 항목을 체크표시 한 후 삭제버튼을 눌러주세요.");
            }
            else
            {
                if (MessageBox.Show("선택하신 항목을 삭제하시겠습니까?", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                   
                        if (dgdReceiveGrid.Items.Count > 0 && dgdReceiveGrid.SelectedItem != null)
                        {
                            Wh_Ar_SelectedLastIndex = dgdReceiveGrid.SelectedIndex;
                        }
                        foreach (Win_Acc_Receive_Pay_U_CodeView Win_Acc_Buy_Receive in dgdReceiveGrid.Items)
                        {
                            if (Win_Acc_Buy_Receive != null)
                            {
                                  Delete_Data(Win_Acc_Buy_Receive.RPNo, Win_Acc_Buy_Receive.RPGBN);
                                
                            }
                        }
                        dgdReceiveGrid.Refresh();
                        Wh_Ar_SelectedLastIndex -= 1;
                        re_Search(Wh_Ar_SelectedLastIndex);
                        //FillGrid_ReceiveGrid();     // 재검색.
                    
                    
                }
            }
        }

        #region (삭제로직) Delete_Data
        // 삭제로직.
        private bool Delete_Data(string RPNo, string RPGBN)
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {

                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("RPNo", RPNo);
                sqlParameter.Add("rpGbn", RPGBN);

                Procedure pro1 = new Procedure();
                pro1.Name = "xp_Acc_RP_DReceivePay";
                pro1.OutputUseYN = "N";
                pro1.OutputName = "RPNo";
                pro1.OutputLength = "15";

                Prolist.Add(pro1);
                ListParameter.Add(sqlParameter);

                string[] Confirm = new string[2];
                Confirm = DataStore.Instance.ExecuteAllProcedureOutputNew(Prolist, ListParameter);
                if (Confirm[0] != "success")
                {
                    MessageBox.Show("[삭제실패]\r\n" + Confirm[1].ToString());
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
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return flag;
        }

        #endregion



        // 저장버튼 클릭.
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Check_EssentialData())
            {
                if (SaveData(InsertOrUpdate))
                {
                    CanBtnControl();

                    //2020.09.28, 장가빈, 입력했던 날짜가 다음 번 입력에도 남아있길 원하셔서.
                    SelectDate = dtpDate2.SelectedDate.Value.ToString("yyyyMMdd");

                    RPItemName = string.Empty;      //저장 후에는 비워줘야지 2020.02.22, 장가빈

                   
                        if (InsertOrUpdate == "I")     //1. 추가 > 저장했다면,
                        {
                            if (dgdReceiveGrid.Items.Count > 0)
                            {
                                re_Search(dgdReceiveGrid.Items.Count - 1);
                                dgdReceiveGrid.Focus();
                            }
                            else
                            { re_Search(0); }
                        }
                        else        //2. 수정 > 저장했다면,
                        {
                            re_Search(Wh_Ar_SelectedLastIndex);
                            dgdReceiveGrid.Focus();
                        }

                }
            }
        }


        #region(저장 전, 필수입력 칸 입력여부 체크) Check_EssentialData
        private bool Check_EssentialData()
        {
            bool Flag = true;

            if (dtpDate2.SelectedDate == null)
            {
                MessageBox.Show("등록일자가 입력되지 않았습니다. 먼저 일자를 입력해주세요");
                Flag = false;
                return Flag;
            }


            if (txtgrbCustom.Tag == null || txtgrbCustom.Text.Length <= 0)
            {
                MessageBox.Show("거래처가 입력되지 않았습니다. 먼저 거래처를 입력해주세요");
                Flag = false;
                return Flag;
            }
            if (cboCurrencyUnit.SelectedValue == null)
            {
                MessageBox.Show("화폐단위가 입력되지 않았습니다. 먼저 화폐단위를 입력해주세요");
                Flag = false;
                return Flag;
            }
            //if (cboTransacClass.SelectedValue == null)
            //{
            //    MessageBox.Show("거래종류가 입력되지 않았습니다. 먼저 거래종류를 입력해주세요");
            //    Flag = false;
            //    return Flag;
            //}
            //if (txtgrbDiscount.Text == string.Empty || txtgrbDiscount.Text.Length <= 0)
            //{
            //    MessageBox.Show("감액이 입력되지 않았습니다. 먼저 감액을 입력해주세요");
            //    Flag = false;
            //    return Flag;
            //}
            if (txtgrbTotalAmount.Text == string.Empty || txtgrbTotalAmount.Text.Length <= 0)
            {
                MessageBox.Show("합계금액이 입력되지 않았습니다. 먼저 합계금액을 입력해주세요");
                Flag = false;
                return Flag;
            }

            if (txtgrbReceivePayItems.Tag == null)
            {
                MessageBox.Show("계정과목이 선택되지 않았습니다. 플러스 파인더를 통해 계정과목을 선택해 주세요");
                Flag = false;
                return Flag;
            }

            return Flag;
        }


        #endregion

        #region (저장로직) SaveData
        private bool SaveData(string InsertOrUpdate)
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {

                string RPNo = string.Empty;                                 // 관리번호(P-KEY)
                string RPGBN = string.Empty;
                if (tbnReceive.IsChecked == true) { RPGBN = "2"; }          //매입
                else if (tbnPay.IsChecked == true) { RPGBN = "1"; }     //출금

                string BSITEM = txtgrbBsItem.Text;
                string BSItemName = txtgrbBsItem.Text;

              
                var WinAccBuySale = dgdReceiveGrid.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;

                if (WinAccBuySale != null) // 수정1
                {
                    RPNo = WinAccBuySale.RPNo;
                }


                if (txtOrderNo.Tag == null || txtOrderNo.Text.Length <= 0)
                {
                    txtOrderNo.Tag = (object)"";
                }
                if (cboBank.Tag == null || cboBank.Text.Length <= 0)
                {
                    cboBank.Tag = (object)"";
                }


                double D_CashAmount = 0;
                double D_BillAmount = 0;
                double D_BankAmount = 0;
                double D_CardAmount = 0;
                double D_DCAmount = 0;
                double D_ReceiveBillAmount = 0;
                double D_RefAmount = 0;


                double.TryParse(txtgrbCash.Text, out D_CashAmount);
                double.TryParse(txtgrbBankPaper.Text, out D_BillAmount);
                double.TryParse(txtgrbBankPay.Text, out D_BankAmount);
                double.TryParse(txtgrbCard.Text, out D_CardAmount);
                double.TryParse(txtgrbDiscount.Text, out D_DCAmount);
                double.TryParse(txtgrbTotalAmount.Text.Replace(",", ""), out D_ReceiveBillAmount);
                double.TryParse(txtgrbRefAmount.Text, out D_RefAmount);

                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();


                sqlParameter.Add("RPGBN", RPGBN);                                                      //입금 = 1.
                sqlParameter.Add("companyid", "0001");                                               // 기본.
                sqlParameter.Add("RPDate", dtpDate2.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("BSItem", txtgrbReceivePayItems.Tag.ToString());       //txtgrbReceivePayItems.Text);  //매입매출번호
                sqlParameter.Add("RPItemCode", txtgrbReceivePayItems.Tag != null ? txtgrbReceivePayItems.Tag.ToString() : "");
             
                sqlParameter.Add("CurrencyUnit", cboCurrencyUnit.SelectedValue.ToString());         // 화폐단위
                sqlParameter.Add("CustomID", txtgrbCustom.Tag.ToString());                          // 거래처.
                sqlParameter.Add("SalesCharge", txtgrbSalesCharge.Text);                                                // (     )
                sqlParameter.Add("BankID", cbogrbBank.SelectedValue != null ? cbogrbBank.SelectedValue.ToString() : "");          // 계좌선택
                sqlParameter.Add("CashAmount", D_CashAmount);                                       // 현금

                sqlParameter.Add("BillAmount", D_BillAmount);                                       // 어음
                sqlParameter.Add("BankAmount", D_BankAmount);                                       // 은행
                sqlParameter.Add("DCAmount", D_DCAmount);                                           // 감액
                sqlParameter.Add("BillNo", txtgrbBankPaperNo.Text);                                 // 어음 번호
                sqlParameter.Add("VATAmount", 0);                                         //부가세

                sqlParameter.Add("ForReceiveBillAmount", txtgrbTotalAmount.Text);                      // 합계금액
                sqlParameter.Add("ReceiveNowDateYN","");  // 거래종류
                sqlParameter.Add("CardAmount", D_CardAmount);                       //카드
                sqlParameter.Add("ReceivePersonName", txtgrbReceiveName.Text);
                sqlParameter.Add("Bank", cbogrbBank.SelectedValue != null ? cbogrbBank.SelectedValue.ToString() : "");  
           
                sqlParameter.Add("Comments", txtgrbSemiComment.Text);                                   // 비고
                sqlParameter.Add("OrderID", txtOrderNo.Text);
                sqlParameter.Add("RefBSNO", "");
                sqlParameter.Add("OrderFlag", 0);
                sqlParameter.Add("RefRPItemCode", "");

                sqlParameter.Add("RefComments", txtgrbRefComments.Text);                            // 적요(sub비고)
                sqlParameter.Add("RefAccountYN", cboRefYN.SelectedValue != null ? cboRefYN.SelectedValue.ToString() : "");
                sqlParameter.Add("RefAmount", D_RefAmount);                                                  //금액
                sqlParameter.Add("Createuserid", MainWindow.CurrentUser);                           // 생성자.

             /*   sqlParameter.Add("KCustomName", KCustomName.Text.ToString());          */                 //거래처명

                Procedure pro1 = new Procedure();
                if (InsertOrUpdate == "I")
                {
                    sqlParameter.Add("RPNo", "");

                    pro1.Name = "xp_Acc_RP_iReceivePay_WPF";
                    pro1.OutputUseYN = "N";
                    pro1.OutputName = "";
                    pro1.OutputLength = "10";
                }
                else
                {
                    sqlParameter.Add("RPNo", RPNo);

                    pro1.Name = "xp_Acc_RP_uReceivePay_WPF";
                    pro1.OutputUseYN = "N";
                    pro1.OutputName = "";
                    pro1.OutputLength = "10";

                }



                Prolist.Add(pro1);
                ListParameter.Add(sqlParameter);

                List<KeyValue> list_Result = new List<KeyValue>();
                list_Result = DataStore.Instance.ExecuteAllProcedureOutputGetCS(Prolist, ListParameter);

                if (list_Result[0].key.ToLower() == "success")
                {
                    flag = true;
                }
                else
                {
                    MessageBox.Show("[저장실패]\r\n" + list_Result[0].value.ToString());
                    flag = false;
                }
                Prolist.Clear();
                ListParameter.Clear();
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







        #endregion

        // 취소버튼 클릭.
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CanBtnControl();

           
                if (InsertOrUpdate == "I")     //1. 추가 도중 취소했다면.
                {
                    if (dgdReceiveGrid.Items.Count > 0)
                    {
                        re_Search(Wh_Ar_SelectedLastIndex);
                        dgdReceiveGrid.Focus();
                    }
                    else
                    { re_Search(0); }
                }
                else        //2. 수정 도중 취소했다면
                {
                    re_Search(Wh_Ar_SelectedLastIndex);
                    dgdReceiveGrid.Focus();
                }

        }

        // 닫기버튼 클릭.
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }


        // 데이터 그리드 항목 클릭_ SelectionChanged
        private void dgdReceiveGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbnReceive.IsChecked == true)
            {
                var WinAccBuySale = dgdReceiveGrid.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;
                if (WinAccBuySale != null)
                {
                    this.DataContext = WinAccBuySale;

                    txtgrbReceivePayItems.Tag = WinAccBuySale.RPItemCode;
                    DateTime aa = DateTime.ParseExact(WinAccBuySale.RPDate, "yyyyMMdd", null);
                    dtpDate2.SelectedDate = aa;
                    //WinAccBuySale.RPDate = dtpDate2.SelectedDate.Value.ToString("yyyyMMdd");


                }
            }
        }
 
      

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] dgdStr = new string[2];
            if (tbnReceive.IsChecked == true)
            {
                dgdStr[0] = "입금 등록 리스트";
                dgdStr[1] = dgdReceiveGrid.Name;
            }
            else
            {
                dgdStr[0] = "출금 등록 리스트";
                dgdStr[1] = dgdReceiveGrid.Name;
            }

            ExportExcelxaml ExpExc = new ExportExcelxaml(dgdStr);
            ExpExc.ShowDialog();

            if (ExpExc.DialogResult.HasValue)
            {
                if (ExpExc.choice.Equals(dgdReceiveGrid.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdReceiveGrid);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdReceiveGrid);

                    Name = dgdReceiveGrid.Name;
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




        string RPItemName = string.Empty;
        //계정과목 임의 수정해서 저장되지 않도록, 플러스파인더 코드 값으로 항목에서 Name을 가져오기
        private string RPItemFindName()
        {
            string sql = "select RPItemName, RPItemCode from Acc_RPItem_Code where RPItemCode =" + txtgrbReceivePayItems.Tag.ToString().Trim();

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
                            RPItemName = item[0].ToString().Trim();

                            //MessageBox.Show(":" + RPItemName.ToString().Trim());
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

            return RPItemName;
        }


        // 거래종류 에서 계정과목으로 포커스 이동.
        private void cboTransacClass_DropDownClosed(object sender, EventArgs e)
        {
            txtgrbReceivePayItems.Focus();
        }
        // 화폐단위에서 은행명으로 포커스 이동.
        private void cboCurrencyUnit_DropDownClosed(object sender, EventArgs e)
        {
           
        }
        // 은행명에서 어음번호로 포커스 이동.
        private void cboAccountChoice_DropDownClosed(object sender, EventArgs e)
        {
            txtgrbBankPaperNo.Focus();
        }
        // 어음번호에서 적요로 포커스 이동.
        private void txtgrbBankPaperNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtgrbSemiComment.Focus();
            }
        }
        // 적요에서 비고로 포커스 이동.
        private void txtgrbSemiComment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtgrbReceiveName.Focus();
            }
        }
        // 비고에서 현금으로 포커스 이동
        private void txtgrbComment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtgrbCash.Focus();
            }
        }
        // 현금에서 은행으로 포커스 이동
        private void txtgrbCash_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtgrbBankPay.Focus();
            }
        }
        // 은행에서 카드로 포커스 이동
        private void txtgrbBankPay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtgrbCard.Focus();
            }
        }
        // 카드에서 어음으로 포커스 이동
        private void txtgrbCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtgrbBankPaper.Focus();
            }
        }
        // 어음에서 감액으로 포커스 이동
        private void txtgrbBankPaper_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtgrbDiscount.Focus();
            }
        }
        // 감액에서 다시 거래종류로 이동 (순환반복)
        private void txtgrbDiscount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtgrbReceivePayItems.Focus();

            }
        }


        #region 기타 매서드

        // 천 단위 콤마, 소수점 버리기
        private string stringFormatN0(object obj)
        {
            return string.Format("{0:N0}", obj);
        }

        // 천 단위 콤마, 소수점 두자리
        private string stringFormatN2(object obj)
        {
            return string.Format("{0:N2}", obj);
        }

        // 데이터피커 포맷으로 변경
        private string DatePickerFormat(string str)
        {
            string result = "";

            if (str.Length == 8)
            {
                if (!str.Trim().Equals(""))
                {
                    result = str.Substring(0, 4) + "-" + str.Substring(4, 2) + "-" + str.Substring(6, 2);
                }
            }

            return result;
        }

        // Int로 변환
        private int ConvertInt(string str)
        {
            int result = 0;
            int chkInt = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Replace(",", "");

                if (Int32.TryParse(str, out chkInt) == true)
                {
                    result = Int32.Parse(str);
                }
            }

            return result;
        }

        // 소수로 변환 가능한지 체크 이벤트
        private bool CheckConvertDouble(string str)
        {
            bool flag = false;
            double chkDouble = 0;

            if (!str.Trim().Equals(""))
            {
                if (Double.TryParse(str, out chkDouble) == true)
                {
                    flag = true;
                }
            }

            return flag;
        }

        // 숫자로 변환 가능한지 체크 이벤트
        private bool CheckConvertInt(string str)
        {
            bool flag = false;
            int chkInt = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Trim().Replace(",", "");

                if (Int32.TryParse(str, out chkInt) == true)
                {
                    flag = true;
                }
            }

            return flag;
        }

        // 소수로 변환
        private double ConvertDouble(string str)
        {
            double result = 0;
            double chkDouble = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Replace(",", "");

                if (Double.TryParse(str, out chkDouble) == true)
                {
                    result = Double.Parse(str);
                }
            }

            return result;
        }

        //숫자 외에 다른 문자열 못들어오도록
        public bool IsNumeric(string source)
        {

            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(source);
        }

        //나눗셈, 분모가 0이면 0값 반환
        private double division(double a, double b)
        {
            if (b == 0)
            {
                return 0;
            }
            else
            {
                return a / b;
            }
        }


        #endregion 기타 매서드

        ////외화지불, 외화입금을 선택시 화페단위는 $로 변화되도록.
        //private void CboTransacClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (tbnPay.IsChecked == true)
        //    {
        //        if (cboTransacClass.SelectedValue != null)
        //        {
        //            if (cboTransacClass.SelectedValue.ToString().Equals("1"))
        //            {
        //                cboCurrencyUnit.SelectedValue = "0";  //현금이면 원화
        //            }
        //            else if (cboTransacClass.SelectedValue.ToString().Equals("3"))
        //            {
        //                cboCurrencyUnit.SelectedValue = "1";  //외화면 미화
        //            }
        //            else
        //            {
        //                cboCurrencyUnit.SelectedValue = "0";  //어음이면 그냥 원화.
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (cboTransacClass.SelectedValue != null)
        //        {
        //            if (cboTransacClass.SelectedValue.ToString().Equals("4"))
        //            {
        //                cboCurrencyUnit.SelectedValue = "0";  //현금이면 원화
        //            }
        //            else if (cboTransacClass.SelectedValue.ToString().Equals("6"))
        //            {
        //                cboCurrencyUnit.SelectedValue = "1";  //외화면 미화
        //            }
        //            else
        //            {
        //                cboCurrencyUnit.SelectedValue = "0";  //어음이면 그냥 원화.
        //            }
        //        }
        //    }

        //}

        //    //입금 체크 이벤트(합계 계산)
        //    private void ChkC_Checked_In(object sender, RoutedEventArgs e)
        //    {
        //        var SumIn = new Win_Acc_Receive_Pay_U_CodeView_Sum();

        //        int j = 0;

        //        for (int i = 0; i < dgdReceiveGrid.Items.Count; i++)
        //        {
        //            var Data = dgdReceiveGrid.Items[i] as Win_Acc_Receive_Pay_U_CodeView;

        //            if (Data.IsCheck == true)
        //            {
        //                SumIn.SumCash += ConvertDouble(Data.CashAmount);
        //                SumIn.SumCard += ConvertDouble(Data.CardAmount);
        //                SumIn.SumBill += ConvertDouble(Data.BillAmount);
        //                SumIn.SumBank += ConvertDouble(Data.BankAmount);
        //                SumIn.SumDC += ConvertDouble(Data.DCAmount);
        //                SumIn.SumAmount += ConvertDouble(Data.ForReceiveBillAmount);

        //                j = j + 1;
        //            }

        //            txtblockSearchCountIn.Text = "      합계 : " + j.ToString() + "건"; ;
        //            txtblockSearchCashIn.Text = "현금 : " + stringFormatN0(SumIn.SumCash) + "원";
        //            txtblockSearchCardIn.Text = "카드 : " + stringFormatN0(SumIn.SumCard) + "원";
        //            txtblockSearchBillIn.Text = "어음 : " + stringFormatN0(SumIn.SumBill) + "원";
        //            txtblockSearchBankIn.Text = "은행 : " + stringFormatN0(SumIn.SumBank) + "원";
        //            txtblockSearchDCIn.Text = "감액 : " + stringFormatN0(SumIn.SumDC) + "원";
        //            txtblockSearchTotalIn.Text = "합계금액 : " + stringFormatN0(SumIn.SumAmount) + "원";

        //        }
        //    }

        //    //입금 체크 해제 이벤트(합계 계산)
        //    private void ChkC_Unchecked_In(object sender, RoutedEventArgs e)
        //    {
        //        var SumIn = new Win_Acc_Receive_Pay_U_CodeView_Sum();

        //        int j = 0;

        //        for (int i = 0; i < dgdReceiveGrid.Items.Count; i++)
        //        {
        //            var Data = dgdReceiveGrid.Items[i] as Win_Acc_Receive_Pay_U_CodeView;

        //            if (Data.IsCheck == true)
        //            {
        //                SumIn.SumCash += ConvertDouble(Data.CashAmount);
        //                SumIn.SumCard += ConvertDouble(Data.CardAmount);
        //                SumIn.SumBill += ConvertDouble(Data.BillAmount);
        //                SumIn.SumBank += ConvertDouble(Data.BankAmount);
        //                SumIn.SumDC += ConvertDouble(Data.DCAmount);
        //                SumIn.SumAmount += ConvertDouble(Data.ForReceiveBillAmount);

        //                j = j + 1;
        //            }

        //            txtblockSearchCountIn.Text = "      합계 : " + j.ToString() + "건"; ;
        //            txtblockSearchCashIn.Text = "현금 : " + stringFormatN0(SumIn.SumCash) + "원";
        //            txtblockSearchCardIn.Text = "카드 : " + stringFormatN0(SumIn.SumCard) + "원";
        //            txtblockSearchBillIn.Text = "어음 : " + stringFormatN0(SumIn.SumBill) + "원";
        //            txtblockSearchBankIn.Text = "은행 : " + stringFormatN0(SumIn.SumBank) + "원";
        //            txtblockSearchDCIn.Text = "감액 : " + stringFormatN0(SumIn.SumDC) + "원";
        //            txtblockSearchTotalIn.Text = "합계금액 : " + stringFormatN0(SumIn.SumAmount) + "원";

        //        }
        //    }

        //    //출금 체크 이벤트(합계 계산)
        //    private void ChkC_Checked_Out(object sender, RoutedEventArgs e)
        //    {
        //        var SumOut = new Win_Acc_Receive_Pay_U_CodeView_Sum();

        //        int j = 0;

        //        for (int i = 0; i < dgdPayGrid.Items.Count; i++)
        //        {
        //            var Data = dgdPayGrid.Items[i] as Win_Acc_Receive_Pay_U_CodeView;

        //            if (Data.IsCheck == true)
        //            {
        //                SumOut.SumCash += ConvertDouble(Data.CashAmount);
        //                SumOut.SumCard += ConvertDouble(Data.CardAmount);
        //                SumOut.SumBill += ConvertDouble(Data.BillAmount);
        //                SumOut.SumBank += ConvertDouble(Data.BankAmount);
        //                SumOut.SumDC += ConvertDouble(Data.DCAmount);
        //                SumOut.SumAmount += ConvertDouble(Data.ForReceiveBillAmount);

        //                j = j + 1;
        //            }

        //            txtblockSearchCountOut.Text = "      합계 : " + j.ToString() + "건"; ;
        //            txtblockSearchCashOut.Text = "현금 : " + stringFormatN0(SumOut.SumCash) + "원";
        //            txtblockSearchCardOut.Text = "카드 : " + stringFormatN0(SumOut.SumCard) + "원";
        //            txtblockSearchBillOut.Text = "어음 : " + stringFormatN0(SumOut.SumBill) + "원";
        //            txtblockSearchBankOut.Text = "은행 : " + stringFormatN0(SumOut.SumBank) + "원";
        //            txtblockSearchDCOut.Text = "감액 : " + stringFormatN0(SumOut.SumDC) + "원";
        //            txtblockSearchTotalOut.Text = "합계금액 : " + stringFormatN0(SumOut.SumAmount) + "원";

        //        }
        //    }

        //    //출금 체크 해제 이벤트(합계 계산)
        //    private void ChkC_Unchecked_Out(object sender, RoutedEventArgs e)
        //    {
        //        var SumOut = new Win_Acc_Receive_Pay_U_CodeView_Sum();

        //        int j = 0;

        //        for (int i = 0; i < dgdPayGrid.Items.Count; i++)
        //        {
        //            var Data = dgdPayGrid.Items[i] as Win_Acc_Receive_Pay_U_CodeView;

        //            if (Data.IsCheck == true)
        //            {
        //                SumOut.SumCash += ConvertDouble(Data.CashAmount);
        //                SumOut.SumCard += ConvertDouble(Data.CardAmount);
        //                SumOut.SumBill += ConvertDouble(Data.BillAmount);
        //                SumOut.SumBank += ConvertDouble(Data.BankAmount);
        //                SumOut.SumDC += ConvertDouble(Data.DCAmount);
        //                SumOut.SumAmount += ConvertDouble(Data.ForReceiveBillAmount);

        //                j = j + 1;
        //            }

        //            txtblockSearchCountOut.Text = "      합계 : " + j.ToString() + "건"; ;
        //            txtblockSearchCashOut.Text = "현금 : " + stringFormatN0(SumOut.SumCash) + "원";
        //            txtblockSearchCardOut.Text = "카드 : " + stringFormatN0(SumOut.SumCard) + "원";
        //            txtblockSearchBillOut.Text = "어음 : " + stringFormatN0(SumOut.SumBill) + "원";
        //            txtblockSearchBankOut.Text = "은행 : " + stringFormatN0(SumOut.SumBank) + "원";
        //            txtblockSearchDCOut.Text = "감액 : " + stringFormatN0(SumOut.SumDC) + "원";
        //            txtblockSearchTotalOut.Text = "합계금액 : " + stringFormatN0(SumOut.SumAmount) + "원";

        //        }
        //    }

        //    //전체선택 텍스트블럭 이벤트
        //    private void TbkSelectAll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //    {
        //        if (chkSelectAll.IsChecked == false)
        //        {
        //            chkSelectAll.IsChecked = true;
        //        }
        //        else
        //        {
        //            chkSelectAll.IsChecked = false;
        //        }
        //    }

        //    //전체선택 체크박스 체크
        //    private void ChkSelectAll_Checked(object sender, RoutedEventArgs e)
        //    {
        //        if (tbnPay.IsChecked == true)
        //        {
        //            if (dgdPayGrid.Items.Count > 0)
        //            {
        //                foreach (Win_Acc_Receive_Pay_U_CodeView RP_Chek in dgdPayGrid.Items)
        //                {
        //                    if (RP_Chek != null)
        //                    {
        //                        RP_Chek.IsCheck = true;
        //                    }
        //                }

        //                dgdPayGrid.Items.Refresh();
        //            }
        //        }
        //        else if (tbnReceive.IsChecked == true)
        //        {
        //            if (dgdReceiveGrid.Items.Count > 0)
        //            {
        //                foreach (Win_Acc_Receive_Pay_U_CodeView RP_Chek in dgdReceiveGrid.Items)
        //                {
        //                    if (RP_Chek != null)
        //                    {
        //                        RP_Chek.IsCheck = true;
        //                    }
        //                }

        //                dgdReceiveGrid.Items.Refresh();
        //            }
        //        }
        //    }

        //    //전체선택 체크박스 체크해제
        //    private void ChkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        //    {
        //            if (dgdReceiveGrid.Items.Count > 0)
        //            {
        //                foreach (Win_Acc_Receive_Pay_U_CodeView RP_Chek in dgdReceiveGrid.Items)
        //                {
        //                    if (RP_Chek != null)
        //                    {
        //                        RP_Chek.IsCheck = false;
        //                    }
        //                }

        //                dgdReceiveGrid.Items.Refresh();
        //            }

        //    }
        //}




        class Win_Acc_Receive_Pay_U_CodeView
        {
            public override string ToString()
            {
                return (this.ReportAllProperties());
            }

            public int Num { get; set; }
           
            public string RPDate { get; set; }
            public string CurrencyUnit { get; set; }
            public string KCustom { get; set; }
            public string BSItem { get; set; }
            public string SalesCharge { get; set; }

            public string CashAmount { get; set; }
            public string BillAmount { get; set; }
            public string BankAmount { get; set; }

            public string RSumAmount { get; set; }
            public string DCAmount { get; set; }
            public string Comments { get; set; }
            public string BankName { get; set; }
            public string CompanyID { get; set; }

            public string RPNo { get; set; }
            public string RPItemCode { get; set; }
            public string KCustomName { get; set; }
            public string RefRPItemCode { get; set; }
            public string RefComments { get; set; }
            public string BankID { get; set; }

            public string RefAccountYN { get; set; }            // 부가세
            public string RefAmount { get; set; }
            public string BillNo { get; set; }
            public string RefBSNO { get; set; }
            public string RPGBN { get; set; }
            public string SumAmount { get; set; }
            public string ReceivePersonName { get; set; }

        }

        class RPItemCode
        {
            string RPItemName { get; set; }
        }

        class Win_Acc_Receive_Pay_U_CodeView_Sum
        {
            public double SumCash { get; set; }
            public double SumCard { get; set; }
            public double SumBill { get; set; }
            public double SumBank { get; set; }
            public double SumDC { get; set; }
            public double SumAmount { get; set; }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog().GetValueOrDefault())
            {
                FontFamily fontFamily = new FontFamily("나눔고딕코딩");

                Grid grid = new Grid();

                grid.SetValue(FontFamilyProperty, fontFamily);
                grid.SetValue(FontSizeProperty, 32d);

                for (int i = 0; i < 5; i++)
                {
                    ColumnDefinition columnDefinition = new ColumnDefinition();

                    grid.ColumnDefinitions.Add(columnDefinition);

                    RowDefinition rowDefinition = new RowDefinition();

                    grid.RowDefinitions.Add(rowDefinition);
                }

                grid.Background = new LinearGradientBrush
                (
                    Colors.Gray,
                    Colors.White,
                    new Point(0, 0),
                    new Point(1, 1)
                );

                for (int i = 0; i < 25; i++)
                {
                    Button button = new Button();

                    button.Margin = new Thickness(10);
                    button.HorizontalAlignment = HorizontalAlignment.Center;
                    button.VerticalAlignment = VerticalAlignment.Center;
                    button.Content = $"버튼 {i + 1,0:d2}";

                    grid.Children.Add(button);

                    Grid.SetRow(button, i % 5);
                    Grid.SetColumn(button, i / 5);
                }

                grid.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                Point gridPoint = new Point
                (
                    (printDialog.PrintableAreaWidth - grid.DesiredSize.Width) / 2,
                    (printDialog.PrintableAreaHeight - grid.DesiredSize.Height) / 2
                );

                Canvas.SetLeft(grid, gridPoint.X);
                Canvas.SetTop(grid, gridPoint.Y);

                Canvas canvas = new Canvas();

                canvas.Width = printDialog.PrintableAreaWidth;
                canvas.Height = printDialog.PrintableAreaHeight;
                canvas.Background = null;

                canvas.Children.Add(grid);

                printDialog.PrintVisual(canvas, "Sample");
            }
        }

        private void btnSearchMoney_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (MenuViewModel mvm in MainWindow.mMenulist)
            {
                if (mvm.Menu.Equals("거래처별  매입/매출 수금/지불 내역서"))
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

    }
}
