using RBClient.Classes.CustomClasses;
using RBClient.Classes.DocumentClasses;
using RBClient.Classes.InternalClasses.Models;
using RBClient.Classes.OrderClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient.Classes
{
    class SerializationHipManager : LoggerBaseMdi
    {
        private static SerializationHipManager _Instance;
        public static SerializationHipManager Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    _Instance = new SerializationHipManager();
                }
                return _Instance;
            }           
        }

        internal void SaveToHip(object _object)
        {
            if (_object == null) return;

            if (_object is OrderStateClass)
            {
                SaveOrderStateToHip((OrderStateClass)_object);
            }else
            {
                throw new NotImplementedException();
            }
        }

        internal void SaveOrderStateToHip(OrderStateClass webServiceStateClass)
        {
            if (webServiceStateClass is WebService1CStateClass)
            {
                Insert1CWebServieObjectToHip((WebService1CStateClass)webServiceStateClass);
            }
        }

        private void Insert1CWebServieObjectToHip(WebService1CStateClass webService1CStateClass)
        {
            t_SerializationHip hipRecord = CreateHipRecord(webService1CStateClass);
            if (hipRecord == null)
            {
                Log("Insert1CWebServieObjectToHip error searchHip is null");
                return;
            }
            t_SerializationHip existedRecord = null;
            if (CheckIfRecordAlreadyExist(ref hipRecord, ref existedRecord))
            {
                UpdateHipRecord(hipRecord,existedRecord);
            }
            else
            {
                InsertHiRecord(hipRecord);
            }
        }

        private void UpdateHipRecord(t_SerializationHip hipRecord, t_SerializationHip existedRecord)
        {
            if (existedRecord !=null && hipRecord.object_data != existedRecord.object_data)
            {
                existedRecord.object_data = hipRecord.object_data;
                existedRecord.UpdateOle();
            }
        }

        private void InsertHiRecord(t_SerializationHip hipRecord)
        {
            hipRecord.CreateOle();
        }

        private bool CheckIfRecordAlreadyExist(ref t_SerializationHip searchHip, ref t_SerializationHip existedRecord)
        {
            if (searchHip == null)
            {
                Log("CheckIfRecordAlreadyExist error searchHip is null");
                return false;
            }

           List<t_SerializationHip> validtasks = GetValidTasks();
            t_SerializationHip hipRecord=searchHip;

            var temp=validtasks.Where(a => IsEquals(a,hipRecord));
                
            if (temp.NotNullOrEmpty())
            {
                var tmp = temp.First();
                existedRecord = tmp;
                //tmp.object_data = searchHip.object_data;
               // searchHip =tmp;
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool IsEquals(t_SerializationHip a,t_SerializationHip b)
        {
            if (a == null || b == null) return false;
            if (a.object_id == b.object_id &&
                a.object_name == b.object_name &&
                a.object_type == b.object_type &&
                a.related_doc_id == b.related_doc_id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<t_SerializationHip> GetValidTasks()
        {
            List<t_SerializationHip> validtasks = new t_SerializationHip().Select<t_SerializationHip>("isvalid=-1").ToList();
            return validtasks;
        }


        private t_SerializationHip CreateHipRecord<T>(T webService1CStateClass) where T:WebService1CStateClass
        {
            t_SerializationHip shiprecrd = null;
            
            new TryAction1((o) =>
            {
                shiprecrd = new t_SerializationHip();
                shiprecrd.object_name = webService1CStateClass.FunctionName;
                shiprecrd.object_type = webService1CStateClass.ServiceType;
                shiprecrd.object_data = Serializer.BinarySerialize(webService1CStateClass.Params);
                shiprecrd.date_added = webService1CStateClass.CreationDate;
                shiprecrd.isvalid = true;
                shiprecrd.related_doc_id = webService1CStateClass.order.Id;
            }, null).Start();

            return shiprecrd;
        }

        internal void UnValidSerializedDocument(OrderClass orderClass)
        {
            
            t_SerializationHip result = FindState(orderClass);
            if (result != null)
            {
                result.isvalid = false;
                result.UpdateOle();

                DeleteSerializedDocument(result);
            }
        }

        public void DeleteSerializedDocument(t_SerializationHip result)
        {
            result.Delete();
        }

        internal t_SerializationHip FindDocState(OrderClass orderClass)
        {
            t_SerializationHip result = FindState(orderClass);
            return result;
        }

        private static t_SerializationHip FindState(OrderClass orderClass)
        {
            t_SerializationHip result = new t_SerializationHip().SelectFirst<t_SerializationHip>(String.Format("isvalid=-1 AND related_doc_id={0}", orderClass.Id));
            return result;
        }
    }
}
