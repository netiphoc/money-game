using System.Collections.Generic;
using UnityEngine;

namespace UI.Tablet.Leagues
{
    public class UITabletLeaguesFighter : BaseUI
    {
        [Header("UI")] 
        [SerializeField] private UITabletLeagues uiTabletLeagues;

        [Header("Fight Slot")]
        [SerializeField] private UITabletLeaguesSlot uiTabletLeaguesSlot;
        [SerializeField] private Transform containerLeagueSlot;

        private PoolingUI<UITabletLeaguesSlot> _poolingUI;
        
        private List<UITabletLeaguesSlot> _fightSlots = new List<UITabletLeaguesSlot>();
        
        private PoolingUI<UITabletLeaguesSlot> GetPool()
        {
            return _poolingUI ??= new PoolingUI<UITabletLeaguesSlot>(uiTabletLeaguesSlot);
        }
        
        private UITabletLeaguesSlot GetSlot()
        {
            return GetPool().Request(containerLeagueSlot);
        }

        public void RefreshFightLeaguesSlot(BoxerController boxerController, FightDataSO fightDataSo)
        {
            GetPool().ClearPool();

            int rank = 1;
            
            foreach (var opponent in fightDataSo.opponents)
            {
                UITabletLeaguesSlot slot = GetSlot();
                slot.SetOpponentRank(rank++);
                slot.SetOpponent(boxerController, opponent, fightDataSo);
                slot.OnFightResult = uiTabletLeagues.OnFlightResult;
                _fightSlots.Add(slot);
            }
        }
    }
}