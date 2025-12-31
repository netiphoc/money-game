using SaveLoadSystem;
using UI;
using Utilities;

namespace Core
{
    public class GameController : BaseMonoBehaviour
    {
        public bool IsTheDayStarted { get; private set; }

        protected override void Start()
        {
            base.Start();
            
            foreach (var room in GameManager.Instance.GymRooms)
            {
                room.OnRoomUnlocked += OnRoomUnlocked;
            }
            
            SaveSystem.Instance.LoadAll();
        }

        private void OnApplicationQuit()
        {
            SaveSystem.Instance.SaveAll();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (var room in GameManager.Instance.GymRooms)
            {
                room.OnRoomUnlocked -= OnRoomUnlocked;
            }
        }
        
        private void OnRoomUnlocked(GymRoom room)
        {
            if(!IsTheDayStarted) return;
            room.assignedBoxer.StartVisualCycle();
        }

        #region Day Controller

        public void StartTheDay()
        {
            GameManager.Instance.GameTimeManager.StartClock();

            foreach (var room in GameManager.Instance.GymRooms)
            {
                if(room.assignedBoxer == null) continue;
                if(!room.IsUnlocked) continue;
                room.assignedBoxer.StartVisualCycle();
            }

            IsTheDayStarted = true;
        }

        public void EndTheDay()
        {
            UIManager.Instance.ShowUI(UIManager.Instance.UISummary);
            UIManager.Instance.LockUIInput = true;
            GameManager.Instance.AddDay();
            SaveSystem.Instance.SaveAll();
        }

        #endregion
    }
}