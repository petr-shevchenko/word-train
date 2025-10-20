using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTrain
{
    public class Theme
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ToLearn { get; set; }
        public int Total { get; set; }
    
    }

    public class ThemeList : List<Theme>
    {
        private EWordList _wordList;
        public ThemeList(EWordList wordList)
            : base()
        {
            _wordList = wordList;
            BuildThemeList();
        }

        public void BuildThemeList()
        {
            base.Clear();
            base.Add(new Theme() { ID = 1, Name = "All words", Total = _wordList.Count, ToLearn = _wordList.Where(x => x.Repeat != 0).Count() });
            var themes = (from c in _wordList orderby c.Theme group c by c.Theme into ws select new Theme() { Name = ws.First().Theme, Total = ws.Count(), ToLearn = ws.Where(x => x.Repeat > 0).Count() }).ToList();

            int i = 2;
            foreach (Theme t in themes)
            {
                t.ID = i++;
                base.Add(t);
            }
        
        }

        public void UpdateThemeStat(string themeName)
        {
            UpdateOneThemeStat(themeName);
            UpdateOneThemeStat("All words");
        }
        public void UpdateOneThemeStat(string themeName)
        {
            int themeID = base.FindIndex(x => x.Name.ToLower().Trim() == themeName.ToLower().Trim());
            if (themeID >= 0)
            {
                List<EWord> TH;
                if (themeName == "All words") TH = _wordList;
                else TH = _wordList.FindAll(w => w.Theme.Contains(themeName));

                base[themeID].Total = TH.Count;
                base[themeID].ToLearn = TH.FindAll(w => w.Repeat > 0).Count;
            }

        }

    }
}
