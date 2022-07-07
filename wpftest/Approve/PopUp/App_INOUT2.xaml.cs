using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WizMes_Alpha_JA.PopUp
{
    /// <summary>
    /// MuniChoice.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App_INOUT2 : Window
    {
        Lib lib = new Lib();

        public bool SrhFlag = false;
        public string strFlag = "";

        public string AppReqID = "";
        //public bool MuniDataCountZero = false;
        //public string SelectTextFileName = string.Empty;
        //public string SelectM04PlusData = string.Empty;

        public DateTime ReqDate = DateTime.Today;

        public List<App_IN_CodeView2> lstAppIn = new List<App_IN_CodeView2>();
        public List<App_IN_CodeView2> lstAppOut = new List<App_IN_CodeView2>();

        public List<string> lstChkIn = new List<string>();
        public List<string> lstChkOut = new List<string>();

        // 인쇄 활용 객체
        private Microsoft.Office.Interop.Excel.Application excelapp;
        private Microsoft.Office.Interop.Excel.Workbook workbook;
        private Microsoft.Office.Interop.Excel.Worksheet worksheet;
        private Microsoft.Office.Interop.Excel.Range workrange;
        private Microsoft.Office.Interop.Excel.Worksheet stempsheet;
        private Microsoft.Office.Interop.Excel.Worksheet pastesheet;
        WizMes_Alpha_JA.PopUp.NoticeMessage msg = new WizMes_Alpha_JA.PopUp.NoticeMessage();

        public string AppStepID = string.Empty; // 엑셀 변환 시, 결재 순서 작성을 위한 변수

        #region FTP 변수들

        string strImagePath = string.Empty;
        string strFullPath = string.Empty;

        List<string[]> listFtpFile = new List<string[]>();
        List<string[]> deleteListFtpFile = new List<string[]>(); // 삭제할 파일 리스트
        private FTP_EX _ftp = null;

        //string FTP_ADDRESS = "ftp://wizis.iptime.org/ImageData";
        //string FTP_ADDRESS = "ftp://wizis.iptime.org/ImageData/Draw";
        string FTP_ADDRESS = "ftp://" + LoadINI.FileSvr + ":" + LoadINI.FTPPort + LoadINI.FtpImagePath + "/Approval";
        private const string FTP_ID = "wizuser";
        private const string FTP_PASS = "wiz9999";
        private const string LOCAL_DOWN_PATH = "C:\\Temp";

        //string FTP_ADDRESS = "ftp://192.168.0.28/Approval";

        #endregion // FTP 변수들

        private bool firstFlag = false;

        public App_INOUT2()
        {
            InitializeComponent();
        }

        public App_INOUT2(DateTime ReqDate)
        {
            InitializeComponent();

            this.ReqDate = ReqDate;
        }

        public App_INOUT2(DateTime ReqDate, List<string> lstChkIn, List<string> lstChkOut)
        {
            InitializeComponent();

            this.ReqDate = ReqDate;

            this.lstChkIn = lstChkIn;
            this.lstChkOut = lstChkOut;
        }

        #region 검색조건

        // 매입 일자 체크 이벤트
        private void lblDate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (chkDate.IsChecked == true)
            {
                chkDate.IsChecked = false;
            }
            else
            {
                chkDate.IsChecked = true;
            }
        }
        private void chkDate_Checked(object sender, RoutedEventArgs e)
        {
            chkDate.IsChecked = true;
            dtpSDate.IsEnabled = true;
            dtpEDate.IsEnabled = true;
        }
        private void chkDate_Unchecked(object sender, RoutedEventArgs e)
        {
            chkDate.IsChecked = false;
            dtpSDate.IsEnabled = false;
            dtpEDate.IsEnabled = false;
        }

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

        #endregion // 검색조건

        // 확인버튼 클릭.
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            lstChkIn.Clear();
            lstAppIn.Clear();

            lstChkOut.Clear();
            lstAppOut.Clear();

            for (int i = 0; i < dgdMain.Items.Count; i++)
            {
                var AppIn = dgdMain.Items[i] as App_IN_CodeView2;

                if (AppIn != null
                    && AppIn.Chk == true)
                {
                    lstChkIn.Add(AppIn.BSNo);
                    lstAppIn.Add(AppIn);
                }
            }

            for (int i = 0; i < dgdSub.Items.Count; i++)
            {
                var AppOut = dgdSub.Items[i] as App_IN_CodeView2;

                if (AppOut != null
                    && AppOut.Chk == true)
                {
                    lstChkOut.Add(AppOut.BSNo);
                    lstAppOut.Add(AppOut);
                }
            }

            DialogResult = true;
        }

        // 확인버튼 클릭.
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region 매입매출 조회 메서드

        private void App_INOUT2_Loaded(object sender, RoutedEventArgs e)
        {
            //Application curApp = Application.Current;
            //Window mainWindow = curApp.MainWindow;
            //this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            //this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;


            if (SrhFlag == false) // 결재 처리에서는 체크박스랑 데이트피커 안보이게설정
            {
                grdSrh1.Visibility = Visibility.Hidden;
                grdBottom.Visibility = Visibility.Hidden;

                dgdMain_Chk.Visibility = Visibility.Hidden;

                FillGrid_InApp();
            }
            else
            {
                grdSrh1.Visibility = Visibility.Visible;
                grdBottom.Visibility = Visibility.Visible;

                dgdMain_Chk.Visibility = Visibility.Visible;

                //DateTime ToDate = ReqDate.AddDays(-30);

                // 2020.11.04 요청에 의한 검색조건 당일로 변경
                dtpSDate.SelectedDate = DateTime.Today;
                dtpEDate.SelectedDate = DateTime.Today;

                if (!strFlag.Equals("U"))
                {
                    chkDate.IsChecked = true;
                }


                FillGrid(strFlag);
                //FillGridSub(strFlag);               

            }            
        }

        #endregion

        #region 결재등록에서 조회 메서드 - 매입

        private void FillGrid(string strFlag)
        {
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear();
            }

            try
            {

                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("JobFlag", strFlag.Trim().Equals("I") ? 0 : 1); // 0 이면 추가 : 결재에 올라가있지 않은 건만!!! / 1 이면 수정 : 현 결재건의 매입리스트 + 결재되어있지 않은건!!!!!!!!!!
                sqlParameter.Add("AppReqID", AppReqID); // 
                sqlParameter.Add("bsGbnID", "1"); // 매입 : 1
                sqlParameter.Add("sBSDate", chkDate.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sDate", dtpSDate.SelectedDate != null ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("eDate", dtpEDate.SelectedDate != null ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("CustomID", "");
                sqlParameter.Add("BSItemCode", "");
                sqlParameter.Add("ArticleID", "");
                sqlParameter.Add("ApprovalYN", 1); // 0 : 전체 , 1 : 안된것, 2 : 된것

                ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sAppBuyList_ForIU", sqlParameter, false);

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
                            var WinAccBuySale = new App_IN_CodeView2()
                            {
                                Num = i + 1,
                                //Chk = strFlag.Trim().Equals("U") && dr["AppReqID"].ToString().Equals("") ? false : true,

                                BSNo = dr["BSNo"].ToString(),
                                BSGBN = dr["BSGBN"].ToString(),
                                BSDate = DatePickerFormat(dr["BSDate"].ToString()),
                                CustomID = dr["CustomID"].ToString(),
                                InOutCustomID = dr["InOutCustomID"].ToString(),

                                RollQty = stringFormatN0(dr["RollQty"]),
                                QTY = stringFormatN0(dr["QTY"]),
                                UnitPrice = stringFormatN0(dr["UnitPrice"]),
                                VatINDYN = dr["VatINDYN"].ToString(),
                                AMOUNT = stringFormatN0(dr["AMOUNT"]),

                                VATAmount = stringFormatN0(dr["VATAmount"]),
                                TotalAmount = stringFormatN0(dr["TotalAmount"]),
                                CurrencyUnit = dr["CurrencyUnit"].ToString(),
                                InOutNo = dr["InOutNo"].ToString(),
                                InOutNoSEQ = dr["InOutNoSEQ"].ToString(),

                                ArticleID = dr["ArticleID"].ToString(),
                                Article = dr["Article"].ToString(),
                                ORDERNO = dr["ORDERNO"].ToString(),
                                OrderID = dr["OrderID"].ToString(),
                                BSITEM = dr["BSITEM"].ToString(),

                                BSItemName = dr["BSItemName"].ToString(),
                                KCustom = dr["KCustom"].ToString(),
                                Comments = dr["Comments"].ToString(),

                                QTY_CV = ConvertDouble(dr["QTY"].ToString()),
                                UnitPrice_CV = ConvertDouble(dr["UnitPrice"].ToString()),
                                AMOUNT_CV = ConvertDouble(dr["AMOUNT"].ToString()),
                                VATAmount_CV = ConvertDouble(dr["VATAmount"].ToString()),
                            };

                            // 화폐단위
                            if (WinAccBuySale.CurrencyUnit.Trim().Equals("0"))
                            {
                                WinAccBuySale.CurrencyUnitName = "₩";
                            }
                            else
                            {
                                WinAccBuySale.CurrencyUnitName = "$";
                            }

                            // 만약에 lstInChk 에 있다면 체크하기
                            if (IsInCheckList(lstChkIn, WinAccBuySale.BSNo)) { WinAccBuySale.Chk = true; }

                            dgdMain.Items.Add(WinAccBuySale);
                            i++;
                        }
                    }
                }

                setCalculateAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion

        #region lstInChk / lstOutChk 안에 존재하는지 확인

        private bool IsInCheckList(List<string> lst, string BSNo)
        {
            bool flag = false;

            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].Equals(BSNo))
                {
                    flag = true;
                }
            }

            return flag;
        }
 
        #endregion

        #region 결재등록에서 조회 메서드 - 매출

        private void FillGridSub(string strFlag)
        {
            if (dgdSub.Items.Count > 0)
            {
                dgdSub.Items.Clear();
            }

            try
            {

                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("JobFlag", strFlag.Trim().Equals("I") ? 0 : 1); // 0 이면 추가 : 결재에 올라가있지 않은 건만!!! / 1 이면 수정 : 현 결재건의 매입리스트 + 결재되어있지 않은건!!!!!!!!!!
                sqlParameter.Add("AppReqID", AppReqID); // 
                sqlParameter.Add("bsGbnID", "2"); // 매출 : 2
                sqlParameter.Add("sBSDate", chkDate.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sDate", dtpSDate.SelectedDate != null ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("eDate", dtpEDate.SelectedDate != null ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("CustomID", "");
                sqlParameter.Add("BSItemCode", "");
                sqlParameter.Add("ArticleID", "");
                sqlParameter.Add("ApprovalYN", 1); // 0 : 전체 , 1 : 안된것, 2 : 된것

                ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sAppBuyList_ForIU", sqlParameter, false);

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
                            var WinAccBuySale = new App_IN_CodeView2()
                            {
                                Num = i + 1,
                                //Chk = strFlag.Trim().Equals("U") && dr["AppReqID"].ToString().Equals("") ? false : true,

                                BSNo = dr["BSNo"].ToString(),
                                BSGBN = dr["BSGBN"].ToString(),
                                BSDate = DatePickerFormat(dr["BSDate"].ToString()),
                                CustomID = dr["CustomID"].ToString(),
                                InOutCustomID = dr["InOutCustomID"].ToString(),

                                RollQty = stringFormatN0(dr["RollQty"]),
                                QTY = stringFormatN0(dr["QTY"]),
                                UnitPrice = stringFormatN0(dr["UnitPrice"]),
                                VatINDYN = dr["VatINDYN"].ToString(),
                                AMOUNT = stringFormatN0(dr["AMOUNT"]),

                                VATAmount = stringFormatN0(dr["VATAmount"]),
                                TotalAmount = stringFormatN0(dr["TotalAmount"]),
                                CurrencyUnit = dr["CurrencyUnit"].ToString(),
                                InOutNo = dr["InOutNo"].ToString(),
                                InOutNoSEQ = dr["InOutNoSEQ"].ToString(),

                                ArticleID = dr["ArticleID"].ToString(),
                                Article = dr["Article"].ToString(),
                                ORDERNO = dr["ORDERNO"].ToString(),
                                OrderID = dr["OrderID"].ToString(),
                                BSITEM = dr["BSITEM"].ToString(),

                                BSItemName = dr["BSItemName"].ToString(),
                                KCustom = dr["KCustom"].ToString(),
                                Comments = dr["Comments"].ToString(),

                                QTY_CV = ConvertDouble(dr["QTY"].ToString()),
                                UnitPrice_CV = ConvertDouble(dr["UnitPrice"].ToString()),
                                AMOUNT_CV = ConvertDouble(dr["AMOUNT"].ToString()),
                                VATAmount_CV = ConvertDouble(dr["VATAmount"].ToString()),
                            };

                            // 화폐단위
                            if (WinAccBuySale.CurrencyUnit.Trim().Equals("0"))
                            {
                                WinAccBuySale.CurrencyUnitName = "₩";
                            }
                            else
                            {
                                WinAccBuySale.CurrencyUnitName = "$";
                            }

                            // 만약에 lstInChk 에 있다면 체크하기
                            if (IsInCheckList(lstChkOut, WinAccBuySale.BSNo)) { WinAccBuySale.Chk = true; }

                            dgdSub.Items.Add(WinAccBuySale);
                            i++;
                        }
                    }
                }

                setCalculateAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion

        #region 결재처리에서 조회 메서드

        private void FillGrid_InApp()
        {
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear();
            }

            try
            {

                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("AppReqID", AppReqID);

                ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sAppBuyList", sqlParameter, false);

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
                            var WinAccBuySale = new App_IN_CodeView2()
                            {
                                Num = i + 1,
                                IsCheck = false,

                                BSNo = dr["BSNo"].ToString(),
                                BSGBN = dr["BSGBN"].ToString(),
                                BSDate = DatePickerFormat(dr["BSDate"].ToString()),
                                CustomID = dr["CustomID"].ToString(),
                                InOutCustomID = dr["InOutCustomID"].ToString(),

                                RollQty = stringFormatN0(dr["RollQty"]),
                                QTY = stringFormatN0(dr["QTY"]),
                                UnitPrice = stringFormatN0(dr["UnitPrice"]),
                                VatINDYN = dr["VatINDYN"].ToString(),
                                AMOUNT = stringFormatN0(dr["AMOUNT"]),

                                VATAmount = stringFormatN0(dr["VATAmount"]),
                                TotalAmount = stringFormatN0(dr["TotalAmount"]),
                                CurrencyUnit = dr["CurrencyUnit"].ToString(),
                                InOutNo = dr["InOutNo"].ToString(),
                                InOutNoSEQ = dr["InOutNoSEQ"].ToString(),

                                ArticleID = dr["ArticleID"].ToString(),
                                Article = dr["Article"].ToString(),
                                ORDERNO = dr["ORDERNO"].ToString(),
                                OrderID = dr["OrderID"].ToString(),
                                BSITEM = dr["BSITEM"].ToString(),

                                BSItemName = dr["BSItemName"].ToString(),
                                KCustom = dr["KCustom"].ToString(),
                                Comments = dr["Comments"].ToString()
                            };

                            // 화폐단위
                            if (WinAccBuySale.CurrencyUnit.Trim().Equals("0"))
                            {
                                WinAccBuySale.CurrencyUnitName = "₩";
                            }
                            else
                            {
                                WinAccBuySale.CurrencyUnitName = "$";
                            }

                            dgdMain.Items.Add(WinAccBuySale);
                            i++;
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

        #region 기타 메서드 모음

        // 천마리 콤마, 소수점 버리기
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




        #endregion

        #region 체크박스 관련

        // 체크박스 추가하기
        private void chkApp_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkSender = sender as CheckBox;
            var AppM = chkSender.DataContext as App_IN_CodeView2;

            if (AppM != null)
            {
                if (chkSender.IsChecked == true)
                {
                    AppM.Chk = true;
                }
                else
                {
                    AppM.Chk = false;
                }
            }

            setCalculateAmount();
        }

        // 전체 선택
        private void AllCheck_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dgdMain.Items.Count; i++)
            {
                var AppIn = dgdMain.Items[i] as App_IN_CodeView2;
                AppIn.Chk = true;
            }

            setCalculateAmount();
        }
        private void AllCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dgdMain.Items.Count; i++)
            {
                var AppIn = dgdMain.Items[i] as App_IN_CodeView2;
                AppIn.Chk = false;
            }
            setCalculateAmount();

        }

        // 전체 선택
        private void AllCheckSub_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dgdSub.Items.Count; i++)
            {
                var AppIn = dgdSub.Items[i] as App_IN_CodeView2;
                AppIn.Chk = true;
            }

            setCalculateAmount();
        }
        private void AllCheckSub_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dgdSub.Items.Count; i++)
            {
                var AppIn = dgdSub.Items[i] as App_IN_CodeView2;
                AppIn.Chk = false;
            }

            setCalculateAmount();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // 체크된 것들은 검색 후에 체크 되어 있도록 하기 위해 추가

            // true : 매입건
            if (chkIsIN.IsChecked == true)
            {
                setListChk("매입");
                FillGrid(strFlag);
            }
            else
            {
                setListChk("매출");
                FillGridSub(strFlag);
            }
        }

        private void setListChk(string BSGubun)
        {
            if (BSGubun.Equals("매입"))
            {
                lstChkIn.Clear();
                for (int i = 0; i < dgdMain.Items.Count; i++)
                {
                    var Main = dgdMain.Items[i] as App_IN_CodeView2;
                    if (Main != null
                        && Main.Chk == true)
                    {
                        lstChkIn.Add(Main.BSNo);
                    }
                }
            }
            else if (BSGubun.Equals("매출")
                && firstFlag == true)
            {
                lstChkOut.Clear();
                for (int i = 0; i < dgdSub.Items.Count; i++)
                {
                    var Sub = dgdSub.Items[i] as App_IN_CodeView2;
                    if (Sub != null
                        && Sub.Chk == true)
                    {
                        lstChkOut.Add(Sub.BSNo);
                    }
                }
            }
        }

        #endregion // 체크박스 관련

        // 타이틀 클릭 시
        private void vbTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Viewbox senderView = sender as Viewbox;
            if (senderView.Name.Equals("vbTitleOut")) // 매출
            {
                tblTitleOut.Foreground = System.Windows.Media.Brushes.Black;
                grdBarOut.Visibility = Visibility.Visible;

                tblTitleIn.Foreground = System.Windows.Media.Brushes.Gray;
                grdBarIn.Visibility = Visibility.Hidden;

                dgdMain.Visibility = Visibility.Hidden;
                dgdSub.Visibility = Visibility.Visible;

                tblDate.Text = "매출 일자";

                stpAllCheck.Visibility = Visibility.Hidden;
                stpAllCheckSub.Visibility = Visibility.Visible;

                chkIsIN.IsChecked = false;

                if (firstFlag == false)
                {
                    if (strFlag.Equals("U"))
                    {
                        chkDate.IsChecked = false;
                    }

                    btnSearch_Click(null, null);
                    firstFlag = true;
                }
            }
            else
            {
                tblTitleOut.Foreground = System.Windows.Media.Brushes.Gray;
                grdBarOut.Visibility = Visibility.Hidden;

                tblTitleIn.Foreground = System.Windows.Media.Brushes.Black;
                grdBarIn.Visibility = Visibility.Visible;

                dgdMain.Visibility = Visibility.Visible;
                dgdSub.Visibility = Visibility.Hidden;

                tblDate.Text = "매입 일자";

                stpAllCheck.Visibility = Visibility.Visible;
                stpAllCheckSub.Visibility = Visibility.Hidden;

                chkIsIN.IsChecked = true;
            }
        }

        #region 2020.09.09 체크시, 총 금액을 표시할 수 있도록 요청건!!!

        private void setCalculateAmount()
        {
            int InCnt = 0;
            int InAmount = 0;
            int OutCnt = 0;
            int OutAmount = 0;

            for(int i = 0; i < dgdMain.Items.Count; i++)
            {
                var Main = dgdMain.Items[i] as App_IN_CodeView2;
                if (Main != null
                    && Main.Chk == true)
                {
                    InCnt++;
                    InAmount += ConvertInt(Main.AMOUNT);
                }
            }

            for (int i = 0; i < dgdSub.Items.Count; i++)
            {
                var Sub = dgdSub.Items[i] as App_IN_CodeView2;
                if (Sub != null
                    && Sub.Chk == true)
                {
                    OutCnt++;
                    OutAmount += ConvertInt(Sub.AMOUNT);
                }
            }

            tblMsg.Text = string.Format("▶ 선택 매입건 : {0:N0}건, 총 금액 : {1:N0} / ▶ 선택 매출건 : {2:N0}건, 총 금액 : {3:N0}", InCnt, InAmount, OutCnt, OutAmount);
        }


        #endregion

        // 2020.11.04 요청에 의한 엑셀 추가.
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region 3. 인쇄 양식으로(도장까지) ← 2020.10.30 요청사항 : 1. 한장 넘길 일이 없다고 함 

                // 결재순서 
                DataTable dt = getAppStep(AppStepID);

                if (dt == null
                    || dt.Rows.Count == 0)
                {
                    if (MessageBox.Show("결재 단계가 선택되지 않았습니다. 계속 하시겠습니까?", "엑셀 변환 전 체크", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                List<App_IN_CodeView2> lstApp = new List<App_IN_CodeView2>();

                for (int i = 0; i < dgdMain.Items.Count; i++)
                {
                    var AppIn = dgdMain.Items[i] as App_IN_CodeView2;

                    if (AppIn != null
                        && AppIn.Chk == true)
                    {
                        lstApp.Add(AppIn);
                    }
                }

                for (int i = 0; i < dgdSub.Items.Count; i++)
                {
                    var AppOut = dgdSub.Items[i] as App_IN_CodeView2;

                    if (AppOut != null
                        && AppOut.Chk == true)
                    {
                        lstApp.Add(AppOut);
                    }
                }

                // 조회 결과가 없으면 리턴
                if (lstApp == null
                    || lstApp.Count == 0)
                {
                    //MessageBox.Show("반려나 부결 건은 ");
                    return;
                }

                // 엑셀 시작
                excelapp = new Microsoft.Office.Interop.Excel.Application();

                string MyBookPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Report\\매입리스트(품의)출력.xls";
                workbook = excelapp.Workbooks.Add(MyBookPath);
                worksheet = workbook.Sheets["Form"];
                stempsheet = workbook.Sheets["Stemp"];

                int startRowIndex = 7; // 시작하는 행
                int endRowIndex = 30; // 마지막 행

                int excelRow = 0;
                for (int k = 0; k < lstApp.Count; k++)
                {

                    var App = lstApp[k];

                    if (k == 0) // 최초 한번 입력
                    {
                        // 일자 : 2020년 10월
                        workrange = worksheet.get_Range("H4");
                        workrange.Value2 = ReqDate.ToString("yyyy-MM-dd");
                    }

                    excelRow = startRowIndex + k;

                    // 순번
                    workrange = worksheet.get_Range("A" + excelRow);
                    workrange.Value2 = k + 1;

                    // 일자
                    workrange = worksheet.get_Range("B" + excelRow);
                    workrange.Value2 = DatePickerFormat(App.BSDate.Trim());

                    // 매출항목
                    workrange = worksheet.get_Range("C" + excelRow);
                    workrange.Value2 = App.BSItemName.Trim();

                    // 거래처
                    workrange = worksheet.get_Range("D" + excelRow);
                    workrange.Value2 = App.KCustom.Trim();

                    // 품명
                    workrange = worksheet.get_Range("E" + excelRow);
                    workrange.Value2 = App.Article.Trim();

                    // 수량
                    workrange = worksheet.get_Range("F" + excelRow);
                    workrange.Value2 = ConvertDouble(App.QTY.Trim());

                    // 단가
                    workrange = worksheet.get_Range("G" + excelRow);
                    workrange.Value2 = ConvertDouble(App.UnitPrice.Trim());

                    // 금액
                    workrange = worksheet.get_Range("H" + excelRow);
                    workrange.Value2 = ConvertDouble(App.AMOUNT.Trim());

                    // 비고
                    workrange = worksheet.get_Range("I" + excelRow);
                    workrange.Value2 = App.Comments.Trim();
                }

                // 빈 행은 삭제하기
                if (endRowIndex - excelRow > 0)
                {
                    worksheet.Range["A" + (excelRow + 1), "A" + endRowIndex].EntireRow.Delete();
                }

                //// 줄선 긋기
                //worksheet.Range["A3", "G" + (2 + dgdMain.Items.Count)].BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin);

                //workrange = worksheet.Range["A3", "I" + (2 + dgdMain.Items.Count)];
                //workrange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                //workrange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin;

                #region 도장 → 로직 개선 필요

                string[] strColName = { "F", "G", "H", "I", "J", "K" };
                int stempRowIndex = 9;

                List<string> lstStep = new List<string>();

                foreach (DataRow dr in dt.Rows)
                {
                    lstStep.Add(dr["Resably"].ToString());
                }

                //List<App_Stemp> lstStep = FillGridStep(AppReqID); // 직급
                //List<App_Stemp> lstStemp = FillGridStemp(AppReqID); // 도장

                for (int i = 0; i < lstStep.Count; i++)
                {
                    workrange = stempsheet.get_Range(strColName[i + 1] + stempRowIndex);
                    if (lstStep[i].Trim().Equals("대표이사"))
                    {
                        lstStep[i] = "사장";
                    }
                    workrange.Value2 = ResablyFormat(lstStep[i]);
                    //workrange.Font.Size = 11;
                }

                // 여기에 +로 border 추가
                workrange = stempsheet.Range[strColName[1] + stempRowIndex, strColName[lstStep.Count] + (stempRowIndex + 1)];
                //workrange.Range[strColName[1] + "13", strColName[lstStep.Count - 1] + "14"].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                //workrange.Range[strColName[1] + "13", strColName[lstStep.Count - 1] + "14"].Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop].Weight = 10d;
                //workrange.Range[strColName[1] + "13", strColName[lstStep.Count - 1] + "14"].Borders.ColorIndex = Microsoft.Office.Interop.Excel.XlRgbColor.rgbBlack;
                //workrange.Range[strColName[1] + "13", strColName[lstStep.Count - 1] + "14"].BorderAround(Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous, Microsoft.Office.Interop.Excel.XlRgbColor.rgbBlack);

                workrange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].Color = System.Drawing.Color.Black;
                workrange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop].Color = System.Drawing.Color.Black;
                workrange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft].Color = System.Drawing.Color.Black;
                workrange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].Color = System.Drawing.Color.Black;
                workrange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].Color = System.Drawing.Color.Black;
                workrange.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].Color = System.Drawing.Color.Black;

                // 결재 등록 중이므로, 이미지가 불필요 → 주석처리
                //try
                //{
                //    string str_path = FTP_ADDRESS + '/' + "Stemp";
                //    _ftp = new FTP_EX(str_path, FTP_ID, FTP_PASS);

                //    for (int m = 0; m < lstStemp.Count; m++)
                //    {
                //        string str_remotepath = lstStemp[m].StempFileName;
                //        string str_localpath = LOCAL_DOWN_PATH + "\\" + lstStemp[m].StempFileName;

                //        DirectoryInfo DI = new DirectoryInfo(LOCAL_DOWN_PATH);      // Temp 폴더가 없는 컴터라면, 만들어 줘야지.
                //        if (DI.Exists == false)
                //        {
                //            DI.Create();
                //        }

                //        FileInfo file = new FileInfo(str_localpath);
                //        if (file.Exists)
                //        {
                //            file.Delete();
                //        }

                //        //if (_ftp.StempDownload(lstStemp[m].PersonID + '/' + str_remotepath, str_localpath, ""))
                //        if (_ftp.StempDownload(@"20241119/정유진사원.jpg", str_localpath, ""))
                //        {
                //            //workrange.CopyPicture()
                //            workrange = stempsheet.get_Range(strColName[m] + stempRowIndex);
                //            // 엑셀 도장 이미지 조절 후 셀에 삽입 → 디자인 폼에 x, y, 간격 텍스트 박스에 기본값을 세팅 후 가져옴. → 테스트는 이걸로
                //            stempsheet.Shapes.AddPicture("C:\\Temp\\" + lstStemp[m].StempFileName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, ConvertFloat(txtX.Text) + (ConvertFloat(txtWidth.Text) * m), ConvertFloat(txtY.Text), 30, 30);
                //        }
                //        else
                //        {
                //            continue;
                //        }
                //    }
                //}
                //catch (Exception ep1)
                //{
                //    MessageBox.Show(ep1.Message);
                //}

                #endregion

                // 기본 폼 활성화 후 보이도록
                worksheet.Activate();
                worksheet.Range["A1"].Select();

                excelapp.Visible = true;
                msg.Hide();

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Marshal.ReleaseComObject(worksheet);
                Marshal.ReleaseComObject(stempsheet);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(excelapp);
            }
        }

        // 
        private DataTable getAppStep(string AppStepID)
        {
            DataTable dt = new DataTable();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("AppStepID", AppStepID);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sApprovalStepSub", sqlParameter, false);

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

        // 두글자면 중간에 띄어쓰기 한번
        private string ResablyFormat(string str)
        {
            if (!str.Trim().Equals(""))
            {
                if (str.Trim().Length == 2)
                {
                    string F = str.Trim().Substring(0, 1);
                    string S = str.Trim().Substring(1, 1);

                    str = F + " " + S;
                }
            }

            return str;
        }

        // 소수로 변환
        private float ConvertFloat(string str)
        {
            if (str == null) { return 0; }

            float result = 0;
            float chkFloat = 0;

            if (!str.Trim().Equals(""))
            {
                str = str.Replace(",", "");

                if (float.TryParse(str, out chkFloat) == true)
                {
                    result = float.Parse(str);
                }
            }

            return result;
        }

        #region 엑셀용 - 담당자들 + 승인된것들 도장 이미지

        // 조회 1
        private List<App_Stemp> FillGridStep(string strID)
        {
            List<App_Stemp> lstStep = new List<App_Stemp>();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("AppReqID", strID);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sApproval_Step", sqlParameter, false);

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
                            var Stemp = new App_Stemp()
                            {
                                Num = i,

                                PersonID = dr["PersonID"].ToString(),
                                Resably = dr["Resably"].ToString(),
                                Name = dr["Name"].ToString(),

                            };

                            lstStep.Add(Stemp);
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

            return lstStep;
        }

        // 조회 2
        private List<App_Stemp> FillGridStemp(string strID)
        {
            List<App_Stemp> lstStemp = new List<App_Stemp>();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("AppReqID", strID);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sApproval_Stemp", sqlParameter, false);

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
                            var Stemp = new App_Stemp()
                            {
                                Num = i,

                                PersonID = dr["PersonID"].ToString(),
                                Resably = dr["Resably"].ToString(),
                                FolderName = dr["FolderName"].ToString(),
                                StempFileName = dr["StempFileName"].ToString(),
                            };

                            lstStemp.Add(Stemp);
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

            return lstStemp;
        }


        #endregion // 엑셀용 - 담당자들 + 승인된것들 도장 이미지

    }


    public class App_IN_CodeView2 : BaseView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }                     // 순
        public bool IsCheck { get; set; }                // 체크

        public string BSNo { get; set; }                 // P-KEY
        public string BSGBN { get; set; }                // 매입. 매출 구분자 이것도 PK
        public string BSDate { get; set; }               // 일자
        public string CustomID { get; set; }             // 거래처
        public string InOutCustomID { get; set; }        // 입.출고처

        public string RollQty { get; set; }              // 절 수
        public string QTY { get; set; }                  // 수량
        public string UnitPrice { get; set; }            // 단가
        public string VatINDYN { get; set; }             // 부가세 YN
        public string AMOUNT { get; set; }               // 금액

        public string VATAmount { get; set; }            // 부가세
        public string TotalAmount { get; set; }          // 토탈금액
        public string CurrencyUnit { get; set; }         // 화폐단위
        public string CurrencyUnitName { get; set; }    // 화폐단위 구분
        public string InOutNo { get; set; }              // 입출고 번호
        public string InOutNoSEQ { get; set; }           // 입출고 순번

        public string ArticleID { get; set; }            // 품명
        public string Article { get; set; }              // 품명
        public string ORDERNO { get; set; }              // 오더번호
        public string OrderID { get; set; }              // 오더ID
        public string BSITEM { get; set; }               // 매입.매출번호

        public string BSItemName { get; set; }           // 매입.매출 항목명
        public string KCustom { get; set; }              // 매입.매출 항목명
        public string Comments { get; set; }             // 비고, 코멘트.

        public string CloseClss { get; set; }
        public string SALESCHARGE { get; set; }
        public string BSPlace { get; set; }

        public string color { get; set; }
        public string CompanyID { get; set; }
        public string BasisYearMon { get; set; }
        public string ProcSeq { get; set; }
        public string TaxBillYN { get; set; }

        public string ExchRate { get; set; }

        // 필터용
        public double QTY_CV { get; set; }
        public double UnitPrice_CV { get; set; }
        public double AMOUNT_CV { get; set; }
        public double VATAmount_CV { get; set; }

        public bool Chk { get; set; }
    }

}
