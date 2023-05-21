using UniRx;

namespace Project.Meta
{
    public interface IUser
    {
        public IReadOnlyReactiveProperty<int> Coins
        {
            get;
        }
        public IReadOnlyReactiveProperty<BodyType> BodyType
        {
            get;
        }
        public IReadOnlyReactiveProperty<TurretType> TurretType
        {
            get;
        }

        void SetTurretType(TurretType type);
        void SetBodyType(BodyType type);

        bool CanUpgrade(CurrencyType type, int amount);
        void SetCurrency(CurrencyType type, int amount);
    }
}