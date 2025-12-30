using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))] 
public class SimpleSpriteLoop : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public List<Sprite> sprites;     
    public float intervalTime = 0.25f;    

    private int index = 0;
    private float timer = 0;

    void Start()
    {
        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (sprites.Count == 0) return;

        timer += Time.deltaTime;
        if (timer >= intervalTime)
        {
            timer = 0;
            index++;

            if (index >= sprites.Count) index = 0;

            spriteRenderer.sprite = sprites[index];
        }
    }
}