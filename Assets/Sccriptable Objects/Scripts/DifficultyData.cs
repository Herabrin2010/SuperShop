using UnityEngine;

[CreateAssetMenu(fileName = "NewDifficulty", menuName = "Game/Difficulty")]
public class DifficultyData : ScriptableObject
{
    public string difficultyName;

    [Header ("Player")]
    public int playerHealth;
    public int taskTime;
    public int scoreNeed;

    [Header("Monster")]
    public bool generateMonster;

    public int monserRunSpeed;
    public int monserSprintSpeed;
    public int monsterDetectionRange;
    public int monsterAttackRange;
}