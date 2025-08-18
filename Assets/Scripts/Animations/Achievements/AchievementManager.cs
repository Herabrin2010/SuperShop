using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class AchievementManager : MonoBehaviour
{
    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public Sprite icon;
        public bool isHidden;
        public bool isUnlocked;
        public int rewardPoints = 0;

        [Header("Progress")]
        public bool isProgressive = false;
        public int currentProgress = 0;
        public int targetProgress = 1;

        [Header("Visuals")]
        public Color unlockedColor = new Color(0.5f, 1f, 0.5f);
        public Color lockedColor = new Color(0.8f, 0.8f, 0.8f);

        [System.NonSerialized] public UnityEvent<Achievement> OnUnlocked = new UnityEvent<Achievement>();

        public void Unlock()
        {
            if (isUnlocked) return;

            isUnlocked = true;
            currentProgress = targetProgress;
            OnUnlocked.Invoke(this);
        }

        public void AddProgress(int amount = 1)
        {
            if (isUnlocked || !isProgressive) return;

            currentProgress = Mathf.Clamp(currentProgress + amount, 0, targetProgress);
            if (currentProgress >= targetProgress)
            {
                Unlock();
            }
        }

        public float GetProgress() => isProgressive ? (float)currentProgress / targetProgress : 0f;
    }

    public static AchievementManager Instance { get; private set; }

    // В AchievementManager вместо Achievement
    public bool IsAchievementUnlocked(string achievementId)
    {
        Achievement achievement = GetAchievement(achievementId);
        return achievement != null && achievement.isUnlocked;
    }

    [SerializeField] private List<Achievement> _achievements = new List<Achievement>();
    public AudioClip unlockSound;

    public UnityEvent<Achievement> onAchievementUnlocked = new UnityEvent<Achievement>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAchievements();
    }

    public void UnlockAchievement(string id)
    {
        Achievement achievement = GetAchievement(id);
        if (achievement != null && !achievement.isUnlocked)
        {
            achievement.Unlock();
            onAchievementUnlocked.Invoke(achievement);
            SaveAchievements();
        }
    }

    public void AddProgress(string id, int amount = 1)
    {
        Achievement achievement = GetAchievement(id);
        if (achievement != null)
        {
            achievement.AddProgress(amount);
            SaveAchievements();
        }
    }

    public Achievement GetAchievement(string id) => _achievements.Find(a => a.id == id);
    public List<Achievement> GetAllAchievements() => _achievements;

    private void SaveAchievements()
    {
        foreach (var achievement in _achievements)
        {
            PlayerPrefs.SetInt($"ACH_{achievement.id}_UNLOCKED", achievement.isUnlocked ? 1 : 0);
            PlayerPrefs.SetInt($"ACH_{achievement.id}_PROGRESS", achievement.currentProgress);
        }
    }

    private void LoadAchievements()
    {
        foreach (var achievement in _achievements)
        {
            achievement.OnUnlocked = new UnityEvent<Achievement>(); // Переинициализация
            achievement.isUnlocked = PlayerPrefs.GetInt($"ACH_{achievement.id}_UNLOCKED", 0) == 1;
            achievement.currentProgress = PlayerPrefs.GetInt($"ACH_{achievement.id}_PROGRESS", 0);

            if (achievement.isProgressive && achievement.currentProgress >= achievement.targetProgress)
            {
                achievement.Unlock();
            }
        }
    }
}