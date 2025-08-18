using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class AchievementUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject _achievementPanel;
    [SerializeField] private Transform _contentParent;
    [SerializeField] private GameObject _achievementPrefab;
    [SerializeField] private Button _toggleHideCompletedButton;
    [SerializeField] private TextMeshProUGUI _counterText;
    [SerializeField] private AudioSource _audioSource;

    [Header("Notification")]
    [SerializeField] private GameObject _notificationPrefab;
    [SerializeField] private Transform _notificationParent;
    [SerializeField] private float _notificationDuration = 3f;

    private List<AchievementManager.Achievement> _allAchievements;
    private bool _hideCompleted = false;

    private void Start()
    {
        _allAchievements = AchievementManager.Instance.GetAllAchievements();
        _toggleHideCompletedButton.onClick.AddListener(ToggleHideCompleted);

        // Правильная подписка на событие
        AchievementManager.Instance.onAchievementUnlocked.AddListener(OnAchievementUnlocked);

        RefreshAchievementsList();
        UpdateCounter();
    }

    private void OnDestroy()
    {
        // Не забываем отписаться при уничтожении объекта
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.onAchievementUnlocked.RemoveListener(OnAchievementUnlocked);
        }
    }

    private void ToggleHideCompleted()
    {
        _hideCompleted = !_hideCompleted;
        RefreshAchievementsList();
    }

    private void RefreshAchievementsList()
    {
        // Очистка старых элементов
        foreach (Transform child in _contentParent)
        {
            Destroy(child.gameObject);
        }

        // Фильтрация
        var filtered = _allAchievements
            .Where(a => !(_hideCompleted && a.isUnlocked))
            .OrderByDescending(a => a.isUnlocked)
            .ThenBy(a => a.id);

        // Создание новых элементов
        foreach (var achievement in filtered)
        {
            var newItem = Instantiate(_achievementPrefab, _contentParent).GetComponent<AchievementUIItem>();
            newItem.Setup(achievement);
        }
    }

    private void OnAchievementUnlocked(AchievementManager.Achievement achievement)
    {
        // Создание и настройка уведомления
        var notification = Instantiate(_notificationPrefab, _notificationParent);
        var notificationUI = notification.GetComponent<AchievementNotification>();
        notificationUI.Initialize(achievement);

        // Обновление интерфейса
        RefreshAchievementsList();
        UpdateCounter();

        // Воспроизведение звука
        if (_audioSource != null && AchievementManager.Instance.unlockSound != null)
        {
            _audioSource.ignoreListenerPause = true;
            _audioSource.PlayOneShot(AchievementManager.Instance.unlockSound);
        }
    }

    private void UpdateCounter()
    {
        int unlocked = _allAchievements.Count(a => a.isUnlocked);
        _counterText.text = $"{unlocked}/{_allAchievements.Count}";
    }

    public void TogleUI() => _achievementPanel.SetActive(!_achievementPanel.activeSelf);
}