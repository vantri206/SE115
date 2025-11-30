using NUnit.Framework;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    public SwordDamage damage;
    public void Awake()
    {
        if(damage == null)
            damage = GetComponent<SwordDamage>();
    }
    private void OnEnable()
    {
        damage.ResetHitList();
    }
}