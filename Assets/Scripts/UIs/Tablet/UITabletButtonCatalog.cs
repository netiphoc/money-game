using System;
using UnityEngine;

namespace UIs.Tablet
{
    public class UITabletButtonCatalog : BaseUIButton
    {
        [SerializeField] private SectionDataSO[] visibleSections;

        public Action OnClickedCatalog;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            ActiveButton.onClick.AddListener(OnButtonClickedCatalog);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ActiveButton.onClick.RemoveListener(OnButtonClickedCatalog);
        }

        public void CheckSection(SectionDataSO sectionDataSo)
        {
            foreach (var data in visibleSections)
            {
                if(!sectionDataSo.Equals(data)) continue;
                SetVisible(true);
                return;
            }
            
            SetVisible(false);
        }
        
        private void OnButtonClickedCatalog()
        {
            OnClickedCatalog?.Invoke();
        }

    }
}