using UnityEngine;
using System.Collections.Generic;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerStatsBarUI statsBarUI;
    [SerializeField] private PlayerHealthTextUI healthTextUI;
    [SerializeField] private PlayerJumpCountUI jumpCountUI;
    [SerializeField] private PlayerSkillContainer skillContainerUI;

    private PlayerController currentPlayer;

    public void Initialize(PlayerController player)
    {
        currentPlayer = player;

        PlayerHealth currentHealth = player.GetComponent<PlayerHealth>();
        PlayerMana currentMana = player.GetComponent<PlayerMana>();
        PlayerSkillManager currentSkill = player.GetComponent<PlayerSkillManager>();

        if (currentHealth != null)
        {
            if (statsBarUI != null) statsBarUI.Initialize(currentHealth, currentMana);
            if (healthTextUI != null) healthTextUI.Initialize(currentHealth);
        }

        if(currentPlayer != null)
        {
            if (jumpCountUI != null) jumpCountUI.Initialize(currentPlayer);
        }

        if(currentSkill != null)
        {
            if (skillContainerUI != null) skillContainerUI.Initialize(currentSkill);
        }

    }
}