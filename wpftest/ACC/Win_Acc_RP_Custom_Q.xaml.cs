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
using WizMes_Alpha_JA.PopUp;
using WizMes_Alpha_JA.PopUP;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// Win_mtr_Subul_Q_New.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Acc_RP_Custom_Q : UserControl
    {
        int rowNum = 0;

        ScrollViewer scrollView = null;
        ScrollViewer scrollView2 = null;

        public Win_Acc_RP_Custom_Q()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 스크롤 동기화
            scrollView = dgdMainHeader;
            scrollView2 = getScrollbar(dgdMain);

            if (null != scrollView)
            {
                scrollView.ScrollChanged += new ScrollChangedEventHandler(scrollView_ScrollChanged);
            }
            if (null != scrollView2)
            {
                scrollView2.ScrollChanged += new ScrollChangedEventHandler(scrollView_ScrollChanged);
            }

            chkDateSrh.IsChecked = true;
            tbnOutware.IsChecked = true;
            dtpSDate.SelectedDate = DateTime.Today;
            //dtpEDate.SelectedDate = DateTime.Today;
        }

        #region Header 부분 - 검색조건

        // 일자
        private void lblDateSrh_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (chkDateSrh.IsEnabled == true)
            {
                if (chkDateSrh.IsChecked == true)
                {
                    chkDateSrh.IsChecked = false;
                }
                else
                {
                    chkDateSrh.IsChecked = true;
                }
            }
        }
        private void chkDateSrh_Checked(object sender, RoutedEventArgs e)
        {
            if (chkDateSrh.IsEnabled == true)
            {
                chkDateSrh.IsChecked = true;
                dtpSDate.IsEnabled = true;
                //dtpEDate.IsEnabled = true;

             
                btnLastYear.IsEnabled = true;
                btnThisYear.IsEnabled = true;
            }
        }
        private void chkDateSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkDateSrh.IsEnabled == true)
            {
                chkDateSrh.IsChecked = false;
                dtpSDate.IsEnabled = false;
                //dtpEDate.IsEnabled = false;

             
                btnLastYear.IsEnabled = false;
                btnThisYear.IsEnabled = false;
            }
        }

        // 전일 금일 전월 금월 버튼
      
        //금년
        private void btnThisYear_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = Lib.Instance.BringThisYearDatetimeFormat()[0];
            //dtpEDate.SelectedDate = Lib.Instance.BringThisYearDatetimeFormat()[1];
        }
        //전년
        private void btnLastYear_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = Lib.Instance.BringLastYearDatetime()[0];
     
           

            //if (dtpSDate.SelectedDate != null)
            //{
            //    DateTime ThatMonth1 = dtpSDate.SelectedDate.Value.AddDays(-(dtpSDate.SelectedDate.Value.Day - 1)); // 선택한 일자 달의 1일!

            //    DateTime LastMonth1 = ThatMonth1.AddMonths(-1); // 저번달 1일
            //    DateTime LastMonth31 = ThatMonth1.AddDays(-1); // 저번달 말일

            //    dtpSDate.SelectedDate = LastMonth1;
            //    dtpEDate.SelectedDate = LastMonth31;
            //}
            //else
            //{
            //    DateTime ThisMonth1 = DateTime.Today.AddDays(-(DateTime.Today.Day - 1)); // 이번달 1일

            //    DateTime LastMonth1 = ThisMonth1.AddMonths(-1); // 저번달 1일
            //    DateTime LastMonth31 = ThisMonth1.AddDays(-1); // 저번달 말일

            //    dtpSDate.SelectedDate = LastMonth1;
            //    dtpEDate.SelectedDate = LastMonth31;
            //}
        }
      


        #endregion // Header 부분 - 검색조건

        #region Header 부분 - 오른쪽 버튼 모음 (검색, 닫기, 엑셀)

        // 검색버튼
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }

        private void beSearch()
        {
            //if (dtpEDate.SelectedDate != null)
            //{
            //    DateTime FromDate = dtpEDate.SelectedDate.Value.AddMonths(-11);
            //    dtpSDate.SelectedDate = FromDate;
            //}

            re_search(rowNum);
        }

        // 닫기버튼
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        // 엑셀버튼
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] lst = new string[2];
            lst[0] = "기간별 작업자 실적";
            lst[1] = dgdMain.Name;

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
                else
                {
                    if (dt != null)
                    {
                        dt.Clear();
                    }
                }
            }
        }

        #endregion // Header 부분 - 오른쪽 버튼 모음 (검색, 닫기, 엑셀)

        private void re_search(int selectedIndex)
        {
            
          
            FillGrid();

            if (dgdMain.Items.Count > 0)
            {
                dgdMain.SelectedIndex = selectedIndex;
            }
            else
            {
                MessageBox.Show("조회된 데이터가 없습니다.");
                return;
            }
        }

        #region 조회 메서드

        private void FillGrid()
        {
            string sBSGbn = string.Empty;
            if(tbnStuffin.IsChecked == true)
            {
                sBSGbn = "2";
            } else
            {
                sBSGbn = "1";
            }
            string[] Header = new string[12];

            //DateTime ToDate = dtpEDate.SelectedDate.Value;
            //ToDate = ToDate.AddMonths(1);

            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

     
                sqlParameter.Add("sYYYY", dtpSDate != null ? dtpSDate.SelectedDate.Value.ToString("yyyy") : "");
                sqlParameter.Add("sBSGbn", sBSGbn);
                sqlParameter.Add("nChkCompanyID", "0");
                sqlParameter.Add("sCompanyID", "");
           

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_RP_CustomYear_Q", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;

                    bool firstFlag = false;
                    if (dt.Rows.Count > 1)
                    {
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow dr in drc)
                        {
                            i++;

                            var WinR = new Win_Acc_RP_Custom_Q_CodeView()
                            {
                                Num = i,

                                CompanyID = dr["CompanyID"].ToString(),
                                BSGBN = dr["BSGBN"].ToString(),
                                customID = dr["customID"].ToString(),
                                KCustom = dr["KCustom"].ToString(),
                                CurrencyUnit = dr["CurrencyUnit"].ToString(),

                                InitRemainAmount = stringFormatN0(dr["InitRemainAmount"]),
                                RPSumAmount01 = stringFormatN0(dr["RPSumAmount01"]),
                                RPDCAmount01 = stringFormatN0(dr["RPDCAmount01"]),
                                BSTotalAmount01 = stringFormatN0(dr["BSTotalAmount01"]),
                                RemainAmount01 = stringFormatN0(dr["RemainAmount01"]),

                                RPSumAmount02 = stringFormatN0(dr["RPSumAmount02"]),
                                RPDCAmount02 = stringFormatN0(dr["RPDCAmount02"]),
                                BSTotalAmount02 = stringFormatN0(dr["BSTotalAmount02"]),
                                RemainAmount02 = stringFormatN0(dr["RemainAmount02"]),

                                RPSumAmount03 = stringFormatN0(dr["RPSumAmount03"]),
                                RPDCAmount03 = stringFormatN0(dr["RPDCAmount03"]),
                                BSTotalAmount03 = stringFormatN0(dr["BSTotalAmount03"]),
                                RemainAmount03 = stringFormatN0(dr["RemainAmount03"]),

                                RPSumAmount04 = stringFormatN0(dr["RPSumAmount04"]),
                                RPDCAmount04 = stringFormatN0(dr["RPDCAmount04"]),
                                BSTotalAmount04 = stringFormatN0(dr["BSTotalAmount04"]),
                                RemainAmount04 = stringFormatN0(dr["RemainAmount04"]),

                                RPSumAmount05 = stringFormatN0(dr["RPSumAmount05"]),
                                RPDCAmount05 = stringFormatN0(dr["RPDCAmount05"]),
                                BSTotalAmount05 = stringFormatN0(dr["BSTotalAmount05"]),
                                RemainAmount05 = stringFormatN0(dr["RemainAmount05"]),

                                RPSumAmount06 = stringFormatN0(dr["RPSumAmount06"]),
                                RPDCAmount06 = stringFormatN0(dr["RPDCAmount06"]),
                                BSTotalAmount06 = stringFormatN0(dr["BSTotalAmount06"]),
                                RemainAmount06 = stringFormatN0(dr["RemainAmount06"]),

                                RPSumAmount07 = stringFormatN0(dr["RPSumAmount07"]),
                                RPDCAmount07 = stringFormatN0(dr["RPDCAmount07"]),
                                BSTotalAmount07 = stringFormatN0(dr["BSTotalAmount07"]),
                                RemainAmount07 = stringFormatN0(dr["RemainAmount07"]),

                                RPSumAmount08 = stringFormatN0(dr["RPSumAmount08"]),
                                RPDCAmount08 = stringFormatN0(dr["RPDCAmount08"]),
                                BSTotalAmount08 = stringFormatN0(dr["BSTotalAmount08"]),
                                RemainAmount08 = stringFormatN0(dr["RemainAmount08"]),

                                RPSumAmount09 = stringFormatN0(dr["RPSumAmount09"]),
                                RPDCAmount09 = stringFormatN0(dr["RPDCAmount09"]),
                                BSTotalAmount09 = stringFormatN0(dr["BSTotalAmount09"]),
                                RemainAmount09 = stringFormatN0(dr["RemainAmount09"]),

                                RPSumAmount10 = stringFormatN0(dr["RPSumAmount10"]),
                                RPDCAmount10 = stringFormatN0(dr["RPDCAmount10"]),
                                BSTotalAmount10 = stringFormatN0(dr["BSTotalAmount10"]),
                                RemainAmount10 = stringFormatN0(dr["RemainAmount10"]),

                                RPSumAmount11 = stringFormatN0(dr["RPSumAmount11"]),
                                RPDCAmount11 = stringFormatN0(dr["RPDCAmount11"]),
                                BSTotalAmount11 = stringFormatN0(dr["BSTotalAmount11"]),
                                RemainAmount11 = stringFormatN0(dr["RemainAmount11"]),

                                RPSumAmount12 = stringFormatN0(dr["RPSumAmount12"]),
                                RPDCAmount12 = stringFormatN0(dr["RPDCAmount12"]),
                                BSTotalAmount12 = stringFormatN0(dr["BSTotalAmount12"]),
                                RemainAmount12 = stringFormatN0(dr["RemainAmount12"]),

                                RPSumAmount13 = stringFormatN0(dr["RPSumAmount13"]),
                                RPDCAmount13 = stringFormatN0(dr["RPDCAmount13"]),
                                BSTotalAmount13 = stringFormatN0(dr["BSTotalAmount13"]),
                                RemainAmount13 = stringFormatN0(dr["RemainAmount13"]),

                            };

                            ////헤더 값 세팅
                            //if (firstFlag == false)
                            //{
                            //    Header[0] = getYearMonth(WinR.Month01);
                            //    Header[1] = getYearMonth(WinR.Month02);
                            //    Header[2] = getYearMonth(WinR.Month03);
                            //    Header[3] = getYearMonth(WinR.Month04);
                            //    Header[4] = getYearMonth(WinR.Month05);
                            //    Header[5] = getYearMonth(WinR.Month06);
                            //    Header[6] = getYearMonth(WinR.Month07);
                            //    Header[7] = getYearMonth(WinR.Month08);
                            //    Header[8] = getYearMonth(WinR.Month09);
                            //    Header[9] = getYearMonth(WinR.Month10);
                            //    Header[10] = getYearMonth(WinR.Month11);
                            //    Header[11] = getYearMonth(WinR.Month12);

                            //    firstFlag = true;
                            //}

                            
                            dgdMain.Items.Add(WinR);

                        }
                    }

                    //// 헤더 세팅!!!!!
                    //dgdHeader1.Content = Header[0];
                    //dgdHeader2.Content = Header[1];
                    //dgdHeader3.Content = Header[2];
                    //dgdHeader4.Content = Header[3];
                    //dgdHeader5.Content = Header[4];
                    //dgdHeader6.Content = Header[5];
                    //dgdHeader7.Content = Header[6];
                    //dgdHeader8.Content = Header[7];
                    //dgdHeader9.Content = Header[8];
                    //dgdHeader10.Content = Header[9];
                    //dgdHeader11.Content = Header[10];
                    //dgdHeader12.Content = Header[11];

                    //tbkCount.Text = " ▶ 검색 결과 : " + i + " 건";
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

        #endregion // 조회 메서드

        #region 기타 메서드 모음

        // 월 만들기
        private string getYearMonth(string str)
        {
            str = str.Trim();

            if (str.Length == 6)
            {
                string Y = str.Substring(0, 4);
                string M = str.Substring(4, 2);

                if (M.Substring(0, 1).Equals("0"))
                {
                    M = M.Substring(1, 1);
                }

                str = Y + "년 " + M + "월";
            }
            else if (str.Length == 8)
            {
                string Y = str.Substring(0, 4);
                string M = str.Substring(4, 2);

                if (M.Substring(0, 1).Equals("0"))
                {
                    M = M.Substring(1, 1);
                }

                str = Y + "년 " + M + "월";
            }

            return str;
        }

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

        // 시간 형식 6글자라면! 11:11:11
        private string DateTimeFormat(string str)
        {
            str = str.Replace(":", "").Trim();

            if (str.Length == 6)
            {
                string Hour = str.Substring(0, 2);
                string Min = str.Substring(2, 2);
                string Sec = str.Substring(4, 2);

                str = Hour + ":" + Min + ":" + Sec;
            }

            return str;
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

        #region 스크롤 Scroll 메서드 모음

        void scrollView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var newOffset = e.HorizontalOffset;

            if ((null != scrollView) && (null != scrollView2))
            {
                scrollView.ScrollToHorizontalOffset(newOffset);
                scrollView2.ScrollToHorizontalOffset(newOffset);
            }
        }

        private ScrollViewer getScrollbar(DependencyObject dep)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
            {
                var child = VisualTreeHelper.GetChild(dep, i);
                if ((null != child) && child is ScrollViewer)
                {
                    return (ScrollViewer)child;
                }
                else
                {
                    ScrollViewer sub = getScrollbar(child);
                    if (sub != null)
                    {
                        return sub;
                    }
                }
            }
            return null;
        }

        #endregion // 스크롤 Scroll 메서드 모음

        #region (상단 조회조건 체크박스 enable 모음)
        // 수금/지불 토글버튼
        private void tbnOutware_Checked(object sender, RoutedEventArgs e)
        {
            tbnStuffin.IsChecked = false;
            tbnOutware.IsChecked = true;

        }
        // 수금 버튼 
        private void tbnOutware_Unchecked(object sender, RoutedEventArgs e)
        {
            tbnOutware.IsChecked = false;
            tbnStuffin.IsChecked = true;
        }
        // 지불 버튼
        private void tbnStuffin_Checked(object sender, RoutedEventArgs e)
        {
            tbnOutware.IsChecked = false;
            tbnStuffin.IsChecked = true;

        }

        private void tbnStuffin_Unchecked(object sender, RoutedEventArgs e)
        {
            tbnStuffin.IsChecked = false;
            tbnOutware.IsChecked = true;


        }
        #endregion
    }

    class Win_Acc_RP_Custom_Q_CodeView
    {
        public int Num { get; set; }


        public string CompanyID { get; set; }
        public string BSGBN { get; set; }
        public string customID { get; set; }
        public string KCustom { get; set; }
        public string CurrencyUnit { get; set; }

        public string InitRemainAmount { get; set; }
        public string RPSumAmount01 { get; set; }
        public string RPDCAmount01 { get; set; }
        public string BSTotalAmount01 { get; set; }
        public string RemainAmount01 { get; set; }

        public string RPSumAmount02 { get; set; }
        public string RPDCAmount02 { get; set; }
        public string BSTotalAmount02 { get; set; }
        public string RemainAmount02 { get; set; }

        public string RPSumAmount03 { get; set; }
        public string RPDCAmount03 { get; set; }
        public string BSTotalAmount03 { get; set; }
        public string RemainAmount03 { get; set; }

        public string RPSumAmount04 { get; set; }
        public string RPDCAmount04 { get; set; }
        public string BSTotalAmount04 { get; set; }
        public string RemainAmount04 { get; set; }

        public string RPSumAmount05 { get; set; }
        public string RPDCAmount05 { get; set; }
        public string BSTotalAmount05 { get; set; }
        public string RemainAmount05 { get; set; }

        public string RPSumAmount06 { get; set; }
        public string RPDCAmount06 { get; set; }
        public string BSTotalAmount06 { get; set; }
        public string RemainAmount06 { get; set; }

        public string RPSumAmount07 { get; set; }
        public string RPDCAmount07 { get; set; }
        public string BSTotalAmount07 { get; set; }
        public string RemainAmount07 { get; set; }

        public string RPSumAmount08 { get; set; }
        public string RPDCAmount08 { get; set; }
        public string BSTotalAmount08 { get; set; }
        public string RemainAmount08 { get; set; }

        public string RPSumAmount09 { get; set; }
        public string RPDCAmount09 { get; set; }
        public string BSTotalAmount09 { get; set; }
        public string RemainAmount09 { get; set; }

        public string RPSumAmount10 { get; set; }
        public string RPDCAmount10 { get; set; }
        public string BSTotalAmount10 { get; set; }
        public string RemainAmount10 { get; set; }

        public string RPSumAmount11 { get; set; }
        public string RPDCAmount11 { get; set; }
        public string BSTotalAmount11 { get; set; }
        public string RemainAmount11 { get; set; }

        public string RPSumAmount12 { get; set; }
        public string RPDCAmount12 { get; set; }
        public string BSTotalAmount12 { get; set; }
        public string RemainAmount12 { get; set; }

        public string RPSumAmount13 { get; set; }
        public string RPDCAmount13 { get; set; }
        public string BSTotalAmount13 { get; set; }
        public string RemainAmount13 { get; set; }

    }
}

