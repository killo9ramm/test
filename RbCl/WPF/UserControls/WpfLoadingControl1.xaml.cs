using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;

namespace RBClient.WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для WpfLoadingControl.xaml
    /// </summary>
    public partial class WpfLoadingControl1 : UserControl
    {
        public WpfLoadingControl1()
        {
            InitializeComponent();
        }

        public void Start(int countBack)
        {
            try
            {
                CountBackValue = countBack;
                if (timer != null)
                    timer.Start();
            }catch(Exception ex)
            {
            }
        }
        public void Stop()
        {
            UpdateUI(0);
            timer.Stop();
            timer.Dispose();
        }

        private int _initialCount;
        private int _CountBackValue;
        private int CountBackValue
        {
            set
            {
                try
                {
                    //перезапускает таймер
                    if (timer != null) TimerDispose();

                    _CountBackValue = value;
                    _initialCount = value;
                    if (value != 0)
                    {
                        ResetPgBar(_CountBackValue);
                        timer = CreateNewTimer(1000, _CountBackValue, TimerAction);
                    }
                    else
                    {
                        ResetPgBar(0);
                    }
                }catch(Exception ex)
                {
                }
            }
        }

        private void ResetPgBar(int _CountBackValue)
        {
            pgbar.Value = 0;
            pgbar.Maximum = _CountBackValue;
            pgbar.Minimum = 0;
        }

        private System.Timers.Timer timer=null;
        
        private void TimerDispose()
        {
            if (timer != null)
            {
                Stop();
            }
        }

        private void TimerAction(object sender,ElapsedEventArgs e)
        {
            if (_CountBackValue == 0) { Stop(); return; }
            _CountBackValue -= 1;
            if (_CountBackValue == 0) { Stop(); return; }

            UpdateUI(_CountBackValue);
        }

        private void UpdateUI(int _CountBackValue)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle,new Action(delegate()
            {
                pgbar.Value = pgbar.Maximum - _CountBackValue;
                txtbl_timer.Text = SetTimeFromInt(_CountBackValue);
            }));
        }

        private string SetTimeFromInt(int val)
        {
            if (val < 0) return null;
            string time = "";
            if(val>0) time=String.Format("{0:00}",(val%60));
            if (val / 60 > 0)
            {
                time = String.Format("{0:00}", ((val / 60) % 60))+":"+time;
            }
            if (val / 360 > 0)
            {
                time = String.Format("{0:00}", (val / 360)) + ":" + time;
            }
            return time;
        }

        private Timer CreateNewTimer(int interval, int count_back, ElapsedEventHandler tm_action)
        {
            Timer tm = new Timer(interval);
            tm.Elapsed += tm_action;

            return tm;
        }

        public string Text
        {
            get
            {
                return header_lbl.Content.ToString();
            }
            set
            {
                header_lbl.Content = value;
            }
        }
    }
}
