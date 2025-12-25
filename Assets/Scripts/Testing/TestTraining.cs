using UnityEngine;
using UnityEngine.InputSystem;

public class TestTraining : MonoBehaviour
{
    public BoxerController boxer;
    public TrainingEquipment bag;

    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            boxer.GoTrain(bag);
        }
    }
}