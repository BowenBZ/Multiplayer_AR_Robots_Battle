using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkDataShare : MonoBehaviour
{
   NetworkClient myClient;

    public class MyMsgType {
        public static short Score = 48;
        public static short serverAction = 49;
    };

    public class ScoreMessage : MessageBase
    {
        public int score;
        public Vector3 scorePos;
        public int lives;
    }

    public void SendScore()
    {
        ScoreMessage msg = new ScoreMessage();
        msg.score = 123;
        msg.scorePos = new Vector3(0, 1, 2);
        msg.lives = 3;
        // NetworkServer.SendToAll(MyMsgType.Score, msg);
        myClient.Send(MyMsgType.serverAction, msg);
    }


    // public void SendScore(int score, Vector3 scorePos, int lives)
    // {
    //     ScoreMessage msg = new ScoreMessage();
    //     msg.score = score;
    //     msg.scorePos = scorePos;
    //     msg.lives = lives;

    //     NetworkServer.SendToAll(MyMsgType.Score, msg);
    // }

    public void SetupClientServer(NetworkClient localClient)
    {
        // Set up serve handler
        NetworkServer.RegisterHandler(MyMsgType.serverAction, OnServer);

        // Set up client handler
        myClient = localClient;
        myClient.RegisterHandler(MyMsgType.Score, OnScore);
    }

    public void OnScore(NetworkMessage netMsg)
    {
        ScoreMessage msg = netMsg.ReadMessage<ScoreMessage>();
        Debug.Log("OnScoreMessage " + msg.score);
    }

    public void OnServer(NetworkMessage netMsg)
    {
        ScoreMessage msg = netMsg.ReadMessage<ScoreMessage>();
        NetworkServer.SendToAll(MyMsgType.Score, msg);
    }

}
