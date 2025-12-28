using System;
using UnityEngine;

namespace UI.Tablet.Management
{
    public class UITabletButtonSection : BaseUIButton
    {
        [SerializeField] private SectionDataSO sectionDataSo;

        public Action<SectionDataSO> OnClickedSection;

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
            OnClickedSection?.Invoke(sectionDataSo);
        }
    }
}