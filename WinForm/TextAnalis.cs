using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WordTrain
{
    public class TextAnalis
    {
        
        public class Word
        {
            public string word="";
            public int count=0;
            public int IKnow = 0;
            public string SimilarToTrain = "";
            public double SimilarKoef = 0.0;
        }

        #region 
        public class wordComparer : IComparer<Word>
        {
            public int Compare(Word w1, Word w2)
            {
                return w1.word.CompareTo(w2.word);
            }
        }
        public class countComparer : IComparer<Word>
        {
            public int Compare(Word w1, Word w2)
            {
                return w1.count.CompareTo(w2.count);
            }
        }

        public class IKnowComparer : IComparer<Word>
        {
            public int Compare(Word w1, Word w2)
            {
                return w1.IKnow.CompareTo(w2.IKnow);
            }
        }
        public class SimilarKoefComparer : IComparer<Word>
        {
            public int Compare(Word w1, Word w2)
            {
                return w1.SimilarKoef.CompareTo(w2.SimilarKoef);
            }
        }

        #endregion

        private void SortListBy(int Col)
        {

            switch (Col)
            {
                case 1: { TextWordList.Sort(new wordComparer()); break; }
                case 2: { TextWordList.Sort(new countComparer()); break; }
                case 3: { TextWordList.Sort(new IKnowComparer()); break; }
                case 4: { TextWordList.Sort(new SimilarKoefComparer()); break; }
            }

        }

        private List<Word> TextWordList=new List<Word>(1000);
        
        private string Path = "";
        public TextAnalis(string path)
        {
            Path = path;        
        }

        public double PercentKnownWords = 0.0;
        public double PercentLearningWords = 0.0;
        public int KnownWords = 0;
        public int LearningWords = 0;
        public int WordsCount = 0;

        private void WriteSimilarData(int index, double koef, List<EWord> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TextWordList[index].SimilarKoef = koef;
                TextWordList[index].SimilarToTrain += list[i].Eng+" ";
            }
        
        }

        //анализ слов - поиск таких же в изучаемых и известных словах
        public void CheckMyWords(MyWords MyKnownWords, EWordList TrainWords)
        {
            FWait.SetText("Анализируем слова... ",0);
           
            Parallel.For(0, TextWordList.Count, j =>
            { 
                string CheckWord=TextWordList[j].word.ToLower();
                int i = MyKnownWords.MWL.FindIndex(r => (r.Eng.ToLower() == CheckWord));
                if (i >= 0)
                {
                    TextWordList[j].IKnow = 2;
                }
                else
                {
                    i = TrainWords.FindIndex(r => (r.Eng.ToLower() == CheckWord));
                    if (i >= 0) TextWordList[j].IKnow = 1;
                }

                if ((TextWordList[j].IKnow == 0) && (TextWordList[j].word.Length>=3))
                {

                    string part = "";
                    double SimilarKoef = 0.0;
                    List<EWord> PrevTempWords = new EWordList(100);
                    List<EWord> ResTempWords = new EWordList(100);
                    List<EWord> TempWords = new EWordList(100);
                    bool stop = false;
                    for (int i1 = 3; i1 <= CheckWord.Length; i1++)
                    {
                        TempWords.Clear();
                        PrevTempWords.Clear();
                        for (int i2 = 0; i2 <= CheckWord.Length - i1; i2++)
                        {
                            part = CheckWord.Substring(i2, i1);
                            TempWords = TrainWords.FindAll(r => r.Eng.ToLower().Contains(part));

                            for (int i3 = 0; i3 < TempWords.Count; i3++) PrevTempWords.Add(TempWords[i3]);
                        }

                        if (PrevTempWords.Count == 0)
                        {
                            if (ResTempWords.Count != 0)
                            {
                                WriteSimilarData(j, (i1 - 1) * 100.0 / (CheckWord.Length * 1.0), ResTempWords);
                                ResTempWords.Clear();
                                break;
                            }

                        }
                        else 
                        {
                            ResTempWords.Clear();
                            for (int i3 = 0; i3 < PrevTempWords.Count; i3++) ResTempWords.Add(PrevTempWords[i3]);
                        } 
                        
                    }
                    if (ResTempWords.Count != 0)
                    {
                        WriteSimilarData(j, 100.0, ResTempWords);
                        ResTempWords.Clear();
                    }
                }
            });

            FWait.SetText("Собиарем статистику..", 100);
            //статистическая информация
            List<Word> LearningWord = TextWordList.FindAll(r => r.IKnow == 1);
            LearningWords = LearningWord.Count;

            List<Word> KnownWord = TextWordList.FindAll(r => r.IKnow == 2);
            KnownWords = KnownWord.Count;
            PercentKnownWords = (KnownWords * 1.0 / TextWordList.Count * 1.0) * 100.0;
            PercentLearningWords = (LearningWords * 1.0 / TextWordList.Count * 1.0) * 100.0;
            
            WordsCount = TextWordList.Count;
        }

        public void CreateEnglishWordList()
        {
            FWait.SetText("Читаем файл...", 0);
            StreamReader sr = new StreamReader(Path);
            List<string> AllLines = new List<string>(10000);
            string str1;
            StringBuilder SB = new StringBuilder();
            while ((str1 = sr.ReadLine()) != null)
            {
                SB.Clear();
                foreach (char c in str1)
                {
                    int code = Convert.ToInt32(c);
                    if (((code < 65) || (code > 122)) && (code != 39) && (code != Convert.ToInt32('\n')))
                        SB.Append(" ");
                    else SB.Append(c);
                }
                AllLines.Add(SB.ToString().ToLower());
           }

            FWait.SetText("Получаем слова...", 100.0);
            Parallel.ForEach(AllLines, ln =>
            {

                char[] del = new char[1];
                del[0] = ' ';

                string[] arrLine = ln.Split(del, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in arrLine)
                {
                    if ((str != "") && (str != " "))
                    {
                        int ind = TextWordList.FindIndex(r => r.word == str);
                        if (ind < 0)
                        {
                            Word w = new Word();
                            w.word = str;
                            w.count = 1;
                            lock (TextWordList) TextWordList.Add(w);

                        }
                        else lock (TextWordList) TextWordList[ind].count++;

                    }

                }

            });


            



        }

        public void SaveToGrid(DataGridView DGV, int SortCol=0)
        {
            SortListBy(SortCol);
            DGV.ColumnCount = 6;
            DGV.RowHeadersVisible = false;
            DGV.Columns[0].Width = 50;
            DGV.Columns[1].Width = 200;
            DGV.Columns[2].Width = 100;
            DGV.Columns[3].Width = 100;
            DGV.Columns[4].Width = 100;
            DGV.Columns[5].Width = 300;
            DGV.Columns[0].HeaderText = "№";
            DGV.Columns[1].HeaderText = "Слово";
            DGV.Columns[2].HeaderText = "Кол-во";
            DGV.Columns[3].HeaderText = "Известно";
            DGV.Columns[4].HeaderText = "Коэф подоб";
            DGV.Columns[5].HeaderText = "Похожие слова";
            DGV.Rows.Clear();

            DGV.RowCount = Math.Max(1, TextWordList.Count);
            for (int i = 0; i < TextWordList.Count; i++)
            {
                DGV.Rows[i].Cells[0].Value = (i+1).ToString();
                DGV.Rows[i].Cells[1].Value = TextWordList[i].word;
                DGV.Rows[i].Cells[2].Value = TextWordList[i].count.ToString();
                if (TextWordList[i].IKnow == 0) DGV.Rows[i].Cells[3].Value = "Нет";
                else if (TextWordList[i].IKnow == 1) DGV.Rows[i].Cells[3].Value = "В изучаемых";
                else DGV.Rows[i].Cells[3].Value = "Да";
                DGV.Rows[i].Cells[4].Value = TextWordList[i].SimilarKoef.ToString("0.00");
                DGV.Rows[i].Cells[5].Value = TextWordList[i].SimilarToTrain;


            }

            for (int i = 0; i < DGV.Columns.Count; i++)
            {
                DGV.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
             }

        }

    
    }
}
