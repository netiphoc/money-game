using UnityEngine;

namespace UI.Tablet
{
    public class UITabletHome : BaseUI
    {
        [SerializeField] private UITablet uiTablet;
        
        [Header("App")]
        [SerializeField] private UITabletButtonHomeApp uiTabletButtonHomeAppManagement;
        [SerializeField] private UITabletButtonHomeApp uiTabletButtonHomeAppLeagues;

        protected override void OnEnable()
        {
            base.OnEnable();
            uiTabletButtonHomeAppManagement.onClick.AddListener(OnAppClickedManagement);
            uiTabletButtonHomeAppLeagues.onClick.AddListener(OnAppClickedLeagues);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            uiTabletButtonHomeAppManagement.onClick.RemoveListener(OnAppClickedManagement);
            uiTabletButtonHomeAppLeagues.onClick.RemoveListener(OnAppClickedLeagues);
        }

        private void OnAppClickedManagement()
        {
            uiTablet.Show(uiTablet.UITabletManager);
        }

        private void OnAppClickedLeagues()
        {
            uiTablet.Show(uiTablet.UITabletLeagues);
        }
    }
}