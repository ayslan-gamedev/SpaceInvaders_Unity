using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float velocity = 10f;

    private const string Axis_Horizontal = "Horizontal";
    
    private Movement movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = new()
        {
            @object = this.gameObject
        };
    }

    // Update is called once per frame
    void Update()
    {
        movement.Move(Vector2.right * Input.GetAxisRaw(Axis_Horizontal), velocity, true);
    }
}