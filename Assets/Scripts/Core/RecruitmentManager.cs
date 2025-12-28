using UnityEngine;
using System.Collections.Generic;

public class RecruitmentManager : MonoBehaviour
{
    public static RecruitmentManager Instance;

    [Header("Settings")]
    public GameObject boxerPrefab; // The NPC model to spawn
    public Transform spawnPoint; // Where they appear before walking to room

    // Names database
    private string[] firstNames = { "Rocky", "Apollo", "Mike", "Sugar", "Floyd", "Manny" };
    private string[] lastNames = { "Balboa", "Creed", "Tyson", "Ray", "Mayweather", "Pacquiao" };

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Generate 3 random applicants based on your Gym Level (or just random)
    public List<BoxerData> GenerateApplicants(int tier)
    {
        List<BoxerData> applicants = new List<BoxerData>();

        for (int i = 0; i < 3; i++)
        {
            BoxerData newBoxer = new BoxerData();
            newBoxer.boxerName = $"{firstNames[Random.Range(0, firstNames.Length)]} {lastNames[Random.Range(0, lastNames.Length)]}";
            
            // Stats based on Tier (Tier 1 = Low stats, Tier 3 = High stats)
            float baseStat = tier * 5f; 
            newBoxer.strength = baseStat + Random.Range(1, 5);
            newBoxer.agility = baseStat + Random.Range(1, 5);
            newBoxer.stamina = baseStat + Random.Range(1, 5);

            newBoxer.hiringCost = (tier * 500) + Random.Range(0, 100);
            newBoxer.statMultiplier = 1.0f + (tier * 0.1f);

            applicants.Add(newBoxer);
        }
        return applicants;
    }

    public void RecruitBoxer(BoxerData data, GymRoom targetRoom)
    {
        if (GameManager.Instance.TrySpendMoney(data.hiringCost))
        {
            // 1. Spawn the Visual NPC
            GameObject npcObj = Instantiate(boxerPrefab, spawnPoint.position, Quaternion.identity);
            
            // 2. Initialize Controller
            BoxerController controller = npcObj.GetComponent<BoxerController>();
            controller.stats = data; // Apply the stats we just generated

            // 3. Assign to Room
            targetRoom.assignedBoxer = controller;
            
            Debug.Log($"Hired {data.boxerName} for {targetRoom.roomName}");
        }
    }
}