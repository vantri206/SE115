using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform effectsPool;

    [Header("Dash Effect")]
    [SerializeField] private DashEffect dashEffect;

    [Header("Hurt Effect")]
    [SerializeField] private GameObject hurtEffect;
    [SerializeField] private Vector3 hurtEffectOffset = new Vector3(1.0f, 1.0f, 0.0f);

    private void Awake()
    {
        if (player == null)
            player = GetComponentInParent<PlayerController>();
        if (effectsPool == null)
            effectsPool = EffectsPool.Instance;

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
    #region For Hurt Effect
    public void PlayHurtEffect()
    {
        float playerDirection = player.facingDirection.x;
        Vector3 effectPos = new Vector3(transform.position.x + hurtEffectOffset.x * playerDirection, 
                                        transform.position.y + hurtEffectOffset.y, transform.position.z);
        GameObject hurtEffectObj = Instantiate(hurtEffect, effectPos, Quaternion.identity, effectsPool);
        hurtEffectObj.transform.localScale = new Vector3(hurtEffect.transform.localScale.x * -playerDirection, 
                                                         hurtEffect.transform.localScale.y, hurtEffect.transform.localScale.z);
    }
    #endregion
}