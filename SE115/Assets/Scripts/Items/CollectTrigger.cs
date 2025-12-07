using Unity.VisualScripting;
using UnityEngine;

public class CollectTrigger : MonoBehaviour
{
    public ICollectable collectable;
    private void Awake()
    {
        if (collectable == null)
            collectable = gameObject.GetComponentInParent<ICollectable>();
    }
}
