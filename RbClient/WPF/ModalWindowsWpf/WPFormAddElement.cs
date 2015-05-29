using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using RBClient.WPF.UserControls;

namespace RBClient.WPF.UserControls.ModalWindowsWpf
{
    public partial class WPFormAddElement : Form
    {
        public ElementHost elhost;
        public AddElement wpfctl;

        public WPFormAddElement()
        {
            InitializeComponent();

            elhost = new ElementHost();
            elhost.Size = new Size(this.Width-20, this.Height-40);
            elhost.Location = new Point(0, 0);

            wpfctl = new AddElement();
            wpfctl.Parent = this;
            elhost.Child = wpfctl;

            this.Controls.Add(elhost);
            Resize += Resized;
        }

        private void Resized(object sender, EventArgs e)
        {
            elhost.Size = new Size(this.Width-20, this.Height-40);
        }
    }
}
