using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Collections;

namespace RBClient
{
    public partial class FormListBoxShow<T> : Form
       // where T:object
    {
        public int orr_id;
        public int opd_id;
        public string orr_1C;
        public bool is_updated = false; // true = данные обновлены
        public List<T> list_toView;
        internal T outItem;

        private FormListBoxShow()
        {
            InitializeComponent();
            
          //  Load += FormSpisok_Load;
        }


        public FormListBoxShow(List<T> _list)
        {
            InitializeComponent();
            list_toView = _list;
        }


        private void FormSpisok_Load(object sender, EventArgs e)
        {
            try
            {   // заполняем бокс
                reason.DataSource = list_toView;
                
                this.AcceptButton = button_OK;
                this.CancelButton = button_Cancel;

                reason.DropDownWidth = DropDownWidth(reason);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        int DropDownWidth(ComboBox myCombo)
        {
            int maxWidth = 0;
            int temp = 0;
            Label label1 = new Label();
            label1.Font = reason.Font;


            foreach (var obj in myCombo.Items)
            {
                label1.Text = obj.ToString();
                temp = label1.PreferredWidth;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            label1.Dispose();
            return maxWidth;
        }


        private void button_OK_Click(object sender, EventArgs e)
        {
            AddReason();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void AddReason()
        {
            try
            {
                outItem = (T)reason.SelectedItem;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public void Delete_Comm()
        {
            
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
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
