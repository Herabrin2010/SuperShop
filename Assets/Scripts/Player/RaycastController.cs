using UnityEngine;
using TMPro;

public class RaycastController : MonoBehaviour
{
    [Header("Настройки")]
    public Camera playerCamera;
    public float interactionDistance = 3f;
    public TextMeshProUGUI help;

    [SerializeField] private bool isStudying = false;

    [Header("Ссылки")]
    private InventoryController inventoryController;
    private PlayerController playerController;
    private RaycastController raycastController;
    private CutsceneManager cutsceneManager;
    private KeyRebinder keyRebinder;
    private KeyBindingsData keyBindingData;
    private LaptopControll laptopControll;
    private Tasks tasks;
    private Generation generation;
    private OpenDoor _openDoor;

    private GameObject currentTarget;

    private GameObject lastHitDoor;
    private int lastDoorIndex = -1;

    private bool isLaptopOpen = false;

    private void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (help != null) help.gameObject.SetActive(false);

        #region Links
        inventoryController = FindAnyObjectByType<InventoryController>();
        playerController = FindAnyObjectByType<PlayerController>();
        raycastController = FindAnyObjectByType<RaycastController>();
        cutsceneManager = FindAnyObjectByType<CutsceneManager>();
        keyRebinder = FindAnyObjectByType<KeyRebinder>();
        keyBindingData = FindAnyObjectByType<KeyBindingsData>();
        laptopControll = FindAnyObjectByType<LaptopControll>();
        tasks = FindAnyObjectByType<Tasks>();
        generation = FindAnyObjectByType<Generation>();
        _openDoor = FindAnyObjectByType<OpenDoor>();
        #endregion

        if (inventoryController == null) Debug.LogError("InventoryController not found!");
        if (keyRebinder == null) Debug.LogError("KeyRebinder not found!");
        if (tasks == null) Debug.LogError("Tasks component not found!");
    }

    private void Update()
    {
        PerformRaycast();
        CheckForInteraction();
    }

    private string getInteractionKey()
    {
        if (keyBindingData != null)
        {
            foreach (var bind in keyBindingData.keyBinds)
            {
                if (bind.actionName == "Interaction")
                {
                    return bind.keyCode.ToString();
                }
            }
        }
        return "E"; // Значение по умолчанию, если не найдено
    }

    private void PerformRaycast()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out var hit, interactionDistance))
        {
            lastHitDoor = hit.collider.transform.parent.gameObject;
            lastDoorIndex = generation.generatedDoors.IndexOf(lastHitDoor);

            HandleNewTarget(hit.collider.gameObject);
        }
        else
        {
            ClearCurrentTarget();
        }
    }

    private void HandleNewTarget(GameObject newTarget)
    {
        if (newTarget == currentTarget) return;
        ClearCurrentTarget();
        currentTarget = newTarget;

        switch (currentTarget.tag)
        {
            case "Obj":
                ShowInteractionPrompt($"Нажмите {getInteractionKey()} чтобы взять {currentTarget.name}");
                break;
            case "Trashcan":
                ShowInteractionPrompt($"Нажмите {getInteractionKey()} чтобы очистить инвентарь");
                break;
            case "Door":
                ShowInteractionPrompt($"Нажмите {getInteractionKey()} чтобы открыть дверь");
                break;
            case "Laptop":
                ShowInteractionPrompt($"Нажмите {getInteractionKey()} чтобы открыть ноутбук");
                break;
        }
    }

    private void ShowInteractionPrompt(string message)
    {
        if (help != null)
        {
            help.text = message;
            help.gameObject.SetActive(true);
        }
    }

    private void ClearCurrentTarget()
    {
        if (currentTarget != null && help != null)
        {
            help.gameObject.SetActive(false);
        }
        currentTarget = null;
    }

    private void CheckForInteraction()
    {
        if (keyRebinder != null && keyRebinder.GetActionDown("Interaction") && currentTarget != null)
        {
            ProcessInteraction();
        }
    }

    private void ProcessInteraction()
    {
        if (currentTarget == null) return;

        switch (currentTarget.tag)
        {
            case "Obj":
                PickUpItem();
                break;
            case "Trashcan":
                ClearInventory();
                break;
            case "Door":
                openDoor();
                break;
            case "Laptop":
                if (!isLaptopOpen) { openLaptop(); isLaptopOpen = true; }
                else { closeLaptop(); isLaptopOpen = false; }
                break;

            
        }
        ClearCurrentTarget();
    }

    private void PickUpItem()
    {
        var itemInfo = currentTarget.GetComponent<InformationAboutObject>();
        if (itemInfo != null && inventoryController != null)
        {
            bool itemAdded = inventoryController.AddItemToInventory(itemInfo.gameObject, currentTarget);
            if (itemAdded) Debug.Log($"Предмет {itemInfo._name} добавлен в инвентарь");
        }
    }

    private void ClearInventory()
    {
        inventoryController.ResetSlots();
    }

    private void openDoor() 
    {
        if (isStudying == true)
        {
            _openDoor.openDoor();
        }
        else
        {
            generation.OpenDoor(lastDoorIndex);
        }
    }

    private void openLaptop()
    {
        cutsceneManager.PlayCutscene(0);
    }

    private void closeLaptop()
    {
        cutsceneManager.PlayCutscene(1);
    }
}