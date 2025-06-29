using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Данные игрока
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public float cameraRotationX;
    public int playerHealth;
    public bool isTimeTaskOn;

    // Данные инвентаря
    public List<InventorySlotData> inventorySlots = new List<InventorySlotData>();

    // Данные таймера и счета
    public int timeLeft;
    public int currentScore;

    // Данные задания
    public string currentTaskName;

    // Данные предметов на сцене
    public List<ItemData> spawnedItems = new List<ItemData>();

    // Данные монстра (если есть)
    public Vector3 monsterPosition;
    public Quaternion monsterRotation;
}

[System.Serializable]
public class InventorySlotData
{
    public string itemName;
    public string itemPrefabPath;
    public bool isFree;
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}