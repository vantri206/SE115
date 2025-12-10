using System.Linq;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public bool isDestroyed = true;
    public float lifetime = 0.0f;

    public LayerMask destroyAfterTriggerObject;

    private void OnEnable()
    {
        Invoke("DestroyObject", lifetime);
    }
    private void DestroyObject()
    {
        if(isDestroyed)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(((1 << collision.gameObject.layer) & destroyAfterTriggerObject) != 0)
        {
            DestroyObject();
        }
    }
}
