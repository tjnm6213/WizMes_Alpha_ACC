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
    public partial class ChangePerson : UserControl
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

        public ChangePerson()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Lib.Instance.UiLoading(sender);

            txtOrigin.Text = MainWindow.CurrentPersonID;
        }





        #region Header 부분 - 오른쪽 상단 버튼 이벤트

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //this.DataContext = null;

            //strFlag = "I";
            //dtpReqDate.SelectedDate = DateTime.Today;
            //cboAppGBN.SelectedIndex = 0;

            //SaveUpdateMode();

            try
            {
                MainWindow.CurrentPersonID = txtRequester.Tag.ToString();

                txtHoit.Text = MainWindow.CurrentPersonID;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        // 요청자 사원 엔터 → 플러스파인더
        private void txtRequester_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtRequester, (int)Defind_CodeFind.DCF_PERSON, "");

                try
                {
                    MainWindow.CurrentPersonID = txtRequester.Tag.ToString();

                    txtHoit.Text = MainWindow.CurrentPersonID;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        // 요청자 사원 플러스파인더
        private void btnPfRequester_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtRequester, (int)Defind_CodeFind.DCF_PERSON, "");
        }

        //private void btnUpdate_Click(object sender, RoutedEventArgs e)
        //{
        //    var AppReq = dgdMain.SelectedItem as Win_App_Approval_U_CodeView;

        //    if (AppReq == null)
        //    {
        //        MessageBox.Show("수정할 데이터를 선택해주세요.");
        //    }
        //    else
        //    {
        //        strFlag = "U";
        //        SaveUpdateMode();
        //        rowNum = dgdMain.SelectedIndex;
        //    }
        //}

        //private void btnDelete_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    Lib.Instance.ChildMenuClose(this.ToString());
        //}

        //private void btnSearch_Click(object sender, RoutedEventArgs e)
        //{
        //    rowNum = 0;
        //    re_Search(rowNum);
        //}

        //private void btnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SaveData(strFlag))
        //    {
        //        CompleteCancelMode();

        //        re_Search(rowNum);
        //        strFlag = "";
        //    }
        //}

        //private void btnCancel_Click(object sender, RoutedEventArgs e)
        //{
        //    this.DataContext = null;
        //    CompleteCancelMode();
        //    strFlag = "";

        //    re_Search(rowNum);
        //}

        //private void btnExcel_Click(object sender, RoutedEventArgs e)
        //{

        //}


        #endregion // Header 부분 - 오른쪽 상단 버튼 이벤트
    }
        
}
