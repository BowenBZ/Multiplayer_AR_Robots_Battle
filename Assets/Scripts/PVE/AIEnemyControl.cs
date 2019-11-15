using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/// <summary>
/// Control the movement of the AI enemy
/// </summary>
public class AIEnemyControl : MonoBehaviour
{
    PVEGameManager pveGameManager;

    Animator anim;                          // Animator controller
    AnimatorStateInfo animatorStateInfo;    // Animator controller state

    // Robot Status
    public enum RobotStatus { Normal, Attack, Beaten, Die };
    [HideInInspector] public RobotStatus currentStatus = RobotStatus.Normal;

    [HideInInspector] public float HP = 100.0f, MP = 100.0f;        // HP and MP value

    GameObject harmText;
    [HideInInspector] public GameObject lockIcon;

    NavMeshAgent navi;

    // Start is called before the first frame update
    void Start()
    {
        pveGameManager = GameObject.Find("Manager").GetComponent<PVEGameManager>();

        // Get the animator controller
        anim = GetComponent<Animator>();

        CheckPoints = new Transform[CheckPointsCollections.childCount];
        for (int i = 0; i < CheckPointsCollections.childCount; i++)
        {
            CheckPoints[i] = CheckPointsCollections.GetChild(i);
        }

        navi = GetComponent<NavMeshAgent>();
        navi.updatePosition = true;
        navi.updateRotation = false;

        harmText = transform.Find("HPMP").Find("HarmText").gameObject;
        
        lockIcon = transform.Find("HPMP").Find("LockArrow").gameObject;
        lockIcon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            anim.SetTrigger("Attack");
        }


        // Get current animatorStateInfo 
        animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // Set the player
        SetPlayer();

        // Let the AI Enemy walk in the environment
        // WalkInEnvironment();

        // Detect the player
        DetectPlayer();

        // Update the robot status
        UpdateStatus();


    }

    /// <summary>
    /// Update the status of the AI Enemy
    /// </summary>
    void UpdateStatus()
    {
        if (animatorStateInfo.IsTag("Normal"))
        {
            currentStatus = RobotStatus.Normal;
        }
        else if (animatorStateInfo.IsTag("Attack"))
        {
            currentStatus = RobotStatus.Attack;
        }
        else if (animatorStateInfo.IsTag("Beaten"))
        {
            currentStatus = RobotStatus.Beaten;
        }
    }

    /// <summary>
    /// Update the HP when Attacked
    /// </summary>
    /// <param name="deltaHP"></param>
    /// <param name="causeStill"></param>
    /// <param name="stillType"></param>
    public void UpdateHP(int deltaHP, bool causeStill = false, int stillType = 0)
    {
        HP -= deltaHP;
        harmText.transform.GetComponent<Text>().text = "-" + deltaHP.ToString();
        harmText.SetActive(false);
        harmText.SetActive(true);
        if (causeStill)
        {
            switch (stillType)
            {
                case 0:
                    anim.SetTrigger("AttackedStill");
                    break;
                case 1:
                    anim.SetTrigger("SkillAttackedStill");
                    break;
                default:
                    break;
            }
        }
    }

    Transform player;

    /// <summary>
    /// Get the transform of the player and store it
    /// </summary>
    void SetPlayer()
    {
        if (player == null && pveGameManager.clientRobot != null)
        {
            player = pveGameManager.clientRobot.transform;
        }
    }


    float distanceToPlayer;
    int checkingPoint = 0;
    public Transform CheckPointsCollections;
    Transform[] CheckPoints;

    bool switchFlag = false;
    /// <summary>
    /// Walk in the environment when didn't find the player
    /// </summary>
    void WalkInEnvironment()
    {
        // If it is in the walking status
        if (animatorStateInfo.IsName("WalkRun"))
        {
            anim.ResetTrigger("Move");

            // When it approach the destination
            if (Vector3.Magnitude(CheckPoints[checkingPoint].position - transform.position) <= 1.0f || !navi.hasPath)
            {
                // Covert to the Idle status
                anim.SetTrigger("Idle");

                // Shut down the speed
                navi.speed = 0.0f;

                // Switch to next position
                if (switchFlag)
                {
                    checkingPoint = (checkingPoint + 1) % CheckPoints.Length;
                    switchFlag = false;
                }
            }
            else
            {
                // Set the max speed
                navi.speed = 1.0f;

                // Update the animation
                anim.SetFloat("Speed", Vector3.Magnitude(navi.velocity));

                // Update the rot according to the agent
                if (Vector3.Magnitude(navi.velocity) != 0)
                {
                    CorrectDirection(navi.velocity);
                    transform.forward = directionVector;
                }
            }

        }
        else if (animatorStateInfo.IsName("LookAround"))
        {
            anim.ResetTrigger("Idle");

            navi.speed = 0.0f;
            navi.SetDestination(CheckPoints[checkingPoint].position);
            anim.SetTrigger("Move");
            switchFlag = true;
        }
        else
        {
            anim.SetTrigger("Idle");
        }
        // Debug.Log(checkingPoint);
    }

    void OnAnimatorMove()
    {
        // transform.position = navi.nextPosition;
    }

    Vector3 directionVector;
    float maxDetectionDistance = 5.0f;
    float minDetectionDistance = 0.5f;
    float minDetectionDistance_Move = 0.5f;
    float minDetectionDistance_Attack = 1.0f;
    /// <summary>
    /// Detect the distance to the player and update the behavior
    /// </summary>
    void DetectPlayer()
    {
        // Check the distance to the player
        CorrectDirection(player);
        distanceToPlayer = Vector3.Magnitude(directionVector);

        // Player is out of the detection distance
        if (distanceToPlayer > maxDetectionDistance)
        {
            navi.enabled = true;
            WalkInEnvironment();
        }
        // Detect the player and chase the player
        else if (minDetectionDistance <= distanceToPlayer && distanceToPlayer <= maxDetectionDistance)
        {
            minDetectionDistance = minDetectionDistance_Move;
            navi.enabled = true;
            if (animatorStateInfo.IsName("Chase"))
            {
                anim.ResetTrigger("StartChase");

                navi.SetDestination(player.position);
                // Set the max speed
                navi.speed = 3.0f;
                // Update the animation
                anim.SetFloat("Speed", Vector3.Magnitude(navi.velocity));
                // Update the rot according to the agent
                if (Vector3.Magnitude(navi.velocity) != 0)
                {
                    CorrectDirection(navi.velocity);
                    transform.forward = directionVector;
                }
            }
            else
            {
                anim.SetTrigger("StartChase");
            }
        }
        else if (distanceToPlayer < minDetectionDistance)
        {
            minDetectionDistance = minDetectionDistance_Attack;

            // Shut down the speed and navi
            navi.speed = 0.0f;
            navi.enabled = false;

            // Shut down the animation
            anim.SetFloat("Speed", 0);

            // Update the direction
            CorrectDirection(player);
            transform.forward = directionVector;

            // Start to fight
            if (!animatorStateInfo.IsTag("Attack"))
            {
                anim.SetTrigger("Attack");
            }
            else
            {
                anim.ResetTrigger("Attack");
            }
        }
    }

    /// <summary>
    /// Correct the direction Vector
    /// </summary>
    /// <param name="target"></param>
    void CorrectDirection(Transform target)
    {
        directionVector = target.position - transform.position;
        directionVector.y = 0;
    }

    /// <summary>
    /// Correct the direction Vector
    /// </summary>
    /// <param name="target"></param>
    void CorrectDirection(Vector3 dir)
    {
        directionVector = dir;
        directionVector.y = 0;
    }
}
