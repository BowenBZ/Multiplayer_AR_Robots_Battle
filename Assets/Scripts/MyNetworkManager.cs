using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

#pragma warning disable 0618
public class MyNetworkManager : NetworkManager
{
    // Text to show the room info
    Text roomNames;

    // Flag to presents whether in a room
    [HideInInspector]
    public bool isInRoom;

    void Start()
    {
        roomNames = GameObject.Find("RoomName").GetComponent<Text>();
        isInRoom = false;
    }

    
    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (LogFilter.logDebug) { Debug.LogFormat("NetworkManager OnMatchCreate Success:{0}, ExtendedInfo:{1}, matchInfo:{2}", success, extendedInfo, matchInfo); }

        if (success)
        {
            NetworkClient localClient = StartHost(matchInfo);
            GetComponent<NetworkDataShare>().SetupClientServer(localClient);
            Debug.Log("Save Local Client during Creating");
            roomNames.text = "Created " + roomNames.text;
            isInRoom = true;
        }
    }

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (LogFilter.logDebug) { Debug.LogFormat("NetworkManager OnMatchJoined Success:{0}, ExtendedInfo:{1}, matchInfo:{2}", success, extendedInfo, matchInfo); }

        if (success)
        {
            NetworkClient localClient = StartClient(matchInfo);
            GetComponent<NetworkDataShare>().SetupClientServer(localClient);
            Debug.Log("Save Local Client during Joining");

            // Change the settings
            roomNames.text = "Joined " + roomNames.text;
            isInRoom = true;
        }
    }
}
