using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVECameraFollower : CameraFollower
{

    PVEGameManager pveGameManager;

    // Start is called before the first frame update
    protected override void Start()
    {
        pveGameManager = GameObject.Find("Manager").GetComponent<PVEGameManager>();
        target = null;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Check whether client robot has been established
    /// </summary>
    protected override void CheckClientRobot()
    {
        if (pveGameManager.clientRobot != null)
        {
            SetTarget(pveGameManager.clientRobot.transform.Find("Hips"));
        }
    }
}
