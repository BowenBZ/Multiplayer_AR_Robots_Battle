using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transfer parameters among different scenes
/// </summary>
public static class SceneBridge
{
    public static int clientRobotIndex;
    
    public enum PlayMode { onlineMode, ARMode };
    public static PlayMode playMode;
}
