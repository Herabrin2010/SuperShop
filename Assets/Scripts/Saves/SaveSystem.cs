using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame()
    {
        SaveData saveData = new SaveData();

        // ��������� ������ ������
        PlayerController player = Object.FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            saveData.playerPosition = player.transform.position;
            saveData.playerRotation = player.transform.rotation;
            saveData.cameraRotationX = player.currentCameraRotationX;
            saveData.playerHealth = player.PlayerHealth;
            saveData.isTimeTaskOn = player.isTime_Task;
        }

        // ��������� ���������
        InventoryController inventory = Object.FindAnyObjectByType<InventoryController>();
        if (inventory != null)
        {
            foreach (var slot in inventory.slots)
            {
                InventorySlotData slotData = new InventorySlotData
                {
                    itemName = slot.ItemName,
                    itemPrefabPath = slot.ItemPrefab != null ? slot.ItemPrefab.name : "",
                    isFree = slot.IsFree
                };
                saveData.inventorySlots.Add(slotData);
            }
        }

        // ��������� ������ � ����
        Score_Timer scoreTimer = Object.FindAnyObjectByType<Score_Timer>();
        if (scoreTimer != null)
        {
            saveData.timeLeft = scoreTimer.timeLeft;
            saveData.currentScore = scoreTimer.CurrectScore;
        }

        // ��������� ������� �������
        Tasks tasks = Object.FindAnyObjectByType<Tasks>();
        if (tasks != null)
        {
            saveData.currentTaskName = tasks.currentTaskName;
        }

        // ��������� �������� �� �����
        ItemGenerator itemGenerator = Object.FindAnyObjectByType<ItemGenerator>();
        if (itemGenerator != null)
        {
            foreach (var item in itemGenerator.spawnedItems)
            {
                if (item != null)
                {
                    ItemData itemData = new ItemData
                    {
                        itemName = item.name.Replace("(Clone)", ""),
                        prefabName = item.name.Replace("(Clone)", ""),
                        position = item.transform.position,
                        rotation = item.transform.rotation,
                        scale = item.transform.localScale
                    };
                    saveData.spawnedItems.Add(itemData);
                }
            }
        }

        // ��������� ������ ������� (���� ����)
        GameObject monster = GameObject.FindGameObjectWithTag("Monster");
        if (monster != null)
        {
            saveData.monsterPosition = monster.transform.position;
            saveData.monsterRotation = monster.transform.rotation;
        }

        // ����������� � JSON � ���������
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Game saved to: " + savePath);
    }

    public static bool LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            // ��������� ������ ������
            PlayerController player = Object.FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                player.transform.position = saveData.playerPosition;
                player.transform.rotation = saveData.playerRotation;
                player.currentCameraRotationX = saveData.cameraRotationX;
                player.PlayerHealth = saveData.playerHealth;
                player.isTime_Task = saveData.isTimeTaskOn;
                player.isTime_TaskOn = saveData.isTimeTaskOn;
                player.isTime_TaskOff = !saveData.isTimeTaskOn;
            }

            // ��������� ���������
            InventoryController inventory = Object.FindAnyObjectByType<InventoryController>();
            if (inventory != null)
            {
                for (int i = 0; i < saveData.inventorySlots.Count && i < inventory.slots.Count; i++)
                {
                    var slotData = saveData.inventorySlots[i];
                    if (!slotData.isFree)
                    {
                        // ������� ������ ��������
                        GameObject prefab = Resources.Load<GameObject>(slotData.itemPrefabPath);
                        if (prefab != null)
                        {
                            inventory.slots[i].FillSlot(slotData.itemName, prefab.GetComponent<InformationAboutObject>()._sprite, prefab);
                        }
                    }
                    else
                    {
                        inventory.slots[i].ClearSlot();
                    }
                }
            }

            // ��������� ������ � ����
            Score_Timer scoreTimer = Object.FindAnyObjectByType<Score_Timer>();
            if (scoreTimer != null)
            {
                scoreTimer.timeLeft = saveData.timeLeft;
                scoreTimer.CurrectScore = saveData.currentScore;
                scoreTimer.StopAllCoroutines();
                scoreTimer.StartCoroutine(scoreTimer.GameOverTimer());
                scoreTimer._timeLeft.text = "�����: " + saveData.timeLeft.ToString();
                scoreTimer.valueTextCurrectScore.text = "������� ����: " + saveData.currentScore.ToString();
            }

            // ��������� ������� �������
            Tasks tasks = Object.FindAnyObjectByType<Tasks>();
            if (tasks != null)
            {
                tasks.currentTaskName = saveData.currentTaskName;
                tasks.UpdateTaskUI();
            }

            // ������� ������ �������� � ��������� �����������
            ItemGenerator itemGenerator = Object.FindAnyObjectByType<ItemGenerator>();
            if (itemGenerator != null)
            {
                itemGenerator.ClearItems();
                foreach (var itemData in saveData.spawnedItems)
                {
                    GameObject prefab = Resources.Load<GameObject>(itemData.prefabName);
                    if (prefab != null)
                    {
                        GameObject item = Object.Instantiate(prefab, itemData.position, itemData.rotation);
                        item.transform.localScale = itemData.scale;
                        item.name = itemData.itemName;
                        itemGenerator.spawnedItems.Add(item);
                    }
                }
            }

            // ��������� ������ ������� (���� ����)
            GameObject monster = GameObject.FindGameObjectWithTag("Monster");
            if (monster != null)
            {
                monster.transform.position = saveData.monsterPosition;
                monster.transform.rotation = saveData.monsterRotation;
            }

            Debug.Log("Game loaded from: " + savePath);
            return true;
        }
        else
        {
            Debug.LogWarning("No save file found at: " + savePath);
            return false;
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted");
        }
    }
}