using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillContainer : MonoBehaviour
{
    [Header("Skill System")]
    [SerializeField] private GameObject skillSlotPrefab;

    private List<PlayerSkillDisplay> skillSlots = new List<PlayerSkillDisplay>();

    private PlayerSkillManager playerSkill;

    public void Initialize(PlayerSkillManager playerSkill)
    {
        this.playerSkill = playerSkill;

        foreach (Transform child in this.transform)
            Destroy(child.gameObject);
        skillSlots.Clear();

        for (int i = 0; i < playerSkill.skillsSlot.Count; i++)
        {
            CreateSlotUI(i, playerSkill.skillsSlot[i]);
        }

        playerSkill.onSkillLeared += CreateSlotUI;

    }
    private void OnDisable()
    {
        if (playerSkill != null)
        {
            playerSkill.onSkillLeared -= CreateSlotUI;
        }
    }
    private void CreateSlotUI(int index, SkillBase skillData)
    {
        GameObject newSkillSlot = Instantiate(skillSlotPrefab, this.transform);

        PlayerSkillDisplay newSkillUI = newSkillSlot.GetComponent<PlayerSkillDisplay>();

        newSkillUI.Initialize(playerSkill, index, skillData);

        skillSlots.Add(newSkillUI);
    }
}