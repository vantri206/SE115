using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public StateManager stateManager;
    public Rigidbody2D myRigidbody;
    public Animator animator;
    public Sword sword;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        stateManager = gameObject.GetComponent<StateManager>();
        sword.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }
}
