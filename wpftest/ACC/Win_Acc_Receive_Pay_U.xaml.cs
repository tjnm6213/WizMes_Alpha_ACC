﻿using System;
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


        // 거래처
        private void lblCustom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkCustom.IsChecked == true) { chkCustom.IsChecked = false; }
            else { chkCustom.IsChecked = true; }
        }
        // 거래처
        private void chkCustom_Checked(object sender, RoutedEventArgs e)
        {
            txtCustom.IsEnabled = true;
            btnPfCustom.IsEnabled = true;
            txtCustom.Focus();
        }
        // 거래처
        private void chkCustom_Unchecked(object sender, RoutedEventArgs e)
        {
            txtCustom.IsEnabled = false;
            btnPfCustom.IsEnabled = false;
        }
        // 매출항목
        private void lblSaleItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkSaleItems.IsChecked == true) { chkSaleItems.IsChecked = false; }
            else { chkSaleItems.IsChecked = true; }
        }
        // 매출항목
        private void chkSaleItems_Checked(object sender, RoutedEventArgs e)
        {
            txtSaleItems.IsEnabled = true;
            btnPfSaleItems.IsEnabled = true;
            txtSaleItems.Focus();
        }
        // 매출항목
        private void chkSaleItems_Unchecked(object sender, RoutedEventArgs e)
        {
            txtSaleItems.IsEnabled = false;
            btnPfSaleItems.IsEnabled = false;
        }
        // 품명
        private void lblArticle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkArticle.IsChecked == true) { chkArticle.IsChecked = false; }
            else { chkArticle.IsChecked = true; }
        }
        // 품명
        private void chkArticle_Checked(object sender, RoutedEventArgs e)
        {
            txtArticle.IsEnabled = true;
            btnPfArticle.IsEnabled = true;
            txtArticle.Focus();
        }
        // 품명
        private void chkArticle_Unchecked(object sender, RoutedEventArgs e)
        {
            txtArticle.IsEnabled = false;
            btnPfArticle.IsEnabled = false;
        }
        // 오더번호
        private void lblOrderNum_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkOrderNum.IsChecked == true) { chkOrderNum.IsChecked = false; }
            else { chkOrderNum.IsChecked = true; }
        }
        // 오더번호
        private void chkOrderNum_Checked(object sender, RoutedEventArgs e)
        {
            txtOrderNum.IsEnabled = true;
            btnPfOrderNum.IsEnabled = true;
            txtOrderNum.Focus();
        }
        // 오더번호
        private void chkOrderNum_Unchecked(object sender, RoutedEventArgs e)
        {
            txtOrderNum.IsEnabled = false;
            btnPfOrderNum.IsEnabled = false;
        }

        #endregion

        #region (콤보박스 세팅) SetComboBox
        private void SetComboBox()
        {

            if (tbnPay.IsChecked == true)
            {
                // 거래종류 출금일 경우        
                ObservableCollection<CodeView> ovcTransacClass = ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "PYM", "Y", "");
                this.cboTransacClass.ItemsSource = ovcTransacClass;
                this.cboTransacClass.DisplayMemberPath = "code_name";
                this.cboTransacClass.SelectedValuePath = "code_id";
            }
            else if (tbnReceive.IsChecked == true)
            {

                //거래종류 입금일 경우
                List<string[]> depositlist = new List<string[]>();
                string[] list03 = new string[] { "4", "현금입금" };
                string[] list04 = new string[] { "5", "어음입금" };
                //string[] list05 = new string[] { "6", "외화입금" };
                depositlist.Add(list03);
                depositlist.Add(list04);
                //depositlist.Add(list05);

                ObservableCollection<CodeView> depositClss = ComboBoxUtil.Instance.Direct_SetComboBox(depositlist);
                this.cboTransacClass.ItemsSource = depositClss;
                this.cboTransacClass.DisplayMemberPath = "code_name";
                this.cboTransacClass.SelectedValuePath = "code_id";

            }


            //매입,매출 화폐단위(입력)
            List<string[]> listPrice = new List<string[]>();
            string[] Price01 = new string[] { "0", "₩" };
            //string[] Price02 = new string[] { "1", "$" };
            listPrice.Add(Price01);
            //listPrice.Add(Price02);

            ObservableCollection<CodeView> ovcCurrencyUnit = ComboBoxUtil.Instance.Direct_SetComboBox(listPrice);
            this.cboCurrencyUnit.ItemsSource = ovcCurrencyUnit;
            this.cboCurrencyUnit.DisplayMemberPath = "code_name";
            this.cboCurrencyUnit.SelectedValuePath = "code_id";


            // 계좌선택
            ObservableCollection<CodeView> ovcAccountChoice = ComboBoxUtil.Instance.GetBankList();
            this.cboAccountChoice.ItemsSource = ovcAccountChoice;
            this.cboAccountChoice.DisplayMemberPath = "code_name";
            this.cboAccountChoice.SelectedValuePath = "code_id";

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
            dgdReceive.IsHitTestVisible = true;

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
            dgdReceive.IsHitTestVisible = false;

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
                //KCustomName은 사용자가 입력하는 데이터로 테이블에 저장되고, 그 값을 불러오는 것으로 수정 2020.02.06, 장가빈
                //KCustomName.Text = txtgrbCustom.Text;
                KCustomName.Focus();
                
            }
        }
        // 플러스파인더 그룹박스 거래처
        private void btngrbpfCustom_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtgrbCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
        }



        // 플러스파인더 계정과목
        private void txtSaleItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbnReceive.IsChecked == true)
                {

                    MainWindow.pf.ReturnCode(txtSaleItems, 80, "In");
                }
                else
                {
                    MainWindow.pf.ReturnCode(txtSaleItems, 80, "Out");
                }
            }
        }
        // 플러스파인더 계정과목
        private void btnPfSaleItems_Click(object sender, RoutedEventArgs e)
        {
            if (tbnReceive.IsChecked == true)
            {

                MainWindow.pf.ReturnCode(txtSaleItems, 80, "In");
            }
            else
            {
                MainWindow.pf.ReturnCode(txtSaleItems, 80, "Out");
            }
        }


        // 플러스파인더 품명
        private void txtArticle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtArticle, (int)Defind_CodeFind.DCF_Article, "");
            }
        }
        // 플러스파인더 품명
        private void btnPfArticle_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtArticle, (int)Defind_CodeFind.DCF_Article, "");
        }




        // 플러스파인더 오더번호
        private void txtOrderNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 4번.
                MainWindow.pf.ReturnCode(txtOrderNum, (int)Defind_CodeFind.DCF_ORDER, "");
            }
        }
        // 플러스파인더 오더번호
        private void btnPfOrderNum_Click(object sender, RoutedEventArgs e)
        {
            // 4번.
            MainWindow.pf.ReturnCode(txtOrderNum, (int)Defind_CodeFind.DCF_ORDER, "");
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
                //// 32번.
                //MainWindow.pf.ReturnCode(txtgrbReceivePayItems, (int)Defind_CodeFind.DCF_ACCRCVITEM, "S");
                //txtgrbCustom.Focus();
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

        #endregion

        #region (토글버튼 체크 체인지 이벤트) CheckedChange
        // 입금 클릭.
        private void tbnReceive_CheckedChange()
        {
            this.DataContext = null;

            //txtgrbReceivePayItems.Text = "계정과목";
            lbrgrbKCustom.Content = "거래처";
            lblgrbBSITEM.Content = "입출금번호";

            grbdgdInGrid.Visibility = Visibility.Hidden;
            grbdgdOutGrid.Visibility = Visibility.Visible;

            SetComboBox();

            // 2020.01.16 추가 - 토글버튼을 클릭했을 때 데이터그리드가 전환되고, 전환된 그 데이터그리드가 선택이 되어있다면, 그 정보를 볼 수있도록. 
            var Receive = dgdReceive.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;
            if (Receive != null)
            {
                this.DataContext = Receive;

                if (Receive.ReceiveNowDateYN == "현금입금")
                {
                    cboTransacClass.SelectedValue = "3";
                }
                else
                {
                    cboTransacClass.SelectedValue = "4";
                }
            }
        }

        // 출금 클릭.
        private void tbnPay_CheckedChange()
        {
            this.DataContext = null;

            //txtgrbReceivePayItems.Text = "계정과목";
            lbrgrbKCustom.Content = "거래처";
            lblgrbBSITEM.Content = "입출금번호";

            grbdgdOutGrid.Visibility = Visibility.Hidden;
            grbdgdInGrid.Visibility = Visibility.Visible;

            SetComboBox();

            // 2020.01.16 추가 - 토글버튼을 클릭했을 때 데이터그리드가 전환되고, 전환된 그 데이터그리드가 선택이 되어있다면, 그 정보를 볼 수있도록. 
            var Receive = dgdPayGrid.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;
            if (Receive != null)
            {
                this.DataContext = Receive;

                if (Receive.ReceiveNowDateYN == "현금지불")
                {
                    cboTransacClass.SelectedValue = "1";
                }
                else
                {
                    cboTransacClass.SelectedValue = "2";
                }
            }
        }

        #endregion



        // 검색버튼 클릭.
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Wh_Ar_SelectedLastIndex = 0;
            re_Search(Wh_Ar_SelectedLastIndex);

            // 2020.01.16 조회 두번 타는거 제외 
            //if (tbnReceive.IsChecked == true) // 입금버튼
            //{
            //    FillGrid_ReceiveGrid();
            //}
            //else if (tbnPay.IsChecked == true) // 출금버튼
            //{
            //    FillGrid_tbnPayGrid();
            //}
        }

        /// <summary>
        /// 재검색(수정,삭제,추가 저장후에 자동 재검색)
        /// </summary>
        /// <param name="selectedIndex"></param>
        private void re_Search(int selectedIndex)
        {
            if (tbnReceive.IsChecked == true) // 입금버튼
            {
                FillGrid_ReceiveGrid();

                if (dgdReceive.Items.Count > 0)
                {
                    dgdReceive.SelectedIndex = selectedIndex;
                }

            }
            else if (tbnPay.IsChecked == true) // 출금버튼
            {
                FillGrid_tbnPayGrid();

                if (dgdPayGrid.Items.Count > 0)
                {
                    dgdPayGrid.SelectedIndex = selectedIndex;
                }
            }
        }


        #region (입금용 그리드 채우기) FillGrid_ReceiveGrid
        // 입금용 그리드 채우기.
        private void FillGrid_ReceiveGrid()
        {
            if (dgdReceive.Items.Count > 0)
            {
                dgdReceive.Items.Clear();
            }

            //합계변수
            var SumIn = new Win_Acc_Receive_Pay_U_CodeView_Sum();


            try
            {
                // 매입 / 매출 토글박스 구분.
                string bsGbnID = "2";

                if (tbnReceive.IsChecked == true) { bsGbnID = "2"; }
                else if (tbnPay.IsChecked == true) { bsGbnID = "1"; }

                // 기간 체크여부 yn.
                int sBSDate = 0;
                if (chkPeriod.IsChecked == true) { sBSDate = 1; }



                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("RPGBN", bsGbnID);
                sqlParameter.Add("sBSDate", sBSDate);
                sqlParameter.Add("sDate", sBSDate == 1 ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("eDate", sBSDate == 1 ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("CustomID", chkCustom.IsChecked == true && txtCustom.Tag != null ? txtCustom.Tag.ToString() : "");
                sqlParameter.Add("BSItemCode", chkSaleItems.IsChecked == true && txtSaleItems.Tag != null ? txtSaleItems.Tag.ToString() : "");
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

                        txtblockSearchCountIn.Text = "      합계 :";
                        txtblockSearchCashIn.Text = "현금 :";
                        txtblockSearchCardIn.Text = "카드 :";
                        txtblockSearchBillIn.Text = "어음 :";
                        txtblockSearchBankIn.Text = "은행 :";
                        txtblockSearchDCIn.Text = "감액 :";
                        txtblockSearchTotalIn.Text = "합계금액 :";

                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;
                        foreach (DataRow dr in drc)
                        {
                            var WinAccBuySale = new Win_Acc_Receive_Pay_U_CodeView()
                            {
                                Num = i + 1,
                                IsCheck = false,

                                RPNo = dr["RPNo"].ToString(),
                                RPGBN = dr["RPGBN"].ToString(),
                                companyid = dr["companyid"].ToString(),
                                RPDate = DateTime.ParseExact(dr["RPDate"].ToString(), "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                BSItem = dr["BSItem"].ToString(),

                                RPItemCode = dr["RPItemCode"].ToString(),
                                CurrencyUnit = dr["CurrencyUnit"].ToString(),
                                CustomID = dr["CustomID"].ToString(),
                                SalesCharge = dr["SalesCharge"].ToString(),
                                BankID = dr["BankID"].ToString(),
                                BankName = dr["BankName"].ToString(),

                                CashAmount = dr["CashAmount"].ToString(),
                                BillAmount = dr["BillAmount"].ToString(),
                                BankAmount = dr["BankAmount"].ToString(),
                                DCAmount = dr["DCAmount"].ToString(),
                                BillNo = dr["BillNo"].ToString(),

                                VATAmount = dr["VATAmount"].ToString(),
                                ForReceiveBillAmount = dr["ForReceiveBillAmount"].ToString(),
                                ReceiveNowDateYN = dr["ReceiveNowDateYN"].ToString(),
                                CardAmount = dr["CardAmount"].ToString(),
                                ReceivePersonName = dr["ReceivePersonName"].ToString(),
                                Bank = dr["Bank"].ToString(),
                                Comments = dr["Comments"].ToString(),

                                OrderID = dr["OrderID"].ToString(),
                                RefBSNO = dr["RefBSNO"].ToString(),
                                OrderFlag = dr["OrderFlag"].ToString(),
                                RefRPItemCode = dr["RefRPItemCode"].ToString(),
                                RefComments = dr["RefComments"].ToString(),

                                RefAccountYN = dr["RefAccountYN"].ToString(),
                                RefAmount = dr["RefAmount"].ToString(),

                                KCustom = dr["KCustom"].ToString(),
                                KCustomName = dr["KCustomName"].ToString(),

                            };

                            SumIn.SumCash += ConvertDouble(WinAccBuySale.CashAmount);
                            SumIn.SumCard += ConvertDouble(WinAccBuySale.CardAmount);
                            SumIn.SumBill += ConvertDouble(WinAccBuySale.BillAmount);
                            SumIn.SumBank += ConvertDouble(WinAccBuySale.BankAmount);
                            SumIn.SumDC += ConvertDouble(WinAccBuySale.DCAmount);
                            SumIn.SumTotal += ConvertDouble(WinAccBuySale.ForReceiveBillAmount);


                            //거래종류
                            if (WinAccBuySale.ReceiveNowDateYN.Trim().Equals("Y") && WinAccBuySale.CurrencyUnit.ToString().Equals("0"))
                            {
                                WinAccBuySale.ReceiveNowDateYN = "현금입금";
                                WinAccBuySale.cboReceiveNowDateYN = "4";
                            }
                            //else if(WinAccBuySale.ReceiveNowDateYN.Trim().Equals("Y") && WinAccBuySale.CurrencyUnit.ToString().Equals("1"))
                            //{
                            //    WinAccBuySale.ReceiveNowDateYN = "외화입금";
                            //    WinAccBuySale.cboReceiveNowDateYN = "6";
                            //}
                            else
                            {
                                WinAccBuySale.ReceiveNowDateYN = "어음입금";
                                WinAccBuySale.cboReceiveNowDateYN = "5";
                            }

                            // 화폐단위
                            if (WinAccBuySale.CurrencyUnit.Trim().Equals("0"))
                            {
                                WinAccBuySale.CurrencyUnitName = "₩";
                            }
                            else
                            {
                                WinAccBuySale.CurrencyUnitName = "$";
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
                            // 콤마입히기 > 카드
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.CardAmount))
                            {
                                WinAccBuySale.CardAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.CardAmount);
                            }
                            // 콤마입히기 > 감액
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.DCAmount))
                            {
                                WinAccBuySale.DCAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.DCAmount);
                            }
                            // 콤마입히기 > 합계
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.ForReceiveBillAmount))
                            {
                                WinAccBuySale.ForReceiveBillAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.ForReceiveBillAmount);
                            }


                            dgdReceive.Items.Add(WinAccBuySale);
                            i++;
                        }

                        txtblockSearchCountIn.Text = "      합계 : " + i.ToString() + "건"; ;
                        txtblockSearchCashIn.Text = "현금 : " + stringFormatN0(SumIn.SumCash) + "원";
                        txtblockSearchCardIn.Text = "카드 : " + stringFormatN0(SumIn.SumCard) + "원";
                        txtblockSearchBillIn.Text = "어음 : " + stringFormatN0(SumIn.SumBill) + "원";
                        txtblockSearchBankIn.Text = "은행 : " + stringFormatN0(SumIn.SumBank) + "원";
                        txtblockSearchDCIn.Text = "감액 : " + stringFormatN0(SumIn.SumDC) + "원";
                        txtblockSearchTotalIn.Text = "합계금액 : " + stringFormatN0(SumIn.SumTotal) + "원";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion

        #region (출금용 그리드 채우기) FillGrid_tbnPayGrid
        private void FillGrid_tbnPayGrid()
        {
            if (dgdPayGrid.Items.Count > 0)
            {
                dgdPayGrid.Items.Clear();
            }

            //합계변수
            var SumOut = new Win_Acc_Receive_Pay_U_CodeView_Sum();

            try
            {
                // 매입 / 매출 토글박스 구분.
                string bsGbnID = "1";

                // 기간 체크여부 yn.
                int sBSDate = 0;
                if (chkPeriod.IsChecked == true) { sBSDate = 1; }



                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("bsGbnID", bsGbnID);
                sqlParameter.Add("sBSDate", sBSDate);
                sqlParameter.Add("sDate", sBSDate == 1 ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("eDate", sBSDate == 1 ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("CustomID", chkCustom.IsChecked == true && txtCustom.Tag != null ? txtCustom.Tag.ToString() : "");
                sqlParameter.Add("BSItemCode", chkSaleItems.IsChecked == true && txtSaleItems.Tag != null ? txtSaleItems.Tag.ToString() : "");
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

                        txtblockSearchCountOut.Text = "      합계 :";
                        txtblockSearchCashOut.Text = "현금 :";
                        txtblockSearchCardOut.Text = "카드 :";
                        txtblockSearchBillOut.Text = "어음 :";
                        txtblockSearchBankOut.Text = "은행 :";
                        txtblockSearchDCOut.Text = "감액 :";
                        txtblockSearchTotalOut.Text = "합계금액 :";
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;
                        foreach (DataRow dr in drc)
                        {
                            var WinAccBuySale = new Win_Acc_Receive_Pay_U_CodeView()
                            {
                                Num = i + 1,
                                IsCheck = false,

                                RPNo = dr["RPNo"].ToString(),
                                RPGBN = dr["RPGBN"].ToString(),
                                companyid = dr["companyid"].ToString(),
                                RPDate = DateTime.ParseExact(dr["RPDate"].ToString(), "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                BSItem = dr["BSItem"].ToString(),

                                RPItemCode = dr["RPItemCode"].ToString(),
                                CurrencyUnit = dr["CurrencyUnit"].ToString(),
                                CustomID = dr["CustomID"].ToString(),
                                SalesCharge = dr["SalesCharge"].ToString(),
                                BankID = dr["BankID"].ToString(),
                                BankName = dr["BankName"].ToString(),

                                CashAmount = dr["CashAmount"].ToString(),
                                BillAmount = dr["BillAmount"].ToString(),
                                BankAmount = dr["BankAmount"].ToString(),
                                DCAmount = dr["DCAmount"].ToString(),
                                BillNo = dr["BillNo"].ToString(),

                                VATAmount = dr["VATAmount"].ToString(),
                                ForReceiveBillAmount = dr["ForReceiveBillAmount"].ToString(),
                                ReceiveNowDateYN = dr["ReceiveNowDateYN"].ToString(),
                                CardAmount = dr["CardAmount"].ToString(),
                                ReceivePersonName = dr["ReceivePersonName"].ToString(),
                                Bank = dr["Bank"].ToString(),
                                Comments = dr["Comments"].ToString(),

                                OrderID = dr["OrderID"].ToString(),
                                RefBSNO = dr["RefBSNO"].ToString(),
                                OrderFlag = dr["OrderFlag"].ToString(),
                                RefRPItemCode = dr["RefRPItemCode"].ToString(),
                                RefComments = dr["RefComments"].ToString(),

                                RefAccountYN = dr["RefAccountYN"].ToString(),
                                RefAmount = dr["RefAmount"].ToString(),

                                KCustom = dr["KCustom"].ToString(),
                                KCustomName = dr["KCustomName"].ToString()
                            };

                            SumOut.SumCash += ConvertDouble(WinAccBuySale.CashAmount);
                            SumOut.SumCard += ConvertDouble(WinAccBuySale.CardAmount);
                            SumOut.SumBill += ConvertDouble(WinAccBuySale.BillAmount);
                            SumOut.SumBank += ConvertDouble(WinAccBuySale.BankAmount);
                            SumOut.SumDC += ConvertDouble(WinAccBuySale.DCAmount);
                            SumOut.SumTotal += ConvertDouble(WinAccBuySale.ForReceiveBillAmount);



                            //거래종류
                            if (WinAccBuySale.ReceiveNowDateYN.Trim().Equals("Y") && WinAccBuySale.CurrencyUnit.ToString().Equals("0"))
                            {
                                WinAccBuySale.ReceiveNowDateYN = "현금지불";
                                WinAccBuySale.cboReceiveNowDateYN = "1";
                            }
                            //else if(WinAccBuySale.ReceiveNowDateYN.Trim().Equals("Y") && WinAccBuySale.CurrencyUnit.ToString().Equals("1"))
                            //{
                            //    WinAccBuySale.ReceiveNowDateYN = "외화지불";
                            //    WinAccBuySale.cboReceiveNowDateYN = "3";
                            //}
                            else
                            {
                                WinAccBuySale.ReceiveNowDateYN = "어음지불";
                                WinAccBuySale.cboReceiveNowDateYN = "2";
                            }

                            // 화폐단위
                            if (WinAccBuySale.CurrencyUnit.Trim().Equals("0"))
                            {
                                WinAccBuySale.CurrencyUnitName = "₩";
                            }
                            else
                            {
                                WinAccBuySale.CurrencyUnitName = "$";
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
                            // 콤마입히기 > 카드
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.CardAmount))
                            {
                                WinAccBuySale.CardAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.CardAmount);
                            }
                            // 콤마입히기 > 감액
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.DCAmount))
                            {
                                WinAccBuySale.DCAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.DCAmount);
                            }
                            // 콤마입히기 > 합계
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.ForReceiveBillAmount))
                            {
                                WinAccBuySale.ForReceiveBillAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.ForReceiveBillAmount);
                            }

                            dgdPayGrid.Items.Add(WinAccBuySale);
                            i++;
                        }

                        txtblockSearchCountOut.Text = "      합계 : " + i.ToString() + "건"; ;
                        txtblockSearchCashOut.Text = "현금 : " + stringFormatN0(SumOut.SumCash) + "원";
                        txtblockSearchCardOut.Text = "카드 : " + stringFormatN0(SumOut.SumCard) + "원";
                        txtblockSearchBillOut.Text = "어음 : " + stringFormatN0(SumOut.SumBill) + "원";
                        txtblockSearchBankOut.Text = "은행 : " + stringFormatN0(SumOut.SumBank) + "원";
                        txtblockSearchDCOut.Text = "감액 : " + stringFormatN0(SumOut.SumDC) + "원";
                        txtblockSearchTotalOut.Text = "합계금액 : " + stringFormatN0(SumOut.SumTotal) + "원";
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
            //dtpDate.SelectedDate = DateTime.Today;

            if (SelectDate != "")
            {
                dtpDate.SelectedDate = DateTime.Parse(SelectDate.Substring(0, 4) + "-" + SelectDate.Substring(4, 2) + "-" + SelectDate.Substring(6, 2));
            }


            cboCurrencyUnit.SelectedIndex = 0;
            cboTransacClass.SelectedIndex = 0;
            cboAccountChoice.SelectedIndex = 0;

            InsertOrUpdate = "I";

            // 추가시 자동 포커스 초기세팅.
            cboTransacClass.Focus();
            cboTransacClass.IsDropDownOpen = true;

            if (tbnReceive.IsChecked == true)   // 입금탭이라면,
            {
                if (dgdReceive.Items.Count > 0)
                {
                    Wh_Ar_SelectedLastIndex = dgdReceive.SelectedIndex;
                }
                else
                {
                    Wh_Ar_SelectedLastIndex = 0;
                }
            }
            else if (tbnPay.IsChecked == true)  // 출금탭이라면,
            {
                if (dgdPayGrid.Items.Count > 0)
                {
                    Wh_Ar_SelectedLastIndex = dgdPayGrid.SelectedIndex;
                }
                else
                {
                    Wh_Ar_SelectedLastIndex = 0;
                }
            }
        }

        // 수정버튼 클릭.
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (tbnReceive.IsChecked == true)
            {
                if (dgdReceive.Items.Count < 1)
                {
                    MessageBox.Show("먼저 검색해 주세요.");
                    return;
                }
                var OBJ = dgdReceive.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;
                if (OBJ == null)
                {
                    MessageBox.Show("수정할 항목이 정확히 선택되지 않았습니다.");
                    return;
                }
            }
            else if (tbnPay.IsChecked == true)
            {
                if (dgdPayGrid.Items.Count < 1)
                {
                    MessageBox.Show("먼저 검색해 주세요.");
                    return;
                }
                var OBJ = dgdPayGrid.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;
                if (OBJ == null)
                {
                    MessageBox.Show("수정할 항목이 정확히 선택되지 않았습니다.");
                    return;
                }
            }

            CantBtnControl();
            InsertOrUpdate = "U";

            if (tbnReceive.IsChecked == true)
            {
                Wh_Ar_SelectedLastIndex = dgdReceive.SelectedIndex;
            }
            else if (tbnPay.IsChecked == true)
            {
                Wh_Ar_SelectedLastIndex = dgdPayGrid.SelectedIndex;
            }

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
            if (tbnReceive.IsChecked == true)
            {
                foreach (Win_Acc_Receive_Pay_U_CodeView Win_Acc_Buy_Receive in dgdReceive.Items)
                {
                    if (Win_Acc_Buy_Receive != null)
                    {
                        if (Win_Acc_Buy_Receive.IsCheck == true)
                        {
                            D_Check++;
                        }
                    }
                }
            }
            else if (tbnPay.IsChecked == true)
            {
                foreach (Win_Acc_Receive_Pay_U_CodeView Win_Acc_Buy_Receive in dgdPayGrid.Items)
                {
                    if (Win_Acc_Buy_Receive != null)
                    {
                        if (Win_Acc_Buy_Receive.IsCheck == true)
                        {

                            D_Check++;
                        }
                    }
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
                    if (tbnReceive.IsChecked == true)
                    {
                        if (dgdReceive.Items.Count > 0 && dgdReceive.SelectedItem != null)
                        {
                            Wh_Ar_SelectedLastIndex = dgdReceive.SelectedIndex;
                        }
                        foreach (Win_Acc_Receive_Pay_U_CodeView Win_Acc_Buy_Receive in dgdReceive.Items)
                        {
                            if (Win_Acc_Buy_Receive != null)
                            {
                                if (Win_Acc_Buy_Receive.IsCheck == true)
                                {
                                    Delete_Data(Win_Acc_Buy_Receive.RPNo, Win_Acc_Buy_Receive.RPGBN);
                                }
                            }
                        }
                        dgdReceive.Refresh();
                        Wh_Ar_SelectedLastIndex -= 1;
                        re_Search(Wh_Ar_SelectedLastIndex);
                        //FillGrid_ReceiveGrid();     // 재검색.
                    }
                    else if (tbnPay.IsChecked == true)
                    {
                        if (dgdPayGrid.Items.Count > 0 && dgdPayGrid.SelectedItem != null)
                        {
                            Wh_Ar_SelectedLastIndex = dgdPayGrid.SelectedIndex;
                        }
                        foreach (Win_Acc_Receive_Pay_U_CodeView Win_Acc_Buy_Receive in dgdPayGrid.Items)
                        {
                            if (Win_Acc_Buy_Receive != null)
                            {
                                if (Win_Acc_Buy_Receive.IsCheck == true)
                                {
                                    Delete_Data(Win_Acc_Buy_Receive.RPNo, Win_Acc_Buy_Receive.RPGBN);
                                }
                            }
                        }
                        dgdPayGrid.Refresh();
                        Wh_Ar_SelectedLastIndex -= 1;
                        re_Search(Wh_Ar_SelectedLastIndex);
                        //FillGrid_tbnPayGrid();     // 재검색.
                    }
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
                    SelectDate = dtpDate.SelectedDate.Value.ToString("yyyyMMdd");

                    RPItemName = string.Empty;      //저장 후에는 비워줘야지 2020.02.22, 장가빈

                    if (tbnReceive.IsChecked == true)
                    {
                        if (InsertOrUpdate == "I")     //1. 추가 > 저장했다면,
                        {
                            if (dgdReceive.Items.Count > 0)
                            {
                                re_Search(dgdReceive.Items.Count - 1);
                                dgdReceive.Focus();
                            }
                            else
                            { re_Search(0); }
                        }
                        else        //2. 수정 > 저장했다면,
                        {
                            re_Search(Wh_Ar_SelectedLastIndex);
                            dgdReceive.Focus();
                        }
                    }
                    else if (tbnPay.IsChecked == true)
                    {
                        if (InsertOrUpdate == "I")     //1. 추가 > 저장했다면,
                        {
                            if (dgdPayGrid.Items.Count > 0)
                            {
                                re_Search(dgdPayGrid.Items.Count - 1);
                                dgdPayGrid.Focus();
                            }
                            else
                            { re_Search(0); }
                        }
                        else        //2. 수정 > 저장했다면,
                        {
                            re_Search(Wh_Ar_SelectedLastIndex);
                            dgdPayGrid.Focus();
                        }
                    }
                }
            }
        }


        #region(저장 전, 필수입력 칸 입력여부 체크) Check_EssentialData
        private bool Check_EssentialData()
        {
            bool Flag = true;

            if (dtpDate.SelectedDate == null)
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
            if (cboTransacClass.SelectedValue == null)
            {
                MessageBox.Show("거래종류가 입력되지 않았습니다. 먼저 거래종류를 입력해주세요");
                Flag = false;
                return Flag;
            }
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

                if (RPGBN == "2")
                {
                    var WinAccBuySale = dgdReceive.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;

                    if (WinAccBuySale != null) // 수정1
                    {
                        RPNo = WinAccBuySale.RPNo;
                    }
                }
                else if (RPGBN == "1")
                {
                    var WinAccBuySale = dgdPayGrid.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;

                    if (WinAccBuySale != null)
                    {
                        RPNo = WinAccBuySale.RPNo;
                    }
                }

                if (txtOrderNum.Tag == null || txtOrderNum.Text.Length <= 0)
                {
                    txtOrderNum.Tag = (object)"";
                }
                if (txtArticle.Tag == null || txtArticle.Text.Length <= 0)
                {
                    txtArticle.Tag = (object)"";
                }


                double D_CashAmount = 0;
                double D_BillAmount = 0;
                double D_BankAmount = 0;
                double D_CardAmount = 0;
                double D_DCAmount = 0;
                double D_ReceiveBillAmount = 0;


                double.TryParse(txtgrbCash.Text, out D_CashAmount);
                double.TryParse(txtgrbBankPaper.Text, out D_BillAmount);
                double.TryParse(txtgrbBankPay.Text, out D_BankAmount);
                double.TryParse(txtgrbCard.Text, out D_CardAmount);
                double.TryParse(txtgrbDiscount.Text, out D_DCAmount);
                double.TryParse(txtgrbTotalAmount.Text.Replace(",", ""), out D_ReceiveBillAmount);

                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();


                sqlParameter.Add("RPGBN", RPGBN);                                                      //입금 = 1.
                sqlParameter.Add("companyid", "0001");                                               // 기본.
                sqlParameter.Add("RPDate", dtpDate.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("BSItem", !RPItemFindName().Equals("") ? RPItemFindName().ToString().Trim() : "");       //txtgrbReceivePayItems.Text);  //매입매출번호
                sqlParameter.Add("RPItemCode", txtgrbReceivePayItems.Tag != null ? txtgrbReceivePayItems.Tag.ToString() : "");                 // 계정과목

                sqlParameter.Add("CurrencyUnit", cboCurrencyUnit.SelectedValue.ToString());         // 화폐단위
                sqlParameter.Add("CustomID", txtgrbCustom.Tag.ToString());                          // 거래처.
                sqlParameter.Add("SalesCharge", "");                                                // (     )
                sqlParameter.Add("BankID", cboAccountChoice.SelectedValue != null ? cboAccountChoice.SelectedValue.ToString() : "");              // 계좌선택
                sqlParameter.Add("CashAmount", D_CashAmount);                                       // 현금

                sqlParameter.Add("BillAmount", D_BillAmount);                                       // 어음
                sqlParameter.Add("BankAmount", D_BankAmount);                                       // 은행
                sqlParameter.Add("DCAmount", D_DCAmount);                                           // 감액
                sqlParameter.Add("BillNo", txtgrbBankPaperNo.Text);                                 // 어음 번호
                sqlParameter.Add("VATAmount", 0);                                         //부가세

                sqlParameter.Add("ForReceiveBillAmount", D_ReceiveBillAmount);                      // 합계금액

                if (RPGBN == "2") //입금일 경우에는 
                {
                    sqlParameter.Add("ReceiveNowDateYN", cboTransacClass.SelectedValue.ToString() == "4" ? "Y" : "N");  // 거래종류
                }
                else             //출금일 경우에는
                {
                    sqlParameter.Add("ReceiveNowDateYN", cboTransacClass.SelectedValue.ToString() == "1" ? "Y" : "N");  // 거래종류
                }

                sqlParameter.Add("CardAmount", ConvertDouble(txtgrbCard.Text));                       //카드
                sqlParameter.Add("ReceivePersonName", "");
                sqlParameter.Add("Bank", txtgrbBankPay.Text); //은행  ??? 

                sqlParameter.Add("Comments", txtgrbComment.Text);                                   // 비고
                sqlParameter.Add("OrderID", "");
                sqlParameter.Add("RefBSNO", "");
                sqlParameter.Add("OrderFlag", 0);
                sqlParameter.Add("RefRPItemCode", "");

                sqlParameter.Add("RefComments", txtgrbSemiComment.Text);                            // 적요(sub비고)
                sqlParameter.Add("RefAccountYN", "");
                sqlParameter.Add("RefAmount", 0);                                                  //금액
                sqlParameter.Add("Createuserid", MainWindow.CurrentUser);                           // 생성자.
                sqlParameter.Add("KCustomName", KCustomName.Text.ToString());                           //거래처명

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

            if (tbnReceive.IsChecked == true)
            {
                if (InsertOrUpdate == "I")     //1. 추가 도중 취소했다면.
                {
                    if (dgdReceive.Items.Count > 0)
                    {
                        re_Search(Wh_Ar_SelectedLastIndex);
                        dgdReceive.Focus();
                    }
                    else
                    { re_Search(0); }
                }
                else        //2. 수정 도중 취소했다면
                {
                    re_Search(Wh_Ar_SelectedLastIndex);
                    dgdReceive.Focus();
                }
            }
            else if (tbnPay.IsChecked == true)
            {
                if (InsertOrUpdate == "I")     //1. 추가 도중 취소했다면.
                {
                    if (dgdPayGrid.Items.Count > 0)
                    {
                        re_Search(Wh_Ar_SelectedLastIndex);
                        dgdPayGrid.Focus();
                    }
                    else
                    { re_Search(0); }
                }
                else        //2. 수정 도중 취소했다면
                {
                    re_Search(Wh_Ar_SelectedLastIndex);
                    dgdPayGrid.Focus();
                }
            }
        }

        // 닫기버튼 클릭.
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }


        // 데이터 그리드 항목 클릭_ SelectionChanged
        private void dgdOutGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbnReceive.IsChecked == true)
            {
                var WinAccBuySale = dgdReceive.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;
                if (WinAccBuySale != null)
                {
                    this.DataContext = WinAccBuySale;

                    txtgrbReceivePayItems.Tag = WinAccBuySale.RPItemCode;

                    //  2020.01.16 거래구분이
                    //if (WinAccBuySale.ReceiveNowDateYN == "현금입금")
                    //{
                    //    cboTransacClass.SelectedValue = "3";
                    //}
                    //else
                    //{
                    //    cboTransacClass.SelectedValue = "4";
                    //}
                }
            }
        }
        // 데이터 그리드 항목 클릭_ SelectionChanged
        private void dgdInGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbnPay.IsChecked == true)
            {
                var WinAccBuySale = dgdPayGrid.SelectedItem as Win_Acc_Receive_Pay_U_CodeView;
                if (WinAccBuySale != null)
                {
                    this.DataContext = WinAccBuySale;

                    txtgrbReceivePayItems.Tag = WinAccBuySale.RPItemCode;


                    //if (WinAccBuySale.ReceiveNowDateYN == "현금지불")
                    //{
                    //    cboTransacClass.SelectedValue = "1";
                    //}
                    //else
                    //{
                    //    cboTransacClass.SelectedValue = "2";
                    //}
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
                dgdStr[1] = dgdReceive.Name;
            }
            else
            {
                dgdStr[0] = "출금 등록 리스트";
                dgdStr[1] = dgdPayGrid.Name;
            }

            ExportExcelxaml ExpExc = new ExportExcelxaml(dgdStr);
            ExpExc.ShowDialog();

            if (ExpExc.DialogResult.HasValue)
            {
                if (ExpExc.choice.Equals(dgdReceive.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdReceive);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdReceive);

                    Name = dgdReceive.Name;
                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else if (ExpExc.choice.Equals(dgdPayGrid.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdPayGrid);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdPayGrid);

                    Name = dgdPayGrid.Name;
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
        //거래처에서 화폐단위로 포커스 이동 
        private void KCustomName_KeyDown(object sender, EventArgs e)
        {
            cboCurrencyUnit.Focus();
            cboCurrencyUnit.IsDropDownOpen = true;
        }

        // 화폐단위에서 은행명으로 포커스 이동.
        private void cboCurrencyUnit_DropDownClosed(object sender, EventArgs e)
        {
            cboAccountChoice.Focus();
            cboAccountChoice.IsDropDownOpen = true;
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
                txtgrbComment.Focus();
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
                txtgrbCard.Focus();
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
                cboTransacClass.Focus();
                cboTransacClass.IsDropDownOpen = true;
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

        //외화지불, 외화입금을 선택시 화페단위는 $로 변화되도록.
        private void CboTransacClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbnPay.IsChecked == true)
            {
                if (cboTransacClass.SelectedValue != null)
                {
                    if (cboTransacClass.SelectedValue.ToString().Equals("1"))
                    {
                        cboCurrencyUnit.SelectedValue = "0";  //현금이면 원화
                    }
                    else if (cboTransacClass.SelectedValue.ToString().Equals("3"))
                    {
                        cboCurrencyUnit.SelectedValue = "1";  //외화면 미화
                    }
                    else
                    {
                        cboCurrencyUnit.SelectedValue = "0";  //어음이면 그냥 원화.
                    }
                }
            }
            else
            {
                if (cboTransacClass.SelectedValue != null)
                {
                    if (cboTransacClass.SelectedValue.ToString().Equals("4"))
                    {
                        cboCurrencyUnit.SelectedValue = "0";  //현금이면 원화
                    }
                    else if (cboTransacClass.SelectedValue.ToString().Equals("6"))
                    {
                        cboCurrencyUnit.SelectedValue = "1";  //외화면 미화
                    }
                    else
                    {
                        cboCurrencyUnit.SelectedValue = "0";  //어음이면 그냥 원화.
                    }
                }
            }

        }

        //입금 체크 이벤트(합계 계산)
        private void ChkC_Checked_In(object sender, RoutedEventArgs e)
        {
            var SumIn = new Win_Acc_Receive_Pay_U_CodeView_Sum();

            int j = 0;

            for (int i = 0; i < dgdReceive.Items.Count; i++)
            {
                var Data = dgdReceive.Items[i] as Win_Acc_Receive_Pay_U_CodeView;

                if (Data.IsCheck == true)
                {
                    SumIn.SumCash += ConvertDouble(Data.CashAmount);
                    SumIn.SumCard += ConvertDouble(Data.CardAmount);
                    SumIn.SumBill += ConvertDouble(Data.BillAmount);
                    SumIn.SumBank += ConvertDouble(Data.BankAmount);
                    SumIn.SumDC += ConvertDouble(Data.DCAmount);
                    SumIn.SumTotal += ConvertDouble(Data.ForReceiveBillAmount);

                    j = j + 1;
                }

                txtblockSearchCountIn.Text = "      합계 : " + j.ToString() + "건"; ;
                txtblockSearchCashIn.Text = "현금 : " + stringFormatN0(SumIn.SumCash) + "원";
                txtblockSearchCardIn.Text = "카드 : " + stringFormatN0(SumIn.SumCard) + "원";
                txtblockSearchBillIn.Text = "어음 : " + stringFormatN0(SumIn.SumBill) + "원";
                txtblockSearchBankIn.Text = "은행 : " + stringFormatN0(SumIn.SumBank) + "원";
                txtblockSearchDCIn.Text = "감액 : " + stringFormatN0(SumIn.SumDC) + "원";
                txtblockSearchTotalIn.Text = "합계금액 : " + stringFormatN0(SumIn.SumTotal) + "원";

            }
        }

        //입금 체크 해제 이벤트(합계 계산)
        private void ChkC_Unchecked_In(object sender, RoutedEventArgs e)
        {
            var SumIn = new Win_Acc_Receive_Pay_U_CodeView_Sum();

            int j = 0;

            for (int i = 0; i < dgdReceive.Items.Count; i++)
            {
                var Data = dgdReceive.Items[i] as Win_Acc_Receive_Pay_U_CodeView;

                if (Data.IsCheck == true)
                {
                    SumIn.SumCash += ConvertDouble(Data.CashAmount);
                    SumIn.SumCard += ConvertDouble(Data.CardAmount);
                    SumIn.SumBill += ConvertDouble(Data.BillAmount);
                    SumIn.SumBank += ConvertDouble(Data.BankAmount);
                    SumIn.SumDC += ConvertDouble(Data.DCAmount);
                    SumIn.SumTotal += ConvertDouble(Data.ForReceiveBillAmount);

                    j = j + 1;
                }

                txtblockSearchCountIn.Text = "      합계 : " + j.ToString() + "건"; ;
                txtblockSearchCashIn.Text = "현금 : " + stringFormatN0(SumIn.SumCash) + "원";
                txtblockSearchCardIn.Text = "카드 : " + stringFormatN0(SumIn.SumCard) + "원";
                txtblockSearchBillIn.Text = "어음 : " + stringFormatN0(SumIn.SumBill) + "원";
                txtblockSearchBankIn.Text = "은행 : " + stringFormatN0(SumIn.SumBank) + "원";
                txtblockSearchDCIn.Text = "감액 : " + stringFormatN0(SumIn.SumDC) + "원";
                txtblockSearchTotalIn.Text = "합계금액 : " + stringFormatN0(SumIn.SumTotal) + "원";

            }
        }

        //출금 체크 이벤트(합계 계산)
        private void ChkC_Checked_Out(object sender, RoutedEventArgs e)
        {
            var SumOut = new Win_Acc_Receive_Pay_U_CodeView_Sum();

            int j = 0;

            for (int i = 0; i < dgdPayGrid.Items.Count; i++)
            {
                var Data = dgdPayGrid.Items[i] as Win_Acc_Receive_Pay_U_CodeView;

                if (Data.IsCheck == true)
                {
                    SumOut.SumCash += ConvertDouble(Data.CashAmount);
                    SumOut.SumCard += ConvertDouble(Data.CardAmount);
                    SumOut.SumBill += ConvertDouble(Data.BillAmount);
                    SumOut.SumBank += ConvertDouble(Data.BankAmount);
                    SumOut.SumDC += ConvertDouble(Data.DCAmount);
                    SumOut.SumTotal += ConvertDouble(Data.ForReceiveBillAmount);

                    j = j + 1;
                }

                txtblockSearchCountOut.Text = "      합계 : " + j.ToString() + "건"; ;
                txtblockSearchCashOut.Text = "현금 : " + stringFormatN0(SumOut.SumCash) + "원";
                txtblockSearchCardOut.Text = "카드 : " + stringFormatN0(SumOut.SumCard) + "원";
                txtblockSearchBillOut.Text = "어음 : " + stringFormatN0(SumOut.SumBill) + "원";
                txtblockSearchBankOut.Text = "은행 : " + stringFormatN0(SumOut.SumBank) + "원";
                txtblockSearchDCOut.Text = "감액 : " + stringFormatN0(SumOut.SumDC) + "원";
                txtblockSearchTotalOut.Text = "합계금액 : " + stringFormatN0(SumOut.SumTotal) + "원";

            }
        }

        //출금 체크 해제 이벤트(합계 계산)
        private void ChkC_Unchecked_Out(object sender, RoutedEventArgs e)
        {
            var SumOut = new Win_Acc_Receive_Pay_U_CodeView_Sum();

            int j = 0;

            for (int i = 0; i < dgdPayGrid.Items.Count; i++)
            {
                var Data = dgdPayGrid.Items[i] as Win_Acc_Receive_Pay_U_CodeView;

                if (Data.IsCheck == true)
                {
                    SumOut.SumCash += ConvertDouble(Data.CashAmount);
                    SumOut.SumCard += ConvertDouble(Data.CardAmount);
                    SumOut.SumBill += ConvertDouble(Data.BillAmount);
                    SumOut.SumBank += ConvertDouble(Data.BankAmount);
                    SumOut.SumDC += ConvertDouble(Data.DCAmount);
                    SumOut.SumTotal += ConvertDouble(Data.ForReceiveBillAmount);

                    j = j + 1;
                }

                txtblockSearchCountOut.Text = "      합계 : " + j.ToString() + "건"; ;
                txtblockSearchCashOut.Text = "현금 : " + stringFormatN0(SumOut.SumCash) + "원";
                txtblockSearchCardOut.Text = "카드 : " + stringFormatN0(SumOut.SumCard) + "원";
                txtblockSearchBillOut.Text = "어음 : " + stringFormatN0(SumOut.SumBill) + "원";
                txtblockSearchBankOut.Text = "은행 : " + stringFormatN0(SumOut.SumBank) + "원";
                txtblockSearchDCOut.Text = "감액 : " + stringFormatN0(SumOut.SumDC) + "원";
                txtblockSearchTotalOut.Text = "합계금액 : " + stringFormatN0(SumOut.SumTotal) + "원";

            }
        }

        //전체선택 텍스트블럭 이벤트
        private void TbkSelectAll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkSelectAll.IsChecked == false)
            {
                chkSelectAll.IsChecked = true;
            }
            else
            {
                chkSelectAll.IsChecked = false;
            }
        }

        //전체선택 체크박스 체크
        private void ChkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (tbnPay.IsChecked == true)
            {
                if (dgdPayGrid.Items.Count > 0)
                {
                    foreach (Win_Acc_Receive_Pay_U_CodeView RP_Chek in dgdPayGrid.Items)
                    {
                        if (RP_Chek != null)
                        {
                            RP_Chek.IsCheck = true;
                        }
                    }

                    dgdPayGrid.Items.Refresh();
                }
            }
            else if (tbnReceive.IsChecked == true)
            {
                if (dgdReceive.Items.Count > 0)
                {
                    foreach (Win_Acc_Receive_Pay_U_CodeView RP_Chek in dgdReceive.Items)
                    {
                        if (RP_Chek != null)
                        {
                            RP_Chek.IsCheck = true;
                        }
                    }

                    dgdReceive.Items.Refresh();
                }
            }
        }

        //전체선택 체크박스 체크해제
        private void ChkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tbnPay.IsChecked == true)
            {
                if (dgdPayGrid.Items.Count > 0)
                {
                    foreach (Win_Acc_Receive_Pay_U_CodeView RP_Chek in dgdPayGrid.Items)
                    {
                        if (RP_Chek != null)
                        {
                            RP_Chek.IsCheck = false;
                        }
                    }

                    dgdPayGrid.Items.Refresh();
                }
            }
            else if (tbnReceive.IsChecked == true)
            {
                if (dgdReceive.Items.Count > 0)
                {
                    foreach (Win_Acc_Receive_Pay_U_CodeView RP_Chek in dgdReceive.Items)
                    {
                        if (RP_Chek != null)
                        {
                            RP_Chek.IsCheck = false;
                        }
                    }

                    dgdReceive.Items.Refresh();
                }
            }
        }
    }




    class Win_Acc_Receive_Pay_U_CodeView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }
        public bool IsCheck { get; set; }

        public string RPNo { get; set; }
        public string RPGBN { get; set; }
        public string companyid { get; set; }
        public string RPDate { get; set; }
        public string BSItem { get; set; }

        public string RPItemCode { get; set; }
        public string CurrencyUnit { get; set; }
        public string CurrencyUnitName { get; set; }

        public string CustomID { get; set; }
        public string SalesCharge { get; set; }
        public string BankID { get; set; }
        public string BankName { get; set; }

        public string CashAmount { get; set; }
        public string BillAmount { get; set; }
        public string BankAmount { get; set; }
        public string DCAmount { get; set; }
        public string BillNo { get; set; }

        public string VATAmount { get; set; }            // 부가세
        public string ForReceiveBillAmount { get; set; }
        public string ReceiveNowDateYN { get; set; }
        public string cboReceiveNowDateYN { get; set; }  //이게 무슨 짓인지 모르겠지만 일단 추가.


        public string CardAmount { get; set; }
        public string ReceivePersonName { get; set; }
        public string Bank { get; set; } // 은행
        public string Comments { get; set; }

        public string OrderID { get; set; }
        public string RefBSNO { get; set; }
        public string OrderFlag { get; set; }
        public string RefRPItemCode { get; set; }
        public string RefComments { get; set; }

        public string RefAccountYN { get; set; }
        public string RefAmount { get; set; }


        public string KCustom { get; set; }
        public string KCustomName { get; set; }
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
        public double SumTotal { get; set; }
    }
}
