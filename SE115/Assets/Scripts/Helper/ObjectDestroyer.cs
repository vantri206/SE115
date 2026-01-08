using System.Linq;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    [Header("Destroy Settings")]
    public bool isDestroyed = true;
    public float lifetime = 0.0f;
    public LayerMask destroyAfterTriggerObject;

    [Header("Effect")]
    [SerializeField] private GameObject destroyEffect;

    private void OnEnable()
    {
        CancelInvoke();
        Invoke("DestroyObject", lifetime);
    }
    public void DestroyObject()
    {
        SpawnEffectVFX();

        if (isDestroyed)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void SpawnEffectVFX()
    {
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
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
