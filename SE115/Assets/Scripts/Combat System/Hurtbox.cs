using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Hurtbox : MonoBehaviour
{
    public IDamageable health;
    private void Awake()
    {
        if (health == null)
            health = gameObject.GetComponentInParent<IDamageable>();
    }
}
