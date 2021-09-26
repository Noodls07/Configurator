using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Configurator.Model
{
    static class DbConector
    {
        public static SqliteConnection newConn;
        public static void Connect()
        {
            string path = Environment.CurrentDirectory+ @"\newDB";
            newConn = new SqliteConnection($"Data Source={path};");
            newConn.Open();
        }
        public  static void Disconnect()
        {
            if (newConn.State==System.Data.ConnectionState.Open)
            {
                newConn.Close();
            }
        }



    }
}
