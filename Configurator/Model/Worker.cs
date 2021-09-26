﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Model
{
    static class Worker
    {

        public static List<IO> GetAllIO()
        {

            List<IO> all = new List<IO>();
            SqliteCommand sqlite_cmd = DbConector.newConn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM IOList order by NUM";

            SqliteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                IO io = new IO();
                
                io.Num = Convert.ToInt32(sqlite_datareader.GetString(0));
                io.Code = sqlite_datareader.GetString(1);
                io.Title = sqlite_datareader.GetString(2);

               io.Rekvs = GetAllRvForIO(io);

                all.Add(io);
            }
            return all;
        }

        public static List<Rv> GetAllRvForIO(IO io)
        {

            List<Rv> all = new List<Rv>();
            SqliteCommand sqlite_cmd = DbConector.newConn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT * FROM IOLogic where num={io.Num} order by SN ";

            SqliteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                Rv rv = new Rv();
                rv.Sn = Convert.ToInt32(sqlite_datareader.GetString(0));
                rv.Num = Convert.ToInt32(sqlite_datareader.GetString(1));
                rv.Code = sqlite_datareader.GetString(2);
                rv.Title = sqlite_datareader.GetString(3);
                rv.IsMany = Convert.ToInt32(sqlite_datareader.GetString(4));
                rv.Par = !sqlite_datareader.IsDBNull(5) ? sqlite_datareader.GetString(5) : "";


                all.Add(rv);
            }
            return all;
        }

    }
}