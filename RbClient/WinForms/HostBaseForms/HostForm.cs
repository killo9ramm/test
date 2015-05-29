using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace RBClient
{
    public partial class HostForm : Form
    {
        ElementHost elhost;
        System.Windows.Controls.UserControl MainControl;
        public HostForm(System.Windows.Controls.UserControl ucontrol)
        {
            InitializeComponent();

            elhost = new ElementHost();
            elhost.Size = new Size(this.Width - 20, this.Height - 40);
            elhost.Location = new Point(0, 0);

            MainControl = ucontrol;
            elhost.Child = MainControl;
            this.Controls.Add(elhost);
            this.Resize += FormMarochOtch_Resize;
        }

        private void FormMarochOtch_Resize(object sender, EventArgs e)
        {
            elhost.Size = new Size(this.Width - 20, this.Height - 40);
        }
    }
}
