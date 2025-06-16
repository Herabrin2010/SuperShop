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

    private void Start()
    {
        tasks = FindObjectOfType<Tasks>();
        foreach (var slot in Slots)
        {
            if (slot.TryGetComponent<SlotInformation>(out var slotInfo))
            {
                slotInfo.InitializeSlot();
                slots.Add(slotInfo);
            }
        }
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
                tasks?.CheckTaskForItem(itemPrefab);
                return true;
            }
        }
        ShowInventoryFullMessage();
        return false;
    }

    public void ResetSlots(bool fromTrashcan = false)
    {
        if (fromTrashcan)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsFree)
                {
                    // Проверка задания для каждого удаляемого предмета
                    tasks?.CheckTaskForItem(slot.ItemPrefab);
                }
            }
        }

        foreach (var slot in slots) slot.ClearSlot();
        HideInventoryFullMessage();
    }

    private void ShowInventoryFullMessage() => inventoryFullText?.gameObject.SetActive(true);
    private void HideInventoryFullMessage() => inventoryFullText?.gameObject.SetActive(false);
}