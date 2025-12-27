using System;
using Data;
using TMPro;
using UnityEngine;

namespace UIs.Tablet
{
    public class UITabletButtonRoom : BaseUIButton
    {
        [SerializeField] private TMP_Text textRoomName;
        
        public Action<RoomDataSO> OnClickRoomData;
        public RoomDataSO RoomDataSo { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            ActiveButton.onClick.AddListener(OnRoomDataLoaded);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ActiveButton.onClick.RemoveListener(OnRoomDataLoaded);
        }

        public void SetRoomData(RoomDataSO roomDataSo)
        {
            RoomDataSo = roomDataSo;
            SetRoomName(roomDataSo.roomName);
        }
        
        private void SetRoomName(string roomName)
        {
            textRoomName.SetText(roomName);
        }
        
        public void OnRoomDataLoaded()
        {
            OnClickRoomData?.Invoke(RoomDataSo);
        }
    }
}