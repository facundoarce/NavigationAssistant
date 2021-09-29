using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI; // for NavMeshAgent
using GoogleARCore.Examples.Common; // for DepthMenu

public class SettingsController : MonoBehaviour
{
    [Header( "Common Settings" )]

    /// <summary>
    /// Scene object that contains all setting menu UI elements.
    /// </summary>
    /// 
    [SerializeField]
    private GameObject _settingsWindows = null;

    /// <summary>
    /// Setting menu that contains options for different features.
    /// </summary>
    [SerializeField]
    private GameObject _settingMenuUi = null;

    /// <summary>
    /// Setting button for opening menu windows.
    /// </summary>
    [SerializeField]
    private Button _settingButton = null;

    [Header( "Depth" )]

    /// <summary>
    /// Depth menu UI.
    /// </summary>
    [SerializeField]
    private GameObject _depthMenuUi = null;

    /// <summary>
    /// The button to open depth menu.
    /// </summary>
    [SerializeField]
    private Button _depthButton = null;

    /// <summary>
    /// The component to control depth menu.
    /// </summary>
    [SerializeField]
    private DepthMenu _depthMenu = null;

    [Header( "Map Download" )]

    /// <summary>
    /// Map download menu for selecting device to download map from.
    /// </summary>
    [SerializeField]
    private GameObject _mapDownloadMenuUi = null;

    /// <summary>
    /// The button to open map download menu.
    /// </summary>
    [SerializeField]
    private Button _mapDownloadButton = null;

    /// <summary>
    /// The component to control map dowload menu.
    /// </summary>
    [SerializeField]
    private MapDownloadMenu _mapDownloadMenu = null;

    [Header( "Enable Wall Collisions" )]

    /// <summary>
    /// The toggle to enable wall collisions.
    /// </summary>
    [SerializeField]
    private Toggle _enableCollisionsToggle = null;

    /// <summary>
    /// The toggle to enable wall collisions.
    /// </summary>
    [SerializeField]
    private GameObject _player = null;


    // Start is called before the first frame update
    void Start()
    {
        _settingsWindows.SetActive( false );
        _settingMenuUi.SetActive( false );
        _settingButton.onClick.AddListener( OnMenuButtonClick );

        _mapDownloadMenuUi.SetActive( false );
        _mapDownloadButton.onClick.AddListener( OnMapDownloadClick );

        _depthMenuUi.SetActive( false );
        _depthButton.onClick.AddListener( OnDepthClick );

        _enableCollisionsToggle.isOn = false;
        _player.GetComponent<NavMeshAgent>().enabled = false;
        _enableCollisionsToggle.onValueChanged.AddListener( delegate { OnCollisionsToggle(); } );
    }


    /// <summary>
    /// Callback event for setting button.
    /// </summary>
    void OnMenuButtonClick()
    {
        if ( _settingsWindows.activeSelf )
        {
            OnMenuClosed();
        }
        else
        {
            _settingsWindows.SetActive( true );
            _settingMenuUi.SetActive( true );
        }
    }

    /// <summary>
    /// Callback event for opening map download menu.
    /// </summary>
    void OnMapDownloadClick()
    {
        _mapDownloadMenuUi.SetActive( true );
        _mapDownloadMenu.OnMenuButtonClicked();
    }

    /// <summary>
    /// Callback event for opening map download menu.
    /// </summary>
    void OnDepthClick()
    {
        _depthMenuUi.SetActive( true );
        _depthMenu.OnMenuButtonClicked();
    }

    /// <summary>
    /// Callback event for collisions toggle changing value.
    /// </summary>
    void OnCollisionsToggle()
    {
        _player.GetComponent<NavMeshAgent>().enabled = _enableCollisionsToggle.isOn;
    }

    /// <summary>
    /// Callback event for closing the setting menu.
    /// </summary>
    public void OnMenuClosed()
    {
        _settingsWindows.SetActive( false );
        _settingMenuUi.SetActive( false );
        _mapDownloadMenuUi.SetActive( false );
        _depthMenuUi.SetActive( false );
    }
}
