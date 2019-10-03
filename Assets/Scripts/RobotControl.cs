using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;

public class RobotControl : MonoBehaviour
{
    // Animator controller
    Animator anim;
    // Animator controller state
    AnimatorStateInfo animatorStateInfo;

    // Some Middle Parameters
    Vector3 currentDirection;
    Vector3 cameraDirection;
    float offsetAngle;
    Vector2 projectDirection;
    float currentSpeed;

    // Network parameters
    NetworkDataShare networkDataControl;

    NetworkDataShare.RobotMessage msg;

    // Robot action status
    public enum RobotStatus { normal, attack };
    [HideInInspector]
    public RobotStatus robotStatus;

    // Robot HP & MP
    [HideInInspector]
    public float HP, MP;

    void Start()
    {
        // Get the animator controller
        anim = GetComponent<Animator>();
        // Get the network control
        networkDataControl = GameObject.Find("Manager").GetComponent<NetworkDataShare>();
        // Initialize the message
        msg = new NetworkDataShare.RobotMessage();
        // Initialize the robot status
        robotStatus = RobotStatus.normal;
        // Initialize the HP and MP
        HP = 100.0f;
        MP = 100.0f;
    }


    void Update()
    {
#if !UNITY_EDITOR
        if (gameObject.name != networkDataControl.clientID)
            return;
#endif
        // Update the position of robot
        UpdatePos();

        // Update the actions of robot
        UpdateAction();

        // Update HP & MP
        UpdateMP();

        // Send the msg to server after msg is updated by the above 3 functions
        SendMessagetoServer();

        // Set the status of the robot according to the animation state
        UpdateRobotStatus();
    }

    /// <summary>
    /// Update the position and rotation according to the input. 
    /// Trigger different moving animation and store speed parameters.
    /// </summary>
    void UpdatePos()
    {
        if (robotStatus == RobotStatus.normal)
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
            if (currentSpeed > 0)
                this.transform.forward = new Vector3(projectDirection.x, 0, projectDirection.y);
        }
        // Store data
        msg.Speed = currentSpeed;
    }

    /// <summary>
    /// Update the action according to the input. Recover the action after a period of time. 
    /// Update MP and store actions parameters.
    /// </summary>
    void UpdateAction()
    {
        // Set action trigger
        animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // Jump Action
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

        // Normal Attack 1
        if (TCKInput.GetAction("Button0", EActionEvent.Down))
        {
            anim.SetBool("Attack1", true);
            msg.Attack1 = true;
        }
        else if (animatorStateInfo.IsName("Attack1") || animatorStateInfo.IsName("Attack1-1") || animatorStateInfo.IsName("Attack1-2"))
        {
            anim.SetBool("Attack1", false);
            msg.Attack1 = false;
        }

        // Normal Attack 2
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

        // Normal Attack 3
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

        // Attack 2
        if (TCKInput.GetAction("Button1", EActionEvent.Down) && MP > 10.0f)
        {
            // Update animator
            anim.SetBool("Attack2", true);
            // Update msg
            msg.Attack2 = true;
            // Update MP
            if (!animatorStateInfo.IsName("Attack2"))
            {
                MP -= 10.0f;
            }
            // Update msg
            msg.MP = MP;
        }
        else if (animatorStateInfo.IsName("Attack2"))
        {
            anim.SetBool("Attack2", false);
            msg.Attack2 = false;
        }

        // Attack 3
        if (TCKInput.GetAction("Button2", EActionEvent.Down) && MP > 20.0f)
        {
            // Update animator
            anim.SetBool("Attack3", true);
            // Update msg
            msg.Attack3 = true;
            // Update MP
            if (!animatorStateInfo.IsName("Attack3") && !animatorStateInfo.IsName("Attack3-1"))
            {
                MP -= 20.0f;
            }
            // Update msg
            msg.MP = MP;
        }
        else if (animatorStateInfo.IsName("Attack3") || animatorStateInfo.IsName("Attack3-1"))
        {
            anim.SetBool("Attack3", false);
            msg.Attack3 = false;
        }
    }

    /// <summary>
    /// Add HP, Position, Rotation parameters to msg and send them
    /// </summary>
    void SendMessagetoServer()
    {
        msg.HP = HP;
        msg.localPos = transform.localPosition;
        msg.localRot = transform.localRotation;
        networkDataControl.SendMessagetoServer(msg);
    }

    /// <summary>
    /// Update robot status for collsion judgement
    /// </summary>
    void UpdateRobotStatus()
    {
        if (animatorStateInfo.IsName("WalkRun") ||
                    animatorStateInfo.IsName("StandJump") ||
                    animatorStateInfo.IsName("RunJump"))
        {
            robotStatus = RobotStatus.normal;
        }
        else
        {
            robotStatus = RobotStatus.attack;
        }
        msg.robotStatus = (int)robotStatus;
    }

    /// <summary>
    /// Recover MP after a period of time
    /// </summary>
    void UpdateMP()
    {
        if (MP < 100)
        {
            MP += 1.5f * Time.deltaTime;
        }
        else
        {
            MP = 100.0f;
        }
        msg.MP = MP;
    }

    /// <summary>
    /// Update HP when was attacked by others
    /// </summary>
    public void UpdateHP(float harm)
    {
#if !UNITY_EDITOR
        if (gameObject.name != networkDataControl.clientID)
            return;
            
        Handheld.Vibrate();
#endif
        HP -= harm;
    }
}
