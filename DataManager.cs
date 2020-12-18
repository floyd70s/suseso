using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SQLite;
//using Microsoft.Data.Sqlite;

namespace suseso
{
   public class DataManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string connectionString;
        /// <summary>
        /// constructor for DataManager Class
        /// </summary>
        /// <param name="sConnectionString">Conection strign from app.config</param>
        public DataManager(string sConnectionString)
        {
            this.connectionString = sConnectionString;
        }

        /// <summary>
        /// generic function for execute SQLite command with SQLite BDDriver.
        /// non return data
        /// </summary>
        /// <param name="query">query for execution insert or update</param>
        public string setData(string query)
        {
            DataTable dt = new DataTable();
            SQLiteConnection connection;
            SQLiteCommand command;
            connection = new SQLiteConnection(this.connectionString);

            try
            {
                connection.Open();
                command = new SQLiteCommand(query, connection);
                dt.Load(command.ExecuteReader());
                connection.Close();
                return "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                return "error";
            }
        }

        /// <summary>
        /// generic function for execute SQL command with SQL Server BDDriver.
        /// </summary>
        /// <param name="query">query for execution insert or update</param>
        public string setDataSQL(string query)
        {
            DataTable dt = new DataTable();
            SqlConnection connection;
            SqlCommand command;

            connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                command = new SqlCommand(query, connection);
                dt.Load(command.ExecuteReader());
                connection.Close();
                return "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                return "error";
            }
        }


        /// <summary>
        /// generic function for execute SQL command with SQLite BDDriver.
        /// </summary>
        /// <param name="query">query to execute</param>
        /// <param name="connectionString"></param>
        /// <returns>return DataTable with result</returns>
        public DataTable getData(string query)
        {
            DataTable dt = new DataTable();
            SQLiteConnection connection;
            SQLiteCommand command;
            connection = new SQLiteConnection(connectionString);

            try
            {
                connection.Open();
                command = new SQLiteCommand(query, connection);
                dt.Load(command.ExecuteReader());
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
            }
            return dt;
        }


        public DataTable getDataTemp(string query)
        {
            DataTable dt = new DataTable();
            SQLiteConnection connection;
            SQLiteCommand command;
            connection = new SQLiteConnection(connectionString);
            DataColumn column;
            DataRow row;
              
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "aid";
            dt.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "title";
            dt.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "abstract";
            dt.Columns.Add(column);


            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "name";
            dt.Columns.Add(column);


            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "insertDate";
            dt.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "status";
            dt.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "rol";
            dt.Columns.Add(column);


            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "sentenceDate";
            dt.Columns.Add(column);


            try
            {
                connection.Open();
                command = new SQLiteCommand(query, connection);
                // dt.Load(command.ExecuteReader());
                // connection.Close();

                SQLiteDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    row = dt.NewRow();

                    //select aid, title,abstract,name,insertDate,status,rol,sentenceDate from SUSESO where status=0

                    row["aid"] = reader.GetInt32(0);
                    row["title"] = reader.GetString(1);
                    row["abstract"] = reader.GetString(2);
                    row["name"] = reader.GetString(3);
                    row["insertDate"] = reader.GetString(4);
                    row["status"] = reader.GetInt32(5);
                    row["rol"] = reader.GetInt32(6);
                    row["sentenceDate"] = reader.GetString(7);

                    dt.Rows.Add(row);
                }
                reader.Close();
                connection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
            }
            return dt;
        }

        /// <summary>
        /// generic function for execute SQL command with SQL SERVER BDDriver.
        /// </summary>
        /// <param name="query">query to execute</param>
        /// <param name="connectionString"></param>
        /// <returns>return DataTable with result</returns>
        public DataTable getDataSQL(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection connection =  new SqlConnection(connectionString);
                SqlCommand command;
            
                connection.Open();
                command = new SqlCommand(query, connection);
                dt.Load(command.ExecuteReader());
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
                Console.WriteLine("........Fail");
                log.Error("[Fatal Error]\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.InnerException + "\r\n" + ex.Source);
            }
            return dt;
        }
    }
}
