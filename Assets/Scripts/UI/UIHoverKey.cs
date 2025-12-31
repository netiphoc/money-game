using Systems;
using UnityEngine;

namespace UI
{
    public class UIHoverKey : BaseUI
    {
        [SerializeField] private UIHoverKeySlot uiHoverKeySlot;
        [SerializeField] private Transform containerSlot;
        
        private PoolingUI<UIHoverKeySlot> _pooling;
        private PoolingUI<UIHoverKeySlot> GetPool()
        {
            return _pooling ??= new PoolingUI<UIHoverKeySlot>(uiHoverKeySlot);
        }
        
        public void SetInteractionData(InteractionPromptData[] data)
        {
            GetPool().ClearPool();
            foreach (var interactionPromptData in data)
            {
                UIHoverKeySlot slot = GetPool().RequestRecycle(containerSlot);
                slot.SetInteractionPromptData(interactionPromptData);
            }
        }
    }
}