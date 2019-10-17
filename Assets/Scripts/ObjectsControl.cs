using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ObjectsControl : InputInteractionBase
{
    string clientID;
    GameObject clientRobot;
    string enemyID;
    Dictionary<string, GameObject> enemyRobotList;
    GameObject enemyRobot;
    MainSceneRobotSelection robotSelection;
    RobotControl enemyControl;
    Animator enemyAnim;

    AzureAnchorControl anchorControl;
    Transform anchorTransform;
    RoomControl roomControl;

    public override void Start()
    {
        base.Start();
        anchorControl = GameObject.Find("AzureSpatialAnchors").GetComponent<AzureAnchorControl>();
        robotSelection = GetComponent<MainSceneRobotSelection>();
        roomControl = GetComponent<RoomControl>();
        enemyRobotList = new Dictionary<string, GameObject>();
    }

    public override void Update()
    {
        base.Update();

        // Used for test
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("S Pressed.");
            CreateClientRobot(new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    protected override void OnSelectObjectInteraction(Vector3 hitPoint, object target)
    {
        CreateClientRobot(hitPoint, Quaternion.AngleAxis(0, Vector3.up));
    }

    // When local client heard from other clients
    public void ControlEnemyRobot(RobotMessage.Message msg)
    {
        // If haven't set up the room or anchor
        if (!roomControl.IsInRoom || !anchorControl.IsAnchorSync)
            return;

        // Check wether the robot has already existed
        enemyID = msg.ID;

        // If not, create an object
        if (!enemyRobotList.ContainsKey(enemyID))
        {
            // Insantiate the robot according to the robot index
            enemyRobot = GameObject.Instantiate(robotSelection.objects[msg.robotIndex]);
            // Assign the Id to the name
            enemyRobot.name = enemyID;
            // Put it into dictionary
            enemyRobotList.Add(enemyID, enemyRobot);
            // Set it parent
            if (anchorTransform == null)
            {
                anchorTransform = anchorControl.AnchorTransform;
                // anchorTransform = GameObject.FindWithTag("AnchorOriginPoint").transform;
            }
            enemyRobot.transform.parent = anchorTransform;
        }
        else
        {
            enemyRobot = enemyRobotList[enemyID];
        }

        // Set it positions
        enemyRobot.transform.localPosition = msg.localPos;
        enemyRobot.transform.localRotation = msg.localRot;

        // Set it HP and MP
        enemyControl = enemyRobot.GetComponent<RobotControl>();
        enemyControl.HP = msg.HP;
        enemyControl.MP = msg.MP;

        // Set it robot status
        enemyControl.robotStatus = (RobotControl.RobotStatus)msg.robotStatus;

        // Set it animator parameters
        enemyAnim = enemyRobot.GetComponent<Animator>();
        enemyAnim.SetFloat("Speed", msg.Speed);

        enemyAnim.SetBool("Jump", msg.Jump);
        enemyAnim.SetBool("Attack1", msg.Attack1);
        enemyAnim.SetBool("Attack1-1", msg.Attack1_1);
        enemyAnim.SetBool("Attack1-2", msg.Attack1_2);
        enemyAnim.SetBool("Attack2", msg.Attack2);
        enemyAnim.SetBool("Attack3", msg.Attack3);

        enemyAnim.SetBool("Hit", msg.Hit);
    }

    public void CreateClientRobot(Vector3 pos, Quaternion rot)
    {
#if !UNITY_EDITOR
        // If haven't set up the anchor or haven't join the room
        if (!anchorControl.IsAnchorSync || !roomControl.IsInRoom)
            return;
#endif

        // If have already set up the client robot
        if (clientRobot != null)
            return;

        // Create the object
        clientRobot = GameObject.Instantiate(robotSelection.objects[SceneBridge.clientRobotIndex], pos, rot);
        // Assign the ID to the name
        clientID = GetComponent<RoomControl>().ClientID;
        clientRobot.name = clientID;

        // Find the anchor object and set it to the parent
        anchorTransform = anchorControl.AnchorTransform;
        clientRobot.transform.parent = anchorTransform;

        // Establish the animation parater
        RobotMessage.Message msg = new RobotMessage.Message();
        msg.localPos = clientRobot.transform.localPosition;
        msg.localRot = clientRobot.transform.localRotation;
        msg.HP = 100.0f;
        msg.MP = 100.0f;

        // Send it
        GetComponent<RoomControl>().SendMessagetoServer(msg);

        // Distable the plane detection
        GameObject.Find("CameraParent").GetComponent<XRCameraPicker>().arCamera.GetComponent<ARPlaneManager>().detectionMode = PlaneDetectionMode.None;
        GameObject[] planes = GameObject.FindGameObjectsWithTag("DetectedPlane");
        for (int i = 0; i < planes.Length; i++)
            Destroy(planes[i]);
    }

}
