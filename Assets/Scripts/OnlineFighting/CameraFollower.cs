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
    protected Transform target;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        objectsControl = GameObject.Find("Manager").GetComponent<ObjectsControl>();
        target = null;
    }

    // Update is called once per frame
    protected virtual void Update()
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
    protected virtual void CheckClientRobot()
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
    protected void SetTarget(Transform robot)
    {
        target = robot;
    }

    // Distance between camera to the target, should support edit in the future
    public float distanceToTarget = 2.7f;

    /// <summary>
    /// Update the position of the camera
    /// </summary>
    protected void UpdatePos()
    {
        transform.position = target.position - distanceToTarget * transform.forward + 0.5f * transform.up;
    }


    protected Vector2 look;
    protected float xRot = -20.0f;
    protected Vector3 newEulerAngles = new Vector3(0, 0, 0);

    /// <summary>
    /// Update the rotation of camera with mouse input or touch input
    /// </summary>
    protected void UpdateRot()
    {
        look = TCKInput.GetAxis("Touchpad");
        PlayerRotation(look.x, look.y);
    }

    /// <summary>
    /// Handle the rotation of the camera
    /// </summary>
    /// <param name="horizontal"></param>
    /// <param name="vertical"></param>
    protected void PlayerRotation(float horizontal, float vertical)
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
