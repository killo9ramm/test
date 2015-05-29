using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes;
using RBClient.Classes.DocumentClasses;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls;
using RBClient.Classes.WindowAddElement;
using System.Windows.Media;
using RBClient.Classes.CustomWindows;
using System.Collections;

namespace RBClient.WPF.ViewModels
{
    public interface IGropElement
    {
        string group_name { get; set; }
        int group_index { get; set; }
        void SetGroup();
        object GetGroupTitle();
    }

    public class Group
    {
        public string group_name { get; set; }
        public int group_index { get; set; }

        private Group()
        {
        }

        public Group(int _gi)
        {
            group_index = _gi;
        }
        public Group(int _gi,string _gn):this(_gi)
        {
            group_name = _gn;
        }
        public override int GetHashCode()
        {
            return group_index;
        }

        public override bool Equals(object obj)
        {
            if (obj is Group)
                return group_index.Equals(((Group)obj).group_index);
            else
                return base.Equals(obj);
        }
        public override string ToString()
        {
            return group_name;
        }
    }

    public interface IHasInnerListViewer
    {
        ItemsControl GetListSource();
    }


    [Serializable]
    public abstract class ViewModelItem : INotifyPropertyChanged
    {
        public ModelItemClass modelitem { get; set; }
        public static object MakeTempObject(Type model_type, string query)
        {
            object o = Activator.CreateInstance(model_type);
            object o1 = StaticHelperClass.ExecuteMethodByName(o, "SelectReflection", new object[] { query });
            return o1;
        }
        //-----------------changes for inkass

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }

