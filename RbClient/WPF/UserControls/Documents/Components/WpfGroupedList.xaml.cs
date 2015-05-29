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
using RBClient.WPF.ViewModels;
using RBClient.Classes.DocumentClasses;
using System.Collections;
using System.ComponentModel;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes;

namespace RBClient.WPF.UserControls.Documents.Components
{
    /// <summary>
    /// Логика взаимодействия для WpfGroupedList.xaml
    /// </summary>
    public partial class WpfGroupedList : UserControl, IMainGrid
    {
        //IHasInnerListViewer
        private WpfGroupedList()
        {
            InitializeComponent();
        }
        public WpfGroupedList(Type listviewer_type, OrderClass order)
        {
            InitializeComponent();

            this.order = order;
            glist = order.ViewsCollection;
            this.listviewer_type = listviewer_type;
            this.Enabled = !order.IsDocumentBlocked;
            SelfControl = this;

            Fill_grouped_list();
        }

        #region ImainGrid implementation

        public void OnAddNomenclatureEvent(ModelItemClass mic)
        {
            Func<ModelItemClass, ModelItemClass, bool> equalsMM =
                (a, b) => ((t_Order2ProdDetails)a.t_table).opd_nome_id == ((t_Order2ProdDetails)b.t_table).opd_nome_id;

            ModelItemClass detail = order.CreateOrderLineFromTemplate(mic.t_table);
            ViewModelItem vmi = order.InsertDetail(detail, equalsMM);
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

        Dictionary<int, object> ViewCollections;

        public bool Enabled { get;set; }

        public void Fill_grouped_list()
        {
            main_panel.Children.Clear();
            //List<int> groups = glist.Select(p => p.group_index).Distinct().ToList();
            List<int> groups = glist.OfType<IGropElement>().Select(p => p.group_index).Distinct().ToList();
            ViewCollections = new Dictionary<int, object>();
            foreach(int gindex in groups)
            {

                IEnumerable<ViewModelItem> elements_in_group = glist.Where(a => ((IGropElement)a).group_index.Equals(gindex)).ToList();
                CreateGroup(gindex, elements_in_group);
                
            }
        }

        public Expander CreateGroup(int gindex,IEnumerable<ViewModelItem> elements_in_group)
        {
            if (elements_in_group.Count() > 0)
            {

                Expander ex = new Expander();
                ex.MaxHeight = 400;
              //  ScrollViewer scv = new ScrollViewer();
                ex.Header = ((IGropElement)elements_in_group.First()).GetGroupTitle();
                IHasInnerListViewer wpdgrid = (IHasInnerListViewer)Activator.CreateInstance(listviewer_type, new object[] { order });
                StaticHelperClass.SetClassItemValue(listviewer_type, wpdgrid, "Enabled", Enabled);
                        
                wpdgrid.GetListSource().ItemsSource = elements_in_group;

                ex.Content = wpdgrid;
                //scv.Content = wpdgrid;
                //scv.IsEnabled = true;
                //scv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                //ex.Content = scv;
                main_panel.Children.Add(ex);
                ViewCollections.Add(gindex, new ArrayList() { elements_in_group, ex, wpdgrid.GetListSource() });
                return ex;
            }
            return null;
        }

        public void AddView(ViewModelItem viewModelItem)
        {
            try
            {
                IEnumerable<ViewModelItem> lvmi = null;

                if (!ViewCollections.ContainsKey((viewModelItem as IGropElement).group_index))
                {
                    CreateGroup(((IGropElement)viewModelItem).group_index, new List<ViewModelItem>() { viewModelItem });
                }
                else
                {
                    lvmi = ((ViewCollections[(viewModelItem as IGropElement).group_index] as ArrayList)[0] as List<ViewModelItem>);
                    (lvmi as List<ViewModelItem>).Add(viewModelItem);
                    
                    ICollectionView view = CollectionViewSource.GetDefaultView(lvmi);
                    view.Refresh();
                }
                ((Expander)(((ArrayList)ViewCollections[((IGropElement)viewModelItem).group_index])[1])).IsExpanded = true;
                //order.UpdateViewSource(lvmi);
            }catch(Exception ex)
            {
            }
        }

        public void InsertView(ViewModelItem viewModelItem,Func<ModelItemClass,ModelItemClass,bool> comp)
        {
            try
            {
                IEnumerable<ViewModelItem> lvmi = null;

                if (!ViewCollections.ContainsKey((viewModelItem as IGropElement).group_index))
                {
                    CreateGroup(((IGropElement)viewModelItem).group_index, new List<ViewModelItem>() { viewModelItem });
                }
                else
                {
                    lvmi = ((ViewCollections[(viewModelItem as IGropElement).group_index] as ArrayList)[0] as List<ViewModelItem>);

                    var llvmi=(lvmi as List<ViewModelItem>);
                    if (llvmi != null)
                    {
                        var present_item = from c in llvmi where comp(c.modelitem, viewModelItem.modelitem) select c;
                        if (present_item.NotNullOrEmpty())
                        {
                            llvmi.Insert(llvmi.IndexOf(present_item.Last()) + 1, viewModelItem);
                        }
                        else
                        {
                            llvmi.Add(viewModelItem);
                        }
                    }

                    ICollectionView view = CollectionViewSource.GetDefaultView(lvmi);
                    view.Refresh();
                }
                ((Expander)(((ArrayList)ViewCollections[((IGropElement)viewModelItem).group_index])[1])).IsExpanded = true;
            }
            catch (Exception ex)
            {
            }
        }

        public void FindModelItem(ModelItemClass mic)
        {
            try
            {
                if (mic == null) return;
                List<OutboxReturnViewModelItem> source = order.ViewsCollection.
                    OfType<OutboxReturnViewModelItem>().ToList();
                ItemsControl lvmi = null;
                foreach (OutboxReturnViewModelItem s in source)
                {
                    if (((t_Order2ProdDetails)s.modelitem.t_table).opd_nome_id == mic.Id)
                    {
                        lvmi = ((ViewCollections[(s as IGropElement).group_index] as ArrayList)[2] as ItemsControl);
                        if (lvmi != null)
                        {
                            Expander ex = ((Expander)(((ArrayList)ViewCollections[((IGropElement)s).group_index])[1]));
                            foreach(KeyValuePair<int,object> kv in ViewCollections){
                                Expander ex1 = (Expander)((ArrayList)kv.Value)[1];
                                if(ex!=ex1)
                                    ex1.IsExpanded = false;
                            }
                            ex.IsExpanded = true;

                            WpdDocGrid wpd = WpfHelperClass.FindParent<WpdDocGrid>(lvmi);
                            if (wpd != null)
                            {
                                CurrentItem = s.modelitem;
                                wpd.listview_main.SelectedItem = s;
                                wpd.listview_main.ScrollIntoView(s);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        List<ViewModelItem> glist;
        Type listviewer_type;
        OrderClass order;

        class TreeViewItemGrid
        {
            public string Header { get; set; }
            public UserControl Content { get; set; }
        }
    }
}
