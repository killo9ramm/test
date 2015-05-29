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
using System.Windows.Controls.Primitives;
using RBClient.Classes;
using System.ComponentModel;

namespace RBClient.WPF.UserControls
{
    public partial class AutoCompleteTextBoxPop : UserControl
    {
        public List<string> ItemsList = new List<string>() { "1111111", "121211", "12121212", "22222111", "2221111" };
        private List<string> VisibleList;
        public string SelectedItem = null;

        public PassObject SelectionChanged;
        //public Action<object> SelectionChanged;


        public Popup pop;
        public ListBox ItemsSubListBox;

        public AutoCompleteTextBoxPop()
        {
            InitializeComponent();

            pop = new Popup();
            pop.StaysOpen = false;
            ItemsSubListBox = new ListBox();
            pop.Child = ItemsSubListBox;
            pop.IsOpen = false;
            pop.Width = txb_MainText.Width;
            //pop.MaxHeight = this.;

            pop.PlacementTarget = txb_MainText;
            pop.Placement = PlacementMode.Bottom;


            VisibleList = new List<string>();

            txb_MainText.PreviewKeyDown += TextBox_KDown;
            txb_MainText.KeyUp += TextBox_KeyDown;
            

            ItemsSubListBox.PreviewKeyDown += new KeyEventHandler(ItemsSubListBox_PreviewKeyDown);
            ItemsSubListBox.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ItemsSubListBox_PreviewMouseUp);
        }

        private void ItemsSubListBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ItemsSubListBox.SelectedItem != null)
            {
                pop.IsOpen = false;
                txb_MainText.Text = ItemsSubListBox.SelectedItem.ToString();
                if (SelectionChanged != null)
                {
                    SelectionChanged(ItemsSubListBox.SelectedItem);
                }
            }
        }
        private void ItemsSubListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if ((ItemsSubListBox.SelectedIndex == 0))
                {
                    e.Handled = true;
                    txb_MainText.Focus();
                }
            }
            if (e.Key == Key.Down)
            {
                if ((ItemsSubListBox.SelectedIndex == ItemsSubListBox.Items.Count - 1))
                {
                    e.Handled = true;
                }
            }
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                if (ItemsSubListBox.SelectedItem != null)
                {
                    pop.IsOpen = false;
                    txb_MainText.Text = ItemsSubListBox.SelectedItem.ToString();
                    if (SelectionChanged != null)
                    {
                        SelectionChanged(ItemsSubListBox.SelectedItem);
                    }
                }
            }
        }

        private void TextBox_KDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (ItemsSubListBox.Items.Count > 0)
                {
                    if ((ItemsSubListBox.SelectedItem == null))
                    {
                        ItemsSubListBox.SelectedIndex = VisibleList.Count - 1;
                    }
                    if (ItemsSubListBox.SelectedIndex != 0)
                    {
                        ListBoxItem lvi = ItemsSubListBox.ItemContainerGenerator.ContainerFromIndex(ItemsSubListBox.SelectedIndex) as ListBoxItem;
                        if (!lvi.IsFocused) lvi.Focus();
                    }
                }
            }
            if (e.Key == Key.Down)
            {
                if ((ItemsSubListBox.SelectedItem == null && VisibleList.Count > 0))
                {
                    ItemsSubListBox.SelectedItem = VisibleList[0];
                    ListBoxItem lvi = ItemsSubListBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                    if (!lvi.IsFocused) lvi.Focus();
                }
                else
                {
                    if (ItemsSubListBox.Items.Count > 0)
                    {
                        ListBoxItem lvi = ItemsSubListBox.ItemContainerGenerator.ContainerFromIndex(ItemsSubListBox.SelectedIndex) as ListBoxItem;
                        if (!lvi.IsFocused) lvi.Focus();
                    }
                }
                e.Handled=true;
                //try
                //{
                    
                //}
                //catch (Exception ex)
                //{
                //    pop.IsOpen = false;
                //    pop.IsOpen = true;

                //    ListBoxItem lvi = ItemsSubListBox.ItemContainerGenerator.ContainerFromIndex(ItemsSubListBox.SelectedIndex) as ListBoxItem;
                //    if (!lvi.IsFocused) lvi.Focus();

                //    pop = new Popup();
                //    ItemsSubListBox = new ListBox();
                //    pop.Child = ItemsSubListBox;
                    
                //    pop.Width = txb_MainText.Width;
                //    //pop.MaxHeight = this.;

                //    pop.PlacementTarget = txb_MainText;
                //    pop.Placement = PlacementMode.Bottom;

                //    ItemsSubListBox.ItemsSource = VisibleList;
                //    ItemsSubListBox.SelectedIndex = 0;

                //    ItemsSubListBox.PreviewKeyDown += new KeyEventHandler(ItemsSubListBox_PreviewKeyDown);
                //    ItemsSubListBox.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(ItemsSubListBox_PreviewMouseUp);

                //    pop.IsOpen = true;
                //    ListBoxItem lvi1 = ItemsSubListBox.ItemContainerGenerator.ContainerFromIndex(ItemsSubListBox.SelectedIndex) as ListBoxItem;
                //    if (!lvi1.IsFocused) lvi1.Focus();
                //}
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (/*e.Key!=Key.Back &&*/e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.RightShift && e.Key != Key.Home && e.Key != Key.LeftShift && e.Key != Key.End)
            {
                string text = txb_MainText.Text;
                VisibleList.Clear();// = new List<string>();
                ItemsList.ForEach(a =>
                {
                    if (a.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                    {
                        VisibleList.Add(a);
                    }
                });
                ItemsSubListBox.ItemsSource = VisibleList;
                if (VisibleList.Count > 0)
                {
                    
                    if (!pop.IsOpen) pop.IsOpen = true;

                    ICollectionView view = CollectionViewSource.GetDefaultView(VisibleList);
                    view.Refresh();
                }
                else
                {
                    pop.IsOpen = false;
                }
            }
            //if (e.Key == Key.Up)
            //{
            //    if ((ItemsSubListBox.SelectedItem == null))
            //    {
            //        ItemsSubListBox.SelectedIndex = VisibleList.Count - 1;
            //    }
            //    if (ItemsSubListBox.SelectedIndex != 0)
            //    {
            //        ListBoxItem lvi = ItemsSubListBox.ItemContainerGenerator.ContainerFromIndex(ItemsSubListBox.SelectedIndex) as ListBoxItem;
            //        if (!lvi.IsFocused) lvi.Focus();
            //    }
            //}
            //if (e.Key == Key.Down)
            //{
            //    if ((ItemsSubListBox.SelectedItem == null))
            //    {
            //        ItemsSubListBox.SelectedIndex = 0;
            //    }
            //    ListBoxItem lvi = ItemsSubListBox.ItemContainerGenerator.ContainerFromIndex(ItemsSubListBox.SelectedIndex) as ListBoxItem;
            //    if (!lvi.IsFocused) lvi.Focus();
            //}
        }

        public void CursorToEnd()
        {
            //var myTextBox = comboBox1.Template.FindName("PART_EditableTextBox", comboBox1) as TextBox;
            txb_MainText.CaretIndex = txb_MainText.Text.Length;
        }

        public void SelectToEnd()
        {
            //var myTextBox = comboBox1.Template.FindName("PART_EditableTextBox", comboBox1) as TextBox;
            txb_MainText.SelectAll();
        }
    }
}
