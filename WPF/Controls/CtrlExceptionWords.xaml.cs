using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordTrain
{
    /// <summary>
    /// Interaction logic for CtrlExceptionWords.xaml
    /// </summary>
    public partial class CtrlExceptionWords : UserControl
    {
        private EWordList _LearningWordList;
        private EWordList _ExceptionalWordList;
        public EWordList LearningWordList { set { _LearningWordList = value; } }
        public EWordList ExceptionalWordList { set { _ExceptionalWordList = value; dgExceptionalWords.ItemsSource = _ExceptionalWordList; } }
        public CtrlExceptionWords()
        {
            InitializeComponent();
        }

        private void DelLine_Click(object sender, RoutedEventArgs e)
        {
            foreach (EWord ew in dgExceptionalWords.SelectedItems)
            {
                _ExceptionalWordList.Remove(ew);
            }
            dgExceptionalWords.Items.Refresh();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dgExceptionalWords.Items.Count; i++)
                (dgExceptionalWords.Items[i] as EWord).N = i + 1;
            dgExceptionalWords.Items.Refresh();
            _ExceptionalWordList.SaveToFile();
        }

        private void ContextMenuMoveToLearn_Click(object sender, RoutedEventArgs e)
        {
            foreach (EWord ew in dgExceptionalWords.SelectedItems)
            {
                _LearningWordList.AddNewWord(ew);
                _ExceptionalWordList.Remove(ew);
            }
            dgExceptionalWords.Items.Refresh();
        }
    }
}
