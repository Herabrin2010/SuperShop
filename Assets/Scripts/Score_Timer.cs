using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score_Timer : MonoBehaviour
{

    [SerializeField] private int timeLeft;
    private int currentTimeLeft;
    [SerializeField] private GameObject GameOverMenu;
    public int AddScore;
    public int CurrectScore;
    public int RecordScore;
    [SerializeField] private int TimeToWait;

    [Header ("Sliders")]
    [SerializeField] private Slider currectScoreSlider;
    [SerializeField] private Slider recordScoreSlider;

    [SerializeField] private TextMeshProUGUI valueTextCurrectScore;
    [SerializeField] private TextMeshProUGUI valueTextRecordScore;


    [Header ("������")]
    [SerializeField] public TextMeshProUGUI _timeLeft;
    [SerializeField] private TextMeshProUGUI _currentScore;
    [SerializeField] private TextMeshProUGUI _recordScore;


    [Header ("������")]
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

        recordScoreSlider.wholeNumbers = true;
        recordScoreSlider.minValue = 0;
        recordScoreSlider.maxValue = 100;
        recordScoreSlider.onValueChanged.AddListener(UpdateIntValueRecord);

        currentTimeLeft = timeLeft;
        playerPrefsLoad();
    }

    private void Update()
    {
        timerLose();
        win();

        if (RecordScore < CurrectScore)
        {
            RecordScore = CurrectScore;
        }
    }
    private void UpdateIntValueCurrect(float value)
    {
        CurrectScore = (int)value; // ������������ float � int
        if (valueTextCurrectScore != null)
        {
            valueTextCurrectScore.text = null;
            valueTextCurrectScore.text = "������� ����: " + CurrectScore.ToString(); // ��������� �����
        }
            
    }

    private void UpdateIntValueRecord(float value)
    {
        RecordScore = (int)value; // ������������ float � int
        if (valueTextRecordScore != null)
        {
            valueTextRecordScore.text = null;
            valueTextRecordScore.text = "��������� ����: " + RecordScore.ToString(); // ��������� �����
        }
    }
    private void timerLose()
    {
        if (timeLeft == 0)
        {
            _timeLeft.text = "�����: 0";
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            playerController.CameraLock = true;

            StopAllCoroutines();
            GameOverMenu.SetActive(true);

            playerPrefsSave(value: RecordScore);
        }
    }

    public void IfWin()
    {
        StopCoroutine(GameOverTimer());
        CurrectScore += AddScore;
        StartCoroutine(WaitingAfterWin());
    }

    private IEnumerator GameOverTimer()
    {
        adminPanel._TimeLeft = timeLeft;
        for (int i = 0; i < timeLeft; timeLeft--)
        {
            yield return new WaitForSeconds(1);
            _timeLeft.text = "";
            _timeLeft.text = "�����: " +  timeLeft.ToString();

            yield return new WaitUntil(() => adminPanel.TimeStop == false);
        }
    }

    private IEnumerator WaitingAfterWin() 
    {
        for (int i = 0;i < TimeToWait; TimeToWait--) 
        {
            yield return new WaitForSeconds(1);
            tasks.taskText.text = "������� ���������";
        }

        if (TimeToWait == 0)
        {
            StopCoroutine(WaitingAfterWin());
            timeLeft = currentTimeLeft;
        }
    }

    public void Price()
    {
        CurrectScore += AddScore;
    }

    private void playerPrefsSave(float value) 
    {
        RecordScore = (int)value;
        PlayerPrefs.SetInt("RecordScore", RecordScore);
    }

    private void playerPrefsLoad()
    {
        RecordScore = PlayerPrefs.GetInt("RecordScore");
    }
    private void win()
    {
        if (CurrectScore == 100)
        {
            Debug.Log("Win");
        }
    }
}
