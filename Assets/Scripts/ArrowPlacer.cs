using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class ArrowPlacer : MonoBehaviour
{
    public Transform arcoreCamera;
    public LineRenderer line;
    public Text message;
    public GameObject arrowCollider;
    public GameObject pinCollider;
    public GameObject arrowPrefab;
    public GameObject pinPrefab;
    public Transform auxPos;

    private bool hasEnteredPinCollider;
    private Vector2 currPos;
    private float prevAngle = 0.0f;
    private bool first = true;  // first indication

    private GameObject spawnedArrow;
    private GameObject spawnedPin;
    private Anchor anchorArrow;
    private Anchor anchorPin;

    void Start()
    {
        hasEnteredPinCollider = false;
        currPos = new Vector2( this.transform.position.x, this.transform.position.z );
    }

    private void OnTriggerEnter( Collider collider )
    {
        if ( collider.tag.Equals( "Arrow" ) && !hasEnteredPinCollider )
        {
            if ( line.positionCount > 0 )
            {
                // Update player current position
                currPos.Set( this.transform.position.x, this.transform.position.y );

                // Position arrow a bit before the camera and a bit up
                Vector3 pos = arcoreCamera.transform.position + arcoreCamera.transform.forward * 2.0f + arcoreCamera.transform.up * 0.5f;
                // Rotate arrow to neutral orientation
                Quaternion rot = arcoreCamera.transform.rotation * Quaternion.Euler( 45, 180, 0 );
                // Create new anchor
                anchorArrow = Session.CreateAnchor( new Pose( pos, rot ) );
                // Spawn arrow
                spawnedArrow = GameObject.Instantiate( arrowPrefab, anchorArrow.transform.position, anchorArrow.transform.rotation, anchorArrow.transform );

                // Calculate and apply arrow angle
                Vector3 pathNodeAux = line.GetPosition( 1 );
                Vector2 pathNode = new Vector2( pathNodeAux.x, pathNodeAux.y );
                Vector2 auxNode = new Vector2( auxPos.position.x, auxPos.position.y );

                float angle = Mathf.Rad2Deg * ( Mathf.Atan2( auxNode.y - currPos.y, auxNode.x - currPos.x ) - Mathf.Atan2( pathNode.y - currPos.y, pathNode.x - currPos.x ) );
                spawnedArrow.transform.Rotate( 0, angle, 0, Space.Self );

                // Play sound indication
                angle = normalize( angle, -180, 180 );
                if ( angle < -45 )
                {
                    if ( prevAngle > -45 || first ) FindObjectOfType<AudioManager>().Play( "izquierda" );
                }
                else if ( angle > 45 )
                {
                    if ( prevAngle < 45 || first ) FindObjectOfType<AudioManager>().Play( "derecha" );
                }
                else
                {
                    if ( prevAngle < -45 || prevAngle > 45 || first ) FindObjectOfType<AudioManager>().Play( "continue" );
                }
                first = false;

                // Update previous angle
                prevAngle = angle;
            }
        }
        else if ( collider.tag.Equals( "Pin" ) )
        {
            // Position arrow a bit before the camera and a bit up
            Vector3 pos = arcoreCamera.transform.position + arcoreCamera.transform.forward * 3.0f + arcoreCamera.transform.up * 0.5f;
            // Rotation
            Quaternion rot = arcoreCamera.transform.rotation * Quaternion.Euler( 0, 0, 0 );
            // Create new anchor
            anchorPin = Session.CreateAnchor( new Pose( pos, rot ) );
            //Spawn pin
            spawnedPin = GameObject.Instantiate( pinPrefab, anchorPin.transform.position, anchorPin.transform.rotation, anchorPin.transform );
            // Destroy arrow
            hasEnteredPinCollider = true;
            Destroy( spawnedArrow );
            Destroy( anchorArrow );
            first = true;
            // Play sound indication
            FindObjectOfType<AudioManager>().Play( "destino" );
        }
    }

    private void OnTriggerExit( Collider collider )
    {
        if ( collider.tag.Equals( "Pin" ) )
        {
            Destroy( spawnedPin );
            Destroy( anchorPin );
            hasEnteredPinCollider = false;
        }
        else if ( collider.tag.Equals( "Arrow" ) )
        {
            arrowCollider.transform.position = this.transform.position;
            Destroy( spawnedArrow );
            Destroy( anchorArrow );
        }
    }

    // Normalizes any number to an arbitrary range 
    // by assuming the range wraps around when going below min or above max 
    private float normalize( float value, float start, float end ) 
    {
        float width = end - start;
        float offsetValue = value - start;   // value relative to 0

        return ((offsetValue - ( Mathf.Floor( offsetValue / width ) * width ) ) + start ); // + start to reset back to start of original range
    }


}
