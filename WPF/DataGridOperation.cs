using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WordTrain
{
    public static class DataGridOperation
    {

        public static void sort_grid(DataGridView DG, int col, int type)
        {
           
            try
            {

                switch (type)
                {
                    case 0:
                        {

                            for (int i = 0; i < DG.RowCount; i++)
                                for (int j = i + 1; j < DG.RowCount; j++)
                                {
                                    if ((DG.Rows[j].Cells[0].Value.ToString() != " ") && (DG.Rows[i].Cells[0].Value.ToString() != " "))// || //((DG.Rows[j].Cells[0].Value != " ") || (DG.Rows[i].Cells[0].Value != " ")))
                                        if (Convert.ToInt32(DG.Rows[j].Cells[col].Value) < Convert.ToInt32(DG.Rows[i].Cells[col].Value))
                                        {
                                            replace_str(DG, i, j);
                                        }
                                }

                            break;
                        }
                    case 1:
                        {

                                                   
                            break;
                        }
                    case 2: { 
                       // DG.Sort(DG.Columns[col], ListSortDirection.Ascending); break;

                        for (int i = 0; i < DG.RowCount; i++)
                            for (int j = i + 1; j < DG.RowCount; j++)
                            {
                                    if (String.Compare(DG.Rows[i].Cells[col].Value.ToString(),DG.Rows[j].Cells[col].Value.ToString())>0)
                                    {
                                        replace_str(DG, i, j);
                                    }
                            }

                        break;

                    }

                    case 3:
                        {

                            for (int i = 0; i < DG.RowCount; i++)
                                for (int j = i + 1; j < DG.RowCount; j++)
                                {
                                    if ((DG.Rows[j].Cells[0].Value.ToString() != " ") && (DG.Rows[i].Cells[0].Value.ToString() != " "))// || //((DG.Rows[j].Cells[0].Value != " ") || (DG.Rows[i].Cells[0].Value != " ")))
                                        if (Convert.ToDouble(DG.Rows[j].Cells[col].Value) < Convert.ToDouble(DG.Rows[i].Cells[col].Value))
                                        {
                                            replace_str(DG, i, j);
                                        }
                                }

                            break;
                        }
                }

            }
            catch { }

        }


        private static void replace_str(DataGridView DG, int col1, int col2)
        {
            for (int k = 0; k < DG.ColumnCount; k++)
            {
                string s = DG.Rows[col1].Cells[k].Value.ToString();
                DG.Rows[col1].Cells[k].Value = DG.Rows[col2].Cells[k].Value.ToString();
                DG.Rows[col2].Cells[k].Value = s;

            }
        }

        public static void sum_cells(DataGridView DG, out int count, out double sum, out double avg, bool only_sum=false)
        {
            count = 0;
            sum = 0;
            avg = 0;

            double tmp_d = 0;
            int tmp_i = 0;

            count = DG.SelectedCells.Count;
            if (only_sum) return;
            for (int i = 0; i < DG.SelectedCells.Count; i++)
            {
                if (DG.SelectedCells[i].Value == null) continue;
                try
                {
                    if (int.TryParse(DG.SelectedCells[i].Value.ToString(), out tmp_i))
                    {
                        sum += tmp_i;
                    }
                    else if (double.TryParse(DG.SelectedCells[i].Value.ToString(), out tmp_d))
                    {
                        sum += tmp_d;
                    }
                }
                catch { }

            }
           if (count != 0) avg = sum / count;
        }

    }
}
