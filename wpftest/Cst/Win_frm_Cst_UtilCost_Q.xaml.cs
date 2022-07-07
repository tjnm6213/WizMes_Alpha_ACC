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
using WizMes_Alpha_JA;
using WizMes_Alpha_JA.PopUP;

namespace WizMes_Alpha_JA
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_frm_Cst_UtilCost_Q : UserControl
    {
        int rowNum;
        string strFlag = "";
        Win_frm_Cst_UtilCost_U_CodeView UtilCost = new Win_frm_Cst_UtilCost_U_CodeView();

        public Win_frm_Cst_UtilCost_Q()
        {
            InitializeComponent();
        }

        // 취소, 저장 후
        private void CanBtnControl()
        {
            Lib.Instance.UiButtonEnableChange_IUControl(this);
            chkYearSrh.IsEnabled = true;
            if (chkYearSrh.IsChecked == true)
            {
                txtYear.IsEnabled = true;
            }
        }

        // 추가, 수정 버튼 클릭시
        private void CantBtnControl()
        {
            Lib.Instance.UiButtonEnableChange_SCControl(this);
            chkYearSrh.IsEnabled = false;
            txtYear.IsEnabled = false;
        }

        #region Header 부분 메서드

        // 기준년도 체크박스
        private void chkYearSrh_Checked(object sender, RoutedEventArgs e)
        {
            txtYear.IsEnabled = true;
        }

        // 기준년도 체크박스
        private void chkYearSrh_UnChecked(object sender, RoutedEventArgs e)
        {
            txtYear.IsEnabled = false;
        }

        // 작년 버튼 클릭 이벤트
        private void btnLastYear_Click(object sender, RoutedEventArgs e)
        {
            txtYear.Text = Lib.Instance.BringLastYearDatetime()[0].ToString("yyyy");
        }
        // 금년 버튼 클릭 이벤트
        private void btnThisYear_Click(object sender, RoutedEventArgs e)
        {
            txtYear.Text = Lib.Instance.BringThisYearDatetime()[1].ToString().Substring(0, 4);
        }

        // 오른쪽 상단 버튼 이벤트
        // 추가 버튼 이벤트
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            strFlag = "I";
            tbkMsg.Text = "자료 추가 중";
            CantBtnControl();
        }
        // 수정 버튼 이벤트
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            strFlag = "U";
            tbkMsg.Text = "자료 수정 중";
            CantBtnControl();
        }
        // 삭제 버튼 이벤트
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var UtilCost = dgdThisYear.SelectedItem as Win_frm_Cst_UtilCost_U_CodeView;

            // 테스트
            // 일단 객체의 컬럼중 하나의 값으로 판별
            if (UtilCost.WaterUseQty != null 
                && !UtilCost.WaterUseQty.Trim().Equals(""))
            {
                if (MessageBox.Show("선택하신 항목을 삭제하시겠습니까?", "삭제전 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (dgdThisYear.Items.Count > 0 && dgdThisYear.SelectedItem != null)
                    {
                        rowNum = dgdThisYear.SelectedIndex;
                    }

                    if (DeleteData(UtilCost.UtilYYYYMM))
                    {
                        if (dgdThisYear.SelectedIndex != 0)
                        {
                            rowNum -= 1;
                        }
                        re_Search(rowNum);
                    }
                }
            }
            else
            {
                MessageBox.Show("삭제할 데이터가 없습니다.");
                return;
            }
        }
        // 닫기 버튼 이벤트
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Lib.Instance.ChildMenuClose(this.ToString());
        }
        // 검색 버튼 이벤트
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            rowNum = 0;
            re_Search(rowNum);
        }
        // 저장 버튼 이벤트
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SaveData(strFlag))
            {
                lblMsg.Visibility = Visibility.Hidden;

                CanBtnControl();

                rowNum = 0;
                dgdThisYear.Items.Clear();
                re_Search(rowNum);
            }
        }
        // 취소 버튼 이벤트
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CanBtnControl();

            rowNum = 0;
            re_Search(rowNum);
        }
        // 엑셀 버튼 이벤트
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = null;
            string Name = string.Empty;

            string[] dgdStr = new string[4];
            dgdStr[0] = "유틸 비용";
            dgdStr[2] = dgdThisYear.Name;

            ExportExcelxaml ExpExc = new ExportExcelxaml(dgdStr);
            ExpExc.ShowDialog();

            if (ExpExc.DialogResult.HasValue)
            {
                if (ExpExc.choice.Equals(dgdThisYear.Name))
                {
                    if (ExpExc.Check.Equals("Y"))
                        dt = Lib.Instance.DataGridToDTinHidden(dgdThisYear);
                    else
                        dt = Lib.Instance.DataGirdToDataTable(dgdThisYear);

                    Name = dgdThisYear.Name;
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

        // 재조회
        private void re_Search(int selectedIndex)
        {
            FillGrid();

            if (dgdThisYear.Items.Count > 0)
            {
                dgdThisYear.SelectedIndex = selectedIndex;
            }
            else
            {
                this.DataContext = null;
            }
        }

        // 조회
        // 설계 : 2019.09.03 곽동운
        // 1. 일단 1월 ~ 12월까지 월만 입력한 빈 행(12개의 행)을 생성
        // 2. DB에서 해당년도 데이터를 가져와서, 월을 숫자로 변환 → 해당하는 빈 월의 행에 데이터 등록 
        // (DB 조회 프로시저 : 오름차순으로(1월, 2월..) 순서로 조회되도록)
        // 3. 마지막 행에 합계 등록
        private void FillGrid()
        {
            if (dgdThisYear.Items.Count > 0)
            {
                dgdThisYear.Items.Clear();
            }

            // 기준년도 가져오기 → 입력하지 않으면 올해 입력
            string stdYear = chkYearSrh.IsChecked == true ? txtYear.Text : "2019";

            // 타이틀 변경 + 기준년도 값 입력
            txtTitle.Text = stdYear + "년 에너지 현황";
            txtYear.Text = stdYear;

            // 12월까지 순서대로 빈 셀 생성
            for (int i = 1; i < 13; i++)
            {
                var UtilCostBlank = new Win_frm_Cst_UtilCost_U_CodeView()
                {
                    Num = i,
                    UtilMM = i + "월",
                    UtilYYYYMM = ""
                };
                dgdThisYear.Items.Add(UtilCostBlank);
            }
            
            try
            {
                if (CheckData())
                {
                    // 합계 구할 객체 선언
                    var SumUtilCost = new SumUtilCost();

                    Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                    sqlParameter.Clear();
                    sqlParameter.Add("UtilYYYY", stdYear);

                    DataSet ds = DataStore.Instance.ProcedureToDataSet("xp_cst_sUtilCost", sqlParameter, false);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];

                        if(dt.Rows.Count > 0)
                        {
                            //int i = 0;
                            DataRowCollection drc = dt.Rows;

                            foreach (DataRow dr in drc)
                            {
                                // 해당하는 월의 인덱스 값 구하기
                                string year = dr["UtilYYYYMM"].ToString().Substring(0, 4);
                                int month = ConvertInt(dr["UtilYYYYMM"].ToString().Substring(4, 2)); // ConvertInt : int로 변환 불가능시, 0으로 반환
                                int index = month - 1;

                                // 혹시 몰라서 → 지정된 값이 아니면, 그냥 데이터그리드에 추가하기
                                if (year.Equals(txtYear.Text) && month > 0 && month < 13)
                                {
                                    var UtilCost = dgdThisYear.Items[index] as Win_frm_Cst_UtilCost_U_CodeView;

                                    UtilCost.UtilYYYYMM = dr["UtilYYYYMM"].ToString();
                                    UtilCost.WaterUseQty = stringFormatN0(dr["WaterUseQty"]);
                                    UtilCost.WaterUseAmount = stringFormatN0(dr["WaterUseAmount"]);
                                    UtilCost.GasUseQty = stringFormatN0(dr["GasUseQty"]);
                                    UtilCost.GasUseAmount = stringFormatN0(dr["GasUseAmount"]);
                                    UtilCost.ElecUseQty = stringFormatN0(dr["ElecUseQty"]);
                                    UtilCost.ElecUseAmount = stringFormatN0(dr["ElecUseAmount"]);
                                    UtilCost.SteamUseQty = stringFormatN0(dr["SteamUseQty"]);
                                    UtilCost.SteamUseAmount = stringFormatN0(dr["SteamUseAmount"]);
                                    UtilCost.SWaterUseQty = stringFormatN0(dr["SWaterUseQty"]);
                                    UtilCost.SWaterUseAmount = stringFormatN0(dr["SWaterUseAmount"]);
                                    UtilCost.WstWaterQty = stringFormatN0(dr["WstWaterQty"]);
                                    UtilCost.WstWaterBoogaQty = stringFormatN0(dr["WstWaterBoogaQty"]);
                                    UtilCost.WstAlcaliAmount = stringFormatN0(dr["WstAlcaliAmount"]);
                                    UtilCost.WstCODQty = stringFormatN0(dr["WstCODQty"]);
                                    UtilCost.WstBODQty = stringFormatN0(dr["WstBODQty"]);
                                    UtilCost.WstOOQty = stringFormatN0(dr["WstOOQty"]);
                                    UtilCost.WstColorQty = stringFormatN0(dr["WstColorQty"]);
                                    UtilCost.WstWaterAmount = stringFormatN0(dr["WstWaterAmount"]);

                                    // 합계 계산
                                    SumUtilCost.WaterUseQty += ConvertDouble(dr["WaterUseQty"].ToString());
                                    SumUtilCost.WaterUseAmount += ConvertDouble(dr["WaterUseAmount"].ToString());
                                    SumUtilCost.GasUseQty += ConvertDouble(dr["GasUseQty"].ToString());
                                    SumUtilCost.GasUseAmount += ConvertDouble(dr["GasUseAmount"].ToString());
                                    SumUtilCost.ElecUseQty += ConvertDouble(dr["ElecUseQty"].ToString());
                                    SumUtilCost.ElecUseAmount += ConvertDouble(dr["ElecUseAmount"].ToString());
                                    SumUtilCost.SteamUseQty += ConvertDouble(dr["SteamUseQty"].ToString());
                                    SumUtilCost.SteamUseAmount += ConvertDouble(dr["SteamUseAmount"].ToString());
                                    SumUtilCost.SWaterUseQty += ConvertDouble(dr["SWaterUseQty"].ToString());
                                    SumUtilCost.SWaterUseAmount += ConvertDouble(dr["SWaterUseAmount"].ToString());
                                    SumUtilCost.WstWaterQty += ConvertDouble(dr["WstWaterQty"].ToString());
                                    SumUtilCost.WstWaterBoogaQty += ConvertDouble(dr["WstWaterBoogaQty"].ToString());
                                    SumUtilCost.WstAlcaliAmount += ConvertDouble(dr["WstAlcaliAmount"].ToString());
                                    SumUtilCost.WstCODQty += ConvertDouble(dr["WstCODQty"].ToString());
                                    SumUtilCost.WstBODQty += ConvertDouble(dr["WstBODQty"].ToString());
                                    SumUtilCost.WstOOQty += ConvertDouble(dr["WstOOQty"].ToString());
                                    SumUtilCost.WstColorQty += ConvertDouble(dr["WstColorQty"].ToString());
                                    SumUtilCost.WstWaterAmount += ConvertDouble(dr["WstWaterAmount"].ToString());
                                }
                                else // 지정된 값이 아닌 이상한 값이면.. 12월 ~ 합계 사이에 추가 (삭제가능 하도록?) 테스트 : 아니면 바로 삭제를 시켜야 하나??
                                {

                                    var DummyUtilCost = new Win_frm_Cst_UtilCost_U_CodeView()
                                    {
                                        
                                        UtilYYYYMM = dr["UtilYYYYMM"].ToString(),
                                        WaterUseQty = stringFormatN0(dr["WaterUseQty"]),
                                        WaterUseAmount = stringFormatN0(dr["WaterUseAmount"]),
                                        GasUseQty = stringFormatN0(dr["GasUseQty"]),
                                        GasUseAmount = stringFormatN0(dr["GasUseAmount"]),
                                        ElecUseQty = stringFormatN0(dr["ElecUseQty"]),
                                        ElecUseAmount = stringFormatN0(dr["ElecUseAmount"]),
                                        SteamUseQty = stringFormatN0(dr["SteamUseQty"]),
                                        SteamUseAmount = stringFormatN0(dr["SteamUseAmount"]),
                                        SWaterUseQty = stringFormatN0(dr["SWaterUseQty"]),
                                        SWaterUseAmount = stringFormatN0(dr["SWaterUseAmount"]),
                                        WstWaterQty = stringFormatN0(dr["WstWaterQty"]),
                                        WstWaterBoogaQty = stringFormatN0(dr["WstWaterBoogaQty"]),
                                        WstAlcaliAmount = stringFormatN0(dr["WstAlcaliAmount"]),
                                        WstCODQty = stringFormatN0(dr["WstCODQty"]),
                                        WstBODQty = stringFormatN0(dr["WstBODQty"]),
                                        WstOOQty = stringFormatN0(dr["WstOOQty"]),
                                        WstColorQty = stringFormatN0(dr["WstColorQty"]),
                                        WstWaterAmount = stringFormatN0(dr["WstWaterAmount"])
                                    };

                                    dgdThisYear.Items.Add(DummyUtilCost);

                                }
                            }
                        }
                    }
                    SumUtilCost.UtilMM = "합계";
                    dgdThisYear.Items.Add(SumUtilCost);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }
        }

        // 체크 데이터
        private bool CheckData()
        {
            bool flag = true;

            // 수정시 체크
            if (strFlag.Equals("U") || strFlag.Equals("I"))
            {
                //for(int i = 0; i < 12; i++)
                //{
                //    var UtilCost = dgdThisYear.Items[i] as Win_frm_Cst_UtilCost_U_CodeView;

                //    DataGrid item = dgdThisYear.Items[i] as DataGrid;
                //    MessageBox.Show(item.Columns[0].ToString());
                    
                //}
            }
            else
            {
                // 검색할때 사용
                // 기준년도 체크 ON → 빈칸 체크 / 4자리 숫자 체크
                int chkInt = 0;
                if (chkYearSrh.IsChecked == true)
                {
                    if (txtYear.Text == null || txtYear.Text.Trim().Equals(""))
                    {
                        MessageBox.Show("기준년도가 입력되지 않았습니다.");
                        flag = false;
                        return flag;
                    }
                    else if (txtYear.Text == null || txtYear.Text.Length != 4
                        || Int32.TryParse(txtYear.Text, out chkInt) == false)
                    {
                        MessageBox.Show("기준년도는 4자리의 숫자만 입력 가능합니다.");
                        flag = false;
                        return flag;
                    }
                }
            }

            return flag;
        }

        // 저장
        private bool SaveData(string strFlag)
        {
            bool flag = false;
            List<Procedure> Prolist = new List<Procedure>();
            List<Dictionary<string, object>> ListParameter = new List<Dictionary<string, object>>();

            try
            {

                if (strFlag.Equals("U")) // 수정버튼밖에 없음
                {
                    // 1월부터 순서대로 데이터를 가져와서 1~12월 데이터 값 등록
                    for(int i = 0; i < 12; i++)
                    {
                        if (CheckData())
                        {
                            Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                            sqlParameter.Clear();

                            var UtilCost = dgdThisYear.Items[i] as Win_frm_Cst_UtilCost_U_CodeView;

                            // 해당 행에 숫자 유효성 검사
                            if (CheckNumberUtilCost(UtilCost) == false)
                            {
                                dgdThisYear.SelectedIndex = i;
                                flag = false;
                                return flag;
                            }

                            // UtilYYYYMM 가 존재하지 않으면 추가 / 존재하면 수정
                            if (UtilCost.UtilYYYYMM.Equals(""))
                            {
                                // 행의 모든 컬럼에 값이 하나도 없다면, 추가 안함
                                if (UtilCost.WaterUseQty == null
                                    && UtilCost.WaterUseAmount == null
                                    && UtilCost.GasUseQty == null
                                    && UtilCost.GasUseAmount == null
                                    && UtilCost.ElecUseQty == null
                                    && UtilCost.ElecUseAmount == null
                                    && UtilCost.SteamUseQty == null
                                    && UtilCost.SteamUseAmount == null
                                    && UtilCost.SWaterUseQty == null
                                    && UtilCost.SWaterUseAmount == null
                                    && UtilCost.WstWaterQty == null
                                    && UtilCost.WstWaterBoogaQty == null
                                    && UtilCost.WstAlcaliAmount == null
                                    && UtilCost.WstCODQty == null
                                    && UtilCost.WstBODQty == null
                                    && UtilCost.WstOOQty == null
                                    && UtilCost.WstColorQty == null
                                    && UtilCost.WstWaterAmount == null)
                                {
                                    continue;
                                }

                                // YYYYMM 값 구하기 (2019년, i : 0 → 201901)
                                UtilCost.UtilYYYYMM = txtYear.Text + string.Format("{0:D2}", i + 1);

                                sqlParameter.Add("UtilYYYYMM", UtilCost.UtilYYYYMM);
                                sqlParameter.Add("WaterUseQty", UtilCost.WaterUseQty == null || UtilCost.WaterUseQty.Trim().Equals("") ? "0" : UtilCost.WaterUseQty.Replace(",", ""));
                                sqlParameter.Add("WaterUseAmount", UtilCost.WaterUseAmount == null || UtilCost.WaterUseAmount.Trim().Equals("") ? "0" : UtilCost.WaterUseAmount.Replace(",", ""));
                                sqlParameter.Add("GasUseQty", UtilCost.GasUseQty == null || UtilCost.GasUseQty.Trim().Equals("") ? "0" : UtilCost.GasUseQty.Replace(",", ""));
                                sqlParameter.Add("GasUseAmount", UtilCost.GasUseAmount == null || UtilCost.GasUseAmount.Trim().Equals("") ? "0" : UtilCost.GasUseAmount.Replace(",", ""));
                                sqlParameter.Add("ElecUseQty", UtilCost.ElecUseQty == null || UtilCost.ElecUseQty.Trim().Equals("") ? "0" : UtilCost.ElecUseQty.Replace(",", ""));
                                sqlParameter.Add("ElecUseAmount", UtilCost.ElecUseAmount == null || UtilCost.ElecUseAmount.Trim().Equals("") ? "0" : UtilCost.ElecUseAmount.Replace(",", ""));
                                sqlParameter.Add("SteamUseQty", UtilCost.SteamUseQty == null || UtilCost.SteamUseQty.Trim().Equals("") ? "0" : UtilCost.SteamUseQty.Replace(",", ""));
                                sqlParameter.Add("SteamUseAmount", UtilCost.SteamUseAmount == null || UtilCost.SteamUseAmount.Trim().Equals("") ? "0" : UtilCost.SteamUseAmount.Replace(",", ""));
                                sqlParameter.Add("SWaterUseQty", UtilCost.SWaterUseQty == null || UtilCost.SWaterUseQty.Trim().Equals("") ? "0" : UtilCost.SWaterUseQty.Replace(",", ""));
                                sqlParameter.Add("SWaterUseAmount", UtilCost.SWaterUseAmount == null || UtilCost.SWaterUseAmount.Trim().Equals("") ? "0" : UtilCost.SWaterUseAmount.Replace(",", ""));
                                sqlParameter.Add("WstWaterQty", UtilCost.WstWaterQty == null || UtilCost.WstWaterQty.Trim().Equals("") ? "0" : UtilCost.WstWaterQty.Replace(",", ""));
                                sqlParameter.Add("WstWaterBoogaQty", UtilCost.WstWaterBoogaQty == null || UtilCost.WstWaterBoogaQty.Trim().Equals("") ? "0" : UtilCost.WstWaterBoogaQty.Replace(",", ""));
                                sqlParameter.Add("WstAlcaliAmount", UtilCost.WstAlcaliAmount == null || UtilCost.WstAlcaliAmount.Trim().Equals("") ? "0" : UtilCost.WstAlcaliAmount.Replace(",", ""));
                                sqlParameter.Add("WstCODQty", UtilCost.WstCODQty == null || UtilCost.WstCODQty.Trim().Equals("") ? "0" : UtilCost.WstCODQty.Replace(",", ""));
                                sqlParameter.Add("WstBODQty", UtilCost.WstBODQty == null || UtilCost.WstBODQty.Trim().Equals("") ? "0" : UtilCost.WstBODQty.Replace(",", ""));
                                sqlParameter.Add("WstOOQty", UtilCost.WstOOQty == null || UtilCost.WstOOQty.Trim().Equals("") ? "0" : UtilCost.WstOOQty.Replace(",", ""));
                                sqlParameter.Add("WstColorQty ", UtilCost.WstColorQty == null || UtilCost.WstColorQty.Trim().Equals("") ? "0" : UtilCost.WstColorQty.Replace(",", ""));
                                sqlParameter.Add("WstWaterAmount", UtilCost.WstWaterAmount == null || UtilCost.WstWaterAmount.Trim().Equals("") ? "0" : UtilCost.WstWaterAmount.Replace(",", ""));
                                sqlParameter.Add("CreateUserID", MainWindow.CurrentUser);

                                Procedure pro2 = new Procedure();
                                pro2.Name = "xp_CST_iUtilCost";
                                pro2.OutputUseYN = "N";
                                pro2.OutputName = "WorkID";
                                pro2.OutputLength = "4";

                                Prolist.Add(pro2);
                                ListParameter.Add(sqlParameter);
                            }
                            else
                            {
                                sqlParameter.Add("UtilYYYYMM", UtilCost.UtilYYYYMM);
                                sqlParameter.Add("WaterUseQty", UtilCost.WaterUseQty == null || UtilCost.WaterUseQty.Trim().Equals("") ? "0" : UtilCost.WaterUseQty.Replace(",", ""));
                                sqlParameter.Add("WaterUseAmount", UtilCost.WaterUseAmount == null || UtilCost.WaterUseAmount.Trim().Equals("") ? "0" : UtilCost.WaterUseAmount.Replace(",", ""));
                                sqlParameter.Add("GasUseQty", UtilCost.GasUseQty == null || UtilCost.GasUseQty.Trim().Equals("") ? "0" : UtilCost.GasUseQty.Replace(",", ""));
                                sqlParameter.Add("GasUseAmount", UtilCost.GasUseAmount == null || UtilCost.GasUseAmount.Trim().Equals("") ? "0" : UtilCost.GasUseAmount.Replace(",", ""));
                                sqlParameter.Add("ElecUseQty", UtilCost.ElecUseQty == null || UtilCost.ElecUseQty.Trim().Equals("") ? "0" : UtilCost.ElecUseQty.Replace(",", ""));
                                sqlParameter.Add("ElecUseAmount", UtilCost.ElecUseAmount == null || UtilCost.ElecUseAmount.Trim().Equals("") ? "0" : UtilCost.ElecUseAmount.Replace(",", ""));
                                sqlParameter.Add("SteamUseQty", UtilCost.SteamUseQty == null || UtilCost.SteamUseQty.Trim().Equals("") ? "0" : UtilCost.SteamUseQty.Replace(",", ""));
                                sqlParameter.Add("SteamUseAmount", UtilCost.SteamUseAmount == null || UtilCost.SteamUseAmount.Trim().Equals("") ? "0" : UtilCost.SteamUseAmount.Replace(",", ""));
                                sqlParameter.Add("SWaterUseQty", UtilCost.SWaterUseQty == null || UtilCost.SWaterUseQty.Trim().Equals("") ? "0" : UtilCost.SWaterUseQty.Replace(",", ""));
                                sqlParameter.Add("SWaterUseAmount", UtilCost.SWaterUseAmount == null || UtilCost.SWaterUseAmount.Trim().Equals("") ? "0" : UtilCost.SWaterUseAmount.Replace(",", ""));
                                sqlParameter.Add("WstWaterQty", UtilCost.WstWaterQty == null || UtilCost.WstWaterQty.Trim().Equals("") ? "0" : UtilCost.WstWaterQty.Replace(",", ""));
                                sqlParameter.Add("WstWaterBoogaQty", UtilCost.WstWaterBoogaQty == null || UtilCost.WstWaterBoogaQty.Trim().Equals("") ? "0" : UtilCost.WstWaterBoogaQty.Replace(",", ""));
                                sqlParameter.Add("WstAlcaliAmount", UtilCost.WstAlcaliAmount == null || UtilCost.WstAlcaliAmount.Trim().Equals("") ? "0" : UtilCost.WstAlcaliAmount.Replace(",", ""));
                                sqlParameter.Add("WstCODQty", UtilCost.WstCODQty == null || UtilCost.WstCODQty.Trim().Equals("") ? "0" : UtilCost.WstCODQty.Replace(",", ""));
                                sqlParameter.Add("WstBODQty", UtilCost.WstBODQty == null || UtilCost.WstBODQty.Trim().Equals("") ? "0" : UtilCost.WstBODQty.Replace(",", ""));
                                sqlParameter.Add("WstOOQty", UtilCost.WstOOQty == null || UtilCost.WstOOQty.Trim().Equals("") ? "0" : UtilCost.WstOOQty.Replace(",", ""));
                                sqlParameter.Add("WstColorQty ", UtilCost.WstColorQty == null || UtilCost.WstColorQty.Trim().Equals("") ? "0" : UtilCost.WstColorQty.Replace(",", ""));
                                sqlParameter.Add("WstWaterAmount", UtilCost.WstWaterAmount == null || UtilCost.WstWaterAmount.Trim().Equals("") ? "0" : UtilCost.WstWaterAmount.Replace(",", ""));
                                sqlParameter.Add("UserID", MainWindow.CurrentUser);

                                Procedure pro2 = new Procedure();
                                pro2.Name = "xp_CST_uUtilCost";
                                pro2.OutputUseYN = "N";
                                pro2.OutputName = "WorkID";
                                pro2.OutputLength = "4";

                                Prolist.Add(pro2);
                                ListParameter.Add(sqlParameter);
                            }
                        }
                    }

                    string[] result = new string[2];
                    result = DataStore.Instance.ExecuteAllProcedureOutputNew(Prolist, ListParameter);
                    if (result[0] != "success")
                    {
                        MessageBox.Show("저장실패 " + result[1].ToString());
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오류 발생, 오류 내용 : " + ex.Message);
            }
            finally
            {
                DataStore.Instance.CloseConnection();
            }

            return flag;
        }

        private bool CheckNumberUtilCost(Win_frm_Cst_UtilCost_U_CodeView UtilCost)
        {
            bool flag = true;

            if (chkConvertDouble(UtilCost.WaterUseQty) == false
                || chkConvertDouble(UtilCost.WaterUseAmount) == false
                || chkConvertDouble(UtilCost.GasUseQty) == false
                || chkConvertDouble(UtilCost.GasUseAmount) == false
                || chkConvertDouble(UtilCost.ElecUseQty) == false
                || chkConvertDouble(UtilCost.ElecUseAmount) == false
                || chkConvertDouble(UtilCost.SteamUseQty) == false
                || chkConvertDouble(UtilCost.SteamUseAmount) == false
                || chkConvertDouble(UtilCost.SWaterUseQty) == false
                || chkConvertDouble(UtilCost.SWaterUseAmount) == false
                || chkConvertDouble(UtilCost.WstWaterQty) == false
                || chkConvertDouble(UtilCost.WstWaterBoogaQty) == false
                || chkConvertDouble(UtilCost.WstAlcaliAmount) == false
                || chkConvertDouble(UtilCost.WstCODQty) == false
                || chkConvertDouble(UtilCost.WstBODQty) == false
                || chkConvertDouble(UtilCost.WstOOQty) == false
                || chkConvertDouble(UtilCost.WstColorQty) == false
                || chkConvertDouble(UtilCost.WstWaterAmount) == false)
            {
                MessageBox.Show("숫자만 입력이 가능합니다.");
                flag = false;
            }

            return flag;
        }

        // Double 로 형변환이 가능한지 체크
        private bool chkConvertDouble(string str)
        {
            bool flag = true;
            double chkDouble = 0;

            if (Double.TryParse(str, out chkDouble) == false)
            {
                flag = false;
            }

            return flag;
        }

        // 삭제
        private bool DeleteData(string strID)
        {
            bool flag = false;

            try
            {
                Dictionary<string, object> sqlParameter = new Dictionary<string, object>();
                sqlParameter.Clear();
                sqlParameter.Add("UtilYYYYMM", strID);

                string[] result = DataStore.Instance.ExecuteProcedure("xp_CST_dUtilCost", sqlParameter, false);

                if (result[0].Equals("success"))
                {
                    //MessageBox.Show("굳");
                    flag = true;
                }
                else
                {
                    MessageBox.Show("삭제 실패 : " + result[1]);
                    flag = false;
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

            return flag;
        }

        // int로 형변환
        private int ConvertInt(string str)
        {
            int result = 0;
            int chkInt = 0;

            if (str != null && !str.Equals(""))
            {
                if (Int32.TryParse(str, out chkInt) == true)
                {
                    // 숫자가 001 과 같을 경우 → 0 제거
                    if (str.Substring(0, 1).Equals("0"))
                    {
                        str = str.TrimStart('0');

                        if (str.Equals("")) { str = "0"; }
                    }
                    result = Int32.Parse(str);
                }
                else
                {
                    //MessageBox.Show("Int로 변환이 불가능 합니다.");
                }
            }

            return result;
        }

        // double로 형변환
        private double ConvertDouble(string str)
        {
            double result = 0;
            double chkDouble = 0;

            if (str != null && !str.Equals(""))
            {
                if (Double.TryParse(str, out chkDouble) == true)
                {
                    result = Double.Parse(str);
                }
                else
                {
                    //MessageBox.Show("Double로 변환이 불가능 합니다.");
                }
            }

            return result;
        }

        // 천마리 콤마, 소수점 버리기
        private string stringFormatN0(object obj)
        {
            return string.Format("{0:N0}", obj);
        }
    }
}
