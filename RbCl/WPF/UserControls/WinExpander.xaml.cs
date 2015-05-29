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
using RBClient.Classes;

namespace RBClient.WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для WinExpander.xaml
    /// </summary>
    public partial class WinExpander : UserControl
    {
        public WinExpander()
        {
            InitializeComponent();
        }

        public PassObject Maximize_Click;
        public PassObject Minimize_Click;

        private void Button_Click_Min(object sender, RoutedEventArgs e)
        {
            if (Minimize_Click != null)
            {
                Minimize_Click(this);
            }
        }

        private void Button_Click_Max(object sender, RoutedEventArgs e)
        {
            if (Maximize_Click != null)
            {
                Maximize_Click(this);
            }
        }

        public static readonly DependencyProperty ExpanderContentProperty =
           DependencyProperty.RegisterAttached(
           "ExpanderContent",
           typeof(object),
           typeof(WinExpander));

        public object ExpanderContent
        {
            get
            {
                return Expander.Content;
            }
            set
            {
                Expander.Content = value;
            }
        }
    }
}
