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
    public partial class AddElement2win2 : UserControl
    {
        public object ReturnedObject;
        public PassObject ReturnObject;
        public System.Windows.Forms.Form Parent;

        List<string> combo_items_source; //варианты комбобокса


        public AddElement2win2()
        {
            InitializeComponent();
            Unloaded+=new RoutedEventHandler(AddComboElement1_Unloaded);
            listW_MainList.KeyDown+=new KeyEventHandler(listW_MainList_KeyDown);
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
            iteremWpfControl1.TBox.Text = "";
        }
        private void btn_rem_Click(object sender, RoutedEventArgs e)
        {
            Parent.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Parent.Close();
        }
    }
}
