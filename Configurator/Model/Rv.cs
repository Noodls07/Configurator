﻿using System;
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
        public Rv(string RvCode) 
        {
            Code = RvCode;
            Children = new inGroupRv().rvIn;
        }

        private int _num;
        private string _code;
        private string _title;
        private string _par;
        private int _isMany;
        private int _sn;
        private List<string> _children;


        public string Par
        {
            get { return _par; }
            set { _par = value; }
        }
        public int Sn
        {
            get { return _sn; }
            set { _sn = value; }
        }
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



        public int IsMany
        {
            get { return _isMany; }
            set { _isMany = value; }
        }



        public List<string> Children 
        {
            get { return _children; }
            set { _children = value; }
        }


    }
}