using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Transfer parameters among different scenes
/// </summary>
public static class SceneBridge
{
    public static int clientRobotIndex;
    
    public enum PlayMode { onlineMode, ARMode, PVEMode };
    public static PlayMode playMode;

    public static void ExitToMainScene()
    {
        SceneManager.LoadScene("RobotSelection", LoadSceneMode.Single);
    }
}
