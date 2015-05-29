using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes.Utilization;
using System.Data.OleDb;
using System.Data;

namespace RBClient.Classes
{
    class SqlWorker
    {
        internal static bool ExecuteQuerySafe(string query)
        {
            if (query == "") return false;

            CBData _data = new CBData();
            OleDbConnection _conn = null;
            string _str_command = "";

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                OleDbCommand _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = query;
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

                return true;
            }
            catch (Exception _exp)
            {
                MDIParentMain.log.Error("Не получилось выполнить запрос "+ _str_command+" Error: "+
_exp.Message, _exp);
                return false;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            };
        }

        internal static int InsertQuerySafe(string query)
        {
            if (query == "") return 0;

            CBData _data = new CBData();
            OleDbConnection _conn = null;
            string _str_command = "";

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                OleDbCommand _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = query;
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

                _str_command = "SELECT @@IDENTITY";
                _command.CommandText = _str_command;
                int _doc_id = Convert.ToInt32(_command.ExecuteScalar());

                return _doc_id;
            }
            catch (Exception _exp)
            {
                MDIParentMain.log.Error("Не получилось выполнить запрос " + _str_command + " Error: " +
_exp.Message, _exp);
                return 0;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            };
        }

        internal static int InsertQuerySafe(OleDbCommand query)
        {
            if (query == null) return 0;

            CBData _data = new CBData();
            OleDbConnection _conn = null;
            string _str_command = "";

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                OleDbCommand _command = query;
                _command.Connection = _conn;
                _command.ExecuteNonQuery();

                _str_command = "SELECT @@IDENTITY";
                _command.CommandText = _str_command;
                int _doc_id = Convert.ToInt32(_command.ExecuteScalar());

                return _doc_id;
            }
            catch (Exception _exp)
            {
                MDIParentMain.log.Error("Не получилось выполнить запрос " + _str_command + " Error: " +
_exp.Message, _exp);
                return 0;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            };
        }

        internal static bool ExecuteQuerySafe(OleDbCommand query)
        {
            if (query == null) return false;

            CBData _data = new CBData();
            OleDbConnection _conn = null;
            string _str_command = "";

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                OleDbCommand _command = query;
                _command.Connection = _conn;
                _command.ExecuteNonQuery();

                return true;
            }
            catch (Exception _exp)
            {
                MDIParentMain.log.Error("Не получилось выполнить запрос " + _str_command + " Error: " +
_exp.Message, _exp);
                return false;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            };
        }

        internal static bool ExecuteQuery(string query)
        {
            if (query == "") return false;

            CBData _data = new CBData();
            OleDbConnection _conn = null;
            string _str_command = "";

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                OleDbCommand _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = query;
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
             
                return true;
            }
            catch (Exception _exp)
            {
                throw new Exception("Не получилось записать в базу " + _str_command, _exp);
                return false;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            };
        }

        [Obsolete("использовать InsertQuerySafe вместо")]
        internal static bool InsertIntoDB<T>(List<T> list, bool clearTableBeforeWrite) where T : IQueriable
        {
            if (list == null || list.Count == 0) return false;
            CBData _data = new CBData();
            OleDbConnection _conn = null;
            string _str_command = "";

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                if (clearTableBeforeWrite)
                {
                    OleDbCommand _command = new OleDbCommand();
                    _command.Connection = _conn;
                    _str_command=list[0].MakeDeleteAllQuery();
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }

                foreach(IQueriable iq in list)
                {
                    OleDbCommand _command = new OleDbCommand();
                    _command.Connection = _conn;
                    _str_command = iq.MakeInsertQuery();
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception _exp)
            {
                throw new Exception("Не получилось записать в базу " + _str_command, _exp);
                return false;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            };
        }

        internal static bool InsertOrUpdateDB<T>(List<T> list) where T : IQueriable
        {
            if (list == null || list.Count == 0) return false;
            CBData _data = new CBData();
            OleDbConnection _conn = null;
            string _str_command = "";

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                foreach (IQueriable iq in list)
                {
                    OleDbCommand _command = new OleDbCommand();
                    _command.Connection = _conn;
                    _str_command = iq.MakeDeleteQuery();
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();


                    OleDbCommand _command1 = new OleDbCommand();
                    _command1.Connection = _conn;
                    _str_command = iq.MakeInsertQuery();
                    _command1.CommandText = _str_command;
                    _command1.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception _exp)
            {
                throw new Exception("Не получилось записать в базу " + _str_command, _exp);
                return false;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            };
        }

        internal static DataTable SelectFromDB(string SqlQuery,string tabName)
        {
            CBData _data = new CBData();
            OleDbConnection _conn = null;
            DataTable _table=null;
            string _str_command = "";

            if (SqlQuery=="") return null;
            _str_command = SqlQuery;
            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable(tabName);
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw new Exception("Не получилось получить из в базы " + _str_command, _exp);
                return null;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            };
        }

        internal static DataTable GetDocumentById(string docId)
        {
            return SelectFromDBSafe("SELECT t_Doc.* FROM t_Doc WHERE (((t_Doc.doc_id)=" + docId + "));", "t_Doc");
        }

        internal static DataTable SelectFromDBSafe(string SqlQuery, string tabName)
        {
            CBData _data = new CBData();
            OleDbConnection _conn = null;
            DataTable _table = null;
            string _str_command = "";

            if (SqlQuery == "") return null;
            _str_command = SqlQuery;
            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable(tabName);
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                MDIParentMain.log.Error("Не получилось получить из в базы " + _str_command, _exp);
                return null;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            };
        }

        /// <summary>
        /// возвращает значение для записи в базу данных
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static string ReturnDBValue(object s)
        {
            string value = "";
            if (s==null) return "''";
            if (s is int || s is double || s is decimal)
            {
                value = s.ToString();
                if (s is decimal)
                {
                    value = value.Replace(',', '.');
                }
            }
            else
                if (s is bool)
                {
                    if ((bool)s) value = "-1";
                    else value = "0";
                }
                else
                {
                    if (s is byte[])
                    {
                        var arr=(byte[])s;
                        value = "'"+System.Convert.ToBase64String(arr, 0, arr.Length)+"'";
                    }
                    else
                    {
                        value = "'" + s.ToString() + "'";
                    }
                }
            return value;
        }


        /// <summary>
        /// Возвращает дату в нужном для access формате
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal static string ReturnDate(DateTime date)
        {
            string str_date = String.Format("#{1:00}/{0:00}/{2:00}#", date.Day, date.Month, date.Year);
            return str_date;
        }

        /// <summary>
        /// Возвращает дату и время в нужном для access формате
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal static string ReturnDateTime(DateTime date)
        {
            string str_date = String.Format("#{1:00}/{0:00}/{2:00} {5:00}:{6:00}:{7:00}#", date.Day, date.Month, date.Year,date.Hour,date.Minute,date.Second);
            return str_date;
        }


        internal static void UpdateSpravTable(Type type, List<InternalClasses.Models.t_Menu> t_men_list)
        {
            throw new NotImplementedException();
        }
    }

    class QueryHelper
    {
        internal Dictionary<string, string> fields;
        //public string MakeInsertQuery<T>(string tablename, T tableClass)
        //    where T : class
        //{
        //    throw new Exception("Доделать QueryHelper.MakeInsertQuery<T> если понадобится");

        //    //string query = "INSERT INTO " + tablename + " (";

        //    //fields.Keys.ToList<string>().ForEach(s =>
        //    //{
        //    //    query += s + ",";
        //    //});

        //    //query = query.Substring(0, query.Length - 1) + ") VALUES(";

        //    //fields.Values.ToList<string>().ForEach(s =>
        //    //{
        //    //    query += "'" + s + "',";
        //    //});
        //    //query = query.Substring(0, query.Length - 1) + ")";
        //    //return query;
        //}

        public static string MakeInsertQuery<T>(string tablename, T tableClass)
            where T : IEnumerable<Dictionary<string, string>>
        {
            if (tableClass == null || tableClass.Count() == 0)
            {
                return "";
            }

            string query = "INSERT INTO " + tablename + " (";

            List<Dictionary<string, string>> fields_list = tableClass.ToList<Dictionary<string, string>>();


            fields_list[0].Keys.ToList<string>().ForEach(s =>
            {
                query += s + ",";
            });

            query = query.Substring(0, query.Length - 1) + ") VALUES";

            //перебрать поля, добавить в запрос
            foreach (Dictionary<string, string> line in fields_list)
            {
                query += "(";
                line.Values.ToList<string>().ForEach(s =>
                {
                    query += "'" + s + "',";
                });
                query += "),";
            }
            
            query = query.Substring(0, query.Length - 1);
            return query;
        }

        public string MakeDeleteAllQuery(string tablename)
        {
            string query = "DELETE FROM " + tablename;
            return query;
        }
    }

}
