using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTrain
{
    public class EngComparer : IComparer<EWord>
    {
        public int Compare(EWord w1, EWord w2)
        {
            return w1.Eng.CompareTo(w2.Eng);
        }
    }

    public class RusComparer : IComparer<EWord>
    {
        public int Compare(EWord w1, EWord w2)
        {
            return w1.Rus.CompareTo(w2.Rus);
        }
    }
    public class ThemeComparer : IComparer<EWord>
    {
        public int Compare(EWord w1, EWord w2)
        {
            return w1.Theme.CompareTo(w2.Theme);
        }
    }

    public class IDComparer : IComparer<EWord>
    {
        public int Compare(EWord w1, EWord w2)
        {
            return w1.ID.CompareTo(w2.ID);
        }
    }
    public class RepeatComparer : IComparer<EWord>
    {
        public int Compare(EWord w1, EWord w2)
        {
            return w1.Repeat.CompareTo(w2.Repeat);
        }
    }

    public class ErrorsComparer : IComparer<EWord>
    {
        public int Compare(EWord w1, EWord w2)
        {
            return w2.Errors.CompareTo(w1.Errors);
        }
    }
    public class CountComparer : IComparer<EWord>
    {
        public int Compare(EWord w1, EWord w2)
        {
            return w2.Count.CompareTo(w1.Count);
        }
    }

}
