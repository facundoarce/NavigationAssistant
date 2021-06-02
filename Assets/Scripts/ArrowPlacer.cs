﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class ArrowPlacer : MonoBehaviour
{
    public Transform arcoreCamera;
    public LineRenderer line;
    public Text message;
    //public Transform player;
    public GameObject arrowCollider;
    public GameObject pinCollider;
    public GameObject arrowPrefab;
    public GameObject pinPrefab;
    public Transform auxPos;

    private bool hasEntered;
    private bool hasExited;
    private Vector2 currPos;
    private Vector2 prevPos;

    private GameObject spawned;
    private Anchor anchor;

    void Start()
    {
        hasEntered = false;
        hasExited = false;
        currPos = new Vector2( this.transform.position.x, this.transform.position.z );
        prevPos = currPos;
    }

    void Update()
    {
        hasEntered = false;
        hasExited = false;
        //prevPos = currPos;
        //currPos.Set( this.transform.position.x, this.transform.position.z );
    }

    // private void OnTriggerEnter( Collider collider )
    // private void OnTriggerEnter( Collider collider )
    private void OnTriggerEnter( Collider collider )
    {
        if ( line.positionCount > 0 )
        {
            if ( collider.tag.Equals( "Arrow" ) )
            {
                prevPos = currPos;
                //currPos.Set( this.transform.position.x, this.transform.position.z );
                currPos.Set( this.transform.position.x, this.transform.position.y );

                message.text = "Entered arrow collider";

                // Position arrow a bit before the camera and a bit up
                Vector3 pos = arcoreCamera.transform.position + arcoreCamera.transform.forward * 2.0f + arcoreCamera.transform.up * 0.5f;
                // Rotate arrow to neutral orientation
                Quaternion rot = arcoreCamera.transform.rotation * Quaternion.Euler( 45, 180, 0 );
                // Create new anchor
                anchor = Session.CreateAnchor( new Pose( pos, rot ) );
                //Spawn arrow
                spawned = GameObject.Instantiate( arrowPrefab, anchor.transform.position, anchor.transform.rotation, anchor.transform );

                // Calculate arrow angle
                Vector3 pathNodeAux = line.GetPosition( 1 );
                //Vector2 pathNode = new Vector2( pathNodeAux.x, pathNodeAux.z );
                Vector2 pathNode = new Vector2( pathNodeAux.x, pathNodeAux.y );

                //Vector2 auxNode = new Vector2( auxPos.position.x, auxPos.position.z );
                Vector2 auxNode = new Vector2( auxPos.position.x, auxPos.position.y );

                //float angle = Mathf.Rad2Deg * ( Mathf.Atan2( pathNode.y - currPos.y, pathNode.x - currPos.x ) );
                //message.text = string.Format( "{0} deg", angle );

                // Law of cosines https://www.mathsisfun.com/algebra/trig-solving-sss-triangles.html
                //float a = Vector2.Distance( currPos, pathNode );
                //float b = Vector2.Distance( currPos, prevPos );
                //float c = Vector2.Distance( pathNode, prevPos );
                //float angle = Mathf.Rad2Deg * ( Mathf.Acos( ( b * b + c * c - a * a ) / ( 2.0f * b * c ) ) );

                //// Law of cosines https://www.mathsisfun.com/algebra/trig-solving-sss-triangles.html
                //float a = Vector2.Distance( auxNode, pathNode );
                //float b = Vector2.Distance( currPos, auxNode );
                //float c = Vector2.Distance( currPos, pathNode );
                //float angle = Mathf.Rad2Deg * ( Mathf.Acos( ( b * b + c * c - a * a ) / ( 2.0f * b * c ) ) );

                // float angle = Mathf.Rad2Deg * ( Mathf.Atan2( pathNode.y - currPos.y, pathNode.x - currPos.x ) );

                float angle = Mathf.Rad2Deg * ( Mathf.Atan2( auxNode.y - currPos.y, auxNode.x - currPos.x ) - Mathf.Atan2( pathNode.y - currPos.y, pathNode.x - currPos.x ) );


                //float angle = Mathf.Rad2Deg * ( Mathf.Atan2( Vector2.Distance( pathNode, currPos ), Vector2.Distance( prevPos, currPos ) ) );

                // Apply calculated angle
                spawned.transform.Rotate( 0, angle, 0, Space.Self );

                message.text = string.Format("Angle {0} deg", angle );
            }
            else if ( collider.tag.Equals( "Pin" ) )
            {
                message.text = "Entered pin collider";
            }
            else
            {
                //message.text = string.Format( "Entered {0} collider", collider.name );
                return;
            }
        }
    }

    //private void OnCollisionEnter( Collision collision )
    //{
    //    if (collision.gameObject.tag.Equals("Arrow"))
    //    {
    //        message.text = "Entered arrow collider";
    //    }
    //    else if ( collision.gameObject.tag.Equals( "Pin" ) )
    //    {
    //        message.text = "Entered pin collider";
    //    }
    //    else
    //    {
    //        message.text = string.Format( "Entered {0} collider", collision.gameObject.name );
    //    }
    //}

    // private void OnTriggerExit( Collider collider )

    //private void OnCollisionExit( Collision collision )
    //{
    //    if ( collision.gameObject.tag.Equals( "Arrow" ) )
    //    {
    //        message.text = "Exit arrow collider";
    //    }
    //    else if ( collision.gameObject.tag.Equals( "Pin" ) )
    //    {
    //        message.text = "Exit pin collider";
    //    }
    //    else
    //    {
    //        message.text = string.Format( "Exit {0} collider", collision.gameObject.name );
    //    }
    //}

    private void OnTriggerExit( Collider collider )
    {
        if ( collider.tag.Equals( "Arrow" ) )
        {
            //message.text = "Exit arrow collider";
            arrowCollider.transform.position = this.transform.position;
            Destroy( spawned );
            Destroy( anchor );
        }
        else if ( collider.tag.Equals( "Pin" ) )
        {
            message.text = "Exit pin collider";
        }
        else
        {
            message.text = string.Format( "Exit {0} collider", collider.name );
        }
    }


}