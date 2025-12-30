using UnityEngine;

[CreateAssetMenu(menuName = "PlayerSkills/Axe Throw")]
public class AxeThrowSkill : SkillBase
{
    [Header("Skill Setting")]
    public GameObject axePrefab;
    public Vector2 throwForce = Vector2.right;

    [System.NonSerialized]
    private bool isHasAxe = true;

    private void OnEnable()
    {
        isHasAxe = true;
    }

    public override void Cast(Transform castPosition, PlayerController player)
    {
        GameObject axe = Instantiate(axePrefab, castPosition.position, Quaternion.identity, ProjectilesPool.Instance);
        BoomerangProjectiles axeProjectiles = axe.GetComponent<BoomerangProjectiles>();

        if (axeProjectiles != null)
        {
            isHasAxe = false;

            axeProjectiles.Initialize(player.transform, player.facingDirection, 
            () =>
            {
                SwordDamage damage = axeProjectiles.GetComponent<SwordDamage>();
                if(damage != null)
                {
                    damage.ResetHitList();
                }
            },
            () => 
            isHasAxe = true);
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