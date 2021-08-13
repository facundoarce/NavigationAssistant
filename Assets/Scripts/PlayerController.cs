using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public GameObject auxiliar;
    public QRScanner qrScanner; // Needed to prevent moving while relocating
    public Text message;

    private Vector3 prevPos; // Previous position in real-world
    private bool firstIt;    // True if first iteration
    private float prevHeading;

    //public Camera arcoreCamera;

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
            //firstIt = true;
        }
        else
        {
#if DEBUG
            message.text = "NOT relocating";
#endif
            // Current position in real-world
            Vector3 currPos = Frame.Pose.position;
            //float currHeading = Frame.Pose.rotation.eulerAngles.y;
            float currHeading = qrScanner.deltaOrientation + Frame.Pose.rotation.eulerAngles.y;

            // float currHeading = Input.compass.headingAccuracy < 0.0f ? 0.0f : Input.compass.trueHeading;

            if ( firstIt )
            {
                firstIt = false;
                prevPos = currPos;
                prevHeading = currHeading;
                //auxiliar.transform.RotateAround( player.transform.position, new Vector3( 0, 0, 1 ), qrScanner.deltaOrientation );

                //auxiliar.transform.localRotation = Quaternion.Euler( new Vector3( 0, qrScanner.deltaOrientation, 0 ) );
            }

            // Difference between positions
            Vector3 deltaPos = -(currPos - prevPos);
            float deltaHeading = - Mathf.DeltaAngle( prevHeading, currHeading );

            // Update previous position
            prevPos = currPos;
            prevHeading = currHeading;

            // Apply transform to Player (ignoring differences in height)
            //player.transform.Translate( deltaPos.x, 0.0f, deltaPos.z );
            float x = deltaPos.x * Mathf.Cos( qrScanner.deltaOrientation * Mathf.Deg2Rad ) + deltaPos.z * Mathf.Sin( qrScanner.deltaOrientation * Mathf.Deg2Rad );
            float z = deltaPos.z * Mathf.Cos( qrScanner.deltaOrientation * Mathf.Deg2Rad ) - deltaPos.x * Mathf.Sin( qrScanner.deltaOrientation * Mathf.Deg2Rad );
            player.transform.Translate( x, 0.0f, z );

            
            auxiliar.transform.RotateAround( player.transform.position, new Vector3( 0, 0, 1 ), deltaHeading ); //public void RotateAround(Vector3 point, Vector3 axis, float angle);

            //message.text = string.Format( "{0} deg", Frame.Pose.rotation.eulerAngles.y ); // NOOOOOOOOOOOO
            //message.text = string.Format( "{0} deg", Input.compass.trueHeading ); // SIIIIIIIIIIIIIIII 
            //message.text = string.Format( "{0} / {1}", currHeading, Input.compass.headingAccuracy );

            //message.text = string.Format( "{0} deg", currHeading );
            //message.text = string.Format( "{0} deg", qrScanner.deltaOrientation );

            //arcoreCamera.GetComponent<FollowTarget>().targetRot = Frame.Pose.rotation;
        }
    }
}
