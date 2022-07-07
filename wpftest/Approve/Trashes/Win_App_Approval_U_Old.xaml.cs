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

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// Win_App_Approval_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_App_Approval_U_Old : UserControl
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
        //string FTP_ADDRESS = "ftp://" + LoadINI.FileSvr + ":" + LoadINI.FTPPort + LoadINI.FtpImagePath + "/Info";
        private const string FTP_ID = "wizuser";
        private const string FTP_PASS = "wiz9999";
        private const string LOCAL_DOWN_PATH = "C:\\Temp";

        string FTP_ADDRESS = "ftp://192.168.0.4/Approval";

        #endregion // FTP 변수들

        public Win_App_Approval_U_Old()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);

            SetComboBox();

            // 요청일자에 오늘날짜 세팅
            //chkDateSrh.IsChecked = true;
            dtpSDateSrh.SelectedDate = DateTime.Today;
            dtpEDateSrh.SelectedDate = DateTime.Today;

            chkAppSrh.IsChecked = true;
            cboAppSrh.SelectedIndex = 0; // 대기로 고정
        }

        #region 추가, 수정 모드 / 저장완료, 취소 모드

        private void SaveUpdateMode()
        {


            //btnAdd.IsEnabled = false;
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

            gbxInput.IsHitTestVisible = true;
            grdRbn.IsHitTestVisible = true;
            dgdMain.IsEnabled = false;

            // 첨부파일 부분
            btnUpload1.IsEnabled = true;
            btnDel1.IsEnabled = true;

            btnUpload2.IsEnabled = true;
            btnDel2.IsEnabled = true;

            btnUpload3.IsEnabled = true;
            btnDel3.IsEnabled = true;
        }

        private void CompleteCancelMode()
        {


            //btnAdd.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSearch.IsEnabled = true;

            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;

            btnExcel.IsEnabled = true;

            //lblMsg.Visibility = Visibility.Hidden;

            gbxInput.IsHitTestVisible = false;
            grdRbn.IsHitTestVisible = false;
            dgdMain.IsEnabled = true;

            // 첨부파일 부분
            btnUpload1.IsEnabled = false;
            btnDel1.IsEnabled = false;

            btnUpload2.IsEnabled = false;
            btnDel2.IsEnabled = false;

            btnUpload3.IsEnabled = false;
            btnDel3.IsEnabled = false;
        }

        #endregion // 추가, 수정 모드 / 저장완료, 취소 모드

        #region SetComboBox 콤보박스 세팅

        private void SetComboBox()
        {
            // 검색조건 - 결재상태 : 전체 추가하기
            ObservableCollection<CodeView> ovcApp = ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "APPROVAL", "Y", "");
            this.cboAppSrh.ItemsSource = ovcApp;
            this.cboAppSrh.DisplayMemberPath = "code_name";
            this.cboAppSrh.SelectedValuePath = "code_id";

            ObservableCollection<CodeView> ovcApp2 = ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "APPROVAL", "Y", "", "APP");
            // 결재  - 승인 / 부결 / 반려 / 보류
            this.cboApp.ItemsSource = ovcApp2;
            this.cboApp.DisplayMemberPath = "code_name";
            this.cboApp.SelectedValuePath = "code_id";

            // 결재구분  - 일반결재 / 매입결재
            ObservableCollection<CodeView> ovcGBN = ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "APPGBN", "Y", "");
            this.cboAppGBN.ItemsSource = ovcGBN;
            this.cboAppGBN.DisplayMemberPath = "code_name";
            this.cboAppGBN.SelectedValuePath = "code_id";

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
            dtpSDateSrh.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[0];
            dtpEDateSrh.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[1];
        }
        //전일
        private void btnYesterday_Click(object sender, RoutedEventArgs e)
        {
            dtpSDateSrh.SelectedDate = DateTime.Today.AddDays(-1);
            dtpEDateSrh.SelectedDate = DateTime.Today.AddDays(-1);
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

        // 검색조건 - 결재상태
        private void lblAppSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkAppSrh.IsChecked == true)
            {
                chkAppSrh.IsChecked = false;
            }
            else
            {
                chkAppSrh.IsChecked = true;
            }
        }
        private void chkAppSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkAppSrh.IsChecked = true;
            cboAppSrh.IsEnabled = true;
        }
        private void chkAppSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkAppSrh.IsChecked = false;
            cboAppSrh.IsEnabled = false;
        }

        #endregion // Header 부분 - 검색조건

        #region Header 부분 - 오른쪽 상단 버튼 이벤트

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //this.DataContext = null;

            //strFlag = "I";
            //dtpReqDate.SelectedDate = DateTime.Today;
            //cboAppGBN.SelectedIndex = 0;

            //SaveUpdateMode();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var AppReq = dgdMain.SelectedItem as Win_App_Approval_U_CodeView;

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
                }
                else
                {
                    MessageBox.Show("수정이 불가능 합니다.");
                }

            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

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
            var App = dgdMain.SelectedItem as Win_App_Approval_U_CodeView;

            if(SaveData(App))
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

        }


        #endregion // Header 부분 - 오른쪽 상단 버튼 이벤트

        #region Content 입력부분 - 오른쪽 (첨부파일 / 매일결재)

        // 일반결재 라디오 버튼 체크
        private void rbnCommonApp_Checked(object sender, RoutedEventArgs e)
        {
            if (gbxFile != null 
                && gbxPurchase != null)
            {
                gbxFile.Visibility = Visibility.Visible;
                gbxPurchase.Visibility = Visibility.Hidden;
            }
        }
        private void rbnCommonApp_Unchecked(object sender, RoutedEventArgs e)
        {
            if (gbxFile != null
                && gbxPurchase != null)
            {
                gbxFile.Visibility = Visibility.Hidden;
                gbxPurchase.Visibility = Visibility.Visible;
            }
        }

        // 매입결재 라디오 버튼 체크
        private void rbnPurchaseApp_Checked(object sender, RoutedEventArgs e)
        {
            if (gbxFile != null
               && gbxPurchase != null)
            {
                gbxFile.Visibility = Visibility.Hidden;
                gbxPurchase.Visibility = Visibility.Visible;
            }
        }                              
        private void rbnPurchaseApp_Unchecked(object sender, RoutedEventArgs e)
        {
            if (gbxFile != null
               && gbxPurchase != null)
            {
                gbxFile.Visibility = Visibility.Visible;
                gbxPurchase.Visibility = Visibility.Hidden;
            }          
        }

        #endregion // Content 입력부분 - 오른쪽 (첨부파일 / 매일결재)

        #region Content - 메인 그리드

        private void dgdMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var AppReq = dgdMain.SelectedItem as Win_App_Approval_U_CodeView;

            if (AppReq != null)
            {
                this.DataContext = AppReq;

                // 1 : 일반결재 / 2: 매입결제
                if (AppReq.AppGBN.Trim().Equals("1"))
                {
                    rbnCommonApp.IsChecked = true;
                }
                else
                {
                    rbnPurchaseApp.IsChecked = true;
                }

                // 자 만약에, 대기중이라면 cboApp : 대기중으로 세팅하기

                if (AppReq.Approval.Trim().Equals("0"))
                {
                    cboApp.Text = "대기중";
                }
            }
        }

        #endregion // Content - 메인 그리드

        #region 주요 메서드

        private void re_Search(int selectedIndex)
        {
            FillGrid();

            rowNum = 0;
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
                cboAppGBN.SelectedIndex = -1;

                txtFileName1.Text = "";
                txtFileName2.Text = "";
                txtFileName3.Text = "";

                cboApp.SelectedIndex = -1;
                txtReason.Text = "";

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
                sqlParameter.Add("SDate", chkDateSrh.IsChecked == true && dtpSDateSrh.SelectedDate != null ? dtpSDateSrh.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("EDate", chkDateSrh.IsChecked == true && dtpEDateSrh.SelectedDate != null ? dtpEDateSrh.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("nTitle", chkTitleSrh.IsChecked == true ? 1 : 0);
                sqlParameter.Add("Title", txtTitleSrh.Text);

                sqlParameter.Add("PersonID",MainWindow.CurrentPersonID);
                sqlParameter.Add("Approval", chkAppSrh.IsChecked == true && cboAppSrh.SelectedValue != null ? cboAppSrh.SelectedValue.ToString() : "9"); // 9 가 전체

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sApproval", sqlParameter, false);

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
                            var AppReq = new Win_App_Approval_U_CodeView()
                            {
                                Num = i,

                                AppReqID = dr["AppReqID"].ToString(),
                                AppSeq = dr["AppSeq"].ToString(),                              
                                AppDate = dr["AppDate"].ToString(),
                                AppDate_CV = DatePickerFormat(dr["AppDate"].ToString()),

                                Approval = dr["Approval"].ToString(),
                                ApprovalName = dr["ApprovalName"].ToString(),
                                Reason = dr["Reason"].ToString(),
                                TargetID = dr["TargetID"].ToString(),
                                TargetResably = dr["TargetResably"].ToString(),

                                Status = dr["Status"].ToString(),
                                UseClss = dr["UseClss"].ToString(),
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
                                ForderName = dr["ForderName"].ToString(),
                                FileName1 = dr["FileName1"].ToString(),
                                FileName2 = dr["FileName2"].ToString(),
                                FileName3 = dr["FileName3"].ToString(),
                                FileName4 = dr["FileName4"].ToString(),
                                FileName5 = dr["FileName5"].ToString(),
                            };

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

        #region 저장

        private bool SaveData(Win_App_Approval_U_CodeView App)
        {
            bool flag = false;

            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                if (CheckData())
                {
                    sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();

                    sqlParameter.Add("AppReqID", App.AppReqID);
                    sqlParameter.Add("AppSeq", ConvertInt(App.AppSeq));
                    sqlParameter.Add("PersonID", MainWindow.CurrentPersonID);
                    sqlParameter.Add("Approval", cboApp.SelectedValue != null ? cboApp.SelectedValue.ToString() : "");

                    sqlParameter.Add("Reason", cboApp.SelectedValue != null && cboApp.SelectedValue.ToString().Trim().Equals("1") ? "" : txtReason.Text);
                    sqlParameter.Add("UserID", MainWindow.CurrentUser);

                    Procedure pro1 = new Procedure();
                    pro1.list_OutputName = new List<string>();
                    pro1.list_OutputLength = new List<string>();

                    pro1.Name = "xp_Approval_iuApproval";
                    pro1.OutputUseYN = "N";
                    pro1.list_OutputName.Add("OutwareID");
                    pro1.list_OutputLength.Add("12");

                    Prolist.Add(pro1);
                    ListParameter.Add(sqlParameter);

                    List<KeyValue> list_Result = new List<KeyValue>();
                    list_Result = DataStore.Instance.ExecuteAllProcedureOutputListGetCS(Prolist, ListParameter);

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
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }


            return flag;
        }


        #endregion // 저장

        #region 유효성 검사

        private bool CheckData()
        {
            bool flag = true;

            // 입력란은 결재 콤보박스, 사유 뿐인데
            // 부결, 반려, 보류 시 사유는 필수 입력

            // 1. 결재 선택하지 않았을 시
            if (cboApp.SelectedValue == null
                 || cboApp.SelectedValue.ToString().Trim().Equals(""))
            {
                MessageBox.Show("결재를 선택해주세요.");
                flag = false;
                return flag;
            }

            // 2. 부결, 반려, 보류 : 1 2 3 → 사유 필수 입력
            if ((cboApp.SelectedValue.ToString().Trim().Equals("2")
                 || cboApp.SelectedValue.ToString().Trim().Equals("3")
                 || cboApp.SelectedValue.ToString().Trim().Equals("4")
                 ) && txtReason.Text.Trim().Equals(""))
            {
                MessageBox.Show("부결 / 반려 / 보류 시 사유를 필수로 입력해야 합니다.");
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

        #endregion // 삭제

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

        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {

        }

    }

}
