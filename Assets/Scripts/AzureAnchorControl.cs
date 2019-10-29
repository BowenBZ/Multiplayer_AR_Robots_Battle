using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using UnityEngine.SceneManagement;

/// <summary>
/// Control the anchor's movement
/// </summary>
public class AzureAnchorControl : AzureSpatialAnchorsSharedAnchorDemoScript
{
    bool isAnchorSync;

    /// <summary>
    /// Indicate whether finish the anchor sync (upload or download)
    /// </summary>
    /// <value></value>
    public bool IsAnchorSync { get { return isAnchorSync; } }

    // Anchor index
    long anchorIndex;

    // Anchor Object
    GameObject anchorObj;
    public Transform AnchorTransform { get { return anchorObj.transform; } }


    // Start is called before the first frame update
    public override void Start()
    {
#if !UNITY_EDITOR
        base.Start();
#endif
        isAnchorSync = false;
        anchorIndex = -1;
    }

    // Update is called once per frame
    public override void Update()
    {
#if !UNITY_EDITOR
        base.Update();
#endif
    }


    bool allowedPlacingAnchor = false;
    // Only the spawnedObject is null and allowed by users
    protected override bool IsPlacingObject()
    {
        return (allowedPlacingAnchor && spawnedObject == null);
    }

    /// <Summary>
    /// Initialize the session of uploading an anchor
    /// </Summary>
    async Task InitializeUploadSession()
    {
        // Set current anchor to null
        currentCloudAnchor = null;

        // Set spawned object to null;
        spawnedObject = null;

        // Config Session
        ConfigureSession();

        // Start Session
        await CloudManager.StartSessionAsync();

        // Update the guidance text
        currentAppState = AppState.DemoStepCreateLocalAnchor;

        // Enable the user to touch to put anchor
        allowedPlacingAnchor = true;
    }

    /// <Summary>
    /// Initialize the upload session, and then upload the anchor after place the anchor object
    /// </Summary>
    public async Task<long> UploadAnchor()
    {
        await InitializeUploadSession();

        while (spawnedObject == null)
        {
            await Task.Delay(330);
        }

        // Disable the user to touch to put anchor
        allowedPlacingAnchor = false;

        // Save anchor data to cloud
        await SaveCurrentObjectAnchorToCloudAsync();

        // Stop session
        CloudManager.StopSession();

        // Indicate the sync finished
        isAnchorSync = true;

        // // Reset session
        // await CloudManager.ResetSessionAsync();

        // Return the anchor index
        return anchorIndex;
    }

    /// <Summary>
    /// When attached the name to the anchor, store the anchor index
    /// </Summary>
    protected override void AttachTextMesh(GameObject parentObject, long? dataToAttach)
    {
        base.AttachTextMesh(parentObject, dataToAttach);
        anchorObj = parentObject;
        anchorIndex = (long)dataToAttach;
    }

    public void myReturnToLauncher()
    {
        currentCloudAnchor = null;
        spawnedObject = null;
        // currentWatcher = null;
        // CleanupSpawnedObjects();
        // ReturnToLauncher();
        SceneManager.LoadScene("RobotSelection", LoadSceneMode.Single);
    }


#pragma warning disable CS1998 // Conditional compile statements are removing await
    public async Task DownloadAnchor(string anchorIndex)
#pragma warning restore CS1998
    {
        currentAppState = AppState.DemoStepInputAnchorNumber;
        long anchorNumber;
        // string inputText = XRUXPickerForSharedAnchorDemo.Instance.GetDemoInputField().text;

        if (!long.TryParse(anchorIndex, out anchorNumber))
        {
            feedbackBox.text = "Invalid Anchor Number!";
        }
        else
        {
            _anchorNumberToFind = anchorNumber;
#if !UNITY_EDITOR
                _anchorKeyToFind = await anchorExchanger.RetrieveAnchorKey(_anchorNumberToFind.Value);
#endif
            if (_anchorKeyToFind == null)
            {
                feedbackBox.text = "Anchor Number Not Found!";
            }
            else
            {
                _currentDemoFlow = DemoFlow.LocateFlow;
                currentAppState = AppState.DemoStepCreateSession;
                // XRUXPickerForSharedAnchorDemo.Instance.GetDemoInputField().text = "";
            }
        }

        if (currentAppState == AppState.DemoStepCreateSession)
        {
            // Clean previous objects
            CleanupSpawnedObjects();

            // Put cloud anchor to null
            currentCloudAnchor = null;

            // Enable the config session part can add the anchor to find
            currentAppState = AppState.DemoStepCreateSessionForQuery;

            // Config session
            anchorsLocated = 0;
            ConfigureSession();

            // Start session
            await CloudManager.StartSessionAsync();

            // Locate anchors
            currentWatcher = CreateWatcher();
        }
    }

    protected override void OnCloudAnchorLocated(AnchorLocatedEventArgs args)
    {
        if (args.Status == LocateAnchorStatus.Located)
        {
            CloudSpatialAnchor nextCsa = args.Anchor;
            currentCloudAnchor = args.Anchor;

            UnityDispatcher.InvokeOnAppThread(() =>
            {
                anchorsLocated++;
                currentCloudAnchor = nextCsa;
                Pose anchorPose = Pose.identity;

#if UNITY_ANDROID || UNITY_IOS
                anchorPose = currentCloudAnchor.GetPose();
#endif
                // HoloLens: The position will be set based on the unityARUserAnchor that was located.
                GameObject nextObject = SpawnNewAnchoredObject(anchorPose.position, anchorPose.rotation, currentCloudAnchor);
                spawnedObjectMat = nextObject.GetComponent<MeshRenderer>().material;
                AttachTextMesh(nextObject, _anchorNumberToFind);
                otherSpawnedObjects.Add(nextObject);

                if (anchorsLocated >= anchorsExpected)
                {
                    // Stop the session
                    CloudManager.StopSession();
                    // // Reset session
                    // await CloudManager.ResetSessionAsync();
                    isAnchorSync = true;
                }
            });
        }
    }
}
