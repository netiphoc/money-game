using TMPro;
using UnityEngine;

namespace UI.Gameplay
{
    public class UIClock : BaseUI
    {
        [SerializeField] private TMP_Text textTime;
        
        protected override void Start()
        {
            base.Start();
            GameManager.Instance.GameTimeManager.OnGameMinuteTick += OnGameMinuteTick;
            OnGameMinuteTick(GameManager.Instance.GameTimeManager.GetFormattedTime());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameManager.Instance.GameTimeManager.OnGameMinuteTick -= OnGameMinuteTick;
        }

        private void OnGameMinuteTick(string time)
        {
            textTime.SetText(time);
        }
    }
}