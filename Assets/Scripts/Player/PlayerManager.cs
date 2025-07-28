using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private PlayerStats playerStats;
    public PlayerStats PlayerStats { get; private set; }

    private bool isInvertingX = false;
    private bool isInvertingY = false;

    private void Awake()
    {
        Instance = this;
        PlayerStats = playerStats;
    }

    public void InvertingX()
    {
        isInvertingX = !isInvertingX;
        playerStats.invertingX = isInvertingX;
    }

    public void InvertingY()
    {
        isInvertingY = !isInvertingY;
        playerStats.invertingY = isInvertingY;
    }
}