using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RBClient.Classes
{
    class MainProgressReport
    {
        private static MainProgressReport _instance=null;
        public static MainProgressReport Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainProgressReport();
                }
                return _instance;
            }
        }

        public MainProgressReport()
        {
        }

        public void ReportProgress(string message)
        {
            StaticConstants.UpdateMainInterface(StaticConstants.MainWindow, a =>
            {
                ((MDIParentMain)a).Label1.Text = message;
            });
        }

        public void ReportProgress(string message,int procent)
        {
            ReportProgress(procent);
            ReportProgress(message);
        }

        public void ReportProgress(int procent)
        {
            StaticConstants.UpdateMainInterface(StaticConstants.MainWindow, a =>
            {
                ((MDIParentMain)a).progressBarItem1.Value = procent;
            });
        }
    }
}
