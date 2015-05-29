using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RBClient.Classes;
using System.Threading;

namespace RBClient.Classes.CustomClasses
{
    public class CustomAction
    {
        public PassObject StateChangedEvent;    //Делегат события изменения статуса
        public NLogDelegate LogEvent; //событие логирования

        private Action<object> InnerAction;   
        private object Parameter;
        
        private StateEnum _State;
        public StateEnum State
        {
            get
            {
                return _State;
            }
        }

        public CustomAction CurrentAction;
        public List<CustomAction> ActionList;

        private int _Tries = 0;
        public int Tries { get { return _Tries; } set { _Tries=value;} }

        private int _MaxTries = 10;
        public int MaxTries { get { return _MaxTries; } set { _MaxTries=value;} }

        /// <summary>
        /// Таймаут в миллисекундах
        /// </summary>
        private int _Timeout = 500;
        private Action<object> action;
        public int Timeout { get { return _Timeout; } set { _Timeout=value;} }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="act"></param>
        /// <param name="param"></param>
        public CustomAction(Action<object> act,object param)
        {
            InnerAction = act;
            Parameter = param;
            ChangeState(StateEnum.Created);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="act"></param>
        /// <param name="param"></param>
        public CustomAction(List<Action<object>> act, List<object> param)
        {
            if (act.Count != param.Count)
            {
                ChangeState(StateEnum.Error);
                return;
            }
            for (int i = 0; i < act.Count; i++)
            {
                if (act[i] != null && param[i] != null)
                {
                    CustomAction CA = new CustomAction(act[i], param[i]);
                    ActionList.Add(CA);
                }
            }
            ChangeState(StateEnum.Created);
        }


        /// <summary>
        /// Изменяем статус объекта
        /// </summary>
        /// <param name="sta"></param>
        private void ChangeState(StateEnum sta)
        {
            _State = sta;
            if(StateChangedEvent!=null)
            {
                try
                {
                    StateChangedEvent(this);
                }
                catch (Exception ex)
                {
                    ChangeState(StateEnum.Error);
                }
            }
        }

        /// <summary>
        /// выводим лог
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message)
        {
            if (LogEvent != null)
            {
                message += "CustomAction Log: " + message;
                LogEvent(message);
            }
        }

        /// <summary>
        /// выводим лог
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exp"></param>
        private void Log(string message, Exception exp)
        {
            if (LogEvent != null)
                LogEvent(message + " exception: " + exp.Message);
        }

        public Dictionary<string, Exception> EList = new Dictionary<string,Exception>();

        /// <summary>
        /// Рекурсивный метод выполнения действия
        /// </summary>
        /// <param name="fileSource"></param>
        /// <param name="folderDest"></param>
        private void PerformSingleAction()
        {
            if (Tries == 0) ChangeState(StateEnum.Started);

            try
            {
                InnerAction(Parameter);
                ChangeState(StateEnum.SuccessfulComplete);
            }
            catch (Exception exp)
            {
                if (this.Tries > MaxTries)
                {
                    ChangeState(StateEnum.ErrorTriesEnded);
                    return;
                }
                else
                {
                    Log("cannot perform sa. nTry=" + this.Tries, exp);
                    Thread.Sleep(Timeout);
                    this.Tries++;
                    EList.Add("nTry=" + this.Tries,exp);
                    ChangeState(StateEnum.RetryAction);
                    PerformSingleAction();
                    return;
                }
            }
        }

        /// <summary>
        /// Выполняем поочередно задания
        /// </summary>
        private void PerformMultipleActions()
        {
            ActionList.ForEach(a => a.PerformSingleAction());
        }

        public void Start()
        {
            if (State == StateEnum.Error)
            {
                Log("Unable to perfom action. Internal Error. ReCreate Class.");
                return;
            }
            //логика для выбора алгоритма
            
            if (InnerAction == null && ActionList != null)
            {
                PerformMultipleActions();
            }

            if (InnerAction != null && ActionList == null)
            {
                PerformSingleAction();
            }
        }

        /// <summary>
        /// Проверка успешно ли отработало действие
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            if (State == StateEnum.ErrorTriesEnded || State == StateEnum.Error)
                return false;
            return true;
        }
    }

    public class TryAction : BaseAction
    {
        Action CatchAction;
        public TryAction(Action<object> act, object param) : base(act,param)
        {
        }
        public TryAction(Action<object> act, object param,Action _CatchAction):this(act,param)
        {
            CatchAction = _CatchAction;
        }

        public override void Start()
        {
            try
            {
                ChangeState(StateEnum.Started);
                InnerAction(Parameter);
                ChangeState(StateEnum.SuccessfulComplete);
            }catch(Exception ex)
            {
                Log("TryAction error ", ex);
                ChangeState(StateEnum.Error);
                if (CatchAction != null)
                    CatchAction();
            }
        }
    }

    public class TryAction1 : BaseAction
    {
        Action<Exception> CatchAction;
        public TryAction1(Action<object> act, object param) : base(act,param)
        {
        }
        public TryAction1(Action<object> act, object param, Action<Exception> _CatchAction)
            : this(act, param)
        {
            CatchAction = _CatchAction;
        }

        public override void Start()
        {
            try
            {
                ChangeState(StateEnum.Started);
                InnerAction(Parameter);
                ChangeState(StateEnum.SuccessfulComplete);
            }catch(Exception ex)
            {
                Log("TryAction error ", ex);
                ChangeState(StateEnum.Error);
                if (CatchAction != null)
                    CatchAction(ex);
            }
        }
    }

}
