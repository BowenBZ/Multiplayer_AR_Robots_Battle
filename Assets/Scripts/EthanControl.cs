using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EthanControl : MonoBehaviour
{
    Animator anim;
    int rollHash = Animator.StringToHash("Roll");
    int runHash = Animator.StringToHash("Run");
    int jumpHash = Animator.StringToHash("Jump");
    int fightHash = Animator.StringToHash("Fight");



    void Start ()
    {
        anim = GetComponent<Animator>();
    }


    void Update ()
    {
        float move = Input.GetAxis ("Vertical");
        anim.SetFloat("Speed", move);

        AnimatorStateInfo animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if(!animatorStateInfo.IsName("Running") && move > 0)
        {
            anim.SetTrigger (runHash);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger (jumpHash);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetTrigger (rollHash);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim.SetTrigger (fightHash);
        }

    }
}
