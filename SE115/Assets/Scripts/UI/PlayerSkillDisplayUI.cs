using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerSkillDisplay : MonoBehaviour
{
    [Header("Skill Identity")]
    private int skillIndex;
    private SkillBase skill;

    [Header("Player Refrences")]
    private PlayerSkillManager player;

    [Header("UI References")]
    public Image skillIcon;          
    public Image cooldownOverlay;     
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI keyBindText;

    public void Initialize(PlayerSkillManager playerSkill, int index, SkillBase newSkill)
    {
        this.player = playerSkill;
        this.skillIndex = index;

        skill = newSkill;
        skillIcon.sprite = newSkill.icon;

        cooldownOverlay.fillAmount = 0;
        cooldownText.text = "";

        keyBindText.text = PlayerInput.Instance.GetKeyForSkill(index).ToString();

        player.onCooldownChanged += UpdateSkillCooldown;
        player.onSkillUse += UseSkill;
    }
    private void OnDestroy()
    {
        if (player != null)
        {
            player.onCooldownChanged -= UpdateSkillCooldown;
            player.onSkillUse -= UseSkill;
        }
    }
    public void UpdateSkillCooldown(int index, float timeRemaining, float cooldown)
    {
        if (this.skillIndex != index) return;

        if (timeRemaining > 0)
        {
            cooldownText.text = Mathf.CeilToInt(timeRemaining).ToString();
            cooldownOverlay.fillAmount = timeRemaining / cooldown;
        }
        else
        {
            cooldownText.text = "";
            cooldownOverlay.fillAmount = 0;
        }
    }
    public void UseSkill(int index)
    {
        if (this.skillIndex == index)
        {
            cooldownOverlay.fillAmount = 1;
        }
    }
}