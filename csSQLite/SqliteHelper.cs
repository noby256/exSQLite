using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace csSQLite
{
    public class SqliteHelper
    {
        const string date = "'2014-05-29'"; // "'29.05.2014'";
        const string wherePart = " where date=" + date;
        string filename = @"D:\GDrive\Apps\reportingapp.db";
        const string sql = "select * from report_table" + wherePart + ";";

        public SqliteHelper()
        {
            filename = @"D:\GDrive\Apps\reportingapp.db";
        }

        public SqliteHelper(string sqlitefile)
        {
            filename = sqlitefile;
        }

        public void SqlConnect()
        {
            var conn = new SQLiteConnection("Data Source=" + filename + ";Version=3;");
            try
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    Console.WriteLine("Id: " + reader["id_report"] +
                        "\tOclock: " + reader["oclock"] +
                        "\tDate:" + reader["date"]);
                //DataSet ds = new DataSet();
                //var da = new SQLiteDataAdapter(sql, conn);
                //da.Fill(ds);
                //grid.DataSource = ds.Tables[0].DefaultView;
                Console.WriteLine("Sucess");
            }
            catch (Exception)
            {
                Console.WriteLine("Failed!");
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        //public List<>

    }
}
