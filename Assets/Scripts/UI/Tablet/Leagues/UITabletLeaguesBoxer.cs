using UnityEngine;

namespace UI.Tablet.Leagues
{
    public class UITabletLeaguesBoxer : BaseUI
    {
        [Header("UI")] 
        [SerializeField] private UITabletLeagues uiTabletLeagues;
        
        [Header("Boxer Slot")]
        [SerializeField] private UITabletButtonLeaguesBoxer uiTabletLeaguesSlot;
        [SerializeField] private Transform containerLeagueSlot;

        private PoolingUI<UITabletButtonLeaguesBoxer> _poolingUI;

        private PoolingUI<UITabletButtonLeaguesBoxer> GetPool()
        {
            return _poolingUI ??= new PoolingUI<UITabletButtonLeaguesBoxer>(uiTabletLeaguesSlot);
        }
        
        private UITabletButtonLeaguesBoxer GetSlot()
        {
            return GetPool().Request(containerLeagueSlot);
        }

        public void RefreshBoxerLeaguesSlot(BoxerController[] boxerData)
        {
            GetPool().ClearPool();

            foreach (var boxer in boxerData)
            {
                UITabletButtonLeaguesBoxer slot = GetSlot();
                slot.SetBoxer(boxer);
                slot.OnClickedBoxer = uiTabletLeagues.OnClickedBoxer;
            }
        }
    }
}