using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detect the collision caused by others
/// </summary>
public class AIEnemyCollisionControl : MonoBehaviour
{
    AIEnemyControl AIControl;
    Transform thisRoot;
    Transform otherRoot;
    RobotControl otherRobot;

    // Start is called before the first frame update
    void Start()
    {
        thisRoot = transform.parent;
        AIControl = thisRoot.GetComponent<AIEnemyControl>();
    }   

    void OnTriggerEnter(Collider other)
    {
        OnCollisionOrTrigger(other);
    }

    void OnTriggerStay(Collider other)
    {
        // OnCollisionOrTrigger(other);
    }

    void OnCollisionOrTrigger(Collider other)
    {
        otherRoot = GetRobotTransform(other.transform);
        otherRobot = otherRoot.GetComponent<RobotControl>();

        if (otherRoot != thisRoot &&
            other.gameObject.tag == "AttackOrgan" &&
            otherRobot != null)
        {
            switch (otherRobot.robotStatus)
            {
                case RobotControl.RobotStatus.attack:
                    AIControl.UpdateUP(1);
                    break;
                case RobotControl.RobotStatus.skillAttack1:
                    AIControl.UpdateUP(10, true, 0);
                    break;
                case RobotControl.RobotStatus.skillAttack2:
                    AIControl.UpdateUP(3, true, 1);
                    break;
                default:
                    break;
            }
        }
    }

    Transform GetRobotTransform(Transform cur)
    {
        if (cur.GetComponent<RobotControl>() ||
            cur.GetComponent<AIEnemyControl>() ||
            cur.parent == null)
        {
            return cur;
        }
        else
        {
            return GetRobotTransform(cur.parent);
        }
    }
}
