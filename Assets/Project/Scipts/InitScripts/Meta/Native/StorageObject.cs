using Zenject;

namespace Project.Meta
{
    public class StorageObject<T> : StorageObject where T : StorageData, new()
    {
        public T ConcreteData
        {
            get => StorageData as T;
        }

        public override string Key
        {
            get => typeof(T).ToString();
        }

        public StorageObject()
        {
            StorageData = new T();
        }
    }
    
    public abstract class StorageObject : IStoragable
    {
        [Inject] 
        private LoadStorageDataCommand.Factory _loadCommandFactory = null;
        [Inject] 
        private SaveStorageDataCommand.Factory _saveCommandFactory = null;

        public StorageData StorageData
        {
            get;
            set;
        } = null;

        public abstract string Key
        {
            get;
        }

        public void Save()
        {
            ExecuteCommand(_saveCommandFactory.Create(this));
        }

        public void Load()
        {
            ExecuteCommand(_loadCommandFactory.Create(this));
        }

        private void ExecuteCommand(ICommand command)
        {
            command.Completed += OnComplete;
            command.Execute();
        }

        protected virtual void OnComplete(ICommand command, bool result)
        {
            command.Completed -= OnComplete;
        }
    }
}
