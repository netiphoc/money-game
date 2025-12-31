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
    
    public bool CanBeatOpponent(BoxerController playerBoxer, OpponentSO opponent)
    {
        return playerBoxer.stats.strength >= opponent.strength &&
               playerBoxer.stats.agility >= opponent.agility &&
               playerBoxer.stats.stamina >= opponent.stamina;
    }
    
    public bool StartFight(BoxerController playerBoxer, OpponentSO opponent)
    {
        int wonCount = 0;
        int totalBoxerExp = 0;
        int totalPlayerExp = 0;
        int totalMoney = 0;
        
        while (CanBeatOpponent(playerBoxer, opponent))
        {
            wonCount++;

            totalBoxerExp += opponent.boxerXPReward;
            totalPlayerExp += opponent.playerXPReward;
            totalMoney += opponent.moneyReward;
            
            // Reset Boxer Energy/Stats...
            playerBoxer.stats.strength -= opponent.strength;
            playerBoxer.stats.agility -= opponent.agility;
            playerBoxer.stats.stamina -= opponent.stamina;
        }

        bool playerWon = wonCount > 0;
        
        if (playerWon)
        {
            // 1. Give Money
            GameManager.Instance.AddMoney(totalMoney);
            
            // 2. Level Up the BOXER (Unit Progression)
            // (Assuming BoxerStats has an AddXP method)
            playerBoxer.stats.AddXp(totalBoxerExp); 
            
            // 3. Level Up the PLAYER (Gym Progression)
            GameManager.Instance.AddPlayerXP(totalPlayerExp);
            FloatingTextManager.Instance.ShowFixedText($"Training Camp +{totalPlayerExp} exp", Color.green, TextSpawnPointType.Exp);
            //FloatingTextManager.Instance.ShowWorldText(playerBoxer.transform.position, $"+{totalBoxerExp} exp", Color.green);
            
            Debug.Log($"YOU WON! x{wonCount}");
        }
        else
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