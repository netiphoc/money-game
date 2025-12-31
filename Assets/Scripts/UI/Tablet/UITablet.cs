using System.Collections.Generic;
using UI.Tablet.Leagues;
using UnityEngine;

namespace UI.Tablet
{
    public class UITablet : BaseUI
    {
        [field: SerializeField] public UITabletHome UITabletHome { get; private set; }
        [field: SerializeField] public UITabletManager UITabletManager { get; private set; }
        [field: SerializeField] public UITabletLeagues UITabletLeagues { get; private set; }

        private readonly List<BaseUI> _allApp = new List<BaseUI>();
        
        protected override void Awake()
        {
            base.Awake();
            InitApp();
        }  

        private void InitApp()
        {
            _allApp.Add(UITabletHome);
            _allApp.Add(UITabletManager);
            _allApp.Add(UITabletLeagues);
        }

        public void ShowHome()
        {
            ShowUI(UITabletHome);
        }
        
        public void ShowUI(BaseUI ui)
        {
            foreach (var app in _allApp)
            {
                app.SetVisible(app == ui);
            }
        }

        public void ShowQuickLeagueBoxer(BoxerController boxerController)
        {
            if(Visible) return;
            UIManager.Instance.ShowUI(UIManager.Instance.UITablet);
            ShowUI(UITabletLeagues);
            UITabletLeagues.OnClickedBoxer(boxerController);
        }
    }
}