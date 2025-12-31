using Systems;
using UI;
using UnityEngine;

public class TrashObject : BaseInteractable
{
    [Header("Settings")]
    public float timeToClean = 1.0f;
    public int moneyReward = 5;
    
    private float cleanTimer = 0f;
    private float lastInteractTime;

    protected override void InitInteractionPrompts()
    {
        AddInteractionPrompt(new []
        {
            new InteractionPromptData
            {
                Icon = KeyIcon.MOUSE_LEFT_CLICK,
                Prompt = "(Hold) Clean"
            },
        });
    }

    public override void OnInteract(PlayerInteraction player)
    {
        // This method is called every frame the button is held (via PlayerInteraction)
        lastInteractTime = Time.time;
        cleanTimer += Time.deltaTime;

        // Visual feedback (optional: shake the trash)
        transform.localScale = Vector3.one * (1f - (cleanTimer / timeToClean) * 0.2f);

        Debug.Log("Trash");
        if (cleanTimer >= timeToClean)
        {
            CleanUp();
        }
    }

    public override void OnAltInteract(PlayerInteraction player) { }

    protected override void Update()
    {
        base.Update();
        
        // Reset Logic: If we haven't been interacted with for 0.1 seconds, reset
        if (Time.time - lastInteractTime > 0.1f)
        {
            cleanTimer = 0;
            transform.localScale = Vector3.one; // Reset size
        }
    }

    private void CleanUp()
    {
        GameManager.Instance.AddMoney(moneyReward);
        Destroy(gameObject);
    }
}