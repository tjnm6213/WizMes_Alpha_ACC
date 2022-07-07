
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Xml;
using WizMes_Alpha_JA.PopUp;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// Win_App_ApprovalReq_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_App_ApprovalReq_U : UserControl
    {
        int rowNum = 0;
        string strFlag = "";

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

        // 인쇄 활용 용도 (프린트)
        private Microsoft.Office.Interop.Excel.Application excelapp;
        private Microsoft.Office.Interop.Excel.Workbook workbook;
        private Microsoft.Office.Interop.Excel.Worksheet worksheet;
        private Microsoft.Office.Interop.Excel.Range workrange;
        private Microsoft.Office.Interop.Excel.Worksheet copysheet;
        private Microsoft.Office.Interop.Excel.Worksheet pastesheet;
        WizMes_Alpha_JA.PopUp.NoticeMessage msg = new PopUp.NoticeMessage();

        List<App_Stemp> lstStemp = new List<App_Stemp>();
        List<App_Stemp> lstStep = new List<App_Stemp>();

        // 매입리스트 변수
        //List<App_IN_CodeView> lstAppIn = new List<App_IN_CodeView>();

        List<App_IN_CodeView2> lstAppIn = new List<App_IN_CodeView2>();
        List<App_IN_CodeView2> lstAppOut = new List<App_IN_CodeView2>();

        List<string> lstChkIn = new List<string>();
        List<string> lstChkOut = new List<string>();


        // 입출금
        public List<App_CashIn_CodeView2> lstApp_CashIn = new List<App_CashIn_CodeView2>();
        public List<App_CashIn_CodeView2> lstApp_CashOut = new List<App_CashIn_CodeView2>();

        public List<string> lstChk_CashIn = new List<string>();
        public List<string> lstChk_CashOut = new List<string>();

        public Win_App_ApprovalReq_U()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            //FillGridIsStemp();

            SetComboBox();

            // 요청일자에 오늘날짜 세팅
            chkDateSrh.IsChecked = true;
            dtpSDateSrh.SelectedDate = DateTime.Today;
            dtpEDateSrh.SelectedDate = DateTime.Today;
        }

        #region 도장 이미지 관리 버튼 보이기 / 안보이기 - 권한 조회

        private void FillGridIsStemp()
        {
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("PersonID", MainWindow.CurrentPersonID);
                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sChkStempSub", sqlParameter, false);
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    btnStemp.Visibility = Visibility.Visible;
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

        #endregion // 도장 이미지 관리 버튼 보이기 / 안보이기 - 권한 조회

        #region 추가, 수정 모드 / 저장완료, 취소 모드

        private void SaveUpdateMode()
        {
            btnAdd.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSearch.IsEnabled = false;

            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;

            btnExcel.IsEnabled = false;

            //if (strFlag.Trim().Equals("I"))
            //{
            //    tbkMsg.Text = "자료 추가중";
            //}
            //else
            //{
            //    tbkMsg.Text = "자료 수정중";
            //}
            //lblMsg.Visibility = Visibility.Visible;

            //gbxInput.IsHitTestVisible = true;
            // 위의 그룹박스로 했었는데, 요청 내용이 너무 길어지면 스크롤을 내려야 하는데 이용자체가 불가능하니, 각자 개별로 관리...
            cboAppStep.IsHitTestVisible = true;
            dtpReqDate.IsHitTestVisible = true;
            txtTitle.IsReadOnly = false;
            txtRequester.IsHitTestVisible = true;
            btnPfRequester.IsEnabled = true;
            txtContent.IsReadOnly = false;
            txtRemark.IsReadOnly = false;
            cboHandle.IsHitTestVisible = true;

            grdRbn.IsHitTestVisible = true;
            dgdMain.IsEnabled = false;

            // 첨부파일 부분
            btnUpload_M1.IsEnabled = true;
            btnUpload1.IsEnabled = true;
            btnUpload_C1.IsEnabled = true;
            btnDel1.IsEnabled = true;
            btnDel_M1.IsEnabled = true;
            btnDel_C1.IsEnabled = true;

            btnUpload2.IsEnabled = true;
            btnUpload_M2.IsEnabled = true;
            btnUpload_C2.IsEnabled = true;
            btnDel2.IsEnabled = true;
            btnDel_M2.IsEnabled = true;
            btnDel_C2.IsEnabled = true;

            btnUpload3.IsEnabled = true;
            btnUpload_M3.IsEnabled = true;
            btnUpload_C3.IsEnabled = true;
            btnDel3.IsEnabled = true;
            btnDel_M3.IsEnabled = true;
            btnDel_C3.IsEnabled = true;

            btnUpload4.IsEnabled = true;
            btnUpload_M4.IsEnabled = true;
            btnUpload_C4.IsEnabled = true;
            btnDel4.IsEnabled = true;
            btnDel_M4.IsEnabled = true;
            btnDel_C4.IsEnabled = true;

            btnUpload5.IsEnabled = true;
            btnUpload_M5.IsEnabled = true;
            btnUpload_C5.IsEnabled = true;
            btnDel5.IsEnabled = true;
            btnDel_M5.IsEnabled = true;
            btnDel_C5.IsEnabled = true;
        }

        private void CompleteCancelMode()
        {
            btnAdd.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSearch.IsEnabled = true;

            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;

            btnExcel.IsEnabled = true;

            //lblMsg.Visibility = Visibility.Hidden;

            //gbxInput.IsHitTestVisible = false;
            cboAppStep.IsHitTestVisible = false;
            dtpReqDate.IsHitTestVisible = false;
            txtTitle.IsReadOnly = true;
            txtRequester.IsHitTestVisible = false;
            btnPfRequester.IsEnabled = false;
            txtContent.IsReadOnly = true;
            txtRemark.IsReadOnly = true;
            cboHandle.IsHitTestVisible = false;

            grdRbn.IsHitTestVisible = false;
            dgdMain.IsEnabled = true;

            // 첨부파일 부분
            btnUpload1.IsEnabled = false;
            btnUpload_M1.IsEnabled = false;
            btnUpload_C1.IsEnabled = false;
            btnDel1.IsEnabled = false;
            btnDel_M1.IsEnabled = false;
            btnDel_C1.IsEnabled = false;

            btnUpload2.IsEnabled = false;
            btnUpload_M2.IsEnabled = false;
            btnUpload_C2.IsEnabled = false;
            btnDel2.IsEnabled = false;
            btnDel_M2.IsEnabled = false;
            btnDel_C2.IsEnabled = false;

            btnUpload3.IsEnabled = false;
            btnUpload_M3.IsEnabled = false;
            btnUpload_C3.IsEnabled = false;
            btnDel3.IsEnabled = false;
            btnDel_M3.IsEnabled = false;
            btnDel_C3.IsEnabled = false;

            btnUpload4.IsEnabled = false;
            btnUpload_M4.IsEnabled = false;
            btnUpload_C4.IsEnabled = false;
            btnDel4.IsEnabled = false;
            btnDel_M4.IsEnabled = false;
            btnDel_C4.IsEnabled = false;

            btnUpload5.IsEnabled = false;
            btnUpload_M5.IsEnabled = false;
            btnUpload_C5.IsEnabled = false;
            btnDel5.IsEnabled = false;
            btnDel_M5.IsEnabled = false;
            btnDel_C5.IsEnabled = false;
        }

        #endregion // 추가, 수정 모드 / 저장완료, 취소 모드

        #region SetComboBox 콤보박스 세팅

        private void SetComboBox()
        {
            // 처리 Handle
            ObservableCollection<CodeView> ovcHandle = ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "APPHANDLE", "Y", "");
            this.cboHandle.ItemsSource = ovcHandle;
            this.cboHandle.DisplayMemberPath = "code_name";
            this.cboHandle.SelectedValuePath = "code_id";

            // 결재순서
            ObservableCollection<CodeView> ovcAppStep = GetApprovalStepGrp();
            this.cboAppStep.ItemsSource = ovcAppStep;
            this.cboAppStep.DisplayMemberPath = "code_name";
            this.cboAppStep.SelectedValuePath = "code_id";
        }

        #region 결재 순서 콤보박스 세팅

        public ObservableCollection<CodeView> GetApprovalStepGrp()
        {
            ObservableCollection<CodeView> retunCollection = new ObservableCollection<CodeView>();
            string sql = " select AppStepID, AppStepName";
            sql += " from App_ApprovalStep";
            sql += " where UseClss <> '*'";

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

        #endregion // 결재 순서 콤보박스 세팅

        #endregion // SetComboBox 콤보박스 세팅

        #region Header 부분 - 검색조건

        // 검색 이동일자 라벨 이벤트
        private void lblDateSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
        // 검색 이동일자 체크박스 이벤트
        private void chkDateSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkDateSrh.IsChecked = true;

            dtpSDateSrh.IsEnabled = true;
            dtpEDateSrh.IsEnabled = true;

            btnYesterday.IsEnabled = true;
            btnToday.IsEnabled = true;
            btnLastMonth.IsEnabled = true;
            btnThisMonth.IsEnabled = true;
        }
        private void chkDateSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkDateSrh.IsChecked = false;

            dtpSDateSrh.IsEnabled = false;
            dtpEDateSrh.IsEnabled = false;

            btnYesterday.IsEnabled = false;
            btnToday.IsEnabled = false;
            btnLastMonth.IsEnabled = false;
            btnThisMonth.IsEnabled = false;
        }
        //금일
        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            dtpSDateSrh.SelectedDate = DateTime.Today;
            dtpEDateSrh.SelectedDate = DateTime.Today;
        }
        //금월
        private void btnThisMonth_Click(object sender, RoutedEventArgs e)
        {
            dtpSDateSrh.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[0];
            dtpEDateSrh.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[1];
        }
        //전월
        private void btnLastMonth_Click(object sender, RoutedEventArgs e)
        {
            //dtpSDateSrh.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[0];
            //dtpEDateSrh.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[1];

            if (dtpSDateSrh.SelectedDate != null)
            {
                DateTime ThatMonth1 = dtpSDateSrh.SelectedDate.Value.AddDays(-(dtpSDateSrh.SelectedDate.Value.Day - 1)); // 선택한 일자 달의 1일!

                DateTime LastMonth1 = ThatMonth1.AddMonths(-1); // 저번달 1일
                DateTime LastMonth31 = ThatMonth1.AddDays(-1); // 저번달 말일

                dtpSDateSrh.SelectedDate = LastMonth1;
                dtpEDateSrh.SelectedDate = LastMonth31;
            }
            else
            {
                DateTime ThisMonth1 = DateTime.Today.AddDays(-(DateTime.Today.Day - 1)); // 이번달 1일

                DateTime LastMonth1 = ThisMonth1.AddMonths(-1); // 저번달 1일
                DateTime LastMonth31 = ThisMonth1.AddDays(-1); // 저번달 말일

                dtpSDateSrh.SelectedDate = LastMonth1;
                dtpEDateSrh.SelectedDate = LastMonth31;
            }
        }
        //전일
        private void btnYesterday_Click(object sender, RoutedEventArgs e)
        {
            //dtpSDateSrh.SelectedDate = DateTime.Today.AddDays(-1);
            //dtpEDateSrh.SelectedDate = DateTime.Today.AddDays(-1);

            if (dtpSDateSrh.SelectedDate != null)
            {
                dtpSDateSrh.SelectedDate = dtpSDateSrh.SelectedDate.Value.AddDays(-1);
                dtpEDateSrh.SelectedDate = dtpSDateSrh.SelectedDate;
            }
            else
            {
                dtpSDateSrh.SelectedDate = DateTime.Today.AddDays(-1);
                dtpEDateSrh.SelectedDate = DateTime.Today.AddDays(-1);
            }
        }

        // 검색조건 - 제목
        private void lblTitleSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkTitleSrh.IsChecked == true)
            {
                chkTitleSrh.IsChecked = false;
            }
            else
            {
                chkTitleSrh.IsChecked = true;
            }
        }

        private void chkTitleSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkTitleSrh.IsChecked = true;
            txtTitleSrh.IsEnabled = true;
        }

        private void chkTitleSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkTitleSrh.IsChecked = false;
            txtTitleSrh.IsEnabled = false;
        }


        #endregion // Header 부분 - 검색조건

        #region Header 부분 - 오른쪽 상단 버튼 이벤트

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;

            strFlag = "I";
            dtpReqDate.SelectedDate = DateTime.Today;
            cboHandle.SelectedIndex = 0;

            txtRequester.Text = MainWindow.CurrentPerson;
            txtRequester.Tag = MainWindow.CurrentPersonID;

            SaveUpdateMode();

            lstAppIn.Clear();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var AppReq = dgdMain.SelectedItem as Win_App_ApprovalReq_U_CodeView;

            if (AppReq == null)
            {
                MessageBox.Show("수정할 데이터를 선택해주세요.");
            }
            else
            {
                if (!AppReq.UseClss.Trim().Equals("*"))
                {
                    strFlag = "U";
                SaveUpdateMode();
                rowNum = dgdMain.SelectedIndex;

                lstAppIn.Clear();
                }
                else
                {
                    MessageBox.Show("수정 권한이 없습니다.");
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var AppReq = dgdMain.SelectedItem as Win_App_ApprovalReq_U_CodeView;

            if (AppReq == null)
            {
                MessageBox.Show("삭제할 데이터를 선택해주세요.");
            }
            else
            {
                // 2020.09.09
                // 요청자와 지금 로그인한 사람이 같다면!!!!
                // 그럼 삭제가 가능 하도록
                if (AppReq.RequesterID.Trim().Equals(MainWindow.CurrentPersonID.Trim()))
                {

                    if (MessageBox.Show("선택한 항목을 삭제하시겠습니까?", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes) // \r\n(해당 결제건이 반환처리 됩니다.)
                    {
                        rowNum = dgdMain.SelectedIndex - 1;
                        if (rowNum < 0) { rowNum = 0; }

                        if (DeleteData(AppReq.AppReqID))
                        {
                            re_Search(rowNum);
                        }
                    }
                    
                }
                else
                {
                    MessageBox.Show("삭제 권한이 없습니다.");
                }

            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            rowNum = 0;
            re_Search(rowNum);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(SaveData(strFlag))
            {
                CompleteCancelMode();

                re_Search(rowNum);
                strFlag = "";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            CompleteCancelMode();
            strFlag = "";

            re_Search(rowNum);
        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            var App = dgdMain.SelectedItem as Win_App_ApprovalReq_U_CodeView;

            if (App != null)
            {
                PrintWork(false, true, App);
            }
        }

        #region 프린트 버튼

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var App = dgdMain.SelectedItem as Win_App_ApprovalReq_U_CodeView;

            if (App != null)
            {
                // 인쇄 메서드
                ContextMenu menu = btnPrint.ContextMenu;
                menu.StaysOpen = true;
                menu.IsOpen = true;
            }
        }

        private void menuSeeAhead_Click(object sender, RoutedEventArgs e)
        {
            var App = dgdMain.SelectedItem as Win_App_ApprovalReq_U_CodeView;

            if (App != null)
            {
                msg.Show();
                msg.Topmost = true;
                msg.Refresh();

                Lib.Instance.Delay(1000);

                PrintWork(true, true, App);

                msg.Visibility = Visibility.Hidden;
            }
        }

        private void menuRightPrint_Click(object sender, RoutedEventArgs e)
        {
            var App = dgdMain.SelectedItem as Win_App_ApprovalReq_U_CodeView;

            if (App != null)
            {
                msg.Show();
                msg.Topmost = true;
                msg.Refresh();

                Lib.Instance.Delay(1000);

                PrintWork(true, false, App);

                msg.Visibility = Visibility.Hidden;
            }
        }

        private void menuClose_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = btnPrint.ContextMenu;
            menu.StaysOpen = false;
            menu.IsOpen = false;
        }

        #endregion // 프린트 버튼

        #region 엑셀 프린트 메서드

        // 실제 엑셀작업 스타트.
        private void PrintWork(bool excelFlag, bool previewYN, Win_App_ApprovalReq_U_CodeView App)
        {
            try
            {
                excelapp = new Microsoft.Office.Interop.Excel.Application();

                string MyBookPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Report\\품의(기안)서 양식2.xls";
                workbook = excelapp.Workbooks.Add(MyBookPath);
                worksheet = workbook.Sheets["Form"];

                // 제목 B2 J3
                workrange = worksheet.get_Range("B2", "J3");
                workrange.Value2 = App.Title;
                //workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                workrange.Font.Size = 15;

                // 작성일 L2
                workrange = worksheet.get_Range("L2");
                workrange.Value2 = App.ReqDate_CV;
                workrange.Font.Size = 12;

                // 작성자 L3
                workrange = worksheet.get_Range("L3");
                workrange.Value2 = App.Requester;
                workrange.Font.Size = 12;

                // 내용 B9
                workrange = worksheet.get_Range("B9");
                workrange.Value2 = App.Content;
                workrange.Font.Size = 13;

                // 단계!! Q13(담당은 그대로), R13, S13, T13, U13, V13
                FillGridStep(App.AppReqID);
                FillGridStemp(App.AppReqID);

                string[] strColName = {"Q", "R", "S", "T", "U", "V" };
                string ForderName = "";

                for (int i = 0; i < lstStep.Count; i++)
                {
                    workrange = worksheet.get_Range(strColName[i+1] + "13");
                    workrange.Value2 = ResablyFormat(lstStep[i].Resably);
                    workrange.Font.Size = 9;
                }

                // 여기에 +로 border 추가
                workrange = worksheet.Range[strColName[1] + "13", strColName[lstStep.Count] + "14"];
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

                try
                {
                    string str_path = FTP_ADDRESS + '/' + "Stemp";
                    _ftp = new FTP_EX(str_path, FTP_ID, FTP_PASS);

                    for (int k = 0; k < lstStemp.Count; k++)
                    {
                        string str_remotepath = lstStemp[k].StempFileName;
                        string str_localpath = LOCAL_DOWN_PATH + "\\" + lstStemp[k].StempFileName;

                        DirectoryInfo DI = new DirectoryInfo(LOCAL_DOWN_PATH);      // Temp 폴더가 없는 컴터라면, 만들어 줘야지.
                        if (DI.Exists == false)
                        {
                            DI.Create();
                        }

                        FileInfo file = new FileInfo(str_localpath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }

                        if (_ftp.StempDownload(lstStemp[k].PersonID + '/' + str_remotepath, str_localpath, k == 0 ? App.Requester : lstStep[k - 1].Name))
                        {
                            //workrange.CopyPicture()
                            workrange = worksheet.get_Range(strColName[k] + 14);
                            worksheet.Shapes.AddPicture("C:\\Temp\\" + lstStemp[k].StempFileName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 704 + (40 * k), 384, 30, 30);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                catch (Exception ep1)
                {

                }

                // 도장이미지 + 1
                //_ftp = new FTP_EX(FTP_ADDRESS, FTP_ID, FTP_PASS);
                //BitmapImage bit = new BitmapImage();
                //bit = _ftp.DrawingImageByByte(FTP_ADDRESS + '/' + "Stemp" + '/' + lstStemp[0].StempFileName + "");   //주소값 / 폴더명 / 이미지 이름(경로) 

                //string _stLOGO = "C:\\Temp\\" + lstStemp[0].StempFileName;
                //System.Drawing.Image oImage = System.Drawing.Image.FromFile(_stLOGO);
                //System.Drawing.Image img = oImage.GetThumbnailImage(50, 50, null, IntPtr.Zero);
                //System.Windows.Forms.Clipboard.SetDataObject(img, true);
                //worksheet.Paste(workrange, _stLOGO);

                //workrange.Font.Size = 9;

                #region 필요 없는거

                ///////////////////////////////////
                //int Page = 0;
                //int DataCount = 0;
                //int copyLine = 0;

                //copysheet = workbook.Sheets["Form"];
                //pastesheet = workbook.Sheets["Print"];

                //string str_reqid = string.Empty;
                //string str_articleid = string.Empty;
                //string str_reqqty = string.Empty;
                //string str_OutWareReqType = string.Empty;
                //string str_kcustom = string.Empty;

                //while (DT.Rows.Count - 1 > DataCount)
                //{
                //    Page++;
                //    if (Page != 1) { DataCount++; }           // +1. 
                //    copyLine = (Page - 1) * 29;
                //    copysheet.Select();
                //    copysheet.UsedRange.Copy();
                //    pastesheet.Select();
                //    workrange = pastesheet.Cells[copyLine + 1, 1];
                //    workrange.Select();
                //    pastesheet.Paste();                 // 프린트 열에 page번째 항목 복사완료.


                //    int j = 0;
                //    for (int i = DataCount; i < DT.Rows.Count; i++)
                //    {
                //        if (j == 19) { break; }
                //        int insertline = copyLine + 9 + j;

                //        str_reqid = DT.Rows[i]["ReqID"].ToString();
                //        str_articleid = DT.Rows[i]["ArticleID"].ToString();
                //        str_reqqty = DT.Rows[i]["ReqQty"].ToString();
                //        str_OutWareReqType = DT.Rows[i]["OutWareReqTypeID"].ToString();

                //        ObservableCollection<CodeView> cbdgdOutWareReqTypeID = ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "OutReqType", "Y", "", "");
                //        int lol = cbdgdOutWareReqTypeID.Count;
                //        for (int v = 0; v < lol; v++)
                //        {
                //            if (cbdgdOutWareReqTypeID[v].code_id.ToString() == str_OutWareReqType)
                //            {
                //                str_OutWareReqType = cbdgdOutWareReqTypeID[v].code_name.ToString();
                //            }
                //        }

                //        str_kcustom = DT.Rows[i]["KCustom"].ToString();

                //        workrange = pastesheet.get_Range("A" + insertline, "B" + insertline);    //넘버
                //        workrange.Value2 = (i + 1).ToString();
                //        workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                //        workrange.Font.Size = 11;

                //        workrange = pastesheet.get_Range("C" + insertline, "H" + insertline);    //사번(여기선 출고요청id)
                //        workrange.Value2 = str_reqid;
                //        workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                //        workrange.Font.Size = 11;

                //        workrange = pastesheet.get_Range("I" + insertline, "N" + insertline);    //품번
                //        workrange.Value2 = str_articleid;
                //        workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                //        workrange.Font.Size = 11;

                //        workrange = pastesheet.get_Range("O" + insertline, "S" + insertline);    //수주량
                //        workrange.Value2 = str_reqqty;
                //        workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                //        workrange.Font.Size = 11;

                //        workrange = pastesheet.get_Range("T" + insertline, "W" + insertline);    //납품유형
                //        workrange.Value2 = str_OutWareReqType;
                //        workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                //        workrange.Font.Size = 11;

                //        workrange = pastesheet.get_Range("AB" + insertline, "AI" + insertline);    //비고
                //        workrange.Value2 = str_kcustom;
                //        workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                //        workrange.Font.Size = 11;


                //        DataCount = i;
                //        j++;
                //    }
                //}

                //pastesheet.PageSetup.Zoom = 110;

                #endregion // 필요 없는거

                excelapp.Visible = true;
                msg.Hide();

                if (excelFlag == true)
                {
                    if (previewYN == true)
                    {
                        worksheet.PrintPreview();
                    }
                    else
                    {
                        worksheet.PrintOutEx();
                    }
                }

            }
            catch (Exception ex7)
            {
                MessageBox.Show(ex7.Message);
            }
            finally
            {
                Marshal.ReleaseComObject(worksheet);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(excelapp);
            }

        }


        // 두글자면 중간에 띄어쓰기 두번
        private string ResablyFormat(string str)
        {
            if (!str.Trim().Equals(""))
            {
                if (str.Trim().Length == 2)
                {
                    string F = str.Trim().Substring(0, 1);
                    string S = str.Trim().Substring(1, 1);

                    str = F + "  " + S;
                }
            }

            return str;
        }

        // 비트맵을 비트맵 이미지로 형태변환시키기.<0823 허윤구> 
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        #endregion // 엑셀 프린트 메서드

        #region 엑셀용 - 담당자들 + 승인된것들 도장 이미지

        // 조회 1
        private void FillGridStep(string strID)
        {
            lstStep.Clear();

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
        }

        // 조회 2
        private void FillGridStemp(string strID)
        {
            lstStemp.Clear();

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
        }


        #endregion // 엑셀용 - 담당자들 + 승인된것들 도장 이미지


        #endregion // Header 부분 - 오른쪽 상단 버튼 이벤트

        #region Content 입력부분 - 왼쪽

        // 요청자 사원 엔터 → 플러스파인더
        private void txtRequester_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtRequester, (int)Defind_CodeFind.DCF_PERSON, "");
            }
        }
        // 요청자 사원 플러스파인더
        private void btnPfRequester_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtRequester, (int)Defind_CodeFind.DCF_PERSON, "");
        }

        #endregion // Content 입력부분 - 왼쪽

        #region Content 입력부분 - 오른쪽 (첨부파일 / 매일결재)

        // 일반결재 라디오 버튼 체크
        private void rbnCommonApp_Checked(object sender, RoutedEventArgs e)
        {
            if (gbxFile != null 
                && gbxPurchase != null
                && gbxCash != null)
            {
                gbxFile.Visibility = Visibility.Visible;
                gbxPurchase.Visibility = Visibility.Hidden;
                gbxCash.Visibility = Visibility.Hidden;
            }
        }
        private void rbnCommonApp_Unchecked(object sender, RoutedEventArgs e)
        {
            //if (gbxFile != null
            //     && gbxPurchase != null
            //     && gbxCash != null)
            //{
            //    gbxFile.Visibility = Visibility.Visible;
            //    gbxPurchase.Visibility = Visibility.Hidden;
            //    gbxCash.Visibility = Visibility.Hidden;
            //}
        }

        // 매입결재 라디오 버튼 체크
        private void rbnPurchaseApp_Checked(object sender, RoutedEventArgs e)
        {
            if (gbxFile != null
               && gbxPurchase != null
               && gbxCash != null)
            {
                gbxFile.Visibility = Visibility.Hidden;
                gbxPurchase.Visibility = Visibility.Visible;
                gbxCash.Visibility = Visibility.Hidden;
            }
        }                              
        private void rbnPurchaseApp_Unchecked(object sender, RoutedEventArgs e)
        {
            //if (gbxFile != null
            //   && gbxPurchase != null)
            //   && gbxPurchase != null)
            //{
            //    gbxFile.Visibility = Visibility.Visible;
            //    gbxPurchase.Visibility = Visibility.Hidden;
            //}          
        }

        // 입출금 라디오 버튼 체크
        private void rbnCashApp_Checked(object sender, RoutedEventArgs e)
        {
            if (gbxFile != null
               && gbxPurchase != null
               && gbxCash != null)
            {
                gbxFile.Visibility = Visibility.Hidden;
                gbxPurchase.Visibility = Visibility.Hidden;
                gbxCash.Visibility = Visibility.Visible;
            }
        }
        private void rbnCashApp_Unchecked(object sender, RoutedEventArgs e)
        {
            //if (gbxFile != null
            //   && gbxPurchase != null)
            //   && gbxPurchase != null)
            //{
            //    gbxFile.Visibility = Visibility.Visible;
            //    gbxPurchase.Visibility = Visibility.Hidden;
            //}
        }

        #endregion // Content 입력부분 - 오른쪽 (첨부파일 / 매일결재)

        #region Content - 메인 그리드

        private void dgdMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var AppReq = dgdMain.SelectedItem as Win_App_ApprovalReq_U_CodeView;

            if (AppReq != null)
            {
                this.DataContext = AppReq;

                // 1 : 일반결재 / 2: 매입/매출 결제
                if (AppReq.AppGBN.Trim().Equals("1"))
                {
                    rbnCommonApp.IsChecked = true;
                }
                else if (AppReq.AppGBN.Trim().Equals("2"))
                {
                    rbnPurchaseApp.IsChecked = true;

                    FillGridBuyList(AppReq.AppReqID);
                }
                else if (AppReq.AppGBN.Trim().Equals("3"))
                {
                    rbnCashApp.IsChecked = true;

                    FillGridReceivePayList(AppReq.AppReqID);
                }
            }
        }

        #endregion // Content - 메인 그리드

        #region 주요 메서드

        private void re_Search(int selectedIndex)
        {
            FillGrid();

            //rowNum = 0;
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.SelectedIndex = strFlag.Trim().Equals("I") ? dgdMain.Items.Count - 1 : rowNum;
            }
            else
            {
                txtAppReqID.Text = "";
                txtTitle.Text = "";
                txtContent.Text = "";
                txtRemark.Text = "";
                dtpReqDate.SelectedDate = null;
                txtRequester.Text = "";
                cboAppStep.SelectedIndex = -1;
                cboHandle.SelectedIndex = -1;

                txtFileName1.Text = "";
                txtFileName2.Text = "";
                txtFileName3.Text = "";

                MessageBox.Show("조회된 데이터가 없습니다.");
                return;

                
            }
        }

        #region 조회

        // 조회
        private void FillGrid()
        {
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("nDate", chkDateSrh.IsChecked == true ? 1 : 0);
                sqlParameter.Add("StartDate", chkDateSrh.IsChecked == true && dtpSDateSrh.SelectedDate != null ? dtpSDateSrh.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("EndDate", chkDateSrh.IsChecked == true && dtpEDateSrh.SelectedDate != null ? dtpEDateSrh.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("nTitle", chkTitleSrh.IsChecked == true ? 1 : 0);
                sqlParameter.Add("Title", txtTitleSrh.Text);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sApprovalReq", sqlParameter, false);

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
                            var AppReq = new Win_App_ApprovalReq_U_CodeView()
                            {
                                Num = i,

                                AppReqID = dr["AppReqID"].ToString(),
                                AppGBN = dr["AppGBN"].ToString(),
                                AppGBN_Name = dr["AppGBN_Name"].ToString(),
                                ReqDate = dr["ReqDate"].ToString(),
                                ReqDate_CV = DatePickerFormat(dr["ReqDate"].ToString()),

                                RequesterID = dr["RequesterID"].ToString(),
                                Requester = dr["Requester"].ToString(),
                                AppStepID = dr["AppStepID"].ToString(),
                                AppStepName = dr["AppStepName"].ToString(),
                                Title = dr["Title"].ToString(),

                                Remark = dr["Remark"].ToString(),
                                Content = dr["Content"].ToString(),
                                HandleID = dr["HandleID"].ToString(),
                                HandleName = dr["HandleName"].ToString(),
                                UseClss = dr["UseClss"].ToString(),

                                ForderName = dr["ForderName"].ToString(),

                                FileName1 = dr["FileName1"].ToString(),
                                FileName2 = dr["FileName2"].ToString(),
                                FileName3 = dr["FileName3"].ToString(),
                                FileName4 = dr["FileName4"].ToString(),
                                FileName5 = dr["FileName5"].ToString(),

                                Status = dr["Status"].ToString()
                            };

                            AppReq.Content_OneLine = Regex.Split(AppReq.Content, "[\r\n]+")[0];

                            dgdMain.Items.Add(AppReq);
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


        #endregion // 조회

        #region 매입/매출 조회 : FillGridBuyList()

        // 매입/매출 
        private void FillGridBuyList(string AppReqID)
        {
            lstChkIn.Clear();
            lstChkOut.Clear();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("AppReqID", AppReqID);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sApprovalBuyList", sqlParameter, false);

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
                            if (dr["BSGBN"].ToString().Equals("1")) // 매입
                            {
                                lstChkIn.Add(dr["BSNo"].ToString());
                            }
                            else if (dr["BSGBN"].ToString().Equals("2")) // 매출
                            {
                                lstChkOut.Add(dr["BSNo"].ToString());
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


        #endregion // 조회

        #region 매입/매출 조회 : FillGridReceivePayList()

        // 매입/매출 
        private void FillGridReceivePayList(string AppReqID)
        {
            lstChk_CashIn.Clear();
            lstChk_CashOut.Clear();

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("AppReqID", AppReqID);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sApprovalReceivePayList", sqlParameter, false);

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
                            if (dr["RPGBN"].ToString().Equals("1")) // 입금
                            {
                                lstChk_CashIn.Add(dr["RPNo"].ToString());
                            }
                            else if (dr["RPGBN"].ToString().Equals("2")) // 출금
                            {
                                lstChk_CashOut.Add(dr["RPNo"].ToString());
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


        #endregion // 조회

        #region 저장

        private bool SaveData(string strFlag)
        {
            bool flag = false;

            string GetKey = "";

            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                if (CheckData())
                {
                    sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();

                    sqlParameter.Add("JobFlag", strFlag.Trim());
                    sqlParameter.Add("AppReqID", strFlag.Trim().Equals("U") ? txtAppReqID.Text : "");
                    sqlParameter.Add("AppGBN", rbnCommonApp.IsChecked == true ? "1" : (rbnPurchaseApp.IsChecked == true ? "2" : "3"));
                    sqlParameter.Add("ReqDate", dtpReqDate.SelectedDate.Value.ToString("yyyyMMdd"));
                    sqlParameter.Add("RequesterID", txtRequester.Tag != null ? txtRequester.Tag.ToString() : "");

                    sqlParameter.Add("AppStepID", cboAppStep.SelectedValue != null ? cboAppStep.SelectedValue.ToString() : "");
                    sqlParameter.Add("Title", txtTitle.Text);
                    sqlParameter.Add("Remark", txtRemark.Text);
                    sqlParameter.Add("Content", txtContent.Text);
                    sqlParameter.Add("HandleID", cboHandle.SelectedValue != null ? cboHandle.SelectedValue.ToString() : "");

                    sqlParameter.Add("ForderName", "Approval"); // 폴더이름 고정

                    sqlParameter.Add("UserID", MainWindow.CurrentUser); // 폴더이름 고정

                    Procedure pro1 = new Procedure();
                    pro1.list_OutputName = new List<string>();
                    pro1.list_OutputLength = new List<string>();

                    pro1.Name = "xp_Approval_iuApprovalReq";
                    pro1.OutputUseYN = strFlag.Trim().Equals("I") ? "Y" : "N";
                    pro1.list_OutputName.Add("AppReqID");
                    pro1.list_OutputLength.Add("12");

                    Prolist.Add(pro1);
                    ListParameter.Add(sqlParameter);

                    List<KeyValue> list_Result = new List<KeyValue>();
                    list_Result = DataStore.Instance.ExecuteAllProcedureOutputListGetCS(Prolist, ListParameter);

                    if (list_Result[0].key.ToLower() == "success")
                    {
                        if (strFlag.Trim().Equals("I"))
                        {
                            list_Result.RemoveAt(0);
                            for (int i = 0; i < list_Result.Count; i++)
                            {
                                KeyValue kv = list_Result[i];
                                if (kv.key == "AppReqID")
                                {
                                    string sGetID = kv.value;
                                    //MessageBox.Show(sGetID);
                                    GetKey = sGetID;
                                    
                                }
                            }
                        }
                        else
                        {
                            GetKey = txtAppReqID.Text;
                        }

                        flag = true;
                    }
                    else
                    {
                        MessageBox.Show("[저장실패]\r\n" + list_Result[0].value.ToString());
                        flag = false;
                    }

                    // FTP 파일 업로드 AttachFileUpdate
                    // 파일을 올리자 : GetKey != "" 라면 파일을 올려보자
                    if (!GetKey.Trim().Equals(""))
                    {
                        if (deleteListFtpFile.Count > 0)
                        {
                            foreach (string[] str in deleteListFtpFile)
                            {
                                FTP_RemoveFile(GetKey + "/" + str[0]);
                            }
                        }

                        if (listFtpFile.Count > 0)
                        {
                            FTP_Save_File(listFtpFile, GetKey);
                        }
                        AttachFileUpdate(GetKey);
                    }

                    // 파일 List 비워주기
                    listFtpFile.Clear();
                    deleteListFtpFile.Clear();

                    #region 매입 리스트 저장하기
                    if (rbnPurchaseApp.IsChecked == true
                        && (lstAppIn.Count > 0
                        || lstAppOut.Count > 0))
                    {
                        Prolist.Clear();
                        ListParameter.Clear();

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();

                        sqlParameter.Add("AppReqID", strFlag.Trim().Equals("I") ? GetKey : txtAppReqID.Text);

                        Procedure pro2 = new Procedure();
                        pro2.list_OutputName = new List<string>();
                        pro2.list_OutputLength = new List<string>();

                        pro2.Name = "xp_Approval_dAppBuyListAll"; // xp_Approval_dAppBuyListAll
                        pro2.OutputUseYN = "N";
                        pro2.list_OutputName.Add("AppReqID");
                        pro2.list_OutputLength.Add("12");

                        Prolist.Add(pro2);
                        ListParameter.Add(sqlParameter);

                        for (int i = 0; i < lstAppIn.Count; i++)
                        {
                            var AppIn = lstAppIn[i] as App_IN_CodeView2;

                            if (AppIn != null && AppIn.Chk == true)
                            {
                                sqlParameter = new Dictionary<string, object>();
                                sqlParameter.Clear();

                                sqlParameter.Add("AppReqID", strFlag.Trim().Equals("I") ? GetKey : txtAppReqID.Text);
                                sqlParameter.Add("Seq", i + 1);
                                sqlParameter.Add("BSNo", AppIn.BSNo.Trim());
                                sqlParameter.Add("BSGBN", AppIn.BSGBN.Trim());

                                Procedure pro3 = new Procedure();
                                pro3.list_OutputName = new List<string>();
                                pro3.list_OutputLength = new List<string>();

                                pro3.Name = "xp_Approval_iAppBuyList";
                                pro3.OutputUseYN = "N";
                                pro3.list_OutputName.Add("AppReqID");
                                pro3.list_OutputLength.Add("12");

                                Prolist.Add(pro3);
                                ListParameter.Add(sqlParameter);

                            }
                        }

                        for (int i = 0; i < lstAppOut.Count; i++)
                        {
                            var AppIn = lstAppOut[i] as App_IN_CodeView2;

                            if (AppIn != null && AppIn.Chk == true)
                            {
                                sqlParameter = new Dictionary<string, object>();
                                sqlParameter.Clear();

                                sqlParameter.Add("AppReqID", strFlag.Trim().Equals("I") ? GetKey : txtAppReqID.Text);
                                sqlParameter.Add("Seq", i + 1);
                                sqlParameter.Add("BSNo", AppIn.BSNo.Trim());
                                sqlParameter.Add("BSGBN", AppIn.BSGBN.Trim());

                                Procedure pro3 = new Procedure();
                                pro3.list_OutputName = new List<string>();
                                pro3.list_OutputLength = new List<string>();

                                pro3.Name = "xp_Approval_iAppBuyList";
                                pro3.OutputUseYN = "N";
                                pro3.list_OutputName.Add("AppReqID");
                                pro3.list_OutputLength.Add("12");

                                Prolist.Add(pro3);
                                ListParameter.Add(sqlParameter);

                            }
                        }

                        List<KeyValue> list_Result2 = new List<KeyValue>();
                        list_Result2 = DataStore.Instance.ExecuteAllProcedureOutputListGetCS(Prolist, ListParameter);

                        if (list_Result2[0].key.ToLower() == "success")
                        {

                            flag = true;
                        }
                        else
                        {
                            MessageBox.Show("[저장실패]\r\n" + list_Result[0].value.ToString());
                            flag = false;
                        }
                    }

                    #endregion // 매입리스트 저장하기

                    #region 입출금 리스트 저장하기

                    if (rbnCashApp.IsChecked == true
                       && (lstApp_CashIn.Count > 0
                       || lstApp_CashOut.Count > 0))
                    {
                        Prolist.Clear();
                        ListParameter.Clear();

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();

                        sqlParameter.Add("AppReqID", strFlag.Trim().Equals("I") ? GetKey : txtAppReqID.Text);

                        Procedure pro2 = new Procedure();
                        pro2.list_OutputName = new List<string>();
                        pro2.list_OutputLength = new List<string>();

                        pro2.Name = "xp_Approval_dAppReceivePayListAll"; // xp_Approval_dAppBuyListAll
                        pro2.OutputUseYN = "N";
                        pro2.list_OutputName.Add("AppReqID");
                        pro2.list_OutputLength.Add("12");

                        Prolist.Add(pro2);
                        ListParameter.Add(sqlParameter);

                        for (int i = 0; i < lstApp_CashIn.Count; i++)
                        {
                            var AppIn = lstApp_CashIn[i] as App_CashIn_CodeView2;

                            if (AppIn != null && AppIn.Chk == true)
                            {
                                sqlParameter = new Dictionary<string, object>();
                                sqlParameter.Clear();

                                sqlParameter.Add("AppReqID", strFlag.Trim().Equals("I") ? GetKey : txtAppReqID.Text);
                                sqlParameter.Add("Seq", i + 1);
                                sqlParameter.Add("RPNo", AppIn.RPNo.Trim());
                                sqlParameter.Add("RPGBN", AppIn.RPGBN.Trim());

                                Procedure pro3 = new Procedure();
                                pro3.list_OutputName = new List<string>();
                                pro3.list_OutputLength = new List<string>();

                                pro3.Name = "xp_Approval_iAppReceivePayList";
                                pro3.OutputUseYN = "N";
                                pro3.list_OutputName.Add("AppReqID");
                                pro3.list_OutputLength.Add("12");

                                Prolist.Add(pro3);
                                ListParameter.Add(sqlParameter);

                            }
                        }

                        for (int i = 0; i < lstApp_CashOut.Count; i++)
                        {
                            var AppIn = lstApp_CashOut[i] as App_CashIn_CodeView2;

                            if (AppIn != null && AppIn.Chk == true)
                            {
                                sqlParameter = new Dictionary<string, object>();
                                sqlParameter.Clear();

                                sqlParameter.Add("AppReqID", strFlag.Trim().Equals("I") ? GetKey : txtAppReqID.Text);
                                sqlParameter.Add("Seq", i + 1);
                                sqlParameter.Add("RPNo", AppIn.RPNo.Trim());
                                sqlParameter.Add("RPGBN", AppIn.RPGBN.Trim());

                                Procedure pro3 = new Procedure();
                                pro3.list_OutputName = new List<string>();
                                pro3.list_OutputLength = new List<string>();

                                pro3.Name = "xp_Approval_iAppReceivePayList";
                                pro3.OutputUseYN = "N";
                                pro3.list_OutputName.Add("AppReqID");
                                pro3.list_OutputLength.Add("12");

                                Prolist.Add(pro3);
                                ListParameter.Add(sqlParameter);

                            }
                        }

                        List<KeyValue> list_Result2 = new List<KeyValue>();
                        list_Result2 = DataStore.Instance.ExecuteAllProcedureOutputListGetCS(Prolist, ListParameter);

                        if (list_Result2[0].key.ToLower() == "success")
                        {

                            flag = true;
                        }
                        else
                        {
                            MessageBox.Show("[저장실패]\r\n" + list_Result[0].value.ToString());
                            flag = false;
                        }
                    }

                    #endregion
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


            return flag;
        }

        // 1) 첨부문서가 있을경우, 2) FTP에 정상적으로 업로드가 완료된 경우.  >> DB에 정보 업데이트 
        private void AttachFileUpdate(string AppReqID)
        {
            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Add("FileName1", rbnCommonApp.IsChecked == true ? txtFileName1.Text : (rbnPurchaseApp.IsChecked == true ? txtFileName_M1.Text : txtFileName_C1.Text));
            sqlParameter.Add("FileName2", rbnCommonApp.IsChecked == true ? txtFileName2.Text : (rbnPurchaseApp.IsChecked == true ? txtFileName_M2.Text : txtFileName_C2.Text));
            sqlParameter.Add("FileName3", rbnCommonApp.IsChecked == true ? txtFileName3.Text : (rbnPurchaseApp.IsChecked == true ? txtFileName_M3.Text : txtFileName_C3.Text));
            sqlParameter.Add("FileName4", rbnCommonApp.IsChecked == true ? txtFileName4.Text : (rbnPurchaseApp.IsChecked == true ? txtFileName_M4.Text : txtFileName_C4.Text));
            sqlParameter.Add("FileName5", rbnCommonApp.IsChecked == true ? txtFileName5.Text : (rbnPurchaseApp.IsChecked == true ? txtFileName_M5.Text : txtFileName_C5.Text));
            sqlParameter.Add("AppReqID", AppReqID);

            string[] result = DataStore.Instance.ExecuteProcedure("xp_Approval_uApprovalReq_FTP", sqlParameter, false);
            if (!result[0].Equals("success"))
            {
                MessageBox.Show("이상발생, 관리자에게 문의하세요");
            }
        }


        #endregion // 저장

        #region 유효성 검사

        private bool CheckData()
        {
            bool flag = true;

            if (cboAppStep.SelectedValue == null)
            {
                MessageBox.Show("결재 단계를 선택해주세요.");
                flag = false;
                return flag;
            }

            if (dtpReqDate.SelectedDate == null)
            {
                MessageBox.Show("요청일자를 선택해주세요.");
                flag = false;
                return flag;
            }

            if (txtTitle.Text.Trim().Equals(""))
            {
                MessageBox.Show("제목을 입력해주세요.");
                flag = false;
                return flag;
            }

            if (txtRequester.Tag == null)
            {
                MessageBox.Show("요청자를 입력해주세요.");
                flag = false;
                return flag;
            }

            if (cboHandle.SelectedValue == null)
            {
                MessageBox.Show("처리(신청 / 반환)을 선택해주세요.");
                flag = false;
                return flag;
            }

            // 만약에 매입결재인데 매입리스트가 없으면 매입 리스트 보기를 클릭하여, 결재할 매입 대상을 선택해주세요.
            if (rbnPurchaseApp.IsChecked == true
                && lstAppIn.Count == 0)
            {
                MessageBox.Show("매입 리스트 보기를 클릭하여, 결재할 매입 대상을 선택해주세요.");
                flag = false;
                return flag;
            }

            return flag;
        }

        // Stuffin 객체에 값이 들어있는지 체크
        //private bool chkSaveStuffin(Win_mtr_OcStuffin_U_CodeView Stuffin)
        //{
        //    bool flag = true;

        //    if (Stuffin.CustomID == null || Stuffin.CustomID.Trim().Equals(""))
        //    {
        //        flag = false;
        //        return false;
        //    }

        //    return flag;
        //}

        #endregion // 유효성 검사

        #region 삭제

        // 삭제 메서드
        private bool DeleteData(string strID)
        {
            bool flag = false;

            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Clear();
            sqlParameter.Add("AppReqID", strID);

            try
            {
                // 매입 리스트 삭제 -- 외래키가 걸려 있으니 이게 먼저 삭제가 되야함
                string[] result2 = DataStore.Instance.ExecuteProcedure("xp_Approval_dAppBuyListAll", sqlParameter, true);

                if (!result2[0].Equals("success"))
                {
                    MessageBox.Show("삭제 실패");
                    flag = false;
                }
                else
                {
                    //MessageBox.Show("성공적으로 삭제되었습니다.");
                    flag = true;
                }

                string[] result = DataStore.Instance.ExecuteProcedure("xp_Approval_dApprovalReq", sqlParameter, true);

                if (!result[0].Equals("success"))
                {
                    MessageBox.Show("삭제 실패");
                    flag = false;
                }
                else
                {
                    //MessageBox.Show("성공적으로 삭제되었습니다.");
                    flag = true;
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

            return flag;
        }

        #endregion // 삭제

        #region 매입 리스트 검색

        //// 매출용 그리드 채우기.
        //private void FillGrid_ApprovalGrid()
        //{
        //    if (dgdMoney.Items.Count > 0)
        //    {
        //        dgdMoney.Items.Clear();
        //    }

        //    try
        //    {

        //        // 기간 체크여부 yn.
        //        //int sBSDate = 0;
        //        //if (chkPeriod.IsChecked == true) { sBSDate = 1; }


        //        DataSet ds = null;
        //        Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
        //        sqlParameter.Clear();

        //        sqlParameter.Add("sBSDate", "");//sBSDate);
        //        sqlParameter.Add("sDate", ""); //sBSDate == 1 ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
        //        sqlParameter.Add("eDate", ""); //sBSDate == 1 ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
        //        sqlParameter.Add("CustomID", "");
        //        sqlParameter.Add("BSItemCode", "");
        //        sqlParameter.Add("ArticleID", "");
        //        sqlParameter.Add("Article", "");
        //        sqlParameter.Add("OrderNo", "");

        //        ds = DataStore.Instance.ProcedureToDataSet("xp_Acc_RP_sReceivePay", sqlParameter, false);

        //        if (ds != null && ds.Tables.Count > 0)
        //        {
        //            DataTable dt = ds.Tables[0];
        //            int i = 0;

        //            if (dt.Rows.Count == 0)
        //            {
        //                MessageBox.Show("조회된 데이터가 없습니다.");
        //            }
        //            else
        //            {
        //                DataRowCollection drc = dt.Rows;
        //                foreach (DataRow dr in drc)
        //                {
        //                    var WinAccBuySale = new Win_App_ApprovalReq_U_Money()
        //                    {
        //                        Num = i + 1,
        //                        IsCheck = false,

        //                        RPNo = dr["RPNo"].ToString(),
        //                        RPGBN = dr["RPGBN"].ToString(),
        //                        companyid = dr["companyid"].ToString(),
        //                        RPDate = dr["RPDate"].ToString(),
        //                        BSItem = dr["BSItem"].ToString(),

        //                        RPItemCode = dr["RPItemCode"].ToString(),
        //                        CurrencyUnit = dr["CurrencyUnit"].ToString(),
        //                        CustomID = dr["CustomID"].ToString(),
        //                        SalesCharge = dr["SalesCharge"].ToString(),
        //                        BankID = dr["BankID"].ToString(),

        //                        CashAmount = dr["CashAmount"].ToString(),
        //                        BillAmount = dr["BillAmount"].ToString(),
        //                        BankAmount = dr["BankAmount"].ToString(),
        //                        DCAmount = dr["DCAmount"].ToString(),
        //                        BillNo = dr["BillNo"].ToString(),

        //                        ForReceiveBillAmount = dr["ForReceiveBillAmount"].ToString(),
        //                        ReceiveNowDateYN = dr["ReceiveNowDateYN"].ToString(),
        //                        CardAmount = dr["CardAmount"].ToString(),
        //                        ReceivePersonName = dr["ReceivePersonName"].ToString(),
        //                        Comments = dr["Comments"].ToString(),

        //                        OrderID = dr["OrderID"].ToString(),
        //                        RefBSNO = dr["RefBSNO"].ToString(),
        //                        OrderFlag = dr["OrderFlag"].ToString(),
        //                        RefRPItemCode = dr["RefRPItemCode"].ToString(),
        //                        RefComments = dr["RefComments"].ToString(),

        //                        RefAccountYN = dr["RefAccountYN"].ToString(),
        //                        RefAmount = dr["RefAmount"].ToString(),

        //                    };
        //                    dgdMoney.Items.Add(WinAccBuySale);
        //                    i++;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
        //    }
        //}

        #endregion // 매입 리스트 검색

        #endregion 주요 메서드


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




        #endregion

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            string btnIndex = ((Button)sender).Tag.ToString();

            if (btnIndex.Trim().Equals("1")) { FTP_Upload_TextBox(txtFileName1); }
            else if (btnIndex.Trim().Equals("2")) { FTP_Upload_TextBox(txtFileName2); }
            else if (btnIndex.Trim().Equals("3")) { FTP_Upload_TextBox(txtFileName3); }
            else if (btnIndex.Trim().Equals("4")) { FTP_Upload_TextBox(txtFileName4); }
            else if (btnIndex.Trim().Equals("5")) { FTP_Upload_TextBox(txtFileName5); }
            else if (btnIndex.Trim().Equals("6")) { FTP_Upload_TextBox(txtFileName_M1); }
            else if (btnIndex.Trim().Equals("7")) { FTP_Upload_TextBox(txtFileName_M2); }
            else if (btnIndex.Trim().Equals("8")) { FTP_Upload_TextBox(txtFileName_M3); }
            else if (btnIndex.Trim().Equals("9")) { FTP_Upload_TextBox(txtFileName_M4); }
            else if (btnIndex.Trim().Equals("10")) { FTP_Upload_TextBox(txtFileName_M5); }
            else if (btnIndex.Trim().Equals("11")) { FTP_Upload_TextBox(txtFileName_C1); }
            else if (btnIndex.Trim().Equals("12")) { FTP_Upload_TextBox(txtFileName_C2); }
            else if (btnIndex.Trim().Equals("13")) { FTP_Upload_TextBox(txtFileName_C3); }
            else if (btnIndex.Trim().Equals("14")) { FTP_Upload_TextBox(txtFileName_C4); }
            else if (btnIndex.Trim().Equals("15")) { FTP_Upload_TextBox(txtFileName_C5); }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgresult = MessageBox.Show("파일을 삭제 하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo);
            if (msgresult == MessageBoxResult.Yes)
            {
                string buttonIndex = ((Button)sender).Tag.ToString();

                if (buttonIndex.Trim().Equals("1") && (txtFileName1.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName1); }
                else if (buttonIndex.Trim().Equals("2") && (txtFileName2.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName2); }
                else if (buttonIndex.Trim().Equals("3") && (txtFileName3.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName3); }
                else if (buttonIndex.Trim().Equals("4") && (txtFileName4.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName4); }
                else if (buttonIndex.Trim().Equals("5") && (txtFileName5.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName5); }
                else if (buttonIndex.Trim().Equals("6") && (txtFileName_M1.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_M1); }
                else if (buttonIndex.Trim().Equals("7") && (txtFileName_M2.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_M2); }
                else if (buttonIndex.Trim().Equals("8") && (txtFileName_M3.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_M3); }
                else if (buttonIndex.Trim().Equals("9") && (txtFileName_M4.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_M4); }
                else if (buttonIndex.Trim().Equals("10") && (txtFileName_M5.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_M5); }
                else if (buttonIndex.Trim().Equals("11") && (txtFileName_C1.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_C1); }
                else if (buttonIndex.Trim().Equals("12") && (txtFileName_C2.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_C2); }
                else if (buttonIndex.Trim().Equals("13") && (txtFileName_C3.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_C3); }
                else if (buttonIndex.Trim().Equals("14") && (txtFileName_C4.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_C4); }
                else if (buttonIndex.Trim().Equals("15") && (txtFileName_C5.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_C5); }
            }
        }

        #region 다운로드 버튼

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            // 1 : 모니터링 / 2 : 첨부파일1 / 3 : 첨부파일2
            string buttonIndex = ((Button)sender).Tag.ToString();

            int iniFlag = 0;

            if ((buttonIndex.Trim().Equals("1") && txtFileName1.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("2") && txtFileName2.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("3") && txtFileName3.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("4") && txtFileName4.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("5") && txtFileName5.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("6") && txtFileName_M1.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("7") && txtFileName_M2.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("8") && txtFileName_M3.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("9") && txtFileName_M4.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("10") && txtFileName_M5.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("11") && txtFileName_C1.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("12") && txtFileName_C2.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("13") && txtFileName_C3.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("14") && txtFileName_C4.Text == string.Empty)
                    || (buttonIndex.Trim().Equals("15") && txtFileName_C5.Text == string.Empty))
            {
                //MessageBox.Show("파일이 없습니다.");
                return;
            }
            else
            {
                MessageBoxResult msgresult = MessageBox.Show("파일을 보시겠습니까?", "보기 확인", MessageBoxButton.YesNo);
                if (msgresult == MessageBoxResult.Yes)
                {
                    try
                    {
                        var App = dgdMain.SelectedItem as Win_App_ApprovalReq_U_CodeView;

                        if (App != null)
                        {
                            // 접속 경로
                            _ftp = new FTP_EX(FTP_ADDRESS, FTP_ID, FTP_PASS);

                            string str_path = string.Empty;
                            str_path = FTP_ADDRESS + '/' + App.AppReqID;
                            _ftp = new FTP_EX(str_path, FTP_ID, FTP_PASS);

                            string str_remotepath = string.Empty;
                            string str_localpath = string.Empty;

                            if (buttonIndex.Trim().Equals("1")) { str_remotepath = txtFileName1.Text; }
                            else if (buttonIndex.Trim().Equals("2")) { str_remotepath = txtFileName2.Text; }
                            else if (buttonIndex.Trim().Equals("3")) { str_remotepath = txtFileName3.Text; }
                            else if (buttonIndex.Trim().Equals("4")) { str_remotepath = txtFileName4.Text; }
                            else if (buttonIndex.Trim().Equals("5")) { str_remotepath = txtFileName5.Text; }
                            else if (buttonIndex.Trim().Equals("6")) { str_remotepath = txtFileName_M1.Text; }
                            else if (buttonIndex.Trim().Equals("7")) { str_remotepath = txtFileName_M2.Text; }
                            else if (buttonIndex.Trim().Equals("8")) { str_remotepath = txtFileName_M3.Text; }
                            else if (buttonIndex.Trim().Equals("9")) { str_remotepath = txtFileName_M4.Text; }
                            else if (buttonIndex.Trim().Equals("10")) { str_remotepath = txtFileName_M5.Text; }
                            else if (buttonIndex.Trim().Equals("11")) { str_remotepath = txtFileName_C1.Text; }
                            else if (buttonIndex.Trim().Equals("12")) { str_remotepath = txtFileName_C2.Text; }
                            else if (buttonIndex.Trim().Equals("13")) { str_remotepath = txtFileName_C3.Text; }
                            else if (buttonIndex.Trim().Equals("14")) { str_remotepath = txtFileName_C4.Text; }
                            else if (buttonIndex.Trim().Equals("15")) { str_remotepath = txtFileName_C5.Text; }

                            if (buttonIndex.Trim().Equals("1")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName1.Text; }
                            else if (buttonIndex.Trim().Equals("2")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName2.Text; }
                            else if (buttonIndex.Trim().Equals("3")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName3.Text; }
                            else if (buttonIndex.Trim().Equals("4")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName4.Text; }
                            else if (buttonIndex.Trim().Equals("5")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName5.Text; }
                            else if (buttonIndex.Trim().Equals("6")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName_M1.Text; }
                            else if (buttonIndex.Trim().Equals("7")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName_M2.Text; }
                            else if (buttonIndex.Trim().Equals("8")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName_M3.Text; }
                            else if (buttonIndex.Trim().Equals("9")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName_M4.Text; }
                            else if (buttonIndex.Trim().Equals("10")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName_M5.Text; }
                            else if (buttonIndex.Trim().Equals("11")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName_C1.Text; }
                            else if (buttonIndex.Trim().Equals("12")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName_C2.Text; }
                            else if (buttonIndex.Trim().Equals("13")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName_C3.Text; }
                            else if (buttonIndex.Trim().Equals("14")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName_C4.Text; }
                            else if (buttonIndex.Trim().Equals("15")) { str_localpath = LOCAL_DOWN_PATH + "\\" + txtFileName_C5.Text; }

                            DirectoryInfo DI = new DirectoryInfo(LOCAL_DOWN_PATH);      // Temp 폴더가 없는 컴터라면, 만들어 줘야지.
                            if (DI.Exists == false)
                            {
                                DI.Create();
                            }

                            FileInfo file = new FileInfo(str_localpath);
                            if (file.Exists)
                            {
                                file.Delete();
                            }

                            _ftp.download(str_remotepath, str_localpath);

                            iniFlag = 1;

                            ProcessStartInfo proc = new ProcessStartInfo(str_localpath);
                            proc.UseShellExecute = true;
                            Process.Start(proc);
                        }


                    }
                    catch (Exception ex) // 뭐든 간에 파일 없다고 하자
                    {

                        if (iniFlag == 0)
                        {
                            //MessageBox.Show("파일이 존재하지 않습니다.\r관리자에게 문의해주세요.");
                        }
                        else
                        {
                            MessageBox.Show("실행하는 형식의 응용 프로그램이 설정되지 않았습니다.");
                        }

                        return;
                    }
                }
            }
        }

        #endregion // 다운로드 버튼

        #region FTP 파일 삭제

        //파일만 삭제 - 버튼에 Tag로 구분
        private void btn_DelAttach_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgresult = MessageBox.Show("파일을 삭제 하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo);
            if (msgresult == MessageBoxResult.Yes)
            {
                string buttonIndex = ((Button)sender).Tag.ToString();

                if (buttonIndex.Trim().Equals("1") && (txtFileName1.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName1); }
                else if (buttonIndex.Trim().Equals("2") && (txtFileName2.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName2); }
                else if (buttonIndex.Trim().Equals("3") && (txtFileName3.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName3); }
                else if (buttonIndex.Trim().Equals("4") && (txtFileName4.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName4); }
                else if (buttonIndex.Trim().Equals("5") && (txtFileName5.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName5); }
                else if (buttonIndex.Trim().Equals("6") && (txtFileName_M1.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_M1); }
                else if (buttonIndex.Trim().Equals("7") && (txtFileName_M2.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_M2); }
                else if (buttonIndex.Trim().Equals("8") && (txtFileName_M3.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_M3); }
                else if (buttonIndex.Trim().Equals("9") && (txtFileName_M4.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_M4); }
                else if (buttonIndex.Trim().Equals("10") && (txtFileName_M5.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_M5); }
                else if (buttonIndex.Trim().Equals("11") && (txtFileName_C1.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_C1); }
                else if (buttonIndex.Trim().Equals("12") && (txtFileName_C2.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_C2); }
                else if (buttonIndex.Trim().Equals("13") && (txtFileName_C3.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_C3); }
                else if (buttonIndex.Trim().Equals("14") && (txtFileName_C4.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_C4); }
                else if (buttonIndex.Trim().Equals("15") && (txtFileName_C5.Text != string.Empty)) { FileDeleteAndTextBoxEmpty(txtFileName_C5); }
            }

            // 보기 버튼체크
            //btnImgSeeCheckAndSetting();
        }
        private void FileDeleteAndTextBoxEmpty(TextBox txt)
        {
            if (strFlag.Equals("U"))
            {
                var Info = dgdMain.SelectedItem as Win_App_ApprovalReq_U_CodeView;

                if (Info != null)
                {
                    //FTP_RemoveFile(Article.ArticleID + "/" + txt.Text);

                    // 파일이름, 파일경로
                    string[] strFtp = { txt.Text, txt.Tag != null ? txt.Tag.ToString() : "" };

                    deleteListFtpFile.Add(strFtp);
                }
            }

            txt.Text = "";
            txt.Tag = "";
        }
        //파일 삭제
        private bool FTP_RemoveFile(string strSaveName)
        {
            _ftp = new FTP_EX(FTP_ADDRESS, FTP_ID, FTP_PASS);
            if (_ftp.delete(strSaveName) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //폴더 삭제(내부 파일 자동 삭제)
        private bool FTP_RemoveDir(string strSaveName)
        {
            _ftp = new FTP_EX(FTP_ADDRESS, FTP_ID, FTP_PASS);
            if (_ftp.removeDir(strSaveName) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion // FTP 파일 삭제

        #region FTP_Upload_TextBox - 파일 경로, 이름 텍스트박스에 올림 + 리스트에 ADD

        private void FTP_Upload_TextBox(TextBox textBox)
        {
            if (!textBox.Text.Equals(string.Empty) && strFlag.Equals("U"))
            {
                MessageBox.Show("먼저 해당파일의 삭제를 진행 후 진행해주세요.");
                return;
            }
            else
            {
                Microsoft.Win32.OpenFileDialog OFdlg = new Microsoft.Win32.OpenFileDialog();
                OFdlg.Filter =
                    "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.pcx, *.pdf) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.pcx; *.pdf | All Files|*.*";

                Nullable<bool> result = OFdlg.ShowDialog();
                if (result == true)
                {
                    strFullPath = OFdlg.FileName;

                    string ImageFileName = OFdlg.SafeFileName;  //명.
                    string ImageFilePath = string.Empty;       // 경로

                    ImageFilePath = strFullPath.Replace(ImageFileName, "");

                    StreamReader sr = new StreamReader(OFdlg.FileName);
                    long FileSize = sr.BaseStream.Length;
                    if (sr.BaseStream.Length > (2048 * 1000))
                    {
                        //업로드 파일 사이즈범위 초과
                        MessageBox.Show("이미지의 파일사이즈가 2M byte를 초과하였습니다.");
                        sr.Close();
                        return;
                    }
                    else
                    {
                        textBox.Text = ImageFileName;
                        textBox.Tag = ImageFilePath;

                        string[] strTemp = new string[] { ImageFileName, ImageFilePath.ToString() };
                        listFtpFile.Add(strTemp);
                    }
                }
            }
        }

        #endregion // FTP_Upload_TextBox - 파일 경로, 이름 텍스트박스에 올림 + 리스트에 ADD

        #region FTP_Save_File - 파일 저장, 폴더 생성

        /// <summary>
        /// 해당영역에 파일 있는지 확인
        /// </summary>
        bool FileInfoAndFlag(string[] strFileList, string FileName)
        {
            bool flag = false;
            foreach (string FileList in strFileList)
            {
                if (FileList == FileName)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        /// <summary>
        /// 해당영역에 폴더가 있는지 확인
        /// </summary>
        bool FolderInfoAndFlag(string[] strFolderList, string FolderName)
        {
            bool flag = false;
            foreach (string FolderList in strFolderList)
            {
                if (FolderList == FolderName)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        // 파일 저장하기.
        private void FTP_Save_File(List<string[]> listStrArrayFileInfo, string MakeFolderName)
        {
            _ftp = new FTP_EX(FTP_ADDRESS, FTP_ID, FTP_PASS);

            List<string[]> UpdateFilesInfo = new List<string[]>();
            string[] fileListSimple;
            string[] fileListDetail = null;
            fileListSimple = _ftp.directoryListSimple("", Encoding.Default);

            // 기존 폴더 확인작업.
            bool MakeFolder = false;
            MakeFolder = FolderInfoAndFlag(fileListSimple, MakeFolderName);

            if (MakeFolder == false)        // 같은 아이를 찾지 못한경우,
            {
                //MIL 폴더에 InspectionID로 저장
                if (_ftp.createDirectory(MakeFolderName) == false)
                {
                    MessageBox.Show("업로드를 위한 폴더를 생성할 수 없습니다.");
                    return;
                }
            }
            else
            {
                fileListDetail = _ftp.directoryListSimple(MakeFolderName, Encoding.Default);
            }

            for (int i = 0; i < listStrArrayFileInfo.Count; i++)
            {
                bool flag = true;

                if (fileListDetail != null)
                {
                    foreach (string compare in fileListDetail)
                    {
                        if (compare.Equals(listStrArrayFileInfo[i][0]))
                        {
                            flag = false;
                            break;
                        }
                    }
                }

                if (flag)
                {
                    listStrArrayFileInfo[i][0] = MakeFolderName + "/" + listStrArrayFileInfo[i][0];
                    UpdateFilesInfo.Add(listStrArrayFileInfo[i]);
                }
            }

            if (!_ftp.UploadTempFilesToFTP(UpdateFilesInfo))
            {
                MessageBox.Show("파일업로드에 실패하였습니다.");
                return;
            }
        }



        #endregion // FTP_Save_File - 파일 저장, 폴더 생성


        //private void btnMoneyClose_Click(object sender, RoutedEventArgs e)
        //{
        //    grdMoney.Visibility = Visibility.Hidden;
        //}

        //private void btnMoney_Click(object sender, RoutedEventArgs e)
        //{
        //    grdMoney.Visibility = Visibility.Visible;
        //}

        #region 매입리스트 보기

        private void btnIncome_Click(object sender, RoutedEventArgs e)
        {
            if (dtpReqDate.SelectedDate != null)
            {
                if (strFlag.Trim().Equals("I") || strFlag.Trim().Equals("U"))
                {
                    App_INOUT2 Income = new App_INOUT2(dtpReqDate.SelectedDate.Value
                                                           , this.lstChkIn, this.lstChkOut);

                    Income.SrhFlag = true;
                    Income.AppReqID = strFlag.Trim().Equals("U") ? txtAppReqID.Text : "";
                    Income.strFlag = strFlag.Trim();
                    Income.AppStepID = cboAppStep.SelectedValue != null ? cboAppStep.SelectedValue.ToString() : "";

                    Income.ShowDialog();

                    if (Income.DialogResult == true)
                    {
                        this.lstAppIn.Clear();
                        this.lstAppOut.Clear();

                        //this.lstChkIn.Clear();
                        //this.lstChkOut.Clear();

                        this.lstAppIn = Income.lstAppIn;
                        this.lstAppOut = Income.lstAppOut;

                        this.lstChkIn = Income.lstChkIn;
                        this.lstChkOut = Income.lstChkOut;
                    }
                }
                else
                {
                    App_INOUT IncomeS = new App_INOUT(dtpReqDate.SelectedDate.Value);

                    IncomeS.SrhFlag = false;
                    IncomeS.AppReqID = txtAppReqID.Text;
                    IncomeS.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("요청 일자를 선택해주세요.");
            }
        }


        #endregion // 매입리스트 보기

        #region 입출금리스트 보기

        private void btnCash_Click(object sender, RoutedEventArgs e)
        {
            if (dtpReqDate.SelectedDate != null)
            {
                if (strFlag.Trim().Equals("I") || strFlag.Trim().Equals("U"))
                {
                    App_CashInOut2 Income = new App_CashInOut2(dtpReqDate.SelectedDate.Value
                                                           , this.lstChk_CashIn, this.lstChk_CashOut);

                    Income.SrhFlag = true;
                    Income.AppReqID = strFlag.Trim().Equals("U") ? txtAppReqID.Text : "";
                    Income.strFlag = strFlag.Trim();
                    Income.ShowDialog();

                    if (Income.DialogResult == true)
                    {
                        this.lstApp_CashIn.Clear();
                        this.lstApp_CashOut.Clear();

                        //this.lstChkIn.Clear();
                        //this.lstChkOut.Clear();

                        this.lstApp_CashIn = Income.lstApp_CashIn;
                        this.lstApp_CashOut = Income.lstApp_CashOut;

                        this.lstChk_CashIn = Income.lstChk_CashIn;
                        this.lstChk_CashOut = Income.lstChk_CashOut;
                    }
                }
                else
                {
                    App_CashInOut IncomeS = new App_CashInOut(dtpReqDate.SelectedDate.Value);

                    IncomeS.SrhFlag = false;
                    IncomeS.AppReqID = txtAppReqID.Text;
                    IncomeS.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("요청 일자를 선택해주세요.");
            }
        }


        #endregion // 매입리스트 보기

        private void btnStemp_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    #region CodeView

    class Win_App_ApprovalReq_U_CodeView
    {
        public int Num { get; set; }

        public string AppReqID { get; set; }
        public string AppGBN { get; set; }
        public string AppGBN_Name { get; set; }
        public string ReqDate { get; set; }
        public string ReqDate_CV { get; set; }
        public string RequesterID { get; set; }

        public string Requester { get; set; }
        public string AppStepID { get; set; }
        public string AppStepName { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }

        public string Content_OneLine { get; set; }

        public string Content { get; set; }
        public string HandleID { get; set; }
        public string HandleName { get; set; }
        public string UseClss { get; set; }
        public string ForderName { get; set; }

        public string FileName1 { get; set; }
        public string FileName2 { get; set; }
        public string FileName3 { get; set; }
        public string FileName4 { get; set; }
        public string FileName5 { get; set; }

        // Approval 에서 가져옴
        public string Status { get; set; } // 결재 상태


    }

    class Win_App_ApprovalReq_U_Money
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
        public string CustomID { get; set; }
        public string SalesCharge { get; set; }
        public string BankID { get; set; }

        public string CashAmount { get; set; }
        public string BillAmount { get; set; }
        public string BankAmount { get; set; }
        public string DCAmount { get; set; }
        public string BillNo { get; set; }

        public string ForReceiveBillAmount { get; set; }
        public string ReceiveNowDateYN { get; set; }
        public string CardAmount { get; set; }
        public string ReceivePersonName { get; set; }
        public string Comments { get; set; }

        public string OrderID { get; set; }
        public string RefBSNO { get; set; }
        public string OrderFlag { get; set; }
        public string RefRPItemCode { get; set; }
        public string RefComments { get; set; }

        public string RefAccountYN { get; set; }
        public string RefAmount { get; set; }

    }

    class App_Stemp
    {
        public int Num { get; set; }

        public string AppStepSeq { get; set; }
        public string PersonID { get; set; }
        public string Resably { get; set; }
        public string FolderName { get; set; }
        public string StempFileName { get; set; }
        public string Name { get; set; }
    }

    #endregion
}
