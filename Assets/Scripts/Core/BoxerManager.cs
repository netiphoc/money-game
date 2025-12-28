using UnityEngine;

public class BoxerManager : MonoBehaviour
{
    public static BoxerManager Instance;

    public BoxerController mainBoxer; // Assign your starting boxer here

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}