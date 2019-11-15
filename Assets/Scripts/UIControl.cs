using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using System.Threading.Tasks;
using UnityEngine.UI;

/// <summary>
/// Control the UI logic
/// </summary>
public class UIControl : MonoBehaviour
{
    GameObject guidanceText;
    GameObject JoinButtonPrefab;

    GameObject RoomManagerBtn;
    GameObject RoomManageBackground;

    float offset = 0.0f;
    float offsetDelta = -160.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneBridge.playMode == SceneBridge.PlayMode.ARMode)
        {
            // Get buttons
            guidanceText = GameObject.Find("CreateFlowText");
            // Set False
            guidanceText.SetActive(false);
        }

        RoomManagerBtn = GameObject.Find("RoomManageBtn");

        RoomManageBackground = GameObject.Find("RoomManageBackground");

        JoinButtonPrefab = GameObject.Find("JoinRoom");
        JoinButtonPrefab.SetActive(false);
        RoomManageBackground.SetActive(false);

    }

    /// <summary>
    /// Switch the status of the room manager
    /// </summary>
    public void SwitchRoomManagerStatus()
    {
        RoomManageBackground.SetActive(!RoomManageBackground.activeInHierarchy);
    }

    /// <Summary>
    /// Show the UI of uploading an anchor
    /// </Summary>
    public void ShowGuidanceUI()
    {
        guidanceText.SetActive(true);
    }

    /// <Summary>
    /// Close all UI elements
    /// </Summary>
    public void HideGuidanceUI()
    {
        if (SceneBridge.playMode == SceneBridge.PlayMode.ARMode)
        {
            guidanceText.SetActive(false);
        }
    }

    /// <summary>
    /// Set the current room name to the room manager button
    /// </summary>
    /// <param name="matchName"></param>
    public void SetCurrentRoomName(string matchName)
    {
        RoomManagerBtn.transform.Find("Text").GetComponent<Text>().text = matchName;
        SwitchRoomManagerStatus();
    }


    List<GameObject> roomItems = new List<GameObject>();
    /// <summary>
    /// Show the searched name list in the room manager background
    /// </summary>
    /// <param name="searchedName"></param>
    public void SetSearchedName(List<string> searchedName)
    {
        // Destory previous items
        for (int i = 0; i < roomItems.Count; i++)
        {
            Destroy(roomItems[i]);
        }
        roomItems = new List<GameObject>();
        offset = 0;

        Transform searchedNameParent = RoomManageBackground.transform.Find("Image");
        for (int i = 0; i < searchedName.Count; i++)
        {
            GameObject roomItem = GameObject.Instantiate(JoinButtonPrefab);
            roomItems.Add(roomItem);
            roomItem.SetActive(true);
            roomItem.transform.parent = searchedNameParent;
            roomItem.transform.localScale = new Vector3(1, 1, 1);
            roomItem.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, offset, 0);
            offset += offsetDelta;
            roomItem.transform.GetChild(0).name = "Room#" + i.ToString();
            roomItem.transform.GetChild(1).GetComponent<Text>().text = searchedName[i];
        }
    }
}
