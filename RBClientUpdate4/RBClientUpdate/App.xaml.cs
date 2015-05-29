using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using CustomLogger;

namespace RBClientUpdate
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        Logger log = new Logger();

        public virtual void Log(string message)
        {
            log.Log(message);
        }

        public virtual void Log(Exception ex, string message)
        {
            log.Log("Error " + ex.Message + " Message:" + message);
        }
        public void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Log(args.Exception,"An unexpected application exception occurred");

            //MessageBox.Show("An unexpected exception has occurred. Shutting down the application. Please check the log file for more details.");

            // Prevent default unhandled exception processing
            args.Handled = true;

            Environment.Exit(0);
        }
    }
}
