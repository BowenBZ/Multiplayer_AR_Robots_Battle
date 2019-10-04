using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class AnchorUIControl : MonoBehaviour
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
    /// Start the sequence of upload an anchor
    /// </Summary>
    public async void StartCreateAnchor()
    {
        // Disable check room button since this client aims to create a room
        CheckButton.SetActive(false);
        JoinButton.SetActive(false);
        // Starts the first flow to prepare for upload an anchor
        await anchorControl.myCreateFlow1Async();
        // Enable relevant UI
        guidanceText.SetActive(true);
        confirmUploadButton.SetActive(true);
    }

    /// <Summary>
    /// Finish the sequence of upload an anchor
    /// </Summary>
    public async void FinishCreateAnchor()
    {
        // Disable confirm button after select an anchor
        confirmUploadButton.SetActive(false);
        // Starts the second flow to upload the anchor
        await anchorControl.myCreateFlow2Async();
        // Disable guidance UI when finishing uploading
        guidanceText.SetActive(false);
        // Real create a room
        hostGame.CreateNewRoom(anchorControl.anchorIndex);
        // Disable create room button
        CreateButton.SetActive(false);
    }

    /// <Summary>
    /// Start download the sequence of upload an anchor
    /// </Summary>
    public async void StartDownloadAnchor(string anchorIndex)
    {
        // Disable create room and join room button
        CreateButton.SetActive(false);
        JoinButton.SetActive(false);
        // Enable guidance text
        guidanceText.SetActive(true);
        // Start the download process
        await anchorControl.myLocateFlow1Async(anchorIndex);
    }

    public void AnchorNameSuccess()
    {
        guidanceText.SetActive(false);
    }

    public void SwitchChecktoJoin()
    {
        CheckButton.SetActive(false);
        JoinButton.SetActive(true);
    }
}
