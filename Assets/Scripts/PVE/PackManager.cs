using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager the user's backpack
/// </summary>
public class PackManager : MonoBehaviour
{
    PVEGameManager pveGameManager;

    // Start is called before the first frame update
    void Start()
    {
        pveGameManager = GameObject.Find("Manager").GetComponent<PVEGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPickUp();
    }

    Ray ray;
    RaycastHit hitInfo;
    /// <summary>
    /// Trigger a ray from the touch position to the object
    /// </summary>
    void CheckPickUp()
    {
        // Used for mouse interaction
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            CheckPickUpCore();
        }

        // Used for touch interaction
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            CheckPickUpCore();
        }
    }

    /// <summary>
    /// If target at a material, check the distance and update the pack
    /// </summary>
    void CheckPickUpCore()
    {
        if (Physics.Raycast(ray, out hitInfo, 100.0f, 5))
        {
            SingleMaterial targetMaterial = hitInfo.transform.GetComponent<SingleMaterial>();
            if (targetMaterial && targetMaterial.DetectDistance(pveGameManager.clientRobot.transform))
            {
                targetMaterial.AddToPack();
            }
        }
    }

    [HideInInspector] public int[] materialCount = new int[2];
}
