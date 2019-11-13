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

    RobotMessage.Message msg = new RobotMessage.Message();

    // Robot action status
    public enum RobotStatus { normal, jump, attack, skillAttack1, skillAttack2, hit };

    [HideInInspector]
    public RobotStatus robotStatus = RobotStatus.normal;

    // Robot HP & MP
    [HideInInspector]
    public float HP, MP;

    // Network parameters
    bool isClientRobot = false;

    // Attack Lock
    bool AttackLock = false;

    PVEGameManager pveManager;

    void Start()
    {
        // Get the animator controller
        anim = GetComponent<Animator>();
        // Get the network control
        roomControl = GameObject.Find("Manager").GetComponent<RoomControl>();
        // Get PVE manager for pve mode
        pveManager = GameObject.Find("Manager").GetComponent<PVEGameManager>();
        // Set HP and MP
        HP = 100.0f;
        MP = 100.0f;
    }

    void FixedUpdate()
    {
        // Only allow the client to control its robot
        if (!isClientRobot)
        {
            return;
        }

        // Get current animator state
        animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);


        // Update the position of robot
        UpdatePos();

        // Get nearest enemy if needed
        GetNearsetEnemy();

        // Update the actions of robot
        UpdateAction();


        // Update HP & MP
        UpdateMP();

        // Send the msg to server after msg is updated by the above 3 functions
        SendMessagetoServer();
    }

    /// <summary>
    /// Enable local client to control this robot
    /// </summary>
    public void EnableControl()
    {
        isClientRobot = true;
    }

    Vector2 joystickInput = new Vector2(0, 0);
    /// <summary>
    /// Update the position and rotation according to the input. 
    /// Trigger different moving animation and store speed parameters.
    /// </summary>
    void UpdatePos()
    {
        // The robot can be control only if it is in normal mode
        if (robotStatus == RobotStatus.normal)
        {
            joystickInput = TCKInput.GetAxis("Joystick0");
            if (Vector2.SqrMagnitude(joystickInput) == 0)
            {
                joystickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }

            // Set speed
            currentSpeed = (Vector2.SqrMagnitude(joystickInput) > 0) ? 1.0f : 0.0f;
            anim.SetFloat("Speed", currentSpeed);

            // Set direction of robot
            if (currentSpeed > 0 && robotStatus == RobotStatus.normal)
            {
                // Current Robot Forward Direction
                currentDirection = Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0));
                // Current Camera Forward Direction
                cameraDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward, new Vector3(0, 1, 0));
                // Angle from the forward of robot to the forward of camera
                offsetAngle = Vector2.SignedAngle(new Vector3(0, 1), new Vector2(cameraDirection.x, cameraDirection.z));
                // Rotate the joystick angle with the above angle
                projectDirection = Quaternion.Euler(0, 0, offsetAngle) * joystickInput;
                // Set the forward direction
                this.transform.forward = new Vector3(projectDirection.x, 0, projectDirection.y);
            }
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
        // Users can operate the robot only if they are not in hit status
        if (robotStatus != RobotStatus.hit)
        {
            // Jump Action
            if (Input.GetKeyDown(KeyCode.Space) || TCKInput.GetAction("ButtonJump", EActionEvent.Down))
            {
                anim.SetBool("Jump", true);
                msg.Jump = true;
            }
            else if (animatorStateInfo.IsTag("Jump"))
            {
                anim.SetBool("Jump", false);
                msg.Jump = false;
            }

            // If currently in the attacking mode, correction the direction
            if (animatorStateInfo.IsTag("Attack") || animatorStateInfo.IsTag("SkillAttack1") || animatorStateInfo.IsTag("SkillAttack2"))
            {
                CorrectAttackDirection();
            }

            // Normal Attack 1
            if (animatorStateInfo.IsName("WalkRun") && TCKInput.GetAction("Button0", EActionEvent.Down))
            {
                // CorrectAttackDirection();
                anim.SetBool("Attack1", true);
                msg.Attack1 = true;
            }
            else if (animatorStateInfo.IsTag("Attack"))
            {
                anim.SetBool("Attack1", false);
                msg.Attack1 = false;
            }

            // Normal Attack 1-1
            if (animatorStateInfo.IsName("Attack1") && TCKInput.GetAction("Button0", EActionEvent.Down))
            {
                anim.SetBool("Attack1-1", true);
                msg.Attack1_1 = true;
            }
            else if (animatorStateInfo.IsTag("Attack") && !animatorStateInfo.IsName("Attack1"))
            {
                anim.SetBool("Attack1-1", false);
                msg.Attack1_1 = false;
            }

            // Normal Attack 1-2
            if (animatorStateInfo.IsName("Attack1-1") && TCKInput.GetAction("Button0", EActionEvent.Down))
            {
                anim.SetBool("Attack1-2", true);
                msg.Attack1_2 = true;
            }
            else if (animatorStateInfo.IsTag("Attack") && !animatorStateInfo.IsName("Attack1-1"))
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
                if (!animatorStateInfo.IsTag("SkillAttack1"))
                {
                    MP -= 20.0f;
                }
                // Update msg
                msg.MP = MP;
            }
            else if (animatorStateInfo.IsTag("SkillAttack1"))
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
                if (!animatorStateInfo.IsTag("SkillAttack2"))
                {
                    MP -= 40.0f;
                }
                // Update msg
                msg.MP = MP;
            }
            else if (animatorStateInfo.IsTag("SkillAttack2"))
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
        if (animatorStateInfo.IsTag("Normal"))
        {
            robotStatus = RobotStatus.normal;
        }
        else if (animatorStateInfo.IsTag("Jump"))
        {
            robotStatus = RobotStatus.jump;
        }
        else if (animatorStateInfo.IsTag("Attack"))
        {
            robotStatus = RobotStatus.attack;
        }
        else if (animatorStateInfo.IsTag("SkillAttack1"))
        {
            robotStatus = RobotStatus.skillAttack1;
        }
        else if (animatorStateInfo.IsTag("SkillAttack2"))
        {
            robotStatus = RobotStatus.skillAttack2;
        }
        else if (animatorStateInfo.IsTag("Beaten"))
        {
            robotStatus = RobotStatus.hit;
        }

        // Store robot status into msg
        msg.robotStatus = (int)robotStatus;
    }

    /// <summary>
    /// Add HP, Position, Rotation parameters to msg and send them
    /// </summary>
    void SendMessagetoServer()
    {
        msg.HP = HP;
        msg.localPos = transform.localPosition;
        msg.localRot = transform.localRotation;
        if (roomControl != null)
        {
            roomControl.SendMessagetoServer(msg);
        }
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
        if (!isClientRobot)
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
        if (!isClientRobot)
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

    Transform lockedEnemy;
    float lockEnemyDistance = 3.0f;

    /// <summary>
    /// Detect the nearest enemy and set it to lockedEnemy
    /// </summary>
    void GetNearsetEnemy()
    {
        // Update the state
        if (TCKInput.GetAction("AttackLock", EActionEvent.Down))
        {
            AttackLock = !AttackLock;
        }

        if (!AttackLock)
        {
            if (lockedEnemy != null)
            {
                lockedEnemy.GetComponent<AIEnemyControl>().lockIcon.SetActive(false);
                lockedEnemy = null;
            }
            return;
        }

        if (lockedEnemy != null)
        {
            if (Vector3.Magnitude(lockedEnemy.position - transform.position) > lockEnemyDistance)
            {
                lockedEnemy.GetComponent<AIEnemyControl>().lockIcon.SetActive(false);
                lockedEnemy = null;
            }
        }
        else
        {
            // Needs to be modified here
            if (SceneBridge.playMode == SceneBridge.PlayMode.PVEMode || SceneBridge.playMode == SceneBridge.PlayMode.onlineMode)
            {
                for (int i = 0; i < pveManager.enemies.childCount; i++)
                {
                    if (Vector3.Magnitude(pveManager.enemies.GetChild(i).position - transform.position) < lockEnemyDistance)
                    {
                        lockedEnemy = pveManager.enemies.GetChild(i);
                        lockedEnemy.GetComponent<AIEnemyControl>().lockIcon.SetActive(true);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Correct the direction of robot
    /// </summary>
    void CorrectAttackDirection()
    {
        if (!AttackLock || lockedEnemy == null)
        {
            return;
        }

        Vector3 tmp = lockedEnemy.transform.position - transform.position;
        this.transform.forward = new Vector3(tmp.x, 0, tmp.z);
    }
}
