using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerSkillDisplay : MonoBehaviour
{
    [Header("Skill Identity")]
    public int skillIndex;
    public SkillBase skill;

    [Header("Player Refrences")]
    public PlayerSkillManager playerSkill;

    [Header("UI References")]
    public Image skillIcon;          
    public Image cooldownOverlay;     
    public TextMeshProUGUI cooldownText; 

    private void Awake()
    {
        if (playerSkill == null)
            playerSkill = FindFirstObjectByType<PlayerSkillManager>();

        playerSkill.onSkillLeared += ActiveSkill;
        playerSkill.onCooldownChanged += UpdateSkillCooldown;
        playerSkill.onSkillUse += UseSkill;
    }
    void Start()
    {
        cooldownOverlay.fillAmount = 0;
        cooldownText.text = " ";
        skillIcon.sprite = null;

        gameObject.SetActive(false);
    }
    public void ActiveSkill(int index, SkillBase newSkill)
    {
        skillIndex = index;
        skill = newSkill;
        skillIcon.sprite = newSkill.icon;

        gameObject.SetActive(true);
    }
    public void UpdateSkillCooldown(int index, float newCooldown)
    {
        if (skillIndex == index)
        {
            if (newCooldown != 0.0f)
                cooldownText.text = Mathf.CeilToInt(newCooldown).ToString();
            else
                cooldownText.text = " ";
            cooldownOverlay.fillAmount = newCooldown / skill.cooldownTime;
        }
    }
    public void UseSkill(int index)
    {
        if(skillIndex == index)
            cooldownOverlay.fillAmount = 1;
    }
}