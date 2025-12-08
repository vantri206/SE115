using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    [Header("Skill Setting")]
    public string skillName;
    public float cooldownTime;
    public Sprite icon;

    [Header("Animation setting")]
    public string aniBoolParameterName;

    public abstract void Cast(Transform castPosition, PlayerController player);
    public virtual void OnSkillStart(PlayerController player)
    {
        if (aniBoolParameterName != null)
        {
            player.animator.SetBool(aniBoolParameterName, true);
        }
    }
    public virtual void OnSkillEnd(PlayerController player)
    {
        if (aniBoolParameterName != null)
        {
            player.animator.SetBool(aniBoolParameterName, false);
        }
    }
}