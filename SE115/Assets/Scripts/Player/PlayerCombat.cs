using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;

    [Header("Sword")]
    public PlayerSword sword;

    [Header("Attack Settings")]
    public int comboCount = 0;
    public float comboResetTime = 1.5f;

    private float comboTimer = 0.0f;

    public bool isAniSkillFinished = false;

    private void Awake()
    {
        if (player == null)
            player = GetComponent<PlayerController>();

        if (sword != null) 
            sword.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (comboTimer > comboResetTime)
        {
            comboCount = 0;
        }
    }
    #region For Sword Attack
    public void SwordAttack()
    {
        player.isAttacking = true;
        EnableSword();

        comboTimer = float.NegativeInfinity;

        if (!player.CheckOnGround())
        {
            player.onAirAttackLeft -= 1;
        }
    }
    public void FinishAttack()
    {
        player.isAttacking = false;
        DisableSword();

        comboTimer = 0;

        comboCount++;
        if (comboCount >= 4) comboCount = 0;
    }

    public void EnableSword() { sword.gameObject.SetActive(true); }
    public void DisableSword() { sword.gameObject.SetActive(false); }
    #endregion
}