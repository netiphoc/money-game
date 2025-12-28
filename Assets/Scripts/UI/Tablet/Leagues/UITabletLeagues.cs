using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tablet.Leagues
{
    public class UITabletLeagues : BaseUI
    {
        [Header("Data")] 
        [SerializeField] private BoxerData[] testBoxers;
        [SerializeField] private OpponentSO[] opponents;
        
        [Header("UI")]
        [SerializeField] private UITablet uiTablet;
        [SerializeField] private UITabletLeaguesBoxer uiTabletLeaguesBoxer;
        [SerializeField] private UITabletLeaguesFighter uiTabletLeaguesFighter;
        [SerializeField] private UITabletLeaguesFightLive uiTabletLeaguesFightLive;
        
        [Header("Boxer")]
        
        [Header("Button")]
        [SerializeField] private Button buttonHome;

        protected override void Awake()
        {
            base.Awake();
            
            uiTabletLeaguesBoxer.RefreshBoxerLeaguesSlot(testBoxers);
            uiTabletLeaguesFighter.RefreshFightLeaguesSlot(testBoxers[0], opponents);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            buttonHome.onClick.AddListener(OnButtonClickedHome);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            buttonHome.onClick.RemoveListener(OnButtonClickedHome);
        }

        private void OnButtonClickedHome()
        {
            uiTablet.ShowHome();
        }
        
        public void OnClickedBoxer(BoxerData boxerData)
        {
            uiTabletLeaguesFighter.RefreshFightLeaguesSlot(boxerData, opponents);
        }

        public void OnFlightResult(FightResultData fightResultData)
        {
            uiTabletLeaguesFightLive.SetVisible(true);
            uiTabletLeaguesFightLive.ShowLiveResult(fightResultData);
        }
    }
}