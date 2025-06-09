using System;
using UnityEngine;
using UnityEngine.Events;

public class Pause : MonoBehaviour
{
    [SerializeField] private UnityEvent OnPauseEnable;
    [SerializeField] private UnityEvent OnPauseDisable;

    private bool IsActive;

    private void Awake()
    {
        PauseOFF();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseON();
        }
    }

    private void TogglePause()
    {
        if (IsActive == true)
        {
            PauseOFF();
        }

        else if (IsActive == false)
        {
            PauseON();
        }
    }

    public void PauseON()
    {
        Time.timeScale = 0f;
        OnPauseEnable.Invoke();
        IsActive = true;
    }

    public void PauseOFF()
    {
        Time.timeScale = 1f;
        OnPauseDisable.Invoke();
        IsActive = false;
    }

}
