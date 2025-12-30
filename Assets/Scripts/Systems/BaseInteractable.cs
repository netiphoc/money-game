using UI;
using Utilities;

namespace Systems
{
    public abstract class BaseInteractable : BaseMonoBehaviour, IInteractable
    {
        public virtual string GetInteractionPrompt()
        {
            return $"{name}";
        }

        public virtual BaseUI GetUI()
        {
            return null;
        }

        public virtual void OnInteract(PlayerInteraction player)
        {
        }

        public virtual void OnAltInteract(PlayerInteraction player)
        {
        }
    }
}