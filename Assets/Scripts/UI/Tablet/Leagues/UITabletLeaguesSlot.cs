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

        public int Power { get; private set; }

        public void SetOpponent(BoxerData boxerData, OpponentSO opponentSo)
        {
            int boxerPower = boxerData.totalTrainingPoints;
            int opponentPower = opponentSo.TotalPower;
            Power = opponentPower;
            
            SetOpponentAvatar(opponentSo.avatar);
            SetOpponentName(opponentSo.opponentName);
            SetOpponentPower(opponentPower);
            SetOpponentStrength(opponentSo.strength);
            SetOpponentAgility(opponentSo.agility);
            SetOpponentStamina(opponentSo.stamina);
            SettPowerLeft(boxerPower - opponentPower);
            SetRequiredLevel(opponentSo.level);

            bool canFight = boxerData.level >= opponentSo.level;
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
    }
}