using UnityEngine;
using UnityEngine.AI;

public class BoxerController : MonoBehaviour
{
    public enum BoxerState { Idle, Moving, Training, NeedsBreak }

    [Header("Data")]
    public BoxerData stats;
    public BoxerState currentState = BoxerState.Idle;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator; // For "Punching", "Running" animations

    // Training State
    private TrainingEquipment currentEquipment;
    private float trainingTimer;

    private void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        stats.UpdateTotal();
    }

    private void Update()
    {
        switch (currentState)
        {
            case BoxerState.Moving:
                CheckArrival();
                break;
            case BoxerState.Training:
                HandleTraining();
                break;
        }
    }

    // --- COMMANDS ---

    public void GoTrain(TrainingEquipment equipment)
    {
        if (equipment == null) return;
        
        currentEquipment = equipment;
        
        // 1. Move to the machine's interaction point
        agent.SetDestination(equipment.interactionPoint.position);
        
        // 2. Set animation to "Walk"
        animator.SetBool("IsWalking", true);
        
        currentState = BoxerState.Moving;
    }

    // --- LOGIC ---

    private void CheckArrival()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                StartTraining();
            }
        }
    }

    private void StartTraining()
    {
        currentState = BoxerState.Training;
        animator.SetBool("IsWalking", false);
        
        // Face the machine properly
        transform.rotation = currentEquipment.interactionPoint.rotation;
        
        // Play specific animation (e.g. "Box_Jab" or "Run")
        animator.SetTrigger(currentEquipment.animationTrigger);
    }

    private void HandleTraining()
    {
        trainingTimer += Time.deltaTime;

        // Visual Progress (Optional: floating text or UI bar)
        
        // Check if cycle is done
        if (trainingTimer >= currentEquipment.trainingDuration)
        {
            CompleteTrainingCycle();
        }
    }

    private void CompleteTrainingCycle()
    {
        // 1. Gain Stats
        stats.AddStats(currentEquipment.strGain, currentEquipment.agiGain, currentEquipment.staGain);
        Debug.Log($"{stats.boxerName} gained stats! Total: {stats.totalTrainingPoints}");

        // 2. Reset Timer
        trainingTimer = 0;

        // 3. Check Energy/Hunger (Simple logic for now)
        stats.energy -= 5f;
        if (stats.energy <= 0)
        {
            currentState = BoxerState.NeedsBreak;
            animator.SetTrigger("Tired");
            Debug.Log("Boxer is tired!");
        }
        else
        {
            // Loop training or stop?
            // For an Idle game, they usually loop until stopped by player.
            // We just let the timer run again in the next frame.
        }
    }
}