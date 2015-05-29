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
using System.Collections;
using System.Threading;
using System.Windows.Threading;

namespace RBClient.WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для IteremWpfControl.xaml
    /// </summary>
    public partial class IteremWpfControl : UserControl
    {
        public IteremWpfControl()
        {
            InitializeComponent();

            textBox.PreviewKeyDown+=new KeyEventHandler(textBox_PreviewKeyDown);
        }


        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (listBox.ItemsSource == null) return;
            if (e.Key == Key.Up)
            {
                if (listBox.SelectedIndex > 0)
                {
                    listBox.SelectedIndex = listBox.SelectedIndex - 1;
                    listBox.ScrollIntoView(listBox.SelectedItem);
                }
            }
            if (e.Key == Key.Down)
            {
                if (listBox.SelectedIndex < listBox.Items.Count - 1)
                {
                    listBox.SelectedIndex = listBox.SelectedIndex + 1;
                    listBox.ScrollIntoView(listBox.SelectedItem);
                }
            }
        }

        public IEnumerable ItemsSource
        {
            set
            {
                LBox.ItemsSource = value;
                InitialList = value;
            }
        }

        public TextBox TBox
        {
            get
            {
                return textBox;
            }
        }
        public ListBox LBox
        {
            get
            {
                return listBox;
            }
        }
        public IEnumerable InitialList;

        private string old_text = "";
        private object syncObject = new object();
        public int RenewSpeed = 500;

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (InitialList == null) return;
            string new_text = textBox.Text;
            if (new_text.Length == old_text.Length + 1 && new_text.StartsWith(old_text))
            {
                lock (syncObject)
                {
                    List<object> o = listBox.Items.OfType<object>().ToList();
                    listBox.ItemsSource = o.Where(a => a.ToString().IndexOf(textBox.Text, StringComparison.OrdinalIgnoreCase) != -1).ToList();
                    old_text = textBox.Text;
                }
            }
            else
            {
                ThreadPool.QueueUserWorkItem((oo) =>
                {
                    string n_text = new_text;
                    Thread.Sleep(RenewSpeed);
                    Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                            {
                                if (n_text == textBox.Text)
                                {
                                    lock (syncObject)
                                    {
                                        List<object> o = InitialList.OfType<object>().ToList();
                                        listBox.ItemsSource = o.Where(a => a.ToString().IndexOf(n_text, StringComparison.OrdinalIgnoreCase) != -1).ToList();
                                        old_text = textBox.Text;
                                    }
                                }
                            }));
                });
            }
        }
    }
}
