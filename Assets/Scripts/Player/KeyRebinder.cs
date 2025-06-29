using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class KeyRebinder : MonoBehaviour
{
    [System.Serializable]
    public class KeyBind
    {
        public string actionName; // Например, "Jump"
        public KeyCode defaultKey; // Клавиша по умолчанию (например, KeyCode.Space)
        public Button bindButton; // Кнопка в UI
        public TextMeshProUGUI bindText; // Текст, отображающий текущую клавишу
        [HideInInspector] public KeyCode currentKey; // Текущая назначенная клавиша
    }

    public KeyBind[] keyBinds; // Массив действий
    private bool isWaitingForInput = false; // Флаг ожидания ввода
    private KeyBind currentKeyBind; // Текущее действие для переназначения

    public KeyBindingsData keyBindingsConfig;

    void Start()
    {
        // Загружаем сохранённые клавиши (или используем значения по умолчанию)
        LoadKeyBinds();

        // Инициализация кнопок
        foreach (var keyBind in keyBinds)
        {
            keyBind.bindText.text = keyBind.currentKey.ToString();
            keyBind.bindButton.onClick.AddListener(() => StartRebinding(keyBind));
        }

        foreach (var data in keyBindingsConfig.keyBinds)
        {
            var uiElement = keyBinds.FirstOrDefault(k => k.actionName == data.actionName);
            if (uiElement != null)
            {
                uiElement.currentKey = data.keyCode;
                uiElement.bindText.text = data.keyCode.ToString();
            }
        }
    }

    void Update()
    {
        if (isWaitingForInput == true)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        // Назначаем новую клавишу
                        currentKeyBind.currentKey = keyCode;
                        currentKeyBind.bindText.text = keyCode.ToString();
                        isWaitingForInput = false;

                        // Сохраняем изменения
                        SaveKeyBinds();
                        break;
                    }
                }
            }
        }
    }

    // Начинаем переназначение клавиши
    public void StartRebinding(KeyBind keyBind)
    {
        if (!isWaitingForInput)
        {
            currentKeyBind = keyBind;
            keyBind.bindText.text = "Press any key...";
            isWaitingForInput = true;
        }
    }

    // Сохраняем все текущие клавиши в PlayerPrefs
    private void SaveKeyBinds()
    {
        foreach (var keyBind in keyBinds)
        {
            // Сохраняем в PlayerPrefs
            PlayerPrefs.SetString(keyBind.actionName, keyBind.currentKey.ToString());

            // Обновляем ScriptableObject
            var data = keyBindingsConfig.keyBinds.FirstOrDefault(k => k.actionName == keyBind.actionName);
            if (data != null) data.keyCode = keyBind.currentKey;
        }
        PlayerPrefs.Save();
    }

    // Загружаем сохранённые клавиши (или используем значения по умолчанию)
    private void LoadKeyBinds()
    {
        foreach (var keyBind in keyBinds)
        {
            // Пытаемся загрузить сохранённую клавишу
            string savedKey = PlayerPrefs.GetString(keyBind.actionName, keyBind.defaultKey.ToString());

            // Парсим строку обратно в KeyCode
            if (System.Enum.TryParse(savedKey, out KeyCode loadedKey))
            {
                keyBind.currentKey = loadedKey;
            }
            else
            {
                keyBind.currentKey = keyBind.defaultKey; // Если не получилось — используем дефолт
            }
        }
    }

    // Сброс всех клавиш к значениям по умолчанию
    public void ResetToDefaults()
    {
        foreach (var keyBind in keyBinds)
        {
            keyBind.currentKey = keyBind.defaultKey;
            keyBind.bindText.text = keyBind.defaultKey.ToString();
        }
        SaveKeyBinds(); // Сохраняем сброс
    }

// Пример использования в игре (например, для прыжка)
public bool GetActionDown(string actionName)
    {
        foreach (var keyBind in keyBinds)
        {
            if (keyBind.actionName == actionName)
            {
                return Input.GetKeyDown(keyBind.currentKey);
            }
        }
        return false;
    }

    public bool GetActionUp(string actionname)
    {
        foreach (var keyBind in keyBinds)
        {
            if (keyBind.actionName == actionname)
            {
                return Input.GetKeyUp(keyBind.currentKey);
            }
        }
        return false;
    }

    public bool GetAction(string actionName)
    {
        foreach (var keyBind in keyBinds)
        {
            if ((keyBind.actionName == actionName))
            {
                return Input.GetKey(keyBind.currentKey);
            }
        }
        return false;
    }
}
