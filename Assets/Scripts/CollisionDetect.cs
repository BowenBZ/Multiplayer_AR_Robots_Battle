using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Detect the collision among robots when they attack
/// </summary>
public class CollisionDetect : MonoBehaviour
{

    RobotControl robotControl;
    Transform thisRoot;
    Transform otherRoot;
    RobotControl otherRobot;
    AIEnemyControl AIEnemy;


    // Start is called before the first frame update
    void Start()
    {
        thisRoot = GetRobotTransform(transform);
        robotControl = thisRoot.GetComponent<RobotControl>();
    }

    void OnTriggerEnter(Collider other)
    {
        otherRoot = GetRobotTransform(other.transform);

        // For PVP
        otherRobot = otherRoot.GetComponent<RobotControl>();
        if (otherRoot != thisRoot &&
            other.gameObject.tag == "AttackOrgan" &&
            otherRobot != null)
        {
            switch (otherRobot.robotStatus)
            {
                case RobotControl.RobotStatus.attack:
                    robotControl.UpdateHP(1);
                    break;
                case RobotControl.RobotStatus.skillAttack1:
                    robotControl.UpdateHP(3);
                    break;
                case RobotControl.RobotStatus.skillAttack2:
                    robotControl.UpdateHP(5);
                    robotControl.BeAttacked();
                    break;
                default:
                    break;
            }
        }

        // For PVE
        AIEnemy = otherRoot.GetComponent<AIEnemyControl>();
        if (otherRoot != thisRoot &&
            other.gameObject.tag == "AttackOrgan" &&
            AIEnemy != null)
        {
            switch (AIEnemy.currentStatus)
            {
                case AIEnemyControl.RobotStatus.Attack:
                    robotControl.UpdateHP(10);
                    robotControl.BeAttacked();
                    break;
                default:
                    break;
            }
        }
    }

    Transform GetRobotTransform(Transform cur)
    {
        if (cur.GetComponent<RobotControl>() || cur.GetComponent<AIEnemyControl>() || cur.parent == null)
        {
            return cur;
        }
        else
        {
            return GetRobotTransform(cur.parent);
        }
    }
}
