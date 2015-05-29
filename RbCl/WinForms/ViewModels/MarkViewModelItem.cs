using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes.DocumentClasses;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using RBClient.Classes.InternalClasses.Models;
using System.Drawing;
using RBClient.WPF.ViewModels;
using RBClient.Classes;

namespace RBClient.WinForms.ViewModels
{
    [Serializable]
    public class MarkViewModelItem : ViewModelItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private OrderClass order;

        private Hashtable rht<T>() where T:ModelClass
        {
            return order.HDetailsDictionary[typeof(T)];
        }

        public static MarkViewModelItem Createt_MI(OrderClass order, ModelItemClass modelitem)
        {
            try
            {
                t_MarkItems db_line = (t_MarkItems)modelitem.t_table;

                MarkViewModelItem mi = new MarkViewModelItem();
                mi.order = order;
                mi.modelitem = modelitem;

                if (db_line.mark_name != RBClient.FormMark2.Itog_class.itog_1C)
                {
                    mi.Id = db_line.mark_id;
                    mi._user_fio = ((t_Employee)mi.rht<t_Employee>()[db_line.mark_name]).employee_name;

                    if (db_line.mark_res != null)
                    {
                        if (mi.rht<t_Responsibility>().ContainsKey(db_line.mark_res))
                            mi._responsibility = ((t_Responsibility)mi.rht<t_Responsibility>()[db_line.mark_res]).res_guid;
                    }

                    mi._itog_vert = db_line.mark_total;
                }
                else
                {
                    mi._user_fio = RBClient.FormMark2.Itog_class.itog_name;
                }

                mi.RowId = db_line.mark_row_id;

                for (int i = 1; i <= DateTime.DaysInMonth(order.CurrentDocument.doc_datetime.Year,
                    order.CurrentDocument.doc_datetime.Month); i++)
                {
                    StaticHelperClass.SetClassItemValue(mi, "__" + i.ToString(),
                    StaticHelperClass.ReturnClassItemValue(db_line, "mark_" + i.ToString()));
                }
                mi.ComputeTotal();
                return mi;
            }catch(Exception ex)
            {
                MDIParentMain.Log("CreateMI error "+ex.Message);
                return null;
            }
        }

        public static MarkViewModelItem Createt_EMP(OrderClass order, ModelItemClass modelitem)
        {
            t_Employee db_line = (t_Employee)modelitem.t_table;

            MarkViewModelItem mi = new MarkViewModelItem();
            mi.order = order;

            t_MarkItems mrki = new t_MarkItems();
            mrki.mark_name = db_line.employee_1C;
            mrki.mark_doc_id = order.CurrentDocument.doc_id;

            mrki.mark_row_id = order.DetailsDictionary[typeof(t_MarkItems)].Count;
            mrki.Create();

            mi.modelitem = new ModelItemClass(mrki);

            return mi;
        }

        public static MarkViewModelItem Create(OrderClass order, ModelItemClass modelitem)
        {
            if (modelitem.t_table is t_MarkItems)
            {
                MarkViewModelItem mi = Createt_MI(order,modelitem);
                return mi;
            }
            if (modelitem.t_table is t_Employee)
            {
                MarkViewModelItem mi = Createt_EMP(order, modelitem);
                return mi;
            }

            return null;
        }


        private void ComputeTotal()
        {
            int smen = 0;
            double chasov=0;
            if (modelitem != null)
            {
                t_MarkItems mi = (t_MarkItems)modelitem.t_table;

                for (int i = 1; i <= DateTime.DaysInMonth(order.CurrentDocument.doc_datetime.Year,
                    order.CurrentDocument.doc_datetime.Month); i++)
                {
                    object o = StaticHelperClass.ReturnClassItemValue<t_MarkItems>(
                        mi, "mark_" + i.ToString());
                    if (o != null && ((string)o).Trim() != "")
                    {
                        smen++;
                        chasov += double.Parse(
                            (string)StaticHelperClass.ReturnClassItemValue<t_MarkItems>(
                        mi, "mark_work_" + i.ToString())
                            );
                    }
                }
                _itog_vert = smen.ToString() + " см. (" + chasov.ToString() + " ч.)";
                SetValueToModel("mark_total", _itog_vert);
                // Итого = smen.ToString() + " см. (" + chasov.ToString() + " ч.)";
            }
        }

