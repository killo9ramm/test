using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.ru.teremok.msk;
using System.Net;
using RBClient.Classes;

namespace RBClient
{
    public partial class FormEmployee : Form
    {
        public int docID = 0;
        public int rowCount = 0;

        ARMWeb systemService = StaticConstants.WebService;

        public FormEmployee()
        {
            InitializeComponent();
        }

        private void FormEmployee_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button_OK_Click(this,null);
            }

            if (e.KeyCode == Keys.Escape)
            {
                button_Cancel_Click(this, null);
            }
        }

        private void FormEmploeye_Load(object sender, EventArgs e)
        {
            try
            {
                this.KeyPreview = true;
                this.KeyUp+=new KeyEventHandler(FormEmployee_KeyUp);

                this.SetBounds((Screen.GetBounds(this).Width / 4) - (this.Width / 4),
               (Screen.GetBounds(this).Height / 4) - (this.Height / 4), 337, 190);

                CBData _data = new CBData();
                ddlEmploeye.DisplayMember = "employee_name";
                ddlEmploeye.ValueMember = "employee_1C";
                ddlEmploeye.DataSource = _data.EmpFullList();
            }
            catch
            {
            }
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            try
            {
                CBData _data = new CBData();
                // systemService.Credentials = new NetworkCredential("iteremok", "Ge6!))~nC5@0");
                string guid_1C = Convert.ToString(ddlEmploeye.SelectedValue.ToString());
                string FIO = Convert.ToString(ddlEmploeye.Text.ToString());

                //WorkOther[] wo = systemService.GetWorkOther(193, guid_1C, DateTime.Now);
                //foreach (var item in wo)
                //{
                //    _data.WorkOther(guid_1C, item.GuidShift, item.Day, item.Value, item.FirstTime, item.LastTime);
                //}
                // _data.OrderAddEmp(guid_1C, _data.GeFunction(guid_1C), docID, rowCount);

                _data.OrderAddEmp(guid_1C, docID, rowCount);
                this.Close();
            }
            catch
            {
            }
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
