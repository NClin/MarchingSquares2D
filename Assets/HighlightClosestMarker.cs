using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightClosestMarker : MonoBehaviour
{
    private Marker knownClosest;
    
    
    [SerializeField] private Marker dummyMarker;
    
    // Update is called once per frame
    void Update()
    {
        Marker[] markers = MarkerRegistry.GetMarkers();
        float minDist = float.MaxValue;
        Marker nearestMarker = dummyMarker;
        
        foreach (Marker marker in markers)
        {
            float dist = Vector2.Distance(transform.position, marker.position);
            if (dist < minDist)
            {
                nearestMarker = marker;
                minDist = dist;
            }
        }

        if (nearestMarker != knownClosest)
        {
            SetNewClosest(nearestMarker);
        }
        
        
    }

    void SetNewClosest(Marker marker)
    {
        if (knownClosest != null)
        {
            knownClosest.GetComponent<SpriteRenderer>().color = Color.red;
        }

        marker.GetComponent<SpriteRenderer>().color = Color.yellow;
        knownClosest = marker;
    }
}
