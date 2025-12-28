using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingUIManager : MonoBehaviour
{
    public static TrainingUIManager Instance;

    [Header("UI References")]
    public GameObject menuPanel;
    public TextMeshProUGUI boxerNameText;
    public TextMeshProUGUI boxerStatsText; // "Str: 10 | Agi: 5 | Sta: 8"
    
    [Header("List Generation")]
    public Transform contentParent; // The Content object of a ScrollView
    public GameObject equipmentButtonPrefab; // The button template

    private GymRoom currentRoom;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        menuPanel.SetActive(false);
    }

    public void OpenMenu(GymRoom room)
    {
        currentRoom = room;
        menuPanel.SetActive(true);

        RefreshUI();
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        
        // Lock cursor back to game mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void RefreshUI()
    {
        // 1. Show Boxer Info
        if (currentRoom.assignedBoxer != null)
        {
            BoxerData stats = currentRoom.assignedBoxer.stats;
            boxerNameText.text = stats.boxerName;
            boxerStatsText.text = $"STR: {stats.strength} | AGI: {stats.agility} | STA: {stats.stamina}";
        }
        else
        {
            boxerNameText.text = "No Boxer Assigned";
            boxerStatsText.text = "";
        }

        // 2. Clear old buttons
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        // 3. Generate new buttons for Equipment in the room
        foreach (TrainingEquipment equip in currentRoom.equipmentInRoom)
        {
            GameObject btnObj = Instantiate(equipmentButtonPrefab, contentParent);
            
            // Setup Text
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = $"{equip.equipmentName} (+{equip.strGain} Str, +{equip.agiGain} Agi)";

            // Setup Click Event
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnTrainButtonClicked(equip));
        }
    }

    private void OnTrainButtonClicked(TrainingEquipment equip)
    {
        if (currentRoom.assignedBoxer != null)
        {
            // Command the boxer!
            currentRoom.assignedBoxer.GoTrain(equip);
            CloseMenu(); // Close UI after selection
        }
    }
}