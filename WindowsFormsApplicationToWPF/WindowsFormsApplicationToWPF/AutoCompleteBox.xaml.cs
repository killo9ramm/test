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
    public delegate void PassObject(object o);
    /// <summary>
    /// Логика взаимодействия для AutoCompleteBox.xaml
    /// </summary>
    public partial class AutoCompleteBox : UserControl
    {
        public List<string> ItemsList = new List<string>() { "1111111", "121211", "12121212", "22222111", "2221111" };
        public string SelectedItem = null;

        public PassObject SelectionChanged;

        public AutoCompleteBox()
        {
            InitializeComponent();
            comboBox1.IsTextSearchEnabled = false;
        }

       
        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.RightShift && e.Key != Key.Home && e.Key != Key.LeftShift && e.Key != Key.End)
            {
                i = -1;
                List<string> ls = new List<string>();
                string st = comboBox1.Text;
                ItemsList.ForEach(a =>
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
                    comboBox1.Text = st;

                    CursorToEnd();
                    
                }
            }
            if (e.Key == Key.Up && i > 0)
            {
                i--;
                comboBox1.SelectedIndex = i;
            }
            if (e.Key == Key.Down && i+1 < comboBox1.Items.Count)
            {
                i++;
                comboBox1.SelectedIndex = i;
                
            }
            if (e.Key == Key.Enter)
            {
                comboBox1.IsDropDownOpen = false;
                List<string> vals=comboBox1.ItemsSource as List<string>;
                if (vals.Contains(comboBox1.Text))
                {
                    if (SelectionChanged != null)
                    {
                        SelectionChanged(comboBox1.SelectedItem);
                    }
                }
                else
                {
                    if (vals.Count > 0)
                    {
                        comboBox1.Text = vals[0];
                        CursorToEnd();
                    }
                }
            }
            //if (e.Key == Key.Tab)
            //{
            //    List<string> vals = comboBox1.ItemsSource as List<string>;
            //    if (vals.Count > 0)
            //    {
            //        comboBox1.Text = vals[0];
            //        CursorToEnd();
            //        e.Handled = true;
            //    }
            //}
        }

        int i = -1;

        private void CursorToEnd()
        {
            var myTextBox = comboBox1.Template.FindName("PART_EditableTextBox", comboBox1) as TextBox;
            myTextBox.CaretIndex = comboBox1.Text.Length;
        }

        private void comboBox1_MouseDown(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                if (SelectionChanged != null)
                {
                    SelectionChanged(comboBox1.SelectedItem);
                }
            }
        }


    }
}
