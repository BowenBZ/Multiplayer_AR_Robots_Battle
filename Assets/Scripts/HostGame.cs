using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;

public class HostGame : MonoBehaviour
{
    MyNetworkManager manager;
    Text roomNames;
    UIControl UIManager;
    AzureAnchorControl anchorControl;

    void Awake()
    {
        manager = transform.GetComponent<MyNetworkManager>();
        manager.StartMatchMaker();
        anchorControl = GameObject.Find("AzureSpatialAnchors").GetComponent<AzureAnchorControl>();
    }

    void Start()
    {
        roomNames = GameObject.Find("RoomName").GetComponent<Text>();
        roomNames.text = "";
        UIManager = GetComponent<UIControl>();
    }

    /// <Summary>
    /// Multi-entry function
    /// </Summary>
    public void CreateRoom()
    {
#if !UNITY_EDITOR
        InitializeAnchorUpload();
#else
        UploadAnchorCreateRoom();
#endif
    }

    /// <Summary>
    /// Initialize the anchor uploading process
    /// </Summary>
    public async void InitializeAnchorUpload()
    {
        if (!manager.isInRoom)
        {
            UIManager.EnableUploadUIFlow();
            await anchorControl.InitializeUploadSession();
        }
    }

    /// <Summary>
    /// Upload the anchors, then create the room
    /// </Summary>
    public async void UploadAnchorCreateRoom()
    {
        if (!manager.isInRoom)
        {
            string matchName = "l33tKidz#";

#if !UNITY_EDITOR
            // Finishing uploading the anchors
            long anchorIndex = await anchorControl.UploadAnchor();

            if (anchorIndex != -1)
            {
                matchName += anchorIndex.ToString();
#endif
            // Close UI
            UIManager.AllUIClose();

            // Set up the room
            roomNames.text = matchName;
            uint matchSize = 8;
            manager.matchMaker.CreateMatch(matchName, matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);

#if !UNITY_EDITOR
            }
#endif
        }
    }

    /// <Summary>
    /// Check whether there is a room in the server 
    /// </Summary>
    public void CheckRoom()
    {
        if (!manager.isInRoom)
        {
            manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, manager.OnMatchList);
            if (manager.matches != null && manager.matches.Count > 0)
            {
                roomNames.text = manager.matches[0].name;
                UIManager.SwitchChecktoJoin();
            }
        }
    }

    /// <Summary>
    /// Join the room and download the anchors 
    /// </Summary>
    public async void JoinExistingRoom()
    {
        if (!manager.isInRoom && manager.matches != null && manager.matches.Count > 0)
        {
            manager.matchName = manager.matches[0].name;
            manager.matchMaker.JoinMatch(manager.matches[0].networkId, "", "", "", 0, 0, manager.OnMatchJoined);

#if !UNITY_EDITOR
            // Wait until it joins a room
            while (!manager.isInRoom)
            {
                await Task.Delay(330);
            }

            // Start to download anchors according to the room name
            string[] s = roomNames.text.Split('#');
            string anchorIndex = s[1];
            await anchorControl.DownloadAnchor(anchorIndex);
#endif

            // Disable guidance text after the download process finished
            UIManager.AllUIClose();
        }
    }
}