using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UI;
using UnityEngine.UI;

public class TabletUIManager : MonoBehaviour
{
    [Header("Input")]
    public InputActionProperty toggleTabletInput;

    [Header("UI References")]
    public GameObject tabletCanvas; // The whole UI
    public Transform opponentListParent;
    public UITabletFightButton opponentRowPrefab; // Template for the list item

    [Header("Data")]
    public OpponentSO[] allOpponents; // Drag all your ScriptableObjects here
    public BoxerController currentActiveBoxer; // For now, manually drag your main boxer here

    private bool isTabletOpen = false;

    private void OnEnable()
    {
        toggleTabletInput.action.Enable();
    }

    private void Update()
    {
        if (toggleTabletInput.action.WasPerformedThisFrame())
        {
            ToggleTablet();
        }
    }

    public void ToggleTablet()
    {
        isTabletOpen = !isTabletOpen;
        tabletCanvas.SetActive(isTabletOpen);

        if (isTabletOpen)
        {
            RefreshOpponentList();
            
            // Unlock Cursor so we can click
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Pause Player Interaction? (Optional, but good practice)
            // FindObjectOfType<PlayerInteraction>().enabled = false; 
        }
        else
        {
            // Lock Cursor back
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // FindObjectOfType<PlayerInteraction>().enabled = true;
        }
    }

    private void RefreshOpponentList()
    {
        // 1. Clean up old list
        foreach (Transform child in opponentListParent) Destroy(child.gameObject);

        // 2. Create new rows
        foreach (OpponentSO opp in allOpponents)
        {
            UITabletFightButton row = Instantiate(opponentRowPrefab, opponentListParent);

            // --- SET TEXT ---
            row.nameText.text = opp.opponentName;
            row.statsText.text = $"Power: {opp.TotalPower}"; // Or show specific Str/Agi/Sta
            row.rewardText.text = $"${opp.moneyReward}";

            // --- CHECK WIN CHANCE (Visual Helper) ---
            int myPower = currentActiveBoxer.stats.totalTrainingPoints;
            if (myPower >= opp.TotalPower) 
                row.statsText.color = Color.green; // Easy Fight
            else 
                row.statsText.color = Color.red;   // Hard Fight

            // --- SETUP BUTTON ---
            row.fightBtn.onClick.AddListener(() => OnFightClicked(opp));
        }
    }

    private void OnFightClicked(OpponentSO opponent)
    {
        FightManager.Instance.StartFight(currentActiveBoxer, opponent);
        
        // Refresh to show updated (reset) stats
        RefreshOpponentList(); 
        
        // Optional: Close tablet or show "Result Popup"
    }
}