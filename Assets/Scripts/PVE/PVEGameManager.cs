using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PVEGameManager : MonoBehaviour
{

    MainSceneRobotSelection robotSelection;     // Robots' prefabs

    [HideInInspector]
    public GameObject clientRobot;

    public GameObject ExitDialog;

    PackManager packManager;

    public Transform enemies;

    // Start is called before the first frame update
    void Start()
    {
        robotSelection = GetComponent<MainSceneRobotSelection>();
        CreateClientRobot();
        ExitDialog.SetActive(false);
        packManager = GetComponent<PackManager>();
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
        Vector2 offset = Random.insideUnitCircle * 10f;
        // clientRobot = GameObject.Instantiate(robotSelection.objects[SceneBridge.clientRobotIndex], new Vector3(offset.x, 5, offset.y), Quaternion.identity);
        clientRobot = GameObject.Instantiate(robotSelection.objects[SceneBridge.clientRobotIndex], new Vector3(0, 0, 0), Quaternion.identity);
        // Enable the control
        clientRobot.GetComponent<RobotControl>().EnableControl();
    }

    /// <summary>
    /// Show the ExitDialog
    /// </summary>
    public void ShowExitDialog()
    {
        if (ExitDialog.activeInHierarchy)
        {
            return;
        }
        else
        {
            ExitDialog.SetActive(true);
            string content = "";
            for(int i = 0; i < packManager.materialCount.Length; i++)
            {
                content += "X        " + packManager.materialCount[i].ToString() + "\n\n\n";
            }
            ExitDialog.transform.Find("Text").GetComponent<Text>().text = content;
        }
    }

    /// <summary>
    /// Go back to gameplay
    /// </summary>
    public void BackToGame()
    {
        ExitDialog.SetActive(false);
    }

    /// <summary>
    /// Exit to main scene
    /// </summary>
    public void ExitToMainScene()
    {
        SceneBridge.ExitToMainScene();
    }
}
