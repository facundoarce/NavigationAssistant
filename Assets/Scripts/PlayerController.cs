using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public QRScanner qrScanner; // Needed to prevent moving while relocating
    public Text message;

    private Vector3 prevPos; // Previous position in real-world
    private bool firstIt;    // True if first iteration

    // Start is called before the first frame update
    void Start()
    {
        // Initial position
        prevPos = Vector3.zero;

        firstIt = true;
    }

    // Update is called once per frame
    void Update()
    {
        if ( qrScanner.IsRelocating() )
        {
#if DEBUG
            message.text = "Relocating";
#endif
        }
        else
        {
#if DEBUG
            message.text = "NOT relocating";
#endif
            // Current position in real-world
            Vector3 currPos = Frame.Pose.position;

            if ( firstIt )
            {
                firstIt = false;
                prevPos = currPos;
            }

            // Difference between positions
            Vector3 deltaPos = currPos - prevPos;

            // Update previous position
            prevPos = currPos;

            // Apply transform to Player (ignoring differences in height)
            player.transform.Translate( deltaPos.x, 0.0f, deltaPos.z );
        }
    }
}
