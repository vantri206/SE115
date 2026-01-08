using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private string hurtboxLayerName = "Hurtbox";

    public IDamageable health;

    private int layerId;

    private void Awake()
    {
        if (health == null)
            health = gameObject.GetComponentInParent<IDamageable>();

        layerId = LayerMask.NameToLayer(hurtboxLayerName);

        if (layerId == -1)
        {
            Debug.LogError("Not found layer hurtbox: " + hurtboxLayerName);
        }
        else
        {
            gameObject.layer = layerId;
        }
    }
}
