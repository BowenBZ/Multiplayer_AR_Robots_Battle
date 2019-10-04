using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618
public class NetworkDataShare : MonoBehaviour
{
    NetworkClient myClient;

    [HideInInspector]
    public string clientID;
    AllRobotControl enemyRobotControl;

    void Start()
    {
        enemyRobotControl = transform.GetComponent<AllRobotControl>();
        clientID = "";
    }

    public class MyMsgType
    {
        public static short RobotMessageforClient = 48;
        public static short RobotMessageforServer = 49;
    };

    public void SetupClientServer(NetworkClient localClient)
    {
        // Set up client handler
        myClient = localClient;
        myClient.RegisterHandler(MyMsgType.RobotMessageforClient, OnClientReceiveMsg);
        clientID = GenerateRandomString(10);

        // Set up serve handler
        NetworkServer.RegisterHandler(MyMsgType.RobotMessageforServer, OnServerReceiveMsg);

    }

    private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    public static string GenerateRandomString(int length)
    {
        string checkCode = string.Empty;
        Random rd = new Random();
        for (int i = 0; i < length; i++)
        {
            checkCode += constant[Mathf.RoundToInt(Random.Range(0, 36))].ToString();
        }
        return checkCode;
    }

    public class RobotMessage : MessageBase
    {
        public string ID;
        public int robotIndex;
        public float HP;
        public float MP;
        public Vector3 localPos;
        public Quaternion localRot;
        public float Speed;
        public bool Jump;
        public bool Attack1;
        public bool Attack1_1;
        public bool Attack1_2;
        public bool Attack2;
        public bool Attack3;
        public bool Hit;
        public int robotStatus;
    }

    public void SendMessagetoServer(RobotMessage msg)
    {
        if(clientID == "")
            return;

        msg.ID = clientID;
        msg.robotIndex = SceneBridge.clientRobotIndex;
        myClient.Send(MyMsgType.RobotMessageforServer, msg);
    }


    // Server receive one message and send it to other clients
    public void OnServerReceiveMsg(NetworkMessage netMsg)
    {
        RobotMessage msg = netMsg.ReadMessage<RobotMessage>();
        NetworkServer.SendToAll(MyMsgType.RobotMessageforClient, msg);
    }


    public void OnClientReceiveMsg(NetworkMessage netMsg)
    {
        RobotMessage msg = netMsg.ReadMessage<RobotMessage>();
        Debug.Log("OnScoreMessage " + msg.ID);
        // Filter the msg sent from this client 
        if (msg.ID != clientID)
        {
            enemyRobotControl.ControlEnemyRobot(msg);
        }
    }
}
