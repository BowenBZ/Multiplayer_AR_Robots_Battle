using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionDetect : MonoBehaviour
{

    RobotControl robotControl;
    Transform thisRoot;
    Transform otherRoot;

    // Start is called before the first frame update
    void Start()
    {
        thisRoot = transform.parent;
        robotControl = thisRoot.gameObject.GetComponent<RobotControl>();
    }

    void OnCollisionEnter(Collision other)
    {
        otherRoot = GetRobotTransform(other.transform);
        if (otherRoot != thisRoot &&
           other.gameObject.name.Contains("Collider") &&
           otherRoot.GetComponent<RobotControl>() != null &&
           otherRoot.GetComponent<RobotControl>().robotStatus == RobotControl.RobotStatus.attack)
        {
            // Debug.Log(other.gameObject.name);
            // GameObject.Find("DebugText").GetComponent<Text>().text = other.gameObject.name;
            robotControl.UpdateHP(1);
        }
    }

    Transform GetRobotTransform(Transform cur)
    {
        if(cur.GetComponent<RobotControl>() || cur.parent == null)
        {
            return cur;
        }
        else
        {
            return GetRobotTransform(cur.parent);
        }
    }
}
