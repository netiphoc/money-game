using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text textMesh;
    public float moveSpeed = 1f;
    public float fadeTime = 1f;
    private float alpha = 1f;

    public void Setup(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
    }

    private void Update()
    {
        // Float Up
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade Out
        alpha -= Time.deltaTime / fadeTime;
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);

        if (alpha <= 0) Destroy(gameObject);
    }
}