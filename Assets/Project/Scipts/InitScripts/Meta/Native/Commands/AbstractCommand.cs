using System;

namespace Project.Meta
{
    public abstract class AbstractCommand : ICommand
    {
        public event Action<ICommand, bool> Completed = null;

        public bool IsCompleted
        {
            get; 
            private set;
        } = false;
        
        public abstract void Execute();

        protected void OnCompleted(bool result)
        {
            IsCompleted = true;
            Completed?.Invoke(this, result);
        }
    }
}