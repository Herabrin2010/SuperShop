using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class AdminPanel : MonoBehaviour
{
    [Header("Ссылки")]
    private KeyRebinder keyRebinder;
    private PlayerController playerController;
    private Tasks tasks;
    private Score_Timer score_Timer;

    [Header("Переменные для выдачи")]
    [HideInInspector] public bool TimeStop;
    [HideInInspector] public int _TimeLeft;
    [HideInInspector] public bool TimerLose;

    [Header ("Canvas")]
    [SerializeField] private GameObject passwordPanel;
    [SerializeField] private GameObject adminPanel;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField addscoreInputField;
    [SerializeField] private TMP_InputField timeLeftInputField;

    [Header("Пароль от панели")]
    private string password = "230200";

    private bool isTimeStop = false;

    private bool admin = false;
    public void taskComplete()
    {
        tasks.CompleteTask();
    }

    public void timeStop()
    {
        if (isTimeStop == false)
        {
            TimeStop = true; 
            isTimeStop = true;
        }

        else if (isTimeStop == true) 
        {
            TimeStop = false;
            isTimeStop= false;
        }
    }

    public void timerLose() 
    {
        TimerLose = true;
    }

    private void Awake()
    {
        passwordPanel.SetActive(false);
        adminPanel.gameObject.SetActive(false);
        keyRebinder = FindAnyObjectByType<KeyRebinder>();
        playerController = FindAnyObjectByType<PlayerController>();
        tasks = FindAnyObjectByType<Tasks>();
        score_Timer = FindAnyObjectByType<Score_Timer>();
    }
    private void Start()
    {
        passwordInputField.onEndEdit.AddListener(checkPassword);
        addscoreInputField.onEndEdit.AddListener(addScore);
        timeLeftInputField.onEndEdit.AddListener(timeLeft);
    }

    private void Update()
    {
        if (keyRebinder.GetActionDown("Admin"))
        {
            if (admin == false)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                playerController.CameraLock = true;

                passwordPanel.gameObject.SetActive(true);
                admin = true;

                playerController.MovementLock = true;
            }

            else if (admin == true && adminPanel == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                playerController.CameraLock = false;
                
                adminPanel.gameObject.SetActive(false);
                passwordPanel.gameObject.SetActive(false);
                admin = false;

                playerController.MovementLock = false;
            }
        }

    }

    private void checkPassword(string inputText)
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            if (inputText == password)
            {
                Debug.Log("Пароль верный! Доступ разрешён.");
                adminPanel.gameObject.SetActive(true);
                passwordPanel.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Неверный пароль!");
                passwordInputField.text = passwordInputField.text; 
            }
        }
    }

    private void addScore(string inputText)
    {
        inputText = score_Timer.AddScore.ToString();
    }

    private void timeLeft(string inputText)
    {
        inputText = _TimeLeft.ToString();
    }
}
