using UniRx;

namespace Project.Meta
{
    public interface IUser
    {
        public IReadOnlyReactiveProperty<int> Coins
        {
            get;
        }
        
        bool CanUpgrade(CurrencyType type, int amount);
        void SetCurrency(CurrencyType type, int amount);
    }
}