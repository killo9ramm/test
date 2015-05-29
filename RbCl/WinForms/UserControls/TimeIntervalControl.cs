using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.Classes.CustomTypes;
using RBClient.Classes;

namespace RBClient.WinForms.UserControls
{
    public partial class TimeIntervalControl : UserControl
    {
        public TimeIntervalControl()
        {
            InitializeComponent();

            BindEvents();
            from_mtb.TypeValidationCompleted+=_TypeValidationCompleted;
            to_mtb.TypeValidationCompleted += _TypeValidationCompleted;
        }

        private void _TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            double frm = TimeInterval.ReturnInt(from_mtb.Text);
            double to = TimeInterval.ReturnInt(to_mtb.Text);

            if (frm > to) e.Cancel = true;
            else
            {
                SetValue("From", from_mtb.Text);
                SetValue("To", to_mtb.Text);
            }
        }

        private void BindEvents()
        {
            
            //from_mtb.TextChanged += from_mtb_TextChanged;
            //to_mtb.TextChanged += to_mtb_TextChanged;

            //from_mtb.LostFocus += from_mtb_TextChanged;
            //to_mtb.LostFocus += to_mtb_TextChanged;
        }

        private void UnbindEvents()
        {
            //from_mtb.TextChanged -= from_mtb_TextChanged;
            //to_mtb.TextChanged -= to_mtb_TextChanged;
        }

        private TimeInterval _interval;
        public TimeInterval TimeInterval
        {
            get
            {
                return _interval;
            }
            set
            {
                if (_interval != null) _interval.PropertyChanged -= _interval_PropertyChanged;
                _interval = value;
                _interval.PropertyChanged+=_interval_PropertyChanged;
            }
        }

        private void _interval_PropertyChanged(object sender,PropertyChangedEventArgs e)
        {
            //UnbindEvents();
            switch (e.PropertyName)
            {
                case "From" :
                    from_mtb.Text = _interval.From;
                    hours_txtbx.Text=_interval.Hours.ToString("F1");
                    break;
                case "To":
                    to_mtb.Text = _interval.To;
                    hours_txtbx.Text=_interval.Hours.ToString("F1");
                    break;
            }
            //BindEvents();
        }

        

        //private void from_mtb_TextChanged(object sender, EventArgs e)
        //{
        //    SetValue("From", from_mtb.Text);
        //}

        //private void to_mtb_TextChanged(object sender, EventArgs e)
        //{
        //    SetValue("To", to_mtb.Text);
        //}

        private void SetValue(string pname, string pvalue)
        {
            if (TimeInterval != null)
            {
                StaticHelperClass.SetClassItemValue(TimeInterval, pname, pvalue);
            }
        }

    }
}
