using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}
