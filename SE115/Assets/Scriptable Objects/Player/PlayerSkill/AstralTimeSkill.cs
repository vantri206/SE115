using UnityEngine;

[CreateAssetMenu(fileName = "AstralTimeSkill", menuName = "Scriptable Objects/AstralTimeSkill")]
public class AstralTimeSkill : SkillBase
{
    [Header("Skill Setting")]
    [SerializeField] private float duration = 5.0f;
    [SerializeField] private float manaCost = 50f;
    [SerializeField] private float astralHP = 1.0f;
    [SerializeField] private Color astralColor = new Color(1f, 0.5f, 0.5f, 0.8f);

    public GameObject astralPrefab;
    public override void Cast(Transform castPosition, PlayerController player)
    {
        var runner = player.gameObject.AddComponent<AstralTimeRunner>();
        runner.Initialize(this, player, duration, astralPrefab, astralColor, astralHP);
    }
    public override void OnSkillStart(PlayerController player)
    {
        base.OnSkillStart(player);

        IManable playerMana = player.mana;
        playerMana.ConsumeMana(manaCost);
    }
    public override void OnSkillEnd(PlayerController player)
    {
        base.OnSkillEnd(player);
    }
    public override bool CanUse(PlayerController player)
    {
        IManable manable = player.GetComponent<IManable>();
        if (manable != null)
        {
            if (manable.currentMana >= manaCost)
                return true;
        }
        return false;
    }
}
