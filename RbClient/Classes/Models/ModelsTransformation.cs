using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes.DocumentClasses;

namespace RBClient.Classes.InternalClasses.Models
{
    class ModelsTransformation
    {
        public static t_Order2ProdDetails Transform(t_Nome2Teremok model)
        {
            if (model == null) return null;
            t_Order2ProdDetails o2tpd = new t_Order2ProdDetails();
            o2tpd.opd_nome_id = model.n2t_nome_id;
            o2tpd.opd_ed = model.n2t_ed;
            o2tpd.opd_bed = model.n2t_bed;
            o2tpd.opd_K = model.n2t_K;
            o2tpd.opd_quota = model.n2t_Quota;
            o2tpd.opd_maxquota = model.n2t_maxquota;
            o2tpd.opd_ke = model.n2t_ke;
            o2tpd.group_id = model.group_id;

            return o2tpd;
        }
        public static t_MarkItems Transform(t_Employee model)
        {
            if (model == null) return null;
            t_MarkItems o2tpd = new t_MarkItems();
            o2tpd.mark_name = model.employee_1C;
            
            return o2tpd;
        }

        public static t_Nome2Teremok Transform(t_Nomenclature model,int doc_type_id,int teremok_id)
        {
            if (model == null || doc_type_id == 0 || teremok_id==0) return null;
            t_Nome2Teremok nome2terem = new t_Nome2Teremok().SelectFirst<t_Nome2Teremok>("n2t_nome_id=" + model.nome_id +
                " AND n2t_nt_id=" + doc_type_id + " AND n2t_teremok_id=" + teremok_id);
            return nome2terem;
        }

        public static T Transform<T>(ModelClass model) where T : ModelClass
        {
            //return Transform(Transform<t_Nome2Teremok>(model));


            return null;
        }
    }
}
