using UnityEngine;

public class SkillUnlockItem : MonoBehaviour, ICollectable
{
    [SerializeField] private SkillBase skillUnlock;
    public void Collect(GameObject target)
    {
        PlayerSkillManager targetSkill = target.GetComponentInChildren<PlayerSkillManager>();

        if (targetSkill != null)
        {
            targetSkill.UnlockSkill(skillUnlock);
            Destroy(gameObject);
        }
    }
}
