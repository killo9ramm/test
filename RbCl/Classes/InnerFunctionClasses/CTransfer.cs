using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using System.Data.OleDb;

using System.IO;
using System.IO.Compression;

using System.Windows.Forms;
using System.Diagnostics;

using System.Xml;
using System.Xml.Linq;
using Common.Logging;
using RBClient.Classes.Utilization;
using RBClient.Classes;
using System.Text.RegularExpressions;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes.CustomClasses;

namespace RBClient
{
    class CTransfer
    {
        private static ILog log = LogManager.GetCurrentClassLogger();



        internal void InsertGroupRefrence(FileInfo _file, string type)
        {
            List<t_GroupRef> _list = ParseGroupXml(_file);
            if (_list.NotNullOrEmpty())
            {
                
                    new t_GroupRef().UpdateSpravAddOrUpdate<t_GroupRef>(
                    _list,
                     b => b.doc_type_id.ToString() + b.group_id,
                     b => b.doc_type_id.ToString() + b.group_id + b.group_name+b.parent_id+b.deleted);
                
            }
        }

        private List<t_GroupRef> ParseGroupXml(FileInfo _file)
        {
            List<t_GroupRef> group = new List<t_GroupRef>();
            var doc = XDocument.Load(_file.FullName);
            if (doc.Descendants("group").NotNullOrEmpty())
            {
                foreach (var gr in doc.Descendants("group"))
                {
                    var a = CreateGroupRef(gr);
                    if (a != null)
                        group.Add(a);
                }
            }
            return group;
        }

        private t_GroupRef CreateGroupRef(XElement gr)
        {
            t_GroupRef tgr = null;
            new TryAction((o) =>
            {
                tgr = new t_GroupRef()
                {
                    group_id = int.Parse(gr.Attribute("c").Value),
                    group_name = gr.Attribute("n").Value,
                    doc_type_id = int.Parse(gr.Attribute("doc_type").Value),
                    parent_id = int.Parse(gr.Attribute("parent").Value)
                };
            }, null){ LogEventt=MDIParentMain.Log}.Start();
            return tgr;
        }

