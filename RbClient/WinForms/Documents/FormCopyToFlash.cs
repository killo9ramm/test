using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;

using System.Collections;

namespace RBClient
{
    public partial class FormCopyToFlash : Form
    {
        public string m_drive = "";

        public FormCopyToFlash()
        {
            InitializeComponent();
        }

        private void button_export_Click(object sender, EventArgs e)
        {
            ToDo();
            MDIParentMain mdi = new MDIParentMain();
            FormDoc _fd = new FormDoc();
            mdi.CheckZReportFlesh();
            mdi.Exchange(m_drive);
            Copy2Flesh();
            _fd.IninData();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormCopyToFlash_Load(object sender, EventArgs e)
        {
            if (m_drive != null)
            {
                InitData();
            }
        }

        private void InitData()
        {
            this.SetBounds(0, 0, Parent.Width - 4, Parent.Height - 4);
            // ищем документы, которые нужно послать
            string _d;
            try
            {
                CBData _data = new CBData();
                DataTable _dt = _data.GetDoc2Send();
                foreach (DataRow _row in _dt.Rows)
                {
                    _d = _row[0].ToString() + " №:" + _row[1].ToString() + ": (от " + _row[3].ToString() + ")";
                    checkedListBox_Tasks.Items.Add(_d, true);
                }
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        //помететь в базе, что выбранные документы готовы к отправке.
        private void ToDo()
        {
            string[] _s;
            ArrayList _array = new ArrayList();
            char[] _separator = ":".ToCharArray();
            CBData _data = new CBData();

            try
            {
                int _i = 0;
                for (; _i < checkedListBox_Tasks.Items.Count; _i++)
                {
                    if (checkedListBox_Tasks.GetItemChecked(_i))
                    {
                        _s = checkedListBox_Tasks.Items[_i].ToString().Split(_separator);
                        _data.AddTaskExchange(_s[1]);
                    }
                }

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
            }
        }

        private void Copy2Flesh()
        {
            try
            {
                DirectoryInfo _dir = new DirectoryInfo(CParam.AppFolder + "\\outbox\\");
                foreach (FileInfo _file in _dir.GetFiles())
                {
                    if (!Directory.Exists(m_drive + "\\outbox\\"))
                        Directory.CreateDirectory(m_drive + "\\outbox\\");
                    File.Copy(_file.FullName, m_drive + "\\outbox\\" + _file.Name, true);
                    File.Delete(CParam.AppFolder + "\\outbox\\" + _file.Name);
                }
            }
            catch
            {
            }
        }
    }
}
