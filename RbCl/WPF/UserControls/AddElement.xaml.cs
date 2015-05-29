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
using RBClient.Classes.DocumentClasses;
using System.Windows.Controls.Primitives;


namespace RBClient.WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AddElement.xaml
    /// </summary>
    public partial class AddElement : UserControl
    {
        public object ReturnedObject;
        public PassObject ReturnObject;
        
        public System.Windows.Forms.Form Parent;

        public AddElement()
        {
            InitializeComponent();
            Loaded+=new RoutedEventHandler(AddElement_Loaded);
            btn_add.Click+=new RoutedEventHandler(btn_add_Click);
            btn_rem.Click+=new RoutedEventHandler(btn_rem_Click);
            
            listW_MainList.SelectionChanged+=new SelectionChangedEventHandler(listW_MainList_SelectionChanged);
            txb_Search.TextChanged+=new TextChangedEventHandler(txb_Search_TextChanged);

            listW_MainList.MouseDoubleClick += (s1, e1) =>
            {
                if (listW_MainList.SelectedItem != null)
                {
                    //проверяем не на скролле ли щелкнул пользователь
                    DependencyObject src = (DependencyObject)(e1.OriginalSource);
                    ScrollBar scbar = WpfHelperClass.FindVisualParent<ScrollBar>(src);

                    if (scbar == null)
                    {
                        btn_add_Click(s1, e1);
                    }
                }
            };
        }


        public void txb_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txb_Search.Text != "")
            {
                ListViewItem i=null;
                foreach (ListViewItem j in listW_MainList.Items.OfType<ListViewItem>())
                {
                    if (j.Content.ToString().IndexOf(txb_Search.Text,StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        i = j;
                        break;
                    }
                }


                if (i != null)
                {
                    listW_MainList.SelectedItem = i;
                    listW_MainList.ScrollIntoView(i);
                }

                ModelItemClass mi = null;
                foreach (ModelItemClass j in listW_MainList.Items.OfType<ModelItemClass>())
                {
                    if (j.ToString().IndexOf(txb_Search.Text, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        mi = j;
                        break;
                    }
                }


                if (mi != null)
                {
                    listW_MainList.SelectedItem = mi;
                    listW_MainList.ScrollIntoView(mi);
                }
            }
        }
        
        private void listW_MainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listW_MainList.SelectedItem != null)
            {
                if (ReturnObject != null) ReturnObject(listW_MainList.SelectedItem);
            }
        }

        private void AddElement_Loaded(object sender,RoutedEventArgs e)
        {
            txb_Search.Focus();
        }

        public void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (listW_MainList != null && listW_MainList.SelectedItem != null)
            {
                if (ReturnObject != null)
                {    
                    ReturnObject(listW_MainList.SelectedItem);
                }
                ReturnedObject = listW_MainList.SelectedItem;
            }
            
            Parent.DialogResult = System.Windows.Forms.DialogResult.OK;
            Parent.Close();
        }
        private void btn_rem_Click(object sender, RoutedEventArgs e)
        {
            Parent.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Parent.Close();
        }

        private void txb_Search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (listW_MainList.SelectedIndex > 0)
                {
                    listW_MainList.SelectedIndex = listW_MainList.SelectedIndex - 1;
                    listW_MainList.ScrollIntoView(listW_MainList.SelectedItem);
                }
            }
            if (e.Key == Key.Down)
            {
                if (listW_MainList.SelectedIndex < listW_MainList.Items.Count - 1)
                {
                    listW_MainList.SelectedIndex = listW_MainList.SelectedIndex + 1;
                    listW_MainList.ScrollIntoView(listW_MainList.SelectedItem);
                }
            }
        }

        private void list_mouse_down(object sender, MouseButtonEventArgs e)
        {

          //ListViewItem lwi=WpfHelperClass.FindParent<ListViewItem>((Control)e.OriginalSource);
          ListViewItem lwi=WpfHelperClass.FindVisualParent<ListViewItem>((DependencyObject)e.OriginalSource);
          if (lwi != null)
          {
              listW_MainList.SelectedItem = lwi.Content;
          }
        }
    }
}
