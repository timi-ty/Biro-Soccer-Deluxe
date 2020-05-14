using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMarkers : MonoBehaviour
{
    public List<Transform> FFAMarkers;
    public List<Transform> TEAM1Markers;
    public List<Transform> TEAM2Markers;
    void Start()
    {
        for(int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            FFAMarkers.Add(transform.GetChild(0).GetChild(i));
        }

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            TEAM1Markers.Add(transform.GetChild(1).GetChild(i));
        }

        for (int i = 0; i < transform.GetChild(2).childCount; i++)
        {
            TEAM2Markers.Add(transform.GetChild(2).GetChild(i));
        }
    }
}
