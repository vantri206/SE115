using System.Collections.Generic;
using UnityEngine;

public static class CombatManager
{
    private static readonly Dictionary<DamageTeam, DamageTeam[]> canDealDamageMap = new Dictionary<DamageTeam, DamageTeam[]>()
    {
        { DamageTeam.Player, new DamageTeam[]{ DamageTeam.Enemy } },
        { DamageTeam.Enemy,  new DamageTeam[]{ DamageTeam.Player } },
        { DamageTeam.Trap,   new DamageTeam[]{ DamageTeam.Player } },
    };
    public static bool CanDamage(DamageTeam attacker, DamageTeam target)
    {
        if (!canDealDamageMap.ContainsKey(attacker)) return false;
        return System.Array.Exists(canDealDamageMap[attacker], team => team == target);
    }
}