        /// <summary>
        /// Загрузка справочника электрического счетчика
        /// </summary>
        internal bool InsertSpravElick(FileInfo file, string type, string info)  //info=Новый шаблон счетчика эл.
        {
            try
            {
                XDocument xdoc = XDocument.Load(file.FullName);
                string sp_type = "";
                Regex line_reg = new Regex(@"<nom.*?(?i:элек).*?/>");
                //парсинг хмл
                string file_text = xdoc.ToString();

                if (!line_reg.IsMatch(file_text)) return false;
                sp_type = "24";
                string elick_line = line_reg.Match(file_text).Value;

                Regex attrib_reg =new Regex("([A-Za-zа-яА-Я]+)=\"(.*?)\"");

                Dictionary<string, string> attributes = new Dictionary<string, string>();

                attrib_reg.Matches(elick_line).Cast<Match>().ToList().ForEach(a =>
                    {
                       // attributes.Add(a.Groups[1].Value.Replace(",", " "), a.Groups[2].Value.Replace(",", " "));
                        attributes.Add(a.Groups[1].Value, a.Groups[2].Value);
                    });

                if (sp_type == "" || attributes.Count==0)
                {
                    log.Debug("Не удалось определить тип документа или выделить аттрибуты fileNme:" + file.Name+" file_text: "+file_text);
                    return false;
                }
                else
                {
                    
                    //проверить есть ли в t_Count_Service информация по данному типу документа
                    DataTable dt = SqlWorker.SelectFromDBSafe("SELECT t_Counts_Service.count_length FROM t_Counts_Service WHERE (((t_Counts_Service.doc_type_id)=" + sp_type + "));", "t_Counts_Service");
                    
                    if (0 == dt.Rows.Count)
                    {
                        string query = "INSERT INTO t_Counts_Service(doc_type_id,count_length,nome_1C,count_name,count_mask,count_koeff) " +
                                                                   "VALUES(" + sp_type + "," + attributes["m"].Length + ",'" + attributes["c"] + "','" + attributes["n"] + "','" + attributes["m"] + "'," + attributes["к"] + ")";
                        //добавить запись в таблицу t_Count_Service
                        SqlWorker.ExecuteQuery(query);
                    }
                    else
                    {
                        //обновить запись в таблицу t_Count_Service
                        SqlWorker.ExecuteQuery("UPDATE t_Counts_Service SET count_length = " + attributes["m"].Length +", "+
                                                                           "nome_1C='" + attributes["c"] + "', " +
                                                                           "count_name='" + attributes["n"] + "', " +
                                                                           "count_mask='" + attributes["m"] + "', " +
                                                                           "count_koeff=" + attributes["к"] + " " +
                                                                           "WHERE doc_type_id = " + sp_type + ";");
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error("Загрузка списаний не удалась!", ex);
                throw new Exception("Загрузка списаний не удалась!", ex);
                return false;
            }
        }


        internal bool InsertSpravSpisanie(FileInfo file,string type)
        {
            try
            {
                //добавление в таблицу t_UtilizationRef
                XDocument xdoc = XDocument.Load(file.FullName);
                XElement date_elem = (from c in xdoc.Descendants("timestamp")
                                     select c).FirstOrDefault<XElement>();

                List<XElement> list = (from c in xdoc.Descendants("UtilReasonRef")
                                       select c).ToList<XElement>();


                List<t_UtilReasonRef> old_reasons_ref = new t_UtilReasonRef().Select<t_UtilReasonRef>();
                List<t_UtilReasonRef> new_reasons_ref = new List<t_UtilReasonRef>();

                List<t_UtilReasonRef> add_reasons_ref = new List<t_UtilReasonRef>();
                List<t_UtilReasonRef> remove_reasons_ref = new List<t_UtilReasonRef>();
                List<t_UtilReasonRef> update_reasons_ref = new List<t_UtilReasonRef>();

                list.ForEach(a =>
                {
                        new_reasons_ref.Add(new t_UtilReasonRef() { doc_type = a.Attribute("doc_type").Value, urr_name = a.Attribute("urr_name").Value, id_1c = a.Attribute("id_1c").Value });    
                });


                add_reasons_ref = (from new_ut in new_reasons_ref
                        where !old_reasons_ref.Select(a => a.id_1c).Contains(new_ut.id_1c)
                        select new_ut).ToList();

                remove_reasons_ref = (from ut in old_reasons_ref
                                      where !new_reasons_ref.Select(a => a.id_1c).Contains(ut.id_1c)
                                   select ut).ToList();

                update_reasons_ref = (from new_ut in new_reasons_ref
                                      join old_ut in old_reasons_ref
                                      on new_ut.id_1c equals old_ut.id_1c
                                      where old_ut.urr_name != new_ut.urr_name
                                      select new t_UtilReasonRef() { 
                                          urr_id=old_ut.urr_id, 
                                          urr_name=new_ut.urr_name,
                                          doc_type=new_ut.doc_type,
                                          id_1c=new_ut.id_1c,
                                           nome_include=new_ut.nome_include
                                      }).ToList();

                if (add_reasons_ref.Count > 0) add_reasons_ref.ForEach(a=>a.Create());
                if (remove_reasons_ref.Count > 0) remove_reasons_ref.ForEach(a => a.Delete());
                if (update_reasons_ref.Count > 0) update_reasons_ref.ForEach(a => a.Update());

                //List<UtilizationRef> utilRef = UtilizationRef.Create(list);
                ////UtilizationRef.InsertIntoDB(utilRef,true);
                //SqlWorker.InsertIntoDB(utilRef, true);
                
                
                //добавление в таблицу t_doc
                    //получить статус
                    DataTable dt = SqlWorker.SelectFromDBSafe("SELECT t_DocStatusRef.docstatusref_id FROM t_DocStatusRef WHERE (((t_DocStatusRef.doctype_id)="+type+"));","t_DocStatusRTef");
                    object doc_status_ref_obj = CellHelper.FindCell(dt, 0, "docstatusref_id");
                    //добавить в t_doc
                    SqlWorker.ExecuteQuerySafe("INSERT INTO t_Doc(doc_type_id,doc_status_id,doc_teremok_id) VALUES(" + type + "," + doc_status_ref_obj.ToString()+ ","+StaticConstants.Teremok_ID+")");
                    
                return true;
            }catch(Exception ex)
            {
                log.Error("Загрузка списаний не удалась!",ex);
                throw new Exception("Загрузка списаний не удалась!",ex);
                return false;
            }   
        }


        public void LoadNomenclature(string file_name, int type_nome_id, int teremok_id, int ban)
        {
            CBData _data = new CBData();
            XmlReader _xmlreader = null;
            string _ell;

            OleDbConnection _conn = null;

            string _c = "";
            string _n = "";
            string _be = "";
            string _k = "";
            string _ue = "";
            string _mn = "0";
            string _mx = "0";            
            string _ke = "";
            string _ze = "";
            string _bl = "";
            string _ty = "0";
            int _int = 0;
            int _group = 0;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                _xmlreader = XmlReader.Create(file_name);
                XDocument _doc = XDocument.Load(_xmlreader);

                // очистим шаблон для загрузки
                _data.NomeClearTemplate(_conn, type_nome_id, teremok_id);

                // закачаем по позициям
                foreach (XElement element in _doc.Root.Elements())
                {
                    foreach (XAttribute _atr in element.Attributes())
                    {
                        _ell = _atr.Name.ToString();
                        switch (_ell)
                        {
                            case "c":
                                _c = _atr.Value.ToString();
                                break;
                            case "n":
                                _n = _atr.Value.ToString();
                                break;
                            case "be":
                                _be = _atr.Value.ToString();
                                break;
                            case "k":
                                _k = _atr.Value.ToString();
                                break;
                            case "ue":
                                _ue = _atr.Value.ToString();
                                break;
                            case "mn":
                                _mn = _atr.Value.ToString();
                                break;
                            case "mx":
                                _mx = _atr.Value.ToString();
                                break;
                            case "ke":
                                _ke = _atr.Value.ToString();
                                break;
                            case "ze":
                                _ze = _atr.Value.ToString();
                                break;
                            case "bl":
                                _bl = _atr.Value.ToString();
                                break;
                            case "t":
                                _ty = _atr.Value.ToString();
                                break;
                            case "int":
                                _int = Convert.ToInt32(_atr.Value.ToString());
                                break;
                            case "group":
                                _group = Convert.ToInt32(_atr.Value.ToString());
                                break;
                        }
                    }
                    if (CParam.AppCity == 1)
                    {
                        _data.ImportNome(_conn, _c, _n, _be, _k, _ue, _mn, _mx, _ke, _ze, _bl, Convert.ToInt32(_ty), type_nome_id, teremok_id, ban,_group);
                    }
                    else
                    {
                        if (_be == _ze)
                        {
                            string _ka = "1";
                            _data.ImportNome(_conn, _c, _n, _be, _ka, _ue, _mn, _mx, _ke, _ze, _bl, Convert.ToInt32(_ty), type_nome_id, teremok_id, ban, _group);
                        }
                        else
                        {
                            _data.ImportNome(_conn, _c, _n, _be, _k, _ue, _mn, _mx, _ke, _ze, _bl, Convert.ToInt32(_ty), type_nome_id, teremok_id, ban, _group);
                        }
                    }
                }
                // новый документ
                _data.NoneAddDoc(_conn, type_nome_id, teremok_id);

                _xmlreader.Close();
                File.Delete(file_name);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_xmlreader != null)
                    _xmlreader.Close();
                if (_conn != null)
                    _conn.Close();
            }
        }


