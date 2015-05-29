using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Collections;
using System.Data.OleDb;

namespace RBClient.Classes.InternalClasses.Models
{
    [Serializable]
    public class ModelClass
    {
        private string tablename = "";

        private Dictionary<Type, IEnumerable<ModelClass>> _JoinTables;
        public Dictionary<Type, IEnumerable<ModelClass>> JoinTables
        {
            get
            {
                if (_JoinTables == null)
                {
                    _JoinTables = new Dictionary<Type, IEnumerable<ModelClass>>();
                }
                return _JoinTables;
            }
            set
            {
                _JoinTables = value;
            }
        }

        private string JoinCondition = "";

        public void Join<T>(string JoinCondition) where T:ModelClass,new()
        {
            var o = new T().Select<T>(JoinCondition); if (o == null) return;

            IEnumerable<ModelClass> joins = o.OfType<ModelClass>();
            joins.ToList().ForEach(a => a.JoinCondition = JoinCondition);
           if (joins != null && joins.Count() > 0)
           {
               if (JoinTables.Keys.Contains(typeof(T)))
               {
                   JoinTables[typeof(T)].ToList().AddRange(joins);
               }
               else
               {
                   JoinTables.Add(typeof(T), (IEnumerable<ModelClass>)joins);
               }
           }
        }

        public void RecursiveDelete()
        {
            if (JoinTables == null || JoinTables.Count == 0)
            {
                Delete();
            }
            else
            {
                JoinTables.Keys.ToList().ForEach(a =>
                {
                    JoinTables[a].ToList().ForEach(b =>
                    {
                        b.RecursiveDelete();
                    });
                    JoinTables.Remove(a);
                });
                RecursiveDelete();
            }
        }

        public void DeleteJoinTablesMass()
        {
            if (JoinTables == null || JoinTables.Count == 0)
            {
                return;
            }
            else
            {
                
                JoinTables.ToList().ForEach(a=>
                {
                    List<ModelClass> ml = a.Value.ToList();
                    ml.ForEach(b =>
                    {

                        if (b.JoinTables != null && b.JoinTables.Count > 0)
                        {
                            b.DeleteJoinTablesMass();
                        }
                    });
                    //удалить все дочерние таблицы
                    SqlWorker.ExecuteQuerySafe("delete from " + a.Key.Name + " where " + ml[0].JoinCondition);
                    //удалить из словаря данную запись
                    JoinTables.Remove(a.Key);
                });
                
            }
        }

        //public void Join<T>(Func<object,object,bool> JoinCondition) where T:ModelClass,new()
        //{
        //    try
        //    {
        //        List<T> joins = new List<T>();
        //        new T().Select<T>().Where(a => JoinCondition(this, a)).ToList();
        //    }
        //    catch()
        //    {
        //    }
        //}

        /// <summary>
        /// Создает запись
        /// </summary>
        public bool Create()
        {
            tablename = this.GetType().Name;
            string query = MakeInsertQuery();

            int new_id = SqlWorker.InsertQuerySafe(query);  //получить id добавленной записи

            ClassFactory.SetValueToClassProperty(this, new_id, 0); //обновить текущий объект

            if (new_id != 0)
            {
                
                return true;
            }
            else return false;
        }


        /// <summary>
        /// Очищаем указанные свойства из таблицы
        /// </summary>
        public bool Clear(List<string> list)
        {
            tablename = this.GetType().Name;
            OleDbCommand query = MakeClearQuery(list);

            if (SqlWorker.ExecuteQuerySafe(query))
            {
                list.ForEach(a =>
                {
                    StaticHelperClass.SetClassItemValue(this, a, null);
                });
                return true;
            }
            else return false;
        }


