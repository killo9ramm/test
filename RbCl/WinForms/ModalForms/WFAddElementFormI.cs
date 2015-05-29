using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.Classes;
using RBClient.WinForms.UserControls;

namespace RBClient.WinForms
{
    public partial class WFAddElementFormI : Form
    {
        public WFAddElementFormI()
        {
            InitializeComponent();

            Load+=Form_Load;

            iteremControl1.TBox.KeyDown += _KeyDownEvent;
            iteremControl1.LBox.KeyDown += _KeyDownEvent;

            iteremControl1.LBox.MouseDoubleClick += listBox1_MouseDoubleClick;

            iteremControl1.LBox.Font = new Font("TimesNewRoman", 11);
            iteremControl1.TBox.Font = iteremControl1.LBox.Font;
        }

        public Font Font
        {
            get
            {
                return iteremControl1.LBox.Font;
            }
            set
            {
                iteremControl1.LBox.Font = value;
            }
        }

        private void _KeyDownEvent(object s,KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(s, e);
            }
            if (e.KeyCode == Keys.Escape)
            {
                button2_Click(s, e);
            }
            if (e.KeyCode == Keys.Down && s is TextBox)
            {
                if (iteremControl1.LBox.SelectedIndex < iteremControl1.LBox.Items.Count - 1)
                {
                    iteremControl1.LBox.SelectedIndex = iteremControl1.LBox.SelectedIndex + 1;
                    e.Handled = true;
                }
            }
            if (e.KeyCode == Keys.Up && s is TextBox)
            {
                if (iteremControl1.LBox.SelectedIndex > 1)
                {
                    iteremControl1.LBox.SelectedIndex = iteremControl1.LBox.SelectedIndex-1;
                    e.Handled = true;
                }
            }
        }

        private void listBox1_MouseDoubleClick(object s,MouseEventArgs e)
        {
            button1_Click(s,e);
        }

        private void Form_Load(object sender,EventArgs e)
        {
            ActiveControl = iteremControl1.TBox;
        }
        
        public object ReturnedObject;
        public PassObject ReturnObject;

        public ListBox mainList
        {
            get
            {
                return iteremControl1.LBox;
            }
        }

        public IteremControl mainControl
        {
            get
            {
                return iteremControl1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ReturnObject != null)
            {
                if (iteremControl1.LBox.SelectedItem != null)
                {
                    ReturnedObject = iteremControl1.LBox.SelectedItem;
                    ReturnObject(ReturnedObject);
                }
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        public DialogResult ShowDialoG()
        {
            iteremControl1.TBox.Text = "";
            return base.ShowDialog();
        }
    }
}
