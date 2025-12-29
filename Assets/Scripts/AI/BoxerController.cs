using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public class BoxerController : MonoBehaviour
{
    public enum AIState { Sleep, Idle, MovingToMachine, TrainingVisual }

    [Header("Data")]
    public BoxerData stats;
    public AIState currentState = AIState.Idle;

    [Header("Assignment")]
    public GymRoom assignedRoom;

    [Header("Visual Settings")]
    public float minTrainTime = 5f;
    public float maxTrainTime = 15f;
    public float restTime = 2f;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator; // CHANGED TO PUBLIC for manual assignment

    // Internal
    private TrainingEquipment targetEquipment;
    private Coroutine currentRoutine;

    private void Awake()
    {
        // Fallback: If you forgot to assign them, try to find them automatically
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (assignedRoom != null)
        {
            EnterSleepState();
        }
    }

    public void AssignToRoom(GymRoom room)
    {
        assignedRoom = room;
        StartVisualCycle();
    }

    public void StartVisualCycle()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AI_Routine());
    }

    private IEnumerator AI_Routine()
    {
        while (true)
        {
            // 1. CHECK: Is there ANY equipment?
            if (assignedRoom == null || assignedRoom.equipmentInRoom.Count == 0)
            {
                EnterIdleState();
                yield return new WaitForSeconds(2f);
                continue; 
            }

            // 2. DECIDE
            int randomIndex = Random.Range(0, assignedRoom.equipmentInRoom.Count);
            targetEquipment = assignedRoom.equipmentInRoom[randomIndex];

            // 3. MOVE
            currentState = AIState.MovingToMachine;
            agent.SetDestination(targetEquipment.interactionPoint.position);
            if(animator) animator.SetBool("IsWalking", true);

            // Wait until arrived
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                if (!IsTargetValid())
                {
                    Debug.Log("Equipment destroyed while walking! Rethinking...");
                    StartVisualCycle();
                    yield break; 
                }
                yield return null;
            }

            // 4. ARRIVED -> SETUP
            agent.velocity = Vector3.zero; 
            if(animator) animator.SetBool("IsWalking", false);
            currentState = AIState.TrainingVisual;

            transform.rotation = targetEquipment.interactionPoint.rotation;

            if (animator && !string.IsNullOrEmpty(targetEquipment.animationTrigger))
            {
                animator.SetTrigger(targetEquipment.animationTrigger);
            }

            // 5. TRAIN LOOP
            float trainDuration = Random.Range(minTrainTime, maxTrainTime);
            float trainTimer = 0f;

            while (trainTimer < trainDuration)
            {
                trainTimer += Time.deltaTime;

                if (!IsTargetValid())
                {
                    StopVisualTraining();
                    yield break; 
                }
                
                yield return null;
            }

            // 6. FINISH NORMALLY
            StopVisualTraining();

            // Optional: Spawn "+1" floating text here?
            // Inside ProduceStats loop, after updating stats:
            if (stats.unrealizedStrength > 0 || stats.unrealizedAgility > 0)
            {
                // Only spawn text if looking at the room? (Optimization for 16 rooms)
                // For now, spawn above boxer head
                FloatingTextManager.Instance.ShowWorldText(transform.position, $"+{stats.unrealizedStrength+stats.unrealizedAgility+stats.unrealizedStamina}", Color.green);
                stats.ApplyUnrealizeStats();
            }
            
            // 7. REST
            currentState = AIState.Idle;
            yield return new WaitForSeconds(restTime);
        }
    }
    
    // --- HELPER METHODS ---

    private bool IsTargetValid()
    {
        if (targetEquipment == null) return false;
        return true;
    }

    private void EnterSleepState()
    {
        currentState = AIState.Sleep;
        if(agent.isOnNavMesh) agent.ResetPath();
        if(animator) animator.SetTrigger("IsSleeping"); 
    }
    
    private void EnterIdleState()
    {
        currentState = AIState.Idle;
        if(agent.isOnNavMesh) agent.ResetPath();
        if(animator) animator.SetBool("IsWalking", false);
    }

    private void StopVisualTraining()
    {
        if(animator) animator.SetTrigger("StopTraining"); 
        currentState = AIState.Idle;
    }

    public void ResetFuel()
    {
        stats.strength = 0;
        stats.agility = 0;
        stats.stamina = 0;
        stats.UpdateTotal();
    }
}