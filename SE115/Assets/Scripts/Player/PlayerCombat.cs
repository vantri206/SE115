using System;
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

    [Header("Skill Setting")]
    public int currentSkillIndex = -1;
    public SkillBase currentSkill;
    public SkillBase[] skillsSlot;
    public Transform throwFirePoint;

    private float comboTimer = 0.0f;
    private float[] skillsCooldownTimer;

    public bool isAniSkillFinished = false;

    private void Awake()
    {
        if (player == null)
            player = GetComponent<PlayerController>();

        if (sword != null) 
            sword.gameObject.SetActive(false);

        skillsCooldownTimer = new float[skillsSlot.Length];
        for (int i = 0; i < skillsCooldownTimer.Length; i++)
            skillsCooldownTimer[i] = 0.0f;
    }

    private void Update()
    {
        comboTimer += Time.deltaTime;
        for (int i = 0; i < skillsCooldownTimer.Length; i++)
            skillsCooldownTimer[i] += Time.deltaTime; 

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
    #region For Skill
    public void SetCurrentSkill(int index)
    {
        if (index < 0 || index >= skillsSlot.Length)
        {
            currentSkillIndex = -1;
            currentSkill = null;
        }
        else
        {
            currentSkillIndex = index;
            currentSkill = skillsSlot[index];
        }
    }
    public void StartSkill()
    {
        if (currentSkill != null)
        {
            skillsCooldownTimer[currentSkillIndex] = 0.0f;
            isAniSkillFinished = false;
            currentSkill.OnSkillStart(player);
        }
    }
    public void TriggerSkill()
    {
        if (currentSkill != null)
        {
            currentSkill.Cast(throwFirePoint, player);      //Co the thay doi logic nay thanh cast point rieng voi moi skill, dung offset mb
        }
    }
    public void FinishSkill()
    {
        if (currentSkill != null)
        {
            currentSkill.OnSkillEnd(player);
            SetCurrentSkill(-1);
        }
    }
    public bool CanUseSkill(int index)
    {
        if (skillsCooldownTimer[index] >= skillsSlot[index].cooldownTime)
            return true;
        else 
            return false;
    }
    #endregion
    public void AE_FinishSkillAni() => isAniSkillFinished = true;
}