using UnityEngine;
using UnityEngine.Events;

public class Canvas : MonoBehaviour
{
    [SerializeField] private UnityEvent escape;

    private bool isInverted = false;

    private void Update()
    {
        Escape();
    }

    private void Escape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escape.Invoke();
        }
    }

    #region Inverting

    public void InvertingX()
    {
        if (isInverted == false)
        {
            Debug.Log("InvertingXOn");
            isInverted = true;
            PlayerManager.Instance.PlayerStats.invertingX = true;
        }

        else if (isInverted == true)
        {
            Debug.Log("InvertingXOff");
            isInverted = false;
            PlayerManager.Instance.PlayerStats.invertingX = false;
        }
    }

    public void InvertingY()
    {
        if (isInverted == false)
        {
            Debug.Log("InvertingYOn");
            isInverted = true;
            PlayerManager.Instance.PlayerStats.invertingY = true;
        }

        else if (isInverted == true)
        {
            Debug.Log("InvertingYOff");
            isInverted = false;
            PlayerManager.Instance.PlayerStats.invertingY = false;
        }
    }
    #endregion
}
