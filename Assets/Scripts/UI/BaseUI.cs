using Utilities;

namespace UI
{
    public abstract class BaseUI : BaseMonoBehaviour
    {
        public virtual void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}