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

namespace RBClientUpdateApplication
{
    /// <summary>
    /// Логика взаимодействия для RestoranControl.xaml
    /// </summary>
    public partial class RestoranControl : UserControl
    {
        public event EventHandler RestoranChecked;
        public void OnRestoranChecked()
        {
            if(RestoranChecked!=null)
            {
                RestoranChecked(this, null);
            }
        }
        

        public RestoranControl()
        {
            InitializeComponent();
            checkBox1.Checked+=checkBox1_Checked;
            checkBox1.Unchecked += checkBox1_Checked;
        }

        private void checkBox1_Checked(object sender,RoutedEventArgs e)
        {
            _isChecked = (bool)checkBox1.IsChecked;
            OnRestoranChecked();
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
    }
}
