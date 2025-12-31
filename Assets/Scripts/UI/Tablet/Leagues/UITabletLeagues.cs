using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tablet.Leagues
{
    public class UITabletLeagues : BaseUI
    {
        [Header("UI")]
        [SerializeField] private UITablet uiTablet;
        [SerializeField] private UITabletLeaguesBoxer uiTabletLeaguesBoxer;
        [SerializeField] private UITabletLeaguesFighter uiTabletLeaguesFighter;
        [SerializeField] private UITabletLeaguesFightLive uiTabletLeaguesFightLive;
        
        [Header("Boxer")]
        
        [Header("Button")]
        [SerializeField] private Button buttonHome;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            buttonHome.onClick.AddListener(OnButtonClickedHome);
            RefreshBoxers();
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
        
        public void OnClickedBoxer(BoxerController boxerController)
        {
            uiTabletLeaguesFighter.RefreshFightLeaguesSlot(boxerController, FightManager.Instance.OpponentSo);
        }

        private void RefreshBoxers()
        {
            List<BoxerController> boxerControllers = new List<BoxerController>();
            foreach (var gymRoom in GameManager.Instance.GymRooms)
            {
                if(!gymRoom.assignedBoxer) continue; 
                boxerControllers.Add(gymRoom.assignedBoxer);
            }

            if (boxerControllers.Count > 0)
            {
                uiTabletLeaguesBoxer.RefreshBoxerLeaguesSlot(boxerControllers.ToArray());
                uiTabletLeaguesFighter.RefreshFightLeaguesSlot(boxerControllers[0], FightManager.Instance.OpponentSo);
            }
        }
        
        public void OnFlightResult(FightResultData fightResultData)
        {
            uiTabletLeaguesFightLive.SetVisible(true);
            //uiTabletLeaguesFightLive.ShowLiveResult(fightResultData);
        }
    }
}