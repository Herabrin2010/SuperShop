using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
    [Header("���������")]
    public List<GameObject> Slots = new();
    public TextMeshProUGUI inventoryFullText;
    public List<SlotInformation> slots = new();

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
    public Dictionary<string, int> GetInventoryData()
    {
        var inventoryData = new Dictionary<string, int>();
        foreach (var slot in slots)
        {
            if (!slot.IsFree && !inventoryData.ContainsKey(slot.ItemName))
            {
                inventoryData.Add(slot.ItemName, 1); // ���� �������� ���������, ����� ����������� �������
            }
        }
        return inventoryData;
    }

    public void LoadInventoryData(Dictionary<string, int> inventoryData)
    {
        ResetSlots(); // ������� ��������� ����� ���������

        foreach (var item in inventoryData)
        {
            // ������������, ��� � ���� ���� ������� ��������� � Resources
            var itemPrefab = Resources.Load<GameObject>(item.Key);
            if (itemPrefab != null)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    AddItemToInventory(itemPrefab, null); // ��������� ������� (��� ����������� �������)
                }
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

                // �������� ������� ��� �������
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
                Debug.Log($"������ �������: {tasks.currentTaskName}");
                tasks.CompleteTask();
            }
            slot.ClearSlot();
        }
        inventoryFullText.gameObject.SetActive(false);
    }
}