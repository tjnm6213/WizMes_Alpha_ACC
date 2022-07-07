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
using System.Windows.Shapes;

namespace WizMes_Alpha_JA.PopUp
{
    /// <summary>
    /// LoginPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginPage : Window
    {
        public string strLogRegID = string.Empty;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetInfo();
        }

        //로그인
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Log(txtUserID.Text))
            {
                strLogRegID = txtUserID.Text;
                Lib.Instance.SetLogResitry(strLogRegID);
                DialogResult = true;
            }
            else
            {
                txtPassWd.Password = "";
                return;
            }
        }

        //취소
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private bool Log(string strID)
        {
            bool flag = true;

            DataSet ds = null;
            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Clear();
            sqlParameter.Add("UserID", strID);
            ds = DataStore.Instance.ProcedureToDataSet("xp_Common_Login", sqlParameter, false);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("존재하지 않는 ID 입니다.");
                    flag = false;
                    return flag;
                }
                else
                {
                    if (!dt.Rows[0]["Password"].ToString().Equals(txtPassWd.Password))
                    {
                        MessageBox.Show("비밀번호가 잘못되었습니다.");
                        flag = false;
                        return flag;
                    }

                    //if (!dt.Rows[0]["Name"].Equals("20150401") && !dt.Rows[0]["Name"].Equals("admin"))
                    //{
                    //    MessageBox.Show("권한이 없는 사용자입니다.");
                    //    return flag;
                    //}
                }
            }

            return flag;
        }


        private void GetInfo()
        {
            txtUserID.Text = Lib.Instance.GetLogResitry();

            if (txtUserID.Text.Equals(""))
            {
                txtUserID.Focus();
            }
            else
            {
                txtPassWd.Focus();
            }
        }

        private void txtPassWd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Log(txtUserID.Text))
                {
                    strLogRegID = txtUserID.Text;
                    Lib.Instance.SetLogResitry(strLogRegID);
                    DialogResult = true;
                }
                else
                {
                    txtPassWd.Password = "";
                    return;
                }
            }
        }
    }
}
