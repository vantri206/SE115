using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private StateManager stateManager;

    public Rigidbody2D myRigidbody;
    public Animator animator;
    
    void Start()
    {
        stateManager = new StateManager();
        animator = gameObject.GetComponent<Animator>();
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }
}
