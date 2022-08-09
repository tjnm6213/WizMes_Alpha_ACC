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
    /// Win_Acc_BS_Summary_Q.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 


    public partial class Win_Acc_BS_Summary_Q : UserControl
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

        string bsGbnID = string.Empty;
        string DyeAuxGroupID = string.Empty;

        public Win_Acc_BS_Summary_Q()
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

            tbnOutware.IsChecked = true;  // 로드시 매출버튼 기본선택.
            chkCompany.IsChecked = true;
            cboCompany.SelectedIndex = 0;
            YYYY.IsChecked = true;


            //처음 화면 로드시 집계항목은 모두 체크되어 있는 상태로 출력.
            chkCollectionYear.IsChecked = false;
            chkCollectionMonth.IsChecked = false;
            chkCollectionCustom.IsChecked = false;
            chkCollectionArticle.IsChecked = false; // 이건은 매출항목

        }


        #region (상단 조회조건 체크박스 enable 모음)
        // 입금 / 출금 토글버튼
        private void tbnOutware_Checked(object sender, RoutedEventArgs e)
        {
            tbnStuffin.IsChecked = false;
            tbnOutware.IsChecked = true;

            // 매출버튼 클릭. > 명칭변경 및 항목, 그리드 체인지.
            tbnOutware_Checked();
        }
        // 입금 / 출금 토글버튼
        private void tbnOutware_Unchecked(object sender, RoutedEventArgs e)
        {
            tbnOutware.IsChecked = false;
            tbnStuffin.IsChecked = true;
        }
        // 입금 / 출금 토글버튼
        private void tbnStuffin_Checked(object sender, RoutedEventArgs e)
        {
            tbnOutware.IsChecked = false;
            tbnStuffin.IsChecked = true;


            // 출금버튼 클릭. > 명칭변경 및 항목, 그리드 체인지.
            tbnStuffin_Checked();
        }
        // 입금 / 출금 토글버튼
        private void tbnStuffin_Unchecked(object sender, RoutedEventArgs e)
        {
            tbnStuffin.IsChecked = false;
            tbnOutware.IsChecked = true;

        }

        private void YYYY_Click(object sender, RoutedEventArgs e)
        {
            dtpEDate.Visibility = Visibility.Visible;
            dtpSDate.Visibility = Visibility.Visible;
            dtpEDate2.Visibility = Visibility.Hidden;
            dtpSDate2.Visibility = Visibility.Hidden;

            DateTime today = DateTime.Now.Date;
            DateTime firstday = today.AddDays(1 - today.Day);
            DateTime lastday = firstday.AddMonths(1).AddDays(-1);
            dtpSDate.SelectedDate = firstday;
            dtpEDate.SelectedDate = lastday;
        }

        private void YYYYMM_Click(object sender, RoutedEventArgs e)
        {
            dtpEDate2.Visibility = Visibility.Visible;
            dtpSDate2.Visibility = Visibility.Visible;
            dtpEDate.Visibility = Visibility.Hidden;
            dtpSDate.Visibility = Visibility.Hidden;

            DateTime today = DateTime.Now.Date;
            DateTime firstday = today.AddDays(1 - today.Day);
            DateTime lastday = firstday.AddMonths(1).AddDays(-1);
            dtpSDate2.SelectedDate = firstday;
            dtpEDate2.SelectedDate = lastday;

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

        //매출사업장
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
        //매출사업장
        private void chkCompany_UnChecked(object sender, RoutedEventArgs e)
        {
            cboCompany.IsEnabled = false;
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
            btnpfCustom.IsEnabled = true;
            txtCustom.Focus();
        }
        // 거래처
        private void chkCustom_UnChecked(object sender, RoutedEventArgs e)
        {
            txtCustom.IsEnabled = false;
            btnpfCustom.IsEnabled = false;
        }
        // 거래처
        private void txtCustom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
            }
        }



        // 매출항목
        private void lblBSItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkBSItem.IsChecked == true) { chkBSItem.IsChecked = false; }
            else { chkBSItem.IsChecked = true; }
        }
        // 매출항목
        private void chkBSItem_Checked(object sender, RoutedEventArgs e)
        {
            txtBSItem.IsEnabled = true;
            btnPfBSItem.IsEnabled = true;
            txtBSItem.Focus();
        }
        // 매출항목
        private void chkBSItem_UnChecked(object sender, RoutedEventArgs e)
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
        private void lblOrder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkOrder.IsChecked == true) { chkOrder.IsChecked = false; }
            else { chkOrder.IsChecked = true; }
        }
        // 오더번호
        private void chkOrder_Checked(object sender, RoutedEventArgs e)
        {
            txtOrder.IsEnabled = true;
            txtOrder.Focus();
        }
        // 오더번호
        private void chkOrder_Unchecked(object sender, RoutedEventArgs e)
        {
            txtOrder.IsEnabled = false;
        }



        // 화폐
        private void lblMoney_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkMoney.IsChecked == true) { chkMoney.IsChecked = false; }
            else { chkMoney.IsChecked = true; }
        }
        // 화폐
        private void chkMoney_Checked(object sender, RoutedEventArgs e)
        {
            cboMoney.IsEnabled = true;
        }
        // 화폐
        private void chkMoney_Unchecked(object sender, RoutedEventArgs e)
        {
            cboMoney.IsEnabled = false;
        }


        #endregion

        #region (플러스파인더 모음)

        // 플러스파인더 >> 거래처.
        private void btnPfCustom_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
        }



        // 플러스파인더 >> 매출항목
        private void btnPfBSItem_Click(object sender, RoutedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)   //매출 
            {
                MainWindow.pf.ReturnCode(txtBSItem, 32, "Out");
            }
            else
            {
                MainWindow.pf.ReturnCode(txtBSItem, 31, "In");
            }
        }

        // 플러스파인더 >> 영업사원
        private void btnPfSalesCharge_Click(object sender, RoutedEventArgs e)
        {
            // 4번.
            MainWindow.pf.ReturnCode(txtSalesCharge, (int)Defind_CodeFind.DCF_SalesCharge, "");
        }
        // 플러스파인더 >> 영업사원
        private void txtSalesCharge_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 4번.
                MainWindow.pf.ReturnCode(txtSalesCharge, (int)Defind_CodeFind.DCF_SalesCharge, "");
            }
        }


        //플러스파인더 >> 품명
        private void txtArticle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtArticle, (int)Defind_CodeFind.DCF_Article, "");
            }
        }
        // 플러스파인더 >> 품명
        private void btnPfArticle_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtArticle, (int)Defind_CodeFind.DCF_Article, "");
        }




        // 플러스파인더 >> 오더번호
        private void btnPfOrderNum_Click(object sender, RoutedEventArgs e)
        {
            // 4번.
            MainWindow.pf.ReturnCode(txtOrder, (int)Defind_CodeFind.DCF_ORDER, "");
        }
        // 플러스파인더 >> 오더번호
        private void txtOrderNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 4번.
                MainWindow.pf.ReturnCode(txtOrder, (int)Defind_CodeFind.DCF_ORDER, "");
            }
        }

        #endregion

        #region (콤보박스 세팅) SetComboBox
        private void SetComboBox()
        {

            //매입,매출 화폐단위(입력)
            List<string[]> listPrice = new List<string[]>();
            string[] Price01 = new string[] { "0", "₩" };
            string[] Price02 = new string[] { "1", "$" };
            string[] Price03 = new string[] { "2", "EUR" };
            string[] Price04 = new string[] { "3", "ALL" };
            listPrice.Add(Price01);
            listPrice.Add(Price02);

            ObservableCollection<CodeView> ovcPrice = ComboBoxUtil.Instance.Direct_SetComboBox(listPrice);
            this.cboMoney.ItemsSource = ovcPrice;
            this.cboMoney.DisplayMemberPath = "code_name";
            this.cboMoney.SelectedValuePath = "code_id";

            //매출거래처
            List<string[]> listSaleItems = new List<string[]>();
            string[] Saleitems01 = new string[] { "0", "(주)알파신소재" };
            listSaleItems.Add(Saleitems01);

            ObservableCollection<CodeView> ovcSaleItems = ComboBoxUtil.Instance.Direct_SetComboBox(listSaleItems);
            this.cboCompany.ItemsSource = ovcSaleItems;
            this.cboCompany.DisplayMemberPath = "code_name";
            this.cboCompany.SelectedValuePath = "code_id";

        }

        #endregion

        #region (토글버튼 체크 체인지 이벤트) CheckedChange
        // 매출 클릭.
        private void tbnOutware_Checked()
        {
            this.DataContext = null;
            Company.Text = "매출사업장";
            tbkBSItem.Text = "매출항목";
            month.Text = "매출월";

        }

        // 매입 클릭.
        private void tbnStuffin_Checked()
        {
            this.DataContext = null;
            Company.Text = "매입사업장";
            tbkBSItem.Text = "매입항목";
            month.Text = "매입월";


        }


        #endregion


        // 검색버튼 클릭.
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if(tbnOutware.IsChecked == true)
            {
                bsGbnID = "2";
            } else if(tbnStuffin.IsChecked == true)
            {
                bsGbnID = "1";
            }

            FillGrid_dgdOutSummaryGrid();
            
        }

        #region (검색 >> 매출입 집계) FillGrid_dgdSummaryGrid
        // 매출용 그리드 채우기.
        private void FillGrid_dgdOutSummaryGrid()
        {
            if (dgdSummaryGrid.Items.Count > 0)
            {
                dgdSummaryGrid.Items.Clear();
            }

            try
            {
                //매출/ 매입 토글박스 구분.
                if (tbnOutware.IsChecked == true) { bsGbnID = "2"; }
                else if (tbnStuffin.IsChecked == true) { bsGbnID = "1"; }

                // 일자 체크여부 yn
                int sBSDate = 0;
                if (YYYY.IsChecked == true) { sBSDate = 1; } //발생일 기준 yyyy-mm-dd
                else if (YYYYMM.IsChecked == true) { sBSDate = 2; } //매입매출월 기준 yyyy-mm  


                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Clear();

                sqlParameter.Add("bsGbnID", bsGbnID);       // 매출 매입 구분자.
                sqlParameter.Add("sBSDate", sBSDate);
                sqlParameter.Add("sDate", chkPeriod.IsChecked == true ? dtpSDate.SelectedDate.Value.ToString("yyyyMM") : "");
                sqlParameter.Add("eDate", chkPeriod.IsChecked == true ? dtpEDate.SelectedDate.Value.ToString("yyyyMM") : "");

                sqlParameter.Add("CompanyID", chkCompany.IsChecked == true ? "0001" : "");
                sqlParameter.Add("CustomID", chkCustom.IsChecked == true && txtCustom.Tag != null ? txtCustom.Tag.ToString() : "");
                sqlParameter.Add("BSItemCode", chkBSItem.IsChecked == true && chkBSItem.Tag != null ? chkBSItem.Tag.ToString() : "");
                sqlParameter.Add("ArticleKind", chkSalesCharge.IsChecked == true && txtSalesCharge.Tag != null ? txtSalesCharge.Tag.ToString() : "");
                sqlParameter.Add("Article", chkArticle.IsChecked == true && txtArticle.Tag != null ? txtArticle.Tag.ToString() : "");
                sqlParameter.Add("OrderNo", "");

                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_BS_Sum_Q_WPF", sqlParameter, false);


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
                            i++;
                            var WinAccBSSummary = new Win_Acc_BS_Summary_Q_CodeView()
                            {
                                Num = i,
                                TotalAmount = dr["TotalAmount"].ToString(),
                                CurrencyUnitName = dr["CurrencyUnitName"].ToString(),
                                YYYY = dr["BSDate"].ToString(),
                                BasisYearMon = dr["BasisYearMon"].ToString(),
                                BSItemName = dr["BSItemName"].ToString(),
                                CustomShort = dr["CustomShort"].ToString(),
                                CustomID = dr["CustomID"].ToString(),
                                ArticleKind = dr["ArticleKind"].ToString(),
                                Article = dr["Article"].ToString(),
                                Qty = stringFormatN0(dr["Qty"]),
                                Per = "0.00"
                            };
                            // 콤마입히기 > 합계금액
                            if (Lib.Instance.IsNumOrAnother(WinAccBSSummary.TotalAmount))
                            {
                                WinAccBSSummary.TotalAmount = Lib.Instance.returnNumStringZero(WinAccBSSummary.TotalAmount);
                            }
                            dgdSummaryGrid.Items.Add(WinAccBSSummary);
                        }
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion


      



        //매출 / 매입 토글버튼
        #region 매출입 토글버튼
        private void chkCollectionYear_Checked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionYear_UnChecked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionMonth_Checked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionMonth_UnChecked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionCustom_Checked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionCustom_UnChecked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionArticle_Checked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionArticle_UnChecked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionBSItemCode_Checked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionBSItemCode_UnChecked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionArticleKind_Checked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }
        private void chkCollectionArticleKind_UnChecked(object sender, RoutedEventArgs e)
        {
            CollectionListAddMinusWithDatagrid();
        }

        #endregion


        #region (집계항목 체크 및 그리드 visible 작업) CollectionListAddMinusWithDatagrid
        private void CollectionListAddMinusWithDatagrid()
        {
            int i = 1;
            dgdtxtcolYYYY.Visibility = Visibility.Hidden;
            dgdtxtcolMonth.Visibility = Visibility.Hidden;
            dgdtxtcolCustom.Visibility = Visibility.Hidden;
            dgdtxtcolArticle.Visibility = Visibility.Hidden;
            dgdtxtcolBSItemCode.Visibility = Visibility.Hidden;
            dgdtxtcolArticleKind.Visibility = Visibility.Hidden;  

            if (chkCollectionYear.IsChecked == true) //년도 
            {
                //tbkCollection1.Text = i.ToString();
                //tbkCollection1.Margin = new Thickness(3, 3, 3, 3);
                dgdtxtcolYYYY.Visibility = Visibility.Visible;
                i++;
            }
            if (chkCollectionMonth.IsChecked == true) //매출월 
            {
                //tbkCollection2.Text = i.ToString();
                //tbkCollection2.Margin = new Thickness(3, 3, 3, 3);
                dgdtxtcolMonth.Visibility = Visibility.Visible;
                i++;
            }
            if (chkCollectionCustom.IsChecked == true) //거래처 
            {
                //tbkCollection3.Text = i.ToString();
                //tbkCollection3.Margin = new Thickness(3, 3, 3, 3);
                dgdtxtcolCustom.Visibility = Visibility.Visible;
                i++;
            }
            if (chkCollectionArticle.IsChecked == true) //품명
            {
                //tbkCollection4.Text = i.ToString();
                //tbkCollection4.Margin = new Thickness(3, 3, 3, 3);
                dgdtxtcolArticle.Visibility = Visibility.Visible;
                i++;
            }
            if (chkCollectionBSItemCode.IsChecked == true) //항목 
            {
                dgdtxtcolBSItemCode.Visibility = Visibility.Visible;
                i++;
            }
            if (chkCollectionArticleKind.IsChecked == true) //품명종류
            {
                //tbkCollection4.Text = i.ToString();
                //tbkCollection4.Margin = new Thickness(3, 3, 3, 3);
                dgdtxtcolArticleKind.Visibility = Visibility.Visible;
                i++;
            }


        }


        #endregion

        // 닫기버튼 클릭.
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        // 데이터 그리드 항목 클릭_ SelectionChanged
        private void dgdInGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbnOutware.IsChecked == true)
            {
                var WinAccSummary = dgdSummaryGrid.SelectedItem as Win_Acc_BS_Summary_Q_CodeView;
                if (WinAccSummary != null)
                {
                    this.DataContext = WinAccSummary;
                }
            }
        }
        // 데이터 그리드 항목 클릭_ SelectionChanged
        private void dgdOutGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbnStuffin.IsChecked == true)
            {
                var WinAccSummary = dgdSummaryGrid.SelectedItem as Win_Acc_BS_Summary_Q_CodeView;
                if (WinAccSummary != null)
                {
                    this.DataContext = WinAccSummary;
                }
            }
        }

        //엑셀변환 요청하신 엑셀 파일로 수정. 2020.11.03, 장가빈
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            //매입 1, 매출 2
            string bsGbnID = tbnStuffin.IsChecked == true ? "1" : "2";
            string sDateMM = dtpSDate.SelectedDate.Value.ToString("yyyyMMdd").Substring(0, 6);
            string eDateMM = dtpEDate.SelectedDate.Value.ToString("yyyyMMdd").Substring(0, 6);
          


            try
            {
                #region 기존 엑셀 이벤트

                //DataTable dt = null;
                //string Name = string.Empty;

                //string[] dgdStr = new string[2];
                //if (tbnOutware.IsChecked == true)
                //{
                //    dgdStr[0] = "매출 집계";
                //    dgdStr[1] = dgdOutSummaryGrid.Name;
                //}
                //else
                //{
                //    dgdStr[0] = "매입 집계";
                //    dgdStr[1] = dgdSummaryGrid.Name;
                //}

                //ExportExcelxaml ExpExc = new ExportExcelxaml(dgdStr);
                //ExpExc.ShowDialog();

                //if (ExpExc.DialogResult.HasValue)
                //{
                //    if (ExpExc.choice.Equals(dgdSummaryGrid.Name))
                //    {
                //        if (ExpExc.Check.Equals("Y"))
                //            dt = Lib.Instance.DataGridToDTinHidden(dgdSummaryGrid);
                //        else
                //            dt = Lib.Instance.DataGirdToDataTable(dgdSummaryGrid);

                //        Name = dgdSummaryGrid.Name;
                //        if (Lib.Instance.GenerateExcel(dt, Name))
                //            Lib.Instance.excel.Visible = true;
                //        else
                //            return;
                //    }
                //    else if (ExpExc.choice.Equals(dgdOutSummaryGrid.Name))
                //    {
                //        if (ExpExc.Check.Equals("Y"))
                //            dt = Lib.Instance.DataGridToDTinHidden(dgdOutSummaryGrid);
                //        else
                //            dt = Lib.Instance.DataGirdToDataTable(dgdOutSummaryGrid);

                //        Name = dgdOutSummaryGrid.Name;
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
                DataTable dt = get_BS_SummayList(bsGbnID, sDateMM, eDateMM, DyeAuxGroupID);

                // 엑셀 시작
                excelapp = new Microsoft.Office.Interop.Excel.Application();


                string MyBookPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Report\\매입.출 집계표 양식.xls";
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
                int PageAll = (int)Math.Ceiling(rowCount / 37.0);       //전체페이지 변수
                int DataCount = 0;          //데이터 반영 활용 변수
                int excelNum = 0;                  //엑셀 행번호 변수


                int startRowIndex = 5; // 시작하는 행
                //int endRowIndex = 37; // 마지막 행

                int excelRow = 0;

                for (int k = 0; k < dt.Rows.Count; k++)
                {

                    if (DataCount == 37 * Page)     //페이지 수 곱하기 한 페이지에 들어갈 수 있는 데이터 값과 같아지면
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
                            copyLine = ((Page - 1) * 43);      // copy 시작 값

                            // 기존에 있는 데이터 지우기
                            worksheet.Range["A5", "H41"].EntireRow.ClearContents();
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

                        //매입일 경우 
                        if (bsGbnID.Equals("1"))
                        {
                            workrange = worksheet.get_Range("A1");
                            workrange.Value2 = "매입 집계표";
                        }
                        else
                        {
                            workrange = worksheet.get_Range("A1");
                            workrange.Value2 = "매출 집계표";
                        }

                        //매입일 경우 
                        if (bsGbnID.Equals("1"))
                        {
                            workrange = worksheet.get_Range("E4");
                            workrange.Value2 = "매입항목";
                        }
                        else
                        {
                            workrange = worksheet.get_Range("E4");
                            workrange.Value2 = "매출항목";
                        }

                    }

                    //엑셀 행 지정
                    excelRow = startRowIndex + excelNum;

                    // 순번
                    workrange = worksheet.get_Range("A" + excelRow);
                    workrange.Value2 = k + 1;


                    // 매입(매출)항목
                    //workrange = worksheet.get_Range("E" + excelRow);        // 년도
                    workrange = worksheet.get_Range("B" + excelRow);
                    workrange.Value2 = dr["YYYY"].ToString();

                    // 월
                    workrange = worksheet.get_Range("C" + excelRow);
                    workrange.Value2 = dr["MM"].ToString();

                    // 거래처
                    workrange = worksheet.get_Range("D" + excelRow);
                    workrange.Value2 = dr["KCustom"].ToString().Trim();

                    //매입항목
                    workrange = worksheet.get_Range("E" + excelRow);
                    workrange.Value2 = dr["BSItemName"].ToString().Trim();

                    // 공급가액
                    workrange = worksheet.get_Range("F" + excelRow);
                    workrange.Value2 = chkNullNum(dr["Amount"]);

                    // 부가가치세
                    workrange = worksheet.get_Range("G" + excelRow);
                    workrange.Value2 = chkNullNum(dr["VATAmount"]);

                    // 합계금액
                    workrange = worksheet.get_Range("H" + excelRow);
                    workrange.Value2 = chkNullNum(dr["TotalAmount"]);

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
        private DataTable get_BS_SummayList(string bsGbnID, string sDateMM, string eDateMM, string DyeAuxGroupID)
        {
            DataTable dt = new DataTable();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("bsGbnID", bsGbnID);
                sqlParameter.Add("sDateMM", sDateMM);
                sqlParameter.Add("eDateMM", eDateMM);
                sqlParameter.Add("DyeAuxGroupID", DyeAuxGroupID);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_BS_Summary_Q_ForExcel", sqlParameter, false);

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

        private object chkNullNum(object num)
        {
            if (num == null) { return 0; }

            return num;
        }

        private string getYear(string str)
        {
            str = str.Trim().Replace("-", "").Replace("/", "").Replace(".", "");

            if (str.Length == 8)
            {
                str = str.Substring(0, 4);
            }

            return str;
        }

        private string getMonth(string str)
        {
            str = str.Trim().Replace("-", "").Replace("/", "").Replace(".", "");

            if (str.Length == 8)
            {
                str = str.Substring(4, 2);
            }

            return str;
        }

        #endregion



        class Win_Acc_BS_Summary_Q_CodeView
        {
            public override string ToString()
            {
                return (this.ReportAllProperties());
            }

            public int Num { get; set; }

            public string Qty { get; set; }
            public string TotalAmount { get; set; }
            public string CurrencyUnitName { get; set; }
            public string Per { get; set; }
            public string YYYY { get; set; }

            public string BasisYearMon { get; set; }
            public string CustomShort { get; set; }
            public string CustomID { get; set; }
            public string BSItemName { get; set; }
            public string ArticleKind { get; set; }
            public string Article { get; set; }



        }

       
    }
}
