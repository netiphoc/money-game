using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tablet.Leagues
{
    public class UITabletLeaguesSlot : BaseUI
    {
        [SerializeField] private Image iconAvatar;
        [SerializeField] private Image iconRank;
        [SerializeField] private Color[] rankColors;
        [SerializeField] private TMP_Text textRank;
        [SerializeField] private TMP_Text textName;
        [SerializeField] private TMP_Text textPower;
        [SerializeField] private TMP_Text textStrength;
        [SerializeField] private TMP_Text textAgility;
        [SerializeField] private TMP_Text textStamina;
        [SerializeField] private TMP_Text textPowerLeft;
        [SerializeField] private Transform lockLevelGroup;
        [SerializeField] private TMP_Text textRequiredLevel;
        [SerializeField] private Button buttonFight;

        public Action<FightResultData> OnFightResult;
        public int Power { get; private set; }

        private BoxerController _boxerController;
        private OpponentSO _opponentSo;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            buttonFight.onClick.AddListener(OnButtonClickedFight);
            GameManager.Instance.GameTimeManager.OnGameMinuteTick += OnGameMinuteTick;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            buttonFight.onClick.RemoveListener(OnButtonClickedFight);
            GameManager.Instance.GameTimeManager.OnGameMinuteTick += OnGameMinuteTick;
        }

        private void OnGameMinuteTick(string obj)
        {
            /*
            if(!_boxerController) return;
            if(!_opponentSo) return;
            int boxerPower = _boxerController.stats.totalPower;
            int opponentPower = _opponentSo.TotalPower;
            SettPowerLeft(boxerPower - opponentPower);
            */
            
            if(!_boxerController) return;
            SettPowerLeft(_boxerController.stats.totalPower);
        }
        
        public void SetOpponent(BoxerController boxerController, OpponentSO opponentSo)
        {
            _boxerController = boxerController;
            _opponentSo = opponentSo;

            int boxerPower = boxerController.stats.totalPower;
            int opponentPower = opponentSo.TotalPower;
            Power = opponentPower;
            
            SetOpponentAvatar(opponentSo.avatar);
            SetOpponentName(opponentSo.opponentName);
            SetOpponentPower(opponentPower);
            SetOpponentStrength(opponentSo.strength);
            SetOpponentAgility(opponentSo.agility);
            SetOpponentStamina(opponentSo.stamina);
            SettPowerLeft(boxerPower);
            //SettPowerLeft(boxerPower - opponentPower);
            SetRequiredLevel(opponentSo.requiredBoxerLevel);

            bool canFight = boxerController.stats.level >= opponentSo.requiredBoxerLevel;
            lockLevelGroup.gameObject.SetActive(!canFight);
        }
        
        public void SetOpponentRank(int rank)
        {
            textRank.SetText($"{rank}");
            UpdateRankIconColor(rank);
        }
        
        private void SetOpponentAvatar(Sprite sprite)
        {
            iconAvatar.sprite = sprite;
        }

        private void SetOpponentName(string opponentName)
        {
            textName.SetText(opponentName);
        }
        
        private void SetOpponentPower(int power)
        {
            textPower.SetText($"{power}");
        }
        
        private void SetOpponentStrength(int strength)
        {
            textStrength.SetText($"{strength}");
        }
        
        private void SetOpponentAgility(int agility)
        {
            textAgility.SetText($"{agility}");
        }
        
        private void SetOpponentStamina(int stamina)
        {
            textStamina.SetText($"{stamina}");
        }
        
        private void SettPowerLeft(int powerLeft)
        {
            textPowerLeft.SetText($"{powerLeft}");
        }
        
        private void SetRequiredLevel(int level)
        {
            textRequiredLevel.SetText($"(Level {level})");
        }

        private void UpdateRankIconColor(int rank)
        {
            switch (rank)
            {
                case 1:
                    iconRank.color = rankColors[0];
                    break;
                case 2:
                    iconRank.color = rankColors[1];
                    break;
                case 3:
                    iconRank.color = rankColors[2];
                    break;
                default:
                    iconRank.color = rankColors[3];
                    break;
            }
        }
        
        private void OnButtonClickedFight()
        {
            FightResultData fightResultData = new FightResultData
            {
                BoxerController = _boxerController,
                Opponent = _opponentSo
            };
            
            FightManager.Instance.StartFight(_boxerController, _opponentSo);
            //OnFightResult?.Invoke(fightResultData);
        }
    }
}