using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;

/// <summary>
/// Control the movement of the camera to make it follow the client robot
/// </summary>
public class CameraFollower : MonoBehaviour
{
    ObjectsControl objectsControl;
    Transform target;

    // Start is called before the first frame update
    void Start()
    {
        objectsControl = GameObject.Find("Manager").GetComponent<ObjectsControl>();
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            CheckClientRobot();
        }
        else
        {
            UpdateRot();
            UpdatePos();
        }
    }

    /// <summary>
    /// Check whether client robot has been established
    /// </summary>
    void CheckClientRobot()
    {
        if (objectsControl.clientRobot != null)
        {
            SetTarget(objectsControl.clientRobot.transform);
        }
    }

    /// <summary>
    /// Set the target of the robot
    /// </summary>
    /// <param name="robot"></param>
    void SetTarget(Transform robot)
    {
        target = robot;
    }

    // Distance between camera to the target, should support edit in the future
    public float distanceToTarget = 2.0f;

    /// <summary>
    /// Update the position of the camera
    /// </summary>
    void UpdatePos()
    {
        transform.position = target.position - distanceToTarget * transform.forward + 0.5f * transform.up;
    }


    Vector2 look;
    float xRot = -20.0f;
    Vector3 newEulerAngles = new Vector3(0, 0, 0);

    /// <summary>
    /// Update the rotation of camera with mouse input or touch input
    /// </summary>
    void UpdateRot()
    {
        look = TCKInput.GetAxis("Touchpad");
        PlayerRotation(look.x, look.y);
    }

    /// <summary>
    /// Handle the rotation of the camera
    /// </summary>
    /// <param name="horizontal"></param>
    /// <param name="vertical"></param>
    public void PlayerRotation(float horizontal, float vertical)
    {
        transform.Rotate(0f, horizontal * 12f, 0f);

        // Vertical angle can only from -60 to 0
        xRot += vertical * 12f;
        xRot = Mathf.Clamp(xRot, -60f, 0f);
        
        newEulerAngles.x = -xRot;
        newEulerAngles.y = transform.eulerAngles.y;
        newEulerAngles.z = 0.0f;
        
        transform.eulerAngles = newEulerAngles;
    }
}
