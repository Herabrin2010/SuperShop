using UnityEngine;
using TMPro;

public class RaycastController : MonoBehaviour
{
    [Header("���������")]
    public Camera playerCamera;
    public float interactionDistance = 3f;
    public TextMeshProUGUI help;

    [Header("������")]
    private InventoryController inventoryController;
    private KeyRebinder keyRebinder;
    private KeyBindingsData keyBindingData;
    private Tasks tasks;
    private MultiCutsceneManager multiCutsceneManager;

    private GameObject currentTarget;

    private void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (help != null) help.gameObject.SetActive(false);

        multiCutsceneManager = FindAnyObjectByType<MultiCutsceneManager>();
        inventoryController = FindAnyObjectByType<InventoryController>();
        keyRebinder = FindAnyObjectByType<KeyRebinder>();
        keyBindingData = FindAnyObjectByType<KeyBindingsData>();
        tasks = FindAnyObjectByType<Tasks>();


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
        return "E"; // �������� �� ���������, ���� �� �������
    }

    private void PerformRaycast()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out var hit, interactionDistance))
        {
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
                ShowInteractionPrompt($"������� {getInteractionKey()} ����� ����� {currentTarget.name}");
                break;
            case "Trashcan":
                ShowInteractionPrompt($"������� {getInteractionKey()} ����� �������� ���������");
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
        }
        ClearCurrentTarget();
    }

    private void PickUpItem()
    {
        var itemInfo = currentTarget.GetComponent<InformationAboutObject>();
        if (itemInfo != null && inventoryController != null)
        {
            bool itemAdded = inventoryController.AddItemToInventory(itemInfo.gameObject, currentTarget);
            if (itemAdded) Debug.Log($"������� {itemInfo._name} �������� � ���������");
        }
    }

    private void ClearInventory()
    {
        multiCutsceneManager.PlayCutscene(1);
        inventoryController.ResetSlots();
    }
}