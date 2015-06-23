using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedAlliance.Erc.Sql
{
    public partial class Code
    {
        public int Row
        {
            get
            {
                return Convert.ToInt32(InputSymbol, 16);
            }
        }
    }
}
