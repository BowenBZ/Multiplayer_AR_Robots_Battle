using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVEGameManager : MonoBehaviour
{

    MainSceneRobotSelection robotSelection;     // Robots' prefabs
    
    [HideInInspector]
    public GameObject clientRobot;

    // Start is called before the first frame update
    void Start()
    {
        robotSelection = GetComponent<MainSceneRobotSelection>();
        CreateClientRobot();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Create the robot for local user, used in online mode
    /// </summary>
    public void CreateClientRobot()
    {
        // Create the local robot for client
        clientRobot = GameObject.Instantiate(robotSelection.objects[SceneBridge.clientRobotIndex], new Vector3(0, 0, 0), Quaternion.identity);
        // Enable the control
        clientRobot.GetComponent<RobotControl>().EnableControl();
    }

    public void ExitToMainScene()
    {
        SceneBridge.ExitToMainScene();
    }
}
