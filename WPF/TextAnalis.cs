using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;


namespace WordTrain
{
    public class TextAnalis
    {
        public WTProgressBar WTpb { get; set; }
        public enum WordType
        {
            Unknown=0,
            NewWord=1,
            LearnWord=2,
            ExceptionWord =3        
        }

        public class Word
        {
            public string word { get; set; }
            public int count { get; set; }
            public int IKnow { get; set; }

            public WordType TypeOfWord { get; set; }
            public string SimilarToTrain { get; set; }
            public double SimilarKoef { get; set; }

            public Word()
            {
                word = "";
                count = 0;
                IKnow = 0;
                SimilarToTrain = "";
                SimilarKoef = 0.0;
                TypeOfWord = WordType.Unknown;
            }
        
        }
        public List<Word> TextWordList=new List<Word>(1000);
        public double PercentExceptionalWords = 0.0;
        public double PercentLearningWords = 0.0;
        public int ExceptionalWordsCount = 0;
        public int LearningWordsCount = 0;
        public int TotalWordsCount = 0;
 
        public void AnalyseText(string filePath, EWordList ExceptionalWords, EWordList LearningWords)
        {
            CreateEnglishWordList(filePath);
            CheckMyWords(ExceptionalWords, LearningWords);
        }


        private void WriteSimilarData(int index, double koef, List<EWord> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                TextWordList[index].SimilarKoef = koef;
                TextWordList[index].SimilarToTrain += list[i].Eng+" ";
            }
        
        }

      

        //анализ слов - поиск таких же в изучаемых и известных словах
        public void CheckMyWords(EWordList ExceptionalWords, EWordList LearningWords)
        {
            WTpb.Dispatcher.Invoke(() => { WTpb.startBar("Анализируем слова.....", TextWordList.Count); });
            Parallel.For(0, TextWordList.Count, j =>
            {
                WTpb.Dispatcher.Invoke(() => { WTpb.plusBar(); });
                string CheckWord=TextWordList[j].word.ToLower();
                int i = ExceptionalWords.FindIndex(r => (r.Eng.ToLower() == CheckWord));
                if (i >= 0)
                {
                    TextWordList[j].TypeOfWord = WordType.ExceptionWord;
                }
                else
                {
                    i = LearningWords.FindIndex(r => (r.Eng.ToLower() == CheckWord));
                    if (i >= 0) TextWordList[j].TypeOfWord = WordType.LearnWord;
                }
                if ((TextWordList[j].TypeOfWord == WordType.NewWord) && (TextWordList[j].word.Length >= 3))
                {
                    double SimilarKoef = 0.0;
                    List<EWord> temppp = GetSimilarWords(CheckWord, LearningWords, out SimilarKoef);
                    WriteSimilarData(j, SimilarKoef, temppp);
                }
            });
            WTpb.Dispatcher.Invoke(() => { WTpb.startBar("Собиарем статистику....."); });
            List<Word> LearningWord = TextWordList.FindAll(r => r.TypeOfWord == WordType.LearnWord);
            LearningWordsCount = LearningWord.Count;

            List<Word> KnownWord = TextWordList.FindAll(r => r.TypeOfWord == WordType.ExceptionWord);
            ExceptionalWordsCount = KnownWord.Count;
            PercentExceptionalWords = (ExceptionalWordsCount * 1.0 / TextWordList.Count * 1.0);
            PercentLearningWords = (LearningWordsCount * 1.0 / TextWordList.Count * 1.0);
            
            TotalWordsCount = TextWordList.Count;
            WTpb.Dispatcher.Invoke(() => { WTpb.closeBar(); });
        }

        public void CreateEnglishWordList(string Path)
        {
            WTpb.Dispatcher.Invoke(() => { WTpb.startBar("Чтение файла.."); });
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
            WTpb.Dispatcher.Invoke(() => { WTpb.startBar("Разбор файла..", AllLines.Count); });
            Parallel.ForEach(AllLines, ln =>
            {
                WTpb.Dispatcher.Invoke(() => { WTpb.plusBar(); });
                string[] arrLine = ln.Split(new char[] {' '}, System.StringSplitOptions.RemoveEmptyEntries);
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
                            w.TypeOfWord = WordType.NewWord;
                            lock (TextWordList) TextWordList.Add(w);

                        }
                        else lock (TextWordList) TextWordList[ind].count++;

                    }

                }

            });
        WTpb.Dispatcher.Invoke(() => { WTpb.closeBar(); });
        }


        public static List<EWord> GetSimilarWords(string CheckWord, EWordList SourceWordList, out double SimilarKoef)
        {
            SimilarKoef = 0.0;
            List<EWord> Result = new List<EWord>();
            string part = "";
            List<EWord> PrevTempWords = new List<EWord>(100);
            List<EWord> ResTempWords = new List<EWord>(100);
            List<EWord> TempWords = new List<EWord>(100);

            for (int i1 = 3; i1 <= CheckWord.Length; i1++)
            {
                TempWords.Clear();
                PrevTempWords.Clear();
                for (int i2 = 0; i2 <= CheckWord.Length - i1; i2++)
                {
                    part = CheckWord.Substring(i2, i1);
                    TempWords = SourceWordList.FindAll(r => r.Eng.ToLower().Contains(part) && r.Eng.ToLower() != CheckWord.ToLower());

                    //for (int i3 = 0; i3 < TempWords.Count; i3++) PrevTempWords.Add(TempWords[i3]);
                    foreach (var word in TempWords) PrevTempWords.Add(word);
                }

                if (PrevTempWords.Count == 0)
                {
                    if (ResTempWords.Count != 0)
                    {
                      //  WriteSimilarData(j, (i1 - 1) * 100.0 / (CheckWord.Length * 1.0), ResTempWords);
                        foreach (var word in ResTempWords) Result.Add(word);
                        SimilarKoef = (i1 - 1) * 100.0 / (CheckWord.Length * 1.0);
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
             //   WriteSimilarData(j, 100.0, ResTempWords);
                foreach (var word in ResTempWords) Result.Add(word);
                SimilarKoef = 100.0;

                ResTempWords.Clear();
            }
            return Result;
        
        }
    }
}
