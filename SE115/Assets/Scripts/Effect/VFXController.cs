using UnityEngine;

public class VFXController : MonoBehaviour
{
    public Animator animator;
    [SerializeField] bool destroyAfterPlay = true;
    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        float duration = 1.0f;

        if(animator != null)
        {
            animator.Update(0.0f);

            AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
            duration = animatorState.length;
        }

        Invoke("DestroyEffect", duration + 0.1f);
    }
    public void DestroyEffect()
    {
        if (destroyAfterPlay)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
