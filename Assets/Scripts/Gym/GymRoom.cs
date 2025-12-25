using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))] // The Room Boundaries
public class GymRoom : MonoBehaviour
{
    [Header("Room Status")]
    public string roomName = "Training Room 1";
    public BoxerController assignedBoxer; // Drag your Boxer here manually for now
    public List<TrainingEquipment> equipmentInRoom = new List<TrainingEquipment>();

    private void OnTriggerEnter(Collider other)
    {
        // Auto-detect Equipment when placed
        TrainingEquipment equip = other.GetComponent<TrainingEquipment>();
        if (equip != null && !equipmentInRoom.Contains(equip))
        {
            equipmentInRoom.Add(equip);
            
            Debug.Log("OnTriggerEnter TrainingEquipment ADD");
        }
        else
        {
            Debug.Log("OnTriggerEnter TrainingEquipment NULL");
        }
        
        // Auto-detect Boxer
        BoxerController boxer = other.GetComponent<BoxerController>();
        if (boxer != null)
        {
            assignedBoxer = boxer;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove if moved out
        TrainingEquipment equip = other.GetComponent<TrainingEquipment>();
        if (equip != null && equipmentInRoom.Contains(equip))
        {
            equipmentInRoom.Remove(equip);
        }
    }
}