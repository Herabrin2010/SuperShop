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
    private int currentTimeLeft;
    public int timeLeft;
    private int scoreToWin;
    [SerializeField] private int TimeToWait;
    [HideInInspector] public int CurrectScore;

    [Header ("Тексты")]
    [SerializeField] public TextMeshPro _timeLeft;

    [Header ("Ссылки")]
    private AdminPanel adminPanel;
    private Tasks tasks;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        adminPanel = FindAnyObjectByType<AdminPanel>();
        tasks = FindAnyObjectByType<Tasks>();
        CurrectScore = 0;

    }

    private void Start()
    {
        #region Difficulty Settings
        timeLeft = DifficultyManager.Instance.CurrentDifficulty.taskTime;
        adminPanel._TimeLeft = DifficultyManager.Instance.CurrentDifficulty.taskTime;
        scoreToWin = DifficultyManager.Instance.CurrentDifficulty.scoreNeed;
        #endregion

        currentTimeLeft = timeLeft;
        StartCoroutine(GameOverTimer());

    }

    private void Update()
    {
        GameOver();
        win();
    }
    public void GameOver()
    {
        if (timeLeft == 0 || adminPanel.GameOver == true)
        {
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
        StopCoroutine(GameOverTimer());
        CurrectScore += AddScore;
    }

    public IEnumerator GameOverTimer()
    {
        timeLeft = adminPanel._TimeLeft;
        for (int i = 0; i < timeLeft; timeLeft--)
        {
            yield return new WaitForSeconds(1);
            _timeLeft.text = "";
            _timeLeft.text = "Время: " +  timeLeft.ToString();

            yield return new WaitUntil(() => adminPanel.TimeStop == false);
        }
    }

    public void Price()
    {
        CurrectScore += AddScore;
    }
    
    private void win()
    {
        if (CurrectScore == scoreToWin)
        {
            Debug.Log("Win");
        }
    }
}
