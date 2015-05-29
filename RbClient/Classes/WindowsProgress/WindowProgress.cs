using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RBClient.Classes
{
    public partial class WindowProgress : Form
    {
        private string _header = "";
        public string Header
        {
            get { return _header; }
            set 
            {
                _header = value;
                label1.Text = _header;
            }
        }

        private int _status = 0;
        public int Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                if (_status < 0) _status = 0;
                if (_status > 100) _status = 100;

                progressBar1.Value = _status;
            }
        }

        public WindowProgress()
        {
            InitializeComponent();
        }

        private void WindowProgress_FormClosing(object sender, EventArgs e)
        {
            Status = 100;
        }

        public void CClose()
        {
            Status = 100;
            this.Close();
        }
    }
}
