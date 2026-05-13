using UnityEngine;
using UnityEngine.InputSystem;

public class botAnimator : MonoBehaviour
{
    Animator animator;
    float velocity = 0.0f;
    float accelaration = 0.5f;
    float deceleration = 0.5f;

    int VelocityHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        VelocityHash = Animator.StringToHash("Velocity");
    }

    void Update()
    {
        bool forwardPressed = Keyboard.current.rightArrowKey.isPressed;
        //bool runPressed = Keyboard.current.rightArrowKey.isPressed;

        if (forwardPressed && velocity < 1.0f)
        {
            velocity += Time.deltaTime * accelaration;
        }
        if (!forwardPressed && velocity > 0.0f)
        {
            velocity -= Time.deltaTime * deceleration;
        }
        if (!forwardPressed && velocity < 0.1f)
        {
            velocity = 0.0f;
        }
        animator.SetFloat(VelocityHash, velocity, 0.1f, Time.deltaTime);

    }
}