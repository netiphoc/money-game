using UnityEngine.UI;

namespace UIs
{
    public abstract class BaseUIButton : BaseUI
    {
        protected Button ActiveButton;

        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out ActiveButton);
        }
    }
}