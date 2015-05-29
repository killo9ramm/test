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


using RBClient.Classes;
using System.Windows.Forms;


namespace RBClient
{
    public enum DocStatusType{New=1,Sending=2,Sended=3,UpdatedRecieved=4,Recieved=5}
    class CBData
    {
        public int m_teremok_id = 0;

        public CBData()
        {
        }

        public DataTable getdKkms()
        {
            string _str_connect = ConnString;
            OleDbConnection _conn = null;
            string _str_command = "SELECT conf_value FROM t_conf " +
                                   "WHERE (((t_conf.[conf_param]) Like 'folder_kkm%')AND ((t_conf.conf_value)<>''))";
            //;";
            
            DataTable _table;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);

                _table = new DataTable("t_conf");
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

        public List<string> getKkms()
        {
            DataTable dt = getdKkms();
            if (dt != null && dt.Rows.OfType<DataRow>().NotNullOrEmpty())
            {
                List<string> ls=new List<string>();
                for(int i=0;i<dt.Rows.Count;i++)
                {
                    ls.Add(dt.Rows[i][0].ToString());
                }
                return ls;
            }
            return null;
        }

        public CBData(int user_id, int teremok_id)
        {
            m_teremok_id = teremok_id;
        }

        public static string ConnString = "";

        public void SetConnString(string AppFolder)
        {
            ConnString = "Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;Data Source=" + AppFolder + "\\Data\\RBA.accdb";
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
                _conn = new OleDbConnection(ConnString);
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
                _conn = new OleDbConnection(ConnString);
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

        
        public int GetDocTypeID(string doctype_name)
        {
            // проверим на тип ВСЕ
            if (doctype_name == "Все документы")
                return 0;

            string _str_connect = ConnString;
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

        public string Get2(int teremId)
        {
            string _str_connect = ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT teremok_1C FROM t_teremok WHERE teremok_id=" + teremId + "";
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

        public int Get1()
        {
            string _str_connect = ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT TOP 1 t_Conf.conf_value from t_conf where (t_Conf.conf_param='teremok_id');";

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

        public DataTable CountCurrentTeremok()
        {
            string _str_connect = ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            DataTable dt=new DataTable();

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = "SELECT t_Teremok.[teremok_id],teremok_name,teremok_1C FROM t_Teremok where  teremok_current=-1;";

                _command.CommandText = _str_command;
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_command);
                _data_adapter.Fill(dt);

                return dt;
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

        internal void UnCurrentTeremok(string _terem_id)
        {
            string _str_connect = ConnString;
            string _str_command;
            OleDbConnection _conn = null;
            OleDbCommand _command = null;
            DataTable dt = new DataTable();

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;
                _str_command = String.Format("update t_Teremok set teremok_current=0 where teremok_id={0};",_terem_id);
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
    }

    class CellHelper
    {
        internal int RowIndex;
        internal int ColumnIndex;
        internal int ColumnName;
        internal object CellValue;

        private static List<DataGridViewComboBoxCell> _parent;
        internal static List<DataGridViewComboBoxCell> Parent
        {
            get
            {
                if (_parent == null) _parent = new List<DataGridViewComboBoxCell>();
                return _parent;
            }
        }

        internal static int FindColumnIndex(DataGridView gridView, string colname)
        {
            int index = -1;
            foreach (DataGridViewColumn col in gridView.Columns)
            {
                if (col.DataPropertyName == colname)
                {
                    index = col.Index;
                    break;
                }
            }
            return index;
        }

        internal static int FindColumnIndex(DataTable gridView, string colname)
        {
            int index = -1;
            foreach (DataColumn col in gridView.Columns)
            {
                if (col.ColumnName == colname)
                {
                    index = gridView.Columns.IndexOf(col);
                    break;
                }
            }
            return index;
        }

        internal static DataRow FindRow(DataTable gridView, string colname, string value)
        {
            int colIndex = CellHelper.FindColumnIndex(gridView, colname);
            foreach (DataRow row in gridView.Rows)
            {
                if (row.ItemArray[colIndex].ToString() == value)
                {
                    return row;
                }
            }
            return null;
        }

        internal static object FindCell(DataGridView gridView, int rowIndex, string colName)
        {
            object val = null;
            int colIndex = -1;

            colIndex = FindColumnIndex(gridView, colName);

            if (colIndex == -1)
            {
                return null;
            }
            val = FindCell(gridView, rowIndex, colIndex);

            return val;
        }

        internal static object FindCell(DataGridView gridView, int rowIndex, int colIndex)
        {
            DataGridViewCell cell = gridView.Rows[rowIndex].Cells[colIndex];
            if (cell != null) return cell;
            return null;
        }


        internal static int FindColumn(DataTable gridView, string colname)
        {
            int index = -1;
            foreach (DataColumn col in gridView.Columns)
            {
                if (col.ColumnName.ToLower() == colname.ToLower())
                {
                    index = gridView.Columns.IndexOf(col);
                }
            }
            return index;
        }

        internal static object FindCell(DataTable gridView, int rowIndex, string colName)
        {
            object val = null;
            int colIndex = -1;

            colIndex = FindColumn(gridView, colName);

            if (colIndex == -1)
            {
                return null;
            }
            val = FindCell(gridView, rowIndex, colIndex);

            return val;
        }

        internal static object FindCell(DataTable gridView, int rowIndex, int colIndex)
        {
            object cell = gridView.Rows[rowIndex].ItemArray[colIndex];
            if (cell != null) return cell;
            return null;
        }

        internal static object FindCell(DataRow rowData, string colName)
        {
            object val = null;
            int colIndex = -1;

            DataTable gridView = rowData.Table;
            colIndex = FindColumn(gridView, colName);
            int rowIndex = gridView.Rows.IndexOf(rowData);


            if (colIndex == -1)
            {
                return null;
            }
            val = FindCell(gridView, rowIndex, colIndex);

            return val;
        }

        internal static object FindCell(DataRow rowData, int colIndex)
        {
            object val = null;
            DataTable gridView = rowData.Table;
            int rowIndex = gridView.Rows.IndexOf(rowData);
            if (colIndex == -1)
            {
                return null;
            }
            val = FindCell(gridView, rowIndex, colIndex);
            return val;
        }
    }

}
