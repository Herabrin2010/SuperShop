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
        public bool lockPlayerControls = true;
        public UnityEvent onCutsceneStart;
        public UnityEvent onCutsceneUpdate;
        public UnityEvent onCutsceneFinished;
        public float duration;
    }

    public Cutscene[] cutscenes;
    public Camera mainCamera;

    [Header("Settings")]
    public bool showDebugLogs = true;

    public bool IsCutscenePlaying { get; private set; }

    private PlayerController playerController;
    private Coroutine currentCutsceneRoutine;
    private PlayableDirector currentTimeline;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            mainCamera.tag = "MainCamera";
        }
    }

    public void PlayCutscene(int index)
    {
        if (IsCutscenePlaying)
        {
            Debug.LogWarning("Cutscene is already playing!");
            return;
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
        IsCutscenePlaying = true;
        LockPlayerControls();
        cutscene.onCutsceneStart.Invoke();

        if (cutscene.cutsceneObject != null)
        {
            cutscene.cutsceneObject.SetActive(true);
            currentTimeline = cutscene.cutsceneObject.GetComponent<PlayableDirector>();
        }

        if (cutscene.cutsceneCamera != null)
        {
            cutscene.cutsceneCamera.gameObject.SetActive(true);
            if (cutscene.disableMainCamera && mainCamera != null)
                mainCamera.gameObject.SetActive(false);
        }

        if (showDebugLogs)
            Debug.Log($"Starting cutscene: {cutscene.name}");

        float timer = 0f;
        float duration = cutscene.duration > 0 ? cutscene.duration :
                         currentTimeline != null ? (float)currentTimeline.duration : 0f;

        while (duration == 0 || timer < duration)
        {
            timer += Time.deltaTime;
            cutscene.onCutsceneUpdate.Invoke();

            if (currentTimeline != null && currentTimeline.state != PlayState.Playing)
                break;

            yield return null;
        }

        StopCurrentCutscene();
        cutscene.onCutsceneFinished.Invoke();

        if (showDebugLogs)
            Debug.Log($"Finished cutscene: {cutscene.name}");

        IsCutscenePlaying = false;
    }

    private void StopCurrentCutscene()
    {
        if (currentTimeline != null)
        {
            currentTimeline.Stop();

            if (currentTimeline.gameObject != null)
            {
                currentTimeline.gameObject.SetActive(false);
            }

            currentTimeline = null;
        }

        RestoreMainCamera();
        UnlockPlayerControls();
    }

    private void RestoreMainCamera()
    {
        if (mainCamera != null && !mainCamera.gameObject.activeSelf)
        {
            mainCamera.gameObject.SetActive(true);
            mainCamera.transform.position = Vector3.zero;
        }

        foreach (var cutscene in cutscenes)
        {
            if (cutscene.cutsceneCamera != null && cutscene.cutsceneCamera.gameObject.activeSelf)
            {
                cutscene.cutsceneCamera.gameObject.SetActive(false);
            }
        }
    }

    private void LockPlayerControls()
    {
        foreach (var cutscene in cutscenes)
        {
            if (!cutscene.lockPlayerControls || playerController == null) return;
        }

        playerController.CameraLock = true;
        playerController.MovementLock = true;
        playerController.CameraLockX = true;
        playerController.CameraLockY = true;
    }

    private void UnlockPlayerControls()
    {
        foreach (var cutscene in cutscenes)
        {
            if (!cutscene.lockPlayerControls || playerController == null) return;
        }

        playerController.CameraLock = false;
        playerController.MovementLock = false;
        playerController.CameraLockX = false;
        playerController.CameraLockY = false;
    }

    private void OnDisable()
    {
        if (IsCutscenePlaying)
        {
            StopCurrentCutscene();
            IsCutscenePlaying = false;
        }

        if (playerController != null)
        {
            playerController.CameraLock = false;
            playerController.CameraLockX = false;
            playerController.CameraLockY = false;
        }
    }
}