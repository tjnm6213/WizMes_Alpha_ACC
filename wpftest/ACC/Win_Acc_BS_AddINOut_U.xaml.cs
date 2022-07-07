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
    /// Win_Acc_BS_AddINOut_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Acc_BS_AddINOut_U : UserControl
    {

        int Wh_Ar_SelectedLastIndex = 0;        // 그리드 마지막 선택 줄 임시저장 그릇

        WizMes_Alpha_JA.PopUp.NoticeMessage msg = new PopUp.NoticeMessage();
        //(기다림 알림 메시지창)

        Win_Acc_BS_AddINOut_U_CodeView WinAccBS_AddINOut = new Win_Acc_BS_AddINOut_U_CodeView();
        Win_Acc_BS_AddIN_U_CodeView WinAccBS_AddIN = new Win_Acc_BS_AddIN_U_CodeView();

        public Win_Acc_BS_AddINOut_U()
        {
            InitializeComponent();
        }

        // 로드 이벤트.
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            chkPeriod.IsChecked = true;
            SetComboBox();

            // 로드시, 출고버튼 클릭상태로 시작하게끔.
            tbnOutware.IsChecked = true;
            cboLeadWorkHouse.SelectedIndex = 0;

            //금일 날짜 넣기
            dtpOutSDate.SelectedDate = DateTime.Today;
            dtpOutEDate.SelectedDate = DateTime.Today;
            dtpInSDate.SelectedDate = DateTime.Today;
            dtpInEDate.SelectedDate = DateTime.Today;

            //적용월 칸에 오늘 날짜가 들어가도록 했지만, 자꾸 변경하는 걸 잊는다고 하셔서, 빈 칸으로 두고 체크 데이터를 태워보자.
            //2020.08.05, 장가빈
            //dtpgrb_Apply_Year_Month.SelectedDate = DateTime.Now;

            if (tbnOutware.IsChecked == true)
            {
                MainItemAutoSelecting();
            }
        }



        #region (출고/입고 메인아이템 자동선택) MainItemAutoSelecting
        // 출고 / 입고 버튼 Selecting에 따라 각 항목의 메인아이템 Y인거 찾아서 그거 자동으로 넣어주기.
        private void MainItemAutoSelecting()
        {
            DataSet ds = null;
            if (tbnOutware.IsChecked == true)
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("bsGbnID", "2"); //매출
                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_BS_sMainItemOne", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;
                    if (dt.Rows.Count == 0)
                    {
                        // 효과없음 끝.
                    }
                    else
                    {
                        DataRow dr = dt.Rows[0];
                        txtgrb_INOutItem.Tag = dr["BSItemCode"].ToString();
                        txtgrb_INOutItem.Text = dr["BSItemName"].ToString();
                    }
                }
            }
            else if (tbnStuffin.IsChecked == true)
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("bsGbnID", "1"); //매입
                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_BS_sMainItemOne", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;
                    if (dt.Rows.Count == 0)
                    {
                        // 효과없음 끝.
                    }
                    else
                    {
                        DataRow dr = dt.Rows[0];
                        txtgrb_INOutItem.Tag = dr["BSItemCode"].ToString();
                        txtgrb_INOutItem.Text = dr["BSItemName"].ToString();
                    }
                }
            }
        }

        #endregion


        #region (상단 조회조건 체크박스 enable 모음)

        //  입고/출고 토글버튼
        private void tbnOutware_Checked(object sender, RoutedEventArgs e)
        {
            tbnStuffin.IsChecked = false;
            tbnOutware.IsChecked = true;

            // 출고버튼 클릭. > 명칭변경 및 항목, 그리드 체인지.
            tbnOutware_CheckedChange();

            //검색 버튼 누를 때 마다 전체선택 버튼 리셋 하기
            chkSelectAll.IsChecked = false;

            //출고버튼을 체크하면 출고의 월 데이트피커가 보이도록.
            dtpOutEDate.Visibility = Visibility.Visible;
            dtpOutSDate.Visibility = Visibility.Visible;

            //입고의 데이트피커는 hidden 처리
            dtpInEDate.Visibility = Visibility.Hidden;
            dtpInSDate.Visibility = Visibility.Hidden;


        }
        //  입고/출고 토글버튼
        private void tbnOutware_Unchecked(object sender, RoutedEventArgs e)
        {
            tbnOutware.IsChecked = false;
            tbnStuffin.IsChecked = true;
        }

        //  입고/출고 토글버튼
        private void tbnStuffin_Checked(object sender, RoutedEventArgs e)
        {
            tbnOutware.IsChecked = false;
            tbnStuffin.IsChecked = true;

            // 입고버튼 클릭. > 명칭변경 및 항목, 그리드 체인지.
            tbnStuffin_CheckedChange();

            //검색 버튼 누를 때 마다 전체선택 버튼 리셋 하기
            chkSelectAll.IsChecked = false;


            //입버튼을 체크하면 입고의 데이트피커가 보이도록.
            dtpInSDate.Visibility = Visibility.Visible;
            dtpInEDate.Visibility = Visibility.Visible;

            //출고의 데이트피커는 hidden 처리
            dtpOutSDate.Visibility = Visibility.Hidden;
            dtpOutEDate.Visibility = Visibility.Hidden;

        }
        //  입고/출고 토글버튼
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
            if(tbnOutware.IsChecked == true)
            {
                dtpOutEDate.IsEnabled = true;
                dtpOutSDate.IsEnabled = true;
            }
            else
            {
                dtpInEDate.IsEnabled = true;
                dtpInSDate.IsEnabled = true;
            }

            //dtpSDate.IsEnabled = true;
            //dtpEDate.IsEnabled = true;

        }
        // 기간
        private void chkPeriod_Unchecked(object sender, RoutedEventArgs e)
        {
            if(tbnOutware.IsChecked == true)
            {
                dtpOutEDate.IsEnabled = false;
                dtpOutSDate.IsEnabled = false;
            }
            else
            {
                dtpInEDate.IsEnabled = false;
                dtpInSDate.IsEnabled = false;
            }                       
            //dtpSDate.IsEnabled = false;
            //dtpEDate.IsEnabled = false;
        }


        #region (상단 조회 일자변경 버튼 이벤트)
        //금일
        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            if(tbnOutware.IsChecked == true)
            {
                dtpOutEDate.SelectedDate = DateTime.Today;
                dtpOutSDate.SelectedDate = DateTime.Today;
            }
            else
            {
                dtpInEDate.SelectedDate = DateTime.Today;
                dtpInSDate.SelectedDate = DateTime.Today;
            }

            //dtpSDate.SelectedDate = DateTime.Today;
            //dtpEDate.SelectedDate = DateTime.Today;
        }
        //금월
        private void btnThisMonth_Click(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                dtpOutSDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[0];
                dtpOutEDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[1];
            }
            else
            {
                dtpInSDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[0];
                dtpInEDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[1];
            }
                       
            //dtpSDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[0];
            //dtpEDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[1];
        }
        //전월
        private void btnLastMonth_Click(object sender, RoutedEventArgs e)
        {
            //dtpSDate.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[0];
            //dtpEDate.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[1];

            if(tbnOutware.IsChecked == true)  //dtpOutEDate  //dtpOutSDate
            {
                if (dtpOutSDate.SelectedDate != null)
                {
                    DateTime ThatMonth1 = dtpOutSDate.SelectedDate.Value.AddDays(-(dtpOutSDate.SelectedDate.Value.Day - 1)); // 선택한 일자 달의 1일!

                    DateTime LastMonth1 = ThatMonth1.AddMonths(-1); // 저번달 1일
                    DateTime LastMonth31 = ThatMonth1.AddDays(-1); // 저번달 말일

                    dtpOutSDate.SelectedDate = LastMonth1;
                    dtpOutEDate.SelectedDate = LastMonth31;
                }
                else
                {
                    DateTime ThisMonth1 = DateTime.Today.AddDays(-(DateTime.Today.Day - 1)); // 이번달 1일

                    DateTime LastMonth1 = ThisMonth1.AddMonths(-1); // 저번달 1일
                    DateTime LastMonth31 = ThisMonth1.AddDays(-1); // 저번달 말일

                    dtpOutSDate.SelectedDate = LastMonth1;
                    dtpOutEDate.SelectedDate = LastMonth31;
                }
            }
            else
            {
                if (dtpInSDate.SelectedDate != null) //dtpInEDate //dtpInSDate
                {
                    DateTime ThatMonth1 = dtpInSDate.SelectedDate.Value.AddDays(-(dtpInSDate.SelectedDate.Value.Day - 1)); // 선택한 일자 달의 1일!

                    DateTime LastMonth1 = ThatMonth1.AddMonths(-1); // 저번달 1일
                    DateTime LastMonth31 = ThatMonth1.AddDays(-1); // 저번달 말일

                    dtpInSDate.SelectedDate = LastMonth1;
                    dtpInEDate.SelectedDate = LastMonth31;
                }
                else
                {
                    DateTime ThisMonth1 = DateTime.Today.AddDays(-(DateTime.Today.Day - 1)); // 이번달 1일

                    DateTime LastMonth1 = ThisMonth1.AddMonths(-1); // 저번달 1일
                    DateTime LastMonth31 = ThisMonth1.AddDays(-1); // 저번달 말일

                    dtpInSDate.SelectedDate = LastMonth1;
                    dtpInEDate.SelectedDate = LastMonth31;
                }
            }

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
        //전일
        private void btnYesterday_Click(object sender, RoutedEventArgs e)
        {
            //dtpSDate.SelectedDate = DateTime.Today.AddDays(-1);
            //dtpEDate.SelectedDate = DateTime.Today.AddDays(-1);

            if(tbnOutware.IsChecked == true)  //dtpOutSDate //dtpOutEDate 
            {
                if (dtpOutSDate.SelectedDate != null)
                {
                    dtpOutSDate.SelectedDate = dtpOutSDate.SelectedDate.Value.AddDays(-1);
                    dtpOutEDate.SelectedDate = dtpOutSDate.SelectedDate;
                }
                else
                {
                    dtpOutSDate.SelectedDate = DateTime.Today.AddDays(-1);
                    dtpOutEDate.SelectedDate = DateTime.Today.AddDays(-1);
                }
            }
            else
            {
                if (dtpInSDate.SelectedDate != null) //dtpInSDate //dtpInEDate
                {
                    dtpInSDate.SelectedDate = dtpInSDate.SelectedDate.Value.AddDays(-1);
                    dtpInEDate.SelectedDate = dtpInSDate.SelectedDate;
                }
                else
                {
                    dtpInSDate.SelectedDate = DateTime.Today.AddDays(-1);
                    dtpInEDate.SelectedDate = DateTime.Today.AddDays(-1);
                }
            }
                                                  

            //if (dtpSDate.SelectedDate != null)
            //{
            //    dtpSDate.SelectedDate = dtpSDate.SelectedDate.Value.AddDays(-1);
            //    dtpEDate.SelectedDate = dtpSDate.SelectedDate;
            //}
            //else
            //{
            //    dtpSDate.SelectedDate = DateTime.Today.AddDays(-1);
            //    dtpEDate.SelectedDate = DateTime.Today.AddDays(-1);
            //}

        }

        #endregion


        // 관리사업장
        private void lblLeadWorkHouse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkLeadWorkHouse.IsChecked == true) { chkLeadWorkHouse.IsChecked = false; }
            else { chkLeadWorkHouse.IsChecked = true; }
        }
        // 관리사업장
        private void chkLeadWorkHouse_Checked(object sender, RoutedEventArgs e)
        {
            cboLeadWorkHouse.IsEnabled = true;
        }
        // 관리사업장
        private void chkLeadWorkHouse_Unchecked(object sender, RoutedEventArgs e)
        {
            cboLeadWorkHouse.IsEnabled = false;
        }
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
            btnArticle.IsEnabled = true;
            txtArticle.Focus();
        }
        // 품명
        private void chkArticle_Unchecked(object sender, RoutedEventArgs e)
        {
            txtArticle.IsEnabled = false;
            btnArticle.IsEnabled = false;
        }

        #endregion

        #region (플러스파인더 호출묶음) PlusFinder

        // 플러스파인더 >> 거래처.
        private void btnPfCustom_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
        }
        // 플러스파인더 >> 거래처
        private void txtCustom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
            }
        }

        // 플러스파인더 >> 품명 종류
        private void btnPfArticle_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtArticle, (int)Defind_CodeFind.DCF_Article, "");
        }
        // 플러스파인더 >> 품명 종류
        private void txtArticle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtArticle, (int)Defind_CodeFind.DCF_Article, "");
            }
        }



        // 플러스파인더 >> 그룹박스 내 매출항목
        private void btngrbpf_INOutItem_Click(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                MainWindow.pf.ReturnCode(txtgrb_INOutItem, (int)Defind_CodeFind.DCF_BSItemCode, "2");
            }
            else if (tbnStuffin.IsChecked == true)
            {
                MainWindow.pf.ReturnCode(txtgrb_INOutItem, (int)Defind_CodeFind.DCF_BSItemCode, "1");
            }
            
        }
        // 플러스파인더 >> 그룹박스 내 매출항목
        private void txtgrb_INOutItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbnOutware.IsChecked == true)
                {
                    MainWindow.pf.ReturnCode(txtgrb_INOutItem, (int)Defind_CodeFind.DCF_BSItemCode, "2");
                }
                else if (tbnStuffin.IsChecked == true)
                {
                    MainWindow.pf.ReturnCode(txtgrb_INOutItem, (int)Defind_CodeFind.DCF_BSItemCode, "1");
                }
            }
        }


        #endregion

        #region (콤보박스 세팅) SetComboBox
        private void SetComboBox()
        {
            ObservableCollection<CodeView> ovcWorkHouse = ComboBoxUtil.Instance.Get_CompanyID();
            cboLeadWorkHouse.ItemsSource = ovcWorkHouse;
            cboLeadWorkHouse.DisplayMemberPath = "code_name";
            cboLeadWorkHouse.SelectedValuePath = "code_id";
            cboLeadWorkHouse.SelectedIndex = 0;
        }

        #endregion

        #region (토글버튼 체크 체인지 이벤트) CheckedChange
        private void tbnOutware_CheckedChange()
        {
            btngrb_BuySaleProcess.Content = "매출처리";
            lblgrb_InOutItem.Content = "매출항목";

            lblArticle.IsEnabled = true;
            if (chkArticle.IsChecked == true)
            {
                txtArticle.IsEnabled = true;
                btnArticle.IsEnabled = true;
            }

            dgdInGrid.Visibility = Visibility.Hidden;
            dgdOutGrid.Visibility = Visibility.Visible;

            //적용월 칸에 오늘 날짜가 들어가도록 했지만, 자꾸 변경하는 걸 잊는다고 하셔서, 빈 칸으로 두고 체크 데이터를 태워보자.
            //2020.08.05, 장가빈
            //dtpgrb_Apply_Year_Month.SelectedDate = DateTime.Now;
            txtgrb_INOutItem.Text = string.Empty;

            MainItemAutoSelecting();
        }

        private void tbnStuffin_CheckedChange()
        {
            btngrb_BuySaleProcess.Content = "매입처리";
            lblgrb_InOutItem.Content = "매입항목";

            lblArticle.IsEnabled = false;
            txtArticle.IsEnabled = false;
            btnArticle.IsEnabled = false;

            dgdOutGrid.Visibility = Visibility.Hidden;
            dgdInGrid.Visibility = Visibility.Visible;

            //적용월 칸에 오늘 날짜가 들어가도록 했지만, 자꾸 변경하는 걸 잊는다고 하셔서, 빈 칸으로 두고 체크 데이터를 태워보자.
            //2020.08.05, 장가빈
            //dtpgrb_Apply_Year_Month.SelectedDate = DateTime.Now;
            txtgrb_INOutItem.Text = string.Empty;

            MainItemAutoSelecting();
        }

        #endregion



        // 검색버튼 클릭.
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                FillGrid_OutGrid();
            }
            else if (tbnStuffin.IsChecked == true)
            {
                FillGrid_InGrid();
            }

            //검색 버튼 누를 때 마다 전체선택 버튼 리셋 하기
            chkSelectAll.IsChecked = false;
        }


        #region (매출용 그리드 채우기) FillGrid_OutGrid
        // 매출용 그리드 채우기.
        private void FillGrid_OutGrid()
        {
            if (dgdOutGrid.Items.Count > 0)
            {
                dgdOutGrid.Items.Clear();
            }

            try
            {
                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("sFromDate", chkPeriod.IsChecked == true ? dtpOutSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("sToDate", chkPeriod.IsChecked == true ? dtpOutEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("sCompanyID", cboLeadWorkHouse.SelectedValue);
                sqlParameter.Add("CustomID", chkCustom.IsChecked == true ? txtCustom.Tag.ToString() : "");
                sqlParameter.Add("ArticleID", chkArticle.IsChecked == true ? txtArticle.Tag.ToString() : "");
                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_Outware_Q_WPF", sqlParameter, false);

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
                            var WinAccBS_AddINOut = new Win_Acc_BS_AddINOut_U_CodeView()
                            {
                                Num = i + 1,
                                IsCheck = false,

                                BasisYearMon = dr["BasisYearMon"].ToString(),
                                BSItem = dr["BSItem"].ToString(), // 시작할때ㅡ 여기 두칸은 빈칸으로.
                                OutDate = dr["OutDate"].ToString(),
                                OrderNo = dr["OrderNO"].ToString(),
                                OrderID = dr["OrderID"].ToString(),
                                OrderSeq = dr["OrderSeq"].ToString(),

                                Article = dr["Article"].ToString(),
                                KCustom = dr["KCustom"].ToString(),
                                OutQty = dr["OutQty"].ToString(),
                                UnitPrice = dr["UnitPrice"].ToString(),
                                Amount = dr["Amount"].ToString(),
                                VATAmount = dr["VATAmount"].ToString(),
                                TotalAmount = dr["TotalAmount"].ToString(),
                                VatINDYN = dr["VatINDYN"].ToString(),
                                CurrencyUnit = dr["CurrencyUnit"].ToString(),
                                OrderSpec = dr["OrderSpec"].ToString(),
                                OutWareID = dr["OutWareID"].ToString(),
                                Outroll = dr["Outroll"].ToString(),
                                CurrencyUnitID  = dr["CurrencyUnitID"].ToString(),
                                CompanyID = dr["CompanyID"].ToString(),
                                ArticleID = dr["ArticleID"].ToString(),
                                BusinessChargeID = dr["BusinessChargeID"].ToString(),
                                CustomID = dr["CustomID"].ToString(),
                                BSItemCode = dr["BSItemCode"].ToString(),
                                


                            };

                            dgdOutGrid.Items.Add(WinAccBS_AddINOut);
                            i++;
                        }
                    }
                    txtblockSearchCount.Text = "검색건수 : " + i.ToString() + "건.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }

        }

        #endregion

        #region (매입용 그리드 채우기) FillGrid_InGrid
        // 매입용 그리드 채우기.
        private void FillGrid_InGrid()
        {
            if (dgdInGrid.Items.Count > 0)
            {
                dgdInGrid.Items.Clear();
            }

            try
            {
                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("sFromDate", chkPeriod.IsChecked == true ? dtpInSDate.SelectedDate.Value.ToString("yyyyMMdd") : "19900101");
                sqlParameter.Add("sToDate", chkPeriod.IsChecked == true ? dtpInEDate.SelectedDate.Value.ToString("yyyyMMdd") : "21000101");
                sqlParameter.Add("sCompanyID", cboLeadWorkHouse.SelectedValue);
                sqlParameter.Add("CustomID", chkCustom.IsChecked == true ? txtCustom.Tag.ToString() : "");
                sqlParameter.Add("ArticleID", chkArticle.IsChecked == true ? txtArticle.Tag.ToString() : "");

                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_Inware_Q_WPF", sqlParameter, false);

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
                            var WinAccBS_AddIN = new Win_Acc_BS_AddIN_U_CodeView()
                            {
                                Num = i + 1,
                                IsCheck = false,


                                Color = dr["Color"].ToString(),
                                BSdate = dr["BSdate"].ToString(),
                                KCustom = dr["KCustom"].ToString(),
                                StuffSeq = dr["StuffSeq"].ToString(),
                                Article = dr["Article"].ToString(),
                                Amount = stringFormatN0(dr["Amount"]),
                                UnitPrice = stringFormatN0(dr["UnitPrice"]),
                                VAT_IND_YN = stringFormatN0(dr["VAT_IND_YN"]),

                                VatAmount = stringFormatN0(dr["VatAmount"]),
                                InQty = stringFormatN0(dr["InQty"]),
                                TotalAmount = stringFormatN0(dr["TotalAmount"]),
                                StuffinID = dr["StuffinID"].ToString(),
                                CustomID = dr["CustomID"].ToString(),
                                CurrencyUnitID = dr["CurrencyUnitID"].ToString(),
                                Remark = dr["Remark"].ToString(),
                                BasisYearMon = dr["BasisYearMon"].ToString(),
                                ArticleID = dr["ArticleID"].ToString(),
                     

                            };

                            //입고일자
                            WinAccBS_AddIN.BSdate = DatePickerFormat(WinAccBS_AddIN.BSdate);

                            dgdInGrid.Items.Add(WinAccBS_AddIN);
                            i++;
                        }
                    }
                    txtblockSearchCount.Text = "검색건수 : " + i.ToString() + "건.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }

        }

        #endregion


        #region (그리드 하단 전체선택 클릭 관련)
        // 전체선택 클릭 관련
        private void tbkSelectAll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkSelectAll.IsChecked == true) { chkSelectAll.IsChecked = false; }
            else { chkSelectAll.IsChecked = true; }
        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                if (dgdOutGrid.Items.Count > 0)
                {
                    foreach (Win_Acc_BS_AddINOut_U_CodeView WinAccBS_AddINOut in dgdOutGrid.Items)
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
                    foreach (Win_Acc_BS_AddIN_U_CodeView WinAccBS_AddIN in dgdInGrid.Items)
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

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                if (dgdOutGrid.Items.Count > 0)
                {
                    foreach (Win_Acc_BS_AddINOut_U_CodeView WinAccBS_AddINOut in dgdOutGrid.Items)
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
                    foreach (Win_Acc_BS_AddIN_U_CodeView WinAccBS_AddIN in dgdInGrid.Items)
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

        #endregion

        #region 항목적용 / 적용 취소
        // 항목적용 버튼 클릭.
        private void btngrb_ItemApply_Click(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                if (tbnOutware.IsChecked == true)
                {
                    foreach (Win_Acc_BS_AddINOut_U_CodeView WinAccBS_AddINOut in dgdOutGrid.Items)
                    {
                        if (WinAccBS_AddINOut != null)
                        {
                            if (WinAccBS_AddINOut.IsCheck == true)
                            {
                                WinAccBS_AddINOut.BasisYearMon = dtpgrb_Apply_Year_Month.SelectedDate.Value.ToString("yyyyMM");
                                WinAccBS_AddINOut.BSItem = txtgrb_INOutItem.Text;
                                WinAccBS_AddINOut.BSItemCode = txtgrb_INOutItem.Tag.ToString();
                            }
                        }
                    }
                    btngrb_BuySaleProcess.IsEnabled = true;
                    dgdOutGrid.Items.Refresh();
                }
                else if (tbnStuffin.IsChecked == true)
                {
                    foreach (Win_Acc_BS_AddIN_U_CodeView WinAccBS_AddIN in dgdInGrid.Items)
                    {
                        if (WinAccBS_AddIN != null)
                        {
                            if (WinAccBS_AddIN.IsCheck == true)
                            {
                                WinAccBS_AddIN.BasisYearMon = dtpgrb_Apply_Year_Month.SelectedDate.Value.ToString("yyyyMM");
                                WinAccBS_AddIN.Article = txtgrb_INOutItem.Text;
                                WinAccBS_AddIN.ArticleID = txtgrb_INOutItem.Tag.ToString();
                            }
                        }
                    }
                    btngrb_BuySaleProcess.IsEnabled = true;
                    dgdInGrid.Items.Refresh();
                }
            }                        
        }

        // 항목적용 취소버튼.
        private void btngrb_ItmeCancel_Click(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                foreach (Win_Acc_BS_AddINOut_U_CodeView WinAccBS_AddINOut in dgdOutGrid.Items)
                {
                    if (WinAccBS_AddINOut != null)
                    {
                        if (WinAccBS_AddINOut.IsCheck == true)
                        {
                            WinAccBS_AddINOut.BasisYearMon = string.Empty;
                            WinAccBS_AddINOut.BSItem = string.Empty;
                        }
                    }
                }
                btngrb_BuySaleProcess.IsEnabled = false;
                dgdOutGrid.Items.Refresh();
            }
            else if (tbnStuffin.IsChecked == true)
            {
                foreach (Win_Acc_BS_AddIN_U_CodeView WinAccBS_AddIN in dgdInGrid.Items)
                {
                    if (WinAccBS_AddIN != null)
                    {
                        if (WinAccBS_AddIN.IsCheck == true)
                        {
                            WinAccBS_AddIN.BasisYearMon = string.Empty;
                            WinAccBS_AddIN.Article = string.Empty;
                        }
                    }
                }
                btngrb_BuySaleProcess.IsEnabled = false;
                dgdInGrid.Items.Refresh();
            }            
        }

        #endregion




        // 닫기버튼 클릭.
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }


        // 매입 / 매출 처리버튼.
        private void btngrb_BuySaleProcess_Click(object sender, RoutedEventArgs e)
        {
            // 매출처리.
            if (tbnOutware.IsChecked == true)
            {
                if (dgdOutGrid.Items.Count > 0 && dgdOutGrid.SelectedItem != null)
                {
                    Wh_Ar_SelectedLastIndex = dgdOutGrid.SelectedIndex;
                }

                if (SaveAcc_Outware())
                {
                    if (tbnOutware.IsChecked == true)
                    {
                        Wh_Ar_SelectedLastIndex -= 1;
                        FillGrid_OutGrid();
                        if (dgdOutGrid.Items.Count > 0)
                        {
                            dgdOutGrid.SelectedIndex = Wh_Ar_SelectedLastIndex;
                            dgdOutGrid.Focus();
                        }
                    }
                }
            }

            // 반품/환불(매입처리) 처리.
            if (tbnStuffin.IsChecked == true)
            {
                if (dgdInGrid.Items.Count > 0 && dgdInGrid.SelectedItem != null)
                {
                    Wh_Ar_SelectedLastIndex = dgdInGrid.SelectedIndex;
                }

                if (SaveAcc_Stuffin())
                {
                    Wh_Ar_SelectedLastIndex -= 1;
                    FillGrid_InGrid();
                    if (dgdInGrid.Items.Count > 0)
                    {
                        dgdInGrid.SelectedIndex = Wh_Ar_SelectedLastIndex;
                        dgdInGrid.Focus();
                    }
                }
            }
        }


        #region (매출처리버튼 클릭.) SaveAcc_Outware
        // 매출처리
        private bool SaveAcc_Outware()
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                int CheckCount = 0;

                foreach (Win_Acc_BS_AddINOut_U_CodeView WinAccBS_AddINOut in dgdOutGrid.Items)
                {
                    if (WinAccBS_AddINOut != null)
                    {
                        if (WinAccBS_AddINOut.IsCheck == true)
                        { CheckCount++; }
                    }
                }
                // 체크된 그리드가 하나 이상 있을 경우에.
                if (CheckCount > 0)
                {
                    double D_UnitPrice = 0;
                    double D_QTY = 0;
                    double D_Amount = 0;
                    double D_VATAmount = 0;
                    double D_TotalAmount = 0;

                    foreach (Win_Acc_BS_AddINOut_U_CodeView WinAccBS_AddINOut in dgdOutGrid.Items)
                    {
                        if (WinAccBS_AddINOut != null)
                        {
                            if (WinAccBS_AddINOut.IsCheck == true)
                            {

                                //Double.TryParse(WinAccBS_AddINOut.UnitPrice, out D_UnitPrice);
                                //Double.TryParse(WinAccBS_AddINOut.OutQty, out D_QTY);
                                Double.TryParse(WinAccBS_AddINOut.Amount, out D_Amount);
                                Double.TryParse(WinAccBS_AddINOut.VATAmount, out D_VATAmount);
                                Double.TryParse(WinAccBS_AddINOut.TotalAmount, out D_TotalAmount);


                                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                                sqlParameter.Clear();
                                sqlParameter.Add("sYYYYMM", WinAccBS_AddINOut.BasisYearMon);
                                sqlParameter.Add("sBSGbn", "2");    // SALE = 2
                                sqlParameter.Add("sCompanyID", "");
                                sqlParameter.Add("sBSItemCode", WinAccBS_AddINOut.BSItemCode);
                                sqlParameter.Add("sInOutWareNo", WinAccBS_AddINOut.OutWareID);

                                sqlParameter.Add("sInOutDate", WinAccBS_AddINOut.OutDate);
                                sqlParameter.Add("sCustomID", WinAccBS_AddINOut.CustomID);
                                sqlParameter.Add("sCurrencyUnit", "0");  //화페단위 원화. cm_code where code_gbn = 'CMMPRC'
                                sqlParameter.Add("sSales_Charge", "");

                                sqlParameter.Add("sBSPlace", "");
                                sqlParameter.Add("nRollQty", 0);
                                sqlParameter.Add("nUnitPrice", 0);
                                sqlParameter.Add("nQTY", 0);
                                sqlParameter.Add("nAmount", D_Amount);

                                sqlParameter.Add("sVat_IND_YN", ""); //받아오는 값 없음. 
                                sqlParameter.Add("sOrderID", "");
                                sqlParameter.Add("sColor", "");
                                sqlParameter.Add("nVATAmount", D_VATAmount);
                                sqlParameter.Add("nTotalAmount", D_TotalAmount);

                                sqlParameter.Add("sComments", ""); 
                                sqlParameter.Add("nOrderSeq", 1);  //?? 이건 모르겠음
                                sqlParameter.Add("sCreateUserID", MainWindow.CurrentUser);
                                sqlParameter.Add("OutMsg", "");
    
                                

                                Procedure pro1 = new Procedure();
                                pro1.Name = "xp_Acc_BS_iFromInOutBatch_WPF";
                                pro1.OutputUseYN = "N";
                                pro1.OutputName = "";
                                pro1.OutputLength = "10";

                                Prolist.Add(pro1);
                                ListParameter.Add(sqlParameter);                                
                            }
                        }
                    }

                    List<KeyValue> list_Result = new List<KeyValue>();
                    list_Result = DataStore.Instance.ExecuteAllProcedureOutputGetCS(Prolist, ListParameter);
                    string sGetID = string.Empty;

                    if (list_Result[0].key.ToLower() == "success")
                    {
                        flag = true;
                    }
                    else
                    {
                        MessageBox.Show("[저장실패]\r\n" + list_Result[0].value.ToString());
                        flag = false;
                    }                    
                }
                else
                {
                    MessageBox.Show("[저장실패]\r\n 매출처리할 체크항목이 없습니다.");
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
                flag = false;
            }
            finally
            {
                MessageBox.Show("매출처리가 완료되었습니다");
                DataStore.Instance.CloseConnection();
            }
            return flag;
        }

        #endregion

        #region (매입처리 버튼 클릭.) SaveAcc_Stuffin()
        private bool SaveAcc_Stuffin()
        {

            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                int CheckCount = 0;

                foreach (Win_Acc_BS_AddIN_U_CodeView WinAccBS_AddIN in dgdInGrid.Items)
                {
                    if (WinAccBS_AddIN != null)
                    {
                        if (WinAccBS_AddIN.IsCheck == true)
                        { CheckCount++; }
                    }
                }
                // 체크된 그리드가 하나 이상 있을 경우에.
                if (CheckCount > 0)
                {
                    //double D_UnitPrice = 0;
                    //double D_QTY = 0;
                    double D_Amount = 0;
                    double D_VATAmount = 0;
                    double D_TotalAmount = 0;

                    foreach (Win_Acc_BS_AddIN_U_CodeView WinAccBS_AddIN in dgdInGrid.Items)
                    {
                        if (WinAccBS_AddIN != null)
                        {
                            if (WinAccBS_AddIN.IsCheck == true)
                            {
                                //Double.TryParse(WinAccBS_AddIN.UnitPrice, out D_UnitPrice);
                                //Double.TryParse(WinAccBS_AddIN.Qty, out D_QTY);
                                Double.TryParse(WinAccBS_AddIN.Amount, out D_Amount);
                                Double.TryParse(WinAccBS_AddIN.VatAmount, out D_VATAmount);
                                Double.TryParse(WinAccBS_AddIN.TotalAmount, out D_TotalAmount);


                                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                                sqlParameter.Clear();
                                //sqlParameter.Add("sYYYYMM", WinAccBS_AddIN.BasisYearMon);
                                sqlParameter.Add("sBSGbn", "1");    //BUY = 1
                                sqlParameter.Add("sCompanyID", "");
                                //sqlParameter.Add("sBSItem", WinAccBS_AddIN.INOutItem);
                               // sqlParameter.Add("sBSItemCode", WinAccBS_AddIN.INOutItem_ID);

                                //sqlParameter.Add("sInOutDate", WinAccBS_AddIN.Indate.Replace("-", ""));
                                sqlParameter.Add("sCustomID", WinAccBS_AddIN.CustomID);
                                sqlParameter.Add("sCurrencyUnit", "0"); //화페단위 원화. cm_code where code_gbn = 'CMMPRC' 
                                sqlParameter.Add("sSales_Charge", "");

                                sqlParameter.Add("sBSPlace", "");
                                sqlParameter.Add("nRollQty", 0);
                                sqlParameter.Add("nUnitPrice", WinAccBS_AddIN.UnitPrice);
                                sqlParameter.Add("nQTY", WinAccBS_AddIN.InQty);
                                sqlParameter.Add("nAmount", D_Amount);

                                sqlParameter.Add("sVat_IND_YN", ""); //받아오는 값 없음. 
                                sqlParameter.Add("sOrderID", "");

                                sqlParameter.Add("sArticleID", "");
                                sqlParameter.Add("sArticle", "");
                                sqlParameter.Add("sColor", "");
                                sqlParameter.Add("nVATAmount", D_VATAmount);
                                sqlParameter.Add("nTotalAmount", D_TotalAmount);

                                sqlParameter.Add("sComments", WinAccBS_AddIN.Remark);

                                sqlParameter.Add("nOrderSeq", 1); //?? 이건 모르겠음
                                sqlParameter.Add("sCreateUserID", MainWindow.CurrentUser);

                                sqlParameter.Add("OutMsg", "");
                                sqlParameter.Add("ExchRate", 0);
                                sqlParameter.Add("StuffinID", WinAccBS_AddIN.StuffinID);



                                Procedure pro1 = new Procedure();
                                pro1.Name = "xp_Acc_BS_iFromInOutBatch_WPF";
                                pro1.OutputUseYN = "N";
                                pro1.OutputName = "";
                                pro1.OutputLength = "10";

                                Prolist.Add(pro1);
                                ListParameter.Add(sqlParameter);
                            }
                        }
                    }

                    List<KeyValue> list_Result = new List<KeyValue>();
                    list_Result = DataStore.Instance.ExecuteAllProcedureOutputGetCS(Prolist, ListParameter);
                    string sGetID = string.Empty;

                    if (list_Result[0].key.ToLower() == "success")
                    {
                        flag = true;
                    }
                    else
                    {
                        MessageBox.Show("[저장실패]\r\n" + list_Result[0].value.ToString());
                        flag = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
                flag = false;
            }
            finally
            {
                MessageBox.Show("매입처리가 완료되었습니다");
                DataStore.Instance.CloseConnection();
            }

            return flag;
        }



        #endregion



        // 엑셀 버튼 클릭.
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] dgdStr = new string[2];
            if (tbnOutware.IsChecked == true)
            {
                dgdStr[0] = "일괄출고 리스트";
                dgdStr[1] = dgdOutGrid.Name;
            }
            else
            {
                dgdStr[0] = "일괄입고 리스트";
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

        //데이터 체크
        private bool CheckData()
        {
            bool flag = true;
            //[적용 월]을 선택했는지 확인
            if (dtpgrb_Apply_Year_Month.SelectedDate == null)
            {
                MessageBox.Show("적용 월이 입력되지 않았습니다. 캘린더를 열어 적용 월을 선택해주세요.");
                flag = false;
                return flag;
            }


            return flag;
        }




        #region 기타 

        //천 단위 구분기호, 소수점 자릿수 0 
        private string stringFormatN0(object obj)
        {
            return string.Format("{0:N0}", obj);
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


        #endregion

    }






    class Win_Acc_BS_AddINOut_U_CodeView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }
        public bool IsCheck { get; set; }                // 체크

        public string BasisYearMon { get; set; }             // 매출월
        public string BSItem { get; set; }           // 매출 항목
        public string OutDate { get; set; }         // 발행일

        public string OrderSeq { get; set; }             //출고순번
        public string OrderNo { get; set; }             // 오더번호
        public string OrderID { get; set; }             // 오더ID

        public string CustomID { get; set; }            // 거래처ID
        public string KCustom { get; set; }             // 거래처
        public string ArticleID { get; set; }           // 품명ID
        public string Article { get; set; }             // 품명
        public string Cnt { get; set; }                 // 건 수

        public string OutQty { get; set; }              // 수량
        public string UnitPrice { get; set; }           // 단가
        public string Amount { get; set; }           // 금액
        public string VATAmount { get; set; }                 // 부가세
        public string TotalAmount { get; set; }      // 합계금액

        public string VatINDYN { get; set; }             // 부가세 포함여부
        public string CurrencyUnit { get; set; }           // 화폐단위ID
        public string CurrencyUnitID { get; set; }           // 화폐단위ID
        public string PriceClssName { get; set; }       // 화폐단위
        public string OutWareID { get; set; }       // 출고번호
        public string Outroll { get; set; }       // 건수

        public string OrderSpec { get; set; }              // 비고
        public string ExchRate { get; set; }            //환율
        public string CompanyID { get; set; }            //사업장ID
        public string BusinessChargeID { get; set; }            //영업담당자ID
        public string BSItemCode { get; set; }            
           
    }


    // (매입)(입고) 그리드.
    class Win_Acc_BS_AddIN_U_CodeView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }
        public bool IsCheck { get; set; }                // 체크
        public string Color { get; set; }                

        public string BSdate { get; set; }             // 입고일 
        public string StuffSeq { get; set; }           // 입고순번

        public string KCustom { get; set; }              // 거래처
        public string InQty { get; set; }               // 수량
        public string UnitPrice { get; set; }           // 단가
        public string Amount { get; set; }              // 금액
        public string Article { get; set; }              // 품명

        public string VatAmount { get; set; }           // 부가세
        public string TotalAmount { get; set; }         // 합계금액
        public string VAT_IND_YN { get; set; }            // 부가세 별도
        public string CustomID { get; set; }            //거래처ID
        public string CurrencyUnitID { get; set; }       //화폐단위
        public string BasisYearMon { get; set; }       //화폐단위
        
        public string Remark { get; set; }                 // 비고
        public string StuffinID{ get; set; }            // 매입항목 ID   
        public string ArticleID { get; set; }            // 매입항목    


    }



}
