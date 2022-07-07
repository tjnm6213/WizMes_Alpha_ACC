using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
    /// Win_frm_Cst_AfterCostAnal_Q.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_frm_Cst_AfterCostAnal_Q : UserControl
    {
        string USD = "";
        int rowNum = 0; // 행번호

        // 오더 번호 플러스파인더 변수
        int pf_Order = 72; // 오더번호

        Win_frm_Cst_AfterCostAnal_Q_CodeView AfterCost = new Win_frm_Cst_AfterCostAnal_Q_CodeView(); // 메인 데이터그리드 객체
        Win_frm_Cst_AfterCostAnal_Detail_CodeView AfterCost_Detail = new Win_frm_Cst_AfterCostAnal_Detail_CodeView(); // 상세정보 데이터그리드 객체

        // 인쇄 활용 객체
        private Microsoft.Office.Interop.Excel.Application excelapp;
        private Microsoft.Office.Interop.Excel.Workbook workbook;
        private Microsoft.Office.Interop.Excel.Worksheet worksheet;
        private Microsoft.Office.Interop.Excel.Range workrange;
        private Microsoft.Office.Interop.Excel.Worksheet copysheet;
        private Microsoft.Office.Interop.Excel.Worksheet pastesheet;
        WizMes_Alpha_JA.PopUp.NoticeMessage msg = new WizMes_Alpha_JA.PopUp.NoticeMessage();


        public Win_frm_Cst_AfterCostAnal_Q()
        {
            InitializeComponent();
        }

        #region Header 부분 메서드

        // 전일, 금일, 전월, 금월 버튼 이벤트
        //전일
        private void btnYesterday_Click(object sender, RoutedEventArgs e)
        {
            orderSDate.SelectedDate = DateTime.Today.AddDays(-1);
            orderEDate.SelectedDate = DateTime.Today;
        }
        //금일
        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            orderSDate.SelectedDate = DateTime.Today;
            orderEDate.SelectedDate = DateTime.Today;
        }
        //전월
        private void btnLastMonth_Click(object sender, RoutedEventArgs e)
        {
            orderSDate.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[0];
            orderEDate.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[1];
        }
        //금월
        private void btnThisMonth_Click(object sender, RoutedEventArgs e)
        {
            orderSDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[0];
            orderEDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[1];
        }

        // 거래처 검색 플러스파인더 버튼 클릭 이벤트
        private void btnCustomSrh_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtCustomSrh, (int)Defind_CodeFind.DCF_CUSTOM, "");
        }
        // 거래처 텍스트박스 엔터키 → 플러스파인더 이벤트
        private void TxtCustomSrh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtCustomSrh, (int)Defind_CodeFind.DCF_CUSTOM, "");
            }
        }

        // 품명 검색 플러스파인더 버튼 클릭 이벤트
        private void btnArticleSrh_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtArticleSrh, (int)Defind_CodeFind.DCF_Article, "");
        }
        // 품명 텍스트박스 엔터키 → 플러스파인더 이벤트
        private void TxtArticleSrh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtArticleSrh, (int)Defind_CodeFind.DCF_Article, "");
            }
        }

        // 오더번호 검색 플러스파인더 버튼 클릭 이벤트
        private void btnOrderNoSrh_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtOrderNoSrh, pf_Order, "");
        }
        // 오더번호 텍스트박스 엔터키 → 플러스파인더 이벤트
        private void TxtOrderSrh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtOrderNoSrh, pf_Order, "");
            }
        }

        // 오른쪽 상단 버튼 이벤트
        // 검색 버튼 이벤트
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // 한국 수출입은행 환율 구하는 메서드
            //USD = getExchangeRate("USD");

            rowNum = 0;
            re_Search(rowNum);
        }
        // 닫기 버튼
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        #region 인쇄버튼 클릭 이벤트(관련 메서드)
        // 인쇄 버튼
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = btnPrint.ContextMenu;
            menu.StaysOpen = true;
            menu.IsOpen = true;
        }
        // 인쇄 - 미리보기 클릭
        private void menuSeeAhead_Click(object sender, RoutedEventArgs e)
        {
            if (dgdSub.Items.Count == 0)
            {
                MessageBox.Show("해당 자료가 존재하지 않습니다.");
                return;
            }
            msg.Show();
            msg.Topmost = true;
            msg.Refresh();

            Lib.Instance.Delay(1000);

            PrintWork(true);
            msg.Visibility = Visibility.Hidden;
        }
        private void menuRighPrint_Click(object sender, RoutedEventArgs e)
        {
            //if (dgdOutware.Items.Count == 0)
            //{
            //    MessageBox.Show("먼저 검색해 주세요.");
            //    return;
            //}
            //var OBJ = dgdOutware.SelectedItem as Win_out_Outware_Scan_View;
            //if (OBJ == null)
            //{
            //    MessageBox.Show("거래명세표 항목이 정확히 선택되지 않았습니다.");
            //    return;
            //}
            msg.Show();
            msg.Topmost = true;
            msg.Refresh();

            Lib.Instance.Delay(1000);

            PrintWork(false);
            msg.Visibility = Visibility.Hidden;
        }
        private void menuClose_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = btnPrint.ContextMenu;
            menu.StaysOpen = false;
            menu.IsOpen = false;
        }
        // 실제 엑셀작업
        private void PrintWork(bool previewYN)
        {
            excelapp = new Microsoft.Office.Interop.Excel.Application();

            string MyBookPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Report\\AfterCostDetail.xlsx";
            workbook = excelapp.Workbooks.Add(MyBookPath);
            worksheet = workbook.Sheets["Form"];
            pastesheet = workbook.Sheets["Print"];

            // 데이터 입력
            int rowCount = dgdSub.Items.Count - 1; // 상세항목 실제 행 개수(합계를 제외한)
            int excelStartRow = 7; // 엑셀의 데이터 값을 넣을 행 번호

            int copyLine = 0;
            int Page = 0;
            int PageAll = (int)Math.Ceiling(rowCount / 35.0);
            int DataCount = 0;


            // 오더넘버, 날짜 입력
            // 현재날짜 입력
            string today = DateTime.Now.ToString("yyyy.MM.dd");
            workrange = worksheet.get_Range("Z5");
            workrange.Value2 = today;
            workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
            workrange.Font.Size = 11;

            // 오더번호 입력
            var AfterCost = dgdMain.SelectedItem as Win_frm_Cst_AfterCostAnal_Q_CodeView;
            workrange = worksheet.get_Range("C5");
            workrange.Value2 = AfterCost.OrderNo;
            workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
            workrange.Font.Size = 11;

            while (rowCount > DataCount)
            {
                Page++;
                copyLine = ((Page - 1) * 43);

                int excelNum = 0;

                // 기존에 있는 데이터 지우기 "A7", "W41"
                //workrange =  worksheet.get_Range(worksheet.Cells["A7", "W41"]);
                //workrange.Delete();

                //workrange = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells["A7", "W41"];
                //workrange.Delete();
                worksheet.Range["A7", "W41"].EntireRow.ClearContents();

                for (int i = DataCount; i < rowCount; i++)
                {
                    if (i == 35 * Page)
                    {
                        break;
                    }

                    var AfterCostDetail = dgdSub.Items[i] as Win_frm_Cst_AfterCostAnal_Detail_CodeView;
                    int excelRow = excelStartRow + excelNum;

                    workrange = worksheet.get_Range("A" + excelRow);
                    workrange.Value2 = AfterCostDetail.Num;
                    workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    workrange.Font.Size = 11;

                    workrange = worksheet.get_Range("C" + excelRow);
                    workrange.Value2 = AfterCostDetail.CostItemName;
                    workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    workrange.Font.Size = 11;

                    workrange = worksheet.get_Range("R" + excelRow);
                    workrange.Value2 = StringFormatCommaD(AfterCostDetail.CostItemUnitPrice, 3);
                    workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    workrange.Font.Size = 11;

                    workrange = worksheet.get_Range("W" + excelRow);
                    workrange.Value2 = StringFormatCommaD(AfterCostDetail.CostItemAmount, 3);
                    workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    workrange.Font.Size = 11;

                    excelNum++;
                    DataCount = i;

                }

                // 합계 - 마지막줄에 (엑셀 42번행) / 총개수 를 만났을때 합계를 입력 및 출력
                if (DataCount == rowCount - 1)
                {
                    // 합계 입력
                    var SumAfterCostDetail = dgdSub.Items[rowCount] as Win_frm_Cst_AfterCostAnal_Detail_CodeView;

                    workrange = worksheet.get_Range("C" + 42);
                    workrange.Value2 = SumAfterCostDetail.CostItemName;
                    workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    workrange.Font.Size = 11;

                    workrange = worksheet.get_Range("R" + 42);
                    workrange.Value2 = StringFormatCommaD(SumAfterCostDetail.CostItemUnitPrice, 3);
                    workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    workrange.Font.Size = 11;

                    workrange = worksheet.get_Range("W" + 42);
                    workrange.Value2 = StringFormatCommaD(SumAfterCostDetail.CostItemAmount, 3);
                    workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    workrange.Font.Size = 11;

                }

                // 2장 이상 넘어가면 페이지 넘버 입력
                if (PageAll > 1)
                {
                    // M43 에 페이지 넘버
                    //workrange = worksheet.get_Range("M" + 43);
                    //workrange.Value2 = "'" + Page + " / " + PageAll;
                    //workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    //workrange.Font.Size = 9;

                    pastesheet.PageSetup.CenterFooter = "&P / &N";
                }

                // Form 시트 내용 Print 시트에 복사 붙여넣기
                worksheet.Select();
                worksheet.UsedRange.EntireRow.Copy();
                pastesheet.Select();
                workrange = pastesheet.Cells[copyLine + 1, 1];
                workrange.Select();
                pastesheet.Paste();

                DataCount++;

            }

            // 
            excelapp.Visible = true;
            msg.Hide();

            if (previewYN == true)
            {
                pastesheet.PrintPreview();
            }
            else
            {
                pastesheet.PrintOutEx();
            }
        }

        #endregion
        // 엑셀 버튼
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] dgdStr = new string[4];
            dgdStr[0] = "사후 원가 분석";
            dgdStr[1] = "사후 원가 분석 상세내역";
            dgdStr[2] = dgdMain.Name;
            dgdStr[3] = dgdSub.Name;

            ExportExcelxaml ExpExc = new ExportExcelxaml(dgdStr);
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
                else if (ExpExc.choice.Equals(dgdSub.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdSub);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdSub);

                    Name = dgdSub.Name;
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

        #endregion

        // 메인 그리드 선택 이벤트 → 선택 됬을때 해당되는 상세 정보를 출력
        private void dgdMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AfterCost = dgdMain.SelectedItem as Win_frm_Cst_AfterCostAnal_Q_CodeView;
            this.DataContext = AfterCost;
            if (AfterCost != null)
            {
                dgdMain.SelectedIndex = AfterCost.Num - 1;
                FillGrid_Detail(AfterCost.OrderID);
            }

        }

        // 재조회
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

        // 조회
        private void FillGrid()
        {
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear();
                dgdSub.Items.Clear();
            }

            try
            {
                if(CheckData())
                {
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();
                    sqlParameter.Add("chkDate", chkDateSrh.IsChecked == true ? 1 : 0);
                    sqlParameter.Add("FromDate", chkDateSrh.IsChecked == true ? orderSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                    sqlParameter.Add("ToDate", chkDateSrh.IsChecked == true ? orderEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                    sqlParameter.Add("chkCustom", chkCustomSrh.IsChecked == true ? 1 : 0);
                    sqlParameter.Add("CustomID", chkCustomSrh.IsChecked == true ? txtCustomSrh.Tag.ToString() : "");
                    sqlParameter.Add("chkArticleID", chkArticleSrh.IsChecked == true ? 1 : 0);
                    sqlParameter.Add("ArticleID", chkArticleSrh.IsChecked == true ? txtArticleSrh.Tag.ToString() : "");
                    sqlParameter.Add("chkOrderID", chkOrderNoSrh.IsChecked == true ? 1 : 0);
                    sqlParameter.Add("OrderID", chkOrderNoSrh.IsChecked == true ? txtOrderNoSrh.Tag.ToString() : "");
                    //sqlParameter.Add("nExchrate", Convert.ToDecimal(USD));
                    sqlParameter.Add("nExchrate", txtExchangeRate.Text == null || txtExchangeRate.Text.Trim() == "" ? 1 : Convert.ToDecimal(txtExchangeRate.Text) / 1000);

                    DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_CST_sAfterCostSum", sqlParameter, false);

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
                                var AfterCost = new Win_frm_Cst_AfterCostAnal_Q_CodeView()
                                {
                                    Num = i,
                                    OrderID = dr["OrderID"].ToString(),
                                    OrderNo = dr["OrderNO"].ToString(),
                                    KCustom = dr["KCustom"].ToString(),
                                    OutQty = Decimal6Format(dr["OutQty"]),
                                    OutAmountY = Decimal6Format(dr["OutAmountY"]),
                                    OutAmount = Decimal6Format(dr["OutAmount"]),
                                    AfterCostY = Decimal6Format(dr["AfterCostY"]),
                                    AfterCost = Decimal6Format(dr["AfterCost"]),
                                    ProfitY = Decimal6Format(dr["profitY"]),
                                    Profit = Decimal6Format(dr["profit"])
                                };

                                dgdMain.Items.Add(AfterCost);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }

        // 상세항목 조회
        private void FillGrid_Detail(string OrderID)
        {
            Decimal sumUtilPrice = 0;
            Decimal sumAmount = 0;

            if (dgdSub.Items.Count > 0)
            {
                dgdSub.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("OrderID", OrderID);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_CST_sAfterCostDetail", sqlParameter, false);

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

                            // 합계 더하기 → 소수로 변환 가능한지 체크
                            if (CheckConvertDecimal(dr["CostItemUnitPrice"].ToString()) == true
                                    && CheckConvertDecimal(dr["CostItemAmount"].ToString()) == true)
                            {
                                sumUtilPrice += Decimal.Parse(dr["CostItemUnitPrice"].ToString());
                                sumAmount += Decimal.Parse(dr["CostItemAmount"].ToString());
                            }

                            var AfterCostDetail = new Win_frm_Cst_AfterCostAnal_Detail_CodeView()
                            {
                                Num = i,
                                CostGbnName = dr["CostGbnName"].ToString(),
                                CostItemName = dr["CostItemName"].ToString(),
                                CostItemUnitPrice = Decimal6Format(dr["CostItemUnitPrice"]),
                                CostItemAmount = Decimal6Format(dr["CostItemAmount"])
                            };

                            dgdSub.Items.Add(AfterCostDetail);

                            // 합계 마지막 줄에 추가
                            if (i == drc.Count)
                            {
                                i++;
                                var SumAfterCostDetail = new Win_frm_Cst_AfterCostAnal_Detail_CodeView()
                                {
                                    Num = i,
                                    CostGbnName = "",
                                    CostItemName = "합계",
                                    CostItemUnitPrice = Decimal6Format(sumUtilPrice),
                                    CostItemAmount = Decimal6Format(sumAmount)
                                };

                                dgdSub.Items.Add(SumAfterCostDetail);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }

        //체크 데이터
        private bool CheckData()
        {
            bool flag = true;

            // 수주일자 체크 ON → 날짜 입력하지 않았을 때
            if (chkDateSrh.IsChecked == true
                    && ((orderSDate.SelectedDate == null || orderSDate.SelectedDate.Value.Equals(""))
                    || orderEDate.SelectedDate == null || orderEDate.SelectedDate.Value.Equals("")))
            {
                MessageBox.Show("수주일자가 입력되지 않았습니다.");
                flag = false;
                return flag;
            }
            // 거래처 체크 ON → 거래처 입력하지 않았을 때
            if (chkCustomSrh.IsChecked == true
                && (txtCustomSrh.Tag == null || txtCustomSrh.Tag.ToString().Equals("")))
            {
                MessageBox.Show("거래처가 입력되지 않았습니다.");
                flag = false;
                return flag;
            }

            // 품명 체크 ON → 품명 입력하지 않았을 때
            if (chkArticleSrh.IsChecked == true
                && (txtArticleSrh.Tag == null || txtArticleSrh.Tag.ToString().Equals("")))
            {
                MessageBox.Show("품명이 입력되지 않았습니다.");
                flag = false;
                return flag;
            }

            // 오더번호 체크 ON → 오더번호 입력하지 않았을 때
            if (chkOrderNoSrh.IsChecked == true
               && (txtOrderNoSrh.Tag == null || txtOrderNoSrh.Tag.ToString().Equals("")))
            {
                MessageBox.Show("오더번호가 입력되지 않았습니다.");
                flag = false;
                return flag;
            }

            // 환율 입력하지 않았거나 → 소수로 변환이 가능한지 체크
            //if (txtExchangeRate.Text == null || txtExchangeRate.Text.Trim().Equals(""))
            //{
            //    MessageBox.Show("환율이 입력되지 않았습니다.");
            //    flag = false;
            //    return flag;
            //}
            //else 
            if (txtExchangeRate.Text != null 
                    && txtExchangeRate.Text.Trim().Equals("") == false
                    && CheckConvertDecimal(txtExchangeRate.Text) == false)
            {
                MessageBox.Show("환율은 숫자만 입력이 가능합니다.");
                flag = false;
                return flag;
            }

            return flag;
        }

        // API 이용 → 환율 구하기(달러 : USD)
        private string getExchangeRate(string Country)
        {
            string exchangeRate = "";
            // 한국 수출입은행 API
            string key = "il3mwjOdDgoRPw5wCqcR87lvtJvggU8g";
            string searchdate = "20190830";

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                string jsonURL = "https://www.koreaexim.go.kr/site/program/financial/exchangeJSON?authkey="
                    + key + "&searchdate=" + searchdate + "&data=AP01";
                string json = wc.DownloadString(jsonURL);
                JArray array = JArray.Parse(json);
                foreach (JObject job in array)
                {
                    // 달러
                    if (job["cur_unit"].ToString().Equals(Country))
                        exchangeRate = job["deal_bas_r"].ToString();
                }
            }

            return exchangeRate;
        }

        // 소수점 6자리 포맷 (뒤의 불필요한 0 제거하기)
        public string Decimal6Format(Object dec)
        {
            string result = string.Format("{0:0.######}", dec);

            return result;
        }

        // string → Decimal 변환 가능한지 체크
        public bool CheckConvertDecimal(string dec)
        {
            bool flag = true;
            Decimal chkDecimal = 0;

            if (Decimal.TryParse(dec, out chkDecimal) == false)
                flag = false;

            return flag;
        }

        // 문자열 천단위 콤마 찍기 string → Double  → string 
        // Dobule로 변환이 불가능하면 원본 문자 반환
        // parameter - decimalDigit : 소수점 자릿수
        public string StringFormatCommaD(string str, int decimalDigit)
        {
            string result = str;
            double chkDouble = 0;
            double num = 0;

            if (Double.TryParse(str, out chkDouble) == true)
            {
                num = Double.Parse(str);

                result = string.Format("{0:N" + decimalDigit + "}", num);
            }

            return result;
        }

    }

    #region CodeView
    // 메인 그리드 객체
    class Win_frm_Cst_AfterCostAnal_Q_CodeView : BaseView
    {
        public int Num { get; set; }
        public string OrderID { get; set; }
        public string OrderNo { get; set; }
        public string KCustom { get; set; }
        public string OutQty { get; set; }
        public string OutAmountY { get; set; }
        public string OutAmount { get; set; }
        public string AfterCostY { get; set; }
        public string AfterCost { get; set; }
        public string ProfitY { get; set; }
        public string Profit { get; set; }
    }

    // 상세항목 객체
    class Win_frm_Cst_AfterCostAnal_Detail_CodeView : BaseView
    {
        public int Num { get; set; }
        public string CostGbnName { get; set; }
        public string CostItemName { get; set; }
        public string CostItemUnitPrice { get; set; }
        public string CostItemAmount { get; set; }
    }
    #endregion
}
