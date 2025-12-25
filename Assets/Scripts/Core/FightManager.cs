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
        // 1. Calculate Power
        int playerPower = playerBoxer.stats.totalTrainingPoints;
        int enemyPower = opponent.TotalPower;

        Debug.Log($"Fight Started! Player: {playerPower} vs Enemy: {enemyPower}");

        // 2. Determine Winner (Simple Logic)
        // You can add RNG here if you want (e.g., + Random.Range(-5, 5))
        bool playerWon = playerPower >= enemyPower;

        // 3. Distribute Rewards
        if (playerWon)
        {
            GameManager.Instance.AddMoney(opponent.moneyReward);
            // GameManager.Instance.AddXP(opponent.xpReward); // (Assume you add XP logic later)
            // GameManager.Instance.AddTrophies(opponent.trophyReward);
            Debug.Log("YOU WON!");
        }
        else
        {
            // Consolation Prize (Half Money, No Trophies)
            GameManager.Instance.AddMoney(opponent.moneyReward / 2);
            Debug.Log("YOU LOST!");
        }

        // 4. RESET BOXER STATS (As per your game loop)
        // Reset to base values (e.g., 5, 5, 5)
        playerBoxer.stats.strength = 5;
        playerBoxer.stats.agility = 5;
        playerBoxer.stats.stamina = 5;
        playerBoxer.stats.UpdateTotal();

        // 5. Notify Listeners (UI)
        OnFightComplete?.Invoke(playerWon, opponent);
    }
}