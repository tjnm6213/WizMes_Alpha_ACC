﻿using System;
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
    /// Win_Acc_Remain_Summary_Q.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_Acc_Remain_Summary_Q : UserControl
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

        string RPGbn = string.Empty;

        public Win_Acc_Remain_Summary_Q()
        {
            InitializeComponent();
        }

        // 로드 이벤트.
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);

            //chkPeriod.IsChecked = true;

            SetComboBox();
            tbnOutware.IsChecked = true;  // 로드시 수금버튼 기본선택.


            //처음 화면 로드시 집계항목은 모두 체크되어 있는 상태로 출력.
            chkCompany.IsChecked = true;
            cboCompany.SelectedIndex = 0;
            //chkPeriod.IsChecked = true;
            dtpSDate.SelectedDate = DateTime.Today;
            //YYYY.IsChecked = true;

        }



        #region (상단 조회조건 체크박스 enable 모음)
        // 수금/지불 토글버튼
        private void tbnOutware_Checked(object sender, RoutedEventArgs e)
        {
            tbnStuffin.IsChecked = false;
            tbnOutware.IsChecked = true;

            // 매출버튼 클릭. > 명칭변경 및 항목, 그리드 체인지.
            tbnOutware_Checked();
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
            

            // 출금버튼 클릭. > 명칭변경 및 항목, 그리드 체인지.
            tbnStuffin_Checked();
        }
       
        private void tbnStuffin_Unchecked(object sender, RoutedEventArgs e)
        {
            tbnStuffin.IsChecked = false;
            tbnOutware.IsChecked = true;


        }




        // 기간
        private void lblPeriod_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (chkPeriod.IsChecked == true) { chkPeriod.IsChecked = false; }
            //else { chkPeriod.IsChecked = true; }
        }
        // 기간
        private void chkPeriod_Checked(object sender, RoutedEventArgs e)
        {
            dtpSDate.IsEnabled = true;
 
        }
        // 기간
        private void chkPeriod_Unchecked(object sender, RoutedEventArgs e)
        {
            dtpSDate.IsEnabled = false;
  
        }
        private void YYYY_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.Visibility = Visibility.Visible;
           
           
            DateTime today = DateTime.Now.Date;
            DateTime firstday = today.AddDays(1 - today.Day);
            dtpSDate.SelectedDate = firstday;
          
        }

        private void YYYYMM_Click(object sender, RoutedEventArgs e)
        {
          
            dtpSDate.Visibility = Visibility.Hidden;


            DateTime today = DateTime.Now.Date;
            DateTime firstday = today.AddDays(1 - today.Day);
            DateTime lastday = firstday.AddMonths(1).AddDays(-1);
           

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
            //btnPfCustom.IsEnabled = true;
            txtCustom.Focus();
        }
        // 거래처
        private void chkCustom_Unchecked(object sender, RoutedEventArgs e)
        {
            txtCustom.IsEnabled = false;
            btnpfCustom.IsEnabled = false;
            //btnPfCustom.IsEnabled = false;
        }
        // 거래처
        private void txtCustom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
            }
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
            cboMoney.Focus();
        }
        // 화폐
        private void chkMoney_Unchecked(object sender, RoutedEventArgs e)
        {
            cboMoney.IsEnabled = false;
           
        }



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
        private void chkCompany_Unchecked(object sender, RoutedEventArgs e)
        {
            cboCompany.IsEnabled = false;
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

        // 입금계정
        
        // 입금계정
        private void chkBSItem_Checked(object sender, RoutedEventArgs e)
        {
            txtBSItem.IsEnabled = true;
            btnPfBSItem.IsEnabled = true;
            txtBSItem.Focus();
        }
        // 입금계정
        private void chkBSItem_Unchecked(object sender, RoutedEventArgs e)
        {
            txtBSItem.IsEnabled = false;
            btnPfBSItem.IsEnabled = false;
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



        // 플러스파인더 >> 품명
        //private void txtArticle_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        MainWindow.pf.ReturnCode(txtArticle, (int)Defind_CodeFind.DCF_Article, "");
        //    }
        //}
        //// 플러스파인더 >> 품명
        //private void btnPfArticle_Click(object sender, RoutedEventArgs e)
        //{
        //    MainWindow.pf.ReturnCode(txtArticle, (int)Defind_CodeFind.DCF_Article, "");
        //}



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
            listPrice.Add(Price03);
            listPrice.Add(Price04);

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

            grbdgdOutGrid.Visibility = Visibility.Visible;
            lblMiddle.Visibility = Visibility.Hidden;
            txtMiddle.Visibility = Visibility.Hidden;
            btnPfMiddle.Visibility = Visibility.Hidden;

            lblBSItem.Visibility = Visibility.Visible;
            txtBSItem.Visibility = Visibility.Visible;
            btnPfBSItem.Visibility = Visibility.Visible;
            
        }

        // 매입 클릭.
        private void tbnStuffin_Checked()
        {
            this.DataContext = null;

            grbdgdOutGrid.Visibility = Visibility.Visible;


            //매입 염조제 중분류 상단조건 visible;
            lblMiddle.Visibility = Visibility.Visible;
            txtMiddle.Visibility = Visibility.Visible;
            btnPfMiddle.Visibility = Visibility.Visible;
            lblBSItem.Visibility = Visibility.Hidden;
            txtBSItem.Visibility = Visibility.Hidden;
            btnPfBSItem.Visibility = Visibility.Hidden;
            

        }


        #endregion


        // 검색버튼 클릭.
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ////검색버튼을 누르기 전에 집계항목 중 하나라도 체크가 되어 있어야 한다.)
            //if(chkCollectionArticle.IsChecked == false 
            //    && chkCollectionCustom.IsChecked == false
            //    && chkCollectionMonth.IsChecked == false
            //    && chkCollectionYear.IsChecked == false)
            //{
            //    MessageBox.Show("집계항목 중 하나라도 체크가 되어 있어야 합니다.");
            //    return;
            //}
            //else
            //{
                if (tbnOutware.IsChecked == true) // 매출용 그리드
                {
                RPGbn = "2";
                FillGrid();
                }
                else if (tbnStuffin.IsChecked == true) // 매입용 그리드
                {
                RPGbn = "1";
                FillGrid();
                }
            //}
        }

        #region (검색 >> 매출입 집계) FillGrid_dgdOutSummaryGrid
        // 수금용 그리드 채우기.
        private void FillGrid()
        {
            if (dgdOutSummaryGrid.Items.Count > 0)
            {
                dgdOutSummaryGrid.Items.Clear();
            }

            try
            {
                //매출/ 매입 토글박스 구분.
                




                DataSet ds = null;
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Clear();

                sqlParameter.Add("sDate", dtpSDate.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("sBSGbn", tbnStuffin.IsChecked == true ? "2" : "1");    //1:buy 2:sale
                sqlParameter.Add("nChkCompanyID", chkCompany.IsChecked == true ? "1" : "0");

                sqlParameter.Add("sCompanyID ", chkCompany.IsChecked == true ? "0001" : "");
                sqlParameter.Add("nChkCustom", chkCustom.IsChecked == true ? "1" : "0");
                sqlParameter.Add("CustomID", txtCustom.Text != null ? txtCustom.Text.ToString() : "" );
                sqlParameter.Add("nChkRPItemcode", chkBSItem.IsChecked == true ? "1" : "0");    // 입금계정 
                sqlParameter.Add("RPItemcode", txtBSItem.Text != null ? txtBSItem.Text.ToString() : "");

                sqlParameter.Add("nChkBusinessCharge", chkSalesCharge.IsChecked == true ? "1" : "0");
                sqlParameter.Add("BusinessCharge", txtSalesCharge.Text != null ? txtSalesCharge.Text.ToString() : "");
                sqlParameter.Add("nChkCurrencyUnit", chkMoney.IsChecked == true ? "1" : "0");
                sqlParameter.Add("CurrencyUnit", chkMoney.IsChecked == true ? cboMoney.SelectedValue.ToString() : "");



                ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_RP_RemainSumbyCustom_Q", sqlParameter, false);


                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int i = 0;
                    
                    if (dt.Rows.Count == 1)
                    {
                        MessageBox.Show("조회된 데이터가 없습니다.");
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;
                        foreach (DataRow dr in drc)
                        {
                            i++;
                                var WinAccBSSummary = new Win_Acc_Remain_Summary_Q_CodeView()
                                {
                                    Num = i,
                                    customID = dr["customID"].ToString(),
                                    KCustom = dr["KCustom"].ToString(),
                                    RemainAmount = dr["RemainAmount"].ToString()


                                };
                                // 콤마입히기 > 수량
                                //if (Lib.Instance.IsNumOrAnother(WinAccBSSummary.QTY))
                                //{
                                //    WinAccBSSummary.QTY = Lib.Instance.returnNumStringZero(WinAccBSSummary.QTY);
                                //}
                               
                                // 콤마입히기 > 잔액
                                if (Lib.Instance.IsNumOrAnother(WinAccBSSummary.RemainAmount))
                                {
                                    WinAccBSSummary.RemainAmount = Lib.Instance.returnNumStringZero(WinAccBSSummary.RemainAmount);
                                }
                              dgdOutSummaryGrid.Items.Add(WinAccBSSummary);
                        }
                        SearchCount.Text = "검색건수 : " + i.ToString() + " 건";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
        }

        #endregion


 

        // 닫기버튼 클릭.
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

       
        // 데이터 그리드 항목 클릭_ SelectionChanged
        private void dgdOutGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbnStuffin.IsChecked == true)
            {
                var WinAccSummary = dgdOutSummaryGrid.SelectedItem as Win_Acc_Remain_Summary_Q_CodeView;
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
            string RPGbn = tbnStuffin.IsChecked == true ? "1" : "2";
            string sDateMM = dtpSDate.SelectedDate.Value.ToString("yyyyMMdd").Substring(0, 6);

            string DyeAuxGroupID = chkMiddle.IsChecked == true ? (txtMiddle.Tag.ToString() != null ? txtMiddle.Tag.ToString(): ""): "" ;


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
                DataTable dt = get_BS_SummayList(RPGbn, sDateMM);

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
                        if (RPGbn.Equals("1"))
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
                        if (RPGbn.Equals("1"))
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
        private DataTable get_BS_SummayList(string @RPGbn, string sDateMM)
        {
            DataTable dt = new DataTable();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("@RPGbn", @RPGbn);
                sqlParameter.Add("sDateMM", sDateMM);
 

                DataSet ds = DataStore.Instance.ProcedureToDataSet("3" +
                    "", sqlParameter, false);

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

        //중분류 라벨 이벤트
        private void LblMiddle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkMiddle.IsChecked == true)
            {
                chkMiddle.IsChecked = false;
            }
            else
            {
                chkMiddle.IsChecked = true;
            }
        }

        //중분류 체크박스
        private void ChkMiddle_Checked(object sender, RoutedEventArgs e)
        {
            txtMiddle.IsEnabled = true;
            btnPfMiddle.IsEnabled = true;
            txtMiddle.Focus();
        }

        //중분류 체크해제
        private void ChkMiddle_Unchecked(object sender, RoutedEventArgs e)
        {
            txtMiddle.IsEnabled = false;
            btnPfMiddle.IsEnabled = false;
        }

        //중분류 키다운
        private void TxtMiddle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtMiddle, 83, "");
            }
        }

        //중분류 플러스파인더
        private void btnPfMiddle_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtMiddle, 83, "");
        }

        //인쇄 
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog().GetValueOrDefault())
            {
                FontFamily fontFamily = new FontFamily("나눔고딕코딩");

                Grid grid = new Grid();

                grid.SetValue(FontFamilyProperty, fontFamily);
                grid.SetValue(FontSizeProperty, 32d);

                for (int i = 0; i < 5 ; i++)
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

        //더블클릭하면 팝업 띄우기 
        private void dgdOutSummaryGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 재고현황(제품포함)
            int i = 0;
            foreach (MenuViewModel mvm in MainWindow.mMenulist)
            {
                if (mvm.Menu.Equals("거래처 잔액일보(총괄)"))
                {
                    break;
                }
                i++;
            }
            try
            {
               
                    Type type = Type.GetType("WizMes_Alpha." + MainWindow.mMenulist[i].ProgramID.Trim(), true);
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
            catch (Exception ex)
            {
                MessageBox.Show("해당 화면이 존재하지 않습니다.");
            }
        }
    }

    class Win_Acc_Remain_Summary_Q_CodeView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }
        public bool IsCheck { get; set; }
        public string customID { get; set; }
        public string KCustom { get; set; }
        public string RemainAmount { get; set; }
       
       
       
    }



}
