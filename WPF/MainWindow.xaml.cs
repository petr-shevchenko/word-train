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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Path = "words.xml";
        public string MyWordsPath = "MyWords.xml";

        private EWordList LearnWords;
        private EWordList ExceptionalWord;

        public MainWindow()
        {
            InitializeComponent();
            LearnWords = new EWordList(10000, Path, progressBar);
            ExceptionalWord = new EWordList(1000, MyWordsPath, progressBar);
            UCManageWord.WTpb = progressBar;

            UCLearnWord.GotoPage = GotoPage;
            UCLearnWord.GoToWord = UCManageWord.GoToWord;

            UCTextAnalis.ExceptionalWords = ExceptionalWord;
            UCTextAnalis.LearningWords = LearnWords;
            UCTextAnalis.WTpb = progressBar; 
        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && (e.AddedItems[0] is TabItem))
            {
                UCManageWord.dgWordList.ItemsSource = null;
                UCExceptionWords.dgExceptionalWords.ItemsSource = null;
                if ((e.AddedItems[0] as TabItem).Name == "TabManage")
                {
                    UCManageWord.LearningWordList = LearnWords;
                    UCManageWord.ExceptionalWordList = ExceptionalWord;
                }
                else if ((e.AddedItems[0] as TabItem).Name == "TabException")
                {
                    UCExceptionWords.LearningWordList = LearnWords;
                    UCExceptionWords.ExceptionalWordList = ExceptionalWord;
                }
                else if ((e.AddedItems[0] as TabItem).Name == "TabLearn")
                {
                    UCLearnWord.dgThemeList.ItemsSource = LearnWords.ThemeList;
                    UCLearnWord.dgThemeList.Items.Refresh();
                    UCLearnWord.WordList = LearnWords;
                }
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string s = "";
            try
            {
                LearnWords.SaveToFile();
            }
            catch (Exception ex)
            {
                s +="Изучаемые слова: "+ex.Message+'\n';
            }
            try
            {
                ExceptionalWord.SaveToFile();
            }
            catch (Exception ex)
            {
                s += "Слова-исключения: " + ex.Message + '\n';
            }
            if (s != "")
            {
                e.Cancel = !(MessageBox.Show("При сохранении файлов перед закрытием произошли ошибки: " + '\n' + s + '\n' + "Всё равно закрыть программу?", "Ошибка при закрытии", MessageBoxButton.OKCancel) == MessageBoxResult.OK);
                 
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void GotoPage(int pageIndex)
        {
            tabControl.SelectedIndex = pageIndex;
        }
       
    }
}
