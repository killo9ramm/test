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
    public partial class FormReestr : Form
    {
        public FormReestr()
        {
            InitializeComponent();
        }

        private void FormReestr_Load(object sender, EventArgs e)
        {
            try
            {
                // установить размеры окна
                this.SetBounds(0, 0, Parent.Width - 5, Parent.Height - 5);

                Uri _uri = new Uri(CParam.AppFolder + "/help/reestrdoc.mht");
                webBrowser1.Url = _uri;
            }
            catch(Exception _exp)
            {
                MessageBox.Show(_exp.Message);
            }
        }
    }
}
