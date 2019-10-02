using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;

public class RobotControl : MonoBehaviour
{
    Animator anim;
    float velocityVertical, velocityHorizontal;
    AnimatorStateInfo animatorStateInfo;

    public enum InputType { keyboardInput, mobileInput };
    public InputType inputType;

    // Some Middle Parameters
    Vector3 currentDirection;
    Vector3 cameraDirection;
    float offsetAngle;
    Vector2 projectDirection;
    float currentSpeed;

    NetworkDataShare networkDataControl;
    NetworkDataShare.RobotMessage msg;

    void Start()
    {
        anim = GetComponent<Animator>();
        networkDataControl = GameObject.Find("Manager").GetComponent<NetworkDataShare>();
        msg = new NetworkDataShare.RobotMessage();
    }


    void Update()
    {
        // Commented when testing the robot locally
        if (gameObject.name != networkDataControl.clientID)
            return;

        // For Keyboard Input - Don't use temporary
        if (inputType == InputType.keyboardInput)
        {
            velocityVertical = Input.GetAxis("Vertical");
            velocityHorizontal = Input.GetAxis("Horizontal");
            anim.SetFloat("Speed", velocityVertical);
            anim.SetFloat("VelocityVertical", velocityVertical);
            anim.SetFloat("VelocityHorizontal", velocityHorizontal);

            // Send data
            msg.Speed = velocityVertical;
        }

        // For Mobile Input
        if (inputType == InputType.mobileInput)
        {
            // Current Robot Forward Direction
            currentDirection = Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0));
            // Current Camera Forward Direction
            cameraDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward, new Vector3(0, 1, 0));
            // Angle from the forward of robot to the forward of camera
            offsetAngle = Vector2.SignedAngle(new Vector3(0, 1), new Vector2(cameraDirection.x, cameraDirection.z));
            // Rotate the joystick angle with the above angle
            projectDirection = Quaternion.Euler(0, 0, offsetAngle) * TCKInput.GetAxis("Joystick0");

            // Set speed
            currentSpeed = Vector2.SqrMagnitude(TCKInput.GetAxis("Joystick0"));
            anim.SetFloat("Speed", currentSpeed);

            // Set direction of robot
            if(currentSpeed > 0)
                this.transform.forward = new Vector3(projectDirection.x, 0, projectDirection.y);
            
            // Send data
            msg.Speed = currentSpeed;
        }

        // Set action trigger
        animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (Input.GetKeyDown(KeyCode.Space) || TCKInput.GetAction("ButtonJump", EActionEvent.Down))
        {
            anim.SetBool("Jump", true);
            msg.Jump = true;
        }
        else if (animatorStateInfo.IsName("RunJump") || animatorStateInfo.IsName("StandJump"))
        {
            anim.SetBool("Jump", false);
            msg.Jump = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || TCKInput.GetAction("Button0", EActionEvent.Down))
        {
            anim.SetBool("Attack1", true);
            msg.Attack1 = true;
        }
        else if (animatorStateInfo.IsName("Attack1") || animatorStateInfo.IsName("Attack1-1") || animatorStateInfo.IsName("Attack1-2"))
        {
            anim.SetBool("Attack1", false);
            msg.Attack1 = false;
        }

        if (animatorStateInfo.IsName("Attack1") && TCKInput.GetAction("Button0", EActionEvent.Down))
        {
            anim.SetBool("Attack1-1", true);
            msg.Attack1_1 = true;
        }
        else if (animatorStateInfo.IsName("Attack1-1") || animatorStateInfo.IsName("Attack1-2"))
        {
            anim.SetBool("Attack1-1", false);
            msg.Attack1_1 = false;
        }

        if (animatorStateInfo.IsName("Attack1-1") && TCKInput.GetAction("Button0", EActionEvent.Down))
        {
            anim.SetBool("Attack1-2", true);
            msg.Attack1_2 = true;
        }
        else if (animatorStateInfo.IsName("Attack1-2"))
        {
            anim.SetBool("Attack1-2", false);
            msg.Attack1_2 = false;
        }


        if (Input.GetKeyDown(KeyCode.Alpha2) || TCKInput.GetAction("Button1", EActionEvent.Down))
        {
            anim.SetBool("Attack2", true);
            msg.Attack2 = true;
        }
        else if (animatorStateInfo.IsName("Attack2"))
        {
            anim.SetBool("Attack2", false);
            msg.Attack2 = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || TCKInput.GetAction("Button2", EActionEvent.Down))
        {
            anim.SetBool("Attack3", true);
            msg.Attack3 = true;
        }
        else if (animatorStateInfo.IsName("Attack3") || animatorStateInfo.IsName("Attack3-1"))
        {
            anim.SetBool("Attack3", false);
            msg.Attack3 = false;
        }

        // Send the data
        msg.localPos = transform.localPosition;
        msg.localRot = transform.localRotation;
        networkDataControl.SendMessagetoServer(msg);
    }
}
