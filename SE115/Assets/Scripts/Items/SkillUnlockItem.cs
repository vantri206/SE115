using UnityEngine;

public class SkillUnlockItem : MonoBehaviour, ICollectable
{
    [SerializeField] private SkillBase skillUnlock;
    public void Collect(GameObject target)
    {
        PlayerSkillManager targetSkill = target.GetComponentInChildren<PlayerSkillManager>();

        if (targetSkill != null)
        {
            if(skillUnlock.showMessages == true)
                targetSkill.UnlockSkill(skillUnlock, true);
            else
                targetSkill.UnlockSkill(skillUnlock, false);
            Destroy(gameObject);
        }
    }
}
