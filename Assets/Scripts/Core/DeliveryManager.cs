using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;

    [Header("Settings")]
    public Transform deliveryZone; // Where boxes appear (e.g., sidewalk)
    public GameObject itemBoxPrefab; // The cardboard box
    public float spawnOffset = 1.0f; // Space between multiple boxes

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Call this from your UI Button
    public void OrderItem(PlaceableDataSO furnitureData)
    {
        // 1. Check Money FIRST
        if (GameManager.Instance.TrySpendMoney(furnitureData.cost))
        {
            SpawnDeliveryBox(furnitureData);
        }
        else
        {
            Debug.Log("Not enough money!");
            // Play Error Sound
        }
    }

    private void SpawnDeliveryBox(PlaceableDataSO data)
    {
        // 2. Validate Link
        if (data.linkedItemData == null)
        {
            Debug.LogError($"Furniture {data.objectName} has no Linked Item Data! Cannot box it.");
            return;
        }

        // 3. Determine Spawn Position (Simple Stack logic)
        // In a real game, you might want to find a free spot.
        // For now, we spawn with a random small offset so they don't stack perfectly.
        Vector3 spawnPos = deliveryZone.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));

        // 4. Create Box
        GameObject boxObj = Instantiate(itemBoxPrefab, spawnPos, Quaternion.identity);
        ItemBox boxScript = boxObj.GetComponent<ItemBox>();

        // 5. Setup Box Contents
        if (boxScript != null)
        {
            boxScript.itemData = data.linkedItemData;
            boxScript.currentQuantity = 1; // Furniture usually comes 1 per box
        }
        
        // Play "Thump" sound
    }
}