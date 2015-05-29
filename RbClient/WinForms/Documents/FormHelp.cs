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
    public partial class FormHelp : Form
    {
        public FormHelp()
        {
            InitializeComponent();
        }

        private void FormHelp_Load(object sender, EventArgs e)
        {
            try
            {
                // установить размеры окна
                this.SetBounds(0, 0, Parent.Width - 5, Parent.Height - 5);
                Uri _uri = new Uri(CParam.AppFolder + "/help/index.mht");
                webBrowser1.Url = _uri;
            }
            catch(Exception _exp)
            {
                MessageBox.Show(_exp.Message);
            }
        }
    }
}
