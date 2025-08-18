using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class LaptopControll : MonoBehaviour
{
    [Header("Video Surveillance")]
    [SerializeField] private List<Camera> cameras = new List<Camera>();
    [SerializeField] private Material laptopScreen;
    [SerializeField] private Material screenOff;
    [SerializeField] private TextMeshPro _cameraIndex;
    [SerializeField] private Vector2Int renderTextureResolution = new Vector2Int(1024, 768);

    [Header("PTZ Settings")]
    [SerializeField] private int rotationSpeed = 30;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 30f;
    [SerializeField] private float minHorizontalAngle = -60f;
    [SerializeField] private float maxHorizontalAngle = 60f;

    [Header("Transition Settings")]
    [SerializeField] private VideoClip transitonVideo;
    private VideoPlayer videoPlayer;
    private Texture originalTexture;
    private int pendingCameraIndex; // Для хранения индекса камеры при переходе

    private int currentCameraIndex = 0;
    private Vector2 currentRotation;
    private List<RenderTexture> renderTextures = new List<RenderTexture>();

    [Header("Links")]
    private PlayerController playerController;
    private Generation generation;
    private RaycastController raycastController;
    private KeyRebinder keyRebinder;

    private void Awake()
    {
        #region Links
        playerController = FindAnyObjectByType<PlayerController>();
        generation = FindAnyObjectByType<Generation>();
        raycastController = FindAnyObjectByType<RaycastController>();
        keyRebinder = FindAnyObjectByType<KeyRebinder>();
        #endregion

        _cameraIndex.gameObject.SetActive(false);
    }

    private void Start()
    {

        InitializeCameras();
        DisableAllCameras();
        SwitchCamera(0);

        InitializeVideoPlayer();
    }

    private void InitializeVideoPlayer()
    {
        videoPlayer = GetComponent<VideoPlayer>() ?? gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;

        if (videoPlayer.targetTexture == null)
        {
            videoPlayer.targetTexture = new RenderTexture(1920, 1080, 24);
        }
    }

    public void DisableAllCameras()
    {
        foreach (Camera cam in cameras)
        {
            if (cam != null && cam.gameObject.activeSelf)
            {
                cam.gameObject.SetActive(false);
            }
        }
    }

    private void InitializeCameras()
    {
        // Очищаем старые RenderTextures
        foreach (var rt in renderTextures)
        {
            if (rt != null) rt.Release();
        }
        renderTextures.Clear();

        // Создаем новые RenderTextures
        foreach (Camera cam in cameras)
        {
            if (cam == null) continue;

            RenderTexture rt = new RenderTexture(
                renderTextureResolution.x,
                renderTextureResolution.y,
                24,
                RenderTextureFormat.ARGB32)
            {
                name = $"Cam_{cameras.IndexOf(cam)}_RT",
                antiAliasing = 2
            };

            cam.targetTexture = rt;
            cam.gameObject.SetActive(false);
            renderTextures.Add(rt);
        }
    }

    private void Update()
    {
        HandleCameraSwitch();
        HandlePTZRotation();
    }

    private void HandleCameraSwitch()
    {
        if (cameras.Count == 0) return;

        if (keyRebinder.GetActionDown("Next Camera Key"))
        {
            SwitchCamera((currentCameraIndex + 1) % cameras.Count);
        }
        else if (keyRebinder.GetActionDown("Previous Camera Key"))
        {
            SwitchCamera((currentCameraIndex - 1 + cameras.Count) % cameras.Count);
        }
    }

    private void SwitchCamera(int newIndex)
    {
        if (cameras.Count == 0) return;

        // Корректируем индекс
        newIndex = (newIndex + cameras.Count) % cameras.Count;

        // Отключаем текущую камеру
        if (currentCameraIndex >= 0 && currentCameraIndex < cameras.Count &&
            cameras[currentCameraIndex] != null)
        {
            cameras[currentCameraIndex].gameObject.SetActive(false);
        }

        // Сохраняем оригинальную текстуру
        if (originalTexture == null && laptopScreen != null)
        {
            originalTexture = laptopScreen.mainTexture;
        }

        // Обработка видео-перехода
        if (laptopScreen != null && videoPlayer != null && transitonVideo != null)
        {
            StartCoroutine(TransitionToCamera(newIndex));
        }
        else
        {
            DirectCameraSwitch(newIndex);
        }
    }

    private IEnumerator TransitionToCamera(int newIndex)
    {
        _cameraIndex.gameObject.SetActive(false);
        pendingCameraIndex = newIndex;

        // Настройка VideoPlayer
        videoPlayer.Stop();
        videoPlayer.clip = transitonVideo;
        laptopScreen.mainTexture = videoPlayer.targetTexture;

        // Удаляем старые подписки
        videoPlayer.loopPointReached -= HandleVideoEnd;
        videoPlayer.prepareCompleted -= HandleVideoPrepare;

        // Подписываемся на события
        videoPlayer.loopPointReached += HandleVideoEnd;
        videoPlayer.prepareCompleted += HandleVideoPrepare;

        // Запуск видео
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        videoPlayer.Play();

        yield return new WaitWhile(() => videoPlayer.isPlaying);
    }

    private void HandleVideoPrepare(VideoPlayer source)
    {
        if (source == videoPlayer && videoPlayer.clip == transitonVideo)
        {
            videoPlayer.Play();
        }
    }

    private void HandleVideoEnd(VideoPlayer source)
    {
        if (source == videoPlayer && videoPlayer.clip == transitonVideo)
        {
            DirectCameraSwitch(pendingCameraIndex);
            _cameraIndex.gameObject.SetActive(true);

            // Отписываемся после завершения
            videoPlayer.loopPointReached -= HandleVideoEnd;
            videoPlayer.prepareCompleted -= HandleVideoPrepare;
        }
    }

    private void DirectCameraSwitch(int newIndex)
    {
        currentCameraIndex = newIndex;

        if (currentCameraIndex >= 0 && currentCameraIndex < cameras.Count &&
            cameras[currentCameraIndex] != null)
        {
            cameras[currentCameraIndex].gameObject.SetActive(true);

            if (laptopScreen != null && currentCameraIndex < renderTextures.Count)
            {
                laptopScreen.mainTexture = renderTextures[currentCameraIndex];
            }

            _cameraIndex.text = (currentCameraIndex + 1).ToString();
            currentRotation = Vector2.zero;
            UpdateCameraRotation();
        }
    }

    private void HandlePTZRotation()
    {
        if (cameras.Count == 0 ||
            currentCameraIndex < 0 ||
            currentCameraIndex >= cameras.Count ||
            !cameras[currentCameraIndex].gameObject.activeSelf)
            return;

        float delta = rotationSpeed * Time.deltaTime;

        if (keyRebinder.GetAction("Movement Left")) currentRotation.y -= delta;
        if (keyRebinder.GetAction("Movement Right")) currentRotation.y += delta;
        if (keyRebinder.GetAction("Movement Forward")) currentRotation.x -= delta;
        if (keyRebinder.GetAction("Movement Back")) currentRotation.x += delta;

        currentRotation.x = Mathf.Clamp(currentRotation.x, minVerticalAngle, maxVerticalAngle);
        currentRotation.y = Mathf.Clamp(currentRotation.y, minHorizontalAngle, maxHorizontalAngle);

        UpdateCameraRotation();
    }

    private void UpdateCameraRotation()
    {
        if (cameras.Count == 0 ||
            currentCameraIndex < 0 ||
            currentCameraIndex >= cameras.Count)
            return;

        cameras[currentCameraIndex].transform.localRotation = Quaternion.Euler(
            currentRotation.x,
            currentRotation.y,
            0
        );
    }

    public Camera GetCurrentCamera()
    {
        if (currentCameraIndex >= 0 && currentCameraIndex < cameras.Count)
            return cameras[currentCameraIndex];
        return null;
    }

    public void OpenLaptop()
    {
        _cameraIndex.gameObject.SetActive(true);
        raycastController.help.gameObject.SetActive(false);

        playerController.CameraLock = true;
        playerController.MovementLock = true;

        if (laptopScreen != null && currentCameraIndex < renderTextures.Count)
        {
            laptopScreen.mainTexture = renderTextures[currentCameraIndex];
        }
    }

    public void CloseLaptop()
    {
        _cameraIndex.gameObject.SetActive(false);
        if (screenOff != null) laptopScreen.mainTexture = screenOff.mainTexture;

        playerController.CameraLock = false;
        playerController.MovementLock = false;
    }

    private void OnDestroy()
    {
        // Очищаем RenderTextures
        foreach (var rt in renderTextures)
        {
            if (rt != null) rt.Release();
        }

        // Отписываемся от событий VideoPlayer
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= HandleVideoEnd;
            videoPlayer.prepareCompleted -= HandleVideoPrepare;
        }
    }
}