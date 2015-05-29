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
using RBClient.Classes.DocumentClasses;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes;
using RBClient.Classes.WindowAddElement;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using RBClient.WPF.UserControls.Documents.Components;
using RBClient.WPF.ViewModels;

namespace RBClient.WPF.UserControls.Documents
{
    /// <summary>
    /// Логика взаимодействия для FormGridHeader.xaml
    /// </summary>
    public partial class FormGridHeader : UserControl,IHeaderControl
    {
        private FormGridHeader()
        {
            InitializeComponent();
        }

        OrderClass order;
        FormAddElement add_user_form;
        ModelItemClass SelectedItem;
        IMainGrid MainControl;

        private PassDataBaseObject AddNomenclatureEvent;
        private PassDataBaseObject SearchNomenclatureEvent;

        #region IheaderControl implementation
        public void AddInnerGrid(IMainGrid InnerControl)
        {
            MainControl = InnerControl;
            this.Content_panel.Children.Add(InnerControl.SelfControl);

            //прицепить события хедера

            this.AddNomenclatureEvent = (o) =>
            {
                ModelItemClass mic = o as ModelItemClass;
                if (mic != null)
                {
                    InnerControl.OnAddNomenclatureEvent(mic);
                }
            };

            this.SearchNomenclatureEvent = (o) =>
            {
                ModelItemClass mic = o as ModelItemClass;
                if (o != null)
                {
                    InnerControl.OnSearchNomenclatureEvent(mic);
                }
            };
        }
        #endregion

        #region ISelfControl implementation
        public UserControl SelfControl { get; set; }
        #endregion

        public FormGridHeader(OrderClass order)
        {
            InitializeComponent();
            this.order = order;
            SelfControl = this;

            Loaded += (s, e) =>
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
                {
                    #region окно добавления пользователей
                    //добавить пользователя
                    add_user_form = new FormAddElement();
                    add_user_form.wpfctl.txbl_Header.Text = "Выберите номенклатуру";

                    //передать ему список добавляемых номенклатур
                    add_user_form.wpfctl.mainListControl.ItemsSource = AddNomenclatures;

                    add_user_form.wpfctl.listW_MainList.FontSize = 12;
                    add_user_form.wpfctl.ReturnObject = (o) =>
                    {
                        SelectedItem = (ModelItemClass)o;
                    };
                    
                    #endregion
                }));
            };
        }


        

        private List<ModelItemClass> AddNomenclatures
        {
            get
            {
                return order.DetailsDictionary[typeof(t_Nomenclature)];
            }
        }

        private List<ModelItemClass> _findNomenclatures;
        private List<ModelItemClass> FindNomenclatures
        {
            get
            {
                if (_findNomenclatures == null)
                {
                    List<ModelItemClass> nomes = order.DetailsDictionary[typeof(t_Nomenclature)];
                    List<ModelItemClass> details = order.DetailsDictionary[typeof(t_Order2ProdDetails)];
                    _findNomenclatures = new List<ModelItemClass>();
                    details.ForEach(a =>
                    {
                        t_Order2ProdDetails o2pd = (t_Order2ProdDetails)a.t_table;
                        ModelItemClass n = nomes.Find(b => ((t_Nomenclature)b.t_table).nome_id == o2pd.opd_nome_id);
                        if (n != null)
                        {
                            _findNomenclatures.Add(n);
                        }
                    });
                }
                return _findNomenclatures;
            }
        }
        
        private void nomenclature_add_btn_Click(object sender, RoutedEventArgs e)
        {
            //открыть окно добавления
            if (add_user_form != null)
            {
                add_user_form.wpfctl.mainListControl.TBox.Text = "";
                System.Windows.Forms.DialogResult d = add_user_form.ShowDialog();
                if (d == System.Windows.Forms.DialogResult.OK)
                {
                    //если нажата ок - то добавить номенклатуру
                    //возбудить событие и вернуть туда моделайтем создаваемой номенклатуры
                    if (AddNomenclatureEvent != null && SelectedItem != null)
                    {
                        AddNomenclatureEvent(SelectedItem);
                    }
                }
            }
        }
        
        private void search_btn_Click(object sender, RoutedEventArgs e)
        {
            //при начале введения создать автозаполнение
            // при конце введения  - возбудеть событие и вернуть моделайтем искомой номенклатуры
            if (search_txtbx.Text != "")
            {
                int idx = 0;

                if (MainControl.CurrentItem != null)
                {
                    var temp = FindNomenclatures.WhereFirst(a => ((t_Nomenclature)a.t_table).nome_id == ((t_Order2ProdDetails)MainControl.CurrentItem.t_table).opd_nome_id);
                    if(temp!=null)
                        idx = FindNomenclatures.IndexOf(temp)+1;
                }

                ModelItemClass mi = null;
                foreach (ModelItemClass j in FindNomenclatures.GetRange(idx, FindNomenclatures.Count()-idx))
                {
                    t_Nomenclature nome = (t_Nomenclature)j.t_table;
                    if (nome.nome_name.IndexOf(search_txtbx.Text, StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        mi = j;
                        break;
                    }
                }

                if (mi == null)
                {
                    foreach (ModelItemClass j in FindNomenclatures)
                    {
                        t_Nomenclature nome = (t_Nomenclature)j.t_table;
                        if (nome.nome_name.IndexOf(search_txtbx.Text, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            mi = j;
                            break;
                        }
                    }

                }

                if (mi != null)
                {
                    if (SearchNomenclatureEvent != null && mi != null)
                    {
                        SearchNomenclatureEvent(mi);
                    }
                }
            }

        }

        private void search_txtbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            search_btn_Click(sender, e);
        }

        public PassObject GroupViewChecked;
        private void group_view_chkbx_Checked(object sender, RoutedEventArgs e)
        {
            if (GroupViewChecked != null)
            {
                GroupViewChecked(group_view_chkbx.IsChecked);
            }
        }

        private void search_txtbx_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                search_btn_Click(sender, e);
            }
        }
    }
}
