using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace svh_host
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Form1 form = new Form1();
            form.ShowDialog();

            //string text=File.ReadAllText(@"G:\RRepo\myhostproject\RBClient\CParam.cs");
            //var typer=Typer.NewTyper(text);

            //typer.Type();

        }
    }
}
