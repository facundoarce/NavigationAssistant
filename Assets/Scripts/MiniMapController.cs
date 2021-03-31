using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class MiniMapController : MonoBehaviour
{
    public Camera FirstPersonCamera;
    public GameObject Player;

    private Vector3 PrevPos; // Previous position in real-world
    private bool FirstIt;    // True if first iteration

    // Start is called before the first frame update
    void Start()
    {
        // Initial position
        PrevPos = Vector3.zero;

        FirstIt = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Current position in real-world
        Vector3 CurrPos = Frame.Pose.position;

        if (FirstIt)
        {
            FirstIt = false;
            PrevPos = CurrPos;
        }

        // Difference between positions
        Vector3 DeltaPos = CurrPos - PrevPos;

        // Update previous position
        PrevPos = CurrPos;

        // Apply transform to Player (ignoring differences in height)
        Player.transform.Translate(DeltaPos.x, 0.0f, DeltaPos.z);
    }
}
