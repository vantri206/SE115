using UnityEngine;
public abstract class TriggerReceiver : MonoBehaviour
{
    [Header("Settings")]
    public bool isActivated = false;

    public virtual void Trigger()
    {
        isActivated = !isActivated;

        if (isActivated)
            OnTriggerActive();
        else
            OnTriggerInactive();
    }
    protected abstract void OnTriggerActive();  
    protected abstract void OnTriggerInactive(); 
}