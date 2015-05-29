using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RBClient.Classes.CustomClasses
{
    class TimeOutAction : BaseAction
    {
        public TimeOutAction(Action<object> act, object param)
            : base(act, param)
        { }

        public Thread InnerThread = null;

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
            catch (ThreadAbortException ex)
            {
                ChangeState(StateEnum.Error);
                return;
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
                    this.Tries++;
                    ChangeState(StateEnum.RetryAction);
                    PerformSingleAction();
                    return;
                }
            }
        }
        public override void Start()
        {
            if (InnerThread != null) { InnerThread.Abort(); InnerThread = null; }
            InnerThread = new Thread(new ThreadStart(PerformSingleAction));
            InnerThread.Start();
            bool finished = InnerThread.Join(Timeout);

            if (!finished)
            {
                InnerThread.Abort();
            }
        }
    }
}
