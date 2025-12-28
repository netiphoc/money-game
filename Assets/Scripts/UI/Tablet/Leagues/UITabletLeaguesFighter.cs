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

        public void RefreshFightLeaguesSlot(BoxerData boxerData, OpponentSO[] opponentSo)
        {
            Debug.Log($"RefreshFightLeaguesSlot: {boxerData.boxerName} Lvl.{boxerData.level}");
            GetPool().ClearPool();

            int rank = 1;
            
            foreach (var opponent in opponentSo)
            {
                UITabletLeaguesSlot slot = GetSlot();
                slot.SetOpponentRank(rank++);
                slot.SetOpponent(boxerData, opponent);
                slot.OnFightResult = uiTabletLeagues.OnFlightResult;
                _fightSlots.Add(slot);
            }
        }
    }
}