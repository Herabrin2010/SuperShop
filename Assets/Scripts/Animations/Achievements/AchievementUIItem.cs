using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchievementUIItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private CanvasGroup canvasGroup;

    private float animationProgress;
    private bool isAnimating;

    private void Update()
    {
        if (isAnimating)
        {
            animationProgress += Time.unscaledDeltaTime * 2f; // Скорость анимации
            canvasGroup.alpha = Mathf.Clamp01(animationProgress);

            if (animationProgress >= 1f)
            {
                isAnimating = false;
            }
        }
    }

    public void Setup(AchievementManager.Achievement achievement)
    {
        icon.sprite = achievement.icon;
        title.text = achievement.title;
        description.text = achievement.description;
        lockedOverlay.SetActive(!achievement.isUnlocked);

        // Настройка аниматора
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;

            if (achievement.isUnlocked)
            {
                animator.SetTrigger("Unlock");
                StartCoroutine(PlayScaleAnimation());
            }
        }

        // Запуск анимации появления
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            isAnimating = true;
            animationProgress = 0f;
        }
    }

    private IEnumerator PlayScaleAnimation()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * 1.1f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, startScale, t);
            yield return null;
        }
    }
}