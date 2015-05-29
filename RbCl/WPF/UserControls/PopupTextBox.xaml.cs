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

namespace RBClient.WPF.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PopupTextBox.xaml
    /// </summary>
    public partial class PopupTextBox : UserControl
    {
        public string ToolTipText = "Введите комментарий";
        public Popup pop;
        public PopupControl pc;
        public event TextChangedEventHandler MainTextChanged;

        //public readonly static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(PopupTextBox));
        public readonly static DependencyProperty TextProperty;

        public string Text
        {
            get
            {
                string value = (string)GetValue(PopupTextBox.TextProperty);
                if (value != null)
                {
                    return value;
                }
                return string.Empty;
            }
            set
            {

                if (value == null)
                {
                    throw new ArgumentNullException("Text");
                }
                SetValue(PopupTextBox.TextProperty, value);
                //MainText.SetValue(TextBox.TextProperty, value);
                MainText.Text = value;
            }
        }

        static PopupTextBox()
        {
            PopupTextBox.TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(PopupTextBox));
        }

        public PopupTextBox()
        {
            InitializeComponent();

            MainText.ToolTip = ToolTipText;
            //MainText.Text = ToolTipText;

            MainText.TextChanged += MainText_TextChanged;
            MainText.PreviewMouseUp += MainText_MouseUp;

            pc = new PopupControl();
            pc.txbl_Header.Text = "Введите комментарий";
            pop = new Popup();
            pop.MinHeight = 200;
            pop.MinWidth = 250;
            pop.PlacementTarget = this;
            pop.Placement = PlacementMode.Mouse;
            pop.Child = pc;

            pc.btn_add.Click += Ok_Button_Ckicked;
            pc.btn_rem.Click += Cancel_Button_Ckicked;

            
        }

        private void MainText_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PopupOpen();
        }

        public void PopupOpen()
        {
            //открыть окно попап с текст боксом и кнопкой
            pc.MainText.Text = Text;
            pop.IsOpen = true;
            pc.MainText.Focus();
        }

        private void MainText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainText.Text != ToolTipText)
            {
                MainText.ToolTip = MainText.Text;
                SetValue(PopupTextBox.TextProperty, MainText.Text);
                if (MainTextChanged != null)
                {
                    MainTextChanged(sender, e);
                }
            }
        }

        public void Ok_Button_Ckicked(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = false;
            MainText.Text = pc.MainText.Text;
        }
        public void Cancel_Button_Ckicked(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = false;
        }
    }
}
