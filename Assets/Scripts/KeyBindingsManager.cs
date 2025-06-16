using System.Linq;
using UnityEngine;

public class KeyBindingsManager : MonoBehaviour
{
    public static KeyBindingsManager Instance { get; private set; }
    public KeyBindingsData keyBindingsData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public KeyCode GetKeyCode(string actionName)
    {
        var binding = keyBindingsData.keyBinds.FirstOrDefault(k => k.actionName == actionName);
        return binding?.keyCode ?? KeyCode.None;
    }
}