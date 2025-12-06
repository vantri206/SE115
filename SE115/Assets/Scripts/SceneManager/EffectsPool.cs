using UnityEngine;

public class EffectsPool : MonoBehaviour
{
    public static Transform Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this.transform;
        DontDestroyOnLoad(gameObject);
    }
}
