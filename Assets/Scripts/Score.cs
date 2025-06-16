using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int currectScore;
    private int recordScore;
    [SerializeField] private int timeLeft;
    [SerializeField] private TextMeshProUGUI _timeLeft;
    [SerializeField] private GameObject GameOverMenu;
    [SerializeField] private TextMeshProUGUI _currentScore;
    [SerializeField] private TextMeshProUGUI _recordScore;
    [SerializeField] public int addScore;

    private Tasks tasks;
    private void Awake()
    {
        tasks = FindObjectOfType<Tasks>();
        currectScore = 0;
    }

    private void Start()
    {
        StartCoroutine(GameOverTimer());
    }

    private void Update()
    {
        timer();
    }

    private void timer()
    {
        if (tasks.IsTaskComplete == true)
        {
            StopAllCoroutines();
            currectScore += addScore;
            GameOverTimer();


        }

        if (timeLeft == 0)
        {
            _timeLeft.text = "Время: 0";
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            StopAllCoroutines();
            GameOverMenu.SetActive(true);
        }



    }

    private IEnumerator GameOverTimer()
    {
        for (int i = 0; i < timeLeft; timeLeft--)
        {
            yield return new WaitForSeconds(1);
            _timeLeft.text = null;
            _timeLeft.text = "Время: " +  timeLeft.ToString();
            Debug.Log(timeLeft.ToString());
        }

    }
}
