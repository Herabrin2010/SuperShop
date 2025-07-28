using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [System.Serializable]
    public class Cutscene
    {
        public int cutsceneIndex;
        public string name;
        public GameObject cutsceneObject;
        public Camera cutsceneCamera;
        public bool disableMainCamera = true;

        [Tooltip("Срабатывает при начале катсцены")]
        public UnityEvent onCutsceneStart;

        [Tooltip("Срабатывает во время катсцены (можно использовать для синхронизации событий)")]
        public UnityEvent onCutsceneUpdate;

        [Tooltip("Срабатывает при завершении катсцены")]
        public UnityEvent onCutsceneFinished;

        [Tooltip("Длительность катсцены в секундах (0 для автоматического определения)")]
        public float duration;
    }

    public Cutscene[] cutscenes;
    public Camera mainCamera;

    [Header("Settings")]
    public bool lockPlayerControls = true;
    public bool showDebugLogs = true;

    private PlayerController playerController;
    private Coroutine currentCutsceneRoutine;
    private PlayableDirector currentTimeline;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();

        mainCamera.gameObject.SetActive(true);
        mainCamera.tag = "MainCamera";
    }

    public void PlayCutscene(int index)
    {
        if (currentCutsceneRoutine != null)
        {
            StopCoroutine(currentCutsceneRoutine);
            StopCurrentCutscene();
        }

        foreach (var cutscene in cutscenes)
        {
            if (cutscene.cutsceneIndex == index)
            {
                currentCutsceneRoutine = StartCoroutine(PlayCutsceneRoutine(cutscene));
                return;
            }
        }

        Debug.LogWarning($"Cutscene with index '{index}' not found!");
    }

    private IEnumerator PlayCutsceneRoutine(Cutscene cutscene)
    {
        // Подготовка катсцены
        LockPlayerControls();
        cutscene.onCutsceneStart.Invoke();

        // Активация объектов катсцены
        if (cutscene.cutsceneObject != null)
        {
            cutscene.cutsceneObject.SetActive(true);
            currentTimeline = cutscene.cutsceneObject.GetComponent<PlayableDirector>();
        }

        // Переключение камер
        if (cutscene.cutsceneCamera != null)
        {
            cutscene.cutsceneCamera.gameObject.SetActive(true);
            if (cutscene.disableMainCamera && mainCamera != null)
                mainCamera.gameObject.SetActive(false);
        }

        if (showDebugLogs)
            Debug.Log($"Starting cutscene: {cutscene.name}");

        // Запуск таймера катсцены
        float timer = 0f;
        float duration = cutscene.duration > 0 ? cutscene.duration :
                         currentTimeline != null ? (float)currentTimeline.duration : 0f;

        while (duration == 0 || timer < duration)
        {
            timer += Time.deltaTime;
            cutscene.onCutsceneUpdate.Invoke(); // Событие каждый кадр

            if (currentTimeline != null && currentTimeline.state != PlayState.Playing)
                break;

            yield return null;
        }

        // Завершение катсцены
        StopCurrentCutscene();
        cutscene.onCutsceneFinished.Invoke();

        if (showDebugLogs)
            Debug.Log($"Finished cutscene: {cutscene.name}");
    }

    private void StopCurrentCutscene()
    {
        if (currentTimeline != null)
        {
            currentTimeline.Stop();
            currentTimeline = null;
        }

        UnlockPlayerControls();
    }

    private void LockPlayerControls()
    {
        if (!lockPlayerControls || playerController == null) return;

        playerController.CameraLock = true;
        playerController.MovementLock = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UnlockPlayerControls()
    {
        if (!lockPlayerControls || playerController == null) return;

        playerController.CameraLock = false;
        playerController.MovementLock = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}