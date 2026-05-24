using UnityEngine;

public class CharacterMovement2 : MonoBehaviour
{
    private Animator animator;
    private int velocityHash;
    private float _baseRotation;

    void Start()
    {
        animator = GetComponent<Animator>();
        velocityHash = Animator.StringToHash("Velocity");
    }

    public void ReceiveNetworkInput(float velocity, float rotation)
    {
        if (animator == null) return;
        animator.SetFloat(velocityHash, velocity);

        if (velocity <= 0f)
            _baseRotation = transform.eulerAngles.y;
        else
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, _baseRotation + rotation, transform.eulerAngles.z);
    }
}
