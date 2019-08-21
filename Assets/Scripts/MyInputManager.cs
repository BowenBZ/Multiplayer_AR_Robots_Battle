using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.XR.ARFoundation;
#endif
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class MyInputManager : InputInteractionBase
{
    AllRobotControl clientRobotControl;

    // Start is called before the first frame update
    public override void Start()
    {
        clientRobotControl = GetComponent<AllRobotControl>();
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    Quaternion rotation;

    protected override void OnSelectObjectInteraction(Vector3 hitPoint, object target)
    {
        rotation = Quaternion.AngleAxis(0, Vector3.up);
        clientRobotControl.CreateClientRobot(hitPoint, rotation);
    }
}
