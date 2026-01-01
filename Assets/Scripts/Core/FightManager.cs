using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class FightData
{
    public readonly BoxerData Player;
    public readonly OpponentSO Enemy;
    
    public Action OnPlayerHits;
    public Action OnPlayerMiss;
    public Action OnPlayerWin;
    public Action OnPlayerLose;
    
    public Action OnEnemyHits;
    public Action OnEnemyMiss;
    public Action OnEnemyWin;
    public Action OnEnemyLose;
    
    public float PlayerHp { get; set; }
    public float EnemyHp { get; set; }
    public float EnemyStrength { get; set; }
    public float EnemyAgility { get; set; }
    public float EnemyStamina { get; set; }
    
    public bool RoundEnd { get; set; }

    public FightData(BoxerData boxerData, OpponentSO opponentSo)
    {
        Player = boxerData;
        Enemy = opponentSo;
        
        PlayerHp = 100f;
        EnemyHp = 100f;
        
        EnemyStrength = opponentSo.strength;
        EnemyAgility = opponentSo.agility;
        EnemyStamina = opponentSo.stamina;
    }

}

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;
    
    [Header("Opponents")] 
    [field: SerializeField] public OpponentSO[] FighterDataTierA { get; private set; }
    [field: SerializeField] public OpponentSO[] FighterDataTierB { get; private set; }
    [field: SerializeField] public OpponentSO[] FighterDataTierC { get; private set; }
    [field: SerializeField] public OpponentSO[] FighterDataTierD { get; private set; }

    // Events to update UI or play sounds
    private readonly List<FightData> _fightData = new List<FightData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.GameTimeManager.OnGameMinuteTick += OnGameMinuteTick;
    }

    private void OnDestroy()
    {
        GameManager.Instance.GameTimeManager.OnGameMinuteTick -= OnGameMinuteTick;
    }

    private void OnGameMinuteTick(string obj)
    {
        ProcessFightData();
    }

    public bool CanBeatOpponent(BoxerController playerBoxer, OpponentSO opponent)
    {
        return playerBoxer.stats.strength >= opponent.strength &&
               playerBoxer.stats.agility >= opponent.agility &&
               playerBoxer.stats.stamina >= opponent.stamina;
    }

    public void StartFightData(FightData fightData)
    {
        _fightData.Add(fightData);
    }

    private void ProcessFightData()
    {
        for (int i = 0; i < _fightData.Count; i++)
        {
            FightData data = _fightData[i];
            ProcessRoundForOpponent(data);
            ProcessRoundForPlayer(data);
            if(!data.RoundEnd) continue;
            _fightData.Remove(data);
        }
    }
    
    private void ProcessRoundForOpponent(FightData fightData)
    {
        // 1. Calculate Hit Chance (Agility Contest)
        float hitChance = fightData.EnemyAgility / (fightData.EnemyAgility + fightData.Player.agility);
        bool playerHits = Random.value < hitChance;

        // 2. Damage Step
        if (playerHits)
        {
            float damage = fightData.EnemyStrength * Random.Range(0.8f, 1.2f);
            fightData.PlayerHp -= damage;
            
            // FUEL COST: Strength is consumed when you hit
            fightData.EnemyStrength -= 50; 
            fightData.OnEnemyHits?.Invoke();
        }
        else
        {
            // FUEL COST: Agility is consumed when you miss/dodge
            fightData.EnemyAgility -= 30;
            fightData.OnEnemyMiss?.Invoke();
        }

        // 3. Passive Drain
        fightData.EnemyStamina -= 10; 

        // 4. Check Win/Loss
        if (fightData.PlayerHp <= 0)
        {           
            fightData.RoundEnd = true;
            fightData.OnEnemyWin?.Invoke();
        }

        if (fightData.EnemyStrength <= 0 || fightData.EnemyStamina <= 0)
        {
            fightData.RoundEnd = true;
            fightData.OnEnemyLose?.Invoke();
        }
    }
    
    private void ProcessRoundForPlayer(FightData fightData)
    {
        // 1. Calculate Hit Chance (Agility Contest)
        float hitChance = fightData.Player.agility / (fightData.Player.agility + fightData.Player.agility);
        bool playerHits = Random.value < hitChance;

        // 2. Damage Step
        if (playerHits)
        {
            float damage = fightData.Player.strength * Random.Range(0.8f, 1.2f);
            fightData.EnemyHp -= damage;
            
            // FUEL COST: Strength is consumed when you hit
            fightData.Player.strength -= 50; 
            fightData.OnPlayerHits?.Invoke();
        }
        else
        {
            // FUEL COST: Agility is consumed when you miss/dodge
            fightData.Player.agility -= 30;
            fightData.OnPlayerMiss?.Invoke();
        }

        // 3. Passive Drain
        fightData.Player.stamina -= 10; 

        // 4. Check Win/Loss
        if (fightData.EnemyHp <= 0)
        {           
            fightData.RoundEnd = true;
            fightData.OnPlayerWin?.Invoke();
        }

        if (fightData.Player.strength <= 0 || fightData.Player.stamina <= 0)
        {
            fightData.RoundEnd = true;
            fightData.OnPlayerLose?.Invoke();
        }
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
            //FloatingTextManager.Instance.ShowFixedText($"+${totalMoney:F0}", Color.green, TextSpawnPointType.Money);
            FloatingTextManager.Instance.ShowMoneyText(totalMoney);
            
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
        
        return playerWon;
    }
}