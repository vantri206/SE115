using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] private float lifetime = 3.0f; 
                                                  

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}