using Configurator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;


namespace Configurator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
  
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Worker.Start();
            Worker.ShowIO();


            Add_Item.Click += Add_Item_Click;
            Delete_Item.Click += Delete_Item_Click;

            Add_IO.Click += Add_IO_Click;
            Delete_IO.Click += Delete_IO_Click;

            SyncBtn.Click += SyncBtn_Click;
            SaveBtn.Click += SaveBtn_Click;
            CloseBtn.Click += CloseBtn_Click;

            //ComboBoxes action
            //ManyBox.SelectionChanged += ComboBox_SelectionChanged;
            //ParentGroupBox.SelectionChanged += ComboBox_SelectionChanged;
            //TypeBox.SelectionChanged += ComboBox_SelectionChanged;

            //CodeBox.TextChanged += CodeBox_TextChanged;
        }

        private void SyncBtn_Click(object sender, RoutedEventArgs e)
        {
            Worker.SyncDB();
        }

        //private void CodeBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //Worker.TextBox_TextChanged(sender, e);
        //}

        private void RevizitTree_SelectedItem(object sender, RoutedEventArgs e)
        {
            //(e.OriginalSource as TreeViewItem).IsSelected = false;
            //Worker.TreeItem_Selected(sender, e);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Worker.SaveChanges();
        }

        private void Delete_IO_Click(object sender, RoutedEventArgs e)
        {
            Worker.RemoveIOButton();
        }

        private void Add_IO_Click(object sender, RoutedEventArgs e)
        {
            Worker.CreateNewIO();
        }

        private void Add_Item_Click(object sender, RoutedEventArgs e)
        {
            Worker.AddTreeItem();
        }

        private void Delete_Item_Click(object sender, RoutedEventArgs e)
        {
            Worker.DeleteTreeItem();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
           
            dbWorker.Disconnect();
            Environment.Exit(0);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           DragMove();
        }

        private void RevizitTree_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            //Worker.DeleteTreeItem();
            Worker.TreeItem_KeyDown(sender, e);
        }

        //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Worker.ComboBoxItems_CurrentChanged(sender, e);
        //}
    }
}
