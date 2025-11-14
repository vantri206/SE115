using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public StateManager stateManager;
    public Rigidbody2D myRigidbody;
    public Animator animator;
    
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        stateManager = gameObject.GetComponent<StateManager>();
    }

    void Update()
    {
        
    }
}