        public t_Doc CreateDocumentRecieved(int doc_type_id,string description)
        {
            t_DocStatusRef dsr = new t_DocStatusRef().SelectFirst<t_DocStatusRef>("doctype_id=" + doc_type_id.ToString()+
            " AND statustype_id="+4);
            return CreateDocument(dsr,description);
        }

        public t_Doc CreateDocument(t_DocStatusRef doc_status_ref,string description)
        {
            t_Doc doc = new t_Doc();
            doc.doc_type_id = doc_status_ref.doctype_id;
            doc.doc_datetime = DateTime.Now;
            doc.doc_status_id = doc_status_ref.docstatusref_id;
            doc.doc_teremok_id = int.Parse(StaticConstants.Teremok_ID);

            doc.doc_desc = description;
            doc.Create();
            return doc;
        }

        public void LoadVideoArchive(string file_name,int type_nome_id, int teremok_id)
        {
            CBData _data = new CBData();
            XmlReader _xmlreader = null;
            string _ell;

            OleDbConnection _conn = null;

            try
            {
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                // очистим шаблон для загрузки
                //_data.NomeClearTemplate(_conn, type_nome_id, teremok_id);

                
                // новый документ
                _data.NoneAddDoc(_conn, type_nome_id, teremok_id);


                File.Delete(file_name);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_xmlreader != null)
                    _xmlreader.Close();
                if (_conn != null)
                    _conn.Close();
            }
        }

