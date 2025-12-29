using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Room Data", menuName = "GymTycoon/Room Data")]
    public class RoomDataSO : ScriptableObject
    {
        public SectionDataSO sectionData;
        public string roomName;
        public int requiredGymLevel;
        public int unlockCost;
        public LicenseSO[] licenses;
        public ItemDataSO[] furnitures;
    }
}