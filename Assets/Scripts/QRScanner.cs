//#define DEBUG

using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using ZXing;


public class QRScanner : MonoBehaviour
{
    public GameObject calibrationPoints;    // objects with the transform of calibration points
    public GameObject player;               // player indicator
    public GameObject mapUI;                // map UI to be hidden until first calibration
    public Text message;                    // message board
    public NavMeshSurface surface;
    public float deltaOrientation;

    private bool searchingForMarker;        // bool to say if looking for marker
    private bool initialized;               // bool to say if the map was initializad with first calibration point
    private bool first;                     // bool to fix multiple scan findings
    private bool isRelocating;


    private void Start()
    {
        searchingForMarker = true;
        initialized = false;
        first = true;
        isRelocating = false;
        mapUI.SetActive(false);    // hide map UI indicator until the first calibration
        deltaOrientation = 0.0f;
        // message.text = "Please scan QR code to start"; // moved to MapDownloadMenu
    }


    void Update()
    {
        if (searchingForMarker)
        {
            Scan();
        }
    }


    /// Capture and scan the current frame
    void Scan()
    {
        System.Action<byte[], int, int> callback = (bytes, width, height) =>
        {
            if (bytes == null)
            {
                // No image is available.
                return;
            }

            // Decode the image using ZXing parser
            try
            {
                IBarcodeReader barcodeReader = new BarcodeReader();
                var result = barcodeReader.Decode(bytes, width, height, RGBLuminanceSource.BitmapFormat.Gray8);
                string resultText = result.Text;
                
                // Relocate player
                if (first)
                {
                    Relocate(resultText);
#if DEBUG
                    message.text = resultText;
#endif
                    first = false;              // avoids relocating on every Update() to the same calibration point
                }
            }
            catch (NullReferenceException)    //barcodeReader.Decode() throws NullReferenceException if no code was found
            {
#if DEBUG
                message.text = "NullReference";
#endif
                first = true;                   // allows relocating next time it finds a qr code
            }
            catch (Exception)
            {
#if DEBUG
                message.text = "Unknown exception";
#endif
            }
        };

        CaptureScreenAsync(callback);
    }


    // Move the player indicator to the calibration point
    private void Relocate(string text)
    {
        text = text.Trim(); //remove spaces
        //message.text = text;

        //Find the correct calibration point scanned and move the player to that position
        foreach (Transform child in calibrationPoints.transform)
        {
            //message.text = child?.name;
            if ( child.name == text )
            {
                isRelocating = true;

                if (!initialized)
                {
                    mapUI.SetActive(true);  // show map UI
                    initialized = true;
                    surface.BuildNavMesh();
                }

                // player.transform.position = child.position; // doesn't work properly with Navmesh Agent
                player.GetComponent<NavMeshAgent>().Warp( child.position );
                //player.transform.rotation = child.rotation;
                //deltaOrientation = Mathf.DeltaAngle( deltaOrientation, Mathf.DeltaAngle( Frame.Pose.rotation.eulerAngles.y, child.localRotation.eulerAngles.y ) );
                deltaOrientation = Mathf.DeltaAngle( Frame.Pose.rotation.eulerAngles.y, child.localRotation.eulerAngles.y );

                message.text = "";
                isRelocating = false;
            }
        }
    }

    
    // Capture the screen using CameraImage.AcquireCameraImageBytes.
    void CaptureScreenAsync(Action<byte[], int, int> callback)
    {
        Task.Run(() =>
        {
            byte[] imageByteArray = null;
            int width;
            int height;

            using (var imageBytes = Frame.CameraImage.AcquireCameraImageBytes())
            {
                if (!imageBytes.IsAvailable)
                {
                    callback(null, 0, 0);
                    return;
                }

                int bufferSize = imageBytes.YRowStride * imageBytes.Height;

                imageByteArray = new byte[bufferSize];

                Marshal.Copy(imageBytes.Y, imageByteArray, 0, bufferSize);

                width = imageBytes.Width;
                height = imageBytes.Height;
            }

            callback(imageByteArray, width, height);
        });
    }

    public bool IsRelocating()
    {
        return isRelocating;
    }

}
