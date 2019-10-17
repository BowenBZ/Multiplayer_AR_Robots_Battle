using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618
public static class RobotMessage
{
    public class MessageType
    {
        public static short ToClient = 48;
        public static short ToServer = 49;
    };

    public class Message : MessageBase
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
    };
}