        /// <summary>
        /// Загрузка входящей накладной
        /// </summary>
        /// <param name="file_name"></param>
        /// <param name="teremok_id"></param>
        public void LoadTransfer(string file_name, int teremok_id)
        {
            CBData _data = new CBData();
            XmlReader _xmlreader = null;
            string _ell;

            OleDbConnection _conn = null;

            int _doc_id = 0;
            string _c = "";
            string _q = "";
            string _n = "";
            string _be = "";
            string _ue = "";
            string _k = "";

            string _teremok2_code = "";
            string _code_1C = "";
            string _date_transf = "";

            log.Info("LoadTransfer Step 1");

            try
            {
                // получить дату документа

                _xmlreader = XmlReader.Create(file_name);
                XDocument _doc = XDocument.Load(_xmlreader);
                               
                foreach (XAttribute _atr in _doc.Root.Attributes())
                {
                    _ell = _atr.Name.ToString();
                    switch (_ell)
                    {
                        case "teremok2_code":
                            _teremok2_code = _atr.Value.ToString();
                            break;
                        case "code-1C":
                            _code_1C = _atr.Value.ToString();
                            break;
                        case "date-transf":
                            _date_transf = _atr.Value.ToString();
                            break;                        
                    }
                }

                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                // формируем документ в базуs
                if (_teremok2_code != "" && _code_1C != "")
                {
                    _doc_id = _data.TransferAdd( teremok_id, Convert.ToInt32(_data.GetTeremokIDBy1C(_teremok2_code)), Convert.ToDateTime(_date_transf), _code_1C);
                    log.Info("LoadTransfer Step 2");
                }
                else
                {
                    log.Info("LoadTransfer Step 3 (error)");
                    throw new Exception("НЕ закачено входящее перемещение");
                }

                // закачаем по позициям
                foreach (XElement element in _doc.Root.Elements())
                {
                    foreach (XAttribute _atr in element.Attributes())
                    {
                        _ell = _atr.Name.ToString();
                        switch (_ell)
                        {
                            case "c":
                                _c = _atr.Value.ToString();
                                break;
                            case "q":
                                _q = _atr.Value.ToString();
                                break;
                            case "n":
                                _n = _atr.Value.ToString();
                                break;
                            case "ue":
                                _ue = _atr.Value.ToString();
                                break;
                            case "be":
                                _be = _atr.Value.ToString();
                                break;
                            case "k":
                                _k = _atr.Value.ToString();
                                break;                         

                        }
                    }
                    // проверим, есть ли такая позиция в базе
                    _data.NomeCheckInDB( _c, _n, _ue, _be, _k, teremok_id);
                    log.Info("LoadTransfer Step 4");

                    // вставить данные
                    _data.TransferAddItem( _c, _q, _doc_id, teremok_id);
                    log.Info("LoadTransfer Step 5 - insert" + _n.ToString());
                }

                _xmlreader.Close();
                File.Delete(file_name);
            }
            catch (Exception _exp)
            {
                log.Error("LoadTransfer Step 6" + _exp.Message);
                throw _exp;
            }
            finally
            {
                log.Info("LoadTransfer Step 6 Final");
                if (_xmlreader != null)
                    _xmlreader.Close();
                if (_conn != null)
                    _conn.Close();
            }
        }

