using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tablet.Leagues
{
    public class UITabletLeagues : BaseUI
    {
        [Header("Data")] 
        [SerializeField] private GymRoom[] gymRooms;
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

            List<BoxerController> boxerControllers = new List<BoxerController>();
            foreach (var gymRoom in gymRooms)
            {
               if(!gymRoom.assignedBoxer) continue; 
               boxerControllers.Add(gymRoom.assignedBoxer);
            }
            
            uiTabletLeaguesBoxer.RefreshBoxerLeaguesSlot(boxerControllers.ToArray());
            uiTabletLeaguesFighter.RefreshFightLeaguesSlot(boxerControllers[0], opponents);
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
        
        public void OnClickedBoxer(BoxerController boxerController)
        {
            uiTabletLeaguesFighter.RefreshFightLeaguesSlot(boxerController, opponents);
        }

        public void OnFlightResult(FightResultData fightResultData)
        {
            uiTabletLeaguesFightLive.SetVisible(true);
            //uiTabletLeaguesFightLive.ShowLiveResult(fightResultData);
        }
    }
}