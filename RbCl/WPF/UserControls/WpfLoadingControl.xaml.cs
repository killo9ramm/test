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

namespace RBClient.WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для WpfLoadingControl.xaml
    /// </summary>
    public partial class WpfLoadingControl : UserControl
    {
        public WpfLoadingControl()
        {
            InitializeComponent();
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
