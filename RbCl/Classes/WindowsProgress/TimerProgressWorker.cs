using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace RBClient.Classes.WindowsProgress
{
    class TimerProgressWorker : ProgressWorker
    {
        public TimerProgressWorker(string Name, string _Header,Bitmap image) : base(Name,_Header,image)
        {
            timer = DefaultTimer();
        }

        public TimerProgressWorker(string Name, string _Header)
            : base(Name, _Header)
        {
            timer = DefaultTimer();
        }

         public TimerProgressWorker(Form MdiParent, string Name, string _Header)
            : base(MdiParent,Name,_Header)
        {
            timer = DefaultTimer();
        }

         public void Start()
         {
             ПриращениеСтатуса = 100 / ОжидаемоеВремяОткрытия;

             timer.Start();
             base.Start();
         }

         System.Timers.Timer DefaultTimer()
         {
             System.Timers.Timer tm = new System.Timers.Timer();
             tm.Interval=1000;
             tm.Elapsed+=timer_Tick;
             return tm;
         }
        
        public System.Timers.Timer timer;
        public int ОжидаемоеВремяОткрытия = 10;
        private int ПриращениеСтатуса = 0;

         public void timer_Tick(object s,EventArgs e)
         {
             //Debug.WriteLine(progrWindow.Status + ПриращениеСтатуса);
             ReportProgress(progrWindow.Status+ПриращениеСтатуса);
         }

         public void Stop()
         {
             timer.Stop();
             timer.Dispose();
             base.Stop();
         }
    }
}
