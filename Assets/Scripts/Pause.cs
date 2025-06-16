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
    private RaycastController raycastController;
    private InventoryController inventoryController;

    private void Awake()
    {
        keyRebinder = FindObjectOfType<KeyRebinder>();
        raycastController = FindObjectOfType<RaycastController>();
        inventoryController = FindObjectOfType<InventoryController>();
        PauseOFF();
    }

    private void Update()
    {
        if (keyRebinder.GetActionDown("Pause"))
        {
            if (pause == false)
            {
                PauseON();
                pause = true;
            }
            else
            {
                PauseOFF();
                pause = false; 
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
        point.gameObject.SetActive(false);
        inventoryController.inventoryFullText.gameObject.SetActive(false);
        
    }

    public void PauseOFF()
    {
        Time.timeScale = 1f;
        OnPauseDisable.Invoke();
        IsActive = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        point.gameObject.SetActive(true);
    }

}
