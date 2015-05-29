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
    public partial class AddElementI : UserControl
    {
        public object ReturnedObject;
        public PassObject ReturnObject;
        
        public System.Windows.Forms.Form Parent;

        public AddElementI()
        {
            InitializeComponent();
            Loaded+=new RoutedEventHandler(AddElement_Loaded);
            btn_add.Click+=new RoutedEventHandler(btn_add_Click);
            btn_rem.Click+=new RoutedEventHandler(btn_rem_Click);
            
            
            mainListControl.LBox.SelectionChanged+=new SelectionChangedEventHandler(listW_MainList_SelectionChanged);
            mainListControl.TBox.TextChanged += new TextChangedEventHandler(txb_Search_TextChanged);

            mainListControl.LBox.MouseDoubleClick += (s1, e1) =>
            {
                if (mainListControl.LBox.SelectedItem != null)
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


        public Selector listW_MainList
        {
            get
            {
                return mainListControl.LBox;
            }
        }

        public TextBox txb_Search
        {
            get
            {
                return mainListControl.TBox;
            }
        }

        public void txb_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mainListControl.TBox.Text != "")
            {
                ListViewItem i=null;
                foreach (ListViewItem j in mainListControl.LBox.Items.OfType<ListViewItem>())
                {
                    if (j.Content.ToString().IndexOf(mainListControl.TBox.Text, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        i = j;
                        break;
                    }
                }


                if (i != null)
                {
                    mainListControl.LBox.SelectedItem = i;
                    mainListControl.LBox.ScrollIntoView(i);
                }

                ModelItemClass mi = null;
                foreach (ModelItemClass j in mainListControl.LBox.Items.OfType<ModelItemClass>())
                {
                    if (j.ToString().IndexOf(mainListControl.TBox.Text, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        mi = j;
                        break;
                    }
                }


                if (mi != null)
                {
                    mainListControl.LBox.SelectedItem = mi;
                    mainListControl.LBox.ScrollIntoView(mi);
                }
            }
        }
        
        private void listW_MainList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainListControl.LBox.SelectedItem != null)
            {
                if (ReturnObject != null) ReturnObject(mainListControl.LBox.SelectedItem);
            }
        }

        private void AddElement_Loaded(object sender,RoutedEventArgs e)
        {
            mainListControl.TBox.Focus();
        }

        public void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (mainListControl.LBox != null && mainListControl.LBox.SelectedItem != null)
            {
                if (ReturnObject != null)
                {
                    ReturnObject(mainListControl.LBox.SelectedItem);
                }
                ReturnedObject = mainListControl.LBox.SelectedItem;
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
                if (mainListControl.LBox.SelectedIndex > 0)
                {
                    mainListControl.LBox.SelectedIndex = mainListControl.LBox.SelectedIndex - 1;
                    mainListControl.LBox.ScrollIntoView(mainListControl.LBox.SelectedItem);
                }
            }
            if (e.Key == Key.Down)
            {
                if (mainListControl.LBox.SelectedIndex < mainListControl.LBox.Items.Count - 1)
                {
                    mainListControl.LBox.SelectedIndex = mainListControl.LBox.SelectedIndex + 1;
                    mainListControl.LBox.ScrollIntoView(mainListControl.LBox.SelectedItem);
                }
            }
        }

        private void list_mouse_down(object sender, MouseButtonEventArgs e)
        {

          //ListViewItem lwi=WpfHelperClass.FindParent<ListViewItem>((Control)e.OriginalSource);
          ListViewItem lwi=WpfHelperClass.FindVisualParent<ListViewItem>((DependencyObject)e.OriginalSource);
          if (lwi != null)
          {
              mainListControl.LBox.SelectedItem = lwi.Content;
          }
        }
    }
}
