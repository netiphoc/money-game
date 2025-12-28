using UnityEngine;

namespace UI.Tablet.Management
{
    public class UITabletButtonCatalog : BaseUIButton
    {
        [SerializeField] private SectionDataSO[] visibleSections;

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
    }
}