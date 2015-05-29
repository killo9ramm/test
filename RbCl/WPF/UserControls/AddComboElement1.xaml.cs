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


namespace RBClient.WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AddElement.xaml
    /// </summary>
    public partial class AddComboElement1 : UserControl
    {
        public object ReturnedObject;
        public PassObject ReturnObject;
        public System.Windows.Forms.Form Parent;

        List<string> combo_items_source; //варианты комбобокса
        

        public AddComboElement1()
        {
            InitializeComponent();
            Unloaded+=new RoutedEventHandler(AddComboElement1_Unloaded);
        }

        private void AddComboElement1_Unloaded(object sender, RoutedEventArgs e)
        {
            if (comboBox1.pop.IsOpen)
            {
                comboBox1.pop.IsOpen = false;
            }
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
    }
}
