using UnityEngine;

public abstract class EnemyWeapon : MonoBehaviour
{
    public abstract void PerformAttack();
    public virtual void FinishAttack() { }
}