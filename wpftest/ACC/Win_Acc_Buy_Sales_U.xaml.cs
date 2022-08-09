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
using WPF.MDI;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// Win_Acc_Buy_Sales_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Acc_Buy_Sales_U : UserControl
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

        //환율 관련 변수
        double ExchSumAmount = 0; // 공급가액 합계
        double ExchSumVat = 0; // 부가세 합계

        double j = 0;
        double t = 0;

        int BuyListYN = 0;
        string BSGBN = string.Empty;




        public Win_Acc_Buy_Sales_U()
        {
            InitializeComponent();
        }

        // 로드 이벤트.
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            btnToday_Click(null, null);
            chkPeriod.IsChecked = true;
            SetComboBox();
            CanBtnControl();
            DateTime today = DateTime.Now.Date;
            DateTime firstday = today.AddDays(1 - today.Day);
            DateTime lastday = firstday.AddMonths(1).AddDays(-1);
            dtpSDate.SelectedDate = firstday;
            dtpEDate.SelectedDate = lastday;
            date.IsChecked = true;
            tbnOutware.IsChecked = true;  // 로드시 매출버튼 기본선택.
            chkCompany.IsChecked = true;
            cboCompany.SelectedIndex = 0;




        }


        #region (상단 조회조건 체크박스 enable 모음)

        //  매입/매출 토글버튼
        private void tbnOutware_Checked(object sender, RoutedEventArgs e)
        {
            tbnStuffin.IsChecked = false;
            tbnOutware.IsChecked = true;

            // 매출버튼 클릭. > 명칭변경 및 항목, 그리드 체인지.
            tbnOutware_CheckedChange();

            //전체선택 버튼도 체크 해제(초기화)
            chkSelectAll.IsChecked = false;
        }
        //  매입/매출 토글버튼
        private void tbnOutware_Unchecked(object sender, RoutedEventArgs e)
        {
            tbnOutware.IsChecked = false;
            tbnStuffin.IsChecked = true;
        }
        //  매입/매출 토글버튼
        private void tbnStuffin_Checked(object sender, RoutedEventArgs e)
        {
            tbnOutware.IsChecked = false;
            tbnStuffin.IsChecked = true;

            // 매입버튼 클릭. > 명칭변경 및 항목, 그리드 체인지.
            tbnStuffin_CheckedChange();

            //전체선택 버튼도 체크 해제(초기화)
            chkSelectAll.IsChecked = false;
        }
        //  매입/매출 토글버튼
        private void tbnStuffin_Unchecked(object sender, RoutedEventArgs e)
        {
            tbnStuffin.IsChecked = false;
            tbnOutware.IsChecked = true;
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
        private void lblBSItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkBSItems.IsChecked == true) { chkBSItems.IsChecked = false; }
            else { chkBSItems.IsChecked = true; }
        }
        // 매출항목
        private void chkBSItems_Checked(object sender, RoutedEventArgs e)
        {
            txtBSItems.IsEnabled = true;
            btnPfBSItems.IsEnabled = true;
            txtBSItems.Focus();
        }
        // 매출항목
        private void chkBSItems_Unchecked(object sender, RoutedEventArgs e)
        {
            txtBSItems.IsEnabled = false;
            btnPfBSItems.IsEnabled = false;
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
        

        #endregion

        #region (콤보박스 세팅) SetComboBox
        private void SetComboBox()
        {
            var WinAccBuySale = new Win_Acc_Buy_Sales_U_CodeView();

            //매입,매출 화폐단위(입력)
            List<string[]> listPrice = new List<string[]>();
            string[] Price01 = new string[] { "0", "₩" };
            string[] Price02 = new string[] { "1", "$" };
            string[] Price03 = new string[] { "2", "EUR" };
            listPrice.Add(Price01);
            listPrice.Add(Price02);
            listPrice.Add(Price03);

            ObservableCollection<CodeView> ovcCurrencyUnit = ComboBoxUtil.Instance.Direct_SetComboBox(listPrice);
            this.cboCurrencyUnitName.ItemsSource = ovcCurrencyUnit; 
            this.cboCurrencyUnitName.DisplayMemberPath = "code_name";
            this.cboCurrencyUnitName.SelectedValuePath = "code_id";


            // 부가세 별도.
            List<string[]> strValueYN0 = new List<string[]>();
            strValueYN0.Add(new string[] { "Y", "Y" });
            strValueYN0.Add(new string[] { "N", "N" });
            strValueYN0.Add(new string[] { "0", "0" });

            ObservableCollection<CodeView> ovcVatINDYN = ComboBoxUtil.Instance.Direct_SetComboBox(strValueYN0);
            this.cboVATAmountYN.ItemsSource = ovcVatINDYN; 
            this.cboVATAmountYN.DisplayMemberPath = "code_name";
            this.cboVATAmountYN.SelectedValuePath = "code_id";

            //사업장 가져오기 
            ObservableCollection<CodeView> cboCompanyList = Direct_SetComboBoxCompany();
            this.cboCompanyName.ItemsSource = cboCompanyList;
            this.cboCompanyName.DisplayMemberPath = "code_name";
            this.cboCompanyName.SelectedValuePath = "code_id";

            //매출사업장 가져오기 
            ObservableCollection<CodeView> chkCompanyList = Direct_SetComboBoxCompany();
            this.cboCompany.ItemsSource = chkCompanyList;
            this.cboCompany.DisplayMemberPath = "code_name";
            this.cboCompany.SelectedValuePath = "code_id";


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




            //금액은 쓸 수 없도록 해. >> 수량 * 단가로 자동도출할거야.
            txtAmount.IsEnabled = false;
            // 합계금액도 쓸 수 없도록. >> 금액 + (부가세) = 합계금액.
            txtTotalAmount.IsEnabled = false; 

            dgdOutGrid.IsHitTestVisible = true;

            InsertOrUpdate = string.Empty;

            //lblMsg.Visibility = Visibility.Hidden;
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
           
            
        }

        private void CanText()
        {
            dtpBSDate.IsEnabled = true;
            btnpfBSItemname.IsEnabled = true;
            btnpfOrderNo.IsEnabled = true;
            btnpfCustomNat.IsEnabled = true;
            btnpfArticle.IsEnabled = true;
            grbbtnpfSalesman.IsEnabled = true;
            cboCurrencyUnitName.IsEnabled = true;
            txtUnitPrice.IsEnabled = true;
            cboVATAmountYN.IsEnabled = true;
            txtQTY.IsEnabled = true;
            txtAmount.IsEnabled = true;
            txtVATAmount.IsEnabled = true;
            txtTotalAmount.IsEnabled = true;
            txtComments.IsEnabled = true;


        }
        #endregion

        #region (플러스파인더 호출묶음) PlusFinder

        // **상단 플러스파인더**

        // 거래처
        private void btnPfCustom_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
        }

        //매입매출항목
        private void btnPfBSItems_Click(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                //25번. 매출아이템.
                MainWindow.pf.ReturnCode(txtBSItems, (int)Defind_CodeFind.DCF_BSItemCode, "2");
            }
            else if (tbnStuffin.IsChecked == true)
            {
                //26번. 매입아이템.
                MainWindow.pf.ReturnCode(txtBSItems, (int)Defind_CodeFind.DCF_BSItemCode, "1");
            }
        }

        //영업사원
        private void btnPfSalesman_Click(object sender, RoutedEventArgs e)
        {
           MainWindow.pf.ReturnCode(txtBSItems, (int)Defind_CodeFind.DCF_SalesCharge, "");
           
         }

        //품명
        private void btnPfArticle_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtArticle, (int)Defind_CodeFind.DCF_Article, "");
        }


        // **오른쪽 플러스파인더**

        //매입매출항목
        private void btnpfBSItemname_Click(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                //25번. 매출아이템.
                MainWindow.pf.ReturnCode(txtBSItemname, (int)Defind_CodeFind.DCF_BSItemCode, "2");
            }
            else if (tbnStuffin.IsChecked == true)
            {
                //26번. 매입아이템.
                MainWindow.pf.ReturnCode(txtBSItemname, (int)Defind_CodeFind.DCF_BSItemCode, "1");
            }
        }

        //오더번호
        private void btnpfOrderNo_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtOrderNo, (int)Defind_CodeFind.DCF_ORDER, "");

        }

        //매입매출거래처
        private void btnpfCustomNat_Click(object sender, RoutedEventArgs e)
        {

            MainWindow.pf.ReturnCode(txtCustomNat, (int)Defind_CodeFind.DCF_CUSTOM, "");
           
 
           
        }

        //품명
        private void BtnpArticle_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtArticle2, (int)Defind_CodeFind.DCF_Article, "");
        }

        //영업사원
        private void grbBtnpSalesman_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtSalesman2, (int)Defind_CodeFind.DCF_SalesCharge, "");

        }

        #endregion

        #region (토글버튼 체크 체인지 이벤트) CheckedChange
        // 매출 클릭.
        private void tbnOutware_CheckedChange()
        {
            if (dgSum.Items.Count > 0)
            {
                dgSum.Items.Clear();
            }
            this.DataContext = null;

            tbkBSItem.Text = "매출항목";
            LbBSItemname.Content = "매출항목";
            grbSaleAdd.Content = "매출등록";
            colSumTotalAmount.Header = "매출량";
            LbBasisYearMon.Content = "매출월";

            grbdgdInGrid.Visibility = Visibility.Hidden;
            grbdgdOutGrid.Visibility = Visibility.Visible;

        }

        // 매입 클릭.
        private void tbnStuffin_CheckedChange()
        {
            if (dgSum.Items.Count > 0)
            {
                dgSum.Items.Clear();
            }

            this.DataContext = null;

            tbkBSItem.Text = "매입항목";
            grbSaleAdd.Content = "매입등록";
            LbBSItemname.Content = "매입항목";
            colSumTotalAmount.Header = "매입량";
            LbBasisYearMon.Content = "매입월";

            grbdgdOutGrid.Visibility = Visibility.Hidden;
            grbdgdInGrid.Visibility = Visibility.Visible;
            sellMM.Visibility = Visibility.Hidden;
            buyMM.Visibility = Visibility.Visible;
            //txtblockSearchTotalOut.Refresh();

        }

        #endregion



        // 검색버튼 클릭.
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            cboCurrencyUnitName.SelectedIndex = 0;
            cboVATAmountYN.SelectedIndex = 0;
            cboCompanyName.SelectedIndex = 0;

            grbSaleAdd.IsEnabled = false;

            if (tbnOutware.IsChecked == true) //매출버튼
            {
                FillGrid_OutGrid();
                FillGrid_SumGrid();
            }
            else if (tbnStuffin.IsChecked == true)  //매입버튼
            {
                FillGrid_InGrid();
                FillGrid_SumGrid();
            }


        }

        private void FillGrid_SumGrid()
        {
            chkSelectAll.IsChecked = false;

            if (dgSum.Items.Count > 0)
            {
                dgSum.Items.Clear();
            }

            try
            {
                // 매입 / 매출 토글박스 구분.

                
                if (tbnOutware.IsChecked == true)
                {
                    BSGBN = "2";
                } else if (tbnStuffin.IsChecked == true)
                {
                    BSGBN = "1";
                }
                

                // 매출월/ 발생일 체크 
                int sBSDate = 0;
                if (date.IsChecked == true) { sBSDate = 1; }
                else if (sellMM.IsChecked == true) { sBSDate = 2; }

                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("bsGbn", BSGBN);
                sqlParameter.Add("sBSDate", sBSDate);
                sqlParameter.Add("sDate", sBSDate == 1 ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : dtpSDate2.SelectedDate.Value.ToString("yyyyMM"));
                sqlParameter.Add("eDate", sBSDate == 1 ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : dtpEDate2.SelectedDate.Value.ToString("yyyyMM"));
                sqlParameter.Add("CompanyID", chkCompany.IsChecked == true ? cboCompany.SelectedValue.ToString() : "");

                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_BS_Q2_WPF", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("조회된 데이터가 없습니다."); 
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;
                        foreach (DataRow dr in drc)
                        {
                            var WinAccBuySale = new Win_Acc_Buy_Sales_U_CodeView()
                            {
                                Num = i + 1,
                                IsCheck = false,

                                Count = stringFormatN0(dr["건수"]),
                                SumQty = stringFormatN0(dr["수량"]),
                                SumTotalAmount = stringFormatN0(dr["매출량"]),
                                Sumdollar = stringFormatN0(dr["￦"])
                                
                            };
                           
                            // 콤마입히기 > 금액
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.AMOUNT))
                            {
                                WinAccBuySale.AMOUNT = Lib.Instance.returnNumStringZero(WinAccBuySale.AMOUNT);
                            }
                            // 콤마입히기 > 부가세
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.VATAmount))
                            {
                                WinAccBuySale.VATAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.VATAmount);
                            }
                            // 콤마입히기 > 합계금
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.TotalAmount))
                            {
                                WinAccBuySale.TotalAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.TotalAmount);
                            }


                            dgSum.Items.Add(WinAccBuySale);
                            i++;
                        }

                        //txtblockSearchTotalOut.Text = "      합계 : " + i.ToString() + "건";
                        //txtblockSearchAmountOut.Text = "금액 : " + stringFormatN0(SumOut.SumAmount) + "원";
                        //txtblockSearchVATOut.Text = "부가세 : " + stringFormatN0(SumOut.SumVatAmount) + "원";
                        //txtblockSearchTotalOut.Text = "합계금액 : " + stringFormatN0(SumOut.SumTotalAmount) +"원";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

    

    #region (매출용 그리드 채우기) FillGrid_OutGrid
    // 매출용 그리드 채우기.
    private void FillGrid_OutGrid()
        {

            //전체선택 버튼도 체크 해제(초기화)
            chkSelectAll.IsChecked = false;

            //환율 관련 변수들을 리셋 시켜주고 시작
            ExchSumAmount = 0;
            ExchSumVat = 0;
            j = 0;
            t = 0;

            //합계변수
            var SumOut = new Win_Acc_Buy_Sales_U_CodeView_Sum();



            if (dgdOutGrid.Items.Count > 0)
            {
                dgdOutGrid.Items.Clear();
            }

            if (dgSum.Items.Count > 0)
            {
                dgSum.Items.Clear();
            }

            try
            {
                // 매입 / 매출 토글박스 구분.
                string BSGBN = "2";

                // 매출월/ 발생일 체크 
                int sBSDate = 0;
                if (date.IsChecked == true) { sBSDate = 1; }
                else if (sellMM.IsChecked == true) { sBSDate = 2; }

                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("bsGbn", BSGBN);
                sqlParameter.Add("sBSDate", sBSDate);
                sqlParameter.Add("sDate", sBSDate == 1 ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : dtpSDate2.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("eDate", sBSDate == 1 ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : dtpEDate2.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("CompanyID", chkCompany.IsChecked == true ? cboCompany.SelectedValue.ToString() : "");

                sqlParameter.Add("CustomID", chkCustom.IsChecked == true ? txtCustom.Tag.ToString() : "");
                sqlParameter.Add("BSItemCode", chkBSItems.IsChecked == true ? txtBSItems.Tag.ToString() : "");
                sqlParameter.Add("ArticleID", chkArticle.IsChecked == true ? txtArticle.Tag.ToString() : "");
                sqlParameter.Add("Article", chkArticle.IsChecked == true ? txtArticle.Text.ToString() : "");
            

                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_BS_Q_WPF", sqlParameter, false);
                
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("조회된 데이터가 없습니다.");
                        //txtblockSearchCountOut.Text = "      합계 :"; ㅇㅇㅇ
                        //txtblockSearchAmountOut.Text = "금액 :";
                        //txtblockSearchVATOut.Text = "부가세  :";
                        //txtblockSearchTotalOut.Text = "합계금액 :";
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;
                        foreach (DataRow dr in drc)
                        {
                            var WinAccBuySale = new Win_Acc_Buy_Sales_U_CodeView()
                            {
                                Num = i + 1,
                                IsCheck = false,

                                //BSNo = dr["BSNo"].ToString(),
                                BSDate = dr["BSDate"].ToString(),
                                //BSDate = DateTime.ParseExact(dr["BSDate"].ToString(), "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                BSNo = dr["BSNo"].ToString(),
                                BSItemname = dr["BSItemname"].ToString(),
                                Article = dr["Article"].ToString(),
                                OrderID = dr["OrderID"].ToString(),
                                CurrencyUnitName = dr["CurrencyUnitName"].ToString(),
                                CustomNat = dr["CustomNat"].ToString(),
                                SalesChargeName = dr["SalesChargeName"].ToString(),
                                CustomShort = dr["CustomShort"].ToString(),
                                QTY = stringFormatN0(dr["QTY"]),
                                UnitPrice = stringFormatN0(dr["UnitPrice"]),
                                AMOUNT = stringFormatN0(dr["Amount"]),
                                VATAmount = stringFormatN0(dr["VATAmount"]),
                                INOUTNO = dr["InOutNo"].ToString(),
                                TotalAmount = stringFormatN0(dr["TotalAmount"]),
                                Color = dr["Color"].ToString(),
                                CompanyName = dr["CompanyName"].ToString(),
                                BasisYearMon = dr["BasisYearMon"].ToString(),
                                taxbillYN = dr["taxbillYN"].ToString(),
                                BSItem = dr["BSItem"].ToString(),
                                ArticleID = dr["ArticleID"].ToString(),
                                Comments = dr["comments"].ToString(),
                                CompanyID = dr["CompanyID"].ToString(),
                                CustomID = dr["CustomID"].ToString(),
                                BSGBN = dr["BSGBN"].ToString()
                                

                               
                            };
                            // 화폐단위
                            //if (WinAccBuySale.CurrencyUnit.Trim().Equals("0"))
                            //{
                            //    WinAccBuySale.CurrencyUnitName = "₩";
                            //}
                            //else
                            //{
                            //    WinAccBuySale.CurrencyUnitName = "$";
                            //}

                            //공급가액의 합계
                            double.TryParse(WinAccBuySale.AMOUNT, out j);
                            ExchSumAmount += j;

                            //부가세의 합계
                            double.TryParse(WinAccBuySale.VATAmount, out t);
                            ExchSumVat += t;

                            SumOut.SumAmount += ConvertDouble(WinAccBuySale.AMOUNT);
                            SumOut.SumVatAmount += ConvertDouble(WinAccBuySale.VATAmount);
                            SumOut.SumTotalAmount += ConvertDouble(WinAccBuySale.TotalAmount);

                            //// 콤마입히기 > 절수
                            //if (Lib.Instance.IsNumOrAnother(WinAccBuySale.RollQty))
                            //{
                            //    WinAccBuySale.RollQty = Lib.Instance.returnNumStringZero(WinAccBuySale.RollQty);
                            //}
                            //// 콤마입히기 > 수량
                            //if (Lib.Instance.IsNumOrAnother(WinAccBuySale.QTY))
                            //{
                            //    WinAccBuySale.QTY = Lib.Instance.returnNumStringZero(WinAccBuySale.QTY);
                            //}
                            //// 콤마입히기 > 단가
                            //if (Lib.Instance.IsNumOrAnother(WinAccBuySale.UnitPrice))
                            //{
                            //    WinAccBuySale.UnitPrice = Lib.Instance.returnNumStringZero(WinAccBuySale.UnitPrice);
                            //}
                            // 콤마입히기 > 금액
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.AMOUNT))
                            {
                                WinAccBuySale.AMOUNT = Lib.Instance.returnNumStringZero(WinAccBuySale.AMOUNT);
                            }
                            // 콤마입히기 > 부가세
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.VATAmount))
                            {
                                WinAccBuySale.VATAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.VATAmount);
                            }
                            // 콤마입히기 > 합계금
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.TotalAmount))
                            {
                                WinAccBuySale.TotalAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.TotalAmount);
                            }
                            

                            dgdOutGrid.Items.Add(WinAccBuySale);
                            i++;
                        }

                        //txtblockSearchTotalOut.Text = "      합계 : " + i.ToString() + "건";
                        //txtblockSearchAmountOut.Text = "금액 : " + stringFormatN0(SumOut.SumAmount) + "원";
                        //txtblockSearchVATOut.Text = "부가세 : " + stringFormatN0(SumOut.SumVatAmount) + "원";
                        //txtblockSearchTotalOut.Text = "합계금액 : " + stringFormatN0(SumOut.SumTotalAmount) +"원";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion

        #region (매입용 그리드 채우기) FillGrid_InGrid
        private void FillGrid_InGrid()
        {
            //전체선택 버튼도 체크 해제(초기화)
            chkSelectAll.IsChecked = false;

            //환율 관련 변수들을 리셋 시켜주고 시작
            ExchSumAmount = 0;
            ExchSumVat = 0;
            j = 0;
            t = 0;

            //합계변수
            var SumIn = new Win_Acc_Buy_Sales_U_CodeView_Sum();

            if (dgdInGrid.Items.Count > 0)
            {
                dgdInGrid.Items.Clear();
            }
            if (dgSum.Items.Count > 0)
            {
                dgSum.Items.Clear();
            }

            try
            {
                // 매입 / 매출 토글박스 구분.
                string BSGBN = "1";

                // 기간 체크여부 yn.
                int sBSDate = 0;
                if (date.IsChecked == true) { sBSDate = 1; }
                else if (sellMM.IsChecked == true) { sBSDate = 2; }



                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("bsGbn", BSGBN);
                sqlParameter.Add("sBSDate", sBSDate);
                sqlParameter.Add("sDate", sBSDate == 1 ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : dtpSDate2.SelectedDate.Value.ToString("yyyyMM"));
                sqlParameter.Add("eDate", sBSDate == 1 ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : dtpEDate2.SelectedDate.Value.ToString("yyyyMM"));
                sqlParameter.Add("CompanyID", chkCompany.IsChecked == true ? cboCompany.SelectedValue.ToString() : "");

                sqlParameter.Add("CustomID", chkCustom.IsChecked == true ? txtCustom.Tag.ToString() : "");
                sqlParameter.Add("BSItemCode", chkBSItems.IsChecked == true ? txtBSItems.Tag.ToString() : "");
                sqlParameter.Add("ArticleID", chkArticle.IsChecked == true ? txtArticle.Tag.ToString() : "");
                sqlParameter.Add("Article", chkArticle.IsChecked == true ? txtArticle.Tag.ToString() : "");


                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_BS_Q_WPF", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("조회된 데이터가 없습니다.");
                        //txtblockSearchCountIn.Text = "      합계 :";
                        //txtblockSearchAmountIn.Text = "금액 :";
                        //txtblockSearchVATIn.Text = "부가세 :"; 
                        //txtblockSearchTotalIn.Text = "합계금액 :";
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;
                        foreach (DataRow dr in drc)
                        {
                            var WinAccBuySale = new Win_Acc_Buy_Sales_U_CodeView()
                            {
                                Num = i + 1,
                                chkC = false,

                               
                                BSDate = DateTime.ParseExact(dr["BSDate"].ToString(), "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                BSNo = dr["BSNo"].ToString(),
                                BSItemname = dr["BSItemname"].ToString(),
                                Article = dr["Article"].ToString(),
                                OrderID = dr["OrderID"].ToString(),
                                CurrencyUnitName = dr["CurrencyUnitName"].ToString(),
                                CustomNat = dr["CustomNat"].ToString(),
                                SalesChargeName = dr["SalesChargeName"].ToString(),
                                CustomShort = dr["CustomShort"].ToString(),
                                QTY = stringFormatN0(dr["QTY"]),
                                UnitPrice = stringFormatN0(dr["UnitPrice"]),
                                AMOUNT = stringFormatN0(dr["Amount"]),
                                VATAmount = stringFormatN0(dr["VATAmount"]),
                                INOUTNO = dr["InOutNo"].ToString(),
                                TotalAmount = stringFormatN0(dr["TotalAmount"]),
                                Color = dr["Color"].ToString(),
                                CompanyName = dr["CompanyName"].ToString(),
                                BasisYearMon = dr["BasisYearMon"].ToString(),
                                taxbillYN = dr["taxbillYN"].ToString(),
                                BSItem = dr["BSItem"].ToString(),
                                ArticleID = dr["ArticleID"].ToString(),
                                Comments = dr["comments"].ToString(),
                                CompanyID = dr["CompanyID"].ToString(),
                                CustomID = dr["CustomID"].ToString()

                            };
                            //// 화폐단위
                            //if (WinAccBuySale.CurrencyUnit.Trim().Equals("0"))
                            //{
                            //    WinAccBuySale.CurrencyUnitName = "₩";
                            //}
                            //else
                            //{
                            //    WinAccBuySale.CurrencyUnitName = "$";
                            //}


                            SumIn.SumAmount += ConvertDouble(WinAccBuySale.AMOUNT);
                            SumIn.SumVatAmount += ConvertDouble(WinAccBuySale.VATAmount);
                            SumIn.SumTotalAmount += ConvertDouble(WinAccBuySale.TotalAmount);

                            //공급가액의 합계
                            double.TryParse(WinAccBuySale.AMOUNT, out j);
                            ExchSumAmount += j;

                            //부가세의 합계
                            double.TryParse(WinAccBuySale.VATAmount, out t);
                            ExchSumVat += t;


                            //// 콤마입히기 > 절수
                            //if (Lib.Instance.IsNumOrAnother(WinAccBuySale.RollQty))
                            //{
                            //    WinAccBuySale.RollQty = Lib.Instance.returnNumStringZero(WinAccBuySale.RollQty);
                            //}
                            //// 콤마입히기 > 수량
                            //if (Lib.Instance.IsNumOrAnother(WinAccBuySale.QTY))
                            //{
                            //    WinAccBuySale.QTY = Lib.Instance.returnNumStringZero(WinAccBuySale.QTY);
                            //}
                            //// 콤마입히기 > 단가
                            //if (Lib.Instance.IsNumOrAnother(WinAccBuySale.UnitPrice))
                            //{
                            //    WinAccBuySale.UnitPrice = Lib.Instance.returnNumStringZero(WinAccBuySale.UnitPrice);
                            //}
                            // 콤마입히기 > 금액
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.AMOUNT))
                            {
                                WinAccBuySale.AMOUNT = Lib.Instance.returnNumStringZero(WinAccBuySale.AMOUNT);
                            }
                            // 콤마입히기 > 부가세
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.VATAmount))
                            {
                                WinAccBuySale.VATAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.VATAmount);
                            }
                            // 콤마입히기 > 합계금
                            if (Lib.Instance.IsNumOrAnother(WinAccBuySale.TotalAmount))
                            {
                                WinAccBuySale.TotalAmount = Lib.Instance.returnNumStringZero(WinAccBuySale.TotalAmount);
                            }

                            dgdInGrid.Items.Add(WinAccBuySale);
                            i++;
                        }

                        //txtblockSearchTotalOut.Text = "      검색건수 : " + i.ToString() + "건";
                        //txtblockSearchAmountIn.Text = "금액 : " + stringFormatN0(SumIn.SumAmount) + "원";
                        //txtblockSearchVATIn.Text = "부가세 : " + stringFormatN0(SumIn.SumVatAmount) + "원";
                        //txtblockSearchTotalIn.Text = "합계금액 : " + stringFormatN0(SumIn.SumTotalAmount) + "원";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion



        //// 추가버튼 클릭.
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            CantBtnControl();
            CanText();
            dtpBSDate.SelectedDate = DateTime.Today;
            cboCurrencyUnitName.SelectedIndex = 0;
            cboVATAmountYN.SelectedIndex = 0;
            cboCompanyName.SelectedIndex = 0;
            txtTotalAmount.IsEnabled = false;
            txtAmount.IsEnabled = false;

            grbSaleAdd.IsEnabled = true;
            
            
            InsertOrUpdate = "I"; 
            dtpBasisYearMon.SelectedDate = DateTime.Today;

            // 첫 포커스 자동설정.
            //txtgrbSaleItems.Focus();
        }

        // 수정버튼 클릭.
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var WinAccBuySale = dgdOutGrid.SelectedItem as Win_Acc_Buy_Sales_U_CodeView;
            this.DataContext = WinAccBuySale;
            CantBtnControl();
            CanText();
            InsertOrUpdate = "U";
            dtpBSDate.SelectedDate = DateTime.Today;
            cboCurrencyUnitName.SelectedIndex = 0;
            cboVATAmountYN.SelectedIndex = 0;
            cboCompanyName.SelectedIndex = 0;
            grbSaleAdd.IsEnabled = true;
            btnpfBSItemname.IsEnabled = true;
            txtTotalAmount.IsEnabled = false;
            txtAmount.IsEnabled = false;


        }

        #region (텍스트 박스 숫자만 들어가게끔) PreviewTextInput
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 수량 )
        private void txtgrbQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 단가 )
        private void txtgrbUnitPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 금액 )
        private void txtgrbAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 부가세 )
        private void txtgrbVATAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }
        // 프리뷰 인풋. 숫자만 들어가지도록.  ( 합계금액 )
        private void txtgrbTotalAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Lib.Instance.CheckIsNumericOnly((TextBox)sender, e);
        }

        #endregion

        #region 금액 / 합계금액 자동도출 관련
        //// 텍스트 체인지..수량. (수량 * 단가 = 금액) 이거하니까 문자열 오류남 
        //private void txtQTY_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (txtUnitPrice.Text != string.Empty && txtQTY.Text != string.Empty)
        //    {
        //        int A = Convert.ToInt32(txtQTY.Text);
        //        int B = Convert.ToInt32(txtUnitPrice.Text);

        //        int C = A * B;
        //        txtAmount.Text = C.ToString();
        //    }
        //}
        // 텍스트 체인지..단가. (수량 * 단가 = 금액)
        private void txtgrbUnitPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtUnitPrice.Text != string.Empty && txtQTY.Text != string.Empty)
            {
                double A = Convert.ToDouble(txtQTY.Text);
                double B = Convert.ToDouble(txtUnitPrice.Text);

                double C = A * B;
                txtAmount.Text = C.ToString();
            }
        }
        // 텍스트 체인지.. 부가세. (금액 + 부가세 = 합계금액)
        private void txtgrbVATAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cboVATAmountYN.SelectedValue.ToString() == "Y")
            {
                if (txtAmount.Text != string.Empty)
                {
                    txtTotalAmount.Text = txtAmount.Text;
                }
            }
            else
            {
                if (txtVATAmount.Text != string.Empty && txtAmount.Text != string.Empty)
                {
                    int A = Convert.ToInt32(txtVATAmount.Text);
                    int B = Convert.ToInt32(txtAmount.Text);

                    int C = A + B;
                    txtTotalAmount.Text = C.ToString();
                }
            }
        }
        // (부가세별도) 콤보박스 셀렉션 체인지 이벤트.
        private void cboVatINDYN_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboVATAmountYN.SelectedValue.ToString() == "Y")
            {
                // 부가세 별도 YES.
                if (txtAmount.Text != string.Empty)
                {
                    txtTotalAmount.Text = txtAmount.Text;
                }
            }
            else if (cboVATAmountYN.SelectedValue.ToString() == "N")
            {
                // 부가세 별도 NO.
                if (txtVATAmount.Text != string.Empty && txtAmount.Text != string.Empty)
                {
                    int A = Convert.ToInt32(txtVATAmount.Text);
                    int B = Convert.ToInt32(txtAmount.Text);

                    int C = A + B;
                    txtTotalAmount.Text = C.ToString();
                }
            }
        }

        #endregion



        // 삭제버튼 클릭.
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int D_Check = 0;
            if (tbnOutware.IsChecked == true)
            {
                foreach (Win_Acc_Buy_Sales_U_CodeView Win_Acc_Buy_Sales in dgdOutGrid.Items)
                {
                    if (Win_Acc_Buy_Sales != null)
                    {
                        if (Win_Acc_Buy_Sales.IsCheck == true)
                        {
                            D_Check++;
                        }
                    }
                }
            }
            else if (tbnStuffin.IsChecked == true)
            {
                foreach (Win_Acc_Buy_Sales_U_CodeView Win_Acc_Buy_Sales in dgdInGrid.Items)
                {
                    if (Win_Acc_Buy_Sales != null)
                    {
                        if (Win_Acc_Buy_Sales.IsCheck == true)
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
                    if (tbnOutware.IsChecked == true)
                    {
                        BSGBN = "2";
                        foreach (Win_Acc_Buy_Sales_U_CodeView Win_Acc_Buy_Sales in dgdOutGrid.Items)
                        {
                            if (Win_Acc_Buy_Sales != null)
                            {
                                if (Win_Acc_Buy_Sales.IsCheck == true)
                                {
                                    Delete_Data(Win_Acc_Buy_Sales.BSNo, BSGBN);
                                }

                            }

                        }

                        FillGrid_OutGrid();     // 재검색.
                    }
                    else if (tbnStuffin.IsChecked == true)
                    {
                        BSGBN = "1";
                        foreach (Win_Acc_Buy_Sales_U_CodeView Win_Acc_Buy_Sales in dgdInGrid.Items)
                        {
                            if (Win_Acc_Buy_Sales != null)
                            {
                                    if (Win_Acc_Buy_Sales.IsCheck == true)
                                    {
                                        Delete_Data(Win_Acc_Buy_Sales.BSNo, BSGBN);
                                    }

                            }
                        }

                        FillGrid_InGrid();     // 재검색.
                    }
                }
            }
        }

        #region (삭제로직) Delete_Data
        // 삭제로직.
        private bool Delete_Data(string BSNo, string BSGBN)
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {

                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("BSNo", txtBSNo.Text.ToString());
                sqlParameter.Add("BSGBN", BSGBN);


                Procedure pro1 = new Procedure();
                pro1.Name = "xp_Acc_BS_DAccBuySales";
                pro1.OutputUseYN = "N";
                pro1.OutputName = "BSNo";
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
                    //MessageBox.Show("결재등록된 항목을 제외하고 삭제가 완료되었습니다.");
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
                    if (tbnOutware.IsChecked == true)
                    {
                        FillGrid_OutGrid();
                    }
                    else if (tbnStuffin.IsChecked == true)
                    {
                        FillGrid_InGrid();
                    }
                }
            }
        }

        #region(저장 전, 필수입력 칸 입력여부 체크) Check_EssentialData
        private bool Check_EssentialData()
        {
            bool Flag = true;
            var WinAccBuySale = dgdOutGrid.SelectedItem as Win_Acc_Buy_Sales_U_CodeView;
            this.DataContext = WinAccBuySale;

            if (txtBSItemname.Text == null || txtBSItemname.Text.Length <= 0)
            {
                MessageBox.Show("매출항목이 입력되지 않았습니다. 먼저 매출항목을 입력해주세요");
                Flag = false;
                return Flag;
            }
            if (txtCustomNat.Text == null || txtCustomNat.Text.Length <= 0)
            {
                MessageBox.Show("거래처가 입력되지 않았습니다. 먼저 거래처를 입력해주세요");
                Flag = false;
                return Flag;
            }
            if (cboCurrencyUnitName.SelectedValue == null)
            {
                MessageBox.Show("화폐단위가 입력되지 않았습니다. 먼저 화폐단위를 입력해주세요");
                Flag = false;
                return Flag;
            }
            if (cboVATAmountYN.SelectedValue == null)
            {
                MessageBox.Show("부가세 별도여부가 입력되지 않았습니다. 먼저 부가세 별도여부를 입력해주세요");
                Flag = false;
                return Flag;
            }
            if (txtUnitPrice.Text == string.Empty || txtUnitPrice.Text.Length <= 0)
            {
                MessageBox.Show("단가가 입력되지 않았습니다. 먼저 단가를 입력해주세요");
                Flag = false;
                return Flag;
            }
            if (txtQTY.Text == string.Empty || txtQTY.Text.Length <= 0)
            {
                MessageBox.Show("수량이 입력되지 않았습니다. 먼저 수량을 입력해주세요");
                Flag = false;
                return Flag;
            }
            if (txtAmount.Text == string.Empty || txtAmount.Text.Length <= 0)
            {
                MessageBox.Show("금액이 입력되지 않았습니다. 먼저 금액을 입력해주세요");
                Flag = false;
                return Flag;
            }
            if (txtVATAmount.Text == string.Empty || txtVATAmount.Text.Length <= 0)
            {
                MessageBox.Show("부가세가 입력되지 않았습니다. 먼저 부가세를 입력해주세요");
                Flag = false;
                return Flag;
            }
            if (txtTotalAmount.Text == string.Empty || txtTotalAmount.Text.Length <= 0)
            {
                MessageBox.Show("총액이 입력되지 않았습니다. 먼저 총액을 입력해주세요");
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
                string BSNo = string.Empty;                                 // 관리번호(P-KEY)
                string BSGBN = string.Empty;
                if (tbnOutware.IsChecked == true) { BSGBN = "2"; }          //매출
                else if (tbnStuffin.IsChecked == true) { BSGBN = "1"; }     //매입


                if (BSGBN == "2")
                {
                    var WinAccBuySale = dgdOutGrid.SelectedItem as Win_Acc_Buy_Sales_U_CodeView;
                    //BSNo = WinAccBuySale.BSNo;
                }
                else if (BSGBN == "1")
                {
                    var WinAccBuySale = dgdInGrid.SelectedItem as Win_Acc_Buy_Sales_U_CodeView;
                    //BSNo = WinAccBuySale.BSNo;
                }

                if (tbkBSItem.Tag == null || tbkBSItem.Text.Length <= 0)
                {
                    tbkBSItem.Tag = (object)"";
                }

                int I_UnitPrice = 0;
                double D_QTY = 0;
                double D_Amount = 0;
                double D_VATAmount = 0;
                double D_TotalAmount = 0;

                int.TryParse(txtUnitPrice.Text, out I_UnitPrice);
                double.TryParse(txtQTY.Text, out D_QTY);
                double.TryParse(txtAmount.Text, out D_Amount);
                double.TryParse(txtVATAmount.Text, out D_VATAmount);
                double.TryParse(txtTotalAmount.Text, out D_TotalAmount);


                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("BSGBN", BSGBN);                                                   // 매입 / 매출 구분                                            
                sqlParameter.Add("sBSItemCode", txtBSItemname.Tag.ToString());
                sqlParameter.Add("sInOutWareNo", txtINOUTNO.Text);                 
                sqlParameter.Add("sInOutDate", dtpBSDate.SelectedDate.Value.ToString("yyyyMMdd"));        // 일자.
                sqlParameter.Add("sCompanyID", cboCompanyName.SelectedValue.ToString());        

                sqlParameter.Add("sCustomID", txtCustomNat.Tag.ToString());        
                sqlParameter.Add("sCurrencyUnit", cboCurrencyUnitName.SelectedValue.ToString());                          // 거래처.
                sqlParameter.Add("sSales_Charge", "");         // 화폐단위
                sqlParameter.Add("nUnitPrice", I_UnitPrice);                                         // 단가
                sqlParameter.Add("nQTY", D_QTY);                                                     // 수량

                sqlParameter.Add("nAmount", D_Amount);                                                // 금액
                sqlParameter.Add("OrderID", txtOrderNo.Text.ToString());                                                // 금액
                sqlParameter.Add("ArticleID", txtArticle2.Tag.ToString());                        // 품명 id
                sqlParameter.Add("Article", txtArticle2.Text);                                    // 품명
                sqlParameter.Add("nVATAmount", D_VATAmount);                                         // 부가세

                sqlParameter.Add("nTotalAmount", D_TotalAmount);                                     // 토탈금액
                sqlParameter.Add("sComments", txtComments.Text);                                  // 비고
                sqlParameter.Add("sCreateUserID", "");                          // 생성자 id.
                sqlParameter.Add("sVat_IND_YN", cboVATAmountYN.SelectedValue.ToString());                       
                sqlParameter.Add("OutMsg", "");                   
               




                Procedure pro1 = new Procedure();
                if (InsertOrUpdate == "I")
                {
                    pro1.Name = "xp_Acc_BS_iAccBuySales_WPF";
                    pro1.OutputUseYN = "N";
                    pro1.OutputName = "";
                    pro1.OutputLength = "10";
                }
                else
                {
                    sqlParameter.Add("BSNo", txtBSNo.Text.ToString());
                    sqlParameter.Add("tbntaxbillYN", tbntaxbillY.IsChecked == true ? "Y" : "N");

                    pro1.Name = "xp_Acc_BS_uAccBuySales_WPF";
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
                FillGrid_OutGrid();
            }

            // 닫기버튼 클릭.
            private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }



        // 데이터 그리드 항목 클릭_ SelectionChanged
        private void dgdOutGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                var WinAccBuySale = dgdOutGrid.SelectedItem as Win_Acc_Buy_Sales_U_CodeView;
                if (WinAccBuySale != null)
                {
                    this.DataContext = WinAccBuySale;

                    DateTime aa = DateTime.ParseExact(WinAccBuySale.BSDate, "yyyyMMdd", null);
                    dtpBSDate.SelectedDate = aa;

                    //DateTime bb = DateTime.ParseExact(WinAccBuySale.BasisYearMon, "yyyyMM", null);
                    //dtpBasisYearMon.SelectedDate = bb;



                }
            }
        }
        // 데이터 그리드 항목 클릭_ SelectionChanged
        private void dgdInGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbnStuffin.IsChecked == true)
            {
                var WinAccBuySale = dgdInGrid.SelectedItem as Win_Acc_Buy_Sales_U_CodeView;
                if (WinAccBuySale != null)
                {
                    this.DataContext = WinAccBuySale;
                }
            }
        }
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
        // 엑셀버튼 클릭.
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] dgdStr = new string[2];
            if (tbnOutware.IsChecked == true)
            {
                dgdStr[0] = "매출 등록 리스트";
                dgdStr[1] = dgdOutGrid.Name;
            }
            else
            {
                dgdStr[0] = "매입 등록 리스트";
                dgdStr[1] = dgdInGrid.Name;
            }

            ExportExcelxaml ExpExc = new ExportExcelxaml(dgdStr);
            ExpExc.ShowDialog();

            if (ExpExc.DialogResult.HasValue)
            {
                if (ExpExc.choice.Equals(dgdOutGrid.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdOutGrid);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdOutGrid);

                    Name = dgdOutGrid.Name;
                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else if (ExpExc.choice.Equals(dgdInGrid.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdInGrid);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdInGrid);

                    Name = dgdInGrid.Name;
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

        #region 자동 포커스 이동 

        // 화폐단위 선택 후 자동 포커스 이동
        private void cboCurrencyUnit_DropDownClosed(object sender, EventArgs e)
        {
            txtUnitPrice.Focus();
        }
        // 부가세별도 선택 후 자동 포커스 이동
        private void cboVatINDYN_DropDownClosed(object sender, EventArgs e)
        {
            txtQTY.Focus();
        }
        // 수량 입력 후 자동 포커스 이동
        private void txtgrbQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtVATAmount.Focus();
            }
        }
        // 단가 입력 후 자동 포커스 이동
        private void txtVATAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtCustomShort.Focus();
            }
        }
        // 부가세 입력 후 자동 포커스 이동
        private void txtCustomShort_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtComments.Focus();
            }
        }
        // 입출고처 입력 후 자동 포커스 이동
        private void txtComments_KeyDown(object sender, KeyEventArgs e)
        {
            dtpBasisYearMon.Focus();

        }
        #endregion



        //환율 계산하는 이벤트 ㅇㅇㅇㅇ
        //private void BtnExchange_Click(object sender, RoutedEventArgs e)
        //{
        //    환율 버튼을 누르면
        //        string a = txtExchange.Text.ToString().Replace(",", "");

        //    환율 공급가액 텍스트 박스에 환율* 공급가액 합계 대입
        //        txtExchAmount.Text = (ConvertDouble(a) * ExchSumAmount).ToString();

        //    환율 부가세 텍스트 박스에 환율* 부가세 합계를 대입
        //        txtExchVAT.Text = (ConvertDouble(a) * ExchSumVat).ToString();

        //    대입한 값을 각 변수에 대입
        //        string b = txtExchAmount.Text.ToString().Replace(",", "");
        //    string c = txtExchVAT.Text.ToString().Replace(",", "");


        //    환율 환산 합계 텍스트 박스에 공급가액 +부가세를 대입
        //        txtExchToTal.Text = (ConvertDouble(b) + ConvertDouble(c)).ToString();


        //    천단위 구분기호를 넣어서 보여주기
        //        if (txtExchange.Text != null)
        //    {
        //        txtExchange.Text = string.Format("{0:N2}", double.Parse(txtExchange.Text));
        //    }
        //    txtExchAmount.Text = string.Format("{0:N2}", double.Parse(txtExchAmount.Text));
        //    txtExchVAT.Text = string.Format("{0:N2}", double.Parse(txtExchVAT.Text));
        //    txtExchToTal.Text = string.Format("{0:N2}", double.Parse(txtExchToTal.Text));


        //    텍스트 박스에 다 대입 후 0으로 리셋
        //        a = "";
        //    b = "";
        //    c = "";
        //}




        #region 기타 

        //천 단위 구분기호, 소수점 자릿수 0 
        private string stringFormatN0(object obj)
        {
            return string.Format("{0:N0}", obj);
        }

        //더블로 형식 변환
        private double ConvertDouble(string str)
        {
            double result = 0;
            double chkDouble = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Trim().Replace(",", "");

                if (double.TryParse(str, out chkDouble) == true)
                {
                    result = double.Parse(str);
                }
            }

            return result;
        }

        #endregion


        //전체 선택 텍스트 블럭 클릭 이벤트
        private void tbkSelectAll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkSelectAll.IsChecked == true) { chkSelectAll.IsChecked = false; }
            else { chkSelectAll.IsChecked = true; }
        }

        //전체 선택 체크박스 체크 이벤트
        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                if (dgdOutGrid.Items.Count > 0)
                {
                    foreach (Win_Acc_Buy_Sales_U_CodeView WinAccBS_AddINOut in dgdOutGrid.Items)
                    {
                        if (WinAccBS_AddINOut != null)
                        {
                            WinAccBS_AddINOut.IsCheck = true;
                        }
                    }
                    dgdOutGrid.Items.Refresh();
                }
            }
            else if (tbnStuffin.IsChecked == true)
            {
                if (dgdInGrid.Items.Count > 0)
                {
                    foreach (Win_Acc_Buy_Sales_U_CodeView WinAccBS_AddIN in dgdInGrid.Items)
                    {
                        if (WinAccBS_AddIN != null)
                        {
                            WinAccBS_AddIN.IsCheck = true;
                        }
                    }
                    dgdInGrid.Items.Refresh();
                }
            }
        }

        //전체 선택 체크 박스 체크 해제 이벤트
        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                if (dgdOutGrid.Items.Count > 0)
                {
                    foreach (Win_Acc_Buy_Sales_U_CodeView WinAccBS_AddINOut in dgdOutGrid.Items)
                    {
                        if (WinAccBS_AddINOut != null)
                        {
                            WinAccBS_AddINOut.IsCheck = false;
                        }
                    }
                    dgdOutGrid.Items.Refresh();
                }
            }
            else if (tbnStuffin.IsChecked == true)
            {
                if (dgdInGrid.Items.Count > 0)
                {
                    foreach (Win_Acc_Buy_Sales_U_CodeView WinAccBS_AddIN in dgdInGrid.Items)
                    {
                        if (WinAccBS_AddIN != null)
                        {
                            WinAccBS_AddIN.IsCheck = false;
                        }
                    }
                    dgdInGrid.Items.Refresh();
                }
            }
        }

        //매출 체크 이벤트(합계 계산)
        private void ChkC_Checked_Out(object sender, RoutedEventArgs e)
        {
            var SumOut = new Win_Acc_Buy_Sales_U_CodeView_Sum();

            int j = 0;

            for (int i = 0; i < dgdOutGrid.Items.Count; i++)
            {
                var Data = dgdOutGrid.Items[i] as Win_Acc_Buy_Sales_U_CodeView;

                if(Data.IsCheck == true)
                {
                    SumOut.SumAmount += ConvertDouble(Data.AMOUNT);
                    SumOut.SumVatAmount += ConvertDouble(Data.VATAmount);
                    SumOut.SumTotalAmount += ConvertDouble(Data.TotalAmount);

                    j = j + 1;
                }

                //txtblockSearchCountOut.Text = "      합계 : " + j.ToString() + "건"; ㅇㅇㅇ
                //txtblockSearchAmountOut.Text = "금액 : " + stringFormatN0(SumOut.SumAmount) + "원";
                //txtblockSearchVATOut.Text = "부가세 : " + stringFormatN0(SumOut.SumVatAmount) + "원";
                //txtblockSearchTotalOut.Text = "합계금액 : " + stringFormatN0(SumOut.SumTotalAmount) + "원";
            }
        }

        //매출 체크 해제 이벤트(합계 계산)
        private void ChkC_Unchecked_Out(object sender, RoutedEventArgs e)
        {
            var SumOut = new Win_Acc_Buy_Sales_U_CodeView_Sum();

            int j = 0;

            for (int i = 0; i < dgdOutGrid.Items.Count; i++)
            {
                var Data = dgdOutGrid.Items[i] as Win_Acc_Buy_Sales_U_CodeView;

                if (Data.IsCheck == true)
                {
                    SumOut.SumAmount += ConvertDouble(Data.AMOUNT);
                    SumOut.SumVatAmount += ConvertDouble(Data.VATAmount);
                    SumOut.SumTotalAmount += ConvertDouble(Data.TotalAmount);

                    j = j + 1;
                }

                //txtblockSearchCountOut.Text = "      합계 : " + j.ToString() + "건"; ㅇㅇㅇ
                //txtblockSearchAmountOut.Text = "금액 : " + stringFormatN0(SumOut.SumAmount) + "원";
                //txtblockSearchVATOut.Text = "부가세 : " + stringFormatN0(SumOut.SumVatAmount) + "원";
                //txtblockSearchTotalOut.Text = "합계금액 : " + stringFormatN0(SumOut.SumTotalAmount) + "원";
            }
        }

        //매입 체크 이벤트(합계 계산)
        private void ChkC_Checked_In(object sender, RoutedEventArgs e)
        {
            var SumIn = new Win_Acc_Buy_Sales_U_CodeView_Sum();

            int j = 0;

            for (int i = 0; i < dgdInGrid.Items.Count; i++)
            {
                var Data = dgdInGrid.Items[i] as Win_Acc_Buy_Sales_U_CodeView;

                if (Data.IsCheck == true)
                {
                    SumIn.SumAmount += ConvertDouble(Data.AMOUNT);
                    SumIn.SumVatAmount += ConvertDouble(Data.VATAmount);
                    SumIn.SumTotalAmount += ConvertDouble(Data.TotalAmount);

                    j = j + 1;
                }

                //txtblockSearchCountIn.Text = "      합계 : " + j.ToString() + "건";
                //txtblockSearchAmountIn.Text = "금액 : " + stringFormatN0(SumIn.SumAmount) + "원";
                //txtblockSearchVATIn.Text = "부가세 : " + stringFormatN0(SumIn.SumVatAmount) + "원";
                //txtblockSearchTotalIn.Text = "합계금액 : " + stringFormatN0(SumIn.SumTotalAmount) + "원";
            }
        }

        //매입 체크 해제 이벤트(합계 계산)
        private void ChkC_Unchecked_In(object sender, RoutedEventArgs e)
        {
            var SumIn = new Win_Acc_Buy_Sales_U_CodeView_Sum();

            int j = 0;

            for (int i = 0; i < dgdInGrid.Items.Count; i++)
            {
                var Data = dgdInGrid.Items[i] as Win_Acc_Buy_Sales_U_CodeView;

                if (Data.IsCheck == true)
                {
                    SumIn.SumAmount += ConvertDouble(Data.AMOUNT);
                    SumIn.SumVatAmount += ConvertDouble(Data.VATAmount);
                    SumIn.SumTotalAmount += ConvertDouble(Data.TotalAmount);

                    j = j + 1;
                }

                //    txtblockSearchCountIn.Text = "      합계 : " + j.ToString() + "건";
                //    txtblockSearchAmountIn.Text = "금액 : " + stringFormatN0(SumIn.SumAmount) + "원";
                //    txtblockSearchVATIn.Text = "부가세 : " + stringFormatN0(SumIn.SumVatAmount) + "원";
                //    txtblockSearchTotalIn.Text = "합계금액 : " + stringFormatN0(SumIn.SumTotalAmount) + "원";
            }
        }

        private void lblCompany_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void chkCompany_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void chkCompany_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void lblSalesman_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void chkSalesman_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void chkSalesman_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void txtSalesman_KeyDown(object sender, KeyEventArgs e)
        {

        }


        private void chktaxbillYN_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void chktaxbillYN_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void lbltaxbillYN_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }


        private void sfchkBSItems_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void sfchkBSItems_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void sflblBSItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void tbntaxbillN_Checked(object sender, RoutedEventArgs e)
        {
            tbntaxbillN.IsChecked = true; ;
            tbntaxbillY.IsChecked = false;

            if (tbntaxbillN.IsChecked == true)
            {
                InsertOrUpdate = "U";
                SaveData(InsertOrUpdate);
                
            }
            
        }

        private void tbntaxbillN_Unchecked(object sender, RoutedEventArgs e)
        {
            tbntaxbillN.IsChecked = false;
            tbntaxbillY.IsChecked = true;
        }

        private void tbntaxbillY_Checked(object sender, RoutedEventArgs e)
        {
            tbntaxbillY.IsChecked = true; 
            tbntaxbillN.IsChecked = false;

            if(tbntaxbillY.IsChecked == true)
            {
                InsertOrUpdate = "U";
                SaveData(InsertOrUpdate);
                
            }
            
        }

        private void tbntaxbillY_Unchecked(object sender, RoutedEventArgs e)
        {
            tbntaxbillY.IsChecked = false; 
            tbntaxbillN.IsChecked = true;
        }

        

        private ObservableCollection<CodeView> Direct_SetComboBoxCompany()
        {
            ObservableCollection<CodeView> retunCollection = new ObservableCollection<CodeView>();
            string sql = " select distinct CompanyID, CompanyName from vi_Acc_Buy_Sales  ";

            if (tbnOutware.IsChecked == true && tbnOutware.IsChecked == false)
            {
                sql += " where BSGBN = 2 ";
            }
            else if (tbnStuffin.IsChecked == true && tbnStuffin.IsChecked == false)
            {
                sql += " where BSGBN = 1 ";
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

        private void date_Click(object sender, RoutedEventArgs e)
        {
            
            period.Text = "발생일";

            DateTime today = DateTime.Now.Date;
            DateTime firstday = today.AddDays(1 - today.Day);
            DateTime lastday = firstday.AddMonths(1).AddDays(-1);
            dtpSDate.SelectedDate = firstday;
            dtpEDate.SelectedDate = lastday;

            dtpSDate.Visibility = Visibility.Visible;
            dtpEDate.Visibility = Visibility.Visible;
            dtpSDate2.Visibility = Visibility.Hidden;
            dtpEDate2.Visibility = Visibility.Hidden;
        }

        private void sellMM_Click(object sender, RoutedEventArgs e)
        {
            period.Text = "매출월";
            dtpSDate.Visibility = Visibility.Hidden;
            dtpEDate.Visibility = Visibility.Hidden;
            dtpSDate2.Visibility = Visibility.Visible;
            dtpEDate2.Visibility = Visibility.Visible;
            DateTime today = DateTime.Now.Date;
            DateTime firstday = today.AddDays(1 - today.Day);
            DateTime lastday = firstday.AddMonths(1).AddDays(-1);
            dtpSDate2.SelectedDate = firstday;
            dtpEDate2.SelectedDate = lastday;
        }

        private void buyMM_Click(object sender, RoutedEventArgs e)
        {
            period.Text = "매입월";
        }
    }


    class Win_Acc_Buy_Sales_U_CodeView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }                     // 순
        public bool IsCheck { get; set; }                // 체크
        public bool chkC { get; set; }                // 체크

        public string BSNo { get; set; }                 // 매출번호 P-KEY
        public string BSGBN { get; set; }                // 매입. 매출 구분자
        public string BSDate { get; set; }               // 발생일
        public string BSDate_s { get; set; }               // 발생일
        public string BSItemname { get; set; }               // 매출항목
        public string Article { get; set; }               // 품명
        public string OrderID { get; set; }               // 오더번호
        public string CurrencyUnitName { get; set; }               // 화폐단위
        public string CustomNat { get; set; }               // 거래처
        public string SalesChargeName { get; set; }               // 영업담당
                     
        public string QTY { get; set; }               // 수량
        public string UnitPrice { get; set; }               // 단가
        public string AMOUNT { get; set; }               // 금액
        public string VATAmount { get; set; }               // 부가세
        public string TotalAmount { get; set; }               // 입출고번호
        public string CompanyName { get; set; }               // 사업장
        public string BasisYearMon { get; set; }               // 매출월
        public string taxbillYN { get; set; }               // 계산서
        public string BSItem { get; set; }               // 매출항목ID
        public string ArticleID { get; set; }               // 항목ID
        public string CustomShort { get; set; }              
        public string Color { get; set; }              
        public string CustomID { get; set; }
        public string Comments { get; set; }             // 비고, 코멘트.
        public string INOUTNO { get; set; }             // 비고, 코멘트.
        public string CompanyID { get; set; }             // 비고, 코멘트.
        public string Count { get; set; }             // 비고, 코멘트.
        public string SumQty { get; set; }             // 비고, 코멘트.
        public string SumTotalAmount { get; set; }             // 비고, 코멘트.
        public string Sumdollar { get; set; }             // 비고, 코멘트.
 
    }

    class Win_Acc_Buy_Sales_U_CodeView_Sum
    {
        public double SumAmount { get; set; }
        public double SumVatAmount { get; set; }
        public double SumTotalAmount { get; set; }
    }
}
