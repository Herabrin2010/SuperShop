using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatManager : MonoBehaviour
{
    [Header ("Seetings")]
    public GameObject chatPanel;  // Панель чата (Canvas → Panel)
    public TMP_Text messagePrefab; // Префаб текста сообщения
    public TMP_Text diologPrefab;
    public Transform messageParent; // Родительский объект для сообщений (ScrollView Content)
    public ScrollRect chatScrollRect;

    [Header("Links")]
    private PlayerController playerController;

    [Header ("Diolog")]
    [SerializeField] private TextMeshProUGUI diologText;
    private int _numberOfPhrase;

    public string[] phrases;


    private bool isChatOpen = false;

    private void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // Открываем/закрываем чат по нажатию T
        {

            isChatOpen = !isChatOpen;
            chatPanel.SetActive(isChatOpen);
            diologPrefab.gameObject.SetActive(!isChatOpen);

            playerController.CameraLock = isChatOpen;
            playerController.MovementLock = isChatOpen;

            #region Cursor
            Cursor.visible = isChatOpen;

            if (isChatOpen == true) Cursor.lockState = CursorLockMode.None;

            else if (isChatOpen == false) Cursor.lockState = CursorLockMode.Locked;
            #endregion

            
        }
    }

    // Метод для добавления сообщения в чат
    public void AddMessage(string speaker)
    {
        TMP_Text newMessage = Instantiate(messagePrefab, messageParent);
        newMessage.text = $"{speaker}: {phrases[_numberOfPhrase]}";

        StartCoroutine(ScrollToBottom());
    }

    public void NewPhraseStart(int nubmerOfPhrase)
    {
        diologText.text = null;
        diologText.text = phrases[nubmerOfPhrase];
        _numberOfPhrase = nubmerOfPhrase;
    }

    private IEnumerator ScrollToBottom()
    {
        if (chatScrollRect == null)
        {
            Debug.LogError("ScrollRect not assigned!");
            yield break;
        }

        yield return new WaitForEndOfFrame();
        chatScrollRect.verticalNormalizedPosition = 0;
    }
}