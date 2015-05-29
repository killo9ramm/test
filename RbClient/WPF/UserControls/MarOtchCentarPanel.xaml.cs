using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Windows.Controls.Primitives;
using System.Collections;
using RBClient.Classes.InternalClasses;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes;
using RBClient.Classes.WindowAddElement;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RBClient.Classes.CustomClasses;
using System.IO;
using System.Text.RegularExpressions;
using RBClient.Classes.CustomWindows;
using System.Windows.Threading;
using RBClient.ru.teremok.msk;
using System.Net;
using System.Net.Security;
using System.Collections.Specialized;
using RBClient.Classes.WindowMarochOtch;
using System.Globalization;
using RBClient.Classes.WindowsProgress;
using RBClient.Classes.DocumentClasses;
using RBClient.WinForms;
using RBClient.Classes.OrderClasses;

namespace RBClient.WPF.UserControls
{
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double columnsCount = 0;
            double.TryParse(parameter.ToString().Replace(".", ","), out columnsCount);
            
            ListView lv = value as ListView;
            double actualWidth = lv.ActualWidth;
            GridView gv = lv.View as GridView;
            DependencyObject dp= lv.ContainerFromElement(lv);
            double width = (actualWidth / gv.Columns.Count) * (columnsCount);
            return width;
        }

        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    int columnsCount = System.Convert.ToInt32(parameter);
        //    double width = (double)value;
        //    return width / columnsCount;
        //}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Логика взаимодействия для MarOtchCentarPanel.xaml
    /// </summary>
    public partial class MarOtchCentarPanel : UserControl
    {
        //public class WidthConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        int columnsCount = System.Convert.ToInt32(parameter);
        //        double width = (double)value;
        //        return width / columnsCount;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        #region good logging

        ProgressWorker pw;
        OrderClass order;

        /// <summary>
        /// конструктор класса
        /// </summary>
        public MarOtchCentarPanel(OrderClass _order)
        {
            order = _order;
            pw = new ProgressWorker(StaticConstants.MainWindow, "11", "Подождите, идет загрузка документа \"Смена и обеды\"...");
            pw.FontSize = 10;
            pw.Start();
            pw.ReportProgress(10);

            InitializeComponent();
            bd = new CBData();
            Loaded += new RoutedEventHandler(MarOtchCentarPanel_Loaded);
           
            pw.ReportProgress(20);
        }

        /// <summary>
        /// привязываем логирование к мди
        /// </summary>
        /// <param name="text"></param>
        private void Log(string text)
        {
            MDIParentMain.Log(text);
        }


        public void FormBeforeClose(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (MarotchList.Where(a => a.response == "" || a.response == null).Count() > 0)
            {
                WpfCustomMessageBox.Show("Укажите у сотрудников обязанность!", "Внимание", MessageBoxButton.OK);
                e.Cancel = true;
                //   return;
            }

            if(VisDinnersList.Where(a=>(!a.SubDinnersItems.NotNullOrEmpty())).Count()>0)
            {
                WpfCustomMessageBox.Show("Укажите у сотрудников обеды!", "Внимание", MessageBoxButton.OK);
                e.Cancel = true;
            }
            
        }

        /// <summary>
        /// привязываем логирование к мди
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="text"></param>
        private void Log(Exception ex, string text)
        {
            MDIParentMain.Log(ex, text);
        }
        #endregion


        CBData bd; //текущий класс базы
        t_Doc CurrentDoc;   //текущий документ
        int DocType = 37; //тип документа

        ObservableCollection<t_Marotch> MarotchList = new ObservableCollection<t_Marotch>();   //коллекция для марочных отчетов
        ObservableCollection<t_Dinner> DinnersList = new ObservableCollection<t_Dinner>();     //коллекция для обедов

        ObservableCollection<v_Dinner> VisDinnersList = new ObservableCollection<v_Dinner>();     //коллекция для обедов
        ObservableCollection<v_Marotch> VisMarotchList = new ObservableCollection<v_Marotch>();    //коллекция для марочных отчетов

        WpfLoadingControl1 wpf_lc; //контрол для загрузки с кассы

        DateTime DocDate = DateTime.Now; //сегодняшняя дата
        List<t_Employee> EmployeesList; //список работников
        t_Employee CurrentUser; //текущий работник
        t_Dinner CurrentDinner; //текущий обед
        List<t_Dinner2t_Menu> CurrentDinnersDetails;// информация по обедам сотрудника
        FormAddElement add_user_form; //форма добавления сотрудников

        HostForm add_smena_form; //форма добавления смены
        WpfSmenaAdd wpf_add_smena_control;//контрол смены

        EditSmenaWindow1 wf_add_smena_control;

        List<v_Responsibility> responsibility_list;//список обязанностей

        ToggleButton cbutton = null; //текущая нажатая кнопка


        Dictionary<string, SolidColorBrush> color_table = new Dictionary<string, SolidColorBrush>(); //таблица цветов
        List<t_ShiftType> shifts = new List<t_ShiftType>(); //типы смен
        
        List<t_ButtonTemplate> sm = new List<t_ButtonTemplate>(); // шаблоны кнопок
        List<ToggleButton> smButtons = new List<ToggleButton>();

        Smena_list_item result_smena; //текущая смена
        SmenaViewClass1 result_smena1;//текущая смена

        t_WorkTeremok teremok_work_time;

        List<t_Menu> _MenuItemsSprav;//справочник меню
        List<t_Menu> MenuItemsSprav
        {
            get
            {
                return _MenuItemsSprav;
            }
            set
            {
                _MenuItemsSprav = value;
                if (_MenuItemsSprav != null && _MenuItemsSprav.Count > 0)
                {
                    MenuItemsSpravStr = new List<string>();
                    _MenuItemsSprav.ForEach(a =>
                    {
                        MenuItemsSpravStr.Add(a.menu_nome);
                    });
                    MenuItemsSpravStr=MenuItemsSpravStr.Distinct().ToList();
                }
            }
        }

        List<string> MenuItemsSpravStr;//справочник номенклатур в строках

        HostForm add_menu_form; //форма добавления меню
        AddElement2win2 wpf_add_menu_control;//контрол меню

        HostForm add_comment;
        AddElementText wpf_add_comment;
        bool commentSmenaDinner;
        public t_PropValue DinnerComment;
        public t_PropValue SmenaComment;

        #region window load event
        private void MarOtchCentarPanel_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region 1 создание интерфейса

                txtDateBlk.Text = DocDate.ToShortDateString();

                #region подгрузить кнопки
                //создать класс coloredcheckbutton
                DataTable ptr = bd.ButtonFullList();
                sm = ClassFactory.CreateClasses<t_ButtonTemplate>(ptr);

                sm.ForEach(a =>
                {
                    ToggleButton tb = new ToggleButton();
                    tb.Content = a.bnt_value;
                    tb.MinWidth = 40;
                    tb.Height = 30;
                    tb.ToolTip = a.btn_name;
                    tb.Background = color_table.CreateOrreturnElement<string, SolidColorBrush, string>(a.btn_SmenaType, a.btn_color, c => new SolidColorBrush(StaticHelperClass.ReturnColor(c)));
                    tb.IsEnabled = false;
                    tb.Checked += togglebutton_button_Click;
                    tb.Tag = a;
                    smButtons.Add(tb);
                    smena_button_panel.Children.Add(tb);

                });

                #endregion
                #endregion
                pw.ReportProgress(30);
                #region 2 проверка есть ли документ, если его нет то создать
                //получить сегодняшний документ
                List<t_Doc> docs = new t_Doc().Select<t_Doc>(String.Format("doc_datetime>=" + SqlWorker.ReturnDate(DocDate) + " AND doc_type_id=" + DocType + " AND doc_teremok_id=" + StaticConstants.Teremok_ID))
                    .Where(a => a.doc_datetime.Date == DocDate.Date).ToList();
                if (docs.Count == 0)
                {
                    t_DocStatusRef status = new t_DocStatusRef().Select<t_DocStatusRef>("doctype_id=" + DocType + " AND statustype_id=1").First();

                    t_Doc this_doc = new t_Doc()
                    {
                        doc_datetime = DocDate,
                        doc_type_id = DocType,
                        doc_guid = Guid.NewGuid().ToString(),
                        doc_teremok_id = int.Parse(StaticConstants.Teremok_ID),
                        doc_status_id = status.docstatusref_id
                    };
                    if (this_doc.Create()) CurrentDoc = this_doc;
                }
                else
                {
                    if (docs.Count > 1)
                    {
                        Log("Ошибка! Больше одного документа за текущую дату " + DocDate);
                    }
                    CurrentDoc = docs.Last();
                }
                #endregion
                pw.ReportProgress(37);
                #region 3 Получить все данные из таблиц t_Dinner и t_Marotch по данному документу и вписать их в таблицы
                shifts = new t_ShiftType().Select<t_ShiftType>("Deleted=False"); //получить все смены по документу
                if (CurrentDoc == null)
                {
                    Log("Ошибка! Невозможно найти/создать документ " + DocType);

#warning Сделать цивилизованные ворнинги
                    MessageBox.Show("Ошибка! Невозможно найти/создать документ. Обратитесь в тех.поддержку!!!");

                    return;
                }
                else
                {
                    List<t_Dinner> dinner_list = new t_Dinner().Select<t_Dinner>("doc_id=" + CurrentDoc.doc_id);
                    List<t_Marotch> t_Marotch_list = new t_Marotch().Select<t_Marotch>("doc_id=" + CurrentDoc.doc_id);

                    if (dinner_list != null && dinner_list.Count != 0)
                    {
                        dinner_list.ForEach(a => DinnersList.Add(a));
                    }
                    if (t_Marotch_list != null && t_Marotch_list.Count != 0)
                    {
                        t_Marotch_list.ForEach(a => MarotchList.Add(a));
                    }
                }
                #endregion
                pw.ReportProgress(44);
                #region 4 подгрузить пользователей и смены
                if (EmployeesList == null)
                {
                    List<t_Employee> emps = new t_Employee().Select<t_Employee>("employee_WorkField=-1 AND Deleted=False");
                    if (emps != null && emps.Count != 0)
                    {
                        EmployeesList = emps;
                    }
                }

                #endregion
                pw.ReportProgress(51);
                #region 5 подгрузить окно пользователей
                if (EmployeesList != null)
                {
                    //добавить пользователя
                    add_user_form = new FormAddElement();
                
                    add_user_form.wpfctl.txbl_Header.Text = "Выберите пользователя";

                    List<ListViewItem> lvi_list = new List<ListViewItem>();


                    EmployeesList.Sort((a, b) => a.employee_name.CompareTo(b.employee_name));


                    add_user_form.wpfctl.listW_MainList.FontSize = 13;
                    add_user_form.wpfctl.mainListControl.ItemsSource=
                    EmployeesList.Select(a => new ModelItemClass(a)).ToList();

                    add_user_form.wpfctl.ReturnObject += delegate(object o)
                    {
                        if (o == null) return;

                        CurrentUser = (t_Employee)(o as ModelItemClass).t_table;

                    };
                }
                #endregion
                pw.ReportProgress(58);
                #region 6 подгрузка окна добавления смены

                List<Smena_list_item> shifts_string = new List<Smena_list_item>();
                shifts.ForEach(a =>
                {
                    IEnumerable<string> shl = shifts_string.Select(b => b.smena_guid);
                    if (!shl.Contains(a.type_guid) && a.type_value != "")
                    {
                        shifts_string.Add(new Smena_list_item { smena_guid = a.type_guid, smena_name = a.type_name });
                    }
                });

                //заполнить add_smena   настройки окна
                wpf_add_smena_control = new WpfSmenaAdd();
                add_smena_form = new HostForm(wpf_add_smena_control);
                wpf_add_smena_control.Parent = add_smena_form;
                wpf_add_smena_control.txbl_Header.Text = "Заполните смену";
                add_smena_form.Width = 350;
                add_smena_form.Height = 295;

            //     smena_window = new EditSmenaWindow1(order.HDetailsDictionary[typeof(t_ShiftType)]);
            //smena_window.ReturnObject = (o) =>
            //{
            //    EditDay((SmenaViewClass1)o);
            //};


                wf_add_smena_control = new EditSmenaWindow1(shifts);
                wf_add_smena_control.ReturnObject += (o) =>
                {
                    result_smena1 = o as SmenaViewClass1;
                };



                // заполнить начальные значения
                wpf_add_smena_control.combo_list.ItemsSource = shifts_string;
                wpf_add_smena_control.combo_list.DisplayMemberPath = "smena_name";

                if (shifts_string != null && shifts_string.Count > 0)
                {
                    wpf_add_smena_control.combo_list.SelectedItem = shifts_string[0];
                }

                wpf_add_smena_control.hours_from = 10;



                //повесить событие на оК
                wpf_add_smena_control.ReturnObject += delegate(object o)
                {
                    result_smena = o as Smena_list_item;
                };

                #endregion
                pw.ReportProgress(65);
                #region 7 сделаем дата байнд и прицепим события list_view смены
                responsibility_list = new List<v_Responsibility>();

                new t_Responsibility().Select<t_Responsibility>("Deleted=False").ForEach(a => responsibility_list.Add(
                    new v_Responsibility() { res_name = a.res_name, t_response = a }));

                teremok_work_time = new t_WorkTeremok().SelectFirst<t_WorkTeremok>(String.Format("teremok_id='{0}' AND " +
                        "teremok_day='{1}' AND teremok_month='{2}' AND teremok_year='{3}'", CParam.TeremokId, DocDate.Day, DocDate.Month, DocDate.Year));//DocDate.Month,DocDate.Year

                foreach (t_Marotch mar in MarotchList)
                {
                    VisMarotchList.Add(MakeVisualMarotch(mar));
                    
                }

                listview_smeni.ItemsSource = VisMarotchList;                

                listview_smeni.KeyDown += new KeyEventHandler(listview_smeni_KeyDown);

                #endregion
                pw.ReportProgress(72);
                #region 8 загрузим меню + окно выбора меню

                MenuItemsSprav = new t_Menu().Select<t_Menu>();
                if (MenuItemsSprav != null && MenuItemsSprav.Count > 0)
                {
                    //заполнить add_smena   настройки окна
                    wpf_add_menu_control = new AddElement2win2();

                    add_menu_form = new HostForm(wpf_add_menu_control);
                    add_menu_form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                    wpf_add_menu_control.Parent = add_menu_form;
                    add_menu_form.Width = 910;
                    add_menu_form.Height = 500;

                    
                    wpf_add_menu_control.btn_rem.Visibility = Visibility.Collapsed;
                    wpf_add_menu_control.txbl_Header.Text = "Добавьте обеды";
                    wpf_add_menu_control.btn_add.IsDefault = false;


                    wpf_add_menu_control.iteremWpfControl1.ItemsSource = MenuItemsSpravStr;


                    #region pke
                    //wpf_add_menu_control.listW_MainList.PreviewKeyDown+=(s,e1)=>{
                        
                    //    List<SelectedListBoxItem> menu_items_Source = wpf_add_menu_control.listW_MainList
                    //        .ItemsSource as List<SelectedListBoxItem>;
                    //    SelectedListBoxItem lsbi = wpf_add_menu_control.listW_MainList.SelectedItem as SelectedListBoxItem;
                    //    if (lsbi != null)
                    //    {
                    //        if (e1.Key == Key.Space)
                    //        {
                    //            if (wpf_add_menu_control.listW_MainList.SelectedItem == lsbi)
                    //            {
                    //                lsbi.IsChecked = !lsbi.IsChecked;
                    //            }
                    //            e.Handled = true;
                    //        }
                    //        if (e1.Key == Key.Delete)
                    //        {
                    //            if (wpf_add_menu_control.listW_MainList.SelectedItem == lsbi)
                    //            {
                    //                lsbi.IsChecked = false;
                    //                lsbi.Visibility = Visibility.Collapsed;

                    //                ListViewItem lvi = wpf_add_menu_control.listW_MainList.ItemContainerGenerator
                    //                    .ContainerFromIndex(wpf_add_menu_control.listW_MainList.SelectedIndex) as ListViewItem;
                    //                lvi.IsEnabled = false;
                    //            }
                    //            e.Handled = true;
                    //        }
                    //        if (e1.Key == Key.Insert)
                    //        {
                    //            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                    //            {
                    //                wpf_add_menu_control.text_input.Text = "";
                    //                wpf_add_menu_control.text_input.Focus();
                    //            }));
                    //        }
                    //        if (e1.Key == Key.Enter)
                    //        {
                    //            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                    //            {
                    //                wpf_add_menu_control.btn_add.Focus();
                    //            }));
                    //        }
                    //    }

                    //};
                    #endregion

                    wpf_add_menu_control.listW_MainList.PreviewMouseLeftButtonDown += (s1, e1) =>
                    {
                        var lbi = (ListViewItem)WpfHelperClass.SelectTaskItemClick<UIElement, ListViewItem>(e1.OriginalSource, e);
                        if(lbi!=null)
                            wpf_add_menu_control.listW_MainList.SelectedItem = lbi.DataContext;
                    };

                    wpf_add_menu_control.PreviewKeyDown += (s1, e1) =>
                    {
                        #region ++
                        if (e1.Key == Key.Insert)
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                            {
                                wpf_add_menu_control.iteremWpfControl1.TBox.Text = "";
                                wpf_add_menu_control.iteremWpfControl1.TBox.Focus();
                            }));
                        }
                        #endregion
                    };

                    

                    wpf_add_menu_control.btn_remove_position.Click += (s2, e2) =>
                    {
                        if (wpf_add_menu_control.listW_MainList.SelectedItem!=null)
                        {
                             List<SelectedListBoxItem> menu_items_Source = 
                                wpf_add_menu_control.listW_MainList.ItemsSource as List<SelectedListBoxItem>;

                             SelectedListBoxItem slbi = (SelectedListBoxItem)wpf_add_menu_control.listW_MainList.SelectedItem;

                             menu_items_Source.Remove(slbi);

                             t_Menu t_m = slbi.InnerObject as t_Menu;
                             t_Dinner2t_Menu t_din2t = slbi.InnerObject as t_Dinner2t_Menu;
                             if (t_din2t != null) t_din2t.Delete();

                            wpf_add_menu_control.listW_MainList.ItemsSource = menu_items_Source;
                            UpdateView(menu_items_Source);
                        }
                    };

                    RoutedEventHandler btn_add_position_Click = (s2, e2) =>
                    {
                        #region ++
                        if (wpf_add_menu_control.iteremWpfControl1.LBox.SelectedItem == null) return;
                        object o = wpf_add_menu_control.iteremWpfControl1.LBox.SelectedItem;
                        t_Menu t_m = MenuItemsSprav.WhereFirst(a => a.menu_nome == o.ToString());
                        if (t_m == null) { WpfCustomMessageBox.Show("Не удалось найти указанный пункт меню", "Внимание!!"); return; }

                        List<SelectedListBoxItem> menu_items_Source =
                            wpf_add_menu_control.listW_MainList.ItemsSource as List<SelectedListBoxItem>;
                        if (menu_items_Source == null) menu_items_Source = new List<SelectedListBoxItem>();


                        SelectedListBoxItem lsbi = new SelectedListBoxItem();
                        lsbi.checkBox1.Visibility = Visibility.Collapsed;
                        lsbi.TextBox1.Text = "1";
                        lsbi.ControlHeader = t_m.menu_nome;
                        lsbi.InnerObject = t_m;

                        lsbi.TextBox1.PreviewKeyDown += (s, e1) =>
                        {
                            if (e1.Key == Key.Enter)
                            {
                                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                                {
                                    wpf_add_menu_control.btn_add.Focus();
                                }));
                                e.Handled = true;
                            }


                            if (e1.Key == Key.Insert)
                            {
                                lsbi.InputEnded();
                            }
                        };

                        lsbi.TextInputEnded += (oo) =>
                        {
                            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                            {
                                wpf_add_menu_control.iteremWpfControl1.TBox.Text = "";
                                wpf_add_menu_control.iteremWpfControl1.TBox.Focus();
                            }));
                        };

                        Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                        {
                            lsbi.TextBox1.Focus();
                        }));

                        menu_items_Source.Add(lsbi);

                        wpf_add_menu_control.listW_MainList.ItemsSource = menu_items_Source;
                        wpf_add_menu_control.listW_MainList.SelectedItem = lsbi;
                        ICollectionView view = CollectionViewSource.GetDefaultView(menu_items_Source);
                        view.Refresh();
                        #endregion
                    };

                    wpf_add_menu_control.btn_add_position.Click += btn_add_position_Click;

                    wpf_add_menu_control.iteremWpfControl1.TBox.KeyUp += (s, e1) =>
                    {
                        if (e1.Key == Key.Escape) wpf_add_menu_control.btn_add_Click(s, e1);

                        if (e1.Key == Key.Enter)
                        {
                            btn_add_position_Click(s, e1);
                        }
                    };

                    wpf_add_menu_control.iteremWpfControl1.LBox.PreviewKeyUp += (s, e1) =>
                    {
                        if (e1.Key == Key.Enter)
                        {
                            btn_add_position_Click(s, e1);
                        }
                    };

                    wpf_add_menu_control.iteremWpfControl1.LBox.MouseDoubleClick += (s, e1) =>
                    {
                            btn_add_position_Click(s, e1);
                    };

                    //wpf_add_menu_control.btn_add.Click += (s1, e1) =>
                    //    {
                    //        Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                    //        {
                    //            wpf_add_menu_control.iteremWpfControl1.TBox.Text = "";
                    //        }));
                    //    };

                    wpf_add_menu_control.ReturnObject += (o) =>
                    {
                        #region ++
                        v_Dinner temp = null;
                        //Добавить обеды в базу данных и в грид
                        List<SelectedListBoxItem> menu_items_list = wpf_add_menu_control.listW_MainList.Items.OfType<SelectedListBoxItem>().Where(a => (bool)a.checkBox1.IsChecked).ToList();//.ToList().Where;

                        List<SelectedListBoxItem> menu_items_list_to_del = wpf_add_menu_control.listW_MainList.Items.OfType<SelectedListBoxItem>().Where(a => !(bool)(a.checkBox1.IsChecked)).ToList();

                        menu_items_list_to_del.ForEach(a =>
                        {
                            t_Dinner2t_Menu t_d2m = a.InnerObject as t_Dinner2t_Menu;
                            if (t_d2m != null)
                            {
                                t_d2m.Delete();
                            }

                        });

                        if (menu_items_list.Count > 0 && CurrentUser != null && CurrentDinner != null)
                        {
                            menu_items_list.ForEach(a =>
                            {
                                t_Menu t_m = a.InnerObject as t_Menu;
                                t_Dinner2t_Menu t_din2t = a.InnerObject as t_Dinner2t_Menu;

                                if (t_din2t == null && t_m != null)
                                {
                                    t_din2t = new t_Dinner2t_Menu();
                                    if (t_din2t == null)
                                    {
                                        Log("t_din2t is null");
                                        return;
                                    }

                                    t_din2t.item_quantity = int.Parse(a.TextBox1.Text);

                                    t_din2t.menu_item_id = t_m.menu_id;
                                    t_din2t.dinner_id = CurrentDinner.id;
                                    t_din2t.item_price = t_m.price;
                                    t_din2t.Create();
                                }

                                if (t_din2t != null && t_m == null)
                                {
                                    t_m = MenuItemsSprav.WhereFirst(b => b.menu_nome == a.ControlHeader);
                                    if (t_m == null)
                                    {
                                        Log("t_m is null");
                                        return;
                                    }

                                    t_din2t.item_quantity = int.Parse(a.TextBox1.Text);

                                    t_din2t.menu_item_id = t_m.menu_id;
                                    t_din2t.dinner_id = CurrentDinner.id;
                                    t_din2t.item_price = t_m.price;
                                    t_din2t.Update();
                                }
                            });
                        
                        }

                        DinnersUpdate(CurrentDinner);
                        v_Dinner v_d = new v_Dinner() { t_dinner = CurrentDinner };
                        DinnersUpdate(v_d);
                        temp = v_d;

                        //перевести фокус на обед в списке обедов
                        #region
                        Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                        {

                            if (listView_dinners.SelectedItem == null && temp != null)
                            {
                                listView_dinners.SelectedIndex = listView_dinners.Items.IndexOf(temp);

                                ListViewItem lvi = listView_dinners.ItemContainerGenerator
                                    .ContainerFromIndex(listView_dinners.Items.IndexOf(temp)) as ListViewItem;
                                lvi.Focus();
                            }
                        }));
                        #endregion

                        #endregion
                    };

                    //перевести фокус на текстбокс
                    #region ++
                    wpf_add_menu_control.Loaded += (s, e1) =>
                    {
                    
                        Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                           {
                               wpf_add_menu_control.iteremWpfControl1.TBox.Text = "";
                               wpf_add_menu_control.iteremWpfControl1.TBox.Focus();
                           }));

                    };
                    #endregion
                }
                #endregion
                pw.ReportProgress(79);
                #region 9 сделаем дата байнд и прицепим события list_view обеды

               
                          listView_dinners.ItemsSource = VisDinnersList;
                     

                foreach (t_Dinner dinner in DinnersList)
                {
                    v_Dinner v_d = new v_Dinner() { t_dinner = dinner,Number=VisDinnersList.Count+1};
                    VisDinnersList.Add(v_d);

                    DinnersUpdate(v_d);
                }

                listView_dinners.KeyDown += new KeyEventHandler(listView_dinners_KeyDown);

                listview_smeni.MouseDoubleClick += (s1, e1) =>
                {
                    if (e1.OriginalSource.GetType() == typeof(ScrollViewer))
                    {
                        button_addUser_Click(s1, e1);
                    }

                };
                listView_dinners.MouseDoubleClick += (s1, e1) =>
                {
                    if (e1.OriginalSource.GetType() == typeof(ScrollViewer))
                    {
                        button_addUser1_Click(s1, e1);
                    }
                };
                #endregion
                pw.ReportProgress(86);
                #region 10 загрузить окно комментариев
                DinnerComment = new t_PropValue().SelectFirst<t_PropValue>("Prop_name='" + CurrentDoc.doc_id + "' AND Prop_type='DinnerComment'");
                if (DinnerComment != null) comment_dinner_btn.ToolTip = DinnerComment.prop_value;
                
                SmenaComment = new t_PropValue().SelectFirst<t_PropValue>("Prop_name='" + CurrentDoc.doc_id + "' AND Prop_type='SmenaComment'");
                if (SmenaComment != null) comment_smena_btn.ToolTip = SmenaComment.prop_value;

                wpf_add_comment = new AddElementText();
                add_comment = new HostForm(wpf_add_comment);

                wpf_add_comment.Parent = add_comment;
                add_comment.Width = 600;
                add_comment.Height = 400;

                wpf_add_comment.txbl_Header.Text = "Введите комментарий";

                wpf_add_comment.ReturnObject += (o) =>
                    {
                        string text = o.ToString();

                        if (commentSmenaDinner)
                        {
                            if (SmenaComment == null)
                            {
                                SmenaComment = new t_PropValue() { prop_name = CurrentDoc.doc_id.ToString(), prop_type = "SmenaComment", prop_value = text };
                                SmenaComment.Create();
                            }
                            else
                            {
                                SmenaComment.prop_value = text;
                                SmenaComment.Update();
                            }
                            comment_smena_btn.ToolTip = text;
                        }
                        else
                        {
                            if (DinnerComment == null)
                            {
                                DinnerComment = new t_PropValue() { prop_name = CurrentDoc.doc_id.ToString(), prop_type = "DinnerComment", prop_value = text };
                                DinnerComment.Create();
                            }
                            else
                            {
                                DinnerComment.prop_value = text;
                                DinnerComment.Update();
                            }
                            comment_dinner_btn.ToolTip = text;
                        }
                    };

                #endregion
                pw.ReportProgress(94);
                #region 11 сделать панели
                make1Panel();
                make2Panel();
