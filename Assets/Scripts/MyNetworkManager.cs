using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MyNetworkManager : NetworkManager
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    // {
    //     var player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(1, 5, 0), Quaternion.identity);
    //     //GameObject player = GameObject.Find("Robot#1");
    //     NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    // }


    public virtual void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (LogFilter.logDebug) { Debug.LogFormat("NetworkManager OnMatchCreate Success:{0}, ExtendedInfo:{1}, matchInfo:{2}", success, extendedInfo, matchInfo); }

        if (success)
        { 
            NetworkClient localClient = StartHost(matchInfo);
            GetComponent<NetworkDataShare>().SetupClientServer(localClient);
            Debug.Log("Save Local Client during Creating");
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
        }
    }
}
