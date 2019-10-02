
# Social Robot AR

This repo is the AR social functions of L33tKidz's launch product. It is based on AR Foundation and AzureSpatialAnchor. It aims to build a multi-platform AR shared experience including ARCore(Android), ARKit(IOS), and WMR(HoloLens). Currently, it is tested on ARCore platform only.

## Environment

* Unity 2019.1.14f1
* Android 9.+(Q)

## Problems during setting Up

1. Follow the [AR Core Tutorial](https://developers.google.com/ar/develop/unity/quickstart-android)
2. There is a problem when you import the ARCore SDK for Unity if you use the Unity 2019. To fix them, import `Multiplayer HLAPI` and `XR Legacy Input Helpers packages` in package manager. [Reference](https://forum.unity.com/threads/arcore-sdk-console-error-spatialtracking-does-not-exist-in-the-namespace-unityengine.531243/)
3. [Azure Spatial Anchor Service](https://docs.microsoft.com/zh-cn/azure/spatial-anchors/quickstarts/get-started-unity-android)
    * Account ID 6616fce2-1b28-4fed-8ba9-c0bfb5acb35b
    * Primary Key AWFhKCI/4SBNs+M9ZrHuuD3KT+9bxoLDTJZmokyRxPM=
    * Note: Azure Spatial Anchor Unity project requires Unity 2019.1.14. Also, the `AR Foundation` package in package manger needs to be installed.
4. Azure Sharing Service Web App
    * URL https://sharingserviceforsocialar.azurewebsites.net
5. Azure Cosmos DB
    * Connection String: DefaultEndpointsProtocol=https;
    * AccountName=shared-ar-db;
    * AccountKey=rr3rUn7kia2fjq4tqMY67ZuAzoEOEUt97LXZSrhp16EykUDn4U7NbQIVeNRn4lX3EwCBkfHxrxXnle03dD3dRA==;TableEndpoint=https://shared-ar-db.table.cosmos.azure.com:443/;

## Azure Spatial Anchor Unity Project Analysis
* Create
    * Create anchor and share it
    * AzureSpatialAnchorsSharedAnchorDemoScript.cs InitializeCreateFlowDemo()
    * Steps:
        * Change the currentAppState
        * Trigger EnableCorrectUIControls()
        * Trigger AdvanceDemoAsync() ?
        * Trigger AdvanceCreateFlowDemoAsync()
* Load
    * Download and match
    * AzureSpatialAnchorsSharedAnchorDemoScript.cs InitializeLocateFlowDemo()
* Exit
    * Reload the scene
    * DemoScriptBase.cs ReturnToLauncher()