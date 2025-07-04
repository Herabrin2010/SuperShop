using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Pause : MonoBehaviour
{
    [Header ("Bools")]
    private bool pause = false;
    public bool IsActive;
    public bool IsSettings = false;
    public bool IsPause = false;

    private KeyRebinder keyRebinder;

    [SerializeField] private GameObject point;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject SettingsPanel;


    [Header ("—сылки")]
    private RaycastController raycastController;
    private InventoryController inventoryController;
    private PlayerController playerController;
    private Animator pauseAnimator;
    private Animator settingsAnimator;

    private void Awake()
    {
        pauseAnimator = PausePanel.GetComponent<Animator>();
        settingsAnimator = SettingsPanel.GetComponent<Animator>();
        keyRebinder = FindAnyObjectByType<KeyRebinder>();
        raycastController = FindAnyObjectByType<RaycastController>();
        inventoryController = FindAnyObjectByType<InventoryController>();
        playerController = FindAnyObjectByType<PlayerController>();
        ClosePause();
    }

    private void Update()
    {
        updateAnimation();
        if (keyRebinder.GetActionDown("Pause"))
        {
            if (pause == false)
            {
                OpenPause();
                pause = true;
            }
            else
            {
                if (SettingsPanel.activeSelf)
                {
                    SettingsPanel.SetActive(false);
                    OpenPause();
                }
                else
                {
                    ClosePause();
                    pause = false;
                }
            }
        }
    }

    public void OpenPause()
    {
        Time.timeScale = 0f;
        PausePanel.SetActive(true);
        IsActive = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerController.CameraLock = true;

        point.gameObject.SetActive(false);
        inventoryController.inventoryFullText.gameObject.SetActive(false);
        raycastController.help.gameObject.SetActive(false);
        inventoryController.inventory.gameObject.SetActive(false);
    }

    public void ClosePause()
    {
        Time.timeScale = 1f;
        PausePanel.gameObject.SetActive(false);
        IsActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerController.CameraLock = false;

        point.gameObject.SetActive(true);
        inventoryController.inventory.gameObject.SetActive(true);
    }

    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }
    public void CloseSettings() 
    { 
        SettingsPanel.gameObject.SetActive(false);
    }

    private void updateAnimation() 
    {
        pauseAnimator.SetBool("Pause", IsPause);
        settingsAnimator.SetBool("Settings", IsSettings);
    }
}
