using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class MultiCutsceneManager : MonoBehaviour
{
    [Header("Основные настройки")]
    public Camera playerCamera; // Обычная камера игрока
    public GameObject player; // Объект игрока
    public CutsceneData[] cutscenes; // Массив всех катсцен

    private MonoBehaviour[] playerMovementScripts;
    private Animator playerAnimator;
    private CutsceneData currentCutscene; // Текущая активная катсцена

    private void Awake()
    {
        // Получаем компоненты игрока
        if (player != null)
        {
            playerMovementScripts = player.GetComponents<MonoBehaviour>();
            playerAnimator = player.GetComponent<Animator>();
        }

        // Выключаем все камеры всех катсцен в начале
        foreach (var cutscene in cutscenes)
        {
            SetCutsceneCamerasActive(cutscene, false);
        }

        if (playerCamera != null)
        {
            playerCamera.tag = "MainCamera"; // Тег обязателен!
        }
    }

    // === Запуск катсцены по ID ===
    public void PlayCutscene(int cutsceneID)
    {
        if (cutsceneID < 0 || cutsceneID >= cutscenes.Length)
        {
            Debug.LogError($"Катсцены с ID {cutsceneID} не существует!");
            return;
        }

        currentCutscene = cutscenes[cutsceneID];
        PlayCutscene(currentCutscene);
    }

    // === Запуск катсцены по имени ===
    public void PlayCutscene(string cutsceneName)
    {
        foreach (var cutscene in cutscenes)
        {
            if (cutscene.name == cutsceneName)
            {
                currentCutscene = cutscene;
                PlayCutscene(cutscene);
                return;
            }
        }

        Debug.LogError($"Катсцены с именем {cutsceneName} не найдено!");
    }

    // === Основной метод запуска ===
    private void PlayCutscene(CutsceneData cutscene)
    {
        if (cutscene.timeline == null)
        {
            Debug.LogError("Timeline не назначен!");
            return;
        }

        // 1. Отключаем управление игроком (если нужно)
        if (cutscene.disablePlayerMovement)
            TogglePlayerMovement(false);

        // 2. Выключаем обычную камеру, включаем камеры катсцены
        if (playerCamera != null)
            playerCamera.enabled = false;

        SetCutsceneCamerasActive(cutscene, true);

        // 3. Запускаем Timeline
        cutscene.timeline.Play();

        // 4. Подписываемся на завершение
        cutscene.timeline.stopped += OnCutsceneEnd;
    }

    // === Обработчик завершения катсцены ===
    private void OnCutsceneEnd(PlayableDirector director)
    {
        // 1. Возвращаем управление (если было отключено)
        if (currentCutscene.disablePlayerMovement)
            TogglePlayerMovement(true);

        // 2. Выключаем камеры катсцены, включаем обычную камеру
        SetCutsceneCamerasActive(currentCutscene, false);

        if (playerCamera != null)
            playerCamera.enabled = true;

        // 3. Сбрасываем анимацию (если нужно)
        if (currentCutscene.resetAnimations && playerAnimator != null)
        {
            playerAnimator.Rebind();
            playerAnimator.Update(0f);
        }

        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            playerCamera.gameObject.SetActive(true); // На всякий случай
            playerCamera.transform.position = new Vector3(0f, 0f, 0f);
        }

        // 4. Отписываемся от события
        director.stopped -= OnCutsceneEnd;
        currentCutscene = null;
    }

    // === Включение/выключение управления игроком ===
    private void TogglePlayerMovement(bool enable)
    {
        foreach (var script in playerMovementScripts)
            if (script is MonoBehaviour && script != this)
                script.enabled = enable;
    }

    // === Управление камерами катсцены ===
    private void SetCutsceneCamerasActive(CutsceneData cutscene, bool active)
    {
        foreach (var cam in cutscene.cameras)
            cam.gameObject.SetActive(active);
    }

}