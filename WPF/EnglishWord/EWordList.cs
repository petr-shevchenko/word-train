using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace WordTrain
{
    public class EWordList : List<EWord>
    {
        public ThemeList ThemeList { get; set; }
        public int LastID { get; set; }

        private string filePath="";
        private WTProgressBar progressBar;

        public EWordList(int count = 5000, string filePath = "", WTProgressBar progressBar=null)
            : base(count)
        {
            this.filePath = filePath;
            this.progressBar = progressBar;
            LastID = 0;
            if (filePath.Length > 0) LoadFromFile();
        }


        public void AddNewWord(EWord word)
        {
            //AddNewWord(word.Eng, word.Rus, word.Theme, word.Repeat, word.Errors, word.Count);

            int ind = base.FindIndex(r => r.Eng == word.Eng);

            if ((word.Eng == "" && word.Rus == "") || (ind < 0))
            {
                LastID++;
                word.ID = LastID;
                base.Add(word);
            }
            else if (ind >= 0)
            {
                if ((!base[ind].Theme.Contains(word.Theme)) && (word.Theme != ""))
                {
                    base[ind].Theme += ';' + word.Theme;
                }
                base[ind].Count += word.Count;
            }
        }

        public bool LoadFromFile(string NewFilePath="")
        {
            try
            {
                if (progressBar != null) progressBar.Dispatcher.Invoke(() => { progressBar.startBar("Загружаем данные..."); });
                if (NewFilePath == "") NewFilePath = filePath;
                int i=1;
                base.Clear();
                XmlReader rdr = XmlReader.Create(NewFilePath);
                EWord rec = new EWord();
                while (!rdr.EOF)
                {
                    if (rdr.NodeType == XmlNodeType.Element)
                    {
                        switch (rdr.Name)
                        {
                            case "ID": rec.ID = rdr.ReadElementContentAsInt(); break;
                            case "Eng": rec.Eng = rdr.ReadElementContentAsString(); break;
                            case "Rus": rec.Rus = rdr.ReadElementContentAsString(); break;
                            case "Theme": rec.Theme = rdr.ReadElementContentAsString(); break;
                            case "Repeat": rec.Repeat = rdr.ReadElementContentAsInt(); break;
                            case "Errors": rec.Errors = rdr.ReadElementContentAsInt(); break;
                            case "Count": rec.Count = rdr.ReadElementContentAsInt(); break;
                            default: rdr.Read(); break;
                        }
                    }
                    else
                        if ((rdr.NodeType == XmlNodeType.EndElement) && (rdr.Name == "Word"))
                        {
                            rec.N = i++;
                            base.Add(rec);
                            LastID = Math.Max(rec.ID, LastID);
                            rec = new EWord();
                            rdr.Read();
                        }
                        else rdr.Read();
                }
                rdr.Close();
                ThemeList = new ThemeList(this);
                if (progressBar != null) progressBar.Dispatcher.Invoke(() => { progressBar.closeBar(); });
                return (true);
                
            }
            catch { return (false); }



        }

        public void SaveToFile(string NewFilePath = "")
        {
            string s = "";
            try
            {
                if (progressBar != null) progressBar.Dispatcher.Invoke(() => { progressBar.startBar("Сохраняем данные..."); });
                s = "Подготовка к сохранению";
                if (NewFilePath == "") NewFilePath = filePath;
                DateTime t1 = DateTime.Now;
                TimeSpan ts = new TimeSpan();

                s = "Сортировка по порядку";
                this.Sort(new NComparer());
                EWord[] AL = this.ToArray();
                int cnt = AL.Length;
                MemoryStream ms = new MemoryStream();
                XmlWriter writer = XmlWriter.Create(ms);
                writer.WriteStartDocument();
                writer.WriteStartElement("SyncDirList");
                for (int i = 0; i < cnt; i++)
                {
                    AL[i].ID = i + 1;
                    s = "Запись слова " + AL[i].ID.ToString() + " " + AL[i].Eng;
                    writer.WriteStartElement("Word");
                    writer.WriteElementString("ID", AL[i].ID.ToString());
                    writer.WriteElementString("Eng", AL[i].Eng);
                    writer.WriteElementString("Rus", AL[i].Rus);
                    writer.WriteElementString("Theme", AL[i].Theme);
                    writer.WriteElementString("Repeat", AL[i].Repeat.ToString());
                    writer.WriteElementString("Errors", AL[i].Errors.ToString());
                    writer.WriteElementString("Count", AL[i].Count.ToString());
                    writer.WriteEndElement();
                    writer.WriteRaw("\n");
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
                s = "Запись файла";
                FileStream st = new FileStream(NewFilePath, FileMode.Create, FileAccess.Write);
                ms.WriteTo(st);
                st.Close();
                ms.Close();

                //FWait.SetText("Сохраняем слова... ", -1);
                DateTime t2 = DateTime.Now;
                ts = t2 - t1;
                //       MessageBox.Show("Время: " + ts.ToString());
                if (progressBar != null) progressBar.Dispatcher.Invoke(() => { progressBar.closeBar(); });
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при сохранении данных на этапе " + s + " : " + ex.Message, ex);
            }
        }

        //===============================================
        public EWord GetRandomWord(string theme="", int type=0)
        {
            List<EWord> tempList;// = new List<EWord>(100);
            if ((theme == "")||(theme=="All words")) tempList = base.FindAll(x=>true);
            else tempList = base.FindAll(w => w.Theme.Contains(theme));
            
            tempList = tempList.FindAll(w => w.Repeat > 0);
            
            if ((type == 1)&&(tempList.Count>0))
            {
                var tmp = from x in tempList where x.TempRepeatFlag==1 select x;
                if (tmp.Count() == 0)
                {
                    foreach (var x in tempList)
                        x.TempRepeatFlag = 1;
                }
                tempList = tmp.ToList();
            }
            return (GetRandWord(tempList));
        }

        private EWord GetRandWord(List<EWord> SL)
        {
            EWord X = new EWord();
            Random r = new Random();
            if (SL.Count == 0) return (X);
            int i = r.Next(0, SL.Count);
            if (i >= SL.Count) i = SL.Count - 1; 
            return (SL[i]);
        }

        //===============================================
        public void DecRepeat(int id)
        {
            int ind = base.FindIndex(r => r.ID == id);
            if (ind >= 0)
            {
                base[ind].Repeat--;
                if (base[ind].Repeat < 0) base[ind].Repeat = 0;
                if (base[ind].Repeat <= 0) ThemeList.UpdateThemeStat(base[ind].Theme);
            }
        }

        public void IncRepeat(int id)
        {
            int ind = base.FindIndex(r => r.ID == id);
            if (ind >= 0) base[ind].Repeat++;
        }

        public void SetRepeat(int id, int value)
        {
            int ind = base.FindIndex(r => r.ID == id);
            if (ind >= 0)
            {
                base[ind].Repeat = value;
                if (base[ind].Repeat <= 0) ThemeList.UpdateThemeStat(base[ind].Theme);
            }
        }

        //===============================================
        public void IncError(int id)
        {
            int ind = base.FindIndex(r => r.ID == id);
            if (ind >= 0) base[ind].Errors++;
        }


    }

}
