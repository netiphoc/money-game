using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class UISummary : BaseUI
    {
        [SerializeField] private TMP_Text textIncome;
        [SerializeField] private TMP_Text textSupplyCosts;
        [SerializeField] private TMP_Text textUpgradeCosts;
        [SerializeField] private TMP_Text textProfit;
        [SerializeField] private Button nextDay;

        protected override void OnEnable()
        {
            base.OnEnable();
            nextDay.onClick.AddListener(OnClickNextDay);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            nextDay.onClick.RemoveListener(OnClickNextDay);
        }
        
        private void OnClickNextDay()
        {
            // Reload scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void UpdateSummary()
        {
            textIncome.SetText($"${GameManager.Instance.totalIncome}");
            textSupplyCosts.SetText($"${GameManager.Instance.totalSupplyCosts}");
            textUpgradeCosts.SetText($"${GameManager.Instance.totalUpgradeCosts}");
            textProfit.SetText($"${GameManager.Instance.totalIncome - GameManager.Instance.totalUpgradeCosts - GameManager.Instance.totalSupplyCosts}");
        }

        public override void SetVisible(bool visible)
        {
            base.SetVisible(visible);
            
            if (visible)
            {
                UpdateSummary();
            }
        }
    }
}