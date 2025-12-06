using UnityEngine;

public class ProjectilesPool : MonoBehaviour
{
    public static Transform Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this.transform;
        DontDestroyOnLoad(gameObject);
    }
}
