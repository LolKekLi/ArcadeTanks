using Zenject;

namespace Project.Meta
{
    public class SaveStorageDataCommand : AbstractCommand
    {
        public class Factory : PlaceholderFactory<IStoragable, SaveStorageDataCommand>
        {
            
        }

        [Inject] 
        private Storage _storage = null;

        private IStoragable _storagable = null;
        
        public SaveStorageDataCommand(IStoragable storagable)
        {
            _storagable = storagable;
        }

        public override void Execute()
        {
            _storage.Save(_storagable);
            OnCompleted(true);
        }
    }
}
