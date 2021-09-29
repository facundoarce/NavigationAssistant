using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions; // for Regex
using UnityEngine;
using UnityEngine.UI;
using TechTweaking.Bluetooth;
using TMPro;
using System;

public class MapDownloadMenu : MonoBehaviour
{
    /// <summary>
    /// Map download menu UI for selecting device to download map from.
    /// </summary>
    [SerializeField]
    private GameObject _mapDownloadMenuUi = null;

    /// <summary>
    /// The button to select BT device.
    /// </summary>
    [SerializeField]
    private Button _deviceButton = null;

    /// <summary>
    /// The button to apply the config and close the menu window.
    /// </summary>
    [SerializeField]
    private Button _applyButton = null;

    /// <summary>
    /// The button to cancel the changes and close the menu window.
    /// </summary>
    [SerializeField]
    private Button _cancelButton = null;

    /// <summary>
    /// Text object for selected device.
    /// </summary>
    public Text devNameText;

    /// <summary>
    /// Map download process UI.
    /// </summary>
    [SerializeField]
    private GameObject _mapDownloadProcessUi = null;

    /// <summary>
    /// Text object for download process.
    /// </summary>
    [SerializeField]
    private Text _processText;

    /// <summary>
    /// The button to accept and close the process UI.
    /// </summary>
    [SerializeField]
    private Button _okButton = null;

    /// <summary>
    /// Text object for message showing / debugging.
    /// </summary>
    [SerializeField]
    private Text message;

    /// <summary>
    /// Parent transform for calibration coordinates.
    /// </summary>
    [SerializeField]
    private Transform parent;

    /// <summary>
    /// Parent transform for calibration coordinates labes.
    /// </summary>
    [SerializeField]
    private Transform labelParent;

    /// <summary>
    /// TextMeshPro Prefab for calibration coordinates labes.
    /// </summary>
    [SerializeField]
    private GameObject labelPrefab;

    /// <summary>
    /// Path finder script to populate dropdown list with calibration coordinates.
    /// </summary>
    [SerializeField]
    private PathFinder pathFinder;

    /// <summary>
    /// Bluetooth device object that download the map from.
    /// </summary>
    private BluetoothDevice device;
    
    
    void Start()
    {
        _mapDownloadProcessUi.SetActive( false );
        _mapDownloadMenuUi.SetActive( false );

        _applyButton.onClick.AddListener( OnApplyButtonClicked );
        _cancelButton.onClick.AddListener( OnCancelButtonClicked );
        _deviceButton.onClick.AddListener( OnSelectButtonClicked );
        _okButton.onClick.AddListener( OnOkButtonClicked );
        _applyButton.interactable = false;

        BluetoothAdapter.OnDeviceOFF += HandleOnDeviceOff;
        BluetoothAdapter.OnDevicePicked += HandleOnDevicePicked; //To get what device the user picked out of the devices list

        message.text = "Please download map\nClick on settings button";
    }
    
    //############### UI BUTTONS RELATED METHODS #####################

    /// <summary>
    /// Callback event when the map download menu button (in setting UI) is clicked.
    /// </summary>
    public void OnMenuButtonClicked()
    {
    }

    /// <summary>
    /// Callback event when the select device button (in map download UI) is clicked.
    /// </summary>
    public void OnSelectButtonClicked()
    {
        BluetoothAdapter.askEnableBluetooth(); // ask user to enable Bluetooth
        BluetoothAdapter.showDevices(); // show a list of all devices // any picked device will be sent to this.HandleOnDevicePicked()
    }

    /// <summary>
    /// Callback event when the connect button (in map download UI) is clicked.
    /// </summary>
    public void OnApplyButtonClicked() 
    {
        //Switch to Process UI
        _mapDownloadProcessUi.SetActive( true );
        _mapDownloadMenuUi.SetActive( false );

        connect();
        StartCoroutine( getCalibrationPoints() );
    }

    /// <summary>
    /// Callback event when the cancel button (in map download UI) is clicked.
    /// </summary>
    public void OnCancelButtonClicked()
    {
        _mapDownloadMenuUi.SetActive( false );
    }

    /// <summary>
    /// Callback event when the ok button (in map download process UI) is clicked.
    /// </summary>
    public void OnOkButtonClicked()
    {
        // Close windows
        _mapDownloadProcessUi.SetActive( false );
        _mapDownloadMenuUi.SetActive( false );

        // Disconnect device
        disconnect();
    }

    //############### BLUETOOTH COMMUNICATION MEHTODS #####################

    /// <summary>
    /// Called when device cannot connect
    /// </summary>
    void HandleOnDeviceOff( BluetoothDevice dev )
    {
        if ( !string.IsNullOrEmpty( dev.Name ) )
            _processText.text = "Couldn't connect to " + dev.Name + ", device might be OFF\n";
        else if ( !string.IsNullOrEmpty( dev.MacAddress ) )
        {
            _processText.text = "Couldn't connect to " + dev.MacAddress + ", device might be OFF\n";
        }
    }

