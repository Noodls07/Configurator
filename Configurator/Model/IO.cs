using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Model
{
    class IO 
    {
        private string _code;
        private string _title;
        private int _num;

        public List<Rv> Rekvs;

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

        public int Num
        {
            get { return _num; }
            set { _num = value; }
        }


        public IO()
        {
            
        }
        public IO(int num, string code, string title)
        {
            Code = code;
            Title = title;
            Num = num;
            Rekvs = new List<Rv>();

        }


    }
}
