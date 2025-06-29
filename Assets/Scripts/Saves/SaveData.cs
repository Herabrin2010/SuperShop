using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // ������ ������
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public float cameraRotationX;
    public int playerHealth;
    public bool isTimeTaskOn;

    // ������ ���������
    public List<InventorySlotData> inventorySlots = new List<InventorySlotData>();

    // ������ ������� � �����
    public int timeLeft;
    public int currentScore;

    // ������ �������
    public string currentTaskName;

    // ������ ��������� �� �����
    public List<ItemData> spawnedItems = new List<ItemData>();

    // ������ ������� (���� ����)
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