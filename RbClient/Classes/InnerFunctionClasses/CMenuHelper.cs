using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace RBClient
{
    /// <summary>
    /// Работа с меню
    /// </summary>
    class CMenuHelper
    {
        public void MenuLoad(int doc_id, string menu_1c, string menu_name,  string menu_date)
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            int _menu_id;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                // проверим, есть ли уже в базе такая позиция
                _str_command = "SELECT top 1 menu_id FROM t_Menu WHERE menu_nome_1C = @menu_nome_1C";
                _command.Parameters.AddWithValue("@menu_nome_1C", menu_1c);
                _command.Parameters.AddWithValue("@menu_nome", menu_name);
                _command.CommandText = _str_command;
                _menu_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_menu_id == 0)
                {
                    // вставить новую строчку
                    _str_command = "INSERT INTO t_Menu (menu_nome_1C, menu_nome) VALUES(@menu_nome_1C, @menu_nome)";
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();

                    // id документа
                    _str_command = "SELECT @@IDENTITY";
                    _command.CommandText = _str_command;
                    _menu_id = Convert.ToInt32(_command.ExecuteScalar());
                }
                else
                {
                    // обновить название
                }
                // удалить прайс
                _str_command = "DELETE FROM t_MenuPrice WHERE mp_menu_id = @mp_menu_id AND mp_date = @mp_date";
                _command.CommandText = _str_command;
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@mp_menu_id", _menu_id);
                _command.Parameters.AddWithValue("@mp_date", menu_date);
                _command.ExecuteNonQuery();

                // вставить новый прайс
                _str_command = "INSERT INTO t_MenuPrice (mp_doc_id, mp_menu_id, mp_date) VALUES(@mp_doc_id, @mp_menu_id, @mp_date)";
                _command.CommandText = _str_command;
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@mp_doc_id", doc_id);
                _command.Parameters.AddWithValue("@mp_menu_id", _menu_id);
                _command.Parameters.AddWithValue("@mp_date", menu_date);
                _command.ExecuteNonQuery();

            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public int MenuDoc(string teremok_id, string menu_date, string menu_dep)
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            int _doc_id;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // проверим, есть ли уже в базе такая позиция
                _str_command = "SELECT top 1 doc_id FROM t_Doc WHERE doc_teremok_id = @doc_teremok_id AND doc_datetime= @doc_datetime AND doc_type_id=7 AND doc_menu_dep =" + menu_dep;
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_teremok_id", teremok_id);
                _command.Parameters.AddWithValue("@doc_datetime", menu_date);
                _command.Parameters.AddWithValue("@doc_menu_dep", menu_dep);

                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    // вставить новую строчку
                    if (menu_dep == "TR")
                    {
                        _str_command = "INSERT INTO t_Doc (doc_type_id, doc_status_id, doc_desc, doc_teremok_id, doc_datetime, doc_menu_dep) VALUES(7, 11, 'получено новое меню Рестораны! ТРЕБУЕТСЯ ОБНОВЛЕНИЕ В КАССЫ!', @doc_teremok_id, @doc_datetime, @doc_menu_dep) ";
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                        // id документа
                        _str_command = "SELECT @@IDENTITY";
                        _command.CommandText = _str_command;
                        _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                    }
                    if (menu_dep == "CF")
                    {
                        _str_command = "INSERT INTO t_Doc (doc_type_id, doc_status_id, doc_desc, doc_teremok_id, doc_datetime, doc_menu_dep) VALUES(7, 11, 'получено новое меню Кофейни! ТРЕБУЕТСЯ ОБНОВЛЕНИЕ В КАССЫ!', @doc_teremok_id, @doc_datetime, @doc_menu_dep) ";
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                        // id документа
                        _str_command = "SELECT @@IDENTITY";
                        _command.CommandText = _str_command;
                        _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                    }
                    if (menu_dep == "TC")
                    {
                        _str_command = "INSERT INTO t_Doc (doc_type_id, doc_status_id, doc_desc, doc_teremok_id, doc_datetime, doc_menu_dep) VALUES(7, 11, 'получено новое меню - касса Теремок и Кофейня! ТРЕБУЕТСЯ ОБНОВЛЕНИЕ В КАССЫ!', @doc_teremok_id, @doc_datetime, @doc_menu_dep) ";
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                        // id документа
                        _str_command = "SELECT @@IDENTITY";
                        _command.CommandText = _str_command;
                        _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                    }
                    if (menu_dep == "EX")
                    {
                        _str_command = "INSERT INTO t_Doc (doc_type_id, doc_status_id, doc_desc, doc_teremok_id, doc_datetime, doc_menu_dep) VALUES(7, 11, 'получено новое меню ЭКСПРЕСС! ТРЕБУЕТСЯ ОБНОВЛЕНИЕ В КАССЫ!', @doc_teremok_id, @doc_datetime, @doc_menu_dep) ";
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                        // id документа
                        _str_command = "SELECT @@IDENTITY";
                        _command.CommandText = _str_command;
                        _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                    }
                }
                else
                {
                    if (menu_dep == "TU")
                    {
                        // меню уже загружено в базу, удалим цены
                        _str_command = "DELETE FROM t_MenuPrice WHERE mp_doc_id = " + _doc_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();

                        // изменим статус и коммент
                        _str_command = "UPDATE t_Doc SET doc_status_id = 11, doc_desc='получено новое меню Рестораны! ТРЕБУЕТСЯ ОБНОВЛЕНИЕ В КАССЫ!' WHERE doc_id=" + _doc_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                    }
                    if (menu_dep == "TR")
                    {
                    // меню уже загружено в базу, удалим цены
                    _str_command = "DELETE FROM t_MenuPrice WHERE mp_doc_id = " + _doc_id.ToString();
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();

                        // изменим статус и коммент
                        _str_command = "UPDATE t_Doc SET doc_status_id = 11, doc_desc='получено новое меню Рестораны! ТРЕБУЕТСЯ ОБНОВЛЕНИЕ В КАССЫ!' WHERE doc_id=" + _doc_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                    }
                    if (menu_dep == "CF")
                    {
                        // меню уже загружено в базу, удалим цены
                        _str_command = "DELETE FROM t_MenuPrice WHERE mp_doc_id = " + _doc_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();

                        // изменим статус и коммент
                        _str_command = "UPDATE t_Doc SET doc_status_id = 11, doc_desc='получено новое меню Кофейни! ТРЕБУЕТСЯ ОБНОВЛЕНИЕ В КАССЫ!' WHERE doc_id=" + _doc_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                    }
                    if (menu_dep == "TC")
                    {
                        // меню уже загружено в базу, удалим цены
                        _str_command = "DELETE FROM t_MenuPrice WHERE mp_doc_id = " + _doc_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();

                        // изменим статус и коммент
                        _str_command = "UPDATE t_Doc SET doc_status_id = 11, doc_desc='получено новое меню Кофейни+Теремок! ТРЕБУЕТСЯ ОБНОВЛЕНИЕ В КАССЫ!' WHERE doc_id=" + _doc_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                    }
                    if (menu_dep == "EX")
                    {
                        // меню уже загружено в базу, удалим цены
                        _str_command = "DELETE FROM t_MenuPrice WHERE mp_doc_id = " + _doc_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();

                        // изменим статус и коммент
                        _str_command = "UPDATE t_Doc SET doc_status_id = 11, doc_desc='получено новое меню Экспресс! ТРЕБУЕТСЯ ОБНОВЛЕНИЕ В КАССЫ!' WHERE doc_id=" + _doc_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                    }
                }

                return _doc_id;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public DateTime GetDateMenu(int doc_id)
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            //int _doc_id;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // проверим, есть ли уже в базе такая позиция
                _str_command = "SELECT doc_datetime FROM t_Doc WHERE doc_id= @doc_id";
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_id", doc_id);
                return Convert.ToDateTime(_command.ExecuteScalar());
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public int GetMaxDateMenu()
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            //int _doc_id;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // проверим, есть ли уже в базе такая позиция
                _str_command = "SELECT MAX(doc_id)FROM t_Doc WHERE doc_type_id = 7";
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_type_id", 7);
                return Convert.ToInt32(_command.ExecuteScalar());
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public int GetMaxDateEmark()
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            //int _doc_id;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // проверим, есть ли уже в базе такая позиция
                _str_command = "SELECT MAX(doc_id)FROM t_Doc WHERE doc_type_id = 30";
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_type_id", 30);
                return Convert.ToInt32(_command.ExecuteScalar());
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public void MenuLoadPrepare(int teremok_id)
        {

            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // сбросить старое меню
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_Nomenclature SET nome_is_active = 0 WHERE nome_type_id=3 AND nome_teremok_id=" + teremok_id.ToString();
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

                // вставаить новую строчку
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public int CardDoc(string teremok_id, string menu_date)
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            int _doc_id;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // проверим, есть ли уже в базе такая позиция
                _str_command = "SELECT top 1 doc_id FROM t_Doc WHERE doc_teremok_id = @doc_teremok_id AND doc_datetime= @doc_datetime AND doc_type_id=4";
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_teremok_id", teremok_id);
                _command.Parameters.AddWithValue("@doc_datetime", menu_date);

                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    // вставить новую строчку
                    _str_command = "INSERT INTO t_Doc (doc_type_id, doc_status_id, doc_desc, doc_teremok_id, doc_datetime) VALUES(4, 13, 'получено обновление по сотрудникам', @doc_teremok_id, @doc_datetime) ";
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();

                    // id документа
                    _str_command = "SELECT @@IDENTITY";
                    _command.CommandText = _str_command;
                    _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                }
                else
                {
                    // изменим статус и коммент
                    _str_command = "UPDATE t_Doc SET doc_status_id = 13, doc_desc='получено обновление по сотрудникам' WHERE doc_id=" + _doc_id.ToString();
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }

                return _doc_id;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public int Emark(string teremok_id, string emark_date, string buildNumber)
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            int _doc_id;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // проверим, есть ли уже в базе такая позиция
                _str_command = "SELECT top 1 doc_id FROM t_Doc WHERE doc_teremok_id = @doc_teremok_id AND doc_type_id=30 AND doc_menu_dep ='" + buildNumber +"'" ;
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_teremok_id", teremok_id);
                //_command.Parameters.AddWithValue("@doc_datetime", emark_date);
                _command.Parameters.AddWithValue("@doc_menu_dep", buildNumber);

                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    // вставить новую строчку           
                    _str_command = "INSERT INTO t_Doc (doc_type_id, doc_status_id, doc_desc, doc_teremok_id, doc_menu_dep) VALUES(30, 78, 'Получено обновление рекламы на мониторах', @doc_teremok_id, @doc_menu_dep) ";
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();

                    // id документа
                    _str_command = "SELECT @@IDENTITY";
                    _command.CommandText = _str_command;
                    _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                }
                else
                {
                    // изменим статус и коммент
                    _str_command = "UPDATE t_Doc SET doc_status_id = 78, doc_desc='Получено обновление рекламы на мониторах' WHERE doc_id=" + _doc_id.ToString();
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }

                return _doc_id;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }
    }
}
