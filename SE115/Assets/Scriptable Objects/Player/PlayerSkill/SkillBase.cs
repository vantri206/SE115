using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    [Header("Skill Settings")]
    public string skillName;
    public float cooldownTime;
    public Sprite icon;

    [Header("Player State Settings")]
    public bool usePlayerSkillState = true;

    [Header("Animation Settings")]
    public string aniBoolParameterName;
    public abstract void Cast(Transform castPosition, PlayerController player);
    public abstract bool CanUse(PlayerController player);
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