#endregion

                #region 12 поставить фокус на кнопку
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                {
                    button_addUser.Focus();
                }));
                #endregion
                pw.ReportProgress(100);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if(pw!=null)pw.Stop();
            }
        }

        private void UpdateView(object menu_items_Source)
        {
            WpfHelperClass.DispatcherIdleRun(this, () =>
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(menu_items_Source);
                try
                {
                    view.Refresh();
                }
                catch (Exception ex)
                {
                    Log(ex, "UpdateView error");
                    if (view != null)
                    {
                        if (view.SourceCollection.OfType<v_Marotch>().Count() > 0)
                        {
                            ReBindLVSmeni();
                        }
                        if (view.SourceCollection.OfType<v_Dinner>().Count() > 0)
                        {
                            ReBindLVDinners();
                        }
                    }
                }
            });
        }

        private void ReBindLVSmeni()
        {
            try
            {
                listview_smeni.ItemsSource = null;
                listview_smeni.Items.Clear();
                listview_smeni.ItemsSource = VisMarotchList;
            }catch(Exception ex)
            {
            }
        }

        private void ReBindLVDinners()
        {
            try
            {
                listView_dinners.ItemsSource = null;
                listView_dinners.Items.Clear();
                listView_dinners.ItemsSource = VisDinnersList;
            }
            catch (Exception ex)
            {
            }
        }

        private void make1Panel()
        {
            WpfHelperClass.ChangeParentFor(Expander_base1.Content, Win_Expander_base1.Expander);

            Win_Expander_base1.Maximize_Click = (o) =>
            {
                Win_Expander_base2.Expander.IsExpanded = false;
                Win_Expander_base1.Expander.IsExpanded = true;

                listview_smeni.MaxHeight = double.PositiveInfinity;// 1000; 
                
            };

            Win_Expander_base1.Minimize_Click = (o) =>
            {
                Win_Expander_base2.Expander.IsExpanded = true;
                Win_Expander_base1.Expander.IsExpanded = true;
                listview_smeni.MaxHeight = 210;
                listView_dinners.MaxHeight = 210;
            };
            Expander_base1.Visibility = Visibility.Collapsed;

            Win_Expander_base1.Header.Content = Expander_base1.Header;
            
        }
        private void make2Panel()
        {
            WpfHelperClass.ChangeParentFor(Expander_base2.Content, Win_Expander_base2.Expander);

            Win_Expander_base2.Maximize_Click = (o) =>
            {
                Win_Expander_base2.Expander.IsExpanded = true;
                Win_Expander_base1.Expander.IsExpanded = false;
               listView_dinners.MaxHeight=double.PositiveInfinity;
            };

            Win_Expander_base2.Minimize_Click = (o) =>
            {
                Win_Expander_base2.Expander.IsExpanded = true;
                Win_Expander_base1.Expander.IsExpanded = true;
                listview_smeni.MaxHeight = 210;
                listView_dinners.MaxHeight = 210;
            };
            Expander_base2.Visibility = Visibility.Collapsed;
            Win_Expander_base2.Header.Content = Expander_base2.Header;
        }

        private void ClearMarotch(v_Marotch v_m)
        {
            v_m.t_Marotch.Delete();
        }
        public void button_clear_Click(object sender, RoutedEventArgs e)
        {
            if (WpfCustomMessageBox.Show("Вы действительно хотите удалить все записи из таблицы?", "Внимание!!!", MessageBoxButton.OKCancel)
                == MessageBoxResult.OK)
            {
                foreach (var a in VisMarotchList)
                {
                    ClearMarotch(a);
                }
                VisMarotchList.Clear();
                MarotchList.Clear();
            }
        }

        private void ClearDinner(v_Dinner v_d)
        {
            List<t_Dinner2t_Menu> din_details_list = new t_Dinner2t_Menu().Select<t_Dinner2t_Menu>("Dinner_id=" + v_d.t_dinner.id);
            din_details_list.ForEach(a => a.Delete());
            v_d.t_dinner.Delete();
        }
        public void button_clear_Click1(object sender, RoutedEventArgs e)
        {
            if (WpfCustomMessageBox.Show("Вы действительно хотите удалить все записи из таблицы?", "Внимание!!!", MessageBoxButton.OKCancel)
                == MessageBoxResult.OK)
            {
                foreach (var a in VisDinnersList)
                {
                    ClearDinner(a);
                }
                VisDinnersList.Clear();
                DinnersList.Clear();
            }
        }

        public void button_addComment_Click(object sender,RoutedEventArgs e)
        {
            commentSmenaDinner = true;
            if(SmenaComment!=null){
                wpf_add_comment.MainText.Text = SmenaComment.prop_value;
            }
            add_comment.ShowDialog();
        }

        public void button_addCommentdinner_Click(object sender, RoutedEventArgs e)
        {
            commentSmenaDinner = false;
            if (DinnerComment != null)
            {
                wpf_add_comment.MainText.Text = DinnerComment.prop_value;
            }
            add_comment.ShowDialog();
        }

        private void dinners_base_workout(object sen, TextChangedEventArgs eva)
        {
            //if (wpf_add_menu_control.txb_Search.Text != "")
            //{
            //    SelectedListBoxItem i = null;
            //    foreach (SelectedListBoxItem j in wpf_add_menu_control.listW_MainList.Items.OfType<SelectedListBoxItem>())
            //    {
            //        if (j.ControlHeader.IndexOf(wpf_add_menu_control.txb_Search.Text, StringComparison.OrdinalIgnoreCase) != -1)
            //        {
            //            i = j;
            //            break;
            //        }
            //    }
            //    if (i != null)
            //    {
            //        wpf_add_menu_control.listW_MainList.SelectedItem = i;
            //        wpf_add_menu_control.listW_MainList.ScrollIntoView(i);
            //    }
            //}
        }
        private void DinnerItemCheckBoxChecked(object sender,EventArgs e)
        {
            
            SelectedListBoxItem lsbi = sender as SelectedListBoxItem; //получаем элемент меню 
            
            t_Menu current_nome_selected_dinner=lsbi.InnerObject as t_Menu; //из него получаем запись меню
            v_Dinner vdinn = listView_dinners.SelectedItem as v_Dinner; //получаем текущий обед
            if (CurrentDinnersDetails != null && CurrentDinnersDetails.Count > 0)    //текущий пользователь редактирует обед
            {
                IEnumerable<t_Dinner2t_Menu> _list = CurrentDinnersDetails.Where(a => a.menu_item_id == current_nome_selected_dinner.menu_id); //получаем запись об обеде
                if (_list != null && _list.Count() > 0 && _list.First()!=null)
                {
                    if (!(bool)lsbi.IsChecked) //если галка снята то удаляем обед
                    {
                        t_Dinner2t_Menu d2m = _list.First();
                        d2m.Delete();   //удаляем из базы

                        DinnersUpdate(vdinn.t_dinner);

                    }
                }
                else //текущий пользователь создает обед
                {
                    if ((bool)lsbi.IsChecked) //если галка снята то удаляем обед
                    {
                        t_Dinner2t_Menu new_d2m = new t_Dinner2t_Menu() { dinner_id = vdinn.t_dinner.id, menu_item_id = current_nome_selected_dinner.menu_id }; //создаем детали обеда
                        new_d2m.Create();
                        DinnersUpdate(vdinn.t_dinner);
                    }
                }

            }
            else //текущий пользователь создает обед
            {
                if ((bool)lsbi.IsChecked) //если галка снята то удаляем обед
                {
                    t_Dinner2t_Menu new_d2m = new t_Dinner2t_Menu() { dinner_id = vdinn.t_dinner.id, menu_item_id = current_nome_selected_dinner.menu_id }; //создаем детали обеда
                    new_d2m.Create();
                    DinnersUpdate(vdinn.t_dinner);
                }
            }
        }


        private void DinnersUpdate(v_Dinner v_dinner_to_update)
        {
            List<t_Dinner2t_Menu> sub_dinners = new t_Dinner2t_Menu().
                Select<t_Dinner2t_Menu>("Dinner_id=" + v_dinner_to_update.t_dinner.id);
            if (sub_dinners != null && sub_dinners.Count > 0)
            {
                sub_dinners.ForEach(a =>
                {
                    t_Menu menu_item = MenuItemsSprav.WhereFirst(b => b.menu_id == a.menu_item_id);
                    if (menu_item != null)
                    {
                        v_Dinner2t_Menu v_d = new v_Dinner2t_Menu();
                        v_d.m_nome = menu_item.menu_nome;
                        v_d.m_quant = a.item_quantity;
                        v_d.m_price = (double)menu_item.price;
                        
                        v_dinner_to_update.SubDinnersItems.Add(v_d);
                    }
                });

                if (v_dinner_to_update._SubDinnersItems != null && v_dinner_to_update._SubDinnersItems.Count > 0)
                {
                    v_Dinner2t_Menu old_itog=v_dinner_to_update._SubDinnersItems.WhereFirst(b => b.m_nome == "Итого:");
                    if (old_itog != null) v_dinner_to_update._SubDinnersItems.Remove(old_itog);
                    v_Dinner2t_Menu v_d = new v_Dinner2t_Menu();
                    v_d.m_nome = "Итого:";
                    v_dinner_to_update._SubDinnersItems.ToList().ForEach(a =>
                    {
                        v_d.m_quant += a.m_quant;
                        v_d.m_price += a.m_price * a.m_quant;
                    });

                    v_dinner_to_update.SubDinnersItems.Add(v_d);
                    
                }
            }

            v_Dinner v_din= VisDinnersList.WhereFirst(a => a.t_dinner == v_dinner_to_update.t_dinner);
            if (v_din != null)
            {
                v_dinner_to_update.Number = v_din.Number;
                VisDinnersList[VisDinnersList.IndexOf(v_din)] = v_dinner_to_update;
            }
            //else
            //{
            //    VisDinnersList.Add(v_dinner_to_update);
            //}

            ICollectionView view = CollectionViewSource.GetDefaultView(VisDinnersList);
            view.Refresh();
        }

        private void DinnersUpdate(t_Dinner dinner_to_update)
        {
            List<t_Dinner2t_Menu> sub_dinners = new t_Dinner2t_Menu().Select<t_Dinner2t_Menu>("Dinner_id=" + dinner_to_update.id);
            if (sub_dinners!=null && sub_dinners.Count>1)
            {
                //dinner_to_update.nome_name = "Составной обед";
                string dinners_text = "";
                sub_dinners.ForEach(a =>
                {
                    //MenuItemsSprav.Where(b=>b.menu_id==a.menu_item_id).Count
                    t_Menu menu_item=MenuItemsSprav.WhereFirst(b => b.menu_id == a.menu_item_id);
                    if (menu_item != null) dinners_text += menu_item.menu_nome + " " + a.item_quantity + "\r\n";
                });
                dinner_to_update.nome_name = dinners_text.Substring(0, dinners_text.Length > 254 ? 254 : dinners_text.Length).Trim();
            }
            if (sub_dinners != null && sub_dinners.Count ==1)
            {
                dinner_to_update.nome_name = MenuItemsSprav.Where(a => a.menu_id == sub_dinners.First().menu_item_id).First().menu_nome;
            }
            dinner_to_update.Update();

            
        }

        private void dinn_fact_dclick(object sender, RoutedEventArgs e)
        {
            try
            {
                e.Handled = true;
                if (wpf_add_menu_control == null)
                {
                    WpfCustomMessageBox.Show("Справочник номенклатур не загружен!!!!");
                    return;
                }

                List<SelectedListBoxItem> menu_items_list = 
                    wpf_add_menu_control.listW_MainList.Items.OfType<SelectedListBoxItem>().ToList();
                menu_items_list.Clear();
                wpf_add_menu_control.listW_MainList.ItemsSource = null;

                ////получить текущего пользователя
                var lbi = (ListBoxItem)WpfHelperClass.SelectTaskItemClick<UIElement, ListBoxItem>(sender, e);
                v_Dinner vdinn = lbi.DataContext as v_Dinner;

                CurrentDinner = vdinn.t_dinner;
                CurrentUser = EmployeesList.WhereFirst<t_Employee>(a => a.employee_id == vdinn.t_dinner.employee_id);

                if (CurrentUser == null)
                {
                    Log("Current user is null");
                }

                listView_dinners.SelectedItem = vdinn;

                CurrentDinnersDetails = new t_Dinner2t_Menu().Select<t_Dinner2t_Menu>("Dinner_id=" + vdinn.t_dinner.id);

                if (CurrentDinnersDetails != null && CurrentDinnersDetails.Count > 0)
                {
                    //получить список обедов по пользователю

                    foreach (t_Dinner2t_Menu dinmenu in CurrentDinnersDetails)
                    {
                        t_Menu t_m = MenuItemsSprav.WhereFirst<t_Menu>(a => a.menu_id == dinmenu.menu_item_id);
                        if (t_m == null) continue;

                        SelectedListBoxItem lsbi = new SelectedListBoxItem();
                        lsbi.InnerObject = dinmenu;

                        lsbi.checkBox1.Visibility = Visibility.Collapsed;

                        lsbi.ControlHeader = t_m.menu_nome;
                        lsbi.TextBox1.Text = dinmenu.item_quantity.ToString();
                        lsbi.checkBox1.IsChecked = true;

                        menu_items_list.Add(lsbi);
                    }
                    wpf_add_menu_control.listW_MainList.ItemsSource = menu_items_list;
                }

                add_menu_form.ShowDialog();
            }catch(Exception ex)
            {
                MDIParentMain.Log(ex,"Не получилось добавить окно добавления обедов");
            }
        }

        /// <summary>
        /// Hot Buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listView_dinners_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (listView_dinners.SelectedItem != null)
                    button_delUser1_Click(sender, null);
            }
            if (e.Key == Key.Insert)
            {
                button_addUser1_Click(sender, e);
            }
            if (e.Key == Key.Enter)
            {
                if (listView_dinners.SelectedItem != null)
                {
                    dinn_fact_dclick(e.OriginalSource, e);
                }
            }
        }

        /// <summary>
        /// Hot buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listview_smeni_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (listview_smeni.SelectedItem != null)
                    button_delUser_Click(sender, null);
            }
            if (e.Key == Key.Insert)
            {
                button_addUser_Click(sender, e);
            }
        }

        #endregion


        #region ready elements
        /// <summary>
        /// Делаем тогл баттоны как флажки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void togglebutton_button_Click(object sender, RoutedEventArgs e)
        {
            cbutton = e.Source as ToggleButton;
            if (cbutton != null)
            {
                if ((bool)cbutton.IsChecked)
                {
                    List<ToggleButton> blist = smena_button_panel.Children.OfType<ToggleButton>().ToList();
                    blist.ForEach(a =>
                        {
                            if (a != cbutton) a.IsChecked = false;
                        });
                }
            }
        }

        /// <summary>
        /// Создает визуальный элемент марочного отчета
        /// </summary>
        /// <param name="marotch"></param>
        /// <returns></returns>
        private v_Marotch MakeVisualMarotch(t_Marotch marotch)
        {
            v_Marotch new_v_mar = new v_Marotch();
            
            new_v_mar.parent = this;
            new_v_mar.color_table = color_table;
            new_v_mar.teremok_work_time = teremok_work_time;

            new_v_mar.id = marotch.id;
            new_v_mar.user_name = EmployeesList.Where(a => a.employee_id == marotch.employee_id).First().employee_name;

   
            new_v_mar.user_response_list = responsibility_list;

            if (responsibility_list != null)
            {
                new_v_mar.user_response = responsibility_list.WhereFirst(a => a.t_response.res_guid == marotch.response);
            }

            new_v_mar.t_Marotch = marotch;
                //EmployeesList.Where(a => a.employee_id == marotch.employee_id).First().employee_FunctionName;
            new_v_mar.user_plan = marotch.m_plan;
            new_v_mar.user_plan_color = marotch.responce_guid_plan != null && color_table.ContainsKey(marotch.responce_guid_plan) ? color_table[marotch.responce_guid_plan] : null;
            new_v_mar.user_fact = marotch.m_fact;
            new_v_mar.user_fact_color = marotch.responce_guid_fact != null && color_table.ContainsKey(marotch.responce_guid_fact) ? color_table[marotch.responce_guid_fact] : null;
            new_v_mar.Number = VisMarotchList.Count + 1;
            return new_v_mar;
        }

        #region Dinners Actions

        /// <summary>
        /// Добавляет пользователя в таблицу обедов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_addUser1_Click(object sender, RoutedEventArgs e)
        {
            if (add_user_form == null)
            {
                WpfCustomMessageBox.Show("Не загружен справочник пользователей!!!", "Внимание!");
                return;
            }

            add_user_form.wpfctl.mainListControl.TBox.Text = "";
            System.Windows.Forms.DialogResult d = add_user_form.ShowDialog();
            if (d == System.Windows.Forms.DialogResult.OK)
            {
                AddUserToDinner(CurrentUser);
            }
        }

        /// <summary>
        /// Удаляем обед отчет
        /// </summary>
        private void DeleteDinner(v_Dinner v_dinner)
        {
            //удалить все детали обеда
            List<t_Dinner2t_Menu> din_details_list = new t_Dinner2t_Menu().Select<t_Dinner2t_Menu>("Dinner_id=" + v_dinner.t_dinner.id);
            din_details_list.ForEach(a => a.Delete());

            //удалить обед из базы 
            v_dinner.t_dinner.Delete();
            Dispatcher.Invoke(new Action(delegate
            {
                try
                {
                    VisDinnersList.Remove(v_dinner);
                    RecountDinnerUpdation();
                }
                catch (Exception ex)
                {
                }
            }));
           
        }

        /// <summary>
        /// Удаляем диннер и все его зависимости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_delUser1_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить пользователя?", "Предупреждение", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (listView_dinners.SelectedItem != null)
                {
                    v_Dinner v_dinner = listView_dinners.SelectedItem as v_Dinner;
                    DeleteDinner(v_dinner);
                }
            }
        }
    
        #endregion

        #region maroch actions

        /// <summary>
        /// Добавляет пользователя в таблицу марочных отчетов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_addUser_Click(object sender, RoutedEventArgs e)
        {   
            if (add_user_form == null)
            {
                WpfCustomMessageBox.Show("Не загружен справочник пользователей!!!","Внимание!");
                return;
            }
            if (add_user_form.wpfctl.listW_MainList==null || add_user_form.wpfctl.listW_MainList.Items.Count==0)
            {
                WpfCustomMessageBox.Show("Справочник пользователей пустой!!!!");
                return;
            }
                add_user_form.wpfctl.txb_Search.Text = "";
                System.Windows.Forms.DialogResult d = add_user_form.ShowDialog();
                if (d == System.Windows.Forms.DialogResult.OK)
                {
                        AddUserToMarotch(CurrentUser);
                }
            
        }

        /// <summary>
        /// Добавляем пользователя в Смена и обеды а Смена и обеды в грид
        /// </summary>
        /// <param name="user"></param>
        private void AddUserToMarotch(t_Employee user)
        {
            if (user == null) return;

            if (MarotchList.Select(b => b.employee_id).Contains(user.employee_id))
            { 
                //пользователь уже есть 
                WpfCustomMessageBox.Show("Такой пользователь уже есть!!!", "Внимание!!!", MessageBoxButton.OK);
                return;
            }


            t_Marotch new_t_marotch = new t_Marotch(); //создаем класс обеда

            DateTime dinterval = new DateTime(DocDate.Year, DocDate.Month, 1); //1е число месяца
            t_Doc tabel_doc = new t_Doc().SelectFirst<t_Doc>(String.Format("doc_type_id={0} AND doc_teremok_id={1} AND doc_datetime>{2} AND doc_datetime<{3}",
                28, CurrentDoc.doc_teremok_id, SqlWorker.ReturnDate(dinterval), SqlWorker.ReturnDate(dinterval.AddMonths(1)))); //получаем ассоциированный табель

            //#warning Сделано для теста потом раскомментировать
            t_MarkItems t_mark = null;
            if (tabel_doc != null)
            {
                t_mark = new t_MarkItems().SelectFirst<t_MarkItems>(String.Format("mark_doc_id={0} AND mark_name='{1}'", tabel_doc.doc_id, user.employee_1C)); //получаем ассоциированные кишки табеля
            }
            
            //  t_MarkItems t_mark = new t_MarkItems().SelectFirst<t_MarkItems>(String.Format("mark_doc_id={0} AND mark_name='{1}'", 50, "885a68ff-b08b-11e2-8b2b-3085a9967c88"));


            object resp_guid = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(t_mark, String.Format("mark_guidsmena_{0}", DocDate.Day));
            //object resp_guid = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(t_mark, String.Format("mark_guidsmena_{0}", 21));    //получить нужный день и вытащить смену

            object user_mark = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(t_mark, String.Format("mark_{0}", DocDate.Day));    //получить нужный день и имя смены
            //object user_mark = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(t_mark, String.Format("mark_{0}", 21));    //получить нужный день и вытащить имя смены


            //добавить в базу
            new_t_marotch.doc_id = CurrentDoc.doc_id;
            new_t_marotch.employee_id = user.employee_id;
            new_t_marotch.responce_guid_plan = resp_guid.ToString();
            new_t_marotch.m_plan = user_mark.ToString();
            new_t_marotch.m_date = DocDate;
            new_t_marotch.Create();

            MarotchList.Add(new_t_marotch);
            //добавить в грид

            VisMarotchList.Add(MakeVisualMarotch(new_t_marotch));
        }

        private void ReturnMarotch(t_Employee user, out t_Marotch t_marotch,out v_Marotch v_marotch)
        {
            t_Marotch new_t_marotch = new t_Marotch();

            DateTime dinterval = new DateTime(DocDate.Year, DocDate.Month, 1); //1е число месяца
            t_Doc tabel_doc = new t_Doc().SelectFirst<t_Doc>(String.Format("doc_type_id={0} AND doc_teremok_id={1} AND doc_datetime>{2} AND doc_datetime<{3}",
                28, CurrentDoc.doc_teremok_id, SqlWorker.ReturnDate(dinterval), SqlWorker.ReturnDate(dinterval.AddMonths(1)))); //получаем ассоциированный табель

            //#warning Сделано для теста потом раскомментировать
            t_MarkItems t_mark = new t_MarkItems().SelectFirst<t_MarkItems>(String.Format("mark_doc_id={0} AND mark_name='{1}'", tabel_doc.doc_id, user.employee_1C)); //получаем ассоциированные кишки табеля
            //  t_MarkItems t_mark = new t_MarkItems().SelectFirst<t_MarkItems>(String.Format("mark_doc_id={0} AND mark_name='{1}'", 50, "885a68ff-b08b-11e2-8b2b-3085a9967c88"));


            object resp_guid = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(t_mark, String.Format("mark_guidsmena_{0}", DocDate.Day));
            //object resp_guid = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(t_mark, String.Format("mark_guidsmena_{0}", 21));    //получить нужный день и вытащить смену

            object user_mark = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(t_mark, String.Format("mark_{0}", DocDate.Day));    //получить нужный день и имя смены
            //object user_mark = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(t_mark, String.Format("mark_{0}", 21));    //получить нужный день и вытащить имя смены

            object respp_guid = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(t_mark, "mark_res");

            //добавить в базу
            new_t_marotch.doc_id = CurrentDoc.doc_id;
            new_t_marotch.employee_id = user.employee_id;
            new_t_marotch.responce_guid_plan = resp_guid.ToString();
            new_t_marotch.response = respp_guid.ToString();
            new_t_marotch.m_plan = user_mark.ToString();
            new_t_marotch.m_date = DocDate;
            
            v_Marotch new_v_marotch = MakeVisualMarotch(new_t_marotch);

            t_marotch = new_t_marotch;
            v_marotch = new_v_marotch;
        }

        /// <summary>
        /// Удаляем Смена и обеды
        /// </summary>
        private void DeleteUserMarOtch(v_Marotch v_mar)
        {
            //удалить пользователя из базы 
            t_Marotch t_mar = MarotchList.Where(a => a.id == v_mar.t_Marotch.id).First();
            t_mar.Delete();
            VisMarotchList.Remove(v_mar);
            MarotchList.Remove(t_mar);

            RecountMarotchUpdation();
            //удалить пользователя из грида
        }

        /// <summary>
        /// удалить пользователя из марочного отчета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_delUser_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить пользователя?", "Предупреждение", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (listview_smeni.SelectedItem != null)
                {
                    v_Marotch del_mar = listview_smeni.SelectedItem as v_Marotch;
                    DeleteUserMarOtch(del_mar);
                }
            }
        }

        /// <summary>
        /// Добавить пользователей из табеля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_loadUser_Click(object sender, RoutedEventArgs e)
        {

            if (VisMarotchList.NotNullOrEmpty())
            {
                if (MessageBox.Show("Вы хотите заново заполнить смену из табеля?\r\n      Табель будет очищен и перезаполнен.", "Внимание!!!", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }
                else
                {
                    VisMarotchList.ToList().ForEach(a => a.t_Marotch.Delete());
                    VisMarotchList.Clear();
                    MarotchList.Clear();
                }
            }

                t_Doc tabel_doc = null;

                tabel_doc = Find_Asociated_Tabel();

                if (null == tabel_doc)
                {
                    WpfCustomMessageBox.Show("Нет табеля!", "Внимание!!!");
                    return;
                }

                List<t_MarkItems> t_mark = new t_MarkItems().Select<t_MarkItems>(String.Format("mark_doc_id={0}", tabel_doc.doc_id)); //получаем ассоциированные кишки табеля

                if (null == t_mark || t_mark.Count == 0)
                {
                    WpfCustomMessageBox.Show("Нет информации в табеле за " + DocDate.ToShortDateString() + "!", "Внимание!!!");
                    return;
                }

                List<t_Employee> emp_list = new List<t_Employee>();
                //фильтруем по дню
                t_mark.ForEach(a =>
                {
                    object user_mark = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(a, String.Format("mark_{0}", DocDate.Day));

                    if (user_mark==null||user_mark.ToString() == "")
                    {
                       // t_mark.Remove(a);
                    }
                    else
                    {
                        t_Employee emp = new t_Employee().SelectFirst<t_Employee>(String.Format("employee_1C='{0}'", a.mark_name));
                        emp_list.Add(emp);
                    }
                });

                if (emp_list.Count == 0)
                {
                    WpfCustomMessageBox.Show("Нет информации в табеле за " + DocDate.ToShortDateString() + "!", "Внимание!!!");
                    return;
                }

                emp_list.ForEach(a =>
                {
                    t_Marotch t_m = new t_Marotch();
                    v_Marotch v_m = new v_Marotch();
                    
                    ReturnMarotch(a, out t_m, out v_m);

                    if (!MarotchList.Select(b => b.employee_id + b.m_plan).Contains(t_m.employee_id + t_m.m_plan))
                    {
                        t_m.Create();
                        v_m.id = t_m.id;
                        MarotchList.Add(t_m);
                        VisMarotchList.Add(v_m);
                    }
                });
                ListViewUpdate(VisMarotchList);
        }

        private t_Doc Find_Asociated_Tabel()
        {
            DateTime dinterval = new DateTime(DocDate.Year, DocDate.Month, 1);
            t_Doc tabel_doc = new t_Doc().SelectFirst<t_Doc>(String.Format("doc_type_id={0} AND doc_teremok_id={1} AND doc_datetime>{2} AND doc_datetime<{3}",
                28, CurrentDoc.doc_teremok_id, SqlWorker.ReturnDate(dinterval), SqlWorker.ReturnDate(dinterval.AddMonths(1)))); //получаем ассоциированный табель
            return tabel_doc;
        }


        /// <summary>
        /// Выбрать нужный элемент в листбоксе марочного отчета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectTaskItemClick(object sender, RoutedEventArgs e)
        {
            var lbi = (ListBoxItem)WpfHelperClass.SelectTaskItemClick<TextBox, ListBoxItem>(sender, e); ;
            listview_smeni.SelectedItem = lbi.DataContext as v_Marotch;
        }


        private void user_update_click(object sender,MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                SelectCurrentListBoxItem(sender, e);
                return;
            }
            if (e.ClickCount == 2)
            {
                if (add_user_form == null)
                {
                    WpfCustomMessageBox.Show("Не загружен справочник пользователей!!!", "Внимание!");
                    return;
                }
                if (add_user_form.wpfctl.listW_MainList == null || add_user_form.wpfctl.listW_MainList.Items.Count == 0)
                {
                    WpfCustomMessageBox.Show("Справочник пользователей пустой!!!!");
                    return;
                }
                add_user_form.wpfctl.txb_Search.Text = "";
                System.Windows.Forms.DialogResult d = add_user_form.ShowDialog();
                if (d == System.Windows.Forms.DialogResult.OK)
                {
                    var lbi = (ListBoxItem)WpfHelperClass.SelectTaskItemClick<UIElement, ListBoxItem>(sender, e);
                    v_Marotch vmar = lbi.DataContext as v_Marotch;
                    vmar.t_Marotch.employee_id = CurrentUser.employee_id;
                    vmar.t_Marotch.Update();
                    vmar.user_name = CurrentUser.employee_name;
                    ListViewUpdate(VisMarotchList);
                }
                return;
            }
        }

        private void SelectCurrentListBoxItem(object sender, MouseButtonEventArgs e)
        {            
            var lbi = (ListViewItem)WpfHelperClass.SelectTaskItemClick<UIElement, ListViewItem>(sender, e);

            e.Handled = false;
            WpfHelperClass.DispatcherIdleRun(this, () =>
            {
                listview_smeni.UnselectAll();
                lbi.IsSelected = true;

                listview_smeni_SelectionChanged(lbi.Content, e);
            });
            
        }

        private void ListViewUpdate(object menu_items_Source)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(menu_items_Source);
            view.Refresh();
        }



        

        /// <summary>
        /// Клик на ячейке марочного отчета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cell_fact_dclick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                SelectTaskItemClick(sender, e);
                v_Marotch v_mar = listview_smeni.SelectedItem as v_Marotch;

                if (cbutton != null && (bool)cbutton.IsChecked && (bool)cbutton.IsEnabled)
                {
                    //добавить фактическую смену
                    t_ButtonTemplate bt = sm.Where(a => a.bnt_value == cbutton.Content.ToString()).First();
                    if (bt.bnt_value == "") //кнопка отчистить
                    {
                        v_mar.ClearWorkDetails();
                    }
                    else   //кнопка нажата
                    {
                        v_mar.SetWorkDetailsButton(bt);
                    }
                }
                else
                {
                    //открыть окно для добавления смены

                    t_Marotch t_m = v_mar.t_Marotch;
                    if (t_m == null) { Log("Ошибка в документе - выбранный элемент смен - нуль"); return; }
                    result_smena1 = null;

                    t_Responsibility resp=GetResponsibility(v_mar);
                    if (resp == null)
                    {
                        WpfCustomMessageBox.Show("Необходимо выбрать обязанность сотрудника!");
                        return;
                    }
                    wf_add_smena_control.UpdateShiftList(GetAllowedShiftTypes(resp.res_guid).ToList());
                    if (wf_add_smena_control.Show(t_m, teremok_work_time) == System.Windows.Forms.DialogResult.OK)
                    {
                        if (result_smena1 != null)
                        {
                            v_mar.SetWorkDetailsSmena(result_smena1);
                        }
                    }
                }
                UpdateView(VisMarotchList);
            }catch(Exception ex)
            {
                MDIParentMain.Log(ex,"Не получилось открыть окно фактических смен");
            }
        }

        private t_Responsibility GetResponsibility(v_Marotch v_mar)
        {
            if (v_mar == null || v_mar.user_response == null) return null;
            return v_mar.user_response.t_response;
        }

       
        #endregion

        #endregion

        private void RecountMarotch()
        {
            for (int i = 0; i < VisMarotchList.Count; i++)
            {
                VisMarotchList[i].Number = i + 1;
            }
        }

        private void RecountMarotchUpdation()
        {
            RecountMarotch();
            ListViewUpdate(VisMarotchList);
        }

        private void RecountDinner()
        {
            for(int i=0;i<VisDinnersList.Count;i++)
            {
                VisDinnersList[i].Number=i+1;
            }
        }

        private void RecountDinnerUpdation()
        {
            RecountDinner();
            ListViewUpdate(VisDinnersList);
        }
        /// <summary>
        /// Добавляем строку документа обед
        /// </summary>
        /// <param name="user"></param>  
        private void AddUserToDinner(t_Employee user)
        {

            if (VisDinnersList.Select(b => b.t_dinner.employee_id).Contains(user.employee_id))
            {
                WpfCustomMessageBox.Show("Такой пользователь уже есть!!!", "Внимание!!!", MessageBoxButton.OK);
                return;
            }

            v_Dinner dinner = new v_Dinner() { 
                t_dinner = new t_Dinner() 
                    {doc_id = CurrentDoc.doc_id, employee_id = user.employee_id, 
                     emploee_name = user.employee_name, d_date = DocDate },
                Number = VisDinnersList.Count+1
            };
            dinner.t_dinner.Create();
            Dispatcher.Invoke(new Action(delegate
            {
                try
                {
                    VisDinnersList.Add(dinner);
                }
                catch (Exception ex)
                {
                }
            }));
            
        }

        


        /// <summary>
        /// Класс визуализации обеда
        /// </summary>
        class v_Dinner : INotifyPropertyChanged
        {
            public t_Dinner t_dinner;

            private int _number = 0;
            public int Number
            {
                get
                {
                    return _number;
                }
                set
                {
                    _number = value;
                }
            }

            public string user_name
            {
                get { return t_dinner.emploee_name; }
                set
                {
                    t_dinner.emploee_name = value;
                    t_dinner.Update();
                }
            }
            public string dinner_name
            {
                get { return t_dinner.nome_name; }
                set
                {
                    t_dinner.nome_name = value;
                    t_dinner.Update();
                }
            }

            public string kkm_number
            {
                get { return t_dinner.kkm_code; }
                set
                {
                    t_dinner.kkm_code = value;
                    t_dinner.Update();
                }
            }

            private bool _dinner_enable = true;
            public bool dinner_enable
            {
                get { return _dinner_enable; }
                set { _dinner_enable = value; }
            }

            private bool _dinner_kkm_loaded = false;
            public bool dinner_kkm_loaded
            {
                get { return _dinner_kkm_loaded; }
                set { _dinner_kkm_loaded = value; }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            List<t_Menu> _InternalDinnerItems=null;
            List<t_Menu> InternalDinnerItems
            {
                get
                {
                    if (_InternalDinnerItems == null)
                    {
                        _InternalDinnerItems = new List<t_Menu>();
                        
                    }
                    return _InternalDinnerItems;
                }
            }
            
            public ObservableCollection<v_Dinner2t_Menu> _SubDinnersItems = null;
            public ObservableCollection<v_Dinner2t_Menu> SubDinnersItems
            {
                get
                {
                    if (_SubDinnersItems == null)
                    {
                        _SubDinnersItems = new ObservableCollection<v_Dinner2t_Menu>();
                        _SubDinnersItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_InternalDinnerItems_CollectionChanged);
                    }
                    return _SubDinnersItems;
                }
                set
                {
                    _SubDinnersItems = value;
                }
            }

           private void _InternalDinnerItems_CollectionChanged(object sender,NotifyCollectionChangedEventArgs e)
           {
               if(PropertyChanged!=null)
               PropertyChanged(this, new PropertyChangedEventArgs("SubDinnersItems"));
           }
        }

        /// <summary>
        /// Визуальный класс марочного отчета
        /// </summary>
        class v_Marotch : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public int id { get; set; }
            public int Number { get; set; }
            public string user_name { get; set; }

            private v_Responsibility _user_response;
            public v_Responsibility user_response 
            { 
                get
                {
                    return _user_response;
                }
                set
                {
                    if (_user_response != null)
                    {
                        previous_user_response = _user_response;
                    }
                    _user_response = value;
                    OnPropertyChanged("user_response");
                }
            }
            public v_Responsibility previous_user_response { get; set; }
            public List<v_Responsibility> user_response_list { get; set; }
            public string user_plan { get; set; }
            public SolidColorBrush user_plan_color { get; set; }
            public string user_fact { get; set; }
            public SolidColorBrush user_fact_color { get; set; }

            //parent fields
            public MarOtchCentarPanel parent;
            public OrderClass order;
            public Dictionary<string, SolidColorBrush> color_table;
            public t_WorkTeremok teremok_work_time;

            public t_Marotch t_Marotch;
            public string comment { 
                get
            {
                return t_Marotch.comment;
            }
                set
                {
                    t_Marotch.comment = value;
                    t_Marotch.Update();
                }
            }

            public void ClearWorkDetails()
            {
                SetWorkDetailsButton(null);
            }

            internal void SetWorkDetailsSmena(SmenaViewClass1 result_smena1)
            {
                if (result_smena1 != null)
                {
                    user_fact = result_smena1.ShiftType.type_value + result_smena1.TimeInterval.Hours.ToString("F1");
                    user_fact_color = color_table.ContainsKey(result_smena1.ShiftType.type_guid) ? color_table[result_smena1.ShiftType.type_guid] : null;

                    SetWorkDetails(
                        result_smena1.ShiftType.type_guid,
                        result_smena1.ShiftType.type_value + result_smena1.TimeInterval.Hours.ToString("F1"),
                        result_smena1.TimeInterval.From,
                        result_smena1.TimeInterval.To
                        );
                    
                }
            }

            private void SetWorkDetails(string smena_guid, string mHours, string timeFrom, string timeTo)
            {
                t_Marotch.responce_guid_fact = smena_guid;
                t_Marotch.m_fact = mHours;
                t_Marotch.fact_work_from = timeFrom;
                t_Marotch.fact_work_to = timeTo;
                t_Marotch.Update();
            }

            public void SetWorkDetailsButton(t_ButtonTemplate bt)
            {
                v_Marotch v_mar = this;
                t_Marotch t_mar = v_mar.t_Marotch;
                if (t_mar != null)
                {
                    if (bt == null)
                    {
                        v_mar.user_fact = "";
                        v_mar.user_fact_color = null;
                        SetWorkDetails("","", "", "");
                    }
                    else
                    {
                        v_mar.user_fact = bt.bnt_value;
                        v_mar.user_fact_color = bt.btn_SmenaType != null && color_table.ContainsKey(bt.btn_SmenaType) ? color_table[bt.btn_SmenaType] : null;

                        string timeFrom = "";
                        string timeTo = "";
                        CountTime(bt.bnt_value,ref timeFrom,ref timeTo);

                        SetWorkDetails(bt.btn_SmenaType,bt.bnt_value, timeFrom, timeTo);
                    }
                }
            }

            private void CountTime(string button_value, ref string timeFrom, ref string timeTo)
            {
               // if (timeTo == "" || timeFrom == "") return;
                int chours = RBClient.WinForms.ViewModels.MarkViewModelItem.DefaultEndTimeInt - RBClient.WinForms.ViewModels.MarkViewModelItem.DefaultStartTimeInt;
                decimal hours = GetHours(button_value);
                if (hours < chours)
                {
                    timeFrom = TodbDateFormat(RBClient.WinForms.ViewModels.MarkViewModelItem.DefaultStartTimeInt);
                    timeTo = TodbDateFormat(RBClient.WinForms.ViewModels.MarkViewModelItem.DefaultStartTimeInt + hours);
                }
                else
                {
                    timeFrom = TodbDateFormat(24-hours);
                    timeTo = TodbDateFormat(24);
                }
            }

            public static string TodbDateFormat(decimal hours)
            {
                var intlaces = Math.Truncate(hours);
                var decPlaces = (hours % 1) * 0.6m;

                if (decPlaces > 0)
                {
                    return (intlaces + decPlaces).ToString("F2").Replace(',', ':');
                }
                else
                {
                    return hours.ToString("F2").Replace(',', ':');
                }
            }

            private decimal GetHours(string button_value)
            {
                if (String.IsNullOrEmpty(button_value)) return 0;
                string hours = button_value.Substring(1);
                decimal h = 0;
                if (decimal.TryParse(hours, out h))
                {
                    return h;
                }
                else
                {
                    return 0;
                }   
            }

            protected void OnPropertyChanged(string name)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        /// <summary>
        /// Класс типа смены
        /// </summary>
        class Smena_list_item
        {
            public int hours_from { get; set; }
            public int hcount { get; set; }
            public int to_hours { get; set; }
            public string smena_name { get; set; }
            public string smena_guid { get; set; }
        }

        /// <summary>
        /// Класс типа обязанности
        /// </summary>
        class v_Responsibility
        {
          public string res_name { get; set; }
          public t_Responsibility t_response;
        }

        /// <summary>
        /// 
        /// </summary>
        class v_Dinner2t_Menu
        {
            t_Dinner2t_Menu t_d2t_m;
            public string m_nome { get; set; }
            public int m_quant { get; set; }
            public double m_price { get; set; }
        }


        Regex reg_smena_0 = new Regex("[1-9]+");
        /// <summary>
        /// Копируем пользователей из табеля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_copyUsers_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Скопировать пользователей из верхней таблицы?", "Предупреждение", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                VisMarotchList.ToList().ForEach(a =>
                {
                    t_Marotch t_mar = MarotchList.Where(b => b.id == a.id).First();

                    if (reg_smena_0.IsMatch(t_mar.m_fact.ReturnOrEmpty() + t_mar.m_plan.ReturnOrEmpty()))
                    {
                        t_Employee emp = EmployeesList.Where(c => c.employee_id == t_mar.employee_id).First();
                        if (!VisDinnersList.Select(b => b.t_dinner.employee_id).Contains(emp.employee_id))
                        {
                            AddUserToDinner(emp);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// известить компонент об изменении
        /// </summary>
        /// <param name="message"></param>
        //private void ReportWpfLoadComponent(string message)
        //{
        //    if (wpf_lc != null)
        //    {
        //        Dispatcher.Invoke(new Action(delegate
        //        {
        //            wpf_lc.header_lbl.Content = message;
        //        }));
        //    }
        //}

        private void ReportWpfLoadComponent(string message,int count_back)
        {
            if (wpf_lc != null)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    try
                    {
                        if (wpf_lc != null)
                        {
                            wpf_lc.header_lbl.Content = message;
                            wpf_lc.Start(count_back);
                        }
                    }catch(Exception ex)
                    {
                    }
                }));
            }
        }

        /// <summary>
        /// Получает парсит и добавляет обеды из т-репорта
        /// </summary>
        public void GetTReportDinnersFromKKm() 
        {
            bool timer_kkm_walked = StaticConstants.MainWindow.m_kkm1_online || StaticConstants.MainWindow.m_kkm2_online || StaticConstants.MainWindow.m_kkm3_online ||
               StaticConstants.MainWindow.m_kkm4_online || StaticConstants.MainWindow.m_kkm5_online;
            string old_message = "";

            if(!timer_kkm_walked){
                       old_message = wpf_lc.header_lbl.Content.ToString();
                       ReportWpfLoadComponent("Проверяем наличие касс...",200);
            }

            //получить пути для касс
            List<string> kkmInList = RbClientGlobalStaticMethods.ReturnKKmInPathes();
            List<string> kkmOutList = RbClientGlobalStaticMethods.ReturnKKmOutPathes();
            if (kkmInList == null || kkmOutList == null)
            {
                WpfCustomMessageBox.Show("Нет касс в сети!!!");
                ReportWpfLoadComponent("Нет касс в сети!!!...", 0);
                ErrorKKmLoad();
                return;
            }
            
            if (!timer_kkm_walked)
            {
                ReportWpfLoadComponent(old_message,0);
            }

            //в задание
            //закинуть туда 0.dat
            
            //Очистить файлы t-report на кассах
            ReportWpfLoadComponent("Подключаемся к кассам...", 5 * kkmInList.Count);
            kkmOutList.ForEach(a =>
            {
                DirectoryInfo dir = new DirectoryInfo(a);
                List<FileInfo> file_pathes = dir.GetFiles("T*", SearchOption.TopDirectoryOnly).ToList();
                file_pathes.ForEach(b=>new CustomAction((o) => 
                {
                    b.Delete();
                }
                , null).Start());
            });

            ReportWpfLoadComponent("Запрашиваем временные отчеты с касс...", 5 * kkmInList.Count);
            kkmInList.ForEach(a => new CustomAction((o) =>
            {string filename = o.ToString();File.Create(System.IO.Path.Combine(a, filename));}, "0.dat").Start());

//            ReportWpfLoadComponent("Ожидаем ответа от касс...", kkmOutList.Count * 100);
            //дождаться ответа касс
            DirectoryInfo s_treport_dir = null;
            DirectoryInfo m_treport_dir = RbClientGlobalStaticMethods.GetDirectory("TReport");
            s_treport_dir = m_treport_dir.GetDecendantDirectory("Marotch");
            
            
            s_treport_dir.DeleteOldFilesInDir(0);

            ReportWpfLoadComponent("Ожидаем ответа от касс...", kkmOutList.Count * 110);

            kkmOutList.ForEach(a => new CustomAction((o) =>
            {
                DirectoryInfo kkm_dir = new DirectoryInfo(a);
                List<FileInfo> file_pathes = kkm_dir.GetFiles("T*", SearchOption.TopDirectoryOnly).ToList();
                if (file_pathes == null || file_pathes.Count == 0) throw new Exception("Еще пока нет Трепорта");
                file_pathes.Sort((c, b) => b.CreationTime.CompareTo(c.CreationTime));

                new CustomAction((oo) =>
                {
                    FileInfo o1 = oo as FileInfo; if (o1 == null) return;
#if(!DEB)
                    o1.MoveTo(System.IO.Path.Combine(s_treport_dir.FullName, o1.Name) + kkmOutList.IndexOf(a));
#endif
#if(DEB)
                    o1.CopyTo(System.IO.Path.Combine(s_treport_dir.FullName, o1.Name) + kkmOutList.IndexOf(a));
#endif
                }, file_pathes[0]) { Timeout = 5000 }.Start();
            }, null) { Timeout = 10000 }.Start());


            ReportWpfLoadComponent("Создаем обеды с кассы...",0);
            //пропарсить t-otchet и создать обеды
            if (s_treport_dir != null && s_treport_dir.GetFiles().Count() > 0)
            {
                //DinnersList
                s_treport_dir.GetFiles("T*", SearchOption.TopDirectoryOnly).ToList().ForEach(a => T_Report_parse_dinners_add(a));
            }
            else
            {
                WpfCustomMessageBox.Show("Не получены обеды с кассы! Нет временного файла кассы!");
            }
            
            //обновляем грид
            //ICollectionView view = CollectionViewSource.GetDefaultView(VisDinnersList); view.Refresh();
        }

        /// <summary>
        /// Парсит обед и добавляет в базу и в лист
        /// </summary>
        /// <param name="t_rep_file"></param>
        public void T_Report_parse_dinners_add(FileInfo t_rep_file)
        {

            string fi_tesxt = File.ReadAllText(t_rep_file.FullName, Encoding.GetEncoding(1251));


            Regex fio_regex = new Regex(@"(?s)([А-Яа-я].*?)(\$P|Оплата)");
            Regex check_regex = new Regex(@"(?s)(Чек)\s*\d+/\d+3\s*(\d{2}.\d{2}.\d{2}\s*\d{2}:\d{2}:\d{2})\s*(\d)\s*(\d+[.]\d+)(.*?Оплата)\s+(4)\s+(\d+[.]\d+)(.*?[+])");
            Regex check_item_regex = new Regex(@"(\d+)\s+([\d.\d]+)\s+([\d.\d]+)\s+[\d.\d]+\s+[\d+\s\d-]+\s+[\d.%]+\s(\d+)\s+\D+\s\D+\D+\s");
            Regex kkm_code_regex = new Regex(@"Касса (\d+)");

            string kkm_code = "";
            if (kkm_code_regex.IsMatch(fi_tesxt))
            {
                kkm_code = kkm_code_regex.Match(fi_tesxt).Groups[1].Value;
            }

            int dinner_parsed_count = 0;

            foreach (Match m in check_regex.Matches(fi_tesxt))  //проходим по обедам в файле
            {
                t_Dinner t_dinner = new t_Dinner();
                List<t_Dinner2t_Menu> din_items_list = new List<t_Dinner2t_Menu>();
                DateTime current_check_datetime = DateTime.Now;//CurrentDoc.doc_datetime;
                DateTime.TryParse(m.Groups[2].Value, out current_check_datetime);


                string id_1c = "";
                double quantity = 0;
                double price = 0;
                int nome_ad = 0;
                try
                {
                    foreach (Match c in check_item_regex.Matches(m.Value))
                    {
                        id_1c = c.Groups[1].Value;
                        quantity = 0; double.TryParse(c.Groups[2].Value.Replace(".", ","), out quantity);
                        price = 0; double.TryParse(c.Groups[3].Value.Replace(".", ","), out price);

                         IEnumerable<t_Menu> tm=MenuItemsSprav.Where(a => a.menu_nome_1C == id_1c);
                        if(tm==null || tm.Count()==0)
                        {
                            Log("Не найдена номенклатура " + id_1c);
                            continue;
                        }

                        nome_ad = tm.Select(a => a.menu_id).First<int>();
                            
                        t_Dinner2t_Menu t_din2menu = new t_Dinner2t_Menu() { menu_item_id = nome_ad, item_quantity = (int)quantity, item_price = (decimal)price };
                        din_items_list.Add(t_din2menu);

                        if (fio_regex.IsMatch(c.Value))
                        {
                            string employee_fio = fio_regex.Match(c.Value).Groups[1].Value.Trim();

                            t_Employee curr_emp = EmployeesList.WhereFirst(a => a.employee_name == employee_fio);

                            if (curr_emp == null)
                            {
                                WpfCustomMessageBox.Show("Пользователь " + employee_fio + " не найден в базе данных!!!", "Внимание!!!");
                                continue;
                            }



                            t_dinner.employee_id = curr_emp.employee_id;
                            t_dinner.emploee_name = curr_emp.employee_name;


                            t_dinner.doc_id = CurrentDoc.doc_id; t_dinner.d_date = current_check_datetime;
                            t_dinner.kkm_code = kkm_code;

                            //если дата в обеде не соответствует дате документа
#if(!DEB)
                            if (current_check_datetime.Date != DocDate.Date)
                                continue;
#endif
                            //проверка если пользователя еще нет в списке обедов
#if(!DEB)
                            if (!VisDinnersList.Select(q => q.t_dinner.emploee_name + q.t_dinner.kkm_code + q.t_dinner.d_date)
                                .Contains(t_dinner.emploee_name + t_dinner.kkm_code + t_dinner.d_date))
#else
                            if (!VisDinnersList.Select(q => q.t_dinner.emploee_name)
                                .Contains(t_dinner.emploee_name))
#endif
                            {
                                CreateVisualDinnerFromKKm(t_dinner, din_items_list);
                                dinner_parsed_count++;
                                continue;
                            }
                            else
                            {
                                //v_Dinner v_dinn = VisDinnersList.WhereFirst(q =>
                                //    (q.t_dinner.emploee_name + q.t_dinner.kkm_code + q.t_dinner.d_date) ==
                                //    t_dinner.emploee_name + t_dinner.kkm_code + t_dinner.d_date);

                                v_Dinner v_dinn = VisDinnersList.WhereFirst(q =>
                                    (q.t_dinner.emploee_name) ==
                                    t_dinner.emploee_name);

                                ////проверка если пользователь в и у него нет обеда
                                if (!v_dinn.SubDinnersItems.NotNullOrEmpty())
                                {
                                    DeleteDinner(v_dinn);
                                    CreateVisualDinnerFromKKm(t_dinner, din_items_list);
                                    dinner_parsed_count++;
                                    continue;
                                }
                                else
                                {
                                    //если у пользователя уже есть обед
                                    if(WpfCustomMessageBox.Show("У сотрудника "+employee_fio+" уже внесен обед . Перезаписать данными с кассы?",
                                        "Внимание!!",MessageBoxButton.OKCancel)==MessageBoxResult.OK)
                                    {
                                        DeleteDinner(v_dinn);
                                        CreateVisualDinnerFromKKm(t_dinner, din_items_list);
                                        dinner_parsed_count++;
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log("Не удалось пропарсить обед. Error " + ex.Message + " объекты:" + Serializer.JsonSerialize(t_dinner) + " и элементы этого обеда " + Serializer.JsonSerialize(din_items_list));
                    WpfCustomMessageBox.Show("Не удалось загрузить обеды!!!");
                }
            }

            if (dinner_parsed_count == 0)
            {
                WpfCustomMessageBox.Show("Нет обедов за текущую дату! "+DocDate.Date.ToShortDateString());
            }
        }

        /// <summary>
        /// Создать обед и его детали
        /// </summary>
        /// <param name="t_dinner"></param>
        /// <param name="din_items_list"></param>
        private void CreateVisualDinnerFromKKm(t_Dinner t_dinner, List<t_Dinner2t_Menu> din_items_list)
        {
            try
            {
                t_dinner.Create();
                din_items_list.ForEach(a =>
                {
                    a.dinner_id = t_dinner.id; a.Create();
                });

                v_Dinner dinner = new v_Dinner() { t_dinner = t_dinner, dinner_kkm_loaded = true };
                DinnersUpdate(dinner.t_dinner);
                DinnersUpdate(dinner);

                Dispatcher.Invoke(new Action(delegate
                {
                    try
                    {
                        VisDinnersList.Add(dinner);
                    }
                    catch (Exception ex)
                    {
                    }
                }));
            }catch(Exception ex)
            {
                Log("Не удалось добавить обед из кассы. Error " + ex.Message + " объекты:" + Serializer.JsonSerialize(t_dinner) + " и элементы этого обеда " + Serializer.JsonSerialize(din_items_list));
            }
        }

        private void DisableKKmLoad()
        {
            Dispatcher.Invoke(new Action(delegate
            {
                dinner_dock_panel.Children.Remove(wpf_lc);
                wpf_lc = null;
                listView_dinners.Visibility = Visibility.Visible;
                ICollectionView view = CollectionViewSource.GetDefaultView(VisDinnersList); view.Refresh();
                button_copyFromMark.IsEnabled = true;
            }));
        }

        private void ErrorKKmLoad()
        {
            WpfCustomMessageBox.Show("Не удалось загрузить обеды с касс!");
            if (wpf_lc!=null) dinner_dock_panel.Children.Remove(wpf_lc);
            
            listView_dinners.Visibility = Visibility.Visible;
            button_copyFromMark.IsEnabled = true;
        }

        private void EnableKkmLoad()
        {
            listView_dinners.Visibility = Visibility.Collapsed;

            if (wpf_lc == null)
            {
                wpf_lc = new WpfLoadingControl1();
                wpf_lc.Margin = new Thickness(0, 40, 40, 0);
            }

            dinner_dock_panel.Children.Add(wpf_lc);
            button_copyFromMark.IsEnabled = false;
        }

        private void button_copyFromMark_Click(object sender, RoutedEventArgs e)
        {
            EnableKkmLoad();
            CustomAction ca = new CustomAction((o) => { GetTReportDinnersFromKKm(); }, null);
            ca.StateChangedEvent = (o) =>
            {
                CustomAction thi4 = o as CustomAction;
                if (thi4.State == StateEnum.SuccessfulComplete)
                {
                    DisableKKmLoad();
                    
                }
                if (thi4.State == StateEnum.ErrorTriesEnded || thi4.State == StateEnum.Error)
                {
                    ErrorKKmLoad();
                }

            };

            //BackgroundWorker bkgr = new BackgroundWorker();
            System.Threading.Thread th = new System.Threading.Thread(ca.Start);
            th.Start();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //#if(DEB)
            //            return;
            //#endif
            //            if (!this.IsEnabled)
            //            {
            //                return;
            //            }

            int sending_status = 2;

            try
            {
                #region set temp variables
                //отправить тестовый документ
                int docid = CurrentDoc.doc_id;

                // MainProgressReport.Instance.ReportProgress("Отправляем \"Смена и обеды\" № " + docid, 0);

                t_Doc currDoc = CurrentDoc;
                List<t_Employee> emp_list = order.GetDetailTable<t_Employee>().Select(a => (t_Employee)a.t_table).ToList();
                //new t_Employee().Select<t_Employee>();

                List<t_Menu> menu_list = new t_Menu().Select<t_Menu>();

                List<t_Dinner> t_din_list = new t_Dinner().Select<t_Dinner>("doc_id=" + currDoc.doc_id);

                List<t_Marotch> t_mar_list = new t_Marotch().Select<t_Marotch>("doc_id=" + currDoc.doc_id);

                List<t_Dinner2t_Menu> t_dishes_list = new List<t_Dinner2t_Menu>();

                t_din_list.ForEach(a =>
                {
                    List<t_Dinner2t_Menu> tempList = new t_Dinner2t_Menu().Select<t_Dinner2t_Menu>("Dinner_id=" + a.id);
                    if (tempList != null && tempList.Count > 0)
                    {
                        t_dishes_list.AddRange(tempList);
                    }
                });
                #endregion

                StaffReport sta = new StaffReport();
                sta.Date = currDoc.doc_datetime;
                sta.Id = currDoc.doc_guid;

                if (DinnerComment == null) sta.RationComment = ""; else sta.RationComment = DinnerComment.prop_value;
                if (SmenaComment == null) sta.ShiftComment = ""; else sta.ShiftComment = SmenaComment.prop_value;

                #region fill shiftsOfEmployee
                List<ShiftOfEmployee> soi_list = new List<ShiftOfEmployee>();
                Regex dig_rex = new Regex(@"\d+");
                t_mar_list.ForEach(a =>
                {
                    ShiftOfEmployee shi = new ShiftOfEmployee();
                    shi.Employee = emp_list.WhereFirst(b => b.employee_id == a.employee_id).employee_1C;
                    shi.ShiftType = a.responce_guid_fact;


                    decimal val = 0;
                    decimal.TryParse(dig_rex.Match(a.m_fact).Value, out val);
                    shi.HoursWorked = val;

                    shi.Responsibility = a.response;
                    shi.Comment = a.comment;

                    soi_list.Add(shi);
                });
                #endregion

                #region fill RationOfEmployee
                List<RationOfEmployee> roi_list = new List<RationOfEmployee>();

                var roi_temp_list1 =

                    from var2 in
                        (
                            from var1 in
                                (from dinner in t_din_list
                                 join emp in emp_list
                                 on dinner.employee_id equals emp.employee_id
                                 select new { DinnId = dinner.id, EmpGiud = emp.employee_1C })
                            join dish in t_dishes_list
                            on var1.DinnId equals dish.dinner_id
                            select new { Empl = var1.EmpGiud, Quant = dish.item_quantity, DishId = dish.menu_item_id })
                    join men in menu_list
                    on var2.DishId equals men.menu_id
                    select new { Empl = var2.Empl, Quant = var2.Quant, Dish1C = men.menu_nome_1C, Price = men.price };

                foreach (var roi_temp in roi_temp_list1)
                {
                    RationOfEmployee roi = new RationOfEmployee();
                    roi.Dish = roi_temp.Dish1C;
                    roi.Employee = roi_temp.Empl;
                    roi.Quantity = roi_temp.Quant;

                    roi.Price = roi_temp.Price;
                    roi.Sum = roi_temp.Price * roi_temp.Quant;

                    roi_list.Add(roi);
                }
                #endregion

                sta.ShiftOfEmployee = soi_list.ToArray();
                sta.RationOfEmployee = roi_list.ToArray();

                string hash = Serializer.JsonSerialize(sta);
                hash = Hashing.GetMd5Hash(hash);

                if (CurrentDoc.doc_menu_dep != hash)
                {
                    //продумать просто сохранение если задача уже не валидна или состояние удалено
                    order.Document_SaveCurrentDocumentState(new WebService1CStateClass("PutStaffReport", new object[] { int.Parse(CParam.TeremokId), sta }));
                }
            }
            catch (Exception ex)
            {
                Log(ex, "Не удалось пометить на отправку документ \"Смена и обеды\" " + Serializer.JsonSerialize(CurrentDoc));
            }
        }

        private void cmb_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb == null) return;
            v_Responsibility v_r=cmb.SelectedItem as v_Responsibility;
            var lsbi = WpfHelperClass.SelectTaskItemClick<ComboBox, ListBoxItem>(sender, e);
            v_Marotch v_mar = lsbi.Content as v_Marotch;

            if (v_r != null)
            {
                if (HasFactSmena(v_mar))
                {
                    if (IsNewResponsSelected(v_r, v_mar))
                    {
                        if (PreviousResponseIsNotNull(v_mar))
                        {

                            if (WpfCustomMessageBox.Show("Выбранная смена будет очищена! Продолжить?", "Внимание", MessageBoxButton.OKCancel) ==
                        MessageBoxResult.OK)
                            {
                                WpfHelperClass.DispatcherIdleRun(this, () =>
                                {
                                    FillResponse(v_r, v_mar);
                                    v_mar.ClearWorkDetails();
                                    UpdateView(VisMarotchList);
                                    
                                });
                            }
                            else
                            {
                         
                                WpfHelperClass.DispatcherIdleRun(this, () =>
                                {
                                    cmb.SelectionChanged -= cmb_selectionChanged;
                                    cmb.SelectedItem = v_mar.previous_user_response;
                                    cmb.SelectionChanged += cmb_selectionChanged;

                                    
                                });
                            }
                        }
                    }
                }
                else
                {
                    FillResponse(v_r, v_mar);
                }
            }

            
        }

        private bool PreviousResponseIsNotNull(v_Marotch v_mar)
        {
            return v_mar.previous_user_response != null;
        }

        private bool IsNewResponsSelected(v_Responsibility v_r, v_Marotch v_mar)
        {
            if (v_r.t_response.res_guid != v_mar.t_Marotch.response)
            {
                //var list=GetShiftsAllowed(v_r.t_response.res_guid).OfType<t_Shifts_Allowed>().Select(a=>a.shift_guid);
                //if(v_mar.)
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool HasFactSmena(v_Marotch v_mar)
        {
            if (String.IsNullOrEmpty(v_mar.user_fact))
                return false;
            else
                return true;
        }

        private void FillResponse(v_Responsibility v_r, v_Marotch v_mar)
        {
            FilterButtons(v_r);
            t_Marotch t_mar = v_mar.t_Marotch;
            t_mar.response = v_r.t_response.res_guid;
            t_mar.Update();
        }

        private void FilterButtons(v_Responsibility response)
        {
            if (response == null)
            {
                FilterButtons("");
            }
            else
            {
                FilterButtons(response.t_response.res_guid);
            }
        }

        private void FilterButtons(string res_guid)
        {
            BlockAllButtons();
            if (String.IsNullOrEmpty(res_guid)) return;
            var lisr = GetShiftsAllowed(res_guid);
            if (lisr.NotNullOrEmpty())
            {
                UnblockButtons(lisr.Select(a => (t_Shifts_Allowed)a.t_table).ToList());
            }
        }

        private IEnumerable<ModelItemClass> GetShiftsAllowed(string res_guid)
        {
            var lisr = order.GetDetailTable<t_Shifts_Allowed>().Where(a => ((t_Shifts_Allowed)a.t_table).res_guid == res_guid);
            return lisr;
        }

        private IEnumerable<t_ShiftType> GetAllowedShiftTypes(string res_guid)
        {
            var lisr = GetShiftsAllowed(res_guid).Select(a=>((t_Shifts_Allowed)a.t_table).shift_guid).ToList();
            return shifts.Where(a => lisr.Contains(a.type_guid));
        }

        private void UnblockButtons(List<t_Shifts_Allowed> list)
        {
            var temp = list.Select(a => a.shift_guid).ToList();
            smButtons.ForEach(a =>
            {
                if(temp.Contains(((t_ButtonTemplate)a.Tag).btn_SmenaType))
                {
                    a.IsEnabled = true;
                }
            });
        }

        private void BlockAllButtons()
        {
            smButtons.ForEach(a =>
            {
                a.IsEnabled = false;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          //  this.Parent
            if (MarotchList.Where(a => a.response == "" || a.response == null).Count() > 0)
            {
                WpfCustomMessageBox.Show("Укажите у сотрудников обязанность!", "Внимание", MessageBoxButton.OK);
                return;
            }

            if (WpfCustomMessageBox.Show("Дествительно закрыть и отправить документ?", "Внимание!!!", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                System.Windows.Forms.Form[] _forms = StaticConstants.MainWindow.MdiChildren;

                foreach (System.Windows.Forms.Form _f in _forms)
                {
                    if (_f.GetType() == typeof(FormMarochOtch))
                    {
                        _f.Close();
                    }
                }
            }
        }

        private void Button_comment_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentDoc != null)
            {
                //wpf_add_comment.MainText = CurrentDoc.comment;
                add_comment.ShowDialog();
            }
        }

        private void popComment_MainTextChanged(object sender, TextChangedEventArgs e)
        {
            var lbi = (ListBoxItem)WpfHelperClass.SelectTaskItemClick<TextBox, ListBoxItem>(sender, e);
            v_Marotch v_m=lbi.DataContext as v_Marotch;
            listview_smeni.SelectedItem = v_m;
            v_m.comment = (sender as TextBox).Text;
        }

        private void listview_smeni_SelectionChanged(object sender,EventArgs e)
        {
            try
            {
                var selItem = listview_smeni.SelectedItem as v_Marotch;
                if (selItem != null)
                {
                    var resp = selItem.user_response;

                    FilterButtons(resp);
                }
            }
            catch (Exception ex)
            {
                Log(ex, "listview_smeni_SelectionChanged error");
            }
        }
    }
}