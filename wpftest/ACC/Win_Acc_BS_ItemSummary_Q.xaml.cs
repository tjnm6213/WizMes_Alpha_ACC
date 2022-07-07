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
    /// Win_Acc_BS_ItemSummary_Q.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Acc_BS_ItemSummary_Q : UserControl
    {
        WizMes_Alpha_JA.PopUp.NoticeMessage msg = new PopUp.NoticeMessage();
        //(기다림 알림 메시지창)

        private Microsoft.Office.Interop.Excel.Application excelapp;
        private Microsoft.Office.Interop.Excel.Workbook workbook;
        private Microsoft.Office.Interop.Excel.Worksheet worksheet;
        private Microsoft.Office.Interop.Excel.Range workrange;
        private Microsoft.Office.Interop.Excel.Worksheet copysheet;
        private Microsoft.Office.Interop.Excel.Worksheet pastesheet;


        public Win_Acc_BS_ItemSummary_Q()
        {
            InitializeComponent();
        }

        // 로드 이벤트.
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            btnToday_Click(null, null);
            chkPeriod.IsChecked = true;
            YYYY.IsChecked = true;
            SetComboBox();
            chkSalePatner.IsChecked = true;
            cboSalePartner.SelectedIndex = 1;
        }


        #region (상단 조회조건 체크박스 enable 모음)

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

        // 화폐단위
        private void LblMoney_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkMoney.IsChecked == true) { chkMoney.IsChecked = false; }
            else { chkMoney.IsChecked = true; }
        }
        // 화폐단위
        private void ChkMoney_Checked(object sender, RoutedEventArgs e)
        {
            cboMoney.IsEnabled = true;
        }
        // 화폐단위
        private void ChkMoney_Unchecked(object sender, RoutedEventArgs e)
        {
            cboMoney.IsEnabled = false;
        }

        // 화폐단위
        private void lblSalePatner_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkSalePatner.IsChecked == true) { chkSalePatner.IsChecked = false; }
            else { chkSalePatner.IsChecked = true; }
        }
        // 화폐단위
        private void chkSalePatner_Checked(object sender, RoutedEventArgs e)
        {
            cboSalePartner.IsEnabled = true;
        }
        // 화폐단위
        private void chkSalePatner_Unchecked(object sender, RoutedEventArgs e)
        {
            cboSalePartner.IsEnabled = false;
        }



        #endregion


        #endregion

        #region (콤보박스 세팅) SetComboBox
        private void SetComboBox()
        {
            //매입,매출 화폐단위(입력)
            List<string[]> listPrice = new List<string[]>();
            string[] Price01 = new string[] { "0", "₩" };
            string[] Price02 = new string[] { "1", "$" };
            listPrice.Add(Price01);
            listPrice.Add(Price02);

            ObservableCollection<CodeView> ovcPrice = ComboBoxUtil.Instance.Direct_SetComboBox(listPrice);
            this.cboMoney.ItemsSource = ovcPrice;
            this.cboMoney.DisplayMemberPath = "code_name";
            this.cboMoney.SelectedValuePath = "code_id";


            ObservableCollection<CodeView> ovcWorkHouse = ComboBoxUtil.Instance.Get_CompanyID();
            cboSalePartner.ItemsSource = ovcWorkHouse;
            cboSalePartner.DisplayMemberPath = "code_name";
            cboSalePartner.SelectedValuePath = "code_id";
            cboSalePartner.SelectedIndex = 0;


        }

        #endregion


        // 검색버튼 클릭.
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgdIS_BuySale.Items.Clear();
            dgdIS_ReceivePay.Items.Clear();
            dgdIS_ReceivePayDetail.Items.Clear();

            // 1. [이월/매입/매출/총계]  + 2. [이월/입금/출금/총계]
            FillGrid_dgdDoubleActionGrid();

            // 2. 입금 출금 내역 상세조회
            FillGrid_dgdReceivePayDetail();
        }

        #region ( 1. [이월/매입/매출/총계]  + 2. [이월/입금/출금/총계]) FillGrid_dgdDoubleActionGrid
        // 1. [이월/매입/매출/총계]  + 2. [이월/입금/출금/총계]
        private void FillGrid_dgdDoubleActionGrid()
        {
            int sBSDate = 0;
            if (YYYY.IsChecked == true) { sBSDate = 1; } //발생일 기준 yyyy-mm-dd
            else if (YYYYMM.IsChecked == true) { sBSDate = 2; } //매입매출월 기준 yyyy-mm  

            try
            {

                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Clear();

                sqlParameter.Add("sDate", sBSDate == 1 ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : dtpSDate2.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("EDate", sBSDate == 1 ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : dtpEDate2.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("sDatGbn", YYYY.IsChecked == true ? "1" : "2");
                sqlParameter.Add("nChkCompanyID", chkSalePatner.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sCompanyID", chkSalePatner.IsChecked == true ? cboSalePartner.SelectedValue.ToString() : "");

                sqlParameter.Add("nChkCurrencyUnit", chkMoney.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sCurrencyUnit", chkMoney.IsChecked == true ? cboMoney.SelectedValue.ToString() : "");
                sqlParameter.Add("nChkorder", 0);
                sqlParameter.Add("sOrder", "");
                sqlParameter.Add("nchkNotInZero", chkNotInZero.IsChecked == true ? 1 : 0);
                sqlParameter.Add("CustomID", "");





                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_RP_Period_Sum_Q_WPF", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;
                    int j = 0;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("조회된 데이터가 없습니다.");
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;
                        foreach (DataRow dr in drc)
                        {
                            var WinAcc_IS_BuySale = (dynamic)null;
                            var WinAcc_IS_ReceivePay = (dynamic)null;

                            if (dr["GBN"].ToString() == "1" || (dr["GBN"].ToString() == "3" && dr["CLS"].ToString() == "1"))   // 상단그리드 : [이월 / 매입 / 매출 / 총계]
                            {

                                WinAcc_IS_BuySale = new Win_Acc_BS_ItemSummary_BuySale_Q_CodeView()
                                {
                                    Num = i + 1,
                                    cls = dr["BSGBNNAME"].ToString(),
                                    Item = dr["LName"].ToString(),
                                    Amount = dr["Amount"].ToString(),
                                    VatAmount = dr["Vat"].ToString(),
                                    TotalAmount = dr["TotAmount"].ToString(),
                                    Currency = dr["currencyUnitName"].ToString()
                                };
                                // 콤마입히기 > 금액
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_BuySale.Amount))
                                {
                                    WinAcc_IS_BuySale.Amount = Lib.Instance.returnNumStringZero(WinAcc_IS_BuySale.Amount);
                                }
                                // 콤마입히기 > 부가세
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_BuySale.VatAmount))
                                {
                                    WinAcc_IS_BuySale.VatAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_BuySale.VatAmount);
                                }
                                // 콤마입히기 > 합계
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_BuySale.TotalAmount))
                                {
                                    WinAcc_IS_BuySale.TotalAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_BuySale.TotalAmount);
                                }

                            }
                            else if (dr["GBN"].ToString() == "2")   // 하단그리드 : [이월 / 입금 / 출금 / 총계]
                            {

                                WinAcc_IS_ReceivePay = new Win_Acc_BS_ItemSummary_ReceivePay_Q_CodeView()
                                {
                                    Num = j + 1,
                                    cls = dr["BSGBNName"].ToString(),
                                    Item = dr["LName"].ToString(),
                                    Cash = dr["CashAmount"].ToString(),
                                    Bank = dr["BankAmount"].ToString(),
                                    Bill = dr["BillAmount"].ToString(),
                                    DisCount = dr["DcAmount"].ToString(),
                                    TotalAmount = dr["TotAmount"].ToString(),
                                    Currency = dr["currencyUnitName"].ToString(),
                                    AlterItem = dr["RefItemName"].ToString()
                                };
                                // 콤마입히기 > 현금
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay.Cash))
                                {
                                    WinAcc_IS_ReceivePay.Cash = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay.Cash);
                                }
                                // 콤마입히기 > 현금
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay.Card))
                                {
                                    WinAcc_IS_ReceivePay.Card = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay.Card);
                                }
                                // 콤마입히기 > 은행
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay.Bank))
                                {
                                    WinAcc_IS_ReceivePay.Bank = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay.Bank);
                                }
                                // 콤마입히기 > 어음
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay.Bill))
                                {
                                    WinAcc_IS_ReceivePay.Bill = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay.Bill);
                                }
                                // 콤마입히기 > 할인
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay.DisCount))
                                {
                                    WinAcc_IS_ReceivePay.DisCount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay.DisCount);
                                }
                                // 콤마입히기 > 합계
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay.TotalAmount))
                                {
                                    WinAcc_IS_ReceivePay.TotalAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay.TotalAmount);
                                }

                            }
                            if (dr["GBN"].ToString() == "1" || (dr["GBN"].ToString() == "3" && dr["CLS"].ToString() == "1"))
                            {
                                dgdIS_BuySale.Items.Add(WinAcc_IS_BuySale);
                                i++;
                            }
                            else if (dr["GBN"].ToString() == "2")
                            {
                                dgdIS_ReceivePay.Items.Add(WinAcc_IS_ReceivePay);
                                j++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion

        #region ( 2. 입/출금 내역 상세조회 )  FillGrid_dgdReceivePayDetail
        // 2. 입금 출금 내역 상세조회
        private void FillGrid_dgdReceivePayDetail()
        {
            int sBSDate = 0;
            if (YYYY.IsChecked == true) { sBSDate = 1; } //발생일 기준 yyyy-mm-dd
            else if (YYYYMM.IsChecked == true) { sBSDate = 2; } //매입매출월 기준 yyyy-mm  

            try
            {

                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Clear();

                sqlParameter.Add("sDate", sBSDate == 1 ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : dtpSDate2.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("EDate", sBSDate == 1 ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : dtpEDate2.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("nChkCompanyID", chkSalePatner.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sCompanyID", chkSalePatner.IsChecked == true ? cboSalePartner.SelectedValue.ToString() : "");
                sqlParameter.Add("CustomID", "");

                sqlParameter.Add("nChkCurrencyUnit", chkMoney.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sCurrencyUnit", chkMoney.IsChecked == true ? cboMoney.SelectedValue.ToString() : "");
                sqlParameter.Add("nChkorder", 0);
                sqlParameter.Add("sOrder", "");
                sqlParameter.Add("nchkRealRPITEM", 0);
                sqlParameter.Add("nchkNotInZero", chkNotInZero.IsChecked == true ? 1 : 0);




                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_RP_Period_deteail_Q_WPF", sqlParameter, false);

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
                            double CashAmount = 0;
                        
                            double BankAmount = 0;
                            double BillAmount = 0;
                            double DcAmount = 0;
                            double RAmount = 0;
                            double PAmount = 0;

                            double.TryParse(dr["CashAmount"].ToString(), out CashAmount);
                            double.TryParse(dr["BankAmount"].ToString(), out BankAmount);
                            double.TryParse(dr["BillAmount"].ToString(), out BillAmount);
                            double.TryParse(dr["DcAmount"].ToString(), out DcAmount);
                            double.TryParse(dr["RAmount"].ToString(), out RAmount);
                            double.TryParse(dr["PAmount"].ToString(), out PAmount);


                            if (dr["GBN"].ToString() == "3")        //총계일 때는,, 입금총계, 출금총계만 보여준다.
                            {
                                var WinAcc_IS_ReceivePay_Detail = new Win_Acc_BS_ItemSummary_ReceivePay_Detail_Q_CodeView()
                                {
                                    Num = i + 1,
                                    cls = dr["RPGBNName"].ToString(),

                                    RPDate = "총계",
                                    KCustom = dr["CustomNat"].ToString(),
                                    BSItem = dr["RPItemName"].ToString(),
                                    RefComments = dr["RefComments"].ToString(),
                                    CashAmount = "",

                                    Bank = dr["BankName"].ToString(),
                                    BankAmount = "",
                                    BillAmount = "",
                                    DcAmount = "",
                                    RAmount = dr["RAmount"].ToString(),

                                    PAmount = dr["PAmount"].ToString(),
                                    currencyUnitName = dr["currencyUnitName"].ToString()
                                };

                                // 콤마입히기 > 입금총계
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay_Detail.RAmount))
                                {
                                    WinAcc_IS_ReceivePay_Detail.RAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay_Detail.RAmount);
                                }
                                // 콤마입히기 > 출금총계
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay_Detail.PAmount))
                                {
                                    WinAcc_IS_ReceivePay_Detail.PAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay_Detail.PAmount);
                                }


                                dgdIS_ReceivePayDetail.Items.Add(WinAcc_IS_ReceivePay_Detail);
                                i++;
                            }
                            else
                            {
                                var WinAcc_IS_ReceivePay_Detail = new Win_Acc_BS_ItemSummary_ReceivePay_Detail_Q_CodeView()
                                {
                                    Num = i + 1,
                                    cls = dr["RPGBNName"].ToString(),

                                    RPDate = DatePickerFormat(dr["RPDate"].ToString()),
                                    KCustom = dr["CustomNat"].ToString(),
                                    BSItem = dr["RPItemName"].ToString(),
                                    RefComments = dr["RefComments"].ToString(),
                                    CashAmount = dr["CashAmount"].ToString(),
                                    Bank = dr["BankName"].ToString(),
                                    BankAmount = dr["BankAmount"].ToString(),
                                    BillAmount = dr["BillAmount"].ToString(),
                                    DcAmount = dr["DcAmount"].ToString(),
                                    RAmount = dr["RAmount"].ToString(),

                                    PAmount = dr["PAmount"].ToString(),
                                    currencyUnitName = dr["currencyUnitName"].ToString()
                                };
                                // 콤마입히기 > 현금
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay_Detail.CashAmount))
                                {
                                    WinAcc_IS_ReceivePay_Detail.CashAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay_Detail.CashAmount);
                                }
                                // 콤마입히기 > 카드
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay_Detail.CardAmount))
                                {
                                    WinAcc_IS_ReceivePay_Detail.CardAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay_Detail.CardAmount);
                                }
                                // 콤마입히기 > 은행
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay_Detail.BankAmount))
                                {
                                    WinAcc_IS_ReceivePay_Detail.BankAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay_Detail.BankAmount);
                                }
                                // 콤마입히기 > 어음
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay_Detail.BillAmount))
                                {
                                    WinAcc_IS_ReceivePay_Detail.BillAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay_Detail.BillAmount);
                                }
                                // 콤마입히기 > 할인
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay_Detail.DcAmount))
                                {
                                    WinAcc_IS_ReceivePay_Detail.DcAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay_Detail.DcAmount);
                                }
                                // 콤마입히기 > 입금총계
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay_Detail.RAmount))
                                {
                                    WinAcc_IS_ReceivePay_Detail.RAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay_Detail.RAmount);
                                }
                                // 콤마입히기 > 출금총계
                                if (Lib.Instance.IsNumOrAnother(WinAcc_IS_ReceivePay_Detail.PAmount))
                                {
                                    WinAcc_IS_ReceivePay_Detail.PAmount = Lib.Instance.returnNumStringZero(WinAcc_IS_ReceivePay_Detail.PAmount);
                                }
                                dgdIS_ReceivePayDetail.Items.Add(WinAcc_IS_ReceivePay_Detail);
                                i++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion


        // 엑셀버튼 클릭.
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] dgdStr = new string[6];
            dgdStr[0] = "거래처 매입,매출 집계";
            dgdStr[1] = "계정별 입출금 집계";
            dgdStr[2] = "입출금 상세";
            dgdStr[3] = dgdIS_BuySale.Name;
            dgdStr[4] = dgdIS_ReceivePay.Name;
            dgdStr[5] = dgdIS_ReceivePayDetail.Name;

            ExportExcelxaml ExpExc = new ExportExcelxaml(dgdStr);
            ExpExc.ShowDialog();

            if (ExpExc.DialogResult.HasValue)
            {
                if (ExpExc.choice.Equals(dgdIS_BuySale.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdIS_BuySale);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdIS_BuySale);

                    Name = dgdIS_BuySale.Name;
                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else if (ExpExc.choice.Equals(dgdIS_ReceivePay.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdIS_ReceivePay);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdIS_ReceivePay);

                    Name = dgdIS_ReceivePay.Name;
                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else if (ExpExc.choice.Equals(dgdIS_ReceivePayDetail.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdIS_ReceivePayDetail);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdIS_ReceivePayDetail);

                    Name = dgdIS_ReceivePayDetail.Name;
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

        // 닫기 버튼.
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }


        #region (현금출납장 , 거래원장 양식지 인쇄버튼 관련 묶음)

        // 현금 출납장 양식지 버튼 클릭.
        private void btnPrintCash_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = btnPrintCash.ContextMenu;
            menu.StaysOpen = true;
            menu.IsOpen = true;
        }
        // 거래원장 양식지 버튼 클릭.
        private void btnPrintCustom_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = btnPrintCustom.ContextMenu;
            menu.StaysOpen = true;
            menu.IsOpen = true;
        }

        //현금출납장 미리보기 클릭
        private void menuSeeAhead_Click(object sender, RoutedEventArgs e)
        {
            msg.Show();
            msg.Topmost = true;
            msg.Refresh();

            PrintWork(true, "btnPrintCash");
        }

        //현금출납장 바로인쇄 클릭
        private void menuRightPrint_Click(object sender, RoutedEventArgs e)
        {
            msg.Show();
            msg.Topmost = true;
            msg.Refresh();

            PrintWork(false, "btnPrintCash");
        }

        //현금출납장 메뉴버튼 닫기 
        private void menuClose_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = btnPrintCash.ContextMenu;
            menu.StaysOpen = false;
            menu.IsOpen = false;
        }

        //거래원장 미리보기 클릭
        private void menuSeeAhead2_Click(object sender, RoutedEventArgs e)
        {
            PopUp.JangBooPCustom JangBooPCustom = new PopUp.JangBooPCustom();
            JangBooPCustom.ShowDialog();

            if (JangBooPCustom.DialogResult == true)
            {
                string sBSGbn = JangBooPCustom.Wh_Ar_BSGbn;
                int nChkCustom = JangBooPCustom.Wh_Ar_ChkCustom;
                string CustomID = JangBooPCustom.Wh_Ar_CustomID;

                msg.Show();
                msg.Topmost = true;
                msg.Refresh();

                PrintWork2(true, "btnPrintCustom", sBSGbn, nChkCustom, CustomID);
            }
            else
            {
                menuClose2_Click(null, null);
            }


        }

        //현금출납장 바로 인쇄
        private void menuRightPrint2_Click(object sender, RoutedEventArgs e)
        {
            PopUp.JangBooPCustom JangBooPCustom = new PopUp.JangBooPCustom();
            JangBooPCustom.ShowDialog();

            if (JangBooPCustom.DialogResult == true)
            {
                string sBSGbn = JangBooPCustom.Wh_Ar_BSGbn;
                int nChkCustom = JangBooPCustom.Wh_Ar_ChkCustom;
                string CustomID = JangBooPCustom.Wh_Ar_CustomID;

                msg.Show();
                msg.Topmost = true;
                msg.Refresh();

                PrintWork2(false, "btnPrintCustom", sBSGbn, nChkCustom, CustomID);
            }
            else
            {
                menuClose2_Click(null, null);
            }
        }

        //거래원장 닫기
        private void menuClose2_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = btnPrintCustom.ContextMenu;
            menu.StaysOpen = false;
            menu.IsOpen = false;
        }


        // 현금출납장 인쇄 >> 인쇄 실질 동작
        private void PrintWork(bool preview_click, string SendButtonName)
        {
            try
            {
                if (SendButtonName == "btnPrintCash")           //현금출납장
                {

                    //현금출납장 내역을 프로시저로 가져와서 양식에 반영하는 듯
                    DataSet ds = null;
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                    sqlParameter.Clear();

                    sqlParameter.Add("sDate", chkPeriod.IsChecked == true ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : "20000101");
                    sqlParameter.Add("eDate", chkPeriod.IsChecked == true ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : "21000101");
                    ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_RP_pCash", sqlParameter, false);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("현금 출납장에 기입할 데이터가 없습니다.");
                            msg.Hide();
                        }
                        else
                        {
                            DataRowCollection drc = dt.Rows;
                            excelapp = new Microsoft.Office.Interop.Excel.Application();

                            string MyBookPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Report\\ACC_PCash.xls";
                            workbook = excelapp.Workbooks.Add(MyBookPath);
                            worksheet = workbook.Sheets["Form"];
                            pastesheet = workbook.Sheets["Print"];

                            // 페이지 계산
                            int rowCount = 0;
                            foreach (DataRow dr in drc)
                            {
                                rowCount++;              //반영할 데이터 갯수 rowCount ㅜㅜ 이렇게 밖에 모루겠다.
                            }

                            int excelStartRow = 7;      //엑셀에 데이터를 반영할 시작 행번호

                            int copyLine = 1;           //??
                            int Page = 1;               //페이지 변수
                            int PageAll = (int)Math.Ceiling(rowCount / 39.0);       //전체페이지 변수(현금출납장은 1페이지에 39개 들어감)
                            int DataCountCash = 0;          //데이터 반영 활용 변수

                            //상단의 기간
                            workrange = worksheet.get_Range("E5", "L5");//셀 범위 지정
                            workrange.Value2 = dtpSDate.SelectedDate.Value.ToString("yyyy-MM-dd") + " ~ " + dtpEDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                            workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                            //상단의 작성일
                            workrange = worksheet.get_Range("X5", "AA5");//셀 범위 지정
                            workrange.Value2 = DateTime.Today.ToString("yyyy-MM-dd");
                            workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                            int excelNum = 0;                  //엑셀 행번호 변수

                            // 기존에 있는 데이터 지우기
                            worksheet.Range["A7", "AA45"].EntireRow.ClearContents();
                            //worksheet.Range["A5", "AA45"].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlLineStyleNone; //라인 스타일도 지운다는 건가??


                            int i = 0;
                            foreach (DataRow dr in drc)
                            {
                                int excelRow = excelStartRow + excelNum;        //데이터를 반영할 시작행 + 행 숫자 넣을 임시변수

                                if (DataCountCash == 39 * Page)     //페이지 수 곱하기 한 페이지에 들어갈 수 있는 데이터 값과 같아지면
                                {
                                    // Form 시트 내용 Print 시트에 복사 붙여넣기
                                    worksheet.Select();
                                    worksheet.UsedRange.EntireRow.Copy();
                                    pastesheet.Select();
                                    workrange = pastesheet.Cells[copyLine + 1, 1];
                                    workrange.Select();
                                    pastesheet.Paste();

                                    if (Page < PageAll)
                                    {
                                        Page++;                            //페이지 값 증가(전체페이지 값이 될 때까지)
                                        copyLine = ((Page - 1) * 49);      // copy 시작 값

                                        // 기존에 있는 데이터 지우기
                                        worksheet.Range["A7", "AA45"].EntireRow.ClearContents();
                                        // 행번호 7번부터 시작하도록 초기화
                                        excelRow = excelStartRow;
                                        excelNum = 0;

                                    }
                                }

                                workrange = worksheet.get_Range("A" + excelRow);    //순번
                                workrange.Value2 = i + 1;
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("B" + excelRow, "D" + excelRow);    //일자
                                workrange.Value2 = dr["RPDate"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("E" + excelRow, "I" + excelRow);    //계정과목
                                workrange.Value2 = dr["BSItem"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("J" + excelRow, "M" + excelRow);    //입금
                                workrange.Value2 = dr["InAmount"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("N" + excelRow, "Q" + excelRow);    //출금
                                workrange.Value2 = dr["OutAmount"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("R" + excelRow, "U" + excelRow);    //잔액
                                workrange.Value2 = dr["RemainAmount"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("V" + excelRow, "AA" + excelRow);    //적요
                                workrange.Value2 = dr["Comments"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                i++;
                                excelNum++;     // 행 번호 임시변수 1증가
                                DataCountCash++;    // 데이터 변수 1증가

                            }

                            if (DataCountCash == rowCount)        //마지막페이지의 경우
                            {
                                // Form 시트 내용 Print 시트에 복사 붙여넣기
                                worksheet.Select();
                                worksheet.UsedRange.EntireRow.Copy();
                                pastesheet.Select();
                                workrange = pastesheet.Cells[copyLine + 1, 1];
                                workrange.Select();
                                pastesheet.Paste();

                            }


                            // 2장 이상 넘어가면 페이지 넘버 입력
                            if (PageAll > 1)
                            {
                                pastesheet.PageSetup.CenterFooter = "&P / &N";
                            }

                            pastesheet.UsedRange.EntireRow.Select();
                            msg.Hide();

                            if (preview_click == true)      //미리보기 버튼이 클릭이라면
                            {
                                excelapp.Visible = true;
                                pastesheet.PrintPreview();
                            }
                            else
                            {
                                excelapp.Visible = true;
                                pastesheet.PrintOutEx();
                            }
                        }
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
            //MessageBox.Show("가빈이 잘했어!!");
        }

        // 거래원장 인쇄 >> 인쇄 실질 동작
        private void PrintWork2(bool preview_click, string SendButtonName, string sBSGbn, int nChkCustom, string CustomID)
        {
            try
            {
                if (SendButtonName == "btnPrintCustom")            // 거래원장
                {
                    DataSet ds = null;
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                    sqlParameter.Clear();

                    sqlParameter.Add("sBSGbn", sBSGbn);
                    //2020.01.19 선택된 날짜가 없으면 오늘 날짜 넣어줘야 프로시저 값 나옴
                    sqlParameter.Add("sDate", chkPeriod.IsChecked == true ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : DateTime.Now.ToString("yyyyMMdd"));
                    sqlParameter.Add("eDate", chkPeriod.IsChecked == true ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : DateTime.Now.ToString("yyyyMMdd"));
                    sqlParameter.Add("nChkCustom", nChkCustom);
                    sqlParameter.Add("CustomID", CustomID);
                    ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_RP_pCustom", sqlParameter, false);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("거래원장에 기입할 데이터가 없습니다.");
                            msg.Hide();
                        }
                        else
                        {
                            DataRowCollection drc = dt.Rows;
                            excelapp = new Microsoft.Office.Interop.Excel.Application();

                            string MyBookPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Report\\ACC_PCustom.xls";
                            workbook = excelapp.Workbooks.Add(MyBookPath);
                            worksheet = workbook.Sheets["Form"];
                            pastesheet = workbook.Sheets["Print"];

                            // 페이지 계산
                            int rowCount = 0;
                            foreach (DataRow dr in drc)
                            {
                                rowCount++;              //반영할 데이터 갯수 rowCount ㅜㅜ 이렇게 밖에 모루겠다.
                            }

                            int excelStartRow = 7;      //엑셀에 데이터를 반영할 시작 행번호

                            int copyLine = 1;           //??
                            int Page = 1;               //페이지 변수
                            int PageAll = (int)Math.Ceiling(rowCount / 39.0);       //전체페이지 변수(현금출납장은 1페이지에 39개 들어감)
                            int DataCountCash = 0;          //데이터 반영 활용 변수


                            //상단의 거래처
                            workrange = worksheet.get_Range("C4", "Q4");//셀 범위 지정
                            workrange.Value2 = drc[0]["KCustom"].ToString();
                            if (nChkCustom == 0 && sBSGbn == "1")
                            {
                                workrange.Value2 = drc[0]["KCustom"].ToString() + "등 매입업체";
                            }
                            else if (nChkCustom == 0 && sBSGbn == "2")
                            {
                                workrange.Value2 = drc[0]["KCustom"].ToString() + "등 매출업체";
                            }
                            workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                            //상단의 기간
                            workrange = worksheet.get_Range("C5", "Q5");//셀 범위 지정
                            workrange.Value2 = dtpSDate.SelectedDate.Value.ToString("yyyy-MM-dd") + " ~ " + dtpEDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                            workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                            //상단의 작성일
                            workrange = worksheet.get_Range("Y5", "AD5");//셀 범위 지정
                            workrange.Value2 = DateTime.Today.ToString("yyyy-MM-dd");
                            workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                            //컬럼교체건(입금액 OR 출금액)
                            workrange = worksheet.get_Range("S6", "V6");//셀 범위 지정
                            if (sBSGbn == "1")
                            {
                                workrange.Value2 = "출금액";
                            }
                            else if (sBSGbn == "2")
                            {
                                workrange.Value2 = "입금액";
                            }
                            workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                            int excelNum = 0;                  //엑셀 행번호 변수

                            // 기존에 있는 데이터 지우기
                            worksheet.Range["A7", "AD42"].EntireRow.ClearContents();
                            //worksheet.Range["A7", "AD42"].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlLineStyleNone; //라인 스타일도 지운다는 건가??

                            int i = 0;
                            foreach (DataRow dr in drc)
                            {
                                int excelRow = excelStartRow + excelNum;        //데이터를 반영할 시작행 + 행 숫자 넣을 임시변수
                                if (DataCountCash == 36 * Page)     //페이지 수 곱하기 한 페이지에 들어갈 수 있는 데이터 값과 같아지면
                                {
                                    // Form 시트 내용 Print 시트에 복사 붙여넣기
                                    worksheet.Select();
                                    worksheet.UsedRange.EntireRow.Copy();
                                    pastesheet.Select();
                                    workrange = pastesheet.Cells[copyLine + 1, 1];
                                    workrange.Select();
                                    pastesheet.Paste();

                                    if (Page < PageAll)
                                    {
                                        Page++;                            //페이지 값 증가(전체페이지 값이 될 때까지)
                                        copyLine = ((Page - 1) * 47);      // copy 시작 값

                                        // 기존에 있는 데이터 지우기
                                        worksheet.Range["A7", "AD42"].EntireRow.ClearContents();
                                        // 행번호 7번부터 시작하도록 초기화
                                        excelRow = excelStartRow;
                                        excelNum = 0;

                                    }
                                }

                                workrange = worksheet.get_Range("A" + excelRow);    //순번
                                workrange.Value2 = i + 1;
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("B" + excelRow, "C" + excelRow);    //일자
                                if (dr["BSDate"].ToString().Length == 8)
                                {
                                    workrange.Value2 = dr["BSDate"].ToString().Substring(4, 2) + "/" + dr["BSDate"].ToString().Substring(6, 2);
                                }
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("D" + excelRow, "H" + excelRow);    //품명
                                workrange.Value2 = dr["BSItemName"].ToString().Trim();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("I" + excelRow, "J" + excelRow);    //수량
                                workrange.Value2 = dr["Qty"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("K" + excelRow, "N" + excelRow);    //단가
                                workrange.Value2 = dr["UnitPrice"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("O" + excelRow, "R" + excelRow);    //금액
                                workrange.Value2 = dr["Amount"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("S" + excelRow, "V" + excelRow);    //입금액
                                workrange.Value2 = dr["InOutAmount"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("W" + excelRow, "Z" + excelRow);    //잔액
                                workrange.Value2 = dr["InitAmount"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                workrange = worksheet.get_Range("AA" + excelRow, "AD" + excelRow);    //비고
                                workrange.Value2 = dr["Comments"].ToString();
                                workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                                i++;            //순번 1증가
                                excelNum++;     // 행 번호 임시변수 1증가
                                DataCountCash++;    // 데이터 변수 1증가
                            }

                            if (DataCountCash == rowCount)        //마지막페이지의 경우
                            {
                                // Form 시트 내용 Print 시트에 복사 붙여넣기
                                worksheet.Select();
                                worksheet.UsedRange.EntireRow.Copy();
                                pastesheet.Select();
                                workrange = pastesheet.Cells[copyLine + 1, 1];
                                workrange.Select();
                                pastesheet.Paste();

                            }

                            // 2장 이상 넘어가면 페이지 넘버 입력
                            if (PageAll > 1)
                            {
                                pastesheet.PageSetup.CenterFooter = "&P / &N";
                            }

                            pastesheet.UsedRange.EntireRow.Select();
                            msg.Hide();

                            if (preview_click == true)
                            {
                                excelapp.Visible = true;
                                pastesheet.PrintPreview();
                            }
                            else
                            {
                                excelapp.Visible = true;
                                pastesheet.PrintOutEx();
                            }
                        }
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
        }
        #endregion


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

       




        class Win_Acc_BS_ItemSummary_BuySale_Q_CodeView
        {
            public override string ToString()
            {
                return (this.ReportAllProperties());
            }

            public int Num { get; set; }
            public string cls { get; set; }

            public string Item { get; set; }
            public string Amount { get; set; }
            public string VatAmount { get; set; }
            public string TotalAmount { get; set; }
            public string Currency { get; set; }

        }
        class Win_Acc_BS_ItemSummary_ReceivePay_Q_CodeView
        {
            public override string ToString()
            {
                return (this.ReportAllProperties());
            }

            public int Num { get; set; }
            public string cls { get; set; }

            public string Item { get; set; }
            public string Cash { get; set; }
            public string Card { get; set; }
            public string Bank { get; set; }
            public string Bill { get; set; }
            public string DisCount { get; set; }

            public string TotalAmount { get; set; }
            public string Currency { get; set; }
            public string AlterItem { get; set; }

        }

        class Win_Acc_BS_ItemSummary_ReceivePay_Detail_Q_CodeView
        {
            public override string ToString()
            {
                return (this.ReportAllProperties());
            }

            public int Num { get; set; }
            public string cls { get; set; }

            public string RPDate { get; set; }
            public string KCustom { get; set; }
            public string BSItem { get; set; }
            public string RefComments { get; set; }
            public string CashAmount { get; set; }

            public string CardAmount { get; set; }

            public string Bank { get; set; }
            public string BankAmount { get; set; }
            public string BillAmount { get; set; }
            public string DcAmount { get; set; }
            public string RAmount { get; set; }

            public string PAmount { get; set; }
            public string currencyUnitName { get; set; }

        }


    }
}
