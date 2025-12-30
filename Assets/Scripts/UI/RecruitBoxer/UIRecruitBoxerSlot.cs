using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.RecruitBoxer
{
    public class UIRecruitBoxerSlot : BaseUI
    {
        [SerializeField] private TMP_Text textName;
        [SerializeField] private Image icoAvatar;
        [SerializeField] private UIRecruitBoxerButton buttonUnlock;
        
        public Action<BoxerData> OnClickUnlock;

        private BoxerData _boxer;
        protected override void OnEnable()
        {
            base.OnEnable();
            buttonUnlock.onClick.AddListener(OnButtonClickedUnlock);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            buttonUnlock.onClick.RemoveListener(OnButtonClickedUnlock);
        }

        private void OnButtonClickedUnlock()
        {
            OnClickUnlock?.Invoke(_boxer);
        }

        public void SetBoxer(BoxerData boxerData)
        {
            _boxer = boxerData;
            icoAvatar.sprite = boxerData.avatar;
            textName.SetText($"{boxerData.boxerName}");
        }
    }
}