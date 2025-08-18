using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchievementNotification : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float showDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Start()
    {
        StartCoroutine(ShowNotification());
    }

    private IEnumerator ShowNotification()
    {
        // Анимация появления
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            yield return null;
        }

        // Ожидание
        yield return new WaitForSecondsRealtime(showDuration);

        // Анимация исчезновения
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            yield return null;
        }

        Destroy(gameObject);
    }

    public void Initialize(AchievementManager.Achievement achievement)
    {
        icon.sprite = achievement.icon;
        titleText.text = achievement.title;
    }
}