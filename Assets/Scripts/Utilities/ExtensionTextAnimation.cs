using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Utilities
{
    public static class ExtensionTextAnimation
    {
        public static void DoTextPunch(this TMP_Text tmpText)
        {
            const float punch = 0.2f;
            tmpText.transform.DOPunchScale(new Vector3(punch,punch,punch), 0.2f);
        }
    }
}