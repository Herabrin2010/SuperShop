using UnityEngine;

[CreateAssetMenu(fileName = "KeyBindingsData", menuName = "Game/KeyBindingsData")]
public class KeyBindingsData : ScriptableObject
{
    [System.Serializable]
    public class KeyBind
    {
        public string actionName;
        public KeyCode keyCode;
    }

    public KeyBind[] keyBinds;
}