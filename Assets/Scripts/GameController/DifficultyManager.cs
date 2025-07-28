using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [SerializeField] private DifficultyData[] difficulties;
    public DifficultyData CurrentDifficulty { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // По умолчанию выбираем среднюю сложность
        SetDifficulty(1); // 0 - Easy, 1 - Medium, 2 - Hard
    }

    public void SetDifficulty(int index)
    {
        if (index >= 0 && index < difficulties.Length)
        {
            CurrentDifficulty = difficulties[index];
            Debug.Log($"Установлена сложность: {CurrentDifficulty.difficultyName}");
        }
    }
}