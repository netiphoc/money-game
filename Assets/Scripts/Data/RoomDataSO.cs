using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Room Data", menuName = "GymTycoon/Room Data")]
    public class RoomDataSO : ScriptableObject
    {
        public SectionDataSO sectionData;
        public string roomName;
        public LicenseDataSO[] licenses;
        public ItemDataSO[] furnitures;
    }
}