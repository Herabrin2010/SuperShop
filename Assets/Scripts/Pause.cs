using System;
using UnityEngine;
using UnityEngine.Events;

public class Pause : MonoBehaviour
{
    [SerializeField] private UnityEvent OnPauseEnable;
    [SerializeField] private UnityEvent OnPauseDisable;
    private bool pause = false;
    public bool IsActive;

    private KeyRebinder keyRebinder;

    [SerializeField] private GameObject point;
    [SerializeField] private GameObject SettingsPanel;


    [Header ("Ссылки")]
    private RaycastController raycastController;
    private InventoryController inventoryController;
    private PlayerController playerController;

    private void Awake()
    {
        keyRebinder = FindAnyObjectByType<KeyRebinder>();
        raycastController = FindAnyObjectByType<RaycastController>();
        inventoryController = FindAnyObjectByType<InventoryController>();
        playerController = FindAnyObjectByType<PlayerController>();
        PauseOFF();
    }

    private void Update()
    {
        if (keyRebinder.GetActionDown("Pause"))
        {
            if (pause == false)
            {
                // Если игра не на паузе - включаем паузу
                PauseON();
                pause = true;
            }
            else
            {
                if (SettingsPanel.activeSelf)
                {
                    SettingsPanel.SetActive(false);
                    PauseON();
                }
                else
                {
                    PauseOFF();
                    pause = false;
                }
            }
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerController.CameraLock = true;

        point.gameObject.SetActive(false);
        inventoryController.inventoryFullText.gameObject.SetActive(false);
        raycastController.help.gameObject.SetActive(false);
        inventoryController.inventory.gameObject.SetActive(false);

    }

    public void PauseOFF()
    {
        Time.timeScale = 1f;
        OnPauseDisable.Invoke();
        IsActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerController.CameraLock = false;

        point.gameObject.SetActive(true);
        inventoryController.inventory.gameObject.SetActive(true);
    }

}
