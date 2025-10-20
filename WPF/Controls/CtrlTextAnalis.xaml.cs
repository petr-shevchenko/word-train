using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace WordTrain
{
    /// <summary>
    /// Interaction logic for CtrlTextAnalis.xaml
    /// </summary>
    public partial class CtrlTextAnalis : UserControl
    {
        public EWordList ExceptionalWords { get; set; }
        public EWordList LearningWords { get; set; }

        public WTProgressBar WTpb { get; set; }

        public CtrlTextAnalis()
        {
            InitializeComponent();
        }

        private async void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
           
            if (ofd.ShowDialog() == true)
            {
                
                tbTheme.Text = Path.GetFileNameWithoutExtension(ofd.FileName);
                TextAnalis TA = new TextAnalis();
                TA.WTpb = this.WTpb;
                 await Task.Run(() => { 
                    TA.AnalyseText(ofd.FileName,ExceptionalWords, LearningWords);
                 });
                dgTextAnalisWords.ItemsSource = TA.TextWordList;
                tbTotalWordsCount.Text = TA.TextWordList.Count.ToString();
                tbLearningWordsCount.Text = TA.LearningWordsCount.ToString();
                tbLearningWordsPercent.Text = TA.PercentLearningWords.ToString("0.00%");
                tbExceptionalWordsCount.Text = TA.ExceptionalWordsCount.ToString();
                tbExceptionalWordsPercent.Text = TA.PercentExceptionalWords.ToString("0.00%");
                tbNewWordsCount.Text = (TA.TextWordList.Count-TA.ExceptionalWordsCount-TA.LearningWordsCount).ToString();
                tbNewWordsPercent.Text = (1 - TA.PercentExceptionalWords - TA.PercentLearningWords).ToString("0.00%");
            }
            
        }

        private void ContextMenuMoveToLearn_Click(object sender, RoutedEventArgs e)
        {
            foreach (TextAnalis.Word word in dgTextAnalisWords.SelectedItems)
            {
                EWord eword = new EWord()
                {
                    Eng = word.word,
                    Count = word.count,
                    Theme = tbTheme.Text
                };
                if (Convert.ToInt32((sender as MenuItem).Tag) == 1) LearningWords.AddNewWord(eword);
                else if (Convert.ToInt32((sender as MenuItem).Tag) == 2) ExceptionalWords.AddNewWord(eword);
            }
        }

       
    }
}
