using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RBClient.Classes.CustomTypes
{
    public class TimeInterval : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _from;
        public string From
        {
            get { return _from; }
            set
            {
                _from = value;
                if (_from.Length == 4) _from = "0" + _from;
                RecountHours();
                OnPropertyChanged("From");
            }
        }

        private string _to;
        public string To { get
        {
            return _to;
        }
            set
            {
                _to = value;
                if (_to.Length == 4) _to = "0" + _to;
                RecountHours();
                OnPropertyChanged("To");
            }
        }

        private double hours;
        public double Hours
        {
            get
            {
                return hours;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }


        public double ReturnInt(string time)
        {
            try
            {
                string[] times = time.Split(':');
                double t1 = double.Parse(times[0]);
                double t2 = double.Parse(times[1]) / 60;
                return t1 + t2;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        private void RecountHours()
        {
            double frm = ReturnInt(From);
            double to = ReturnInt(To);

            if (frm != 0 || to != 0)
                hours = to - frm;
        }
    }
}
