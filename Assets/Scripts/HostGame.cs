using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using UnityEngine.UI;

public class HostGame : MonoBehaviour
{
    MyNetworkManager manager;
    Text roomNames;

    void Awake()
    {
        manager = transform.GetComponent<MyNetworkManager>();
        manager.StartMatchMaker();
    }

    void Start()
    {
        roomNames = GameObject.Find("RoomName").GetComponent<Text>();
        roomNames.text = "";
    }

    public void CreateNewRoom()
    {
        string matchName = "l33tKidz";
        uint matchSize = 8;

        manager.matchMaker.CreateMatch(matchName, matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
    }

    public void CheckRoom()
    {
        manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, manager.OnMatchList);
        if (manager.matches != null && manager.matches.Count > 0)
        {
            roomNames.text = manager.matches[0].name;
        }
    }

    public void JoinExistingRoom()
    {
        //manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, manager.OnMatchList);
        if (manager.matches != null && manager.matches.Count > 0)
        {
            manager.matchName = manager.matches[0].name;
            manager.matchMaker.JoinMatch(manager.matches[0].networkId, "", "", "", 0, 0, manager.OnMatchJoined);
        }
    }


}