using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes.DocumentClasses;
using RBClient.Classes;
using System.Collections;
using RBClient.WinForms.ViewModels;

namespace RBClient.WinForms
{
    public partial class EditSmenaWindow1 : Form
    {
        public PassObject ReturnObject;
        public object ReturnedObject;
        Hashtable shiftsHashList;

        public EditSmenaWindow1(Hashtable comboList)
        {
            InitializeComponent();
            shiftsHashList = comboList;
            comboBox1.DataSource = comboList.Values.OfType<t_ShiftType>().ToList();
        }

        public EditSmenaWindow1(List<t_ShiftType> shList)
        {
            InitializeComponent();
            UpdateShiftList(shList);
        }

        public void UpdateShiftList(List<t_ShiftType> shList)
        {
            shiftsHashList = new Hashtable();
            shList.ForEach(a => shiftsHashList.Add(a.type_guid, a));
            comboBox1.DataSource = shList;
        }

        private void FillComboBox(IEnumerable<string> smena_guids)
        {
            List<t_ShiftType> ls = new List<t_ShiftType>();
            if (smena_guids.NotNullOrEmpty())
            {
                foreach (var a in smena_guids)
                {
                    var b = new t_ShiftType().SelectFirst<t_ShiftType>(
                        String.Format("type_guid='{0}' AND deleted<>-1", a));
                    if (b != null)
                        ls.Add(b);
                }
            }

            comboBox1.DataSource = ls;

        }

        SmenaViewClass1 SmenaCurrent;
        public void Show(t_MarkItems mi, string day, t_WorkTeremok t_w)
        {
            SmenaCurrent = CreateSmena(mi, day,t_w);
            ShowInGui(SmenaCurrent);
            this.ShowDialog();
        }

        public DialogResult Show(t_Marotch mi, t_WorkTeremok t_w)
        {
            SmenaCurrent = CreateSmena(mi,t_w);
            return this.ShowDialog();
        }

        internal void Show(t_MarkItems t_MarkItems, string day, t_WorkTeremok teremok_work_time, Hashtable SmenaFiltersGuids)
        {
            FillComboBox((IEnumerable<string>)SmenaFiltersGuids[t_MarkItems.mark_res]);
            SmenaCurrent = CreateSmena(t_MarkItems, day, teremok_work_time);
            ShowInGui(SmenaCurrent);
            this.ShowDialog();
        }

        private SmenaViewClass1 CreateSmena(t_Marotch mi,t_WorkTeremok t_w)
        {
            SmenaViewClass1 sm = new SmenaViewClass1();
            sm.ID = mi.id;

            string o = mi.responce_guid_fact;
            if (o != null && o != "")
            {
                sm.ShiftType = (t_ShiftType)shiftsHashList[mi.responce_guid_fact];
            }
            else
            {
                if (comboBox1.SelectedItem != null) sm.ShiftType = (t_ShiftType)comboBox1.SelectedItem;
            }

            sm.TimeInterval = new Classes.CustomTypes.TimeInterval();
            timeIntervalControl1.TimeInterval = sm.TimeInterval;

            string stFrom = (string)StaticHelperClass.ReturnClassItemValue(mi, "fact_work_from");
            if (stFrom.NotNullOrEmpty()) sm.TimeInterval.From = stFrom;
            else
            sm.TimeInterval.From = RbClientGlobalStaticMethods.getDefaultTimeFrom(t_w);
                //sm.TimeInterval.From = MarkViewModelItem.DefaultStartTime;
            

            string stTo = (string)StaticHelperClass.ReturnClassItemValue(mi, "fact_work_to");
            if (stTo.NotNullOrEmpty()) sm.TimeInterval.To = stTo;
            else sm.TimeInterval.To = RbClientGlobalStaticMethods.getDefaultTimeTo(t_w);//MarkViewModelItem.DefaultEndTime;

            return sm;
        }

        public SmenaViewClass1 CreateSmena(t_MarkItems mi, string day, t_WorkTeremok t_w)
        {
            SmenaViewClass1 sm = new SmenaViewClass1();
            sm.ID = mi.mark_id;
            sm.Day = day;
            object o = StaticHelperClass.ReturnClassItemValue(mi, "mark_guidsmena_" + day);
            if ((string)o != "" && o != null)/* null && o!="")*/ sm.ShiftType = (t_ShiftType)shiftsHashList[o];
            else
            {
                if (comboBox1.SelectedItem != null) sm.ShiftType = (t_ShiftType)comboBox1.SelectedItem;
            }
            sm.TimeInterval = new Classes.CustomTypes.TimeInterval();
            timeIntervalControl1.TimeInterval = sm.TimeInterval;

            string stFrom = (string)StaticHelperClass.ReturnClassItemValue(mi, "mark_firstime_" + day);
            if (stFrom.NotNullOrEmpty()) sm.TimeInterval.From = stFrom;
            else sm.TimeInterval.From = RbClientGlobalStaticMethods.getDefaultTimeFrom(t_w);

            string stTo = (string)StaticHelperClass.ReturnClassItemValue(mi, "mark_lasttime_" + day);
            if (stTo.NotNullOrEmpty()) sm.TimeInterval.To = stTo;
            else sm.TimeInterval.To = RbClientGlobalStaticMethods.getDefaultTimeTo(t_w);

            return sm;
        }

        public SmenaViewClass1 CreateSmena(t_MarkItems mi, string day)
        {
            SmenaViewClass1 sm = new SmenaViewClass1();
            sm.ID = mi.mark_id;
            sm.Day = day;
            object o = StaticHelperClass.ReturnClassItemValue(mi, "mark_guidsmena_" + day);
            if ((string)o != "" && o != null)/* null && o!="")*/ sm.ShiftType = (t_ShiftType)shiftsHashList[o];
            else
            {
                if (comboBox1.SelectedItem != null) sm.ShiftType = (t_ShiftType)comboBox1.SelectedItem;
            }
            sm.TimeInterval = new Classes.CustomTypes.TimeInterval();
            timeIntervalControl1.TimeInterval = sm.TimeInterval;

            string stFrom = (string)StaticHelperClass.ReturnClassItemValue(mi, "mark_firstime_" + day);
            if (stFrom.NotNullOrEmpty()) sm.TimeInterval.From = stFrom;
            else sm.TimeInterval.From = MarkViewModelItem.DefaultStartTime;

            string stTo = (string)StaticHelperClass.ReturnClassItemValue(mi, "mark_lasttime_" + day);
            if (stTo.NotNullOrEmpty()) sm.TimeInterval.To = stTo;
            else sm.TimeInterval.To = MarkViewModelItem.DefaultEndTime;

            return sm;
        }

        public void ShowInGui(SmenaViewClass1 sm)
        {
            if (sm.ShiftType!=null && comboBox1.Items.Contains(sm.ShiftType))
            {
                comboBox1.SelectedItem = sm.ShiftType;
            }
           
        }
        public int ReturnIntTime(string stFrom)
        {
            return int.Parse(stFrom.Substring(0, stFrom.IndexOf(":")));
        }

       


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SmenaCurrent != null)
            {
                if (comboBox1.SelectedItem != null)
                {
                    SmenaCurrent.ShiftType = (t_ShiftType)comboBox1.SelectedItem;
                }
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (SmenaCurrent != null)
            {
                if (ReturnObject != null)
                {
                    ReturnObject(SmenaCurrent);
                }
            }
            this.Close();
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
