using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class MyNetworkManager : NetworkManager
{

    Text roomNames;

    void Start()
    {
        roomNames = GameObject.Find("RoomName").GetComponent<Text>();
    }


    public virtual void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (LogFilter.logDebug) { Debug.LogFormat("NetworkManager OnMatchCreate Success:{0}, ExtendedInfo:{1}, matchInfo:{2}", success, extendedInfo, matchInfo); }

        if (success)
        {
            NetworkClient localClient = StartHost(matchInfo);
            GetComponent<NetworkDataShare>().SetupClientServer(localClient);
            Debug.Log("Save Local Client during Creating");
            roomNames.text = "Created room";
        }
    }

    public virtual void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (LogFilter.logDebug) { Debug.LogFormat("NetworkManager OnMatchJoined Success:{0}, ExtendedInfo:{1}, matchInfo:{2}", success, extendedInfo, matchInfo); }

        if (success)
        {
            NetworkClient localClient = StartClient(matchInfo);
            GetComponent<NetworkDataShare>().SetupClientServer(localClient);
            Debug.Log("Save Local Client during Joining");
            roomNames.text = "Joined Room";
        }
    }
}
