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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CtrlLearnWord : UserControl
    {
        private EWord CurrentWord = new EWord();
        private bool IsChecked = false;
        private EWordList _wordList;


        private Brush incorrectAnsware;
        private Brush correctAnsware;
        private Brush defaultAnsware;

        public Action<EWord> GoToWord { get; set; }
        public Action<int> GotoPage;
        public EWordList WordList { set { _wordList = value; } }
        public CtrlLearnWord()
        {
            InitializeComponent();
            WordList = null;
            defaultAnsware = tbUserAnsware.Background;
            correctAnsware = new LinearGradientBrush(Color.FromRgb(0x55, 0x83, 0x64), Color.FromRgb(0x8B, 0xE0, 0xA6), new Point(0.5, 1), new Point(0.5, 0));
            incorrectAnsware = new LinearGradientBrush(Color.FromRgb(0xB0, 0x37, 0x37), Color.FromRgb(0xFB, 0xBA, 0xBA), new Point(0.5, 1), new Point(0.5, 0));

          
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_wordList == null) return;
            string theme = "";
            
            Theme th = (dgThemeList.SelectedItem as Theme);
            if (th != null) theme = th.Name;
            if ((rbLearning.IsChecked == true)||(rbChecking.IsChecked == true)) CurrentWord = _wordList.GetRandomWord(theme);
            else if (rbLearning2.IsChecked == true) CurrentWord = _wordList.GetRandomWord(theme,1);
            CurrentWord.Rus = CurrentWord.Rus.Replace("ё", "е");
            tbWord.Text = CurrentWord.Eng;
            tbUserAnsware.Text = "";
            tbCorrectAnsware.Text = "";
            tbUserAnsware.Background = defaultAnsware;
            IsChecked = false;
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            if (IsChecked) return;
            if (tbUserAnsware.Text == "") tbUserAnsware.Text = "@";
            tbUserAnsware.Text = tbUserAnsware.Text.ToLower().Replace("ё", "е");
            tbCorrectAnsware.Text = CurrentWord.Rus;
            CurrentWord.TempRepeatFlag = 0;
            if (CurrentWord.Rus.ToLower().Contains(tbUserAnsware.Text))
            {
                _wordList.DecRepeat(CurrentWord.ID);
                tbUserAnsware.Background = correctAnsware;
            }
            else
            {
                if ((rbLearning.IsChecked == true)||(rbLearning2.IsChecked == true)) _wordList.IncRepeat(CurrentWord.ID);
                else _wordList.SetRepeat(CurrentWord.ID, -1);
                _wordList.IncError(CurrentWord.ID);
                tbUserAnsware.Background = incorrectAnsware;
            }
            IsChecked = true;
            dgThemeList.Items.Refresh();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (IsChecked) BtnNext_Click(sender, e);
                else BtnCheck_Click(sender, e);
            
            }
        }

        private void tbWord_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GoToWord != null)
            {
                if (GotoPage != null) GotoPage(1);
                GoToWord(CurrentWord);
                
            }

        }
    }
}
