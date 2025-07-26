using UnityEngine;

public class Teleportaion : MonoBehaviour
{
    private GameController controller;

    private void Awake()
    {
        controller =  FindAnyObjectByType<GameController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Portal"))
        {
            controller.LoadLevel(2);
        }
    }
}
