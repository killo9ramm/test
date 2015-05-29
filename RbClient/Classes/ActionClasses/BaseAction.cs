using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient.Classes.CustomClasses
{
    

    public abstract class BaseAction
    {
        public PassObject StateChangedEvent;    //Делегат события изменения статуса
        public NLogDelegate LogEvent; //событие логирования
        public Action<string> LogEventt; //событие логирования

        protected Action<object> InnerAction;
        protected object Parameter;

        protected StateEnum _State;
        public StateEnum State
        {
            get
            {
                return _State;
            }
        }

        public CustomAction CurrentAction;
        public List<CustomAction> ActionList;

        internal int _Tries = 0;
        public int Tries { get { return _Tries; } set { _Tries=value;} }

        internal int _MaxTries = 10;
        public int MaxTries { get { return _MaxTries; } set { _MaxTries=value;} }

        /// <summary>
        /// Таймаут в миллисекундах
        /// </summary>
        internal int _Timeout = 500;
        public int Timeout { get { return _Timeout; } set { _Timeout=value;} }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="act"></param>
        /// <param name="param"></param>
        public BaseAction(Action<object> act,object param)
        {
            InnerAction = act;
            Parameter = param;
            ChangeState(StateEnum.Created);
        }

        /// <summary>
        /// Изменяем статус объекта
        /// </summary>
        /// <param name="sta"></param>
        internal void ChangeState(StateEnum sta)
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
        internal void Log(string message)
        {
            if (LogEvent != null)
                LogEvent(message);

            if (LogEventt != null)
                LogEventt(message);
        }

        /// <summary>
        /// выводим лог
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exp"></param>
        internal void Log(string message, Exception exp)
        {
            if (LogEvent != null)
                LogEvent(message + " exception: " + exp.Message);

            if (LogEventt != null)
                LogEventt(message + " exception: " + exp.Message);
        }

        public virtual void Start()
        {
            
        }

    }
}
