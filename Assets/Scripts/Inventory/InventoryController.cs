using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public List<GameObject> Slots = new List<GameObject>();


    public void SearchingFreeSlot(string name, Sprite sprite)
    {
        foreach (GameObject item in Slots)
        {
            

            SlotInformation slot = item.GetComponent<SlotInformation>();
            if (!slot.FreeSlot)
            {
                Debug.Log(name);
                slot.UpdateIcon(name, sprite);
                break;
            }

        }
    }
}
