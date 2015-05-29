using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes.CustomWindows;
using System.Windows;

namespace RBClient.Classes.CustomClasses
{
    public static class ValidationErrors
    {
        
        public static readonly DependencyProperty InputErrorProperty =
           DependencyProperty.RegisterAttached(
           "InputError",
           typeof(bool),
           typeof(ValidationErrors),
           new UIPropertyMetadata(false, OnInputErrorPropertyChanged));


        public static readonly DependencyProperty StringErrorProperty =
           DependencyProperty.RegisterAttached(
           "StringError",
           typeof(object),
           typeof(ValidationErrors),
           new UIPropertyMetadata(false, OnStringErrorPropertyChanged));


        public static void OnInputErrorPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            
            //((TextBoxBase)o).SelectAll();
        }

        public static void OnStringErrorPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is bool))
                ShowError(e.NewValue.ToString());
            
        }

        public static object GetStringError(object treeViewItem)
        {
            return "";
        }

        public static void SetStringError(object treeViewItem, object value)
        {
            ShowError(value.ToString());
        }

        public static bool GetInputError(object treeViewItem)
        {
            return false;
        }

        public static void SetInputError(object treeViewItem, bool value)
        {
            ShowError();
        }

        public static bool InputError
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                {
                    ShowError();
                }
            }
        }
       
        public static void ShowError()
        {
            WpfCustomMessageBox.Show("Вы ввели неверное значение!!!");
        }

        public static void ShowError(string message)
        {
            WpfCustomMessageBox.Show(message);
        }
    }
}
