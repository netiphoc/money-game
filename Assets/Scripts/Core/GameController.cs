using Utilities;

namespace Core
{
    public class GameController : BaseMonoBehaviour
    {
        protected override void Start()
        {
            base.Start();
            
            foreach (var room in GameManager.Instance.GymRooms)
            {
                room.OnRoomUnlocked += OnRoomUnlocked;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (var room in GameManager.Instance.GymRooms)
            {
                room.OnRoomUnlocked -= OnRoomUnlocked;
            }
        }

        public void StartTheDay()
        {
            GameManager.Instance.GameTimeManager.StartClock();

            foreach (var room in GameManager.Instance.GymRooms)
            {
                if(room.assignedBoxer == null) continue;
                if(!room.IsUnlocked) continue;
                room.assignedBoxer.StartVisualCycle();
            }
        }

        private void OnRoomUnlocked(GymRoom room)
        {
            room.assignedBoxer.StartVisualCycle();
        }

        public void EndTheDay()
        {
        }
    }
}