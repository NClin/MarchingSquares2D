using System.Collections.Generic;
using UnityEngine;

public static class MarkerRegistry
{
    private static List<Marker> markers = new List<Marker>();

    public static void AddMarker(Marker marker)
    {
        markers.Add(marker);
    }

    public static void ClearMarkers()
    {
        markers.Clear();
    }

    public static Marker[] GetMarkers()
    {
        return markers.ToArray();
    }
}
