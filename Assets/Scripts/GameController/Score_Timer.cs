using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score_Timer : MonoBehaviour
{

    [SerializeField] private GameObject GameOverMenu;

    [Header ("Настройки")]
    public int AddScore;
    private int currentTimeLeft;
    public int timeLeft;
    [SerializeField] private int TimeToWait;
    [HideInInspector] public int CurrectScore;

    [Header ("Sliders")]
    [SerializeField] private Slider currectScoreSlider;

    public TextMeshProUGUI valueTextCurrectScore;

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
        StartCoroutine(GameOverTimer());

        currectScoreSlider.wholeNumbers = true;
        currectScoreSlider.maxValue = 0;
        currectScoreSlider.maxValue = 100;
        currectScoreSlider.onValueChanged.AddListener(UpdateIntValueCurrect);


        currentTimeLeft = timeLeft;
    }

    private void Update()
    {
        GameOver();
        win();
    }
    private void UpdateIntValueCurrect(float value)
    {
        CurrectScore = (int)value; // Конвертируем float в int
        if (valueTextCurrectScore != null)
        {
            valueTextCurrectScore.text = null;
            valueTextCurrectScore.text = "Текущий счёт: " + CurrectScore.ToString(); // Обновляем текст
        }
            
    }

    public void GameOver()
    {
        if (timeLeft == 0 || adminPanel.TimerLose == true)
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

    public void IfWin()
    {
        StopCoroutine(GameOverTimer());
        CurrectScore += AddScore;
    }

    public IEnumerator GameOverTimer()
    {
        adminPanel._TimeLeft = timeLeft;
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
        if (CurrectScore == 100)
        {
            Debug.Log("Win");
        }
    }
}
