using UnityEngine;
using TMPro;

public class RaycastController : MonoBehaviour
{
    [Header("Настройки")]
    public Camera playerCamera;
    public float interactionDistance = 3f;
    public TextMeshProUGUI interactionText;

    [Header("Ссылки")]
    private InventoryController inventoryController;
    private KeyRebinder inputSystem;
    private Tasks tasks;

    private GameObject currentTarget;

    private void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (interactionText != null) interactionText.gameObject.SetActive(false);

        inventoryController = FindObjectOfType<InventoryController>();
        inputSystem = FindObjectOfType<KeyRebinder>();
        tasks = FindObjectOfType<Tasks>();

        if (inventoryController == null) Debug.LogError("InventoryController not found!");
        if (inputSystem == null) Debug.LogError("KeyRebinder not found!");
        if (tasks == null) Debug.LogError("Tasks component not found!");
    }

    private void Update()
    {
        PerformRaycast();
        CheckForInteraction();
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
                ShowInteractionPrompt($"Нажмите E чтобы взять {currentTarget.name}");
                break;
            case "Trashcan":
                ShowInteractionPrompt("Нажмите E чтобы очистить инвентарь");
                break;
        }
    }

    private void ShowInteractionPrompt(string message)
    {
        if (interactionText != null)
        {
            interactionText.text = message;
            interactionText.gameObject.SetActive(true);
        }
    }

    private void ClearCurrentTarget()
    {
        if (currentTarget != null && interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
        currentTarget = null;
    }

    private void CheckForInteraction()
    {
        if (inputSystem != null && inputSystem.GetActionDown("Interaction") && currentTarget != null)
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
            if (itemAdded) Debug.Log($"Предмет {itemInfo._name} добавлен в инвентарь");
        }
    }

    private void ClearInventory()
    {
        inventoryController?.ResetSlots(true);
    }
}