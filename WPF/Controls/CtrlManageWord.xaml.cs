using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WordTrain
{
    /// <summary>
    /// Interaction logic for CtrlManageWord.xaml
    /// </summary>
    public partial class CtrlManageWord : UserControl
    {
        

        private List<EWord> filterWordList = new List<EWord>();
        private EWordList _LearningWordList;
        private EWordList _ExceptionalWordList;
        public EWordList LearningWordList { set { _LearningWordList = value; dgWordList.ItemsSource = _LearningWordList; } }
        public EWordList ExceptionalWordList { set { _ExceptionalWordList = value; } }
        public WTProgressBar WTpb { get; set; }
        public CtrlManageWord()
        {
            InitializeComponent();
        }

        

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
           if (cbAddSimilar.IsChecked == true) SetFilterWithSimilar();
           else timerCallBack(null);
        }

        private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbFilter.Text.Length == 0) dgWordList.ItemsSource = _LearningWordList;
            else
            {
                if (timer!=null) timer.Change(1000, 0);
                else timer = new Timer(timerCallBack, null, 1000, 0);
            }

        }

        private void SetFilter()
        {
            if (tbFilter.Text.Length > 2)
            {
                if (rbEng.IsChecked == true) filterWordList = _LearningWordList.Where(x => x.Eng.Contains(tbFilter.Text.ToLower())).ToList();
                else if (rbRus.IsChecked == true) filterWordList = _LearningWordList.Where(x => x.Rus.Contains(tbFilter.Text.ToLower())).ToList();
                else if (rbTheme.IsChecked == true) filterWordList = _LearningWordList.Where(x => x.Theme.Contains(tbFilter.Text)).ToList();
                dgWordList.ItemsSource = filterWordList;
            }
            else dgWordList.ItemsSource = _LearningWordList;
            dgWordList.Items.Refresh();
           
        }

        private Timer timer;
        private void timerCallBack(object state)
        {
            List<EWord> tempFilterWordList = new List<EWord>();
            string str = "";
            this.Dispatcher.Invoke(() => { str = tbFilter.Text.ToLower(); });
            int column = 0;
            this.Dispatcher.Invoke(() => {
                if (rbEng.IsChecked == true) column=1;
                else if (rbRus.IsChecked == true) column=2;
                else if (rbTheme.IsChecked == true) column=3;

            });

            if (str.Length > 0)
            {
                if (column == 1) tempFilterWordList = _LearningWordList.Where(x => x.Eng.ToLower().Contains(str)).ToList();
                else if (column == 2) tempFilterWordList = _LearningWordList.Where(x => x.Rus.ToLower().Contains(str)).ToList();
                else if (column == 3) tempFilterWordList = _LearningWordList.Where(x => x.Theme.ToLower().Contains(str)).ToList();
                this.Dispatcher.Invoke(() => { filterWordList = tempFilterWordList; dgWordList.ItemsSource = filterWordList; });
            }

            else this.Dispatcher.Invoke(() => { dgWordList.ItemsSource = _LearningWordList; });
            this.Dispatcher.Invoke(() => { dgWordList.Items.Refresh(); });
        
        }

        private async void SetFilterWithSimilar()
        {
            SetFilter();
            List<EWord> newFilterWordList=new List<EWord>();
            await Task.Run(() =>
            {
                WTpb.Dispatcher.Invoke(() => { WTpb.startBar("Анализируем слова.....", filterWordList.Count); });
                Parallel.ForEach(filterWordList, (word) =>
                {
                    WTpb.Dispatcher.Invoke(() => { WTpb.plusBar(); });
                    double koef = 0.0;
                    List<EWord> tempList = TextAnalis.GetSimilarWords(word.Eng, _LearningWordList, out koef);
                    newFilterWordList.Add(word);
                    if (koef > 79)
                    {
                        foreach (EWord add in tempList)
                        {
                          //  if ((newFilterWordList.FindIndex(x => x.Eng == add.Eng) < 0) && (filterWordList.FindIndex(x => x.Eng == add.Eng) < 0))
                                newFilterWordList.Add(add);
                        }
                    }
                });
                WTpb.Dispatcher.Invoke(() => { WTpb.closeBar(); });
            });
            filterWordList = newFilterWordList;
            dgWordList.ItemsSource = filterWordList;
            dgWordList.Items.Refresh();
        }


        private void AddLine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EWord word = new EWord() { Count=1, Repeat=1}; 
                _LearningWordList.AddNewWord(word);
                dgWordList.Items.Refresh();
                dgWordList.ScrollIntoView(word);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении строки: " + ex.Message);
            }

        }

        private void DelLine_Click(object sender, RoutedEventArgs e)
        {
            var a = (from w in dgWordList.SelectedCells select w.Item as EWord).Distinct().ToList();
            foreach (EWord ew in a)
            {
                _LearningWordList.Remove(ew);
                filterWordList.Remove(ew);
            }
            dgWordList.Items.Refresh();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dgWordList.Items.Count; i++)
                (dgWordList.Items[i] as EWord).N = i+1;
            await Task.Run(() =>
                {
                    _LearningWordList.SaveToFile();
                    _LearningWordList.ThemeList.BuildThemeList();
                });
            dgWordList.Items.Refresh();
            
        }

      

       

        private void ContextMenuMoveToExcept_Click(object sender, RoutedEventArgs e)
        {
            var a = (from w in dgWordList.SelectedCells select w.Item as EWord).Distinct().ToList();
            foreach (EWord ew in a)
            {
                _ExceptionalWordList.AddNewWord(ew);
                _LearningWordList.Remove(ew);
                filterWordList.Remove(ew);
            }
            dgWordList.Items.Refresh();
        }

        private void ResetRepeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Parallel.ForEach(_LearningWordList, (EWord ew) => { ew.Repeat = 1; });
                _LearningWordList.ThemeList.BuildThemeList();
                dgWordList.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сбросе повторов: " + ex.Message);
            }

        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Filter_Click(sender, e);
            }
            else if (e.Key == Key.Delete)
            {
                DelLine_Click(sender, e);
            }
        }



        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation def = new DoubleAnimation();
            def.From = 0;
            def.To = expandGrid.Height;
            def.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            expandGrid.BeginAnimation(DataGrid.HeightProperty, def);

        }

        private void AddWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EWord word = new EWord() { Eng = tbEng.Text, Rus = tbRus.Text, Theme = tbTheme.Text, Count=1, Repeat=1 };
                _LearningWordList.AddNewWord(word);
                dgWordList.Items.Refresh();
                dgWordList.ScrollIntoView(word);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении слова: " + ex.Message);
            }
        }

        private void btnSetText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in dgWordList.SelectedCells)
                {
                    EWord ew = item.Item as EWord;
                    switch (item.Column.SortMemberPath)
                    {
                        case "Eng": ew.Eng = tbInputTetx.Text; break;
                        case "Rus": ew.Rus = tbInputTetx.Text; break;
                        case "Theme": ew.Theme = tbInputTetx.Text; break;
                        case "Repeat": ew.Repeat = Convert.ToInt32(tbInputTetx.Text); break;
                        case "Count": ew.Count = Convert.ToInt32(tbInputTetx.Text); break;
                        case "Errors": ew.Errors = Convert.ToInt32(tbInputTetx.Text); break;
                    }
                }
            }
            catch (FormatException fEx)
            {
                MessageBox.Show("Ошибка при задании значения ячейкам (скорей всего данные не соответствуют типу ячейки): " + fEx.Message);            
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Ошибка при задании значения ячейкам: " + Ex.Message);
            }

            dgWordList.Items.Refresh();

        }


        public void GoToWord(EWord word)
        {
         //   dgWordList.Items.Refresh();
            dgWordList.ScrollIntoView(word);
           // dgWordList.Items.Refresh();
        }

    }
}
