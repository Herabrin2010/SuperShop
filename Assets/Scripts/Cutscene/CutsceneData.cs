using Cinemachine;
using UnityEngine.Playables;

[System.Serializable]
public class CutsceneData
{
    public string name; // �������� ��� �������� (�����������)
    public PlayableDirector timeline; // Timeline ��������
    public CinemachineVirtualCamera[] cameras; // ������ ���� ��������
    public bool disablePlayerMovement = true; // ����������� ����������?
    public bool resetAnimations = true; // �������� �������� ����� ����������?
}