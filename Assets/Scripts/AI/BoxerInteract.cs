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

        public override string GetInteractionPrompt()
        {
            return _boxerController.stats.boxerName;
        }

        public override BaseUI GetUI()
        {
            _uiBoxerStat.SetBoxerController(_boxerController);
            return _uiBoxerStat;
        }

        public override void OnInteract(PlayerInteraction player)
        {
            Debug.Log($"OnInteract: {_boxerController.stats.boxerName}");
        }

        public override void OnAltInteract(PlayerInteraction player)
        {
            if(UIManager.Instance.UITablet.Visible) return;
            UIManager.Instance.UITablet.ShowQuickLeagueBoxer(_boxerController);
            Debug.Log($"OnAltInteract: {_boxerController.stats.boxerName}");
        }
    }
}