using UnityEngine;

public class AstralTimeRunner : MonoBehaviour
{
    private AstralTimeSkill astralSkill;
    private PlayerController player;
    private GameObject currentAstral;
    private float timer;

    private PlayerSaveData saveData = new PlayerSaveData();

    public void Initialize(AstralTimeSkill astralSkill, PlayerController player, float duration, GameObject astral, Color astralColor, float astralHP)
    {
        this.astralSkill = astralSkill;
        this.player = player;
        this.timer = duration;

        SavePlayerData();

        if (astral != null)
        {
            currentAstral = Instantiate(astral, saveData.position, Quaternion.identity);
            currentAstral.transform.rotation = saveData.rotation;
            currentAstral.transform.localScale = saveData.localScale;
        }

        player.health.SetCurrentHeal(astralHP);

        player.spriteRenderer.color = astralColor;

        player.movement.StopMoving();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            AstralReturn();
        }
    }

    public void AstralReturn()
    {
        ReturnPlayerData();

        if (currentAstral != null) Destroy(currentAstral);

        player.movement.StopMoving();

        astralSkill.OnSkillEnd(player);

        Destroy(this);
    }
    private void SavePlayerData()
    {
        saveData = player.GetCurrentPlayerData();
    }
    private void ReturnPlayerData()
    {
        player.RestorePlayerData(saveData);
    }
}