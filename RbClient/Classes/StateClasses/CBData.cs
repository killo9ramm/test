using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;

using System.IO;
using System.IO.Compression;
using Common.Logging;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes;
using RBClient.ru.teremok.msk;
using RBClient.Classes.DocumentClasses;
using RBClient.Classes.CustomClasses;

namespace RBClient
{
    public enum DocStatusType{New=1,Sending=2,Sended=3,UpdatedRecieved=4,Recieved=5}
    class CBData : LoggerBaseMdi
    {
        public int m_teremok_id = 0;
        private ILog log = LogManager.GetCurrentClassLogger();

        public CBData(int user_id, int teremok_id)
        {
            m_teremok_id = teremok_id;
        }

        public CBData()
        {
        }


        public void DeleteRecordsFromDB<T>(string query, int portion,int maxLoopCount=10) where T : ModelClass,new()
        {
            int i = 0;
            List<T> docs = new T().Select<T>(query).ToList();
            while (docs.Count > 0 && i < maxLoopCount)
            {
                i++;
                var ddocs=docs.Take(portion).ToList();
                foreach (var dd in ddocs)
                {
                    dd.Delete();
                }
                docs = new T().Select<T>(query).ToList();
            }
        }

        // список документов
        public DataTable ViewDoc(int doc_type_id, int teremok_id)
        {
            OleDbConnection _conn = null;
            string _str_command = "SELECT TOP 150 doc_id, doc_datetime, doctype_name, docstatusref_name, doc_desc, doc_type_id, doc_status_id, " +
                " teremok_name FROM t_Teremok INNER JOIN (t_DocTypeRef INNER JOIN " +
                " (t_DocStatusRef INNER JOIN t_Doc ON t_DocStatusRef.docstatusref_id = t_Doc.doc_status_id) ON t_DocTypeRef.doctype_id = t_Doc.doc_type_id) " +
                " ON t_Teremok.teremok_id = t_Doc.doc_teremok_id WHERE teremok_id = " + teremok_id.ToString();
            if (doc_type_id != 0)
                _str_command += " AND doc_type_id = " + doc_type_id.ToString();
            _str_command += " ORDER BY doc_id DESC";

            DataTable _table;
            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("Doc");
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

        public void DeleteOldReports()
        {
            CZReportHelper _zreport = new CZReportHelper();
            string teremok_id = CParam.TeremokId;
            int doc_type_id = 0;

            OleDbConnection _conn = null;
            string _str_command = "SELECT TOP 150 doc_id, doc_datetime, doctype_name, docstatusref_name, doc_desc, doc_type_id, doc_status_id, " +
                " teremok_name FROM t_Teremok INNER JOIN (t_DocTypeRef INNER JOIN " +
                " (t_DocStatusRef INNER JOIN t_Doc ON t_DocStatusRef.docstatusref_id = t_Doc.doc_status_id) ON t_DocTypeRef.doctype_id = t_Doc.doc_type_id) " +
                " ON t_Teremok.teremok_id = t_Doc.doc_teremok_id WHERE teremok_id = " + teremok_id.ToString();
            if (doc_type_id != 0)
                _str_command += " AND doc_type_id = " + doc_type_id.ToString();
            _str_command += " ORDER BY doc_id DESC";

            DataTable _table;
            try
            {

                //#warning !сделать нормальную отчистку базы

                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("Doc");
                _data_adapter.Fill(_table);

                foreach (DataRow _row in _table.Rows)
                {
                    DateTime doc_time = Convert.ToDateTime(_row[1].ToString());
                    DateTime current_time = DateTime.Today.AddDays(-90);
                    if (doc_time <= current_time)
                    {
                        int _doc_type = Convert.ToInt32(_row[5].ToString());
                        int _doc_status_id = Convert.ToInt32(_row[6].ToString());
                        if (_doc_type != 5)
                        {
                            OrderDelete(Convert.ToInt32(_row[0].ToString()));
                        }
                        if (_doc_type == 5) // Z-отчет
                        {
                            // удаляем
                            if (_doc_status_id == 22)
                            {
                                _zreport.ZReportDelete(Convert.ToInt32(_row[0].ToString()));
                            }
                            else
                            {
                                _zreport.ZReportDelete(Convert.ToInt32(_row[0].ToString()));
                            }
                        }
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

        /// <summary>
        /// Получить список параметров для редактирования
        /// </summary>
        /// <returns></returns>
        public DataTable GetParam()
        {
            OleDbConnection _conn = null;
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter("SELECT * FROM t_Conf", _conn);

                _table = new DataTable("Param");
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

        /// <summary>
        /// Обновсить параметры системы
        /// </summary>
        /// <param name="param_id"></param>
        /// <param name="param_value"></param>
        public void ParamUpdate(string param_id, string param_value, string param_dep)
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
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_Conf SET conf_value = @conf_value, conf_dep = @conf_dep  WHERE conf_id = @conf_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@conf_value", param_value);
                _command.Parameters.AddWithValue("@conf_dep", param_dep);
                _command.Parameters.AddWithValue("@conf_id", param_id);
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

        public int GetDocTypeID(string doctype_name)
        {
            // проверим на тип ВСЕ
            if (doctype_name == "Все документы")
                return 0;

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
                _str_command = "SELECT doctype_id FROM t_DocTypeRef WHERE doctype_name='" + doctype_name + "'";
                _command.CommandText = _str_command;
                return Convert.ToInt32(_command.ExecuteScalar().ToString());
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

        // заказ
        public DataTable OrderDetailsView(int doc_id, int type_doc)
        {

            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            if (type_doc == 1)
                _str_command = "SELECT opd_id, nome_name, opd_order, opd_ed, opd_K, opd_bed, opd_order*opd_K AS opd_order1, opd_order2, opd_quota, opd_maxquota, orr_id, orr_name, orr_1C, opd_rerreason_desc, opd_order3, opd_total, opd_order * opd_K * opd_ke as opd_order_total, opd_ke, opd_K * opd_ke as opd_kke, opd_ze, opd_bold, nome_1C, opd_retreason_id " +
                    " FROM t_OrderRetReason RIGHT JOIN (t_Nomenclature INNER JOIN t_Order2ProdDetails ON t_Nomenclature.nome_id = t_Order2ProdDetails.opd_nome_id " +
                    " ) ON t_OrderRetReason.orr_1C = t_Order2ProdDetails.opd_retreason_id " +
                    " WHERE opd_doc_id=" + doc_id.ToString() + " ORDER BY opd_id ASC";
            else
                if (type_doc == 9 || type_doc == 13)
                {
                    if (StaticConstants.UpdateHelper.IsNewerVersion("5.6.4.0"))
                    {
                        _str_command = "SELECT t_Order2ProdDetails.opd_id, t_Nomenclature.nome_name, t_Order2ProdDetails.opd_order, t_Order2ProdDetails.opd_ed, t_Order2ProdDetails.opd_K, t_Order2ProdDetails.opd_bed, opd_order*opd_K AS opd_order1, t_Order2ProdDetails.opd_order2, t_Order2ProdDetails.opd_quota, t_Order2ProdDetails.opd_maxquota, t_OrderRetReason.orr_id, t_OrderRetReason.orr_name, t_OrderRetReason.orr_1C, t_Order2ProdDetails.opd_rerreason_desc, t_Order2ProdDetails.opd_order3, t_Order2ProdDetails.opd_total, opd_order*opd_K*opd_ke AS opd_order_total, t_Order2ProdDetails.opd_ke, opd_K*opd_ke AS opd_kke, t_Order2ProdDetails.opd_ze, t_Order2ProdDetails.opd_bold, t_Nomenclature.nome_1C, t_Order2ProdDetails.opd_retreason_id, t_UtilReasonRef.urr_id, t_UtilReasonRef.urr_name, t_Nomenclature.nome_id,t_Order2ProdDetails.opd_doc_id " +
                                        "FROM t_Nomenclature INNER JOIN (((t_OrderRetReason RIGHT JOIN t_Order2ProdDetails ON t_OrderRetReason.orr_1C = t_Order2ProdDetails.opd_retreason_id) LEFT JOIN t_Utilization ON (t_Order2ProdDetails.opd_nome_id = t_Utilization.util_nome_id) AND (t_Order2ProdDetails.opd_doc_id = t_Utilization.util_doc_id)) LEFT JOIN t_UtilReasonRef ON t_Utilization.util_reason_id = t_UtilReasonRef.urr_id) ON t_Nomenclature.nome_id = t_Order2ProdDetails.opd_nome_id " +
                                        "WHERE (((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ")) " +
                                        "ORDER BY t_Order2ProdDetails.opd_id;";
                    }
                    else
                    {
                        _str_command = "SELECT t_Order2ProdDetails.opd_id, t_Nomenclature.nome_name, t_Order2ProdDetails.opd_order, t_Order2ProdDetails.opd_ed, t_Order2ProdDetails.opd_K, t_Order2ProdDetails.opd_bed, opd_order*opd_K AS opd_order1, t_Order2ProdDetails.opd_order2, t_Order2ProdDetails.opd_quota, t_Order2ProdDetails.opd_maxquota, t_OrderRetReason.orr_id, t_OrderRetReason.orr_name, t_OrderRetReason.orr_1C, t_Order2ProdDetails.opd_rerreason_desc, t_Order2ProdDetails.opd_order3, t_Order2ProdDetails.opd_total, opd_order*opd_K*opd_ke AS opd_order_total, t_Order2ProdDetails.opd_ke, opd_K*opd_ke AS opd_kke, t_Order2ProdDetails.opd_ze, t_Order2ProdDetails.opd_bold, t_Nomenclature.nome_1C, t_Order2ProdDetails.opd_retreason_id, t_UtilReasonRef.urr_id, t_UtilReasonRef.urr_name, t_Nomenclature.nome_id, t_Order2ProdDetails.opd_doc_id " +
                                       "FROM (t_Nomenclature INNER JOIN (t_OrderRetReason RIGHT JOIN t_Order2ProdDetails ON t_OrderRetReason.orr_1C = t_Order2ProdDetails.opd_retreason_id) ON t_Nomenclature.nome_id = t_Order2ProdDetails.opd_nome_id) LEFT JOIN (t_Utilization LEFT JOIN t_UtilReasonRef ON t_Utilization.util_reason_id = t_UtilReasonRef.urr_id) ON t_Order2ProdDetails.opd_id = t_Utilization.util_opd_id " +
                                       "WHERE (((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ")) " +
                                       "ORDER BY t_Order2ProdDetails.opd_id;";
                    }

                    //_str_command = "SELECT TOP 2 t_Order2ProdDetails.opd_id, t_Nomenclature.nome_name, t_Order2ProdDetails.opd_order, t_Order2ProdDetails.opd_ed, t_Order2ProdDetails.opd_K, t_Order2ProdDetails.opd_bed, opd_order*opd_K AS opd_order1, t_Order2ProdDetails.opd_order2, t_Order2ProdDetails.opd_quota, t_Order2ProdDetails.opd_maxquota, t_OrderRetReason.orr_id, t_OrderRetReason.orr_name, t_OrderRetReason.orr_1C, t_Order2ProdDetails.opd_rerreason_desc, t_Order2ProdDetails.opd_order3, t_Order2ProdDetails.opd_total, opd_order*opd_K*opd_ke AS opd_order_total, t_Order2ProdDetails.opd_ke, opd_K*opd_ke AS opd_kke, t_Order2ProdDetails.opd_ze, t_Order2ProdDetails.opd_bold, t_Nomenclature.nome_1C, t_Order2ProdDetails.opd_retreason_id, t_UtilReasonRef.urr_name, t_UtilReasonRef.urr_id "+
                    //                "FROM t_Nomenclature INNER JOIN (((t_OrderRetReason RIGHT JOIN t_Order2ProdDetails ON t_OrderRetReason.orr_1C = t_Order2ProdDetails.opd_retreason_id) LEFT JOIN t_Utilization ON (t_Order2ProdDetails.opd_nome_id = t_Utilization.util_nome_id) AND (t_Order2ProdDetails.opd_doc_id = t_Utilization.util_doc_id)) LEFT JOIN t_UtilReasonRef ON t_Utilization.util_reason_id = t_UtilReasonRef.urr_id) ON t_Nomenclature.nome_id = t_Order2ProdDetails.opd_nome_id "+
                    //                "WHERE (((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ")) " +
                    //                "ORDER BY t_Order2ProdDetails.opd_id;";

                }
                else
                {
                    _str_command = "SELECT opd_id, nome_name, opd_order, opd_ed, opd_K, opd_bed, opd_order AS opd_order1, opd_order2, opd_quota, opd_maxquota, orr_id, orr_name, orr_1C, opd_rerreason_desc, opd_order3, opd_total, opd_order * opd_K * opd_ke as opd_order_total, opd_ke, opd_K * opd_ke as opd_kke, opd_ze, opd_bold, nome_1C, opd_retreason_id " +
                        " FROM t_OrderRetReason RIGHT JOIN (t_Nomenclature INNER JOIN t_Order2ProdDetails ON t_Nomenclature.nome_id = t_Order2ProdDetails.opd_nome_id " +
                        " ) ON t_OrderRetReason.orr_1C = t_Order2ProdDetails.opd_retreason_id " +
                        " WHERE opd_doc_id=" + doc_id.ToString() + " ORDER BY opd_id ASC";
                }

            DataTable _table;
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("DocDetails");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public int GetDocState(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT doc_status_id FROM t_Doc WHERE doc_id = " + doc_id.ToString();
                _command.CommandText = _str_command;
                return Convert.ToInt32(_command.ExecuteScalar());
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public string GetDocStateName(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT docstatusref_name FROM t_DocStatusRef WHERE docstatusref_id = " + doc_id.ToString();
                _command.CommandText = _str_command;
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public int GetDocType(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;
            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT doc_type_id FROM t_Doc WHERE doc_id = " + doc_id.ToString();
                _command.CommandText = _str_command;
                return Convert.ToInt32(_command.ExecuteScalar());
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        // новый документ
        public int OrderAdd(int doc_type_id, int teremok_id, int teremok2_id)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            DataTable _table;
            string _str_command;
            int _doc_id;
            CBData _data = new CBData();

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                // проверим, есть ли ШАБЛОН
                _str_command = "SELECT count(*) FROM t_Nomenclature INNER JOIN t_Nome2Teremok ON t_Nomenclature.nome_id = t_Nome2Teremok.n2t_nome_id WHERE n2t_nt_id = " + doc_type_id.ToString() + " AND n2t_teremok_id=" + teremok_id.ToString();
                _command.CommandText = _str_command;
                int _count = Convert.ToInt32(_command.ExecuteScalar());
                if (_count == 0)
                {
                    return 0;
                }

                // создать новый документ
                if (teremok2_id == 0)
                    _str_command = "INSERT INTO t_Doc(doc_type_id, doc_teremok_id, doc_status_id) VALUES (@doc_type_id, @doc_teremok_id, @doc_status_id)";
                else
                    _str_command = "INSERT INTO t_Doc(doc_type_id, doc_teremok_id, doc_status_id, doc_teremok2_id, doc_desc) VALUES (@doc_type_id, @doc_teremok_id, @doc_status_id, @doc_teremok2_id, @doc_desc)";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_type_id", doc_type_id);
                _command.Parameters.AddWithValue("@doc_teremok_id", teremok_id);
                _command.Parameters.AddWithValue("@doc_status_id", _data.GetStatusID(null, doc_type_id, 1));
                if (teremok2_id != 0)
                {
                    _command.Parameters.AddWithValue("@doc_teremok2_id", teremok2_id);
                    _command.Parameters.AddWithValue("@doc_desc", "на объект: " + _data.GetTeremokName(teremok2_id));
                }
                _command.ExecuteNonQuery();
                // id документа
                _str_command = "SELECT @@IDENTITY";
                _command.CommandText = _str_command;
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());


                _str_command = "SELECT nome_id, n2t_ed, n2t_bed, n2t_K, n2t_quota, n2t_maxquota, n2t_ke, n2t_ze, n2t_bold FROM t_Nomenclature " +
                    " INNER JOIN t_Nome2Teremok ON t_Nomenclature.nome_id = t_Nome2Teremok.n2t_nome_id WHERE n2t_nt_id = "
                        + doc_type_id.ToString() + " AND n2t_teremok_id=" + teremok_id.ToString() + " ORDER BY n2t_id ASC";

                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("t_Nomenclature");
                _data_adapter.Fill(_table);

                {
                    _str_command = "INSERT INTO t_Order2ProdDetails(opd_doc_id, opd_nome_id, opd_ed, opd_bed, opd_K, opd_quota, opd_maxquota, opd_ke, opd_ze, opd_bold) VALUES (@opd_doc_id, @opd_nome_id, @opd_ed, @opd_bed, @opd_K, @opd_quota, @opd_maxquota, @opd_ke, @opd_ze, @opd_bold)";
                    _command.CommandText = _str_command;
                    _command.Parameters.Clear();
                    _command.Parameters.Add("@opd_doc_id", OleDbType.Integer);
                    _command.Parameters.Add("@opd_nome_id", OleDbType.Integer);
                    _command.Parameters.Add("@opd_ed", OleDbType.VarChar, 50);
                    _command.Parameters.Add("@opd_bed", OleDbType.VarChar, 50);
                    _command.Parameters.Add("@opd_K", OleDbType.Double);
                    _command.Parameters.Add("@opd_quota", OleDbType.Double);
                    _command.Parameters.Add("@opd_maxquota", OleDbType.Double);
                    _command.Parameters.Add("@opd_ke", OleDbType.Double);
                    _command.Parameters.Add("@opd_ze", OleDbType.VarChar, 50);
                    _command.Parameters.Add("@opd_bold", OleDbType.Integer);

                    // заполняем таблицу заказов
                    foreach (DataRow _row in _table.Rows)
                    {
                        _command.Parameters[0].Value = _doc_id;
                        _command.Parameters[1].Value = Convert.ToInt32(_row[0]);
                        _command.Parameters[2].Value = _row[1].ToString();
                        _command.Parameters[3].Value = _row[2].ToString();
                        _command.Parameters[4].Value = Convert.ToDecimal(_row[3]);
                        _command.Parameters[5].Value = Convert.ToDecimal(_row[4]);
                        _command.Parameters[6].Value = Convert.ToDecimal(_row[5]);
                        _command.Parameters[7].Value = Convert.ToDecimal(_row[6]);
                        _command.Parameters[8].Value = _row[7].ToString();
                        _command.Parameters[9].Value = Convert.ToInt32(_row[8]);
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
            return _doc_id;
        }

        /// <summary>
        /// Добавляем рекламу
        /// </summary>
        /// <param name="doc_type_id"></param>
        /// <param name="status_id"></param>
        /// <returns></returns>
        public int AdvAdd(int doc_type_id, int teremok_id, int status_id, string comment)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            DataTable _table;
            string _str_command;
            int _doc_id;
            CBData _data = new CBData();

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;


                _str_command = "INSERT INTO t_Doc(doc_type_id, doc_teremok_id, doc_status_id,doc_desc) VALUES (@doc_type_id, @doc_teremok_id, @doc_status_id,@doc_desc)";

                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_type_id", doc_type_id);
                _command.Parameters.AddWithValue("@doc_teremok_id", teremok_id);
                _command.Parameters.AddWithValue("@doc_status_id", status_id);
                _command.Parameters.AddWithValue("@doc_desc", comment);


                _command.ExecuteNonQuery();

                // id документа
                _command.CommandText = "SELECT @@IDENTITY";
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());

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
            return _doc_id;
        }


        /// <summary>
        /// Загрузка входящей накладной (создать документ в базе)
        /// </summary>
        /// <param name="teremok_id">id кто принимает накладную</param>
        /// <param name="teremok2_id">id кто отправляет накладную</param>
        /// <param name="date">дата накладной</param>
        /// <param name="code_1C">код 1С наклданой</param>
        /// <returns></returns>
        public int TransferAdd(int teremok_id, int teremok2_id, DateTime date, string code_1C)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            CBData _data = new CBData();
            int _doc_id;
            string doc_unique = code_1C + "_" + date.ToShortDateString();

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;

                // проверим, есть ли уже этот документ у нас
                _command.CommandText = "SELECT TOP 1 doc_id FROM t_Doc WHERE doc_1C='" + doc_unique + "'";
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id != 0)
                {
                    // удалим все записи из документа
                    _command.CommandText = "DELETE FROM t_Order2ProdDetails WHERE opd_doc_id=" + _doc_id.ToString(); ;
                    _command.ExecuteNonQuery();
                }
                else
                {
                    // создать новый документ
                    _command.CommandText = "INSERT INTO t_Doc(doc_type_id, doc_datetime, doc_status_id, doc_desc, doc_teremok_id, doc_teremok2_id, doc_1C) "
                    + " VALUES (15, @doc_datetime, 41, @doc_desc, @doc_teremok_id, @doc_teremok2_id, @doc_1C)";

                    OleDbParameter parm = new OleDbParameter("@doc_datetime", OleDbType.Date);
                    parm.Value = date;
                    _command.Parameters.Add(parm);

                    // _command.Parameters.AddWithValue("@doc_datetime", date.ToShortDateString());
                    _command.Parameters.AddWithValue("@doc_desc", "Отправитель: " + _data.GetTeremokName(teremok2_id));
                    _command.Parameters.AddWithValue("@doc_teremok_id", teremok_id);
                    _command.Parameters.AddWithValue("@doc_teremok2_id", teremok2_id);
                    _command.Parameters.AddWithValue("@doc_1C", doc_unique);

                    _command.ExecuteNonQuery();

                    // id документа
                    _command.CommandText = "SELECT @@IDENTITY";
                    _doc_id = Convert.ToInt32(_command.ExecuteScalar());
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

        private string AttachZeroToDate(int c)
        {
            if (c < 10)
                return "0" + c.ToString();
            else
                return c.ToString();
        }

        public void TransferAddItem(string code_nome, string quantity, int doc_id, int teremok_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            string _str_command;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;

                // получить параметры номенлатуры
                // получить список номенлатуры
                _str_command = "SELECT TOP 1 nome_id, n2t_ed, n2t_bed, n2t_K FROM t_Nomenclature " +
                    " INNER JOIN t_Nome2Teremok ON t_Nomenclature.nome_id = t_Nome2Teremok.n2t_nome_id WHERE n2t_nt_id = 15 "
                        + " AND n2t_teremok_id=" + teremok_id.ToString() + " AND nome_1C ='" + code_nome + "'";

                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                DataTable _table = new DataTable("t_Nomenclature");
                _data_adapter.Fill(_table);

                // создать новый документ
                _command.CommandText = "INSERT INTO t_Order2ProdDetails(opd_doc_id, opd_nome_id, opd_order, opd_order2, opd_order3, opd_ed, opd_bed, opd_K, opd_quota, opd_maxquota) "
                + " VALUES (@opd_doc_id, @opd_nome_id, @opd_order, 0, 0, @opd_ed, @opd_bed, @opd_K, 0, 0)";

                // заполняем таблицу заказов
                foreach (DataRow _row in _table.Rows)
                {
                    _command.Parameters.AddWithValue("@opd_doc_id", doc_id);
                    _command.Parameters.AddWithValue("@opd_nome_id", _row[0].ToString());
                    _command.Parameters.AddWithValue("@opd_order", CUtilHelper.ParceAmount(quantity, 1));
                    _command.Parameters.AddWithValue("@opd_ed", _row[1].ToString());
                    _command.Parameters.AddWithValue("@opd_bed", _row[2].ToString());
                    _command.Parameters.AddWithValue("@opd_K", _row[3].ToString());
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

        public void NomeCheckInDB(string nome_1C, string nome_name, string nome_ue, string nome_be, string nome_k, int teremok_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            string _str_command;
            string _nome_id;
            try
            {
                // проверка, есть такая номенклатура в базе 
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT count(*) FROM t_Nomenclature " +
                   " INNER JOIN t_Nome2Teremok ON t_Nomenclature.nome_id = t_Nome2Teremok.n2t_nome_id WHERE n2t_nt_id = 15 "
                       + " AND n2t_teremok_id=@n2t_teremok_id AND nome_1C = @nome_1C";

                _command.Parameters.AddWithValue("@n2t_teremok_id", teremok_id);
                _command.Parameters.AddWithValue("@nome_1C", nome_1C);

                _command.CommandText = _str_command;
                if (Convert.ToInt32(_command.ExecuteScalar()) == 0)
                {
                    // новая запись
                    _str_command = "INSERT INTO t_nomenclature (nome_1C, nome_name, nome_ed, nome_bed, nome_K) VALUES(@nome_1C, @nome_name, @nome_ed, @nome_bed, @nome_K)";
                    _command.Parameters.Clear();
                    _command.Parameters.AddWithValue("@nome_1C", nome_1C);
                    _command.Parameters.AddWithValue("@nome_name", nome_name);
                    _command.Parameters.AddWithValue("@nome_ed", nome_ue);
                    _command.Parameters.AddWithValue("@nome_bed", nome_be);
                    _command.Parameters.AddWithValue("@nome_K", CUtilHelper.ParceAmount(nome_k, 1));
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();

                    _str_command = "SELECT @@IDENTITY";
                    _command.CommandText = _str_command;
                    _nome_id = _command.ExecuteScalar().ToString();

                    // вставляем в шаблон
                    _str_command = "INSERT INTO t_Nome2Teremok (n2t_teremok_id, n2t_nome_id, n2t_nt_id, n2t_ed, n2t_bed, n2t_K, n2t_quota, n2t_maxquota) VALUES(@n2t_teremok_id, @n2t_nome_id, @n2t_nt_id, @n2t_ed, @n2t_bed, @n2t_K, @n2t_quota, @n2t_maxquota)";
                    _command.Parameters.Clear();
                    _command.Parameters.AddWithValue("@n2t_teremok_id", teremok_id);
                    _command.Parameters.AddWithValue("@n2t_nome_id", _nome_id);
                    _command.Parameters.AddWithValue("@n2t_nt_id", 15);
                    _command.Parameters.AddWithValue("@n2t_ed", nome_ue);
                    _command.Parameters.AddWithValue("@n2t_bed", nome_be);
                    _command.Parameters.AddWithValue("@n2t_K", CUtilHelper.ParceAmount(nome_k, 1));
                    _command.Parameters.AddWithValue("@n2t_quota", 0);
                    _command.Parameters.AddWithValue("@n2t_maxquota", 0);
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

        public int Order2ProdAddItem(int doc_type_id, int teremok_id, int doc_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            DataTable _table;
            string _str_command;
            int result = 0;
            int _doc_id = doc_id;
            CBData _data = new CBData();

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;

                _str_command = "SELECT nome_id, n2t_ed, n2t_bed, n2t_K, n2t_quota, n2t_maxquota, n2t_ke, n2t_ze, n2t_bold FROM t_Nomenclature " +
                        " INNER JOIN t_Nome2Teremok ON t_Nomenclature.nome_id = t_Nome2Teremok.n2t_nome_id WHERE n2t_nt_id = "
                            + doc_type_id.ToString() + " AND n2t_teremok_id=" + teremok_id.ToString() + " ORDER BY n2t_id ASC";

                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("t_Nomenclature");
                _data_adapter.Fill(_table);

                {
                    _str_command = "INSERT INTO t_Order2ProdDetails(opd_doc_id, opd_nome_id, opd_ed, opd_bed, opd_K, opd_quota, opd_maxquota, opd_ke, opd_ze, opd_bold) VALUES (@opd_doc_id, @opd_nome_id, @opd_ed, @opd_bed, @opd_K, @opd_quota, @opd_maxquota, @opd_ke, @opd_ze, @opd_bold)";
                    _command.CommandText = _str_command;
                    _command.Parameters.Clear();
                    _command.Parameters.Add("@opd_doc_id", OleDbType.Integer);
                    _command.Parameters.Add("@opd_nome_id", OleDbType.Integer);
                    _command.Parameters.Add("@opd_ed", OleDbType.VarChar, 50);
                    _command.Parameters.Add("@opd_bed", OleDbType.VarChar, 50);
                    _command.Parameters.Add("@opd_K", OleDbType.Double);
                    _command.Parameters.Add("@opd_quota", OleDbType.Double);
                    _command.Parameters.Add("@opd_maxquota", OleDbType.Double);
                    _command.Parameters.Add("@opd_ke", OleDbType.Double);
                    _command.Parameters.Add("@opd_ze", OleDbType.VarChar, 50);
                    _command.Parameters.Add("@opd_bold", OleDbType.Integer);

                    // заполняем таблицу заказов
                    foreach (DataRow _row in _table.Rows)
                    {
                        _command.Parameters[0].Value = _doc_id;
                        _command.Parameters[1].Value = Convert.ToInt32(_row[0]);
                        _command.Parameters[2].Value = _row[1].ToString();
                        _command.Parameters[3].Value = _row[2].ToString();
                        _command.Parameters[4].Value = Convert.ToDecimal(_row[3]);
                        _command.Parameters[5].Value = Convert.ToDecimal(_row[4]);
                        _command.Parameters[6].Value = Convert.ToDecimal(_row[5]);
                        _command.Parameters[7].Value = Convert.ToDecimal(_row[6]);
                        _command.Parameters[8].Value = _row[7].ToString();
                        _command.Parameters[9].Value = Convert.ToInt32(_row[8]);
                        _command.ExecuteNonQuery();
                    }

                    _str_command = "SELECT @@IDENTITY";
                    _command.CommandText = _str_command;
                    result = Convert.ToInt32(_command.ExecuteScalar());
                }
                return result;
            }
            catch (Exception _exp)
            {
                return result;
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();

            }
        }


        public void Order2ProdUpdateItem(string opd_id, string opd_order, string opd_order2, string reason, string opd_order3, string opd_total1, string opd_time)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_Order2ProdDetails SET opd_order = @opd_order, opd_order2 = @opd_order2, opd_rerreason_desc = @opd_rerreason_desc, opd_order3 = @opd_order3, opd_total = @opd_total, opd_retreason_id = @opd_retreason_id  WHERE opd_id = @opd_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@opd_order", CUtilHelper.ParceAmount(opd_order, 1));
                _command.Parameters.AddWithValue("@opd_order2", CUtilHelper.ParceAmount(opd_order2, 1));
                _command.Parameters.AddWithValue("@opd_rerreason_desc", reason);
                _command.Parameters.AddWithValue("@opd_order3", CUtilHelper.ParceAmount(opd_order3, 1));
                _command.Parameters.AddWithValue("@opd_total", CUtilHelper.ParceAmount(opd_total1, 1));
                _command.Parameters.AddWithValue("@opd_retreason_id", opd_time);
                _command.Parameters.AddWithValue("@opd_id", opd_id);
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

        public void Order2ProdDeleteItem(string opd_id)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "DELETE from t_Order2ProdDetails WHERE opd_id = @opd_id";
                _command.CommandText = _str_command;

                _command.Parameters.AddWithValue("@opd_id", opd_id);
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

        public void Order2ProdUpdateItemReason(int opd_id, string orr_1C)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_Order2ProdDetails SET opd_retreason_id = @opd_retreason_id WHERE opd_id = @opd_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@opd_retreason_id", orr_1C);
                _command.Parameters.AddWithValue("@opd_id", opd_id);
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

        public void Order2ProdDeleteItemReason(int opd_id)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_Order2ProdDetails SET opd_retreason_id = null WHERE opd_id = @opd_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@opd_id", opd_id);
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

        // получить список неотправленных документов
        //public DataTable GetUnsendedDoc(int doc_type_id)
        //{
        //    //
        //    string _str_connect = CParam.ConnString;
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command;
        //    DataTable _table;
        //    string _str_command;

        //    try
        //    {
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        _command = new OleDbCommand();                
        //        // получить список документов
        //        _str_command = "SELECT doc_id, doc_datetime FROM t_Doc WHERE doc_status_id = 1 AND doc_type_id = " + doc_type_id.ToString();
        //        OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
        //        _table = new DataTable("t_Nomenclature");
        //        _data_adapter.Fill(_table);
        //        return _table;
        //    }
        //    catch (Exception _exp)
        //    {
        //        throw _exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //    }            
        //}


        // экспорт данных по заказу
        public DataTable ExportOrder(int doc_id, bool is_sent_null)
        {
            //
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
                if (is_sent_null)
                    _str_command = "SELECT t_Nomenclature.nome_1C AS c, t_Order2ProdDetails.opd_order AS q, t_Order2ProdDetails.opd_doc_id AS i, t_Order2ProdDetails.opd_order2 AS dv, t_Order2ProdDetails.opd_retreason_id AS d, t_Order2ProdDetails.opd_rerreason_desc AS g, t_Order2ProdDetails.opd_order3 AS di, t_Order2ProdDetails.opd_total AS dt FROM t_Nomenclature INNER JOIN t_Order2ProdDetails ON t_Nomenclature.nome_id = t_Order2ProdDetails.opd_nome_id WHERE t_Order2ProdDetails.opd_doc_id = " + doc_id.ToString();
                else
                    _str_command = "SELECT t_Nomenclature.nome_1C AS c, t_Order2ProdDetails.opd_order AS q, t_Order2ProdDetails.opd_doc_id AS i, t_Order2ProdDetails.opd_order2 AS dv, t_Order2ProdDetails.opd_retreason_id AS d, t_Order2ProdDetails.opd_rerreason_desc AS g, t_Order2ProdDetails.opd_order3 AS di, t_Order2ProdDetails.opd_total AS dt FROM t_Nomenclature INNER JOIN t_Order2ProdDetails ON t_Nomenclature.nome_id = t_Order2ProdDetails.opd_nome_id WHERE (t_Order2ProdDetails.opd_order <> 0 OR t_Order2ProdDetails.opd_order2 <> 0 OR t_Order2ProdDetails.opd_order3 <> 0 OR t_Order2ProdDetails.opd_total <> 0) AND t_Order2ProdDetails.opd_doc_id = " + doc_id.ToString();
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("o2p");
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

        public DataTable ExportOrder913(int doc_id, bool is_sent_null)
        {
            //
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

                if (StaticConstants.UpdateHelper.IsNewerVersion("5.6.4.0"))
                {
                    _str_command = "SELECT t_Nomenclature.nome_1C AS c, t_Order2ProdDetails.opd_order AS q, t_Order2ProdDetails.opd_doc_id AS i, t_Order2ProdDetails.opd_order2 AS dv, t_Order2ProdDetails.opd_retreason_id AS d, t_Order2ProdDetails.opd_rerreason_desc AS g, t_Order2ProdDetails.opd_order3 AS di, t_Order2ProdDetails.opd_total AS dt, t_UtilReasonRef.id_1c AS sp " +
    "FROM ((t_Nomenclature INNER JOIN t_Order2ProdDetails ON t_Nomenclature.nome_id = t_Order2ProdDetails.opd_nome_id) LEFT JOIN t_Utilization ON (t_Order2ProdDetails.opd_doc_id = t_Utilization.util_doc_id) AND (t_Order2ProdDetails.opd_nome_id = t_Utilization.util_nome_id)) LEFT JOIN t_UtilReasonRef ON t_Utilization.util_reason_id = t_UtilReasonRef.urr_id " +
     "WHERE (((t_Order2ProdDetails.opd_order)<>0) AND ((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ")) OR (((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ") AND ((t_Order2ProdDetails.opd_order2)<>0)) OR (((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ") AND ((t_Order2ProdDetails.opd_order3)<>0)) OR (((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ") AND ((t_Order2ProdDetails.opd_total)<>0)); ";
                }
                else
                {
                    _str_command = "SELECT t_Nomenclature.nome_1C AS c, t_Order2ProdDetails.opd_order AS q, t_Order2ProdDetails.opd_doc_id AS i, t_Order2ProdDetails.opd_order2 AS dv, t_Order2ProdDetails.opd_retreason_id AS d, t_Order2ProdDetails.opd_rerreason_desc AS g, t_Order2ProdDetails.opd_order3 AS di, t_Order2ProdDetails.opd_total AS dt, t_UtilReasonRef.id_1c AS sp " +
                                    "FROM (t_Nomenclature INNER JOIN t_Order2ProdDetails ON t_Nomenclature.nome_id = t_Order2ProdDetails.opd_nome_id) LEFT JOIN (t_Utilization LEFT JOIN t_UtilReasonRef ON t_Utilization.util_reason_id = t_UtilReasonRef.urr_id) ON t_Order2ProdDetails.opd_id = t_Utilization.util_opd_id " +
                                    "WHERE (((t_Order2ProdDetails.opd_order)<>0) AND ((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ")) OR (((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ") AND ((t_Order2ProdDetails.opd_order2)<>0)) OR (((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ") AND ((t_Order2ProdDetails.opd_order3)<>0)) OR (((t_Order2ProdDetails.opd_doc_id)=" + doc_id.ToString() + ") AND ((t_Order2ProdDetails.opd_total)<>0));";
                }

                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("o2p");
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

        /// <summary>
        /// Обновляем статус документа
        /// </summary>
        /// <param name="doc_id">id документа</param>
        /// <param name="state_id">тип статуса</param>
        /// <param name="oper">описание</param>
        public void ОбновитьСтатусДокумента(OrderClass order, int state_id, string oper)
        {
            DocUpdateState(order.Id, GetStatusID(null,order.CurrentDocument.doc_type_id, state_id), oper); // откатить на новый статус
        }

        public void DocUpdateState(int doc_id, int state_id, string oper)
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
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_Doc SET doc_status_id = @doc_status_id, doc_desc=' " + oper + DateTime.Now.ToShortDateString() + " в " + DateTime.Now.ToShortTimeString() + "' WHERE doc_id = @doc_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_status_id", state_id);
                _command.Parameters.AddWithValue("@doc_id", doc_id);
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

        public int _GetDocStatusType(t_Doc doc)
        {
            t_DocStatusRef status = new t_DocStatusRef().SelectFirst<t_DocStatusRef>("doctype_id=" + doc.doc_type_id +
                " AND docstatusref_id=" + doc.doc_status_id);

            if (status != null)
            {
                return status.statustype_id;
            }
            return -1;
        }

        public t_Doc DocUpdateDocStateNew(int doc_id, int statustype_id, string oper)
        {
            t_Doc doc = new t_Doc().SelectFirst<t_Doc>("doc_id=" + doc_id);
            if (doc != null)
            {
                t_DocStatusRef status = new t_DocStatusRef().SelectFirst<t_DocStatusRef>("doctype_id=" + doc.doc_type_id +
                    " AND statustype_id=" + statustype_id);
                if (status != null)
                {
                    doc.doc_status_id = status.docstatusref_id;
                    doc.doc_desc = oper + DateTime.Now.ToShortDateString() + " в " + DateTime.Now.ToShortTimeString();
                    doc.Update();
                }
                return doc;
            }
            return null;
        }
        public void DocUpdateStateMark(int doc_id, int state_id, string oper)
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
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_Doc SET doc_status_id = @doc_status_id, doc_desc=' " + oper + DateTime.Now.ToShortDateString() + " в " + DateTime.Now.ToShortTimeString() + "' WHERE doc_id = @doc_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_status_id", state_id);
                _command.Parameters.AddWithValue("@doc_id", doc_id);
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


        //public void ImportNomeFromFile()
        //{
        //    //
        //    string _str_connect = CParam.ConnString;
        //    string _str_command;
        //    OleDbConnection _conn = null;
        //    OleDbCommand _command = null;

        //    StreamReader _sr = null;
        //    string _line;
        //    char[] _separator = ";".ToCharArray();
        //    string[] _s;

        //    try
        //    {
        //        _sr = new StreamReader(CParam.AppFolder + "\\Data\\3.txt", System.Text.Encoding.GetEncoding(1251));
        //        _conn = new OleDbConnection(_str_connect);
        //        _conn.Open();
        //        // обновить запись
        //        _command = new OleDbCommand();
        //        _command.Connection = _conn;
        //        _str_command = "INSERT INTO t_Nomenclature(nome_name, nome_ed, nome_K, nome_bed, nome_1C, nome_type_id, nome_is_active) VALUES (@nome_name, @nome_ed, @nome_K, @nome_bed, @nome_1C, 2, 1)";
        //        _command.CommandText = _str_command;
        //        _command.Parameters.Add("@nome_name", OleDbType.VarChar);
        //        _command.Parameters.Add("@nome_ed", OleDbType.VarChar);
        //        _command.Parameters.Add("@nome_K", OleDbType.Double);
        //        _command.Parameters.Add("@nome_bed", OleDbType.VarChar);
        //        _command.Parameters.Add("@nome_1C", OleDbType.VarChar);

        //        while ((_line = _sr.ReadLine()) != null)
        //        {
        //            _s = _line.Split(_separator);
        //            _command.Parameters[0].Value = _s[0];
        //            _command.Parameters[1].Value = _s[1];
        //            _command.Parameters[2].Value = _s[2];
        //            _command.Parameters[3].Value = _s[3];
        //            _command.Parameters[4].Value = _s[0];

        //            _command.ExecuteNonQuery();
        //        }                              
        //    }
        //    catch (Exception _exp)
        //    {
        //        throw _exp;
        //    }
        //    finally
        //    {
        //        if (_conn != null)
        //            _conn.Close();
        //        if (_sr != null)
        //            _sr.Close();
        //    }
        //}


        // список ресторанов
        public DataTable TeremokView(OleDbConnection conn)
        {
            //
            string _str_connect = CParam.ConnString;

            string _str_command = "SELECT teremok_id, teremok_name, teremok_1c FROM t_Teremok WHERE teremok_current = TRUE";
            //string _str_command = "SELECT teremok_id, teremok_name, teremok_1c FROM t_Teremok";
            DataTable _table;

            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);

                _table = new DataTable("Teremok");
                _data_adapter.Fill(_table);
                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        // список ресторанов и теремков для перемещения
        public DataTable TeremokFullList()
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            string _str_command = "SELECT teremok_id, teremok_name FROM t_Teremok ORDER BY teremok_name ASC";
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("Teremok");
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



        // список товара для добовления
        public DataTable NomenclatureFullList(int doc_id)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;

            string _str_command = "SELECT DISTINCT t_Nomenclature.nome_id, t_Nomenclature.nome_name, t_Nome2Teremok.n2t_nome_id, t_Nome2Teremok.n2t_teremok_id FROM t_Nomenclature INNER JOIN t_Nome2Teremok ON t_Nomenclature.nome_id = t_Nome2Teremok.n2t_nome_id WHERE t_Nome2Teremok.n2t_teremok_id=" + GetTeremokIDByDocID(doc_id).ToString() + " ORDER BY t_Nomenclature.nome_name ASC";

            DataTable _table;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("Nomenclature");
                _data_adapter.Fill(_table);



                DataRow[] dr_a1 = new DataRow[_table.Rows.Count];
                _table.Rows.CopyTo(dr_a1, 0);

                List<DataRow> dr_list = dr_a1.ToList();
                List<DataRow> dr_to_remove = new List<DataRow>();

                dr_list.ForEach(a =>
                {
                    List<DataRow> drlist = dr_list.Where(b => CellHelper.FindCell(b, "nome_name").ToString() == CellHelper.FindCell(a, "nome_name").ToString()).ToList<DataRow>();
                    drlist.Sort((c, d) => ((int)CellHelper.FindCell(d, "nome_id")).CompareTo((int)CellHelper.FindCell(c, "nome_id")));

                    List<DataRow> drlist1 = drlist.GetRange(1, drlist.Count - 1);
                    drlist1.ForEach(b => { if (!dr_to_remove.Contains(b))dr_to_remove.Add(b); });

                });

                dr_to_remove.ForEach(a => _table.Rows.Remove(a));
                //_table.Rows.Remove

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

        // список причин возврата
        public DataTable ReasonFullList()
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            string _str_command = "SELECT orr_1C, orr_name FROM t_OrderRetReason ORDER BY orr_1C ASC";
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("OrderRetReason");
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

        public string TeremokName(int teremok_id)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                // формируем комме
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // создать новый документ
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT teremok_name FROM t_Teremok WHERE teremok_id = @teremok_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@teremok_id", teremok_id);
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


        // список видов документов
        public DataTable DocRef(OleDbConnection conn)
        {
            string _str_command = "SELECT doctype_name FROM t_DocTypeRef WHERE doctype_name <> '0' order by doctype_name";
            DataTable _table;

            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);

                _table = new DataTable("t_DocTypeRef");
                _data_adapter.Fill(_table);
                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        // список документов для отправки
        public DataTable GetDoc2Send()
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            string _str_command = "";
            if (StaticConstants.UpdateHelper.IsNewerVersion("5.6.4.2"))
            {
                _str_command = "SELECT doctype_name, doc_id, teremok_name, doc_datetime FROM t_Teremok INNER JOIN (t_DocTypeRef INNER JOIN (t_DocStatusRef INNER JOIN t_Doc ON t_DocStatusRef.docstatusref_id = t_Doc.doc_status_id) ON t_DocTypeRef.doctype_id = t_Doc.doc_type_id) ON t_Teremok.teremok_id = t_Doc.doc_teremok_id WHERE statustype_id = 1";
            }
            else
            {
                _str_command = "SELECT t_DocTypeRef.doctype_name, t_Doc.doc_id, t_Teremok.teremok_name, t_Doc.doc_datetime, t_DocTypeRef.sendtype_type " +
                               "FROM t_Teremok INNER JOIN (t_DocTypeRef INNER JOIN (t_DocStatusRef INNER JOIN t_Doc ON t_DocStatusRef.docstatusref_id = t_Doc.doc_status_id) ON t_DocTypeRef.doctype_id = t_Doc.doc_type_id) ON t_Teremok.teremok_id = t_Doc.doc_teremok_id " +
                               "WHERE (((t_DocStatusRef.[statustype_id])=1) AND ((t_DocTypeRef.sendtype_type)<>0)); ";
            }
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("Doc");
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

        // список папок
        public DataTable GetTeremokFolders()
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            string _str_command = "SELECT teremok_1C FROM t_teremok WHERE teremok_current = TRUE";
            //string _str_command = "SELECT teremok_1C FROM t_teremok";
            DataTable _table;
            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("TeremokFolders");
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

        // список документов для отправки
        public DataTable GetDoc2SendExch(string teremok_id)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            string _str_command = "SELECT t_TaskExchange.[task_doc_id], t_TaskExchange.task_state_id, t_Doc.doc_teremok_id FROM t_Doc INNER JOIN t_TaskExchange ON t_Doc.doc_id = t_TaskExchange.task_doc_id WHERE t_TaskExchange.task_state_id = 0 AND t_Doc.doc_teremok_id=" + teremok_id;
            DataTable _table;
            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("TaskExchange");
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

        // список документов для отправки
        public DataTable GetDoc2SendExch(string teremok_id, int send_type)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            string _str_command = "SELECT t_TaskExchange.task_doc_id, t_TaskExchange.task_state_id, t_Doc.doc_teremok_id, t_DocTypeRef.sendtype_type " +
                                  "FROM (t_DocTypeRef INNER JOIN (t_Doc INNER JOIN t_TaskExchange ON t_Doc.doc_id = t_TaskExchange.task_doc_id) ON t_DocTypeRef.doctype_id = t_Doc.doc_type_id) LEFT JOIN t_DocSendTypeRef ON t_DocTypeRef.sendtype_type = t_DocSendTypeRef.sendtype_type " +
                                  "WHERE (((t_TaskExchange.task_state_id)=0) AND ((t_Doc.doc_teremok_id)=" + teremok_id + ") AND ((t_DocTypeRef.sendtype_type)=" + send_type + "));";
            DataTable _table;
            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("TaskExchange");
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

        public int GetDocID2Send()
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                // формируем комме
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // создать новый документ
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT TOP 1 task_doc_id FROM t_TaskExchange WHERE task_state_id = 0";
                _command.CommandText = _str_command;
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

        public int GetTypeDoc(int doc_id)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                // формируем комме
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // создать новый документ
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT doc_type_id FROM t_Doc WHERE doc_id = @doc_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_id", doc_id);
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

        public string GetTypeNameDoc(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                // создать новый документ
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT doctype_name FROM t_Teremok INNER JOIN (t_DocTypeRef INNER JOIN (t_DocStatusRef INNER JOIN t_Doc ON t_DocStatusRef.docstatusref_id = t_Doc.doc_status_id) ON t_DocTypeRef.doctype_id = t_Doc.doc_type_id) ON t_Teremok.teremok_id = t_Doc.doc_teremok_id WHERE doc_id = @doc_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_id", doc_id);
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        /// <summary>
        /// получить код теремка, КУДА отправляется накладная
        /// </summary>
        /// <param name="doc_id"></param>
        /// <returns></returns>
        public int GetTeremok2Doc(int doc_id)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                // формируем комме
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // создать новый документ
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT doc_teremok2_id FROM t_doc WHERE doc_id = @doc_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_id", doc_id);
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


        public DateTime GetDateDoc(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                // формируем комме
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // создать новый документ
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT doc_datetime FROM t_Doc WHERE doc_id = @doc_id";
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

        public int GetTeremokIDByDocID(int doc_id)
        {

            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                // формируем комме
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // создать новый документ
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT doc_teremok_id FROM t_Doc WHERE doc_id = @doc_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_id", doc_id);
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

        // подтверждение примема товара
        public void TaskUpdateState(int _doc_id, int _state_id, string message, bool update_message)
        {

            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            string _task_id;

            try
            {
                // формируем комме
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // получить ID задачи
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT TOP 1 task_id FROM t_TaskExchange WHERE task_doc_id = " + _doc_id.ToString() + " ORDER BY task_id DESC";
                _command.CommandText = _str_command;
                _task_id = _command.ExecuteScalar().ToString();

                // обновить статус задачи
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_TaskExchange SET task_state_id = 1, task_datetime_completed=Now() WHERE task_id=" + _task_id;
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

                if (update_message)
                    _str_command = "UPDATE t_Doc SET doc_status_id = @doc_status_id,  doc_desc='" + message + DateTime.Now.ToShortDateString() + " в " + DateTime.Now.ToShortTimeString() + "' WHERE doc_id=@doc_id";
                else
                    _str_command = "UPDATE t_Doc SET doc_status_id = @doc_status_id,  doc_desc= doc_desc + ' " + message + DateTime.Now.ToShortDateString() + " в " + DateTime.Now.ToShortTimeString() + "' WHERE doc_id=@doc_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_status_id", _state_id);
                _command.Parameters.AddWithValue("@doc_id", _doc_id);

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

        // удаление заказа
        public void OrderDelete(int _doc_id)
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
                // удалить записи
                _command = new OleDbCommand();
                _command.Connection = _conn;

                _str_command = "DELETE FROM t_TaskExchange WHERE task_doc_id=" + _doc_id.ToString();
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

                _str_command = "DELETE FROM t_Order2ProdDetails WHERE opd_doc_id=" + _doc_id.ToString();
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

        public string GetTeremokFolder(int teremok_id)
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
                _str_command = "SELECT teremok_1C FROM t_Teremok WHERE teremok_id=" + teremok_id.ToString();
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

        // получить код подразделения, в который происходит перемещение
        public string GetTeremokTransferTo(int doc_id)
        {
            //
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            string _teremok_to_id;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT doc_teremok2_id FROM t_Doc WHERE doc_id=" + doc_id.ToString();
                _command.CommandText = _str_command;
                _teremok_to_id = _command.ExecuteScalar().ToString();

                _str_command = "SELECT teremok_1C FROM t_Teremok WHERE teremok_id=" + _teremok_to_id;
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


        public int GetTeremokID(string teremok_name)
        {
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
                _str_command = "SELECT teremok_id FROM t_Teremok WHERE teremok_name='" + teremok_name + "'";
                _command.CommandText = _str_command;
                return Convert.ToInt32(_command.ExecuteScalar().ToString());
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

        public string GetTeremokName(int teremok_id)
        {
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
                _str_command = "SELECT teremok_name FROM t_Teremok WHERE teremok_id=" + teremok_id.ToString();
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


        public string GetTeremo1CByID(int teremok_id)
        {
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
                _str_command = "SELECT teremok_1C FROM t_Teremok WHERE teremok_id=" + teremok_id.ToString();
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

        public string GetDocCode1C(int doc_id)
        {
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
                _str_command = "SELECT doc_1C FROM t_doc WHERE doc_id=" + doc_id.ToString();
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


        public string GetDateInkass(int doc_id)
        {
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
                _str_command = "SELECT max(task_datetime) FROM t_TaskExchange WHERE task_doc_id=" + doc_id.ToString();
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

        public string GetDateCreate(int doc_id)
        {
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
                _str_command = "SELECT doc_datetime FROM t_Doc WHERE doc_id=" + doc_id.ToString();
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

        public int getBanid(int doc_type_id)
        {
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
                _str_command = "SELECT DISTINCT n2t_ban FROM t_Nome2Teremok WHERE n2t_nt_id=" + doc_type_id;
                _command.CommandText = _str_command;
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

        public int GetTeremokDep(int teremok_id)
        {
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
                _str_command = "SELECT teremok_dep FROM t_Teremok WHERE teremok_id=" + teremok_id.ToString();
                _command.CommandText = _str_command;
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

        public int GetNomeShift()
        {
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
                _str_command = "SELECT * FROM t_Nome2Teremok WHERE n2t_shift = '1'";
                _command.CommandText = _str_command;
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

        public string GetTeremokIDBy1C(string teremok_1C)
        {
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
                _str_command = "SELECT teremok_id FROM t_Teremok WHERE teremok_1C='" + teremok_1C + "'";
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

        public void ImportTeremok(OleDbConnection conn, string teremok_id, string teremok_name, string teremok_1C, string teremok_dep)
        {
            if (teremok_id == "")
                return;

            string _str_command;
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT count(*) FROM t_teremok WHERE teremok_id=" + teremok_id;
                _command.CommandText = _str_command;
                if (Convert.ToInt32(_command.ExecuteScalar()) == 0)
                {
                    // новая запись
                    _str_command = "INSERT INTO t_Teremok (teremok_id, teremok_name, teremok_1C, teremok_dep) VALUES(@teremok_id, @teremok_name, @teremok_1C, @teremok_dep)";
                    _command.Parameters.AddWithValue("@teremok_id", teremok_id);
                    _command.Parameters.AddWithValue("@teremok_name", teremok_name);
                    _command.Parameters.AddWithValue("@teremok_1C", teremok_1C);
                    _command.Parameters.AddWithValue("@teremok_dep", teremok_dep);
                }
                else
                {
                    // обновление 
                    _str_command = "UPDATE t_Teremok SET teremok_name = @teremok_name, teremok_1C = @teremok_1C, teremok_dep = @teremok_dep WHERE teremok_id = @teremok_id";
                    _command.Parameters.AddWithValue("@teremok_name", teremok_name);
                    _command.Parameters.AddWithValue("@teremok_1C", teremok_1C);
                    _command.Parameters.AddWithValue("@teremok_id", teremok_id);
                    _command.Parameters.AddWithValue("@teremok_dep", teremok_dep);

                }
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public void ImportCash(OleDbConnection conn, string t_name, string t_path_name, string t_hash_code, string t_creation_time)
        {
            string _str_command;
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "INSERT INTO t_Standart (t_name, t_path_name, t_hash_code, t_creation_time) VALUES(@t_name, @t_path_name, @t_hash_code, @t_creation_time)";
                _command.Parameters.AddWithValue("@t_name", t_name);
                _command.Parameters.AddWithValue("@t_path_name", t_path_name);
                _command.Parameters.AddWithValue("@t_hash_code", t_hash_code);
                _command.Parameters.AddWithValue("@t_creation_time", t_creation_time);
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public void ImportVideo(OleDbConnection conn, string t_release, string t_switch, string t_fileName, string t_screenName, string t_category)
        {
            string _str_command;
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "INSERT INTO t_DocReestr (t_doc_release, t_doc_switch, t_doc_filename, t_doc_screenName, t_doc_category) VALUES (@t_doc_release, @t_doc_switch, @t_doc_filename, @t_doc_screenName, @t_doc_category)";
                _command.Parameters.AddWithValue("@t_doc_release", t_release);
                _command.Parameters.AddWithValue("@t_doc_switch", t_switch);
                _command.Parameters.AddWithValue("@t_doc_filename", t_fileName);
                _command.Parameters.AddWithValue("@t_doc_screenName", t_screenName);
                _command.Parameters.AddWithValue("@t_doc_category", t_category);
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public void ImportNewDoc(OleDbConnection conn, string t_fileName, string t_screenName)
        {
            string _str_command;
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT count(*) FROM t_NewDoc WHERE t_filename='" + t_fileName + "' AND t_screenName = '" + t_screenName + "'";
                _command.CommandText = _str_command;
                if (Convert.ToInt32(_command.ExecuteScalar()) == 0)
                {
                    _str_command = "INSERT INTO t_NewDoc (t_filename, t_screenName) VALUES (@t_filename, @t_screenName)";
                    _command.Parameters.AddWithValue("@t_filename", t_fileName);
                    _command.Parameters.AddWithValue("@t_screenName", t_screenName);
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public bool existsFile(OleDbConnection conn, string t_fileName, string t_screenName)
        {
            string _str_command;
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT count(*) FROM t_NewDoc WHERE t_filename='" + t_fileName + "' AND t_screenName = '" + t_screenName + "'";
                _command.CommandText = _str_command;
                if (Convert.ToInt32(_command.ExecuteScalar()) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public bool existsFileSecondMonitor(OleDbConnection conn, string t_fileName, string t_screenName)
        {
            string _str_command;
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT count(*) FROM t_SecondMonitor WHERE FileName='" + t_fileName + "' AND Name = '" + t_screenName + "'";
                _command.CommandText = _str_command;
                if (Convert.ToInt32(_command.ExecuteScalar()) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public DataTable NewDoc()
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
                _str_command = "SELECT DISTINCT TOP 50 * FROM t_NewDoc ORDER BY id DESC";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("v_NewDoc");
                _data_adapter.Fill(_table);
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
            return _table;
        }


        public DataTable categoryVideo()
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
                _str_command = "SELECT DISTINCT t_doc_category FROM t_DocReestr WHERE t_doc_switch = 'video'";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("v_video");
                _data_adapter.Fill(_table);
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
            return _table;
        }

        public DataTable TreeReestr(string t_category)
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
                _str_command = "SELECT * FROM t_DocReestr WHERE t_doc_switch = 'doc' AND t_doc_category = '" + t_category + "'";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("v_doc");
                _data_adapter.Fill(_table);
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
            return _table;
        }

        public DataTable categoryReestr()
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
                _str_command = "SELECT DISTINCT t_doc_category FROM t_DocReestr WHERE t_doc_switch = 'doc'";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("v_reestr");
                _data_adapter.Fill(_table);
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
            return _table;
        }

        public DataTable categorySwitch()
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
                _str_command = "SELECT DISTINCT t_doc_switch FROM t_DocReestr";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("v_categorySwitch");
                _data_adapter.Fill(_table);
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
            return _table;
        }

        public DataTable TreeVideo(string t_category)
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
                _str_command = "SELECT * FROM t_DocReestr WHERE t_doc_switch = 'video' AND t_doc_category = '" + t_category + "'";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("v_video");
                _data_adapter.Fill(_table);
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
            return _table;
        }

        public string videoPath(string videoName)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            string query = "SELECT t_doc_filename FROM t_DocReestr WHERE t_doc_screenName = '" + videoName + "'";
            // string query = "SELECT t_doc_filename FROM t_DocReestr WHERE t_doc_category = '" + videoName + "'";
            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = query;
                _command.Parameters.AddWithValue("@t_doc_screenName", videoName);
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception _exp)
            {
                MDIParentMain.log.Error("Ошибка при обращении к таблице : " + query, _exp);
                throw _exp;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public string newDocPath(string videoName)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = "SELECT t_filename FROM t_NewDoc WHERE t_screenName = '" + videoName + "'";
                _command.Parameters.AddWithValue("@t_doc_screenName", videoName);
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

        public DataTable TreeVideoItems(string t_category, string t_switch)
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
                _str_command = "SELECT * FROM t_DocReestr WHERE t_doc_switch = '" + t_switch + "' AND t_doc_category = '" + t_category + "'";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("v_videoItems");
                _data_adapter.Fill(_table);
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
            return _table;
        }

        public DataTable TreeReestr()
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
                _str_command = "SELECT DISTINCT t_doc_element FROM t_DocReestr";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("v_reestr");
                _data_adapter.Fill(_table);
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
            return _table;
        }

        public DataTable TreeDocItems(string elements)
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
                _str_command = "SELECT * FROM t_DocReestr WHERE t_doc_element = '" + elements + "'";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("v_DocItems");
                _data_adapter.Fill(_table);
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
            return _table;
        }

        public void ImportRetReason(OleDbConnection conn, string orr_name, string orr_1C)
        {
            if (orr_1C == "")
                return;

            string _str_command;
            OleDbCommand _command = null;
            int _orr_id;
            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT count(*) FROM t_OrderRetReason WHERE orr_1C='" + orr_1C + "'";
                _command.CommandText = _str_command;
                if (Convert.ToInt32(_command.ExecuteScalar()) == 0)
                {
                    // новая запись
                    _str_command = "INSERT INTO t_OrderRetReason (orr_1C, orr_name) VALUES(@orr_1C, @orr_name)";
                    _command.Parameters.AddWithValue("@orr_1C", orr_1C);
                    _command.Parameters.AddWithValue("@orr_name", orr_name);
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();

                    // получить orr_id
                    _str_command = "SELECT @@IDENTITY";
                    _command.CommandText = _str_command;
                    _orr_id = Convert.ToInt32(_command.ExecuteScalar());
                }
                else
                {
                    // обновление 
                    _str_command = "UPDATE t_OrderRetReason SET orr_name = @orr_name WHERE orr_1C= @orr_1C";
                    _command.Parameters.AddWithValue("@orr_name", orr_name);
                    _command.Parameters.AddWithValue("@orr_1C", orr_1C);
                }
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }



        public void ImportNome(OleDbConnection conn, string nome_1C, string nome_name, string nome_be, string nome_k, 
            string nome_ue, string nome_mn, string nome_mx, string nome_ke, string nome_ze, string nome_bl, int nome_ty, int nome_type_id, int teremok_id, int ban,int group=0)
        {
            if (nome_1C == "")
                return;
            //            
            string _str_command;

            OleDbCommand _command = null;

            int _nome_id;

            try
            {
                // проверка, есть такая номенклатура в базе 
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT count(*) FROM t_nomenclature WHERE nome_1C='" + nome_1C + "'";
                _command.CommandText = _str_command;
                if (Convert.ToInt32(_command.ExecuteScalar()) == 0)
                {
                    // новая запись
                    _str_command = "INSERT INTO t_nomenclature (nome_1C, nome_name, nome_ed, nome_bed, nome_K) VALUES(@nome_1C, @nome_name, @nome_ed, @nome_bed, @nome_K)";
                    _command.Parameters.AddWithValue("@nome_1C", nome_1C);
                    _command.Parameters.AddWithValue("@nome_name", nome_name);
                    _command.Parameters.AddWithValue("@nome_ed", nome_ue);
                    _command.Parameters.AddWithValue("@nome_bed", nome_be);
                    _command.Parameters.AddWithValue("@nome_K", CUtilHelper.ParceAmount(nome_k, 1));

                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                    // получить nome_id
                    _str_command = "SELECT @@IDENTITY";
                    _command.CommandText = _str_command;
                    _nome_id = Convert.ToInt32(_command.ExecuteScalar());
                }
                else
                {
                    // обновление              
                    _str_command = "UPDATE t_nomenclature SET nome_name = @nome_name, nome_ed = @nome_ed, nome_bed = @nome_bed, nome_K = @nome_K WHERE nome_1C='" + nome_1C + "'";
                    _command.Parameters.Clear();
                    _command.Parameters.AddWithValue("@nome_name", nome_name);
                    _command.Parameters.AddWithValue("@nome_ed", nome_ue);
                    _command.Parameters.AddWithValue("@nome_bed", nome_be);
                    _command.Parameters.AddWithValue("@nome_K", CUtilHelper.ParceAmount(nome_k, 1));
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();

                    _str_command = "SELECT top 1 nome_id FROM t_nomenclature WHERE nome_1C='" + nome_1C + "'";
                    _command.CommandText = _str_command;
                    _nome_id = Convert.ToInt32(_command.ExecuteScalar());
                }
                // вставляем в базу
                _str_command = "INSERT INTO t_Nome2Teremok (n2t_teremok_id, n2t_nome_id, n2t_nt_id, n2t_ed, n2t_bed, n2t_K, n2t_quota, n2t_maxquota, n2t_ke, n2t_ze, n2t_bold, n2t_ban) VALUES (@n2t_teremok_id, @n2t_nome_id, @n2t_nt_id, @n2t_ed, @n2t_bed, @n2t_K, @n2t_quota, @n2t_maxquota, @n2t_ke, @n2t_ze, @n2t_bold, @n2t_ban)";
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@n2t_teremok_id", teremok_id);
                _command.Parameters.AddWithValue("@n2t_nome_id", _nome_id);
                if (nome_ty == 1)
                {
                    _command.Parameters.AddWithValue("@n2t_nt_id", nome_type_id + 1);
                }
                if (nome_ty == 2)
                {
                    _command.Parameters.AddWithValue("@n2t_nt_id", nome_type_id + 3);
                }
                if (nome_ty == 3)
                {
                    _command.Parameters.AddWithValue("@n2t_nt_id", nome_type_id);
                }
                if (nome_ty != 1 && nome_ty != 2 && nome_ty != 3)
                {
                    _command.Parameters.AddWithValue("@n2t_nt_id", nome_type_id);
                }
                _command.Parameters.AddWithValue("@n2t_ed", nome_ue);
                _command.Parameters.AddWithValue("@n2t_bed", nome_be);
                _command.Parameters.AddWithValue("@n2t_K", CUtilHelper.ParceAmount(nome_k, 1));
                _command.Parameters.AddWithValue("@n2t_quota", CUtilHelper.ParceAmount(nome_mn, 1));
                _command.Parameters.AddWithValue("@n2t_maxquota", CUtilHelper.ParceAmount(nome_mx, 1));
                _command.Parameters.AddWithValue("@n2t_ke", CUtilHelper.ParceAmount(nome_ke, 1));
                _command.Parameters.AddWithValue("@n2t_ze", nome_ze);
                _command.Parameters.AddWithValue("@n2t_bold", nome_bl);
                _command.Parameters.AddWithValue("@n2t_ban", ban);
                if (group != 0)
                {
                    _str_command = "INSERT INTO t_Nome2Teremok (n2t_teremok_id, n2t_nome_id, n2t_nt_id, n2t_ed, n2t_bed, n2t_K, n2t_quota, n2t_maxquota, n2t_ke, n2t_ze, n2t_bold, n2t_ban,group_id) " +
                        "VALUES (@n2t_teremok_id, @n2t_nome_id, @n2t_nt_id, @n2t_ed, @n2t_bed, @n2t_K, @n2t_quota, @n2t_maxquota, @n2t_ke, @n2t_ze, @n2t_bold, @n2t_ban,@group_id)";
                    _command.Parameters.AddWithValue("@group_id", group);
                }

                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();

            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public void NomeClearTemplate(OleDbConnection conn, int nome_type_id, int teremok_id)
        {
            //            
            string _str_command;
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "DELETE FROM t_Nome2Teremok WHERE n2t_teremok_id=" + teremok_id.ToString() + " AND n2t_nt_id=" + nome_type_id.ToString();
                _command.CommandText = _str_command;
                _command.ExecuteScalar();

                if (nome_type_id == 23)
                {
                    _command = new OleDbCommand();
                    _command.Connection = conn;
                    _str_command = "DELETE FROM t_Nome2Teremok WHERE n2t_teremok_id=" + teremok_id.ToString() + " AND n2t_nt_id=24";
                    _command.CommandText = _str_command;
                    _command.ExecuteScalar();
                }
                if (nome_type_id == 23)
                {
                    _command = new OleDbCommand();
                    _command.Connection = conn;
                    _str_command = "DELETE FROM t_Nome2Teremok WHERE n2t_teremok_id=" + teremok_id.ToString() + " AND n2t_nt_id=26";
                    _command.CommandText = _str_command;
                    _command.ExecuteScalar();
                }
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public void NomeClearCash()
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "DELETE * FROM t_Standart";
                _command.CommandText = _str_command;
                _command.ExecuteScalar();
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

        public void NomeClearVideo()
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "DELETE * FROM t_DocReestr";
                _command.CommandText = _str_command;
                _command.ExecuteScalar();
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

        // проверяем, есть ли в базе заказа, есть есть, отправляем ID, если нет, то 0
        public int OrderCheck(int teremok_id, int type_order)
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
                _command = new OleDbCommand();
                _command.Connection = _conn;
                // проверим, есть ли документ на сегодня
                _str_command = "SELECT doc_id FROM t_Doc WHERE t_Doc.doc_type_id=" + type_order.ToString() + " AND " + "doc_teremok_id = " + teremok_id.ToString() + " AND t_Doc.doc_datetime>=#" + DateTime.Today.Month + "/" + DateTime.Today.Day + "/" + DateTime.Today.Year + " 00:00:00# AND t_Doc.doc_datetime<=#" + DateTime.Today.Month + "/" + DateTime.Today.Day + "/" + DateTime.Today.Year + " 23:59:59#;";
                _command.CommandText = _str_command;
                int _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    return 0;
                }
                else
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

        public void AddTaskExchange(string doc_id)
        {
            //
            CBData _data = new CBData();
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            int _type_doc = _data.GetTypeDoc(Convert.ToInt32(doc_id));

            try
            {
                // поставить задачу
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "INSERT INTO t_TaskExchange (task_doc_id, task_datetime, task_state_id) VALUES(@task_doc_id, Now(), 0)";
                _command.Parameters.AddWithValue("@task_doc_id", doc_id);
                _command.CommandText = _str_command;

                _command.ExecuteNonQuery();

                // сменить статус
                _str_command = "UPDATE t_Doc SET doc_status_id=@doc_status_id WHERE doc_id=@doc_id";
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@doc_status_id", _data.GetStatusID(null, _type_doc, 2));
                _command.Parameters.AddWithValue("@task_doc_id", doc_id);
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

        public void NoneAddDoc(OleDbConnection conn, int type_nome_id, int teremok_id)
        {
            CBData _data = new CBData();
            string _str_command;
            OleDbCommand _command = null;
            string _desc = null;

            try
            {
                if (CParam.AppCity == 1)
                {
                    switch (type_nome_id)
                    {
                        case 1:
                            _desc = "новый шаблон производства";
                            break;
                        case 29:
                            _desc = "новый шаблон заказа-кондитерки";
                            break;
                        case 2:
                            _desc = "новый шаблон склад";
                            break;
                        case 3:
                            _desc = "новый шаблон остатков";
                            break;
                        case 6:
                            _desc = "новый шаблон списания";
                            break;
                        case 9:
                            _desc = "новый шаблон списания ГП";
                            break;
                        case 10:
                            _desc = "новый шаблон 10ных остатков";
                            break;
                        case 11:
                            _desc = "новый шаблон обедов";
                            break;
                        case 13:
                            _desc = "новый шаблон списания СП";
                            break;
                        case 14:
                            _desc = "новый шаблон остатков инвентаря";
                            break;
                        case 16:
                            _desc = "новый шаблон накладной";
                            break;
                        case 17:
                            _desc = "новый шаблон заказа ХС недельный";
                            break;
                        case 18:
                            _desc = "новый шаблон заказа ХС месячный";
                            break;
                        case 19:
                            _desc = "новый шаблон инкассции";
                            break;
                        case 20:
                            _desc = "новый шаблон категорий возврата";
                            break;
                        case 21:
                            _desc = "новый шаблон перемещения";
                            break;
                        case 23:
                            _desc = "новый шаблон счетчика воды";
                            break;
                        case 24:
                            _desc = "новый шаблон счетчика эл.";
                            break;
                        case 30:
                            _desc = "новая рекалама на видео";
                            break;
                        case 35:
                            _desc = "новые рекаламные изображения";
                            break;
                        case 36:
                            _desc = "новое обучающее видео";
                            break;
                    }
                }
                else
                {
                    switch (type_nome_id)
                    {
                        case 1:
                            _desc = "новый шаблон заказа";
                            break;
                        case 29:
                            _desc = "новый шаблон заказа-кондитерки";
                            break;
                        case 2:
                            _desc = "новый шаблон заказа сан.хоз";
                            break;
                        case 3:
                            _desc = "новый шаблон остатков";
                            break;
                        case 6:
                            _desc = "новый шаблон списания";
                            break;
                        case 9:
                            _desc = "новый шаблон списания ГП";
                            break;
                        case 10:
                            _desc = "новый шаблон контрольных остатков";
                            break;
                        case 11:
                            _desc = "новый шаблон обедов";
                            break;
                        case 13:
                            _desc = "новый шаблон списания СП";
                            break;
                        case 14:
                            _desc = "новый шаблон остатков инвентаря";
                            break;
                        case 16:
                            _desc = "новый шаблон накладной";
                            break;
                        case 17:
                            _desc = "новый шаблон заказа ХС недельный";
                            break;
                        case 18:
                            _desc = "новый шаблон заказа ХС месячный";
                            break;
                        case 19:
                            _desc = "новый шаблон инкассации";
                            break;
                        case 20:
                            _desc = "новый шаблон категорий возврата";
                            break;
                        case 21:
                            _desc = "новый шаблон перемещения";
                            break;
                        case 23:
                            _desc = "новый шаблон счетчика воды";
                            break;
                        case 24:
                            _desc = "новый шаблон счетчика эл.";
                            break;
                        case 30:
                            _desc = "новая рекалама на видео";
                            break;
                        case 35:
                            _desc = "новые рекаламные изображения";
                            break;
                        case 36:
                            _desc = "новое обучающее видео";
                            break;
                    }
                }


                _command = new OleDbCommand();
                _command.Connection = conn;
                if (type_nome_id == 30)
                {
                    _str_command = "INSERT INTO t_Doc (doc_type_id, doc_datetime, doc_status_id, doc_desc, doc_teremok_id) VALUES(34, Now(), 25, @doc_desc, @doc_teremok_id)";
                }
                else if (type_nome_id == 35 || type_nome_id == 36)
                {
                    _str_command = "INSERT INTO t_Doc (doc_type_id, doc_datetime, doc_status_id, doc_desc, doc_teremok_id) VALUES(" + type_nome_id + ", Now(), 25, @doc_desc, @doc_teremok_id)";
                }
                else
                {
                    _str_command = "INSERT INTO t_Doc (doc_type_id, doc_datetime, doc_status_id, doc_desc, doc_teremok_id) VALUES(8, Now(), 25, @doc_desc, @doc_teremok_id)";
                }

                _command.Parameters.AddWithValue("@doc_desc", _desc);
                _command.Parameters.AddWithValue("@doc_teremok_id", teremok_id);
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public DataTable GetExchangeHistory()
        {
            //
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
                _str_command = "SELECT TOP 200 doctype_name, task_doc_id, task_datetime, task_datetime_completed FROM t_DocTypeRef INNER JOIN (t_Doc INNER JOIN t_TaskExchange ON t_Doc.doc_id = t_TaskExchange.task_doc_id) ON t_DocTypeRef.doctype_id = t_Doc.doc_type_id ORDER BY task_datetime DESC";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("v_TaskExchange");
                _data_adapter.Fill(_table);
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
            return _table;
        }

        public int ShowUnsendedDoc()
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
                _str_command = "SELECT count(*) FROM t_DocStatusRef INNER JOIN t_Doc ON t_DocStatusRef.docstatusref_id = t_Doc.doc_status_id WHERE (((t_DocStatusRef.statustype_id)=1)) OR (((t_DocStatusRef.statustype_id)=2))";
                _command.CommandText = _str_command;
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

        /// <summary>
        /// Получить статус документа по ID документа, типу документа и коду статуса
        /// </summary>
        /// <param name="doc_id">код документа, если 0 - то новый</param>
        /// <param name="type_doc_id">тип документа, для которого нужен статус</param>
        /// <returns>код статуса</returns>
        public int GetStatusID(OleDbConnection conn, int type_doc_id, int status_type_id)
        {
            OleDbCommand _command = null;
            OleDbConnection _conn = null;

            try
            {
                if (conn == null)
                {
                    _conn = new OleDbConnection(CParam.ConnString);
                    _conn.Open();
                }
                else
                    _conn = conn;
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = "SELECT docstatusref_id FROM t_DocStatusRef WHERE"+
               " doctype_id = @doctype_id AND statustype_id = @statustype_id";
                _command.Parameters.AddWithValue("@doctype_id", type_doc_id);
                _command.Parameters.AddWithValue("@statustype_id", status_type_id);
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

        public string GetStatusColor(OleDbConnection conn, string status_doc_id)
        {
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                _command.CommandText = "SELECT t_StatusType.status_type_color FROM t_StatusType INNER JOIN t_DocStatusRef ON t_StatusType.status_type_id = t_DocStatusRef.statustype_id WHERE (((t_DocStatusRef.docstatusref_id)=@docstatusref_id))";
                _command.Parameters.AddWithValue("@docstatusref_id", status_doc_id);
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        // добавить новую строчку в заказ
        public bool OrderAddNewItem(int doc_id, int nome_id)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            DataTable _table = null;

            string opd_nome_ed = "";
            string opd_nome_bed = "";
            string opd_nome_K = "";
            int _check_count = 0;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;


                // проверка на наличие этой позиции
                _str_command = "SELECT COUNT(*) FROM t_Order2ProdDetails WHERE opd_doc_id=@opd_doc_id AND opd_nome_id=@opd_nome_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@opd_doc_id", doc_id);
                _command.Parameters.AddWithValue("@opd_nome_id", nome_id);
                _check_count = Convert.ToInt32(_command.ExecuteScalar());
                if (_check_count > 0)
                    return false;

                // получить список номенлатуры
                _str_command = "SELECT TOP 1 n2t_ed, n2t_bed, n2t_K FROM t_Nome2Teremok WHERE n2t_teremok_id="
                    + GetTeremokIDByDocID(doc_id).ToString() + " AND n2t_nome_id = " + nome_id.ToString();
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("t_Nomenclature");
                _data_adapter.Fill(_table);

                foreach (DataRow _row in _table.Rows)
                {
                    opd_nome_ed = _row[0].ToString();
                    opd_nome_bed = _row[1].ToString();
                    opd_nome_K = _row[2].ToString();
                }

                _str_command = "INSERT INTO t_Order2ProdDetails(opd_doc_id, opd_nome_id, opd_ed, opd_bed, opd_k, opd_quota, opd_maxquota) VALUES (@opd_doc_id, @opd_nome_id, @opd_nome_ed, @opd_nome_bed, @opd_nome_k, @opd_quota, @opd_maxquota)";
                _command.CommandText = _str_command;
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@opd_doc_id", doc_id);
                _command.Parameters.AddWithValue("@opd_nome_id", nome_id);
                _command.Parameters.AddWithValue("@opd_nome_ed", opd_nome_ed);
                _command.Parameters.AddWithValue("@opd_nome_bed", opd_nome_bed);
                _command.Parameters.AddWithValue("@opd_nome_k", CUtilHelper.ParceAmount(opd_nome_K, 1));
                _command.Parameters.AddWithValue("@opd_quota", 0);
                _command.Parameters.AddWithValue("@opd_maxquota", 0);
                _command.ExecuteNonQuery();

                return false;
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
        /// Получить тип статуса из ID статуса
        /// </summary>
        /// <param name="status_id"></param>
        /// <returns></returns>
        public int GetStatusTypeID(int status_id)
        {
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = "SELECT statustype_id FROM t_DocStatusRef WHERE docstatusref_id = @docstatusref_id";
                _command.Parameters.AddWithValue("@docstatusref_id", status_id);
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

        public int HelpDoc(string teremok_id, DateTime help_date)
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
                _str_command = "SELECT top 1 doc_id FROM t_Doc WHERE doc_teremok_id = @doc_teremok_id AND doc_datetime= @doc_datetime AND doc_type_id=22";
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_teremok_id", teremok_id);
                _command.Parameters.AddWithValue("@doc_datetime", help_date);

                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    // вставить новую строчку
                    _str_command = "INSERT INTO t_Doc (doc_type_id, doc_status_id, doc_desc, doc_teremok_id, doc_datetime) VALUES(22, 65, 'получено обновление инструкции', @doc_teremok_id, @doc_datetime) ";
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
                    _str_command = "UPDATE t_Doc SET doc_status_id = 65, doc_desc='получено обновление инструкции ' WHERE doc_id=" + _doc_id.ToString();
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

        public string GetDep_kkm(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = "SELECT doc_menu_dep FROM t_Doc WHERE doc_id = " + doc_id;
                _command.Parameters.AddWithValue("@doc_id", doc_id);
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

        public DataTable SalesReport(DateTime date, bool allKKM)
        {
            DataTable _table;
            string _str_connect = CParam.ConnString;
            using (OleDbConnection _conn = new OleDbConnection(_str_connect))
            {
                string _str_command;
                {
                    // прошедшие периоды
                    if (!allKKM)

                        _str_command = "SELECT t_ZReport.z_kkm, t_Check.check_datetime, t_Menu.menu_nome, Sum(t_CheckItem.ch_count) AS [Sum-ch_count], Sum(t_CheckItem.ch_amount1) AS [Sum-ch_amount1], Sum(t_CheckItem.ch_amount1) / Sum(t_CheckItem.ch_count) AS [Min-ch_amount1] " +
                                                "FROM t_ZReport INNER JOIN (t_Menu INNER JOIN (t_Check INNER JOIN t_CheckItem ON t_Check.check_id = t_CheckItem.ch_check_id) ON t_Menu.menu_id = t_CheckItem.ch_menu_id) ON t_ZReport.z_id = t_Check.check_z_id " +
                                                "WHERE t_Check.check_datetime = @from " +
                                                "GROUP BY t_ZReport.z_kkm, t_Menu.menu_nome, t_Check.check_datetime " +
                                                "ORDER BY t_ZReport.z_kkm, t_Menu.menu_nome, t_Check.check_datetime ";
                    else
                        _str_command = "SELECT '-' as z_kkm, t_Check.check_datetime, t_Menu.menu_nome, Sum(t_CheckItem.ch_count) AS [Sum-ch_count], Sum(t_CheckItem.ch_amount1) AS [Sum-ch_amount1], Sum(t_CheckItem.ch_amount1) / Sum(t_CheckItem.ch_count) AS [Min-ch_amount1] " +
                                            "FROM t_ZReport INNER JOIN (t_Menu INNER JOIN (t_Check INNER JOIN t_CheckItem ON t_Check.check_id = t_CheckItem.ch_check_id) ON t_Menu.menu_id = t_CheckItem.ch_menu_id) ON t_ZReport.z_id = t_Check.check_z_id " +
                                            "WHERE t_Check.check_datetime = @from " +
                                            "GROUP BY t_Menu.menu_nome, t_Check.check_datetime " +
                                            "ORDER BY t_Menu.menu_nome, t_Check.check_datetime ";
                }

                try
                {
                    using (OleDbCommand _command = new OleDbCommand())
                    {
                        _command.Connection = _conn;
                        _command.CommandText = _str_command;
                        _command.Parameters.AddWithValue("@from", date.ToShortDateString());

                        using (OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_command))
                        {
                            _table = new DataTable("RBADataSet_SalesReport2_v_SalesReport2");
                            _data_adapter.Fill(_table);
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

            return _table;
        }

        public bool FileSearch(string _name)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            int _file_count = 0;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;

                _str_command = "SELECT COUNT(*) FROM t_Standart WHERE t_name='" + _name + "'";
                _command.CommandText = _str_command;
                _file_count = Convert.ToInt32(_command.ExecuteScalar());
                if (_file_count != 0)
                    return true;
                else
                    return false;
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

        public string СompareStandart(string _name)
        {
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
                _str_command = "SELECT t_hash_code FROM t_Standart WHERE t_name= '" + _name.ToString() + "'";
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

        public DataTable QueryTable()
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

                _str_command = "SELECT t_path_name AS a FROM t_Standart";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("test");
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

        public bool returnStatusColumnID(int column_id, int column_doc_id)
        {
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
                _str_command = "SELECT column_status FROM t_SettingsDoc WHERE column_doc_id= " + column_doc_id + " AND column_id = " + column_id;
                _command.CommandText = _str_command;
                if (Convert.ToBoolean(_command.ExecuteScalar()) == true)
                {
                    return true;
                }
                else
                {
                    return false;
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

        public DataTable returnStatusColumn(int column_doc_id)
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

                _str_command = "SELECT * FROM t_SettingsDoc WHERE column_doc_id = " + column_doc_id;
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("t_SettingsDoc");
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

        public void updateStatusDoc(string column_name, int column_doc_id, bool status)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_SettingsDoc SET column_status = @column_status WHERE column_name = @column_name AND column_doc_id = @column_doc_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@column_status", status);
                _command.Parameters.AddWithValue("@column_name", column_name);
                _command.Parameters.AddWithValue("@column_doc_id", column_doc_id);
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

        #region old mark doc find


        public string MarkCheck(int teremok_id, int type_order)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            int _doc_id = 0;
            string guid;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                // проверим, есть ли документ на сегодня
                _str_command = "SELECT doc_id FROM t_Doc WHERE t_Doc.doc_type_id=" + type_order.ToString() + " AND doc_teremok_id = " + teremok_id.ToString() + " AND t_Doc.doc_datetime>=#" + DateTime.Today.Month + "/01/" + DateTime.Today.Year + " 00:00:00# AND t_Doc.doc_datetime<=#" + DateTime.Today.Month + "/" + DateTime.DaysInMonth(Convert.ToInt32(DateTime.Today.Year), Convert.ToInt32(DateTime.Today.Month)) + "/" + DateTime.Today.Year + " 23:59:59#;";
                _command.CommandText = _str_command;
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    return "";
                }
                else
                {
                    _str_command = "SELECT doc_guid FROM t_Doc WHERE doc_id = " + _doc_id;
                    _command.CommandText = _str_command;
                    guid = _command.ExecuteScalar().ToString();
                    return guid;
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

        public int MarkDocID(int teremok_id, int type_order)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            int _doc_id = 0;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                // проверим, есть ли документ на сегодня
                _str_command = "SELECT doc_id FROM t_Doc WHERE t_Doc.doc_type_id=" + type_order.ToString() + " AND doc_teremok_id = " + teremok_id.ToString() + " AND t_Doc.doc_datetime>=#" + DateTime.Today.Month + "/01/" + DateTime.Today.Year + " 00:00:00# AND t_Doc.doc_datetime<=#" + DateTime.Today.Month + "/" + DateTime.DaysInMonth(Convert.ToInt32(DateTime.Today.Year), Convert.ToInt32(DateTime.Today.Month)) + "/" + DateTime.Today.Year + " 23:59:59#;";
                _command.CommandText = _str_command;
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    return 0;
                }
                else
                {
                    return _doc_id;
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

        #endregion


        #region new mark doc find

        public string MarkCheck(int teremok_id, int type_order, DateTime doc_date)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            int _doc_id = 0;
            string guid;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                // проверим, есть ли документ на сегодня
                _str_command = "SELECT doc_id FROM t_Doc WHERE t_Doc.doc_type_id=" + type_order.ToString() + " AND doc_teremok_id = " + teremok_id.ToString() + " AND t_Doc.doc_datetime>=#" + doc_date.Month + "/01/" +
                    doc_date.Year + " 00:00:00# AND t_Doc.doc_datetime<=#" + doc_date.Month + "/" + DateTime.DaysInMonth(Convert.ToInt32(doc_date.Year), Convert.ToInt32(doc_date.Month)) + "/" + doc_date.Year + " 23:59:59#;";
                _command.CommandText = _str_command;
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    return "";
                }
                else
                {
                    _str_command = "SELECT doc_guid FROM t_Doc WHERE doc_id = " + _doc_id;
                    _command.CommandText = _str_command;
                    guid = _command.ExecuteScalar().ToString();
                    return guid;
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

        public int MarkDocID(int teremok_id, int type_order, DateTime doc_date)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            int _doc_id = 0;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                // проверим, есть ли документ на сегодня

                _str_command = "SELECT doc_id FROM t_Doc WHERE t_Doc.doc_type_id=" + type_order.ToString() + " AND doc_teremok_id = " + teremok_id.ToString() + " AND t_Doc.doc_datetime>=#" +
                    doc_date.Month + "/01/" + doc_date.Year + " 00:00:00# AND t_Doc.doc_datetime<=#" + doc_date.Month + "/" + DateTime.DaysInMonth(Convert.ToInt32(doc_date.Year), Convert.ToInt32(doc_date.Month)) +
                    "/" + doc_date.Year + " 23:59:59#;";

                _command.CommandText = _str_command;
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    return 0;
                }
                else
                {
                    return _doc_id;
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

        #endregion


        public int MarkDocIDGuid(int teremok_id, string guid)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            int _doc_id = 0;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                // проверим, есть ли документ на сегодня
                _str_command = "SELECT doc_id FROM t_Doc WHERE doc_guid='" + guid.ToString() + "' AND doc_teremok_id = " + teremok_id.ToString();
                _command.CommandText = _str_command;
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    return 0;
                }
                else
                {
                    return _doc_id;
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


        #region old mark doc creation
        public int OrderAddMark(int doc_type_id, int teremok_id, string guid)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            int _doc_id;
            CBData _data = new CBData();

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;

                // создать новый документ
                _str_command = "INSERT INTO t_Doc(doc_type_id, doc_teremok_id, doc_status_id, doc_guid) VALUES (@doc_type_id, @doc_teremok_id, @doc_status_id, @doc_guid)";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_type_id", doc_type_id);
                _command.Parameters.AddWithValue("@doc_teremok_id", teremok_id);
                _command.Parameters.AddWithValue("@doc_status_id", _data.GetStatusID(null, doc_type_id, 1));
                _command.Parameters.AddWithValue("@doc_guid", guid);
                _command.ExecuteNonQuery();

                // id документа
                _str_command = "SELECT @@IDENTITY";
                _command.CommandText = _str_command;
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
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
            return _doc_id;
        }
        #endregion

        #region new mark doc creation


        /// <summary>
        /// Получить хэш табеля
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public string _GetTabelHash(PlanDocument pd)
        {
            try
            {
                string hash = "";
                hash += pd.ID;
                if (pd.ArrayWorkEmployee.NotNullOrEmpty())
                {
                    foreach (WorkEmployee wrk in pd.ArrayWorkEmployee)
                    {
                        hash += wrk.Name.ReturnOrEmpty() + wrk.Responsibility.ReturnOrEmpty();
                        int days_count = DateTime.DaysInMonth(Convert.ToInt32(pd.Date.Year), Convert.ToInt32(pd.Date.Month));

                        //foreach (DayWork dw in wrk.ArrayDayWork)
                        for (int i = 0; i < days_count; i++)
                        {
                            DayWork dw = wrk.ArrayDayWork[i];
                            hash += dw.Number.ToString();
                            hash += dw.SmenaType.ReturnOrEmpty();
                            hash += dw.Value.ReturnOrEmpty();
                        }
                    }
                }
                return Hashing.GetMd5Hash(hash);
            }
            catch (Exception ex)
            {
                MDIParentMain.Log("_GetTabelHash error " + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Получить флаг последнего обмена
        /// </summary>
        /// <param name="func_name">имя функции</param>
        /// <param name="teremok_id">ид теремка</param>
        /// <returns></returns>
        public t_PropValue _GetWebServiceLastExchangeFlag(string func_name, int teremok_id)
        {
            t_PropValue updationFlag = new t_PropValue().
               SelectFirst<t_PropValue>("Prop_name='" + func_name + "' AND Prop_type='"
               + CParam.TeremokId + "'");

            if (updationFlag == null)
            {
                updationFlag = new t_PropValue() { prop_name = "GetDocument", prop_value = "0", prop_type = CParam.TeremokId };
                updationFlag.Create();
            }

            return updationFlag;
        }

        /// <summary>
        /// Создать t_Doc документ
        /// </summary>
        /// <param name="doc_type_id"></param>
        /// <param name="teremok_id"></param>
        /// <param name="guid"></param>
        /// <param name="doc_date"></param>
        /// <param name="doc_stat_type"></param>
        /// <returns></returns>
        public t_Doc _CreateTDoc(int doc_type_id, int teremok_id, string guid, DateTime doc_date, DocStatusType doc_stat_type)
        {
            t_Doc doc = new t_Doc();
            doc.doc_type_id = doc_type_id;
            doc.doc_teremok_id = teremok_id;
            if (guid != "") doc.doc_guid = guid;
            doc.doc_datetime = doc_date;
            doc.doc_status_id = GetStatusID(doc_type_id, doc_stat_type);
            doc.Create();
            return doc;
        }


        public int OrderAddMark(int doc_type_id, int teremok_id, string guid, DateTime doc_date)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;
            int _doc_id;
            CBData _data = new CBData();

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;

                // создать новый документ
                _str_command = "INSERT INTO t_Doc(doc_type_id, doc_datetime, doc_teremok_id, doc_status_id, doc_guid) VALUES (@doc_type_id, @doc_datetime, @doc_teremok_id,@doc_status_id, @doc_guid)";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@doc_type_id", doc_type_id);

                OleDbParameter parm = new OleDbParameter("@doc_datetime", OleDbType.Date);
                parm.Value = doc_date;
                _command.Parameters.Add(parm);

                _command.Parameters.AddWithValue("@doc_teremok_id", teremok_id);

                int stat_id = _data.GetStatusID(null, doc_type_id, 1);

                _command.Parameters.AddWithValue("@doc_status_id", stat_id);
                _command.Parameters.AddWithValue("@doc_guid", guid);
                _command.ExecuteNonQuery();

                // id документа
                _str_command = "SELECT @@IDENTITY";
                _command.CommandText = _str_command;
                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
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
            return _doc_id;
        }
        #endregion

        public string returnJobTeremokStart(int numberDay)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;
            int day = numberDay - 3;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT teremok_firstTime FROM t_WorkTeremok WHERE teremok_day ='" + day + "'";
                _command.CommandText = _str_command;
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception _exp)
            {
                return "";
                //throw _exp;
                //null;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public string returnJobTeremokEnd(int numberDay)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;
            int day = numberDay - 3;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT teremok_lastTime FROM t_WorkTeremok WHERE teremok_day ='" + day + "'";
                _command.CommandText = _str_command;
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable ButtonFullList()
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            //string _str_command = "SELECT * FROM t_ButtonTemplate ORDER BY btn_Order ASC";
            string _str_command = "SELECT * " +
                "FROM t_ButtonTemplate " +
                "WHERE (((t_ButtonTemplate.btn_IsUsed)=True) AND ((t_ButtonTemplate.Deleted)=False)) " +
                "ORDER BY t_ButtonTemplate.btn_Order;";
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("Template");
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

        public string returnDocGuid(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT doc_guid FROM t_Doc WHERE doc_id = " + doc_id;
                _command.CommandText = _str_command;
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable OrderMark(int doc_id, int type_doc)
        {
            DataTable _table;
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            _str_command = "SELECT t_MarkItems.*, t_Employee.employee_name, t_Employee.employee_FunctionName " +
                           "FROM t_MarkItems INNER JOIN t_Employee ON t_MarkItems.mark_name = t_Employee.employee_1C " +
                           "WHERE mark_doc_id =" + doc_id + " ORDER BY mark_row_id ASC";
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("t_MarkItems");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable Responsibility()
        {

            DataTable _table;
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            //_str_command = "SELECT * FROM t_Responsibility";
            _str_command = "SELECT * FROM t_Responsibility where Deleted=False;";
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("t_Responsibility");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public string returnColor(string _guid)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT btn_color FROM t_ButtonTemplate WHERE btn_guid = '" + _guid + "'";
                _command.CommandText = _str_command;
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void OrderMarkUpdate(string mark, string bntvalue, int num, int row, DateTime StartJob, int time, string guidSmena)
        {
            string _str_command;
            string _str_command1;
            string _str_command2;
            string _str_command3;
            string _str_command4;
            string _str_command5;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            OleDbCommand _command1 = null;
            OleDbCommand _command2 = null;
            OleDbCommand _command3 = null;
            OleDbCommand _command4 = null;
            OleDbCommand _command5 = null;
            int nu = num - 3;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_MarkItems SET mark_" + nu + " = @mark_" + nu + " WHERE mark_id =" + row;
                _command.CommandText = _str_command;

                if (bntvalue != "")
                {
                    _command.Parameters.AddWithValue("@mark_" + nu, bntvalue);
                }
                else
                {
                    _command.Parameters.AddWithValue("@mark_" + nu, DBNull.Value);
                }
                _command.ExecuteNonQuery();

                _command1 = new OleDbCommand();
                _command1.Connection = _conn;
                _str_command1 = "UPDATE t_MarkItems SET mark_guid_" + nu + " = @mark_guid_" + nu + " WHERE mark_id =" + row;
                _command1.CommandText = _str_command1;
                _command1.Parameters.AddWithValue("@mark_guid_" + nu, mark);
                _command1.ExecuteNonQuery();

                _command2 = new OleDbCommand();
                _command2.Connection = _conn;
                _str_command2 = "UPDATE t_MarkItems SET mark_firstime_" + nu + " = @mark_firstime_" + nu + " WHERE mark_id =" + row;
                _command2.CommandText = _str_command2;
                _command2.Parameters.AddWithValue("@mark_firstime_" + nu, StartJob.ToShortTimeString());
                _command2.ExecuteNonQuery();

                _command3 = new OleDbCommand();
                _command3.Connection = _conn;
                _str_command3 = "UPDATE t_MarkItems SET mark_lasttime_" + nu + " = @mark_lasttime_" + nu + " WHERE mark_id =" + row;
                _command3.CommandText = _str_command3;
                _command3.Parameters.AddWithValue("@mark_lasttime_" + nu, StartJob.AddHours(time).ToShortTimeString());
                _command3.ExecuteNonQuery();

                _command4 = new OleDbCommand();
                _command4.Connection = _conn;
                _str_command4 = "UPDATE t_MarkItems SET mark_work_" + nu + " = @mark_work_" + nu + " WHERE mark_id =" + row;
                _command4.CommandText = _str_command4;
                _command4.Parameters.AddWithValue("@mark_work_" + nu, time);
                _command4.ExecuteNonQuery();

                _command5 = new OleDbCommand();
                _command5.Connection = _conn;
                _str_command5 = "UPDATE t_MarkItems SET mark_guidsmena_" + nu + " = @mark_guidsmena_" + nu + " WHERE mark_id =" + row;
                _command5.CommandText = _str_command5;
                _command5.Parameters.AddWithValue("@mark_guidsmena_" + nu, guidSmena);
                _command5.ExecuteNonQuery();
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

        public void TotalUpdate(string row, string total)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_MarkItems SET mark_total = @mark_total WHERE mark_id =" + row;
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@mark_total", total);
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

        public void OrderMarkUpdateEmp(string res, string resGUID, int row)
        {
            string _str_command;
            string _str_command2;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            OleDbCommand _command2 = null;
            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_MarkItems SET mark_office = @mark_office WHERE mark_id =" + row;
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@mark_office", res);
                _command.ExecuteNonQuery();

                _command2 = new OleDbCommand();
                _command2.Connection = _conn;
                _str_command2 = "UPDATE t_MarkItems SET mark_res = @mark_res WHERE mark_id =" + row;
                _command2.CommandText = _str_command2;
                _command2.Parameters.AddWithValue("@mark_res", resGUID);
                _command2.ExecuteNonQuery();
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

        public int CountMark(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT count(*) FROM t_MarkItems WHERE mark_doc_id = " + doc_id;
                _command.CommandText = _str_command;
                return Convert.ToInt32(_command.ExecuteScalar());
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable allMark(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            _str_command = "SELECT mark_name, mark_res, mark_guid_1, mark_guid_2, mark_guid_3, mark_guid_4, mark_guid_5, mark_guid_6, mark_guid_7, mark_guid_8, mark_guid_9, mark_guid_10, " +
            " mark_guid_11, mark_guid_12, mark_guid_13, mark_guid_14, mark_guid_15, mark_guid_16, mark_guid_17, mark_guid_18, mark_guid_19, mark_guid_20, mark_guid_21, mark_guid_22, " +
            " mark_guid_23, mark_guid_24, mark_guid_25, mark_guid_26, mark_guid_27, mark_guid_28, mark_guid_29, mark_guid_30, mark_guid_31 " +
            " FROM t_MarkItems WHERE mark_doc_id = " + doc_id;

            DataTable _table;
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("allMark");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable marktime(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            _str_command = "SELECT mark_firstime_1, mark_firstime_2, mark_firstime_3, mark_firstime_4, mark_firstime_5, mark_firstime_6, mark_firstime_7, mark_firstime_8, mark_firstime_9, mark_firstime_10, " +
            " mark_firstime_11, mark_firstime_12, mark_firstime_13, mark_firstime_14, mark_firstime_15, mark_firstime_16, mark_firstime_17, mark_firstime_18, mark_firstime_19, mark_firstime_20, mark_firstime_21, mark_firstime_22, " +
            " mark_firstime_23, mark_firstime_24, mark_firstime_25, mark_firstime_26, mark_firstime_27, mark_firstime_28, mark_firstime_29, mark_firstime_30, mark_firstime_31 " +
            " FROM t_MarkItems WHERE mark_doc_id = " + doc_id;

            DataTable _table;
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("allMark");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable guidSmena(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            _str_command = "SELECT mark_guidsmena_1, mark_guidsmena_2, mark_guidsmena_3, mark_guidsmena_4, mark_guidsmena_5, mark_guidsmena_6, mark_guidsmena_7, mark_guidsmena_8, mark_guidsmena_9, mark_guidsmena_10, " +
            " mark_guidsmena_11, mark_guidsmena_12, mark_guidsmena_13, mark_guidsmena_14, mark_guidsmena_15, mark_guidsmena_16, mark_guidsmena_17, mark_guidsmena_18, mark_guidsmena_19, mark_guidsmena_20, mark_guidsmena_21, mark_guidsmena_22, " +
            " mark_guidsmena_23, mark_guidsmena_24, mark_guidsmena_25, mark_guidsmena_26, mark_guidsmena_27, mark_guidsmena_28, mark_guidsmena_29, mark_guidsmena_30, mark_guidsmena_31 " +
            " FROM t_MarkItems WHERE mark_doc_id = " + doc_id;

            DataTable _table;
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("allMark");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable marklasttime(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            _str_command = "SELECT mark_lasttime_1, mark_lasttime_2, mark_lasttime_3, mark_lasttime_4, mark_lasttime_5, mark_lasttime_6, mark_lasttime_7, mark_lasttime_8, mark_lasttime_9, mark_lasttime_10, " +
            " mark_lasttime_11, mark_lasttime_12, mark_lasttime_13, mark_lasttime_14, mark_lasttime_15, mark_lasttime_16, mark_lasttime_17, mark_lasttime_18, mark_lasttime_19, mark_lasttime_20, mark_lasttime_21, mark_lasttime_22, " +
            " mark_lasttime_23, mark_lasttime_24, mark_lasttime_25, mark_lasttime_26, mark_lasttime_27, mark_lasttime_28, mark_lasttime_29, mark_lasttime_30, mark_lasttime_31 " +
            " FROM t_MarkItems WHERE mark_doc_id = " + doc_id;

            DataTable _table;
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("allMark");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable valueTime(int doc_id)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            _str_command = "SELECT mark_work_1, mark_work_2, mark_work_3, mark_work_4, mark_work_5, mark_work_6, mark_work_7, mark_work_8, mark_work_9, mark_work_10, " +
            " mark_work_11, mark_work_12, mark_work_13, mark_work_14, mark_work_15, mark_work_16, mark_work_17, mark_work_18, mark_work_19, mark_work_20, mark_work_21, mark_work_22, " +
            " mark_work_23, mark_work_24, mark_work_25, mark_work_26, mark_work_27, mark_work_28, mark_work_29, mark_work_30, mark_work_31 " +
            " FROM t_MarkItems WHERE mark_doc_id = " + doc_id;

            DataTable _table;
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("allMark");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void MarkDelete(int _mark_id)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // удалить записи
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "DELETE FROM t_MarkItems WHERE mark_id = " + _mark_id.ToString();
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

        public void dUpdate(int mark_doc_id, int mark_row_id, int mark_id)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_MarkItems SET mark_row_id = @mark_row_id WHERE mark_doc_id = @mark_doc_id AND mark_id = @mark_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@mark_row_id", mark_row_id);
                _command.Parameters.AddWithValue("@mark_doc_id", mark_doc_id);
                _command.Parameters.AddWithValue("@mark_id", mark_id);
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

        public DataTable OrderMarkDetails(int doc_id, int column, int row)
        {

            DataTable _table;
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            //_str_command = "SELECT mark_firstime_" + column + ", mark_lasttime_" + column + " FROM t_MarkItems WHERE mark_id = " + row;
            _str_command = "SELECT mark_firstime_" + column + ", mark_lasttime_" + column + ", mark_work_" + column + " FROM t_MarkItems WHERE mark_id = " + row;
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("OrderMarkDetails");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public int timeValue(int doc_id, int column, int row)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT mark_work_" + column + " FROM t_MarkItems WHERE mark_id =" + row;
                _command.CommandText = _str_command;
                return Convert.ToInt32(_command.ExecuteScalar());
            }
            catch (Exception)
            {
                // throw _exp;
                return 0;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable returnColorMarkNew()
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            _str_command = "SELECT * FROM t_ButtonTemplate";
            //_str_command = "SELECT  * FROM t_ButtonTemplate where t_ButtonTemplate.Deleted=False;";

            DataTable _table;
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("GetShift");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable returnRDR()
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            _str_command = "SELECT * FROM t_WorkOther";

            DataTable _table;
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("RDR");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public DataTable returnRowCount()
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection conn = null;
            conn = new OleDbConnection(_str_connect);
            conn.Open();

            _str_command = "SELECT mark_id FROM t_MarkItems";

            DataTable _table;
            try
            {
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, conn);
                _table = new DataTable("RDR");
                _data_adapter.Fill(_table);

                return _table;
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public int GetIdMark(string guid)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT mark_id FROM t_MarkItems WHERE mark_name = '" + guid.ToString() + "'";
                _command.CommandText = _str_command;
                return Convert.ToInt32(_command.ExecuteScalar());
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        //Добавим тип смены, после обращения получаем сокращение и добавляем value после чего обновляем грид.
        public void OrderMarkUpdate2(int row, int column, string sdate, string edate, string value, string shiftType, string shortName)
        {
            string _str_command;
            string _str_command1;
            string _str_command2;
            string _str_command3;
            string _str_command4;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            OleDbCommand _command1 = null;
            OleDbCommand _command2 = null;
            OleDbCommand _command3 = null;
            OleDbCommand _command4 = null;
            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                // обновить запись
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "UPDATE t_MarkItems SET mark_firstime_" + column + " = @mark_firstime_" + column + " WHERE mark_id =" + row;
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@mark_firstime_" + column, sdate);
                _command.ExecuteNonQuery();

                _command1 = new OleDbCommand();
                _command1.Connection = _conn;
                _str_command1 = "UPDATE t_MarkItems SET mark_lasttime_" + column + " = @mark_lasttime_" + column + " WHERE mark_id =" + row;
                _command1.CommandText = _str_command1;
                _command1.Parameters.AddWithValue("@mark_lasttime_" + column, edate);
                _command1.ExecuteNonQuery();

                _command2 = new OleDbCommand();
                _command2.Connection = _conn;
                _str_command2 = "UPDATE t_MarkItems SET mark_work_" + column + " = @mark_work_" + column + " WHERE mark_id =" + row;
                _command2.CommandText = _str_command2;
                _command2.Parameters.AddWithValue("@mark_work_" + column, value);
                _command2.ExecuteNonQuery();

                _command3 = new OleDbCommand();
                _command3.Connection = _conn;
                _str_command3 = "UPDATE t_MarkItems SET mark_guidsmena_" + column + " = @mark_guidsmena_" + column + " WHERE mark_id =" + row;
                _command3.CommandText = _str_command3;
                _command3.Parameters.AddWithValue("@mark_guidsmena_" + column, shiftType);
                _command3.ExecuteNonQuery();

                _command4 = new OleDbCommand();
                _command4.Connection = _conn;
                _str_command4 = "UPDATE t_MarkItems SET mark_" + column + " = @mark_" + column + " WHERE mark_id =" + row;
                _command4.CommandText = _str_command4;
                _command4.Parameters.AddWithValue("@mark_" + column, shortName);
                _command4.ExecuteNonQuery();
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

        public DataTable EmpFullList()
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            //string _str_command = "SELECT * FROM t_Employee WHERE employee_WorkField <> 0 ORDER BY employee_name ASC";
            string _str_command = "SELECT * FROM t_Employee WHERE employee_WorkField <> 0 AND Deleted=False ORDER BY employee_name ASC";
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("Employee");
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

        public DataTable ShiftTypeFullList_Group()
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            string _str_command = "SELECT t_ShiftType.type_guid, t_ShiftType.type_value, t_ShiftType.type_color, t_ShiftType.type_name " +
"FROM t_ShiftType " +
"where Deleted=False " +
"GROUP BY t_ShiftType.type_guid, t_ShiftType.type_value, t_ShiftType.type_color, t_ShiftType.type_name " +
"ORDER BY t_ShiftType.type_name;";
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("ShiftTypeFullList");
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

        public DataTable ShiftTypeFullList()
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            string _str_command = "SELECT * FROM t_ShiftType ORDER BY type_name ASC";
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("ShiftTypeFullList");
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

        public void WorkOther(string guid, string GuidShift, int Day, string Value, DateTime FirstTime, DateTime LastTime)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            int _check_count = 0;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;


                //// проверка на наличие этой позиции
                //_str_command = "SELECT COUNT(*) FROM t_WorkOther WHERE guid_emp = @guid_emp AND first_time = @first_time AND last_time = @last_time";
                //_command.CommandText = _str_command;
                //_command.Parameters.AddWithValue("@guid_emp", guid);
                //_command.Parameters.AddWithValue("@first_time", FirstTime);
                //_command.Parameters.AddWithValue("@last_time", LastTime);
                //_check_count = Convert.ToInt32(_command.ExecuteScalar());
                //if (_check_count > 0)
                //{
                //    //_str_command = "UPDATE t_WorkOther SET column_status = @column_status WHERE column_name = @column_name AND column_doc_id = @column_doc_id";
                //    //_command.CommandText = _str_command;
                //    //_command.Parameters.AddWithValue("@column_status", );
                //    //_command.Parameters.AddWithValue("@column_name", );
                //    //_command.Parameters.AddWithValue("@column_doc_id", );
                //    //_command.ExecuteNonQuery();
                //}

                _str_command = "INSERT INTO t_WorkOther(guid_emp, guid_shift, day_short, value_time, first_time, last_time) VALUES (@guid_emp, @guid_shift, @day_short, @value_time, @first_time, @last_time)";
                _command.CommandText = _str_command;
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@guid_emp", guid);
                _command.Parameters.AddWithValue("@guid_shift", GuidShift);
                _command.Parameters.AddWithValue("@day_short", Day);
                _command.Parameters.AddWithValue("@value_time", Value);
                _command.Parameters.AddWithValue("@first_time", FirstTime);
                _command.Parameters.AddWithValue("@last_time", LastTime);
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

        public bool OrderAddEmp(string guid, int docID, int mark_row_id)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            int _check_count = 0;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;


                // проверка на наличие этой позиции
                _str_command = "SELECT COUNT(*) FROM t_MarkItems WHERE mark_doc_id = @mark_doc_id AND mark_name = @mark_name";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@mark_doc_id", docID);
                _command.Parameters.AddWithValue("@mark_name", guid);
                //_command.Parameters.AddWithValue("@mark_office", functionGuid);
                _check_count = Convert.ToInt32(_command.ExecuteScalar());
                if (_check_count > 0)
                    return false;


                _str_command = "INSERT INTO t_MarkItems(mark_row_id, mark_doc_id, mark_name) VALUES (@mark_row_id, @mark_doc_id, @mark_name)";
                _command.CommandText = _str_command;
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@mark_row_id", mark_row_id);
                _command.Parameters.AddWithValue("@mark_doc_id", docID);
                _command.Parameters.AddWithValue("@mark_name", guid);
                //_command.Parameters.AddWithValue("@mark_office", functionGuid);
                _command.ExecuteNonQuery();
                return false;
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

        public string GeFunction(string Guid)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT employee_FunctionGuid FROM t_Employee WHERE employee_1C = @employee_1C";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@employee_1C", Guid);
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

        public void ImportEmployee(OleDbConnection conn, string Guid, string Name, string functionName, string functionGuid, bool workField)
        {
            string _str_command;
            OleDbCommand _command = null;
            int _doc_id;
            try
            {
                _str_command = "SELECT COUNT(*) FROM t_Employee WHERE employee_1C = @employee_1C";
                _command = new OleDbCommand();
                _command.Connection = conn;
                _command.CommandText = _str_command;
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@employee_1C", Guid);

                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    _str_command = "INSERT INTO t_Employee (employee_name, employee_1C, employee_FunctionName, employee_FunctionGuid, employee_WorkField) VALUES(@employee_name, @employee_1C, @employee_FunctionName, @employee_FunctionGuid, @employee_WorkField)";
                    _command.Parameters.Clear();
                    _command.Parameters.AddWithValue("@employee_name", Name);
                    _command.Parameters.AddWithValue("@employee_1C", Guid);
                    _command.Parameters.AddWithValue("@employee_FunctionName", functionName);
                    _command.Parameters.AddWithValue("@employee_FunctionGuid", functionGuid);
                    _command.Parameters.AddWithValue("@employee_WorkField", workField);
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }
                else
                {
                    _str_command = "UPDATE t_Employee SET employee_name = @employee_name, employee_FunctionName = @employee_FunctionName, employee_FunctionGuid = @employee_FunctionGuid, employee_WorkField = @employee_WorkField  WHERE employee_1C = '" + Guid + "'";
                    _command.Parameters.Clear();
                    _command.Parameters.AddWithValue("@employee_name", Name);
                    _command.Parameters.AddWithValue("@employee_FunctionName", functionName);
                    _command.Parameters.AddWithValue("@employee_FunctionGuid", functionGuid);
                    _command.Parameters.AddWithValue("@employee_WorkField", workField);
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }



        public void ClearButtonTemplate()
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "DELETE * FROM t_ButtonTemplate";
                _command.CommandText = _str_command;
                _command.ExecuteScalar();
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

        public void ClearRespon()
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "DELETE * FROM t_Responsibility";
                _command.CommandText = _str_command;
                _command.ExecuteScalar();
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

        public void ImportButtonTemplate(OleDbConnection conn, string Guid, string Name, string Value, string Color, string time, bool isUsed, string smenaType, int btnOrder)
        {
            string _str_command;
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                // новая запись
                _str_command = "INSERT INTO t_ButtonTemplate (btn_guid, btn_name, bnt_value, btn_color, btn_hint, btn_IsUsed, btn_SmenaType, btn_order) VALUES(@btn_guid, @btn_name, @bnt_value, @btn_color, @btn_hint, @btn_IsUsed, @btn_SmenaType, @btn_order)";
                _command.Parameters.AddWithValue("@btn_guid", Guid);
                _command.Parameters.AddWithValue("@btn_name", Name);
                _command.Parameters.AddWithValue("@bnt_value", Value);
                _command.Parameters.AddWithValue("@btn_color", Color);
                _command.Parameters.AddWithValue("@btn_hint", time);
                _command.Parameters.AddWithValue("@btn_IsUsed", isUsed);
                _command.Parameters.AddWithValue("@btn_SmenaType", smenaType);
                _command.Parameters.AddWithValue("@btn_Order", btnOrder);
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public void ImportShiftType(OleDbConnection conn, string Guid, string Name, string Value, string Color)
        {
            string _str_command;
            OleDbCommand _command = null;

            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                // новая запись
                _str_command = "INSERT INTO t_ShiftType (type_guid, type_name, type_value, type_color) VALUES(@type_guid, @type_name, @type_value, @type_color)";
                _command.Parameters.AddWithValue("@type_guid", Guid);
                _command.Parameters.AddWithValue("@type_name", Name);
                _command.Parameters.AddWithValue("@type_value", Value);
                _command.Parameters.AddWithValue("@type_color", Color);
                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public void ImportDepart(OleDbConnection conn, string teremok_id, string teremok_1C, string teremok_name, int teremok_dep, string teremok_guid)
        {
            string _str_command;
            OleDbCommand _command = null;
            int _doc_id;
            try
            {
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT COUNT(*) FROM t_Teremok WHERE teremok_id = @teremok_id";
                _command = new OleDbCommand();
                _command.Connection = conn;
                _command.CommandText = _str_command;
                _command.Parameters.Clear();
                _command.Parameters.AddWithValue("@teremok_id", teremok_id);

                _doc_id = Convert.ToInt32(_command.ExecuteScalar());
                if (_doc_id == 0)
                {
                    // новая запись
                    _str_command = "INSERT INTO t_Teremok (teremok_id, teremok_1C, teremok_name, teremok_dep, teremok_guid) VALUES(@teremok_id, @teremok_1C, @teremok_name, @teremok_dep, @teremok_guid)";
                    _command.Parameters.Clear();
                    _command.Parameters.AddWithValue("@teremok_id", teremok_id);
                    _command.Parameters.AddWithValue("@teremok_1C", teremok_1C);
                    _command.Parameters.AddWithValue("@teremok_name", teremok_name);
                    _command.Parameters.AddWithValue("@teremok_dep", teremok_dep);
                    _command.Parameters.AddWithValue("@teremok_guid", teremok_guid);
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }
                else
                {
                    _str_command = "UPDATE t_Teremok SET teremok_1C = @teremok_1C, teremok_name = @teremok_name, teremok_dep = @teremok_dep, teremok_guid = @teremok_guid WHERE teremok_id = " + teremok_id;
                    _command.Parameters.Clear();
                    _command.Parameters.AddWithValue("@teremok_1C", teremok_1C);
                    _command.Parameters.AddWithValue("@teremok_name", teremok_name);
                    _command.Parameters.AddWithValue("@teremok_dep", teremok_dep);
                    _command.Parameters.AddWithValue("@teremok_guid", teremok_guid);
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public int GetDocMarkID(string guid)
        {
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
                _str_command = "SELECT doc_id FROM t_Doc WHERE doc_guid = '" + guid + "'";
                _command.CommandText = _str_command;
                int _count = Convert.ToInt32(_command.ExecuteScalar());
                if (_count == 0)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(_command.ExecuteScalar().ToString());
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

        public void DeleteMark(int _mark_id)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // удалить записи
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "DELETE FROM t_MarkItems WHERE mark_doc_id = " + _mark_id.ToString();
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

        public void ImportMarkEmp(string name, string res_name, string mark_res, int row, int doc)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // удалить записи
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "INSERT INTO t_MarkItems (mark_name, mark_office, mark_res, mark_row_id, mark_doc_id) VALUES(@mark_name, @mark_office, @mark_res, @mark_row_id, @mark_doc_id)";
                _command.Parameters.AddWithValue("@mark_name", name);
                _command.Parameters.AddWithValue("@mark_office", res_name);
                _command.Parameters.AddWithValue("@mark_res", mark_res);
                _command.Parameters.AddWithValue("@mark_row_id", row);
                _command.Parameters.AddWithValue("@mark_doc_id", doc);
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

        public void ImportMarkEmpDetails(string num, string smenaType, string value, string name, string First, string Last, string nameEmp)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            string _str_command2;
            string _str_command3;
            string _str_command4;
            string _str_command5;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            //DataTable _dt = new DataTable();
            //DataTable _returnDT;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                // удалить записи
                _command = new OleDbCommand();
                _command.Connection = _conn;
                //_dt = returnColorMarkNew();
                // _returnDT = returnCheckID(_dt, smenaType);
                _str_command = "UPDATE t_MarkItems SET mark_" + num + " = @mark_" + num + " WHERE mark_name = '" + nameEmp + "'";
                if (value != "0")
                {
                    _command.Parameters.AddWithValue("@mark_" + num, name + value);
                }
                else
                {
                    _command.Parameters.AddWithValue("@mark_" + num, name);
                }

                _command.CommandText = _str_command;
                _command.ExecuteNonQuery();
                _command.Parameters.Clear();

                _str_command2 = "UPDATE t_MarkItems SET mark_guidsmena_" + num + " = @mark_guidsmena_" + num + " WHERE mark_name = '" + nameEmp + "'";
                _command.Parameters.AddWithValue("@mark_guidsmena_" + num, smenaType);
                _command.CommandText = _str_command2;
                _command.ExecuteNonQuery();
                _command.Parameters.Clear();

                _str_command3 = "UPDATE t_MarkItems SET mark_firstime_" + num + " = @mark_firstime_" + num + " WHERE mark_name = '" + nameEmp + "'";
                _command.Parameters.AddWithValue("@mark_firstime_" + num, First);
                _command.CommandText = _str_command3;
                _command.ExecuteNonQuery();
                _command.Parameters.Clear();

                _str_command4 = "UPDATE t_MarkItems SET mark_lasttime_" + num + " = @mark_lasttime_" + num + " WHERE mark_name = '" + nameEmp + "'";
                _command.Parameters.AddWithValue("@mark_lasttime_" + num, Last);
                _command.CommandText = _str_command4;
                _command.ExecuteNonQuery();
                _command.Parameters.Clear();

                _str_command5 = "UPDATE t_MarkItems SET mark_work_" + num + " = @mark_work_" + num + " WHERE mark_name = '" + nameEmp + "'";
                _command.Parameters.AddWithValue("@mark_work_" + num, value);
                _command.CommandText = _str_command5;
                _command.ExecuteNonQuery();
                _command.Parameters.Clear();
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

        private DataTable returnCheckID(DataTable dt, string ch_mnome_id)
        {
            DataTable qwe = null;

            var q = (from row in dt.AsEnumerable()
                     where (string)row["btn_guid"] == ch_mnome_id.ToString()
                     select row).ToArray();

            if (q.Length != 0)
            {
                qwe = q.CopyToDataTable();
            }
            return qwe;
        }

        public string GetValue(string Guid)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT btn_hint FROM t_ButtonTemplate WHERE btn_guid = @btn_guid";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@btn_guid", Guid);
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

        public void ImportWorkTeremok(DateTime lastTime, DateTime firstTime, string teremok_ID, int month, int year, int day)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            int _id = 0;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT id FROM t_WorkTeremok WHERE teremok_day = @teremok_day AND teremok_month = @teremok_month AND teremok_year = @teremok_year";
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _command.CommandText = _str_command;
                //_command.Parameters.Clear();
                //_command.Parameters.AddWithValue("@teremok_id", teremok_ID);
                _command.Parameters.AddWithValue("@teremok_day", day);
                _command.Parameters.AddWithValue("@teremok_month", month);
                _command.Parameters.AddWithValue("@teremok_year", year);

                _id = Convert.ToInt32(_command.ExecuteScalar());
                if (_id == 0)
                {
                    _str_command = "INSERT INTO t_WorkTeremok (teremok_id, teremok_month, teremok_year, teremok_day, teremok_firstTime, teremok_lastTime) VALUES(@teremok_id, @teremok_month, @teremok_year, @teremok_day, @teremok_firstTime, @teremok_lastTime)";
                    _command.Parameters.Clear();
                    _command.Parameters.AddWithValue("@teremok_id", teremok_ID);
                    _command.Parameters.AddWithValue("@teremok_month", month);
                    _command.Parameters.AddWithValue("@teremok_year", year);
                    _command.Parameters.AddWithValue("@teremok_day", day);
                    _command.Parameters.AddWithValue("@teremok_firstTime", firstTime);
                    _command.Parameters.AddWithValue("@teremok_lastTime", lastTime);
                    _command.CommandText = _str_command;
                    _command.ExecuteNonQuery();
                }
                else
                {
                    _str_command = "UPDATE t_WorkTeremok SET teremok_firstTime = @teremok_firstTime, teremok_lastTime = @teremok_lastTime WHERE id = " + _id;
                    _command.Parameters.Clear();
                    _command.Parameters.AddWithValue("@teremok_firstTime", firstTime);
                    _command.Parameters.AddWithValue("@teremok_lastTime", lastTime);
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

        public string GetValueSmena(string GuidTemplate)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT btn_SmenaType FROM t_ButtonTemplate WHERE btn_guid = @btn_guid";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@btn_guid", GuidTemplate);
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

        public void ImportResponsibility(string _Guid, string Name)
        {
            string _str_connect = CParam.ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            int _id = 0;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;

                _str_command = "INSERT INTO t_Responsibility (res_guid, res_name) VALUES (@res_guid, @res_name)";
                _command.Parameters.Clear();
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@res_guid", _Guid.ToString());
                _command.Parameters.AddWithValue("@res_name", Name.ToString());
                _command.ExecuteNonQuery();
                //_str_command = "SELECT COUNT(*) FROM t_Responsibility WHERE Guid = @Guid AND Name = @Name";
                //_command.Parameters.Clear();
                //_command = new OleDbCommand();
                //_command.Connection = _conn;
                //_command.CommandText = _str_command;
                //_command.Parameters.Clear();
                //_command.Parameters.AddWithValue("@Guid", _Guid);
                //_command.Parameters.AddWithValue("@Name", Name);

                //_id = Convert.ToInt32(_command.ExecuteScalar());
                //if (_id == 0)
                //{
                //    _str_command = "INSERT INTO t_Responsibility (Guid, Name) VALUES (@Guid, @Name)";
                //    _command.Parameters.Clear();
                //    _command.Parameters.AddWithValue("@Guid", _Guid);
                //    _command.Parameters.AddWithValue("@Name", Name);
                //    _command.CommandText = _str_command;
                //    _command.ExecuteNonQuery();
                //}
                //else
                //{
                //    _str_command = "UPDATE t_Responsibility SET Guid = @Guid, Name = @Name WHERE id = " + _id;
                //    _command.Parameters.Clear();
                //    _command.Parameters.AddWithValue("@Guid", _Guid);
                //    _command.Parameters.AddWithValue("@Name", Name);
                //    _command.CommandText = _str_command;
                //    _command.ExecuteNonQuery();
                //}
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

        public string resGUID(string name)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                conn = new OleDbConnection(_str_connect);
                conn.Open();
                _command = new OleDbCommand();
                _command.Connection = conn;
                _str_command = "SELECT res_guid FROM t_Responsibility WHERE res_name = '" + name + "'";
                _command.CommandText = _str_command;
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception _exp)
            {
                return "";
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public string returnShortName(string Guid)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT type_value FROM t_ShiftType WHERE type_guid = @type_guid";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@type_guid", Guid);
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

        public string returnShiftType(string Guid)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT type_name FROM t_ShiftType WHERE type_guid = @type_guid";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@type_guid", Guid);
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception _exp)
            {
                return null;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public string returnShiftTypeGuid(int numColumn, int numRow)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT mark_guidsmena_" + numColumn + " FROM t_MarkItems WHERE mark_id = @mark_id";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@mark_id", numRow);
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public string returnShift(string nameShift)
        {
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT type_guid FROM t_ShiftType WHERE type_name = @type_name";
                _command.CommandText = _str_command;
                _command.Parameters.AddWithValue("@type_name", nameShift);
                return _command.ExecuteScalar().ToString();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }

        public DataTable returnDTTotalDays(string row)
        {
            OleDbConnection _conn = null;
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter("SELECT mark_work_1, mark_work_2, mark_work_3, mark_work_4, mark_work_5, mark_work_6, mark_work_7, mark_work_8, mark_work_9, mark_work_10, mark_work_11, mark_work_12, mark_work_13, mark_work_14, mark_work_15, mark_work_16, mark_work_17, mark_work_18, mark_work_19, mark_work_20, mark_work_21, mark_work_22, mark_work_23, mark_work_24, mark_work_25, mark_work_26, mark_work_27, mark_work_28, mark_work_29, mark_work_30, mark_work_31 FROM t_MarkItems WHERE mark_id = " + row, _conn);

                _table = new DataTable("DTTotalDays");
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

        public DataTable returnDTrow(int doc)
        {
            OleDbConnection _conn = null;
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter("SELECT mark_id FROM t_MarkItems WHERE mark_doc_id = " + doc, _conn);

                _table = new DataTable("DTrow");
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

        public int ReturnDocIDFromTask(int doc_id)
        {
            //
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            string _str_command;

            try
            {
                // формируем комме
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT doc_type_id FROM t_Doc WHERE doc_id = " + doc_id;
                _command.CommandText = _str_command;
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


        /// <summary>
        /// Получает документы за текущий месяц
        /// </summary>
        /// <param name="doc_type">тип документа</param>
        /// <param name="month">месяц</param>
        /// <returns></returns>
        public IEnumerable<t_Doc> GetDocumentByMonth(int doc_type, int month)
        {
            if (doc_type <= 0)
                throw new ArgumentException("Ошибка типа документа", "doc_type");
            if (month <= 0 || month > 12)
                throw new ArgumentException("Месяц вне границ", "month");

            return new t_Doc().Select<t_Doc>("doc_type_id=" + doc_type + " AND month(doc_datetime)=" + month);

        }

        /// <summary>
        /// Получает первый документ за указанный месяц
        /// </summary>
        /// <param name="doc_type">тип документа</param>
        /// <param name="month">месяц</param>
        /// <returns></returns>
        public t_Doc GetDocumentByMonthFirst(int doc_type, int month)
        {
            IEnumerable<t_Doc> docs = GetDocumentByMonth(doc_type, month);
            if (docs == null || docs.Count() == 0) return null;
            return docs.First();
        }

        internal int GetStatusID(int Doc_type, DocStatusType docStatusType)
        {
            t_DocStatusRef statusRef = new t_DocStatusRef().SelectFirst<t_DocStatusRef>
                ("doctype_id=" + Doc_type + " AND statustype_id=" + (int)docStatusType);
            return statusRef.docstatusref_id;
        }

        public t_Doc CreateDocumentInDb(int DocType, DateTime DocDate)
        {
            t_Doc this_doc = CreateDocument(DocType, DocDate);
            this_doc.Create();
            FillTableFromTemplateIfNeeded(this_doc);
            return this_doc;
        }
        public t_Doc CreateDocument(int DocType, DateTime DocDate)
        {
            t_Doc this_doc = null;
            t_DocStatusRef status = new t_DocStatusRef().Select<t_DocStatusRef>
                    ("doctype_id=" + DocType + " AND statustype_id=1").First();

            this_doc = new t_Doc()
            {
                doc_datetime = DocDate,
                doc_type_id = DocType,
                doc_guid = Guid.NewGuid().ToString(),
                doc_teremok_id = int.Parse(StaticConstants.Teremok_ID),
                doc_status_id = status.docstatusref_id
            };

            return this_doc;
        }

        public void SetDocStatus(t_Doc t_doc, int doc_status)
        {
            t_DocStatusRef status = new t_DocStatusRef().Select<t_DocStatusRef>
                    ("doctype_id=" + t_doc.doc_type_id + " AND statustype_id="+doc_status).First();

            if (status != null)
            {
                t_doc.doc_status_id = status.docstatusref_id;
                t_doc.Update();
            }
            else
            {
                MDIParentMain.Log("SetDocStatus error "+Serializer.JsonSerialize(new object[]{t_doc,doc_status}));
            }
        }

        public void SetDocStatus(t_Doc t_doc, int doc_status,bool updateView)
        {
            t_DocStatusRef status = new t_DocStatusRef().Select<t_DocStatusRef>
                    ("doctype_id=" + t_doc.doc_type_id + " AND statustype_id=" + doc_status).First();

            if (status != null)
            {
                t_doc.doc_status_id = status.docstatusref_id;

               // public enum DocumentStatusTypes { New = 1, Sending = 2, Sended = 3, Recieved = 4, NRecieved = 5 }
                if (doc_status == 3)
                {
                    t_doc.doc_desc = "Отправлено " + DateTime.Now.ToString();
                }

                t_doc.Update();
                if (updateView)
                {
                    StaticConstants.MainGridUpdate();
                }
            }
            else
            {
                MDIParentMain.Log("SetDocStatus error " + Serializer.JsonSerialize(new object[] { t_doc, doc_status }));
            }
        }

        private void FillTableFromTemplateIfNeeded(t_Doc doc)
        {
            switch (doc.doc_type_id)
            {
                case 16 :
                    FillTableFromTemplate(doc);                        
                    break;
                default :
                    break;
            }
        }

        private void FillTableFromTemplate(t_Doc doc)
        {
            var dbact = new AccessDataBaseAction((o) =>
            {
                OleDbConnection _conn = (OleDbConnection)o;
                OleDbCommand _command = new OleDbCommand();
                _command.Connection = _conn;

                int teremok_id = StaticConstants.Current_Teremok_ID_int;
                int doc_type_id = doc.doc_type_id;
                int _doc_id = doc.doc_id;

                string _str_command = "SELECT nome_id, n2t_ed, n2t_bed, n2t_K, n2t_quota, n2t_maxquota, n2t_ke, n2t_ze, n2t_bold,group_id FROM t_Nomenclature " +
                        " INNER JOIN t_Nome2Teremok ON t_Nomenclature.nome_id = t_Nome2Teremok.n2t_nome_id WHERE n2t_nt_id = "
                            + doc_type_id.ToString() + " AND n2t_teremok_id=" + teremok_id.ToString() + " ORDER BY n2t_id ASC";

                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                DataTable _table = new DataTable("t_Nomenclature");
                _data_adapter.Fill(_table);

                {
                    _str_command = "INSERT INTO t_Order2ProdDetails(opd_doc_id, opd_nome_id, opd_ed, opd_bed, opd_K, opd_quota, opd_maxquota, opd_ke, opd_ze, opd_bold,group_id) VALUES (@opd_doc_id, @opd_nome_id, @opd_ed, @opd_bed, @opd_K, @opd_quota, @opd_maxquota, @opd_ke, @opd_ze, @opd_bold,@group_id)";
                    _command.CommandText = _str_command;
                    _command.Parameters.Clear();
                    _command.Parameters.Add("@opd_doc_id", OleDbType.Integer);
                    _command.Parameters.Add("@opd_nome_id", OleDbType.Integer);
                    _command.Parameters.Add("@opd_ed", OleDbType.VarChar, 50);
                    _command.Parameters.Add("@opd_bed", OleDbType.VarChar, 50);
                    _command.Parameters.Add("@opd_K", OleDbType.Double);
                    _command.Parameters.Add("@opd_quota", OleDbType.Double);
                    _command.Parameters.Add("@opd_maxquota", OleDbType.Double);
                    _command.Parameters.Add("@opd_ke", OleDbType.Double);
                    _command.Parameters.Add("@opd_ze", OleDbType.VarChar, 50);
                    _command.Parameters.Add("@opd_bold", OleDbType.Integer);
                    _command.Parameters.Add("@group_id", OleDbType.Integer);
                    

                    // заполняем таблицу заказов
                    foreach (DataRow _row in _table.Rows)
                    {
                        _command.Parameters[0].Value = _doc_id;
                        _command.Parameters[1].Value = Convert.ToInt32(_row[0]);
                        _command.Parameters[2].Value = _row[1].ToString();
                        _command.Parameters[3].Value = _row[2].ToString();
                        _command.Parameters[4].Value = Convert.ToDecimal(_row[3]);
                        _command.Parameters[5].Value = Convert.ToDecimal(_row[4]);
                        _command.Parameters[6].Value = Convert.ToDecimal(_row[5]);
                        _command.Parameters[7].Value = Convert.ToDecimal(_row[6]);
                        _command.Parameters[8].Value = _row[7].ToString();
                        _command.Parameters[9].Value = Convert.ToInt32(_row[8]);
                        _command.Parameters[10].Value =_row[9];
                        
                        _command.ExecuteNonQuery();
                    }
                }
            }, null);
            dbact.Start();
        }

        public t_Doc GetDocumentByDate(int DocType, DateTime DocDate)
        {
            t_Doc this_doc=null;
            List<t_Doc> docs = new t_Doc().Select<t_Doc>
                (String.Format("doc_datetime>=" + SqlWorker.ReturnDate(DocDate) + " AND doc_type_id=" +
                DocType + " AND doc_teremok_id=" + StaticConstants.Teremok_ID))
                .Where(a => a.doc_datetime.Date == DocDate.Date).ToList();
            if (docs.Count == 0)
            {
                
            }
            else
            {
                if (docs.Count > 1)
                {
                    MDIParentMain.Log("Ошибка! Больше одного документа "+DocType+" за текущую дату " + DocDate);
                }
                this_doc = docs.Last();
            }
            return this_doc;
        }

        internal string GetTypeDocName(int doc_type_id)
        {
            t_DocTypeRef o = new t_DocTypeRef().SelectFirst<t_DocTypeRef>("doctype_id=" + doc_type_id);
            if (o == null)
            {
                return "";
            }
            else
            {
                return o.doctype_name;
            }
        }

        public string _GetStringAssociation(string guid)
        {
            var a = new t_PropValue().Select<t_PropValue>(
                String.Format("prop_name='{0}' and prop_type='{1}'",
                    guid, StaticConstants.Teremok_ID)
                );

            if (a != null) return a.First().prop_value;

            return guid;
        }

        public t_PropValue _GetStringAssociation_(string guid)
        {
            var a = new t_PropValue().Select<t_PropValue>(
                String.Format("prop_name='{0}' and prop_type='{1}'",
                    guid, StaticConstants.Teremok_ID)
                );

            if (a != null) return a.First();
            return null;
        }
    }
}
