using Core;
using Data;
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
        [SerializeField] private RawImage liveRenderTexture;
        
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

            if (BoxerFightManager.Instance.TryGetStage(fightData, out FightStage fightStage))
            {
                liveRenderTexture.texture = fightStage.GetLiveCamRenderTexture();
            }
        }

        private void OnFightAction(FightData fightData, FightActionType fightAction)
        {
            uiTvFightBar.SetLiveBar(fightData);
            
            textResult.SetText(string.Empty);
            switch (fightAction)
            {
                case FightActionType.PLAYER_HITS:
                    break;
                case FightActionType.PLAYER_MISS:
                    break;
                case FightActionType.ENEMY_HITS:
                    break;
                case FightActionType.ENEMY_MISS:
                    break;
                case FightActionType.GAME_RESULT_WIN:
                    if (fightData.IsRoundTimeOut)
                    {
                        textResult.SetText($"{fightData.BoxerData.boxerName} WINS!");
                    }
                    else
                    {
                        textResult.SetText($"{fightData.BoxerData.boxerName} WINS! (KNOCK OUT)");
                    }
                    break;
                case FightActionType.GAME_RESULT_LOSE:
                    if (fightData.IsRoundTimeOut)
                    {
                        textResult.SetText($"{fightData.Enemy.opponentName} WINS!");
                    }
                    else
                    {
                        textResult.SetText($"{fightData.Enemy.opponentName} WINS! (KNOCK OUT)");
                    }
                    break;
                
                case FightActionType.GAME_RESULT_DRAW:
                    textResult.SetText("[ DRAW ]");
                    break;
            }
        }
    }
}