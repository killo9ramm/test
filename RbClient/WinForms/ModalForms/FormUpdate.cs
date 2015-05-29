using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using RBClient.Classes;

namespace RBClient
{
    public partial class FormUpdate : Form
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        int maxtimer_coun = StaticConstants.RBINNER_CONFIG.GetProperty<int>("update_timeout",100);
        int _tick_count = 0;
        public FormUpdate()
        {
            InitializeComponent();

            Load += (s, e) =>
            {
                Count_Down();
                timer.Interval = 1000;
                timer.SynchronizingObject = this;
                timer.Elapsed += (ss, ee) =>
                {
                    Count_Down();
                };
                timer.Start();
            };

            FormClosing+=(s,e)=>
            {
                e.Cancel = true;
            };

            FormClosed += (s, e) =>
            {
                timer.Dispose();
            };
        }
        

        

        private void ribbonControl1_Click(object sender, EventArgs e)
        {

        }

        const string cd_text = "Программа будет обновлена через {0} сек.";
        private void Count_Down()
        {
            int cd=maxtimer_coun-_tick_count;
            labelX2.Text = String.Format(cd_text,cd);
            if (cd == 0)
            {
                buttonX1_Click(this, null);
            }
            _tick_count++;
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            StaticConstants.MainWindow.RbClientUpdate();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            _tick_count = 0;
            Count_Down();
            //this.Close();
        }

        
    }
}
