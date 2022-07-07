/**************************************************************************************************
   '** 작성자    : 정승학
   '** 내용      : 일 근태 처리
   '** 생성일자  : 2019.09.20
   '**------------------------------------------------------------------------------------------------
   ''*************************************************************************************************
   ' 변경일자  , 변경자, 요청자    , 요구사항ID  , 요청 및 작업내용
   '**************************************************************************************************
   ' 2019.00.00  
**************************************************************************************************/

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
using System.Windows.Threading;
using WizMes_Alpha_JA.PopUP;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// Win_mtr_Outware_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class frm_Gte_DailyGte_U : UserControl
    {
        string strFlag = string.Empty;
        string delFlag = string.Empty;

        bool editing = false;

        int rowNum = 0;

        CompareOutware beforeOutware = null;
        List<Gte_DailyGte_U_CodeView> lstDeleteData = new List<Gte_DailyGte_U_CodeView>();
        Gte_DailyGte_U_CodeView Outware = new Gte_DailyGte_U_CodeView();


        int beforeIndex = 0;

        public frm_Gte_DailyGte_U()
        {
            InitializeComponent();
        }

        // 폼 로드
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            SetComboBox();

            chkDateSrh.IsChecked = true;
            dtpSDateSrh.SelectedDate = DateTime.Today;
            dtpEDateSrh.SelectedDate = DateTime.Today;

            btnAdd.IsEnabled = false;
            btnDelete.IsEnabled = false;

            buttonbox();
        }

        private void buttonbox()
        {
            txtPersonIDSrh.IsEnabled = false;
            btnPersonIDSrh.IsEnabled = false;
            TextBoxDepartID.IsEnabled = false;
            ButtonDepartID.IsEnabled = false;

        }

        #region SetComboBox 콤보박스 세팅

        private void SetComboBox()
        {
            //// 수정구분 
            //List<string[]> lstModifyClssGbnID = new List<string[]>();
            //string[] lstModifyClssGbnID1 = new string[] { "", "N" };
            //string[] lstModifyClssGbnID2 = new string[] { "1", "Y" };
            //lstModifyClssGbnID.Add(lstModifyClssGbnID1);
            //lstModifyClssGbnID.Add(lstModifyClssGbnID2);

            //ObservableCollection<CodeView> ovcModifyClssGbnID = ComboBoxUtil.Instance.Direct_SetComboBox(lstModifyClssGbnID);
            //this.cboModifyClssGbnID.ItemsSource = ovcModifyClssGbnID;
            //this.cboModifyClssGbnID.DisplayMemberPath = "code_name";
            //this.cboModifyClssGbnID.SelectedValuePath = "code_id";
        }

        #endregion

        #region 추가, 수정 / 취소, 완료

        // 추가, 수정 시
        private void SaveUpdateMode()
        {
            if (strFlag.Trim().Equals("I"))
            {
                tbkMsg.Text = "자료 추가 중";
            }
            else
            {
                tbkMsg.Text = "자료 수정 중";
            }
            lblMsg.Visibility = Visibility.Visible;

            // Header 검색조건
            grdDateSrh1.IsEnabled = false;
            grdDateSrh2.IsEnabled = false;
            grdDateSrhBtn.IsEnabled = false;

            grdArticleSrh.IsEnabled = false;
            grdOutClssSrh.IsEnabled = false;

            // Header 상단 오른쪽
            btnSearch.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            btnExcel.IsEnabled = false;

            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;

            // Content - 항목추가, 항목삭제 버튼
            btnAdd.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }

        // 완료, 취소 시
        private void CompleteCancelMode()
        {
            lblMsg.Visibility = Visibility.Hidden;

            // Header 검색조건
            grdDateSrh1.IsEnabled = true;
            grdDateSrh2.IsEnabled = true;
            grdDateSrhBtn.IsEnabled = true;

            grdArticleSrh.IsEnabled = true;
            grdOutClssSrh.IsEnabled = true;

            // Header 상단 오른쪽
            btnSearch.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnExcel.IsEnabled = true;

            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;

            // Content - 항목추가, 항목삭제 버튼
            btnAdd.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        #endregion

        #region Header 부분 - 검색조건

        // 검색조건
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

        // 전일
        private void btnYesterday_Click(object sender, RoutedEventArgs e)
        {
            dtpSDateSrh.SelectedDate = DateTime.Today.AddDays(-1);
            dtpEDateSrh.SelectedDate = DateTime.Today.AddDays(-1);
        }
        // 금일
        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            dtpSDateSrh.SelectedDate = DateTime.Today;
            dtpEDateSrh.SelectedDate = DateTime.Today;
        }
        // 전월
        private void btnLastMonth_Click(object sender, RoutedEventArgs e)
        {
            dtpSDateSrh.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[0];
            dtpEDateSrh.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[1];
        }
        // 금월
        private void btnThisMonth_Click(object sender, RoutedEventArgs e)
        {
            dtpSDateSrh.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[0];
            dtpEDateSrh.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[1];
        }

        // 검색조건 - 사원
        private void lblPersonIDSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CheckBoxPersonID.IsChecked == true)
            {
                CheckBoxPersonID.IsChecked = false;
            }
            else
            {
                CheckBoxPersonID.IsChecked = true;
            }
        }

        //사원 체크박스 체크
        private void CheckBoxPersonID_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxPersonID.IsChecked = true;
            txtPersonIDSrh.IsEnabled = true;
            btnPersonIDSrh.IsEnabled = true;
            txtPersonIDSrh.Focus();
        }
        
        //사원 체크박스 체크해제
        private void CheckBoxPersonID_UnChecked(object sender, RoutedEventArgs e)
        {
            CheckBoxPersonID.IsChecked = false;
            txtPersonIDSrh.IsEnabled = false;
            btnPersonIDSrh.IsEnabled = false;
        }

        //사원 플러스파인더 버튼
        private void txtPersnIDSrh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtPersonIDSrh, 2, "");
                //MainWindow.pf.ReturnCode(txtPersonIDSrh, (int)Defind_CodeFind.DCF_PERSON, "");
            }
        }
        private void btnPersnIDSrh_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtPersonIDSrh, 2, "");
            //MainWindow.pf.ReturnCode(txtPersonIDSrh, (int)Defind_CodeFind.DCF_PERSON, "");
        }


        // 사원
        private void txtPerson_keyDown(object sender, KeyEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                if (e.Key == Key.Enter)
                {
                    TextBox tbx = sender as TextBox;

                    MainWindow.pf.ReturnCode(tbx, 2, "");
                }
            }
        }




        // 사원
        private void lblOutClssSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CheckBoxDepartID.IsChecked == true)
            {
                CheckBoxDepartID.IsChecked = false;
            }
            else
            {
                CheckBoxDepartID.IsChecked = true;
            }
        }
        private void chkOutClssSrh_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxDepartID.IsChecked = true;
            CheckBoxDepartID.IsEnabled = true;
        }
        private void chkOutClssSrh_UnChecked(object sender, RoutedEventArgs e)
        {
            CheckBoxDepartID.IsChecked = false;
            CheckBoxDepartID.IsEnabled = false;
        }


        //부서 
        private void lblDepartIDSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CheckBoxDepartID.IsChecked == true)
            {
                CheckBoxDepartID.IsChecked = false;
            }
            else
            {
                CheckBoxDepartID.IsChecked = true;
            }
        }


        //부서 체크박스 체크
        private void CheckBoxDepartID_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxDepartID.IsChecked = true;
            TextBoxDepartID.IsEnabled = true;
            ButtonDepartID.IsEnabled = true;
            TextBoxDepartID.Focus();

        }

        //부서 체크박스 체크해제
        private void CheckBoxDepartID_UnChecked(object sender, RoutedEventArgs e)
        {
            CheckBoxDepartID.IsChecked = false;
            TextBoxDepartID.IsEnabled = false;
            ButtonDepartID.IsEnabled = false;

        }

        //부서 플러스파인더 버튼
        private void ButtonDepartID_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(TextBoxDepartID, 76, "");


        }

        // 부서로 나와야함
        private void TxtDepartID_Click(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tbx = sender as TextBox;

                MainWindow.pf.ReturnCode(tbx, 76, "");
            }
        }

        #endregion // Header 부분 - 검색조건

        #region Header 부분 - 오른쪽 상단 버튼 이벤트

        // 검색
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            rowNum = 0;

            re_Search(rowNum);
        }

        // 작성
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            lstDeleteData.Clear();

            //if (dgdMain.Items.Count < 1)
            //{
            //    MessageBox.Show("검색을 먼저 해주세요.");
            //    return;
            //}

            strFlag = "U";
            SaveUpdateMode();


            if (dgdMain.SelectedItem == null)
            {
                dgdMain.SelectedIndex = 0;
            }

            // 수정 버튼 눌렀을 때부터, 어떤 행이 수정을 해야 되는지 플래그를 등록하기 위해서 초기값을 사용
            beforeOutware = new CompareOutware();

            var Outware = dgdMain.SelectedItem as Gte_DailyGte_U_CodeView;
            if (Outware != null)
            {
                beforeOutware.setOutware(Outware);
                beforeIndex = dgdMain.SelectedIndex;
            }
        }

        // 닫기
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        // 저장
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (beforeOutware != null)
            {
                var Outware = dgdMain.SelectedItem as Gte_DailyGte_U_CodeView;
                if (Outware != null)
                {
                    if (Outware.strFlag == null || !Outware.strFlag.Trim().Equals("I"))
                    {
                        if (beforeOutware.chkIsUpdate(Outware) == false)
                        {
                            Outware.strFlag = "U";
                        }
                    }
                }
            }

            if (SaveData(strFlag))
            {
                CompleteCancelMode();

                strFlag = string.Empty;

                rowNum = 0;
                re_Search(rowNum);
            }
        }

        // 취소
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            strFlag = string.Empty;
            CompleteCancelMode();

            rowNum = 0;
            re_Search(rowNum);
        }

        // 엑셀
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] lst = new string[2];
            lst[0] = "일 근태 처리";
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

        // 테스트
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion //  Header 부분 - 오른쪽 상단 버튼 이벤트

        #region Content 버튼 - 항목추가, 항목삭제

        // 항목추가
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddRow();

            int endRow = dgdMain.Items.Count - 1;
            dgdMain.SelectedIndex = endRow;

            editing = true;
            dgdMain.Focus();
            dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[endRow], dgdMain.Columns[1]);


        }


        private void AddRow()
        {
            var Outware = new Gte_DailyGte_U_CodeView()
            {
                Num = dgdMain.Items.Count + 1,

                GteDay = "",
                DoWeek = "",
                PersonID = "",
                Name = "",
                WorkTimeID = "",

                WorkTimeName = "",
                WorkOffGbnID = "",
                GteComments = "",
                InOfficeTime = "",
                GoOutTime = "",

                GoInTime = "",
                OffOfficeTime = "",
                ModifyClss = "",
                BasicWorkTime = "",
                ExtendWorkTime = "",

                NightWorkTime = "",
                HoliBasicWorkTime = "",
                HoliExtendWorkTime = "",
                HoliNightWorkTime = "",
                LatePeriodTime = "",

                EalyLeavePeriodTime = "",
                GoOutPeriodTime = "",

                Comments = "",
                CreateDate = "",

                strFlag = "I"
            };

            dgdMain.Items.Add(Outware);
        }


        // 항목삭제
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                var Outware = dgdMain.SelectedItem as Gte_DailyGte_U_CodeView;
                int deleteIndex = dgdMain.SelectedIndex;

                if (Outware != null)
                {
                    delFlag = "D";

                    dgdMain.Items.Remove(Outware);

                    if (Outware.strFlag == null || !Outware.strFlag.Trim().Equals("I"))
                    {
                        lstDeleteData.Add(Outware);
                    }

                    // 이제 beforeIndex 를 건드려 봅시다.
                    if (dgdMain.Items.Count > 0)
                    {
                        dgdMain.SelectedIndex = deleteIndex - 1 < 0 ? 0 : deleteIndex - 1;

                        beforeIndex = dgdMain.SelectedIndex;

                        var Out = dgdMain.Items[beforeIndex] as Gte_DailyGte_U_CodeView;

                        if (Out != null)
                        {
                            beforeOutware.setOutware(Out);
                        }

                    }
                    else
                    {
                        beforeOutware = null;
                    }


                    delFlag = string.Empty;
                }
                else
                {
                    MessageBox.Show("삭제할 데이터를 선택해주세요.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("수정 상태일 때만 삭제가 가능합니다.");
                return;
            }
        }


        #endregion // Content 버튼 - 항목추가, 항목삭제 + AddRow(), 

        #region Content - 데이터 그리드

        // 메인 그리드 선택 이벤트
        private void dgdMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {

                if (!delFlag.Equals("D"))
                {
                    if (beforeOutware != null)
                    {
                        // 임시 값이랑, beforeIndex 값이랑 같은지 체크 > 다르면, 수정이 되도록!!
                        var Outware = dgdMain.Items[beforeIndex] as Gte_DailyGte_U_CodeView;

                        if (Outware != null)
                        {
                            if (Outware.strFlag == null || !Outware.strFlag.Trim().Equals("I"))
                            {
                                if (beforeOutware.chkIsUpdate(Outware) == false)
                                {
                                    Outware.strFlag = "U";
                                }
                            }

                            // 그 다음에 before에 지금 행 정보, 인덱스를 넣도록
                            var Outware2 = dgdMain.SelectedItem as Gte_DailyGte_U_CodeView;

                            if (Outware2 != null)
                            {
                                beforeOutware.setOutware(Outware2);
                                beforeIndex = dgdMain.SelectedIndex;
                            }
                        }
                    }
                }

            }
        }

        // 기간 선택 이벤트
        private void dtpOutDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker dtpSender = sender as DatePicker;
            Gte_DailyGte_U_CodeView OcStuffin = dtpSender.DataContext as Gte_DailyGte_U_CodeView;

            if (OcStuffin != null)
            {
                OcStuffin.OutDate_CV = dtpSender.SelectedDate != null ? dtpSender.SelectedDate.Value.ToString("yyyy-MM-dd") : "";
                OcStuffin.GteDay = OcStuffin.OutDate_CV.Replace("-", "");
            }

        }


        private void dtpOutDate_CalendarClosed(object sender, RoutedEventArgs e)
        {
            editing = true;

            int currRow = dgdMain.Items.IndexOf(dgdMain.CurrentItem);
            dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow], dgdMain.Columns[2]);
        }


        // 근무 키 이벤트
        private void txtArticle_KeyDown(object sender, KeyEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                if (e.Key == Key.Enter)
                {
                    TextBox tbx = sender as TextBox;

                    MainWindow.pf.ReturnCode(tbx, 78, "");
                }
            }
        }


        // 요일
        private void txtDayil_keyDown(object sender, KeyEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                if (e.Key == Key.Enter)
                {
                    TextBox tbx = sender as TextBox;

                    MainWindow.pf.ReturnCode(tbx, 75, "");
                }
            }
        }


        #endregion // Content - 데이터 그리드

        // 엔터 → 다음 셀을 위한 텍스트박스 포커스 이벤트
        private void txtBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (editing == true)
            {
                (sender as TextBox).Focus();

                editing = false;
            }
        }

        #region Content 부분 - 데이터 그리드 키 이벤트

        // 2019.08.27 PreviewKeyDown 는 key 다운과 같은것 같음
        private void DataGird_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
                {
                    DataGird_KeyDown(sender, e);
                }
            }
            catch (Exception ex)
            {

            }
        }


        // KeyDown 이벤트
        private void DataGird_KeyDown(object sender, KeyEventArgs e)
        {
            int currRow = dgdMain.Items.IndexOf(dgdMain.CurrentItem);
            int currCol = dgdMain.Columns.IndexOf(dgdMain.CurrentCell.Column);
            int startCol = 1;
            int endCol = 19;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                editing = true;

                // 마지막 열, 마지막 행 아님
                if (endCol == currCol && dgdMain.Items.Count - 1 > currRow)
                {
                    dgdMain.SelectedIndex = currRow + 1; // 이건 한줄 파란색으로 활성화 된 걸 조정하는 것입니다.
                    dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow + 1], dgdMain.Columns[startCol]);

                } // 마지막 열 아님
                else if (endCol > currCol && dgdMain.Items.Count - 1 >= currRow)
                {
                    dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow], dgdMain.Columns[currCol + 1]);

                } // 마지막 열, 마지막 행
                else if (endCol == currCol && dgdMain.Items.Count - 1 == currRow)
                {
                    //btnSave.Focus();
                    if (MessageBox.Show("새로운 행을 추가하시겠습니까?", "추가 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        AddRow();

                        // 새로 추가된 행에 첫번째를 선택 되도록!!!!
                        currRow++;
                        dgdMain.SelectedIndex = currRow;

                        editing = true;

                        dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow], dgdMain.Columns[1]);
                    }
                }
                else
                {
                    MessageBox.Show("오류");
                }
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                // 마지막 행 아님
                if (dgdMain.Items.Count - 1 > currRow)
                {
                    dgdMain.SelectedIndex = currRow + 1;
                    dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow + 1], dgdMain.Columns[currCol]);
                } // 마지막 행일때
                else if (dgdMain.Items.Count - 1 == currRow)
                {
                    if (endCol > currCol) // 마지막 열이 아닌 경우, 열을 오른쪽으로 이동
                    {
                        //dgdMain.SelectedIndex = 0;
                        dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow], dgdMain.Columns[currCol + 1]);
                    }
                    else
                    {
                        //btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                // 첫행 아님
                if (currRow > 0)
                {
                    dgdMain.SelectedIndex = currRow - 1;
                    dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow - 1], dgdMain.Columns[currCol]);
                } // 첫 행
                else if (dgdMain.Items.Count - 1 == currRow)
                {
                    if (0 < currCol) // 첫 열이 아닌 경우, 열을 왼쪽으로 이동
                    {
                        //dgdMain.SelectedIndex = 0;
                        dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow], dgdMain.Columns[currCol - 1]);
                    }
                    else
                    {
                        //btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Left)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (startCol < currCol)
                {
                    dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow], dgdMain.Columns[currCol - 1]);
                }
                else if (startCol == currCol)
                {
                    if (0 < currRow)
                    {
                        dgdMain.SelectedIndex = currRow - 1;
                        dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow - 1], dgdMain.Columns[endCol]);
                    }
                    else
                    {
                        //btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Right)
            {
                e.Handled = true;
                (sender as DataGridCell).IsEditing = false;

                if (endCol > currCol)
                {

                    dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow], dgdMain.Columns[currCol + 1]);
                }
                else if (endCol == currCol)
                {
                    if (dgdMain.Items.Count - 1 > currRow)
                    {
                        dgdMain.SelectedIndex = currRow + 1;
                        dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow + 1], dgdMain.Columns[startCol]);
                    }
                    else
                    {
                        //btnSave.Focus();
                    }
                }
            }
            else if (e.Key == Key.Delete)
            {
                if (MessageBox.Show("현재 행을 삭제하시겠습니까?", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    btnDelete_Click(null, null);
                }
            }
        }
        // KeyUp 이벤트
        private void DataGridIn_TextFocus(object sender, KeyEventArgs e)
        {
            // 엔터 → 포커스 = true → cell != null → 해당 텍스트박스가 null이 아니라면 
            // → 해당 텍스트박스가 포커스가 안되있음 SelectAll() or 포커스
            Lib.Instance.DataGridINTextBoxFocus(sender, e);
        }
        // GotFocus 이벤트
        private void DataGridCell_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                DataGridCell cell = sender as DataGridCell;
                cell.IsEditing = true;
            }
        }
        // 2019.08.27 MouseUp 이벤트
        private void DataGridCell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Lib.Instance.DataGridINTextBoxFocusByMouseUP(sender, e);
        }

        #endregion // Content 부분 - 데이터 그리드 키 이벤트

        #region 주요 메서드

        private void re_Search(int selectedIndex)
        {
            // 수정용 before 지워주기
            beforeOutware = null;
            beforeIndex = 0;

            FillGrid();

            rowNum = 0;
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.SelectedIndex = rowNum;
            }
            else
            {
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

                sqlParameter.Add("ChkDate", chkDateSrh.IsChecked == true ? 1 : 0);

                sqlParameter.Add("FromDate", dtpSDateSrh.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("ToDate", dtpEDateSrh.SelectedDate.Value.ToString("yyyyMMdd"));
                sqlParameter.Add("nchkDeptID", CheckBoxDepartID.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sDeptID", CheckBoxDepartID.IsChecked == true ? (TextBoxDepartID.Tag.Equals("") ? "" : TextBoxDepartID.Tag.ToString()) : "");

                sqlParameter.Add("nchkPersonID", CheckBoxPersonID.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sPersonID", CheckBoxPersonID.IsChecked == true ? (txtPersonIDSrh.Tag.Equals("") ? "" : txtPersonIDSrh.Tag.ToString()) : "");
                

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_GTE_sGtedaily", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        int i = 0;
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow dr in drc)
                        {



                            if (dr["cls"].ToString() == "1")
                            {
                                i++;
                                var Outware = new Gte_DailyGte_U_CodeView()
                                {
                                    Num = i,

                                    cls = dr["cls"].ToString(),
                                    GteDay = dr["GteDay"].ToString(),

                                    DailyID = dr["DailyID"].ToString(),
                                    DailyName = dr["DailyName"].ToString(),
                                    PersonID = dr["PersonID"].ToString(),
                                    Name = dr["Name"].ToString(),

                                    WorkTimeID = dr["WorkTimeID"].ToString(),
                                    WorkTimeName = dr["WorkTimeName"].ToString(),
                                    WorkOffGbnID = dr["WorkOffGbnID"].ToString(),
                                    WorkOffGbnIDName = dr["WorkOffGbnIDName"].ToString(),

                                    GteComments = dr["GteComments"].ToString(),


                                    InOfficeTime = DateTimeFormat(dr["InOfficeTime"].ToString()),

                                    GoOutTime = DateTimeFormat(dr["GoOutTime"].ToString()),
                                    GoInTime = DateTimeFormat(dr["GoInTime"].ToString()),
                                    OffOfficeTime = DateTimeFormat(dr["OffOfficeTime"].ToString()),
                                    ModifyClss = dr["ModifyClss"].ToString(),
                                    ModifyClssName = dr["ModifyClssName"].ToString(),


                                    BasicWorkTime = DateTimeFormat(dr["BasicWorkTime"].ToString()),

                                    ExtendWorkTime = DateTimeFormat(dr["ExtendWorkTime"].ToString()),
                                    NightWorkTime = DateTimeFormat(dr["NightWorkTime"].ToString()),
                                    HoliBasicWorkTime = DateTimeFormat(dr["HoliBasicWorkTime"].ToString()),
                                    HoliExtendWorkTime = DateTimeFormat(dr["HoliExtendWorkTime"].ToString()),
                                    HoliNightWorkTime = DateTimeFormat(dr["HoliNightWorkTime"].ToString()),

                                    LatePeriodTime = dr["LatePeriodTime"].ToString(),
                                    EalyLeavePeriodTime = dr["EalyLeavePeriodTime"].ToString(),
                                    GoOutPeriodTime = dr["GoOutPeriodTime"].ToString(),
                                    Comments = dr["Comments"].ToString(),
                                    CreateDate = dr["CreateDate"].ToString()

                                };

                                Outware.CreateDate = Outware.CreateDate.Substring(0, 10).Replace("-", "");
                                Outware.OutDate_CV = DatePickerFormat(Outware.GteDay);
                                dgdMain.Items.Add(Outware);
                            }
                            else if (dr["cls"].ToString() == "9")
                            {
                                i++;
                                var Outware = new Gte_DailyGte_U_CodeView()
                                {
                                    Num = i,

                                    //GteDay = dr["GteDay"].ToString(),
                                    cls = dr["cls"].ToString(),
                                    GteDay = "합계",
                                    DailyID = dr["DailyID"].ToString(),
                                    DailyName = dr["DailyName"].ToString(),
                                    PersonID = dr["PersonID"].ToString(),
                                    Name = dr["Name"].ToString(),

                                    WorkTimeID = dr["WorkTimeID"].ToString(),
                                    WorkTimeName = dr["WorkTimeName"].ToString(),
                                    WorkOffGbnID = dr["WorkOffGbnID"].ToString(),
                                    WorkOffGbnIDName = dr["WorkOffGbnIDName"].ToString(),

                                    GteComments = dr["GteComments"].ToString(),


                                    InOfficeTime = DateTimeFormat(dr["InOfficeTime"].ToString()),

                                    GoOutTime = DateTimeFormat(dr["GoOutTime"].ToString()),
                                    GoInTime = DateTimeFormat(dr["GoInTime"].ToString()),
                                    OffOfficeTime = DateTimeFormat(dr["OffOfficeTime"].ToString()),

                                    ModifyClss = dr["ModifyClss"].ToString(),
                                    ModifyClssName = dr["ModifyClssName"].ToString(),

                                    BasicWorkTime = DateTimeFormat(dr["BasicWorkTime"].ToString()),

                                    ExtendWorkTime = DateTimeFormat(dr["ExtendWorkTime"].ToString()),
                                    NightWorkTime = DateTimeFormat(dr["NightWorkTime"].ToString()),
                                    HoliBasicWorkTime = DateTimeFormat(dr["HoliBasicWorkTime"].ToString()),
                                    HoliExtendWorkTime = DateTimeFormat(dr["HoliExtendWorkTime"].ToString()),
                                    HoliNightWorkTime = DateTimeFormat(dr["HoliNightWorkTime"].ToString()),

                                    LatePeriodTime = dr["LatePeriodTime"].ToString(),
                                    EalyLeavePeriodTime = dr["EalyLeavePeriodTime"].ToString(),
                                    GoOutPeriodTime = dr["GoOutPeriodTime"].ToString(),
                                    Comments = dr["Comments"].ToString(),
                                    CreateDate = dr["CreateDate"].ToString()

                                };

                                Outware.CreateDate = Outware.CreateDate.Substring(0, 10).Replace("-", "");
                                Outware.OutDate_CV = (Outware.GteDay);
                                dgdMain.Items.Add(Outware);
                            }





                            //Outware.CreateDate = Outware.CreateDate.Substring(0, 10).Replace("-", "");
                            //Outware.OutDate_CV = DatePickerFormat(Outware.GteDay);
                            //dgdMain.Items.Add(Outware);
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

        // Outware, OutwareSub 가 1 : 1 인데, 라벨을 스캔해서 넣는 것도 아니라서, OutwareSub 에 넣을게 없는데??
        // 두개 동시에 넣는 의미가 없는거 아니냐
        private bool SaveData(string strFlag)
        {
            bool flag = false;

            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();


            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            //
            try
            {
                if (CheckData())
                {
                    for (int i = 0; i < dgdMain.Items.Count; i++)
                    {
                        var Outware = dgdMain.Items[i] as Gte_DailyGte_U_CodeView;

                        //if (Outware.InOfficeTime.Length == 4 || Outware.InOfficeTime.Equals(""))
                        //{
                        //    MessageBox.Show("ㅁㅁㅁ");
                        //    flag = false;
                        //}

                        if (Outware != null)
                        {
                            // 플래그가 없다는건 기존에 있던 데이터 → 건너뛰기
                            if (Outware.strFlag == null || Outware.strFlag.Trim().Equals(""))
                            {
                                continue;
                            }
                            else
                            {
                                sqlParameter = new Dictionary<string, object>();
                                sqlParameter.Clear();

                                // 실제로 값을 넣는 값들
                                sqlParameter.Add("JobFlag", Outware.strFlag.Equals("I") ? "I" : "U");

                                sqlParameter.Add("DailyID",Outware.DailyID);
                                sqlParameter.Add("PersonID", Outware.PersonID);
                                sqlParameter.Add("GteDay", Outware.GteDay);
                                
                                sqlParameter.Add("WorkTimeID", Outware.WorkTimeID);
                                sqlParameter.Add("WorkOffGbnID", Outware.WorkOffGbnID);
                                sqlParameter.Add("GteComments", Outware.GteComments);
                                

                                sqlParameter.Add("InOfficeTime", Outware.InOfficeTime.Equals("") == true ? "" : Outware.InOfficeTime.ToString().Replace(":",""));
                                sqlParameter.Add("OffOfficeTime", Outware.OffOfficeTime.Equals("") == true ? "" : Outware.OffOfficeTime.ToString().Replace(":", ""));
                                sqlParameter.Add("GoOutTime", Outware.GoOutTime.Equals("") == true ? "" : Outware.GoOutTime.ToString().Replace(":", ""));
                                sqlParameter.Add("GoInTime", Outware.GoInTime.Equals("") == true ? "" : Outware.GoInTime.ToString().Replace(":", ""));
                                sqlParameter.Add("ModifyClss", Outware.ModifyClss);

                                //sqlParameter.Add("BasicWorkTime", ConvertInt(Outware.BasicWorkTime.Equals("") ==true ? "" : Outware.BasicWorkTime));
                                sqlParameter.Add("BasicWorkTime", ConvertInt(Outware.BasicWorkTime.Equals("") == true ? "" : Outware.BasicWorkTime));


                                sqlParameter.Add("ExtendWorkTime", ConvertInt(Outware.ExtendWorkTime.Equals("") == true ? "" : Outware.ExtendWorkTime)); // 여긴 뭐 넣으라고!!!
                                sqlParameter.Add("NightWorkTime", ConvertInt(Outware.NightWorkTime.Equals("") == true ? "" : Outware.NightWorkTime));
                                sqlParameter.Add("HoliBasicWorkTime", ConvertInt(Outware.HoliBasicWorkTime.Equals("") == true ? "" : Outware.HoliBasicWorkTime));
                                sqlParameter.Add("HoliExtendWorkTime", ConvertInt(Outware.HoliExtendWorkTime.Equals("") == true ? "" : Outware.HoliExtendWorkTime));

                                sqlParameter.Add("HoliNightWorkTime", ConvertInt(Outware.HoliNightWorkTime.Equals("") == true ? "" : Outware.HoliNightWorkTime));
                                sqlParameter.Add("LatePeriodTime", ConvertInt(Outware.LatePeriodTime.Equals("") == true ? "" : Outware.LatePeriodTime));
                                sqlParameter.Add("EalyLeavePeriodTime", ConvertInt(Outware.EalyLeavePeriodTime.Equals("") == true ? "" : Outware.EalyLeavePeriodTime));

                                sqlParameter.Add("GoOutPeriodTime", ConvertInt(Outware.GoOutPeriodTime.Equals("") == true ? "" : Outware.GoOutPeriodTime));
                                sqlParameter.Add("Comments", Outware.Comments);
                                //sqlParameter.Add("CreateDate", DateTime.Today);
                                sqlParameter.Add("UserID", MainWindow.CurrentUser);

                                Procedure pro1 = new Procedure();
                                pro1.list_OutputName = new List<string>();
                                pro1.list_OutputLength = new List<string>();

                                pro1.Name = "xp_Gte_iGteDaily";
                                pro1.OutputUseYN = "N";
                                pro1.list_OutputName.Add("PersonID");
                                pro1.list_OutputLength.Add("12");

                                Prolist.Add(pro1);
                                ListParameter.Add(sqlParameter);
                            }                          
                        }
                    } // for문 끝

                    //삭제
                    if (lstDeleteData.Count > 0)
                    {
                        foreach (var DelStuffin in lstDeleteData)
                        {
                            sqlParameter = new Dictionary<string, object>();
                            sqlParameter.Clear();

                            sqlParameter.Add("PersonID", DelStuffin.PersonID);
                            sqlParameter.Add("GteDay", DelStuffin.GteDay);

                            Procedure pro6 = new Procedure();
                            pro6.Name = "xp_Gte_dGteDaily";
                            pro6.OutputUseYN = "N";
                            pro6.OutputName = "REQ_ID";
                            pro6.OutputLength = "10";

                            Prolist.Add(pro6);
                            ListParameter.Add(sqlParameter);

                        }


                    }

                    string[] Confirm = new string[2];

                    if (Prolist.Count > 0)
                    {
                        for (int i = 0; i < Prolist.Count; i++)
                        {
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

            // 추가와 수정인것만 체크하기
            // 입고일자 → 이걸로 StuffinID 를 만들기 때문에 무조건 있어야 함
            for (int i = 0; i < dgdMain.Items.Count; i++)
            {
                var Stuffin = dgdMain.Items[i] as Gte_DailyGte_U_CodeView;

                if (Stuffin != null)
                {
                    // 추가나 수정이 아니라면 (기존 데이터) 패스
                    if (Stuffin.strFlag == null || (!Stuffin.strFlag.Trim().Equals("I") && !Stuffin.strFlag.Trim().Equals("U")))
                    {
                        continue;
                    }
                    else // 추가나 수정이면... 뭐를 공백 체크해야 되냐... → 일단 거래처랑, 품명이 입력되지 않으면 저장 에러로 저장이 안됨.
                    {
                        // 입고일자 → 입력 안하면 pl_Input PK 가 개판됨
                        if (Stuffin.OutDate_CV == null || Stuffin.OutDate_CV.Trim().Equals(""))
                        {
                            MessageBox.Show("일자를 입력해주세요.");
                            flag = false;

                            // 입력안된 데이터그리드 행 선택
                            dgdMain.SelectedIndex = i;
                            return false;
                        }

                        // 사원명
                        if (Stuffin.PersonID == null || Stuffin.PersonID.Trim().Equals(""))
                        {
                            MessageBox.Show("사원명 입력해주세요.");
                            flag = false;

                            // 입력안된 데이터그리드 행 선택
                            dgdMain.SelectedIndex = i;
                            return false;
                        }
                        

                        return flag;
                    }
                }
            }

            return flag;

        }

        // Stuffin 객체에 값이 들어있는지 체크
        //private bool chkSaveStuffin(Gte_DailyGte_U_CodeView Stuffin)
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

        #region 기타 메서드

        // 천마리 콤마, 소수점 버리기
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

        //시간
        private string DateTimeFormat(string str)
        {
            string result = "";

            if (!str.Trim().Equals(""))
            {

                if (str.Trim().Length == 4)
                {
                    string hour = str.Substring(0, 2);
                    string minutes = str.Substring(2, 2);

                    result = hour + ":" + minutes;
                }
                //else if (str.Trim().Length == 1)
                //{
                //    string hour = str.Substring(0, 1);
                //    string minutes = str.Substring(2, 2);

                //    result = "0" + hour + ":00"+ minutes;

                //}

                else if (str.Trim().Length == 2)
                {
                    string hour = str.Substring(0, 2);

                    result = hour + ":00";

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
                str = str.Replace(":", "");

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

        #endregion // 기타 메서드

        // 데이터피커 공용 로드 이벤트
        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            // 그냥 오늘 설정하고 다음행으로 넘기자
            if (editing == true)
            {
                //(sender as DatePicker).IsDropDownOpen = true;

                (sender as DatePicker).SelectedDate = DateTime.Today;

                //editing = false;
                int currRow = dgdMain.Items.IndexOf(dgdMain.CurrentItem);
                dgdMain.CurrentCell = new DataGridCellInfo(dgdMain.Items[currRow], dgdMain.Columns[2]);

            }
        }

        private void dgdtpetxtEvalGroupName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                Outware = dgdMain.CurrentItem as Gte_DailyGte_U_CodeView;

                if (Outware != null)
                {
                    TextBox tb1 = sender as TextBox;
                    Outware.PersonID = tb1.Text;
                    sender = tb1;
                }
            }
        }


        //근태구분
        private void cboWorkOffGbnID_Loaded(object sender, RoutedEventArgs e)
        {

            ComboBox cboSender = sender as ComboBox;
            cboSender.ItemsSource = null;

            ObservableCollection<CodeView> ovcWorkOffGbn = ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "GTEGBN", "Y", "", "");
            cboSender.ItemsSource = ovcWorkOffGbn;
            cboSender.DisplayMemberPath = "code_name";
            cboSender.SelectedValuePath = "code_id";
        }
        //근태구분
        private void cboWorkOffGbnID_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cboSender = sender as ComboBox;
            var Work = cboSender.DataContext as Gte_DailyGte_U_CodeView;

            if (Work != null)
            {
                Work.WorkOffGbnID = cboSender.SelectedValue != null ? cboSender.SelectedValue.ToString() : "";
                Work.WorkOffGbnName = cboSender.Text;
            }
        }

        //수정구분
        private void cboModifyClssGbnID_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cboSender = sender as ComboBox;
            cboSender.ItemsSource = null;

            ObservableCollection<CodeView> ovcModifyGbn = ComboBoxUtil.Instance.Gf_DB_CM_GetComCodeDataset(null, "MODIFY", "Y", "","");
            cboSender.ItemsSource = ovcModifyGbn;
            cboSender.DisplayMemberPath = "code_name";
            cboSender.SelectedValuePath = "code_id";
        }

        //수정구분
        private void cboModifyClssGbnI_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cboSender = sender as ComboBox;
            var Work = cboSender.DataContext as Gte_DailyGte_U_CodeView;

            if (Work != null)
            {
                Work.ModifyClss = cboSender.SelectedValue != null ? cboSender.SelectedValue.ToString() : "";
                Work.ModifyClssName = cboSender.Text;
            }
        }

        //근태사유 
        private void txtWoffID_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txtSender = sender as TextBox;

            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtSender, 77, "");

                //var Work = txtSender.DataContext as Gte_DailyGte_U_CodeView;
                //if (Work != null)
                //{
                //    MessageBox.Show(Work.GteComments);
                //    MessageBox.Show(Work.GteCommentsID);
                //}
            }
        }

        private void txtTest_PreKeyDown(object sender, KeyEventArgs e)
        {
            if (
                (!Char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key))
                && e.Key != Key.NumPad0
                && e.Key != Key.NumPad1
                && e.Key != Key.NumPad2
                && e.Key != Key.NumPad3
                && e.Key != Key.NumPad4
                && e.Key != Key.NumPad5
                && e.Key != Key.NumPad6
                && e.Key != Key.NumPad7
                && e.Key != Key.NumPad8
                && e.Key != Key.NumPad9
                ) && e.Key != Key.Back
                || e.Key == Key.Space
                )
            {
                e.Handled = true;
            }
        }
    }

    /*
                      
     */

    #region CodeView 코드뷰

    class Gte_DailyGte_U_CodeView : BaseView
    {
        public int Num { get; set; }
        public string cls { get; set; }
        public string strFlag { get; set; }

        public string PersonID { get; set; }
        public string GteDay { get; set; }
        public string DoWeek { get; set; }
        public string Name { get; set; }
        public string WorkTimeID { get; set; }

        public string WorkTimeName { get; set; }
        public string GteComments { get; set; }
        public string InOfficeTime { get; set; }
        public string GoOutTime { get; set; }
        public string GoInTime { get; set; }

        public string OffOfficeTime { get; set; }
        public string ModifyClss { get; set; }
        public string BasicWorkTime { get; set; }
        public string ExtendWorkTime { get; set; }
        public string NightWorkTime { get; set; }

        public string HoliBasicWorkTime { get; set; }
        public string HoliExtendWorkTime { get; set; }
        public string HoliNightWorkTime { get; set; }
        public string LatePeriodTime { get; set; }
        public string EalyLeavePeriodTime { get; set; }

        public string GoOutPeriodTime { get; set; }
        public string WorkOffGbnID { get; set; }
        public string WorkOffGbnName { get; set; }
        public string DOW { get; set; }
        public string DOWID { get; set; }
        public string OutDate_CV { get; set; }
        public string OutClss { get; set; }

        public string OutDate { get; set; }

        public string Comments { get; set; }
        public string CreateDate { get; set; }
        
        public string GteCommentsID { get; set; }
        public string DailyID { get; set; }
        public string DailyName { get; set; }
        public string GteCommentsName { get; set; }
        public string WorkOffGbnIDName { get; set; }
        public string ModifyClssName { get; set; }





        public string OutQtyY { get; set; }
        public string StuffInQty { get; set; }
        public string OutWeight { get; set; }

        public string OutRealWeight { get; set; }
        public string UnitPriceClss { get; set; }
        public string BuyerDirectYN { get; set; }
        public string Vat_Ind_YN { get; set; }
        public string InsStuffINYN { get; set; }

        public string ExchRate { get; set; }
        public string FromLocID { get; set; }
        public string FromLocName { get; set; }
        public string TOLocID { get; set; }
        public string TOLocName { get; set; }

        public string UnitClssName { get; set; }
        public string OutClssName { get; set; }
        public string UnitPrice { get; set; }
        public string Amount { get; set; }
        public string VatAmount { get; set; }

        public string BuyerArticleNo { get; set; }
        public string OutCustomID { get; set; }
        public string BuyerID { get; set; }
        public string BuyerName { get; set; }
        public string Buyer_Chief { get; set; }

        public string Buyer_Address1 { get; set; }
        public string Buyer_Address2 { get; set; }
        public string Buyer_Address3 { get; set; }
        public string CustomNo { get; set; }
        public string Chief { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string OutCustom { get; set; }
        public string OutSubType { get; set; }
    }

    class CompareOutware
    {
        public void setOutware(Gte_DailyGte_U_CodeView Outware)
        {
            this.GteDay = Outware.GteDay;
            this.DailyID = Outware.DailyID;
            this.PersonID = Outware.PersonID;
            this.Name = Outware.Name;
            this.WorkTimeID = Outware.WorkTimeID;
            this.WorkTimeName = Outware.WorkTimeName;
            this.WorkOffGbnID = Outware.WorkOffGbnID;
            this.GteComments = Outware.GteComments;
            this.InOfficeTime = Outware.InOfficeTime;
            this.GoOutTime = Outware.GoOutTime;

            this.GoInTime = Outware.GoInTime;
            this.OffOfficeTime = Outware.OffOfficeTime;
            this.ModifyClss = Outware.ModifyClss;
            this.BasicWorkTime = Outware.BasicWorkTime;
            this.ExtendWorkTime = Outware.ExtendWorkTime;

            this.NightWorkTime = Outware.NightWorkTime;
            this.HoliBasicWorkTime = Outware.HoliBasicWorkTime;
            this.HoliExtendWorkTime = Outware.HoliExtendWorkTime;
            this.HoliNightWorkTime = Outware.HoliNightWorkTime;
            this.LatePeriodTime = Outware.LatePeriodTime;

            this.EalyLeavePeriodTime = Outware.EalyLeavePeriodTime;
            this.GoOutPeriodTime = Outware.GoOutPeriodTime;

        }

        public string cls { get; set; }

        public string PersonID { get; set; }
        public string GteDay { get; set; }
        public string DailyID { get; set; }
        public string Name { get; set; }
        public string WorkTimeID { get; set; }

        public string WorkTimeName { get; set; }
        public string GteComments { get; set; }
        public string GteCommentsName { get; set; }
        
        public string InOfficeTime { get; set; }
        public string GoOutTime { get; set; }
        public string GoInTime { get; set; }

        public string OffOfficeTime { get; set; }
        public string ModifyClss { get; set; }
        public string BasicWorkTime { get; set; }
        public string ExtendWorkTime { get; set; }
        public string NightWorkTime { get; set; }

        public string HoliBasicWorkTime { get; set; }
        public string HoliExtendWorkTime { get; set; }
        public string HoliNightWorkTime { get; set; }
        public string LatePeriodTime { get; set; }
        public string EalyLeavePeriodTime { get; set; }

        public string GoOutPeriodTime { get; set; }
        public string WorkOffGbnID { get; set; }


        public bool chkIsUpdate(Gte_DailyGte_U_CodeView Outware)
        {

            bool flag = true;

            if ( !this.GteDay.Equals(Outware.GteDay)
            || !this.DailyID.Equals(DailyID)
            || !this.PersonID.Equals(PersonID)
            || !this.Name.Equals(Outware.Name)
            || !this.WorkTimeID.Equals(Outware.WorkTimeID)

            || !this.WorkTimeName.Equals(Outware.WorkTimeName)
            || !this.WorkOffGbnID.Equals(Outware.WorkOffGbnID)

            || !this.GteComments.Equals(Outware.GteComments)
            
            || !this.InOfficeTime.Equals(Outware.InOfficeTime)
            || !this.GoOutTime.Equals(Outware.GoOutTime)

            || !this.GoInTime.Equals(Outware.GoInTime)
            || !this.OffOfficeTime.Equals(Outware.OffOfficeTime)
            || !this.ModifyClss.Equals(Outware.ModifyClss)
            || !this.BasicWorkTime.Equals(Outware.BasicWorkTime)
            || !this.ExtendWorkTime.Equals(Outware.ExtendWorkTime)

            || !this.NightWorkTime.Equals(Outware.NightWorkTime)
            || !this.HoliBasicWorkTime.Equals(Outware.HoliBasicWorkTime)
            || !this.HoliExtendWorkTime.Equals(Outware.HoliExtendWorkTime)
            || !this.HoliNightWorkTime.Equals(Outware.HoliNightWorkTime)
            || !this.LatePeriodTime.Equals(Outware.LatePeriodTime)

            || !this.EalyLeavePeriodTime.Equals(Outware.EalyLeavePeriodTime)
            || !this.GoOutPeriodTime.Equals(Outware.GoOutPeriodTime))

            {
                flag = false;
                return flag;
            }

            return flag;
        }
    }

    #endregion // CodeView 코드뷰
}
