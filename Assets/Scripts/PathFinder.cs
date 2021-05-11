using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class PathFinder : MonoBehaviour
{
    public Transform player;
    public GameObject calibrationPoints;
    public Dropdown dropdown;
    public Text message;                    // message board

    private NavMeshPath path;
    private LineRenderer line;
    private List<Transform> destinations;
    private Transform targetDestination;


    void Start()
    {
        path = new NavMeshPath();
        line = GetComponent<LineRenderer>(); // get LineRenderer component on current object
        line.enabled = false;
        destinations = new List<Transform>();
        Clear();
        dropdown.onValueChanged.AddListener( delegate {
            DropdownIndexChanged();
        } );
    }


    void Update()
    {
        //message.text = string.Format( "Size dest {0}", destinations.Count );
        if (targetDestination != null)
        {
            try
            {
                NavMesh.CalculatePath( player.position, targetDestination.position, NavMesh.AllAreas, path );
            }
            catch(Exception e)
            {
                message.text = e.GetType().Name;
            }
            
            line.positionCount = path.corners.Length;
            line.SetPositions( path.corners ); 
            line.enabled = true;
            // message.text = string.Format( "PosCount {0}", line.positionCount );
        }
    }


    private void SetTargetDestination (int index)
    {
        targetDestination = destinations[ index ];
    }


    private void PopulateDropdown()
    {
        List<string> names = new List<string>();
        foreach(Transform dest in destinations)
        {
            names.Add( dest.name );
        }
        dropdown.AddOptions( names );
    }


    private void DropdownIndexChanged()
    {
        // message.text = string.Format( "DropdownIndexChanged {0}", dropdown.value );
        SetTargetDestination( dropdown.value );
    }

    
    public void Clear()
    {
        targetDestination = null;
        line.positionCount = 0;

        foreach ( Transform child in calibrationPoints.transform )
        {
            destinations.Add( child );
        }

        dropdown.ClearOptions();
        PopulateDropdown();
    }
}
