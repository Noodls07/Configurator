using Configurator.Model;
using Configurator.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Configurator.Controls
{
    static class CrTreeView
    {

        public static bool treeItemMoveOrDelete = false;
        //private static BooleanToStringConverter bc = new BooleanToStringConverter();

        #region Work with Tree
        /// <summary>
        /// Create Tree by rekvizites of current IO
        /// </summary>
        /// <ParentGroupam name="rvList"></ParentGroupam>
        /// <ParentGroupam name="item"></ParentGroupam>
        public static void CreateTreeItem(List<RvView> rvList, TreeViewItem item)
        {
            foreach (RvView rv in rvList)
            {
                TreeViewItem newItem = new TreeViewItem();
                newItem.Name = "item_" + rv.Code;
                newItem.Header = $"[{rv.Code}] {rv.Title}";
                newItem.Tag = rv.Type;
                newItem.Selected += NewItem_Selected;
                newItem.KeyDown += TreeItem_KeyDown;
                item.Items.Add(newItem);
                if (rv.Requisites.Any())
                {
                    CreateTreeItem(rv.Requisites, newItem);
                }
            }
        }

        /// <summary>
        /// Add New Rekvizit Item
        /// </summary>
        /// <ParentGroupam name="sender"></ParentGroupam>
        /// <ParentGroupam name="e"></ParentGroupam>
        public static void AddTreeItem()
        {
            if (Worker.SelectedIO == null) return;

            Rv rv = new Rv();
            rv.Code = Worker.GetNewCode();
            rv.Title = Worker.GetNewTitle(rv.Code);
            rv.Num = Worker.SelectedIO.Num;
            rv.Many = false;
            rv.Type = TypeOfRv.Integer;
            rv.Path = Worker.SelectedIO.Code;
            rv.State = StateTypes.Added;
            rv.ParentGroup = string.Empty;
            rv.TableColumnName = rv.Code;
            //Worker.SelectedIO.SetRvTablesName(Worker.SelectedIO.Code, ref rv);


            Worker.SelectedIO.Requisites.Add(new RvView(rv));
            Worker.SelectedIO.State = StateTypes.Modify;
            //IOViewsList.ResetItem(SelectedIO.OrderInList);

            TreeViewItem item = new TreeViewItem();
            item.Header = $"[{rv.Code}] {rv.Title}";
            item.Name = "item_" + rv.Code;
            item.Tag = rv.Type;
            item.Selected += NewItem_Selected;
            item.KeyDown += TreeItem_KeyDown;

            TreeViewItem item2 = new TreeViewItem();
            item2 = (TreeViewItem)(Application.Current.MainWindow as MainWindow).RevizitTree.Items[0];
            item2.Items.Add(item);
        }


        public static void TreeItem_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;//block BUBBLING
            if (e.Key == Key.Delete)
                DeleteTreeItem();
        }

        public static void DeleteTreeItem()
        {
            if (Worker.SelectedIO == null) return;

            TreeViewItem itemSelected = (TreeViewItem)(Application.Current.MainWindow as MainWindow).RevizitTree.SelectedItem;
            Worker.SetToDeletedState(Worker.SelectedIO.FindCurrentRv(itemSelected.Header.ToString()));
            Worker.IOViewsList.ResetItem(Worker.SelectedIO.OrderInList);
            //remove old ParentGroupent group
            DeleteTreeItemParentNode(itemSelected);
        }

        private static void DeleteTreeItemParentNode(TreeViewItem childNode)
        {
            treeItemMoveOrDelete = true;
            DependencyObject parent = childNode.Parent;
            (parent as ItemsControl).Items.Remove(childNode);
            treeItemMoveOrDelete = false;
        }


        /// <summary>
        /// Moving TreeViewItem depends by seted Group
        /// </summary>
        /// <ParentGroupam name="newGroupCode">Seted new group</ParentGroupam>
        public static void MoveTreeItem(string newGroupCode)
        {
            TreeViewItem itemSelected = (TreeViewItem)(Application.Current.MainWindow as MainWindow).RevizitTree.SelectedItem;
            //remove old Parent Node
            DeleteTreeItemParentNode(itemSelected);
            UpToSelected((TreeViewItem)(Application.Current.MainWindow as MainWindow).RevizitTree.Items[0],
                                        newGroupCode,
                                        itemSelected);
            itemSelected.IsSelected = true;
        }

        /// <summary>
        /// Insert selected TreeItem into new Item
        /// </summary>
        /// <ParentGroupam name="mainItem">Items of 2nd level for the start. Then recursive</ParentGroupam>
        /// <ParentGroupam name="serchedItemName">Item where need to insert</ParentGroupam>
        /// <ParentGroupam name="itemToSet">SelectedItem</ParentGroupam>
        private static void UpToSelected(TreeViewItem mainItem, string serchedItemName, TreeViewItem itemToSet)
        {
            if (string.IsNullOrEmpty(serchedItemName))
            {
                mainItem.Items.Add(itemToSet);
                return;
            }
            for (int i = 0; i < mainItem.Items.Count; i++)
            {
                //if ((mainItem.Items[i] as TreeViewItem).Tag.ToString() == "96")
                if ((mainItem.Items[i] as TreeViewItem).Header.ToString().Contains(serchedItemName))
                {
                    (mainItem.Items[i] as TreeViewItem).Items.Add(itemToSet);
                    break;
                }
                else
                    UpToSelected(mainItem.Items[i] as TreeViewItem, serchedItemName, itemToSet);
            }
        }


        public static void NewItem_Selected(object sender, RoutedEventArgs e)
        {
            e.Handled = true;//block BUBBLING

            if (treeItemMoveOrDelete) return;

            //removing blocks &boxs
            if ((Application.Current.MainWindow as MainWindow).csAllPropGrid.RowDefinitions.Count >= 1)
            {
                for (int i = (Application.Current.MainWindow as MainWindow).csAllPropGrid.Children.Count - 1; i >= 0; i--)
                {
                    UIElement el = (Application.Current.MainWindow as MainWindow).csAllPropGrid.Children[i];
                    (Application.Current.MainWindow as MainWindow).csAllPropGrid.Children.Remove(el);
                }
                (Application.Current.MainWindow as MainWindow).csAllPropGrid.RowDefinitions
                    .RemoveRange(0, (Application.Current.MainWindow as MainWindow).csAllPropGrid.RowDefinitions.Count - 1);
            }
            //---
            object currentObj;

            if ((Application.Current.MainWindow as MainWindow).RevizitTree.Items.IndexOf((TreeViewItem)sender) == 0)
            {
                currentObj = Worker.SelectedIO;
            }
            else
            {
                Worker.SelectedRv = Worker.SelectedIO.FindCurrentRv(((TreeViewItem)sender).Header.ToString());//(Application.Current.MainWindow as MainWindow).RevizitTree.SelectedItem as RvView;
                currentObj = Worker.SelectedRv;
            }

            PropertyInfo[] props = currentObj.GetType().GetProperties();

            foreach (KeyValuePair<string, string> property in Worker.ListOfBrowsablePropertys)
            {
                PropertyInfo pr = props.FirstOrDefault(x => x.Name == property.Key);

                if (pr is null) return;

                string pr_value = pr.ConvertToData(currentObj);

                CrTextBlock.BuildTextBlock(property.Key, property.Value);

                switch (property.Key)
                {
                    case "Many":
                    case "ParentGroup":
                    case "Type":
                        CrComboBox.BuildComboBox(property.Key, pr_value);
                        break;
                    default:
                        CrTextBox.BuildTextBox(currentObj, property.Key, pr_value);
                        break;
                }
            }
            #region all propertys
            //foreach (PropertyInfo pr in props)
            //{
            //    AttributeCollection at = TypeDescriptor.GetProperties(currentObj)[pr.Name].Attributes;
            //    BrowsableAttribute BrowsableAtt = (BrowsableAttribute)at[typeof(BrowsableAttribute)];
            //    DisplayNameAttribute DisplayNameAtt = (DisplayNameAttribute)at[typeof(DisplayNameAttribute)];

            //    if (BrowsableAtt.Browsable)
            //    {
            //        //prop names
            //        CrTextBlock tblock = new CrTextBlock();
            //        tblock.BuildTextBlock(pr.Name, DisplayNameAtt.DisplayName);
            //        //prop values 
            //        object value = pr.GetValue(currentObj);

            //        if (pr.PropertyType.FullName == typeof(bool).ToString() && pr.Name == "Many")
            //            value = (string)bc.ConvertTo(value, typeof(string));

            //        if (pr.Name == "Type")
            //            value = MyTypesConverter.GetEnumDescription((TypeOfRv)Convert.ToInt32(value));

            //        ////build components
            //        if (pr.Name == "Many" || pr.Name == "ParentGroup" || pr.Name == "Type")
            //        {
            //            CrComboBox cbox = new CrComboBox();
            //            cbox.BuildComboBox(pr.Name, value.ToString());
            //        }
            //        else
            //        {
            //            CrTextBox tbox = new CrTextBox();
            //            tbox.BuildTextBox(currentObj, pr.Name, value.ToString());
            //        }

            //        ////TODO DatePicker for DateProp
            //        ////TODO Combobox for Dictionary
            //    }
            //}
            #endregion
        }

        #endregion
    }
}
