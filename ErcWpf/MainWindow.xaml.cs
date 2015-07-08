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
        public ObservableCollection<Code> TableItems { get; set; }

        private List<KeyValuePair<int, int>> _dictSpecialPairs = new List<KeyValuePair<int, int>>() 
        { 
            new KeyValuePair<int, int>(0xA, 4),
            new KeyValuePair<int, int>(2, 6), 
            new KeyValuePair<int, int>(1, 0xF),
            new KeyValuePair<int, int>(2, 5), 
            new KeyValuePair<int, int>(6, 7), 
            new KeyValuePair<int, int>(4, 0xB), 
            new KeyValuePair<int, int>(5, 9), 
            new KeyValuePair<int, int>(0xD, 9)
        };

        private List<KeyValuePair<int, int>> _dictBadPairs = new List<KeyValuePair<int, int>>() 
        { 
            new KeyValuePair<int, int>(0xA, 7),
            new KeyValuePair<int, int>(0xC, 1), 
            new KeyValuePair<int, int>(0, 4),
            new KeyValuePair<int, int>(5, 3), 
            new KeyValuePair<int, int>(8, 2), 
            new KeyValuePair<int, int>(5, 8), 
            new KeyValuePair<int, int>(5, 4), 
            new KeyValuePair<int, int>(7, 0xD),
            new KeyValuePair<int, int>(4, 7),
            new KeyValuePair<int, int>(7, 4), 
            new KeyValuePair<int, int>(7, 6),
            new KeyValuePair<int, int>(0xD, 0xC), 
            new KeyValuePair<int, int>(5, 6), 
            new KeyValuePair<int, int>(6, 5), 
            new KeyValuePair<int, int>(7, 7), 
            new KeyValuePair<int, int>(0xD, 3),
            new KeyValuePair<int, int>(0xF, 8), 
            new KeyValuePair<int, int>(7, 5)
        };

        public bool IsMultipleMode
        {
            get
            {
                return _cbMultiple.IsChecked != true;
            }
        }

        public ErcDb GetDataContext()
        {
            return new ErcDb("Data Source=erc_db.sdf;Password=thwerc;DbLinqProvider=SqlCe;DbLinqConnectionType=System.Data.SqlServerCe.SqlCeConnection, System.Data.SqlServerCe");
        }

        public MainWindow()
        {
            InitializeComponent();
            TableItems = new ObservableCollection<Code>();
            _table.ItemsSource = TableItems;
            UpdateTable();
        }

        private void CheckWriteMultiple(int position, string inputSymbol, string addendum, string result)
        {
            using (var dbContext = GetDataContext())
            {
                string outputSymbol = CalculateOutput(position, inputSymbol, addendum, result);
                if (outputSymbol == null) return;

                var code = dbContext.Code.FirstOrDefault(x => x.Position == position && x.InputSymbol == inputSymbol);
                if (code == null)
                {
                    var codeNew = new Code() { InputSymbol = inputSymbol, Position = position, OutputSymbol = outputSymbol };
                    dbContext.Code.InsertOnSubmit(codeNew);
                    dbContext.SubmitChanges();
                }
                else if (!code.OutputSymbol.Contains(outputSymbol))
                {
                    code.OutputSymbol += outputSymbol;
                    dbContext.SubmitChanges();
                }
            }
        }

        private bool Check(int position, string inputSymbol, string addendum, string result)
        {
            using (var dbContext = GetDataContext())
            {
                var code = dbContext.Code.FirstOrDefault(x => x.Position == position && x.InputSymbol == inputSymbol);
                if (code == null) return true;

                string outputSymbol = CalculateOutput(position, inputSymbol, addendum, result);
                if (outputSymbol == null) return true;

                return code.OutputSymbol.Contains(outputSymbol);
            }
        }

        private string CalculateOutput(int position, string inputSymbol, string addendum, string result)
        {
            int outputSymbolNum = Convert.ToInt32(result, 16);
            int addendumNum = Convert.ToInt32(addendum, 16);
            int inputSymbolNum = Convert.ToInt32(inputSymbol, 16);

            if (_dictBadPairs.Any(x => x.Key == addendumNum && x.Value == inputSymbolNum)) return null;

            if (_dictSpecialPairs.Any(x => x.Key == addendumNum && x.Value == inputSymbolNum))
            {
                outputSymbolNum = (outputSymbolNum + addendumNum) % 16;
            }
            else
            {
                if (outputSymbolNum < addendumNum)
                {
                    outputSymbolNum += 16;
                }
                outputSymbolNum -= addendumNum;
            }

            string outputSymbol = outputSymbolNum.ToString("X");

            return outputSymbol;
        }

        private void Write(int position, string inputSymbol, string addendum, string result)
        {
            using (var dbContext = GetDataContext())
            {
                var code = dbContext.Code.FirstOrDefault(x => x.Position == position && x.InputSymbol == inputSymbol);
                if (code == null)
                {
                    string outputSymbol = CalculateOutput(position, inputSymbol, addendum, result);
                    if (outputSymbol == null) return;

                    var codeNew = new Code() { InputSymbol = inputSymbol, Position = position, OutputSymbol = outputSymbol };
                    dbContext.Code.InsertOnSubmit(codeNew);
                    dbContext.SubmitChanges();
                }
            }
        }

        private void UpdateTable()
        {
            using (var dbContext = GetDataContext())
            {
                TableItems.Clear();
                dbContext.Code.ToList().ForEach(TableItems.Add);
                Enumerable.Range(0, 16).ToList().ForEach(x => TableItems.Add(new Code() { InputSymbol = x.ToString("X"), OutputSymbol = x.ToString("X") + "=" }));
            }
        }

        string GetFullBinary(string str, int count)
        {
            while (str.Length < count)
            {
                str = "0" + str;
            }
            return str;
        }

        private void AddBitsRow(int num)
        {
            _pntBits.Children.Add(new TextBlock() { Text = GetFullBinary(Convert.ToString(num, 2), 32) + " " + GetFullBinary(num.ToString("X"), 8) });
        }

        private int GetReverse(int x)
        {
            int reverseSecondPart = 0;

            for (int i = 0; i < 32; i++)
            {
                int temp = x >> (31 - i);
                temp &= 1;
                temp = temp << i;
                reverseSecondPart |= temp;
            }

            return reverseSecondPart;
        }

        private void _btnCheck_Click(object sender, RoutedEventArgs e)
        {
            _tbOutput.Text = "";

            var parts = _tbErc.Text.Split('-');
            string erc = parts.First().Trim();
            string code = parts.Last().Trim();
            if (erc.Length != 16 || code.Length != 8) return;

            int firstPart = Convert.ToInt32(erc.Substring(0, 8), 16);
            int secondPart = Convert.ToInt32(erc.Substring(8), 16);
            int codeBin = Convert.ToInt32(code, 16);

            int reverseSecondPart = GetReverse(secondPart);

            AddBitsRow(firstPart);
            AddBitsRow(reverseSecondPart);            
            int xor = firstPart ^ reverseSecondPart;
            int result = xor - 0xE010A11;
            AddBitsRow(xor);
            AddBitsRow(result);
            AddBitsRow(codeBin);
            _pntBits.Children.Add(new TextBlock());

            return;

            if (IsMultipleMode)
            {
                for (int i = 0; i < 8; i++)
                {
                    //if (i >= 2 && i <= 5) continue;
                    if (code[i] == '*') continue;
                    CheckWriteMultiple(i + 1, new string(erc[15 - i], 1), new string(erc[i], 1), new string(code[i], 1));
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    //if (i >= 2 && i <= 5) continue;
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
                    //if (i >= 2 && i <= 5) continue;
                    if (code[i] == '*') continue;
                    Write(i + 1, new string(erc[15 - i], 1), new string(erc[i], 1), new string(code[i], 1));
                }
            }
            UpdateTable();
        }

        private void _btnClear_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure?", "Clear all data", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (var dbContext = GetDataContext())
                {
                    dbContext.ExecuteCommand("DELETE FROM code");
                }
                GenerateColumns();
                UpdateTable();
            }
        }

        private void GenerateColumns()
        {
            int[] cells = { 0, 8, 4, 0xC, 2, 0xA, 6, 0xE, 1, 9, 5, 0xD, 3, 0xB, 7, 0xF };
            int?[] constants = { 0xF, 2, 0, null, null, 6, 0xF, 0xF };

            using (var dbContext = GetDataContext())
            {
                for (int i = 0; i < constants.Count(); i++)
                {
                    if (!constants[i].HasValue) continue;
                    for (int j = 0; j < cells.Count(); j++)
                    {
                        int outputSymbolNum = (cells[j] + constants[i].Value) % 16;
                        var code = new Code()
                        {
                            InputSymbol = j.ToString("X"),
                            Position = i + 1,
                            OutputSymbol = outputSymbolNum.ToString("X")
                        };
                        dbContext.Code.InsertOnSubmit(code);
                    }
                }
                dbContext.SubmitChanges();
            }
        }

        private void _btnEncode_Click(object sender, RoutedEventArgs e)
        {
            _tbOutputEnc.Text = "";
            string erc = _tbErcEnc.Text.Trim();
            if (erc.Length != 16) return;

            string result = "";

            int firstPart = Convert.ToInt32(erc.Substring(0, 8), 16);
            int secondPart = Convert.ToInt32(erc.Substring(8), 16);
            int reverseSecondPart = GetReverse(secondPart);

            AddBitsRow(firstPart);
            AddBitsRow(reverseSecondPart);
            int xor = firstPart ^ reverseSecondPart;
            int ret = xor - 0xE010A11;
            _tbOutputEnc.Text = GetFullBinary(ret.ToString("X"), 8);

            return;

            using (var dbContext = GetDataContext())
            {
                for (int i = 0; i < 8; i++)
                {
                    string inputSymbol = new string(erc[15 - i], 1);
                    string addendum = new string(erc[i], 1);
                    int position = i + 1;
                    var code = dbContext.Code.FirstOrDefault(x => x.Position == position && x.InputSymbol == inputSymbol);

                    if (code == null)
                    {
                        result += "*";
                    }
                    else
                    {
                        int addendumNum = Convert.ToInt32(addendum, 16);
                        if (code.OutputSymbol.Length > 1)
                        {
                            result += "(";
                        }

                        foreach (char c in code.OutputSymbol)
                        {
                            string outputSymbol = new string(c, 1);
                            int outputSymbolNum = Convert.ToInt32(outputSymbol, 16);
                            int inputSymbolNum = Convert.ToInt32(inputSymbol, 16);
                            int resultSymbolNum = 0;

                            if (_dictSpecialPairs.Any(x => x.Key == addendumNum && x.Value == inputSymbolNum))
                            {
                                resultSymbolNum = outputSymbolNum - addendumNum;
                                if (resultSymbolNum < 0)
                                {
                                    resultSymbolNum += 16;
                                }
                            }
                            else
                            {
                                resultSymbolNum = (addendumNum + outputSymbolNum) % 16;
                            }

                            result += resultSymbolNum.ToString("X");
                        }

                        if (code.OutputSymbol.Length > 1)
                        {
                            result += ")";
                        }
                    }
                }
            }
            _tbOutputEnc.Text = result;
        }
    }
}
