/**************************************************************************************************
   '** 작성자    : 정승학
   '** 내용      : 인사관리
   '** 생성일자  : 2019.09.20
   '**------------------------------------------------------------------------------------------------
   ''*************************************************************************************************
   ' 변경일자  , 변경자, 요청자    , 요구사항ID  , 요청 및 작업내용
   '**************************************************************************************************
   ' 2019.00.00  
**************************************************************************************************/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
    /// frm_Gte_PersonBasic_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class frm_Gte_PersonBasic_U : UserControl
    {
        string strFlag = string.Empty;
        int rowNum = 0;

        // 20200107 둘리
        int PersonSrhIndex = 0;

        Gte_PersonBasic_U_Insa_CodeView gtePersonBasicInsa = new Gte_PersonBasic_U_Insa_CodeView();
        Gte_PersonBasic_U_Home_CodeView gtePersonBasicHome = new Gte_PersonBasic_U_Home_CodeView();
        Gte_PersonBasic_U_License_CodeView gtePersonBasicLicense = new Gte_PersonBasic_U_License_CodeView();
        Gte_PersonBasic_U_PreviousRecord_CodeView gtePersonBasicPreviousRecord = new Gte_PersonBasic_U_PreviousRecord_CodeView();
        Gte_PersonBasic_U_Changes_CodeView gtePersonBasicChanges = new Gte_PersonBasic_U_Changes_CodeView();
        Gte_PersonBasic_U_Reference_CodeView gtePersonBasicReference = new Gte_PersonBasic_U_Reference_CodeView();

        // 인쇄 활용 용도 (프린트)
        private Microsoft.Office.Interop.Excel.Application excelapp;
        private Microsoft.Office.Interop.Excel.Workbook workbook;
        private Microsoft.Office.Interop.Excel.Worksheet worksheet;
        private Microsoft.Office.Interop.Excel.Range workrange;
        private Microsoft.Office.Interop.Excel.Worksheet copysheet;
        private Microsoft.Office.Interop.Excel.Worksheet pastesheet;
        WizMes_Alpha_JA.PopUp.NoticeMessage msg = new PopUp.NoticeMessage();

        public frm_Gte_PersonBasic_U()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            _ftp = new FTP_EX(FTP_ADDRESS, FTP_ID, FTP_PASS);
        }


        #region FTP
        //파일 수정 진행을 위한 flag 3개
        bool ExistFtp = false;
        bool AddFtp = false;
        bool DelFtp = false;

        //FTP
        string strImagePath = string.Empty;
        string strFullpath = string.Empty;
        string strDelFileName = string.Empty;

        private FTP_EX _ftp = null;
        private List<UploadFileInfo> _listFileInfo = new List<UploadFileInfo>();


        string strFullPath = string.Empty;
 

        List<string[]> listFtpFile = new List<string[]>();
        List<string[]> listStempFtpFile = new List<string[]>();
        List<string[]> deleteListFtpFile = new List<string[]>(); // 삭제할 파일 리스트
        List<string[]> deleteListStempFtpFile = new List<string[]>(); // 삭제할 파일 리스트


        internal struct UploadFileInfo
        {
            public string Filename { get; set; }
            public FtpFileType Type { get; set; }
            public DateTime LastModifiedTime { get; set; }
            public long Size { get; set; }
            public string Filepath { get; set; }

        }

        internal enum FtpFileType
        {
            None,
            DIR,
            File
        }

        //주소, 아이디, 비밀번호, 경로
        //string FTP_ADDRESS = "ftp://wizis.iptime.org/ImageData/McCode";

        //알FTP test 경로
        //string FTP_ADDRESS = "ftp://192.168.0.147";

        string FTP_ADDRESS = "ftp://" + LoadINI.FileSvr + ":" + LoadINI.FTPPort + LoadINI.FtpImagePath + "/GTE";
        private const string FTP_ID = "wizuser";
        private const string FTP_PASS = "wiz9999";
        private const string LOCAL_DOWN_PATH = "C:\\Temp";

        #endregion

        #region 상단 레이아웃 활성화 & 비활성화

        // 20200107 둘리
        // 퇴직자 포함 검색
        private void lblUseClssSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkUseClssSrh.IsChecked == true)
            {
                chkUseClssSrh.IsChecked = false;
            }
            else
            {
                chkUseClssSrh.IsChecked = true;
            }
        }
        private void chkUseClssSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkUseClssSrh.IsChecked = true;
            //dgdPersonSrh.Items.Clear();
        }
        private void chkUseClssSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkUseClssSrh.IsChecked = false;
            //dgdPersonSrh.Items.Clear();
        }

        //플러스파인더
        private void ButtonPerson_Click(object sender, RoutedEventArgs e)
        {
            string PT = "";
            if (TextBoxPerson.Tag != null) { PT = TextBoxPerson.Tag.ToString(); }

            MainWindow.pf.ReturnCode(TextBoxPerson, 79, chkUseClssSrh.IsChecked == true ? "Go" : "");

            if (TextBoxPerson.Tag != null && !TextBoxPerson.Tag.ToString().Equals(PT))
            {
                rowNum = 0;
                re_Search(rowNum);

                //if (dgdPersonSrh.Items.Count == 0)
                //{
                //    SetPersonN(79, "Go");
                //}
                dgdPersonSrh.Items.Clear();
                SetPersonN(79, chkUseClssSrh.IsChecked == true ? "Go" : "", TextBoxPerson.Tag.ToString());
            }
        }

        // 엔터
        private void txtPerson_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(TextBoxPerson, 79, chkUseClssSrh.IsChecked == true ? "Go" : "");

                if (TextBoxPerson.Tag != null)
                {
                    rowNum = 0;
                    re_Search(rowNum);

                    //if (dgdPersonSrh.Items.Count == 0)
                    //{
                    //    SetPersonN(79, "Go");
                    //}
                    dgdPersonSrh.Items.Clear();
                    SetPersonN(79, chkUseClssSrh.IsChecked == true ? "Go" : "", TextBoxPerson.Tag.ToString());
                }
            }

           
        }

        // 이전버튼
        private void btnPrevPerson_Click(object sender, RoutedEventArgs e)
        {
            int sIndex = dgdPersonSrh.SelectedIndex - 1;

            if (sIndex >= 0 && sIndex < dgdPersonSrh.Items.Count)
            {
                var Per = dgdPersonSrh.Items[sIndex] as GTE_JustPerson;

                if (Per != null)
                {
                    TextBoxPerson.Text = Per.Name;
                    TextBoxPerson.Tag = Per.PersonID;

                    rowNum = 0;
                    re_Search(rowNum);

                    dgdPersonSrh.SelectedIndex = sIndex;
                }
            }
        }
        // 다음 버튼
        private void btnNextPerson_Click(object sender, RoutedEventArgs e)
        {
            int sIndex = dgdPersonSrh.SelectedIndex + 1;

            if (sIndex >= 0 && sIndex < dgdPersonSrh.Items.Count)
            {
                var Per = dgdPersonSrh.Items[sIndex] as GTE_JustPerson;

                if (Per != null)
                {
                    TextBoxPerson.Text = Per.Name;
                    TextBoxPerson.Tag = Per.PersonID;

                    rowNum = 0;
                    re_Search(rowNum);

                    dgdPersonSrh.SelectedIndex = sIndex;
                }
            }
        }
        // 20200107 둘리
        private void SetPersonN(int large, string smiddle, string PersonID)
        {
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("nLarge", large);    // 데이터 그리드1의 Code_ID == 데이터 그리드2의 Code_GBN
                sqlParameter.Add("sMiddle", smiddle);   //입력된 코드를 추가

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Common_PlusFinder", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        int i = 0;
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow dr in drc)
                        {
                            if (dr["코드"].ToString().Trim().Equals(PersonID))
                            {
                                PersonSrhIndex = i;
                            }

                            i++;

                            var Person = new GTE_JustPerson()
                            {
                                Num = i,
                                Name = dr["성명"].ToString(),
                                PersonID = dr["코드"].ToString()
                            };

                            dgdPersonSrh.Items.Add(Person);
                        }

                        dgdPersonSrh.SelectedIndex = PersonSrhIndex;
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

        #region 버튼 모음
        //추가버튼
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            strFlag = "I";

            lblMsg.Visibility = Visibility.Visible;
            Lib.Instance.UiButtonEnableChange_SCControl(this);

            ButtonHomeAdd.Visibility = Visibility.Visible;
            ButtonHomeDelete.Visibility = Visibility.Visible;
            ButtonLicenseAdd.Visibility = Visibility.Visible;
            ButtonLicenseDelete.Visibility = Visibility.Visible;
            ButtonPreviousRecordAdd.Visibility = Visibility.Visible;
            ButtonPreviousRecordDelete.Visibility = Visibility.Visible;
            ButtonChangesAdd.Visibility = Visibility.Visible;
            ButtonChangesDelete.Visibility = Visibility.Visible;
            ButtonReferenceAdd.Visibility = Visibility.Visible;
            ButtonReferenceDelete.Visibility = Visibility.Visible;

            ButtonImageAdd.Visibility = Visibility.Visible;
            ButtonImageDelete.Visibility = Visibility.Visible;

            // 도장 이미지
            btnStempUpload.Visibility = Visibility.Visible;
            btnStempDelete.Visibility = Visibility.Visible;

            this.DataContext = null;

            AllClear();

            ImageSajinImage.Source = null;
            TextBoxImage.Clear();

            TextBoxInsertOn();

            TextBoxName.IsHitTestVisible = true;

            grdSrh1.IsHitTestVisible = false;
            grdSrh2.IsHitTestVisible = false;
        }

        //수정버튼
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxPerson.Text.Length <= 0 || TextBoxPerson.Text.Equals(""))
            {
                MessageBox.Show("사원이 검색되지 않았습니다. ");
            }
            else
            {
                strFlag = "U";
                Lib.Instance.UiButtonEnableChange_SCControl(this);

                tbkMsg.Text = "자료 수정 중";
                lblMsg.Visibility = Visibility.Visible;

             
                //if (gtePersonBasicInsa.SajinImage.Equals(""))
                //{
                //    ExistFtp = false;
                //}
                //else
                //{
                //    ExistFtp = true;
                //}

                TextBoxInsertOn();

                ButtonHomeAdd.Visibility = Visibility.Visible;
                ButtonHomeDelete.Visibility = Visibility.Visible;
                ButtonLicenseAdd.Visibility = Visibility.Visible;
                ButtonLicenseDelete.Visibility = Visibility.Visible;
                ButtonPreviousRecordAdd.Visibility = Visibility.Visible;
                ButtonPreviousRecordDelete.Visibility = Visibility.Visible;
                ButtonChangesAdd.Visibility = Visibility.Visible;
                ButtonChangesDelete.Visibility = Visibility.Visible;
                ButtonReferenceAdd.Visibility = Visibility.Visible;
                ButtonReferenceDelete.Visibility = Visibility.Visible;

                ButtonImageAdd.Visibility = Visibility.Visible;
                ButtonImageDelete.Visibility = Visibility.Visible;

                // 도장 이미지
                btnStempUpload.Visibility = Visibility.Visible;
                btnStempDelete.Visibility = Visibility.Visible;

                TextBoxName.IsHitTestVisible = false; // 수정일때는 성명 - 한글 막음

                grdSrh1.IsHitTestVisible = false;
                grdSrh2.IsHitTestVisible = false;
            }

            
        }

        //삭제버튼
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxPerson.Tag.ToString() == null)
            {
                MessageBox.Show("사원을 선택하세요.");
            }
            else
            {
                if (MessageBox.Show("모든 항목을 삭제하시겠습니까?", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (DeleteData(TextBoxPerson.Tag.ToString()))
                    {

                        if(TextBoxImage.Tag != null && !TextBoxImage.Text.Equals(string.Empty))
                        {
                            FTP_RemoveDir(TextBoxPerson.Tag.ToString());
                        }


                        ImageSajinImage.Source = null;
                        rowNum -= 1;
                        re_Search(rowNum);

                        //AllClear();

                    }
                }
            }
        }

        //닫기버튼
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        //조회버튼
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxPerson.Text.Length <= 0 || TextBoxPerson.Text.Equals(""))
            {
                MessageBox.Show("사원이 검색되지 않았습니다. ");
            }
            else
            {
                rowNum = 0;
                re_Search(rowNum);
            }


            
        }

        //저장버튼
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiButtonEnableChange_IUControl(this);

            if (SaveData(strFlag))
            {
                lblMsg.Visibility = Visibility.Hidden;
                rowNum = 0;

                ExistFtp = false;
                DelFtp = false;
                AddFtp = false;

                strFlag = string.Empty;
                strImagePath = string.Empty;
                strDelFileName = string.Empty;

                rowNum = 0;
                re_Search(rowNum);

            }
            else
            {
                this.DataContext = null;
            }

            ButtonHomeAdd.Visibility = Visibility.Hidden;
            ButtonHomeDelete.Visibility = Visibility.Hidden;
            ButtonLicenseAdd.Visibility = Visibility.Hidden;
            ButtonLicenseDelete.Visibility = Visibility.Hidden;
            ButtonPreviousRecordAdd.Visibility = Visibility.Hidden;
            ButtonPreviousRecordDelete.Visibility = Visibility.Hidden;
            ButtonChangesAdd.Visibility = Visibility.Hidden;
            ButtonChangesDelete.Visibility = Visibility.Hidden;
            ButtonReferenceAdd.Visibility = Visibility.Hidden;
            ButtonReferenceDelete.Visibility = Visibility.Hidden;

            ButtonImageAdd.Visibility = Visibility.Hidden;
            ButtonImageDelete.Visibility = Visibility.Hidden;
            TextBoxInsertOff();

            // 도장 이미지
            btnStempUpload.Visibility = Visibility.Hidden;
            btnStempDelete.Visibility = Visibility.Hidden;

            grdSrh1.IsHitTestVisible = true;
            grdSrh2.IsHitTestVisible = true;
        }

        //취소버튼
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnSearch_Click(null, null);
            Lib.Instance.UiButtonEnableChange_IUControl(this);

            TextBoxInsertOff();

            ButtonHomeAdd.Visibility = Visibility.Hidden;
            ButtonHomeDelete.Visibility = Visibility.Hidden;
            ButtonLicenseAdd.Visibility = Visibility.Hidden;
            ButtonLicenseDelete.Visibility = Visibility.Hidden;
            ButtonPreviousRecordAdd.Visibility = Visibility.Hidden;
            ButtonPreviousRecordDelete.Visibility = Visibility.Hidden;
            ButtonChangesAdd.Visibility = Visibility.Hidden;
            ButtonChangesDelete.Visibility = Visibility.Hidden;
            ButtonReferenceAdd.Visibility = Visibility.Hidden;
            ButtonReferenceDelete.Visibility = Visibility.Hidden;

            ButtonImageAdd.Visibility = Visibility.Hidden;
            ButtonImageDelete.Visibility = Visibility.Hidden;

            // 도장 이미지
            btnStempUpload.Visibility = Visibility.Hidden;
            btnStempDelete.Visibility = Visibility.Hidden;

            strFlag = string.Empty;
            strImagePath = string.Empty;
            strDelFileName = string.Empty;

            ExistFtp = false;

            // 파일 List 비워주기
            listFtpFile.Clear();
            listStempFtpFile.Clear();
            deleteListFtpFile.Clear();
            deleteListStempFtpFile.Clear();

            grdSrh1.IsHitTestVisible = true;
            grdSrh2.IsHitTestVisible = true;

        }

        //엑셀버튼
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] lst = new string[10];
            lst[0] = "가족사항";
            lst[1] = "자격면허";
            lst[2] = "입사전기록";
            lst[3] = "변동사유";
            lst[4] = "신원보증";
            lst[5] = dgdHome.Name;
            lst[6] = dgdLicense.Name;
            lst[7] = dgdPreviousRecord.Name;
            lst[8] = dgdChanges.Name;
            lst[9] = dgdReference.Name;

            ExportExcelxaml ExpExc = new ExportExcelxaml(lst);

            ExpExc.ShowDialog();

            if (ExpExc.DialogResult.HasValue)
            {
                if (ExpExc.choice.Equals(dgdHome.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdHome);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdHome);

                    Name = dgdHome.Name;

                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else if (ExpExc.choice.Equals(dgdLicense.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdLicense);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdLicense);

                    Name = dgdLicense.Name;

                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else if (ExpExc.choice.Equals(dgdPreviousRecord.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdPreviousRecord);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdPreviousRecord);

                    Name = dgdPreviousRecord.Name;

                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else if (ExpExc.choice.Equals(dgdChanges.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdChanges);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdChanges);

                    Name = dgdChanges.Name;

                    if (Lib.Instance.GenerateExcel(dt, Name))
                        Lib.Instance.excel.Visible = true;
                    else
                        return;
                }
                else if (ExpExc.choice.Equals(dgdReference.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdReference);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdReference);

                    Name = dgdReference.Name;

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

        //인쇄버튼
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var Gte = this.DataContext as Gte_PersonBasic_U_Insa_CodeView;

            if (Gte != null)
            {
                // 인쇄 메서드
                ContextMenu menu = btnPrint.ContextMenu;
                menu.StaysOpen = true;
                menu.IsOpen = true;
            }
        }

        private void menuSeeAhead_Click(object sender, RoutedEventArgs e)
        {
            var Gte = this.DataContext as Gte_PersonBasic_U_Insa_CodeView;

            if (Gte != null)
            {
                msg.Show();
                msg.Topmost = true;
                msg.Refresh();

                Lib.Instance.Delay(1000);

                PrintWork(true, true, Gte);

                msg.Visibility = Visibility.Hidden;
            }
        }

        private void menuRightPrint_Click(object sender, RoutedEventArgs e)
        {
            var Gte = this.DataContext as Gte_PersonBasic_U_Insa_CodeView;

            if (Gte != null)
            {
                msg.Show();
                msg.Topmost = true;
                msg.Refresh();

                Lib.Instance.Delay(1000);

                PrintWork(true, false, Gte);

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
        private void PrintWork(bool excelFlag, bool previewYN, Gte_PersonBasic_U_Insa_CodeView Gte)
        {
            try
            {
                excelapp = new Microsoft.Office.Interop.Excel.Application();

                string MyBookPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Report\\인사기록카드.xls";
                workbook = excelapp.Workbooks.Add(MyBookPath);
                worksheet = workbook.Sheets["인사기록부"];

                // 
                //workrange = worksheet.get_Range("B2", "J3"); // 셀 선택후 
                //workrange.Value2 = Gte.Name;                        // 입력
                //workrange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft; // 양식에서 정렬 되 있을테니 제외
                //workrange.Font.Size = 15; // 양식에서 폰트 사이즈 알아서 되 있을테니 제외


                #region 성명 및 기본정보
                // 1-1. 성명 - 한글, 한글, 영문
                // 한글
                workrange = worksheet.get_Range("D3");
                workrange.Value2 = Gte.Name;
                // 한자
                workrange = worksheet.get_Range("D4");
                workrange.Value2 = Gte.HanjaName;
                // 영문
                workrange = worksheet.get_Range("D5");
                workrange.Value2 = Gte.EngName;

                // 1-2. 입사일, 주민등록번호, 생년월일, 사번, 부서, 팀
                // 입사일
                workrange = worksheet.get_Range("I3");
                workrange.Value2 = DatePickerToFormat(Gte.StartDate);

                // 주민
                workrange = worksheet.get_Range("I4");
                workrange.Value2 = Gte.RegistID;

                // 생년월일
                workrange = worksheet.get_Range("I5");
                workrange.Value2 = DatePickerToFormat(Gte.BirthDay);

                // 사번
                workrange = worksheet.get_Range("N3");
                workrange.Value2 = "'" + Gte.Sabun;
                // 부서
                workrange = worksheet.get_Range("N4");
                workrange.Value2 = Gte.DepartID;
                // 팀
                workrange = worksheet.get_Range("N5");
                workrange.Value2 = Gte.TeamID;

                // 1-3.
                // 직위
                workrange = worksheet.get_Range("I7");
                workrange.Value2 = Gte.ResablyID;
                // 호주
                workrange = worksheet.get_Range("I8");
                workrange.Value2 = Gte.HomeHostName;
                // 승진일
                workrange = worksheet.get_Range("L7");
                workrange.Value2 = Gte.Sabun;
                // 관계
                workrange = worksheet.get_Range("L8");
                workrange.Value2 = Gte.HomeHostRel;
                // 성별
                workrange = worksheet.get_Range("O7");
                workrange.Value2 = Gte.SexGbn;
                // 직업
                workrange = worksheet.get_Range("O8");
                workrange.Value2 = Gte.HomeHostJob;
                // 현주소
                workrange = worksheet.get_Range("I9");
                workrange.Value2 = Gte.Address1;
                // 자택전화
                workrange = worksheet.get_Range("I10");
                workrange.Value2 = Gte.Phone;
                // 휴대폰
                workrange = worksheet.get_Range("I11");
                workrange.Value2 = Gte.HandPhone;
                // 이메일
                workrange = worksheet.get_Range("I12");
                workrange.Value2 = Gte.Email;
                // 팩스
                workrange = worksheet.get_Range("N12");
                workrange.Value2 = Gte.Fax;

                #endregion // 1. 성명 및 기본정보

                #region 개인사정
                // 3. 개인사정
                // 주업무
                workrange = worksheet.get_Range("D14");
                workrange.Value2 = Gte.MainJob;
                // 혼인
                workrange = worksheet.get_Range("D15");
                workrange.Value2 = Gte.MarryYN;
                // 신장
                workrange = worksheet.get_Range("D16");
                workrange.Value2 = Gte.BodyHeight;
                // 체중
                workrange = worksheet.get_Range("D17");
                workrange.Value2 = Gte.BodyWeight;
                // 혈액형
                workrange = worksheet.get_Range("D18");
                workrange.Value2 = Gte.BodyBloodType;
                // 취미
                workrange = worksheet.get_Range("D19");
                workrange.Value2 = Gte.Hobby;
                // 종교
                workrange = worksheet.get_Range("D20");
                workrange.Value2 = Gte.Religon;
                // 교통
                workrange = worksheet.get_Range("D21");
                workrange.Value2 = Gte.Transport;

                #endregion // 개인사정

                #region 학력
                // 4. 학력
                // 고딩
                workrange = worksheet.get_Range("H14");
                workrange.Value2 = Gte.HighSchoolName;

                workrange = worksheet.get_Range("L14");
                workrange.Value2 = Gte.HighSchoolDepart;

                workrange = worksheet.get_Range("N14");
                workrange.Value2 = Gte.HighSchoolFinishYN;

                // 전문대딩
                workrange = worksheet.get_Range("H15");
                workrange.Value2 = Gte.CollegeName;

                workrange = worksheet.get_Range("L15");
                workrange.Value2 = Gte.CollegeDepart;

                workrange = worksheet.get_Range("N15");
                workrange.Value2 = Gte.CollegeFinishYN;

                // 대딩
                workrange = worksheet.get_Range("H16");
                workrange.Value2 = Gte.UniverseName;

                workrange = worksheet.get_Range("L16");
                workrange.Value2 = Gte.UniverseDepart;

                workrange = worksheet.get_Range("N16");
                workrange.Value2 = Gte.UniverseFinishYN;

                // 대학원1
                workrange = worksheet.get_Range("H17");
                workrange.Value2 = Gte.BigUniverse1Name;

                workrange = worksheet.get_Range("L17");
                workrange.Value2 = Gte.BigUniverse1Depart;

                workrange = worksheet.get_Range("N17");
                workrange.Value2 = Gte.BigUniverse1FinishYN;

                // 대학원2
                workrange = worksheet.get_Range("H18");
                workrange.Value2 = Gte.BigUniverse2Name;

                workrange = worksheet.get_Range("L18");
                workrange.Value2 = Gte.BigUniverse2Depart;

                workrange = worksheet.get_Range("N18");
                workrange.Value2 = Gte.BigUniverse2FinishYN;

                #endregion //학력

                #region 병력
                // 5. 병력
                // 미필사유
                workrange = worksheet.get_Range("K19");
                workrange.Value2 = Gte.militaryNotComments;
                // 계급
                workrange = worksheet.get_Range("I20");
                workrange.Value2 = Gte.militaryLevel;
                // 군번
                workrange = worksheet.get_Range("I21");
                workrange.Value2 = Gte.militaryNo;
                // 복무기간
                workrange = worksheet.get_Range("N20");
                workrange.Value2 = Gte.militaryPeriod;
                // 군별
                workrange = worksheet.get_Range("L21");
                workrange.Value2 = Gte.militaryBul;
                // 병과
                workrange = worksheet.get_Range("O21");
                workrange.Value2 = Gte.militaryBungGa;
                #endregion // 병력

                #region 특례
                // 6. 특례
                // 신검년도
                workrange = worksheet.get_Range("D23");
                workrange.Value2 = Gte.militaryTRYear;
                // 복무년도
                workrange = worksheet.get_Range("D24");
                workrange.Value2 = Gte.militaryTRWorkYear;
                // 체격등위
                workrange = worksheet.get_Range("D25");
                workrange.Value2 = Gte.militaryTRHealthLevel;
                // 처분일자
                workrange = worksheet.get_Range("D26");
                workrange.Value2 = Gte.militaryTRDate;
                // 변동사요
                workrange = worksheet.get_Range("D27");
                workrange.Value2 = Gte.militaryTRChageComments;
                #endregion // 특례

                #region 퇴사기록
                // 7. 퇴사기록
                // 퇴사일자
                workrange = worksheet.get_Range("D28");
                workrange.Value2 = Gte.retireDate;

                // 퇴직사유
                workrange = worksheet.get_Range("D29");
                workrange.Value2 = Gte.retireReason;

                // 처리구분
                workrange = worksheet.get_Range("D30");
                workrange.Value2 = Gte.retireChoriGubun;

                // 근속연수
                workrange = worksheet.get_Range("D31");
                workrange.Value2 = Gte.retireWorkYear;

                // 퇴직금
                workrange = worksheet.get_Range("D32");
                workrange.Value2 = Gte.retireAmount;

                #endregion // 퇴사기록

                #region 가족사항 - 데이터그리드
                // 8. 가족사항 DataGrid
                int sRow = 24; // 시작행
                int maximum = 5; // 최대 칸!!
                for (int i = 0; i < dgdHome.Items.Count; i++)
                {
                    var Home = dgdHome.Items[i] as Gte_PersonBasic_U_Home_CodeView;

                    if (Home != null)
                    {
                        // 관계
                        workrange = worksheet.get_Range("H" + (sRow + i));
                        workrange.Value2 = Home.Relation;

                        // 관계
                        workrange = worksheet.get_Range("I" + (sRow + i));
                        workrange.Value2 = Home.Name;

                        // 관계
                        workrange = worksheet.get_Range("K" + (sRow + i));
                        workrange.Value2 = Home.BirthDay;

                        // 관계
                        workrange = worksheet.get_Range("M" + (sRow + i));
                        workrange.Value2 = Home.Job;

                        // 관계
                        workrange = worksheet.get_Range("O" + (sRow + i));
                        workrange.Value2 = Home.LivingTogether;
                    }

                    if (i == maximum - 1)
                    {
                        break;
                    }
                }

                #endregion // 가족사항 - 데이터그리드

                #region 자격면허 - 데이터 그리드
                // 9. 자격면허 DataGrid
                sRow = 30; // 시작행
                maximum = 3; // 최대 칸!!
                for (int i = 0; i < dgdLicense.Items.Count; i++)
                {
                    var License = dgdLicense.Items[i] as Gte_PersonBasic_U_License_CodeView;

                    if (License != null)
                    {
                        // 종류
                        workrange = worksheet.get_Range("H" + (sRow + i));
                        workrange.Value2 = License.LicenseName;

                        // 취득일
                        workrange = worksheet.get_Range("K" + (sRow + i));
                        workrange.Value2 = License.LicenseDate;

                        // 발행처
                        workrange = worksheet.get_Range("M" + (sRow + i));
                        workrange.Value2 = License.PublishingOffice;

                        // 번호
                        workrange = worksheet.get_Range("O" + (sRow + i));
                        workrange.Value2 = License.LicenseNumber;
                    }

                    if (i == maximum - 1)
                    {
                        break;
                    }
                }

                #endregion // 자격먼허 - 데이터 그리드

                #region 입사전기록 - 데이터 그리드
                // 10. 입사전기록 DataGrid
                sRow = 35; // 시작행
                maximum = 3; // 최대 칸!!
                for (int i = 0; i < dgdPreviousRecord.Items.Count; i++)
                {
                    var PRecord = dgdPreviousRecord.Items[i] as Gte_PersonBasic_U_PreviousRecord_CodeView;

                    if (PRecord != null)
                    {
                        // 근무시간
                        workrange = worksheet.get_Range("C" + (sRow + i));
                        workrange.Value2 = PRecord.Workdate;

                        // 직장명
                        workrange = worksheet.get_Range("G" + (sRow + i));
                        workrange.Value2 = PRecord.CompanyName;

                        // 직위
                        workrange = worksheet.get_Range("J" + (sRow + i));
                        workrange.Value2 = PRecord.JobGrade;

                        // 담당업무
                        workrange = worksheet.get_Range("L" + (sRow + i));
                        workrange.Value2 = PRecord.Business;

                        // 급여
                        workrange = worksheet.get_Range("O" + (sRow + i));
                        workrange.Value2 = PRecord.Salary;
                    }

                    if (i == maximum - 1)
                    {
                        break;
                    }
                }

                #endregion // 입사전기록 - 데이터 그리드

                #region 변동사항 - 데이터 그리드
                // 11. 변동사항 DataGrid
                sRow = 40; // 시작행
                maximum = 3; // 최대 칸!!
                for (int i = 0; i < dgdChanges.Items.Count; i++)
                {
                    var Changes = dgdChanges.Items[i] as Gte_PersonBasic_U_Changes_CodeView;

                    if (Changes != null)
                    {
                        // 발령일자
                        workrange = worksheet.get_Range("C" + (sRow + i));
                        workrange.Value2 = Changes.AppointmentDate;

                        // 부서
                        workrange = worksheet.get_Range("E" + (sRow + i));
                        workrange.Value2 = Changes.Department;

                        // 직위
                        workrange = worksheet.get_Range("G" + (sRow + i));
                        workrange.Value2 = Changes.JobGrade;

                        // 담당업무
                        workrange = worksheet.get_Range("H" + (sRow + i));
                        workrange.Value2 = Changes.ChangeWork;

                        // 소속부서장
                        workrange = worksheet.get_Range("J" + (sRow + i));
                        workrange.Value2 = Changes.DepartmentManager;

                        // 급여변동일자
                        workrange = worksheet.get_Range("L" + (sRow + i));
                        workrange.Value2 = Changes.SalaryChangeDate;

                        // 호봉
                        workrange = worksheet.get_Range("N" + (sRow + i));
                        workrange.Value2 = Changes.SalaryClass;

                        // 급여
                        workrange = worksheet.get_Range("O" + (sRow + i));
                        workrange.Value2 = Changes.Salary;
                    }

                    if (i == maximum - 1)
                    {
                        break;
                    }
                }

                #endregion // 변동사항 - 데이터 그리드

                #region 신원보증 - 데이터 그리드
                // 12. 신원보증 DataGrid
                sRow = 44; // 시작행
                maximum = 3; // 최대 칸!!
                for (int i = 0; i < dgdReference.Items.Count; i++)
                {
                    var Reference = dgdReference.Items[i] as Gte_PersonBasic_U_Reference_CodeView;

                    if (Reference != null)
                    {
                        // 보증인
                        workrange = worksheet.get_Range("C" + (sRow + i));
                        workrange.Value2 = Reference.Guarantor;

                        // 관계
                        workrange = worksheet.get_Range("E" + (sRow + i));
                        workrange.Value2 = Reference.Relation;

                        // 주민번호
                        workrange = worksheet.get_Range("G" + (sRow + i));
                        workrange.Value2 = Reference.RRN;

                        // 주소
                        workrange = worksheet.get_Range("J" + (sRow + i));
                        workrange.Value2 = Reference.Address;

                    }

                    if (i == maximum - 1)
                    {
                        break;
                    }
                }

                #endregion // 신원보증 - 데이터 그리드

                #region 사진 고정 : 다운로드 후에 그것으로 파일을 [(폭, 높이, x, y) 크기 + 좌표로] 올리는 방식

                try
                {
                    string str_path = FTP_ADDRESS + '/' + Gte.PersonID;
                    _ftp = new FTP_EX(str_path, FTP_ID, FTP_PASS);

                    string str_remotepath = Gte.SajinImage;
                    string str_localpath = LOCAL_DOWN_PATH + "\\" + Gte.SajinImage;

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

                    //workrange.CopyPicture()
                    //workrange = worksheet.get_Range(14);
                    //worksheet.Shapes.AddPicture("C:\\Temp\\" + Gte.SajinImage, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, ConvertInt(txtX.Text), ConvertInt(txtY.Text), ConvertInt(txtWidth.Text), ConvertInt(txtHeight.Text));
                    worksheet.Shapes.AddPicture("C:\\Temp\\" + Gte.SajinImage, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 42, 99, 121, 106);

                }
                catch (Exception ep1)
                {

                }

                #endregion

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

        }

        // 날짜 형식으로 변환
        private string DatePickerToFormat(string str)
        {
            if (!str.Trim().Equals("")
                && str.Length == 8)
            {
                str = str.Trim();

                string year = str.Substring(0, 4);
                string month = str.Substring(4, 2);
                string day = str.Substring(6, 2);

                str = year + "-" + month + "-" + day;
            }

            return str;
        }

        //가족사항 추가
        private void ButtonHomeAdd_Click(object sender, RoutedEventArgs e)
        {
            SubAddHome();
            int colCount = dgdHome.Columns.IndexOf(DataGridTextColumn_Relation);
            dgdHome.Focus();
            dgdHome.CurrentCell = new DataGridCellInfo(dgdHome.Items[dgdHome.Items.Count - 1], dgdHome.Columns[colCount]);
        }

        //가족사항 제거
        private void ButtonHomeDelete_Click(object sender, RoutedEventArgs e)
        {
            SubDeleteHome();
        }

        //자격면허 추가
        private void ButtonLicenseAdd_Click(object sender, RoutedEventArgs e)
        {
            SubAddLicense();
            int colCount = dgdLicense.Columns.IndexOf(DataGridTextColumnLicenseName);
            dgdLicense.Focus();
            dgdLicense.CurrentCell = new DataGridCellInfo(dgdLicense.Items[dgdLicense.Items.Count - 1], dgdLicense.Columns[colCount]);

        }

        //자격면허 제거
        private void ButtonLicenseDelete_Click(object sender, RoutedEventArgs e)
        {
            SubDeleteLicense();
        }

        //입사전기록 추가
        private void ButtonPreviousRecordAdd_Click(object sender, RoutedEventArgs e)
        {
            SubAddPreviousRecord();
            int colCount = dgdPreviousRecord.Columns.IndexOf(DataGridTextColumnWorkdate);
            dgdPreviousRecord.Focus();
            dgdPreviousRecord.CurrentCell = new DataGridCellInfo(dgdPreviousRecord.Items[dgdPreviousRecord.Items.Count - 1], dgdPreviousRecord.Columns[colCount]);

        }

        //입사전기록 제거
        private void ButtonPreviousRecordDelete_Click(object sender, RoutedEventArgs e)
        {
            SubDeletePreviousRecord();
        }

        //변동사항 추가
        private void ButtonChangesAdd_Click(object sender, RoutedEventArgs e)
        {
            SubAddChanges();
            int colCount = dgdChanges.Columns.IndexOf(DataGridTextColumnAppointmentDate);
            dgdChanges.Focus();
            dgdChanges.CurrentCell = new DataGridCellInfo(dgdChanges.Items[dgdChanges.Items.Count - 1], dgdChanges.Columns[colCount]);

        }

        //변동사항 제거
        private void ButtonChangesDelete_Click(object sender, RoutedEventArgs e)
        {
            SubDeleteChanges();
        }

        //신원보증 추가
        private void ButtonReferenceAdd_Click(object sender, RoutedEventArgs e)
        {
            SubAddReference();
            int colCount = dgdReference.Columns.IndexOf(DataGridTextColumnGuarantor);
            dgdReference.Focus();
            dgdReference.CurrentCell = new DataGridCellInfo(dgdReference.Items[dgdReference.Items.Count - 1], dgdReference.Columns[colCount]);

        }

        //신원보증 제거
        private void ButtonReferenceDelete_Click(object sender, RoutedEventArgs e)
        {
            SubDeleteReference();
        }

        #endregion

        #region 초기화
        private void AllClear()
        {
            TextBoxName.Clear();
            TextBoxHanjaName.Clear();
            TextBoxEngName.Clear();
            TextBoxStartDate.Clear();
            TextBoxRegistID.Clear();
            TextBoxBirthDay.Clear();
            TextBoxSabun.Clear();
            TextBoxDepartID.Clear();
            TextBoxTeamID.Clear();
            TextBoxResablyID.Clear();
            TextBoxSabunDate.Clear();
            TextBoxSexGbn.Clear();
            TextBoxImage.Clear();
            ImageSajinImage.Source = null;
            TextBoxHomeHostName.Clear();
            TextBoxHomeHostRel.Clear();
            TextBoxHomeHostJob.Clear();
            TextBoxAddress1.Clear();
            TextBoxPhone.Clear();
            TextBoxHandPhone.Clear();
            TextBoxEmail.Clear();
            TextBoxFax.Clear();
            TextBoxMainJob.Clear();
            TextBoxMarryYN.Clear();
            TextBoxBodyHeight.Clear();
            TextBoxBodyWeight.Clear();
            TextBoxBodyBloodType.Clear();
            TextBoxHobby.Clear();
            TextBoxReligon.Clear();
            TextBoxTransport.Clear();
            TextBoxHighSchoolName.Clear();
            TextBoxHighSchoolDepart.Clear();
            TextBoxHighSchoolFinishYN.Clear();
            TextBoxCollegeName.Clear();
            TextBoxCollegeDepart.Clear();
            TextBoxCollegeFinishYN.Clear();
            TextBoxBigUniverse1Name.Clear();
            TextBoxUniverseName.Clear();
            TextBoxUniverseDepart.Clear();
            TextBoxUniverseFinishYN.Clear();
            TextBoxBigUniverse1Name.Clear();
            TextBoxBigUniverse1Depart.Clear();
            TextBoxBigUniverse1FinishYN.Clear();
            TextBoxBigUniverse2Name.Clear();
            TextBoxBigUniverse2Depart.Clear();
            TextBoxBigUniverse2FinishYN.Clear();
            TextBoxMilitaryNotComments.Clear();
            TextBoxMilitaryLevel.Clear();
            TextBoxMilitaryPeriod.Clear();
            TextBoxMilitaryNo.Clear();
            TextBoxMilitaryBul.Clear();
            TextBoxMilitaryBunGa.Clear();
            TextBoxMilitaryTRYear.Clear();
            TextBoxMilitaryTRWorkYear.Clear();
            TextBoxMilitaryTRHealthLevel.Clear();
            TextBoxMilitaryTRDate.Clear();
            TextBoxMilitaryTRChageComments.Clear();
            TextBoxRetireDate.Clear();
            TextBoxRetireReason.Clear();
            TextBoxRetireChoriGubun.Clear();
            TextBoxRetireWorkYear.Clear();
            TextBoxRetireAmount.Clear();


            dgdHome.Items.Clear();
            dgdLicense.Items.Clear();
            dgdPreviousRecord.Items.Clear();
            dgdChanges.Items.Clear();
            dgdReference.Items.Clear();

        }

        #endregion

        #region 텍스트박스 입력모드
        private void TextBoxInsertOn()
        {
            TextBoxName.IsReadOnly = false;
            TextBoxHanjaName.IsReadOnly = false;
            TextBoxEngName.IsReadOnly = false;
            //TextBoxStartDate.IsReadOnly = false;
            //TextBoxRegistID.IsReadOnly = false;
            //TextBoxBirthDay.IsReadOnly = false;
            TextBoxSabun.IsReadOnly = false;
            //TextBoxDepartID.IsReadOnly = false;
            //TextBoxTeamID.IsReadOnly = false;
            //TextBoxResablyID.IsReadOnly = false;
            TextBoxSabunDate.IsReadOnly = false;
            TextBoxSexGbn.IsReadOnly = false;
            TextBoxImage.IsReadOnly = false;
            TextBoxHomeHostName.IsReadOnly = false;
            TextBoxHomeHostRel.IsReadOnly = false;
            TextBoxHomeHostJob.IsReadOnly = false;
            //TextBoxAddress1.IsReadOnly = false;
            //TextBoxPhone.IsReadOnly = false;
            //TextBoxHandPhone.IsReadOnly = false;
            //TextBoxEmail.IsReadOnly = false;
            TextBoxFax.IsReadOnly = false;
            TextBoxMainJob.IsReadOnly = false;
            TextBoxMarryYN.IsReadOnly = false;
            TextBoxBodyHeight.IsReadOnly = false;
            TextBoxBodyWeight.IsReadOnly = false;
            TextBoxBodyBloodType.IsReadOnly = false;
            TextBoxHobby.IsReadOnly = false;
            TextBoxReligon.IsReadOnly = false;
            TextBoxTransport.IsReadOnly = false;
            TextBoxHighSchoolName.IsReadOnly = false;
            TextBoxHighSchoolDepart.IsReadOnly = false;
            TextBoxHighSchoolFinishYN.IsReadOnly = false;
            TextBoxCollegeName.IsReadOnly = false;
            TextBoxCollegeDepart.IsReadOnly = false;
            TextBoxCollegeFinishYN.IsReadOnly = false;
            TextBoxBigUniverse1Name.IsReadOnly = false;
            TextBoxUniverseName.IsReadOnly = false;
            TextBoxUniverseDepart.IsReadOnly = false;
            TextBoxUniverseFinishYN.IsReadOnly = false;
            TextBoxBigUniverse1Name.IsReadOnly = false;
            TextBoxBigUniverse1Depart.IsReadOnly = false;
            TextBoxBigUniverse1FinishYN.IsReadOnly = false;
            TextBoxBigUniverse2Name.IsReadOnly = false;
            TextBoxBigUniverse2Depart.IsReadOnly = false;
            TextBoxBigUniverse2FinishYN.IsReadOnly = false;
            TextBoxMilitaryNotComments.IsReadOnly = false;
            TextBoxMilitaryLevel.IsReadOnly = false;
            TextBoxMilitaryPeriod.IsReadOnly = false;
            TextBoxMilitaryNo.IsReadOnly = false;
            TextBoxMilitaryBul.IsReadOnly = false;
            TextBoxMilitaryBunGa.IsReadOnly = false;
            TextBoxMilitaryTRYear.IsReadOnly = false;
            TextBoxMilitaryTRWorkYear.IsReadOnly = false;
            TextBoxMilitaryTRHealthLevel.IsReadOnly = false;
            TextBoxMilitaryTRDate.IsReadOnly = false;
            TextBoxMilitaryTRChageComments.IsReadOnly = false;
            TextBoxRetireDate.IsReadOnly = false;
            TextBoxRetireReason.IsReadOnly = false;
            TextBoxRetireChoriGubun.IsReadOnly = false;
            TextBoxRetireWorkYear.IsReadOnly = false;
            TextBoxRetireAmount.IsReadOnly = false;

        }

        #endregion

        #region 텍스트박스 입력모드 해제
        private void TextBoxInsertOff()
        {
            TextBoxName.IsReadOnly = true;
            TextBoxHanjaName.IsReadOnly = true;
            TextBoxEngName.IsReadOnly = true;
            //TextBoxStartDate.IsReadOnly = true;
            //TextBoxRegistID.IsReadOnly = true;
            //TextBoxBirthDay.IsReadOnly = true;
            TextBoxSabun.IsReadOnly = true;
            //TextBoxDepartID.IsReadOnly = true;
            //TextBoxTeamID.IsReadOnly = true;
            //TextBoxResablyID.IsReadOnly = true;
            TextBoxSabunDate.IsReadOnly = true;
            TextBoxSexGbn.IsReadOnly = true;
            TextBoxImage.IsReadOnly = true;
            TextBoxHomeHostName.IsReadOnly = true;
            TextBoxHomeHostRel.IsReadOnly = true;
            TextBoxHomeHostJob.IsReadOnly = true;
            //TextBoxAddress1.IsReadOnly = true;
            //TextBoxPhone.IsReadOnly = true;
            //TextBoxHandPhone.IsReadOnly = true;
            //TextBoxEmail.IsReadOnly = true;
            TextBoxFax.IsReadOnly = true;
            TextBoxMainJob.IsReadOnly = true;
            TextBoxMarryYN.IsReadOnly = true;
            TextBoxBodyHeight.IsReadOnly = true;
            TextBoxBodyWeight.IsReadOnly = true;
            TextBoxBodyBloodType.IsReadOnly = true;
            TextBoxHobby.IsReadOnly = true;
            TextBoxReligon.IsReadOnly = true;
            TextBoxTransport.IsReadOnly = true;
            TextBoxHighSchoolName.IsReadOnly = true;
            TextBoxHighSchoolDepart.IsReadOnly = true;
            TextBoxHighSchoolFinishYN.IsReadOnly = true;
            TextBoxCollegeName.IsReadOnly = true;
            TextBoxCollegeDepart.IsReadOnly = true;
            TextBoxCollegeFinishYN.IsReadOnly = true;
            TextBoxBigUniverse1Name.IsReadOnly = true;
            TextBoxUniverseName.IsReadOnly = true;
            TextBoxUniverseDepart.IsReadOnly = true;
            TextBoxUniverseFinishYN.IsReadOnly = true;
            TextBoxBigUniverse1Name.IsReadOnly = true;
            TextBoxBigUniverse1Depart.IsReadOnly = true;
            TextBoxBigUniverse1FinishYN.IsReadOnly = true;
            TextBoxBigUniverse2Name.IsReadOnly = true;
            TextBoxBigUniverse2Depart.IsReadOnly = true;
            TextBoxBigUniverse2FinishYN.IsReadOnly = true;
            TextBoxMilitaryNotComments.IsReadOnly = true;
            TextBoxMilitaryLevel.IsReadOnly = true;
            TextBoxMilitaryPeriod.IsReadOnly = true;
            TextBoxMilitaryNo.IsReadOnly = true;
            TextBoxMilitaryBul.IsReadOnly = true;
            TextBoxMilitaryBunGa.IsReadOnly = true;
            TextBoxMilitaryTRYear.IsReadOnly = true;
            TextBoxMilitaryTRWorkYear.IsReadOnly = true;
            TextBoxMilitaryTRHealthLevel.IsReadOnly = true;
            TextBoxMilitaryTRDate.IsReadOnly = true;
            TextBoxMilitaryTRChageComments.IsReadOnly = true;
            TextBoxRetireDate.IsReadOnly = true;
            TextBoxRetireReason.IsReadOnly = true;
            TextBoxRetireChoriGubun.IsReadOnly = true;
            TextBoxRetireWorkYear.IsReadOnly = true;
            TextBoxRetireAmount.IsReadOnly = true;

        }

        #endregion

        #region 조회 dgdMain
        private void FillGrid()
        {


            if (TextBoxPerson.Text.Equals("") == true | TextBoxPerson.Text.Equals(null) == true)
            {
                AllClear();               
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("PersonID", TextBoxPerson.Tag == null ? "" : TextBoxPerson.Tag.ToString());

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Gte_PersonBasic_s", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        DataRowCollection drc = dt.Rows;

                        for (int i = 0; i < drc.Count; i++)
                        {
                            DataRow dr = drc[i];

                            var gteinsa = new Gte_PersonBasic_U_Insa_CodeView()
                            {
                                PersonID = dr["PersonID"].ToString(),
                                Name = dr["Name"].ToString(),
                                EngName = dr["EngName"].ToString(),
                                HanjaName = dr["HanjaName"].ToString(),
                                StartDate = dr["StartDate"].ToString(),
                                RegistID = dr["RegistID"].ToString(),
                                BirthDay = dr["BirthDay"].ToString(),
                                Sabun = dr["Sabun"].ToString(),
                                DepartID = dr["DepartID"].ToString(),
                                TeamID = dr["TeamID"].ToString(),
                                ResablyID = dr["ResablyID"].ToString(),
                                SexGbn = dr["SexGbn"].ToString(),
                                SajinImage = dr["SajinImage"].ToString(),
                                SajinImagePath = dr["SajinImagePath"].ToString(),
                                HomeHostName = dr["HomeHostName"].ToString(),
                                HomeHostRel = dr["HomeHostRel"].ToString(),
                                HomeHostJob = dr["HomeHostJob"].ToString(),
                                Address1 = dr["Address1"].ToString(),
                                Address2 = dr["Address2"].ToString(),
                                AddressJiBun1 = dr["AddressJiBun1"].ToString(),
                                AddressJiBun2 = dr["AddressJiBun2"].ToString(),
                                Phone = dr["Phone"].ToString(),
                                HandPhone = dr["HandPhone"].ToString(),
                                Email = dr["Email"].ToString(),
                                Fax = dr["Fax"].ToString(),
                                MainJob = dr["MainJob"].ToString(),
                                MarryYN = dr["MarryYN"].ToString(),
                                BodyHeight = dr["BodyHeight"].ToString(),
                                BodyWeight = dr["BodyWeight"].ToString(),
                                BodyBloodType = dr["BodyBloodType"].ToString(),
                                Hobby = dr["Hobby"].ToString(),
                                Religon = dr["Religon"].ToString(),
                                Transport = dr["Transport"].ToString(),
                                HighSchoolName = dr["HighSchoolName"].ToString(),
                                HighSchoolDepart = dr["HighSchoolDepart"].ToString(),
                                HighSchoolFinishYN = dr["HighSchoolFinishYN"].ToString(),
                                CollegeName = dr["CollegeName"].ToString(),
                                CollegeDepart = dr["CollegeDepart"].ToString(),
                                CollegeFinishYN = dr["CollegeFinishYN"].ToString(),
                                UniverseName = dr["UniverseName"].ToString(),
                                UniverseDepart = dr["UniverseDepart"].ToString(),
                                UniverseFinishYN = dr["UniverseFinishYN"].ToString(),
                                BigUniverse1Name = dr["BigUniverse1Name"].ToString(),
                                BigUniverse1Depart = dr["BigUniverse1Depart"].ToString(),
                                BigUniverse1FinishYN = dr["BigUniverse1FinishYN"].ToString(),
                                BigUniverse2Name = dr["BigUniverse2Name"].ToString(),
                                BigUniverse2Depart = dr["BigUniverse2Depart"].ToString(),
                                BigUniverse2FinishYN = dr["BigUniverse2FinishYN"].ToString(),
                                militaryNotComments = dr["militaryNotComments"].ToString(),
                                militaryLevel = dr["militaryLevel"].ToString(),
                                militaryPeriod = dr["militaryPeriod"].ToString(),
                                militaryNo = dr["militaryNo"].ToString(),
                                militaryBul = dr["militaryBul"].ToString(),
                                militaryBungGa = dr["militaryBungGa"].ToString(),
                                militaryTRYear = dr["militaryTRYear"].ToString(),
                                militaryTRWorkYear = dr["militaryTRWorkYear"].ToString(),
                                militaryTRHealthLevel = dr["militaryTRHealthLevel"].ToString(),
                                militaryTRDate = dr["militaryTRDate"].ToString(),
                                militaryTRChageComments = dr["militaryTRChageComments"].ToString(),
                                retireDate = dr["retireDate"].ToString(),
                                retireReason = dr["retireReason"].ToString(),
                                retireChoriGubun = dr["retireChoriGubun"].ToString(),
                                retireWorkYear = dr["retireWorkYear"].ToString(),
                                retireAmount = dr["retireAmount"].ToString(),
                                CreateDate = dr["CreateDate"].ToString(),
                                CreateUserID = dr["CreateUserID"].ToString(),
                                LastUpdateDate = dr["LastUpdateDate"].ToString(),
                                LastUpdateUserID = dr["LastUpdateUserID"].ToString(),
                                StempFileName = dr["StempFileName"].ToString(),
                            };

                            //dgdMain.Items.Add(gteinsa);
                            this.DataContext = gteinsa;

                            // 사진 업로드
                            if (TextBoxPerson.Tag != null && !gteinsa.SajinImage.Trim().Equals(""))
                            {
                                //if (!txtImage.Text.Replace(" ", "").Equals(""))     //이미지 경로 값이 빈값이 아니면(사진이 저장되어 있을 겨우)
                                //{
                                //    string imageName = txtImage.Text;               // imageName에 경로 값을 대입

                                //    if (WinMcCode != null)          //선택된 그리드의 값이 null이 아니면
                                //    {
                                //        imgSetting.Source = SetImage(imageName, WinMcCode.Mcid);        //이미지를 보여줘
                                //    }
                                //}

                                ImageSajinImage.Source = SetImage(gteinsa.SajinImage, TextBoxPerson.Tag.ToString());

                                
                            }
                        }

                        
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("조회시 오류 : " + ee);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

        }
        #endregion

        #region 조회 dgdMain 추가용
        private void FillGridAdd()
        {
            if (TextBoxName.Text.Equals("") == true | TextBoxName.Text.Equals(null) == true)
            {
                AllClear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("PersonID", TextBoxName.Tag.ToString());

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Gte_PersonBasic_s", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        DataRowCollection drc = dt.Rows;

                        for (int i = 0; i < drc.Count; i++)
                        {
                            DataRow dr = drc[i];

                            var gteinsa = new Gte_PersonBasic_U_Insa_CodeView()
                            {
                                PersonID = dr["PersonID"].ToString(),
                                Name = dr["Name"].ToString(),
                                EngName = "",
                                HanjaName = "",
                                StartDate = dr["StartDate"].ToString(),
                                RegistID = dr["RegistID"].ToString(),
                                BirthDay = dr["BirthDay"].ToString(),
                                Sabun = "",
                                DepartID = dr["DepartID"].ToString(),
                                TeamID = dr["TeamID"].ToString(),
                                ResablyID = dr["ResablyID"].ToString(),
                                SexGbn = "",
                                SajinImage = "",
                                SajinImagePath = "",
                                HomeHostName = "",
                                HomeHostRel = "",
                                HomeHostJob = "",
                                Address1 = dr["Address1"].ToString(),
                                Address2 = "",
                                AddressJiBun1 = "",
                                AddressJiBun2 = "",
                                Phone = dr["Phone"].ToString(),
                                HandPhone = dr["HandPhone"].ToString(),
                                Email = dr["Email"].ToString(),
                                Fax = "",
                                MainJob = "",
                                MarryYN = "",
                                BodyHeight = "",
                                BodyWeight = "",
                                BodyBloodType = "",
                                Hobby = "",
                                Religon = "",
                                Transport = "",
                                HighSchoolName = "",
                                HighSchoolDepart = "",
                                HighSchoolFinishYN = "",
                                CollegeName = "",
                                CollegeDepart = "",
                                CollegeFinishYN = "",
                                UniverseName = "",
                                UniverseDepart = "",
                                UniverseFinishYN = "",
                                BigUniverse1Name = "",
                                BigUniverse1Depart = "",
                                BigUniverse1FinishYN = "",
                                BigUniverse2Name = "",
                                BigUniverse2Depart = "",
                                BigUniverse2FinishYN = "",
                                militaryNotComments = "",
                                militaryLevel = "",
                                militaryPeriod = "",
                                militaryNo = "",
                                militaryBul = "",
                                militaryBungGa = "",
                                militaryTRYear = "",
                                militaryTRWorkYear = "",
                                militaryTRHealthLevel = "",
                                militaryTRDate = "",
                                militaryTRChageComments = "",
                                retireDate = "",
                                retireReason = "",
                                retireChoriGubun = "",
                                retireWorkYear = "",
                                retireAmount = "",
                                CreateDate = "",
                                CreateUserID = "",
                                LastUpdateDate = "",
                                LastUpdateUserID = "",
                            };

                            //dgdMain.Items.Add(gteinsa);
                            this.DataContext = gteinsa;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("조회시 오류 : " + ee);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

        }
        #endregion

        #region 조회-가족사항 dgdHome
        private void FillGridHome(string PERSONID)
        {
            if (dgdHome.Items.Count > 0)
            {
                dgdHome.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("PersonID", TextBoxPerson.Tag.ToString());

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Gte_sPerson_Home", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        DataRowCollection drc = dt.Rows;
                        int j = 0;

                        for (int i = 0; i < drc.Count; i++)
                        {
                            j = i;
                            DataRow dr = drc[i];

                            var gpbh = new Gte_PersonBasic_U_Home_CodeView()
                            {
                                PersonID = dr["PersonID"].ToString(),
                                Seq = dr["Seq"].ToString(),
                                Relation = dr["Relation"].ToString(),
                                Name = dr["Name"].ToString(),
                                BirthDay = dr["BirthDay"].ToString(),
                                Job = dr["Job"].ToString(),
                                LivingTogether = dr["LivingTogether"].ToString()
                            };

                            dgdHome.Items.Add(gpbh);
                            //this.DataContext = gpbh;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("가족사항 조회 오류 : " + ee);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

        }
        #endregion

        #region 조회-자격면허 dgdLicense
        private void FillGridLicense(string PERSONID)
        {
            if (dgdLicense.Items.Count > 0)
            {
                dgdLicense.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("PersonID", TextBoxPerson.Tag.ToString());

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Gte_sPerson_License", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        DataRowCollection drc = dt.Rows;
                        int j = 0;

                        for (int i = 0; i < drc.Count; i++)
                        {
                            j = i;
                            DataRow dr = drc[i];

                            var gpbl = new Gte_PersonBasic_U_License_CodeView()
                            {
                                PersonID = dr["PersonID"].ToString(),
                                Seq = dr["Seq"].ToString(),
                                LicenseName = dr["LicenseName"].ToString(),
                                LicenseDate = dr["LicenseDate"].ToString(),
                                PublishingOffice = dr["PublishingOffice"].ToString(),
                                LicenseNumber = dr["LicenseNumber"].ToString(),

                            };

                            dgdLicense.Items.Add(gpbl);

                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("자격먼허 조회 오류 : " + ee);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }
        #endregion

        #region 조회-입사전기록 dgdPreviousRecord
        private void FillGridPreviousRecord(string PERSONID)
        {
            if (dgdPreviousRecord.Items.Count > 0)
            {
                dgdPreviousRecord.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("PersonID", TextBoxPerson.Tag.ToString());

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Gte_sPerson_PreviousRecord", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        DataRowCollection drc = dt.Rows;
                        int j = 0;

                        for (int i = 0; i < drc.Count; i++)
                        {
                            j = i;
                            DataRow dr = drc[i];

                            var gpbpr = new Gte_PersonBasic_U_PreviousRecord_CodeView()
                            {
                                PersonID = dr["PersonID"].ToString(),
                                Seq = dr["Seq"].ToString(),
                                Workdate = dr["Workdate"].ToString(),
                                CompanyName = dr["CompanyName"].ToString(),
                                JobGrade = dr["JobGrade"].ToString(),
                                Business = dr["Business"].ToString(),
                                Salary = dr["Salary"].ToString()

                            };

                            dgdPreviousRecord.Items.Add(gpbpr);

                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("입사전기록 조회 오류 : " + ee);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }
        #endregion

        #region 조회-변동사유 dgdChanges
        private void FillGridChanges(string PERSONID)
        {
            if (dgdChanges.Items.Count > 0)
            {
                dgdChanges.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("PersonID", TextBoxPerson.Tag.ToString());

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Gte_sPerson_Changes", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        DataRowCollection drc = dt.Rows;
                        int j = 0;

                        for (int i = 0; i < drc.Count; i++)
                        {
                            j = i;
                            DataRow dr = drc[i];

                            var gpbc = new Gte_PersonBasic_U_Changes_CodeView()
                            {
                                PersonID = dr["PersonID"].ToString(),
                                Seq = dr["Seq"].ToString(),
                                AppointmentDate = dr["AppointmentDate"].ToString(),
                                Department = dr["Department"].ToString(),
                                JobGrade = dr["JobGrade"].ToString(),
                                ChangeWork = dr["ChangeWork"].ToString(),
                                DepartmentManager = dr["DepartmentManager"].ToString(),
                                SalaryChangeDate = dr["SalaryChangeDate"].ToString(),
                                SalaryClass = dr["SalaryClass"].ToString(),
                                Salary = dr["Salary"].ToString()
                            };

                            dgdChanges.Items.Add(gpbc);

                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("변동사유 조회 오류 : " + ee);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }
        #endregion

        #region 조회-신원보증 dgdReference
        private void FillGridReference(string PERSONID)
        {
            if (dgdReference.Items.Count > 0)
            {
                dgdReference.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("PersonID", TextBoxPerson.Tag.ToString());

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Gte_sPerson_Reference", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        DataRowCollection drc = dt.Rows;
                        int j = 0;

                        for (int i = 0; i < drc.Count; i++)
                        {
                            j = i;
                            DataRow dr = drc[i];

                            var gpbr = new Gte_PersonBasic_U_Reference_CodeView()
                            {
                                PersonID = dr["PersonID"].ToString(),
                                Seq = dr["Seq"].ToString(),
                                Guarantor = dr["Guarantor"].ToString(),
                                Relation = dr["Relation"].ToString(),
                                RRN = dr["RRN"].ToString(),
                                Address = dr["Address"].ToString()
                            };

                            dgdReference.Items.Add(gpbr);

                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("신원보증 조회 오류 : " + ee);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }
        #endregion

        #region 재조회
        private void re_Search(int selectedIndex)
        {
            AllClear();

            FillGrid();
            FillGridHome(TextBoxPerson.Tag.ToString());
            FillGridLicense(TextBoxPerson.Tag.ToString());
            FillGridPreviousRecord(TextBoxPerson.Tag.ToString());
            FillGridChanges(TextBoxPerson.Tag.ToString());
            FillGridReference(TextBoxPerson.Tag.ToString());

            ImgStemp.Source = null;
        }

        #endregion

        #region 저장
        private bool SaveData(string strFlag)
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            string GetKey = "";

            try
            {
                //추가
                if (strFlag.Equals("I"))
                {
                    if (CheckIsInsa(TextBoxName.Tag.ToString()) == false) // 데이터가 이미 존재하는지
                    {
                        MessageBox.Show("이미 인사 정보가 등록되어 있어 추가가 불가능 합니다.");
                        return false;
                    }

                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();
                    sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") ? "" : TextBoxName.Tag);
                    sqlParameter.Add("EngName", TextBoxEngName.Text == null ? "" : TextBoxEngName.Text);
                    sqlParameter.Add("HanjaName", TextBoxHanjaName.Text == null ? "" : TextBoxHanjaName.Text);
                    sqlParameter.Add("Sabun", TextBoxSabun.Text == null ? "" : TextBoxSabun.Text);
                    sqlParameter.Add("SexGbn", TextBoxSexGbn.Text == null ? "" : TextBoxSexGbn.Text);

                    sqlParameter.Add("SajinImage", TextBoxImage.Text == null ? "" : TextBoxImage.Text);
                    //sqlParameter.Add("SajinImagePath", strImagePath.Equals("") == true ? "" : strImagePath);
                    sqlParameter.Add("SajinImagePath", "/GTE/" + TextBoxPerson.Tag.ToString());
                    sqlParameter.Add("HomeHostName", TextBoxHomeHostName.Text == null ? "" : TextBoxHomeHostName.Text);
                    sqlParameter.Add("HomeHostRel", TextBoxHomeHostRel.Text == null ? "" : TextBoxHomeHostRel.Text);
                    sqlParameter.Add("HomeHostJob", TextBoxHomeHostJob.Text == null ? "" : TextBoxHomeHostJob.Text);

                    sqlParameter.Add("Fax", TextBoxFax.Text == null ? "" : TextBoxFax.Text);
                    sqlParameter.Add("MainJob", TextBoxMainJob.Text == null ? "" : TextBoxMainJob.Text);
                    sqlParameter.Add("MarryYN", TextBoxMarryYN.Text == null ? "" : TextBoxMarryYN.Text);
                    sqlParameter.Add("Bodyheight", TextBoxBodyHeight.Text == null ? "" : TextBoxBodyHeight.Text);
                    sqlParameter.Add("BodyWeight", TextBoxBodyWeight.Text == null ? "" : TextBoxBodyWeight.Text);

                    sqlParameter.Add("BodyBloodType", TextBoxBodyBloodType.Text == null ? "" : TextBoxBodyBloodType.Text);
                    sqlParameter.Add("Hobby", TextBoxHobby.Text == null ? "" : TextBoxHobby.Text);
                    sqlParameter.Add("Religon", TextBoxReligon.Text == null ? "" : TextBoxReligon.Text);
                    sqlParameter.Add("Transport", TextBoxTransport.Text == null ? "" : TextBoxTransport.Text);
                    sqlParameter.Add("HighSchoolName", TextBoxHighSchoolName.Text == null ? "" : TextBoxHighSchoolName.Text);

                    sqlParameter.Add("HighSchoolDepart", TextBoxHighSchoolDepart.Text == null ? "" : TextBoxHighSchoolDepart.Text);
                    sqlParameter.Add("HighSchoolFinishYN", TextBoxHighSchoolFinishYN.Text == null ? "" : TextBoxHighSchoolFinishYN.Text);
                    sqlParameter.Add("CollegeName", TextBoxCollegeName.Text == null ? "" : TextBoxCollegeName.Text);
                    sqlParameter.Add("CollegeDepart", TextBoxCollegeDepart.Text == null ? "" : TextBoxCollegeDepart.Text);
                    sqlParameter.Add("CollegeFinishYN", TextBoxCollegeFinishYN.Text == null ? "" : TextBoxCollegeFinishYN.Text);

                    sqlParameter.Add("UniverseName", TextBoxUniverseName.Text == null ? "" : TextBoxUniverseName.Text);
                    sqlParameter.Add("UniverseDepart", TextBoxUniverseDepart.Text == null ? "" : TextBoxUniverseDepart.Text);
                    sqlParameter.Add("UniverseFinishYN", TextBoxUniverseFinishYN.Text == null ? "" : TextBoxUniverseFinishYN.Text);
                    sqlParameter.Add("BigUniverse1Name", TextBoxBigUniverse1Name.Text == null ? "" : TextBoxBigUniverse1Name.Text);
                    sqlParameter.Add("BigUniverse1Depart", TextBoxBigUniverse1Depart.Text == null ? "" : TextBoxBigUniverse1Depart.Text);

                    sqlParameter.Add("BigUniverse1FinishYN", TextBoxBigUniverse1FinishYN.Text == null ? "" : TextBoxBigUniverse1FinishYN.Text);
                    sqlParameter.Add("BigUniverse2Name", TextBoxBigUniverse2Name.Text == null ? "" : TextBoxBigUniverse2Name.Text);
                    sqlParameter.Add("BigUniverse2Depart", TextBoxBigUniverse2Depart.Text == null ? "" : TextBoxBigUniverse2Depart.Text);
                    sqlParameter.Add("BigUniverse2FinishYN", TextBoxBigUniverse2FinishYN.Text == null ? "" : TextBoxBigUniverse2FinishYN.Text);
                    sqlParameter.Add("militaryNotComments", TextBoxMilitaryNotComments.Text == null ? "" : TextBoxMilitaryNotComments.Text);

                    sqlParameter.Add("militaryLevel", TextBoxMilitaryLevel.Text == null ? "" : TextBoxMilitaryLevel.Text);
                    sqlParameter.Add("militaryPeriod", TextBoxMilitaryPeriod.Text == null ? "" : TextBoxMilitaryPeriod.Text);
                    sqlParameter.Add("militaryNo", TextBoxMilitaryNo.Text == null ? "" : TextBoxMilitaryNo.Text);
                    sqlParameter.Add("militaryBul", TextBoxMilitaryBul.Text == null ? "" : TextBoxMilitaryBul.Text);
                    sqlParameter.Add("militaryBungGa", TextBoxMilitaryBunGa.Text == null ? "" : TextBoxMilitaryBunGa.Text);

                    sqlParameter.Add("militaryTRYear", TextBoxMilitaryTRYear.Text == null ? "" : TextBoxMilitaryTRYear.Text);
                    sqlParameter.Add("militaryTRWorkYear", TextBoxMilitaryTRWorkYear.Text == null ? "" : TextBoxMilitaryTRWorkYear.Text);
                    sqlParameter.Add("militaryTRHealthLevel", TextBoxMilitaryTRHealthLevel.Text == null ? "" : TextBoxMilitaryTRHealthLevel.Text);
                    sqlParameter.Add("militaryTRDate", TextBoxMilitaryTRDate.Text == null ? "" : TextBoxMilitaryTRDate.Text);
                    sqlParameter.Add("militaryTRChageComments", TextBoxMilitaryTRChageComments.Text == null ? "" : TextBoxMilitaryTRChageComments.Text);

                    sqlParameter.Add("retireDate", TextBoxRetireDate.Text == null ? "" : TextBoxRetireDate.Text);
                    sqlParameter.Add("retireReason", TextBoxRetireReason.Text == null ? "" : TextBoxRetireReason.Text);
                    sqlParameter.Add("retireChoriGubun", TextBoxRetireChoriGubun.Text == null ? "" : TextBoxRetireChoriGubun.Text);
                    sqlParameter.Add("retireWorkYear", TextBoxRetireWorkYear.Text == null ? "" : TextBoxRetireWorkYear.Text);
                    sqlParameter.Add("retireAmount", TextBoxRetireAmount.Text == null ? "" : TextBoxRetireAmount.Text);

                    sqlParameter.Add("CreateDate", DateTime.Today);
                    sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);
                    sqlParameter.Add("LastUpdateDate", "");
                    sqlParameter.Add("LastUpdateUserID", "");

                    Procedure pro1 = new Procedure();
                    pro1.Name = "xp_Gte_PersonBasic_i";
                    pro1.OutputUseYN = "N";
                    pro1.OutputName = "PersonID";
                    pro1.OutputLength = "8";

                    Prolist.Add(pro1);
                    ListParameter.Add(sqlParameter);

                    for (int i = 0; i < dgdHome.Items.Count; i++)
                    {
                        gtePersonBasicHome = dgdHome.Items[i] as Gte_PersonBasic_U_Home_CodeView;

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") == true ? "" : TextBoxName.Tag.ToString());
                        sqlParameter.Add("Seq", i + 1);
                        sqlParameter.Add("Relation", gtePersonBasicHome.Relation.Equals("") == true ? "" : gtePersonBasicHome.Relation);
                        sqlParameter.Add("Name", gtePersonBasicHome.Name.Equals("") == true ? "" : gtePersonBasicHome.Name);
                        sqlParameter.Add("BirthDay", gtePersonBasicHome.BirthDay.Equals("") == true ? "" : gtePersonBasicHome.BirthDay);

                        sqlParameter.Add("Job", gtePersonBasicHome.Job.Equals("") == true ? "" : gtePersonBasicHome.Job);
                        sqlParameter.Add("LivingTogether", gtePersonBasicHome.LivingTogether.Equals("") == true ? "" : gtePersonBasicHome.LivingTogether);
                        sqlParameter.Add("CreateDate", DateTime.Today);
                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro2 = new Procedure();
                        pro2.Name = "xp_Gte_iPerson_Home";
                        pro2.OutputUseYN = "N";
                        pro2.OutputName = "PersonID";
                        pro2.OutputLength = "8";

                        Prolist.Add(pro2);
                        ListParameter.Add(sqlParameter);
                    }

                    for (int i = 0; i < dgdLicense.Items.Count; i++)
                    {
                        gtePersonBasicLicense = dgdLicense.Items[i] as Gte_PersonBasic_U_License_CodeView;

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") == true ? "" : TextBoxName.Tag.ToString());
                        sqlParameter.Add("Seq", i + 1);
                        sqlParameter.Add("LicenseName", gtePersonBasicLicense.LicenseName.Equals("") == true ? "" : gtePersonBasicLicense.LicenseName);
                        sqlParameter.Add("LicenseDate", gtePersonBasicLicense.LicenseDate.Equals("") == true ? "" : gtePersonBasicLicense.LicenseDate);
                        sqlParameter.Add("PublishingOffice", gtePersonBasicLicense.PublishingOffice.Equals("") == true ? "" : gtePersonBasicLicense.PublishingOffice);

                        sqlParameter.Add("LicenseNumber", gtePersonBasicLicense.LicenseNumber.Equals("") == true ? "" : gtePersonBasicLicense.LicenseNumber);
                        sqlParameter.Add("CreateDate", DateTime.Today);
                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro3 = new Procedure();
                        pro3.Name = "xp_Gte_iPerson_License";
                        pro3.OutputUseYN = "N";
                        pro3.OutputName = "PersonID";
                        pro3.OutputLength = "8";

                        Prolist.Add(pro3);
                        ListParameter.Add(sqlParameter);
                    }

                    for (int i = 0; i < dgdPreviousRecord.Items.Count; i++)
                    {
                        gtePersonBasicPreviousRecord = dgdPreviousRecord.Items[i] as Gte_PersonBasic_U_PreviousRecord_CodeView;

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") == true ? "" : TextBoxName.Tag.ToString());
                        sqlParameter.Add("Seq", i + 1);
                        sqlParameter.Add("Workdate", gtePersonBasicPreviousRecord.Workdate.Equals("") == true ? "" : gtePersonBasicPreviousRecord.Workdate);
                        sqlParameter.Add("CompanyName", gtePersonBasicPreviousRecord.CompanyName.Equals("") == true ? "" : gtePersonBasicPreviousRecord.CompanyName);
                        sqlParameter.Add("JobGrade", gtePersonBasicPreviousRecord.JobGrade.Equals("") == true ? "" : gtePersonBasicPreviousRecord.JobGrade);

                        sqlParameter.Add("Business", gtePersonBasicPreviousRecord.Business.Equals("") == true ? "" : gtePersonBasicPreviousRecord.Business);
                        sqlParameter.Add("Salary", gtePersonBasicPreviousRecord.Salary.Equals("") == true ? "" : gtePersonBasicPreviousRecord.Salary);
                        sqlParameter.Add("CreateDate", DateTime.Today);
                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro4 = new Procedure();
                        pro4.Name = "xp_Gte_iPerson_PreviousRecord";
                        pro4.OutputUseYN = "N";
                        pro4.OutputName = "PersonID";
                        pro4.OutputLength = "8";

                        Prolist.Add(pro4);
                        ListParameter.Add(sqlParameter);
                    }

                    for (int i = 0; i < dgdChanges.Items.Count; i++)
                    {
                        gtePersonBasicChanges = dgdChanges.Items[i] as Gte_PersonBasic_U_Changes_CodeView;

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") == true ? "" : TextBoxName.Tag.ToString());
                        sqlParameter.Add("Seq", i + 1);
                        sqlParameter.Add("AppointmentDate", gtePersonBasicChanges.AppointmentDate.Equals("") == true ? "" : gtePersonBasicChanges.AppointmentDate);
                        sqlParameter.Add("Department", gtePersonBasicChanges.Department.Equals("") == true ? "" : gtePersonBasicChanges.Department);
                        sqlParameter.Add("JobGrade", gtePersonBasicChanges.JobGrade.Equals("") == true ? "" : gtePersonBasicChanges.JobGrade);

                        sqlParameter.Add("DepartmentManager", gtePersonBasicChanges.DepartmentManager.Equals("") == true ? "" : gtePersonBasicChanges.DepartmentManager);
                        sqlParameter.Add("SalaryChangeDate", gtePersonBasicChanges.SalaryChangeDate.Equals("") == true ? "" : gtePersonBasicChanges.SalaryChangeDate);
                        sqlParameter.Add("SalaryClass", gtePersonBasicChanges.SalaryClass.Equals("") == true ? "" : gtePersonBasicChanges.SalaryClass);
                        sqlParameter.Add("Salary", gtePersonBasicChanges.Salary.Equals("") == true ? "" : gtePersonBasicChanges.Salary);
                        sqlParameter.Add("CreateDate", DateTime.Today);

                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro5 = new Procedure();
                        pro5.Name = "xp_Gte_iPerson_Changes";
                        pro5.OutputUseYN = "N";
                        pro5.OutputName = "PersonID";
                        pro5.OutputLength = "8";

                        Prolist.Add(pro5);
                        ListParameter.Add(sqlParameter);
                    }

                    for (int i = 0; i < dgdReference.Items.Count; i++)
                    {
                        gtePersonBasicReference = dgdReference.Items[i] as Gte_PersonBasic_U_Reference_CodeView;

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") == true ? "" : TextBoxName.Tag.ToString());
                        sqlParameter.Add("Seq", i + 1);
                        sqlParameter.Add("Guarantor", gtePersonBasicReference.Guarantor.Equals("") == true ? "" : gtePersonBasicReference.Guarantor);
                        sqlParameter.Add("Relation", gtePersonBasicReference.Relation.Equals("") == true ? "" : gtePersonBasicReference.Relation);
                        sqlParameter.Add("RRN", gtePersonBasicReference.RRN.Equals("") == true ? "" : gtePersonBasicReference.RRN);

                        sqlParameter.Add("Address", gtePersonBasicReference.Address.Equals("") == true ? "" : gtePersonBasicReference.Address);
                        sqlParameter.Add("CreateDate", DateTime.Today);
                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro6 = new Procedure();
                        pro6.Name = "xp_Gte_iPerson_Reference";
                        pro6.OutputUseYN = "N";
                        pro6.OutputName = "PersonID";
                        pro6.OutputLength = "8";

                        Prolist.Add(pro6);
                        ListParameter.Add(sqlParameter);
                    }


                }

                //List<KeyValue> list_Result = new List<KeyValue>();
                //list_Result = DataStore.Instance.ExecuteAllProcedureOutputGetCS(Prolist, ListParameter);
                //string sGetID = string.Empty;

                //if (list_Result[0].key.ToLower() == "success")
                //{
                //    list_Result.RemoveAt(0);
                //    for (int i = 0; i < list_Result.Count; i++)
                //    {
                //        KeyValue kv = list_Result[i];
                //        if (kv.key == "sNewMCID")
                //        {
                //            sGetID = kv.value;
                //            flag = true;
                //        }
                //    }

                //    if (flag)
                //    {
                //        if (TextBoxImage.Tag != null)
                //        {

                //        }
                //    }
                //}


                //수정
                if (strFlag.Equals("U"))
                {
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();
                    sqlParameter.Add("PersonID", TextBoxPerson.Tag.ToString());
                    sqlParameter.Add("EngName", TextBoxEngName.Text == null ? "" : TextBoxEngName.Text);
                    sqlParameter.Add("HanjaName", TextBoxHanjaName.Text == null ? "" : TextBoxHanjaName.Text);
                    sqlParameter.Add("Sabun", TextBoxSabun.Text == null ? "" : TextBoxSabun.Text);
                    sqlParameter.Add("SexGbn", TextBoxSexGbn.Text == null ? "" : TextBoxSexGbn.Text);

                    sqlParameter.Add("SajinImage", TextBoxImage.Text == null ? "" : TextBoxImage.Text);
                    //sqlParameter.Add("SajinImagePath", strImagePath.Equals("") == true ? "" : strImagePath);
                    sqlParameter.Add("SajinImagePath", "/GTE/" + TextBoxPerson.Tag.ToString());
                    sqlParameter.Add("HomeHostName", TextBoxHomeHostName.Text == null ? "" : TextBoxHomeHostName.Text);
                    sqlParameter.Add("HomeHostRel", TextBoxHomeHostRel.Text == null ? "" : TextBoxHomeHostRel.Text);
                    sqlParameter.Add("HomeHostJob", TextBoxHomeHostJob.Text == null ? "" : TextBoxHomeHostJob.Text);
                    
                    sqlParameter.Add("Fax", TextBoxFax.Text == null ? "" : TextBoxFax.Text);
                    sqlParameter.Add("MainJob", TextBoxMainJob.Text == null ? "" : TextBoxMainJob.Text);
                    sqlParameter.Add("MarryYN", TextBoxMarryYN.Text == null ? "" : TextBoxMarryYN.Text);
                    sqlParameter.Add("Bodyheight", TextBoxBodyHeight.Text == null ? "" : TextBoxBodyHeight.Text);
                    sqlParameter.Add("BodyWeight", TextBoxBodyWeight.Text == null ? "" : TextBoxBodyWeight.Text);

                    sqlParameter.Add("BodyBloodType", TextBoxBodyBloodType.Text == null ? "" : TextBoxBodyBloodType.Text);
                    sqlParameter.Add("Hobby", TextBoxHobby.Text == null ? "" : TextBoxHobby.Text);
                    sqlParameter.Add("Religon", TextBoxReligon.Text == null ? "" : TextBoxReligon.Text);
                    sqlParameter.Add("Transport", TextBoxTransport.Text == null ? "" : TextBoxTransport.Text);
                    sqlParameter.Add("HighSchoolName", TextBoxHighSchoolName.Text == null ? "" : TextBoxHighSchoolName.Text);

                    sqlParameter.Add("HighSchoolDepart", TextBoxHighSchoolDepart.Text == null ? "" : TextBoxHighSchoolDepart.Text);
                    sqlParameter.Add("HighSchoolFinishYN", TextBoxHighSchoolFinishYN.Text == null ? "" : TextBoxHighSchoolFinishYN.Text);
                    sqlParameter.Add("CollegeName", TextBoxCollegeName.Text == null ? "" : TextBoxCollegeName.Text);
                    sqlParameter.Add("CollegeDepart", TextBoxCollegeDepart.Text == null ? "" : TextBoxCollegeDepart.Text);
                    sqlParameter.Add("CollegeFinishYN", TextBoxCollegeFinishYN.Text == null ? "" : TextBoxCollegeFinishYN.Text);

                    sqlParameter.Add("UniverseName", TextBoxUniverseName.Text == null ? "" : TextBoxUniverseName.Text);
                    sqlParameter.Add("UniverseDepart", TextBoxUniverseDepart.Text == null ? "" : TextBoxUniverseDepart.Text);
                    sqlParameter.Add("UniverseFinishYN", TextBoxUniverseFinishYN.Text == null ? "" : TextBoxUniverseFinishYN.Text);
                    sqlParameter.Add("BigUniverse1Name", TextBoxBigUniverse1Name.Text == null ? "" : TextBoxBigUniverse1Name.Text);
                    sqlParameter.Add("BigUniverse1Depart", TextBoxBigUniverse1Depart.Text == null ? "" : TextBoxBigUniverse1Depart.Text);

                    sqlParameter.Add("BigUniverse1FinishYN", TextBoxBigUniverse1FinishYN.Text == null ? "" : TextBoxBigUniverse1FinishYN.Text);
                    sqlParameter.Add("BigUniverse2Name", TextBoxBigUniverse2Name.Text == null ? "" : TextBoxBigUniverse2Name.Text);
                    sqlParameter.Add("BigUniverse2Depart", TextBoxBigUniverse2Depart.Text == null ? "" : TextBoxBigUniverse2Depart.Text);
                    sqlParameter.Add("BigUniverse2FinishYN", TextBoxBigUniverse2FinishYN.Text == null ? "" : TextBoxBigUniverse2FinishYN.Text);
                    sqlParameter.Add("militaryNotComments", TextBoxMilitaryNotComments.Text == null ? "" : TextBoxMilitaryNotComments.Text);

                    sqlParameter.Add("militaryLevel", TextBoxMilitaryLevel.Text == null ? "" : TextBoxMilitaryLevel.Text);
                    sqlParameter.Add("militaryPeriod", TextBoxMilitaryPeriod.Text == null ? "" : TextBoxMilitaryPeriod.Text);
                    sqlParameter.Add("militaryNo", TextBoxMilitaryNo.Text == null ? "" : TextBoxMilitaryNo.Text);
                    sqlParameter.Add("militaryBul", TextBoxMilitaryBul.Text == null ? "" : TextBoxMilitaryBul.Text);
                    sqlParameter.Add("militaryBungGa", TextBoxMilitaryBunGa.Text == null ? "" : TextBoxMilitaryBunGa.Text);

                    sqlParameter.Add("militaryTRYear", TextBoxMilitaryTRYear.Text == null ? "" : TextBoxMilitaryTRYear.Text);
                    sqlParameter.Add("militaryTRWorkYear", TextBoxMilitaryTRWorkYear.Text == null ? "" : TextBoxMilitaryTRWorkYear.Text);
                    sqlParameter.Add("militaryTRHealthLevel", TextBoxMilitaryTRHealthLevel.Text == null ? "" : TextBoxMilitaryTRHealthLevel.Text);
                    sqlParameter.Add("militaryTRDate", TextBoxMilitaryTRDate.Text == null ? "" : TextBoxMilitaryTRDate.Text);
                    sqlParameter.Add("militaryTRChageComments", TextBoxMilitaryTRChageComments.Text == null ? "" : TextBoxMilitaryTRChageComments.Text);

                    sqlParameter.Add("retireDate", TextBoxRetireDate.Text == null ? "" : TextBoxRetireDate.Text);
                    sqlParameter.Add("retireReason", TextBoxRetireReason.Text == null ? "" : TextBoxRetireReason.Text);
                    sqlParameter.Add("retireChoriGubun", TextBoxRetireChoriGubun.Text == null ? "" : TextBoxRetireChoriGubun.Text);
                    sqlParameter.Add("retireWorkYear", TextBoxRetireWorkYear.Text == null ? "" : TextBoxRetireWorkYear.Text);
                    sqlParameter.Add("retireAmount", TextBoxRetireAmount.Text == null ? "" : TextBoxRetireAmount.Text);


                    sqlParameter.Add("LastUpdateDate", DateTime.Today);
                    sqlParameter.Add("LastUpdateUserID", MainWindow.CurrentUser);

                    Procedure pro1 = new Procedure();
                    pro1.Name = "xp_Gte_PersonBasic_u";
                    pro1.OutputUseYN = "N";
                    pro1.OutputName = "PersonID";
                    pro1.OutputLength = "8";

                    Prolist.Add(pro1);
                    ListParameter.Add(sqlParameter);

                    for (int i = 0; i < dgdHome.Items.Count; i++)
                    {
                        gtePersonBasicHome = dgdHome.Items[i] as Gte_PersonBasic_U_Home_CodeView;

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") == true ? "" : TextBoxName.Tag.ToString());
                        sqlParameter.Add("Seq", i + 1);
                        sqlParameter.Add("Relation", gtePersonBasicHome.Relation.Equals("") == true ? "" : gtePersonBasicHome.Relation);
                        sqlParameter.Add("Name", gtePersonBasicHome.Name.Equals("") == true ? "" : gtePersonBasicHome.Name);
                        sqlParameter.Add("BirthDay", gtePersonBasicHome.BirthDay.Equals("") == true ? "" : gtePersonBasicHome.BirthDay);

                        sqlParameter.Add("Job", gtePersonBasicHome.Job.Equals("") == true ? "" : gtePersonBasicHome.Job);
                        sqlParameter.Add("LivingTogether", gtePersonBasicHome.LivingTogether.Equals("") == true ? "" : gtePersonBasicHome.LivingTogether);
                        sqlParameter.Add("CreateDate", DateTime.Today);
                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro2 = new Procedure();
                        pro2.Name = "xp_Gte_iPerson_Home";
                        pro2.OutputUseYN = "N";
                        pro2.OutputName = "PersonID";
                        pro2.OutputLength = "8";

                        Prolist.Add(pro2);
                        ListParameter.Add(sqlParameter);
                    }

                    for (int i = 0; i < dgdLicense.Items.Count; i++)
                    {
                        gtePersonBasicLicense = dgdLicense.Items[i] as Gte_PersonBasic_U_License_CodeView;

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") == true ? "" : TextBoxName.Tag.ToString());
                        sqlParameter.Add("Seq", i + 1);
                        sqlParameter.Add("LicenseName", gtePersonBasicLicense.LicenseName.Equals("") == true ? "" : gtePersonBasicLicense.LicenseName);
                        sqlParameter.Add("LicenseDate", gtePersonBasicLicense.LicenseDate.Equals("") == true ? "" : gtePersonBasicLicense.LicenseDate);
                        sqlParameter.Add("PublishingOffice", gtePersonBasicLicense.PublishingOffice.Equals("") == true ? "" : gtePersonBasicLicense.PublishingOffice);

                        sqlParameter.Add("LicenseNumber", gtePersonBasicLicense.LicenseNumber.Equals("") == true ? "" : gtePersonBasicLicense.LicenseNumber);
                        sqlParameter.Add("CreateDate", DateTime.Today);
                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro3 = new Procedure();
                        pro3.Name = "xp_Gte_iPerson_License";
                        pro3.OutputUseYN = "N";
                        pro3.OutputName = "PersonID";
                        pro3.OutputLength = "8";

                        Prolist.Add(pro3);
                        ListParameter.Add(sqlParameter);
                    }

                    for (int i = 0; i < dgdPreviousRecord.Items.Count; i++)
                    {
                        gtePersonBasicPreviousRecord = dgdPreviousRecord.Items[i] as Gte_PersonBasic_U_PreviousRecord_CodeView;

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") == true ? "" : TextBoxName.Tag.ToString());
                        sqlParameter.Add("Seq", i + 1);
                        sqlParameter.Add("Workdate", gtePersonBasicPreviousRecord.Workdate.Equals("") == true ? "" : gtePersonBasicPreviousRecord.Workdate);
                        sqlParameter.Add("CompanyName", gtePersonBasicPreviousRecord.CompanyName.Equals("") == true ? "" : gtePersonBasicPreviousRecord.CompanyName);
                        sqlParameter.Add("JobGrade", gtePersonBasicPreviousRecord.JobGrade.Equals("") == true ? "" : gtePersonBasicPreviousRecord.JobGrade);

                        sqlParameter.Add("Business", gtePersonBasicPreviousRecord.Business.Equals("") == true ? "" : gtePersonBasicPreviousRecord.Business);
                        sqlParameter.Add("Salary", gtePersonBasicPreviousRecord.Salary.Equals("") == true ? "" : gtePersonBasicPreviousRecord.Salary);
                        sqlParameter.Add("CreateDate", DateTime.Today);
                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro4 = new Procedure();
                        pro4.Name = "xp_Gte_iPerson_PreviousRecord";
                        pro4.OutputUseYN = "N";
                        pro4.OutputName = "PersonID";
                        pro4.OutputLength = "8";

                        Prolist.Add(pro4);
                        ListParameter.Add(sqlParameter);
                    }

                    for (int i = 0; i < dgdChanges.Items.Count; i++)
                    {
                        gtePersonBasicChanges = dgdChanges.Items[i] as Gte_PersonBasic_U_Changes_CodeView;

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") == true ? "" : TextBoxName.Tag.ToString());
                        sqlParameter.Add("Seq", i + 1);
                        sqlParameter.Add("AppointmentDate", gtePersonBasicChanges.AppointmentDate.Equals("") == true ? "" : gtePersonBasicChanges.AppointmentDate);
                        sqlParameter.Add("Department", gtePersonBasicChanges.Department.Equals("") == true ? "" : gtePersonBasicChanges.Department);
                        sqlParameter.Add("JobGrade", gtePersonBasicChanges.JobGrade.Equals("") == true ? "" : gtePersonBasicChanges.JobGrade);

                        sqlParameter.Add("DepartmentManager", gtePersonBasicChanges.DepartmentManager.Equals("") == true ? "" : gtePersonBasicChanges.DepartmentManager);
                        sqlParameter.Add("SalaryChangeDate", gtePersonBasicChanges.SalaryChangeDate.Equals("") == true ? "" : gtePersonBasicChanges.SalaryChangeDate);
                        sqlParameter.Add("SalaryClass", gtePersonBasicChanges.SalaryClass.Equals("") == true ? "" : gtePersonBasicChanges.SalaryClass);
                        sqlParameter.Add("Salary", gtePersonBasicChanges.Salary.Equals("") == true ? "" : gtePersonBasicChanges.Salary);
                        sqlParameter.Add("CreateDate", DateTime.Today);

                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro5 = new Procedure();
                        pro5.Name = "xp_Gte_iPerson_Changes";
                        pro5.OutputUseYN = "N";
                        pro5.OutputName = "PersonID";
                        pro5.OutputLength = "8";

                        Prolist.Add(pro5);
                        ListParameter.Add(sqlParameter);
                    }

                    for (int i = 0; i < dgdReference.Items.Count; i++)
                    {
                        gtePersonBasicReference = dgdReference.Items[i] as Gte_PersonBasic_U_Reference_CodeView;

                        sqlParameter = new Dictionary<string, object>();
                        sqlParameter.Clear();
                        sqlParameter.Add("PersonID", TextBoxName.Tag.Equals("") == true ? "" : TextBoxName.Tag.ToString());
                        sqlParameter.Add("Seq", i + 1);
                        sqlParameter.Add("Guarantor", gtePersonBasicReference.Guarantor.Equals("") == true ? "" : gtePersonBasicReference.Guarantor);
                        sqlParameter.Add("Relation", gtePersonBasicReference.Relation.Equals("") == true ? "" : gtePersonBasicReference.Relation);
                        sqlParameter.Add("RRN", gtePersonBasicReference.RRN.Equals("") == true ? "" : gtePersonBasicReference.RRN);

                        sqlParameter.Add("Address", gtePersonBasicReference.Address.Equals("") == true ? "" : gtePersonBasicReference.Address);
                        sqlParameter.Add("CreateDate", DateTime.Today);
                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro6 = new Procedure();
                        pro6.Name = "xp_Gte_iPerson_Reference";
                        pro6.OutputUseYN = "N";
                        pro6.OutputName = "PersonID";
                        pro6.OutputLength = "8";

                        Prolist.Add(pro6);
                        ListParameter.Add(sqlParameter);
                    }

                }

                string[] Confirm = new string[2];
                Confirm = DataStore.Instance.ExecuteAllProcedureOutputNew(Prolist, ListParameter);

                if (Confirm[0].ToLower() != "success") 
                {
                    MessageBox.Show("저장실패 " + Confirm[1].ToString());
                    flag = false;
                    return flag;
                }
                else
                {
                    flag = true;

                    if (TextBoxName.Tag != null)
                    {
                        GetKey = TextBoxName.Tag.ToString();
                    }
                    
                }

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
                        //UpdateDBFtp(GetKey);
                    }
                }

                // 도장이미지를 올려보자
                if (!GetKey.Trim().Equals(""))
                {
                    if (deleteListStempFtpFile.Count > 0)
                    {
                        foreach (string[] str in deleteListStempFtpFile)
                        {
                            FTP_RemoveFileStemp(GetKey + "/" + str[0]);
                        }
                    }

                    if (listStempFtpFile.Count > 0)
                    {
                        FTP_Save_FileStemp(listStempFtpFile, GetKey);
                        UpdateStempFtp(GetKey);
                    }
                }

                // 파일 List 비워주기
                listFtpFile.Clear();
                listStempFtpFile.Clear();
                deleteListFtpFile.Clear();
                deleteListStempFtpFile.Clear();

            }
            catch (Exception ee)
            {
                MessageBox.Show("저장1 오류 : " + ee);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return flag;

        }

        #region 도장이미지 프로시저

        private void UpdateStempFtp(string PersonID)
        {
            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("OwnerID", PersonID);
                sqlParameter.Add("FolderName", "Approval/Stemp");
                sqlParameter.Add("StempFileName", txtStemp.Text);

                Procedure pro1 = new Procedure();
                pro1.list_OutputName = new List<string>();
                pro1.list_OutputLength = new List<string>();

                pro1.Name = "xp_Approval_iuApproval_Stemp";
                pro1.OutputUseYN = "N";
                pro1.list_OutputName.Add("OutwareID");
                pro1.list_OutputLength.Add("12");

                Prolist.Add(pro1);
                ListParameter.Add(sqlParameter);

                
                List<KeyValue> list_Result = new List<KeyValue>();
                list_Result = DataStore.Instance.ExecuteAllProcedureOutputListGetCS(Prolist, ListParameter);

                if (list_Result[0].key.ToLower() == "success")
                {

                }
                else
                {
                    MessageBox.Show("[저장실패]\r\n" + list_Result[0].value.ToString());
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

        private bool CheckIsInsa(string strID)
        {
            bool flag = true;

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();

                sqlParameter.Add("PersonID", strID);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Gte_Person_ChkIsThereInsaData", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        int i = 0;
                        DataRowCollection drc = dt.Rows;

                        DataRow dr = drc[0];

                        int cnt = ConvertInt(dr["Num"].ToString());

                        if (cnt > 0)
                        {
                            flag = false;
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

            return flag;
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

        #endregion

        #endregion

        #region 삭제
        private bool DeleteData(string PERSONID)
        {
            bool flag = false;

            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Clear();
            sqlParameter.Add("PersonID", PERSONID);

            string[] result = DataStore.Instance.ExecuteProcedure("xp_Gte_PersonBasic_d", sqlParameter, false);
            DataStore.Instance.CloseConnection();

            if (result[0].Equals("success"))
            {
                flag = true;
            }

            return flag;
        }

        #endregion

        #region 입력 체크
        private bool CheckData()
        {

            bool flag = true;

            return flag;

        }
        #endregion

        #region DataGrid - RowAdd, RowDelete
        //가족사항 추가
        private void SubAddHome()
        {
            int i = 1;

            if (dgdHome.Items.Count > 0)
            {
                i = dgdHome.Items.Count + 1;
            }

            var gtePersonBasicHome = new Gte_PersonBasic_U_Home_CodeView()
            {
                Seq = "",
                Relation = "",
                Name = "",
                BirthDay = "",
                Job = "",
                LivingTogether = ""
            };
            dgdHome.Items.Add(gtePersonBasicHome);
        }

        //가족사항 제거
        private void SubDeleteHome()
        {
            if (dgdHome.Items.Count > 0)
            {
                if (dgdHome.CurrentItem != null)
                {
                    dgdHome.Items.Remove(dgdHome.CurrentItem as Gte_PersonBasic_U_Home_CodeView);
                }
                else
                {
                    dgdHome.Items.Remove((dgdHome.Items[dgdHome.Items.Count - 1]) as Gte_PersonBasic_U_Home_CodeView);
                }

                dgdHome.Refresh();
            }
        }

        //자격면허 추가
        private void SubAddLicense()
        {
            int i = 1;

            if (dgdLicense.Items.Count > 0)
            {
                i = dgdLicense.Items.Count + 1;
            }

            var gtePersonBasicLicense = new Gte_PersonBasic_U_License_CodeView()
            {
                Seq = "",
                LicenseName = "",
                LicenseDate = "",
                PublishingOffice = "",
                LicenseNumber = ""
            };
            dgdLicense.Items.Add(gtePersonBasicLicense);
        }

        //자격면허 제거
        private void SubDeleteLicense()
        {
            if (dgdLicense.Items.Count > 0)
            {
                if (dgdLicense.CurrentItem != null)
                {
                    dgdLicense.Items.Remove(dgdLicense.CurrentItem as Gte_PersonBasic_U_License_CodeView);
                }
                else
                {
                    dgdLicense.Items.Remove((dgdLicense.Items[dgdLicense.Items.Count - 1]) as Gte_PersonBasic_U_License_CodeView);
                }

                dgdLicense.Refresh();
            }
        }

        //입사전기록 추가
        private void SubAddPreviousRecord()
        {
            int i = 1;

            if (dgdPreviousRecord.Items.Count > 0)
            {
                i = dgdPreviousRecord.Items.Count + 1;
            }

            var gtePersonBasicPreviousRecord = new Gte_PersonBasic_U_PreviousRecord_CodeView()
            {
                Seq = "",
                Workdate = "",
                CompanyName = "",
                JobGrade = "",
                Business = "",
                Salary = ""
            };
            dgdPreviousRecord.Items.Add(gtePersonBasicPreviousRecord);
        }

        //입사전기록 제거
        private void SubDeletePreviousRecord()
        {
            if (dgdPreviousRecord.Items.Count > 0)
            {
                if (dgdPreviousRecord.CurrentItem != null)
                {
                    dgdPreviousRecord.Items.Remove(dgdPreviousRecord.CurrentItem as Gte_PersonBasic_U_PreviousRecord_CodeView);
                }
                else
                {
                    dgdPreviousRecord.Items.Remove((dgdPreviousRecord.Items[dgdPreviousRecord.Items.Count - 1]) as Gte_PersonBasic_U_PreviousRecord_CodeView);
                }

                dgdPreviousRecord.Refresh();
            }
        }

        //변동사항 추가
        private void SubAddChanges()
        {
            int i = 1;

            if (dgdChanges.Items.Count > 0)
            {
                i = dgdChanges.Items.Count + 1;
            }

            var gtePersonBasicChanges = new Gte_PersonBasic_U_Changes_CodeView()
            {
                Seq = "",
                AppointmentDate = "",
                Department = "",
                JobGrade = "",
                ChangeWork = "",
                DepartmentManager = "",
                SalaryChangeDate = "",
                SalaryClass = "",
                Salary = ""
            };
            dgdChanges.Items.Add(gtePersonBasicChanges);
        }

        //변동사항 제거
        private void SubDeleteChanges()
        {
            if (dgdChanges.Items.Count > 0)
            {
                if (dgdChanges.CurrentItem != null)
                {
                    dgdChanges.Items.Remove(dgdChanges.CurrentItem as Gte_PersonBasic_U_Changes_CodeView);
                }
                else
                {
                    dgdChanges.Items.Remove((dgdChanges.Items[dgdChanges.Items.Count - 1]) as Gte_PersonBasic_U_Changes_CodeView);
                }

                dgdChanges.Refresh();
            }
        }

        //신원보증 추가
        private void SubAddReference()
        {
            int i = 1;

            if (dgdReference.Items.Count > 0)
            {
                i = dgdReference.Items.Count + 1;
            }

            var gtePersonBasicReference = new Gte_PersonBasic_U_Reference_CodeView()
            {
                Seq = "",
                Guarantor = "",
                Relation = "",
                RRN = "",
                Address = ""
            };
            dgdReference.Items.Add(gtePersonBasicReference);
        }

        //신원보증 제거
        private void SubDeleteReference()
        {
            if (dgdReference.Items.Count > 0)
            {
                if (dgdReference.CurrentItem != null)
                {
                    dgdReference.Items.Remove(dgdReference.CurrentItem as Gte_PersonBasic_U_Reference_CodeView);
                }
                else
                {
                    dgdReference.Items.Remove((dgdReference.Items[dgdReference.Items.Count - 1]) as Gte_PersonBasic_U_Reference_CodeView);
                }

                dgdReference.Refresh();
            }
        }
        #endregion

        #region dgdHome 수정, 이동 이벤트
        private void DataGridHome_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                DataGridHome_KeyDown(sender, e);
            }
        }

        private void DataGridHome_KeyDown(object sender, KeyEventArgs e)
        {
            var gtePersonBasicHome = dgdHome.CurrentItem as Gte_PersonBasic_U_Home_CodeView;
            int rowCount = dgdHome.Items.IndexOf(dgdHome.CurrentItem);
            int colCount = dgdHome.Columns.IndexOf(dgdHome.CurrentCell.Column);
            int lastColcount = 5;
            int startColcount = 0;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (lastColcount == colCount && dgdHome.Items.Count - 1 > rowCount)
                {
                    dgdHome.SelectedIndex = rowCount + 1;
                    dgdHome.CurrentCell = new DataGridCellInfo(dgdHome.Items[rowCount + 1], dgdHome.Columns[startColcount]);
                }
                else if (lastColcount > colCount && dgdHome.Items.Count - 1 > rowCount)
                {
                    dgdHome.CurrentCell = new DataGridCellInfo(dgdHome.Items[rowCount], dgdHome.Columns[colCount + 1]);
                }
                else if (lastColcount == colCount && dgdHome.Items.Count - 1 == rowCount)
                {
                    btnSave.Focus();
                }
                else if (lastColcount > colCount && dgdHome.Items.Count - 1 == rowCount)
                {
                    dgdHome.CurrentCell = new DataGridCellInfo(dgdHome.Items[rowCount], dgdHome.Columns[colCount + 1]);
                }
                else
                {
                    MessageBox.Show("??");
                }
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (dgdHome.Items.Count - 1 > rowCount)
                {
                    dgdHome.SelectedIndex = rowCount + 1;
                    dgdHome.CurrentCell = new DataGridCellInfo(dgdHome.Items[rowCount + 1], dgdHome.Columns[colCount]);
                }
                else if (dgdHome.Items.Count - 1 == rowCount)
                {
                    if (lastColcount > colCount)
                    {
                        dgdHome.SelectedIndex = 0;
                        dgdHome.CurrentCell = new DataGridCellInfo(dgdHome.Items[0], dgdHome.Columns[colCount + 1]);
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (rowCount > 0)
                {
                    dgdHome.SelectedIndex = rowCount - 1;
                    dgdHome.CurrentCell = new DataGridCellInfo(dgdHome.Items[rowCount - 1], dgdHome.Columns[colCount]);
                }
            }
            else if (e.Key == Key.Left)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (colCount > 0)
                {
                    dgdHome.CurrentCell = new DataGridCellInfo(dgdHome.Items[rowCount], dgdHome.Columns[colCount - 1]);
                }
            }
            else if (e.Key == Key.Right)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (lastColcount > colCount)
                {
                    dgdHome.CurrentCell = new DataGridCellInfo(dgdHome.Items[rowCount], dgdHome.Columns[colCount + 1]);
                }
                else if (lastColcount == colCount)
                {
                    if (dgdHome.Items.Count - 1 > rowCount)
                    {
                        dgdHome.SelectedIndex = rowCount + 1;
                        dgdHome.CurrentCell = new DataGridCellInfo(dgdHome.Items[rowCount + 1], dgdHome.Columns[startColcount]);
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
        }

        private void DataGridHome_KeyUp(object sender, KeyEventArgs e)
        {
            Lib.Instance.DataGridINControlFocus(sender, e);
        }

        private void DataGridHome_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Lib.Instance.DataGridINBothByMouseUP(sender, e);
        }

        private void DataGridHome_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                DataGridCell cell = sender as DataGridCell;
                cell.IsEditing = true;
            }
        }

        #endregion

        #region dgdLicense 수정, 이동 이벤트
        private void DataGridLicense_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                DataGridLicense_KeyDown(sender, e);
            }
        }

        private void DataGridLicense_KeyDown(object sender, KeyEventArgs e)
        {
            var gtePersonBasicLicense = dgdLicense.CurrentItem as Gte_PersonBasic_U_License_CodeView;
            int rowCount = dgdLicense.Items.IndexOf(dgdLicense.CurrentItem);
            int colCount = dgdLicense.Columns.IndexOf(dgdLicense.CurrentCell.Column);
            int lastColcount = 4;
            int startColcount = 0;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (lastColcount == colCount && dgdLicense.Items.Count - 1 > rowCount)
                {
                    dgdLicense.SelectedIndex = rowCount + 1;
                    dgdLicense.CurrentCell = new DataGridCellInfo(dgdLicense.Items[rowCount + 1], dgdLicense.Columns[startColcount]);
                }
                else if (lastColcount > colCount && dgdLicense.Items.Count - 1 > rowCount)
                {
                    dgdLicense.CurrentCell = new DataGridCellInfo(dgdLicense.Items[rowCount], dgdLicense.Columns[colCount + 1]);
                }
                else if (lastColcount == colCount && dgdLicense.Items.Count - 1 == rowCount)
                {
                    btnSave.Focus();
                }
                else if (lastColcount > colCount && dgdLicense.Items.Count - 1 == rowCount)
                {
                    dgdLicense.CurrentCell = new DataGridCellInfo(dgdLicense.Items[rowCount], dgdLicense.Columns[colCount + 1]);
                }
                else
                {
                    MessageBox.Show("??");
                }
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (dgdLicense.Items.Count - 1 > rowCount)
                {
                    dgdLicense.SelectedIndex = rowCount + 1;
                    dgdLicense.CurrentCell = new DataGridCellInfo(dgdLicense.Items[rowCount + 1], dgdLicense.Columns[colCount]);
                }
                else if (dgdLicense.Items.Count - 1 == rowCount)
                {
                    if (lastColcount > colCount)
                    {
                        dgdLicense.SelectedIndex = 0;
                        dgdLicense.CurrentCell = new DataGridCellInfo(dgdLicense.Items[0], dgdLicense.Columns[colCount + 1]);
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (rowCount > 0)
                {
                    dgdLicense.SelectedIndex = rowCount - 1;
                    dgdLicense.CurrentCell = new DataGridCellInfo(dgdLicense.Items[rowCount - 1], dgdLicense.Columns[colCount]);
                }
            }
            else if (e.Key == Key.Left)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (colCount > 0)
                {
                    dgdLicense.CurrentCell = new DataGridCellInfo(dgdLicense.Items[rowCount], dgdLicense.Columns[colCount - 1]);
                }
            }
            else if (e.Key == Key.Right)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (lastColcount > colCount)
                {
                    dgdLicense.CurrentCell = new DataGridCellInfo(dgdLicense.Items[rowCount], dgdLicense.Columns[colCount + 1]);
                }
                else if (lastColcount == colCount)
                {
                    if (dgdLicense.Items.Count - 1 > rowCount)
                    {
                        dgdLicense.SelectedIndex = rowCount + 1;
                        dgdLicense.CurrentCell = new DataGridCellInfo(dgdLicense.Items[rowCount + 1], dgdLicense.Columns[startColcount]);
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
        }

        private void DataGridLicense_KeyUp(object sender, KeyEventArgs e)
        {
            Lib.Instance.DataGridINControlFocus(sender, e);
        }

        private void DataGridLicense_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Lib.Instance.DataGridINBothByMouseUP(sender, e);
        }

        private void DataGridLicense_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                DataGridCell cell = sender as DataGridCell;
                cell.IsEditing = true;
            }
        }


        #endregion

        #region dgdPreviousRecord 수정, 이동 이벤트
        private void DataGridPreviousRecord_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                DataGridPreviousRecord_KeyDown(sender, e);
            }
        }

        private void DataGridPreviousRecord_KeyDown(object sender, KeyEventArgs e)
        {
            var gtePersonBasicPreviousRecord = dgdPreviousRecord.CurrentItem as Gte_PersonBasic_U_PreviousRecord_CodeView;
            int rowCount = dgdPreviousRecord.Items.IndexOf(dgdPreviousRecord.CurrentItem);
            int colCount = dgdPreviousRecord.Columns.IndexOf(dgdPreviousRecord.CurrentCell.Column);
            int lastColcount = 5;
            int startColcount = 0;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (lastColcount == colCount && dgdPreviousRecord.Items.Count - 1 > rowCount)
                {
                    dgdPreviousRecord.SelectedIndex = rowCount + 1;
                    dgdPreviousRecord.CurrentCell = new DataGridCellInfo(dgdPreviousRecord.Items[rowCount + 1], dgdPreviousRecord.Columns[startColcount]);
                }
                else if (lastColcount > colCount && dgdPreviousRecord.Items.Count - 1 > rowCount)
                {
                    dgdPreviousRecord.CurrentCell = new DataGridCellInfo(dgdPreviousRecord.Items[rowCount], dgdPreviousRecord.Columns[colCount + 1]);
                }
                else if (lastColcount == colCount && dgdPreviousRecord.Items.Count - 1 == rowCount)
                {
                    btnSave.Focus();
                }
                else if (lastColcount > colCount && dgdPreviousRecord.Items.Count - 1 == rowCount)
                {
                    dgdPreviousRecord.CurrentCell = new DataGridCellInfo(dgdPreviousRecord.Items[rowCount], dgdPreviousRecord.Columns[colCount + 1]);
                }
                else
                {
                    MessageBox.Show("??");
                }
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (dgdPreviousRecord.Items.Count - 1 > rowCount)
                {
                    dgdPreviousRecord.SelectedIndex = rowCount + 1;
                    dgdPreviousRecord.CurrentCell = new DataGridCellInfo(dgdPreviousRecord.Items[rowCount + 1], dgdPreviousRecord.Columns[colCount]);
                }
                else if (dgdPreviousRecord.Items.Count - 1 == rowCount)
                {
                    if (lastColcount > colCount)
                    {
                        dgdPreviousRecord.SelectedIndex = 0;
                        dgdPreviousRecord.CurrentCell = new DataGridCellInfo(dgdPreviousRecord.Items[0], dgdPreviousRecord.Columns[colCount + 1]);
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (rowCount > 0)
                {
                    dgdPreviousRecord.SelectedIndex = rowCount - 1;
                    dgdPreviousRecord.CurrentCell = new DataGridCellInfo(dgdPreviousRecord.Items[rowCount - 1], dgdPreviousRecord.Columns[colCount]);
                }
            }
            else if (e.Key == Key.Left)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (colCount > 0)
                {
                    dgdPreviousRecord.CurrentCell = new DataGridCellInfo(dgdPreviousRecord.Items[rowCount], dgdPreviousRecord.Columns[colCount - 1]);
                }
            }
            else if (e.Key == Key.Right)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (lastColcount > colCount)
                {
                    dgdPreviousRecord.CurrentCell = new DataGridCellInfo(dgdPreviousRecord.Items[rowCount], dgdPreviousRecord.Columns[colCount + 1]);
                }
                else if (lastColcount == colCount)
                {
                    if (dgdPreviousRecord.Items.Count - 1 > rowCount)
                    {
                        dgdPreviousRecord.SelectedIndex = rowCount + 1;
                        dgdPreviousRecord.CurrentCell = new DataGridCellInfo(dgdPreviousRecord.Items[rowCount + 1], dgdPreviousRecord.Columns[startColcount]);
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
        }

        private void DataGridPreviousRecord_KeyUp(object sender, KeyEventArgs e)
        {
            Lib.Instance.DataGridINControlFocus(sender, e);
        }

        private void DataGridPreviousRecord_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Lib.Instance.DataGridINBothByMouseUP(sender, e);
        }

        private void DataGridPreviousRecord_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                DataGridCell cell = sender as DataGridCell;
                cell.IsEditing = true;
            }
        }


        #endregion

        #region dgdChanges 수정, 이동 이벤트
        private void DataGridChanges_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                DataGridChanges_KeyDown(sender, e);
            }
        }

        private void DataGridChanges_KeyDown(object sender, KeyEventArgs e)
        {
            var gtePersonBasicChanges = dgdChanges.CurrentItem as Gte_PersonBasic_U_Changes_CodeView;
            int rowCount = dgdChanges.Items.IndexOf(dgdChanges.CurrentItem);
            int colCount = dgdChanges.Columns.IndexOf(dgdChanges.CurrentCell.Column);
            int lastColcount = 8;
            int startColcount = 0;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (lastColcount == colCount && dgdChanges.Items.Count - 1 > rowCount)
                {
                    dgdChanges.SelectedIndex = rowCount + 1;
                    dgdChanges.CurrentCell = new DataGridCellInfo(dgdChanges.Items[rowCount + 1], dgdChanges.Columns[startColcount]);
                }
                else if (lastColcount > colCount && dgdChanges.Items.Count - 1 > rowCount)
                {
                    dgdChanges.CurrentCell = new DataGridCellInfo(dgdChanges.Items[rowCount], dgdChanges.Columns[colCount + 1]);
                }
                else if (lastColcount == colCount && dgdChanges.Items.Count - 1 == rowCount)
                {
                    btnSave.Focus();
                }
                else if (lastColcount > colCount && dgdChanges.Items.Count - 1 == rowCount)
                {
                    dgdChanges.CurrentCell = new DataGridCellInfo(dgdChanges.Items[rowCount], dgdChanges.Columns[colCount + 1]);
                }
                else
                {
                    MessageBox.Show("??");
                }
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (dgdChanges.Items.Count - 1 > rowCount)
                {
                    dgdChanges.SelectedIndex = rowCount + 1;
                    dgdChanges.CurrentCell = new DataGridCellInfo(dgdChanges.Items[rowCount + 1], dgdChanges.Columns[colCount]);
                }
                else if (dgdChanges.Items.Count - 1 == rowCount)
                {
                    if (lastColcount > colCount)
                    {
                        dgdChanges.SelectedIndex = 0;
                        dgdChanges.CurrentCell = new DataGridCellInfo(dgdChanges.Items[0], dgdChanges.Columns[colCount + 1]);
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (rowCount > 0)
                {
                    dgdChanges.SelectedIndex = rowCount - 1;
                    dgdChanges.CurrentCell = new DataGridCellInfo(dgdChanges.Items[rowCount - 1], dgdChanges.Columns[colCount]);
                }
            }
            else if (e.Key == Key.Left)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (colCount > 0)
                {
                    dgdChanges.CurrentCell = new DataGridCellInfo(dgdChanges.Items[rowCount], dgdChanges.Columns[colCount - 1]);
                }
            }
            else if (e.Key == Key.Right)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (lastColcount > colCount)
                {
                    dgdChanges.CurrentCell = new DataGridCellInfo(dgdChanges.Items[rowCount], dgdChanges.Columns[colCount + 1]);
                }
                else if (lastColcount == colCount)
                {
                    if (dgdChanges.Items.Count - 1 > rowCount)
                    {
                        dgdChanges.SelectedIndex = rowCount + 1;
                        dgdChanges.CurrentCell = new DataGridCellInfo(dgdChanges.Items[rowCount + 1], dgdChanges.Columns[startColcount]);
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
        }

        private void DataGridChanges_KeyUp(object sender, KeyEventArgs e)
        {
            Lib.Instance.DataGridINControlFocus(sender, e);
        }

        private void DataGridChanges_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Lib.Instance.DataGridINBothByMouseUP(sender, e);
        }

        private void DataGridChanges_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                DataGridCell cell = sender as DataGridCell;
                cell.IsEditing = true;
            }
        }


        #endregion

        #region dgdReference 수정, 이동 이벤트
        private void DataGridReference_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                DataGridReference_KeyDown(sender, e);
            }
        }

        private void DataGridReference_KeyDown(object sender, KeyEventArgs e)
        {
            var gtePersonBasicReference = dgdReference.CurrentItem as Gte_PersonBasic_U_Reference_CodeView;
            int rowCount = dgdReference.Items.IndexOf(dgdReference.CurrentItem);
            int colCount = dgdReference.Columns.IndexOf(dgdReference.CurrentCell.Column);
            int lastColcount = 4;
            int startColcount = 0;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (lastColcount == colCount && dgdReference.Items.Count - 1 > rowCount)
                {
                    dgdReference.SelectedIndex = rowCount + 1;
                    dgdReference.CurrentCell = new DataGridCellInfo(dgdReference.Items[rowCount + 1], dgdReference.Columns[startColcount]);
                }
                else if (lastColcount > colCount && dgdReference.Items.Count - 1 > rowCount)
                {
                    dgdReference.CurrentCell = new DataGridCellInfo(dgdReference.Items[rowCount], dgdReference.Columns[colCount + 1]);
                }
                else if (lastColcount == colCount && dgdReference.Items.Count - 1 == rowCount)
                {
                    btnSave.Focus();
                }
                else if (lastColcount > colCount && dgdReference.Items.Count - 1 == rowCount)
                {
                    dgdReference.CurrentCell = new DataGridCellInfo(dgdReference.Items[rowCount], dgdReference.Columns[colCount + 1]);
                }
                else
                {
                    MessageBox.Show("??");
                }
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (dgdReference.Items.Count - 1 > rowCount)
                {
                    dgdReference.SelectedIndex = rowCount + 1;
                    dgdReference.CurrentCell = new DataGridCellInfo(dgdReference.Items[rowCount + 1], dgdReference.Columns[colCount]);
                }
                else if (dgdReference.Items.Count - 1 == rowCount)
                {
                    if (lastColcount > colCount)
                    {
                        dgdReference.SelectedIndex = 0;
                        dgdReference.CurrentCell = new DataGridCellInfo(dgdReference.Items[0], dgdReference.Columns[colCount + 1]);
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (rowCount > 0)
                {
                    dgdReference.SelectedIndex = rowCount - 1;
                    dgdReference.CurrentCell = new DataGridCellInfo(dgdReference.Items[rowCount - 1], dgdReference.Columns[colCount]);
                }
            }
            else if (e.Key == Key.Left)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (colCount > 0)
                {
                    dgdReference.CurrentCell = new DataGridCellInfo(dgdReference.Items[rowCount], dgdReference.Columns[colCount - 1]);
                }
            }
            else if (e.Key == Key.Right)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (lastColcount > colCount)
                {
                    dgdReference.CurrentCell = new DataGridCellInfo(dgdReference.Items[rowCount], dgdReference.Columns[colCount + 1]);
                }
                else if (lastColcount == colCount)
                {
                    if (dgdReference.Items.Count - 1 > rowCount)
                    {
                        dgdReference.SelectedIndex = rowCount + 1;
                        dgdReference.CurrentCell = new DataGridCellInfo(dgdReference.Items[rowCount + 1], dgdReference.Columns[startColcount]);
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
        }

        private void DataGridReference_KeyUp(object sender, KeyEventArgs e)
        {
            Lib.Instance.DataGridINControlFocus(sender, e);
        }

        private void DataGridReference_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Lib.Instance.DataGridINBothByMouseUP(sender, e);
        }

        private void DataGridReference_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                DataGridCell cell = sender as DataGridCell;
                cell.IsEditing = true;
            }
        }
        #endregion

        #region 이미지 2019-12-09 추가(가빈씨꺼 참고)

        //이미지 셋팅
        private BitmapImage SetImage(string ImageName, string FolderName)
        {
            BitmapImage bit = null;             //비트맵 변수를 선언
            _ftp = new FTP_EX(FTP_ADDRESS, FTP_ID, FTP_PASS);       //FTP 주소 값을 대입

            if (_ftp == null)           //null이면 
            {
                return null;            //null변환
            }

            //비트맵으로 이미지 보여주기 
            bit = _ftp.DrawingImageByByte(FTP_ADDRESS + '/' + FolderName + '/' + ImageName + "");   //주소값 / 폴더명 / 이미지 이름(경로)

            return bit;
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

        //삭제할 파일을 삭제리스트에 올린다.
        private void FileDeleteAndTextBoxEmpty(TextBox txt)
        {
            if (strFlag.Equals("U"))
            {
                // 파일이름, 파일경로
                string[] strFtp = { txt.Text, txt.Tag != null ? txt.Tag.ToString() : "" };

                deleteListFtpFile.Add(strFtp);
            }

            txt.Text = "";
            txt.Tag = "";
        }

        // FTP_Upload_TextBox - 파일 경로, 이름 텍스트박스에 올림 + 리스트에 ADD
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

                        Bitmap image = new Bitmap(ImageFilePath + ImageFileName);
                        ImageSajinImage.Source = BitmapToImageSource(image);

                        string[] strTemp = new string[] { ImageFileName, ImageFilePath.ToString() };
                        listFtpFile.Add(strTemp);
                    }
                }
            }
        }

        // 비트맵을 비트맵 이미지로 형태변환시키기.<0823 허윤구> 
        //BitmapImage BitmapToImageSource(Bitmap bitmap)
        //{
        //    using (MemoryStream memory = new MemoryStream())
        //    {
        //        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        //        memory.Position = 0;
        //        BitmapImage bitmapimage = new BitmapImage();
        //        bitmapimage.BeginInit();
        //        bitmapimage.StreamSource = memory;
        //        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        //        bitmapimage.EndInit();

        //        return bitmapimage;
        //    }
        //}


        #endregion 이미지


        #region 이미지 업로드, 삭제
        //업로드 파일 선택 버튼 클릭
        private void ButtonImageAdd_Click(object sender, RoutedEventArgs e)
        {
            if(!TextBoxImage.Text.Equals(string.Empty) && strFlag.Equals("U"))
            {
                MessageBox.Show("이미지가 존재합니다.");
                return;
            }
            else
            {
                AddFtp = true;
                FTP_Upload_TextBox(TextBoxImage);
                //TextBox tb = Ftp_Upload_TextBox();

                //if( tb != null)
                //{
                //    TextBoxImage.Text = tb.Text; //명칭
                //    TextBoxImage.Tag = tb.Tag; //경로
                //}

                //if(TextBoxImage.Tag == null)
                //{
                //    MessageBox.Show(TextBoxImage.Text);
                //    TextBoxImage.Text = "";
                //}
                //else
                //{
                //    strImagePath = "/ImageData/McCode";
                //}


            }
        }

        //이미지 삭제 버튼 클릭 (폴더 삭제 포함)
        private void ButtonImageDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgresult = MessageBox.Show("파일을 삭제 하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo);
            if(msgresult == MessageBoxResult.Yes)
            {
                FileDeleteAndTextBoxEmpty(TextBoxImage);
                ImageSajinImage.Source = null;

                if (strFlag.Equals("U") && ExistFtp == true)
                {


                    //if (FTP_RemoveDir(TextBoxPerson.Tag.ToString()))
                    //{
                    DelFtp = true;
                    //}
                }

                strDelFileName = TextBoxPerson.Tag.ToString();
                TextBoxImage.Text = "";
                TextBoxImage.Tag = null;

            }
        }


        //FTP 업로드 파일체크 및 경로, 파일이름 표시
        private TextBox Ftp_Upload_TextBox()
        {
            TextBox tb = new TextBox();

            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.pcx, *.pdf) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.pcx; *.pdf | All Files|*.*";

            Nullable<bool> result = OFD.ShowDialog();

            if(result == true)
            {
                strFullpath = OFD.FileName;

                string ImageFileName = OFD.SafeFileName; //파일명
                string ImageFilePath = string.Empty; //경로

                ImageFilePath = strFullpath.Replace(ImageFileName, "");

                StreamReader sr = new StreamReader(OFD.FileName);
                long FileSize = sr.BaseStream.Length;

                if(sr.BaseStream.Length > (2048 * 1000))
                {
                    //업로드 파일 사이즈범위 초과
                    //MessageBox.Show("이미지의 파일사이즈가 2M byte를 초과하였습니다.");
                    sr.Close();
                    tb.Text = "파일사이즈초과";
                    //return;
                }
                else
                {
                    tb.Text = ImageFileName;
                    tb.Tag = ImageFilePath;

                    Bitmap image = new Bitmap(ImageFilePath + ImageFileName);
                    ImageSajinImage.Source = BitmapToImageSource(image);

                }
            }
            return tb;
        }

        //비트맵을 비트맵 이미지로 변환
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

        //파일 저장
        private void FTP_Save_File(string SaveName, string FileName, string FullPath)
        {
            UploadFileInfo fileInfo_up = new UploadFileInfo();
            fileInfo_up.Filename = FileName;
            fileInfo_up.Type = FtpFileType.File;

            string[] fileListSimple;
            string[] fileListDetail;

            fileListSimple = _ftp.directoryListSimple("", Encoding.Default);
            fileListDetail = _ftp.directoryListDetailed("", Encoding.Default);

            //기존 폴더 확인
            bool MakeFolder = false;
            for(int i = 0; i< fileListSimple.Length; i++)
            {
                if(fileListSimple[i] == SaveName)
                {
                    MakeFolder = true;
                    break;
                }
            }

            if(MakeFolder == false) //폴더 생성 실패시
            {
                if(_ftp.createDirectory(SaveName) == false)
                {
                    MessageBox.Show("업로드 폴더를 생성할 수 없습니다.");
                    return;
                }
            }

            //생성한 폴더에 파일 업로드
            string str_remotepath = SaveName + "/";
            fileInfo_up.Filepath = str_remotepath;
            str_remotepath += FileName;

            if(_ftp.upload(str_remotepath, FullPath + FileName) == false)
            {
                MessageBox.Show("파일 업로드 실패");
                return;
            }
        }

        #endregion

        //한글명 KeyDown 플러스파인더
        private void TextBoxName_KeyDown(object sender, KeyEventArgs e)
        {
            if(strFlag == "U" | strFlag == "I" )
            {
                if (e.Key == Key.Enter)
                {
                    MainWindow.pf.ReturnCode(TextBoxName, 2, "");

                    if ((TextBoxName.Text != "" | TextBoxName.Text != null
                        ) && TextBoxName.Tag != null)
                    {
                        FillGridAdd();
                    }
                }
                
              
            }
            
        }

        ////폴더 삭제
        //private bool FTP_RemoveDir(string strSaveName)
        //{
        //    string[] fileListSimple;
        //    string[] fileListDetail;

        //    fileListSimple = _ftp.directoryListSimple("", Encoding.Default);
        //    fileListDetail = _ftp.directoryListDetailed("", Encoding.Default);

        //    bool tf_ExistInspectionID = MakeFileInfoList(fileListSimple, fileListDetail, strSaveName);

        //    if(tf_ExistInspectionID == true)
        //    {
        //        if(_ftp.removeDir(strSaveName) == true)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //업로드 폴더 확인
        private bool MakeFileInfoList(string[] simple, string[] detail, string str_InspectID)
        {
            bool tf_return = false;
            foreach (string filename in simple)
            {
                foreach (string info in detail)
                {
                    if (info.Contains(filename) == true)
                    {
                        if (MakeFileInfoList(filename, info, str_InspectID) == true)
                        {
                            tf_return = true;
                        }
                    }
                }
            }
            return tf_return;
        }

        private bool MakeFileInfoList(string simple, string detail, string strCompare)
        {
            UploadFileInfo info = new UploadFileInfo();
            info.Filename = simple;
            info.Filepath = detail;

            if (simple.Length > 0)
            {
                string[] tokens = detail.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                string name = tokens[3].ToString();
                string permissions = tokens[2].ToString();

                if (permissions.Contains("D") == true)
                {
                    info.Type = FtpFileType.DIR;
                }
                else
                {
                    info.Type = FtpFileType.File;
                }

                if (info.Type == FtpFileType.File)
                {
                    info.Size = Convert.ToInt64(detail.Substring(17, detail.LastIndexOf(simple) - 17).Trim());
                }

                _listFileInfo.Add(info);

                if (string.Compare(simple, strCompare, false) == 0)
                    return true;
            }

            return false;
        }

        //FTP 파일 다운로드
        private void FTP_DownLoadFile(string strFilePath)
        {
            string[] fileListSimple;
            string[] fileListDetail;

            fileListSimple = _ftp.directoryListSimple("", Encoding.Default);
            fileListDetail = _ftp.directoryListDetailed("", Encoding.Default);

            bool ExistFile = false;
            ExistFile = MakeFileInfoList(fileListSimple, fileListDetail, strFilePath.Split('/')[3].Trim());

            int fileLength = _listFileInfo.Count;

            if(ExistFile)
            {
                string str_remotepath = string.Empty;
                string str_localpath = string.Empty;

                str_remotepath = strFilePath.ToString();
                str_localpath = LOCAL_DOWN_PATH + "\\" + strFilePath.Substring(strFilePath.LastIndexOf("/")).ToString();

                DirectoryInfo DI = new DirectoryInfo(LOCAL_DOWN_PATH);

                if(DI.Exists)
                {
                    DI.Create();
                }

                FileInfo file = new FileInfo(str_localpath);

                if(file.Exists)
                {
                    file.Delete();
                }

                _ftp.download(str_remotepath.Substring(str_remotepath.Substring(0, str_remotepath.LastIndexOf("/")).LastIndexOf("/")), str_localpath);

                ProcessStartInfo proc = new ProcessStartInfo(str_localpath);
                proc.UseShellExecute = true;
                Process.Start(proc);

            }
        }

        private void TextBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ImageSajinImage.Source = null;

            if(gtePersonBasicInsa != null)
            {
                this.DataContext = gtePersonBasicInsa;

                bool MakeFolder = false;
                if( !TextBoxImage.Text.Replace(" ", "").Equals(""))
                {
                    string[] fileListSimple;
                    string[] fileListDetail;

                    fileListSimple = _ftp.directoryListSimple("", Encoding.Default);
                    fileListDetail = _ftp.directoryListDetailed("", Encoding.Default);

                    //기존 폴더 확인
                    for(int i= 0;  i < fileListSimple.Length; i++)
                    {
                        MakeFolder = true;
                        break;
                    }

                    if(MakeFolder)
                    {
                        ImageSajinImage.Source = SetImage("/" + "insa" + "/" + TextBoxImage.Text);
                    }
                }
                
            }
        }

        //이미지 Bit로
        private BitmapImage SetImage(string strAttachPath)
        {
            BitmapImage bit = _ftp.DrawingImageByByte(FTP_ADDRESS + strAttachPath + "");
            //image.Source = bit;
            return bit;
        }


        #region 도장 이미지
        private void btnStempUpload_Click(object sender, RoutedEventArgs e)
        {
            if (!txtStemp.Text.Trim().Equals(""))
            {
                MessageBox.Show("이미지가 존재합니다.");
                return;
            }
            else
            {
                FTP_Upload_TextBoxStemp(txtStemp);
            }           
        }

        private void btnStempDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgresult = MessageBox.Show("파일을 삭제 하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo);
            if (msgresult == MessageBoxResult.Yes)
            {
                FileDeleteAndTextBoxEmptyStemp(txtStemp);
            }
        }

        private void btnStempSee_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxPerson.Tag != null && !txtStemp.Text.Trim().Equals(""))
            {
                ImgStemp.Source = SetImageStemp(txtStemp.Text.Trim(), TextBoxPerson.Tag.ToString());
            }
        }

        #region FTP 업로드

        private void FTP_Upload_TextBoxStemp(TextBox textBox)
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

                        Bitmap image = new Bitmap(ImageFilePath + ImageFileName);
                        ImgStemp.Source = BitmapToImageSource(image);

                        string[] strTemp = new string[] { ImageFileName, ImageFilePath.ToString() };
                        listStempFtpFile.Add(strTemp);
                    }
                }
            }
        }
        
        #endregion

        #region FTP 삭제
        //삭제할 파일을 삭제리스트에 올린다.
        private void FileDeleteAndTextBoxEmptyStemp(TextBox txt)
        {
            if (strFlag.Equals("U"))
            {
                // 파일이름, 파일경로
                string[] strFtp = { txt.Text, txt.Tag != null ? txt.Tag.ToString() : "" };

                deleteListStempFtpFile.Add(strFtp);
            }

            txt.Text = "";
            txt.Tag = "";
        }
        #endregion

        //파일 삭제
        private bool FTP_RemoveFileStemp(string strSaveName)
        {
            _ftp = new FTP_EX("ftp://" + LoadINI.FileSvr + ":" + LoadINI.FTPPort + LoadINI.FtpImagePath + "/Approval/Stemp", FTP_ID, FTP_PASS);
            if (_ftp.delete(strSaveName) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 이미지 세팅
        private BitmapImage SetImageStemp(string ImageName, string FolderName)
        {
            BitmapImage bit = null;             //비트맵 변수를 선언
            _ftp = new FTP_EX("ftp://" + LoadINI.FileSvr + ":" + LoadINI.FTPPort + LoadINI.FtpImagePath + "/Approval/Stemp", FTP_ID, FTP_PASS);       //FTP 주소 값을 대입

            if (_ftp == null)           //null이면 
            {
                return null;            //null변환
            }

            //비트맵으로 이미지 보여주기 
            bit = _ftp.DrawingImageByByte("ftp://" + LoadINI.FileSvr + ":" + LoadINI.FTPPort + LoadINI.FtpImagePath + "/Approval/Stemp" + '/' + FolderName + '/' + ImageName + "");   //주소값 / 폴더명 / 이미지 이름(경로)

            return bit;
        }

        #region 파일 저장 메서드
        // 파일 저장하기.
        private void FTP_Save_FileStemp(List<string[]> listStrArrayFileInfo, string MakeFolderName)
        {
            _ftp = new FTP_EX("ftp://" + LoadINI.FileSvr + ":" + LoadINI.FTPPort + LoadINI.FtpImagePath + "/Approval/Stemp", FTP_ID, FTP_PASS);

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
        #endregion

        #endregion

       

        // 20200107 둘리
        class GTE_JustPerson
        {
            public int Num { get; set; }
            public string Name { get; set; }
            public string PersonID { get; set; }
        }

        
    }

    #region CodeView
    class Gte_PersonBasic_U_Insa_CodeView
{
    public override string ToString()
    {
        return (this.ReportAllProperties());
    }

    public string PersonID { get; set; }
    public string Name { get; set; }
    public string EngName { get; set; }
    public string HanjaName { get; set; }
    public string StartDate { get; set; }
    public string RegistID { get; set; }
    public string BirthDay { get; set; }
    public string Sabun { get; set; }
    public string DepartID { get; set; }
    public string TeamID { get; set; }
    public string ResablyID { get; set; }
    public string SexGbn { get; set; }
    public string SajinImage { get; set; }
    public string SajinImagePath { get; set; }
    public string HomeHostName { get; set; }
    public string HomeHostRel { get; set; }
    public string HomeHostJob { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string AddressJiBun1 { get; set; }
    public string AddressJiBun2 { get; set; }
    public string Phone { get; set; }
    public string HandPhone { get; set; }
    public string Email { get; set; }
    public string Fax { get; set; }
    public string MainJob { get; set; }
    public string MarryYN { get; set; }
    public string BodyHeight { get; set; }
    public string BodyWeight { get; set; }
    public string BodyBloodType { get; set; }
    public string Hobby { get; set; }
    public string Religon { get; set; }
    public string Transport { get; set; }
    public string HighSchoolName { get; set; }
    public string HighSchoolDepart { get; set; }
    public string HighSchoolFinishYN { get; set; }
    public string CollegeName { get; set; }
    public string CollegeDepart { get; set; }
    public string CollegeFinishYN { get; set; }
    public string UniverseName { get; set; }
    public string UniverseDepart { get; set; }
    public string UniverseFinishYN { get; set; }
    public string BigUniverse1Name { get; set; }
    public string BigUniverse1Depart { get; set; }
    public string BigUniverse1FinishYN { get; set; }
    public string BigUniverse2Name { get; set; }
    public string BigUniverse2Depart { get; set; }
    public string BigUniverse2FinishYN { get; set; }
    public string militaryNotComments { get; set; }
    public string militaryLevel { get; set; }
    public string militaryPeriod { get; set; }
    public string militaryNo { get; set; }
    public string militaryBul { get; set; }
    public string militaryBungGa { get; set; }
    public string militaryTRYear { get; set; }
    public string militaryTRWorkYear { get; set; }
    public string militaryTRHealthLevel { get; set; }
    public string militaryTRDate { get; set; }
    public string militaryTRChageComments { get; set; }
    public string retireDate { get; set; }
    public string retireReason { get; set; }
    public string retireChoriGubun { get; set; }
    public string retireWorkYear { get; set; }
    public string retireAmount { get; set; }
    public string BasicPay { get; set; }
    public string TimePay { get; set; }
    public string CreateDate { get; set; }
    public string CreateUserID { get; set; }
    public string LastUpdateDate { get; set; }
    public string LastUpdateUserID { get; set; }

    public string StempFileName { get; set; }

}

    class Gte_PersonBasic_U_Home_CodeView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public string PersonID { get; set; }
        public string Seq { get; set; }
        public string Relation { get; set; }
        public string Name { get; set; }
        public string BirthDay { get; set; }
        public string Job { get; set; }
        public string LivingTogether { get; set; }
        public string CreateDate { get; set; }
        public string CreateUserID { get; set; }

    }

    class Gte_PersonBasic_U_License_CodeView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public string PersonID { get; set; }
        public string Seq { get; set; }
        public string LicenseName { get; set; }
        public string LicenseDate { get; set; }
        public string PublishingOffice { get; set; }
        public string LicenseNumber { get; set; }
        public string CreateDate { get; set; }
        public string CreateUserID { get; set; }

    }

    class Gte_PersonBasic_U_PreviousRecord_CodeView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public string PersonID { get; set; }
        public string Seq { get; set; }
        public string Workdate { get; set; }
        public string CompanyName { get; set; }
        public string JobGrade { get; set; }
        public string Business { get; set; }
        public string Salary { get; set; }
        public string CreateDate { get; set; }
        public string CreateUserID { get; set; }

    }

    class Gte_PersonBasic_U_Changes_CodeView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public string PersonID { get; set; }
        public string Seq { get; set; }
        public string AppointmentDate { get; set; }
        public string Department { get; set; }
        public string JobGrade { get; set; }
        public string ChangeWork { get; set; }
        public string DepartmentManager { get; set; }
        public string SalaryChangeDate { get; set; }
        public string SalaryClass { get; set; }
        public string Salary { get; set; }
        public string CreateDate { get; set; }
        public string CreateUserID { get; set; }

    }

    class Gte_PersonBasic_U_Reference_CodeView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public string PersonID { get; set; }
        public string Seq { get; set; }
        public string Guarantor { get; set; }
        public string Relation { get; set; }
        public string RRN { get; set; }
        public string Address { get; set; }
        public string CreateDate { get; set; }
        public string CreateUserID { get; set; }

    }
    #endregion
}
