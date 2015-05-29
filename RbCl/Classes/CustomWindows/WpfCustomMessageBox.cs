using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace RBClient.Classes.CustomWindows
{
    class WpfCustomMessageBox
    {

        public static MessageBoxResult Show(string messageBoxText)
        {return MessageBox.Show(messageBoxText);}
       
        public static MessageBoxResult Show(string messageBoxText, string caption)
        {return MessageBox.Show(messageBoxText,caption);}

        public static MessageBoxResult Show(Window owner, string messageBoxText)
        { return MessageBox.Show(owner,messageBoxText); }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        { return MessageBox.Show(messageBoxText, caption,button); }
        
        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
        { return MessageBox.Show(owner,messageBoxText,caption); }
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        { return MessageBox.Show(messageBoxText, caption, button,icon); }
        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button)
        { return MessageBox.Show(owner, messageBoxText,caption,button); }
        
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        { return MessageBox.Show(messageBoxText, caption, button, icon,defaultResult); }
        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        { return MessageBox.Show(owner, messageBoxText,caption,button,icon); }
       
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        { return MessageBox.Show(messageBoxText, caption, button, icon, defaultResult,options); }
        
        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        { return MessageBox.Show(owner, messageBoxText, caption, button, icon,defaultResult); }
       
        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        { return MessageBox.Show(owner,messageBoxText, caption, button, icon, defaultResult,options); }
    }
}
