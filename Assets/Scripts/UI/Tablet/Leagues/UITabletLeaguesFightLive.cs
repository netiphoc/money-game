using TMPro;
using UI.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tablet.Leagues
{
    public class UITabletLeaguesFightLive : BaseUI
    {
        [SerializeField] private Button buttonClose;
        [SerializeField] private TMP_Text textResult;
        [SerializeField] private UITVFightBar uiTvFightBar;
        
        private FightData _fightData;

        protected override void OnEnable()
        {
            base.OnEnable();
            buttonClose.onClick.AddListener(OnButtonClickedClose);
            
            if (_fightData != null)
            {
                _fightData.OnAction += OnFightAction;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            buttonClose.onClick.RemoveListener(OnButtonClickedClose);
            
            if (_fightData != null)
            {
                _fightData.OnAction -= OnFightAction;
            }
        }
        
        private void OnButtonClickedClose()
        {
            SetVisible(false);
        }

        public void ShowLiveResult(FightData fightData)
        {
            SetVisible(true);

            if (_fightData != null)
            {
                fightData.OnAction -= OnFightAction;
            }
            
            _fightData = fightData;
            
            fightData.OnAction += OnFightAction;
            
            textResult.SetText(string.Empty);
            uiTvFightBar.SetLiveBar(fightData);
        }

        private void OnFightAction(FightData fightData, FightActionType fightAction)
        {
            uiTvFightBar.SetLiveBar(fightData);
            
            Debug.Log($"LIVE: {fightAction}");
            
            switch (fightAction)
            {
                case FightActionType.PLAYER_HITS:
                    textResult.SetText("PLAYER PUNCH ENEMY!");
                    break;
                case FightActionType.PLAYER_MISS:
                    textResult.SetText("ENEMY CAN DODGE!");
                    break;
                case FightActionType.ENEMY_HITS:
                    textResult.SetText("ENEMY PUNCH PLAYER!");
                    break;
                case FightActionType.ENEMY_MISS:
                    textResult.SetText("PLAYER DODGE!!!");
                    break;
                case FightActionType.GAME_RESULT_WIN:
                    if (fightData.IsRoundTimeOut)
                    {
                        textResult.SetText("PLAYER WIN THE GAME!");
                    }
                    else
                    {
                        textResult.SetText("PLAYER WIN THE GAME! (KNOCK OUT)");
                    }
                    break;
                case FightActionType.GAME_RESULT_LOSE:
                    if (fightData.IsRoundTimeOut)
                    {
                        textResult.SetText("PLAYER LOSE THE GAME!");
                    }
                    else
                    {
                        textResult.SetText("PLAYER LOSE THE GAME! (KNOCK OUT)");
                    }
                    break;
            }
        }
    }
}