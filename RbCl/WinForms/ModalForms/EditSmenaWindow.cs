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
using RBClient.Classes.CustomTypes;

namespace RBClient.WinForms
{
    public partial class EditSmenaWindow : Form
    {
        public PassObject ReturnObject;
        public object ReturnedObject;
        Hashtable shiftsHashList;

        public EditSmenaWindow(Hashtable comboList)
        {
            InitializeComponent();
            shiftsHashList = comboList;
            comboBox1.DataSource = comboList.Values.OfType<t_ShiftType>().ToList();
        }
        SmenaViewClass SmenaCurrent;
        public void Show(t_MarkItems mi,string day)
        {
            SmenaCurrent = CreateSmena(mi, day);
            UnbindEvents();
            ShowInGui(SmenaCurrent);
            BindEvents();
            this.ShowDialog();
        }

        public void ShowInGui(SmenaViewClass sm)
        {
            txtbxFromMask.Text = sm.From.ToString();
            txtbxToMask.Text = sm.To.ToString();
            txtbxCountMask.Text = sm.HoursCount.ToString();

            if (sm.ShiftType!=null && comboBox1.Items.Contains(sm.ShiftType))
            {
                comboBox1.SelectedItem = sm.ShiftType;
            }
        }
        public int ReturnIntTime(string stFrom)
        {
            return int.Parse(stFrom.Substring(0, stFrom.IndexOf(":")));
        }

        public SmenaViewClass CreateSmena(t_MarkItems mi, string day)
        {
            SmenaViewClass sm=new SmenaViewClass();
            sm.ID=mi.mark_id;
            sm.Day = day;
            object o=StaticHelperClass.ReturnClassItemValue(mi, "mark_guidsmena_" + day);
            if ((string)o !="" && o!=null)/* null && o!="")*/ sm.ShiftType = (t_ShiftType)shiftsHashList[o];
            else
            {
                if (comboBox1.SelectedItem != null) sm.ShiftType = (t_ShiftType)comboBox1.SelectedItem;
            }

            string stFrom=(string)StaticHelperClass.ReturnClassItemValue(mi, "mark_first_time_" + day);
            if (stFrom.NotNullOrEmpty())    sm.From = ReturnIntTime(stFrom);
            else sm.From = MarkViewModelItem.DefaultStartTimeInt;

            string stTo = (string)StaticHelperClass.ReturnClassItemValue(mi, "mark_lasttime_" + day);
            if (stTo.NotNullOrEmpty()) sm.To = ReturnIntTime(stTo);
            else sm.To = MarkViewModelItem.DefaultEndTimeInt;

            sm.HoursCount = sm.To - sm.From;
            return sm;
        }

        private void button_plus_Click(object sender, EventArgs e)
        {
            SmenaCurrent.HoursCount++;
            txtbxCountMask.Text =
            (SmenaCurrent.HoursCount).ToString();
        }

        private void button_minus_Click(object sender, EventArgs e)
        {
            SmenaCurrent.HoursCount--;
            txtbxCountMask.Text =
            (SmenaCurrent.HoursCount).ToString();
        }


        private void txtbxFromMask_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            if (int.TryParse(txtbxFromMask.Text, out i))
            {
                Recount("f",i);
            }
        }
        private void txtbxCountMask_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            if (int.TryParse(txtbxCountMask.Text, out i))
            {
                Recount("c",i);
            }
        }

        private void Recount(string fl,int val)
        {
            UnbindEvents();
            if (fl == "f")
            {
                SmenaCurrent.From=val;
                if (SmenaCurrent.From + SmenaCurrent.HoursCount > 24)
                {
                    SmenaCurrent.From = 24 - SmenaCurrent.HoursCount;
                }
                SmenaCurrent.To = SmenaCurrent.From + SmenaCurrent.HoursCount;
                ShowInGui(SmenaCurrent);
            }
            if (fl == "c")
            {
                SmenaCurrent.HoursCount = val;
                if (SmenaCurrent.From + SmenaCurrent.HoursCount > 24)
                {
                    SmenaCurrent.HoursCount = 24 - SmenaCurrent.From;
                }
                SmenaCurrent.To = SmenaCurrent.From + SmenaCurrent.HoursCount;
                ShowInGui(SmenaCurrent);
            }
            BindEvents();
        }

        private void UnbindEvents()
        {
            txtbxFromMask.TextChanged -= txtbxFromMask_TextChanged;
            txtbxCountMask.TextChanged -= txtbxCountMask_TextChanged;
        }
        private void BindEvents()
        {
            txtbxFromMask.TextChanged+=txtbxFromMask_TextChanged;
            txtbxCountMask.TextChanged+=txtbxCountMask_TextChanged;
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
    public class SmenaViewClass
    {
        public int ID { get; set; }
        public t_ShiftType ShiftType { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int HoursCount { get; set; }
        public string Day { get; set; }
    }
    public class SmenaViewClass1
    {
        public int ID { get; set; }
        public t_ShiftType ShiftType { get; set; }
        public string From { get { return TimeInterval.From; } }
        public string To { get { return TimeInterval.To; } }

        public TimeInterval TimeInterval{ get; set;}

        public double HoursCount { get { return TimeInterval.Hours; } }
        public string Day { get; set; }
    }
}
