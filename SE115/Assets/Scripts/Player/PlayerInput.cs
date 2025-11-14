using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerController player;
    public Vector2 moveInput { get; set; }
    private void Awake()
    {
        player = gameObject.GetComponent<PlayerController>();
    }
    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y);
    }
}
