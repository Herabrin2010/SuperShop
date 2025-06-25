using Cinemachine;
using UnityEngine.Playables;

[System.Serializable]
public class CutsceneData
{
    public string name; // Название для удобства (опционально)
    public PlayableDirector timeline; // Timeline катсцены
    public CinemachineVirtualCamera[] cameras; // Камеры этой катсцены
    public bool disablePlayerMovement = true; // Блокировать управление?
    public bool resetAnimations = true; // Сбросить анимации после завершения?
}