using UnityEngine;

public class Marker : MonoBehaviour
{
    public Vector2 position;
    void Start()
    {
        position = transform.position;
        MarkerRegistry.AddMarker(this);
    }

}
