using Configurator.Model;
using Configurator.ViewModel;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Globalization;
using Configurator.Controls;

namespace Configurator
{
    static class Worker
    {
        public static BindingList<IOView> IOViewsList;
        public static List<IOView> IOSyncList;
        public static List<string> TypePropertyDescription = new List<string>();
        public static List<string> ManyDescription = new List<string>() { "Да", "Нет" };
        private static BooleanToStringConverter bc = new BooleanToStringConverter();
        public static Dictionary<string, string> ListOfBrowsablePropertys;

        public static IOView SelectedIO { get; set; }

        public static RvView SelectedRv { get; set; }

        public static void Start()
        {
            if (dbWorker.Connect())
            {
                GetAllIOView();
                FillTypePropertyDescription();
                ListOfBrowsablePropertys = GetRvBrowsableAttributesDisplayName();
                dbWorker.Disconnect();
                IOViewsList.ListChanged += IOViewsList_ListChanged;
            }
        }

        private static void FillTypePropertyDescription()
        {
            for (int i = 0; i < Enum.GetValues(typeof(TypeOfRv)).Length; i++)
            {
                object val = Enum.GetValues(typeof(TypeOfRv)).GetValue(i);
                val = MyTypesConverter.GetEnumDescription((TypeOfRv)Convert.ToInt32(val));
                TypePropertyDescription.Add(val.ToString());
            }
        }

        private static void IOViewsList_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.Reset:
                    break;
                case ListChangedType.ItemAdded:
                    break;
                case ListChangedType.ItemDeleted:

                    break;
                case ListChangedType.ItemMoved:
                    break;
                case ListChangedType.ItemChanged:
                    if (SelectedIO.State != StateTypes.Added)
                    {
                        SelectedIO.State = StateTypes.Modify;
                    }

                    break;
                case ListChangedType.PropertyDescriptorAdded:
                    break;
                case ListChangedType.PropertyDescriptorDeleted:
                    break;
                case ListChangedType.PropertyDescriptorChanged:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Collect all IO from DB
        /// </summary>
        /// <returns></returns>
        public static void GetAllIOView()
        {
            IOViewsList = new BindingList<IOView>();
            IOSyncList = new List<IOView>();
            int counter = -1;


            SqliteCommand sqlite_cmd = dbWorker.newConn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT NUM, CODE, TITLE FROM IOList order by NUM";

            SqliteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                IO io = new IO();
                io.Num = Convert.ToInt32(sqlite_datareader["NUM"]);// sqlite_datareader.GetInt32(sqlite_datareader.GetOrdinal("NUM"));
                io.Code = Convert.ToString(sqlite_datareader["CODE"]);//sqlite_datareader.GetString(sqlite_datareader.GetOrdinal("CODE"));
                io.Title = Convert.ToString(sqlite_datareader["TITLE"]);//sqlite_datareader.GetString(sqlite_datareader.GetOrdinal("TITLE"));
                io.State = StateTypes.Sync;
                io.OrderInList = ++counter;
                io.TableName = $"IF_{io.Code}";
                io.AllGroups = new List<string>();

                //get all groups 
                SqliteCommand sqcmd = dbWorker.newConn.CreateCommand();
                sqcmd.CommandText = "SELECT DISTINCT PAR FROM IOLogic WHERE  PAR is NOT NULL and PAR <>'' AND  NUM = " + sqlite_datareader.GetInt32(sqlite_datareader.GetOrdinal("NUM"));
                SqliteDataReader gr_sqlite_datareader = sqcmd.ExecuteReader();
                while (gr_sqlite_datareader.Read())
                {
                    io.AllGroups.Add(Convert.ToString(gr_sqlite_datareader["PAR"]));//gr_sqlite_datareader.GetString(gr_sqlite_datareader.GetOrdinal("PAR")));
                }
                io.AllGroups.Insert(0, string.Empty);
                //--

                IOViewsList.Add(new IOView(io));
                IOSyncList.Add(new IOView(io));
            }


            foreach (IOView ioview in IOViewsList)
            {
                CollectAllRvForView(ioview);
                ioview.SortRv();
                ioview.SetPath(ioview.Requisites, ioview.Code);
            }

