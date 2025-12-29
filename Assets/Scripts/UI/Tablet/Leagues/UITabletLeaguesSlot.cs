using System;
using System.Text;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

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
        [SerializeField] private UITabletButtonFight buttonFight;

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

        private void InitBoxer(BoxerController boxerController)
        {
            if (_boxerController != null)
            {
                buttonFight.SetFightAvailable(false);
                _boxerController.stats.OnLevelChanged -= OnLevelChanged;
            }
            
            _boxerController = boxerController;
            _boxerController.stats.OnLevelChanged += OnLevelChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _boxerController.stats.OnLevelChanged -= OnLevelChanged;
        }
        
        private void OnGameMinuteTick(string obj)
        {
            UpdateStats();
            UpdateFightButton();
        }

        private void OnLevelChanged(int level)
        {
            if(!_opponentSo) return;
            UpdateFightButton();
        }
        
        private void UpdateStats()
        {
            if(!_boxerController) return;
            if(!_opponentSo) return;
            int boxerPower = _boxerController.stats.totalPower;
            int opponentPower = _opponentSo.TotalPower;
            SettPowerLeft(boxerPower - opponentPower);
            SetOpponentPower(_opponentSo.TotalPower);

            //if(!_boxerController) return;
            //SettPowerLeft(_boxerController.stats.totalPower);
        }
        
        private void UpdateFightButton()
        {
            if (!IsLevelUnlock())
            {
                buttonFight.SetLock(_opponentSo.requiredBoxerLevel);
                return;
            }
            
            buttonFight.Unlock();
            
            bool canBeat = _opponentSo &&
                           _boxerController &&
                           FightManager.Instance.CanBeatOpponent(_boxerController, _opponentSo);

            buttonFight.SetFightAvailable(canBeat);
        }
        
        public void SetOpponent(BoxerController boxerController, OpponentSO opponentSo)
        {
            InitBoxer(boxerController);
            
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
            //SettPowerLeft(boxerPower);
            SettPowerLeft(boxerPower - opponentPower);

            UpdateFightButton();
        }

        private bool IsLevelUnlock()
        {
           return _boxerController.stats.level >= _opponentSo.requiredBoxerLevel;
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
            bool canBeat = _boxerController && _boxerController.stats.totalPower >= power;
            textPower.color = canBeat ? Color.white : Color.red;
            textPower.SetText($"{power.ToPowerFormat()}");
            //textPower.SetText(IsLevelUnlock() && IsLevelUnlock() ? $"{power.ToPowerFormat()}" : "???");
        }
        
        private void SetOpponentStrength(int strength)
        {
            textStrength.SetText($"{strength.ToPowerFormat()}");
            //textStrength.SetText(IsLevelUnlock() && IsLevelUnlock() ? $"{strength.ToPowerFormat()}" : "-");
        }
        
        private void SetOpponentAgility(int agility)
        {
            textAgility.SetText($"{agility.ToPowerFormat()}");
            //textAgility.SetText(IsLevelUnlock() && IsLevelUnlock() ? $"{agility.ToPowerFormat()}" : "-");
        }
        
        private void SetOpponentStamina(int stamina)
        {
            textStamina.SetText($"{stamina.ToPowerFormat()}");
            //textStamina.SetText(IsLevelUnlock() && IsLevelUnlock() ? $"{stamina.ToPowerFormat()}" : "-");
        }
        
        private void SettPowerLeft(int powerLeft)
        {
            if (!IsLevelUnlock())
            {
                textPowerLeft.SetText(string.Empty);
                return;
            }
            
            StringBuilder stringBuilder = new StringBuilder();
            
            if (_boxerController.stats.strength < _opponentSo.strength)
            {
                int strengthNeeded = Mathf.FloorToInt(_opponentSo.strength - _boxerController.stats.strength);
                stringBuilder.Append($"STR: {strengthNeeded.ToPowerFormat()}");
            }
            
            if (_boxerController.stats.agility < _opponentSo.agility)
            {
                int agilityNeeded = Mathf.FloorToInt(_opponentSo.agility - _boxerController.stats.agility);
                stringBuilder.Append($"\nAGIL: {agilityNeeded.ToPowerFormat()}");
            }
            
            if (_boxerController.stats.stamina < _opponentSo.stamina)
            {
                int staminaNeeded = Mathf.FloorToInt(_opponentSo.stamina - _boxerController.stats.stamina);
                stringBuilder.Append($"\nSTA: {staminaNeeded.ToPowerFormat()}");
            }
            
            textPowerLeft.SetText(stringBuilder.ToString());
            
            //textPowerLeft.SetText($"{powerLeft.ToPowerFormat()}");
            //textPowerLeft.SetText(IsLevelUnlock() && IsLevelUnlock() ? $"{powerLeft.ToPowerFormat()}" : "-");
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
            if(!FightManager.Instance.CanBeatOpponent(_boxerController, _opponentSo)) return;
            bool playerWon = FightManager.Instance.StartFight(_boxerController, _opponentSo);

            FightResultData fightResultData = new FightResultData
            {
                BoxerController = _boxerController,
                Opponent = _opponentSo,
                Win = playerWon,
            };
            
            UpdateStats();
            UpdateFightButton();

            if (!playerWon)
            {
                OnFightResult?.Invoke(fightResultData);
            }
        }
    }
}