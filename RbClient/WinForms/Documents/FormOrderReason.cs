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
    public partial class FormOrderReason : Form
    {
        public int orr_id;
        public int opd_id;
        public string orr_1C;
        public bool is_updated = false; // true = данные обновлены

        public FormOrderReason()
        {
            InitializeComponent();
        }

        private void FormSpisok_Load(object sender, EventArgs e)
        {
            try
            {   // заполняем бокс
                CBData _data = new CBData();
                reason.DisplayMember = "orr_name";
                reason.ValueMember = "orr_1C";
                reason.DataSource = _data.ReasonFullList();
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            AddReason();
        }

        private void AddReason()
        {
            try
            {
                orr_1C = Convert.ToString(reason.SelectedValue.ToString());

                // получить параметры номенклатуры
                CBData _data = new CBData();
                _data.Order2ProdUpdateItemReason(opd_id, orr_1C);
                               
                this.Close();
                is_updated = true;

            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public void Delete_Comm()
        {
            CBData _data = new CBData();
            _data.Order2ProdDeleteItemReason(opd_id);
            is_updated = true;
        }

        private void ddlTeremok_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
               AddReason();
            }
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
