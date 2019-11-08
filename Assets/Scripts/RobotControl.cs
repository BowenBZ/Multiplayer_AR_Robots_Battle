using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;

/// <summary>
/// Control the behaviors of robot
/// </summary>
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
    RoomControl roomControl;

    RobotMessage.Message msg;

    // Robot action status
    public enum RobotStatus { normal, attack, skillAttack1, skillAttack2, hit };
    [HideInInspector]
    public RobotStatus robotStatus;

    // Robot HP & MP
    [HideInInspector]
    public float HP, MP;

    // Network parameters
    bool isClientRobot;

    void Start()
    {
        // Get the animator controller
        anim = GetComponent<Animator>();
        // Get the network control
        roomControl = GameObject.Find("Manager").GetComponent<RoomControl>();
        // Initialize the message
        msg = new RobotMessage.Message();
        // Initialize the robot status
        robotStatus = RobotStatus.normal;
        // Initialize the HP and MP
        HP = 100.0f;
        MP = 100.0f;
        // Initialize the network parameters
        isClientRobot = (transform.gameObject == GameObject.Find("Manager").GetComponent<ObjectsControl>().clientRobot);
    }


    void Update()
    {
        // Only allow the client to control its robot
        if(!isClientRobot)
        {
            return;
        }

        // Update the position of robot
        UpdatePos();

        // Update the actions of robot
        UpdateAction();

        // Update HP & MP
        UpdateMP();

        // Send the msg to server after msg is updated by the above 3 functions
        SendMessagetoServer();
    }

    /// <summary>
    /// Update the position and rotation according to the input. 
    /// Trigger different moving animation and store speed parameters.
    /// </summary>
    void UpdatePos()
    {
        // The robot can be control only if it is in normal mode
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
        // Get current animator state
        animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // Users can operate the robot only if they are not in hit status
        if (robotStatus != RobotStatus.hit)
        {
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
            if (animatorStateInfo.IsName("WalkRun") && TCKInput.GetAction("Button0", EActionEvent.Down))
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
            if (animatorStateInfo.IsName("WalkRun") && TCKInput.GetAction("Button1", EActionEvent.Down) && MP > 20.0f)
            {
                // Update animator
                anim.SetBool("Attack2", true);
                // Update msg
                msg.Attack2 = true;
                // Update MP
                if (!animatorStateInfo.IsName("Attack2"))
                {
                    MP -= 20.0f;
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
            if (animatorStateInfo.IsName("WalkRun") && TCKInput.GetAction("Button2", EActionEvent.Down) && MP > 40.0f)
            {
                // Update animator
                anim.SetBool("Attack3", true);
                // Update msg
                msg.Attack3 = true;
                // Update MP
                if (!animatorStateInfo.IsName("Attack3") && !animatorStateInfo.IsName("Attack3-1"))
                {
                    MP -= 40.0f;
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
        else if (robotStatus == RobotStatus.hit)
        {
            // Recovery from the hit status 
            if (animatorStateInfo.IsName("Hit"))
            {
                anim.SetBool("Hit", false);
                msg.Hit = false;
            }
        }

        // Update the robot status according to the action status
        if (animatorStateInfo.IsName("WalkRun") ||
            animatorStateInfo.IsName("StandJump") ||
            animatorStateInfo.IsName("RunJump"))
        {
            robotStatus = RobotStatus.normal;
        }
        else if (animatorStateInfo.IsName("Attack1") ||
                animatorStateInfo.IsName("Attack1-1") ||
                animatorStateInfo.IsName("Attack1-2"))
        {
            robotStatus = RobotStatus.attack;
        }
        else if (animatorStateInfo.IsName("Attack2"))
        {
            robotStatus = RobotStatus.skillAttack1;
        }
        else if (animatorStateInfo.IsName("Attack3") ||
                animatorStateInfo.IsName("Attack3-1"))
        {
            robotStatus = RobotStatus.skillAttack2;
        }
        else if (animatorStateInfo.IsName("Hit"))
        {
            robotStatus = RobotStatus.hit;
        }

        // Store robot status into msg
        msg.robotStatus = (int)robotStatus;
        // Debug.Log(animatorStateInfo);
    }

    /// <summary>
    /// Add HP, Position, Rotation parameters to msg and send them
    /// </summary>
    void SendMessagetoServer()
    {
        msg.HP = HP;
        msg.localPos = transform.localPosition;
        msg.localRot = transform.localRotation;
        roomControl.SendMessagetoServer(msg);
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
        // Only allow the HP to be updated by the client itself
        if(!isClientRobot)
        {
            return;
        }

        // Don't do this because it causes anchor floating
        // Handheld.Vibrate();

        // Decrase HP
        HP -= harm;
    }

    // Was attacked by other's skill #2
    public void BeAttacked()
    {
        // Only allow the Animation control by itself
        if(!isClientRobot)
        {
            return;
        }

        // Set hit action
        anim.SetBool("Hit", true);
        // Update robot status
        robotStatus = RobotStatus.hit;
        // Store into msg
        msg.Hit = true;
    }
}
