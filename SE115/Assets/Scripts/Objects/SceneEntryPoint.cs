using UnityEngine;

public class SceneEntryPoint : MonoBehaviour
{
    public string entryId;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}