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
    private MonsterAI monsterAI;

    [Header("Переменные для выдачи")]
    [HideInInspector] public bool TimeStop;
    [HideInInspector] public int _TimeLeft;
    [HideInInspector] public bool GameOver;
    [HideInInspector] public bool Invisible;
    [HideInInspector] public bool InfinityHealthOn;

    [Header ("Canvas")]
    [SerializeField] private GameObject passwordPanel;
    [SerializeField] private GameObject adminPanel;
    [SerializeField] private GameObject tpPanel;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] private Slider currectScoreSlider;
    public TextMeshProUGUI valueTextCurrectScore;

    [Header("Пароль от панели")]
    private string password = "230200";


    [Header ("Переключатели внутри скрипта")]
    private bool isTimeStop = false;
    private bool admin = false;
    private bool _invisible = false;
    private bool _infinityHealth = false;

    private void Start()
    {
        passwordInputField.onEndEdit.AddListener(checkPassword);

        passwordPanel.SetActive(false);
        adminPanel.gameObject.SetActive(false);
        tpPanel.gameObject.SetActive(false);

        #region Score Slider
        currectScoreSlider.wholeNumbers = true;
        currectScoreSlider.maxValue = 0;
        currectScoreSlider.maxValue = 100;
        currectScoreSlider.onValueChanged.AddListener(UpdateIntValueCurrect);
        #endregion

        #region Links Connection
        keyRebinder = FindAnyObjectByType<KeyRebinder>();
        playerController = FindAnyObjectByType<PlayerController>();
        tasks = FindAnyObjectByType<Tasks>();
        score_Timer = FindAnyObjectByType<Score_Timer>();
        monsterAI = FindAnyObjectByType<MonsterAI>();
        #endregion
    }

    #region CheatCommands

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
        GameOver = true;
    }

    #region Teleportaion
    public void tpToHome()
    {
        playerController.transform.position = Vector3.zero;
    }

    public void tpToMonster()
    {
        playerController.transform.position = monsterAI.transform.position;
    }

    public void tpMonsterToPlayer()
    {
        monsterAI.transform.position = playerController.transform.position;
    }

    #endregion

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

    #endregion

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

    private void UpdateIntValueCurrect(float value)
    {
        score_Timer.CurrectScore = (int)value; // Конвертируем float в int
        if (valueTextCurrectScore != null)
        {
            valueTextCurrectScore.text = null;
            valueTextCurrectScore.text = "Текущий счёт: " + score_Timer.CurrectScore.ToString(); // Обновляем текст
        }

    }
}
