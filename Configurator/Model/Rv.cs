using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Model
{
    class Rv 
    {
        public Rv(){}

        #region private
        private int _num;
        private string _code;
        private string _title;
        private string _parentGroup;
        private string _path;
        private bool _many;
        private int _sn;
        private string _parentIOCode;
        private TypeOfRv _type;
        private StateTypes _state;
        private string _tableColumnName;
        private string _tableName;
        private string _parentTableName;
        private IO _parentIO;
        private List<Rv> _requisites;
        #endregion

        public int Num
        {
            get { return _num; }
            set { _num = value; }
        }
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public string ParentGroup
        {
            get { return _parentGroup; }
            set { _parentGroup = value; }
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public bool Many
        {
            get { return _many; }
            set { _many = value; }
        }
        public int Sn
        {
            get { return _sn; }
            set { _sn = value; }
        }
        public TypeOfRv Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public StateTypes State
        {
            get { return _state; }
            set { _state = value; }
        }
        public string ParentIOCode
        {
            get { return _parentIOCode; }
            set { _parentIOCode = value; }
        }
        public string TableColumnName
        {
            get { return _tableColumnName; }
            set { _tableColumnName = value; }
        }
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }
        public string ParentTableName
        {
            get { return _parentTableName; }
            set { _parentTableName = value; }
        }
        public IO ParentIO
        {
            get { return _parentIO; }
            set { _parentIO = value; }
        }
        public List<Rv> Requisites
        {
            get { return _requisites; }
            set { _requisites = value; }
        }
    }
}
