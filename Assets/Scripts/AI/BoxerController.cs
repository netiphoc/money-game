using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class BoxerController : MonoBehaviour
{
    public enum AIState { Idle, MovingToMachine, TrainingVisual }

    [Header("Data")]
    public BoxerData stats;
    public AIState currentState = AIState.Idle;

    [Header("Assignment")]
    public GymRoom assignedRoom; // The room this boxer belongs to

    [Header("Visual Settings")]
    public float minTrainTime = 5f;
    public float maxTrainTime = 15f;
    public float restTime = 2f;

    // Internal References
    private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    private TrainingEquipment targetEquipment;
    private Coroutine currentRoutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // If spawned by Recruitment Manager, we might not have a room yet.
        // Wait for assignment.
        if (assignedRoom != null)
        {
            StartVisualCycle();
        }
    }

    public void AssignToRoom(GymRoom room)
    {
        assignedRoom = room;
        // Move to room center logic could go here
        StartVisualCycle();
    }

    private void StartVisualCycle()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AI_Routine());
    }

    private IEnumerator AI_Routine()
    {
        while (true)
        {
            // 1. CHECK: Is there equipment in the room?
            if (assignedRoom == null || assignedRoom.equipmentInRoom.Count == 0)
            {
                currentState = AIState.Idle;
                animator.SetBool("IsWalking", false);
                // Wander randomly or stand still
                yield return new WaitForSeconds(2f);
                continue; 
            }

            // 2. DECIDE: Pick a random machine
            // (In a detailed game, you might prioritize machines matching their lowest stat)
            int randomIndex = Random.Range(0, assignedRoom.equipmentInRoom.Count);
            targetEquipment = assignedRoom.equipmentInRoom[randomIndex];

            // 3. MOVE
            currentState = AIState.MovingToMachine;
            agent.SetDestination(targetEquipment.interactionPoint.position);
            animator.SetBool("IsWalking", true);

            // Wait until arrived
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            // 4. ARRIVED -> SETUP
            agent.velocity = Vector3.zero; // Stop sliding
            animator.SetBool("IsWalking", false);
            currentState = AIState.TrainingVisual;

            // Snap rotation to face the machine correctly
            transform.rotation = targetEquipment.interactionPoint.rotation;

            // 5. TRAIN (Play Animation)
            if (!string.IsNullOrEmpty(targetEquipment.animationTrigger))
            {
                animator.SetTrigger(targetEquipment.animationTrigger);
            }

            // Determine how long to "fake train" for visual variety
            float duration = Random.Range(minTrainTime, maxTrainTime);
            yield return new WaitForSeconds(duration);

            // 6. FINISH -> RESET ANIMATION
            animator.SetTrigger("StopTraining"); // Ensure your Animator has a generic 'Exit' or 'Idle' trigger
            
            // 7. REST (Brief pause between machines)
            currentState = AIState.Idle;
            yield return new WaitForSeconds(restTime);
        }
    }
    
    // Helper used by Fight Manager to reset stats after spending them
    public void ResetFuel()
    {
        stats.strength = 0;
        stats.agility = 0;
        stats.stamina = 0;
        stats.UpdateTotal();
    }
}