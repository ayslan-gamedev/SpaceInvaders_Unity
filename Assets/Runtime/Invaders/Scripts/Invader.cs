using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Invader : MonoBehaviour
{
    public Sprite[] invaderSprite;

    public Life Life { get; set; }
    public Movement Movement { get; set; }

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        Movement = new Movement
        {
            @object = this.gameObject
        };

        Life = new Life
        {
            value = 0
        };

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private int currentSpriteIndex = 0;

    /// <summary>
    /// Atualize animation of invader
    /// </summary>
    public void AtualizeCurrentFrame()
    {
        spriteRenderer.sprite = invaderSprite[currentSpriteIndex];
        currentSpriteIndex = (currentSpriteIndex + 1) % invaderSprite.Length;
    }
}