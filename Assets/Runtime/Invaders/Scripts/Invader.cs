using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Invader : MonoBehaviour
{
    public Movement Movement { get; private set; }

    public Sprite[] invaderSprite;

    private byte currentInvaderSprite;

    private SpriteRenderer spriteRenderer;

    public bool NextMovementWillExitArea
    {
        get
        {
            return gameObject.transform.position.x < 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Movement = new()
        {
            @object = this.gameObject
        };

        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.freezeRotation = true;
    }

    /// <summary>
    /// Make invader move />
    /// </summary>
    /// <param name="direction"></param>
    public void MoveInvader(MovementGemericAtributts movementGemeric)
    {
        Movement.Move(movementGemeric);
        AtualizeCurrentFreame();
    }

    /// <summary>
    /// Make animation in sprite
    /// </summary>
    private void AtualizeCurrentFreame()
    {
        spriteRenderer.sprite = invaderSprite[currentInvaderSprite];
        currentInvaderSprite = (byte)(currentInvaderSprite == 0 ? 1 : 0);
    }
}
