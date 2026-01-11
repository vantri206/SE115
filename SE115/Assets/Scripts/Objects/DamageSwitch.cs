using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DamageSwitch : MonoBehaviour, IDamageable
{
    [Header("Connections")]
    [SerializeField] private List<TriggerReceiver> triggerReceivers;

    [Header("Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool isOneShot = false;

    private bool isTrigger = false;

    public DamageTeam team { get; set; } = DamageTeam.Neutral;

    public bool isDead => isOneShot && isTrigger;

    private void Start()
    {
        animator.SetBool("isTrigger", isTrigger);
    }
    public void TakeDamage(float damageAmount, Vector2 sourcePos)
    {
        if (isOneShot && isTrigger) return;

        ActivateSwitch();
    }
    private void ActivateSwitch()
    {
        if (isOneShot) isTrigger = true;
        else isTrigger = !isTrigger;

        animator.SetBool("isTrigger", isTrigger);
    }
    private void ActiveTrigger()
    {
        foreach (var receiver in triggerReceivers)
        {
            if (receiver != null)
            {
                receiver.Trigger();
            }
        }
    }
    public void AE_ActiveTrigger() { ActiveTrigger(); }
}