            //foreach (IOView iosync in IOSyncList)
            //{
            //    iosync.Requisites = GetAllRvForView(iosync);
            //    iosync.SortRv(iosync);
            //    iosync.SetPath(iosync.Requisites,iosync.Code);
            //}
        }

        /// <summary>
        /// Collect all rezvizite for each IO
        /// </summary>
        /// <ParentGroupam name="io"></ParentGroupam>
        /// <returns></returns>
        public static void CollectAllRvForView(IOView io)
        {
            SqliteCommand sqlite_cmd = dbWorker.newConn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT SN, NUM, CODE, TITLE, MANY, PAR, TYPE FROM IOLogic WHERE NUM = {io.Num}  ORDER BY SN ";

            SqliteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                Rv rv = new Rv();
                rv.Sn = Convert.ToInt32(sqlite_datareader["SN"]); //sqlite_datareader.GetInt32(sqlite_datareader.GetOrdinal("SN"));
                rv.Num = Convert.ToInt32(sqlite_datareader["NUM"]); //sqlite_datareader.GetInt32(sqlite_datareader.GetOrdinal("NUM"));
                rv.Code = Convert.ToString(sqlite_datareader["CODE"]); //sqlite_datareader.GetString(sqlite_datareader.GetOrdinal("CODE"));
                rv.Title = Convert.ToString(sqlite_datareader["TITLE"]); //sqlite_datareader.GetString(sqlite_datareader.GetOrdinal("TITLE"));
                rv.Many = Convert.ToBoolean(Convert.ToInt32(sqlite_datareader["MANY"]));// sqlite_datareader.GetInt32(sqlite_datareader.GetOrdinal("MANY")));//(bool)bc.ConvertFrom
                rv.ParentGroup = !sqlite_datareader.IsDBNull(sqlite_datareader.GetOrdinal("PAR")) ?
                                  Convert.ToString(sqlite_datareader["PAR"]) : ""; //sqlite_datareader.GetString(sqlite_datareader.GetOrdinal("PAR")) : "";
                rv.Type = (TypeOfRv)Enum.ToObject(typeof(TypeOfRv), Convert.ToInt32(sqlite_datareader["TYPE"])); //sqlite_datareader.GetInt32(sqlite_datareader.GetOrdinal("TYPE")));
                rv.State = StateTypes.Sync;
                rv.ParentIOCode = io.Code;
                rv.TableColumnName = rv.Code;
                //io.SetRvTablesName(io.Code, ref rv);


                //rv.ParentIO = io;

                io.Requisites.Add(new RvView(rv));
                io.UnsortedRequisites.Add(new RvView(rv));
            }
        }

        public static void ShowIO()
        {
            foreach (IOView item in IOViewsList)
            {
                CreateButton(item.Code, item.Title, item.OrderInList);
            }
            //SynchronizeDB();
        }

        private static TypeOfRv GetMyType(int index)
        {
            return (TypeOfRv)Enum.GetValues(typeof(TypeOfRv)).GetValue(Convert.ToInt32(index));
        }


        private static void SynchronizeDB()
        {
            dbWorker.Connect();
            foreach (IOView io in IOViewsList)
            {
                if (io.State == StateTypes.Sync)
                {
                    if (!dbWorker.ColumnOrTableExist(tableName: io.TableName))
                        dbWorker.CreateTable(tableName: io.TableName, ioObj: true);

                    SynchronizeRv(io.Requisites);
                }
            }
            dbWorker.Disconnect();
        }
        private static void SynchronizeRv(List<RvView> rvList)
        {
            foreach (RvView rv in rvList)
            {
                if (rv.State != StateTypes.Sync)
                {
                    continue;
                }
                
                if (rv.Type == TypeOfRv.Group) //group
                {
                    if (!dbWorker.ColumnOrTableExist(tableName: rv.ParentTableName, colName: rv.TableColumnName, column: true))
                    {
                        dbWorker.CreateColumn(rv.TableColumnName, rv.ParentTableName, MyTypesConverter.ConvertToDB(rv.Type));
                    }

                    if (!dbWorker.ColumnOrTableExist(tableName: rv.TableName))
                    {
                        dbWorker.CreateTable(rv.TableName, refTable: rv.ParentTableName);
                    }
                    SynchronizeRv(rv.Requisites);
                }
                else
                if (!rv.Many) //simple
                {
                    if (!dbWorker.ColumnOrTableExist(tableName: rv.ParentTableName, colName: rv.TableColumnName, column: true))
                       dbWorker.CreateColumn(rv.TableColumnName, rv.ParentTableName, MyTypesConverter.ConvertToDB(rv.Type));
                }
                else
                if (rv.Many) //many
                {
                    if (!dbWorker.ColumnOrTableExist(tableName: rv.ParentTableName, colName: rv.TableColumnName, column: true))
                    {
                        dbWorker.CreateColumn(rv.TableColumnName, rv.ParentTableName, MyTypesConverter.ConvertToDB(TypeOfRv.Integer));//counter in par tbl
                    }

                    if (!dbWorker.ColumnOrTableExist(tableName: rv.TableName))
                    {
                        dbWorker.CreateTable(rv.TableName, colName: rv.TableColumnName, refTable: rv.ParentTableName, MyTypesConverter.ConvertToDB(rv.Type));
                    }
                }
            }
        }

        public static Dictionary<string, string> GetRvBrowsableAttributesDisplayName()
        {
            RvView rv = new RvView();
            Dictionary<string, string> res = new Dictionary<string, string>();

            foreach (PropertyInfo prop in rv.GetType().GetProperties())
            {
                AttributeCollection at = TypeDescriptor.GetProperties(rv)[prop.Name].Attributes;
                BrowsableAttribute BrowsableAtt = (BrowsableAttribute)at[typeof(BrowsableAttribute)];

                if (BrowsableAtt.Browsable)
                {
                    DisplayNameAttribute DisplayNameAtt = (DisplayNameAttribute)at[typeof(DisplayNameAttribute)];
                    res.Add(prop.Name, DisplayNameAtt.DisplayName);
                }
            }

            return res;
                
        }

        #region Buttons

        public static void CreateButton(string name, string content, int ioOrder, bool addedIO = false)
        {
            Button button = new Button();
            button.Name = "button_" + name;
            button.Height = 30;
            //button.Margin = new Thickness(10, 0, 0, 0);
            button.Width = (Application.Current.MainWindow as MainWindow).mainGrid.ColumnDefinitions[4].ActualWidth - 70;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Content = content;
            button.Tag = ioOrder;
            button.Click += button_Click;
            (Application.Current.MainWindow as MainWindow).objStackPanel.Children.Add(button);
            if (addedIO)
            {
                button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private static void button_Click(object sender, RoutedEventArgs e)
        {
            //if bind by xaml
            //SelectedIO = IOViewsList[(int)((Button)sender).Tag];
            //(Application.Current.MainWindow as MainWindow).RevizitTree.ItemsSource = SelectedIO.Requisites;
            //--

            //dinamic create tree
            SelectedIO = IOViewsList[(int)((Button)sender).Tag];
            (Application.Current.MainWindow as MainWindow).RevizitTree.Items.Clear();
            //main TreeViewNode
            TreeViewItem item = new TreeViewItem();
            item.Header = $"[{SelectedIO.Code}] {SelectedIO.Title}";
            item.Selected += CrTreeView.NewItem_Selected;
            (Application.Current.MainWindow as MainWindow).RevizitTree.Items.Add(item);
            CrTreeView.CreateTreeItem(SelectedIO.Requisites, item);

        }

        public static void RemoveIOButton()
        {
            SelectedIO.State = StateTypes.Deleted;

            foreach (RvView rv in SelectedIO.Requisites)
            {
                SetToDeletedState(rv);
            }

            for (int i = (Application.Current.MainWindow as MainWindow).objStackPanel.Children.Count - 1; i >= 0; i--)
            {
                UIElement el = (Application.Current.MainWindow as MainWindow).objStackPanel.Children[i];
                if ((el as Button).Content.ToString().Contains(SelectedIO.Title))
                {
                    (Application.Current.MainWindow as MainWindow).objStackPanel.Children.Remove(el);
                    break;
                }
            }

            (Application.Current.MainWindow as MainWindow).RevizitTree.Items.Clear();
            //(Application.Current.MainWindow as MainWindow).RevizitTree.ItemsSource = null;

        }
        #endregion

        #region TextBox wpf
        //public static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (e.Changes.ElementAt(0).Offset != 0)
        //    {

        //        //TreeViewItem itemSelected = (TreeViewItem)(Application.Current.MainWindow as MainWindow).RevizitTree.SelectedItem;
        //        if ((sender as TextBox).Name.Contains("Code"))
        //        {
        //            if (SelectedRv != null && SelectedRv.Type == TypeOfRv.Group)
        //            {
        //                ReplaceOldGroup((sender as TextBox).Tag.ToString(), (sender as TextBox).Text);
        //                (sender as TextBox).Tag = (sender as TextBox).Text /*code*/;
        //            }
        //        }
        //        IOViewsList.ResetItem(SelectedIO.OrderInList);
        //    }
        //}
        //#endregion

        //#region ComboBox wpf
        //public static void ComboBoxItems_CurrentChanged(object sender, EventArgs e)
        //{
        //    //ComboBoxItem cbi = (ComboBoxItem)(sender as ComboBox).SelectedItem;
        //    //multy
        //    if ((sender as ComboBox).Name == "ManyBox")
        //    {
        //        if ((sender as ComboBox).SelectionBoxItem != (sender as ComboBox).SelectedItem)
        //        {
        //            if (SelectedRv.Many != (bool)bc.ConvertFrom((sender as ComboBox).SelectedItem))
        //            {
        //                SelectedRv.Many = (bool)bc.ConvertFrom((sender as ComboBox).SelectedItem);

        //                //IOViewsList.ResetItem(SelectedIO.OrderInList);
        //                SelectedIO.State = StateTypes.Modify;
        //            }

        //        }
        //    }
        //    //types of Rv
        //    if ((sender as ComboBox).Name == "TypeBox")
        //    {
        //        if ((sender as ComboBox).SelectionBoxItem != (sender as ComboBox).SelectedItem)
        //        {
        //            if (SelectedRv.Type != MyTypesConverter.GetEnumValue((sender as ComboBox).SelectedItem.ToString()))
        //            {
        //                SelectedRv.Type = MyTypesConverter.GetEnumValue((sender as ComboBox).SelectedItem.ToString());
        //                SelectedRv.Image = SelectedRv.SetImage(SelectedRv.Type);

        //                if (SelectedRv.Type == TypeOfRv.Group)
        //                {
        //                    SelectedRv.Many = true;
        //                    if (!SelectedIO.AllGroups.Contains(SelectedRv.Code))
        //                        SelectedIO.AllGroups.Add(SelectedRv.Code);
        //                }
        //                else
        //                    SelectedIO.AllGroups.Remove(SelectedRv.Code);

        //                IOViewsList.ResetItem(SelectedIO.OrderInList);
        //            }
        //        }
        //    }
        //    //groups
        //    if ((sender as ComboBox).Name == "ParentGroupBox")
        //    {
        //        if ((sender as ComboBox).SelectionBoxItem != (sender as ComboBox).SelectedItem && SelectedRv.Code != (sender as ComboBox).SelectedItem.ToString())
        //        {
        //            SelectedIO.RemoveRv(SelectedRv, SelectedIO.Requisites);//1 - remove from old ParentGroup
        //            SelectedRv.ParentGroup = (sender as ComboBox).SelectedItem.ToString();//2 - set new ParentGroup
        //            SelectedIO.MoveRvToGroup(SelectedRv, SelectedIO.Requisites);//3 - move to new ParentGroup
        //            SelectedRv.TableName = $"IF_{SelectedIO.Code}_{(sender as ComboBox).SelectedItem.ToString()}";

        //            IOViewsList.ResetItem(SelectedIO.OrderInList);
        //        }
        //        else
        //            (sender as ComboBox).SelectedItem = (sender as ComboBox).SelectionBoxItem;
        //    }
        //    (Application.Current.MainWindow as MainWindow).RevizitTree.Items.Refresh();
        //}
        #endregion

        #region AddTreeItem wpf
        //public static void AddTreeItem()
        //{
        //    if (SelectedIO == null) return;

        //    Rv rv = new Rv();
        //    rv.Code = GetNewCode();
        //    rv.Title = GetNewTitle(rv.Code);
        //    rv.Num = SelectedIO.Num;
        //    rv.Many = false;
        //    rv.Type = TypeOfRv.Integer;
        //    rv.Path = SelectedIO.Code;
        //    rv.State = StateTypes.Added;
        //    rv.ParentGroup = string.Empty;
        //    rv.TableColumnName = rv.Code;
        //    rv.TableName = string.IsNullOrEmpty(rv.ParentGroup) ? $"IF_{SelectedIO.Code}" : $"IF_{SelectedIO.Code}_{rv.ParentGroup}";

        //    SelectedIO.Requisites.Add(new RvView(rv));
        //    SelectedIO.State = StateTypes.Modify;
        //    //IOViewsList.ResetItem(SelectedIO.OrderInList);

        //    (Application.Current.MainWindow as MainWindow).RevizitTree.Items.Refresh();
        //}

        //public static void RefreshTree()
        //{
        //    (Application.Current.MainWindow as MainWindow).RevizitTree.Items.Clear();
        //    TreeViewItem item = new TreeViewItem();
        //    item.Header = SelectedIO.Title;
        //    (Application.Current.MainWindow as MainWindow).RevizitTree.Items.Add(item);
        //    CreateTreeItem(SelectedIO.Requisites, item);
        //}
        #endregion

        #region TreeItem_Selected wpf
        //public static void TreeItem_Selected(object sender, RoutedEventArgs e)
        //{
        //    //TreeViewItem item = (TreeViewItem)(Application.Current.MainWindow as MainWindow).RevizitTree
        //    //                    .ItemContainerGenerator
        //    //                    .ContainerFromItem((Application.Current.MainWindow as MainWindow).RevizitTree.SelectedItem);

        //    e.Handled = true;
        //    SelectedRv = (sender as TreeView).SelectedItem as RvView;

        //    //SelectedRv.Selected = true;
        //    //detach actions for fill elements
        //    (Application.Current.MainWindow as MainWindow).TypeBox.SelectionChanged -= ComboBoxItems_CurrentChanged;
        //    (Application.Current.MainWindow as MainWindow).ManyBox.SelectionChanged -= ComboBoxItems_CurrentChanged;
        //    (Application.Current.MainWindow as MainWindow).ParentGroupBox.SelectionChanged -= ComboBoxItems_CurrentChanged;
        //    //--

        //    (Application.Current.MainWindow as MainWindow).csAllPropGrid.DataContext = SelectedRv;

        //    (Application.Current.MainWindow as MainWindow).Code.Text = BrowsProperty[(Application.Current.MainWindow as MainWindow).Code.Name];
        //    (Application.Current.MainWindow as MainWindow).Title.Text = BrowsProperty[(Application.Current.MainWindow as MainWindow).Title.Name];
        //    (Application.Current.MainWindow as MainWindow).Many.Text = BrowsProperty[(Application.Current.MainWindow as MainWindow).Many.Name];
        //    (Application.Current.MainWindow as MainWindow).ParentGroup.Text = BrowsProperty[(Application.Current.MainWindow as MainWindow).ParentGroup.Name];
        //    (Application.Current.MainWindow as MainWindow).Type.Text = BrowsProperty[(Application.Current.MainWindow as MainWindow).Type.Name];

        //    (Application.Current.MainWindow as MainWindow).CodeBox.Tag = SelectedRv.Code;

        //    (Application.Current.MainWindow as MainWindow).TypeBox.ItemsSource = TypePropertyDescription;
        //    (Application.Current.MainWindow as MainWindow).TypeBox.SelectedItem = MyTypesConverter.GetEnumDescription(SelectedRv.Type);

        //    (Application.Current.MainWindow as MainWindow).ManyBox.ItemsSource = ManyDescription;
        //    (Application.Current.MainWindow as MainWindow).ManyBox.SelectedItem = (string)bc.ConvertTo(SelectedRv.Many, typeof(string));

        //    (Application.Current.MainWindow as MainWindow).ParentGroupBox.ItemsSource = SelectedIO.AllGroups;
        //    (Application.Current.MainWindow as MainWindow).ParentGroupBox.SelectedItem = SelectedRv.ParentGroup;//((sender as TreeView).SelectedItem as RvView).ParentGroup;

        //    //attach actions for fill elements
        //    (Application.Current.MainWindow as MainWindow).TypeBox.SelectionChanged += ComboBoxItems_CurrentChanged;
        //    (Application.Current.MainWindow as MainWindow).ManyBox.SelectionChanged += ComboBoxItems_CurrentChanged;
        //    (Application.Current.MainWindow as MainWindow).ParentGroupBox.SelectionChanged += ComboBoxItems_CurrentChanged;

        //}
        #endregion

        public static void SetToDeletedState(RvView rv)
        {
            foreach (RvView item in rv.Requisites)
            {
                item.State = StateTypes.Deleted;
                if (item.Requisites.Any())
                {
                    SetToDeletedState(item);
                }
            }
            rv.State = StateTypes.Deleted;
        }

        public static void ReplaceOldGroup(string oldGroupName, string newGroupName)
        {
            if (SelectedRv != null && SelectedRv.Type == TypeOfRv.Group)
            {
                SelectedIO.AllGroups.Remove(oldGroupName);
                SelectedIO.AllGroups.Add(newGroupName);
                for (int i = (Application.Current.MainWindow as MainWindow).csAllPropGrid.Children.Count - 1; i >= 0; i--)
                {
                    UIElement el = (Application.Current.MainWindow as MainWindow).csAllPropGrid.Children[i];
                    if ((el as ComboBox).Name.Contains("ParentGroup"))
                    {
                        (el as ComboBox).Items.Refresh();
                        break;
                    }
                }

            }
        }

        public static void CreateNewIO()
        {
            IO io = new IO();
            io.Num = dbWorker.GetTableMAxSn("IOList", "NUM");// GetNewIONum();
            io.Code = GetNewCode();
            io.Title = GetNewTitle(io.Code);
            io.State = StateTypes.Added;
            io.TableName = $"IF_{io.Code}";
            io.AllGroups = new List<string>();
            io.AllGroups.Insert(0, "");
            IOViewsList.Add(new IOView(io));
            //IOSyncList.Add(new IOView(io));

            CreateButton(io.Code, io.Title, IOViewsList.Count - 1, true);
        }

        #region Work with IO & Rv
        public static void SaveChanges()
        {
            if (dbWorker.Connect())
            {
                using (SqliteTransaction transaction = dbWorker.newConn.BeginTransaction())
                {
                    try
                    {
                        foreach (IOView viewItem in IOViewsList)
                        {
                            switch (viewItem.State)
                            {
                                case StateTypes.Modify:
                                    CompareObj(viewItem, IOSyncList.First(x => x.Num == viewItem.Num));
                                    SaveRvChanges(viewItem.Requisites);
                                    break;
                                case StateTypes.Added:
                                    AddIO(viewItem);
                                    SaveRvChanges(viewItem.Requisites);
                                    break;
                                case StateTypes.Deleted:
                                    DeleteIO(viewItem);
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        dbWorker.Disconnect();
                        MessageBox.Show($"Error -> {e.Message}. Changes NOT SAVED!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    transaction.Commit();
                    MessageBox.Show($"Изменения сохранены успешно!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                    GetAllIOView();
                }
                dbWorker.Disconnect();
            }
        }

        private static void AddIO(IOView viewItem)
        {
            dbWorker.InsertSql(viewItem, true);
            dbWorker.CreateTable(tableName: viewItem.TableName, ioObj: true);
        }

        public static void DeleteIO(IOView viewItem)
        {
            dbWorker.DeleteSqlLogic(null, viewItem.Num.ToString(), true);
            dbWorker.DeleteSqlAllRv(viewItem.Num.ToString());
            dbWorker.DeleteSqlTable(null, viewItem.Code, true);
        }

        private static void SaveRvChanges(List<RvView> rvList)
        {
            foreach (RvView item in rvList)
            {
                switch (item.State)
                {
                    case StateTypes.Modify:
                        CompareObj(item, SelectedIO.UnsortedRequisites.First(x => x.Sn == item.Sn));
                        break;
                    case StateTypes.Added:
                        AddRv(item);
                        break;
                    case StateTypes.Deleted:
                        DeleteRv(item);
                        break;
                }

                if (item.State != StateTypes.Deleted)
                {
                    if (item.Requisites.Any())
                    {
                        SaveRvChanges(item.Requisites);
                    }

                }

            }
        }

        private static void DeleteRv(RvView rvView)
        {
            dbWorker.DeleteSqlLogic(rvView.Sn.ToString(), rvView.Num.ToString(), false);

            if (rvView.Type == TypeOfRv.Group)
            {
                foreach (RvView rv in rvView.Requisites)
                {
                    DeleteRv(rv);
                }
                dbWorker.DeleteSqlColumn(rvView.ParentTableName, rvView.TableColumnName);
                dbWorker.DeleteSqlTable(tableName: rvView.TableName);
            }
            else
            if (!rvView.Many)
            {
                dbWorker.DeleteSqlColumn(rvView.ParentTableName, rvView.TableColumnName);
            }
            else
            {
                dbWorker.DeleteSqlColumn(rvView.ParentTableName, rvView.TableColumnName);
                dbWorker.DeleteSqlTable(tableName: rvView.TableName);
            }
        }

        private static void CompareObj(object Modifyed, object Sync)
        {
            bool updateRv = Modifyed.GetType().Name == "RvView" ? true : false;

            foreach (PropertyInfo prop in Modifyed.GetType().GetProperties())
            {
                if (prop.Name == "Image" || prop.Name == "ParentIO" || prop.Name == "Requisites" || prop.Name == "Path" || prop.Name == "State") continue;
                //do not check ReferenceEquals//do not check Null object//do not check Type--we mind basic its different objects
                object rvMdata = prop.GetValue(Modifyed);
                object rvSdata = prop.GetValue(Sync);
                if (!rvMdata.Equals(rvSdata))
                {
                    if (updateRv)
                        dbWorker.UpdateSql((Modifyed as RvView).Sn.ToString(), prop.Name, rvMdata, prop.PropertyType.Name, updateRv);
                    else
                        dbWorker.UpdateSql((Modifyed as IOView).Num.ToString(), prop.Name, rvMdata, prop.PropertyType.Name, updateRv);
                }
            }

        }

        private static void AddRv(object rvAdded)
        {
            RvView rv = rvAdded as RvView;

            dbWorker.InsertSql(rv, false);

            if (rv.Type == TypeOfRv.Group) //group
            {
                dbWorker.CreateColumn(rv.TableColumnName, rv.ParentTableName, MyTypesConverter.ConvertToDB(rv.Type));
                dbWorker.CreateTable(rv.TableName, refTable: rv.ParentTableName);
            }
            else
            if (!rv.Many) //simple
            {
                dbWorker.CreateColumn(rv.TableColumnName, rv.ParentTableName, MyTypesConverter.ConvertToDB(rv.Type));
            }
            else
            if (rv.Many) //many
            {
                dbWorker.CreateColumn(rv.TableColumnName, rv.ParentTableName, MyTypesConverter.ConvertToDB(TypeOfRv.Integer));//counter in par tbl
                dbWorker.CreateTable(rv.TableName, colName: rv.TableColumnName, refTable: rv.ParentTableName, MyTypesConverter.ConvertToDB(rv.Type));
            }
        }

        #endregion





        #region Without BINDING work with TreeView

        public static void AddTreeItem() => CrTreeView.AddTreeItem();
        public static void DeleteTreeItem() => CrTreeView.DeleteTreeItem();
        public static void SyncDB() => SynchronizeDB();
        public static void TreeItem_KeyDown(object sender, KeyEventArgs e) => CrTreeView.TreeItem_KeyDown(sender, e);



        #endregion

        #region Random CODE
        const string allowedCharsLatin = "ABCDEFGHIKLMOPQRSTUVWXYZ";
        const string allowedCharsCyrillic = "АБВГДЕЖЗІКЛМНОПСТУФХЦЧШЩ";
        private static Random random = new Random();

        public static string RandomString(string allowedChars, int length)
        {
            return new string(Enumerable.Repeat(allowedChars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GetNewCode()
        {
            string code = string.Empty;
            while (string.IsNullOrEmpty(code))
            {
                string randomString = RandomString(allowedCharsLatin, 2);
                if (randomString != null)
                    code = randomString;
            }
            return code;
        }

        public static string GetNewTitle(string code)
        {
            return string.Concat(
                allowedCharsCyrillic[allowedCharsLatin.IndexOf(code[0])],
                allowedCharsCyrillic[allowedCharsLatin.IndexOf(code[1])]
                );
        }
        #endregion
    }
}