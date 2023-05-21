using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Project.Meta
{
    public class User : StorageObject<UserStorageData>, IUser, ILevelData, IDisposable, IInitializable
    {
        private ReactiveProperty<int> _coins = new ReactiveProperty<int>(0);
        private ReactiveProperty<int> _levelIndex = new ReactiveProperty<int>(0);
        private ReactiveProperty<TurretType> _turretType = new ReactiveProperty<TurretType>(0);
        private ReactiveProperty<BodyType> _bodyType = new ReactiveProperty<BodyType>(0);

        private UniRxSubscribersContainer _subscribersContainer = new UniRxSubscribersContainer();

        public IReadOnlyReactiveProperty<int> Coins
        {
            get =>
                _coins;
        }

        public IReadOnlyReactiveProperty<BodyType> BodyType
        {
            get => _bodyType;
        }

        public IReadOnlyReactiveProperty<TurretType> TurretType
        {
            get => _turretType;
        }

        public IReadOnlyReactiveProperty<int> LevelIndexProperty
        {
            get =>
                _levelIndex;
        }

        public int LevelIndex
        {
            get =>
                _levelIndex.Value;
            set =>
                _levelIndex.Value = Mathf.Max(0, value);
        }

        bool IUser.CanUpgrade(CurrencyType type, int amount)
        {
            bool canPurchase = false;

            switch (type)
            {
                case CurrencyType.Coin:
                    canPurchase = amount <= _coins.Value;
                    break;

                default:
                    canPurchase = true;
                    break;
            }

            return canPurchase;
        }

        void IUser.SetCurrency(CurrencyType type, int amount)
        {
            if (type == CurrencyType.Coin)
            {
                _coins.Value += amount;
            }
        }
        
        void IUser.SetTurretType(TurretType type)
        {
            _turretType.Value = type;
        }

        void IUser.SetBodyType(BodyType type)
        {
            _bodyType.Value = type;
        }

        void IDisposable.Dispose()
        {
            Save();
        }

        void IInitializable.Initialize()
        {
            Load();

            _coins.Value = ConcreteData.MoneyCount;
            _levelIndex.Value = ConcreteData.LevelIndex;
            _bodyType.Value = ConcreteData.BodyType;
            _turretType.Value = ConcreteData.TurretType;

            _subscribersContainer.Subscribe(Coins, i =>
            {
                ConcreteData.MoneyCount = i;

                Save();
            });

            _subscribersContainer.Subscribe(LevelIndexProperty, i =>
            {
                ConcreteData.LevelIndex = i;

                Save();
            });

            _subscribersContainer.Subscribe(_turretType, type =>
            {
                ConcreteData.TurretType = type;
                Save();
            });

            _subscribersContainer.Subscribe(_bodyType, type =>
            {
                ConcreteData.BodyType = type;
                Save();
            });
        }
    }
}