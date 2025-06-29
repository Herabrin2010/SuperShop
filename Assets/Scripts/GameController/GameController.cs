using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    public void ReloadScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void SaveGame()
    {
        SaveSystem.SaveGame();
        // ����� �������� ��������� �� �������� ����������
    }
    public void LoadGame()
    {
        if (SaveSystem.SaveExists())
        {
            SaveSystem.LoadGame();
            // ����� �������� ��������� �� �������� ��������
        }
        else
        {
            Debug.Log("��� ���������� ��� ��������");
            // �������� ��������� ������
        }
    }

    public void DeleteSave()
    {
        SaveSystem.DeleteSave();
        // ����� �������� ��������� �� �������� ����������
    }
}
