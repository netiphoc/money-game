using System;
using Data;
using TMPro;
using UnityEngine;

namespace UI.Tablet.Management
{
    public class UITabletButtonRoom : BaseUIButton
    {
        [SerializeField] private TMP_Text textRoomName;
        
        public Action<RoomDataSO> OnClickRoomData;
        public RoomDataSO RoomDataSo { get; private set; }

        public void SetRoomData(RoomDataSO roomDataSo)
        {
            RoomDataSo = roomDataSo;
            SetRoomName(roomDataSo.roomName);
        }
        
        private void SetRoomName(string roomName)
        {
            textRoomName.SetText(roomName);
        }

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
            OnRoomDataLoaded();
        }

        public void OnRoomDataLoaded()
        {
            OnClickRoomData?.Invoke(RoomDataSo);
        }
    }
}