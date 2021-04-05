//#define DEBUG

using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZXing;


public class QRScanner : MonoBehaviour
{
    public GameObject calibrationPoints;    // objects with the transform of calibration points
    public GameObject player;               // player indicator
    public Text message;                    // message board

    private bool searchingForMarker;        // bool to say if looking for marker
    private bool initialized;               // bool to say if the map was initializad with first calibration point
    private bool first;                     // bool to fix multiple scan findings

    
    private void Start()
    {
        searchingForMarker = true;
        initialized = false;
        first = true;
        player.SetActive(false);    // hide player indicator until the first calibration
        message.text = "Please scan QR code to start";
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
            catch (NullReferenceException e)    //barcodeReader.Decode() throws NullReferenceException if no code was found
            {
#if DEBUG
                message.text = "NullReference";
#endif
                first = true;                   // allows relocating next time it finds a qr code
            }
            catch (Exception e)
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

        //Find the correct calibration point scanned and move the player to that position
        foreach (Transform child in calibrationPoints.transform)
        {
            if (child.name.Equals(text))
            {
                if (!initialized)
                {
                    player.SetActive(true);
                    initialized = true;
                }
                player.transform.position = child.position;
                message.text = "";
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

}
