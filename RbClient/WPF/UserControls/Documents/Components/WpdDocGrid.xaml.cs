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
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes.DocumentClasses;
using RBClient.WPF.ViewModels;
using RBClient.Classes;
using System.Windows.Threading;
using RBClient.Classes.CustomClasses;
using System.Globalization;
using System.Windows.Markup;

namespace RBClient.WPF.UserControls.Documents.Components
{
    //public class WidthConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        double columnsCount = 0;
    //        double.TryParse(parameter.ToString().Replace(".", ","), out columnsCount);

    //        ListView lv = value as ListView;
    //        double actualWidth = lv.ActualWidth;
    //        GridView gv = lv.View as GridView;
    //        DependencyObject dp = lv.ContainerFromElement(lv);
    //        double width = (actualWidth / gv.Columns.Count) * (columnsCount);
    //        return width;
    //    }

    //    //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    //{
    //    //    int columnsCount = System.Convert.ToInt32(parameter);
    //    //    double width = (double)value;
    //    //    return width / columnsCount;
    //    //}

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class yourConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //WpfHelperClass.FindChild
            return true;//((GroupItem)parameter).Contains(value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Логика взаимодействия для WpdDocGrid.xaml
    /// </summary>
    public partial class WpdDocGrid : UserControl, IHasInnerListViewer, IMainGrid
    {
        public IEnumerable<object> Properties;
        public TextChangedEventHandler QuantityChanged;
        public TextChangedEventHandler CommentReturnChanged;
        public OrderClass CurrentOrder;

        #region ImainGrid implementation

        public void OnAddNomenclatureEvent(ModelItemClass mic)
        {
            try
            {
                Func<ModelItemClass, ModelItemClass, bool> equalsMM =
                    (a, b) => ((t_Order2ProdDetails)a.t_table).opd_nome_id == ((t_Order2ProdDetails)b.t_table).opd_nome_id;

                ModelItemClass detail = CurrentOrder.CreateOrderLineFromTemplate(mic.t_table);
                ViewModelItem vmi = CurrentOrder.InsertDetail(detail, equalsMM);

                WpfHelperClass.DispatcherIdleRun(this, () =>
                {
                    SelectAndShowItem(vmi, false);
                });

            }catch(Exception ex)
            {
            }
        }

        public void OnSearchNomenclatureEvent(ModelItemClass mic)
        {
            this.FindModelItem(mic);
        }

        public ModelItemClass CurrentItem { get; set; }
        #endregion

        #region ISelfControl implementation
        public UserControl SelfControl { get; set; }
        #endregion


        public WpdDocGrid(OrderClass order)
        {
            WpfSetRusCulrture();
            InitializeComponent();

            CurrentOrder = order;
            SelfControl = this;
            this.listview_main.ItemsSource = order.ViewsCollection;
            this.Enabled = !order.IsDocumentBlocked;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listview_main.ItemsSource);

            PropertyGroupDescription groupDescription3 = new PropertyGroupDescription("Group3");
            view.GroupDescriptions.Add(groupDescription3);

            PropertyGroupDescription groupDescription2 = new PropertyGroupDescription("Group2");
            view.GroupDescriptions.Add(groupDescription2);

            PropertyGroupDescription groupDescription1 = new PropertyGroupDescription("Group1");
            view.GroupDescriptions.Add(groupDescription1);

            

            

            //this.listview_main.ItemsSource = view;

            //Свернуть все експандеры
            #region