    /// <summary>
    /// Called when device is picked by user
    /// </summary>
    void HandleOnDevicePicked( BluetoothDevice dev )
    {
        this.device = dev; //Save a global reference to the device

        //Here we assign the 'Coroutine' that will handle your reading Functionality, this will improve your code style
        //Another way to achieve this would be listening to the event Bt.OnReadingStarted, and starting the courotine from there by yourself.
        dev.ReadingCoroutine = ManageConnection;

        devNameText.text = "Device: " + dev.Name;

        _applyButton.interactable = true;
    }

    /// <summary>
    /// Connec to bluetooth device
    /// </summary>
    void connect() // Connect to the public global variable "device" if it's not null.
    {
        if ( device != null )
        {
            device.connect();
            _processText.text = "Trying to connect...\n";
        }
    }

    IEnumerator getCalibrationPoints()
    {
        yield return new WaitForSeconds( 3.0f );
        send( "Trying" );
        do
        {
            send( "Hello" );
            yield return new WaitForSeconds( 2.0f );
        } while ( !_processText.text.Contains( "Done" ) );

        // Clean all calibration points
        List<GameObject> children = new List<GameObject>();
        foreach ( Transform child in parent ) children.Add( child.gameObject );
        children.ForEach( child => Destroy( child ) );

        // Message parsing
        string pattern1 = @"\[([^\[\]]+)\]";  // Separate calibration points from rest of message
        string pattern2 = @"[^,;\[\]\n\r]+";  // Split each calibration point into name and coordinates

        foreach ( Match m1 in Regex.Matches( _processText.text, pattern1 ) )
        {
            Match mName = Regex.Match( m1.Groups[ 0 ].Value, pattern2 );
            Match mX = mName.NextMatch();
            Match mZ = mX.NextMatch();
            Match mRY = mZ.NextMatch();

            // Check if point is repeated
            bool isNameRepeated = false;
            foreach ( Transform child in parent )
            {
                isNameRepeated |= child.gameObject.name == mName.Groups[ 0 ].Value;
            }

            if ( !isNameRepeated )
            {
                // Add calibration points
                GameObject calibPoint = new GameObject( mName.Groups[ 0 ].Value );
                calibPoint.transform.parent = parent;
                calibPoint.transform.name = mName.Groups[ 0 ].Value;
                calibPoint.transform.localPosition = new Vector3( float.Parse( mX.Groups[ 0 ].Value ), 0.0f, float.Parse( mZ.Groups[ 0 ].Value ) );
                calibPoint.transform.localEulerAngles = new Vector3( 0.0f, float.Parse( mRY.Groups[ 0 ].Value ), 0.0f );

                // Add labels in minimap
                GameObject label = GameObject.Instantiate( labelPrefab, labelParent );
                label.transform.localPosition += calibPoint.transform.localPosition;
                label.GetComponent<TextMeshPro>().SetText( calibPoint.transform.name );
            }
        }

        pathFinder.Clear(); // populate dropdown with calibration points
        message.text = "Please scan QR code to start";
    }

    /// <summary>
    /// Disconnect to bluetooth device
    /// </summary>
    void disconnect() // Disconnect the public global variable "device" if it's not null.
    {
        if ( device != null )
            device.close();
    }

    /// <summary>
    /// Send a message to bluetooth device
    /// </summary>
    void send( string msg )
    {
        if ( device != null && !string.IsNullOrEmpty( msg ) )
        {
            device.send( System.Text.Encoding.ASCII.GetBytes( msg ) );
        }
    }

    //############### Reading Data  #####################
    //Please note that you don't have to use Couroutienes, you can just put your code in the Update() method. Like what we did in the BasicDemo
    IEnumerator ManageConnection( BluetoothDevice device )
    {//Manage Reading Coroutine
        while ( device.IsReading )
        {
            byte[] msg = device.read();
            if ( msg != null )
            {
                // Convert byte array to string.
                string content = System.Text.Encoding.UTF8.GetString( msg );

                // Split the string into lines. '\n','\r' are charachter used to represent new line.
                string[] lines = content.Split( new char[] { '\n', '\r' } );

                // Add those lines to the processText
                _processText.text += content;

                /* Note: You should notice the possiblity that at the time of calling read() a whole line has not yet completly recieved.
                 * This will split a line into two or more lines between consecutive read() calls. This is not hard to fix, but the goal here is to keep the code simple.
                 * To see a solution using methods of this library check out the "High Bit Rate demo". 
                 */
            }
            yield return null;
        }
    }

    /// <summary>
    /// Unity's OnDestroy() method.
    /// </summary>
    public void OnDestroy()
    {
        _applyButton.onClick.RemoveListener( OnApplyButtonClicked );
        _cancelButton.onClick.RemoveListener( OnCancelButtonClicked );
        _deviceButton.onClick.RemoveListener( OnSelectButtonClicked );
        _okButton.onClick.RemoveListener( OnOkButtonClicked );

        BluetoothAdapter.OnDevicePicked -= HandleOnDevicePicked;
        BluetoothAdapter.OnDeviceOFF -= HandleOnDeviceOff;
    }
}
