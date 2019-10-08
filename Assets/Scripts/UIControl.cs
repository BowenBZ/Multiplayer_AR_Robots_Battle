using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using System.Threading.Tasks;

public class UIControl : MonoBehaviour
{
    // UI elements
    GameObject guidanceText;
    GameObject confirmUploadButton;

    GameObject CreateButton;
    GameObject CheckButton;
    GameObject JoinButton;

    // Other scripts
    AzureAnchorControl anchorControl;
    HostGame hostGame;

    // Start is called before the first frame update
    void Start()
    {
        // Get buttons
        guidanceText = GameObject.Find("CreateFlowText");
        confirmUploadButton = GameObject.Find("CreateFlowButton");
        CreateButton = GameObject.Find("CreateRoom");
        CheckButton = GameObject.Find("CheckRoom");
        JoinButton = GameObject.Find("JoinRoom");

        // Set False
        guidanceText.SetActive(false);
        confirmUploadButton.SetActive(false);
        JoinButton.SetActive(false);

        // Get anchor controller
        anchorControl = GameObject.Find("AzureSpatialAnchors").GetComponent<AzureAnchorControl>();
        // Get Host Game
        hostGame = GetComponent<HostGame>();
    }

    /// <Summary>
    /// Start the UI flow of uploading an anchor
    /// </Summary>
    public void EnableUploadUIFlow()
    {
        RoomButtonsClose();
        guidanceText.SetActive(true);
        confirmUploadButton.SetActive(true);
    }


    /// <Summary>
    /// Start download the sequence of upload an anchor
    /// </Summary>
    public void EnableDownloadUIFlow()
    {
        // Disable create room and join room button
        CreateButton.SetActive(false);
        JoinButton.SetActive(false);

        // Enable guidance text
        guidanceText.SetActive(true);
    }

    /// <Summary>
    /// Switch the check button to join button
    /// </Summary>
    public void SwitchChecktoJoin()
    {
        CheckButton.SetActive(false);
        JoinButton.SetActive(true);
    }

    /// <Summary>
    /// Close all room button UI elements
    /// </Summary>
    public void RoomButtonsClose()
    {
        CreateButton.SetActive(false);
        CheckButton.SetActive(false);
        JoinButton.SetActive(false);
    }

    /// <Summary>
    /// Close all UI elements
    /// </Summary>
    public void AllUIClose()
    {
        RoomButtonsClose();
        guidanceText.SetActive(false);
        confirmUploadButton.SetActive(false);
    }
}
