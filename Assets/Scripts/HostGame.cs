using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using UnityEngine.UI;

public class HostGame : MonoBehaviour
{
    MyNetworkManager manager;
    Text roomNames;
    AnchorUIControl anchorUIControl;

    void Awake()
    {
        manager = transform.GetComponent<MyNetworkManager>();
        manager.StartMatchMaker();
    }

    void Start()
    {
        roomNames = GameObject.Find("RoomName").GetComponent<Text>();
        roomNames.text = "";
        anchorUIControl = GetComponent<AnchorUIControl>();
    }

    /// <Summary>
    /// Initialize an anchor firstly, then this function will be called
    /// </Summary>
    public void CreateNewRoom(long anchorIndex)
    {
        if (!manager.isInRoom)
        {
            // Create the match name and parameters
            string matchName = "l33tKidz#" + anchorIndex.ToString();
            roomNames.text = matchName;
            uint matchSize = 8;
            manager.matchMaker.CreateMatch(matchName, matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
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
                anchorUIControl.SwitchChecktoJoin();
            }
        }
    }

    /// <Summary>
    /// Join the room and download the anchors 
    /// </Summary>
    public void JoinExistingRoom()
    {
        if (!manager.isInRoom && manager.matches != null && manager.matches.Count > 0)
        {
            manager.matchName = manager.matches[0].name;
            manager.matchMaker.JoinMatch(manager.matches[0].networkId, "", "", "", 0, 0, manager.OnMatchJoined);
        }
    }
}