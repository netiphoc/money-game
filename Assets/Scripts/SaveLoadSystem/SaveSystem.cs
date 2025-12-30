using UnityEngine;

namespace SaveLoadSystem
{
    public class SaveSystem : MonoBehaviour
    {
        [SerializeField] private BoxerController boxerControllerPrefab;
        [SerializeField] private ItemDataSO[] allItems;
        
        public static SaveSystem Instance;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public BoxerController LoadBoxerData(BoxerData boxerData, Transform container)
        {
            BoxerController boxer = Instantiate(boxerControllerPrefab, container);
            boxer.stats = boxerData;
            return boxer;
        }

        public ItemDataSO GetItemDataFromItemName(string itemName)
        {
            foreach (var item in allItems)
            {
                if(!item.itemName.Equals(itemName)) continue;
                return item;
            }

            return default;
        }
    }
}