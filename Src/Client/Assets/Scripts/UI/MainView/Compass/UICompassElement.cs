
using UnityEngine;

public class UICompassElement : MonoBehaviour
{

    public UICompassMarker CompassMarkerPrefab;
    public string TextDirection;
    UICompass compass;

    void Awake()
    {
        compass = FindObjectOfType<UICompass>();
        var markerInstance = Instantiate(CompassMarkerPrefab);
        markerInstance.Initialize(this, TextDirection);
        compass.RegisterCompassElement(transform, markerInstance);
    }

    void OnDestroy()
    {
        compass.UnregisterCompassElement(transform);
    }
}

