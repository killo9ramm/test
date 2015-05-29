using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using Common.Logging;
using RBClient.Classes;
using RBClient.Classes.CustomClasses;

namespace RBClient
{
    class CUpdateHelper
    {
        public bool m_table = false;
        internal ILog log;

        internal CUpdateHelper()
        {
            log = LogManager.GetCurrentClassLogger();
        }

        public bool ExecuteCommand(string Command)
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;

                _command.CommandText = Command;
                _command.ExecuteNonQuery();

                log.Info("success " + Command.Substring(0, Command.Length > 100 ? 100 : Command.Length) + "...");
                return true;
            }
            catch (Exception exp)
            {
                log.Error(exp.Message);
                return false;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public DataTable ExecuteCommandTable(string Command)
        {

            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            DataTable _table;
            string _str_command;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                // получить список документов
                _str_command = Command;
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("table");
                _data_adapter.Fill(_table);
                return _table;
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

        public void DeleteTeremok()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            DataTable _dt_teremok;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;

                _dt_teremok = ItemsTeremok();

                foreach (DataRow _row in _dt_teremok.Rows)
                {
                    if (Convert.ToInt32(_row[0]) == 167 || Convert.ToInt32(_row[0]) == 167)
                    {
                        _command.CommandText = "DELETE FROM t_Teremok WHERE teremok_id =" + Convert.ToInt32(_row[0]);
                        _command.ExecuteNonQuery();
                    };
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
            }
        }

        public DataTable ItemsTeremok()
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            DataTable _table;
            string _str_command;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                // получить список документов
                _str_command = "SELECT teremok_id FROM t_Teremok WHERE teremok_current <> 0";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("table");
                _data_adapter.Fill(_table);
                return _table;
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

        public void NewTeremok()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            int _teremok_id = 0;
            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;

                if (CParam.AppCity == 2)
                {
                    //XL3 + ВДНХ
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 160";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 167";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (167, 'vdnh', 'ВДНХ', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //XL Дмитровка + Перуновский
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 148";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 168";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (168, 'peru', 'Перуновский', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }


                    //Речной + Садово-самотечная
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 163";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 223";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (223, 'sadov', 'Садово-Самотечная, 24', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Твой Дом + Якиманка большая
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 130";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 183";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (183, 'byaki', 'Якиманка Большая', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Серебряный Дом + Воробьевы горы
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 108";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 188";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (188, 'vorob', 'Воробьевы Горы', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Иридиум + Гр вал
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 161";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 175";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (175, 'grval', 'Грузинский Вал', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Тройка + Ярмарка на севастопольской
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 132";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 172";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (172, 'yarma', 'Ярмарка на Севастопольской', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Домодедовский + Магистральная 4-я, 2
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 119";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 230";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (230, 'magis', 'Магистральная 4-я, 2', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //АСТ + Трубная
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 113";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 169";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (169, 'trub', 'Трубная', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Варшавский + Серпуховская
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 135";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 170";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (170, 'serpu', 'Серпуховская', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Капитолий-беляево + Менделеевская
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 111";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 177";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (177, 'mende', 'Менделеевская', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Капитолий-марьена роща + Каланчевская
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 104";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 178";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (178, 'kalan', 'Каланчевская', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Капитолий-Университет + Комсомольский пр-т
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 134";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 179";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (179, 'kompr', 'Комсомольский пр-т', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Красный кит + Бакунинская
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 159";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 245";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (245, 'bakun', 'Бакунинская, 17/28 СТР', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Ладья + Пушечная
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 139";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 173";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (173, 'pushk', 'Пушечная', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //На беговой + Пр. Вернадского 105
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 127";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 195";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (195, 've105', 'Пр. Вернадского 105', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Облака + Пр.Вернадского
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 123";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 176";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (176, 'prver', 'Пр.Вернадского', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Пражский пассаж + Сухаревская
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 138";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 184";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (184, 'msuha', 'Сухаревская', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Принц Плаза + Щепкина
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 109";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 235";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (235, 'shepk', 'Щепкина, 58', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //РИО + Маяковская
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 110";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 201";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (201, 'mayak', 'Маяковская', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Светофор Люберцы + Юшуньская
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 155";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 217";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (217, 'yushu', 'Юшуньская', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Филион + Лесная
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 146";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 166";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (166, 'lesna', 'Лесная', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Щелково + Пр. Вернадского 84
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 115";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 194";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (194, 'ver84', 'Пр. Вернадского 84', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Л-153 + Климентовский
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 136";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 219";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (219, 'klime', 'Климентовский', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
                    }

                    //Рио Дмитровский + Тишинская пл. 8 СТР
                    _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 238";
                    _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                    if (_teremok_id == 0) // нет такой записи, не наш ресторан
                    {
                    }
                    else
                    {
                        // наш ресторан
                        _command.CommandText = "SELECT count(*) FROM t_Teremok WHERE teremok_id = 241";

                        _teremok_id = Convert.ToInt32(_command.ExecuteScalar());
                        if (_teremok_id == 0) // не вставляли
                        {
                            _command.CommandText = "INSERT INTO t_Teremok(teremok_id, teremok_1C, teremok_name, teremok_current, teremok_dep) " +
                            " VALUES (241, 'tishi', 'Тишинская пл. 8 СТР', -1, 2);";
                            _command.ExecuteNonQuery();
                        }
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
            }
        }

        #region trash 3_5_8
        //public void Update_3_5_8()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        if (CParam.AppVer == "3.5.7.1")
        //        {
        //            _conn = new OleDbConnection(CParam.ConnString);
        //            _conn.Open();
        //            _command = new OleDbCommand();
        //            _command.Connection = _conn;

        //            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_total decimal (18,4) DEFAULT 0";
        //            _command.ExecuteNonQuery();

        //            _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_total='0'";
        //            _command.ExecuteNonQuery();

        //            //обновление номера версии
        //            _command.CommandText = "UPDATE t_Conf SET conf_value='3.5.8' WHERE conf_id=1";
        //            _command.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}


        //public void Update_3_5_8()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        if (CParam.AppVer == "3.5.7.1")
        //        {
        //            _conn = new OleDbConnection(CParam.ConnString);
        //            _conn.Open();
        //            _command = new OleDbCommand();
        //            _command.Connection = _conn;

        //            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_total decimal (18,4) DEFAULT 0";
        //            _command.ExecuteNonQuery();
        //            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_ze TEXT(255)";
        //            _command.ExecuteNonQuery();
        //            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_ke decimal (18,4) DEFAULT 0";
        //            _command.ExecuteNonQuery();
        //            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_bold decimal (18,4) DEFAULT 0";
        //            _command.ExecuteNonQuery();

        //            _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_total='0'";
        //            _command.ExecuteNonQuery();
        //            _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_ze='0'";
        //            _command.ExecuteNonQuery();
        //            _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_ke='0'";
        //            _command.ExecuteNonQuery();
        //            _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_bold='0'";
        //            _command.ExecuteNonQuery();

        //            _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_ke decimal (18,4) DEFAULT 0";
        //            _command.ExecuteNonQuery();    
        //            _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_ze TEXT(255)";
        //            _command.ExecuteNonQuery();
        //            _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_bold decimal (18,4) DEFAULT 0";
        //            _command.ExecuteNonQuery();

        //            _command.CommandText = "UPDATE t_Nome2Teremok SET n2t_ke='0'";
        //            _command.ExecuteNonQuery();
        //            _command.CommandText = "UPDATE t_Nome2Teremok SET n2t_bold='0'";
        //            _command.ExecuteNonQuery();

        //            _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(22, 'обновление иснтрукции')";
        //            _command.ExecuteNonQuery();
        //            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('получен',22,4)"; 
        //            _command.ExecuteNonQuery();                      

        //            _command.CommandText = "ALTER TABLE t_Doc ADD COLUMN doc_menu_dep TEXT(255)";
        //            _command.ExecuteNonQuery();     

        //            //обновление номера версии
        //            _command.CommandText = "UPDATE t_Conf SET conf_value='3.6' WHERE conf_id=1";
        //            _command.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}
#endregion

        public void Update_4_0_0_1()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            try
            {
                if (CParam.AppCity == 1)
                {
                    if (CParam.AppVer == "3.5.6")
                    {
                        {
                            _conn = new OleDbConnection(CParam.ConnString);
                            _conn.Open();
                            _command = new OleDbCommand();
                            _command.Connection = _conn;

                            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_total decimal (18,4) DEFAULT 0";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_ze TEXT(255)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_ke decimal (18,4) DEFAULT 0";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_bold decimal (18,4) DEFAULT 0";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_maxquota decimal (18,4) DEFAULT 0";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_order3 decimal (18,4) DEFAULT 0";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_total='0'";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_ke='0'";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_bold='0'";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_maxquota='0'";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_order3='0'";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_maxquota decimal (18,4) DEFAULT 0";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_ke decimal (18,4) DEFAULT 0";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_ze TEXT(255)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_bold decimal (18,4) DEFAULT 0";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "UPDATE t_Nome2Teremok SET n2t_ke='0'";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "UPDATE t_Nome2Teremok SET n2t_bold='0'";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "UPDATE t_Nome2Teremok SET n2t_maxquota='0'";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "ALTER TABLE t_Doc ADD COLUMN doc_menu_dep TEXT(255)";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "ALTER TABLE t_Teremok ADD COLUMN teremok_dep int;";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "UPDATE t_Teremok SET teremok_dep='1'";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(19, 'инкассация')";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(23, 'счетчики воды')";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(24, 'счетчики эл.')";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(22, 'обновление иснтрукции')";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('получен',22,4)";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(23, 'счетчики воды')";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('новый',23,1)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправляется',23,2)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправлен',23,3)";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(24, 'счетчики эл.')";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('новый',24,1)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправляется',24,2)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправлен',24,3)";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name = 'инкассация' WHERE doctype_id=19";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('новый',19,1)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправляется',19,2)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправлен',19,3)";
                            _command.ExecuteNonQuery();
                        }

                        //обновление номера версии
                        _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.1' WHERE conf_id=1";
                        _command.ExecuteNonQuery();
                    }
                }
                else
                {
                    if (CParam.AppVer == "3.5.7.1")
                    {
                        _conn = new OleDbConnection(CParam.ConnString);
                        _conn.Open();
                        _command = new OleDbCommand();
                        _command.Connection = _conn;


                        _command.CommandText = "DELETE FROM t_DocStatusRef WHERE doctype_id = 6";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "DELETE FROM t_DocStatusRef WHERE doctype_id = 11";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "DELETE FROM t_DocStatusRef WHERE doctype_id = 12";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "DELETE FROM t_DocTypeRef WHERE doctype_id = 6";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "DELETE FROM t_DocTypeRef WHERE doctype_id = 11";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "DELETE FROM t_DocTypeRef WHERE doctype_id = 12";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "DELETE FROM t_DocTypeRef WHERE doctype_id = 20";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Ежедневный заказ'  WHERE doctype_id = 1";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Заказ хоз.средств' WHERE doctype_id = 2";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Остатки' WHERE doctype_id = 3";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Карты сотрудников' WHERE doctype_id = 4";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Меню' WHERE doctype_id = 7";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Шаблон' WHERE doctype_id = 8";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Списание Г П' WHERE doctype_id = 9";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Контрольные остатки' WHERE doctype_id = 10";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Списание С П' WHERE doctype_id = 13";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Остатки инвентаря' WHERE doctype_id = 14";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Вх. накладная' WHERE doctype_id = 15";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Исх. накладная' WHERE doctype_id = 16";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Заказ х/с нед.' WHERE doctype_id = 17";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Заказ х/с мес.' WHERE doctype_id = 18";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_total decimal (18,4) DEFAULT 0";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_ze TEXT(255)";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_ke decimal (18,4) DEFAULT 0";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_bold decimal (18,4) DEFAULT 0";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_total='0'";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_ze='0'";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_ke='0'";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_Order2ProdDetails SET opd_bold='0'";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_ke decimal (18,4) DEFAULT 0";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_ze TEXT(255)";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_bold decimal (18,4) DEFAULT 0";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "UPDATE t_Nome2Teremok SET n2t_ke='0'";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "UPDATE t_Nome2Teremok SET n2t_bold='0'";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(22, 'Обновление инструкции')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('получен',22,4)";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "ALTER TABLE t_Doc ADD COLUMN doc_menu_dep TEXT(255)";
                        _command.ExecuteNonQuery();

                        //обновление номера версии
                        _command.CommandText = "UPDATE t_Conf SET conf_value='3.6' WHERE conf_id=1";
                        _command.ExecuteNonQuery();
                    }

                    if (CParam.AppVer == "3.5.7.1" || CParam.AppVer == "3.6")
                    {
                        {
                            _conn = new OleDbConnection(CParam.ConnString);
                            _conn.Open();
                            _command = new OleDbCommand();
                            _command.Connection = _conn;

                            _command.CommandText = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(23, 'Счетчики воды')";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(24, 'Счетчики эл.')";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(23, 'Счетчики воды')";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('новый',23,1)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправляется',23,2)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправлен',23,3)";
                            _command.ExecuteNonQuery();

                            _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(24, 'Счетчики эл.')";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('новый',24,1)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправляется',24,2)";
                            _command.ExecuteNonQuery();
                            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправлен',24,3)";
                            _command.ExecuteNonQuery();
                        }

                        //обновление номера версии
                        _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.1' WHERE conf_id=1";
                        _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_2()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.1")
                {
                    _command.CommandText = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(27, 'остатки ночь')";
                    _command.ExecuteNonQuery();

                    _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(27, 'остатки ночь')";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('новый',27,1)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправляется',27,2)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправлен',27,3)";
                    _command.ExecuteNonQuery();

                    //обновление номера версии
                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.2' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_3()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.2")
                {
                    //_command.CommandText = "CREATE TABLE t_Standart(t_name text(255), t_path_name text(255), t_hash_code text(255), t_creation_time text(255))";
                    //_command.ExecuteNonQuery();


                    _command.CommandText = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(26, 'Cчетчики гр. воды.')";
                    _command.ExecuteNonQuery();

                    _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(26, 'счетчики гр. воды.')";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('новый',26,1)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправляется',26,2)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправлен',26,3)";
                    _command.ExecuteNonQuery();

                    //обновление номера версии
                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.3' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_4()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.3")
                {
                    //обновление номера версии
                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.4' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_5()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.4")
                {
                    //обновление номера версии
                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.5' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_6()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.5")
                {
                    //обновление номера версии
                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.6' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_7()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.6")
                {
                    _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Ежедневный заказ'  WHERE doctype_id = 1";
                    _command.ExecuteNonQuery();

                    //обновление номера версии
                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.7' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_8()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.7")
                {
                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.8' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_9()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.8")
                {
                    //_command.CommandText = "INSERT INTO t_Conf(conf_param, conf_value, conf_dep) VALUES('folder_kkm5_in','', '')";
                    //_command.ExecuteNonQuery();

                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.9' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_10()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppCity == 1)
                {
                    if (CParam.AppVer == "4.0.0.9")
                    {
                        _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.10' WHERE conf_id=1";
                        _command.ExecuteNonQuery();
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
            }
        }

        #region trash
        //public void Update_4_0_0_10() 
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    _conn = new OleDbConnection(CParam.ConnString);
        //    _conn.Open();
        //    _command = new OleDbCommand();
        //    _command.Connection = _conn;
        //    try
        //    {
        //        if (CParam.AppVer == "4.0.0.9")
        //        {
        //            _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.10' WHERE conf_id=1";
        //            _command.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //public void Update_4_0_0_11()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    _conn = new OleDbConnection(CParam.ConnString);
        //    _conn.Open();
        //    _command = new OleDbCommand();
        //    _command.Connection = _conn;
        //    try
        //    {
        //        if (CParam.AppVer == "4.0.0.10")
        //        {
        //            _command.CommandText = "CREATE TABLE t_Standart(t_name text(255), t_path_name text(255), t_hash_code text(255), t_creation_time text(255))";
        //            _command.ExecuteNonQuery();

        //            _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.11' WHERE conf_id=1";
        //            _command.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}
#endregion

        public void Update_4_0_0_11()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.11")
                {
                    //_command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(27, 'пусто')";
                    //_command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(28, 'пусто')";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(29, 'пусто')";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_id, doctype_name) VALUES(30, 'реклама на мониторах')";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('получено',30,5)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('обновлено',30,4)";
                    _command.ExecuteNonQuery();

                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.12' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_12()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppCity == 1)
                {
                    if (CParam.AppVer == "4.0.10" || CParam.AppVer == "4.0.0.10")
                    {
                        _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_ban decimal (18,4) DEFAULT 0";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "UPDATE t_Nome2Teremok SET n2t_ban='0'";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "CREATE TABLE t_SettingsDoc(id integer, column_doc_id integer, column_id integer, column_status logical, column_name text(255));";
                        _command.ExecuteNonQuery();


                        //Заказ
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(1, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(1, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(1, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(1, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Склад (продукты)
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(2, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(2, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(2, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(2, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Остатки
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(3, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(3, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(3, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(3, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Списание
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(6, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(6, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(6, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(6, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //списание Г П
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(9, 5,   0, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(9, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(9, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(9, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //контрольные остатки
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(10, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(10, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(10, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(10, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //списание СП
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(13, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(13, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(13, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(13, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //остатки инвентаря
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(14, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(14, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(14, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(14, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Вх.Накладная
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(15, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(15, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(15, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(15, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //накладная
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(16, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(16, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(16, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(16, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //заказ ХС недельный
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(17, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(17, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(17, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(17, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //заказ ХС месячный
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(18, 5, 1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(18, 15, 1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(18, 16, 1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(18, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //инкассация
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(19, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(19, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(19, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(19, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Перемещение
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(21, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(21, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(21, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(21, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //счетчики воды
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(23, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(23, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(23, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(23, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //счетчики эл.
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(24, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(24, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(24, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(24, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //остатки ночь
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(25, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(25, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(25, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(25, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Cчетчики гр. воды.
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(26, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(26, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(26, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(26, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();


                        _command.CommandText = "CREATE TABLE t_NewDoc(id identity unique NOT NULL, t_filename text(255), t_screenName text(255))";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "CREATE TABLE t_DocReestr(t_doc_release text(255), t_doc_switch text(255), t_doc_filename text(255), t_doc_screenName text(255), t_doc_category text(255))";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.14' WHERE conf_id=1";
                        _command.ExecuteNonQuery();
                    }
                }
                if (CParam.AppCity == 2)
                {
                    if (CParam.AppVer == "4.0.0.12")
                    {
                        _command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_ban decimal (18,4) DEFAULT 0";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "UPDATE t_Nome2Teremok SET n2t_ban='0'";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "CREATE TABLE t_SettingsDoc(id integer, column_doc_id integer, column_id integer, column_status logical, column_name text(255));";
                        _command.ExecuteNonQuery();


                        //Заказ
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(1, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(1, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(1, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(1, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Склад (продукты)
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(2, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(2, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(2, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(2, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Остатки
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(3, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(3, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(3, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(3, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Списание
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(6, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(6, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(6, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(6, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //списание Г П
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(9, 5,   0, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(9, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(9, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(9, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //контрольные остатки
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(10, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(10, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(10, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(10, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //списание СП
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(13, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(13, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(13, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(13, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //остатки инвентаря
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(14, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(14, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(14, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(14, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Вх.Накладная
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(15, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(15, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(15, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(15, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //накладная
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(16, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(16, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(16, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(16, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //заказ ХС недельный
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(17, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(17, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(17, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(17, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //заказ ХС месячный
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(18, 5, 1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(18, 15, 1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(18, 16, 1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(18, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //инкассация
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(19, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(19, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(19, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(19, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Перемещение
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(21, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(21, 15,  0, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(21, 16,  0, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(21, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //счетчики воды
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(23, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(23, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(23, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(23, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //счетчики эл.
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(24, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(24, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(24, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(24, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //остатки ночь
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(25, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(25, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(25, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(25, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();

                        //Cчетчики гр. воды.
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(26, 5,   1, 'Ед. измер.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(26, 15,  1, 'Ед.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(26, 16,  1, 'Коэф.')";
                        _command.ExecuteNonQuery();
                        _command.CommandText = "INSERT INTO t_SettingsDoc(column_doc_id, column_id, column_status, column_name) VALUES(26, 100, 0, 'Информация о номенклатуре')";
                        _command.ExecuteNonQuery();


                        _command.CommandText = "CREATE TABLE t_NewDoc(id identity unique NOT NULL, t_filename text(255), t_screenName text(255))";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "CREATE TABLE t_DocReestr(t_doc_release text(255), t_doc_switch text(255), t_doc_filename text(255), t_doc_screenName text(255), t_doc_category text(255))";
                        _command.ExecuteNonQuery();

                        _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.14' WHERE conf_id=1";
                        _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_15()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.14")
                {
                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.15' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }

        public void Update_4_0_0_16()
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            _conn = new OleDbConnection(CParam.ConnString);
            _conn.Open();
            _command = new OleDbCommand();
            _command.Connection = _conn;
            try
            {
                if (CParam.AppVer == "4.0.0.15")
                {
                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.16' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
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
            }
        }



        public void Update_4_0_0_18()
        {
            if (IsNewerVersion("4.0.0.18"))
            {
                SetVersion("4.0.0.18");
                OleDbConnection _conn = null;
                OleDbCommand _command = null;
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                try
                {
                    //t_Teremok 
                    _command.CommandText = "ALTER TABLE t_Teremok ADD COLUMN teremok_first_time datetime";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "ALTER TABLE t_Teremok ADD COLUMN teremok_last_time datetime";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "ALTER TABLE t_Teremok ADD COLUMN teremok_guid TEXT(255)";
                    _command.ExecuteNonQuery();

                    //t_Employee
                    _command.CommandText = "ALTER TABLE t_Employee ADD COLUMN employee_FunctionName TEXT(255)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "ALTER TABLE t_Employee ADD COLUMN employee_FunctionGuid TEXT(255)";
                    _command.ExecuteNonQuery();

                    //t_Doc doc_guid
                    _command.CommandText = "ALTER TABLE t_Doc ADD COLUMN doc_guid TEXT(255)";
                    _command.ExecuteNonQuery();

                    //t_ButtonTemplate
                    _command.CommandText = "CREATE TABLE t_ButtonTemplate(id identity unique NOT NULL, btn_guid text(255), btn_name text(255), bnt_value text(255), btn_color text(255), btn_hint text(255), btn_IsUsed logical, btn_Order integer, btn_SmenaType text(255))";
                    _command.ExecuteNonQuery();

                    //t_Responsibility
                    _command.CommandText = "CREATE TABLE t_Responsibility(id identity unique NOT NULL, res_guid text(255), res_name text(255))";
                    _command.ExecuteNonQuery();

                    //t_WorkOther
                    _command.CommandText = "CREATE TABLE t_WorkOther(id identity unique NOT NULL, guid_emp text(255), guid_shift text(255), day_short integer, value_time integer, first_time datetime, last_time datetime)";
                    _command.ExecuteNonQuery();

                    _command.CommandText = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=27";
                    if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
                    {
                        _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('Остатки утро')";
                        _command.ExecuteNonQuery();
                    }

                    _command.CommandText = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=28";
                    if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
                    {
                        _command.CommandText = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('Табель')";
                        _command.ExecuteNonQuery();
                    }

                    _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name='Табель' WHERE doctype_id = 28";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('новый',28,1)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправляется',28,2)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправлен',28,3)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('получен',28,4)";
                    _command.ExecuteNonQuery();

                    //t_MarkItems
                    _command.CommandText = "CREATE TABLE t_MarkItems(mark_id identity unique NOT NULL, mark_row_id integer, mark_doc_id integer, mark_name text(255), mark_office text(255), mark_total text(255)," +
                    "mark_1 text(255), mark_2 text(255), mark_3 text(255), mark_4 text(255), mark_5 text(255), mark_6 text(255), mark_7 text(255), mark_8 text(255), mark_9 text(255), mark_10 text(255), " +
                    "mark_11 text(255), mark_12 text(255), mark_13 text(255), mark_14 text(255), mark_15 text(255), mark_16 text(255), mark_17 text(255), mark_18 text(255), mark_19 text(255), mark_20 text(255), " +
                    "mark_21 text(255), mark_22 text(255), mark_23 text(255), mark_24 text(255), mark_25 text(255), mark_26 text(255), mark_27 text(255), mark_28 text(255), mark_29 text(255), mark_30 text(255), mark_31 text(255), " +
                    "mark_guid_1 text(255), mark_guid_2 text(255), mark_guid_3 text(255), mark_guid_4 text(255), mark_guid_5 text(255), mark_guid_6 text(255), mark_guid_7 text(255), mark_guid_8 text(255), mark_guid_9 text(255), mark_guid_10 text(255), " +
                    "mark_guid_11 text(255), mark_guid_12 text(255), mark_guid_13 text(255), mark_guid_14 text(255), mark_guid_15 text(255), mark_guid_16 text(255), mark_guid_17 text(255), mark_guid_18 text(255), mark_guid_19 text(255), mark_guid_20 text(255), " +
                    "mark_guid_21 text(255), mark_guid_22 text(255), mark_guid_23 text(255), mark_guid_24 text(255), mark_guid_25 text(255), mark_guid_26 text(255), mark_guid_27 text(255), mark_guid_28 text(255), mark_guid_29 text(255), mark_guid_30 text(255), mark_guid_31 text(255), " +
                    "mark_firstime_1 text(255), mark_firstime_2 text(255), mark_firstime_3 text(255), mark_firstime_4 text(255), mark_firstime_5 text(255), mark_firstime_6 text(255), mark_firstime_7 text(255), " +
                    "mark_firstime_8 text(255), mark_firstime_9 text(255), mark_firstime_10 text(255), mark_firstime_11 text(255), mark_firstime_12 text(255), mark_firstime_13 text(255), mark_firstime_14 text(255), " +
                    "mark_firstime_15 text(255), mark_firstime_16 text(255), mark_firstime_17 text(255), mark_firstime_18 text(255), mark_firstime_19 text(255), mark_firstime_20 text(255), mark_firstime_21 text(255), " +
                    "mark_firstime_22 text(255), mark_firstime_23 text(255), mark_firstime_24 text(255), mark_firstime_25 text(255), mark_firstime_26 text(255), mark_firstime_27 text(255), mark_firstime_28 text(255), " +
                    "mark_firstime_29 text(255), mark_firstime_30 text(255), mark_firstime_31 text(255), mark_lasttime_1 text(255), mark_lasttime_2 text(255), mark_lasttime_3 text(255), mark_lasttime_4 text(255), " +
                    "mark_lasttime_5 text(255), mark_lasttime_6 text(255), mark_lasttime_7 text(255), mark_lasttime_8 text(255), mark_lasttime_9 text(255), mark_lasttime_10 text(255), mark_lasttime_11 text(255), " +
                    "mark_lasttime_12 text(255), mark_lasttime_13 text(255), mark_lasttime_14 text(255), mark_lasttime_15 text(255), mark_lasttime_16 text(255), mark_lasttime_17 text(255), mark_lasttime_18 text(255), " +
                    "mark_lasttime_19 text(255), mark_lasttime_20 text(255), mark_lasttime_21 text(255), mark_lasttime_22 text(255), mark_lasttime_23 text(255), mark_lasttime_24 text(255), mark_lasttime_25 text(255), " +
                    "mark_lasttime_26 text(255), mark_lasttime_27 text(255), mark_lasttime_28 text(255), mark_lasttime_29 text(255), mark_lasttime_30 text(255), mark_lasttime_31 text(255), " +
                    "mark_work_1 text(255), mark_work_2 text(255), mark_work_3 text(255), mark_work_4 text(255), mark_work_5 text(255), mark_work_6 text(255), mark_work_7 text(255), mark_work_8 text(255), mark_work_9 text(255), " +
                    "mark_work_10 text(255), mark_work_11 text(255), mark_work_12 text(255), mark_work_13 text(255), mark_work_14 text(255), mark_work_15 text(255), mark_work_16 text(255), mark_work_17 text(255), mark_work_18 text(255), " +
                    "mark_work_19 text(255), mark_work_20 text(255), mark_work_21 text(255), mark_work_22 text(255), mark_work_23 text(255), mark_work_24 text(255), mark_work_25 text(255), mark_work_26 text(255), mark_work_27 text(255), " +
                    "mark_work_28 text(255), mark_work_29 text(255), mark_work_30 text(255), mark_work_31 text(255), mark_guidsmena_1 text(255), mark_guidsmena_2 text(255), mark_guidsmena_3 text(255), mark_guidsmena_4 text(255), " +
                    "mark_guidsmena_5 text(255), mark_guidsmena_6 text(255), mark_guidsmena_7 text(255), mark_guidsmena_8 text(255), mark_guidsmena_9 text(255), mark_guidsmena_10 text(255), mark_guidsmena_11 text(255), mark_guidsmena_12 text(255), " +
                    "mark_guidsmena_13 text(255), mark_guidsmena_14 text(255), mark_guidsmena_15 text(255), mark_guidsmena_16 text(255), mark_guidsmena_17 text(255), mark_guidsmena_18 text(255), mark_guidsmena_19 text(255), mark_guidsmena_20 text(255), " +
                    "mark_guidsmena_21 text(255), mark_guidsmena_22 text(255), mark_guidsmena_23 text(255), mark_guidsmena_24 text(255), mark_guidsmena_25 text(255), mark_guidsmena_26 text(255), mark_guidsmena_27 text(255), mark_guidsmena_28 text(255), " +
                    "mark_guidsmena_29 text(255), mark_guidsmena_30 text(255), mark_guidsmena_31 text(255))";
                    _command.ExecuteNonQuery();

                    //t_WorkTeremok
                    _command.CommandText = "CREATE TABLE t_WorkTeremok(id identity unique NOT NULL, teremok_id text(255), teremok_day text(255), teremok_month text(255), teremok_year text(255), teremok_firstTime datetime, teremok_lastTime datetime)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "ALTER TABLE t_MarkItems ADD COLUMN mark_res text(255)";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "CREATE TABLE t_ShiftType(id identity unique NOT NULL, type_guid text(255), type_name text(255), type_value text(255), type_color text(255))";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "ALTER TABLE t_Employee ADD COLUMN employee_WorkField YESNO";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "UPDATE t_Employee SET employee_WorkField = TRUE";
                    _command.ExecuteNonQuery();
                    _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.20' WHERE conf_id=1";
                    _command.ExecuteNonQuery();
                }
                catch (Exception exp)
                {
                    MDIParentMain.log.Error("Не удалось обновиться до 4 0 0 17 ", exp);
                }
                finally
                {
                    if (_conn != null)
                        _conn.Close();
                }
            }
        }

        int updation_Priority = 999;
        public void SetVersion(string version)
        {
            if (ExecuteCommand("UPDATE t_Conf SET conf_value='" + version + "' WHERE conf_id=1"))
            {
                MDIParentMain.Log("version " + version + " Updated");

#if(!DEB)
                StaticConstants.MainWindow.WebTaskSheduler.EnqeueTask("WebServiceSystem", 1, "SetTeremokVersion",
                               new object[] { StaticConstants.Main_Teremok_1cName, version},
                               updation_Priority);
#endif
            }
        }

        private double GetVersion(string str_version)
        {
            string[] versarr = str_version.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            double result = 0;
            for (int i = versarr.Length - 1; i >= 0; i--)
            {
                result += double.Parse(versarr[i]) * Math.Pow(100, (versarr.Length - i));
            }
            return result;
        }

        /// <summary>
        /// Проверка если версия version новее текущей
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool IsNewerVersion(string version)
        {
            DataTable dt = ExecuteCommandTable("SELECT * FROM t_Conf WHERE conf_id=1;");
            string oldVersion = CellHelper.FindCell(dt.Rows[0], "conf_value").ToString();

            double versionNew_int = GetVersion(version);
            double versionOld_int = GetVersion(oldVersion);

            return versionNew_int > versionOld_int;
        }

        /// <summary>
        /// Проверка если версия version равна
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool IsVersion(string version)
        {
            DataTable dt = ExecuteCommandTable("SELECT * FROM t_Conf WHERE conf_id=1;");
            string oldVersion = CellHelper.FindCell(dt.Rows[0], "conf_value").ToString();

            double versionNew_int = GetVersion(version);
            double versionOld_int = GetVersion(oldVersion);

            return versionNew_int == versionOld_int;
        }

        /// <summary>
        /// создать таблицы t_Utilization  и t_UtilizationRef
        /// </summary>
        public void Update_5_3_0_0()
        {
            if (IsNewerVersion("5.3.0.0"))
            {
                SetVersion("5.3.0.0");  //обновление версии
                #region обновление по справочникам

                ExecuteCommand("DROP TABLE t_Utilization;");      // удаление внешних ключей если t_Utilization есть

                ExecuteCommand("CREATE TABLE t_Utilization (util_id AutoIncrement unique NOT NULL, util_nome_id integer,util_quantity integer,util_doc_id integer,util_reason_id integer,"+
                    "util_ed text(255),util_bed text(255),util_K integer);");      // добавление t_Utilization

                ExecuteCommand("CREATE TABLE t_UtilReasonRef (urr_id AutoIncrement unique NOT NULL, urr_name text(255),doc_type text(255),id_1c text(255),nome_include text(255));");      // добавление t_UtilReasonRef
                    
                
                #endregion
            }
        }

        public void Update_5_4_0_0()
        {

            if (IsNewerVersion("5.4.0.0"))
            {
                SetVersion("5.4.0.0");  //обновление версии
                #region обновление по справочникам

                ExecuteCommand("DELETE urr_name, urr_id FROM t_UtilReasonRef");          //очищение старых записей

                ExecuteCommand("ALTER TABLE t_UtilReasonRef ADD COLUMN nome_include text(255)");
                ExecuteCommand("ALTER TABLE t_UtilReasonRef ADD COLUMN doc_type text(255)");      //обновление справочника списаний   
                ExecuteCommand("ALTER TABLE t_UtilReasonRef ADD COLUMN id_1c text(40)");         //обновление справочника списаний

                ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_id,doctype_name) VALUES(29,'Заказ. Кондитерка.')");      // добавление нового вида документа (заказ-кондитерка)
                ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_id,doctype_name) VALUES(31,'Справочник. Причины списания ГП')");      // добавление нового вида документа
                ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_id,doctype_name) VALUES(32,'Справочник. Причины списания СП')");      // добавление нового вида документа

                ExecuteCommand("INSERT INTO t_DocStatusRef(docstatusref_name,doctype_id,statustype_id) VALUES('Получен',31,4)"); //добавление статуса получен для справочника 31
                ExecuteCommand("INSERT INTO t_DocStatusRef(docstatusref_name,doctype_id,statustype_id) VALUES('Получен',32,4)"); //добавление статуса получен для справочника 32
                #endregion
            }
            
        }

        public void Update_5_4_0_1()
        {
            if (IsNewerVersion("5.4.0.1"))
            {
                SetVersion("5.4.0.1");
                ExecuteCommand("UPDATE t_DocTypeRef SET doctype_name =  'Заказ. Кондитерка.' WHERE doctype_id=29");
                ExecuteCommand("INSERT INTO t_NomeTemplate ( nt_id, nt_desc ) VALUES (29,'Заказ.Кондитерка.')");

                ExecuteCommand("INSERT INTO t_DocStatusRef ( docstatusref_name, doctype_id,statustype_id ) VALUES ('новый',29,1);");
                ExecuteCommand("INSERT INTO t_DocStatusRef ( docstatusref_name, doctype_id,statustype_id ) VALUES ('отправлен',29,3);");
                ExecuteCommand("INSERT INTO t_DocStatusRef ( docstatusref_name, doctype_id,statustype_id ) VALUES ('отправляется',29,2);");

                ExecuteCommand("INSERT INTO t_SettingsDoc (colunm_doc_id,column_id,column_status,column_name) VALUES (29,5,-1,'Ед.Измер');");
                ExecuteCommand("INSERT INTO t_SettingsDoc (colunm_doc_id,column_id,column_status,column_name) VALUES (29,15,-1,'Ед.');");
                ExecuteCommand("INSERT INTO t_SettingsDoc (colunm_doc_id,column_id,column_status,column_name) VALUES (29,16,0,'Коэф.');");
                ExecuteCommand("INSERT INTO t_SettingsDoc (colunm_doc_id,column_id,column_status,column_name) VALUES (29,100,0,'Информация о номенклатуре');");
                #region trash
                //       DataTable dt=ExecuteCommandTable("SELECT t_Conf.conf_value, t_Conf.conf_id FROM t_Conf WHERE (((t_Conf.conf_id)=1));");
                //     string curr_version=CellHelper.FindCell(dt, 0, "conf_value").ToString();
                #endregion
            }
        }

        public void Update_5_4_0_3()
        {

            if (IsNewerVersion("5.4.0.3"))
            {
                SetVersion("5.4.0.3");  //обновление версии
                #region обновление по справочникам
                
                //добавление в t_conf строки password
                ExecuteCommand("INSERT INTO t_Conf(conf_param,conf_value) VALUES('password','123')");      // добавление нового вида документа
            }
                #endregion
        }

        public void Update_5_4_4_0()
        {

            if (IsNewerVersion("5.4.4.0"))
            {
                SetVersion("5.4.4.0");  //обновление версии
                #region обновление по справочникам

                //добавление в t_conf строки password
                ExecuteCommand("CREATE TABLE t_Table_Service(id AutoIncrement unique NOT NULL, doc_id integer, doc_period text(255));");      // добавление нового вида документа
            }
                #endregion
        }

        /// <summary>
        /// добавление возможности указывать ссылку для вебсервиса
        /// </summary>
        public void Update_5_4_4_3()
        {

            if (IsNewerVersion("5.4.4.3"))
            {
                SetVersion("5.4.4.3");  //обновление версии
                #region обновление по справочникам

                ExecuteCommand("INSERT INTO t_Conf(conf_param,conf_value) VALUES('web_service_url','')");      // добавление ссылки на вебсервис
                
                #endregion
            }

        }

        /// <summary>
        /// добавляем поддержку форматированного ввода счетчика электричества
        /// </summary>
        public void Update_5_4_5_0()
        {

            if (IsNewerVersion("5.4.5.0"))
            {
                SetVersion("5.4.5.0");  //обновление версии
                
                //добавление нового вида справочника
                //ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_name) VALUES(33,'Справочник. Счетчик эл.')");      // добавление нового вида документа

                //ExecuteCommand("INSERT INTO t_DocStatusRef(docstatusref_name,doctype_id,statustype_id) VALUES('Получен',33,4)"); //добавление статуса получен для справочника 31

                ExecuteCommand("CREATE TABLE t_Counts_Service(id AutoIncrement unique NOT NULL, doc_type_id integer, count_length integer);");      // добавление таблицы для учета справочника
            }
                
        }

        /// <summary>
        /// добавляем поддержку добавления обновления прикассовых мониторов
        /// </summary>
        public void Update_5_4_6_0()
        {

            if (IsNewerVersion("5.4.6.0"))
            {
                SetVersion("5.4.6.0");  //обновление версии

                ExecuteCommand(@"INSERT INTO t_Conf(conf_param,conf_value) VALUES('KashevarConfig','Configs\RBClientKashConfig.xml')");      // добавление ссылки на папку видео кассы 1
            }

        }


        /// <summary>
        /// убираем связи с t_utilization
        /// </summary>
        public void Update_5_4_7_0()
        {

            if (IsNewerVersion("5.4.7.0"))
            {
                SetVersion("5.4.7.0");  //обновление версии

                ExecuteCommand("DROP TABLE t_Utilization;");

                ExecuteCommand("CREATE TABLE t_Utilization (util_id AutoIncrement unique NOT NULL, util_nome_id integer,util_quantity integer,util_doc_id integer,util_reason_id integer," +
                "util_ed text(255),util_bed text(255),util_K integer);");
                //ExecuteCommand("DROP TABLE t_UtilReasonRef;");

                //ExecuteCommand("CREATE TABLE t_UtilReasonRef (urr_id AutoIncrement unique NOT NULL, urr_name text(255),doc_type text(255),id_1c text(255),nome_include text(255));");      // добавление t_UtilReasonRef
                //ExecuteCommand("ALTER TABLE t_UtilReasonRef ADD COLUMN nome_include text(255)");
                //ExecuteCommand("ALTER TABLE t_UtilReasonRef ADD COLUMN doc_type text(255)");      //обновление справочника списаний   
                //ExecuteCommand("ALTER TABLE t_UtilReasonRef ADD COLUMN id_1c text(40)");  
            }

        }

        /// <summary>
        /// добавляем поля для t_Count_service
        /// </summary>
        public void Update_5_4_8_0()
        {

            if (IsNewerVersion("5.4.8.0"))
            {
                SetVersion("5.4.8.0");  //обновление версии

                ExecuteCommand("ALTER TABLE t_Counts_Service ADD COLUMN nome_1C text(255)");
                ExecuteCommand("ALTER TABLE t_Counts_Service ADD COLUMN count_name text(255)");
                ExecuteCommand("ALTER TABLE t_Counts_Service ADD COLUMN count_mask text(255)");
                ExecuteCommand("ALTER TABLE t_Counts_Service ADD COLUMN count_koeff integer");

                //обновление справочника списаний   
                //integer text(255
            }

        }


        /// <summary>
        /// добавляем тип документа реклама на мониторах и статусы для данного документа
        /// </summary>
        public void Update_5_4_9_0()
        {

            if (IsNewerVersion("5.4.9.0"))
            {
                SetVersion("5.4.9.0");  //обновление версии

                ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_name,doctype_id) VALUES('реклама на мониторах',30)");
                ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_name,doctype_id) VALUES('реклама на мониторах',34)");
                ExecuteCommand("UPDATE t_DocTypeRef SET doctype_name =  'реклама на мониторах' WHERE doctype_id=30");
                ExecuteCommand("INSERT INTO t_NomeTemplate ( nt_id, nt_desc ) VALUES (30,'реклама на мониторах')");
                ExecuteCommand("INSERT INTO t_DocStatusRef ( docstatusref_name, doctype_id,statustype_id ) VALUES ('получен',34,1);");
            }

        }

        /// <summary>
        /// добавляем тип документа изображения на мониторах и статусы для данного документа
        /// </summary>
        public void Update_5_5_0_0()
        {

            if (IsNewerVersion("5.5.0.0"))
            {
                SetVersion("5.5.0.0");  //обновление версии

                ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_name,doctype_id) VALUES('изображения на мониторах',35)");
                ExecuteCommand("INSERT INTO t_NomeTemplate ( nt_id, nt_desc ) VALUES (35,'реклама на мониторах')");
                ExecuteCommand("INSERT INTO t_DocStatusRef ( docstatusref_name, doctype_id,statustype_id ) VALUES ('получен',35,1);");
            }

        }

        /// <summary>
        /// добавляем тип документа обучающее видео
        /// </summary>
        public void Update_5_5_1_0()
        {

            if (IsNewerVersion("5.5.1.0"))
            {
                SetVersion("5.5.1.0");  //обновление версии

                ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_name,doctype_id) VALUES('обучающее видео',36)");
                ExecuteCommand("INSERT INTO t_NomeTemplate ( nt_id, nt_desc ) VALUES (36,'обучающее видео')");
                ExecuteCommand("INSERT INTO t_DocStatusRef ( docstatusref_name, doctype_id,statustype_id ) VALUES ('получен',36,1);");
            }

        }

        /// <summary>
        /// добавляем тип документа Смена и обеды
        /// </summary>
        public void Update_5_5_2_0()
        {
            if (IsNewerVersion("5.5.2.0"))
            {
                SetVersion("5.5.2.0");  //обновление версии

                ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_name,doctype_id) VALUES('марочный отчет',37)");
                
                ExecuteCommand("INSERT INTO t_DocStatusRef ( docstatusref_name, doctype_id,statustype_id ) VALUES ('новый',37,1);");
                ExecuteCommand("INSERT INTO t_DocStatusRef ( docstatusref_name, doctype_id,statustype_id ) VALUES ('отправлен',37,3);");
                ExecuteCommand("INSERT INTO t_DocStatusRef ( docstatusref_name, doctype_id,statustype_id ) VALUES ('отправляется',37,2);");
            }
        }


        /// <summary>
        /// Добавляем таблицы обедов и марочных отчетов
        /// </summary>
        public void Update_5_5_3_0()
        {
            if (IsNewerVersion("5.5.3.0"))
            {
                SetVersion("5.5.3.0");  //обновление версии

                ExecuteCommand("drop table t_dinner;");
                ExecuteCommand("drop table t_Marotch;");

                ExecuteCommand("ALTER TABLE t_CheckItem DROP CONSTRAINT t_Menut_CheckItem;");
                ExecuteCommand("ALTER TABLE t_MenuPrice DROP CONSTRAINT t_Menut_MenuPrice;");
                ExecuteCommand("ALTER TABLE t_ShipOrder DROP CONSTRAINT t_Menut_ShipOrder;");

                ExecuteCommand("drop table t_Menu;");

                ExecuteCommand("CREATE TABLE t_Dinner (id AutoIncrement unique NOT NULL,doc_id integer, nome_name text(255),nome_id integer,kkm_code text(255),employee_id integer,emploee_name text(255),d_sum numeric,d_date datetime);");
                ExecuteCommand("CREATE TABLE t_Marotch (id AutoIncrement unique NOT NULL,doc_id integer, employee_id integer, responce_guid_plan text(255), responce_guid_fact text(255),m_plan text(255),m_fact text(255),m_date datetime);");
                ExecuteCommand("CREATE TABLE t_Menu (menu_id AutoIncrement unique NOT NULL,menu_nome_1C text(255), menu_nome text(255), menu_default logical, price decimal (18,2));");
            }

        }

        /// <summary>
        /// Досоздаем таблицы марочных отчетов
        /// </summary>
        public void Update_5_5_4_0()
        {
            if (IsNewerVersion("5.5.4.0"))
            {
                SetVersion("5.5.4.0");  //обновление версии
                ExecuteCommand("CREATE TABLE t_Dinner2t_Menu (Id AutoIncrement unique NOT NULL,Dinner_id integer,Menu_item_id integer,item_quantity integer, item_price decimal (18,2));");
                ExecuteCommand("INSERT INTO t_Conf(conf_param, conf_value) VALUES('validation_by_cert','0')");
            }
        }

        /// <summary>
        /// Добавляем параметр для 
        /// </summary>
        public void Update_5_5_5_0()
        {
            if (IsNewerVersion("5.5.5.0"))
            {
                SetVersion("5.5.5.0");  //обновление версии

                //ExecuteCommand("INSERT INTO t_Conf(conf_param, conf_value) VALUES('validation_by_cert','0')");
                ExecuteCommand("UPDATE t_Conf SET conf_value='1' WHERE conf_id=23");

                //if (CParam.AppCity !=1)
                //{
                //    ExecuteCommand("UPDATE t_Conf SET conf_value='1' WHERE conf_param='validation_by_cert'");
                //}
            }
        }

        /// <summary>
        /// Добавляем параметр для обязанности
        /// </summary>
        public void Update_5_5_6_0()
        {
            if (IsNewerVersion("5.5.6.0"))
            {
                SetVersion("5.5.6.0");  //обновление версии

                ExecuteCommand("ALTER TABLE t_Marotch ADD COLUMN response text(255)");

                //if (CParam.AppCity !=1)
                //{
                //    ExecuteCommand("UPDATE t_Conf SET conf_value='1' WHERE conf_param='validation_by_cert'");
                //}
            }
        }


        /// <summary>
        /// Добавляем параметр для комментария
        /// </summary>
        public void Update_5_5_7_0()
        {
            if (IsNewerVersion("5.5.7.0"))
            {
                SetVersion("5.5.7.0");  //обновление версии

                ExecuteCommand("ALTER TABLE t_Marotch ADD COLUMN comment longtext");
            }
        }

        /// <summary>
        /// Добавляем параметр для комментария
        /// </summary>
        public void Update_5_5_7_1()
        {
            if (IsNewerVersion("5.5.7.1"))
            {
                SetVersion("5.5.7.1");  //обновление версии
                ExecuteCommand("UPDATE t_Conf SET conf_value='RBClientKashConfig.xml' WHERE conf_param='KashevarConfig'");
            }
        }

        /// <summary>
        /// Добавляем параметр для вебсервиса питера + параметр для максимального размера базы данных
        /// </summary>
        public void Update_5_5_8_0()
        {
            if (IsNewerVersion("5.5.8.0"))
            {
                SetVersion("5.5.8.0");  //обновление версии
                if (CParam.AppCity == 1) //питер вебсервис
                {
                    ExecuteCommand("UPDATE t_Conf SET conf_value='https://spb.teremok.ru:63036/ARMWeb/ws/ARMWeb/' WHERE conf_param='web_service_url'");
                }
                ExecuteCommand("INSERT INTO t_Conf(conf_param, conf_value) VALUES('max_database_size_mb','20')");
            }
        }

        /// <summary>
        /// добавляем тип документа PosDisplay
        /// </summary>
        public void Update_5_5_9_0()
        {

            if (IsNewerVersion("5.5.9.0"))
            {
                SetVersion("5.5.9.0");  //обновление версии
                ExecuteCommand("INSERT INTO t_DocTypeRef(doctype_name,doctype_id) VALUES('Программа PosDisplay',38)");
                ExecuteCommand("INSERT INTO t_DocStatusRef ( docstatusref_name, doctype_id,statustype_id ) VALUES ('получена',38,4);");
            }
        }

         /// <summary>
        /// Добавляем чтение конфига для таймера t-report
        /// и внесение имени номенклатуры и кратности для питера
        /// </summary>
        public void Update_5_6_0_0()
        {

            if (IsNewerVersion("5.6.0.0"))
            {
                SetVersion("5.6.0.0");  //обновление версии
                ExecuteCommand("INSERT INTO t_Conf(conf_param, conf_value) VALUES('t_report_interval_ms','3600000')");
                ExecuteCommand("UPDATE t_Conf SET conf_value='null' WHERE conf_param='web_service_url'");


            }
        }

        /// <summary>
        /// Внесение ручника на вебсервис
        /// </summary>
        public void Update_5_6_1_1()
        {

            if (IsNewerVersion("5.6.1.1"))
            {
                SetVersion("5.6.1.1");  //обновление версии
                ExecuteCommand("UPDATE t_Conf SET conf_value='null' WHERE conf_param='web_service_url'");
            }
        }

        /// <summary>
        /// Добавляем новую таблицу для свойств-значений
        /// </summary>
        public void Update_5_6_2_0()
        {

            if (IsNewerVersion("5.6.2.0"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand("CREATE TABLE t_PropValue (Id AutoIncrement unique NOT NULL,Prop_name text(255) NOT NULL,Prop_value text(255)," +
                "Prop_type text(255));");

                flag = flag && ExecuteCommand("ALTER TABLE t_Responsibility ADD COLUMN Deleted YESNO");
                flag = flag && ExecuteCommand("ALTER TABLE t_ShiftType ADD COLUMN Deleted YESNO");
                flag = flag && ExecuteCommand("ALTER TABLE t_Teremok ADD COLUMN Deleted YESNO");
                flag = flag && ExecuteCommand("ALTER TABLE t_Employee ADD COLUMN Deleted YESNO");
                flag = flag && ExecuteCommand("ALTER TABLE t_ButtonTemplate ADD COLUMN Deleted YESNO");

                if (flag)
                {
                    SetVersion("5.6.2.0");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Переименование марочный отчет в смену и обеды
        /// добавление в конфиги количество бэкапов
        /// </summary>
        public void Update_5_6_3_0()
        {
            if (IsNewerVersion("5.6.3.0"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand("UPDATE t_DocTypeRef SET doctype_name='Смена и обеды' WHERE doctype_id=37");
                flag = flag && ExecuteCommand("INSERT INTO t_Conf(conf_param, conf_value) VALUES('db_backups_count','3')");

                if (flag)
                {
                    SetVersion("5.6.3.0");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Добавление в таблицу списаний индекса записи в документе
        /// Для 2 одинаковых номенклатур с разными причинами списаний
        /// </summary>
        public void Update_5_6_4_0()
        {
            if (IsNewerVersion("5.6.4.0"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand("ALTER TABLE t_Utilization ADD COLUMN util_opd_id integer");

                if (flag)
                {
                    SetVersion("5.6.4.0");  //обновление версии
                }
            }
        }


        /// <summary>
        /// Добавляем типы отправки
        /// изменяем таблицу типов документов
        /// </summary>
        public void Update_5_6_4_1()
        {
            if (IsNewerVersion("5.6.4.1"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand("CREATE TABLE t_DocSendTypeRef (id AutoIncrement unique NOT NULL,sendtype_type integer NOT NULL,sendtype_description text(255));");

                flag = flag && ExecuteCommand("INSERT INTO t_DocSendTypeRef(id,sendtype_type, sendtype_description) VALUES(1,0,'not send')");
                flag = flag && ExecuteCommand("INSERT INTO t_DocSendTypeRef(id,sendtype_type, sendtype_description) VALUES(2,1,'send over xml')");
                flag = flag && ExecuteCommand("INSERT INTO t_DocSendTypeRef(id,sendtype_type, sendtype_description) VALUES(3,2,'send over webservice')");
                flag = flag && ExecuteCommand("ALTER TABLE t_DocTypeRef ADD COLUMN sendtype_type integer");

                if (flag)
                {
                    SetVersion("5.6.4.1");  //обновление версии
                }
            }
        }


        /// <summary>
        /// Добавляем таблицу обмена по вебсервису
        /// </summary>
        public void Update_5_6_4_3()
        {
            if (IsNewerVersion("5.6.4.3"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand("CREATE TABLE t_TaskExchangeWeb (id AutoIncrement unique NOT NULL,teremok_id integer NOT NULL,doc_id integer NOT NULL,doc_content OleObject);");

                if (flag)
                {
                    SetVersion("5.6.4.3");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Добавляем таблицу уведомлений по шаблону и уведомлений общих
        /// </summary>
        public void Update_5_6_4_4()
        {
            if (IsNewerVersion("5.6.4.4"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand("CREATE TABLE t_InfoMessage (id AutoIncrement unique NOT NULL"+
                    ",dayofweek integer,dayofmounth integer,timefrom datetime,timeto datetime,doc_type_id integer, type integer" +
                    ",department text(255),doc_id integer,priority integer DEFAULT 0,lastmonthday YESNO, description memo,message memo);");

                flag = flag &&
                    ExecuteCommand("ALTER TABLE t_InfoMessage ADD COLUMN i_guid text(255)");

                if (flag)
                {
                    SetVersion("5.6.4.4");  //обновление версии
                }
            }
        }


        /// <summary>
        /// Изменяем таблицу документа - добавляем хэш код
        /// </summary>
        public void Update_5_6_4_6()
        {
            if (IsNewerVersion("5.6.4.6"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand("ALTER TABLE t_Doc ADD COLUMN doc_hash text(255)");

                if (flag)
                {
                    SetVersion("5.6.4.6");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Изменяем конфигурацию - добавляем параметр репозитория для обучающего видео
        /// </summary>
        public void Update_5_6_4_8()
        {
            if (IsNewerVersion("5.6.4.8"))
            {
                bool flag = true;
                flag = flag &&
                    ExecuteCommand("INSERT INTO t_Conf(conf_param, conf_value) VALUES('education_video_repository','_repository')");
                if (flag)
                {
                    SetVersion("5.6.4.8");  //обновление версии
                }
            }
        }

        
        public void Update_5_6_4_9()
        {
            if (IsNewerVersion("5.6.4.9"))
            {
                    SetVersion("5.6.4.9");  //обновление версии
            }
        }

        
        public void Update_5_6_5_0()
        {
            if (IsNewerVersion("5.6.5.0"))
            {
                SetVersion("5.6.5.0");  //обновление версии
            }
        }

        /// <summary>
        /// Изменяем таблицу документа - добавляем хэш код
        /// </summary>
        public void Update_5_6_5_1()
        {
            if (IsNewerVersion("5.6.5.1"))
            {
                bool flag = true;
                flag = flag &&
                    ExecuteCommand("INSERT INTO t_Conf(conf_param, conf_value) VALUES('web_service_exchange_timeout','600')");
                
                
                    ExecuteCommand("ALTER TABLE t_InfoMessage ADD COLUMN i_guid text(255)");

                flag = flag &&
                    ExecuteCommand("ALTER TABLE t_ZReport ADD COLUMN zreport_sended YESNO");

                if (flag)
                {

                    SetVersion("5.6.5.1");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Изменяем функции получения обучающего видео на вебсервисе
        /// </summary>
        public void Update_5_6_5_2()
        {
            if (IsNewerVersion("5.6.5.2"))
            {
                    SetVersion("5.6.5.2");  //обновление версии
            }
        }

        /// <summary>
        /// Добавляем таблицу по кассам
        /// </summary>
        public void Update_5_6_5_3()
        {
            if (IsNewerVersion("5.6.5.3"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand("CREATE TABLE t_Kkm (id AutoIncrement unique NOT NULL" +
                    ",in_folder memo,out_folder memo,online YESNO,lasttime_online datetime, " +
                "kkm_name text(255),last_treport text(255));");
                     
                if (flag)
                {
                    SetVersion("5.6.5.3");  //обновление версии
                }
            }
        }


        /// <summary>
        /// изменяем таблицу марочного отчета
        /// </summary>
        public void Update_5_6_5_5()
        {
            if (IsVersion("5.6.5.4"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand("ALTER TABLE t_Marotch ADD COLUMN fact_work_from text(255),fact_work_to text(255);");

                if (flag)
                {
                    SetVersion("5.6.5.5");  //обновление версии
                }
            }
        }

        /// <summary>
        /// добавляем таблицы инкассации и справочника вид инкассаций
        /// </summary>
        public void Update_5_6_5_6()
        {
            if (IsVersion("5.6.5.5"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand("CREATE TABLE t_Inkass (id AutoIncrement unique NOT NULL" +
                   ",ink_name text(255),ink_doc_id integer,sum1 decimal(18,2),sum2 decimal(18,2),sum3 decimal(18,2),ink_type text(36),ink_currency text(20),istemplate YESNO);");

                flag = flag && ExecuteCommand("CREATE TABLE t_Inkass_TypeRef (id AutoIncrement unique NOT NULL" +
                   ",ri_guid text(36),ri_name text(255),ri_order integer,deleted YESNO);");

                flag = flag && ExecuteCommand("INSERT INTO t_Inkass(ink_name, ink_type,ink_currency,istemplate) VALUES('Инкассация',1,'руб.',-1)");

                flag = flag && ExecuteCommand("INSERT INTO t_Inkass_TypeRef(ri_guid, ri_name,ri_order) VALUES(1,'внутренняя',1)");
                flag = flag && ExecuteCommand("INSERT INTO t_Inkass_TypeRef(ri_guid, ri_name,ri_order) VALUES(2,'через банк',2)");
                flag = flag && ExecuteCommand("INSERT INTO t_Inkass_TypeRef(ri_guid, ri_name,ri_order) VALUES(3,'через банкомат',3)");

                if (flag)
                {
                    SetVersion("5.6.5.6");  //обновление версии
                }
            }
        }

        /// <summary>
        /// изменяем таблицы t_kkm
        /// </summary>
        public void Update_5_6_5_7()
        {
            if (IsVersion("5.6.5.6"))
            {
                bool flag = true;
                flag = flag && ExecuteCommand
                    ("ALTER TABLE t_Kkm ADD COLUMN last_check_num text(50)"+
                    ",last_check_datetime datetime,workedToDay YESNO,last_zreport text(255);");

                if (flag)
                {
                    SetVersion("5.6.5.7");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Добавить таблицу t_Shifts_Allowed
        /// </summary>
        public void Update_5_6_5_8()
        {
            if (IsVersion("5.6.5.7"))
            {
                bool flag = true;

                flag = flag && ExecuteCommand("CREATE TABLE t_Shifts_Allowed (id AutoIncrement unique NOT NULL" +
                   ",res_guid text(36),shift_guid text(36));");

                if (flag)
                {
                    SetVersion("5.6.5.8");  //обновление версии
                }
            }
        }


        /// <summary>
        /// Зафиксировать обновления exe
        /// </summary>
        public void Update_5_6_5_9()
        {
            if (IsVersion("5.6.5.8"))
            {
                SetVersion("5.6.5.9");  //обновление версии             
            }
        }

        /// <summary>
        /// Добавление полей в Additional в t_propvalue
        /// </summary>
        public void Update_5_6_6_0()
        {
            if (IsVersion("5.6.5.9"))
            {
                bool flag = true;

                flag = flag && ExecuteCommand("ALTER TABLE t_PropValue ADD COLUMN add_prop1 text(255)" +
                    ",add_prop2 text(255);");

                if (flag)
                {
                    SetVersion("5.6.6.0");  //обновление версии
                }
            }
        }


        /// <summary>
        /// Добавление поля коментариев в t_inkass
        /// </summary>
        public void Update_5_6_6_1()
        {
            if (IsVersion("5.6.6.0"))
            {
                bool flag = true;

                flag = flag && ExecuteCommand("ALTER TABLE t_Inkass ADD COLUMN comment text(255);");

                if (flag)
                {
                    SetVersion("5.6.6.1");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Изменение ехе
        /// </summary>
        public void Update_5_6_6_2()
        {
            if (IsVersion("5.6.6.1"))
            {
                bool flag = true;

                if (flag)
                {
                    SetVersion("5.6.6.2");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Изменение ехе
        /// </summary>
        public void Update_5_6_6_21()
        {
            if (IsVersion("5.6.6.2"))
            {
                bool flag = true;

                if (flag)
                {
                    SetVersion("5.6.6.21");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Изменение ехе
        /// </summary>
        public void Update_5_6_6_22()
        {
            if (IsVersion("5.6.6.21"))
            {
                bool flag = true;

                if (flag)
                {
                    SetVersion("5.6.6.22");  //обновление версии
                }
            }
        }
        /// <summary>
        /// Изменение ехе
        /// </summary>
        public void Update_5_6_6_23()
        {
            if (IsVersion("5.6.6.22"))
            {
                bool flag = true;

                if (flag)
                {
                    SetVersion("5.6.6.23");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Добавление выгрузки т и з со сменами + выгрузка логирования кассы на вебсервис
        /// </summary>
        public void Update_5_6_6_24()
        {
            if (IsVersion("5.6.6.23"))
            {
                bool flag = true;

                if (flag)
                {
                    SetVersion("5.6.6.24");  //обновление версии
                }
            }
        }

        /// <summary>
        /// Добавление усиленного парсинга z-отчета - добавление проверки перед обменом
        /// </summary>
        public void Update_5_6_6_25()
        {
            if (IsVersion("5.6.6.24"))
            {
                bool flag = true;

                if (flag)
                {
                    SetVersion("5.6.6.25");  //обновление версии
                }
            }
        }


        /// <summary>
        /// Добавление усиленного парсинга z-отчета - добавление проверки перед обменом
        /// </summary>
        public void Update_5_6_6_26()
        {
            if (IsVersion("5.6.6.25"))
            {
                bool flag = true;

                if (flag)
                {
                    SetVersion("5.6.6.26");  //обновление версии
                }
            }
        }


        /// <summary>
        /// Добавил логирование 2х дней
        /// </summary>
        public void Update_5_6_6_27()
        {
            if (IsVersion("5.6.6.26"))
            {
                bool flag = true;

                if (flag)
                {
                    SetVersion("5.6.6.27");  //обновление версии
                }
            }
        }


        /// <summary>
        /// Переход на 4й фреймворк
        /// </summary>
        public void Update_5_6_6_28()
        {
            if (IsVersion("5.6.6.27"))
            {
                bool flag = true;

                if (flag)
                {
                    SetVersion("5.6.6.28");  //обновление версии
                }
            }
        }


        /// <summary>
        /// Создается таблица для отложенных заданий
        /// </summary>
        public void Update_5_6_6_29()
        {
            if (IsVersion("5.6.6.28"))
            {
                bool flag = true;

                flag = flag && ExecuteCommand("CREATE TABLE t_WebServiceTask (id AutoIncrement unique NOT NULL" +
                   ",taskdate datetime,taskupdatedate datetime,funcname text(255),taskparams OleObject,succed YESNO,failed YESNO,description memo, " +
                   "priority integer,servicename text(255),servicepriority integer,triescount integer);");

                if (flag)
                {
                    SetVersion("5.6.6.29");  //обновление версии
                }
            }
        }

        public void Update_x_x_x_x(string old_version,string new_version,List<string> commands)
        {
            if (IsVersion(old_version))
            {
                bool flag = true;

                if (commands.NotNullOrEmpty())
                {
                    foreach (var command in commands)
                    {
                        flag = flag && ExecuteCommand(command);
                    }
                }

                if (flag)
                {
                    SetVersion(new_version);  //обновление версии
                }
            }
            else
            {
                MDIParentMain.Log(String.Format("Update_x_x_x_x error. Не обновляем на версию {0} т.к. текущая версия {1}",new_version,old_version));
            }

        }

        /// <summary>
        /// Обновляем справочник обедов
        /// </summary>
        public void Update_5_6_6_30()
        {
            Update_x_x_x_x("5.6.6.29", "5.6.6.30",null);
        }

        /// <summary>
        /// Добавляем документ написать что добавили новый тип документа - справочник группировки
        /// добавили новое поле в базу - принадлежность к группе
        /// доработал группировку в документе возврат
        /// </summary>
        public void Update_5_6_6_31()
        {
            if (IsVersion("5.6.6.30"))
            {
                List<string> Commands = new List<string>() 
                {
                "INSERT INTO t_DocTypeRef(doctype_id, doctype_name,sendtype_type) VALUES(39, 'Справочник группировки в документах',0)",
                "ALTER TABLE t_Nome2Teremok ADD COLUMN group_id integer",
                "ALTER TABLE t_Order2ProdDetails ADD COLUMN group_id integer",
                "CREATE TABLE t_GroupRef (id AutoIncrement unique NOT NULL," +
                       "group_name text(255),"+
                       "doc_type_id integer,"+
                       "group_id integer,"+
                       "parent_id integer,"+
                       "deleted YESNO"+
                       ");"
                };
                Update_x_x_x_x("5.6.6.30", "5.6.6.31", Commands);
            }
        }

        /// <summary>
        /// Обновляем справочник обедов
        /// </summary>
        public void Update_5_6_6_32()
        {
            if (IsVersion("5.6.6.31"))
            {
                List<string> Commands = new List<string>() 
                {
                //"INSERT INTO t_DocTypeRef(doctype_id, doctype_name,sendtype_type) VALUES(39, 'Справочник группировки в документах',0)",
                //"ALTER TABLE t_Nome2Teremok ADD COLUMN group_id integer",
                "ALTER TABLE t_WebServiceTask ADD COLUMN related_doc_id integer",
                "CREATE TABLE t_SerializationHip (id AutoIncrement unique NOT NULL," +
                       "object_id integer,"+
                       "object_name text(255),"+
                       "object_type text(255),"+
                       "object_data OleObject,"+
                       "related_doc_id integer,"+
                       "date_added datetime,"+
                       "isvalid YESNO,"+
                       "description memo"+
                       ");"
                };
                Update_x_x_x_x("5.6.6.31", "5.6.6.32", Commands);
            }
        }

        /// <summary>
        /// Удаляет связи из базы где данная таблица главная
        /// </summary>
        /// <param name="table_name"></param>
        public void DeletePKConstraint(string table_name)
        {
            DataTable tab = SqlWorker.SelectFromDBSafe("select szRelationShip from MSysRelationships where szObject='" + table_name + "' WITH OWNERACCESS OPTION;", "");
            if (tab != null)
            {
                foreach (DataRow row in tab.Rows)
                {
                    SqlWorker.ExecuteQuerySafe("ALTER TABLE " + table_name + " DROP CONSTRAINT " + CellHelper.FindCell(row, 0).ToString());
                }
            }
        }

        /// <summary>
        /// Удаляет связи из базы где данная таблица второстепенная
        /// </summary>
        /// <param name="table_name"></param>
        public void DeleteFKConstraint(string table_name)
        {
            DataTable tab = SqlWorker.SelectFromDBSafe("select szObject,szRelationShip from MSysRelationships where szReferencedObject='"+table_name+"';", "");
            if (tab != null)
            {
                foreach (DataRow row in tab.Rows)
                {
                    SqlWorker.ExecuteQuerySafe("ALTER TABLE " + CellHelper.FindCell(row, 0).ToString() + " DROP CONSTRAINT " + CellHelper.FindCell(row, 1).ToString());
                }
            }
        }

        #region comment
        //public void Update_4_0_0_18()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    _conn = new OleDbConnection(CParam.ConnString);
        //    _conn.Open();
        //    _command = new OleDbCommand();
        //    _command.Connection = _conn;
        //    try
        //    {
        //        if (CParam.AppVer == "4.0.0.17")
        //        {
        //            ////t_WorkTeremok
        //            //_command.CommandText = "CREATE TABLE t_WorkTeremok(id identity unique NOT NULL, teremok_id text(255), teremok_day text(255), teremok_month text(255), teremok_year text(255), teremok_firstTime datetime, teremok_lastTime datetime)";
        //            //_command.ExecuteNonQuery();

        //            //_command.CommandText = "ALTER TABLE t_MarkItems ADD COLUMN mark_res text(255)";
        //            //_command.ExecuteNonQuery();

        //            //_command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.18' WHERE conf_id=1";
        //            //_command.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //public void Update_4_0_0_19()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    _conn = new OleDbConnection(CParam.ConnString);
        //    _conn.Open();
        //    _command = new OleDbCommand();
        //    _command.Connection = _conn;
        //    try
        //    {
        //        if (CParam.AppVer == "4.0.0.18")
        //        {
        //            ////t_WorkTeremok
        //            //_command.CommandText = "CREATE TABLE t_WorkTeremok(id identity unique NOT NULL, teremok_id text(255), teremok_day text(255), teremok_month text(255), teremok_year text(255), teremok_firstTime datetime, teremok_lastTime datetime)";
        //            //_command.ExecuteNonQuery();

        //            //_command.CommandText = "ALTER TABLE t_MarkItems ADD COLUMN mark_res text(255)";
        //            //_command.ExecuteNonQuery();

        //            _command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.19' WHERE conf_id=1";
        //            _command.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //public void Update_4_0_0_20()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    _conn = new OleDbConnection(CParam.ConnString);
        //    _conn.Open();
        //    _command = new OleDbCommand();
        //    _command.Connection = _conn;
        //    try
        //    {
        //        if (CParam.AppVer == "4.0.0.19")
        //        {
        //            //_command.CommandText = "CREATE TABLE t_ShiftType(id identity unique NOT NULL, type_guid text(255), type_name text(255), type_value text(255), type_color text(255))";
        //            //_command.ExecuteNonQuery();
        //            //_command.CommandText = "ALTER TABLE t_Employee ADD COLUMN employee_WorkField YESNO";
        //            //_command.ExecuteNonQuery();
        //            //_command.CommandText = "UPDATE t_Employee SET employee_WorkField = TRUE";
        //            //_command.ExecuteNonQuery();
        //            //_command.CommandText = "UPDATE t_Conf SET conf_value='4.0.0.20' WHERE conf_id=1";
        //            //_command.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //public void DisableT_Report()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;

        //    try
        //    {
        //        _conn = new OleDbConnection(CParam.ConnString);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;

        //        _command.CommandText = "UPDATE t_Conf SET conf_value = 0 WHERE conf_id=23"; //25
        //        _command.ExecuteNonQuery();
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //public void Enable-()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;

        //    try
        //    {
        //        _conn = new OleDbConnection(CParam.ConnString);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;

        //        _command.CommandText = "UPDATE t_Conf SET conf_value = 1 WHERE conf_id=23"; //25
        //        _command.ExecuteNonQuery();
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}


        //public void Update_3_5_5()
        //{
        //    string _str_command;
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        // проверка версии
        //        if (CParam.AppVer == "3.5.4")
        //        {
        //            _conn = new OleDbConnection(CParam.ConnString);
        //            _conn.Open();
        //            _command = new OleDbCommand();
        //            _command.Connection = _conn;

        //            // таблица позиций возврата
        //            _str_command = "CREATE TABLE t_OrderRetReason(orr_id identity unique NOT NULL, CONSTRAINT Manager_PK PRIMARY KEY (orr_id), orr_1C text(255) NOT NULL, orr_name text(255) NOT NULL);";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();

        //            // создать колонку
        //            _str_command = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_retreason_id TEXT(255);";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();

        //            // создать колонку
        //            _str_command = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_rerreason_desc TEXT(255);";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();


        //            _str_command = "SELECT count(*) FROM t_NomeTemplate WHERE nt_id=15";

        //            _command.CommandText = _str_command;
        //            if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //            {
        //                _str_command = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(15, 'Вх.Накладная')";
        //                _command.CommandText = _str_command;
        //                _command.ExecuteNonQuery();
        //            }
        //            _str_command = "SELECT count(*) FROM t_NomeTemplate WHERE nt_id=21";

        //            _command.CommandText = _str_command;
        //            if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //            {
        //                _str_command = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(21, 'Перемещение')";
        //                _command.CommandText = _str_command;
        //                _command.ExecuteNonQuery();
        //            }
        //            // 20
        //            _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=20";
        //            _command.CommandText = _str_command;
        //            if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //            {
        //                _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('Свободный')";
        //                _command.CommandText = _str_command;
        //                _command.ExecuteNonQuery();
        //            }
        //            // 21
        //            _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=21";
        //            _command.CommandText = _str_command;
        //            if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //            {
        //                _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('Перемещение')";
        //                _command.CommandText = _str_command;
        //                _command.ExecuteNonQuery();
        //            }
        //            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('новый',15,1)"; _command.ExecuteNonQuery();
        //            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправляется',15,2)"; _command.ExecuteNonQuery();
        //            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправлен',15,3)"; _command.ExecuteNonQuery();
        //            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('новый',21,1)"; _command.ExecuteNonQuery();
        //            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправляется',21,2)"; _command.ExecuteNonQuery();
        //            _command.CommandText = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id, statustype_id) VALUES('отправлен',21,3)"; _command.ExecuteNonQuery();

        //            // обновление номера версии
        //            _command.CommandText = "UPDATE t_Conf SET conf_value='3.5.5' WHERE conf_id=1";
        //            _command.ExecuteNonQuery();
        //        }

        //    }

        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //public void Update_3_5_6()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        // проверка версии
        //        if (CParam.AppVer == "3.5.5.2")
        //        {
        //            _conn = new OleDbConnection(CParam.ConnString);
        //            _conn.Open();
        //            _command = new OleDbCommand();
        //            _command.Connection = _conn;

        //             // обновление номера версии
        //            _command.CommandText = "UPDATE t_Conf SET conf_value='3.5.6' WHERE conf_id=1;";
        //            _command.ExecuteNonQuery();
        //        }

        //    }

        //Исправлена отправка open файла.
        //Изменена загрузка шаблона ресторанов с разделением теремки - рестораны.
        //public void Update_3_5_7()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        if (CParam.AppVer == "3.5.7")
        //        {
        //            _conn = new OleDbConnection(CParam.ConnString);
        //            _conn.Open();
        //            _command = new OleDbCommand();
        //            _command.Connection = _conn;


        //            //_command.CommandText = "Select * from t_Order2ProdDetails where opd_order3=0";

        //            //        //_command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_order3 decimal (18,4) DEFAULT 0";
        //            //        //_command.ExecuteNonQuery();

        //            //_command.CommandText = "UPDATE t_Order2ProdDetails SET opd_order3='0'";
        //            //_command.ExecuteNonQuery();

        //            //_command.CommandText = "ALTER TABLE t_Order2ProdDetails ADD COLUMN opd_maxquota decimal (18,4) DEFAULT 0";
        //            //_command.ExecuteNonQuery();

        //            //_command.CommandText = "UPDATE t_Order2ProdDetails SET opd_maxquota='0'";
        //            //_command.ExecuteNonQuery();

        //            //_command.CommandText = "ALTER TABLE t_Nome2Teremok ADD COLUMN n2t_maxquota decimal (18,4) DEFAULT 0";
        //            //_command.ExecuteNonQuery();

        //            //_command.CommandText = "UPDATE t_Nome2Teremok SET n2t_maxquota='0'";
        //            //_command.ExecuteNonQuery();

        //            //{
        //            //    _command.CommandText = "ALTER TABLE t_Teremok ADD COLUMN teremok_dep int;";
        //            //    _command.ExecuteNonQuery();

        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='1'";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 166";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 167";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 168";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 169";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 170";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 171";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 172";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 173";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 174";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 175";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 176";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 177";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 178";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 179";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 180";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 181";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 182";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 183";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 184";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 185";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 186";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 187";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 188";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 189";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 190";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 191";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 192";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 194";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 195";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "UPDATE t_Teremok SET teremok_dep='2' WHERE teremok_id = 201";
        //            //    _command.ExecuteNonQuery();
        //            //}

        //            //{
        //            //    _command.CommandText = "INSERT INTO t_Conf(conf_param, conf_value) VALUES('folder_emark_kkm1','')";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "INSERT INTO t_Conf(conf_param, conf_value) VALUES('folder_emark_kkm2','')";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "INSERT INTO t_Conf(conf_param, conf_value) VALUES('folder_emark_kkm3','')";
        //            //    _command.ExecuteNonQuery();
        //            //    _command.CommandText = "INSERT INTO t_Conf(conf_param, conf_value) VALUES('folder_emark_kkm4','')";
        //            //    _command.ExecuteNonQuery();
        //            //}


        //            //обновление номера версии
        //            _command.CommandText = "UPDATE t_Conf SET conf_value='3.5.7.1' WHERE conf_id=1";
        //            _command.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //public void Update_3_5_7()
        //{
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        if (CParam.AppVer == "3.5.5" || CParam.AppVer == "3.5.5.2" || CParam.AppVer == "3.5.5.3" || CParam.AppVer == "3.5.6" || CParam.AppVer == "3.5.7")
        //        {
        //            _conn = new OleDbConnection(CParam.ConnString);
        //            _conn.Open();
        //            _command = new OleDbCommand();
        //            _command.Connection = _conn;

        //            //обновление номера версии
        //            _command.CommandText = "UPDATE t_Conf SET conf_value='3.5.8' WHERE conf_id=1";
        //            _command.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}
        //public bool Update_3_5_COMBO()
        //{
        //    try
        //    {
        //        // проверить необходимость апдейта
        //        if (NeedUpdate())
        //        {
        //            //Update_3_5_MicroFix(); // параметры системы
        //            Update_3_5_COMBO_PARAM(); // параметры системы
        //            Update_3_5_COMBO_NEW_TYPE_DOC(); // новые типы документов
        //            Update_3_5_COMBO_STATUS(); // новые статусы
        //            Update_3_5_COMBO_STATUS2(); // типы статусов
        //            Update_3_5_COMBO_TEREMOK(); // закачка всех видов теремков
        //            Update_3_5_COMBO_Z(); // закачка всех видов теремков
        //            return true;
        //        }
        //        return false;

        //    }
        //    catch (Exception _exp)
        //    {
        //        throw _exp;
        //    }
        //}

        //private bool NeedUpdate()
        //{
        //    CConfig _config = new CConfig();
        //    string _str_connect = _config.m_connstring;

        //    string _str_command;
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;


        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=19";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //            return true;
        //        else
        //            return false;
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //private void Update_3_5_MicroFix()
        //{
        //    CConfig _config = new CConfig();
        //    string _str_connect = _config.m_connstring;

        //    string _str_command;
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;

        //        // таблица ПАРАМЕТРОВ
        //        _str_command = "UPDATE t_TaskExchange SET task_state_id=1;";
        //        _command.CommandText = _str_command;
        //        _command.ExecuteNonQuery();

        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //private void Update_3_5_COMBO_PARAM()
        //{
        //    CConfig _config = new CConfig();
        //    string _str_connect = _config.m_connstring;

        //    string _str_command;
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;

        //        // таблица ПАРАМЕТРОВ
        //        _str_command = "CREATE TABLE t_Conf(conf_id identity unique NOT NULL, CONSTRAINT Manager_PK PRIMARY KEY (conf_id), conf_param text(255) NOT NULL, conf_value text(255) NOT NULL);";
        //        _command.CommandText = _str_command;
        //        _command.ExecuteNonQuery();

        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(1, 'ver', '3.5.0')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(2, 'teremok_id', '" + _config.m_teremok_id + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(3, 'timer_exh', '" + _config.m_timer + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(4, 'timer_inbox', '" + _config.m_timer_inbox + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(5, 'timer_report', '" + _config.m_timer_report + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(6, 'ftp_server1', '" + _config.m_FTP_server + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(7, 'ftp_port1', '" + _config.m_FTP_port + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(8, 'ftp_server2', '" + _config.m_FTP_server2 + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(9, 'ftp_port2', '" + _config.m_FTP_port + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(10, 'ftp_login1', '" + _config.m_FTP_login + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(11, 'ftp_pass1', '" + _config.m_FTP_password + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(12, 'ftp_login2', '')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(13, 'ftp_pass2', '')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(14, 'dep', '1')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(15, 'folder_kkm1_in', '" + _config.m_folder_kkm1_in + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(16, 'folder_kkm1_out', '" + _config.m_folder_kkm1_out + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(17, 'folder_kkm2_in', '" + _config.m_folder_kkm2_in + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(18, 'folder_kkm2_out', '" + _config.m_folder_kkm2_out + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(19, 'folder_kkm3_in', '" + _config.m_folder_kkm3_in + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(20, 'folder_kkm3_out', '" + _config.m_folder_kkm3_out + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(21, 'folder_kkm4_in', '" + _config.m_folder_kkm4_in + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(22, 'folder_kkm4_out', '" + _config.m_folder_kkm4_out + "')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_Conf(conf_id, conf_param, conf_value) VALUES(23, 'load_report', '1')";
        //        _command.ExecuteNonQuery();
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //// новые виды документов + СТАТУСЫ
        //private void Update_3_5_COMBO_NEW_TYPE_DOC()
        //{
        //    CConfig _config = new CConfig();
        //    string _str_connect = _config.m_connstring;

        //    string _str_command;
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;

        //        // обновим старые документы
        //        //_command.CommandText = "UPDATE t_DocTypeRef SET doctype_name = 'ежедневный заказ' WHERE doctype_id=1";
        //        //_command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocTypeRef SET doctype_name = 'заказ хоз.средств' WHERE doctype_id=2";
        //        //_command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name = 'списание' WHERE doctype_id=6";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name = 'списание Г П' WHERE doctype_id=9";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name = 'контрольные остатки' WHERE doctype_id=10";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name = 'свободно' WHERE doctype_id=11";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocTypeRef SET doctype_name = 'карты сотрудников' WHERE doctype_id=4";
        //        _command.ExecuteNonQuery();

        //        // 10 контрольные остатки
        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=10";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('контрольные остатки')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        // 11 свободно
        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=11";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('свободно')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        // 12 перемещение
        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=12";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('перемещение')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        // 13 
        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=13";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('списание С П')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        else
        //        {
        //            _str_command = "UPDATE t_DocTypeRef SET doctype_name =  'списание С П' WHERE doctype_id=13";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        // 14 
        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=14";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('остатки инвентаря')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        else
        //        {
        //            _str_command = "UPDATE t_DocTypeRef SET doctype_name =  'остатки инвентаря' WHERE doctype_id=14";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        // 15 
        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=15";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('входящее перемещение')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        // 16 
        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=16";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('накладная')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        // 17
        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=17";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('заказ х/с нед.')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        // 18
        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=18";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('заказ х/с мес.')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        // 19
        //        _str_command = "SELECT count(*) FROM t_DocTypeRef WHERE doctype_id=19";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocTypeRef(doctype_name) VALUES('свободно')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }


        //        // НОВЫЕ СТАТУСЫ

        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=26";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('новый',10)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=27";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправляется',10)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=28";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправлен',10)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=29";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('новый',11)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=30";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправляется',11)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=31";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправлен',11)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=32";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('новый',12)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=33";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправляется',12)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=34";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправлен',12)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=35";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('новый',13)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=36";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправляется',13)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=37";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправлен',13)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=38";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('новый',14)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=39";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправляется',14)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=40";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправлен',14)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=41";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('получено',15)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=42";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('принят',16)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=43";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('новый',17)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=44";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправляется',17)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=45";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправлен',17)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=46";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('новый',18)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=47";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправляется',18)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=48";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправлен',18)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=49";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('новый',2)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=50";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправляется',2)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=51";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправлен',2)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=52";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('новый',16)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=53";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправляется',16)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        _str_command = "SELECT count(*) FROM t_DocStatusRef WHERE docstatusref_id=54";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_DocStatusRef(docstatusref_name, doctype_id) VALUES('отправлен',16)";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }

        //        // шаблоны
        //        _str_command = "SELECT count(*) FROM t_NomeTemplate WHERE nt_id=10";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(10, 'контрольные остатки')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        else
        //        {
        //            _str_command = "UPDATE t_NomeTemplate SET nt_desc='контрольные остатки' WHERE nt_id = 10";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }

        //        _str_command = "SELECT count(*) FROM t_NomeTemplate WHERE nt_id=13";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(13, 'списание СП')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }

        //        _str_command = "SELECT count(*) FROM t_NomeTemplate WHERE nt_id=14";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(14, 'остатки инвентаря')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }

        //        _str_command = "SELECT count(*) FROM t_NomeTemplate WHERE nt_id=17";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(17, 'заказ ХС недельный')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }

        //        _str_command = "SELECT count(*) FROM t_NomeTemplate WHERE nt_id=18";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(18, 'заказ ХС месячный')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }


        //        _str_command = "SELECT count(*) FROM t_NomeTemplate WHERE nt_id=9";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(9, 'списание Г П')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //        else
        //        {
        //            _str_command = "UPDATE t_NomeTemplate SET nt_desc='списание Г П' WHERE nt_id = 9";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }

        //        _str_command = "SELECT count(*) FROM t_NomeTemplate WHERE nt_id=16";
        //        _command.CommandText = _str_command;
        //        if (Convert.ToInt16(_command.ExecuteScalar()) == 0)
        //        {
        //            _str_command = "INSERT INTO t_NomeTemplate(nt_id, nt_desc) VALUES(16, 'накладная')";
        //            _command.CommandText = _str_command;
        //            _command.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}


        //private void Update_3_5_COMBO_STATUS()
        //{
        //    CConfig _config = new CConfig();
        //    string _str_connect = _config.m_connstring;

        //    string _str_command;
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;

        //        // таблица ВИДОВ СТАТУСОВ
        //        _str_command = "CREATE TABLE t_StatusType(status_type_id identity unique NOT NULL, CONSTRAINT Manager_PK PRIMARY KEY (status_type_id), status_type_name text(255) NOT NULL, status_type_color text(255) NOT NULL);";
        //        _command.CommandText = _str_command;
        //        _command.ExecuteNonQuery();

        //        _command.CommandText = "INSERT INTO t_StatusType(status_type_id, status_type_name, status_type_color) VALUES(1, 'новый', 'Yellow')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_StatusType(status_type_id, status_type_name, status_type_color) VALUES(2, 'отправляется', 'Red')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_StatusType(status_type_id, status_type_name, status_type_color) VALUES(3, 'отправлен', 'Thistle')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_StatusType(status_type_id, status_type_name, status_type_color) VALUES(4, 'получен', 'Bisque')";
        //        _command.ExecuteNonQuery();
        //        _command.CommandText = "INSERT INTO t_StatusType(status_type_id, status_type_name, status_type_color) VALUES(5, 'получено', 'Red')";
        //        _command.ExecuteNonQuery();

        //        // новые реквизиты статусов


        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //private void Update_3_5_COMBO_STATUS2()
        //{
        //    CConfig _config = new CConfig();
        //    string _str_connect = _config.m_connstring;

        //    string _str_command;
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;

        //        // таблица ВИДОВ СТАТУСОВ
        //        _str_command = "ALTER TABLE t_DocStatusRef ADD COLUMN statustype_id int;";
        //        _command.CommandText = _str_command;
        //        _command.ExecuteNonQuery();


        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 1"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 2"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 3"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 4"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 5"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 6"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 7"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 8"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 9"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 10"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 5 WHERE docstatusref_id = 11"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 12"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1, docstatusref_name = 'получено' WHERE docstatusref_id = 13"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3, docstatusref_name = 'обновлено' WHERE docstatusref_id = 14"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 15"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1, docstatusref_name = 'новый' WHERE docstatusref_id = 16"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2, docstatusref_name = 'отправляется' WHERE docstatusref_id = 17"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3, docstatusref_name = 'отправлен' WHERE docstatusref_id = 18"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 19"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 20"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 21"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 22"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 23"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 24"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 25"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 26"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 27"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 28"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 29"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 30"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 31"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 32"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 33"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 34"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 35"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 36"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 37"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 38"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 39"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 40"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 41"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 42"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 43"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 44"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 45"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 46"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 47"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 48"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 49"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 50"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 51"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 52"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 53"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 54"; _command.ExecuteNonQuery();

        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}


        //private void Update_3_5_COMBO_TEREMOK()
        //{
        //    CConfig _config = new CConfig();
        //    string _str_connect = _config.m_connstring;

        //    string _str_command;
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;

        //        // новый реквизит для перемещения
        //        _str_command = "ALTER TABLE t_Doc ADD COLUMN doc_1C text(255);";
        //        _command.CommandText = _str_command;
        //        _command.ExecuteNonQuery();

        //        _str_command = "ALTER TABLE t_Doc ADD COLUMN doc_teremok2_id int;";
        //        _command.CommandText = _str_command;
        //        _command.ExecuteNonQuery();

        //        // код 1С
        //        _str_command = "ALTER TABLE t_Teremok ADD COLUMN teremok_1CID TEXT;";
        //        _command.CommandText = _str_command;
        //        _command.ExecuteNonQuery();

        //        // теремок по умолчанию
        //        _str_command = "ALTER TABLE t_Teremok ADD COLUMN teremok_current YESNO;";
        //        _command.CommandText = _str_command;
        //        _command.ExecuteNonQuery();
        //        _str_command = "UPDATE t_Teremok SET teremok_current = TRUE";
        //        _command.CommandText = _str_command;
        //        _command.ExecuteNonQuery();

        //    }
        //    catch (Exception)
        //    {
        //        // не боросать исключение
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //private void Update_3_5_COMBO_Z()
        //{
        //    CConfig _config = new CConfig();
        //    string _str_connect = _config.m_connstring;

        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;

        //        _command.CommandText = "ALTER TABLE t_ZReport ADD COLUMN z_kkm text(255);"; _command.ExecuteNonQuery();
        //        _command.CommandText = "ALTER TABLE t_ZReport ADD COLUMN z_fr text(255);"; _command.ExecuteNonQuery();
        //        _command.CommandText = "ALTER TABLE t_ZReport ADD COLUMN z_cash decimal(18,4);"; _command.ExecuteNonQuery();
        //        _command.CommandText = "ALTER TABLE t_ZReport ADD COLUMN z_cash_return decimal(18,4);"; _command.ExecuteNonQuery();
        //        _command.CommandText = "ALTER TABLE t_ZReport ADD COLUMN z_card decimal(18,4);"; _command.ExecuteNonQuery();
        //        _command.CommandText = "ALTER TABLE t_ZReport ADD COLUMN z_card_return decimal(18,4);"; _command.ExecuteNonQuery();
        //        _command.CommandText = "ALTER TABLE t_ZReport ADD COLUMN z_cupon decimal(18,4);"; _command.ExecuteNonQuery();
        //        _command.CommandText = "ALTER TABLE t_ZReport ADD COLUMN z_cupon_return decimal(18,4);"; _command.ExecuteNonQuery();
        //        _command.CommandText = "ALTER TABLE t_ZReport ADD COLUMN z_dinner decimal(18,4);"; _command.ExecuteNonQuery();
        //        _command.CommandText = "ALTER TABLE t_ZReport ADD COLUMN z_dinner_return decimal(18,4);"; _command.ExecuteNonQuery();

        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}



        //public void FixFB()
        //{
        //    CConfig _config = new CConfig();
        //    string _str_connect = _config.m_connstring;

        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;

        //        // таблица ВИДОВ СТАТУСОВ
        //        //_str_command = "ALTER TABLE t_DocStatusRef ADD COLUMN statustype_id int;";
        //        //_command.CommandText = _str_command;
        //        //_command.ExecuteNonQuery();


        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 1 AND statustype_id = 1 WHERE docstatusref_id = 1"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 1 AND statustype_id = 3 WHERE docstatusref_id = 2"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 3 AND statustype_id = 1 WHERE docstatusref_id = 7"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 3 AND statustype_id = 3 WHERE docstatusref_id = 8"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 6 AND statustype_id = 1 WHERE docstatusref_id = 9"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 6 AND statustype_id = 3 WHERE docstatusref_id = 10"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 7 AND statustype_id = 5 WHERE docstatusref_id = 11"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 5 AND statustype_id = 3 WHERE docstatusref_id = 12"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 4 AND statustype_id = 1 WHERE docstatusref_id = 13"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 4 AND statustype_id = 3 WHERE docstatusref_id = 14"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 7 AND statustype_id = 4 WHERE docstatusref_id = 15"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 9 AND statustype_id = 4 WHERE docstatusref_id = 16"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 9 AND statustype_id = 4 WHERE docstatusref_id = 17"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 9 AND statustype_id = 4 WHERE docstatusref_id = 18"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 1 AND statustype_id = 2 WHERE docstatusref_id = 19"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 3 AND statustype_id = 2 WHERE docstatusref_id = 20"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 4 AND statustype_id = 2 WHERE docstatusref_id = 21"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 5 AND statustype_id = 2 WHERE docstatusref_id = 22"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 6 AND statustype_id = 2 WHERE docstatusref_id = 23"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 9 AND statustype_id = 2 WHERE docstatusref_id = 24"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 8 AND statustype_id = 4 WHERE docstatusref_id = 25"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 10 AND statustype_id = 1 WHERE docstatusref_id = 26"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 10 AND statustype_id = 2 WHERE docstatusref_id = 27"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 10 AND statustype_id = 3 WHERE docstatusref_id = 28"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 11 AND statustype_id = 1 WHERE docstatusref_id = 29"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 11 AND statustype_id = 2 WHERE docstatusref_id = 30"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 11 AND statustype_id = 3 WHERE docstatusref_id = 31"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 12 AND statustype_id = 1 WHERE docstatusref_id = 32"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 12 AND statustype_id = 2 WHERE docstatusref_id = 33"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 12 AND statustype_id = 3 WHERE docstatusref_id = 34"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 13 AND statustype_id = 1 WHERE docstatusref_id = 35"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 13 AND statustype_id = 2 WHERE docstatusref_id = 36"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 13 AND statustype_id = 1 WHERE docstatusref_id = 37"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 14 AND statustype_id = 2 WHERE docstatusref_id = 38"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 14 AND statustype_id = 3 WHERE docstatusref_id = 39"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 14 AND statustype_id = 1 WHERE docstatusref_id = 40"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 15 AND statustype_id = 4 WHERE docstatusref_id = 41"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 16 AND statustype_id = 4 WHERE docstatusref_id = 42"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 17 AND statustype_id = 1 WHERE docstatusref_id = 43"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 17 AND statustype_id = 2 WHERE docstatusref_id = 44"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 17 AND statustype_id = 3 WHERE docstatusref_id = 45"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 18 AND statustype_id = 1 WHERE docstatusref_id = 46"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 18 AND statustype_id = 2 WHERE docstatusref_id = 47"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 18 AND statustype_id = 3 WHERE docstatusref_id = 48"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 2 AND statustype_id = 1 WHERE docstatusref_id = 49"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 2 AND statustype_id = 2 WHERE docstatusref_id = 50"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 2 AND statustype_id = 3 WHERE docstatusref_id = 51"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 16 AND statustype_id = 1 WHERE docstatusref_id = 52"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 16 AND statustype_id = 2 WHERE docstatusref_id = 53"; _command.ExecuteNonQuery();
        //        //_command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 16 AND statustype_id = 3 WHERE docstatusref_id = 54"; _command.ExecuteNonQuery();

        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 1 WHERE docstatusref_id = 1"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 1 WHERE docstatusref_id = 2"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 3 WHERE docstatusref_id = 7"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 3 WHERE docstatusref_id = 8"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 6 WHERE docstatusref_id = 9"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 6 WHERE docstatusref_id = 10"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 7 WHERE docstatusref_id = 11"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 5 WHERE docstatusref_id = 12"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 4 WHERE docstatusref_id = 13"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 4 WHERE docstatusref_id = 14"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 7 WHERE docstatusref_id = 15"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 9 WHERE docstatusref_id = 16"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 9 WHERE docstatusref_id = 17"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 9 WHERE docstatusref_id = 18"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 1 WHERE docstatusref_id = 19"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 3 WHERE docstatusref_id = 20"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 4 WHERE docstatusref_id = 21"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 5 WHERE docstatusref_id = 22"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 6 WHERE docstatusref_id = 23"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 9 WHERE docstatusref_id = 24"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 8 WHERE docstatusref_id = 25"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 10 WHERE docstatusref_id = 26"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 10 WHERE docstatusref_id = 27"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 10 WHERE docstatusref_id = 28"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 11 WHERE docstatusref_id = 29"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 11 WHERE docstatusref_id = 30"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 11 WHERE docstatusref_id = 31"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 12 WHERE docstatusref_id = 32"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 12 WHERE docstatusref_id = 33"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 12 WHERE docstatusref_id = 34"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 13 WHERE docstatusref_id = 35"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 13 WHERE docstatusref_id = 36"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 13 WHERE docstatusref_id = 37"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 14 WHERE docstatusref_id = 38"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 14 WHERE docstatusref_id = 39"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 14 WHERE docstatusref_id = 40"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 15 WHERE docstatusref_id = 41"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 16 WHERE docstatusref_id = 42"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 17 WHERE docstatusref_id = 43"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 17 WHERE docstatusref_id = 44"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 17 WHERE docstatusref_id = 45"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 18 WHERE docstatusref_id = 46"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 18 WHERE docstatusref_id = 47"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 18 WHERE docstatusref_id = 48"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 2 WHERE docstatusref_id = 49"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 2 WHERE docstatusref_id = 50"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 2 WHERE docstatusref_id = 51"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 16 WHERE docstatusref_id = 52"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 16 WHERE docstatusref_id = 53"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET doctype_id = 16 WHERE docstatusref_id = 54"; _command.ExecuteNonQuery();

        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 1"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 2"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 7"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 8"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 9"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 10"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 5 WHERE docstatusref_id = 11"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 12"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 13"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 14"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 15"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 16"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 17"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 18"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 19"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 20"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 21"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 22"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 23"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 24"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 25"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 26"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 27"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 28"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 29"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 30"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 31"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 32"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 33"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 34"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 35"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 36"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 37"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 38"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 39"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 40"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 41"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 4 WHERE docstatusref_id = 42"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 43"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 44"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 45"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 46"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 47"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 48"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 49"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 50"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 51"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 1 WHERE docstatusref_id = 52"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 2 WHERE docstatusref_id = 53"; _command.ExecuteNonQuery();
        //        _command.CommandText = "UPDATE t_DocStatusRef SET statustype_id = 3 WHERE docstatusref_id = 54"; _command.ExecuteNonQuery();



        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}

        //public void FixExch()
        //{            
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;
        //    try
        //    {
        //        _conn = new OleDbConnection(CParam.ConnString);
        //        _conn.Open();
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;
        //        _command.CommandText = "UPDATE t_TaskExchange SET task_state_id = 1 WHERE task_id = 2191"; _command.ExecuteNonQuery();                
        //    }
        //    catch (Exception exp)
        //    {
        //        throw exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }
        //}
        #endregion
    }
}
