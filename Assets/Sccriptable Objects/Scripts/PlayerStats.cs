using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Game/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header ("Movement Settings")]
    public int playerRunSpeed;
    public int playerSprintSpeed;
    public int playerSneekSpeed;
    public int playerJumpHeight;
    public float gravity;

    [Header("Camera Settings")]
    public int rotationSpeed;
    public bool invertingX;
    public bool invertingY;
}
