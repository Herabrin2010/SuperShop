using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f;

    private TMP_Text tmpText;
    private string fullText;
    private bool isTyping = false;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();

        if (tmpText != null)
            fullText = tmpText.text;
        else
            Debug.LogError("Не найден Text или TMP_Text!", this);
    }

    public void StartTyping()
    {
        if (!isTyping && !string.IsNullOrEmpty(fullText))
        {
            StartCoroutine(TypeText());
        }
    }

    public void StopTyping()
    {
        StopAllCoroutines();
        isTyping = false;
    }

    public void RemoveTyping()
    {
        StopAllCoroutines();
        ApplyText("");
        isTyping = false;
    }

    private System.Collections.IEnumerator TypeText()
    {
        isTyping = true;
        string currentText = "";

        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i);
            ApplyText(currentText);
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void ApplyText(string text)
    {
            tmpText.text = text;
            tmpText.ForceMeshUpdate(); // Обновляем TMP
    }
}