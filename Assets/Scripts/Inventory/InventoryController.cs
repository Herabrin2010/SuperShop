using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public List<GameObject> Slots = new List<GameObject>();
    int slots = 0;


    public void SearchingFreeSlot(string name, Sprite sprite, GameObject destroyObject)
    {
        foreach (GameObject item in Slots)
        {

            SlotInformation slot = item.GetComponent<SlotInformation>();
            if (!slot.FreeSlot)
            {
                slots++;
                Debug.Log(name);
                slot.UpdateIcon(name, sprite);
                Destroy(destroyObject);
                break;
            }

        }
        if (slots == Slots.Count)
        {
            Debug.Log("инвентарь заполнен");
        }
    }

    public void ResetSlots()
    {
        foreach(GameObject item in Slots)
        {
            SlotInformation slot = item.GetComponent<SlotInformation>();
            
                slot.ResetInformation();
            
        }
    }
}
