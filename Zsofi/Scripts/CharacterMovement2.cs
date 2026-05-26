using UnityEngine;

public class CharacterMovement2 : MonoBehaviour
{
    public float moveSpeed = 100f;

    [SerializeField] private Animator animator;
    private int velocityHash;
    private float _currentVelocity;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogError($"[CharacterMovement2] No Animator found on {gameObject.name} or its children!");
        velocityHash = Animator.StringToHash("Velocity");
    }

    void Update()
    {
        if (_currentVelocity > 0f)
            transform.position += transform.forward * _currentVelocity * moveSpeed * Time.deltaTime;
    }

    public void ReceiveNetworkInput(float velocity, float rotation)
    {
        Debug.Log($"[CharacterMovement2] ReceiveNetworkInput on {gameObject.name}: vel={velocity} rot={rotation} animator={(animator != null ? animator.gameObject.name : "NULL")}");
        if (animator == null) return;
        if (velocity == 0)
        {
            _currentVelocity = 0f;
        }
        else
        {
            _currentVelocity = velocity + 4f;
        }

        animator.SetFloat(velocityHash, _currentVelocity);

        if (velocity > 0f)
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rotation, transform.eulerAngles.z);
    }
}
