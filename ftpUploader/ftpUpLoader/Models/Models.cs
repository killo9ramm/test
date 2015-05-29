using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    class Models
    {
    }
    [Serializable]
    public class t_Kkm 
    {
        public Int32 id;
        public String in_folder;
        public String out_folder;
        public Boolean online;
        public DateTime lasttime_online;
        public String kkm_name;
        public String last_treport;
        public String last_check_num;
        public DateTime last_check_datetime;
        public Boolean workedToDay;
        public String last_zreport;
    }
}
