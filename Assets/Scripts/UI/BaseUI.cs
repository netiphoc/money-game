using Utilities;

namespace UI
{
    public abstract class BaseUI : BaseMonoBehaviour
    {
        public virtual bool Visible => gameObject.activeSelf;

        public virtual void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}