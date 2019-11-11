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

    // Start is called before the first frame update
    void Start()
    {
        thisRoot = GetRobotTransform(transform);
        robotControl = thisRoot.GetComponent<RobotControl>();
    }

    void OnCollisionEnter(Collision other)
    {
        otherRoot = GetRobotTransform(other.transform);
        if (otherRoot != thisRoot &&
           other.gameObject.name.Contains("Collider") &&
           otherRoot.GetComponent<RobotControl>() != null)
        {
            if (otherRoot.GetComponent<RobotControl>().robotStatus == RobotControl.RobotStatus.attack)
            {
                // GameObject.Find("DebugText").GetComponent<Text>().text = other.gameObject.name;
                robotControl.UpdateHP(1);
            }
            else if(otherRoot.GetComponent<RobotControl>().robotStatus == RobotControl.RobotStatus.skillAttack1)
            {
                robotControl.UpdateHP(3);
            }
            else if(otherRoot.GetComponent<RobotControl>().robotStatus == RobotControl.RobotStatus.skillAttack2)
            {
                robotControl.UpdateHP(5);
                robotControl.BeAttacked();
            }
        }
    }

    Transform GetRobotTransform(Transform cur)
    {
        if (cur.GetComponent<RobotControl>() || cur.parent == null)
        {
            return cur;
        }
        else
        {
            return GetRobotTransform(cur.parent);
        }
    }
}
