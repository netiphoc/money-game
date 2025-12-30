using TMPro;
using UnityEngine;

namespace UI
{
    public class UIHoverKey : BaseUI
    {
        [SerializeField] private TMP_Text textHover;
        
        public void SetText(string text)
        {
            textHover.SetText(text);    
        }
    }
}