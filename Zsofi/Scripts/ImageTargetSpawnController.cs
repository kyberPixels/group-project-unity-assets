using UnityEngine;
using Vuforia;

public class ImageTargetSpawnController : MonoBehaviour
{
    [SerializeField] private GameObject character;
    [SerializeField] private bool hideWhenLost = false;

    void Start()
    {
        var handler = GetComponent<DefaultObserverEventHandler>();
        if (handler == null) return;

        handler.OnTargetFound.AddListener(OnFound);
        if (hideWhenLost)
            handler.OnTargetLost.AddListener(OnLost);
    }

    void OnFound() => character?.SetActive(true);
    void OnLost()  => character?.SetActive(false);
}
