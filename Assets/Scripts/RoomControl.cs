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

        // Client ID
        clientID = "";

        // Other robot's control
        enemyRobotControl = transform.GetComponent<ObjectsControl>();

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
    void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            localClient = networkManager.StartHost(matchInfo);
            SetupClientandServer();
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
    void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            localClient = networkManager.StartClient(matchInfo);
            SetupClientandServer();
            UIManager.AllUIClose();
            Debug.Log("Save Local Client during Joining");
            roomNames.text = "Joined " + roomNames.text;
        }
    }

    string clientID;
    public string ClientID { get { return clientID; } }

    /// </Summary>
    /// Set up the call back of local client
    /// </Summary>
    void SetupClientandServer()
    {
        // Set up client handler
        localClient.RegisterHandler(RobotMessage.MessageType.ToClient, OnClientReceiveMsg);
        clientID = GenerateRandomString(10);

        // Set up serve handler
        NetworkServer.RegisterHandler(RobotMessage.MessageType.ToServer, OnServerReceiveMsg);
    }

    /// </Summary>
    /// Generate the client ID
    /// </Summary>

    string GenerateRandomString(int length)
    {
        char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        string checkCode = string.Empty;
        Random rd = new Random();
        for (int i = 0; i < length; i++)
        {
            checkCode += constant[Mathf.RoundToInt(Random.Range(0, 36))].ToString();
        }
        return checkCode;
    }

    RobotMessage.Message serverReceivedMsg;
    /// </Summary>
    /// Server event: receive one message and send it to other clients
    /// </Summary>
    async void OnServerReceiveMsg(NetworkMessage netMsg)
    {
        serverReceivedMsg = netMsg.ReadMessage<RobotMessage.Message>();
        
        // Debug.Log("Server Receive Message");
        await Task.Run(() => NetworkServer.SendToAll(RobotMessage.MessageType.ToClient, serverReceivedMsg));
    }

    RobotMessage.Message clientReceivedMsg;
    ObjectsControl enemyRobotControl;

    /// </Summary>
    /// Client event: check if received from others, control other's robots
    /// </Summary>
    void OnClientReceiveMsg(NetworkMessage netMsg)
    {
        clientReceivedMsg = netMsg.ReadMessage<RobotMessage.Message>();
        // Debug.Log("Receive Message " + clientReceivedMsg.ID);
        
        // Filter the msg sent from this client 
        if (clientReceivedMsg.ID != clientID)
        {
            enemyRobotControl.ControlEnemyRobot(clientReceivedMsg);
        }
    }

    /// <Summary>
    /// Needs multi-threading here 
    /// </Summary>
    public async void SendMessagetoServer(RobotMessage.Message msg)
    {
        if (!IsInRoom)
            return;

        msg.ID = clientID;
        msg.robotIndex = SceneBridge.clientRobotIndex;
        await Task.Run(() => localClient.Send(RobotMessage.MessageType.ToServer, msg));
    }
}