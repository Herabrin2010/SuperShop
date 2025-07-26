using UnityEngine;

public class HoverManager : MonoBehaviour
{
    [SerializeField] private Animator textAnimator;
    private Animator buttonAnimator;
    public int buttonNumber;

    void Start()
    {
        buttonAnimator = GetComponent<Animator>();
    }

    public void OnPointerEnter()
    {
        buttonAnimator.SetInteger("CurrentButton", buttonNumber);
        textAnimator.SetBool("TextAppear", true);
    }

    public void OnPointerExit()
    {
        buttonAnimator.SetInteger("CurrentButton", 0);
        textAnimator.SetBool("TextAppear", false);
    }
}