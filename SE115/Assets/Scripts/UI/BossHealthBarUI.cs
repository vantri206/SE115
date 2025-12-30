using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image redFill;  
    [SerializeField] private Image ghostFill;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float ghostDelay = 0.5f;    
    [SerializeField] private float ghostDuration = 0.5f;
    [SerializeField] private Ease ghostEase = Ease.OutQuad;

    private BossEnemyHealth currentBossHealth;

    private Tween ghostTween;
    private Tween redTween;

    private void OnDisable()
    {
        if(currentBossHealth != null)
        {
            currentBossHealth.onHealthChanged -= UpdateHealthUI;
        }
    }
    public void Initialize(BossEnemyHealth bossHealth)
    {
        currentBossHealth = bossHealth;

        redFill.fillAmount = 1.0f;
        ghostFill.fillAmount = 1.0f;

        currentBossHealth.onHealthChanged += UpdateHealthUI;

        gameObject.SetActive(true);

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1.0f, 0.5f);
    }
    public void Hide()
    {
        if (currentBossHealth != null)
        {
            currentBossHealth.onHealthChanged -= UpdateHealthUI;
            currentBossHealth = null;
        }
        canvasGroup.DOFade(0.0f, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }
    private void UpdateHealthUI(float oldHealth, float newHealth, float maxHealth)
    {
        float targetFill = newHealth / maxHealth;

        if (ghostTween != null && ghostTween.IsActive()) 
            ghostTween.Kill();
        if (redTween != null && redTween.IsActive()) 
            redTween.Kill();

        if (newHealth > oldHealth)
        {
            ghostFill.fillAmount = targetFill;

            redTween = redFill.DOFillAmount(targetFill, ghostDuration).SetEase(ghostEase);
        }
        else
        {
            redFill.fillAmount = targetFill;

            ghostTween = ghostFill.DOFillAmount(targetFill, ghostDuration)
                                  .SetDelay(ghostDelay)
                                  .SetEase(ghostEase);
        }
    }
}