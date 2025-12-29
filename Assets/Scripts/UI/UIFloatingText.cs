using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIFloatingText : BaseUI
    {
        [Header("Settings")]
        public float moveSpeed = 50f; // Pixels per second
        public float fadeTime = 1f;
    
        private TMP_Text uiText;
        private RectTransform rectTransform;
        private float alpha = 1f;
        private float timer = 0f;

        protected override void Awake()
        {
            base.Awake();
            
            uiText = GetComponent<TMP_Text>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void Setup(string text, Color color)
        {
            uiText.text = text;
            uiText.color = color;
            alpha = 1f;
            timer = 0f;
        }

        protected override void Update()
        {
            base.Update();
            
            timer += Time.deltaTime;

            // 1. Move Up (Using Anchored Position for UI)
            rectTransform.anchoredPosition += Vector2.up * (moveSpeed * Time.deltaTime);

            // 2. Fade Out
            if (timer > (fadeTime * 0.5f)) // Start fading halfway through life
            {
                alpha -= Time.deltaTime / (fadeTime * 0.5f);
                uiText.color = new Color(uiText.color.r, uiText.color.g, uiText.color.b, alpha);
            }

            // 3. Destroy
            if (timer >= fadeTime)
            {
                SetVisible(false);
            }
        }
    }
}