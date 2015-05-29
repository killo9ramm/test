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
using System.ComponentModel;
using RBClient.Classes;

namespace RBClient.WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для SelectedListBoxItem.xaml
    /// </summary>
    public partial class SelectedListBoxItem : UserControl, INotifyPropertyChanged
    {
        
        public object InnerObject;
        public event RoutedEventHandler CheckBoxChecked;
        public PassObject TextInputEnded;

        public void OnCheckBoxChecked(RoutedEventArgs e)
        {
            if (CheckBoxChecked != null)
            {
                CheckBoxChecked(this, e);
            }
        }
        public SelectedListBoxItem()
        {
            InitializeComponent();
            checkBox1.Checked += checkBox1_Checked;
            checkBox1.Unchecked += checkBox1_Checked;
        }
        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            _isChecked = (bool)checkBox1.IsChecked;
            int i = 0;
            string st = TextBox1.Text;
            if (!_isChecked)
            {
                if (!int.TryParse(st, out i))
                TextBox1.Text = min_int.ToString();
            }
            if (_isChecked)
            {
                if (!int.TryParse(st, out i))
                    TextBox1.Text = min_int.ToString();
            }
            OnCheckBoxChecked(e);
        }

        private string _controlHeader = "";
        public string ControlHeader
        {
            get
            {
                return _controlHeader;
            }
            set
            {
                _controlHeader = value;
                checkBox1.Content = value;
                textblock1.Text = value;   
            }
        }

        private bool _isChecked = false;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                checkBox1.IsChecked = value;
            }
        }

        public int min_int = 0;
        public int border_int = 1;
        public int max_int = int.MaxValue;
        public event PropertyChangedEventHandler PropertyChanged;
        private int _hcount = 0;
        public int hcount
        {
            get
            {
                return _hcount;
            }
            set
            {
                if (value < min_int)
                {
                    value = min_int;
                }
                if (value > max_int)
                {
                    value = max_int;
                }
                _hcount = value;
                if (_hcount >= border_int)
                {
                    checkBox1.IsChecked = true;
                }
                else
                {
                    checkBox1.IsChecked = false;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("hcount"));
            }
        }

        private void TextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox1.Select(0, TextBox1.Text.Length);
        }
        private void TextBox1_MouseOver(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Focus();
        }
        
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        public void InputEnded()
        {
            if (TextInputEnded != null)
            {
                TextInputEnded(TextBox1.Text);// e.Handled = true; 
                return;
            }
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {

            if (!int.TryParse(TextBox1.Text, out _hcount))
            {
                TextBox1.Text = _hcount.ToString();
            }
            
        }
    }
}


