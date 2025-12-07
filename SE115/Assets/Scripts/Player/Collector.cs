using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] private GameObject owner;
    private void Awake()
    {
        if (owner == null)
            owner = transform.parent.gameObject;
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        HandleCollision(collider);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleCollision(collider);
    }
    private void HandleCollision(Collider2D collider)
    {
        if (collider.gameObject.layer != LayerMask.NameToLayer("CollectTrigger")) return;

        ICollectable targetCollectable = null;
        CollectTrigger collectTrigger = collider.GetComponent<CollectTrigger>();

        if (collectTrigger != null)
            targetCollectable = collectTrigger.collectable;

        if (targetCollectable != null)
        {
            targetCollectable.Collect(owner);
        }
    }
}
