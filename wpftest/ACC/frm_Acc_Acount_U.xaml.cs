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
using WizMes_Alpha_JA.PopUP;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// frm_Acc_Acount_U.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class frm_Acc_Acount_U : UserControl
    {
        string strFlag = string.Empty;
        int rowNum = 0;
        string strFindID = string.Empty;
        string strFindString = string.Empty;
        frm_Acc_Acount_U_CodeView frmBank = new frm_Acc_Acount_U_CodeView();



        public frm_Acc_Acount_U()
        {
            InitializeComponent();
        }

        private void UserContral_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);
        }

        #region 상단 오른쪽 버튼 

        //추가
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (dgdMain.SelectedItem != null) // 선택된 행이 있다면
            {
                rowNum = dgdMain.SelectedIndex; // rowNum에 행번호를 기억, 취소시 재검색시 rowNum의 자료를 보여줌
            }

            CantBtnControl(); //추가 버튼을 누르면 저장, 취소 외 다른 버튼은 비활성화 / dgd 터치 불가능
            tbkMsg.Text = "자료 입력 중";
            strFlag = "I";
            this.DataContext = null; // 바인딩 되어있는 자료들을 null로 비움?
                       
            //텍스트박스
            txtBankName.IsEnabled = true; // 은행명
            AccountNumber.IsEnabled = true; // 계좌번호
            AccountName.IsEnabled = true; // 예금주
            Comments.IsEnabled = true; // 비고

            //추가 버튼 클릭시 은행명에 커서가 이동되도록
            txtBankName.Focus();
        }


        //수정
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            frmBank = dgdMain.SelectedItem as frm_Acc_Acount_U_CodeView;

            if(frmBank != null)
            {
                rowNum = dgdMain.SelectedIndex;
                CantBtnControl();
                tbkMsg.Text = "자료 수정 중";
                strFlag = "U";

                //텍스트박스
                txtBankName.IsEnabled = true; // 은행명
                AccountNumber.IsEnabled = true; // 계좌번호
                AccountName.IsEnabled = true; // 예금주
                Comments.IsEnabled = true; // 비고
            }
            else
            {
                MessageBox.Show("수정할 자료를 선택하고 눌러주십시오.");
            }
        }

        //삭제
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            frmBank = dgdMain.SelectedItem as frm_Acc_Acount_U_CodeView;

            if (frmBank == null)
            {
                MessageBox.Show("삭제할 데이터가 지정되지 않았습니다. 삭제데이터를 지정하고 눌러주세요");
            }
            else
            {
                if (MessageBox.Show("선택하신 항목을 삭제하시겠습니까?", "삭제 전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (dgdMain.Items.Count > 0 && dgdMain.SelectedItem != null)
                    {
                        rowNum = dgdMain.SelectedIndex;
                    }

                    if (Procedure.Instance.DeleteData(frmBank.BankID, "BankID", "xp_CodeBank_dBank"))
                    {
                        rowNum -= 1;
                        re_Search(rowNum);
                    }
                }
            }
        }

        //닫기
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }

        //검색(조회)
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            rowNum = 0;
            re_Search(rowNum);

            //텍스트박스
            txtBankName.IsEnabled = false; // 은행명
            AccountNumber.IsEnabled = false; // 계좌번호
            AccountName.IsEnabled = false; // 예금주
            Comments.IsEnabled = false; // 비고
        }

        //저장
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            frmBank = dgdMain.SelectedItem as frm_Acc_Acount_U_CodeView;

            if(SaveData(txtBankID.Text, strFlag))
            {
                CanBtnControl();
                re_Search(rowNum);
            }
        }

        //취소
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CanBtnControl();
            strFlag = string.Empty;
            re_Search(rowNum);
        }

        //엑셀
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] dgdStr = new string[2];
            dgdStr[0] = "은행 정보";
            dgdStr[1] = dgdMain.Name;

            ExportExcelxaml ExpExc = new ExportExcelxaml(dgdStr);
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

        #endregion

        private void dgdMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            frmBank = dgdMain.SelectedItem as frm_Acc_Acount_U_CodeView;
            this.DataContext = frmBank;

        }


        #region 기타 메서드

        /// <summary>
        /// 저장이나 취소 버튼 클릭시 동작하는 것을 모아놓음
        /// </summary>
        private void CanBtnControl()
        {
            Lib.Instance.UiButtonEnableChange_IUControl(this);
            btnAdd.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSearch.IsEnabled = true;
            btnExcel.IsEnabled = true;
            btnSave.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;
            //사용안함 버튼
            chkNotUse.IsEnabled = false;


            dgdMain.IsHitTestVisible = true;
        }

        /// <summary>
        /// 추가 또는 수정 버튼 클릭시 동작하는 것을 모아놓음
        /// </summary>
        private void CantBtnControl()
        {
            Lib.Instance.UiButtonEnableChange_SCControl(this);
            btnAdd.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSearch.IsEnabled = false;
            btnExcel.IsEnabled = false;
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            //사용안함 버튼
            chkNotUse.IsEnabled = true;


            dgdMain.IsHitTestVisible = false;
        }

        //재검색
        private void re_Search(int selectedIndex)
        {
            FillGrid();

            if (dgdMain.Items.Count > 0)
            {
                if (strFindString.Equals(string.Empty))
                {
                    dgdMain.SelectedIndex = selectedIndex;
                }
                else
                {
                    dgdMain.SelectedIndex = Lib.Instance.ReTrunIndex(dgdMain, strFindString);
                }
            }
            else
            {
                this.DataContext = null;
            }
            strFindID = string.Empty;
            strFindString = string.Empty;

            //텍스트박스
            txtBankName.IsEnabled = false; // 은행명
            AccountNumber.IsEnabled = false; // 계좌번호
            AccountName.IsEnabled = false; // 예금주
            Comments.IsEnabled = false; // 비고
        }


        //조회
        private void FillGrid()
        {
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear(); // 이전 조회한 데이터 삭제
            }

            try
            {
                DataGrid test = new DataGrid();

                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("UseYN", chkIncDelete.IsChecked == true ? "" : "Y");
                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_CodeBank_sBank", sqlParameter, false);

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
                            var dgdBankInfo = new frm_Acc_Acount_U_CodeView()
                            {
                                Num = i,
                                BankID = dr["BankID"].ToString(),
                                BankName = dr["BankName"].ToString(),
                                AccountName = dr["AccountName"].ToString(),
                                AccountNumber = dr["AccountNumber"].ToString(),
                                Comments = dr["Comments"].ToString(),
                                Use_YN = dr["Use_YN"].ToString()
                            };

                            if (!strFindID.Equals(string.Empty))
                            {
                                if (strFindID.Equals(dgdBankInfo.BankID))
                                {
                                    strFindString = dgdBankInfo.ToString();
                                }
                            }
                            dgdMain.Items.Add(dgdBankInfo);
                        }

                        tbkIndexCount.Text = "검색건수 : " + i.ToString() + " 건";
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

        //저장
        private bool SaveData(string strBankID, string strFlag)
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                if (CheckData()) // 공백여부 확인
                {
                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Add("BankID", strBankID);
                    sqlParameter.Add("BankName", txtBankName.Text);
                    sqlParameter.Add("BankNameEng", "");
                    sqlParameter.Add("Use_YN", chkNotUse.IsChecked == true ? "N" : "Y");
                    sqlParameter.Add("Comments", Comments.Text);
                    sqlParameter.Add("AccountName", AccountName.Text);
                    sqlParameter.Add("AccountNumber", AccountNumber.Text);

                    if (strFlag.Equals("I"))
                    {
                        sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                        Procedure pro1 = new Procedure();
                        pro1.Name = "xp_CodeBank_iBank";
                        pro1.OutputUseYN = "Y";
                        pro1.OutputName = "BankID";
                        pro1.OutputLength = "5";

                        Prolist.Add(pro1);
                        ListParameter.Add(sqlParameter);

                        List<KeyValue> list_Result = new List<KeyValue>();
                        list_Result = DataStore.Instance.ExecuteAllProcedureOutputGetCS(Prolist, ListParameter);
                        string sGetBankID = string.Empty;

                        if (list_Result[0].key.ToLower() == "success")
                        {
                            list_Result.RemoveAt(0);
                            for (int i = 0; i < list_Result.Count; i++)
                            {
                                KeyValue kv = list_Result[i];
                                if (kv.key == "BankID")
                                {
                                    sGetBankID = kv.value;
                                    strFindID = sGetBankID;
                                    flag = true;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("[저장실패]\r\n" + list_Result[0].value.ToString());
                            flag = false;
                            //return false;
                        }
                    }
                    else
                    {
                        sqlParameter.Add("UpdateUserID", MainWindow.CurrentUser);

                        Procedure pro1 = new Procedure();
                        pro1.Name = "xp_CodeBank_uBank";
                        pro1.OutputUseYN = "N";
                        pro1.OutputName = "BankID";
                        pro1.OutputLength = "5";

                        Prolist.Add(pro1);
                        ListParameter.Add(sqlParameter);

                        string[] Confirm = new string[2];
                        Confirm = DataStore.Instance.ExecuteAllProcedureOutputNew(Prolist, ListParameter);
                        if (Confirm[0] != "success")
                        {
                            MessageBox.Show("[저장실패]\r\n" + Confirm[1].ToString());
                            flag = false;
                            //return false;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                }
                else { flag = false; }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.ToString());
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return flag;
        }

        //데이터 체크
        private bool CheckData()
        {
            bool flag = true;

            if (txtBankName.Text.Length <= 0 || txtBankName.Text.Equals(""))
            {
                MessageBox.Show("은행명 입력되지 않았습니다.");
                flag = false;
                return flag;
            }

            if (AccountNumber.Text.Length <= 0 || AccountNumber.Text.Equals(""))
            {
                MessageBox.Show("계좌번호가 입력되지 않았습니다.");
                flag = false;
                return flag;
            }

            if (AccountName.Text.Length <= 0 || AccountName.Text.Equals(""))
            {
                MessageBox.Show("예금주가 입력되지 않았습니다.");
                flag = false;
                return flag;
            }

            return flag;
        }


        #endregion

        #region key.enter 이벤트

        //은행명 key.enter
        private void TxtBankName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                AccountNumber.Focus();
            }
        }

        //계좌번호 key.enter
        private void AccountNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                AccountName.Focus();
            }
        }

        //예금주 key.enter
        private void AccountName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Comments.Focus();
            }
        }

        //비고 key.enter
        private void Comments_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSave.Focus();
            }
        }


        #endregion key.enter 이벤트

        //사용안함 포함 라벨 클릭 시
        private void LblNotUseSrh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(chkIncDelete.IsChecked == false)
            {
                chkIncDelete.IsChecked = true;
            }
            else
            {
                chkIncDelete.IsChecked = false;
            }

        }
    }

    #region 생성자
    class frm_Acc_Acount_U_CodeView : BaseView
    {
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }

        public int Num { get; set; }
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Comments { get; set; }
        public string Use_YN { get; set; }
    }
     #endregion

}

