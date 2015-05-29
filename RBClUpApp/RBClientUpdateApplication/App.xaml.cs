using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using RBClientUpdateApplication.Properties;

namespace RBClientUpdateApplication
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }
    }

}
