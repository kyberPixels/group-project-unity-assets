using UnityEngine;
using UnityEngine.InputSystem;

public class ManoControllerScript : MonoBehaviour
{
    Animator animator;
    float velocity = 0.0f;
    float direction = 0.0f;

    float accelaration = 0.5f;
    float rotationSpeed = 1.2f;

    float deceleration = 0.5f;

    int VelocityHash;
    int DirectionHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        VelocityHash = Animator.StringToHash("Velocity X");
        DirectionHash = Animator.StringToHash("Velocity Z");

    }

    void Update()
    {
        bool forwardPressed = Keyboard.current.upArrowKey.isPressed;
        bool leftPressed = Keyboard.current.leftArrowKey.isPressed;
        bool rightPressed = Keyboard.current.rightArrowKey.isPressed;


        //bool runPressed = Keyboard.current.rightArrowKey.isPressed;

        if (forwardPressed && velocity < 2.0f)
        {
            velocity += Time.deltaTime * accelaration;
        }
        if (!forwardPressed && velocity > 0.0f)
        {
            velocity -= Time.deltaTime * deceleration;
        }
        if (forwardPressed && rightPressed && !leftPressed && direction > -2.0f)
        {
            direction -= Time.deltaTime * rotationSpeed;
        }
        if (forwardPressed && leftPressed && !rightPressed && direction < 2.0f)
        {
            direction += Time.deltaTime * rotationSpeed;
        }
        if (!forwardPressed && velocity < 0.1f)
        {
            velocity = 0.0f;
        }
        animator.SetFloat(VelocityHash, velocity, 0.1f, Time.deltaTime);
        animator.SetFloat(DirectionHash, direction, 0.1f, Time.deltaTime);

    }
}