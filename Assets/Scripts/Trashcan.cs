using System.Collections.Generic;
using UnityEngine;

public class Trashcan : MonoBehaviour
{
    public InventoryController inventoryController;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                inventoryController.ResetSlots();
            }
        }
    }
}
