using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using RedAlliance.Erc.Sql;

namespace RedAlliance.Erc
{
    public partial class MainWindow : Window
    {
        public ErcDb DBContext { get; set; }
        public ObservableCollection<Code> TableItems { get; set; }

        public ErcDb GetDataContext()
        {
            return new ErcDb("Data Source=erc_db.sdf;Password=thwerc;DbLinqProvider=SqlCe;DbLinqConnectionType=System.Data.SqlServerCe.SqlCeConnection, System.Data.SqlServerCe");
        }

        public MainWindow()
        {
            InitializeComponent();
            DBContext = GetDataContext();
            TableItems = new ObservableCollection<Code>();
            _table.ItemsSource = TableItems;
            UpdateTable();
        }

        private bool Check(int position, string inputSymbol, string addendum, string result)
        {
            var code = DBContext.Code.FirstOrDefault(x => x.Position == position && x.InputSymbol == inputSymbol);
            if (code == null) return true;

            string outputSymbol = CalculateOutput(position, inputSymbol, addendum, result);
            return outputSymbol == code.OutputSymbol;
        }

        private string CalculateOutput(int position, string inputSymbol, string addendum, string result)
        {
            int outputSymbolNum = Convert.ToInt32(result, 16);
            int addendumNum = Convert.ToInt32(addendum, 16);
            if (outputSymbolNum < addendumNum)
            {
                outputSymbolNum += 16;
            }
            outputSymbolNum -= addendumNum;
            string outputSymbol = outputSymbolNum.ToString("X");

            return outputSymbol;
        }

        private void Write(int position, string inputSymbol, string addendum, string result)
        {
            var code = DBContext.Code.FirstOrDefault(x => x.Position == position && x.InputSymbol == inputSymbol);
            if (code == null)
            {
                string outputSymbol = CalculateOutput(position, inputSymbol, addendum, result);
                
                var codeNew = new Code() { InputSymbol = inputSymbol, Position = position, OutputSymbol = outputSymbol };
                DBContext.Code.InsertOnSubmit(codeNew);
                DBContext.SubmitChanges();
            }
        }

        private void UpdateTable()
        {
            TableItems.Clear();
            DBContext.Code.ToList().ForEach(TableItems.Add);
            Enumerable.Range(0, 16).ToList().ForEach(x => TableItems.Add(new Code() { InputSymbol = x.ToString("X"), OutputSymbol = x.ToString("X") + "=" }));
        }

        private void _btnCheck_Click(object sender, RoutedEventArgs e)
        {
            _tbOutput.Text = "";

            string erc = _tbErc.Text;
            string code = _tbCode.Text;
            if (erc.Length != 16 || code.Length != 8) return;

            for (int i = 0; i < 8; i++)
            {
                if (code[i] == '*') continue;
                if (!Check(i + 1, new string(erc[15 - i], 1), new string(erc[i], 1), new string(code[i], 1)))
                {
                    _tbOutput.Text = "Error on position: " + (i + 1).ToString();
                    return;
                }
            }
            _tbOutput.Text = "OK";

            for (int i = 0; i < 8; i++)
            {
                if (code[i] == '*') continue;
                Write(i + 1, new string(erc[15 - i], 1), new string(erc[i], 1), new string(code[i], 1));
            }
            UpdateTable();
        }

        private void _btnClear_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure?", "Clear all data", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DBContext.ExecuteCommand("DELETE FROM code");
                UpdateTable();
            }
        }
    }
}