            if (view.Groups.Count > 1)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                {
                    var a = WpfHelperClass.FindChildren<Expander>(listview_main);

                    if (a.NotNullOrEmpty())
                    {
                        a.ForEach(b => b.IsExpanded = false);
                    }

                    // string k="asdasd";
                    //if (listView_dinners.SelectedItem == null && temp != null)
                    //{
                    //    listView_dinners.SelectedIndex = listView_dinners.Items.IndexOf(temp);

                    //    ListViewItem lvi = listView_dinners.ItemContainerGenerator
                    //        .ContainerFromIndex(listView_dinners.Items.IndexOf(temp)) as ListViewItem;
                    //    lvi.Focus();
                    //}
                }));
            }
            #endregion
        }

        private static void WpfSetRusCulrture()
        {
            try
            {
                var curr_meta = FrameworkElement.LanguageProperty.GetMetadata(typeof(FrameworkElement));

                var new_meta = new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                CultureInfo.CurrentCulture.IetfLanguageTag));

                if (!curr_meta.Equals(new_meta))
                {
                    FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new_meta);
                }

            }
            catch (Exception ex)
            {
                MDIParentMain.Log(ex.Message);
            }
        }


        public ItemsControl GetListSource()
        {
            return listview_main;
        }

        private bool _enabled=true;
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                if (_enabled)
                {
                    listview_main.IsEnabled = true;
                  //  main_scroll.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Hidden;
                }
                else
                {
                    listview_main.IsEnabled = false;
                  //  main_scroll.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
                }
            }
        }

        private void quantity_text_changed(object sender, TextChangedEventArgs e)
        {
            if (QuantityChanged != null)
            {
                QuantityChanged(sender,e);
            }
        }
        
        private void return_comment_text_changed(object sender, TextChangedEventArgs e)
        {
            if (CommentReturnChanged != null)
            {
                CommentReturnChanged(sender, e);
            }
        }

        public void FindModelItem(ModelItemClass mic)
        {
            try
            {
                if (mic == null) return;
                List<OutboxReturnViewModelItem> source = listview_main.
                    ItemsSource.OfType<OutboxReturnViewModelItem>().ToList();
                foreach (OutboxReturnViewModelItem s in source)
                {
                    if (((t_Order2ProdDetails)s.modelitem.t_table).opd_nome_id == mic.Id)
                    {
                        SelectAndShowItem(s,true);

                        break;
                    }
                }
            }
            catch(Exception ex)
            {
            }
        }

        private void SelectAndShowItem(ViewModelItem s,bool expandExpander)
        {
            listview_main.SelectedItem = s;
            CurrentItem = s.modelitem;
            //listview_main.ScrollIntoView(s);
            //a.BindingGroup;
            //  listview_main.gr
            if (expandExpander)
            {
                ExpandExpander();
            }
            listview_main.ScrollIntoView(s);
        }

        private void ExpandExpander()
        {
            //Expander expander = WpfHelperClass.FindVisualParent<Expander>(WpfHelperClass.GetListItem(listview_main, listview_main.SelectedItem));
            //expander.IsExpanded = true;

            
                List<Expander> list = WpfHelperClass.FindVisualParents<Expander>(WpfHelperClass.GetListItem(listview_main, listview_main.SelectedItem));
                list.ForEach(a => a.IsExpanded = true);
            
        }

        private void WpdDocGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch(Exception ex)
            {
            }
        }

        private void listview_main_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }



        private void List_sel_changed(object sender, SelectionChangedEventArgs e)
        {
           // CurrentItem = ((OutboxReturnViewModelItem)listview_main.SelectedItem).modelitem;
        }

        private void preview_key_down_list(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (listview_main.SelectedIndex - 1 >= 0)
                {
                    listview_main.SelectedIndex = listview_main.SelectedIndex - 1;
                    ListViewItem lvi = listview_main.ItemContainerGenerator.ContainerFromItem(listview_main.SelectedItem) as ListViewItem;
                    var _lvi = WpfHelperClass.FindChild<TextBox>(lvi
                        , "Quantity_txtbx");
                    Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                    {
                        var txtbx = (TextBox)_lvi;
                        txtbx.Focus();
                        txtbx.SelectAll();
                    }));
                }

            }
            if (e.Key == Key.Down || e.Key == Key.Enter)
            {
                if (listview_main.SelectedIndex + 1 < listview_main.Items.Count)
                {
                    listview_main.SelectedIndex = listview_main.SelectedIndex + 1;
                    ListViewItem lvi = listview_main.ItemContainerGenerator.ContainerFromItem(listview_main.SelectedItem) as ListViewItem;
                    var _lvi = WpfHelperClass.FindChild<TextBox>(lvi
                        , "Quantity_txtbx");
                    Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                    {
                        var txtbx = (TextBox)_lvi;
                        txtbx.Focus();
                        txtbx.SelectAll();
                    }));
                }
            }
        }

        private void Quantity_got_focus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
            {
                var txtbx = (TextBox)sender;
                txtbx.SelectAll();
            }));
        }
    }
}
