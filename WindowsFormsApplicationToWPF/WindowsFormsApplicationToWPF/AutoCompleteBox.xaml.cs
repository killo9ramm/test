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

namespace WindowsFormsApplicationToWPF
{
    /// <summary>
    /// Логика взаимодействия для AutoCompleteBox.xaml
    /// </summary>
    public partial class AutoCompleteBox : UserControl
    {
        public List<string> lo = new List<string>() { "1111111", "121211", "12121212", "22222111", "2221111" };
        public AutoCompleteBox()
        {
            InitializeComponent();

            comboBox1.IsTextSearchEnabled = false;
        }

        private void comboBox1_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            List<string> ls = new List<string>();
            lo.ForEach(a =>
            {
                if (a.StartsWith(comboBox1.Text))
                {
                    ls.Add(a);
                }
            });
            if (ls.Count > 0)
            {
                comboBox1.ItemsSource = ls;
                comboBox1.IsDropDownOpen = true;
            }
        }

        int i = 0;
        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Up)
            //{
            //    comboBox1.ite
            //}
        }
    }
}
