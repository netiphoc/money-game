using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private GameplayManager gameplayManager;

        private void Update()
        {
            gameplayManager.UpdateRoi();

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                AddShop();
            }
            
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                CustomerByItem();
            }
        }
        
        public void AddShop()
        {
            if (gameplayManager.IsAllShopUnlocked())
            {
                Debug.Log("ALL UNLOCKED!");
                return;
            }

            ShopData nextShop = gameplayManager.GetNextShop();
            double nextShopCost = nextShop.baseCost * System.Math.Pow(nextShop.costMultiplier, gameplayManager.GetTotalShop());

            if (gameplayManager.Money >= nextShopCost)
            {
                gameplayManager.Money -= nextShopCost;
                gameplayManager.AddShop(nextShop);
                Debug.Log($"BUY {nextShop.shopName} SUCCESS! (-${nextShopCost})");
            }
            else
            {
                Debug.Log($"BUY {nextShop.shopName} FAILED! (YOU NEED ${nextShopCost})");
            }
        }

        private void CustomerByItem()
        {
            double buyPrice = Random.Range(5, 10);
            if(gameplayManager.UnrealizeIncome < buyPrice) return;
            gameplayManager.UnrealizeIncome -= buyPrice;
            gameplayManager.Money += buyPrice;
            Debug.Log($"Profit: ${gameplayManager.Money}");
        }
    }
}