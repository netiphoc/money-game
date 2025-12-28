using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tablet.Leagues
{
    public class UITabletLeaguesFightLive : BaseUI
    {
        [SerializeField] private Button buttonClose;
        protected override void OnEnable()
        {
            base.OnEnable();
            buttonClose.onClick.AddListener(OnButtonClickedClose);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            buttonClose.onClick.RemoveListener(OnButtonClickedClose);
        }
        
        private void OnButtonClickedClose()
        {
            SetVisible(false);
        }

        public void ShowLiveResult(FightResultData fightResultData)
        {
            Debug.Log("LIVE: Fight result...");
        }
    }
}