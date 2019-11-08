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

    void Start()
    {
        robotControl = transform.parent.gameObject.GetComponent<RobotControl>();
        HPSlider = transform.Find("HP").GetComponent<Slider>();
        MPSlider = transform.Find("MP").GetComponent<Slider>();
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0.0f);
        HPSlider.value = robotControl.HP;
        MPSlider.value = robotControl.MP;
    }
}