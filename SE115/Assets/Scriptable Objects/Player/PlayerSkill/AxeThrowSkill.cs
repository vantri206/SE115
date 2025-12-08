using UnityEngine;

[CreateAssetMenu(menuName = "PlayerSkills/Axe Throw")]
public class AxeThrowSkill : SkillBase
{
    public GameObject axePrefab;

    public Vector2 throwForce = Vector2.right;

    private void Awake()
    {

    }
    public override void Cast(Transform castPosition, PlayerController player)
    {
        GameObject axe = Instantiate(axePrefab, castPosition.position, Quaternion.identity, ProjectilesPool.Instance);
        LinearProjectiles axeProjectiles = axe.GetComponent<LinearProjectiles>();

        if(axeProjectiles != null)
        {
            axeProjectiles.SetDirectionX(player.facingDirection);
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
}