        /// <summary>
        /// Создает запись
        /// </summary>
        public bool CreateOle()
        {
            tablename = this.GetType().Name;
            OleDbCommand query = MakeInsertCommandOle();

            int new_id = SqlWorker.InsertQuerySafe(query);  //получить id добавленной записи

            ClassFactory.SetValueToClassProperty(this, new_id, 0); //обновить текущий объект

            if (new_id != 0)
            {
                //id = new_id;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Обновляет запись
        /// </summary>
        public bool UpdateOle()
        {
            tablename = this.GetType().Name;
            OleDbCommand query = MakeUpdateCommandOle();

            int new_id = SqlWorker.InsertQuerySafe(query);  //получить id добавленной записи

            if (new_id != 0)
            {
                return true;
            }
            else return false;
        }



        /// <summary>
        /// удаляет запись
        /// </summary>
        public bool Delete()
        {
            tablename = this.GetType().Name;
            string query = MakeDeleteQuery();
            return SqlWorker.ExecuteQuerySafe(query);
        }

        /// <summary>
        /// Обновляет запись
        /// </summary>
        public bool Update()
        {
            tablename = this.GetType().Name;
            string query = MakeUpdateQuery();
            return SqlWorker.ExecuteQuerySafe(query);
        }

       

        /// <summary>
        /// Обновляет запись
        /// </summary>
        public bool Update(bool removeEmptyStrings)
        {
            tablename = this.GetType().Name;
            string query = MakeUpdateQuery(removeEmptyStrings);
            return SqlWorker.ExecuteQuerySafe(query);
        }

        /// <summary>
        /// Обновляет запись
        /// </summary>
        //public bool Update(bool removeEmptyStrings,object null_value)
        //{
        //    tablename = this.GetType().Name;
        //    string query = MakeUpdateQuery(removeEmptyStrings,null_value);
        //    return SqlWorker.ExecuteQuerySafe(query);
        //}

        /// <summary>
        /// Обновляет запись новым значением
        /// </summary>
        /// <typeparam name="T">тип таблицы данных</typeparam>
        /// <param name="new_item">новое значение</param>
        /// <returns></returns>
        public bool Update<T>(T new_item) where T : ModelClass, new()
        {
            tablename = this.GetType().Name;
            string query = MakeUpdateQuery(new_item);
            return SqlWorker.ExecuteQuerySafe(query);
        }

        /// <summary>
        /// Обновляет таблицу справочника новыми значениями, удаляя старые
        /// </summary>
        /// <typeparam name="T">тип таблицы модели</typeparam>
        /// <param name="sprav_items">новые значения справочника</param>
        /// <param name="get_id">выражение для получения идентификатора</param>
        /// <param name="get_hash">выражение для получения содержимого</param>
        public void UpdateSprav<T>(List<T> sprav_items,Func<T, string> get_id, Func<T, string> get_hash) where T : ModelClass, new()
        {
            List<T> old_reasons_ref = new T().Select<T>();
            List<T> new_reasons_ref = sprav_items;

            List<T> add_reasons_ref = new List<T>();
            List<T> remove_reasons_ref = new List<T>();

            add_reasons_ref = (from new_ut in new_reasons_ref
                               where !old_reasons_ref.Select(a => get_id(a)).Contains(get_id(new_ut))
                               select new_ut).ToList();

            remove_reasons_ref = (from ut in old_reasons_ref
                                  where !new_reasons_ref.Select(a => get_id(a)).Contains(get_id(ut))
                                  select ut).ToList();

            if (add_reasons_ref.Count > 0) add_reasons_ref.ForEach(a => a.Create());
            if (remove_reasons_ref.Count > 0) remove_reasons_ref.ForEach(a => a.Delete());

            if (get_hash != null)
            {
              var  update_reasons_ref = (from new_ut in new_reasons_ref
                                      join old_ut in old_reasons_ref
                                      on get_id(new_ut) equals get_id(old_ut)
                                      where get_hash(old_ut) != get_hash(new_ut)
                                      select new 
                                      {
                                          old_item=old_ut,
                                          new_item=new_ut
                                      }).ToList();
              update_reasons_ref.ForEach(a => a.old_item.Update(a.new_item));
            }
        }

        /// <summary>
        /// Обновляет таблицу справочника новыми значениями, не удаляя старые
        /// </summary>
        /// <typeparam name="T">тип таблицы модели</typeparam>
        /// <param name="sprav_items">новые значения справочника</param>
        /// <param name="get_id">выражение для получения идентификатора</param>
        /// <param name="get_hash">выражение для получения содержимого</param>
        public void UpdateSpravAddOrUpdate<T>(List<T> sprav_items, Func<T, string> get_id, Func<T, string> get_hash) where T : ModelClass, new()
        {
            List<T> old_reasons_ref = new T().Select<T>();
            List<T> new_reasons_ref = sprav_items;
            List<T> add_reasons_ref = new List<T>();
            
            
            add_reasons_ref = (from new_ut in new_reasons_ref
                               where !old_reasons_ref.Select(a => get_id(a)).Contains(get_id(new_ut))
                               select new_ut).ToList();

            

            if (add_reasons_ref.Count > 0) add_reasons_ref.ForEach(a => a.Create());
            if (get_hash != null)
            {
                var update_reasons_ref = (from new_ut in new_reasons_ref
                                          join old_ut in old_reasons_ref
                                          on get_id(new_ut) equals get_id(old_ut)
                                          where get_hash(old_ut) != get_hash(new_ut)
                                          select new
                                          {
                                              old_item = old_ut,
                                              new_item = new_ut
                                          }).ToList();
                update_reasons_ref.ForEach(a => a.old_item.Update(a.new_item));
            }
        }


        /// <summary>
        /// Обновляет таблицу справочника новыми значениями, не удаляя старые
        /// + обновляем связанную таблицу(удаляя ненужные)
        /// </summary>
        /// <typeparam name="T">тип таблицы модели</typeparam>
        /// <param name="sprav_items">новые значения справочника</param>
        /// <param name="get_id">выражение для получения идентификатора</param>
        /// <param name="get_hash">выражение для получения содержимого</param>
        public void UpdateSpravAddOrUpdateConnection<T,U>(List<T> sprav_items, Func<T, string> get_id, Func<T, string> get_hash) where T : ModelClass, new()
            where U : ModelClass, new()
        {
            List<T> old_reasons_ref = new T().Select<T>();
            List<T> new_reasons_ref = sprav_items;
            List<T> add_reasons_ref = new List<T>();


            add_reasons_ref = (from new_ut in new_reasons_ref
                               where !old_reasons_ref.Select(a => get_id(a)).Contains(get_id(new_ut))
                               select new_ut).ToList();



            if (add_reasons_ref.Count > 0) add_reasons_ref.ForEach(a => a.Create());
            if (get_hash != null)
            {
                var update_reasons_ref = (from new_ut in new_reasons_ref
                                          join old_ut in old_reasons_ref
                                          on get_id(new_ut) equals get_id(old_ut)
                                          where get_hash(old_ut) != get_hash(new_ut)
                                          select new
                                          {
                                              old_item = old_ut,
                                              new_item = new_ut
                                          }).ToList();
                update_reasons_ref.ForEach(a => a.old_item.Update(a.new_item));
            }
        }



        /// <summary>
        /// Делаем выборку из таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Select<T>() where T:ModelClass,new()
        {
            tablename = this.GetType().Name;
            string query = MakeSelectQuery("");
            DataTable dt=SqlWorker.SelectFromDBSafe(query, tablename);
            if (dt == null) return null;
            List<T> list = ClassFactory.CreateClasses<T>(dt);
            return list;
        }
        
        /// <summary>
        /// Делаем выборку из таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where_condition">условие where</param>
        /// <returns></returns>
        public List<T> Select<T>(string where_condition) where T : ModelClass, new()
        {
            tablename = this.GetType().Name;
            string query = MakeSelectQuery(where_condition);
            DataTable dt = SqlWorker.SelectFromDBSafe(query, tablename);
            if (dt == null) return null;
            List<T> list = ClassFactory.CreateClasses<T>(dt);
            return list;
        }

        /// <summary>
        /// Делаем выборку из таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where_condition">условие where</param>
        /// <returns></returns>
        public List<ModelClass> Select(string where_condition) 
        {
            tablename = this.GetType().Name;
            string query = MakeSelectQuery(where_condition);
            DataTable dt = SqlWorker.SelectFromDBSafe(query, tablename);
            if (dt == null) return null;
            List<ModelClass> list = ClassFactory.CreateClasses(dt, this.GetType()).OfType<ModelClass>().ToList();
            return list;
        }

        /// <summary>
        /// Делаем выборку из таблицы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where_condition">условие where</param>
        /// <returns></returns>
        public object SelectReflection(string where_condition)
        {
            tablename = this.GetType().Name;
            string query = MakeSelectQuery(where_condition);
            DataTable dt = SqlWorker.SelectFromDBSafe(query, tablename);
            if (dt == null) return null;
            object list = ClassFactory.CreateClasses(dt, this.GetType());
            return list;
        }

        /// <summary>
        /// Делаем выборку из таблицы одной записи
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where_condition">условие where</param>
        /// <returns></returns>
        public T SelectFirst<T>(string where_condition) where T : ModelClass, new()
        {
            tablename = this.GetType().Name;
            string query = MakeSelectQuery(where_condition);
            DataTable dt = SqlWorker.SelectFromDBSafe(query, tablename);
            List<T> list = ClassFactory.CreateClasses<T>(dt);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Делаем выборку из таблицы одной записи
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where_condition">условие where</param>
        /// <returns></returns>
        public T SelectFirst<T>() where T : ModelClass, new()
        {
            tablename = this.GetType().Name;
            string query = MakeSelectQuery("");
            DataTable dt = SqlWorker.SelectFromDBSafe(query, tablename);
            List<T> list = ClassFactory.CreateClasses<T>(dt);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Делаем выборку из таблицы одной последней записи
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where_condition">условие where</param>
        /// <returns></returns>
        public T SelectLast<T>(string where_condition) where T : ModelClass, new()
        {
            tablename = this.GetType().Name;
            string query = MakeSelectQuery(where_condition);
            DataTable dt = SqlWorker.SelectFromDBSafe(query, tablename);
            List<T> list = ClassFactory.CreateClasses<T>(dt);
            if (list != null && list.Count > 0)
            {
                return list.Last();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Делаем выборку из таблицы одной последней записи
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where_condition">условие where</param>
        /// <returns></returns>
        public T SelectLast<T>() where T : ModelClass, new()
        {
            tablename = this.GetType().Name;
            string query = MakeSelectQuery("");
            DataTable dt = SqlWorker.SelectFromDBSafe(query, tablename);
            List<T> list = ClassFactory.CreateClasses<T>(dt);
            if (list != null && list.Count > 0)
            {
                return list.Last();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает запрос Select
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public string MakeSelectQuery(string condition)
        {
            string query = "SELECT * From " + tablename;

            if (condition != "")
            {
                query += " Where " + condition;
            }
            return query;
        }

        /// <summary>
        /// Создает запрос инсерт
        /// </summary>
        /// <returns></returns>
        public string MakeInsertQuery()
        {
            string query = "INSERT INTO " + tablename + " (";

            Type type = this.GetType();

            List<FieldInfo> pinfos =  type.GetFields().OrderBy(field => field.MetadataToken).ToList();
            if (pinfos[0].GetValue(this).ToString() == "" || pinfos[0].GetValue(this).ToString() == "0")
            {
                pinfos.RemoveAt(0);
            }
                
            pinfos.Select(a=>a.Name).ToList<string>().ForEach(s =>
            {
                    query += s + ",";
            });

            query = query.Substring(0, query.Length - 1) + ") VALUES(";

            pinfos.Select(a=>a.GetValue(this)).ToList<object>().ForEach(s =>
            {
                query += SqlWorker.ReturnDBValue(s) + ",";
            });
            query = query.Substring(0, query.Length - 1) + ")";
            return query;
        }


        /// <summary>
        /// Создает запрос инсерт
        /// </summary>
        /// <returns></returns>
        public OleDbCommand MakeClearQuery(List<string> pinfos)
        {
            OleDbCommand _command = new OleDbCommand();
            string query = "Update " + tablename + " SET ";

            //ClassFactory.SetValueToClassProperty(this, new_id, 0);

            pinfos.ForEach(s =>
            {
                query += s + "=@"+s+", ";
            });

            query = query.Substring(0, query.Length - 2);

            string id_name = "";
            object id=ClassFactory.GetValueFromClassProperty(this, 0,out id_name);

            query += " where " + id_name+"="+id.ToString();

            _command.CommandText = query;
            
            pinfos.ForEach(a =>
            {
                _command.Parameters.AddWithValue("@" + a, DBNull.Value);
            });

            return _command;
        }

        /// <summary>
        /// Создает запрос инсерт 2я версия
        /// </summary>
        /// <returns></returns>
        public OleDbCommand MakeInsertCommandOle()
        {
            OleDbCommand _command = new OleDbCommand();
            string query = "INSERT INTO " + tablename + " (";

            Type type = this.GetType();

            List<FieldInfo> pinfos =  type.GetFields().OrderBy(field => field.MetadataToken).ToList();
            if (pinfos[0].GetValue(this).ToString() == "" || pinfos[0].GetValue(this).ToString() == "0")
            {
                pinfos.RemoveAt(0);
            }

            pinfos.Select(a => a.Name).ToList<string>().ForEach(s =>
            {
                query += s + ",";
            });

            query = query.Substring(0, query.Length - 1) + ") VALUES(";

            pinfos.Select(a => a.Name).ToList<string>().ForEach(s =>
            {
                query += "@"+s + ",";
            });
            query = query.Substring(0, query.Length - 1) + ")";

            _command.CommandText = query;
            //_str_command = "INSERT INTO t_Doc(doc_type_id, doc_datetime, doc_teremok_id, doc_status_id, doc_guid) VALUES (@doc_type_id, @doc_datetime, @doc_teremok_id,@doc_status_id, @doc_guid)";

            pinfos.ForEach(a =>
            {
                object par = a.GetValue(this);
                if (par is byte[])
                {
                    OleDbParameter param = new OleDbParameter();
                    param.OleDbType = OleDbType.VarBinary;
                    param.ParameterName = "@" + a.Name;

                    if (par == null) param.Value = DBNull.Value;
                    else
                    {
                        param.Value = par;
                    }
                    _command.Parameters.Add(param);

                    //var bb=(byte[])par;
                    //byte[] cc = new byte[bb.Length];bb.CopyTo(cc, 0);
                    //if (par == null) param.Value = DBNull.Value; else param.Value=cc;
                    //_command.Parameters.Add(param);
                }
                else
                {
                    if (par is DateTime)
                    {
                        _command.Parameters.AddWithValue("@" + a.Name, par.ToString());
                    }
                    else
                    {
                        if (par == null) par = DBNull.Value;
                        _command.Parameters.AddWithValue("@" + a.Name, par);
                    }
                }
            });

            return _command;
        }

        /// <summary>
        /// Создает запрос update 2я версия
        /// </summary>
        /// <returns></returns>
        public OleDbCommand MakeUpdateCommandOle()
        {
            OleDbCommand _command = new OleDbCommand();
            string query = "Update " + tablename + " Set ";

            Type type = this.GetType();

            List<FieldInfo> pinfos =  type.GetFields().OrderBy(field => field.MetadataToken).ToList();

            FieldInfo pi_id = pinfos[0];
            pinfos.RemoveAt(0);
           

            pinfos.ForEach(a=>{
                query += a.Name + "=@" + a.Name+", ";
            });

            query = query.Substring(0, query.Length - 2);
            query += " WHERE " + pi_id.Name + "=" + SqlWorker.ReturnDBValue(pi_id.GetValue(this));


            _command.CommandText = query;
            //_str_command = "INSERT INTO t_Doc(doc_type_id, doc_datetime, doc_teremok_id, doc_status_id, doc_guid) VALUES (@doc_type_id, @doc_datetime, @doc_teremok_id,@doc_status_id, @doc_guid)";

            pinfos.ForEach(a =>
            {
                object par = a.GetValue(this);
                if (par is DateTime)
                {
                    _command.Parameters.AddWithValue("@" + a.Name, a.GetValue(this).ToString());
                }
                else
                {
                    
                    if (par is byte[])
                    {
                        OleDbParameter param = new OleDbParameter();
                        param.OleDbType = OleDbType.VarBinary;
                        param.ParameterName = "@" + a.Name;

                        if (par == null) param.Value = DBNull.Value;
                        else
                        {
                            param.Value = par;
                        }
                        _command.Parameters.Add(param);
                    }
                    else
                    {
                        if (par == null) par = DBNull.Value;
                        _command.Parameters.AddWithValue("@" + a.Name, par);
                    }
                }
            });

            return _command;
        }

        /// <summary>
        /// Создает запрос апдейт
        /// </summary>
        /// <returns></returns>
        public string MakeUpdateQuery()
        {
            //"UPDATE t_MarkItems SET mark_" + nu + " = @mark_" + nu + " WHERE mark_id =" + row;
            string query = "Update " + tablename + " SET ";

            Type type = this.GetType();

            List<FieldInfo> pinfos =  type.GetFields().OrderBy(field => field.MetadataToken).ToList();

            pinfos.ForEach(a =>
            {
                if (a == pinfos.Last())
                {
                    query += a.Name + "=" + SqlWorker.ReturnDBValue(a.GetValue(this)) + " ";
                }
                else
                {
                    if (a != pinfos[0])
                    {
                        query += a.Name + "=" + SqlWorker.ReturnDBValue(a.GetValue(this)) + ", ";
                    }
                }
            });

            query += " WHERE " + pinfos[0].Name + "=" + SqlWorker.ReturnDBValue(pinfos[0].GetValue(this));

            return query;
        }

        /// <summary>
        /// Создает запрос апдейт
        /// </summary>
        /// <returns></returns>
        public string MakeUpdateQuery(bool removeEmptyStrings)
        {
            //"UPDATE t_MarkItems SET mark_" + nu + " = @mark_" + nu + " WHERE mark_id =" + row;
            string query = "Update " + tablename + " SET ";

            Type type = this.GetType();



            List<FieldInfo> pinfos = type.GetFields()
                                             .OrderBy(field => field.MetadataToken).ToList();
            
            //List<FieldInfo> pinfos = type.GetFields().ToList();


            pinfos.ForEach(a =>
            {
                if (a == pinfos.Last())
                {
                    query += a.Name + "=" + SqlWorker.ReturnDBValue(a.GetValue(this)) + " ";
                }
                else
                {
                    if (removeEmptyStrings && a.GetValue(this) != null && a.GetValue(this).ToString() != "")
                    {
                        if (a != pinfos[0])
                        {
                            query += a.Name + "=" + SqlWorker.ReturnDBValue(a.GetValue(this)) + ", ";
                        }
                    }
                }
            });

            query += " WHERE " + pinfos[0].Name + "=" + SqlWorker.ReturnDBValue(pinfos[0].GetValue(this));

            return query;
        }

        /// <summary>
        /// Создает запрос апдейт
        /// </summary>
        /// <returns></returns>
        public string MakeUpdateQuery(object o)
        {
            //"UPDATE t_MarkItems SET mark_" + nu + " = @mark_" + nu + " WHERE mark_id =" + row;
            string query = "Update " + tablename + " SET ";

            Type type = this.GetType();

            List<FieldInfo> pinfos =  type.GetFields().OrderBy(field => field.MetadataToken).ToList();

            pinfos.ForEach(a =>
            {
                if (a == pinfos.Last())
                {
                    query += a.Name + "=" + SqlWorker.ReturnDBValue(a.GetValue(o)) + " ";
                }
                else
                {
                    if (a != pinfos[0])
                    {
                        query += a.Name + "=" + SqlWorker.ReturnDBValue(a.GetValue(o)) + ", ";
                    }
                }
            });

            query += " WHERE " + pinfos[0].Name + "=" + SqlWorker.ReturnDBValue(pinfos[0].GetValue(this));

            return query;
        }

        /// <summary>
        /// Создает запрос на очищение таблицы
        /// </summary>
        /// <returns></returns>
        public string MakeDeleteAllQuery()
        {
            string query = "DELETE FROM " + tablename;
            return query;
        }

        /// <summary>
        /// Создает запрос на удаление
        /// </summary>
        /// <returns></returns>
        public string MakeDeleteQuery()
        {
            string query = "Delete from " + tablename;
            Type type = this.GetType();
            List<FieldInfo> pinfos =  type.GetFields().OrderBy(field => field.MetadataToken).ToList();
            query += " WHERE " + pinfos[0].Name + "=" + SqlWorker.ReturnDBValue(pinfos[0].GetValue(this));
            return query;
        }

        public string getHashCode(string[] ignoredProps)
        {
            string hash_code = null;

            Type type = this.GetType();

            List<FieldInfo> pinfos =  type.GetFields().OrderBy(field => field.MetadataToken).ToList();

            if (ignoredProps != null)
            {
                ignoredProps.ToList().ForEach(a =>
                {
                    pinfos.RemoveAll(b => b.Name.ToLower().Equals(a.ToLower()));
                });
            }

            if (pinfos.Count > 0)
            {
                hash_code = "";
                pinfos.ForEach(a => hash_code += a.GetValue(this).ToString());
            }

            return hash_code;
        }

        public override string ToString()
        {
            if (this is t_OrderRetReason)
            {
                return (this as t_OrderRetReason).orr_name;
            }
            if (this is t_Nomenclature)
            {
                return (this as t_Nomenclature).nome_name;
            }
            if (this is t_OrderRetReason)
            {
                return (this as t_OrderRetReason).orr_name;
            }
            if (this is t_Employee)
            {
                return (this as t_Employee).employee_name;
            }
            if (this is t_Responsibility)
            {
                return (this as t_Responsibility).res_name;
            }
            if (this is t_ShiftType)
            {
                return (this as t_ShiftType).type_name;
            }
            return base.ToString();
        }
    }
}
