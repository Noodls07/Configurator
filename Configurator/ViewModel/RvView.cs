using Configurator.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Configurator.ViewModel
{
    class RvView : INotifyPropertyChanged
    {
        private int _num;
        private string _code;
        private string _title;
        private string _parentGroup;
        private string _path;
        private bool _many;
        private int _sn;
        private TypeOfRv _type;
        private StateTypes _state;
        private string _parentIOCode;
        private string _tableColumnName;
        private string _tableName;
        private string _parentTableName;
        private IOView _parentIO;
        private List<RvView> _requisites;
        private object _image;
        //private bool _selected = false;

        public event PropertyChangedEventHandler PropertyChanged;

        [Browsable(false)]
        public int Num
        {
            get { return _num; }
            set { _num = value; }
        }

        [DisplayName("Код реквизита")]
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

                if (string.IsNullOrEmpty(_code)) _code = value;
                else
                if (_code != value)
                {
                    _code = value;

                    if (CodeExists(value))
                    {
                        MessageBox.Show($" Указаный код существует!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    OnPropertyChanged();
                }
            }
        }

        [DisplayName("Название реквизита")]
        public string Title
        {
            get { return _title; }
            set
            {
                if (string.IsNullOrEmpty(_title))
                    _title = value;
                else
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }

            }
        }

        [DisplayName("Входит в группу")]
        public string ParentGroup
        {
            get { return _parentGroup; }
            set
            {
                if (string.IsNullOrEmpty(_parentGroup))
                    _parentGroup = value;

                if (_parentGroup != value)
                {
                    _parentGroup = value;
                    OnPropertyChanged();
                }

            }
        }

        [Browsable(false)]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [DisplayName("Множественность")]
        public bool Many
        {
            get { return _many; }
            set
            {
                _many = value;
                OnPropertyChanged();
            }
        }

        [Browsable(false)]
        public int Sn
        {
            get { return _sn; }
            set { _sn = value; }
        }

        [DisplayName("Тип реквизита")]
        public TypeOfRv Type
        {
            get { return _type; }
            set
            {
                if (_type == 0)
                    _type = value;
                else
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                }

            }
        }

        [Browsable(false)]
        public StateTypes State
        {
            get { return _state; }
            set { _state = value; }
        }

        [Browsable(false)]
        public string ParentIOCode
        {
            get { return _parentIOCode; }
            set { _parentIOCode = value; }
        }

        [Browsable(false)]
        public string TableColumnName
        {
            get { return _tableColumnName; }
            set { _tableColumnName = value; }
        }

        [Browsable(false)]
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        [Browsable(false)]
        public string ParentTableName
        {
            get { return _parentTableName; }
            set { _parentTableName = value; }
        }

        [Browsable(false)]
        public List<RvView> Requisites
        {
            get { return _requisites; }
            set { _requisites = value; }
        }

        [Browsable(false)]
        public IOView ParentIO
        {
            get { return _parentIO; }
            set { _parentIO = value; }
        }

        [Browsable(false)]
        public object Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    //OnPropertyChanged();
                }

            }
        }
        //public bool Selected
        //{
        //    get { return _selected; }
        //    set
        //    {
        //        _selected = value;
        //        //OnPropertyChanged();
        //    }
        //}


        public RvView() { }
        public RvView(Rv model)
        {
            Sn = model.Sn;
            Num = model.Num;
            Code = model.Code;
            Title = model.Title;
            Many = model.Many;
            ParentGroup = model.ParentGroup;
            Path = model.Path;
            Type = model.Type;
            Requisites = new List<RvView>();
            Image = SetImage(model.Type);
            State = model.State;
            ParentIOCode = model.ParentIOCode;
            TableColumnName = model.TableColumnName;
            SetRvTablesName(ParentIOCode, this);
            //TableName = model.TableName;
            //ParentTableName = model.ParentTableName;
        }

        public void SetRvTablesName(string ioCode, RvView rv)
        {
            if (rv.Type == TypeOfRv.Group || rv.Many)
            {
                rv.TableName = $"IF_{ioCode}_{rv.Code}";
            }
            else
            {
                rv.TableName = !(rv.Type == TypeOfRv.Group) && !rv.Many && !string.IsNullOrEmpty(rv.ParentGroup)
                    ? $"IF_{ioCode}_{rv.ParentGroup}"
                    : $"IF_{ioCode}";
            }

            rv.ParentTableName = !string.IsNullOrEmpty(rv.ParentGroup) ? $"IF_{ioCode}_{rv.ParentGroup}" : $"IF_{ioCode}";
        }


        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (!(State == StateTypes.Added) && !(State == StateTypes.Deleted))
            {
                State = StateTypes.Modify;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public object SetImage(TypeOfRv type)
        {
            return new HeaderToImageConverter().Convert(type, typeof(BitmapImage), null, System.Globalization.CultureInfo.CurrentCulture);
        }

        private bool CodeExists(string Code)
        {
            return Worker.SelectedIO.UnsortedRequisites.Any(x => x.Code == Code);
        }




        //public RvView this[string code]
        //{
        //    get
        //    {
        //        if (code == Code || Title == code) return this;
        //        if (Children != null)
        //        {
        //            foreach (RvView r in Children)
        //            {
        //                if (r.Code == code || r.Title == code)
        //                {
        //                    return r;
        //                }
        //                if (r.Children.Any())
        //                {
        //                    RvView rv = r[code];
        //                    if (rv != null) return rv;
        //                }
        //            }
        //        }
        //        return null;
        //    }
        //}
    }

}

