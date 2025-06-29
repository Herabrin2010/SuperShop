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
    [HideInInspector] public bool Invisible;
    [HideInInspector] public bool InfinityHealthOn;

    [Header ("Canvas")]
    [SerializeField] private GameObject passwordPanel;
    [SerializeField] private GameObject adminPanel;
    [SerializeField] private GameObject tpPanel;
    [SerializeField] private TMP_InputField passwordInputField;

    [Header("Пароль от панели")]
    private string password = "230200";


    [Header ("Переключатели внутри скрипта")]
    private bool isTimeStop = false;
    private bool admin = false;
    private bool _invisible = false;
    private bool _infinityHealth = false;

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

    public void tpToHome()
    {
        playerController.transform.position = Vector3.zero;
    }

    public void InfinityHealth()
    {
        if (_infinityHealth == false)
        {
            _infinityHealth = true;
            InfinityHealthOn = true;
        }

        else
        {
            _infinityHealth = false;
            _infinityHealth= false;
        }
    }

    public void Regenerate()
    {
        playerController.PlayerHealth = playerController.MaxPlayerHealth;
    }

    public void invisible()
    {
        if (_invisible == false)
        {
            Invisible = true;
            _invisible = true;
        }
        else 
        {
            Invisible = false;
            _invisible = false;
        }
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

            else 
            {
                if (tpPanel.activeSelf) 
                {
                    tpPanel.gameObject.SetActive(false);
                    adminPanel.gameObject.SetActive(true);
                    admin = true;
                }

                else
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
}
