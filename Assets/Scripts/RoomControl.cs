using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;

#pragma warning disable 0618    // Disable the warning of the obsolete UNet
public class RoomControl : MonoBehaviour
{
    NetworkManager networkManager;
    Text roomNames;
    UIControl UIManager;
    AzureAnchorControl anchorControl;
    NetworkClient localClient;      // The local client
    NetworkDataShare networkDataManger;

    // Indicate whether in local client
    public bool IsInRoom { get { return (localClient != null); } }


    /// <Summary>
    /// Get all required elements
    /// </Summary>
    void Start()
    {
        // Set up the network manager
        networkManager = transform.GetComponent<NetworkManager>();
        networkManager.StartMatchMaker();

        // Indicator text
        roomNames = GameObject.Find("RoomName").GetComponent<Text>();
        roomNames.text = "";

        // UI Manager
        UIManager = GetComponent<UIControl>();

        // Set local client to null
        localClient = null;
        networkDataManger = GetComponent<NetworkDataShare>();

#if !UNITY_EDITOR
        anchorControl = GameObject.Find("AzureSpatialAnchors").GetComponent<AzureAnchorControl>();
#endif
    }


    /// <Summary>
    /// Initialize the anchor uploading process, and then create the room
    /// </Summary>
    public async void CreateRoom()
    {
        if (!IsInRoom)
        {
            string matchName = "l33tKidz#";

#if !UNITY_EDITOR
            UIManager.EnableUploadUIFlow();

            // Upload the created anchor, and wait the anchor index return
            long anchorIndex = await anchorControl.UploadAnchor();

            if (anchorIndex != -1)
            {
                matchName += anchorIndex.ToString();
#endif
            // Set up the room
            roomNames.text = matchName;
            uint matchSize = 8;
            networkManager.matchMaker.CreateMatch(matchName, matchSize, true, "", "", "", 0, 0, OnMatchCreate);

#if !UNITY_EDITOR
            }
#endif
        }

    }

    /// <Summary>
    /// Call back of the room creation
    /// </Summary>
    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            localClient = networkManager.StartHost(matchInfo);
            networkDataManger.SetupClientServer(localClient);
            UIManager.AllUIClose();
            Debug.Log("Save Local Client during Creating");
            roomNames.text = "Created " + roomNames.text;
        }
    }

    /// <Summary>
    /// Check whether there is a room in the server 
    /// </Summary>
    public void CheckRoom()
    {
        if (!IsInRoom)
        {
            networkManager.matchMaker.ListMatches(0, 20, "", false, 0, 0, networkManager.OnMatchList);
            if (networkManager.matches != null && networkManager.matches.Count > 0)
            {
                roomNames.text = networkManager.matches[0].name;
                UIManager.SwitchChecktoJoin();
            }
        }
    }

    /// <Summary>
    /// Join the room and download the anchors 
    /// </Summary>
    public async void JoinExistingRoom()
    {
        if (!IsInRoom && networkManager.matches != null && networkManager.matches.Count > 0)
        {
            networkManager.matchName = networkManager.matches[0].name;
            networkManager.matchMaker.JoinMatch(networkManager.matches[0].networkId, "", "", "", 0, 0, OnMatchJoined);

#if !UNITY_EDITOR
            // Wait until it joins a room
            while (!IsInRoom)
            {
                await Task.Delay(330);
            }

            // Start to download anchors according to the room name
            string[] s = roomNames.text.Split('#');
            string anchorIndex = s[1];
            await anchorControl.DownloadAnchor(anchorIndex);
#endif
        }
    }

    /// <Summary>
    /// Call back of the room join
    /// </Summary>
    public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            localClient = networkManager.StartClient(matchInfo);
            networkDataManger.SetupClientServer(localClient);
            UIManager.AllUIClose();
            Debug.Log("Save Local Client during Joining");
            roomNames.text = "Joined " + roomNames.text;
        }
    }
}