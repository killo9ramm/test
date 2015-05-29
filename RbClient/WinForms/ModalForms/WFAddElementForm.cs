using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.Classes;

namespace RBClient.WinForms
{
    public partial class WFAddElementForm : Form
    {
        public WFAddElementForm()
        {
            InitializeComponent();

            Load+=Form_Load;
            
            textBox1.KeyDown += _KeyDownEvent;
            listBox1.KeyDown += _KeyDownEvent;

            listBox1.MouseDoubleClick+= listBox1_MouseDoubleClick;

            listBox1.Font = new Font("TimesNewRoman", 11);
        }

        public Font Font
        {
            get
            {
                return listBox1.Font;
            }
            set
            {
                listBox1.Font = value;
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
                if(listBox1.SelectedIndex<listBox1.Items.Count-1)
                {
                    listBox1.SelectedIndex++;
                    e.Handled = true;
                }
            }
            if (e.KeyCode == Keys.Up && s is TextBox)
            {
                if (listBox1.SelectedIndex > 1)
                {
                    listBox1.SelectedIndex--;
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
            ActiveControl = textBox1;
        }
        
        public object ReturnedObject;
        public PassObject ReturnObject;

        public ListBox mainList
        {
            get
            {
                return listBox1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ReturnObject != null)
            {
                if (listBox1.SelectedItem != null)
                {
                    ReturnedObject = listBox1.SelectedItem;
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
            textBox1.Text = "";
            return base.ShowDialog();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                object i=null;
                foreach (object j in listBox1.Items)
                {
                    if (j.ToString().IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        i = j;
                        break;
                    }
                }


                if (i != null)
                {
                    listBox1.SelectedItem = i;
                    //listBox1.scroScrollIntoView(i);
                }
            }
        }

        
    }
}
