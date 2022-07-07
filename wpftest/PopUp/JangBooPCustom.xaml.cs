using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// ChoiceAorB.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class JangBooPCustom : Window
    {
        public string Wh_Ar_BSGbn = string.Empty;           // 매입 / 매출 구분자
        public int Wh_Ar_ChkCustom = 0;                     // 거래처 선택 유무
        public string Wh_Ar_CustomID = string.Empty;        // 거래처 ID

        public JangBooPCustom()
        {
            InitializeComponent();
        }

        // 첫 로드시.
        private void JangBooPCustom_Loaded(object sender, RoutedEventArgs e)
        {
            //선택없이 바로 확인할 수 있도록 기본 클릭 세팅 잡아줍니다.
            A_Button_Click(null, null);
            lblAllCustom_MouseLeftButtonUp(null, null);
        }

        #region (각종 선택클릭으로 인한 기본세팅)

        // 매입버튼 클릭.
        private void A_Button_Click(object sender, RoutedEventArgs e)
        {
            A_Button.Background = new SolidColorBrush(Colors.LightGreen);
            B_Button.Background = new SolidColorBrush(Colors.LightGray);
            Wh_Ar_BSGbn = "1";
        }
        // 매출버튼 클릭.
        private void B_Button_Click(object sender, RoutedEventArgs e)
        {
            B_Button.Background = new SolidColorBrush(Colors.LightGreen);
            A_Button.Background = new SolidColorBrush(Colors.LightGray);
            Wh_Ar_BSGbn = "2";
        }


        // 전체 거래처
        private void lblAllCustom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkAllCustom.IsChecked == true)
            {
                chkAllCustom.IsChecked = false;
                chkCustom.IsChecked = true;
            }
            else
            {
                chkAllCustom.IsChecked = true;
                chkCustom.IsChecked = false;
            }
        }
        // 전체 거래처
        private void chkAllCustom_Checked(object sender, RoutedEventArgs e)
        {
            chkCustom.IsChecked = false;
        }
        // 전체 거래처
        private void chkAllCustom_Unchecked(object sender, RoutedEventArgs e)
        {
            chkCustom.IsChecked = true;
        }
        // 개별 거래처
        private void lblCustom_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (chkCustom.IsChecked == true)
            {
                chkCustom.IsChecked = false;
                chkAllCustom.IsChecked = true;
            }
            else
            {
                chkCustom.IsChecked = true;
                chkAllCustom.IsChecked = false;
            }
        }
        // 개별 거래처
        private void chkCustom_Checked(object sender, RoutedEventArgs e)
        {
            chkAllCustom.IsChecked = false;

            txtCustom.IsEnabled = true;
            btnPfCustom.IsEnabled = true;
            txtCustom.Focus();
        }
        // 개별 거래처
        private void chkCustom_Unchecked(object sender, RoutedEventArgs e)
        {
            chkAllCustom.IsChecked = true;

            txtCustom.IsEnabled = false;
            btnPfCustom.IsEnabled = false;
        }

        // 플러스파인더 >> 거래처.
        private void btnPfCustom_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
        }
        // 플러스파인더 >> 거래처
        private void txtCustom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.pf.ReturnCode(txtCustom, (int)Defind_CodeFind.DCF_CUSTOM, "");
            }
        }


        #endregion


        // 확인버튼 클릭.
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (chkAllCustom.IsChecked == true)
            {
                Wh_Ar_ChkCustom = 0;
                Wh_Ar_CustomID = string.Empty;
            }
            else if (chkCustom.IsChecked == true)
            {
                Wh_Ar_ChkCustom = 1;
                Wh_Ar_CustomID = txtCustom.Tag.ToString();
            }
            DialogResult = true;
        }


        // 닫기버튼 클릭.
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //소득없이 이대로 끝.
            DialogResult = false;
        }

        
    }
}
