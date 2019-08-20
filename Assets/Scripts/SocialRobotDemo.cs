using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;

public class SocialRobotDemo : AzureSpatialAnchorsSharedAnchorDemoScript
{
    private AppState _currentAppState = AppState.DemoStepChooseFlow;
    private DemoFlow _currentDemoFlow = DemoFlow.CreateFlow;

    private AppState currentAppState
    {
        get
        {
            return _currentAppState;
        }
        set
        {
            if (_currentAppState != value)
            {
                Debug.LogFormat("State from {0} to {1}", _currentAppState, value);
                _currentAppState = value;
                // if (spawnedObjectMat != null)
                // {
                //     spawnedObjectMat.color = stateParams[_currentAppState].StepColor;
                // }

                // feedbackBox.text = stateParams[_currentAppState].StepMessage;
                //EnableCorrectUIControls();
            }
        }
    }


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(currentAppState == AppState.DemoStepStopSessionForQuery)
            myLocateflow2Async();
    }


    public bool FinishAnchorSync { get; set; }

    // public async Task myCreateFlow1Async()
    // {
    //     // Start Session
    //     currentCloudAnchor = null;
    //     // Avoid the currentAppState in a wrong state
    //     currentAppState = AppState.DemoStepConfigSession;
    //     // Config Session
    //     ConfigureSession();
    //     // Start Session
    //     await CloudManager.StartSessionAsync();
    //     // Enable the user to touch to put anchor
    //     currentAppState = AppState.DemoStepCreateLocalAnchor;
    // }

    public async Task myCreateFlow2Async()
    {
        if (spawnedObject != null)
        {
            // Disable the user to touch to put anchor
            currentAppState = AppState.DemoStepSaveCloudAnchor;
            // Save anchor data to cloud
            await SaveCurrentObjectAnchorToCloudAsync();
            // Stop session
            CloudManager.StopSession();
            // // Reset session
            // await CloudManager.ResetSessionAsync();
            FinishAnchorSync = true;
        }
    }

    public void myReturnToLauncher()
    {
        currentCloudAnchor = null;
        currentWatcher = null;
        CleanupSpawnedObjects();
        ReturnToLauncher();
        FinishAnchorSync = false;
    }

// #pragma warning disable CS1998 // Conditional compile statements are removing await
//     public async Task myLocateFlow1Async()
// #pragma warning restore CS1998
//     {
//         currentAppState = AppState.DemoStepInputAnchorNumber;
//         long anchorNumber;
//         string inputText = XRUXPickerForSharedAnchorDemo.Instance.GetDemoInputField().text;

//         if (!long.TryParse(inputText, out anchorNumber))
//         {
//             feedbackBox.text = "Invalid Anchor Number!";
//         }
//         else
//         {
//             _anchorNumberToFind = anchorNumber;
// #if !UNITY_EDITOR
//                 _anchorKeyToFind = await anchorExchanger.RetrieveAnchorKey(_anchorNumberToFind.Value);
// #endif
//             if (_anchorKeyToFind == null)
//             {
//                 feedbackBox.text = "Anchor Number Not Found!";
//             }
//             else
//             {
//                 _currentDemoFlow = DemoFlow.LocateFlow;
//                 currentAppState = AppState.DemoStepCreateSession;
//                 XRUXPickerForSharedAnchorDemo.Instance.GetDemoInputField().text = "";
//             }
//         }

//         if (currentAppState == AppState.DemoStepCreateSession)
//         {
//             // Clean previous objects
//             CleanupSpawnedObjects();
//             // Put cloud anchor to null
//             currentCloudAnchor = null;
//             // Enable the config session part can add the anchor to find
//             currentAppState = AppState.DemoStepCreateSessionForQuery;
//             // Config session
//             anchorsLocated = 0;
//             ConfigureSession();
//             // Start session
//             await CloudManager.StartSessionAsync();
//             // Locate anchors
//             currentWatcher = CreateWatcher();
//         }
//     }

    void myLocateflow2Async()
    {
        // Disable buttons
        GameObject.Find("Manager").GetComponent<AnchorUIControl>().AnchorNameSuccess();
        // Stop the session
        CloudManager.StopSession();
        // // Reset session
        // await CloudManager.ResetSessionAsync();
        FinishAnchorSync = true;
    }
}
