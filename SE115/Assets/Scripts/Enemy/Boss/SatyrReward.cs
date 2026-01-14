using UnityEngine;
using System.Collections;

public class SatyrReward : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private float destroyDelay = 1.0f;
    [SerializeField] private GameObject absorbEffectPrefab; 

    private PlayerController playerInRange;
    private bool isAbsorbed = false;

    private void Start()
    {
        this.enabled = false;
    }

    private void Update()
    {
        if (playerInRange != null && !isAbsorbed)
        {
            if (playerInRange.lastPressedInteractTime > 0.0f)
            {
                playerInRange.input.ResetInteractPressed();
                AbsorbPower();
            }
        }
    }

    private void AbsorbPower()
    {
        isAbsorbed = true;
        Debug.Log("Absorbing Boss Power...");

        if(ScrollMessenger.Instance != null)
            ScrollMessenger.Instance.ShowMessage("You absorbed Satyr Power!\n" +
                "Upgrade Jump to Double Jump.\n" +
                "Now your dash can resist damage and deal damage to enemy");

        ReceiveReward();

        StartCoroutine(DestroyCorpseRoutine());
    }

    private void ReceiveReward()
    {
        PlayerController player = playerInRange;
        player.unlockSlashDash = true;
        player.data.jumpCountAmount = 2;
    }

    IEnumerator DestroyCorpseRoutine()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }

    public void SetPlayerInRange(PlayerController player)
    {
        playerInRange = player;
    }
}