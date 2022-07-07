using System;
using System.Collections.Generic;
using System.Windows;

namespace WizMes_Alpha_JA.PopUp
{
    /// <summary>
    /// Win_comMCRunningTarget_PopUP.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_comMCRunningTarget_PopUP : Window
    {
        public Win_comMCRunningTarget_PopUP()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dtpSDate.SelectedDate = DateTime.Today.AddYears(-1);
            dtpEDate.SelectedDate = DateTime.Today.AddYears(-1);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!CopyData())
            {
                MessageBox.Show("복사등록이 실패하였습니다.");
                DialogResult = false;
            }
            else
            {
                DialogResult = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private bool CopyData()
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {
                #region 추가

                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("cYYYY", dtpSDate.SelectedDate.Value.ToString("yyyy"));
                sqlParameter.Add("vYYYY", dtpEDate.SelectedDate.Value.ToString("yyyy"));
                sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                Procedure pro1 = new Procedure();
                pro1.Name = "xp_McRunningGoal_iCopyMcRunningGoal";
                pro1.OutputUseYN = "N";
                pro1.OutputName = "sArticleID";
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

                #endregion
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
    }
}
