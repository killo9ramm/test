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
    public partial class FormChooseTeremok : Form
    {
        public int m_teremok_id = 0;
        public FormChooseTeremok()
        {
            InitializeComponent();
        }

        private void FormInventChooseTeremok_Load(object sender, EventArgs e)
        {
            try
            {
                CBData _data = new CBData();
                ddlTeremok.DisplayMember = "teremok_name";
                ddlTeremok.ValueMember = "teremok_id";
                ddlTeremok.DataSource = _data.TeremokFullList();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            m_teremok_id = Convert.ToInt32(ddlTeremok.SelectedValue.ToString());
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
