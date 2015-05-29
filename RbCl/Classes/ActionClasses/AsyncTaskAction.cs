using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using CustomLogger;
using System.Threading;

namespace RBClient.Classes.CustomClasses
{
    class AsyncTaskAction
    {
        //public delegate string AsyncMethodCaller(int callDuration, out int threadId);
     //   public delegate string AsyncMethodCaller(out object Result);
        Func<object> act;

        private object _returnValue = null;
        public object Result
        {
            get
            {
                if (state != StateEnum.SuccessfulComplete)
                {
                    return null;
                }
                else
                {
                    return _returnValue;
                }
            }
        }
        public StateEnum state;

        public AsyncTaskAction(Func<object> act)
        {
            this.act = act;
            state = StateEnum.Created;
        }

        public AsyncTaskAction Start()
        {
            state = StateEnum.Started;
            IAsyncResult result = act.BeginInvoke(CallbackMethod,null);
            return this;
        }
        private void CallbackMethod(IAsyncResult ar)
        {
            AsyncResult result = (AsyncResult)ar;
            state = StateEnum.Working;
            Func<object> caller = (Func<object>)result.AsyncDelegate;
            state = StateEnum.SuccessfulComplete;
            _returnValue = caller.EndInvoke(ar);
        }
    }

    class AsyncTaskAction1
    {
        //public delegate string AsyncMethodCaller(int callDuration, out int threadId);
        //   public delegate string AsyncMethodCaller(out object Result);
        Action<object> act;


        public StateEnum state;
        object param = null;

        public AsyncTaskAction1(Action<object> act, object param)
        {
            this.act = act;
            this.param = param;
            state = StateEnum.Created;
        }

        public AsyncTaskAction1 Start()
        {
            state = StateEnum.Started;
            IAsyncResult result = act.BeginInvoke(param,CallbackMethod, null);
            return this;
        }
        private void CallbackMethod(IAsyncResult ar)
        {
            AsyncResult result = (AsyncResult)ar;
            state = StateEnum.Working;
            Action<object> caller = (Action<object>)result.AsyncDelegate;
            state = StateEnum.SuccessfulComplete;
            caller.EndInvoke(ar);
        }
    }

    class AsyncTaskAction2 : LoggerBase
    {
        Action act;
        public StateEnum state;

        public AsyncTaskAction2(Action act)
        {
            this.act = act;
            state = StateEnum.Created;
        }

        public AsyncTaskAction2 Start()
        {
            state = StateEnum.Started;
            ThreadPool.QueueUserWorkItem((o)=>{
                try
                {
                    state = StateEnum.Working;
                    act();
                    CallbackMethod();
                }catch(Exception ex)
                {
                    Log(ex, "AsyncTaskAction2 error");
                    state = StateEnum.Error;
                }
            });
            return this;
        }
        private void CallbackMethod()
        {
            state = StateEnum.SuccessfulComplete;
        }
    }
}
