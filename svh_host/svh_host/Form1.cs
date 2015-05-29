using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace svh_host
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Load+=Form1_Load;
        }
        Typer typer;
        Stopwatch watch = new Stopwatch();
        System.Threading.Timer timer;

        private void Form1_Load(object sender,EventArgs e)
        {
            timer = new System.Threading.Timer((o) =>
            {
                typer_CurrIndexRaise(typer, null);
            });

            //Thread.Sleep(3000);




            //string text = File.ReadAllText(@"G:\RRepo\myhostproject\posdisplay2\POSDisplay2-h\POSMonitor\POSMonitor\bin\Debug\Logger.txt");
            //typer = Typer.NewTyper(text);
            //watch.Start();
            //timer.Change(Timeout.Infinite, 1000);
            //typer.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread.Sleep(3000);
            if (typer != null)
            {
                if (typer.current_sym_int != 0)
                {
                    typer.Resume();
                    watch.Start();
                }
                else
                {
                    watch.Reset();
                    watch.Start();
                    typer.Start();
                }
                timer.Change(0, 1000);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (typer != null)
            {
                watch.Stop();
                typer.Pause();
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void typer_CurrIndexRaise(object sender, EventArgs e)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke((MethodInvoker)delegate() { 
                    textBox1.Text = ((Typer)sender).current_sym_int.ToString();
                    label2.Text = watch.Elapsed.ToString();
                });
            }
            else
            {
                textBox1.Text = ((Typer)sender).current_sym_int.ToString();
                label2.Text = watch.Elapsed.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog opfd = new OpenFileDialog() { InitialDirectory=AppDomain.CurrentDomain.BaseDirectory };
            if (opfd.ShowDialog() == DialogResult.OK)
            {
                button3.Text = opfd.FileName;

                //typer = Typer.NewTyper(File.ReadAllText(opfd.FileName,Encoding.UTF8));
                typer = Typer.NewTyper(File.ReadAllText(opfd.FileName, Encoding.GetEncoding(1251)));
                typer = Typer.NewTyper(File.ReadAllText(opfd.FileName, Encoding.GetEncoding(1251)));
            }
        }
    }
}
