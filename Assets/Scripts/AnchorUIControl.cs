using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class AnchorUIControl : MonoBehaviour
{
    GameObject guidanceText;
    GameObject confirmUploadButton;
    GameObject anchorNameBox;
    GameObject confirmDownloadButton;

    AzureSpatialAnchorsSharedAnchorDemoScript anchorControl;

    // Start is called before the first frame update
    void Start()
    {
        // Get buttons
        guidanceText = GameObject.Find("CreateFlowText");
        confirmUploadButton = GameObject.Find("CreateFlowButton");
        anchorNameBox = GameObject.Find("AnchorNumberBox");
        confirmDownloadButton = GameObject.Find("LocateFlowButton");

        // Get anchor controller
        anchorControl = GameObject.Find("AzureSpatialAnchors").GetComponent<AzureSpatialAnchorsSharedAnchorDemoScript>();

        // Set False
        guidanceText.SetActive(false);
        confirmUploadButton.SetActive(false);
        anchorNameBox.SetActive(false);
        confirmDownloadButton.SetActive(false);
    }

    public async void StartCreateAnchor()
    {
        await anchorControl.myCreateFlow1Async();
        guidanceText.SetActive(true);
        confirmUploadButton.SetActive(true);
    }

    public async void FinishCreateAnchor()
    {
        await anchorControl.myCreateFlow2Async();
        guidanceText.SetActive(false);
        confirmUploadButton.SetActive(false);
    }

    public void InputAnchorName()
    {
        guidanceText.SetActive(true);
        anchorNameBox.SetActive(true);
        confirmDownloadButton.SetActive(true);
    }

    public async void ConfirmAnchorName()
    {
        await anchorControl.myLocateFlow1Async();
    }

    public void AnchorNameSuccess()
    {
        guidanceText.SetActive(false);
        anchorNameBox.SetActive(false);
        confirmDownloadButton.SetActive(false);
    }
}
