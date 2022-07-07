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
using WPF.MDI;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// Win_sys_UserMenuLog_Q.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_com_UserMenuLog_Q : UserControl
    {
        public Win_com_UserMenuLog_Q()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            chkDate.IsChecked = true;
            btnToday_Click(null, null);

        }

        //사용일자
        private void lblDate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkDate.IsChecked == true) { chkDate.IsChecked = false; }
            else { chkDate.IsChecked = true; }
        }

        //사용일자
        private void chkDate_Checked(object sender, RoutedEventArgs e)
        {
            dtpSDate.IsEnabled = true;
            dtpEDate.IsEnabled = true;
        }

        //사용일자
        private void chkDate_Unchecked(object sender, RoutedEventArgs e)
        {
            dtpSDate.IsEnabled = false;
            dtpEDate.IsEnabled = false;
        }

        //전일
        private void btnYesterDay_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = DateTime.Today.AddDays(-1);
            dtpEDate.SelectedDate = DateTime.Today.AddDays(-1);
        }

        //금일
        private void btnToday_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = DateTime.Today;
            dtpEDate.SelectedDate = DateTime.Today;
        }

        //전월
        private void btnLastMonth_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[0];
            dtpEDate.SelectedDate = Lib.Instance.BringLastMonthDatetimeList()[1];
        }

        //금월
        private void btnThisMonth_Click(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[0];
            dtpEDate.SelectedDate = Lib.Instance.BringThisMonthDatetimeList()[1];
        }

        //사원명
        private void lblPersonName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkPersonName.IsChecked == true) { chkPersonName.IsChecked = false; }
            else { chkPersonName.IsChecked = true; }
        }

        //사원명
        private void chkPersonName_Checked(object sender, RoutedEventArgs e)
        {
            txtPersonName.IsEnabled = true;
            //btnPfPersonName.IsEnabled = true;
            txtPersonName.Focus();
        }

        //사원명
        private void chkPersonName_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPersonName.IsEnabled = false;
            //btnPfPersonName.IsEnabled = false;
        }

        //사원명
        private void txtPersonName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtPersonName, (int)Defind_CodeFind.DCF_PERSON, "");
            }
        }

        //사원명
        private void btnPfPersonName_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtPersonName, (int)Defind_CodeFind.DCF_PERSON, "");
        }

        //검색
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (dgdMain.Items.Count > 0)
            {
                dgdMain.Items.Clear();
            }

            FillGrid();

            if (dgdMain.Items.Count > 0)
            {
                dgdMain.SelectedIndex = 0;
            }
        }

        //닫기
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (MenuViewModel mvm in MainWindow.mMenulist)
            {
                if (mvm.subProgramID.ToString().Contains("MDI"))
                {
                    if (this.ToString().Equals((mvm.subProgramID as MdiChild).Content.ToString()))
                    {
                        (MainWindow.mMenulist[i].subProgramID as MdiChild).Close();
                        break;
                    }
                }
                i++;
            }
        }

        //엑셀
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] dgdStr = new string[2];
            dgdStr[0] = "사용자 Log조회";
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

        //실조회
        private void FillGrid()
        {
            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Add("sCompanyID", MainWindow.CompanyID);
                sqlParameter.Add("ChkDate", chkDate.IsChecked == true ? 1 : 0);
                sqlParameter.Add("sFromDate", chkDate.IsChecked == true ? dtpSDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("sToDate", chkDate.IsChecked == true ? dtpEDate.SelectedDate.Value.ToString("yyyyMMdd") : "");
                sqlParameter.Add("ChkPerson", chkPersonName.IsChecked == true ? 1:0);
                sqlParameter.Add("sPerson", chkPersonName.IsChecked == false || txtPersonName.Text == null || txtPersonName.Text.Trim().Equals("") ? "" : txtPersonName.Text);

                DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Common_sLogData", sqlParameter, false);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("조회된 데이터가 없습니다.");
                    }
                    else
                    {
                        DataRowCollection drc = dt.Rows;

                        for (int i = 0; i < drc.Count; i++)
                        {
                            DataRow dr = drc[i];

                            var WinUserMenuLog = new Win_sys_UserMenuLog_Q_CodeView()
                            {
                                Num = (i + 1),
                                WorkDate = dr["WorkDate"].ToString(),
                                WorkTime = dr["WorkTime"].ToString(),
                                PersonID = dr["PersonID"].ToString(),
                                UserID = dr["UserID"].ToString(),
                                Name = dr["Name"].ToString(),
                                MenuID = dr["MenuID"].ToString(),
                                Menu = dr["Menu"].ToString()
                            };

                            WinUserMenuLog.WorkDate = Lib.Instance.StrDateTimeBar(WinUserMenuLog.WorkDate);

                            if (WinUserMenuLog.WorkTime.Length > 0 && WinUserMenuLog.WorkTime.Length == 4)
                            {
                                WinUserMenuLog.WorkTime = WinUserMenuLog.WorkTime.Substring(0, 2) + ":" +
                                    WinUserMenuLog.WorkTime.Substring(2, 2);
                            }

                            dgdMain.Items.Add(WinUserMenuLog);
                        }

                        tbkCount.Text = "▶ 검색 결과 : " + dt.Rows.Count.ToString() + " 건";
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
    }

    class Win_sys_UserMenuLog_Q_CodeView : BaseView
    {
        public int Num { get; set; }
        public string WorkDate { get; set; }
        public string WorkTime { get; set; }
        public string PersonID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string MenuID { get; set; }
        public string Menu { get; set; }
    }
}
