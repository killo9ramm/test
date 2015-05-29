using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes.CustomClasses;

namespace RBClient.Classes
{
    class CAction
    {
        private CAction()
        {
        }

        string Name="";
        CustomAction ca;


        public static CAction CreateCA(Action act, string Name, int Tries,int Timeout)
        {
            CAction cac = CreateCA(act);
            cac.Name = Name;
            cac.ca.Tries = Tries;
            cac.ca.Timeout = Timeout;
            return cac;
        }

        public static CAction CreateCA(Action act, string Name,int Tries)
        {
            CAction cac = CreateCA(act);
            cac.Name = Name;
            cac.ca.Tries = Tries;
            return cac;
        }

        public static CAction CreateCA(Action act, string Name)
        {
            CAction cac = CreateCA(act);
            cac.Name = Name;
            return cac;
        }

        public static CAction CreateCA(Action act)
        {
            CAction cac = new CAction();
            cac.ca = new CustomAction(o => { act(); }, null);
            cac.ca.LogEvent = (oo) => { MDIParentMain.Log(oo.ToString()); };
            return cac;
        }
        
        public void Start()
        {
            _Start(ca);   
        }

        private void _Start(CustomAction ca)
        {
            ca.Start();
            if (!ca.IsSuccess())
            {
                String.Format("CAction <{0}> ended error", Name);
                foreach (var a in ca.EList.Keys)
                {
                    MDIParentMain.Log(String.Format("CAction <{0}> {1} error :{2}", new object[] { Name, a, ca.EList[a].Message }));
                }
            }
            else
            {
                String.Format("CAction <{0}> successful", Name);
            }
        }
    }
}
