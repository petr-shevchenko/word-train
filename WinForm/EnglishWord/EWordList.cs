using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace WordTrain
{
    public class EWordList : List<EWord>
    {
        public EWordList(int count)
            : base(count)
        {
            ThemeList.Add("All words");
        }


        public int LastID = 0;

        public List<string> ThemeList = new List<string>(100);

        public void Clean()
        {
            base.Clear();
            LastID = 0;
        }

        public void AddNewWord(string eng, string rus, string theme, int repeat, int errors = 0, int count = 0)
        {
            int ind = base.FindIndex(r => r.Eng == eng);
            if (ind >= 0)
            {
                if ((!base[ind].Theme.Contains(theme)) && (theme != ""))
                {
                    base[ind].Theme += ';' + theme;
                }
                base[ind].Count += count;
            }
            else
            {
                EWord rec = new EWord();
                LastID++;
                rec.ID = LastID;
                rec.Eng = eng;
                rec.Rus = rus;
                rec.Theme = theme;
                rec.Repeat = repeat;
                rec.Errors = errors;
                rec.Count = count;
                base.Add(rec);
                string[] spl = rec.Theme.Split(';');
                foreach (string s in spl)
                {
                    if (ThemeList.FindIndex(str => str == s) < 0) ThemeList.Add(s);
                }
            }

        }

        public void UpdateWordById(int WordID, string eng, string rus, string theme, int repeat, int errors = 0, int count = 0)
        {
            int ind = base.FindIndex(r => r.ID == WordID);
            if (ind >= 0)
            {
                base[ind].Eng = eng;
                base[ind].Rus = rus;
                base[ind].Theme = theme;
                base[ind].Repeat = repeat;
                base[ind].Errors = errors;
                base[ind].Count = count;
            }



        }

        public void DelWordByID(int WordID)
        {
            base.RemoveAll(r => r.ID == WordID);

        }


        public void LoadDataFromGrid(DataGridView DGV)
        {
            Parallel.For(0, DGV.RowCount, (i) =>
                {
                    UpdateWordById(Convert.ToInt32(DGV.Rows[i].Cells[0].Value), 
                                                                            DGV.Rows[i].Cells[1].Value.ToString(), 
                                                                            DGV.Rows[i].Cells[2].Value.ToString(),
                                                                            DGV.Rows[i].Cells[3].Value.ToString(), 
                                                Convert.ToInt32(DGV.Rows[i].Cells[4].Value), 
                                                Convert.ToInt32(DGV.Rows[i].Cells[5].Value), 
                                                Convert.ToInt32(DGV.Rows[i].Cells[6].Value));
                }
            );

        }


        public bool LoadFromFile(string path)
        {
            try
            {
                base.Clear();
                XmlReader rdr = XmlReader.Create(path);
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
                            base.Add(rec);
                            LastID = Math.Max(rec.ID, LastID);

                            string[] spl = rec.Theme.Split(';');
                            foreach (string s in spl)
                            {
                                if (ThemeList.FindIndex(str => str == s) < 0) ThemeList.Add(s);

                            }



                            rec = new EWord();
                            rdr.Read();
                        }
                        else rdr.Read();
                }
                rdr.Close();
                return (true);
            }
            catch { return (false); }



        }







        public void SaveToFile(string path, int TekSortColumn = 0)
        {
            DateTime t1 = DateTime.Now;
            TimeSpan ts = new TimeSpan();

            FWait.SetText("Сохраняем слова... ", 0);
            SortListBy(TekSortColumn, this);
            EWord[] AL = this.ToArray();
            int cnt = AL.Length;
            MemoryStream ms = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(ms);
            writer.WriteStartDocument();
            writer.WriteStartElement("SyncDirList");
            for (int i = 0; i < cnt; i++)
            {
                AL[i].ID = i + 1;
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

            FileStream st = new FileStream(path, FileMode.Create, FileAccess.Write);
            ms.WriteTo(st);
            st.Close();
            ms.Close();

            FWait.SetText("Сохраняем слова... ", -1);
            DateTime t2 = DateTime.Now;
            ts = t2 - t1;
     //       MessageBox.Show("Время: " + ts.ToString());
        }


      
        private void SaveListToGrid(DataGridView DGV, List<EWord> list)
        {
            DGV.ColumnCount = 8;
            DGV.RowHeadersVisible = false;
            DGV.Columns[0].Width = 50;
            DGV.Columns[1].Width = 200;
            DGV.Columns[2].Width = 200;
            DGV.Columns[3].Width = 100;
            DGV.Columns[4].Width = 50;
            DGV.Columns[5].Width = 50;
            DGV.Columns[6].Width = 50;
            DGV.Columns[7].Width = 50;
            DGV.Columns[0].HeaderText = "ID";
            DGV.Columns[1].HeaderText = "Eng";
            DGV.Columns[2].HeaderText = "Rus";
            DGV.Columns[3].HeaderText = "Theme";
            DGV.Columns[4].HeaderText = "Repeat";
            DGV.Columns[5].HeaderText = "Errors";
            DGV.Columns[6].HeaderText = "Count";
            DGV.Columns[7].HeaderText = "№";
            DGV.Rows.Clear();

            DGV.RowCount = Math.Max(1, list.Count);
            Parallel.For(0, DGV.RowCount, (i) =>
                {
                    DGV.Rows[i].Cells[0].Value = list[i].ID.ToString();
                    DGV.Rows[i].Cells[1].Value = list[i].Eng;
                    DGV.Rows[i].Cells[2].Value = list[i].Rus;
                    DGV.Rows[i].Cells[3].Value = list[i].Theme;
                    DGV.Rows[i].Cells[4].Value = list[i].Repeat.ToString();
                    DGV.Rows[i].Cells[5].Value = list[i].Errors.ToString();
                    DGV.Rows[i].Cells[6].Value = list[i].Count.ToString();
                    DGV.Rows[i].Cells[7].Value = (i + 1).ToString();
                });

            for (int i = 0; i < DGV.Columns.Count; i++)
            {
                DGV.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }


        }


        public void SortListBy(int Col, List<EWord> list)
        {

            switch (Col)
            {
                case 0: { list.Sort(new IDComparer()); break; }
                case 1: { list.Sort(new EngComparer()); break; }
                case 2: { list.Sort(new RusComparer()); break; }
                case 3: { list.Sort(new ThemeComparer()); break; }
                case 4: { list.Sort(new RepeatComparer()); break; }
                case 5: { list.Sort(new ErrorsComparer()); break; }
                case 6: { list.Sort(new CountComparer()); break; }
            }

        }

        public void SaveToGrid(DataGridView DGV, int SortColumn, int startID = 0, int endID = 0, string filter = "", int filterBy = 0)
        {


            List<EWord> res = new List<EWord>();
            if (filter == "") res = base.FindAll(w => true);
            else
                switch (filterBy)
                {
                    case 1: res = base.FindAll(r => r.Eng.ToLower().Contains(filter.ToLower())); break;
                    case 2: res = base.FindAll(r => r.Rus.ToLower().Contains(filter.ToLower())); break;
                    case 3: res = base.FindAll(r => r.Theme.ToLower().Contains(filter.ToLower())); break;
                    default: res = base.FindAll(w => true); break;
                }
            SortListBy(SortColumn, res);
            SaveListToGrid(DGV, res);

        }





    }

}
