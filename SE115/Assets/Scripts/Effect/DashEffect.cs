using UnityEngine;

public class DashEffect : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;

    private void Awake()
    {
        if(trailRenderer == null)
            trailRenderer = GetComponent<TrailRenderer>();
    }

    public void StartEffect()
    {
        trailRenderer.emitting = true;
    }
    public void FinishEffect()
    {
        trailRenderer.emitting = false;
    }

}