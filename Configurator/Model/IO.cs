using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Model
{
    class IO 
    {
        private int _num;
        private string _code;
        private string _title;
        private int _orderInList;
        private StateTypes _state;
        public List<string> AllGroups;
        public List<Rv> Requisites;
        private string _tableName;

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
            set {  _title = value; }
        }

        public int OrderInList
        {
            get { return _orderInList; }
            set { _orderInList = value; }
        }

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

        public IO()
        {
            
        }
        public IO(int num, string code, string title)
        {
            Num = num;
            Code = code;
            Title = title;
            Requisites = new List<Rv>();
            AllGroups = new List<string>();
        }
    }
}
