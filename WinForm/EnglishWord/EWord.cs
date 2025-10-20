using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTrain
{
    public class EWord
    {
        public int ID { get; set; }
        public string Theme { get; set; }
        public string Eng { get; set; }
        public string Rus { get; set; }
        public int Repeat { get; set; }
        public int Errors { get; set; }
        public int Count { get; set; }

        public EWord()
        {
            ID = 0;
            Theme = "";
            Eng = "";
            Rus = "";
            Repeat = 0;
            Errors = 0;
            Count = 0;
        }
    }

}
