using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace csMeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DateTime MyDate = DateTime.Today;
        SqliteHelper db;
        Dictionary<int, string> activityTypes;

        int[] indexDown;
        int[] indexMove;

        public MainWindow()
        {
            db = new SqliteHelper();
            if (!db.SqlConnect())
                MessageBox.Show(db.Message, "SqliteHelper throw Exception", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);           
            InitializeComponent();
        }

        /// <summary>
        /// i-column and j-row by coordinates
        /// </summary>
        /// <returns>0 - 23</returns>
        private int[] getIndexOfReportGrid(Point coords)
        {
            double columnWidth = ReportGrid.ColumnDefinitions[0].ActualWidth;
            double rowHeight = ReportGrid.RowDefinitions[0].ActualHeight;
            
            double gridWidth = ReportGrid.ActualWidth - 1;
            double gridHeight = ReportGrid.ActualHeight - 1;

            int i = (int)(coords.X / columnWidth);
            int j = (int)(coords.Y / rowHeight);

            return new int[] { i, j };
        }

        private void selectReportDay()
        {
            DataTable dt = db.SelectReportDay(MyDate);
            int i = 0;
            foreach (ContentControl el in ReportGrid.Children.Cast<ContentControl>())
            {
                el.Content = String.Format("{0}:00", i++);
            }
            //ReportGrid.Children.Cast<Label>()
            //.Select(a => a.Content = String.Format("{0}:00", i++));
            foreach (DataRow r in dt.Rows)
            {
                int hour = DateTime.Parse(r["datetime"].ToString()).Hour;
                ContentControl el = (ContentControl)ReportGrid.Children[hour];
                el.Content = el.Content.ToString() + " :  " + r["activity"].ToString();
            }
        }

        private void reportHours(int id_activity)
        {
            int hour = indexDown[0] * 8 + indexDown[1];
            if(!db.insertReportHour(MyDate.AddHours(hour), id_activity))
                MessageBox.Show(db.Message, "SqliteHelper insertReportHour throw Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            selectReportDay();
        }

        private void deleteHour()
        {
            int hour = indexDown[0] * 8 + indexDown[1];
            if(!db.deleteReportHour(MyDate.AddHours(hour)))
                MessageBox.Show(db.Message, "SqliteHelper deleteReportHour throw Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            selectReportDay();
        }



        // CONTROL REPORT TAB EVENTS
        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
            activityTypes = db.selectActivityTypes();
            if (activityTypes == null)
                MessageBox.Show(db.Message, "SqliteHelper activityTypes throw Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            ContextMenu contextMenu1 = new ContextMenu();
            foreach (KeyValuePair<int, string> activity in activityTypes)
            {
                MenuItem menuItem1 = new MenuItem();
                menuItem1.Header = activity.Value;
                menuItem1.Click += delegate { reportHours(activity.Key); };
                contextMenu1.Items.Add(menuItem1);
            }
            MenuItem menuItemDelete = new MenuItem();
            menuItemDelete.Header = "-delete-";
            menuItemDelete.Click += delegate { deleteHour(); };
            contextMenu1.Items.Add(menuItemDelete);
            ReportGrid.ContextMenu = contextMenu1;

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 8; j++)
                {
                    Button label = new Button();
                    label.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    label.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    label.Content = (i * 8 + j).ToString();
                    //label.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    //label.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    label.Background = Brushes.LightBlue;
                    Grid.SetRow(label, j);
                    Grid.SetColumn(label, i);
                    ReportGrid.Children.Add(label);
                    //
                    butDate.SelectedDate = DateTime.Today;
                }
        }

        private void butPrevDate_Click(object sender, RoutedEventArgs e)
        {
            MyDate = MyDate.AddDays(-1);
            butDate.SelectedDate = MyDate;
            selectReportDay();
        }

        private void butNextDate_Click(object sender, RoutedEventArgs e)
        {
            MyDate = MyDate.AddDays(1);
            butDate.SelectedDate = MyDate;
            selectReportDay();
        }

        private void butDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            MyDate = butDate.SelectedDate.Value;
            if (ReportGrid.Children.Count != 24)
            {
                Thread.Sleep(100);
                return;
            }
            selectReportDay();
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            indexDown = getIndexOfReportGrid(e.GetPosition(ReportGrid));
        }

        private void butDate_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            selectReportDay();
        }

        private void ReportGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //indexDown = getIndexOfReportGrid(e.GetPosition(ReportGrid));
        }

        private void ReportGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Point coordsUp = e.GetPosition(ReportGrid);
            //foreach(Label label in ReportGrid.Children) 
            //    label.Background = Brushes.LightBlue;
        }

        private void ReportGrid_MouseMove(object sender, MouseEventArgs e)
        {
            //if(e.LeftButton == MouseButtonState.Pressed)
            //{
            //    indexMove = getIndexOfReportGrid(e.GetPosition(ReportGrid));
            //    if(indexMove[0] < 3 && indexMove[1] < 8)
            //    {
            //        Label lab = ReportGrid.Children.Cast<Label>()
            //            .First(a => Grid.GetRow(a) == indexMove[1] && Grid.GetColumn(a) == indexMove[0]);
            //        lab.Background = Brushes.LightGoldenrodYellow;
            //    }
            //    else
            //    {
            //    }
            //}
        }


        // CONTROL DIARY TAB EVENTS

        Dictionary<int, string> dictPersons = new Dictionary<int, string>();
        Dictionary<int, Tuple<int, DateTime, string>> dictDiary = new Dictionary<int, Tuple<int, DateTime, string>>();
        //Dictionary<int, string> 

        private void RefreshTxtDiary()
        {
            txtDiary.Document.Blocks.Clear();
            dictPersons = db.selectPersonsNames();
            if (dictPersons != null)
                lstPersons.ItemsSource = dictPersons.Values; //Items. dict
            dictDiary = db.selectDiaryMessages();
            //txtDiary.Document. = "";
            if (dictDiary != null)
            {
                foreach (Tuple<int, DateTime, string> m in dictDiary.Values)
                {
                    Paragraph para = new Paragraph();
                    para.Inlines.Add(new Bold(new Run("[" + dictPersons[m.Item1] + "] ")));
                    //txtDiary.AppendText();
                    para.Inlines.Add(new Run(m.Item2.ToShortDateString()));
                    para.Inlines.Add(new LineBreak());
                    para.Inlines.Add(new Run(m.Item3));

                    txtDiary.Document.Blocks.Add(para);
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabControl = sender as TabControl;
            var item = tabControl.SelectedItem as TabItem;
            if (item.Name == "tabDiaryMe")
            {
                RefreshTxtDiary();
                //lstPersons.Items;
                e.Handled = true;
            }
        }

        private void lstPersons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   
            //
            lblPerson.Content = dictPersons.Values.ToArray<string>()[lstPersons.SelectedIndex] + ":";
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    // next line
                    int ind = txtMessage.CaretIndex;
                    txtMessage.Text += Environment.NewLine;
                    txtMessage.CaretIndex = ind + Environment.NewLine.Length;
                }
                else
                {
                    if (lstPersons.SelectedIndex == -1)
                        MessageBox.Show("Chose a person before <Enter>","list selection problem",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Send Message
                    else if (db.insertDiaryMessage(txtMessage.Text, dictPersons.Keys.ToArray<int>()[lstPersons.SelectedIndex], DateTime.Now))
                    {
                        txtMessage.Clear();
                        RefreshTxtDiary();
                    }
                    else
                        MessageBox.Show(db.Message, "SqliteHelper insertDiaryMessage() throw Exception",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        

    }
}