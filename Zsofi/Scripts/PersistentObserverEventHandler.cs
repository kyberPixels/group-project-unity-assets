using UnityEngine;

// Drop this on ImageTargetEnv instead of DefaultObserverEventHandler.
// Content stays visible after the marker is first found, even if the marker leaves the camera.
public class PersistentObserverEventHandler : DefaultObserverEventHandler
{
    protected override void OnTrackingLost()
    {
        OnTargetLost?.Invoke();
    }
}
