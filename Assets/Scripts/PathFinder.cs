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
    public GameObject arrowCollider;
    public GameObject pinCollider;
    public Dropdown dropdown;
    public Text message; // message board

    private NavMeshPath path;
    private LineRenderer line;
    private List<Transform> destinations;
    private Transform targetDestination;


    void Start()
    {
        path = new NavMeshPath();
        line = GetComponent<LineRenderer>(); // get LineRenderer component on current object
        line.enabled = false;
        arrowCollider.SetActive( false );
        pinCollider.SetActive( false );
        destinations = new List<Transform>();
        Clear();
        dropdown.interactable = false;
        dropdown.onValueChanged.AddListener( delegate {
            DropdownIndexChanged();
        } );
    }


    void Update()
    {
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
        }
    }


    private void SetTargetDestination (int index)
    {
        targetDestination = destinations[ index ];

        arrowCollider.SetActive( true );
        arrowCollider.transform.position = player.position;
        pinCollider.SetActive( true );
        pinCollider.transform.position = targetDestination.position;
    }


    private void PopulateDropdown()
    {
        dropdown.ClearOptions();
        List<string> names = new List<string>();
        foreach(Transform dest in destinations)
        {
            names.Add( dest.name );
        }
        dropdown.AddOptions( names );
    }
    

    private void DropdownIndexChanged()
    {
        SetTargetDestination( dropdown.value );
    }

    
    public void Clear()
    {
        targetDestination = null;
        line.positionCount = 0;

        destinations.Clear();
        foreach ( Transform child in calibrationPoints.transform )
        {
            destinations.Add( child );
        }

        PopulateDropdown();
        dropdown.interactable = true;
    }
}
