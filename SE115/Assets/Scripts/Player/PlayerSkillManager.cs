using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;

    [Header("Skill Setting")]
    public int currentSkillIndex = 0;
    public SkillBase currentSkill;
    public HashSet<SkillBase> unlockedSkills = new HashSet<SkillBase>();
    public List<SkillBase> skillsSlot = new List<SkillBase>();
    public Transform throwFirePoint;

    private List<float> skillsCooldownTimer = new List<float>();

    public event Action<int, float, float> onCooldownChanged;
    public event Action<int, SkillBase> onSkillLeared;
    public event Action<int> onSkillUse;

    public bool isAniSkillFinished = false;

    private void Awake()
    {
        if (player == null)
            player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        for (int i = 0; i < skillsSlot.Count; i++)
        {
            if (skillsCooldownTimer[i] < skillsSlot[i].cooldownTime)
            {
                skillsCooldownTimer[i] += Time.deltaTime;

                float currentTimer = skillsCooldownTimer[i];
                float cooldownTime = skillsSlot[i].cooldownTime;
                float cooldownRemaining = Mathf.Max(0.0f, cooldownTime - currentTimer);

                onCooldownChanged?.Invoke(i, cooldownRemaining, cooldownTime);
            }
        }
    }
    public void SetCurrentSkill(int index)
    {
        if (index < 0 || index >= skillsSlot.Count)
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
        if (currentSkill != null && CanUseSkill(currentSkillIndex))
        {
            skillsCooldownTimer[currentSkillIndex] = 0.0f;
            onSkillUse?.Invoke(currentSkillIndex);
            onCooldownChanged(currentSkillIndex, Mathf.Max(0.0f, 
                        Mathf.CeilToInt(skillsSlot[currentSkillIndex].cooldownTime)), 
                        Mathf.Max(0.0f, Mathf.CeilToInt(skillsSlot[currentSkillIndex].cooldownTime)));

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
        if (index < 0 || index >= skillsSlot.Count) return false;

        if (skillsCooldownTimer[index] < skillsSlot[index].cooldownTime || 
            !skillsSlot[index].CanUse(player))
        {
            return false;
        }

        return true;
    }
    public bool HasSkill(SkillBase skill)
    {
        return unlockedSkills.Contains(skill);
    }
    public void UnlockSkill(SkillBase skill, int index = -1)
    {
        if (!unlockedSkills.Contains(skill))
        {
            unlockedSkills.Add(skill);
            if (index == -1)
            {
                index = skillsSlot.Count;
                skillsSlot.Add(skill);
                skillsCooldownTimer.Add(skill.cooldownTime);
            }
            else
            {
                skillsSlot[index] = skill;
                skillsCooldownTimer[index] = skill.cooldownTime;
            }

            Debug.Log("Player learning: " + skill.ToString());
            ScrollMessenger.Instance.ShowMessage("Player Learning: " + skill.ToString());
            onSkillLeared?.Invoke(index, skill);

        }
    }
    public void AE_FinishSkillAni() => isAniSkillFinished = true;
}