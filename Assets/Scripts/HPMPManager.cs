using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Control HP and MP bar movement of the robot
/// </summary>
public class HPMPManager : MonoBehaviour
{

    RobotControl robotControl;
    Slider HPSlider;
    Slider MPSlider;

    Transform robotHips;
    Vector3 offset = new Vector3(0f, 1.0f, -0.002244293f);

    void Start()
    {
        robotControl = GetRobotTransform(transform).GetComponent<RobotControl>();
        robotHips = GetRobotTransform(transform).Find("Hips");
        HPSlider = transform.Find("HP").GetComponent<Slider>();
        MPSlider = transform.Find("MP").GetComponent<Slider>();
    }

    void Update()
    {
        transform.position = robotHips.position + offset;
        transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0.0f);
        HPSlider.value = robotControl.HP;
        MPSlider.value = robotControl.MP;
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