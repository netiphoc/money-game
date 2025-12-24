using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class GameplayManager : MonoBehaviour
    {
        private const float ONE_SECONDS = 1f;
        
        [SerializeField] private ShopData[] availableShop;

        public double Money;
        public double UnrealizeIncome { get; set; }
        public double TotalInComePerSecond { get; private set; }

        private List<ShopData> _shopData;

        private float _timer;
        private void Awake()
        {
            _shopData = new List<ShopData>();
        }
        
        public void AddShop(ShopData shopData)
        {
            _shopData.Add(shopData);
            ReCalculateTotalIncome();
        }

        public int GetTotalShop()
        {
            return _shopData.Count;
        }

        public ShopData GetNextShop()
        {
            int index = _shopData.Count;
            return availableShop[index];
        }

        public bool IsAllShopUnlocked()
        {
            return GetTotalShop() >= availableShop.Length;
        }

        private void ReCalculateTotalIncome()
        {
            foreach (var shopData in _shopData)
            {
                TotalInComePerSecond += shopData.GetTotalIncomePerSecond();
            }
        }
        
        public void UpdateRoi()
        {
            _timer += Time.deltaTime;
            if(_timer < ONE_SECONDS) return;
            _timer = 0f;
            
            UnrealizeIncome += TotalInComePerSecond;
            Debug.Log($"UnrealizeIncome: ${UnrealizeIncome}");
        }
    }
}