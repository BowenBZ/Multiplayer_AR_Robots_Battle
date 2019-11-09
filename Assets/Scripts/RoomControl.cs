using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;

#pragma warning disable 0618    // Disable the warning of the obsolete UNet
/// <summary>
/// 1. Control to the create and join network rooms
/// 2. Handle the communication among clients 
/// </summary>
public class RoomControl : MonoBehaviour
{
    NetworkManager networkManager;
    Text roomNames;
    UIControl UIManager;
    AzureAnchorControl anchorControl;
    NetworkClient localClient;      // The local client

    /// <summary>
    /// Indicate whether in a network room
    /// </summary>
    /// <param name="!"></param>
    /// <returns></returns>
    public bool IsInRoom { get { return (localClient != null); } }


    void Awake()
    {
        // Client ID
        clientID = GenerateRandomString(10);
    }

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

        // Other robot's control
        enemyRobotControl = transform.GetComponent<ObjectsControl>();

        // If it is in AR mode, get the AzureSpatialAnchor objects
        if (SceneBridge.playMode == SceneBridge.PlayMode.ARMode)
        {
            anchorControl = GameObject.Find("AzureSpatialAnchors").GetComponent<AzureAnchorControl>();
        }
    }


    /// <summary>
    /// Initialize the anchor uploading process, and then create the room
    /// </summary>
    /// <returns></returns>
    public async void CreateRoom()
    {
        if (!IsInRoom)
        {
            string matchName = "l33tKidz#";

            // AR Mode
            if (SceneBridge.playMode == SceneBridge.PlayMode.ARMode)
            {
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
            else if(SceneBridge.playMode == SceneBridge.PlayMode.onlineMode)  // Online Mode
            {
                char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
                matchName += constant[Mathf.RoundToInt(Random.Range(0, 36))].ToString();

                // Set up the room
                roomNames.text = matchName;
                uint matchSize = 8;
                networkManager.matchMaker.CreateMatch(matchName, matchSize, true, "", "", "", 0, 0, OnMatchCreate);
            }


        }

    }

    /// <summary>
    /// Call back of the room creation
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="matchInfo"></param>
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


    /// <summary>
    /// Check whether there is a room in the server 
    /// </summary>
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

    /// <summary>
    /// Join the room and download the anchors 
    /// </summary>
    /// <returns></returns>
    public async void JoinExistingRoom()
    {
        if (!IsInRoom && networkManager.matches != null && networkManager.matches.Count > 0)
        {
            networkManager.matchName = networkManager.matches[0].name;
            networkManager.matchMaker.JoinMatch(networkManager.matches[0].networkId, "", "", "", 0, 0, OnMatchJoined);

            if (SceneBridge.playMode == SceneBridge.PlayMode.ARMode)
            {
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
    }

    /// <summary>
    /// Call back of joining a network room
    /// </summary>
    /// <param name="success"></param>
    /// <param name="extendedInfo"></param>
    /// <param name="matchInfo"></param>
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
    /// <summary>
    /// The ID of this local client
    /// </summary>
    /// <value></value>
    public string ClientID { get { return clientID; } }

    /// <Summary>
    /// Set up the call back of local client
    /// </Summary>
    void SetupClientandServer()
    {
        // Set up client handler
        localClient.RegisterHandler(RobotMessage.MessageType.ToClient, OnClientReceiveMsg);

        // Set up serve handler
        NetworkServer.RegisterHandler(RobotMessage.MessageType.ToServer, OnServerReceiveMsg);
    }

    /// <summary>
    /// Generate the client ID
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Server event: receive one message and send it to other clients
    /// </summary>
    /// <param name="netMsg"></param>
    void OnServerReceiveMsg(NetworkMessage netMsg)
    {
        serverReceivedMsg = netMsg.ReadMessage<RobotMessage.Message>();

        // Debug.Log("Server Receive Message");
        NetworkServer.SendToAll(RobotMessage.MessageType.ToClient, serverReceivedMsg);
    }

    RobotMessage.Message clientReceivedMsg;
    ObjectsControl enemyRobotControl;

    /// <summary>
    /// Client event: check if received from others, control other's robots
    /// </summary>
    /// <param name="netMsg"></param>
    void OnClientReceiveMsg(NetworkMessage netMsg)
    {
        clientReceivedMsg = netMsg.ReadMessage<RobotMessage.Message>();
        // Debug.Log("Receive Message " + clientReceivedMsg.ID);

        // Filter the msg sent from this client 
        if (clientReceivedMsg.ID != clientID)
        {
            Debug.Log("Receive Message " + clientReceivedMsg.ID);
            enemyRobotControl.ControlEnemyRobot(clientReceivedMsg);
        }
    }

    /// <summary>
    /// Send message from local client
    /// </summary>
    /// <param name="msg"></param>
    public void SendMessagetoServer(RobotMessage.Message msg)
    {
        if (!IsInRoom)
            return;

        msg.ID = clientID;
        msg.robotIndex = SceneBridge.clientRobotIndex;
        localClient.Send(RobotMessage.MessageType.ToServer, msg);
    }

    /// <summary>
    /// Exit to main scene
    /// </summary>
    public void ExitToMainScene()
    {
        SceneBridge.ExitToMainScene();
    }
}