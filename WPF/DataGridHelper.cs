using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WordTrain
{
    public static class DataGridHelper
    {
        public static List<EWord> dgSelectedCell(DataGrid dg)
        {

            var a = (from b in dg.SelectedCells select b).Distinct();
            return null;
        

        }
    }
}
