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
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace RBClient.WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для WpfSmenaAdd.xaml
    /// </summary>
    public partial class WpfSmenaAdd : UserControl,INotifyPropertyChanged
    {
        public object ReturnedObject;
        public PassObject ReturnObject;
        public System.Windows.Forms.Form Parent;

        public Regex mask = new Regex(@"\D+");

        public int min_int = 0;
        public int max_int = 24;

        private int _from=0;
        public int hours_from
        {
            get
            {
                return _from;
            }
            set
            {
                if (value < min_int)
                {
                    value = min_int;
                }
                if (value > max_int)
                {
                    value = max_int;
                }
                _from = value;
                PropertyChanged(this, new PropertyChangedEventArgs("hours_from"));
                
                recount();
                
            }
        }

        private int _to = 0;
        public int to_hours
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
                PropertyChanged(this, new PropertyChangedEventArgs("to_hours"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int _hcount=0;
        public int hcount
        {
            get
            {
                return _hcount;
            }
            set
            {
                if (value < min_int)
                {
                    value = min_int;
                }
                if (value > max_int)
                {
                    value = max_int;
                }
                _hcount = value;
                PropertyChanged(this, new PropertyChangedEventArgs("hcount"));
                recount();
            }
        }

        public WpfSmenaAdd()
        {
            try
            {
                InitializeComponent();

                //кнопка Добавить клик
                btn_add.Click += new RoutedEventHandler(btn_add_Click);
                //кнопка  Отмена клик
                btn_rem.Click += new RoutedEventHandler(btn_rem_Click);

                btn_hours_add.Click += new RoutedEventHandler(btn_hours_add_Click);
                btn_hours_rem.Click += new RoutedEventHandler(btn_hours_rem_Click);

                //Запретить редактирование текст боксов
                txt_to_hours.IsReadOnly = true;

                //поставить события на изменение текстбокса

                txt_from_hours.PreviewTextInput += new TextCompositionEventHandler(txt_from_hours_PreviewTextInput);
                txt_from_hours.TextChanged += new TextChangedEventHandler(txt_from_hours_TextChanged);
                
                txtbx_hours.IsReadOnly = true;

            }catch(Exception ex)
            {
            }
        }

        void txt_from_hours_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (hours_from >= max_int)
            {
                txt_from_hours.Text = hours_from.ToString();
            }
        }

        void txt_from_hours_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string str = e.Text;
            
            if (mask.IsMatch(str))
            {
                e.Handled = true;
            }
        }  

        private void recount()
        {
            if (hours_from + hcount > max_int)
            {
                hcount = max_int - hours_from;
            }
            else
            {
                to_hours = hours_from + hcount;
            }
        }

        /// <summary>
        /// +
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_hours_rem_Click(object sender, RoutedEventArgs e)
        {
            hcount--; 
        }

        /// <summary>
        /// +
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_hours_add_Click(object sender, RoutedEventArgs e)
        {
            hcount++;
        }



        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            //создать событие передачи объекта

            if (combo_list.SelectedItem != null)
            {
                if (ReturnObject != null)
                {
                    object o = combo_list.SelectedItem;
                    Type t =combo_list.SelectedItem.GetType();
                    StaticHelperClass.SetClassItemValue(t, o, "hours_from", hours_from);
                    StaticHelperClass.SetClassItemValue(t, o, "hcount", hcount);
                    StaticHelperClass.SetClassItemValue(t, o, "to_hours", to_hours);
                    ReturnObject(o);
                }
            }

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
