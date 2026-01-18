using UnityEngine;

[CreateAssetMenu(menuName = "PlayerSkills/Throw")]
public class ThrowProjectilesSkill : SkillBase
{
    [Header("Skill Setting")]
    public GameObject projectilesPrefab;
    public Vector2 throwForce = Vector2.right;

    [System.NonSerialized]
    private bool isHasAxe = true;

    private void OnEnable()
    {
        isHasAxe = true;
    }

    public override void Cast(Transform castPosition, PlayerController player)
    {
        GameObject axe = Instantiate(projectilesPrefab, castPosition.position, Quaternion.identity, ProjectilesPool.Instance);
        LinearProjectiles linearProjectiles = axe.GetComponent<LinearProjectiles>();

        if (linearProjectiles != null)
        {
            linearProjectiles.SetDirectionX(new Vector2(player.facingDirection.x, 0f));
        }

    }
    public override void OnSkillStart(PlayerController player)
    {
        base.OnSkillStart(player);
    }
    public override void OnSkillEnd(PlayerController player)
    {
        base.OnSkillEnd(player);
    }
    public override bool CanUse(PlayerController player) => isHasAxe;
}