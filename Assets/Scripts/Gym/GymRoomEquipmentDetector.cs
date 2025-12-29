using UnityEngine;
using UnityEngine.Events;

public class GymRoomEquipmentDetector : MonoBehaviour
{
    public UnityEvent<Collider> onRoomTriggerEnter;
    public UnityEvent<Collider> onRoomTriggerExit;
    
    private void OnTriggerEnter(Collider other)
    {
        onRoomTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onRoomTriggerExit?.Invoke(other);
    }
}