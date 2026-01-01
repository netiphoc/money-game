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

        [Header("Button")] 
        [SerializeField] private Button fighterTierA;
        [SerializeField] private Button fighterTierB;
        [SerializeField] private Button fighterTierC;
        [SerializeField] private Button fighterTierD;

        [Header("Button")]
        [SerializeField] private Button buttonHome;

        private OpponentSO[] _selectedFighterTier;
        private BoxerController _boxer;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            buttonHome.onClick.AddListener(OnButtonClickedHome);
            fighterTierA.onClick.AddListener(OnButtonClickedFighterTierA);
            fighterTierB.onClick.AddListener(OnButtonClickedFighterTierB);
            fighterTierC.onClick.AddListener(OnButtonClickedFighterTierC);
            fighterTierD.onClick.AddListener(OnButtonClickedFighterTierD);
            RefreshBoxers();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            buttonHome.onClick.RemoveListener(OnButtonClickedHome);
            fighterTierA.onClick.RemoveListener(OnButtonClickedFighterTierA);
            fighterTierB.onClick.RemoveListener(OnButtonClickedFighterTierB);
            fighterTierC.onClick.RemoveListener(OnButtonClickedFighterTierC);
            fighterTierD.onClick.RemoveListener(OnButtonClickedFighterTierD);
        }

        private void OnButtonClickedFighterTierA()
        {
            _selectedFighterTier = FightManager.Instance.FighterDataTierA;
            RefreshBoxerOpponents();
        }

        private void OnButtonClickedFighterTierB()
        {
            _selectedFighterTier = FightManager.Instance.FighterDataTierB;
            RefreshBoxerOpponents();
        }

        private void OnButtonClickedFighterTierC()
        {
            _selectedFighterTier = FightManager.Instance.FighterDataTierC;
            RefreshBoxerOpponents();
        }

        private void OnButtonClickedFighterTierD()
        {
            _selectedFighterTier = FightManager.Instance.FighterDataTierD;
            RefreshBoxerOpponents();
        }

        private void OnButtonClickedHome()
        {
            uiTablet.ShowHome();
        }

        private OpponentSO[] GetOpponents()
        {
            return _selectedFighterTier ??= FightManager.Instance.FighterDataTierA;
        }
        
        private void RefreshBoxerOpponents()
        {
            if(_boxer == null) return;
            OnClickedBoxer(_boxer);
        }
        
        public void OnClickedBoxer(BoxerController boxerController)
        {
            _boxer = boxerController;
            uiTabletLeaguesFighter.RefreshFightLeaguesSlot(boxerController,  GetOpponents());
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
                _boxer = boxerControllers[0];
                uiTabletLeaguesBoxer.RefreshBoxerLeaguesSlot(boxerControllers.ToArray());
                uiTabletLeaguesFighter.RefreshFightLeaguesSlot(_boxer,  GetOpponents());
            }
        }
        
        public void OnFlightResult(FightResultData fightResultData)
        {
            uiTabletLeaguesFightLive.SetVisible(true);
            //uiTabletLeaguesFightLive.ShowLiveResult(fightResultData);
        }
    }
}