using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public GameObject auxiliar;
    public GameObject arrow;
    public QRScanner qrScanner; // Needed to prevent moving while relocating
    public Text message;

    private Vector3 prevPos; // Previous position in real-world
    private bool firstIt;    // True if first iteration
    private float prevHeading;


    // Start is called before the first frame update
    void Start()
    {
        // Initial position
        prevPos = Vector3.zero;

        firstIt = true;

        Input.location.Start();
        Input.compass.enabled = true;
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
            float currHeading = qrScanner.deltaOrientation + Frame.Pose.rotation.eulerAngles.y;

            if ( firstIt )
            {
                firstIt = false;
                prevPos = currPos;
                prevHeading = currHeading;
            }

            // Difference between positions
            Vector3 deltaPos = -(currPos - prevPos);
            float deltaHeading = - Mathf.DeltaAngle( prevHeading, currHeading );

            // Update previous position
            prevPos = currPos;
            prevHeading = currHeading;

            // Apply transform to Player (ignoring differences in height)
            float x = deltaPos.x * Mathf.Cos( qrScanner.deltaOrientation * Mathf.Deg2Rad ) + deltaPos.z * Mathf.Sin( qrScanner.deltaOrientation * Mathf.Deg2Rad );
            float z = deltaPos.z * Mathf.Cos( qrScanner.deltaOrientation * Mathf.Deg2Rad ) - deltaPos.x * Mathf.Sin( qrScanner.deltaOrientation * Mathf.Deg2Rad );
            player.transform.Translate( x, 0.0f, z );

            // Apply rotation to arrow indicator and auxiliar cube 
            arrow.transform.localRotation = Quaternion.Euler( new Vector3( 0, currHeading, 0 ) );
            auxiliar.transform.RotateAround( player.transform.position, new Vector3( 0, 0, 1 ), deltaHeading );
        }
    }
}
