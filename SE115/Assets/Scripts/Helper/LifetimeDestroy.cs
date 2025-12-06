using UnityEngine;

public class LifetimeDestroy : MonoBehaviour
{
    [SerializeField] float lifetime = 10.0f;
    [SerializeField] bool destroyAfterLifetime = true;

    private void OnEnable()
    {
        Invoke("DestroyObject", lifetime);
    }
    private void DestroyObject()
    {
        if(destroyAfterLifetime)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
