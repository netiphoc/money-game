using TMPro;
using UnityEngine;

namespace UI.Gameplay
{
    public class UITVFightBar : BaseUI
    {
        [SerializeField] private TMP_Text textPlayerName;
        [SerializeField] private TMP_Text textEnemyName;
        [SerializeField] private TMP_Text textRoundTime;
        [SerializeField] private TMP_Text textFightType;

        [Header("Live UI")]
        [SerializeField] private UIBar healthBarPlayer; 
        [SerializeField] private UIBar strBarPlayer; 
        [SerializeField] private UIBar agilBarPlayer; 
        [SerializeField] private UIBar staBarPlayer; 
        
        [SerializeField] private UIBar healthBarEnemy; 
        [SerializeField] private UIBar strBarEnemy; 
        [SerializeField] private UIBar agilBarEnemy; 
        [SerializeField] private UIBar staBarEnemy;
        
        public void SetLiveBar(FightData fightData)
        {
            textFightType.SetText(fightData.FightDataSO.fightName);
            
            // Time
            textRoundTime.SetText($"{fightData.RoundTimeLeft}");

            // Name
            textPlayerName.SetText(fightData.BoxerData.boxerName);
            textEnemyName.SetText(fightData.Enemy.opponentName);
            
            healthBarPlayer.SetValue(fightData.PlayerHp, fightData.PlayerMaxHp);
            strBarPlayer.SetValue(fightData.BoxerData.strength, fightData.PlayerMaxStr);
            agilBarPlayer.SetValue(fightData.BoxerData.agility, fightData.PlayerMaxAgil);
            staBarPlayer.SetValue(fightData.BoxerData.stamina, fightData.PlayerMaxSta);
            
            healthBarEnemy.SetValue(fightData.EnemyHp, fightData.EnemyMaxHp);
            strBarEnemy.SetValue(fightData.EnemyStrength, fightData.EnemyMaxStr);
            agilBarEnemy.SetValue(fightData.EnemyAgility, fightData.EnemyMaxAgil);
            staBarEnemy.SetValue(fightData.EnemyStamina, fightData.EnemyMaxSta);
        }
    }
}