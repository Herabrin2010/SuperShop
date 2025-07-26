using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public Animator doorAnimator;
    public void openDoor()
    {
        doorAnimator.SetTrigger("OpenDoor");
    }
}
