using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace WordTrain
{
    public class MyWords
    {
        public MyWordList MWL = new MyWordList(10000);
        
        public class MyWord
        {
            public int ID = 0;
            public string Eng = "";
            public string Rus = "";
        }


        public class MyWordList : List<MyWord>
        {
            public MyWordList(int count)
                : base(count)
            { }

            private int LastID = 0;

            public void Clean()
            {
                base.Clear();
                LastID = 0;
            }

            public void AddNewWord(string eng, string rus)
            {
                if (base.FindIndex(s => s.Eng == eng.ToLower()) >= 0) return;
                MyWord rec = new MyWord();
                LastID++;
                rec.ID = LastID;
                rec.Eng = eng.ToLower();
                rec.Rus = rus.ToLower();
                base.Add(rec);

            }

            public void DelWord(int id)
            {
                MyWord w=base.Find(s=>s.ID==id);
                base.Remove(w);
            }

            public void LoadFromGrid(DataGridView DGV)
            {
                Clean();
                for (int i = 0; i < DGV.RowCount; i++)
                {
                    AddNewWord(DGV.Rows[i].Cells[1].Value.ToString(), DGV.Rows[i].Cells[2].Value.ToString());
                }

            }

            public bool LoadFromFile(string path)
            {
                try
                {
                    base.Clear();
                    XmlReader rdr = XmlReader.Create(path);
                    MyWord rec = new MyWord();
                    while (!rdr.EOF)
                    {
                        if (rdr.NodeType == XmlNodeType.Element)
                        {
                            switch (rdr.Name)
                            {
                                case "ID": rec.ID = rdr.ReadElementContentAsInt(); break;
                                case "Eng": rec.Eng = rdr.ReadElementContentAsString().ToLower(); break;
                                case "Rus": rec.Rus = rdr.ReadElementContentAsString().ToLower(); break;
                                default: rdr.Read(); break;
                            }
                        }
                        else
                            if ((rdr.NodeType == XmlNodeType.EndElement) && (rdr.Name == "Word"))
                            {
                                base.Add(rec);
                                LastID = Math.Max(rec.ID, LastID);
                                rec = new MyWord();
                                rdr.Read();
                            }
                            else rdr.Read();
                    }
                    rdr.Close();
                    return (true);
                }
                catch { return (false); }



            }

          
            
            public void SaveToFile(string path)
            {
                XmlWriter writer = XmlWriter.Create(path);
                writer.WriteStartDocument();
                writer.WriteStartElement("WordList");
                for (int i = 0; i < base.Count; i++)
                {
                    writer.WriteStartElement("Word");
                    writer.WriteElementString("ID", base[i].ID.ToString());
                    writer.WriteElementString("Eng", base[i].Eng);
                    writer.WriteElementString("Rus", base[i].Rus);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();


            }

            public void SaveToGrid(DataGridView DGV)
            {
                DGV.ColumnCount = 3;
                DGV.RowHeadersVisible = false;
                DGV.Columns[0].Width = 50;
                DGV.Columns[1].Width = 200;
                DGV.Columns[2].Width = 200;
                DGV.Columns[0].HeaderText = "№";
                DGV.Columns[1].HeaderText = "Eng";
                DGV.Columns[2].HeaderText = "Rus";
                DGV.Rows.Clear();

                DGV.RowCount = Math.Max(1, base.Count);
                for (int i = 0; i < base.Count; i++)
                {
                    DGV.Rows[i].Cells[0].Value = base[i].ID.ToString();
                    DGV.Rows[i].Cells[1].Value = base[i].Eng;
                    DGV.Rows[i].Cells[2].Value = base[i].Rus;
                }

                for (int i = 0; i < DGV.Columns.Count; i++)
                {
                    DGV.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
                }


            }
        }
    }
}
