## Set Up
1. Follow the [AR Core Tutorial](https://developers.google.com/ar/develop/unity/quickstart-android)
2. There is a problem when you import the ARCore SDK for Unity if you use the Unity 2019. To fix them, import `Multiplayer HLAPI` and `XR Legacy Input Helpers packages` in package manager. [Reference](https://forum.unity.com/threads/arcore-sdk-console-error-spatialtracking-does-not-exist-in-the-namespace-unityengine.531243/)
3. [Azure Spatial Anchor Service](https://docs.microsoft.com/zh-cn/azure/spatial-anchors/quickstarts/get-started-unity-android)
    Account ID bddc0674-e781-4f45-a151-faf0ae61d9db
    Primary Key lyTTqfiDBt0c4MBXTNq08L71NvIc/qjgxZGpYGfbuSM=
    Note: Azure Spatial Anchor Unity project requires Unity 2019.1.14. Also, the `AR Foundation` package in package manger needs to be installed.
4. Azure Sharing Service Web App
    URL https://sharingservice20190813104550.azurewebsites.net
    Document https://sharingservice20190813104550.azurewebsites.net/index.html
5. Azure Cosmos DB
    Connection String: DefaultEndpointsProtocol=https;AccountName=shared-ar-db;AccountKey=rr3rUn7kia2fjq4tqMY67ZuAzoEOEUt97LXZSrhp16EykUDn4U7NbQIVeNRn4lX3EwCBkfHxrxXnle03dD3dRA==;TableEndpoint=https://shared-ar-db.table.cosmos.azure.com:443/;
6. Azure Spatial Anchor Unity Project Analysis
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