            if (modelitem != null)
            {
                modelitem.DBUpdate(true);
            }
        }

        private OrderClass Order;

        protected Hashtable rht<T>() where T : ModelClass
        {
            return Order.HDetailsDictionary[typeof(T)];
        }

        protected void SetValueToModel(string prop_name, object prop_value)
        {
            StaticHelperClass.SetClassItemValue(modelitem.t_table.GetType(),
                modelitem.t_table, prop_name, prop_value);
        }
    }

    class OutboxReturnViewModelItem : ViewModelItem, INotifyPropertyChanged, IGropElement
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string NomenclatureName_bnd { get; set; }

        private SolidColorBrush q_color = new SolidColorBrush(Colors.White);
        public SolidColorBrush QuantityColor_bnd
        {
            get
            {
                return q_color;
            }
            set 
            {
                q_color = value;
                OnPropertyChanged("QuantityColor_bnd");
            }
        }

        [System.ComponentModel.DefaultValue(true)]
        public bool QuantityReadonly_bnd { get; set; }

        private decimal? _quantity;
        private decimal? new_quantity;

        public decimal Quantity_bnd
        {
            get
            {
                //return (int)_quantity;
                return (decimal)_quantity;
                
            }
            set
            {
                try
                {
                    _quantity = value;
                    //_quantity = decimal.Parse(value);
                    
                    StaticHelperClass.SetClassItemValue(modelitem.t_table.GetType(), modelitem.t_table, "opd_total",
                        OrderFactory.ComputeTotal(order,modelitem, (decimal)_quantity));

                    StaticHelperClass.SetClassItemValue(modelitem.t_table.GetType(), modelitem.t_table, "opd_order",
                        OrderFactory.ComputeTotal(order, modelitem, (decimal)_quantity));
                    if (_quantity > 0)
                    {
                        AddRefReason();
                    }
                    if (_quantity <= 0)
                    {
                        ReturnCategoty_bnd = "";
                        ReturnComment_bnd = "";
                    }
                    
                    OnPropertyChanged("Quantity_bnd");
                }catch(Exception ex)
                {
                   
                }
               
            }
        }



        public string Error
        {
            set
            {
                ShowError(value);
            }
        }

        public void ShowError(string message)
        {
            WpfCustomMessageBox.Show(message);
        }

        public void AddRefReason()
        {
            FormAddElement add_user_form = TempObject5 as FormAddElement;
            TempObject6 = null;
            add_user_form.wpfctl.txb_Search.Text = "";
            System.Windows.Forms.DialogResult d = add_user_form.ShowDialog();
            if (d == System.Windows.Forms.DialogResult.OK)
            {
                if (TempObject6 != null)
                {
                    ModelItemClass mi = TempObject6 as ModelItemClass;
                    t_OrderRetReason ret_reason = (TempObject4 as List<object>).OfType<t_OrderRetReason>().WhereFirst<t_OrderRetReason>(a => a.orr_id == mi.Id);
                    ReturnCategoty_bnd = ret_reason.orr_name;
                }
            }
            else
            {
                Quantity_bnd = 0;
               // Quantity_bnd = "0";
            }

        }

        public string BazovieEdinici_bnd{get;set;}

        public string _returnCategoty = "";
        public string ReturnCategoty_bnd {
            get { return _returnCategoty; }
            set 
            {
                _returnCategoty = value;

                if (TempObject6 != null)
                {
                    StaticHelperClass.SetClassItemValue(modelitem.t_table.GetType(), modelitem.t_table, "opd_retreason_id",
                        ((t_OrderRetReason)((ModelItemClass)TempObject6).t_table).orr_1C);
                    //ReturnComment_bnd = "";
                    TempObject6 = null;
                }
                else
                {
                    StaticHelperClass.SetClassItemValue(modelitem.t_table.GetType(), modelitem.t_table, "opd_retreason_id","");
                }
                OnPropertyChanged("ReturnCategoty_bnd");
            } 
        }

        public string _returnComment;
        public string ReturnComment_bnd {
            get { return _returnComment; }
            set 
            {
                _returnComment = value;
                StaticHelperClass.SetClassItemValue(modelitem.t_table.GetType(), modelitem.t_table, "opd_rerreason_desc", _returnComment);
                //if (value != null && value != "")
                //{
                //    ReturnCategoty_bnd = "";
                //}
                OnPropertyChanged("ReturnComment_bnd");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }

            if (modelitem != null)
            {
                    modelitem.DBUpdate();  
            }
        }
        
        public int id { get; set; }
        public string group_name { get; set; }
        public int group_index { get; set; }

        public Group Group1 { get; set; }
        public Group Group2 { get; set; }
        public Group Group3 { get; set; }

       // public ModelItemClass modelitem { get; set; }

        private OutboxReturnViewModelItem()
        {
        }

        public static void MakeTempObjects()
        {
            if (TempObject == null)
            {
                TempObject = MakeTempObject(typeof(t_Nomenclature), "");
            }
            //if (TempObject1 == null)
            //{
            //    TempObject1 = MakeTempObject(typeof(t_Nome2Teremok), "");
            //}
            //if (TempObject2 == null)
            //{
            //    TempObject2 = MakeTempObject(typeof(t_DocTypeRef), "");
            //}
            if (TempObject3 == null)
            {
                TempObject3 = new Random(10);
            }
            if (TempObject4 == null)
            {
                TempObject4 = MakeTempObject(typeof(t_OrderRetReason), "");
            }
            if (TempObject5 == null && TempObject4!=null)
            {
                FormAddElement add_user_form;
                add_user_form = new FormAddElement();
                TempObject5 = add_user_form;

                add_user_form.wpfctl.txbl_Header.Text = "Выберите причину возврата";

                //передать ему список добавляемых номенклатур
                add_user_form.wpfctl.listW_MainList.ItemsSource = (TempObject4 as List<object>).OfType<t_OrderRetReason>().ToList().Select(a=>new ModelItemClass(a));
                add_user_form.wpfctl.listW_MainList.FontSize = 12;
                add_user_form.wpfctl.ReturnObject = (o) =>
                {
                    TempObject6 = (ModelItemClass)o;
                };
                add_user_form.wpfctl.listW_MainList.MouseDoubleClick += (s1, e1) =>
                {
                    if (add_user_form.wpfctl.listW_MainList.SelectedItem != null)
                    {
                        add_user_form.wpfctl.btn_add_Click(s1, e1);
                    }
                };
            }

        }
        OrderClass order;
        public static OutboxReturnViewModelItem Create(OrderClass order, ModelItemClass modelitem)
        {
            MakeTempObjects();

            ModelClass model = modelitem.t_table;
            

            OutboxReturnViewModelItem orvmi = new OutboxReturnViewModelItem();
            orvmi.order = order;
            orvmi.modelitem = modelitem;

            t_Order2ProdDetails o2pd=null;
            
            if(model is t_Order2ProdDetails){
                o2pd = model as t_Order2ProdDetails;
            }

            if (model is t_Nomenclature)
            {
                o2pd = (t_Order2ProdDetails)order.CreateOrderLineFromTemplate(model as t_Nomenclature).t_table;
            }

            if (model is t_Nome2Teremok)
            {
                o2pd = (t_Order2ProdDetails)order.CreateOrderLineFromTemplate(model as t_Nome2Teremok).t_table;
            }

            FillInfoFromDbLine(orvmi, o2pd);
            return orvmi;
        }

        private static void FillInfoFromDbLine(OutboxReturnViewModelItem orvmi, t_Order2ProdDetails o2pd)
        {
            if (o2pd == null) return;

            List<t_Nomenclature> templates;
            templates = (TempObject as List<object>).OfType<t_Nomenclature>().ToList();
            if (templates == null) return;

            orvmi.NomenclatureName_bnd = templates.WhereFirst(a =>
                    a.nome_id.Equals((o2pd).opd_nome_id)).nome_name;
            orvmi._quantity = o2pd.opd_order;
            orvmi.BazovieEdinici_bnd = o2pd.opd_bed;
            orvmi._returnComment = o2pd.opd_rerreason_desc;

            //соотнести к группе
            orvmi.SetGroup(o2pd);

            if (o2pd.opd_retreason_id != null && o2pd.opd_retreason_id != "")
            {
                List<t_OrderRetReason> retreasons = (TempObject4 as List<object>).OfType<t_OrderRetReason>().ToList();
                if (retreasons == null) return;

                orvmi._returnCategoty = retreasons.Where(a => a.orr_1C == o2pd.opd_retreason_id).First().orr_name;
            }
            else
            {
                o2pd.opd_retreason_id = "";
            }

            if (o2pd.opd_rerreason_desc != null && o2pd.opd_rerreason_desc != "")
            {
                orvmi._returnComment = o2pd.opd_rerreason_desc;
            }
            else
            {
                o2pd.opd_rerreason_desc = "";
            }

        }

        private void SetGroup(t_Order2ProdDetails o2pd)
        {
            var h = order.HDetailsDictionary[typeof(t_GroupRef)];
            if (h != null)
            {
                int idx = o2pd.group_id;
                int level = 1;
                SetGroupIdx(h, idx,level);
            }
            else
            {
                Group1 = new Group(0, "Без группы");
            }
        }

        private void SetGroupIdx(Hashtable h,int idx,int level)
        {
            if (level == 4) return;
            t_GroupRef hg = h[idx] as t_GroupRef;

            if (hg != null)
            {
                SetGroupIdx(h, hg.parent_id, level + 1);
                StaticHelperClass.SetClassItemPropValue(this, "Group" + level, new Group(hg.group_id, hg.group_name));
            }
            else
            {
                StaticHelperClass.SetClassItemPropValue(this, "Group" + level, new Group(0, "Без группы"));
            }


            //if (level == 4) return;
            //t_GroupRef hg = h[idx] as t_GroupRef;

            //if (hg != null)
            //{
            //    StaticHelperClass.SetClassItemPropValue(this, "Group" + level, new Group(hg.group_id, hg.group_name));
            //    if (hg.parent_id != 0)
            //    {
            //        level++;
            //        SetGroupIdx(h, hg.parent_id, level + 1);
            //    }
            //}
            //else
            //{
            //    StaticHelperClass.SetClassItemPropValue(this, "Group" + level, new Group(0, "Без группы"));
            //}
        }

        public void SetGroup()
        {
            //List<t_Nome2Teremok> nomegroups;
            //    nomegroups = (TempObject1 as List<t_Nome2Teremok>).Where(a=>a.n2t_nt_id!=order.CurrentDocument.doc_type_id).ToList();
            //    if (nomegroups == null) return null;
            //    List<t_DocTypeRef> doc_types;
            //    doc_types = TempObject2 as List<t_DocTypeRef>; if (doc_types == null) return null;


            //group_index = (TempObject3 as Random).Next(10);
            
            //group_name = "Группа " + group_index;

            //Group = new Group(group_index,group_name);
        }

        public object GetGroupTitle()
        {
            return group_name;
        }

        public static object TempObject = null;
        public static object TempObject1 = null;
        public static object TempObject2 = null;
        public static object TempObject3 = null;
        public static object TempObject4 = null;
        public static object TempObject5 = null;
        static object TempObject6 = null;

        public static void ReleaseTempObjects()
        {
            TempObject = null;
            TempObject1 = null;
            TempObject2 = null;
            TempObject3 = null;
            TempObject4 = null;
            TempObject5 = null;
            TempObject6 = null;
        }




























        
        //public string user_name { get; set; }
        //public v_Responsibility user_response { get; set; }
        //public List<v_Responsibility> user_response_list { get; set; }
        //public string user_plan { get; set; }
        //public SolidColorBrush user_plan_color { get; set; }
        //public string user_fact { get; set; }
        //public SolidColorBrush user_fact_color { get; set; }

        //public t_Marotch t_Marotch;
        //public string comment
        //{
        //    get
        //    {
        //        return t_Marotch.comment;
        //    }
        //    set
        //    {
        //        t_Marotch.comment = value;
        //        t_Marotch.Update();
        //    }
        //}
    }
}
