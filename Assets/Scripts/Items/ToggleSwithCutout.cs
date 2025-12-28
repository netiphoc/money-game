using UnityEngine;
using DG.Tweening;

public class ToggleSwithCutout : BaseToggleSwitch
{
    [Header("Animation Settings")]
    public Transform switchHandle; // Drag the moving part here (Child object)
    public Vector3 onRotation = new Vector3(-45, 0, 0); // Rotation when UP (On)
    public Vector3 offRotation = new Vector3(45, 0, 0); // Rotation when DOWN (Off)
    public float tweenDuration = 0.2f;
    public Ease tweenEase = Ease.OutBack; // Adds a nice clicky "snap" feel

    private void Start()
    {
        // Set initial rotation instantly without animation
        if(switchHandle)
        {
            switchHandle.localEulerAngles = isOn ? onRotation : offRotation;
        }
    }

    protected override void Toggle()
    {
        base.Toggle();
        
        // 2. Play DOTween Animation
        if (switchHandle != null)
        {
            Vector3 targetRot = isOn ? onRotation : offRotation;
            switchHandle.DOLocalRotate(targetRot, tweenDuration).SetEase(tweenEase);
        }
    }

    protected override bool ToggleCondition()
    {
        return (!isOn && GameManager.Instance.GameTimeManager.IsStartOfDay()) ||
               (isOn && GameManager.Instance.GameTimeManager.IsEndOfDay());
    }

    protected override string GetDenyPrompt()
    {
        return "You can end the day at 10:00 PM";
    }
}