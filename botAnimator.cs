using UnityEngine;
using UnityEngine.InputSystem;

public class botAnimator : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool forward = Keyboard.current.wKey.isPressed;
        if (forward)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}