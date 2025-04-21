using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float lifetime = 1f;
    public float floatSpeed = 1f;
    public float fadeSpeed = 1f;
    public Color normalColor = Color.white;
    public Color criticalColor = Color.red;

    private void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        Destroy(gameObject, lifetime);
    }

    public void SetText(int amount, bool isCritical)
    {
        // Set text
        textMesh.text = amount.ToString();

        // Set color
        textMesh.color = isCritical ? criticalColor : normalColor;

        // Set size for critical hits
        if (isCritical)
            textMesh.fontSize *= 1.5f;
    }

    private void Update()
    {
        // Float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        float alpha = textMesh.color.a;
        alpha -= fadeSpeed * Time.deltaTime;
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);
    }
}