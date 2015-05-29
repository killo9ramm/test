using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.Classes;

namespace RBClient.ModalWindows
{
    public partial class PasswordForm : Form
    {
        public PasswordForm()
        {
            InitializeComponent();
            this.Activated += windows_activated;
        }

        private void windows_activated(object sender,EventArgs e)
        {
            this.buttonOk.Focus();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            //проверить пароль
            if(checkPassword()) this.DialogResult = DialogResult.OK;
            else this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        internal bool checkPassword()
        {
            string query="SELECT t_Conf.conf_value FROM t_Conf WHERE (((t_Conf.conf_param)=\"password\"));";
            DataTable dt=SqlWorker.SelectFromDBSafe(query,"t_Conf");
            string password = CellHelper.FindCell(dt, 0, 0).ToString();

            if (passwBox.Text == MDIParentMain.AdminPassword) return true;
            if (passwBox.Text == password) return true;
            return false;   
        }
    }
}
