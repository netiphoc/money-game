using UnityEngine;
using TMPro;
using UI;

public class FloatingText : BaseUI
{
    [SerializeField] private TMP_Text textMesh;
    public float moveSpeed = 1f;
    public float fadeTime = 1f;
    
    private float _alpha = 1f;

    public void Setup(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        _alpha = 1f;
    }

    protected override void Update()
    {
        base.Update();
        
        // Float Up
        transform.position += Vector3.up * (moveSpeed * Time.deltaTime);

        // Fade Out
        _alpha -= Time.deltaTime / fadeTime;
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, _alpha);

        if (_alpha <= 0)
        {
            SetVisible(false);
        }
    }
}