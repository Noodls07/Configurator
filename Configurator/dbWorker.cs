using Configurator.ViewModel;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Configurator
{
    static class dbWorker
    {
        public static SqliteConnection newConn = null;
        public static bool Connect()
        {
            try
            {
                if (newConn is null)
                {
                    string path = Environment.CurrentDirectory + @"\newDB";
                    newConn = new SqliteConnection($"Data Source={path};");
                }

                newConn.Open();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error -> {e.Message}. DataBase is NOT founded!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
                throw;
            }
        }

        public static void Disconnect()
        {
            if (newConn.State == System.Data.ConnectionState.Open)
            {
                newConn.Close();
                newConn.Dispose();
            }
        }
        public static void DeleteSqlLogic(string sn , string ioNum, bool io = false)
        {
            SqliteCommand sqlite_cmd = newConn.CreateCommand();

            if (io)
                sqlite_cmd.CommandText = $"DELETE FROM IOList WHERE NUM = {ioNum}";
            else
                sqlite_cmd.CommandText = $"DELETE FROM IOLogic WHERE SN = {sn} AND NUM = {ioNum}";
            
            sqlite_cmd.ExecuteNonQuery();
        }

        public static void DeleteSqlTable(string tableName, string ioCode = null, bool io = false)
        {
            SqliteCommand sqlite_cmd = newConn.CreateCommand();

            if (io)
            {
                foreach (string tbl in GetIOTables(ioCode))
                {
                    sqlite_cmd.CommandText = $"DROP TABLE {tbl}";
                    sqlite_cmd.ExecuteNonQuery();
                }
            }
            else
            {
                sqlite_cmd.CommandText = $"DROP TABLE {tableName}";
                sqlite_cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteSqlAllRv(string ioNum)
        {
            SqliteCommand sqlite_cmd = newConn.CreateCommand();
            sqlite_cmd.CommandText = $"DELETE FROM IOLogic WHERE NUM =  {ioNum}";
            sqlite_cmd.ExecuteNonQuery();
        }


        public static void DeleteSqlColumn(string tableName, string colName)
        {
            string sql = "";
            SqliteCommand sqlite_cmd = newConn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT SQL FROM SQLITE_MASTER WHERE TYPE = 'table' AND NAME = '{tableName}'";//   ALTER TABLE {tableName} DROP {colName}";
            SqliteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                sql = sqlite_datareader.GetString(0);
            }
            sqlite_datareader.Close();

            //find & delete col
            string[] columns = sql.Split(',').ToArray();
            columns = columns.Where(co => co.IndexOf(colName) < 0).ToArray();

            sql = columns.Last().Contains("))") ? string.Join(",", columns) : string.Join(",", columns) + ')';

            sql = "PRAGMA foreign_keys = 0; " +
                  //"BEGIN TRANSACTION; " +
                  $"DROP TABLE { tableName}; " +
                  sql +"; " +
                  //"; COMMIT; "+
                  " PRAGMA foreign_keys = 1;";
            sqlite_cmd.CommandText = sql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public static int GetNewIONum()
        {
            Connect();
            int result = -1;
            SqliteCommand sqlite_cmd = newConn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT MAX(NUM) FROM IOList ";
            SqliteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                result = Convert.ToInt32(sqlite_datareader[0].ToString());
            }
            sqlite_datareader.Close();
            Disconnect();
            return ++result;
        }

        public static void UpdateSql(string sn, string propName, object data, string propType, bool rv)
        {
            data = GetDataByType(propType, data);

            SqliteCommand sqlite_cmd = newConn.CreateCommand();
            sqlite_cmd.CommandText = rv ? $"UPDATE IOLogic SET {propName} = {data} WHERE SN = {sn}" : $"UPDATE IOList SET {propName} = {data} WHERE NUM = {sn}";
            sqlite_cmd.ExecuteNonQuery();

            //if rename code we need create tbl with new col name, drop old table/ TODO


            //if propName=PAR если изминили на другую группу то удалить реквизит из одной табл и создать в новой/ TODO
        }

        public static void InsertSql(object insertingObject, bool IOobj)
        {
            SqliteCommand sqlite_cmd = newConn.CreateCommand();

            if (IOobj)
            {
                IOView io = (IOView)insertingObject;

                sqlite_cmd.CommandText = $"INSERT INTO IOList (CODE, TITLE) VALUES (@CODE, @TITLE)";
                //sqlite_cmd.Parameters.AddWithValue("@NUM", io.Num);
                sqlite_cmd.Parameters.AddWithValue("@CODE", io.Code);
                sqlite_cmd.Parameters.AddWithValue("@TITLE", io.Title);
                sqlite_cmd.ExecuteNonQuery();
                return;
            }

            RvView rv = (RvView)insertingObject;

            sqlite_cmd.CommandText = $"INSERT INTO IOLogic ( NUM, CODE, TITLE, MANY, PAR, TYPE) VALUES ( @NUM, @CODE, @TITLE, @MANY, @PAR, @TYPE )";//{GetTableMAxSn("IOLogic","SN")}
            sqlite_cmd.Parameters.AddWithValue("@NUM", rv.Num);
            sqlite_cmd.Parameters.AddWithValue("@CODE", rv.Code);
            sqlite_cmd.Parameters.AddWithValue("@TITLE", rv.Title);
            sqlite_cmd.Parameters.AddWithValue("@MANY", Convert.ToInt32(rv.Many));
            sqlite_cmd.Parameters.AddWithValue("@PAR", rv.ParentGroup);
            sqlite_cmd.Parameters.AddWithValue("@TYPE", (int)rv.Type);
            sqlite_cmd.ExecuteNonQuery();
        }


        public static int GetTableMAxSn(string tableName, string snColName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                Connect();
                SqliteCommand sqlite_cmd = newConn?.CreateCommand();
                sqlite_cmd.CommandText = $"SELECT MAX({snColName}) FROM {tableName}";
                int maxSn = Convert.ToInt32(sqlite_cmd.ExecuteScalar());
                maxSn++;
                Disconnect();
                return maxSn;
            }
            return -1;
        }



        public static void CreateColumn(string colName, string tableName, Tuple<string, int> colDescription)
        {
            SqliteCommand sqlite_cmd = newConn.CreateCommand();
            sqlite_cmd.CommandText = $"ALTER TABLE {tableName} ADD '{colName}' {colDescription.Item1} ({colDescription.Item2}) ";
            sqlite_cmd.ExecuteNonQuery();
        }

        public static void CreateTable(string tableName, string colName = null, string refTable = null, Tuple<string, int> colDescription = null, bool ioObj = false)
        {
            SqliteCommand sqlite_cmd = newConn.CreateCommand();

            if (ioObj)
                sqlite_cmd.CommandText = $"CREATE TABLE {tableName} ( SN INTEGER PRIMARY KEY AUTOINCREMENT REFERENCES IOList (NUM) ON DELETE NO ACTION UNIQUE NOT NULL) ";
            else
            {
                sqlite_cmd.CommandText = !string.IsNullOrEmpty(colName)
                    ? $"CREATE TABLE {tableName} ( SN INTEGER PRIMARY KEY AUTOINCREMENT REFERENCES {refTable} (SN) ON DELETE NO ACTION UNIQUE NOT NULL, " +
                                                                        $"SN_PAR INTEGER NOT NULL, " +
                                                                        $"SN_IO INTEGER NOT NULL, '{colName}' {colDescription.Item1}  ({colDescription.Item2}) )"
                    : $"CREATE TABLE {tableName} ( SN INTEGER PRIMARY KEY AUTOINCREMENT REFERENCES {refTable} (SN) ON DELETE NO ACTION UNIQUE NOT NULL, " +
                                                                        $"SN_PAR INTEGER NOT NULL, " +
                                                                        $"SN_IO INTEGER NOT NULL)";
            }
            sqlite_cmd.ExecuteNonQuery();
        }

        public static bool ColumnOrTableExist(string tableName, string colName = null, bool column = false)
        {
            SqliteCommand sqlite_cmd = newConn.CreateCommand();

            if (column)
                sqlite_cmd.CommandText = $"SELECT COUNT(*) AS CNTREC FROM PRAGMA_TABLE_INFO('{tableName}') WHERE NAME='{colName}'";
            else
                sqlite_cmd.CommandText = $"SELECT Count(NAME) FROM SQLITE_MASTER WHERE TYPE='table' AND NAME = '{tableName}'";

            return Convert.ToInt32(sqlite_cmd.ExecuteScalar()) > 0;
        }

        public static string[] GetIOTables(string ioCode)
        {
            string[] result = { };
            SqliteCommand sqlite_cmd = newConn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT tbl_name FROM SQLITE_MASTER WHERE TYPE = 'table' AND NAME LIKE 'IF_{ioCode}%'";
            SqliteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                Array.Resize(ref result, result.Length + 1);
                result.SetValue(sqlite_datareader[0], result.Length-1);
            }
            sqlite_datareader.Close();
            return result;
        }




        public static string GetDataByType(string propType, object data)
        {
            switch (propType)
            {
                case "String":
                    return $"'{data}'";
                case "Int32":
                    return $"{data}";
            }
            return data.ToString();
        }


    }
}
