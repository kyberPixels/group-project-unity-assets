using UnityEngine;

public class CharacterMovement2 : MonoBehaviour
{
    public float moveSpeed = 100f;

    private Animator animator;
    private Rigidbody rb; // Added for physical wall collisions
    private int velocityHash;
    private float _currentVelocity;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); // Initialize Rigidbody
        velocityHash = Animator.StringToHash("Velocity");

        // Safety check to ensure physics constraints are locked
        if (rb != null)
        {
            rb.freezeRotation = true; // Prevents the model from falling over like a bowling pin
        }
    }

    // Use FixedUpdate for Rigidbody physical movement instead of regular Update!
    void FixedUpdate()
    {
        if (_currentVelocity > 0f)
        {
            // Calculate direction vector based on where the character is facing
            Vector3 moveDirection = transform.forward * _currentVelocity * moveSpeed * Time.fixedDeltaTime;

            // Move via physics calculation. This respects Box Colliders perfectly!
            rb.MovePosition(rb.position + moveDirection);
        }
        else
        {
            // Stop physical sliding when input is 0, but keep gravity acting on the Y axis
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    public void ReceiveNetworkInput(float velocity, float rotation)
    {
        if (animator == null) return;

        if (velocity == 0)
        {
            _currentVelocity = 0f;
        }
        else
        {
            _currentVelocity = velocity + 4f;
        }

        animator.SetFloat(velocityHash, velocity);

        if (velocity > 0f)
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rotation, transform.eulerAngles.z);
    }
}