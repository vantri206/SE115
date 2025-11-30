using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;

    [Header("Sword")]
    public PlayerSword sword;

    [Header("Combo Settings")]
    public int comboCount = 0;
    private float comboTimer = 0;
    public float comboResetTime = 1.5f;

    private void Awake()
    {
        if (player == null)
            player = GetComponent<PlayerController>();

        if (sword != null) 
            sword.gameObject.SetActive(false);
    }

    private void Update()
    {
        comboTimer += Time.deltaTime;
        if (comboTimer > comboResetTime)
        {
            comboCount = 0;
        }
    }

    public void SwordAttack()
    {
        player.isAttacking = true;
        comboTimer = float.NegativeInfinity;

        EnableSword();
    }
    public void FinishAttack()
    {
        player.isAttacking = false;
        DisableSword();

        comboTimer = 0;

        comboCount++;
        if (comboCount >= 4) comboCount = 0;
    }

    public void EnableSword() { sword.gameObject.SetActive(true); }
    public void DisableSword() { sword.gameObject.SetActive(false); }
}