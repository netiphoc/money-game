using UnityEngine;
using System;

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    // Events to update UI or play sounds
    public event Action<bool, OpponentSO> OnFightComplete; // bool = isWin

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    public void StartFight(BoxerController playerBoxer, OpponentSO opponent)
    {
        // ... (Calculate Power logic) ...
        
        bool playerWon = playerBoxer.stats.totalPower >= opponent.TotalPower;

        if (playerWon)
        {
            // 1. Give Money
            GameManager.Instance.AddMoney(opponent.moneyReward);
            
            // 2. Level Up the BOXER (Unit Progression)
            // (Assuming BoxerStats has an AddXP method)
            playerBoxer.stats.AddXP(opponent.boxerXPReward); 
            
            // 3. Level Up the PLAYER (Gym Progression)
            GameManager.Instance.AddPlayerXP(opponent.playerXPReward);
            
            Debug.Log("YOU WON!");
        }
        else
        {
            // Consolation (maybe small XP)
            GameManager.Instance.AddMoney(opponent.moneyReward / 4);
        }

        // Reset Boxer Energy/Stats...
        OnFightComplete?.Invoke(playerWon, opponent);
    }
}