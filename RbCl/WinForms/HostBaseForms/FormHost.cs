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
using RBClient.Classes;
using RBClient.Classes.DocumentClasses;

namespace RBClient
{
    public partial class FormHost : Form
    {
        protected ElementHost elhost;
        protected System.Windows.Controls.UserControl ChildControl;

        public event FormClosingEventHandler FormClosingRequested;

        public FormHost(string Header, System.Windows.Controls.UserControl wpfctl)
        {
            InitializeComponent();

            elhost = new ElementHost();
            
            elhost.Size = new Size(this.Width-20, this.Height-40);
            elhost.Location = new Point(0, 0);

            this.Text = Header;

            Load += (s, e) =>
            {
                elhost.Child = wpfctl;
                this.Controls.Add(elhost);
            };
            FormClosing += FormMarochOtch_FormClosing;
        }

        public FormHost(OrderClass order)
        {
            InitializeComponent();

            elhost = new ElementHost();

            elhost.Size = new Size(this.Width - 20, this.Height - 40);
            elhost.Location = new Point(0, 0);

            System.Windows.Controls.UserControl wpfctl = OrderFactory.ConstructMainView(order);
            ChildControl = wpfctl;

            Load += (s, e) =>
            {
                elhost.Child = wpfctl;
                this.Controls.Add(elhost);
            };
            FormClosing += FormMarochOtch_FormClosing;
        }

        private void FormMarochOtch_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormClosingRequested != null)
            {
                FormClosingRequested(this, e);
            }
        }

        private void FormHost_Resize(object sender, EventArgs e)
        {
            elhost.Size = new Size(this.Width-20, this.Height-40);
        }
             
        protected override void WndProc(ref Message message)
        {
            try
            {
                const int WM_SYSCOMMAND = 0x0112;
                const int SC_MOVE = 0xF010;

                switch (message.Msg)
                {
                    case WM_SYSCOMMAND:
                        int command = message.WParam.ToInt32() & 0xfff0;
                        if (command == SC_MOVE)
                            return;
                        break;
                }

                base.WndProc(ref message);
            }catch(Exception ex)
            {
                MDIParentMain.Log(ex, "WndProc error");
            }
        }
    }
}
