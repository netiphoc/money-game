using UnityEngine;

namespace SaveLoadSystem
{
    public class SaveSystem : MonoBehaviour
    {
        [SerializeField] private BoxerController boxerControllerPrefab;
        
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
    }
}