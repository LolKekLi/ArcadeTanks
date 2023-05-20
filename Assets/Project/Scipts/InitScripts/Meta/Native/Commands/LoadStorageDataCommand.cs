using Zenject;

namespace Project.Meta
{
    public class LoadStorageDataCommand : AbstractCommand
    {
        public class Factory : PlaceholderFactory<IStoragable, LoadStorageDataCommand>
        {
            
        }
        
        [Inject] 
        private Storage _storage = null;
        
        private IStoragable _storagable = null;
        
        public LoadStorageDataCommand(IStoragable storagable)
        {
            _storagable = storagable;
        }

        public override void Execute()
        {
            if (!_storage.HasKey(_storagable.Key))
            {
                _storage.Save(_storagable);
            }
            else
            {
                _storage.Load(_storagable);
            }

            OnCompleted(true);
        }
    }
}
