using System.Collections;
using UnityEngine;

public class TriggerSpikeTrap : TriggerReceiver
{
    [Header("Trap Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private SwordDamage swordDamage;
    [SerializeField] private float warningTime = 0.2f;
    [SerializeField] private float activeDuration = 1.0f;
    [SerializeField] private float cooldownTime = 0.5f;

    private bool isTrigger = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (swordDamage == null)
            swordDamage = GetComponent<SwordDamage>();
        if (hitbox == null)
            hitbox = GetComponentInChildren<Hitbox>();

        if (hitbox != null) hitbox.gameObject.SetActive(false); 
    }
    protected override void OnTriggerActive()
    {
        if (isTrigger) return;

        StartCoroutine(TriggerSpikeRoutine());
    }
    private IEnumerator TriggerSpikeRoutine()
    {
        isTrigger = true;
        if (animator != null) animator.SetBool("isTrigger", true);
        if (animator != null) animator.SetTrigger("Warning");

        yield return new WaitForSeconds(warningTime);

        if (swordDamage != null) swordDamage.ResetHitList();
        if (hitbox != null) hitbox.gameObject.SetActive(true);

        yield return new WaitForSeconds(activeDuration);

        if (animator != null) animator.SetBool("isTrigger", false);
        if (hitbox != null) hitbox.gameObject.SetActive(false);

        yield return new WaitForSeconds(cooldownTime);

        isTrigger = false;
    }
    protected override void OnTriggerInactive() { }
}