using UnityEngine;

[CreateAssetMenu(fileName = "New License", menuName = "GymTycoon/License Data")]
public class LicenseDataSO : ScriptableObject
{
    public string licenseName;
    public Sprite icon;
    public int cost;
    public ItemDataSO[] items;
    public bool isOwned;
}