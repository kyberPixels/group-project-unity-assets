using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Profiles")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Animator animator;
    
    private Vector3 movementInput;
    private int velocityHash;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        velocityHash = Animator.StringToHash("Velocity");

        // Safety verification check for Phase 1 constraints
        if (rb != null)
        {
            rb.freezeRotation = true; 
        }
    }

    void Update()
    {
        // 1. DUAL TESTING INPUT: Listens to laptop WASD / Arrow Keys right now!
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)    moveZ = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)  moveZ = -1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;

        movementInput = new Vector3(moveX, 0f, moveZ).normalized;

        // 2. ANIMATION SYNC: Tell your animator controller to cycle state trees
        if (animator != null)
        {
            float currentVelocityValue = movementInput.magnitude * 2.0f; // Scale matching your float parameters
            animator.SetFloat(velocityHash, currentVelocityValue, 0.1f, Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // 3. PHYSICAL MOVEMENT: Moves the Rigidbody smoothly through environmental colliders
        if (movementInput.magnitude >= 0.1f)
        {
            // Compute step location vector
            Vector3 targetPosition = rb.position + movementInput * moveSpeed * FixedUpdateCalculationDelta();
            rb.MovePosition(targetPosition);

            // Compute turn angle rotation vector
            Quaternion targetRotation = Quaternion.LookRotation(movementInput);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * FixedUpdateCalculationDelta()));
        }
    }

    private float FixedUpdateCalculationDelta()
    {
        return Time.fixedDeltaTime;
    }

    
    /// Sofi, call this function from your network script later 
    /// when parsing incoming mobile joystick float packages.
    
    public void ReceiveNetworkJoystickInput(float x, float z)
    {
        movementInput = new Vector3(x, 0f, z).normalized;
    }
}