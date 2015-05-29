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
using RBClient.Classes.WindowAddElement;
using System.Windows.Threading;


namespace RBClient.WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AddElement.xaml
    /// </summary>
    public partial class AddElement2win : UserControl
    {
        public object ReturnedObject;
        public PassObject ReturnObject;
        public System.Windows.Forms.Form Parent;

        List<string> combo_items_source; //варианты комбобокса


        public AddElement2win()
        {
            InitializeComponent();
            Unloaded+=new RoutedEventHandler(AddComboElement1_Unloaded);
            text_input.PreviewKeyDown+=new KeyEventHandler(text_input_KeyDown);
            listW_MainList.KeyDown+=new KeyEventHandler(listW_MainList_KeyDown);
        }

        private void text_input_KeyDown(object sender,KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (listBox_positions.SelectedIndex > 0)
                {
                    listBox_positions.SelectedIndex--;
                    listBox_positions.ScrollIntoView(listBox_positions.SelectedItem);
                }
            }

            if (e.Key == Key.Down)
            {
                if (listBox_positions.SelectedIndex < listBox_positions.Items.Count - 1)
                {
                    listBox_positions.SelectedIndex++;
                    listBox_positions.ScrollIntoView(listBox_positions.SelectedItem);
                }
            }
            if (e.Key == Key.Enter)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                {
                    btn_add_position.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }));
            }
        }

        private void listW_MainList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                {
                    btn_add.Focus();
                }));
            }
        }

        private void AddComboElement1_Unloaded(object sender, RoutedEventArgs e)
        {
         
        }

        private void AddElement_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (ReturnObject != null) ReturnObject(listW_MainList.Items);
            Parent.DialogResult = System.Windows.Forms.DialogResult.OK;
            Parent.Close();
        }
        private void btn_rem_Click(object sender, RoutedEventArgs e)
        {
            Parent.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Parent.Close();
        }

        private void text_input_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (text_input.Text != "")
            {
                object i = null;
                foreach (object j in listBox_positions.Items)
                {
                    if (j.ToString().IndexOf(text_input.Text, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        i = j;
                        break;
                    }
                }


                if (i != null)
                {
                    listBox_positions.SelectedItem = i;
                    listBox_positions.ScrollIntoView(i);
                }
            }
        }
    }
}
