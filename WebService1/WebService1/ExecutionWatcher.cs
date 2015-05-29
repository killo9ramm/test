using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace WebService1
{
    public class ExecutionWatcher
    {
        public Stopwatch StartChecking()
        {
            timer = Stopwatch.StartNew();
            return timer;
        }

        public long Stop(Stopwatch timer)
        {
            timer.Stop();
            return timer.ElapsedMilliseconds;
        }
        Stopwatch timer;
        public void DebugTime(Stopwatch timer)
        {
            Debug.WriteLine(Stop(timer));
        }
        public void DebugTime()
        {
            DebugTime(timer);
        }
        public void DebTime(bool reset)
        {
            DebTime(timer);
            if (reset)
            {
                timer.Reset();
                timer.Start();
            }
        }

        private void DebTime(Stopwatch timer)
        {
            Debug.WriteLine(timer.ElapsedMilliseconds);
        }
    }
}