using System;
using System.Collections.Generic;
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
    /// Win_Acc_Cash_Summary_Q.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Acc_Cash_Summary_Q : UserControl
    {
        private Microsoft.Office.Interop.Excel.Application excelapp;
        private Microsoft.Office.Interop.Excel.Workbook workbook;
        private Microsoft.Office.Interop.Excel.Worksheet worksheet;
        private Microsoft.Office.Interop.Excel.Range workrange;
        private Microsoft.Office.Interop.Excel.Worksheet stempsheet;
        private Microsoft.Office.Interop.Excel.Worksheet copysheet;
        private Microsoft.Office.Interop.Excel.Worksheet pastesheet;
        // 엑셀 활용 용도 (프린트)

        WizMes_Alpha_JA.PopUp.NoticeMessage msg = new PopUp.NoticeMessage();
        //(기다림 알림 메시지창)

        public Win_Acc_Cash_Summary_Q()
        {
            InitializeComponent();
        }

        //로드이벤트
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);

            chkPeriod.IsChecked = true;
            dtpSDate.SelectedDate = DateTime.Today;
            dtpEDate.SelectedDate = DateTime.Today;
                       
        }

        //조회기간 라벨 이벤트
        private void lblPeriod_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(chkPeriod.IsChecked == true)
            {
                chkPeriod.IsChecked = false;
            }
            else
            {
                chkPeriod.IsChecked = true;
            }
        }

        //조회기간 체크 이벤트
        private void chkPeriod_Checked(object sender, RoutedEventArgs e)
        {
            dtpSDate.IsEnabled = true;
            dtpEDate.IsEnabled = true;
        }

        //조회기간 체크 해제 이벤트
        private void chkPeriod_Unchecked(object sender, RoutedEventArgs e)
        {
            dtpSDate.IsEnabled = false;
            dtpEDate.IsEnabled = false;
        }

        //전월
        private void btnLastMonth_Click(object sender, RoutedEventArgs e)
        {
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

        //금월
        private void btnThisMonth_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[0];
            dtpEDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[1];
        }

        //전일
        private void btnYesterday_Click(object sender, RoutedEventArgs e)
        {
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

        //금일
        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = DateTime.Today;
            dtpEDate.SelectedDate = DateTime.Today;
        }

        //거래처 라벨 이벤트
        private void lblCustom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(chkCustom.IsChecked == true)
            {
                chkCustom.IsChecked = false;
            }
            else
            {
                chkCustom.IsChecked = true;
            }
        }

        //거래처 체크 이벤트
        private void chkCustom_Checked(object sender, RoutedEventArgs e)
        {
            txtCustom.IsEnabled = true;
            btnPfCustom.IsEnabled = true;
        }

        //거래처 체크해제 이벤트
        private void chkCustom_Unchecked(object sender, RoutedEventArgs e)
        {
            txtCustom.IsEnabled = false;
            btnPfCustom.IsEnabled = false;
        }

        //거래처 키다운
        private void txtCustom_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
            }
        }

        //거래처 플러스 파인더
        private void btnPfCustom_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
        }

        //계정과목 라벨 이벤트
        private void LblSubject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(chkSubject.IsChecked == true)
            {
                chkSubject.IsChecked = false;
            }
            else
            {
                chkSubject.IsChecked = true;
            }
        }

        //계정과목 체크 이벤트
        private void chkSubject_Checked(object sender, RoutedEventArgs e)
        {
            txtSubject.IsEnabled = true;
            btnpfSubject.IsEnabled = true;
        }

        //계정과목 체크해제 이벤트
        private void chkSubject_Unchecked(object sender, RoutedEventArgs e)
        {
            txtSubject.IsEnabled = false;
            btnpfSubject.IsEnabled = false;
        }

        //계정과목 키다운
        private void txtSubject_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtSubject, 82, "");
            }
        }

        //계정과목 플러스 파인더
        private void btnpfpSubject_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtSubject, 82, "");
        }

        //검색
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            //검색버튼을 누르기 전에 집계항목 중 하나라도 체크가 되어 있어야 한다.)
            if (chkCollectionArticle.IsChecked == false
                && chkCollectionCustom.IsChecked == false
                && chkCollectionMonth.IsChecked == false
                && chkCollectionYear.IsChecked == false)
            {
                MessageBox.Show("집계항목 중 하나라도 체크가 되어 있어야 합니다.");
                return;
            }
            else
            {
                re_Search();
            }
        }

        //닫기
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        //엑셀변환 요청하신 엑셀 파일로 수정. 2020.11.03, 장가빈
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            string sDate = dtpSDate.SelectedDate.Value.ToString("yyyyMMdd");
            string eDate = dtpEDate.SelectedDate.Value.ToString("yyyyMMdd");

            try
            {
                #region 기존 엑셀 이벤트

                //DataTable dt = null;
                //string Name = string.Empty;

                //string[] dgdStr = new string[2];
                //dgdStr[0] = "현금출납 리스트";
                //dgdStr[1] = dgdCashSummary.Name;


                //ExportExcelxaml ExpExc = new ExportExcelxaml(dgdStr);
                //ExpExc.ShowDialog();

                //if (ExpExc.DialogResult.HasValue)
                //{
                //    if (ExpExc.choice.Equals(dgdCashSummary.Name))
                //    {
                //        if (ExpExc.Check.Equals("Y"))
                //            dt = Lib.Instance.DataGridToDTinHidden(dgdCashSummary);
                //        else
                //            dt = Lib.Instance.DataGirdToDataTable(dgdCashSummary);

                //        Name = dgdCashSummary.Name;
                //        if (Lib.Instance.GenerateExcel(dt, Name))
                //            Lib.Instance.excel.Visible = true;
                //        else
                //            return;
                //    }
                //    else
                //    {
                //        if (dt != null)
                //        {
                //            dt.Clear();
                //        }
                //    }
                //}

                #endregion 기존 엑셀 이벤트

                #region 호작질을 시작해보자.

                // 년, 월, 거래처별 금액 합계 재조회.
                DataTable dt = get_Cash_SummayList(sDate, eDate);

                // 엑셀 시작
                excelapp = new Microsoft.Office.Interop.Excel.Application();

                string MyBookPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Report\\현금출납집계표 양식.xls";
                workbook = excelapp.Workbooks.Add(MyBookPath);
                worksheet = workbook.Sheets["Form"];
                stempsheet = workbook.Sheets["Stemp"];
                pastesheet = workbook.Sheets["pastesheet"];


                //페이지 계산
                int rowCount = 0;
                int copyLine = 0;           //??

                DataRowCollection drc = dt.Rows;
                foreach (DataRow dr in drc)
                {
                    rowCount++;              //반영할 데이터 갯수 rowCount ㅜㅜ 이렇게 밖에 모루겠다.
                }

                int Page = 1;               //페이지 변수
                int PageAll = (int)Math.Ceiling(rowCount / 38.0);       //전체페이지 변수
                int DataCount = 0;          //데이터 반영 활용 변수
                int excelNum = 0;                  //엑셀 행번호 변수


                int startRowIndex = 5; // 시작하는 행
                //int endRowIndex = 38; // 마지막 행

                int excelRow = 0;

                for (int k = 0; k < dt.Rows.Count; k++)
                {

                    if (DataCount == 38 * Page)     //페이지 수 곱하기 한 페이지에 들어갈 수 있는 데이터 값과 같아지면
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
                            copyLine = ((Page - 1) * 44);      // copy 시작 값

                            // 기존에 있는 데이터 지우기
                            worksheet.Range["A5", "H42"].EntireRow.ClearContents();
                            // 행번호 5번부터 시작하도록 초기화
                            excelRow = startRowIndex;
                            excelNum = 0;


                        }
                    }

                    DataRow dr = dt.Rows[k];

                    if (k == 0) // 최초 한번 입력
                    {
                        // 일자 : 2020년 10월
                        workrange = worksheet.get_Range("A2");
                        workrange.Value2 = dr["YYYY"].ToString() + "년" + dr["MM"].ToString() + "월";
                    }

                    //엑셀 행 지정
                    excelRow = startRowIndex + excelNum;

                    // 순번
                    workrange = worksheet.get_Range("A" + excelRow);
                    workrange.Value2 = k + 1;

                    // 년도
                    workrange = worksheet.get_Range("B" + excelRow);
                    workrange.Value2 = dr["YYYY"].ToString();

                    // 월
                    workrange = worksheet.get_Range("C" + excelRow);
                    workrange.Value2 = dr["MM"].ToString();

                    // 거래처
                    workrange = worksheet.get_Range("D" + excelRow);
                    workrange.Value2 = dr["KCustom"].ToString().Trim();

                    // 거래처명
                    workrange = worksheet.get_Range("E" + excelRow);
                    workrange.Value2 = dr["KCustomName"].ToString().Trim();

                    // 계정과목
                    workrange = worksheet.get_Range("F" + excelRow);
                    workrange.Value2 = dr["BSItem"].ToString().Trim();

                    // 입금
                    workrange = worksheet.get_Range("G" + excelRow);
                    workrange.Value2 = dr["InAmount"].ToString().Equals("0.00") ? "" : chkNullNum(dr["InAmount"]);

                    // 출금
                    workrange = worksheet.get_Range("H" + excelRow);
                    workrange.Value2 = dr["OutAmount"].ToString().Equals("0.00") ? "" : chkNullNum(dr["OutAmount"]);

                    DataCount++; // 데이터 변수 1증가
                    excelNum++; // 행 번호 임시변수 1증가
                }

                if (DataCount == rowCount)        //마지막페이지의 경우
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

                // 기본 폼 활성화 후 보이도록
                pastesheet.Activate();
                pastesheet.Range["A1"].Select();

                excelapp.Visible = true;
                msg.Hide();


                #endregion 호작질을 시작해보자.

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

        //엑셀변환시 사용 프로시저. 2020.11.03, 
        private DataTable get_Cash_SummayList(string sDate, string eDate)
        {
            DataTable dt = new DataTable();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("sDate", sDate);
                sqlParameter.Add("eDate", eDate);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_Cash_Summary_Q_ForExcel", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
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

            return dt;
        }




        //데이터 그리드 셀렉션 체인지드
        private void DgdCashSummary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //없어도 될 듯..
        }


        #region FillGrid

        //재조회
        private void re_Search()
        {
               FillGrid_Cash();
        }

        //조회
        private void FillGrid_Cash()
        {
            if (dgdCashSummary.Items.Count > 0)
            {
                dgdCashSummary.Items.Clear();
            }

            try
            {
                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("chkDate", chkPeriod.IsChecked == true ? 1 : 0);
                sqlParameter.Add("SDate", chkPeriod.IsChecked == true && dtpSDate.SelectedDate != null
                                                            ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("EDate", chkPeriod.IsChecked == true && dtpEDate.SelectedDate != null
                                                            ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("CustomID", chkCustom.IsChecked == true && txtCustom.Tag != null ? txtCustom.Tag.ToString() : "");
                sqlParameter.Add("RPItemCode", chkSubject.IsChecked == true && txtSubject.Tag != null ? txtSubject.Tag.ToString() : "");

                sqlParameter.Add("chkCollectionYear", chkCollectionYear.IsChecked == true ? 1 : 0);            //년
                sqlParameter.Add("chkCollectionMonth", chkCollectionMonth.IsChecked == true ? 1 : 0);           //월
                sqlParameter.Add("chkCollectionCustom", chkCollectionCustom.IsChecked == true ? 1 : 0);          //거래처
                sqlParameter.Add("chkCollectionArticle", chkCollectionArticle.IsChecked == true ? 1 : 0);        //항목명


                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_Cash_Sum_Q", sqlParameter, false);

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
                            if(dr["cls"].ToString() == "1")
                            {
                                var WinAccCash = new Win_Acc_Cash_Summary_Q_CodeView()
                                {
                                    Num = i + 1,

                                    YYYY = dr["YYYY"].ToString(),
                                    MM = dr["MM"].ToString(),

                                    //RPDate = DatePickerFormat(dr["RPDate"].ToString()),
                                    //RPGBN = dr["RPGBN"].ToString(),
                                    CustomID = dr["CustomID"].ToString(),
                                    KCustom = dr["KCustom"].ToString(),
                                    KCustomName = dr["KCustomName"].ToString(),

                                    BSItem = dr["BSItem"].ToString(),
                                    //RPItemCode = dr["RPItemCode"].ToString(),
                                    InAmount = stringFormatN0(dr["InAmount"]),
                                    OutAmount = stringFormatN0(dr["OutAmount"]),
                                    //Comments = dr["Comments"].ToString(),
                                };

                                //값이 0 일 때는 빈 값으로 보여주기
                                if (WinAccCash.InAmount != null && WinAccCash.InAmount == "0")
                                {
                                    WinAccCash.InAmount = "";
                                }
                                if (WinAccCash.OutAmount != null && WinAccCash.OutAmount == "0")
                                {
                                    WinAccCash.OutAmount = "";
                                }

                                dgdCashSummary.Items.Add(WinAccCash);
                                
                            }
                            else if (dr["cls"].ToString() == "2")
                            {
                                var WinAccCash = new Win_Acc_Cash_Summary_Q_CodeView()
                                {
                                    Num = i + 1,

                                    YYYY =  chkCollectionYear.IsChecked == true ? "총계" : "",
                                    MM = chkCollectionYear.IsChecked == false && chkCollectionMonth.IsChecked == true ? "총계" : "",

                                    //RPDate = DatePickerFormat(dr["RPDate"].ToString()),
                                    //RPGBN = dr["RPGBN"].ToString(),
                                    CustomID = dr["CustomID"].ToString(),
                                    KCustom = chkCollectionYear.IsChecked == false && chkCollectionMonth.IsChecked == false 
                                                && chkCollectionCustom.IsChecked == true ?  "총계" : "",
                                    KCustomName = "",

                                    BSItem = chkCollectionYear.IsChecked == false && chkCollectionMonth.IsChecked == false
                                                && chkCollectionCustom.IsChecked == false && chkCollectionArticle.IsChecked == true ? "총계" : "",
                                    //RPItemCode = dr["RPItemCode"].ToString(),
                                    InAmount = stringFormatN0(dr["InAmount"]),
                                    OutAmount = stringFormatN0(dr["OutAmount"]),
                                    //Comments = dr["Comments"].ToString(),

                                    ColorLightGray = "true",
                                };

                                dgdCashSummary.Items.Add(WinAccCash);
                            }
                            i++;
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

        #endregion FrillGrid


        #region 기타 메서드 모음

        // 천단위 콤마, 소수점 버리기
        private string stringFormatN0(object obj)
        {
            return string.Format("{0:N0}", obj);
        }

        // 천마리 콤마, 소수점 두자리
        private string stringFormatN2(object obj)
        {
            return string.Format("{0:N2}", obj);
        }

        // 데이터피커 포맷으로 변경
        private string DatePickerFormat(string str)
        {
            string result = str;

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

        private object chkNullNum(object num)
        {
            if (num == null) { return 0; }

            return num;
        }


        #endregion

        //매출 / 매입 토글버튼
        #region 매출입 토글버튼
        private void chkCollectionYear_Checked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionYear_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionMonth_Checked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionMonth_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionCustom_Checked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionCustom_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionArticle_Checked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionArticle_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        #endregion

        #region (집계항목 체크 및 그리드 visible 작업) CollectionListAddMinusWithDatagrid
        private void CollectionListAddMinusWithDatagrid()
        {
            int i = 1;
            dgdtxtcolYear.Visibility = Visibility.Hidden;
            dgdtxtcolMonth.Visibility = Visibility.Hidden;
            dgdtxtcolCustom.Visibility = Visibility.Hidden;
            dgdCustomName.Visibility = Visibility.Hidden;
            dgdtxtcolArticle.Visibility = Visibility.Hidden;  //매출항목

            //tbkCollection1.Text = string.Empty;
            //tbkCollection2.Text = string.Empty;
            //tbkCollection3.Text = string.Empty;
            //tbkCollection4.Text = string.Empty;

            if (chkCollectionYear.IsChecked == true)
            {
                //tbkCollection1.Text = i.ToString();
                //tbkCollection1.Margin = new Thickness(3, 3, 3, 3);
                dgdtxtcolYear.Visibility = Visibility.Visible;
                i++;
            }
            if (chkCollectionMonth.IsChecked == true)
            {
                //tbkCollection2.Text = i.ToString();
                //tbkCollection2.Margin = new Thickness(3, 3, 3, 3);
                dgdtxtcolMonth.Visibility = Visibility.Visible;
                i++;
            }
            if (chkCollectionCustom.IsChecked == true)
            {
                //tbkCollection3.Text = i.ToString();
                //tbkCollection3.Margin = new Thickness(3, 3, 3, 3);
                dgdtxtcolCustom.Visibility = Visibility.Visible;
                dgdCustomName.Visibility = Visibility.Visible;
                i++;
            }
            if (chkCollectionArticle.IsChecked == true)
            {
                //tbkCollection4.Text = i.ToString();
                //tbkCollection4.Margin = new Thickness(3, 3, 3, 3);
                dgdtxtcolArticle.Visibility = Visibility.Visible;
                i++;
            }

            if (chkCollectionArticle.IsChecked == false
                && chkCollectionCustom.IsChecked == false
                && chkCollectionMonth.IsChecked == false
                && chkCollectionYear.IsChecked == false)
            {
                MessageBox.Show("집계항목 중 하나라도 체크가 되어 있어야 합니다.");
                return;
            }
            else
            {
                FillGrid_Cash();
            }
        }
        #endregion (집계항목 체크 및 그리드 visible 작업) CollectionListAddMinusWithDatagrid


    }

    class Win_Acc_Cash_Summary_Q_CodeView
    {
        public int Num { get; set; }

        public string YYYY { get; set; }
        public string MM { get; set; }

        public string RPDate { get; set; }
        public string RPGBN { get; set; }
        public string CustomID { get; set; }
        public string KCustom { get; set; }
        public string KCustomName { get; set; }

        public string BSItem { get; set; }
        public string RPItemCode { get; set; }
        public string InAmount { get; set; }
        public string OutAmount { get; set; }
        public string Comments { get; set; }

        public string ColorLightLightGray { get; set; }
        public string ColorLightGray { get; set; }
        public string ColorGray { get; set; }
    }
}
