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
        private RoomDataSO _roomDataSo;

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
            _roomDataSo = roomDataSo;
            SetRoomName(roomDataSo.roomName);
        }
        
        private void SetRoomName(string roomName)
        {
            textRoomName.SetText(roomName);
        }
        
        public void OnRoomDataLoaded()
        {
            OnClickRoomData?.Invoke(_roomDataSo);
        }
    }
}