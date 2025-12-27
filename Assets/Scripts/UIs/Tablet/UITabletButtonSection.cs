using System;
using UnityEngine;

namespace UIs.Tablet
{
    public class UITabletButtonSection : BaseUIButton
    {
        [SerializeField] private SectionDataSO sectionDataSo;

        public Action<SectionDataSO> OnClickedSection;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            ActiveButton.onClick.AddListener(OnButtonClickedSection);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ActiveButton.onClick.RemoveListener(OnButtonClickedSection);
        }
        
        private void OnButtonClickedSection()
        {
            OnClickedSection?.Invoke(sectionDataSo);
        }

    }
}