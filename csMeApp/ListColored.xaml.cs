using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ListViewColored : UserControl
    {
        public ListViewColored()
        {
            InitializeComponent();
            ContextMenu contextMenu1 = new ContextMenu();
            this.ContextMenu = contextMenu1;

            MenuItem menuItem = new MenuItem();
            menuItem.Header = "-delete-";
            menuItem.Click += delegate { AddItem(); };
            ContextMenu.Items.Add(menuItem);
            ListBoxItem item = new ListBoxItem();
            item.Content = "item  =  500€";
            //this.a
            //AddChild(item);
        }

        public void AddItem()
        {
            
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
