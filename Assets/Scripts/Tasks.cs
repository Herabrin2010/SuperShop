using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tasks : MonoBehaviour
{
    [Header("Task Settings")]
    public List<GameObject> itemPrefabs;
    public TextMeshPro taskText;

    [Header ("—сылки")]
    private AdminPanel adminPanel;
    private InventoryController inventoryController;
    private Score_Timer score_timer;

    [HideInInspector] public string currentTaskName;

    private void Awake()
    {
        score_timer = FindAnyObjectByType<Score_Timer>();
        inventoryController = FindAnyObjectByType<InventoryController>();
        adminPanel = FindAnyObjectByType<AdminPanel>();

        if (inventoryController == null)
            Debug.LogError("InventoryController not found!");
    }

    private void Start()
    {
     GenerateNewTask();
    }

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
        UpdateTaskUI();

        Debug.Log(score_timer.CurrectScore);
    }

    public void CompleteTask()
    {
        score_timer.Price();
        GenerateNewTask();
        score_timer.IfWin();
    }

    private void UpdateTaskUI()
    {
        if (taskText != null)
            taskText.text = currentTaskName;
    }
}