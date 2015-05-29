using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RBClient
{
    public partial class FormSettingsDoc : Form
    {

        public int column_doc_id = 0;
        DataTable _table = new DataTable();
        CBData _data = new CBData();
        public FormSettingsDoc()
        {
            InitializeComponent();
        }

        private void FormSettingsDoc_Load(object sender, EventArgs e)
        {
            _table = _data.returnStatusColumn(column_doc_id);

            foreach (DataRow _row in _table.Rows)
            {
                checkedListBox1.Items.Add(_row[4], Convert.ToBoolean(_row[3]));
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                if (checkedListBox1.SelectedItem != null)
                {
                    if (checkedListBox1.GetItemChecked(e.Index) == true)
                    {
                        _data.updateStatusDoc(checkedListBox1.SelectedItem.ToString(), column_doc_id, false);
                    }
                    else
                    {
                        _data.updateStatusDoc(checkedListBox1.SelectedItem.ToString(), column_doc_id, true);
                    }
                }
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }
    }
}
