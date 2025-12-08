using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    public enum PlayerSkill
    {
        None,
        AxeThrow,
    }

    private HashSet<PlayerSkill> unlockedSkills = new HashSet<PlayerSkill>();
    public bool HasSkill(PlayerSkill skill)
    {
        return unlockedSkills.Contains(skill);
    }

    public void UnlockSkill(PlayerSkill skill)
    {
        if(!unlockedSkills.Contains(skill))
        {
            unlockedSkills.Add(skill);

            Debug.Log("Player learning " + skill.ToString());

            //Action UI

        }
    }
}
