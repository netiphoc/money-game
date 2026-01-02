using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum FightActionType
{
    PLAYER_HITS,
    PLAYER_MISS,
    ENEMY_HITS,
    ENEMY_MISS,
    GAME_RESULT_WIN,
    GAME_RESULT_LOSE,
}

public class FightData
{
    public const float MinModifier = 0.2f;
    public const float MaxModifier = 1.2f;
    
    public readonly BoxerData Player;
    public readonly BoxerController BoxerController;
    public readonly OpponentSO Enemy;
    public readonly FightDataSO FightDataSO;
    public event Action<FightData, FightActionType> OnAction;
    
    public bool IsRoundOver { get; private set; }
    public bool IsRoundTimeOut => RoundTimeLeft <= 0;
    public float RoundTimeMax { get; }
    public float RoundTimeLeft { get; set; }
    public bool IsPlayerWin { get; private set; }
    public bool IsPlayerTurn { get; set; }

    // Player Stats
    public float PlayerHp { get; set; }
    public float PlayerMaxHp { get; private set; }
    public float PlayerMaxStr { get; private set; }
    public float PlayerMaxAgil { get; private set; }
    public float PlayerMaxSta { get; private set; }
    
    // Enemy Stats
    public float EnemyHp { get; set; }
    public float EnemyMaxHp { get; private set; }
    public float EnemyStrength { get; set; }
    public float EnemyAgility { get; set; }
    public float EnemyStamina { get; set; }
    public float EnemyMaxStr { get; private set; }
    public float EnemyMaxAgil { get; private set; }
    public float EnemyMaxSta { get; private set; }

    // Fight stats cost
    public float StrCost { get; private set; }
    public float AgilCost { get; private set; }
    public float StaCost { get; private set; }
    public float MaxDamage { get; private set; }
    
    public FightData(BoxerController boxerController, OpponentSO opponentSo, FightDataSO fightDataSo, float roundDuration = 10f)
    {
        BoxerController = boxerController;
            
        // Let enemy start first
        IsPlayerTurn = false;
        RoundTimeMax = roundDuration;
        RoundTimeLeft = RoundTimeMax;
        
        Player = boxerController.stats;
        Enemy = opponentSo;
        FightDataSO = fightDataSo;

        const float hpScale = 10f;
        PlayerHp = boxerController.stats.stamina * hpScale;
        EnemyHp = opponentSo.stamina * hpScale;
        PlayerMaxHp = PlayerHp;
        EnemyMaxHp = EnemyHp;

        PlayerMaxStr = Player.strength;
        PlayerMaxAgil = Player.agility;
        PlayerMaxSta = Player.stamina;
            
        EnemyStrength = Enemy.strength;
        EnemyAgility = Enemy.agility;
        EnemyStamina = Enemy.stamina;
        EnemyMaxStr = EnemyStrength;
        EnemyMaxAgil = EnemyAgility;
        EnemyMaxSta = EnemyStamina;

        StrCost = opponentSo.strength / roundDuration;
        AgilCost = opponentSo.agility / roundDuration;
        StaCost = (opponentSo.stamina / roundDuration) * 0.5f;
        MaxDamage = (EnemyHp / roundDuration) * MaxModifier;
        
        // Hide
        boxerController.EnterFight();
    }

    public float GetDamage(float damage)
    {
        return Mathf.Min(damage, MaxDamage);
    }
    
    public void OnActionTriggered(FightActionType fightActionType) => OnAction?.Invoke(this, fightActionType);

