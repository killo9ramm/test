using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient.Classes.InternalClasses.Models
{
   
				[Serializable]
public class MSysNameMap : ModelClass               
{
                   	public Guid gUID;    
                    	public Int32 id;    
                    	public String name;    
                    	public Byte[] nameMap;    
                    	public Int32 type;    
                    }
    [Serializable]			
public class MSysNavPaneGroupCategorie : ModelClass               
{
                   	public String filter;    
                    	public Int32 flags;    
                    	public Int32 id;    
                    	public String name;    
                    	public Int32 position;    
                    	public Int32 selectedObjectID;    
                    	public Int32 type;    
                    }

        [Serializable]		
public class MSysNavPaneGroupToObject : ModelClass               
{
                   	public Int32 flags;    
                    	public Int32 groupID;    
                    	public Int32 icon;    
                    	public Int32 id;    
                    	public String name;    
                    	public Int32 objectID;    
                    	public Int32 position;    
                    }
        [Serializable]		
public class MSysNavPaneObjectID : ModelClass               
{
                   	public Int32 id;    
                    	public String name;    
                    	public Int32 type;    
                    }

        [Serializable]		
public class MSysResource : ModelClass               
{
                   	public String data;    
                    	public String extension;    
                    	public Int32 id;    
                    	public String name;    
                    	public String type;    
                    }
                [Serializable]
public class t_ButtonTemplate : ModelClass               
{
                   	public Int32 id;    
                    	public String btn_guid;    
                    	public String btn_name;    
                    	public String bnt_value;    
                    	public String btn_color;    
                    	public String btn_hint;    
                    	public Boolean btn_IsUsed;    
                    	public Int32 btn_Order;    
                    	public String btn_SmenaType;    
                    	public Boolean deleted;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
        [Serializable]		
public class t_Check : ModelClass               
{
                   	public Int32 check_id;    
                    	public Int32 check_z_id;    
                    	public Decimal check_amount;    
                    	public DateTime check_datetime;
                        public String check_num;
                        public override string ToString()
                        {
                            return base.ToString();
                        }   
                    }
                [Serializable]
public class t_CheckCurrent : ModelClass               
{
                   	public Int32 cc_id;    
                    	public Int32 cc_menu_id;    
                    	public Decimal cc_amount1;    
                    	public Decimal cc_amount2;    
                    	public Int32 cc_count;    
                    	public DateTime cc_datetime;
                        public String cc_KKM;
                        public override string ToString()
                        {
                            return base.ToString();
                        }    
                    }
        [Serializable]		
public class t_CheckItem : ModelClass               
{
                   	public Int32 ch_id;    
                    	public Int32 ch_check_id;    
                    	public Int32 ch_menu_id;    
                    	public Decimal ch_amount1;    
                    	public Decimal ch_amount2;    
                    	public Int32 ch_count;    
                    	public Int32 ch_check_doc_id;
                        public DateTime ch_datetime;
                        public override string ToString()
                        {
                            return base.ToString();
                        }    
                    }
    [Serializable]			
public class t_Conf : ModelClass               
{
                   	public Int32 conf_id;    
                    	public String conf_param;    
                    	public String conf_value;
                        public String conf_dep;
                        public override string ToString()
                        {
                            return base.ToString();
                        }   
                    }
            [Serializable]	
public class t_Counts_Service : ModelClass               
{
                   	public Int32 id;    
                    	public Int32 doc_type_id;    
                    	public Int32 count_length;    
                    	public String nome_1C;    
                    	public String count_name;    
                    	public String count_mask;
                        public Int32 count_koeff;
                        public override string ToString()
                        {
                            return base.ToString();
                        }  
                    }
    [Serializable]			
public class t_Dinner : ModelClass               
{
                   	public Int32 id;    
                    	public Int32 doc_id;    
                    	public String nome_name;    
                    	public Int32 nome_id;    
                    	public String kkm_code;    
                    	public Int32 employee_id;    
                    	public String emploee_name;    
                    	public Decimal d_sum;
                        public DateTime d_date;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }

    [Serializable]	
    public class t_Kkm : ModelClass
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

            [Serializable]	
public class t_Dinner2t_Menu : ModelClass               
{
                   	public Int32 id;    
                    	public Int32 dinner_id;    
                    	public Int32 menu_item_id;    
                    	public Int32 item_quantity;
                        public Decimal item_price;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
[Serializable]				
public class t_Doc : ModelClass               
{
                   	public Int32 doc_id;    
                    	public Int32 doc_type_id;    
                    	public DateTime doc_datetime;    
                    	public Int32 doc_status_id;    
                    	public String doc_desc;    
                    	public Int32 doc_teremok_id;    
                    	public Int32 doc_teremok2_id;    
                    	public String doc_1C;    
                    	public String doc_menu_dep;    
                    	public String doc_guid;
                        public String doc_hash;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }

        [Serializable]		
public class t_DocReestr : ModelClass               
{
                   	public String t_doc_release;    
                    	public String t_doc_switch;    
                    	public String t_doc_filename;    
                    	public String t_doc_screenName;
                        public String t_doc_category;
                        public override string ToString()
                        {
                            return base.ToString();
                        } 
                    }
        [Serializable]		
public class t_DocStatusRef : ModelClass               
{
                   	public Int32 docstatusref_id;    
                    	public String docstatusref_name;    
                    	public Int32 doctype_id;
                        public Int32 statustype_id;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
    [Serializable]
public class t_DocSendTypeRef : ModelClass
{
    public Int32 id;
    public Int32 sendtype_type;
    public String sendtype_description;
    public override string ToString()
    {
        return base.ToString();
    }
}
    [Serializable]
public class t_DocTypeRef : ModelClass               
{
                   	public Int32 doctype_id;    
                    	public String doctype_name;
                        public Int32 sendtype_type;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }

    [Serializable]
    public class t_Inkass : ModelClass
    {
        public Int32 id;
        public String ink_name;
        public Int32 ink_doc_id;
        public Decimal sum1;
        public Decimal sum2;
        public Decimal sum3;
        public String ink_type;
        public String ink_currency;
        public Boolean istemplate;
        public String comment;
    }

     [Serializable]
    public class t_Inkass_TypeRef : ModelClass
    {
        public Int32 id;
        public String ri_guid;
        public String ri_name;
        public Int32 ri_order;
        public Boolean deleted;
    }

        [Serializable]		
public class t_Employee : ModelClass               
{
                   	public Int32 employee_id;    
                    	public String employee_name;    
                    	public String employee_1C;    
                    	public String employee_FunctionName;    
                    	public String employee_FunctionGuid;    
                    	public Boolean employee_WorkField;
                        public Boolean deleted;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
    [Serializable]			
public class t_Invent : ModelClass               
{
                   	public Int32 invent_id;    
                    	public Int32 inv_doc_id;    
                    	public Int32 inv_nome_id;    
                    	public Decimal inv_quantity;    
                    	public String inv_desc;    
                    	public String inv_ed;    
                    	public String inv_bed;    
                    	public Decimal inv_K;
                        public Decimal inv_quantity2;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
				
    [Serializable]
public class t_MarkItems : ModelClass               
{
                   	public Int32 mark_id;    
                    	public Int32 mark_row_id;    
                    	public Int32 mark_doc_id;    
                    	public String mark_name;    
                    	public String mark_office;    
                    	public String mark_total;    
                    	public String mark_1;    
                    	public String mark_2;    
                    	public String mark_3;    
                    	public String mark_4;    
                    	public String mark_5;    
                    	public String mark_6;    
                    	public String mark_7;    
                    	public String mark_8;    
                    	public String mark_9;    
                    	public String mark_10;    
                    	public String mark_11;    
                    	public String mark_12;    
                    	public String mark_13;    
                    	public String mark_14;    
                    	public String mark_15;    
                    	public String mark_16;    
                    	public String mark_17;    
                    	public String mark_18;    
                    	public String mark_19;    
                    	public String mark_20;    
                    	public String mark_21;    
                    	public String mark_22;    
                    	public String mark_23;    
                    	public String mark_24;    
                    	public String mark_25;    
                    	public String mark_26;    
                    	public String mark_27;    
                    	public String mark_28;    
                    	public String mark_29;    
                    	public String mark_30;    
                    	public String mark_31;    
                    	public String mark_guid_1;    
                    	public String mark_guid_2;    
                    	public String mark_guid_3;    
                    	public String mark_guid_4;    
                    	public String mark_guid_5;    
                    	public String mark_guid_6;    
                    	public String mark_guid_7;    
                    	public String mark_guid_8;    
                    	public String mark_guid_9;    
                    	public String mark_guid_10;    
                    	public String mark_guid_11;    
                    	public String mark_guid_12;    
                    	public String mark_guid_13;    
                    	public String mark_guid_14;    
                    	public String mark_guid_15;    
                    	public String mark_guid_16;    
                    	public String mark_guid_17;    
                    	public String mark_guid_18;    
                    	public String mark_guid_19;    
                    	public String mark_guid_20;    
                    	public String mark_guid_21;    
                    	public String mark_guid_22;    
                    	public String mark_guid_23;    
                    	public String mark_guid_24;    
                    	public String mark_guid_25;    
                    	public String mark_guid_26;    
                    	public String mark_guid_27;    
                    	public String mark_guid_28;    
                    	public String mark_guid_29;    
                    	public String mark_guid_30;    
                    	public String mark_guid_31;    
                    	public String mark_firstime_1;    
                    	public String mark_firstime_2;    
                    	public String mark_firstime_3;    
                    	public String mark_firstime_4;    
                    	public String mark_firstime_5;    
                    	public String mark_firstime_6;    
                    	public String mark_firstime_7;    
                    	public String mark_firstime_8;    
                    	public String mark_firstime_9;    
                    	public String mark_firstime_10;    
                    	public String mark_firstime_11;    
                    	public String mark_firstime_12;    
                    	public String mark_firstime_13;    
                    	public String mark_firstime_14;    
                    	public String mark_firstime_15;    
                    	public String mark_firstime_16;    
                    	public String mark_firstime_17;    
                    	public String mark_firstime_18;    
                    	public String mark_firstime_19;    
                    	public String mark_firstime_20;    
                    	public String mark_firstime_21;    
                    	public String mark_firstime_22;    
                    	public String mark_firstime_23;    
                    	public String mark_firstime_24;    
                    	public String mark_firstime_25;    
                    	public String mark_firstime_26;    
                    	public String mark_firstime_27;    
                    	public String mark_firstime_28;    
                    	public String mark_firstime_29;    
                    	public String mark_firstime_30;    
                    	public String mark_firstime_31;    
                    	public String mark_lasttime_1;    
                    	public String mark_lasttime_2;    
                    	public String mark_lasttime_3;    
                    	public String mark_lasttime_4;    
                    	public String mark_lasttime_5;    
                    	public String mark_lasttime_6;    
                    	public String mark_lasttime_7;    
                    	public String mark_lasttime_8;    
                    	public String mark_lasttime_9;    
                    	public String mark_lasttime_10;    
                    	public String mark_lasttime_11;    
                    	public String mark_lasttime_12;    
                    	public String mark_lasttime_13;    
                    	public String mark_lasttime_14;    
                    	public String mark_lasttime_15;    
                    	public String mark_lasttime_16;    
                    	public String mark_lasttime_17;    
                    	public String mark_lasttime_18;    
                    	public String mark_lasttime_19;    
                    	public String mark_lasttime_20;    
                    	public String mark_lasttime_21;    
                    	public String mark_lasttime_22;    
                    	public String mark_lasttime_23;    
                    	public String mark_lasttime_24;    
                    	public String mark_lasttime_25;    
                    	public String mark_lasttime_26;    
                    	public String mark_lasttime_27;    
                    	public String mark_lasttime_28;    
                    	public String mark_lasttime_29;    
                    	public String mark_lasttime_30;    
                    	public String mark_lasttime_31;    
                    	public String mark_work_1;    
                    	public String mark_work_2;    
                    	public String mark_work_3;    
                    	public String mark_work_4;    
                    	public String mark_work_5;    
                    	public String mark_work_6;    
                    	public String mark_work_7;    
                    	public String mark_work_8;    
                    	public String mark_work_9;    
                    	public String mark_work_10;    
                    	public String mark_work_11;    
                    	public String mark_work_12;    
                    	public String mark_work_13;    
                    	public String mark_work_14;    
                    	public String mark_work_15;    
                    	public String mark_work_16;    
                    	public String mark_work_17;    
                    	public String mark_work_18;    
                    	public String mark_work_19;    
                    	public String mark_work_20;    
                    	public String mark_work_21;    
                    	public String mark_work_22;    
                    	public String mark_work_23;    
                    	public String mark_work_24;    
                    	public String mark_work_25;    
                    	public String mark_work_26;    
                    	public String mark_work_27;    
                    	public String mark_work_28;    
                    	public String mark_work_29;    
                    	public String mark_work_30;    
                    	public String mark_work_31;    
                    	public String mark_guidsmena_1;    
                    	public String mark_guidsmena_2;    
                    	public String mark_guidsmena_3;    
                    	public String mark_guidsmena_4;    
                    	public String mark_guidsmena_5;    
                    	public String mark_guidsmena_6;    
                    	public String mark_guidsmena_7;    
                    	public String mark_guidsmena_8;    
                    	public String mark_guidsmena_9;    
                    	public String mark_guidsmena_10;    
                    	public String mark_guidsmena_11;    
                    	public String mark_guidsmena_12;    
                    	public String mark_guidsmena_13;    
                    	public String mark_guidsmena_14;    
                    	public String mark_guidsmena_15;    
                    	public String mark_guidsmena_16;    
                    	public String mark_guidsmena_17;    
                    	public String mark_guidsmena_18;    
                    	public String mark_guidsmena_19;    
                    	public String mark_guidsmena_20;    
                    	public String mark_guidsmena_21;    
                    	public String mark_guidsmena_22;    
                    	public String mark_guidsmena_23;    
                    	public String mark_guidsmena_24;    
                    	public String mark_guidsmena_25;    
                    	public String mark_guidsmena_26;    
                    	public String mark_guidsmena_27;    
                    	public String mark_guidsmena_28;    
                    	public String mark_guidsmena_29;    
                    	public String mark_guidsmena_30;    
                    	public String mark_guidsmena_31;
                        public String mark_res;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
        [Serializable]		
public class t_Marotch : ModelClass               
{
                   	public Int32 id;    
                    	public Int32 doc_id;    
                    	public Int32 employee_id;    
                    	public String response;    
                    	public String responce_guid_plan;    
                    	public String responce_guid_fact;    
                    	public String m_plan;    
                    	public String m_fact;    
                    	public DateTime m_date;
                        public String comment;
                        public String fact_work_from;
                        public String fact_work_to;
                        public override string ToString()
                        {
                            return base.ToString();
                        }

}
    [Serializable]
        public class t_SerializationHip : ModelClass
        {
            public Int32 id;
            public Int32 object_id;
            public String object_name;
            public String object_type;
            public Byte[] object_data;
            public Int32 related_doc_id;
            public DateTime date_added;
            public Boolean isvalid;
            public String description;
            public override string ToString()
            {
                return base.ToString();
            }
        }

    [Serializable]
public class t_Menu : ModelClass               
{
                   	public Int32 menu_id;    
                    	public String menu_nome_1C;    
                    	public String menu_nome;    
                    	public Boolean menu_default;
                        public Decimal price;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
        [Serializable]		
public class t_MenuPrice : ModelClass               
{
                   	public Int32 mp_id;    
                    	public Int32 mp_doc_id;    
                    	public Int32 mp_menu_id;    
                    	public Decimal mp_price;
                        public DateTime mp_date;
                        public override string ToString()
                        {
                            return base.ToString();
                        }  
                    }
            [Serializable]	
public class t_NewDoc : ModelClass               
{
                   	public Int32 id;    
                    	public String t_filename;
                        public String t_screenName;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
[Serializable]				
public class t_Nome2Teremok : ModelClass               
{
                   	public Int32 n2t_id;    
                    	public Int32 n2t_teremok_id;    
                    	public Int32 n2t_nome_id;    
                    	public Int32 n2t_nt_id;    
                    	public String n2t_ed;    
                    	public String n2t_bed;    
                    	public Decimal n2t_K;    
                    	public Decimal n2t_Quota;    
                    	public Decimal n2t_maxquota;    
                    	public Decimal n2t_ke;    
                    	public String n2t_ze;    
                    	public Decimal n2t_bold;
                        public Decimal n2t_ban;
                        public Int32 group_id;    
                        public override string ToString()
                        {
                            return base.ToString();
                        } 
                    }
    [Serializable]
    public class t_GroupRef : ModelClass
    {
        public Int32 id;
        public String group_name;
        public Int32 doc_type_id;
        public Int32 group_id;
        public Int32 parent_id;
        public Boolean deleted;
        public override string ToString()
        {
            return base.ToString();
        }
    }
    [Serializable]			
public class t_NomeDinner : ModelClass               
{
                   	public Int32 nomedinner_id;    
                    	public String nomedinner_name;
                        public String nomedinner_1C;
                        public override string ToString()
                        {
                            return base.ToString();
                        } 
                    }
[Serializable]				
public class t_Nomenclature : ModelClass               
{
                   	public Int32 nome_id;    
                    	public String nome_1C;    
                    	public String nome_name;    
                    	public String nome_ed;    
                    	public String nome_bed;
                        public Decimal nome_K;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
[Serializable]				
public class t_NomeTemplate : ModelClass               
{
                   	public Int32 nt_id;
                    public String nt_desc;
                    public override string ToString()
                    {
                        return base.ToString();
                    } 
                    }
[Serializable]				
public class t_Order2ProdDetails : ModelClass               
{
                   	public Int32 opd_id;    
                    	public Int32 opd_doc_id;    
                    	public Int32 opd_nome_id;    
                    	public Decimal opd_order;    
                    	public Decimal opd_order2;    
                    	public String opd_ed;    
                    	public String opd_bed;    
                    	public Decimal opd_K;    
                    	public Decimal opd_quota;    
                    	public String opd_retreason_id;    
                    	public String opd_rerreason_desc;    
                    	public Decimal opd_order3;    
                    	public Decimal opd_maxquota;    
                    	public Decimal opd_total;    
                    	public String opd_ze;    
                    	public Decimal opd_ke;
                        public Decimal opd_bold;
                        public Int32 group_id;    
                        public override string ToString()
                        {
                            return base.ToString();
                        } 
                    }
[Serializable]				
public class t_OrderRetReason : ModelClass               
{
                   	public Int32 orr_id;    
                    	public String orr_1C;
                        public String orr_name;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
[Serializable]				
public class t_PropValue : ModelClass               
{
                   	public Int32 id;    
                    	public String prop_name;    
                    	public String prop_value;
                        public String prop_type;
                        public String add_prop1;
                        public String add_prop2;
                        public override string ToString()
                        {
                            return base.ToString();
                        } 
                    }
[Serializable]				
public class t_Responsibility : ModelClass               
{
                   	public Int32 id;    
                    	public String res_guid;    
                    	public String res_name;
                        public Boolean deleted;
                        public override string ToString()
                        {
                            return base.ToString();
                        }  
                    }
[Serializable]				
public class t_Shifts_Allowed : ModelClass
{
    public Int32 id;
    public String res_guid;
    public String shift_guid;
}

[Serializable]				
public class t_SettingsDoc : ModelClass               
{
                   	public Int32 id;    
                    	public Int32 column_doc_id;    
                    	public Int32 column_id;    
                    	public Boolean column_status;
                        public String column_name;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
[Serializable]				
public class t_ShiftType : ModelClass               
{
                   	public Int32 id;    
                    	public String type_guid;    
                    	public String type_name;    
                    	public String type_value;    
                    	public String type_color;
                        public Boolean deleted;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
[Serializable]				
public class t_Ship : ModelClass               
{
                   	public Int32 ship_id;    
                    	public Int32 ship_doc_id;    
                    	public DateTime ship_datetime;    
                    	public String ship_fio;    
                    	public String ship_tel;    
                    	public Int32 ship_steet;    
                    	public String ship_house;    
                    	public String ship_room;
                        public Int32 ship_addrees_id;
                        public override string ToString()
                        {
                            return base.ToString();
                        }  
                    }
[Serializable]				
public class t_ShipAddre : ModelClass               
{
                   	public Int32 sa_id;    
                    	public String sa_address;
                        public String sa_comment;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
[Serializable]				
public class t_ShipOrder : ModelClass               
{
                   	public Int32 so_id;    
                    	public Int32 so_ship_id;    
                    	public Int32 so_menu_id;    
                    	public Int32 so_menu_price;
                        public Int32 so_menu_quantiry;
                        public override string ToString()
                        {
                            return base.ToString();
                        }    
                    }
    [Serializable]			
public class t_Standart : ModelClass               
{
                   	public String t_name;    
                    	public String t_path_name;    
                    	public String t_hash_code;
                        public String t_creation_time;
                        public override string ToString()
                        {
                            return base.ToString();
                        }   
                    }
[Serializable]				
public class t_StatusType : ModelClass               
{
                   	public Int32 status_type_id;    
                    	public String status_type_name;
                        public String status_type_color;
                        public override string ToString()
                        {
                            return base.ToString();
                        }  
                    }
[Serializable]				
public class t_Table_Service : ModelClass               
{
                   	public Int32 id;    
                    	public Int32 doc_id;
                        public String doc_period;
                        public override string ToString()
                        {
                            return base.ToString();
                        }    
                    }
[Serializable]				
public class t_TaskExchange : ModelClass               
{
                   	public Int32 task_id;    
                    	public Int32 task_doc_id;    
                    	public DateTime task_datetime;    
                    	public Int32 task_state_id;
                        public DateTime task_datetime_completed;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
[Serializable]
public class t_InfoMessage : ModelClass
{
    public Int32 id;
    public Int32 dayofweek;
    public Int32 dayofmounth;
    public DateTime timefrom;
    public DateTime timeto;
    public Int32 doc_type_id;
    public Int32 type;
    public String department;
    public Int32 doc_id;
    public Int32 priority;
    public Boolean lastmonthday;
    public String description;
    public String message;
    public String i_guid;   
    public override string ToString()
    {
        return base.ToString();
    }
}
[Serializable]
public class t_TaskExchangeWeb : ModelClass
{
    public Int32 id;
    public Int32 teremok_id;
    public Int32 doc_id;
    public Byte[] doc_content;
    public override string ToString()
    {
        return base.ToString();
    }
}
[Serializable]				
public class t_Teremok : ModelClass               
{
                   	public Int32 teremok_id;    
                    	public String teremok_1C;    
                    	public String teremok_name;    
                    	public String teremok_1CID;    
                    	public Boolean teremok_current;    
                    	public Int32 teremok_dep;    
                    	public DateTime teremok_first_time;    
                    	public DateTime teremok_last_time;    
                    	public String teremok_guid;
                        public Boolean deleted;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
[Serializable]				
public class t_Utilization : ModelClass               
{
                   	public Int32 util_id;    
                    	public Int32 util_nome_id;    
                    	public Int32 util_quantity;    
                    	public Int32 util_doc_id;    
                    	public Int32 util_reason_id;    
                    	public String util_ed;    
                    	public String util_bed;    
                    	public Int32 util_K;
                        public Int32 util_opd_id;
                        public override string ToString()
                        {
                            return base.ToString();
                        }   
                    }
    [Serializable]			
public class t_UtilReasonRef : ModelClass               
{
                   	public Int32 urr_id;    
                    	public String urr_name;    
                    	public String nome_include;    
                    	public String doc_type;
                        public String id_1c;
                        public override string ToString()
                        {
                            return base.ToString();
                        } 
                    }
[Serializable]				
public class t_WorkOther : ModelClass               
{
                   	public Int32 id;    
                    	public String guid_emp;    
                    	public String guid_shift;    
                    	public Int32 day_short;    
                    	public Int32 value_time;    
                    	public DateTime first_time;
                        public DateTime last_time;
                        public override string ToString()
                        {
                            return base.ToString();
                        }   
                    }
[Serializable]				
public class t_WorkTeremok : ModelClass               
{
                   	public Int32 id;    
                    	public String teremok_id;    
                    	public String teremok_day;    
                    	public String teremok_month;    
                    	public String teremok_year;    
                    	public DateTime teremok_firstTime;
                        public DateTime teremok_lastTime;
                        public override string ToString()
                        {
                            return base.ToString();
                        }    
                    }
[Serializable]				
public class t_ZReport : ModelClass               
{
                   	public Int32 z_id;    
                    	public String z_num;    
                    	public String z_file;    
                    	public DateTime z_date;    
                    	public String z_kkm_id;    
                    	public Decimal z_total;    
                    	public Decimal z_total_return;    
                    	public Int32 z_doc_id;    
                    	public String z_kkm;    
                    	public String z_fr;    
                    	public Decimal z_cash;    
                    	public Decimal z_cash_return;    
                    	public Decimal z_card;    
                    	public Decimal z_card_return;    
                    	public Decimal z_cupon;    
                    	public Decimal z_cupon_return;    
                    	public Decimal z_dinner;
                        public Decimal z_dinner_return;
                        public override string ToString()
                        {
                            return base.ToString();
                        }
                    }
            [Serializable]				
        public class t_WebServiceTask : ModelClass
        {
            public Int32 id;
            public DateTime taskdate;
            public DateTime taskupdatedate;
            public String funcname;    
            public Boolean succed;
            public Boolean failed;
            public String description;
            public Int32 priority;
            public String servicename;
            public Int32 servicepriority;
            public Byte[] taskparams;
            public Int32 triescount;
            public Int32 related_doc_id;
        }

            //[Serializable]
            //public class t_test : ModelClass
            //{
            //    public Int32 id;
            //    public Byte[] testole;
            //}
}

