using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace csMeApp
{
    public class SqliteHelper
    {
        SQLiteConnection conn;

        public string Message="";

        string filename;    

        public SqliteHelper()
        {
            filename = @"D:\GDrive\Apps\reportingapp.db";
        }

        public SqliteHelper(string sqlitefile)
        {
            filename = sqlitefile;
        }

        public bool SqlConnect()
        {
            try
            {
                conn = new SQLiteConnection("Data Source=" + filename + ";Version=3;");
                Console.WriteLine("Sucсess");
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                Console.WriteLine(ex.Message);
                Console.WriteLine("Failed!");
                return false;
            }
            return true;
        }


        // Diary TABLE
        public Dictionary<int, Tuple<int, DateTime, string>> selectDiaryMessages()
        {
            Dictionary<int, Tuple<int, DateTime, string>> res = new Dictionary<int, Tuple<int, DateTime, string>>();
            string sqlcommand = "SELECT id_diary, id_person, datetime, message FROM diary_table";
            DataSet ds = new DataSet();
            if (Query(sqlcommand, ref ds))
            { 
                DataTable dt = ds.Tables[0];
                foreach (DataRow r in dt.Rows)
                {
                    res.Add(int.Parse(r["id_diary"].ToString()), new Tuple<int, DateTime, string>(
                        int.Parse(r["id_person"].ToString()),  DateTime.Parse(r["datetime"].ToString()), r["message"].ToString()));
                }
                return res;
            }
            else
            {
                return null;
            }
        }

        public bool insertDiaryMessage(string message, int id_person, DateTime dt)
        {
            string sqlcommand = String.Format("INSERT INTO diary_table (id_person, message, datetime) "
                        + "VALUES ({0}, '{1}', '{2}')", id_person, message, dt.ToString("yyyy-MM-dd HH':'mm':'ss"));
            return NonQuery(sqlcommand);
        }

        // Persons TABLE

        public Dictionary<int, string> selectPersonsNames()
        {
            Dictionary<int, string> activityTypes = new Dictionary<int, string>();
            string sqlcommand = "SELECT id_person, name FROM persons_table";
            DataSet ds = new DataSet();
            if (Query(sqlcommand, ref ds))
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow r in dt.Rows)
                {
                    activityTypes.Add(int.Parse(r["id_person"].ToString()), r["name"].ToString());
                }
                return activityTypes;
            }
            else
            {
                return null;
            }
        }

        
        public Dictionary<int, string> selectPersonsColors()
        {
            Dictionary<int, string> activityTypes = new Dictionary<int, string>();
            string sqlcommand = "SELECT id_person, colors FROM persons_table";
            DataSet ds = new DataSet();
            if (Query(sqlcommand, ref ds))
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow r in dt.Rows)
                {
                    activityTypes.Add(int.Parse(r["id_person"].ToString()), r["color"].ToString());
                }
                return activityTypes;
            }
            else
            {
                return null;
            }
        }

        // ActivityTypes TABLE
        public Dictionary<int, string> selectActivityTypes()
        {
            Dictionary<int, string> activityTypes = new Dictionary<int, string>();
            string sqlcommand = "SELECT id_activity_type, activity_name FROM activity_type_table";
            DataSet ds = new DataSet();
            if (Query(sqlcommand, ref ds))
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow r in dt.Rows)
                {
                    activityTypes.Add(int.Parse(r["id_activity_type"].ToString()), r["activity_name"].ToString());
                }
                return activityTypes;
            }
            else
            {
                return null;
            }
        }

        // REPORT TABLE
        public DataTable SelectReportDay(DateTime day)
        {
            string sqlcommand =
            String.Format("SELECT activity_type_table.activity_name AS activity, report_table.datetime AS datetime, strftime('%s', datetime) AS Expr1 "
                    + "FROM activity_type_table INNER JOIN "
                    + "report_table ON activity_type_table.id_activity_type = report_table.id_activity_type "
                    + "WHERE (Expr1 >= strftime('%s', '{0}')) and (Expr1 < strftime('%s', '{1}'))"
                /*+ "ORDER BY report_table.datetime"*/ , day.ToString("yyyy-MM-dd"), day.AddDays(1).ToString("yyyy-MM-dd"));
            DataSet ds = new DataSet();
            Query(sqlcommand, ref ds);
            return ds.Tables[0];
        }
        public bool insertReportHour(DateTime day, int id_activity_type)
        {
            string sqlcommand = String.Format("INSERT INTO report_table (datetime, id_activity_type) "
                        + "VALUES ('{0}', {1})", day.ToString("yyyy-MM-dd HH':'mm':'ss"), id_activity_type);
            return NonQuery(sqlcommand);
        }

        public bool deleteReportHour(DateTime day)
        {
            string sqlcommand = String.Format("DELETE FROM report_table "
                        + "WHERE (strftime('%s', datetime) = strftime('%s', '{0}')) ",
                        day.ToString("yyyy-MM-dd HH':'mm':'ss"));
            return NonQuery(sqlcommand);
        }

        // PRIVATE

        private bool Query(string sqlcommand, ref DataSet ds)
        {
            try
            {
                conn.Open();
                var da = new SQLiteDataAdapter(sqlcommand, conn);
                da.Fill(ds);
                conn.Close();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
            return true;
        }

        private bool NonQuery(string sqlcommand)
        {
            try
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand(sqlcommand, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
            return true;
        }
    }
}