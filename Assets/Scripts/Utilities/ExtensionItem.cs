using System;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public static class ExtensionItem
    {
        
        #region Animation
        public static void AddStockAnimated(this ItemDataSO itemData, Vector3 spawnOrigin, Transform targetSlot, Action onComplete)
        {
            // 2. Spawn Temporary "Flying" Object
            // (Use the prefab from ItemDataSO for the visual)
            GameObject flyingObj = Object.Instantiate(itemData.itemPrefab, spawnOrigin, Quaternion.identity);
        
            // Remove scripts/colliders from flying object so it doesn't interfere
            Object.Destroy(flyingObj.GetComponent<Collider>());
            Object.Destroy(flyingObj.GetComponent<Rigidbody>());

            // 3. DOTween Sequence
            Sequence seq = DOTween.Sequence();

            // A. Pop Scaling (Start small, pop up)
            flyingObj.transform.localScale = Vector3.zero;
            seq.Append(flyingObj.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));

            // B. Jump Arc (Move to shelf slot)
            // Params: EndValue, JumpPower, NumJumps, Duration
            seq.Join(flyingObj.transform.DOJump(targetSlot.position, 0.5f, 1, 0.4f));

            // C. Rotate to match shelf alignment
            seq.Join(flyingObj.transform.DORotate(targetSlot.eulerAngles, 0.4f));

            // 4. On Complete: Cleanup & Finalize
            seq.OnComplete(() => 
            {
                Object.Destroy(flyingObj); // Delete the fake one
                //ConfirmStock(itemData); // Show the real one
                onComplete?.Invoke();
                // Optional: Play "Thud" Sound
            });
        }

        #endregion
    }
}