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
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        bool forward = keyboard.rightArrowKey.isPressed;
        bool run = keyboard.wKey.isPressed;

        if (run)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);

        }
        else if (forward)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);

        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
    }
}