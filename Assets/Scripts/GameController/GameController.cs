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
        // ћожно добавить сообщение об успешном сохранении
    }
    public void LoadGame()
    {
        if (SaveSystem.SaveExists())
        {
            SaveSystem.LoadGame();
            // ћожно добавить сообщение об успешной загрузке
        }
        else
        {
            Debug.Log("Ќет сохранений дл€ загрузки");
            // ѕоказать сообщение игроку
        }
    }

    public void DeleteSave()
    {
        SaveSystem.DeleteSave();
        // ћожно добавить сообщение об удалении сохранени€
    }
}
