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

        private UniRxSubscribersContainer _subscribersContainer = new UniRxSubscribersContainer();

        public IReadOnlyReactiveProperty<int> Coins
        {
            get => _coins;
        }

        public IReadOnlyReactiveProperty<int> LevelIndexProperty
        {
            get => _levelIndex;
        }

        public int LevelIndex
        {
            get => _levelIndex.Value; 
            set => _levelIndex.Value = Mathf.Max(0, value);
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

        void IDisposable.Dispose()
        {
            Save();
        }

        void IInitializable.Initialize()
        {
            Load();

            _coins.Value = ConcreteData.MoneyCount;
            _levelIndex.Value = ConcreteData.LevelIndex;
            
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
        }
    }
}