        public void LoadTeremok(string file_name)
        {
            CBData _data = new CBData();
            OleDbConnection _conn = null;
            XmlReader _xmlreader = null;
            string _ell;

            string _teremok_id = "";
            string _teremok_name = "";
            string _teremok_1C = "";
            string _teremok_dep = "";

            try
            {
                _xmlreader = XmlReader.Create(file_name);
                XDocument _doc = XDocument.Load(_xmlreader);

                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                foreach (XElement el in _doc.Root.Elements())
                {
                    foreach (XAttribute _atr in el.Attributes())
                    {
                        _ell = _atr.Name.ToString();
                        switch (_ell)
                        {
                            case "id":
                                _teremok_id = _atr.Value.ToString();
                                break;
                            case "c":
                                _teremok_1C = _atr.Value.ToString();
                                break;
                            case "n":
                                _teremok_name = _atr.Value.ToString();
                                break;
                            case "d":
                                _teremok_dep = _atr.Value.ToString();
                                break;
                        }
                    }
                    _data.ImportTeremok(_conn, _teremok_id, _teremok_name, _teremok_1C, _teremok_dep);                    
                }

                _xmlreader.Close();                
                File.Delete(file_name);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_xmlreader != null)
                    _xmlreader.Close();
                if (_conn != null)
                    _conn.Close();
            }
        }

        public void LoadCash(string file_name)
        {
            CBData _data = new CBData();
            OleDbConnection _conn = null;
            XmlReader _xmlreader = null;
            string _ell;

            string t_name = "";
            string t_path_name = "";
            string t_hash_code = "";
            string t_creation_time = "";

            try
            {
                _xmlreader = XmlReader.Create(file_name);
                XDocument _doc = XDocument.Load(_xmlreader);

                _data.NomeClearCash();

                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                foreach (XElement el in _doc.Root.Elements())
                {
                    foreach (XAttribute _atr in el.Attributes())
                    {
                        _ell = _atr.Name.ToString();
                        switch (_ell)
                        {
                            case "a":
                                t_name = _atr.Value.ToString();
                                break;
                            case "b":
                                t_path_name = _atr.Value.ToString();
                                break;
                            case "c":
                                t_hash_code = _atr.Value.ToString();
                                break;
                            case "d":
                                t_creation_time = _atr.Value.ToString();
                                break;
                        }
                    }
                    _data.ImportCash(_conn, t_name, t_path_name, t_hash_code, t_creation_time);
                }

                _xmlreader.Close();
                File.Delete(file_name);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_xmlreader != null)
                    _xmlreader.Close();
                if (_conn != null)
                    _conn.Close();
            }
        }

        public void LoadVideo(string file_name)
        {
            CBData _data = new CBData();
            OleDbConnection _conn = null;
            XmlReader _xmlreader = null;
            string _ell;
            string _elhead;

            string t_switch = "";
            string t_screenName = "";
            string t_fileName = "";
            string t_release = "";
            string t_category = "";

            try
            {
                _xmlreader = XmlReader.Create(file_name);
                XDocument _doc = XDocument.Load(_xmlreader);

                _data.NomeClearVideo();

                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                foreach (XElement el in _doc.Root.Elements())
                {
                    foreach (XAttribute _atr in el.Attributes())
                    {
                        _elhead = _atr.Name.ToString();
                        switch (_elhead)
                        {
                            case "number":
                                t_release = _atr.Value.ToString();
                                break;
                        }

                        foreach (XElement ell in el.Elements())
                        {
                            foreach (XElement _at in ell.Elements())
                            {
                                t_switch = _at.Name.ToString();
                                foreach (XAttribute _a in _at.Attributes())
                                {
                                    _ell = _a.Name.ToString();
                                    switch (_ell)
                                    {
                                        case "fileName":
                                            t_fileName = _a.Value.ToString();
                                            break;
                                        case "screenName":
                                            t_screenName = _a.Value.ToString();
                                            break;
                                        case "category":
                                            t_category = _a.Value.ToString();
                                            break;
                                    }
                                }
                                _data.ImportVideo(_conn, t_release, t_switch, t_fileName, t_screenName, t_category);
                                _data.ImportNewDoc(_conn, t_fileName, t_screenName);
                            }
                        }
                    }
                }

                _xmlreader.Close();
                File.Delete(file_name);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_xmlreader != null)
                    _xmlreader.Close();
                if (_conn != null)
                    _conn.Close();
            }
        }

        public void LoadRetReason(string file_name)
        {
            CBData _data = new CBData();
            OleDbConnection _conn = null;
            XmlReader _xmlreader = null;
            string _ell;
            string _orr_name = "";
            string _orr_1C = "";
            
          
            try
            {
                _xmlreader = XmlReader.Create(file_name);
                XDocument _doc = XDocument.Load(_xmlreader);
                
                _conn = new OleDbConnection(CParam.ConnString);
                _conn.Open();

                foreach (XElement el in _doc.Root.Elements())
                {
                    foreach (XAttribute _atr in el.Attributes())
                    {
                        _ell = _atr.Name.ToString();
                        switch (_ell)
                        {
                            case "c":
                                _orr_1C = _atr.Value.ToString();
                                break;
                            case "n":
                                _orr_name = _atr.Value.ToString();
                                break;
                        }
                    }
                    _data.ImportRetReason(_conn, _orr_name, _orr_1C);
                }

                _xmlreader.Close();
                File.Delete(file_name);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
            finally
            {
                if (_xmlreader != null)
                    _xmlreader.Close();
                if (_conn != null)
                    _conn.Close();
            }
        }     

        public void CopyToKKM(int kkm_num, string full_file_name, string file_name, string kkm_dep, bool is_delete)
        {

            string kkm_dir = "";
            string kkm_dir_dep = "";
            try
            {
                switch (kkm_num)
                {
                    case 1: kkm_dir = CParam.Kkm1In;
                            kkm_dir_dep = CParam.kkm1_dep;
                        break;
                    case 2: kkm_dir = CParam.Kkm2In;
                            kkm_dir_dep = CParam.kkm2_dep;
                        break;
                    case 3: kkm_dir = CParam.Kkm3In;
                            kkm_dir_dep = CParam.kkm3_dep;
                        break;                 
                    case 4: kkm_dir = CParam.Kkm4In;
                            kkm_dir_dep = CParam.kkm4_dep;
                        break;
                    case 5: kkm_dir = CParam.Kkm5In;
                        kkm_dir_dep = CParam.kkm5_dep;
                        break;
                    default:
                        kkm_dir = "";
                        kkm_dir_dep = "";
                        break;
             
                }
                if (kkm_dir_dep == kkm_dep)
                {
                    if (File.Exists(kkm_dir + "\\" + file_name))
                        File.Delete(kkm_dir + "\\" + file_name);
                    File.Copy(full_file_name, kkm_dir + "\\" + file_name);
                }
            }
            catch (Exception _exp)
            {
                MDIParentMain.Log(_exp, String.Format("Ошибка копирования в кассу {0}, файла {1}", kkm_num, file_name));
            }
        }

        public void CopyCardToKKM(int kkm_num, string full_file_name, string file_name, bool is_delete)
        {
            string kkm_dir = "";
            try
            {
                switch (kkm_num)
                {
                    case 1: kkm_dir = CParam.Kkm1In;
                        break;
                    case 2: kkm_dir = CParam.Kkm2In;
                        break;
                    case 3: kkm_dir = CParam.Kkm3In;
                        break;
                    case 4: kkm_dir = CParam.Kkm4In;
                        break;
                    case 5: kkm_dir = CParam.Kkm5In;
                        break;
                    default:
                        kkm_dir = "";
                        break;
                }
              
                if (File.Exists(kkm_dir + "\\" + file_name))
                    File.Delete(kkm_dir + "\\" + file_name);
                File.Copy(full_file_name, kkm_dir + "\\" + file_name);                
            }
            catch (Exception _exp)
            {
                MDIParentMain.Log(_exp,String.Format("Ошибка копирования в кассу {0}, файла {1}",kkm_num,file_name));
            }
        }

        public void CopyToEmark(int kkm_num, string full_file_name, string file_name, bool is_delete)
        {

            string kkm_dir;
            try
            {
                switch (kkm_num)
                {
                    case 1: kkm_dir = CParam.emark_Kkm1; break;
                    case 2: kkm_dir = CParam.emark_Kkm2; break;
                    case 3: kkm_dir = CParam.emark_Kkm3; break;
                    case 4: kkm_dir = CParam.emark_Kkm4; break;
                    default:
                        kkm_dir = "";
                        break;
                }

                if (File.Exists(kkm_dir + "\\adv\\" + file_name))
                    File.Delete(kkm_dir + "\\adv\\" + file_name);
                File.Copy(full_file_name, kkm_dir + "\\adv\\" + file_name);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        public void CopyToEmarkxml(int kkm_num, string full_file_name, string file_name, bool is_delete)
        {

            string kkm_dir;
            try
            {
                switch (kkm_num)
                {
                    case 1: kkm_dir = CParam.emark_Kkm1; break;
                    case 2: kkm_dir = CParam.emark_Kkm2; break;
                    case 3: kkm_dir = CParam.emark_Kkm3; break;
                    case 4: kkm_dir = CParam.emark_Kkm4; break;
                    default:
                        kkm_dir = "";
                        break;
                }

                if (File.Exists(kkm_dir + "\\" + file_name))
                    File.Delete(kkm_dir + "\\" + file_name);
                File.Copy(full_file_name, kkm_dir + "\\" + file_name);
            }
            catch (Exception _exp)
            {
                throw _exp;
            }
        }

        
    }
}
