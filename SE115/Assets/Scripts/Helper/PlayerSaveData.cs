using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public string sceneName;     

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;
    public float gravityScale;

    public SpriteRenderer sprite;

    public float currentHealth;

    public int jumpLeft;          
    public int dashLeft;         
    public int onAirAttackLeft;   
    public Vector2 facingDirection;

    public float lastOnGroundTime; 
}