        protected override void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }

            if (name.IndexOf("_") != -1)
            {
                ComputeTotal();
            }

            if (modelitem != null)
            {
                modelitem.DBUpdate(true);
            }
        }

        public int Id { get; set; }

        public int RowId = 0;
        public void ChangeRowIndex(int index)
        {
            RowId = index;
            SetValueToModel("mark_row_id", RowId);
            if (modelitem != null)
            {
                modelitem.DBUpdate(true);
            }
        }

        public string _user_fio;
        public string Сотрудник
        {
            get
            {
                return _user_fio;
            }
            set
            {
                _user_fio = value;
                OnPropertyChanged("Сотрудник");
            }
        }

        public string _responsibility;
        public string Обязанность
        {
            get
            {
                return _responsibility;
            }
            set
            {
                try
                {
                    _responsibility = value;
                    SetValueToModel("mark_res", _responsibility);

                    SetValueToModel("mark_office",
                        ((t_Responsibility)order.HDetailsDictionary[typeof(t_Responsibility)][_responsibility]).res_name);
                    OnPropertyChanged("Обязанность");
                }catch(Exception ex)
                {
                    _responsibility = "";
                }
            }
        }

        public string _itog_vert;
        public string Итого
        {
            get
            {
                return _itog_vert;
            }
            set
            {
                _itog_vert = value;
                OnPropertyChanged("Итого");
            }
        }

        private void SetValueToModel(string prop_name,object prop_value)
        {
            StaticHelperClass.SetClassItemValue(modelitem.t_table.GetType(),
                modelitem.t_table, prop_name, prop_value);
        }

        public const string DefaultStartTime="9:00";
        public const int DefaultStartTimeInt = 9;
        public const string DefaultEndTime = "23:00";
        public const int DefaultEndTimeInt = 23;

        public void EditSmenaCell(SmenaViewClass1 smena)
        {
            string day_str = smena.Day;
            string cell_value = smena.ShiftType.type_value +
                RbClientGlobalStaticMethods.ReturnIntOrDoubleFromDouble(smena.HoursCount,"F1");
            SetValueToModel("mark_" + smena.Day, cell_value);
            SetValueToModel("mark_guid_" + day_str, "");

            SetValueToModel("mark_firstime_" + day_str, smena.From);
            SetValueToModel("mark_lasttime_" + day_str, smena.To);

            SetValueToModel("mark_work_" + day_str,
                RbClientGlobalStaticMethods.ReturnIntOrDoubleFromDouble(smena.HoursCount, "F1"));
            SetValueToModel("mark_guidsmena_" + day_str, smena.ShiftType.type_guid);

            StaticHelperClass.SetClassItemValue(this.GetType(), this, "_" + day_str, cell_value);
        }

        private bool ValueIsClearCell(string cellValue)
        {
            if (cellValue == "Оч")
                return true;
            return false;
        }

        public void ClearAllSmenaCell()
        {
            var a = Enumerable.Range(1, 31);
            List<string> clearList = new List<string>();
            foreach (var day_str in a)
            {
                clearList.AddRange(new List<string>{
                "mark_" + day_str,
                "mark_work_" + day_str,
                "mark_guid_" + day_str,
                "mark_firstime_" + day_str,
                "mark_lasttime_" + day_str,
                "mark_guidsmena_" + day_str});

                StaticHelperClass.SetClassItemValue(this.GetType(), this, "_" + day_str, "");
            }
            if(clearList.NotNullOrEmpty())
            {
                modelitem.Clear(clearList);    
            }
            ComputeTotal();
        }

        public void ClearSmenaCell(string day_str)
        {
            modelitem.Clear(new List<string>{
                "mark_" + day_str,
                "mark_work_" + day_str,
                "mark_guid_" + day_str,
                "mark_firstime_" + day_str,
                "mark_lasttime_" + day_str,
                "mark_guidsmena_" + day_str});

            StaticHelperClass.SetClassItemValue(this.GetType(), this, "_" + day_str, "");
        }

        public void SetSmenaButtonToCell(t_ButtonTemplate tb,string day_str)
        {
            t_MarkItems mi = new t_MarkItems();

            if (ValueIsClearCell(tb.bnt_value))
            {
                ClearSmenaCell(day_str);
            }
            else
            {
                SetValueToModel("mark_" + day_str, tb.bnt_value);
                SetValueToModel("mark_work_" + day_str, tb.btn_hint);
                SetValueToModel("mark_guid_" + day_str, tb.btn_guid);
                string work_key = GetWorkKey(day_str);
                t_WorkTeremok twt = (t_WorkTeremok)order.HDetailsDictionary[typeof(t_WorkTeremok)][work_key];
                if (twt == null)
                {
                    SetWorkingTime(tb, day_str, DefaultStartTimeInt);
                }
                else
                {
                    SetWorkingTime(tb, day_str, twt.teremok_firstTime.Hour);
                }
                SetValueToModel("mark_guidsmena_" + day_str, tb.btn_SmenaType);
                StaticHelperClass.SetClassItemValue(this.GetType(), this, "_" + day_str, tb.bnt_value);
            }
        }

        private void SetWorkingTime(t_ButtonTemplate tb, string day_str, int StartTime)
        {
            int smena_hours=int.Parse(tb.btn_hint);
            int stop_time = StartTime + smena_hours;
            if (stop_time > 24)
            {
                SetValueToModel("mark_lasttime_" + day_str, TodbDateFormat(24));
                SetValueToModel("mark_firstime_" + day_str, TodbDateFormat(24-smena_hours));
            }
            else
            {
                SetValueToModel("mark_lasttime_" + day_str, TodbDateFormat(stop_time));
                SetValueToModel("mark_firstime_" + day_str, TodbDateFormat(smena_hours));
            }
        }

        public static string TodbDateFormat(int hours)
        {
            return hours.ToString() + ":00";
        }
        private string GetWorkKey(string day_str)
        {
            string key=StaticConstants.Teremok_ID+day_str
                +order.CurrentDocument.doc_datetime.Month.ToString()
                +order.CurrentDocument.doc_datetime.Year.ToString();
            return key;
        }

        #region числа
        public string __1;
        public string _1
        {
            get
            {
                return __1;
            }
            set
            {
                __1 = value;
                OnPropertyChanged("_1");
            }
        }
        public string __2;
        public string _2
        {
            get
            {
                return __2;
            }
            set
            {
                __2 = value;
                OnPropertyChanged("_2");
            }
        }
        public string __3;
        public string _3
        {
            get
            {
                return __3;
            }
            set
            {
                __3 = value;
                OnPropertyChanged("_3");
            }
        }
        public string __4;
        public string _4
        {
            get
            {
                return __4;
            }
            set
            {
                __4 = value;
                OnPropertyChanged("_4");
            }
        }
        public string __5;
        public string _5
        {
            get
            {
                return __5;
            }
            set
            {
                __5 = value;
                OnPropertyChanged("_5");
            }
        }
        public string __6;
        public string _6
        {
            get
            {
                return __6;
            }
            set
            {
                __6 = value;
                OnPropertyChanged("_6");
            }
        }
        public string __7;
        public string _7
        {
            get
            {
                return __7;
            }
            set
            {
                __7 = value;
                OnPropertyChanged("_7");
            }
        }
        public string __8;
        public string _8
        {
            get
            {
                return __8;
            }
            set
            {
                __8 = value;
                OnPropertyChanged("_8");
            }
        }
        public string __9;
        public string _9
        {
            get
            {
                return __9;
            }
            set
            {
                __9 = value;
                OnPropertyChanged("_9");
            }
        }
        public string __10;
        public string _10
        {
            get
            {
                return __10;
            }
            set
            {
                __10 = value;
                OnPropertyChanged("_10");
            }
        }
        public string __11;
        public string _11
        {
            get
            {
                return __11;
            }
            set
            {
                __11 = value;
                OnPropertyChanged("_11");
            }
        }
        public string __12;
        public string _12
        {
            get
            {
                return __12;
            }
            set
            {
                __12 = value;
                OnPropertyChanged("_12");
            }
        }
        public string __13;
        public string _13
        {
            get
            {
                return __13;
            }
            set
            {
                __13 = value;
                OnPropertyChanged("_13");
            }
        }
        public string __14;
        public string _14
        {
            get
            {
                return __14;
            }
            set
            {
                __14 = value;
                OnPropertyChanged("_14");
            }
        }
        public string __15;
        public string _15
        {
            get
            {
                return __15;
            }
            set
            {
                __15 = value;
                OnPropertyChanged("_15");
            }
        }
        public string __16;
        public string _16
        {
            get
            {
                return __16;
            }
            set
            {
                __16 = value;
                OnPropertyChanged("_16");
            }
        }
        public string __17;
        public string _17
        {
            get
            {
                return __17;
            }
            set
            {
                __17 = value;
                OnPropertyChanged("_17");
            }
        }
        public string __18;
        public string _18
        {
            get
            {
                return __18;
            }
            set
            {
                __18 = value;
                OnPropertyChanged("_18");
            }
        }
        public string __19;
        public string _19
        {
            get
            {
                return __19;
            }
            set
            {
                __19 = value;
                OnPropertyChanged("_19");
            }
        }
        public string __20;
        public string _20
        {
            get
            {
                return __20;
            }
            set
            {
                __20 = value;
                OnPropertyChanged("_20");
            }
        }
        public string __21;
        public string _21
        {
            get
            {
                return __21;
            }
            set
            {
                __21 = value;
                OnPropertyChanged("_21");
            }
        }
        public string __22;
        public string _22
        {
            get
            {
                return __22;
            }
            set
            {
                __22 = value;
                OnPropertyChanged("_22");
            }
        }
        public string __23;
        public string _23
        {
            get
            {
                return __23;
            }
            set
            {
                __23 = value;
                OnPropertyChanged("_23");
            }
        }
        public string __24;
        public string _24
        {
            get
            {
                return __24;
            }
            set
            {
                __24 = value;
                OnPropertyChanged("_24");
            }
        }
        public string __25;
        public string _25
        {
            get
            {
                return __25;
            }
            set
            {
                __25 = value;
                OnPropertyChanged("_25");
            }
        }
        public string __26;
        public string _26
        {
            get
            {
                return __26;
            }
            set
            {
                __26 = value;
                OnPropertyChanged("_26");
            }
        }
        public string __27;
        public string _27
        {
            get
            {
                return __27;
            }
            set
            {
                __27 = value;
                OnPropertyChanged("_27");
            }
        }
        public string __28;
        public string _28
        {
            get
            {
                return __28;
            }
            set
            {
                __28 = value;
                OnPropertyChanged("_28");
            }
        }
        public string __29;
        public string _29
        {
            get
            {
                return __29;
            }
            set
            {
                __29 = value;
                OnPropertyChanged("_29");
            }
        }
        public string __30;
        public string _30
        {
            get
            {
                return __30;
            }
            set
            {
                __30 = value;
                OnPropertyChanged("_30");
            }
        }
        public string __31;
        public string _31
        {
            get
            {
                return __31;
            }
            set
            {
                __31 = value;
                OnPropertyChanged("_31");
            }
        }
        #endregion


    }
}
