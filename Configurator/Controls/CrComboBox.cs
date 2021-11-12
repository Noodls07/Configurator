using Configurator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Configurator.Controls
{
    static class CrComboBox
    {
        private static BooleanToStringConverter bc = new BooleanToStringConverter();
        #region ComboBox
        public static void BuildComboBox(string name, string value)
        {
            if (Worker.SelectedIO is null || Worker.SelectedRv is null)
            {
                return;
            }

            ComboBox comboBox = new ComboBox();
            comboBox.Name =  name + "Box";
            comboBox.Margin = new Thickness(2, 2, 5, 2);
            comboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            comboBox.VerticalAlignment = VerticalAlignment.Center;
            comboBox.FontSize = 16;
            comboBox.Foreground = Brushes.Black;
            if (name == "ParentGroup")
            {
                comboBox.ItemsSource = Worker.SelectedIO.AllGroups;
                comboBox.SelectedItem = value;
            }
            else if (name == "Type")
            {
                comboBox.ItemsSource = Worker.TypePropertyDescription;
                comboBox.SelectedValue = value;
            }
            else if (name == "Many")
            {
                comboBox.ItemsSource = Worker.ManyDescription;
                comboBox.SelectedValue = value;
            }

            comboBox.SelectionChanged += Items_CurrentChanged;

            //?? how bind to to change Text???????

            //Binding bind = new Binding();
            //bind.Source = SelectedRv;
            //bind.Path = new PropertyPath(name);
            //bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //comboBox.SetBinding(ComboBox.TextProperty , bind);

            (Application.Current.MainWindow as MainWindow).csAllPropGrid.Children.Add(comboBox);
            (Application.Current.MainWindow as MainWindow).csAllPropGrid.RowDefinitions.Add(new RowDefinition());
            Grid.SetColumn(comboBox, 1);
            Grid.SetRow(comboBox, (Application.Current.MainWindow as MainWindow).csAllPropGrid.RowDefinitions.Count - 2);
        }

        private static void Items_CurrentChanged(object sender, EventArgs e)
        {
            if (Worker.SelectedRv is null)
            {
                return;
            }

            //multy
            if ((sender as ComboBox).Name == "ManyBox")
            {
                if ((sender as ComboBox).SelectionBoxItem != (sender as ComboBox).SelectedItem)
                {
                    bool value = (bool)bc.ConvertFrom((sender as ComboBox).SelectedItem);
                    if (Worker.SelectedRv.Many != value)
                    {
                        Worker.SelectedRv.Many = value;
                        Worker.IOViewsList.ResetItem(Worker.SelectedIO.OrderInList);
                    }
                }
            }
            //types of Rv
            if ((sender as ComboBox).Name == "TypeBox")
            {
                if ((sender as ComboBox).SelectionBoxItem != (sender as ComboBox).SelectedItem)
                {
                    TypeOfRv value = MyTypesConverter.GetEnumValue((sender as ComboBox).SelectedItem.ToString());
                    TreeViewItem itemSelected = (TreeViewItem)(Application.Current.MainWindow as MainWindow).RevizitTree.SelectedItem;
                    if (Worker.SelectedRv.Type != value)
                    {
                        Worker.SelectedRv.Type = value;
                        Worker.SelectedRv.Image = Worker.SelectedRv.SetImage(Worker.SelectedRv.Type);
                        itemSelected.Tag = Worker.SelectedRv.Type;

                        if (Worker.SelectedRv.Type == TypeOfRv.Group)
                        {
                            Worker.SelectedRv.Many = true;
                            Worker.SelectedIO.AllGroups.Add(itemSelected.Header.ToString().Substring(1, itemSelected.Header.ToString().IndexOf("]") - 1));
                           
                            if (!Worker.SelectedIO.AllGroups.Contains(Worker.SelectedRv.Code))
                            {
                                Worker.SelectedIO.AllGroups.Add(Worker.SelectedRv.Code);
                            }
                        }
                        else
                        {
                            Worker.SelectedIO.AllGroups.RemoveAll(x => x == itemSelected.Header.ToString().Substring(1, itemSelected.Header.ToString().IndexOf("]") - 1));
                            Worker.SelectedIO.AllGroups.Remove(Worker.SelectedRv.Code);
                        }
                        //itemSelected.IsSelected = false;
                        //itemSelected.IsSelected = true;
                        Worker.IOViewsList.ResetItem(Worker.SelectedIO.OrderInList);
                    }
                }
            }
            //groups
            if ((sender as ComboBox).Name == "ParentGroupBox")
            {
                if ((sender as ComboBox).SelectionBoxItem != (sender as ComboBox).SelectedItem && Worker.SelectedRv.Code != (sender as ComboBox).SelectedItem.ToString())
                {
                    CrTreeView.MoveTreeItem((sender as ComboBox).SelectedItem.ToString());
                    Worker.SelectedIO.RemoveRv(Worker.SelectedRv, Worker.SelectedIO.Requisites);//1 - remove from old ParentGroup
                    Worker.SelectedRv.ParentGroup = (sender as ComboBox).SelectedItem.ToString();//2 - set new ParentGroup
                    Worker.SelectedIO.MoveRvToGroup(Worker.SelectedRv, Worker.SelectedIO.Requisites);//3 - move to new ParentGroup

                    Worker.IOViewsList.ResetItem(Worker.SelectedIO.OrderInList);
                }
                else
                    (sender as ComboBox).SelectedItem = (sender as ComboBox).SelectionBoxItem;
            }
            //(Application.Current.MainWindow as MainWindow).RevizitTree.Items.Refresh();
            Worker.SelectedRv.SetRvTablesName(Worker.SelectedIO.Code, Worker.SelectedRv);
        }

        #endregion
    }
}
