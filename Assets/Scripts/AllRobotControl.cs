using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class AllRobotControl : MonoBehaviour
{
    string enemyID;
    GameObject enemyRobot;
    public GameObject clientRobotPrefab;
    public GameObject enemyRobotPrefab;
    Animator anim;
    AzureSpatialAnchorsSharedAnchorDemoScript anchorControl;

    GameObject anchorObj;

    void Start()
    {
        anchorControl = GameObject.Find("AzureSpatialAnchors").GetComponent<AzureSpatialAnchorsSharedAnchorDemoScript>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            CreateClientRobot(new Vector3(0, 1, 0), Quaternion.identity);
        }

    }

    // When local client heard from other clients
    public void ControlEnemyRobot(NetworkDataShare.RobotMessage msg)
    {
        // If haven't set up the anchor
        if (!anchorControl.FinishAnchorSync)
            return;

        // Check wether the robot has already existed
        string enemyID = msg.ID;
        enemyRobot = GameObject.Find(enemyID);
        // If not, create an object
        if (enemyRobot == null)
        {
            enemyRobot = GameObject.Instantiate(enemyRobotPrefab);
            // Assign the Id to the name
            enemyRobot.name = enemyID;
            // Set it parent
            if (anchorObj == null)
                anchorObj = GameObject.FindWithTag("AnchorOriginPoint");
            enemyRobot.transform.parent = anchorObj.transform;
        }
        // Set it positions
        enemyRobot.transform.localPosition = msg.localPos;
        enemyRobot.transform.localRotation = msg.localRot;

        // Set it animator parameters
        anim = enemyRobot.GetComponent<Animator>();
        anim.SetFloat("Speed", msg.Speed);
        anim.SetFloat("VelocityVertical", msg.VelocityVertical);
        anim.SetFloat("VelocityHorizontal", msg.VelocityHorizontal);

        anim.SetBool("Jump", msg.Jump);
        anim.SetBool("Attack1", msg.Attack1);
        anim.SetBool("Attack1-1", msg.Attack1_1);
        anim.SetBool("Attack1-2", msg.Attack1_2);
        anim.SetBool("Attack2", msg.Attack2);
        anim.SetBool("Attack3", msg.Attack3);

    }

    public void CreateClientRobot(Vector3 pos, Quaternion rot)
    {
        // If haven't set up the anchor
        if (!anchorControl.FinishAnchorSync)
            return;

        // If haven't set up the client
        string robotID = GetComponent<NetworkDataShare>().clientID;
        if (robotID == "" || GameObject.Find(robotID) != null)
            return;

        // Create the object
        GameObject clientRobot = GameObject.Instantiate(clientRobotPrefab, pos, rot);
        // Assign the ID to the name
        clientRobot.name = robotID;
        // Find the anchor object and set it to the parent
        anchorObj = GameObject.FindWithTag("AnchorOriginPoint");
        clientRobot.transform.parent = anchorObj.transform;

        // Establish the animation parater
        NetworkDataShare.RobotMessage msg = new NetworkDataShare.RobotMessage();
        msg.localPos = clientRobot.transform.localPosition;
        msg.localRot = clientRobot.transform.localRotation;

        // Send it
        GetComponent<NetworkDataShare>().SendMessagetoServer(msg);
    }
}
