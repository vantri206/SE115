using System.Collections.Generic;
using UnityEngine;
public class TriggerSensor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private List<TriggerReceiver> linkedTraps;
    [SerializeField] private bool triggerOnExit = false;
    [SerializeField] private bool isOneShot = false;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOneShot && hasTriggered) return;
        if ((playerLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            ActivateTraps();

            if (isOneShot) hasTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (triggerOnExit && !isOneShot)
        {
            if ((playerLayer.value & (1 << other.gameObject.layer)) > 0)
            {
                ActivateTraps();
            }
        }
    }
    private void ActivateTraps()
    {
        foreach (var trap in linkedTraps)
        {
            if (trap != null)
            {
                trap.Trigger();
            }
        }
    }
}