using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    [Header("Настройки")]
    public List<GameObject> Slots = new();
    public TextMeshProUGUI inventoryFullText;
    private List<SlotInformation> slots = new();

    private Tasks tasks;

    public GameObject inventory;

    private void Start()
    {
        tasks = FindAnyObjectByType<Tasks>();
        inventoryFullText.gameObject.SetActive(false);
        foreach (var slot in Slots)
        {
            if (slot.TryGetComponent<SlotInformation>(out var slotInfo))
            {
                slotInfo.InitializeSlot();
                slots.Add(slotInfo);
            }
        }
    }

    private bool HasItem(SlotInformation slot, string targetItemName)
    {
        return !slot.IsFree && slot.ItemName == targetItemName;
    }

    public bool AddItemToInventory(GameObject itemPrefab, GameObject objectToDestroy)
    {
        foreach (var slot in slots)
        {
            if (slot.IsFree)
            {
                var info = itemPrefab.GetComponent<InformationAboutObject>();
                slot.FillSlot(info._name, info._sprite, itemPrefab);
                Destroy(objectToDestroy);

                // Проверка задания при подборе
                return true;
            }
        }
        inventoryFullText.gameObject.SetActive(true);
        return false;
    }

    public void ResetSlots()
    {
        foreach (var slot in slots)
        {
            if (HasItem(slot, tasks.currentTaskName))
            {
                Debug.Log($"Найден предмет: {tasks.currentTaskName}");
                tasks.CompleteTask();
            }
            slot.ClearSlot();
        }
        inventoryFullText.gameObject.SetActive(false);
    }
}