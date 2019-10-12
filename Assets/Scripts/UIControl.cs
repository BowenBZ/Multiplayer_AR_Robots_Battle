using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using System.Threading.Tasks;

public class UIControl : MonoBehaviour
{
    GameObject guidanceText;
    GameObject CreateButton;
    GameObject CheckButton;
    GameObject JoinButton;

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR
        // Get buttons
        guidanceText = GameObject.Find("CreateFlowText");
        // Set False
        guidanceText.SetActive(false);
#endif
        CreateButton = GameObject.Find("CreateRoom");
        CheckButton = GameObject.Find("CheckRoom");
        JoinButton = GameObject.Find("JoinRoom");
        JoinButton.SetActive(false);
    }

    /// <Summary>
    /// Start the UI flow of uploading an anchor
    /// </Summary>
    public void EnableUploadUIFlow()
    {
        RoomButtonsClose();
        guidanceText.SetActive(true);
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
#if !UNITY_EDITOR
        guidanceText.SetActive(false);
#endif
    }
}
