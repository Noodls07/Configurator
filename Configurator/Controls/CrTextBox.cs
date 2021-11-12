using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Configurator.Controls
{
    static class CrTextBox
    {
        #region TextBox
        public static void BuildTextBox(object source, string sourcePath, string value)
        {
            TextBox textBox = new TextBox();
            textBox.Name = "txbox_" + sourcePath;
            textBox.Margin = new Thickness(2, 2, 5, 2);
            //textBox.Text = value;
            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox.VerticalAlignment = VerticalAlignment.Center;
            textBox.FontSize = 16;
            textBox.Foreground = Brushes.Black;
            textBox.TextChanged += TextBox_TextChanged;

            Binding bind = new Binding();
            bind.Source = source; //SelectedRv;
            bind.Path = new PropertyPath(sourcePath);
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textBox.SetBinding(TextBox.TextProperty, bind);
            textBox.Tag = textBox.Text;

            (Application.Current.MainWindow as MainWindow).csAllPropGrid.Children.Add(textBox);
            (Application.Current.MainWindow as MainWindow).csAllPropGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetColumn(textBox, 1);
            Grid.SetRow(textBox, (Application.Current.MainWindow as MainWindow).csAllPropGrid.RowDefinitions.Count - 2);
        }



        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.Changes.ElementAt(0).Offset != 0)
            {

                TreeViewItem itemSelected = (TreeViewItem)(Application.Current.MainWindow as MainWindow).RevizitTree.SelectedItem;
                if ((sender as TextBox).Name.Contains("Code"))
                {
                    string code = (sender as TextBox).Text;
                    string title = itemSelected.Header.ToString().Substring(itemSelected.Header.ToString().IndexOf("]") + 1).Trim();
                    itemSelected.Header = $"[{code}] {title}";
                    //SelectedRv.TableColumnName = code;

                    if (Worker.SelectedRv != null && Worker.SelectedRv.Type == TypeOfRv.Group)
                    {
                        Worker.ReplaceOldGroup((sender as TextBox).Tag.ToString(), (sender as TextBox).Text);
                        (sender as TextBox).Tag = (sender as TextBox).Text /*code*/;
                    }
                }

                if ((sender as TextBox).Name.Contains("Title"))
                {
                    string code = itemSelected.Header.ToString().Substring(1, itemSelected.Header.ToString().IndexOf("]") - 1);
                    string title = (sender as TextBox).Text;
                    itemSelected.Header = $"[{code}] {title}";
                }

                Worker.IOViewsList.ResetItem(Worker.SelectedIO.OrderInList);
            }
        }
        #endregion

    }
}
