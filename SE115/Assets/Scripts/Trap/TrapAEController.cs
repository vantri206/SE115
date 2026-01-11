using UnityEngine;

public class TrapAnimationEvents : MonoBehaviour
{
    [SerializeField] private SwordDamage damageScript;
    public void AE_ResetDamage()
    {
        if (damageScript != null)
        {
            damageScript.ResetHitList();
        }
    }
}