using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTrain
{
    public class EWordListOperation
    {
        public EWordList WordList = new EWordList(10000);

        public EWordListOperation(string path)
        {
            WordList.LoadFromFile(path);
        }


    
        public EWord GetRandWord(List<EWord> SL)
        {
            EWord X = new EWord();
            Random r = new Random();
            if (SL.Count == 0) return (X);
            int i = r.Next(0, SL.Count - 1);
            return (SL[i]);
        }


        public EWord GetRandWord(bool repeat)
        {
            List<EWord> TH = new List<EWord>(100);
            if (repeat) TH = WordList.FindAll(w => w.Repeat > 0);
            else TH = WordList;
            return (GetRandWord(TH));
        }

        public EWord GetRandWord(string theme, bool repeat)
        {
            List<EWord> TH = new List<EWord>(100);
            TH = WordList.FindAll(w => w.Theme.Contains(theme));
            if (repeat) TH = TH.FindAll(w => w.Repeat > 0);
            return (GetRandWord(TH));
        }

        public void GetThemeStat(string theme, out int AllWordCount, out int WorkWordCount)
        {
            AllWordCount = 0;
            WorkWordCount = 0;
            List<EWord> TH = new List<EWord>(100);
            if (theme == "All words")
                TH = WordList;
            else TH = WordList.FindAll(w => w.Theme.Contains(theme));

            AllWordCount = TH.Count;
            TH = TH.FindAll(w => w.Repeat > 0);
            WorkWordCount = TH.Count;

        }



        public void DecRepeat(int id)
        {
            int ind = WordList.FindIndex(r => r.ID == id);
            if (ind >= 0)
            {
                WordList[ind].Repeat--;
                if (WordList[ind].Repeat < 0) WordList[ind].Repeat = 0;
            }
        }
        public void IncRepeat(int id)
        {
            int ind = WordList.FindIndex(r => r.ID == id);
            if (ind >= 0) WordList[ind].Repeat++;
        }

        public void SetRepeat(int rep)
        {
            for (int i = 0; i < WordList.Count; i++) WordList[i].Repeat = rep;
        }

        public void SetRepeat(int id, int rep)
        {
            int ind = WordList.FindIndex(r => r.ID == id);
            if (ind >= 0) WordList[ind].Repeat = rep;
        }

        public void IncError(int id)
        {
            int ind = WordList.FindIndex(r => r.ID == id);
            if (ind >= 0) WordList[ind].Errors++;
        }

    }

}
