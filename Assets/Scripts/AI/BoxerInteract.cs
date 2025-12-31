using Systems;
using UI;
using UI.Gameplay;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(BoxerController))]
    public class BoxerInteract : BaseInteractable
    {
        private BoxerController _boxerController;

        private UIBoxerStat _uiBoxerStat;
        protected override void Awake()
        {
            base.Awake();
            _boxerController = GetComponent<BoxerController>();
        }

        protected override void Start()
        {
            base.Start();
            _uiBoxerStat = UIManager.Instance.UIBoxerStat;
        }

        protected override void InitInteractionPrompts()
        {
            AddInteractionPrompt(new []
            {
                new InteractionPromptData
                {
                    Icon = KeyIcon.MOUSE_LEFT_CLICK,
                    Prompt = "PUSH",
                },
                new InteractionPromptData
                {
                    Icon = KeyIcon.MOUSE_RIGHT_CLICK,
                    Prompt = "FIGHT",
                }
            });
        }

        public override BaseUI GetUI()
        {
            _uiBoxerStat.SetBoxerController(_boxerController);
            return _uiBoxerStat;
        }

        public override void OnInteract(PlayerInteraction player)
        {
        }

        public override void OnAltInteract(PlayerInteraction player)
        {
            if(UIManager.Instance.UITablet.Visible) return;
            UIManager.Instance.UITablet.ShowQuickLeagueBoxer(_boxerController);
        }
    }
}