using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadSystem
{
    public class SaveSystem : MonoBehaviour
    {
        [Header("Save Components")] 
        [SerializeField] private GameObject[] saveLoadSystems;
        
        [Header("Prefabs")]
        [SerializeField] private BoxerController boxerControllerPrefab;
        [SerializeField] private ItemBox itemBox;
        [SerializeField] private ItemDataSO[] allItems;
        
        public static SaveSystem Instance;
        private readonly List<ISaveLoadSystem> _saveLoadSystems = new List<ISaveLoadSystem>();
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            InitSaveLoadComponents();
        }

        private void InitSaveLoadComponents()
        {
            foreach (var go in saveLoadSystems)
            {
                if(!go.TryGetComponent(out ISaveLoadSystem saveLoadSystem)) continue;
                _saveLoadSystems.Add(saveLoadSystem);
            }
        }

        public void LoadAll()
        {
            foreach (var saveLoadSystem in _saveLoadSystems)
            {
                saveLoadSystem.LoadGame();
            }
        }
        
        public void SaveAll()
        {
            foreach (var saveLoadSystem in _saveLoadSystems)
            {
                saveLoadSystem.SaveGame();
            }
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

        public ItemBox GetItemBox()
        {
            return Instantiate(itemBox);
        }
    }
}