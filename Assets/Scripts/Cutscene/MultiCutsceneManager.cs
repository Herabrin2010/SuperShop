using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

public class MultiCutsceneManager : MonoBehaviour
{
    [Header("�������� ���������")]
    public Camera playerCamera; // ������� ������ ������
    public GameObject player; // ������ ������
    public CutsceneData[] cutscenes; // ������ ���� �������

    private MonoBehaviour[] playerMovementScripts;
    private Animator playerAnimator;
    private CutsceneData currentCutscene; // ������� �������� ��������

    private void Awake()
    {
        // �������� ���������� ������
        if (player != null)
        {
            playerMovementScripts = player.GetComponents<MonoBehaviour>();
            playerAnimator = player.GetComponent<Animator>();
        }

        // ��������� ��� ������ ���� ������� � ������
        foreach (var cutscene in cutscenes)
        {
            SetCutsceneCamerasActive(cutscene, false);
        }

        if (playerCamera != null)
        {
            playerCamera.tag = "MainCamera"; // ��� ����������!
        }
    }

    // === ������ �������� �� ID ===
    public void PlayCutscene(int cutsceneID)
    {
        if (cutsceneID < 0 || cutsceneID >= cutscenes.Length)
        {
            Debug.LogError($"�������� � ID {cutsceneID} �� ����������!");
            return;
        }

        currentCutscene = cutscenes[cutsceneID];
        PlayCutscene(currentCutscene);
    }

    // === ������ �������� �� ����� ===
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

        Debug.LogError($"�������� � ������ {cutsceneName} �� �������!");
    }

    // === �������� ����� ������� ===
    private void PlayCutscene(CutsceneData cutscene)
    {
        if (cutscene.timeline == null)
        {
            Debug.LogError("Timeline �� ��������!");
            return;
        }

        // 1. ��������� ���������� ������� (���� �����)
        if (cutscene.disablePlayerMovement)
            TogglePlayerMovement(false);

        // 2. ��������� ������� ������, �������� ������ ��������
        if (playerCamera != null)
            playerCamera.enabled = false;

        SetCutsceneCamerasActive(cutscene, true);

        // 3. ��������� Timeline
        cutscene.timeline.Play();

        // 4. ������������� �� ����������
        cutscene.timeline.stopped += OnCutsceneEnd;
    }

    // === ���������� ���������� �������� ===
    private void OnCutsceneEnd(PlayableDirector director)
    {
        // 1. ���������� ���������� (���� ���� ���������)
        if (currentCutscene.disablePlayerMovement)
            TogglePlayerMovement(true);

        // 2. ��������� ������ ��������, �������� ������� ������
        SetCutsceneCamerasActive(currentCutscene, false);

        if (playerCamera != null)
            playerCamera.enabled = true;

        // 3. ���������� �������� (���� �����)
        if (currentCutscene.resetAnimations && playerAnimator != null)
        {
            playerAnimator.Rebind();
            playerAnimator.Update(0f);
        }

        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            playerCamera.gameObject.SetActive(true); // �� ������ ������
            playerCamera.transform.position = new Vector3(0f, 0f, 0f);
        }

        // 4. ������������ �� �������
        director.stopped -= OnCutsceneEnd;
        currentCutscene = null;
    }

    // === ���������/���������� ���������� ������� ===
    private void TogglePlayerMovement(bool enable)
    {
        foreach (var script in playerMovementScripts)
            if (script is MonoBehaviour && script != this)
                script.enabled = enable;
    }

    // === ���������� �������� �������� ===
    private void SetCutsceneCamerasActive(CutsceneData cutscene, bool active)
    {
        foreach (var cam in cutscene.cameras)
            cam.gameObject.SetActive(active);
    }

}