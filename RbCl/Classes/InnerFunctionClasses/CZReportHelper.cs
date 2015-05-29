using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace RBClient
{
    /// <summary>
    /// Все операция с отправкой и загрузкой данных по кассам (Z-отчет)
    /// </summary>
    class CZReportHelper
    {
        /// <summary>
        /// Проверка, отправлен ли Z-отчет в офис
        /// </summary>
        /// <param name="file_name"></param>
        /// <returns></returns>
        public bool IsZReportSent(string file_name)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            int _count;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // создать новый документ
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT count(*) FROM t_ZReport WHERE z_file = '" + file_name + "'";
                _command.CommandText = _str_command;
                _count = Convert.ToInt32(_command.ExecuteScalar());
                if (_count == 0)
                    return false;
                else
                    return true;
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

        /// <summary>
        /// Проверка, отправлены ли сегодня отчеты. Вызывается при закрытии программы. Процедура требует корреции
        /// </summary>
        /// <returns></returns>
        public bool IsZReportSentToday()
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            int _count;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // создать новый документ
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT count(*) FROM t_ZReport WHERE z_date >=#" + DateTime.Today.Month + "/" + DateTime.Today.Day + "/" + DateTime.Today.Year + " 00:00:00# AND z_date<=#" + DateTime.Today.Month + "/" + DateTime.Today.Day + "/" + DateTime.Today.Year + " 23:59:59#;";
                _command.CommandText = _str_command;
                _count = Convert.ToInt32(_command.ExecuteScalar());
                if (_count == 0)
                    return false;
                else
                    return true;
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


        /// <summary>
        /// Добавить Z-отчет в базу
        /// </summary>
        /// <param name="_conn"></param>
        /// <param name="z_num"></param>
        /// <param name="z_date"></param>
        /// <param name="z_kkm"></param>
        /// <param name="z_file"></param>
        /// <param name="z_total"></param>
        /// <param name="z_total_return"></param>
        /// <returns></returns>
        public int ZReportAdd(OleDbConnection _conn, string z_num, DateTime z_date, string z_kkm, string z_file, 
            double z_total, double z_total_return, double z_cash, double z_cash_return, double z_card, double z_card_return,
            double z_cupon, double z_cupon_return, double z_dinner, double z_dinner_return)
        {
            string _str_command;            
            OleDbCommand _command = null;
            int _doc_id;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = _conn;
                // записываем в базу документов
                string _zrep = "№ " + z_num + " на сумму " + z_total + " руб., касса: " + z_kkm;
                _str_command = "INSERT INTO t_Doc(doc_type_id, doc_teremok_id, doc_status_id, doc_desc) VALUES (5, @doc_teremok_id, 22, '" + _zrep + "')";
                _command.Parameters.AddWithValue("@doc_teremok_id", CParam.TeremokId);
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
                // id документа
                _str_command = "SELECT @@IDENTITY";
                _command.CommandText = _str_command;
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());

                // записывает отчет
                _str_command = "INSERT INTO t_ZReport(z_num, z_file, z_date, z_kkm_id, z_total, z_total_return, " + 
                    "z_cash, z_cash_return, z_card, z_card_return, " +
                    "z_cupon, z_cupon_return, z_dinner, z_dinner_return, z_doc_id, z_kkm) " + 
                    "VALUES (@z_num, @z_file, @z_date, @z_kkm_id, " +
                    "@z_total, @z_total_return, @z_cash, @z_cash_return, @z_card, @z_card_return, " +
                    "@z_cupon, @z_cupon_return, @z_dinner, @z_dinner_return, @z_doc_id, @z_kkm)";
                _command.CommandText = _str_command;
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@z_num", z_num);
                _command.Parameters.AddWithValue("@z_file", z_file);
                _command.Parameters.AddWithValue("@z_date", z_date);
                _command.Parameters.AddWithValue("@z_kkm_id", z_kkm);
                _command.Parameters.AddWithValue("@z_total", z_total);
                _command.Parameters.AddWithValue("@z_total_return", z_total_return);
                _command.Parameters.AddWithValue("@z_cash", z_cash);
                _command.Parameters.AddWithValue("@z_cash_return", z_cash_return);
                _command.Parameters.AddWithValue("@z_card", z_card);
                _command.Parameters.AddWithValue("@z_card_return", z_card_return);
                _command.Parameters.AddWithValue("@z_cupon", z_cupon);
                _command.Parameters.AddWithValue("@z_cupon_return", z_cupon_return);
                _command.Parameters.AddWithValue("@z_dinner", z_dinner);
                _command.Parameters.AddWithValue("@z_dinner_return", z_dinner_return);
                _command.Parameters.AddWithValue("@z_doc_id", _doc_id);
                _command.Parameters.AddWithValue("@z_kkm", z_kkm);
                _command.ExecuteNonQuery();

                // пишем задачу
                _str_command = "INSERT INTO t_TaskExchange(task_doc_id, task_datetime, task_state_id) VALUES (@task_doc_id, Now(), 0)";
                _command.CommandText = _str_command;
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@task_doc_id", _doc_id);
                _command.ExecuteNonQuery();

                // выходим
                return _doc_id;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }        

        public int CheckParse(OleDbConnection _conn, int _doc_id, string _check_num, DateTime _dt, double _amount)
        {
            string _str_command;            
            OleDbCommand _command = null;

            int _z_id;
            int _check_id;

            try
            {
                // вставаить новую строчку
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT z_id FROM t_ZReport WHERE z_doc_id=" + _doc_id.ToString();
                _command.CommandText = _str_command;
                _z_id = Convert.ToInt32(_command.ExecuteScalar());

                // вставляем запись по чеку
                _str_command = "INSERT INTO t_Check (check_z_id, check_amount, check_datetime, check_num) VALUES(@check_z_id, @check_amount, @check_datetime, @check_num)";
                _command.Parameters.AddWithValue("@check_z_id", _z_id);
                _command.Parameters.AddWithValue("@check_amount", _amount);
                _command.Parameters.AddWithValue("@check_datetime", _dt.ToShortDateString());
                _command.Parameters.AddWithValue("@check_num", _check_num);
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

                _str_command = "SELECT @@IDENTITY";
                _command.CommandText = _str_command;
                _command.Parameters.Clear();
                _check_id = Convert.ToInt32(_command.ExecuteScalar());

                return _check_id;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }


        public void CheckItemParse(OleDbConnection _conn, int check_id, string line, int doc_id, DateTime datetime)
        {            
            string _str_command;            
            OleDbCommand _command = null;

            string _menu_id = null;
            string _menu_1C;
            string _combo = ""; 
            double _quantity;
            double _amount1;
            double _amount2;

            try
            {
                // разбираем строчку
                _menu_1C = line.Substring(0, 13);
                _quantity = Convert.ToDouble(CUtilHelper.ParceAmount(line.Substring(13, 12), 1));
                _amount1 = Convert.ToDouble(CUtilHelper.ParceAmount(line.Substring(25, 11),1));
                _amount2 = Convert.ToDouble(CUtilHelper.ParceAmount(line.Substring(36, 11),1));
                if (line.EndsWith("P"))
                {
                    _combo = line.Substring(88, 4);
                }

                // ищем menu_id (иденцтификатор позиции меню)
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT menu_id FROM t_Menu WHERE menu_nome_1C='" + _menu_1C + "'";
                _command.CommandText = _str_command;
                Object _o = _command.ExecuteScalar();  
                
                if (_o == null)
                { 
                    // нет такой позиции меню
                    _str_command = "SELECT menu_id FROM t_Menu WHERE menu_default=TRUE";
                    _command.CommandText = _str_command;
                    _menu_id = _command.ExecuteScalar().ToString();
                }
                else
                    _menu_id = _o.ToString();

                // вставляем запись по чеку
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "INSERT INTO t_CheckItem (ch_check_id, ch_menu_id, ch_amount1, ch_amount2, ch_count, ch_check_doc_id, ch_datetime, ch_combo) VALUES(@ch_check_id, @ch_menu_id, @ch_amount1, @ch_amount2, @ch_count, @ch_check_doc_id, @ch_datetime, @ch_combo)";
                _command.Parameters.AddWithValue("@ch_check_id", check_id);
                _command.Parameters.AddWithValue("@ch_menu_id", _menu_id);
                _command.Parameters.AddWithValue("@ch_amount1", _amount1);
                _command.Parameters.AddWithValue("@ch_amount2", _amount2);
                _command.Parameters.AddWithValue("@ch_count", _quantity);
                _command.Parameters.AddWithValue("@ch_check_doc_id", doc_id);
                _command.Parameters.AddWithValue("@ch_datetime", datetime);
                _command.Parameters.AddWithValue("@ch_combo", _combo);
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

            }
            catch (Exception _exp)
            {
               // throw _exp;
            }
        }

        
        public void ZReportDelete(int _doc_id)
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            string _z_id = "";

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // удалить позиции из чека
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "DELETE FROM t_CheckItem WHERE ch_check_doc_id=" + _doc_id.ToString();
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

                // удалить чеки
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT z_id FROM t_ZReport WHERE z_doc_id=" + _doc_id.ToString();
                _command.CommandText = _str_command;
                _z_id = _command.ExecuteScalar().ToString();

                _str_command = "DELETE FROM t_Check WHERE check_z_id=" + _z_id;
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

                // удалить Z-report
                _str_command = "DELETE FROM t_TaskExchange WHERE task_doc_id=" + _doc_id.ToString();
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

                // удалить Z-report
                _str_command = "DELETE FROM t_ZReport WHERE z_doc_id=" + _doc_id.ToString();
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

                // удалить документ
                _str_command = "DELETE FROM t_Doc WHERE doc_id=" + _doc_id.ToString();
                _command.CommandText = _str_command;
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


        public void ChekItemDelete(DateTime dt)
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            DataTable _table;
            bool delete = false;

            try
            {
                {
                    _conn = new OleDbConnection(_str_connect);
                    _conn.Open();
                    // удалить позиции из чека
                    _command = new OleDbCommand();
                    _command.Connection = _conn;

                    _str_command = "SELECT * FROM t_CheckItem WHERE (ch_datetime BETWEEN #" + dt.Month + "/" + dt.Day + "/" + dt.Year + " 0:00:00# AND #" + dt.Month + "/" + dt.Day + "/" + dt.Year + " 23:59:59#)";

                    OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                    _table = new DataTable("t_ChekItemDel");
                    _data_adapter.Fill(_table);

                    foreach (DataRow _row in _table.Rows)
                    {
                        int ch_id = Convert.ToInt32(_row[0].ToString());
                        _str_command = "DELETE FROM t_CheckItem WHERE ch_id =" + ch_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                    }
                    delete = true;
                }
                if (delete == true)
                {
                    _conn = new OleDbConnection(_str_connect);
                    _conn.Open();
                    // удалить позиции из чека
                    _command = new OleDbCommand();
                    _command.Connection = _conn;

                    _str_command = "SELECT * FROM t_Check WHERE (check_datetime BETWEEN #" + dt.Month + "/" + dt.Day + "/" + dt.Year + " 0:00:00# AND #" + dt.Month + "/" + dt.Day + "/" + dt.Year + " 23:59:59#)";

                    OleDbDataAdapter _data_adapter1 = new OleDbDataAdapter(_str_command, _conn);
                    _table = new DataTable("t_ChekDel");
                    _data_adapter1.Fill(_table);

                    foreach (DataRow _row in _table.Rows)
                    {
                        int check_id = Convert.ToInt32(_row[0].ToString());
                        _str_command = "DELETE FROM t_Check WHERE check_id =" + check_id.ToString();
                        _command.CommandText = _str_command;
                        _command.ExecuteNonQuery();
                    }
                }
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

        public string GetZReportFileName(int doc_id)
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT z_file FROM t_ZReport WHERE z_doc_id=" + doc_id.ToString();
                _command.CommandText = _str_command;
                return _command.ExecuteScalar().ToString();
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

        public void GetZReportDetail(ref string zrep, ref string zdate, ref string ztotal, ref string ztotal_return, 
            ref string zcash, ref string zcash_return, ref string zcard, ref string zcard_return,
            ref string zcupon, ref string zcupon_return, ref string zdinner, ref string zdinner_return, 
            ref string zkks, int m_doc_id)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // создать новый документ
                _command = new OleDbCommand();
                _command.Connection = _conn;

                _str_command = "SELECT top 1 z_num, z_date, z_total, z_total_return, " +
                    "z_cash, z_cash_return, z_card, z_card_return, z_cupon, z_cupon_return, z_dinner, z_dinner_return, " +
                    "z_kkm_id FROM t_ZReport WHERE z_doc_id=@z_doc_id";
                
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@z_doc_id", m_doc_id);

                //_command.ExecuteScalar();
                OleDbDataReader _reader = _command.ExecuteReader();
                while (_reader.Read())
                {
                    zrep = _reader["z_num"].ToString();
                    zdate = _reader["z_date"].ToString();
                    ztotal = _reader["z_total"].ToString();
                    ztotal_return = _reader["z_total_return"].ToString();
                    zcash = _reader["z_cash"].ToString();
                    zcash_return = _reader["z_cash_return"].ToString();
                    zcard = _reader["z_card"].ToString();
                    zcard_return = _reader["z_card_return"].ToString();
                    zcupon = _reader["z_cupon"].ToString();
                    zcupon_return = _reader["z_cupon_return"].ToString();
                    zdinner = _reader["z_dinner"].ToString();
                    zdinner_return = _reader["z_dinner_return"].ToString();
                    zkks = _reader["z_kkm_id"].ToString();
                }
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

        /// <summary>
        /// Загрузка Z-отчета в базу
        /// </summary>
        /// <param name="file"></param>
        public int ZReportParse(FileInfo file)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            StreamReader _sr = null;
            string _line_or; // оригинальная строка
            string _line;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();                
                
                char[] _separator_space = " ".ToCharArray();
                string _result;
                DateTime _date;
                string _param;

                // параметры смены
                string _znum = null;
                string _kkm = null;
                string _FR = null;
                double _total = 0;
                double _total_return = 0;
                double _cash = 0;
                double _cash_return = 0;
                double _card = 0;
                double _card_return = 0;
                double _cupon = 0;
                double _cupon_return = 0;
                double _dinner = 0;
                double _dinner_return = 0;

                // чек
                bool _check_parse = false;
                int _check_id = 0;
                int _doc_id = 0;                
                DateTime _check_datetime = DateTime.Now;
                DateTime _date_del;
              
                //CBData _data = new CBData();
                //CZReportHelper _zreport = new CZReportHelper();
                _sr = new StreamReader(file.FullName, System.Text.Encoding.GetEncoding(1251));
                int _count_line = 0;
                while ((_line_or = _sr.ReadLine()) != null)
                {
                    _line = MyTrim(_line_or);                    
                    string[] _s = _line.Split(_separator_space);
                    _param = _s[0];
                    _count_line++;
                    if (_count_line == 3) // фискальный регистратор                        
                        _FR = _s[0] + " / " + _s[_s.Length - 1];
                    else
                    {
                            switch (_param)
                            {
                                case "Касса":
                                    _kkm = _s[_s.Length - 1];
                                    break;
                                case "Смена":
                                    _result = _s[_s.Length - 1];
                                    break;
                                case "Zномер":
                                    _znum = _s[_s.Length - 1];
                                    break;
                                case "ИтогПродаж":
                                    _total = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 1], 1));
                                    break;
                                case "Возврат/аннулирование":
                                    _total_return = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 1], 1));
                                    break;
                                case "ПродажаНал":
                                    _cash = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 1], 1));
                                    // последний тег, пишем в базу.
                                    // получаем дату файла
                                    _date = new DateTime(Convert.ToInt32("20" + file.Name.Substring(1, 2)), Convert.ToInt32(file.Name.Substring(3, 2)), Convert.ToInt32(file.Name.Substring(5, 2)));
                                    _doc_id = ZReportAdd(_conn, _znum, _date, _kkm, file.Name, _total, _total_return, _cash, _cash_return,
                                        _card, _card_return, _cupon, _cupon_return, _dinner, _dinner_return);

                                    return _doc_id;

                                    _date_del = _date.AddDays(-1);

                                    break;
                                case "ВозвратНал":
                                    _cash_return = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 1], 1));
                                    break;
                                case "ПродажаКарта":
                                    _card = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 1], 1));
                                    break;
                                case "ВозвратКарта":
                                    _card_return = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 1], 1));
                                    break;
                                case "ПродажаКупоны":
                                    _cupon = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 1], 1));
                                    break;
                                case "ВозвратКупоны":
                                    _cupon_return = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 1], 1));
                                    break;
                                case "ПродажаОбеды":
                                    _dinner = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 1], 1));
                                    break;
                                case "ВозвратОбеды":
                                    _dinner_return = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 1], 1));                                 
                                    break;
                                case "Чек":
                                    {   

                                        if (CParam.LoadReportToDataBase == 1)
                                        {
                                            _check_parse = true;
                                            
                                            // парсим чек
                                            _check_datetime = Convert.ToDateTime(_s[2] + " " + _s[3]);                              
                                             double amount = Convert.ToDouble(CUtilHelper.ParceAmount(_s[_s.Length - 3], 1));
                                            _check_id = CheckParse(_conn, _doc_id, _s[1], _check_datetime, amount); // doc_id, номер чека, дата, время
                                        }
                                    }
                                    break;
                                case "+":
                                    _check_parse = false;
                                    break;
                                case "-":
                                    _check_parse = false;
                                    break;
                                case "Оплата":
                                    break;
                                case "":
                                    break;
                                case "Возврат":
                                    break;
                                default:
                                    // параметры чека
                                    if (_check_parse)
                                    {
                                        // вставляем даные по позиции чека
                                        if (CParam.LoadReportToDataBase == 1)
                                            CheckItemParse(_conn, _check_id, _line_or, _doc_id, _check_datetime);
                                    }
                                    break;
                            }
                    }

                }
                return _doc_id;
            }
            catch (Exception exp)
            {
                throw exp;
            }            
            finally
            {
                if (_conn != null)
                    _conn.Close();
                if (_sr != null)
                    _sr.Close();
            }
        }

        

        

        private void CheckItemDeleteByKKM(OleDbConnection _conn, string _kkm)
        {
            string _str_command;
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "DELETE FROM t_CheckCurrent WHERE cc_KKM='" + _kkm + "'";
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }
        
        private string MyTrim(string str)
        {            
            string res = "";
            bool _space = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].ToString() != " ")
                {
                    res = res + str[i];
                    _space = false;
                }
                else
                {
                    if (!_space)
                    {
                        res = res + str[i];
                        _space = true;
                    }
                }
            }
            return res.Trim();
        }

        /// <summary>
        /// проверка промежуточного отчета на предмет нужно ли удалить промежуточные данные
        /// </summary>
        /// <returns></returns>
        public void CheckTReport()
        {            
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            DateTime _dt;// = DateTime.Now;
            
            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT conf_value FROM t_Conf WHERE conf_id=24";
                _command.CommandText = _str_command;

                _dt = Convert.ToDateTime(_command.ExecuteScalar());                

                // проверка на дату
                if (_dt.ToShortDateString() != DateTime.Now.ToShortDateString())
                { 
                    // произошло смена дня, удалить промежуточные данные
                    _str_command = "DELETE FROM t_CheckCurrent";
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();

                    _str_command = "UPDATE t_Conf SET conf_value = Now() WHERE conf_id=24";
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }
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

        private void CheckItemTReport(OleDbConnection _conn, DateTime _check_datetime, string line, string _kkm)
        {
            string _str_command;
            OleDbCommand _command = null;

            string _menu_id = null;
            string _menu_1C;
            double _quantity;
            double _amount1;
            double _amount2;

            try
            {
                // разбираем строчку
                _menu_1C = line.Substring(0, 13);
                _quantity = Convert.ToDouble(CUtilHelper.ParceAmount(line.Substring(13, 12), 1));
                _amount1 = Convert.ToDouble(CUtilHelper.ParceAmount(line.Substring(25, 11), 1));
                _amount2 = Convert.ToDouble(CUtilHelper.ParceAmount(line.Substring(36, 11), 1));

                // ищем menu_id (иденцтификатор позиции меню)
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT menu_id FROM t_Menu WHERE menu_nome_1C='" + _menu_1C + "'";
                _command.CommandText = _str_command;
                Object _o = _command.ExecuteScalar();

                if (_o == null)
                {
                    // нет такой позиции меню
                    _str_command = "SELECT menu_id FROM t_Menu WHERE menu_default=TRUE";
                    _command.CommandText = _str_command;
                    _menu_id = _command.ExecuteScalar().ToString();
                }
                else
                    _menu_id = _o.ToString();

                // вставляем запись по чеку
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "INSERT INTO t_CheckCurrent(cc_menu_id, cc_amount1, cc_amount2, cc_count, cc_datetime, cc_KKM) " +
                    " VALUES(@cc_menu_id, @cc_amount1, @cc_amount2, @cc_count, @cc_datetime, @cc_KKM)";
                _command.Parameters.AddWithValue("@cc_menu_id", _menu_id);
                _command.Parameters.AddWithValue("@cc_amount1", _amount1);
                _command.Parameters.AddWithValue("@cc_amount2", _amount2);
                _command.Parameters.AddWithValue("@cc_count", _quantity);
                _command.Parameters.AddWithValue("@cc_datetime", _check_datetime);
                _command.Parameters.AddWithValue("@cc_KKM", _kkm);
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        /// <summary>
        /// Разложить T-файл
        /// </summary>
        /// <param name="file"></param>
        public void TReportParse(FileInfo file)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            StreamReader _sr = null;

            string _line_or; // оригинальная строка
            string _line;
            char[] _separator_space = " ".ToCharArray();

            string _param;

            // параметры смены                
            string _kkm = null;

            // чек
            bool _check_parse = false;
            string _check_num = "";

            DateTime _check_datetime = DateTime.Now;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();

                //CBData _data = new CBData();                
                _sr = new StreamReader(file.FullName, System.Text.Encoding.GetEncoding(1251));

                while ((_line_or = _sr.ReadLine()) != null)
                {
                    _line = MyTrim(_line_or);
                    string[] _s = _line.Split(_separator_space);
                    _param = _s[0];

                    switch (_param)
                    {
                        case "Касса":
                            _kkm = _s[_s.Length - 1];
                            // удалить все записи по этой кассе
                            CheckItemDeleteByKKM(_conn, _kkm);
                            break;
                        case "Оплата":
                        case "Возврат":
                        case "":
                            break;
                        case "Чек":
                            _check_parse = true;
                            // парсим чек
                            if (_s[2] == "")
                                _check_datetime = Convert.ToDateTime(_s[2] + " " + _s[3]);
                            else
                                _check_datetime = Convert.ToDateTime(_s[6] + " " + _s[7]);
                            _check_num = _s[1];
                            break;
                        case "+":
                        case "-":
                            _check_parse = false;
                            break;
                        default:
                            // параметры чека
                            if (_check_parse)
                                // вставляем даные по позиции чека                                
                                CheckItemTReport(_conn, _check_datetime, _line_or, _kkm);
                            break;
                    }
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
                if (_sr != null)
                    _sr.Close();
            }
        }
    }
}

