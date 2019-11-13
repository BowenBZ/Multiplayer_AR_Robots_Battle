using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control the movement of the AI enemy
/// </summary>
public class AIEnemyControl : MonoBehaviour
{

    
    Animator anim;                          // Animator controller
    AnimatorStateInfo animatorStateInfo;    // Animator controller state

    // Robot Status
    public enum RobotStatus { Normal, Attack, Beaten, Die };
    [HideInInspector] public RobotStatus currentStatus = RobotStatus.Normal;

    [HideInInspector] public float HP = 100.0f, MP = 100.0f;        // HP and MP value

    public GameObject harmText;
    public GameObject lockIcon;

    // Start is called before the first frame update
    void Start()
    {
        // Get the animator controller
        anim = GetComponent<Animator>();
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

        // Update the robot status
        UpdateStatus();
    }

    void UpdateStatus()
    {
        if(animatorStateInfo.IsTag("Normal"))
        {
            currentStatus = RobotStatus.Normal;
        }
        else if(animatorStateInfo.IsTag("Attack"))
        {
            currentStatus = RobotStatus.Attack;
        }
        else if(animatorStateInfo.IsTag("Beaten"))
        {
            currentStatus = RobotStatus.Beaten;
        }
    }

    public void UpdateUP(int deltaHP, bool causeStill = false)
    {
        HP -= deltaHP;
        harmText.transform.GetComponent<Text>().text = "-" + deltaHP.ToString();
        harmText.SetActive(false);
        harmText.SetActive(true);
        if(causeStill)
        {
            anim.SetTrigger("AttackedStill");
        }
    }
}
