using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public float fadeSpeed = 3f; 
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        Color color = spriteRenderer.color;
        color.a -= fadeSpeed * Time.deltaTime;
        spriteRenderer.color = color;

        if (color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}