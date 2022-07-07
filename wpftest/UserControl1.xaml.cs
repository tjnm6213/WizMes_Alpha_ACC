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

namespace wpftest
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        #region cmdOprate search
        private void search_Button_Click_1(object sender, RoutedEventArgs e)
        {
            var data = new Test { Test1 = "Test1", Test2 = "Test2" };

            DataGridTest.Items.Add(data);
        }

        private void update_Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void save_Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void delete_Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        private void fillGrid()
        {
            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
            sqlParameter.Add("@sCustom", "");

            DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_Custom_sCustom", sqlParameter, false);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count == 0)
                {
                    DataRow dr = dt.NewRow();
                    dr[com_WorkShop.WORKSHOPCODE] = "-";
                    dr[com_WorkShop.WORKSHOPNAME] = "-";
                    dt.Rows.Add(dr);
                    cboWorkshop.DataSource = dt;
                    cboWorkshop.SelectedValue = "-";
                }
                //else if (dt.Rows.Count == 1)
                //{
                //    cboWorkshop.DataSource = dt;
                //    cboWorkshop.SelectedValue = dt.Rows[0][com_WorkShop.WORKSHOPCODE];
                //}
                else
                {
                    DataRow dr = dt.NewRow();
                    dr[com_WorkShop.WORKSHOPCODE] = "*";
                    dr[com_WorkShop.WORKSHOPNAME] = "전체";
                    dt.Rows.InsertAt(dr, 0);
                    cboWorkshop.DataSource = dt;


                    if (backup != null && dt.AsEnumerable().Count(p => p["WorkshopCode"].Equals(backup)) > 0)
                    {
                        cboWorkshop.SelectedValue = backup;
                    }
                    else
                    {
                        cboWorkshop.SelectedValue = "*";
                    }
                }
            }
        }

    }

    public class Test
    {
        public string Test1 { get; set; }
        public string Test2 { get; set; }
    }
}
