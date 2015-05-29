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
    public partial class AddElementText : UserControl
    {
        public object ReturnedObject;
        public PassObject ReturnObject;
        public System.Windows.Forms.Form Parent;

        public AddElementText()
        {
            InitializeComponent();
            Loaded+=new RoutedEventHandler(AddElement_Loaded);
            btn_add.Click+=new RoutedEventHandler(btn_add_Click);
            btn_rem.Click+=new RoutedEventHandler(btn_rem_Click);
        }
        
        private void AddElement_Loaded(object sender,RoutedEventArgs e)
        {
            MainText.Focus();
        }

        public void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (ReturnObject != null) ReturnObject(MainText.Text);
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
