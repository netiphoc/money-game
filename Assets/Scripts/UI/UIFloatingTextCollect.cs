using UnityEngine;
using TMPro; // Use TextMesh Pro
using System.Collections;
using UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class UIFloatingTextCollect : BaseUI
{
    [Header("Settings")]
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private float floatHeight = 50f; // Initial "Pop" up height
    [SerializeField] private AnimationCurve motionCurve = AnimationCurve.EaseInOut(0,0,1,1);
    
    [Header("Visuals")]
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private CanvasGroup canvasGroup;

    // Internal state
    private RectTransform _rectTransform;

    protected override void Awake()
    {
        base.Awake();
        
        _rectTransform = GetComponent<RectTransform>();
        if (tmpText == null) tmpText = GetComponent<TextMeshProUGUI>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Starts the animation.
    /// </summary>
    /// <param name="text">The string to display (e.g. "+$50")</param>
    /// <param name="startScreenPos">Where the text starts (Screen Space)</param>
    /// <param name="targetScreenPos">Where the text flies to (e.g., Money UI)</param>
    public void Initialize(string text, Vector2 startScreenPos, Vector2 targetScreenPos)
    {
        tmpText.text = text;
        _rectTransform.position = startScreenPos; // Set initial position
        canvasGroup.alpha = 1;
        StartCoroutine(AnimateRoutine(startScreenPos, targetScreenPos));
    }

    private IEnumerator AnimateRoutine(Vector2 startPos, Vector2 endPos)
    {
        float timer = 0f;

        // Optional: Add a small random offset so multiple texts don't stack perfectly
        Vector2 randomOffset = new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
        startPos += randomOffset;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration; // 0 to 1
            float curveValue = motionCurve.Evaluate(progress);

            // 1. MOVEMENT Logic
            // We want it to pop UP a little bit first, then fly to the target
            // Parabolic arc logic can be complex, so we'll do a simple Lerp for X/Y
            Vector2 currentPos = Vector2.Lerp(startPos, endPos, curveValue);
            
            // Add a little vertical offset that fades out (The "Float" effect)
            // It goes UP and then merges into the path
            float heightOffset = Mathf.Sin(progress * Mathf.PI) * floatHeight * (1 - progress); 
            // Note: The math above creates a small arc
            
            _rectTransform.position = currentPos + new Vector2(0, heightOffset);

            // 2. FADE OUT Logic
            // Fade out in the last 30% of the movement
            if (progress > 0.7f)
            {
                float fadeProgress = (progress - 0.7f) / 0.3f;
                canvasGroup.alpha = 1 - fadeProgress;
            }
            
            // 3. SCALE Logic (Pop in effect)
            // Scale from 0 to 1.2 quickly, then settle to 1
            if (progress < 0.2f)
            {
                float scale = Mathf.Lerp(0f, 1.2f, progress / 0.2f);
                transform.localScale = Vector3.one * scale;
            }
            else
            {
                transform.localScale = Vector3.one;
            }

            yield return null;
        }

        // Animation done
        SetVisible(false);
        
    }
}