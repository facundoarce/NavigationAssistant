using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        //_settingMenuUi.SetActive( false );
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
