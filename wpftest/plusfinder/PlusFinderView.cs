using WizMes_Alpha_JA;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WizMes_Alpha_JA
{
    public class PlusFinderView : BaseView
    {
        public PlusFinderView()
        {
        }

        public ObservableCollection<CodeView> cboTrade { get; set; }

        public string m_sCodeField { get; set; }
        public string m_sNameField { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }
}
