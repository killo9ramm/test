using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace RBClient.WinForms.UserControls
{
    public partial class IteremControl : UserControl
    {
        public IteremControl()
        {
            InitializeComponent();
            textBox.TextChanged += new EventHandler(textBox_TextChanged);
        }
        public IteremControl(IEnumerable _list) 
        {
            InitializeComponent();
            textBox.TextChanged+=new EventHandler(textBox_TextChanged);
            LBox.DataSource = _list;
            InitialList = _list;
        }

        public object DataSource
        {
            get
            {
                return listBox.DataSource;
            }
            set
            {
                LBox.DataSource = value;
                InitialList = (IEnumerable)value;
            }
        }

        public TextBox TBox
        {
            get{
                return textBox;
            }
        }
        public ListBox LBox
        {
            get
            {
                return listBox;
            }
        }
        public IEnumerable InitialList;


        private string old_text="";
        private object syncObject = new object();
        public int RenewSpeed = 500;

        private void textBox_TextChanged(object sender,EventArgs e)
        {
            string new_text=textBox.Text;
            if (new_text.Length== old_text.Length+1 && new_text.StartsWith(old_text))
            {
                lock (syncObject)
                {
                    List<object> o = listBox.Items.OfType<object>().ToList();
                    listBox.DataSource = o.Where(a => a.ToString().IndexOf(textBox.Text, StringComparison.OrdinalIgnoreCase) != -1).ToList();
                    old_text = textBox.Text;
                }
            }
            else
            {
                ThreadPool.QueueUserWorkItem((oo) =>
                 {
                    string n_text = new_text;
                    Thread.Sleep(RenewSpeed);
                    if (n_text == textBox.Text)
                    {
                        lock (syncObject)
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new MethodInvoker(() =>
                                    {
                                        List<object> o = InitialList.OfType<object>().ToList();
                                        listBox.DataSource = o.Where(a => a.ToString().IndexOf(n_text, StringComparison.OrdinalIgnoreCase) != -1).ToList();
                                        old_text = textBox.Text;
                                    }));
                            }
                            else
                            {
                                List<object> o = InitialList.OfType<object>().ToList();
                                listBox.DataSource = o.Where(a => a.ToString().IndexOf(n_text, StringComparison.OrdinalIgnoreCase) != -1).ToList();
                                old_text = textBox.Text;
                            }
                        }
                    }
                });
            }
        }
    }
}
