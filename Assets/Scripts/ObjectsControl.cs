using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Control the generation and modify for all objects
/// </summary>
public class ObjectsControl : InputInteractionBase
{
    #region Localparameters
    MainSceneRobotSelection robotSelection;     // Robots' prefabs
    GameObject clientRobot;         // Local client's robot
    Transform anchorTransform;      // Local anchor's transform
    AzureAnchorControl anchorControl;   // Local anchor's manager
    RoomControl roomControl;        // Local room manager
    #endregion

    #region OtherClientsParameters
    Dictionary<string, GameObject> enemyRobotList;      // Collections of enemies' robots
    string enemyID;             // Enemy's ID
    GameObject enemyRobot;      // Current handle enemy's robot
    RobotControl enemyControl;  // Current handle enemy's script
    Animator enemyAnim;         // Current handle enemy's animation
    #endregion

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
    }

    /// <summary>
    /// When user touch the screen to select an object
    /// </summary>
    /// <param name="hitPoint"></param>
    /// <param name="target"></param>
    protected override void OnSelectObjectInteraction(Vector3 hitPoint, object target)
    {
        // If have already set up the client robot
        if (clientRobot == null)
        {
            CreateClientRobot(hitPoint, Quaternion.AngleAxis(0, Vector3.up));
        }
    }

    /// <summary>
    /// Create the robot for local user according to the position and rotation
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    public void CreateClientRobot(Vector3 pos, Quaternion rot)
    {
#if !UNITY_EDITOR
        // If haven't set up the anchor OR haven't join the room
        if (!anchorControl.IsAnchorSync || !roomControl.IsInRoom)
            return;
#endif

        // Create the local robot for client
        clientRobot = GameObject.Instantiate(robotSelection.objects[SceneBridge.clientRobotIndex], pos, rot);
        // Assign the ID to the name
        clientRobot.name = roomControl.ClientID;

        // Find the anchor's transform and set it to as the parent of the robot
        anchorTransform = anchorControl.AnchorTransform;
        clientRobot.transform.parent = anchorTransform;

        // No need to send the msg of robot to server because the robot itself will send message to server

        // Distable the plane detection
        GameObject.Find("CameraParent").GetComponent<XRCameraPicker>().arCamera.GetComponent<ARPlaneManager>().detectionMode = PlaneDetectionMode.None;
        GameObject[] planes = GameObject.FindGameObjectsWithTag("DetectedPlane");
        for (int i = 0; i < planes.Length; i++)
            Destroy(planes[i]);
    }


    /// <summary>
    /// When local client get message from other clients
    /// </summary>
    /// <param name="msg"></param>
    public void ControlEnemyRobot(RobotMessage.Message msg)
    {
#if !UNITY_EDITOR
        // If haven't set up the anchor OR haven't join the room
        if (!anchorControl.IsAnchorSync || !roomControl.IsInRoom)
            return;
#endif

        // Get the current handle's enemy ID
        enemyID = msg.ID;

        // If not existed before, create a new enemy's robot
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
            }
            enemyRobot.transform.parent = anchorTransform;
        }
        else
        {
            enemyRobot = enemyRobotList[enemyID];
        }


        // enemyRobot = GameObject.Find(enemyID);
        // if(enemyRobot == null)
        // {
        //     // Insantiate the robot according to the robot index
        //     enemyRobot = GameObject.Instantiate(robotSelection.objects[msg.robotIndex]);
        //     // Assign the Id to the name
        //     enemyRobot.name = enemyID;
        //     // Put it into dictionary
        //     enemyRobotList.Add(enemyID, enemyRobot);
        //     // Set it parent
        //     if (anchorTransform == null)
        //     {
        //         anchorTransform = anchorControl.AnchorTransform;
        //         // anchorTransform = GameObject.FindWithTag("AnchorOriginPoint").transform;
        //     }
        //     enemyRobot.transform.parent = anchorTransform;
        // }

        // Set positions for enemy robot
        enemyRobot.transform.localPosition = msg.localPos;
        enemyRobot.transform.localRotation = msg.localRot;

        // Set HP and MP and status for enemy robot
        enemyControl = enemyRobot.GetComponent<RobotControl>();
        enemyControl.HP = msg.HP;
        enemyControl.MP = msg.MP;
        enemyControl.robotStatus = (RobotControl.RobotStatus)msg.robotStatus;

        // Set animator parameters for enemy robot
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
}
