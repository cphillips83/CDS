using System;
using System.Threading.Tasks;

namespace CDS
{
    public abstract class Task2
    {
        //public virtual bool IsCompleted { get; protected set; }
        protected Task _task;

        public Task ProcessAsync()
        {
            _task = Task.Run(new Action(Execute));
            return _task;
        }

        public void Process()
        {
            Execute();
        }

        public virtual bool Wait(int ms = -1)
        {
            if (_task == null)
                return true;
            return _task.Wait(ms);
        }

        protected abstract void Execute();
    }

}
