using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RBClient.ModalWindows
{
    public partial class MaskedFormInput : Form
    {

        public MaskedFormInput()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //проверить пароль
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