    public void OnFightOver(bool isWin)
    {
        IsRoundOver = true;
        IsPlayerWin = isWin;
        OnAction = null;

        int moneyReward = isWin ? Enemy.moneyReward : Mathf.RoundToInt(Enemy.moneyReward * 0.5f);
        int playerExpReward = isWin ? Enemy.playerXPReward : Mathf.RoundToInt(Enemy.playerXPReward * 0.5f);
        int boxerExpReward = isWin ? Enemy.boxerXPReward : Mathf.RoundToInt(Enemy.boxerXPReward * 0.5f);
        
        // 1. Give Money
        GameManager.Instance.AddMoney(moneyReward);
        FloatingTextManager.Instance.ShowMoneyText(moneyReward);
        Player.AddXp(boxerExpReward); 
        GameManager.Instance.AddPlayerXP(playerExpReward);
        FloatingTextManager.Instance.ShowFixedText($"Training Camp +{playerExpReward} exp", Color.green, TextSpawnPointType.Exp);
        //FloatingTextManager.Instance.ShowWorldText(playerBoxer.transform.position, $"+{totalBoxerExp} exp", Color.green);
        
        BoxerController.LeaveFight();
    }
}

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;
    
    [Header("Opponents")] 
    [field: SerializeField] public FightDataSO FighterDataTierA { get; private set; }
    [field: SerializeField] public FightDataSO FighterDataTierB { get; private set; }
    [field: SerializeField] public FightDataSO FighterDataTierC { get; private set; }
    [field: SerializeField] public FightDataSO FighterDataTierD { get; private set; }

    // Events to update UI or play sounds
    private readonly List<FightData> _fightData = new List<FightData>();

    private float _fightDelayTime;
    
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

    private void Update()
    {
        ProcessFightTimer();
    }

    private void ProcessFightTimer()
    {
        _fightDelayTime -= Time.deltaTime;
        if(_fightDelayTime > 0f) return;
        _fightDelayTime = GetFightDelay();
        
        ProcessFightData();
    }

    private float GetFightDelay()
    {
        return 1f * Random.Range(0.8f, 1.2f);
    }

    private void OnGameMinuteTick(string obj)
    {
        //ProcessFightData();
    }

    public bool CanBeatOpponent(BoxerController playerBoxer, OpponentSO opponent)
    {
        return playerBoxer.stats.totalPower >= opponent.TotalPower && 
               playerBoxer.stats.strength >= opponent.strength * 0.5f &&
               playerBoxer.stats.agility >= opponent.agility * 0.5f &&
               playerBoxer.stats.stamina >= opponent.stamina * 0.5f;
        
        return playerBoxer.stats.strength >= opponent.strength &&
               playerBoxer.stats.agility >= opponent.agility &&
               playerBoxer.stats.stamina >= opponent.stamina;
    }

    public bool IsInFight(BoxerController boxerController, out FightData data)
    {
        foreach (var fightData in _fightData)
        {
            if(fightData.Player != boxerController.stats) continue;
            data = fightData;
            return true;
        }

        data = null;
        return false;
    }

    public bool IsInFight(OpponentSO opponentSo, out FightData data)
    {
        foreach (var fightData in _fightData)
        {
            if(!fightData.Enemy.Equals(opponentSo)) continue;
            data = fightData;
            return true;
        }

        data = null;
        return false;
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
            data.RoundTimeLeft--; // Update round timer

            if (data.IsPlayerTurn)
            {
                ProcessRoundForPlayer(data);
            }
            else
            {
                ProcessRoundForOpponent(data);
            }

            data.IsPlayerTurn = !data.IsPlayerTurn;
            
            if(!data.IsRoundOver) continue;
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
            float damage = fightData.EnemyStrength * Random.Range(FightData.MinModifier, FightData.MaxModifier);
            fightData.PlayerHp -= fightData.GetDamage(damage);
            
            // FUEL COST: Strength is consumed when you hit
            fightData.EnemyStrength -= fightData.StrCost;
            fightData.OnActionTriggered(FightActionType.ENEMY_HITS);
        }
        else
        {
            // FUEL COST: Agility is consumed when you miss/dodge
            fightData.EnemyAgility -= fightData.AgilCost;
            fightData.OnActionTriggered(FightActionType.ENEMY_MISS);
        }

        // 3. Passive Drain
        fightData.EnemyStamina -= fightData.StaCost; 

        // 4. Check Win/Loss
        if (fightData.PlayerHp <= 0)
        {           
            fightData.OnActionTriggered(FightActionType.GAME_RESULT_LOSE);
            fightData.OnFightOver(false);
        }

        if (fightData.EnemyStrength <= 0 || fightData.EnemyStamina <= 0)
        {
            fightData.OnActionTriggered(FightActionType.GAME_RESULT_WIN);
            fightData.OnFightOver(true);
        }
    }
    
    private void ProcessRoundForPlayer(FightData fightData)
    {
        // 1. Calculate Hit Chance (Agility Contest)
        float hitChance = fightData.Player.agility / (fightData.Player.agility + fightData.EnemyAgility);
        bool playerHits = Random.value < hitChance;

        // 2. Damage Step
        if (playerHits)
        {
            float damage = fightData.Player.strength * Random.Range(FightData.MinModifier, FightData.MaxModifier);
            fightData.EnemyHp -= fightData.GetDamage(damage);
            
            // FUEL COST: Strength is consumed when you hit
            fightData.Player.strength -= fightData.StrCost; 
            fightData.OnActionTriggered(FightActionType.PLAYER_HITS);
        }
        else
        {
            // FUEL COST: Agility is consumed when you miss/dodge
            fightData.Player.agility -= fightData.AgilCost;
            fightData.OnActionTriggered(FightActionType.PLAYER_MISS);
        }

        // 3. Passive Drain
        fightData.Player.stamina -= fightData.StaCost; 

        // 4. Check Win/Loss
        if (fightData.EnemyHp <= 0)
        {           
            fightData.OnActionTriggered(FightActionType.GAME_RESULT_WIN);
            fightData.OnFightOver(true);
        }
        
        if (fightData.Player.strength <= 0 || fightData.Player.stamina <= 0)
        {
            fightData.OnActionTriggered(FightActionType.GAME_RESULT_LOSE);
            fightData.OnFightOver(false);
        }
    }
    
    public bool StartFight(BoxerController playerBoxer, OpponentSO opponent)
    {
        return false;
        
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