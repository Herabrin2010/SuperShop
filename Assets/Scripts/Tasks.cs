using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tasks : MonoBehaviour
{
    [Header("Task Settings")]
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private TextMeshProUGUI taskText;

    private InventoryController inventoryController;
    private string currentTaskName;
    private GameObject currentTaskPrefab;
    public bool IsTaskComplete { get; private set; }

    private void Awake()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        if (inventoryController == null)
            Debug.LogError("InventoryController not found!");
    }

    private void Start() => GenerateNewTask();

    public void GenerateNewTask()
    {
        if (itemPrefabs == null || itemPrefabs.Count == 0)
        {
            taskText.text = "No tasks available";
            return;
        }

        var randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
        var itemInfo = randomPrefab.GetComponent<InformationAboutObject>();

        if (itemInfo == null) return;

        currentTaskName = itemInfo._name;
        IsTaskComplete = false;
        UpdateTaskUI();
    }

    public void CheckTaskForItem(GameObject itemPrefab)
    {
        if (IsTaskComplete || itemPrefab == null) return;

        var itemInfo = itemPrefab.GetComponent<InformationAboutObject>();
        if (itemInfo != null && itemInfo._name == currentTaskName)
        {
            CompleteTask();
        }
    }


    private void CompleteTask()
    {
        IsTaskComplete = true;
        inventoryController.ResetSlots();
        GenerateNewTask();
    }

    private void UpdateTaskUI()
    {
        if (taskText != null)
            taskText.text = $"Current task:\n- {currentTaskName}";
    }
}