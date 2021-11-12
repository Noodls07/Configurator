using Configurator.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Configurator.ViewModel
{
    class IOView : INotifyPropertyChanged
    {
        private int _num = -1;
        private string _code;
        private string _title;
        private int _orderInList= -1;
        private StateTypes _state;
        private string _tableName;

        public List<string> AllGroups;
        public List<RvView> Requisites;
        public List<RvView> UnsortedRequisites;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (!(State == StateTypes.Added) && !(State == StateTypes.Loading))
            {
                State = StateTypes.Modify;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        [Browsable(false)]
        public int Num
        {
            get { return _num; }
            set { _num = value; OnPropertyChanged(); }
        }

        [DisplayName("Код обьекта")]
        public string Code
        {
            get { return _code; }
            set
            {
                if (value.Length < 2 || value.Length > 3)
                {
                    MessageBox.Show($"Длинна не может быть меньше 2 и больше 3 символов!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                foreach (char ch in value.ToUpper())
                {
                    if ((ch < 65 || ch > 90) && (ch < 48 || ch > 57))
                    {
                        MessageBox.Show($" Символ - '{ch}' - не латинский!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                if (string.IsNullOrEmpty(_code))
                    _code = value;

                if (_code != value && !CodeExists(value))
                {
                    _code = value;
                    OnPropertyChanged();
                }
                    
            }
        }

        [DisplayName("Название обьекта")]
        public string Title
        {
            get { return _title; }
            set {  _title = value; OnPropertyChanged(); }
        }

        [Browsable(false)]
        public int OrderInList
        {
            get { return _orderInList; }
            set { _orderInList = value; }
        }

        [Browsable(false)]
        public StateTypes State
        {
            get { return _state; }
            set { _state = value; }
        }
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public IOView(IO model)
        {
            Num = model.Num;
            Code = model.Code;
            Title = model.Title;
            OrderInList = model.OrderInList;
            AllGroups = model.AllGroups;
            State = model.State;
            TableName = model.TableName;
            Requisites = new List<RvView>();
            UnsortedRequisites = new List<RvView>();
        }


        public void Clear()
        {
            Num = -1;
            Code = null;
            Title = null;
            Requisites = null;
            UnsortedRequisites = null;
            OrderInList = -1;
        }

        private bool CodeExists(string Code)
        {
            return Worker.IOSyncList.Any(x => x.Code == Code);
        }


        //public RvView this[string code]
        //{
        //    get
        //    {
        //        foreach (RvView r in Rekvs)
        //        {
        //            if (r.Code == code || r.Title == code)
        //            {
        //                return r;
        //            }
        //            if (r.Rekvs.Any())
        //            {
        //                RvView rv = r[code];
        //                if (rv != null) return rv;
        //            }
        //        }
        //        return null;
        //    }
        //}



        /// <summary>
        /// Sorting rekvizites by main and child
        /// </summary>
        /// <param name="io"></param>
        public void SortRv()
        {
            foreach (RvView rv in Requisites)
            {
                if (rv.ParentGroup != "")
                {
                    foreach (RvView rv1 in Requisites)
                    {
                        if (rv1.Code == rv.ParentGroup)
                        {
                            rv1.Requisites.Add(rv);
                            break;
                        }
                    }
                }
            }
            Requisites.RemoveAll(x => !string.IsNullOrEmpty(x.ParentGroup));
        }

        /// <summary>
        /// Moving Rv to New Parent
        /// </summary>
        /// <param name="rvToPush">Rv to move</param>
        public void MoveRvToGroup(RvView rvToPush, List<RvView> SelectedIORvList)
        {
            if (string.IsNullOrEmpty(rvToPush.ParentGroup))
            {
                Requisites.Add(rvToPush);
                return;
            }

            foreach (RvView rv in SelectedIORvList)
            {
                if (rv.Code == rvToPush.ParentGroup)
                {
                    rv.Requisites.Add(rvToPush);
                    break;
                }
                else
                if (rv.Requisites.Any())
                {
                    MoveRvToGroup(rvToPush, rv.Requisites);
                }
            }
            
        }
        public void RemoveRv(RvView rvToRemove, List<RvView> SelectedIORvList)
        {
            if (string.IsNullOrEmpty(rvToRemove.ParentGroup))
            {
                Requisites.Remove(rvToRemove);
                return;
            }
            foreach (RvView rv in SelectedIORvList)
            {
                if (rv.Code == rvToRemove.ParentGroup)
                {
                    rv.Requisites.Remove(rvToRemove);
                    break;
                }
                else
                if (rv.Requisites.Any())
                {
                    RemoveRv(rvToRemove, rv.Requisites);
                    
                }
            }
        }
        private string ClearHeaderRv(string RvTitle)
        {
            return RvTitle.Remove(0, RvTitle.IndexOf(']') + 1).Trim();
        }
        public RvView FindCurrentRv(string RvTitle, List<RvView> rvList = null )
        {
            RvView findedRv = null;
            RvTitle = ClearHeaderRv(RvTitle);
            if (rvList == null) rvList = Requisites;
            foreach (RvView rv in rvList)
            {
                if (RvTitle == rv.Title) return findedRv = rv;

                if (rv.Requisites.Any())
                {
                    findedRv = FindCurrentRv( RvTitle, rv.Requisites);
                    if (findedRv != null) return findedRv;
                }
            }
            return findedRv;
        }
        public void SetPath(List<RvView> rvList, string path)
        {
            foreach (RvView rv in rvList)
            {
                if (rv.ParentGroup == "") rv.Path += path;
                if (rv.ParentGroup != "") rv.Path += path + " . " + rv.ParentGroup;
                if (rv.Requisites.Any()) SetPath(rv.Requisites, rv.Path);
            }
        }

    }
}
