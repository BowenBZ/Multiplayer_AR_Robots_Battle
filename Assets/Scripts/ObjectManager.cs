using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.XR.ARFoundation;
#endif
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class ObjectManager : InputInteractionBase
{
    public GameObject robotPrefab;
    GameObject robotObj;
    
    AzureSpatialAnchorsSharedAnchorDemoScript anchorControl;

    // Start is called before the first frame update
    public override void Start()
    {
        // Get anchor controller
        anchorControl = GameObject.Find("AzureSpatialAnchors").GetComponent<AzureSpatialAnchorsSharedAnchorDemoScript>();
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    protected override void OnSelectObjectInteraction(Vector3 hitPoint, object target)
    {
        if(!anchorControl.FinishAnchorSync)
            return;

        Quaternion rotation = Quaternion.AngleAxis(0, Vector3.up);
        
        if (robotObj == null)
        {
            // Create the prefab
            robotObj = GameObject.Instantiate(robotPrefab, hitPoint, rotation);
            // Send to server
        }
    }
}
