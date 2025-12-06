using UnityEngine;

public class SpearLauncher : MonoBehaviour
{
    [Header("Refrences")]

    public Animator animator;
    [Header("Setting")]

    public Transform spearShooter;
    public GameObject spearTrapPrefab;

    [SerializeField] private Vector2 shotDirection = Vector2.left;
    [SerializeField] private float shotCooldown = 5.0f;

    private float shotTimer = 0.0f;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    void Update()
    {
        shotTimer += Time.deltaTime;
        if(shotTimer >= shotCooldown)
        {
            animator.SetTrigger("Shot");
            shotTimer = float.NegativeInfinity;
        }
    }
    public void SpearShot()
    {
        GameObject spear = Instantiate(spearTrapPrefab, spearShooter.position, 
                            spearShooter.localRotation, ProjectilesPool.Instance);
        SpearTrap spearTrap = spear.GetComponent<SpearTrap>();
        if(spearTrap)
        {
            spearTrap.SetDirection(this.shotDirection);
        }
        shotTimer = 0.0f;
    }
}
