using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform effectsPool;

    [Header("Dash Effect")]
    [SerializeField] private DashEffect dashEffect;

    private void Awake()
    {
        if (player == null)
            player = GetComponentInParent<PlayerController>();

    }
    #region For Dash Effect
    public void SpawnDashEffect()
    {
        dashEffect.StartEffect();
    }
    public void FinishDashEffect()
    {
        dashEffect.FinishEffect();
    }
    #endregion
}