using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Data.OleDb;

namespace RBClient
{
    public partial class FormSendZReportManual : Form
    {
        public FormSendZReportManual()
        {
            InitializeComponent();
        }

        private void button_Send_Click(object sender, EventArgs e)
        {
            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;
            OleDbCommand _command;
            DataTable _table;
            string _str_command;            
            CZReportHelper _z = new CZReportHelper();

            int _count = 0; // счетчик удаленных позиций

            // отравим большой пакет данных (в один файл)
            if (checkBox_SentArch.Checked)
            { 
                // отправляем файл с данными
            }

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                _command = new OleDbCommand();
                _command.Connection = _conn;

                _str_command = "SELECT doc_id FROM t_Doc WHERE doc_type_id = 5 AND " +
                    "doc_datetime >= #" + monthCalendar_From.SelectedDate.Month + "/" + monthCalendar_From.SelectedDate.Day + "/" + monthCalendar_From.SelectedDate.Year +
                     " 00:00:00#  AND doc_datetime <= #" + monthCalendar_To.SelectedDate.Month + "/" + monthCalendar_To.SelectedDate.Day + "/" + monthCalendar_To.SelectedDate.Year + " 23:59:59#";
                OleDbDataAdapter _data_adapter = new OleDbDataAdapter(_str_command, _conn);
                _table = new DataTable("t_ZToDel");
                _data_adapter.Fill(_table);

                foreach (DataRow _row in _table.Rows)
                {
                    _z.ZReportDelete(Convert.ToInt32(_row[0].ToString()));
                    _count++;
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

            //MessageBox.Show("Удалено " + _count.ToString() + " Z-отчетов");
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox_SentArch_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void FormSendZReportManual_Load(object sender, EventArgs e)
        {
            monthCalendar_From.DisplayMonth = DateTime.Today;
            monthCalendar_To.DisplayMonth = DateTime.Today;
        }
    }
}
