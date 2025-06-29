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
        public string actionName; // ��������, "Jump"
        public KeyCode defaultKey; // ������� �� ��������� (��������, KeyCode.Space)
        public Button bindButton; // ������ � UI
        public TextMeshProUGUI bindText; // �����, ������������ ������� �������
        [HideInInspector] public KeyCode currentKey; // ������� ����������� �������
    }

    public KeyBind[] keyBinds; // ������ ��������
    private bool isWaitingForInput = false; // ���� �������� �����
    private KeyBind currentKeyBind; // ������� �������� ��� ��������������

    public KeyBindingsData keyBindingsConfig;

    void Start()
    {
        // ��������� ���������� ������� (��� ���������� �������� �� ���������)
        LoadKeyBinds();

        // ������������� ������
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
                        // ��������� ����� �������
                        currentKeyBind.currentKey = keyCode;
                        currentKeyBind.bindText.text = keyCode.ToString();
                        isWaitingForInput = false;

                        // ��������� ���������
                        SaveKeyBinds();
                        break;
                    }
                }
            }
        }
    }

    // �������� �������������� �������
    public void StartRebinding(KeyBind keyBind)
    {
        if (!isWaitingForInput)
        {
            currentKeyBind = keyBind;
            keyBind.bindText.text = "Press any key...";
            isWaitingForInput = true;
        }
    }

    // ��������� ��� ������� ������� � PlayerPrefs
    private void SaveKeyBinds()
    {
        foreach (var keyBind in keyBinds)
        {
            // ��������� � PlayerPrefs
            PlayerPrefs.SetString(keyBind.actionName, keyBind.currentKey.ToString());

            // ��������� ScriptableObject
            var data = keyBindingsConfig.keyBinds.FirstOrDefault(k => k.actionName == keyBind.actionName);
            if (data != null) data.keyCode = keyBind.currentKey;
        }
        PlayerPrefs.Save();
    }

    // ��������� ���������� ������� (��� ���������� �������� �� ���������)
    private void LoadKeyBinds()
    {
        foreach (var keyBind in keyBinds)
        {
            // �������� ��������� ���������� �������
            string savedKey = PlayerPrefs.GetString(keyBind.actionName, keyBind.defaultKey.ToString());

            // ������ ������ ������� � KeyCode
            if (System.Enum.TryParse(savedKey, out KeyCode loadedKey))
            {
                keyBind.currentKey = loadedKey;
            }
            else
            {
                keyBind.currentKey = keyBind.defaultKey; // ���� �� ���������� � ���������� ������
            }
        }
    }

    // ����� ���� ������ � ��������� �� ���������
    public void ResetToDefaults()
    {
        foreach (var keyBind in keyBinds)
        {
            keyBind.currentKey = keyBind.defaultKey;
            keyBind.bindText.text = keyBind.defaultKey.ToString();
        }
        SaveKeyBinds(); // ��������� �����
    }

// ������ ������������� � ���� (��������, ��� ������)
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
