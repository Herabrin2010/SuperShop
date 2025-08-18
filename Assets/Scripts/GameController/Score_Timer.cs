using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score_Timer : MonoBehaviour
{

    [SerializeField] private GameObject GameOverMenu;

    [Header ("Настройки")]
    public int AddScore = 1;
    private int originalTimeLeft;
    public int TimeLeft;
    private int scoreToWin;
    [SerializeField] private int timeToWait;
    [HideInInspector] public int CurrectScore;

    [Header ("Тексты")]
    [SerializeField] public TextMeshPro _timeLeft;

    [Header ("Ссылки")]
    private AdminPanel adminPanel;
    private CutsceneManager cutsceneManager;
    private Tasks tasks;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        adminPanel = FindAnyObjectByType<AdminPanel>();
        cutsceneManager = FindAnyObjectByType<CutsceneManager>();
        tasks = FindAnyObjectByType<Tasks>();
        CurrectScore = 0;

    }

    private void Start()
    {
        #region Difficulty Settings
        TimeLeft = DifficultyManager.Instance.CurrentDifficulty.taskTime;
        adminPanel._TimeLeft = DifficultyManager.Instance.CurrentDifficulty.taskTime;
        scoreToWin = DifficultyManager.Instance.CurrentDifficulty.scoreNeed;
        #endregion

        originalTimeLeft = TimeLeft;
        StartCoroutine(GameOverTimer());

    }

    private void Update()
    {
        GameOver();
        win();
    }
    public void GameOver()
    {
        if (TimeLeft == 0 || adminPanel.GameOver == true)
        {
            AchievementManager.Instance.UnlockAchievement("DieFirstTime");
            AchievementManager.Instance.AddProgress("Die10Times");
            AchievementManager.Instance.AddProgress("Die100Times");

            _timeLeft.text = "Времени осталось: 0";
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            playerController.CameraLock = true;

            StopAllCoroutines();
            GameOverMenu.SetActive(true);
        }
    }

    public void TaskComplete()
    {
        AchievementManager.Instance.UnlockAchievement("SellFirstItem");
        AchievementManager.Instance.AddProgress("Sell10Items");
        AchievementManager.Instance.AddProgress("Sell100Items");

        StopCoroutine(GameOverTimer());
        CurrectScore += AddScore;
    }

    public IEnumerator GameOverTimer()
    {
        TimeLeft = adminPanel._TimeLeft;
        TimeLeft = originalTimeLeft;
        for (int i = 0; i < TimeLeft; TimeLeft--)
        {
            yield return new WaitForSeconds(1);
            _timeLeft.text = "";
            _timeLeft.text = "Время: " + TimeLeft.ToString();

            yield return new WaitUntil(() => adminPanel.TimeStop == false);

            if (TimeLeft == originalTimeLeft - 10)
            {
                AchievementManager.Instance.UnlockAchievement("GiveItemIn10Seconds");
            }
        }
    }
    
    private void win()
    {
        if (CurrectScore == scoreToWin)
        {
            AchievementManager.Instance.UnlockAchievement("End");
            cutsceneManager.PlayCutscene(7);

            if (DifficultyManager.Instance.CurrentDifficulty.name == "Impossible")
            {
                AchievementManager.Instance.UnlockAchievement("WinGameInInpossible");
            }
        }
    }
}
