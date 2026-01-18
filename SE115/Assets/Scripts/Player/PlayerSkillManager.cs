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
    public event Action<int> onSkillRemoved; 

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
            if (skillsSlot[i] != null && skillsCooldownTimer[i] < skillsSlot[i].cooldownTime)
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

            float cooldown = skillsSlot[currentSkillIndex].cooldownTime;
            onCooldownChanged?.Invoke(currentSkillIndex, cooldown, cooldown);

            isAniSkillFinished = false;
            currentSkill.OnSkillStart(player);
        }
    }

    public void TriggerSkill()
    {
        if (currentSkill != null)
        {
            currentSkill.Cast(throwFirePoint, player);
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

        if (skillsSlot[index] == null) return false;

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

    public void UnlockSkill(SkillBase skill, bool shouldMessages, int index = -1)
    {
        if (!unlockedSkills.Contains(skill))
        {
            unlockedSkills.Add(skill);

            if (index == -1)
            {
                int emptyIndex = skillsSlot.IndexOf(null);
                if (emptyIndex != -1)
                {
                    index = emptyIndex;
                    skillsSlot[index] = skill;
                    skillsCooldownTimer[index] = skill.cooldownTime; 
                }
                else
                {
                    index = skillsSlot.Count;
                    skillsSlot.Add(skill);
                    skillsCooldownTimer.Add(skill.cooldownTime);
                }
            }
            else
            {
                if (index < skillsSlot.Count)
                {
                    skillsSlot[index] = skill;
                    skillsCooldownTimer[index] = skill.cooldownTime;
                }
                else
                {
                    skillsSlot.Add(skill);
                    skillsCooldownTimer.Add(skill.cooldownTime);
                    index = skillsSlot.Count - 1;
                }
            }

            Debug.Log("Player learning: " + skill.ToString());

            if (shouldMessages)
                if(ScrollMessenger.Instance != null)
                {
                    ScrollMessenger.Instance.ShowMessage("Player Learning: " + skill.skillName);

                }

            onSkillLeared?.Invoke(index, skill);
        }
    }
    public void RemoveSkill(SkillBase skill)
    {
        if (unlockedSkills.Contains(skill))
        {
            unlockedSkills.Remove(skill);

            int index = skillsSlot.IndexOf(skill);
            if (index != -1)
            {
                skillsSlot[index] = null;

                skillsCooldownTimer[index] = 0f;

                if (currentSkill == skill)
                {
                    currentSkill = null;
                    currentSkillIndex = -1;
                }
                onSkillRemoved?.Invoke(index);

                Debug.Log("Skill Removed: " + skill.name);
            }
        }
    }

    public void AE_FinishSkillAni() => isAniSkillFinished = true;
}