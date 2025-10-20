using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WordTrain
{
    public partial class MainForm : Form
    {
        private MyWords MW=new MyWords();
        
        public string Path="words.xml";
        public string MyWordsPath = "MyWords.xml";
        private EWordListOperation LearnWords = null;
        private EWord EW;
        private CheckBox[] CB = new CheckBox[4];
        public int TrainWordStartID = 0;
        public int TrainWordEndID = 0;

        private int TekSortColumnForTrainWord = 0;

        public MainForm()
        {
            MW = new MyWords();
            InitializeComponent();
            openFileDialog1.InitialDirectory = Application.StartupPath;
            saveFileDialog1.InitialDirectory = Application.StartupPath;
            LoadWordsFromFile(Path);
            LoadMyWords(MyWordsPath);
            eWordListBindingSource.MoveFirst();
            //dataGridView1.DataSource = LearnWords.WordList.ToArray();
        }

        private void LoadMyWords(string path)
        {
            MW.MWL.LoadFromFile(path);
            MW.MWL.SaveToGrid(DG_MyWords);
        }
        private void SaveMyWords(string path)
        {
            MW.MWL.LoadFromGrid(DG_MyWords);
            MW.MWL.SaveToFile(path);
        }





        private void LoadWordsFromFile(string path)
        {
            Path = path;
            this.Text = "WordTrain: " + Path;
            LearnWords = new EWordListOperation(Path);
            LearnWords.WordList.SaveToGrid(DG_TrainWord, TekSortColumnForTrainWord, TrainWordStartID, TrainWordEndID);
            RefreshStat();
            BtnNext.PerformClick();
            RefreshThemeTable();
            this.Text = "WordTrain: " + Path;
        }


        private void RefreshThemeTable()
        {
            if ((DGThemeList.ColumnCount != 4) || (DGThemeList.RowCount != LearnWords.WordList.ThemeList.Count))
            {
                DGThemeList.ColumnCount = 4;
                DGThemeList.RowHeadersVisible = false;
                DGThemeList.Columns[0].Width = 50;
                DGThemeList.Columns[1].Width = 150;
                DGThemeList.Columns[2].Width = 80;
                DGThemeList.Columns[3].Width = 80;
                DGThemeList.Columns[0].HeaderText = "№";
                DGThemeList.Columns[1].HeaderText = "Тема";
                DGThemeList.Columns[2].HeaderText = "Изучить";
                DGThemeList.Columns[3].HeaderText = "Всего";
                DGThemeList.Rows.Clear();
                DGThemeList.RowCount = LearnWords.WordList.ThemeList.Count;
            }
            
            int AllWordCount = 0;
            int WorkWordCount = 0;
            for (int i = 0; i < LearnWords.WordList.ThemeList.Count; i++)
            {
                DGThemeList.Rows[i].Cells[0].Value = (i + 1).ToString();
                DGThemeList.Rows[i].Cells[1].Value = LearnWords.WordList.ThemeList[i];
                LearnWords.GetThemeStat(LearnWords.WordList.ThemeList[i], out AllWordCount, out WorkWordCount);
                DGThemeList.Rows[i].Cells[2].Value = WorkWordCount.ToString();
                DGThemeList.Rows[i].Cells[3].Value = AllWordCount.ToString();
            }


        }

        private void SaveWordsFromFile(string path)
        {
            Path = path;
            this.Text = "WordTrain: " + Path;
            LearnWords.WordList.LoadDataFromGrid(DG_TrainWord);
            LearnWords.WordList.SaveToFile(Path,TekSortColumnForTrainWord);
            LearnWords.WordList.SaveToGrid(DG_TrainWord, TekSortColumnForTrainWord, TrainWordStartID, TrainWordEndID);
            RefreshStat();
        }


        private void BtnNext_Click(object sender, EventArgs e)
        {
            string theme = "All words";
            
            if (DGThemeList.SelectedCells.Count>0) 
            theme = DGThemeList.Rows[DGThemeList.SelectedCells[0].RowIndex].Cells[1].Value.ToString();

            if (theme != "All words") EW = LearnWords.GetRandWord(theme, checkBox1.Checked); 
            else EW = LearnWords.GetRandWord(checkBox1.Checked);
            
            EW.Rus=EW.Rus.Replace("ё", "е");
            TbEng.Text = EW.Eng;
            TbTry.Text = "";
            TbRus.Text = "";
            label1.Text = "";
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            if (TbTry.Text == "") TbTry.Text = "@";
            TbTry.Text = TbTry.Text.ToLower();
            TbTry.Text = TbTry.Text.Replace("ё", "е");
            TbRus.Text = EW.Rus;
            if (EW.Rus.ToLower().Contains(TbTry.Text))
            {
                LearnWords.DecRepeat(EW.ID);
                label1.Text = "Yes! =)";
            }
            else
            {
                if (radioButton1.Checked) LearnWords.IncRepeat(EW.ID);
                else LearnWords.SetRepeat(EW.ID, -1);
                LearnWords.IncError(EW.ID);
                label1.Text = "No =(";
            }
            RefreshStat();
        }

        private void BtnAddWord_Click(object sender, EventArgs e)
        {
            LearnWords.WordList.AddNewWord(TbAddEng.Text, TbAddRus.Text, TbAddTheme.Text,1);
            LearnWords.WordList.SaveToFile(Path);
            LearnWords.WordList.SaveToGrid(DG_TrainWord, TekSortColumnForTrainWord, TrainWordStartID, TrainWordEndID);
            RefreshStat();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            LearnWords.WordList.SaveToFile(Path);
        }

        private void BtnSetRepeat_Click(object sender, EventArgs e)
        {
            LearnWords.SetRepeat(1);
            LearnWords.WordList.SaveToFile(Path);
            LearnWords.WordList.SaveToGrid(DG_TrainWord, TekSortColumnForTrainWord, TrainWordStartID, TrainWordEndID);
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LearnWords.WordList.SaveToGrid(DG_TrainWord, TekSortColumnForTrainWord, TrainWordStartID, TrainWordEndID);
        }

        private void Search_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < DG_TrainWord.SelectedCells.Count; i++)
            {
                DG_TrainWord.SelectedCells[i].Value = TbSetValue.Text;
            }

        }

        private void searchWord(string word)
        {
            List<EWord> Lst = LearnWords.WordList.FindAll(w => (w.Eng.Contains(word)||w.Rus.Contains(word)) );

            DG_TrainWord.RowCount = Math.Max(1, Lst.Count);
            for (int i = 0; i < Lst.Count; i++)
            {
                DG_TrainWord.Rows[i].Cells[0].Value = Lst[i].ID.ToString();
                DG_TrainWord.Rows[i].Cells[1].Value = Lst[i].Eng;
                DG_TrainWord.Rows[i].Cells[2].Value = Lst[i].Rus;
                DG_TrainWord.Rows[i].Cells[3].Value = Lst[i].Theme;
                DG_TrainWord.Rows[i].Cells[4].Value = Lst[i].Repeat.ToString();

            }
        
        
        
        }

        private void RefreshStat()
        {
            int SelTheme = 0;
            if (DGThemeList.SelectedCells.Count > 0) SelTheme=DGThemeList.SelectedCells[0].RowIndex;
            
                RefreshThemeTable();
                for (int i = DGThemeList.SelectedCells.Count - 1; i >= 0; i--)
                    DGThemeList.SelectedCells[i].Selected = false;
            DGThemeList.Rows[SelTheme].Cells[0].Selected = true;
        }

       

        private void TbTry_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void TbTry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if ((label1.Text == "") && (TbTry.Text == "")) BtnCheck.PerformClick();
                else if ((label1.Text != "") && (TbTry.Text != "")) BtnNext.PerformClick();
                else if ((TbTry.Text != ""))BtnCheck.PerformClick();    
                    
                    
                   
            }
            
        }

        private void BtnAddLine_Click(object sender, EventArgs e)
        {
            DG_TrainWord.Rows.Add(1);
            LearnWords.WordList.AddNewWord("", "", "", 1, 0);
            DG_TrainWord.Rows[DG_TrainWord.Rows.Count - 1].Cells[0].Value = LearnWords.WordList.LastID;
        }

        private void BtnDelLine_Click(object sender, EventArgs e)
        {
            int c=DG_TrainWord.SelectedCells.Count;
            for (int i = 0; i <c ; i++)
            {
                LearnWords.WordList.DelWordByID(Convert.ToInt32(DG_TrainWord.Rows[DG_TrainWord.SelectedCells[0].RowIndex].Cells[0].Value));
                DG_TrainWord.Rows.Remove(DG_TrainWord.Rows[DG_TrainWord.SelectedCells[0].RowIndex]);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveWordsFromFile(Path);
        }

        private void выбратьФайлсловарьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadWordsFromFile(openFileDialog1.FileName);
            }

        }

        private void сохранитьФайлсловарьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SaveWordsFromFile(saveFileDialog1.FileName);
            }
        }

        private void BtnDelAll_Click(object sender, EventArgs e)
        {
            LearnWords.WordList.Clean();
            LearnWords.WordList.SaveToGrid(DG_TrainWord, TekSortColumnForTrainWord, TrainWordStartID, TrainWordEndID);
        }

       

        public TextAnalis TA;
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TA = new TextAnalis(openFileDialog1.FileName);
                FWait.SetText("Создаём список английских слов.." , 0);
                TA.CreateEnglishWordList();
                TA.CheckMyWords(MW,LearnWords.WordList);
                FWait.SetText("Загружаем список в таблицу..", 100);
                TA.SaveToGrid(DG_TextAnalis);
                FWait.SetText("Создаём список английских слов..",-1);

                textBox1.Text = TA.WordsCount.ToString();//всего слов
                
                textBox2.Text = TA.KnownWords.ToString();//известных слов
                textBox5.Text = TA.LearningWords.ToString();//изучаемых слов
                textBox6.Text = (TA.WordsCount - TA.KnownWords - TA.LearningWords).ToString();//новых слов

 
                textBox7.Text = (TA.PercentKnownWords).ToString("0.00");//процент известных слов
                textBox4.Text = (TA.PercentLearningWords).ToString("0.00");//процент изучаемых слов
                textBox3.Text = (100 - TA.PercentKnownWords - TA.PercentLearningWords).ToString("0.00");//процент новых слов
               
               

            }

        }

        private void BtnSaveMyWords_Click(object sender, EventArgs e)
        {
            SaveMyWords(MyWordsPath);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            DG_MyWords.Rows.Add(1);
        }

        private void BtnDelMyWord_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(DG_MyWords.Rows[DG_MyWords.SelectedCells[0].RowIndex].Cells[0].Value);
            MW.MWL.DelWord(id);
            DG_MyWords.Rows.Remove(DG_MyWords.Rows[DG_MyWords.SelectedCells[0].RowIndex]);
        }

        private void добавитьВМоиСловаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i=0; i<DG_TrainWord.SelectedCells.Count; i++)
            {
                int RowId = DG_TrainWord.SelectedCells[i].RowIndex;
                MW.MWL.AddNewWord(DG_TrainWord.Rows[RowId].Cells[1].Value.ToString(), DG_TrainWord.Rows[RowId].Cells[2].Value.ToString());
                MW.MWL.SaveToGrid(DG_MyWords);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < DG_TextAnalis.SelectedCells.Count; i++)
            {
                int RowId = DG_TextAnalis.SelectedCells[i].RowIndex;
                MW.MWL.AddNewWord(DG_TextAnalis.Rows[RowId].Cells[1].Value.ToString(), "");
                MW.MWL.SaveToGrid(DG_MyWords);
            }

        }

        private void добавитьВИзучаемыеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FWait.SetText("Анализируем выбранные ячейки...", 0);
            DataGridView DGV;
            int DGVnum = tabControl1.SelectedIndex;
            if (DGVnum==2) DGV = DG_TextAnalis;
            else if (DGVnum == 3) DGV = DG_MyWords;
            else { FWait.SetText("Добавляем слова...", -1.0); return; }



            int AddCount = DGV.SelectedCells.Count;
            int RowId = 0;
            int pRowId = 0;
            for (int i = 0; i < AddCount; i++)
            {
                RowId = DGV.SelectedCells[i].RowIndex;
                if (pRowId != RowId)
                {
                    pRowId = RowId;
                    FWait.SetText("Добавляем слово " + DGV.Rows[RowId].Cells[1].Value.ToString(), "(" + (i + 1).ToString() + " из " + AddCount.ToString() + ")", (i * 100.0 / (AddCount * 1.0)));
                    if (DGVnum == 2) LearnWords.WordList.AddNewWord(DGV.Rows[RowId].Cells[1].Value.ToString(), "", textBoxTheme.Text, 1, count: Convert.ToInt32(DGV.Rows[RowId].Cells[2].Value));
                    else LearnWords.WordList.AddNewWord(DGV.Rows[RowId].Cells[1].Value.ToString(), DGV.Rows[RowId].Cells[2].Value.ToString(), "", 1); 
                }
            }
            FWait.SetText("Сохраняем слова...", 100);
            LearnWords.WordList.SaveToFile(Path);
            LearnWords.WordList.SaveToGrid(DG_TrainWord, TekSortColumnForTrainWord, TrainWordStartID, TrainWordEndID);
           
            RefreshStat();
            FWait.SetText("Добавляем слова...", -1.0);
        }


  
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LearnWords.WordList.SaveToGrid(DG_TrainWord,TekSortColumnForTrainWord,0,0,TbSearch.Text,1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int filterCol = 0;
            if (rb_eng.Checked) filterCol = 1;
            else if (rb_rus.Checked) filterCol = 2;
            else if (rb_theme.Checked) filterCol = 3;
            LearnWords.WordList.SaveToGrid(DG_TrainWord, TekSortColumnForTrainWord, 0, 0, TbFilter.Text, filterCol);
        }

        private void BtnSaveData_Click(object sender, EventArgs e)
        {
            LearnWords.WordList.LoadDataFromGrid(DG_TrainWord);
        }


        public void DG_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int type = 0;
            string s = ((DataGridView)sender).Name;

            switch (s)
            {


                case "DG_MyWords":
                    {
                        switch (e.ColumnIndex)
                        {
                            case 0: { type = 0; break; }
                            case 1:
                            case 2: { type = 2; break; }
                        }
                        break;
                    }

                    DataGridOperation.sort_grid((DataGridView)sender, e.ColumnIndex, type);
            }
        }
     
        private void DG_TrainWord_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int filterCol = 0;
            if (rb_eng.Checked) filterCol = 1;
            else if (rb_rus.Checked) filterCol = 2;
            else if (rb_theme.Checked) filterCol = 3;
            TekSortColumnForTrainWord = e.ColumnIndex;
            LearnWords.WordList.SaveToGrid(DG_TrainWord, TekSortColumnForTrainWord, TrainWordStartID, TrainWordEndID, TbFilter.Text, filterCol);
        }

        private void DG_TextAnalis_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            TA.SaveToGrid(DG_TextAnalis, e.ColumnIndex);
        }

        private void DG_TrainWord_SelectionChanged(object sender, EventArgs e)
        {
            int count;
            double sum,avg;
            DataGridOperation.sum_cells(sender as DataGridView, out count, out sum, out avg, true);
            TT_lbl_count.Text = "Количество: " + count.ToString() + "   ";
            //TT_lbl_sum.Text = "Сумма: " + sum.ToString("0.00") + "   ";
          //  TT_lbl_avg.Text = "Среднее: " + avg.ToString("0.00") + "   ";
        }

        

    }
}
