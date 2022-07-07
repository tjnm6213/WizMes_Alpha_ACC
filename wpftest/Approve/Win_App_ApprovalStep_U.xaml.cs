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

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// Win_App_ApprovalStep_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_App_ApprovalStep_U : UserControl
    {
        int rowNum = 0;
        string strFlag = "";

        public Win_App_ApprovalStep_U()
        {
            InitializeComponent();
        }

        // 폼 로드
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
            MakePersonList(false);
        }

        #region 추가, 수정 모드 / 저장완료, 취소 모드

        private void SaveUpdateMode()
        {
            grdSrh1.IsEnabled = false;
            grdSrh2.IsEnabled = false;

            btnAdd.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSearch.IsEnabled = false;

            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;

            btnExcel.IsEnabled = false;
            
            if (strFlag.Trim().Equals("I"))
            {
                tbkMsg.Text = "자료 추가중";
            }
            else
            {
                tbkMsg.Text = "자료 수정중";
            }
            lblMsg.Visibility = Visibility.Visible;

            dgdMain.IsEnabled = false;
            grdInput.IsHitTestVisible = true;
            
        }

        private void CompleteCancelMode()
        {
            grdSrh1.IsEnabled = true;
            grdSrh2.IsEnabled = true;

            btnAdd.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSearch.IsEnabled = true;

            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;

            btnExcel.IsEnabled = true;

            lblMsg.Visibility = Visibility.Hidden;

            dgdMain.IsEnabled = true;
            grdInput.IsHitTestVisible = false;

            txtPersonSrh.Text = "";
            chkPersonSrh.IsChecked = false;
        }

        #endregion // 추가, 수정 모드 / 저장완료, 취소 모드

        #region Header 부분 - 검색조건

        private void lblAppStepNameSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkAppStepNameSrh.IsChecked == true)
            {
                chkAppStepNameSrh.IsChecked = false;
            }
            else
            {
                chkAppStepNameSrh.IsChecked = true;
            }
        }
        private void chkAppStepNameSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkAppStepNameSrh.IsChecked = true;
            txtAppStepNameSrh.IsEnabled = true;
        }
        private void chkAppStepNameSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkAppStepNameSrh.IsChecked = false;
            txtAppStepNameSrh.IsEnabled = false;
        }

        // 사용안함 포함
        private void lblUseClss_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkUseClss.IsChecked == true)
            {
                chkUseClss.IsChecked = false;
            }
            else
            {
                chkUseClss.IsChecked = true;
            }
        }
        //private void chkUseClss_UnChecked(object sender, RoutedEventArgs e)
        //{
        //    chkUseClss.IsChecked = true;
        //}
        //private void chkUseClss_Checked(object sender, RoutedEventArgs e)
        //{
        //    chkUseClss.IsChecked = false;
        //}

        #endregion // Header 부분 - 검색조건

        #region Header 부분 - 오른쪽 상단 버튼

        // 추가
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            strFlag = "I";
            this.DataContext = null;
            if (dgdSub.Items.Count > 0) { dgdSub.Items.Clear(); }
            SaveUpdateMode();

            rowNum = 0;
        }

        // 수정
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            strFlag = "U";
            SaveUpdateMode();

            rowNum = dgdMain.SelectedIndex;
        }

        // 삭제
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var AppStep = dgdMain.SelectedItem as Win_App_ApprovalStep_U_CodeView;

            if (AppStep == null)
            {
                MessageBox.Show("삭제할 데이터를 선택해주세요.");
            }
            else
            {

                if (MessageBox.Show("선택한 항목을 삭제하시겠습니까?", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    rowNum = dgdMain.SelectedIndex - 1;
                    if (rowNum < 0) { rowNum = 0; }

                    if (DeleteData(AppStep.AppStepID))
                    {
                        re_Search(rowNum);
                    }
                }

            }
        }

        // 닫기
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        // 검색
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            txtPersonSrh.Text = "";
            chkPersonSrh.IsChecked = false;

            rowNum = 0;
            re_Search(rowNum);
        }

        // 저장
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SaveData(strFlag))
            {              
                CompleteCancelMode();

                re_Search(rowNum);
                strFlag = "";
            }
        }

        // 취소
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            CompleteCancelMode();
            strFlag = "";

            re_Search(rowNum);
        }

        // 엑셀
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion // Header 부분 - 오른쪽 상단 버튼

        #region Content - 메인그리드

        private void dgdMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var Person = dgdMain.SelectedItem as Win_App_ApprovalStep_U_CodeView;

            if (Person != null)
            {
                this.DataContext = Person;

                FillGridSub(Person.AppStepID);
            }
        }

        #endregion Content - 메인그리드

        #region 서브 그리드 넘기기 + 순번 재조정

        // 서브 그리드 추가버튼 이벤트
        private void btnAddSelectItem_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem tviPerson = tlvItemList.SelectedItem as TreeViewItem;

            if (tviPerson != null)
            {
                var Person = tviPerson.Header as App_PersonList;

                if (Person != null
                        && Person.Seq != 0)
                {
                    if (CheckIsPerson(Person.PersonID))
                    {
                        Person.Num = dgdSub.Items.Count + 1;

                        dgdSub.Items.Add(Person);
                    }
                }
            }
        }

        // 서브 그리드 삭제 버튼 이벤트
        private void btnDelSelectItem_Click(object sender, RoutedEventArgs e)
        {
            var Person = dgdSub.SelectedItem as App_PersonList;

            if (Person != null)
            {
                dgdSub.Items.Remove(Person);
                SettingNum();
            }
        }

        // 서브 그리드로 넘기기
        private void tvlItemList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                TextBlock tblSender = sender as TextBlock;

                //TreeViewItem tvi = tblSender.DataContext as TreeViewItem;

                //if (tvi != null)
                //{
                //    MessageBox.Show("이것은 트리뷰");
                //}

                var Person = tblSender.DataContext as App_PersonList;

                if (Person != null
                    && Person.Seq != 0)
                {
                    if (CheckIsPerson(Person.PersonID))
                    {
                        Person.Num = dgdSub.Items.Count + 1;

                        dgdSub.Items.Add(Person);
                    }
                }
            }
        }

        // 서브그리드 더블클릭시 → 해당 사원 없애기
        private void dgdSub_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                TextBlock tblSender = sender as TextBlock;
                var Person = tblSender.DataContext as App_PersonList;

                if (Person != null)
                {
                    dgdSub.Items.Remove(Person);
                    SettingNum();
                }
            }
        }

        // 순번 재조정
        private void SettingNum()
        {
            for (int i = 0; i < dgdSub.Items.Count; i++)
            {
                var Person = dgdSub.Items[i] as App_PersonList;

                if (Person != null)
                {
                    Person.Num = i + 1;
                }
            }
        }

        private bool CheckIsPerson(string strID)
        {
            bool flag = true;

            for (int i = 0; i < dgdSub.Items.Count; i++)
            {
                var Person = dgdSub.Items[i] as App_PersonList;

                if (Person != null)
                {
                    if (Person.PersonID.Trim().Equals(strID.Trim()))
                    {
                        flag = false;
                        return flag;
                    }
                }
            }

            return flag;
        }

        #endregion // 서브그리드 넘기기

        #region 서브그리드 - 사원명 검색

        // 서브 그리드 사원명 체크박스 이벤트
        private void lblPersonSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkPersonSrh.IsChecked == true)
            {
                chkPersonSrh.IsChecked = false;
            }
            else
            {
                chkPersonSrh.IsChecked = true;
            }
        }

        private void chkPersonSrh_Checked(object sender, RoutedEventArgs e)
        {
            chkPersonSrh.IsChecked = true;
            txtPersonSrh.IsEnabled = true;
        }

        private void chkPersonSrh_Unchecked(object sender, RoutedEventArgs e)
        {
            chkPersonSrh.IsChecked = false;
            txtPersonSrh.IsEnabled = false;
        }

        // 엔터 → 검색
        private void txtPersonSrh_KeyDown(object sender, KeyEventArgs e)
        {
            if (lblMsg.Visibility == Visibility.Visible)
            {
                if (e.Key == Key.Enter)
                {
                    MakePersonList(true);
                }
            }
        }

        // 서브 트리뷰 - 사원명 검색
        private void btnSearchSub_Click(object sender, RoutedEventArgs e)
        {
            bool flag = chkPersonSrh.IsChecked == true ? true : false;

            MakePersonList(flag);
        }




        #endregion // 서브그리드 - 사원명 검색

        #region 서브 그리드 - 순서 위로 올리기 / 아래로 내리기

        // 결재 순서 변경
        private void btnStepUpDown_Click(object sender, RoutedEventArgs e)
        {
            Button senderBtn = sender as Button;

            App_PersonList AppStepE = new App_PersonList(); // 임시 객체

            // 아래 버튼 클릭시
            if (senderBtn.Tag.ToString().Equals("Down"))
            {
                var StepFrom = dgdSub.SelectedItem as App_PersonList;

                if (StepFrom != null)
                {
                    int currRow = dgdSub.SelectedIndex;

                    int goalRow = currRow + 1;
                    int maxRow = dgdSub.Items.Count - 1;

                    if (goalRow <= maxRow)
                    {
                        var StepTo = dgdSub.Items[goalRow] as App_PersonList;

                        if (StepTo != null)
                        {
                            dgdSub.Items.RemoveAt(currRow); // 선택한 행 지우고
                            dgdSub.Items.RemoveAt(currRow); // 바로 밑의 행 지우고

                            StepTo.Num = currRow + 1;
                            dgdSub.Items.Insert(currRow, StepTo);

                            StepFrom.Num = goalRow + 1;
                            dgdSub.Items.Insert(goalRow, StepFrom);

                            dgdSub.SelectedIndex = goalRow;
                        }
                    }
                }
            }
            else // 위 버튼 클릭시
            {
                var StepFrom = dgdSub.SelectedItem as App_PersonList;

                if (StepFrom != null)
                {
                    int currRow = dgdSub.SelectedIndex;

                    int goalRow = currRow - 1;

                    if (goalRow >= 0)
                    {
                        var StepTo = dgdSub.Items[goalRow] as App_PersonList;

                        if (StepTo != null)
                        {
                            dgdSub.Items.RemoveAt(goalRow); // 선택한 행 지우고
                            dgdSub.Items.RemoveAt(goalRow); // 바로 밑의 행 지우고

                            StepTo.Num = currRow + 1;
                            dgdSub.Items.Insert(goalRow, StepTo);

                            StepFrom.Num = goalRow + 1;
                            dgdSub.Items.Insert(goalRow, StepFrom);

                            dgdSub.SelectedIndex = goalRow;
                        }
                    }
                }
            }
        }

        #endregion // 서브 그리드 - 순서 위로 올리기 / 아래로 내리기

        #region 주요 메서드

        private void re_Search(int selectedIndex)
        {
            FillGrid();

            MakePersonList(false);

            if (dgdMain.Items.Count > 0)
            {
                dgdMain.SelectedIndex = strFlag.Trim().Equals("I") ? dgdMain.Items.Count - 1 : selectedIndex;
            }
            else
            {
                txtAppStepID.Text = "";
                txtAppStepName.Text = "";

                dgdSub.Items.Clear();
                MessageBox.Show("조회된 데이터가 없습니다.");
                return;
            }
        }

        #region 결제 단계 조회 - FillGrid

        private void FillGrid()
        {
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear();
                dgdSub.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Clear();
                sqlParameter.Add("nAppStepName", chkAppStepNameSrh.IsChecked == true ? 1 : 0);
                sqlParameter.Add("AppStepName", txtAppStepNameSrh.Text);
                sqlParameter.Add("UseClss", chkUseClss.IsChecked == true ? 1: 0);
                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sApprovalStep", sqlParameter, false);

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
                            var App = new Win_App_ApprovalStep_U_CodeView()
                            {
                                Num = i,
                                AppStepID = dr["AppStepID"].ToString(),
                                AppStepName = dr["AppStepName"].ToString()
                                
                            };  

                            dgdMain.Items.Add(App);
                        }

                        // 2019.08.28 검색결과에 갯수 추가
                        //sPersonCount.Text = "▶검색 결과 : " + i + "건";
                    }
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

        #endregion // 결제 단계 조회

        #region 결제 단계 순서 조회 - FillGridSub

        private void FillGridSub(string strID)
        {
            if (dgdSub.Items.Count > 0)
            {
                dgdSub.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();

                sqlParameter.Clear();
                sqlParameter.Add("AppStepID", strID);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sApprovalStepSub", sqlParameter, false);

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
                            var AppSub = new App_PersonList()
                            {
                                Num = i,

                                AppStepID = dr["AppStepID"].ToString(),
                                AppStepSeq = dr["AppStepSeq"].ToString(),
                                PersonID = dr["PersonID"].ToString(),
                                Name = dr["Name"].ToString(),
                                ResablyID = dr["ResablyID"].ToString(),

                                Resably = dr["Resably"].ToString(),
                                DepartID = dr["DepartID"].ToString(),
                                Depart = dr["Depart"].ToString()
                            };

                            dgdSub.Items.Add(AppSub);
                        }

                        // 2019.08.28 검색결과에 갯수 추가
                        //sPersonCount.Text = "▶검색 결과 : " + i + "건";
                    }
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

        #endregion // 결제 단계 순서 조회 - FillGridSub

        #region 사원 리스트

        private void MakePersonList(bool IsExpanded)
        {
            TreeViewItem mTreeDepart = null;
            TreeViewItem mTreePerson = null;

            if (tlvItemList.Items.Count > 0)
            {
                tlvItemList.Items.Clear();
            }

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("nPerson", chkPersonSrh.IsChecked == true ? 1 : 0);
                sqlParameter.Add("Name", txtPersonSrh.Text);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Approval_sPersonList_Depart", sqlParameter, false);

                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {

                        int i = 0;
                        DataRowCollection drc = dt.Rows;

                        foreach (DataRow dr in drc)
                        {
                            i++;

                            var Person = new App_PersonList
                            {
                                Num = i,

                                DepartID = dr["DepartID"].ToString(),
                                Depart = dr["Depart"].ToString(),
                                ResablyID = dr["ResablyID"].ToString(),
                                Resably = dr["Resably"].ToString(),
                                PersonID = dr["PersonID"].ToString(),

                                Name = dr["Name"].ToString(),
                                Seq = ConvertInt(dr["Seq"].ToString())
                            };

                            if (Person.Seq == 0) // 부서명
                            {
                                Person.FirstColumn = Person.Depart;
                                mTreeDepart = new TreeViewItem() { Header = Person, IsExpanded = IsExpanded };
                                if (mTreeDepart != null)
                                {
                                    tlvItemList.Items.Add(mTreeDepart);
                                }

                                
                            }
                            else // 사원들
                            {
                                Person.FirstColumn = Person.Name;
                                mTreePerson = new TreeViewItem() { Header = Person, IsExpanded = false };
                                if (mTreePerson != null)
                                {
                                    mTreeDepart.Items.Add(mTreePerson);
                                }
                            }

                      
                        }
                    }
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

        #endregion // 사원 리스트

        #region 저장 메서드 SaveData()

        private bool SaveData(string strFlag)
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                if (CheckData())
                {
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();

                    sqlParameter.Add("JobFlag", strFlag.Trim());
                    sqlParameter.Add("AppStepID", strFlag.Trim().Equals("I") ? "" : txtAppStepID.Text);
                    sqlParameter.Add("AppStepName", txtAppStepName.Text);
                    sqlParameter.Add("UseClss", "");
                    sqlParameter.Add("UserID", MainWindow.CurrentUser);

                    Procedure pro1 = new Procedure();
                    pro1.list_OutputName = new List<string>();
                    pro1.list_OutputLength = new List<string>();

                    pro1.Name = "xp_Approval_iuApprovalStep";
                    pro1.OutputUseYN = strFlag.Trim().Equals("I") ? "Y" : "N";
                    pro1.list_OutputName.Add("AppStepID");
                    pro1.list_OutputLength.Add("5");

                    Prolist.Add(pro1);
                    ListParameter.Add(sqlParameter);

                    for(int i = 0; i < dgdSub.Items.Count; i++)
                    {                     
                        var Person = dgdSub.Items[i] as App_PersonList;

                        if (Person != null)
                        {
                            sqlParameter = new Dictionary<string, object>();
                            sqlParameter.Clear();

                            sqlParameter.Add("AppStepID", strFlag.Trim().Equals("I") ? "" : txtAppStepID.Text);
                            sqlParameter.Add("AppStepSeq", i + 1);
                            sqlParameter.Add("PersonID", Person.PersonID);
                            sqlParameter.Add("ResablyID", Person.ResablyID);
                            sqlParameter.Add("UserID", MainWindow.CurrentUser);

                            Procedure pro2 = new Procedure();

                            pro2.Name = "xp_Approval_iApprovalStepSub";
                            pro2.OutputUseYN = "N";
                            pro2.OutputName = "sCodeID";
                            pro2.OutputLength = "30";

                            Prolist.Add(pro2);
                            ListParameter.Add(sqlParameter);
                        }                       
                    }

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

            return flag;
        }

        #endregion // 저장 메서드

        #region 유효성 검사 CheckData()

        private bool CheckData()
        {
            bool flag = true;



            return flag;
        }

        #endregion //유효성 검사

        #region 삭제

        // 삭제 메서드
        private bool DeleteData(string strID)
        {
            bool flag = false;

            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Clear();
            sqlParameter.Add("AppStepID", strID);

            try
            {
                string[] result = DataStore.Instance.ExecuteProcedure("xp_Approval_dApprovalStep", sqlParameter, true);

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

        #endregion // 주요 메서드


        private void TlvItemList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
           
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

    }

    class Win_App_ApprovalStep_U_CodeView : BaseView
    {
        public int Num { get; set; }
        public string AppStepID { get; set; }
        public string AppStepName { get; set; }
    }

    class App_PersonList : BaseView
    {
        public int Num { get; set; }
        public string FirstColumn { get; set; }

        public string DepartID { get; set; }
        public string Depart { get; set; }
        public string ResablyID { get; set; }
        public string Resably { get; set; }
        public string PersonID { get; set; }

        public string Name { get; set; }
        public string AppStepID { get; set; }
        public string AppStepSeq { get; set; }
        public int Seq { get; set; }
    }
}
