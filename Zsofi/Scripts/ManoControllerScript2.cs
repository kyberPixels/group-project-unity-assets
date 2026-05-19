using UnityEngine;
using UnityEngine.InputSystem;

public class manoControllerScript2 : MonoBehaviour
{
    Animator animator;
    float velocity = 0.0f;
    float direction = 0.0f;

    int VelocityHash;
    int DirectionHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        VelocityHash = Animator.StringToHash("Velocity");
        DirectionHash = Animator.StringToHash("Direction");

    }

    void Update()
    {
        animator.SetFloat(VelocityHash, velocity, 0.1f, Time.deltaTime);
        animator.SetFloat(DirectionHash, direction, 0.1f, Time.deltaTime);

    }
}