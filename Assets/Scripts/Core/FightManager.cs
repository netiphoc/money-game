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
    
    public bool StartFight(BoxerController playerBoxer, OpponentSO opponent)
    {
        // ... (Calculate Power logic) ...

        bool IsPlayerWon()
        {
            return playerBoxer.stats.strength >= opponent.strength &&
                   playerBoxer.stats.agility >= opponent.agility &&
                   playerBoxer.stats.stamina >= opponent.stamina;
        }

        int wonCount = 0;
        
        while (IsPlayerWon())
        {
            wonCount++;
            // 1. Give Money
            GameManager.Instance.AddMoney(opponent.moneyReward);
            
            // 2. Level Up the BOXER (Unit Progression)
            // (Assuming BoxerStats has an AddXP method)
            playerBoxer.stats.AddXP(opponent.boxerXPReward); 
            
            // 3. Level Up the PLAYER (Gym Progression)
            GameManager.Instance.AddPlayerXP(opponent.playerXPReward);
            
            Debug.Log($"YOU WON! x{wonCount}");
            
            // Reset Boxer Energy/Stats...
            playerBoxer.stats.strength -= opponent.strength;
            playerBoxer.stats.agility -= opponent.agility;
            playerBoxer.stats.stamina -= opponent.stamina;
        }

        bool playerWon = wonCount > 0;
        
        if (!playerWon)
        {
            // Consolation (maybe small XP)
            GameManager.Instance.AddMoney(opponent.moneyReward / 4);
            playerBoxer.stats.strength -= opponent.strength;
            playerBoxer.stats.agility -= opponent.agility;
            playerBoxer.stats.stamina -= opponent.stamina;
        }
        
        OnFightComplete?.Invoke(playerWon, opponent);

        return playerWon;
    }
}