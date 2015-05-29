using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace RBClient.Classes.CustomClasses
{
    class AccessDataBaseAction : BaseAction
    {
        Action<OleDbConnection> dbact;
        public AccessDataBaseAction(Action<object> act, object param)
            : base(act, param)
        {
        }

        public override void Start()
        {
            ChangeState(StateEnum.Started);

            string _str_connect = CParam.ConnString;
            OleDbConnection _conn = null;

            try
            {
                _conn = new OleDbConnection(_str_connect);
                _conn.Open();
                if (Parameter == null)
                {
                    InnerAction(_conn);
                }
                else
                {
                    InnerAction(Parameter);
                }
                ChangeState(StateEnum.SuccessfulComplete);
            }
            catch (Exception _exp)
            {
                Log("AccessDataBaseAction error", _exp);
                ChangeState(StateEnum.Error);
            }
            finally
            {
                if (_conn != null)
                    _conn.Close();
            }
        }
    }
}
