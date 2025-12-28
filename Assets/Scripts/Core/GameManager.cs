using UnityEngine;
using System;
using Core;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("System")]
    [field: SerializeField] public GameTimeManager GameTimeManager { get; private set; }
    [field: SerializeField] public GameController GameController { get; private set; }

    [Header("Economy")]
    public int currentMoney = 1000; // Starting cash
    public int currentExp = 0;
    
    // Events allow the UI to update automatically when money changes
    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool TrySpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney);
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
    }
}