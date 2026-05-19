using UnityEngine;

public class LockCameraTransform : MonoBehaviour
{
    private Vector3 _lockedPosition;
    private Quaternion _lockedRotation;

    void Start()
    {
        _lockedPosition = transform.position;
        _lockedRotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.SetPositionAndRotation(_lockedPosition, _lockedRotation);
    }
}
