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


namespace Configurator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<IO> allIO ;
  
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            DbConector.Connect();
            allIO = Worker.GetAllIO();


            foreach (var item in allIO)
            {
                Button newB = new Button();
                newB.Name = "first" + item.Code;
                newB.Height = 30;
                newB.Width = mainGrid.ColumnDefinitions[0].ActualWidth;
                newB.VerticalAlignment = VerticalAlignment.Top;
                newB.HorizontalAlignment = HorizontalAlignment.Left;
                newB.Content = item.Title;

                newB.Click += NewB_Click;

                objStackPanel.Children.Add(newB);
            }



        }

        private void NewB_Click(object sender, RoutedEventArgs e)
        {



            //TreeViewItem item = new TreeViewItem();

            //item.Header = io.Title;

            

            //foreach (var rv in io.Rvs.Children)
            //{
            //    item.Items.Add(rv);
            //}
            
            
            
            //myTR.Items.Add(item);

            
        }

        private void TxBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //objStackPanel.Children.Add(newB);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
