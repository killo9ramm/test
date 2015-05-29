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
            comboBox1.GotFocus += comboBox1_GotFocus;
        }

        private void comboBox1_GotFocus(object sender,RoutedEventArgs e)
        {
            e.Handled = true;
           // comboBox1.SelectedItem = null;
            //if(comboBox1.ItemsSource!=null)comboBox1.ItemsSource.OfType<ComboBoxItem>().ToList().ForEach(a=>a.IsEnabled=false);
            //comboBox1.ItemsSource = null;
            //comboBox1.se
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (/*e.Key!=Key.Back &&*/e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.RightShift && e.Key != Key.Home && e.Key != Key.LeftShift && e.Key != Key.End)
            {
                i = -1;
                List<string> ls = new List<string>();
                string new_text = comboBox1.Text;// +e.ToString();
                string st = comboBox1.Text;
                ItemsList.ForEach(a =>
                {
                    if (a.StartsWith(new_text,StringComparison.OrdinalIgnoreCase))
                    {
                        ls.Add(a);
                    }
                });
                if (ls.Count > 0)
                {
                    comboBox1.ItemsSource = null;
                    comboBox1.ItemsSource = ls;
                    comboBox1.Items.Refresh();
                    comboBox1.IsDropDownOpen = true;

                    comboBox1.Text = st;

                    CursorToEnd();
                }
            }
            if (e.Key == Key.Up && (i==-1))
            {
                //i--;
                if ((comboBox1.SelectedItem ==null)) comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            }
            if (e.Key == Key.Down && i + 1 < comboBox1.Items.Count && (i==-1))
            {
                i++;
                if (!(comboBox1.SelectedIndex>0))comboBox1.SelectedIndex = i;

            }
            if (e.Key == Key.Enter)
            {
                comboBox1.IsDropDownOpen = false;
                List<string> vals = comboBox1.ItemsSource as List<string>;
                if (vals.Contains(comboBox1.Text))
                {
                    if (SelectionChanged != null)
                    {
                        if (ItemsList.Contains(comboBox1.Text))
                        {
                            SelectionChanged(comboBox1.Text);
                            comboBox1.Text = "";
                            comboBox1.SelectedItem = null;
                        }
                        else
                        {
                            SelectionChanged(comboBox1.SelectedItem);
                            comboBox1.SelectedItem = null;
                            comboBox1.Text = "";
                        }    
                    }
                }
                else
                {
                    if (vals.Count > 0)
                    {
                        comboBox1.Text = vals[0];
                        CursorToEnd();
                        SelectToEnd();
                        comboBox1.IsDropDownOpen = true;
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

        public void CursorToEnd()
        {
            var myTextBox = comboBox1.Template.FindName("PART_EditableTextBox", comboBox1) as TextBox;
            myTextBox.CaretIndex = comboBox1.Text.Length;
        }

        public void SelectToEnd()
        {
            var myTextBox = comboBox1.Template.FindName("PART_EditableTextBox", comboBox1) as TextBox;
            myTextBox.SelectAll();
        }

        //private void comboBox1_MouseDown(object sender, EventArgs e)
        //{
        //    if (comboBox1.SelectedItem != null)
        //    {
        //        if (SelectionChanged != null)
        //        {
        //            SelectionChanged(comboBox1.SelectedItem);
        //        }
        //    }
        //}

        private void comboBox1_MouseDown(object sender, RoutedEventArgs e)
        {
            if (sender != null && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (SelectionChanged != null)
                {
                    SelectionChanged((sender as ComboBoxItem).Content);
                    comboBox1.SelectedItem = null;
                    comboBox1.Text = "";
                }
            }
        }

        //private void comboBox1_LayoutUpdated(object sender, EventArgs e)
        //{
        //    if (comboBox1.Text != "")
        //    {
        //        if (comboBox1.Text == ItemsList[0]) comboBox1.Text = "";
        //    }
        //}
    }
}
