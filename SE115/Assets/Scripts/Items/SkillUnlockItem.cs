using UnityEngine;
using static PlayerSkillManager;

public class SkillUnlockItem : MonoBehaviour, ICollectable
{
    [SerializeField] private PlayerSkill skillUnlock = PlayerSkill.None;
    public void Collect(GameObject target)
    {
        PlayerSkillManager playerSkill = target.GetComponentInChildren<PlayerSkillManager>();

        if (playerSkill != null)
        {
            playerSkill.UnlockSkill(skillUnlock);
            Destroy(gameObject);
        }
    }
}
