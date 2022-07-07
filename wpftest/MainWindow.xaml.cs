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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.MDI;

namespace wpftest
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        private static MdiChild userContorl1 = new MdiChild()
        {
            Title = "test1",
            Height = 300,
            Width = 300,
            //Here CustomerMaster is the class that you have created for mainWindow.xaml user control.
            Content = new UserControl1()
        };

        private static MdiChild userContorl2 = new MdiChild()
        {
            Title = " Child Window",
            Height = 300,
            Width = 300,
            //Here CustomerMaster is the class that you have created for mainWindow.xaml user control.
            Content = new UserControl2()
        };

    public MainWindow()
        {
            InitializeComponent();
        }

        private void submenu1Customer_Click(object sender, RoutedEventArgs e)
        {
            List<MdiChild> list= MainMdiContainer.Children.ToList();
            if (list.Contains(userContorl1))
                return;
            else
                MainMdiContainer.Children.Add(userContorl1);
        }

        private void submenu2Customer_Click(object sender, RoutedEventArgs e)
        {
            List<MdiChild> list = MainMdiContainer.Children.ToList();
            if (list.Contains(userContorl2))
                return;
            else
                MainMdiContainer.Children.Add(userContorl2);
        }
    }
}
