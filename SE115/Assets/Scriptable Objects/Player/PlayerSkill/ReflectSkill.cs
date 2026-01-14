using UnityEngine;

[CreateAssetMenu(menuName = "PlayerSkills/ReflectSkill")]
public class ReflectSkill : SkillBase
{
    public override bool CanUse(PlayerController player)
    {
        return !player.isJumping && !player.isShielding;
    }
    public override void Cast(Transform castPosition, PlayerController player)
    {
        player.StartShield();
    }
    public override void OnSkillStart(PlayerController player)
    {
        base.OnSkillStart(player);
    }
    public override void OnSkillEnd(PlayerController player)
    {
        base.OnSkillEnd(player);
        player.skill.RemoveSkill(this);
    }
}