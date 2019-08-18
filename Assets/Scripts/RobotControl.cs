using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;

public class RobotControl : MonoBehaviour
{
    Animator anim;
    int jumpHash = Animator.StringToHash("Jump");
    float velocityVertical, velocityHorizontal;
    AnimatorStateInfo animatorStateInfo;

    public enum InputType { keyboardInput, mobileInput };
    public InputType inputType;
    public AnimationCurve angleSpeedCurve;  // 0 edgree equals to 0 speed, 180 degree equals to 1 speed


    // Some Middle Parameters
    Vector3 currentDirection;
    Vector3 cameraDirection;
    float offsetAngle;
    Vector2 projectDirection;
    float angleToTurn;
    float currentSpeed;
    float preSpeed;
    float preAngletoTurn;

    void Start()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        // For Keyboard Input
        if (inputType == InputType.keyboardInput)
        {
            velocityVertical = Input.GetAxis("Vertical");
            velocityHorizontal = Input.GetAxis("Horizontal");
            anim.SetFloat("Speed", velocityVertical);
            anim.SetFloat("VelocityVertical", velocityVertical);
            anim.SetFloat("VelocityHorizontal", velocityHorizontal);
        }

        // For Mobile Input
        if (inputType == InputType.mobileInput)
        {
            // Current Robot Forward Direction
            currentDirection = Vector3.ProjectOnPlane(transform.forward, new Vector3(0, 1, 0));
            // Current Camera Forward Direction
            cameraDirection = Vector3.ProjectOnPlane(Camera.main.transform.forward, new Vector3(0, 1, 0));
            // Angle from the forward of robot to the forward of camera
            offsetAngle = Vector2.SignedAngle(new Vector3(0, 1), new Vector2(cameraDirection.x, cameraDirection.z));
            // Rotate the joystick angle with the above angle
            projectDirection = Quaternion.Euler(0, 0, offsetAngle) * TCKInput.GetAxis("Joystick0");

            // Canculate the angle from the robot forward direction to the projected joystick direction
            angleToTurn = Vector2.SignedAngle(new Vector2(currentDirection.x, currentDirection.z),
                                                    projectDirection);

            // Set vertical speed
            currentSpeed = Vector2.SqrMagnitude(TCKInput.GetAxis("Joystick0"));
            if (currentSpeed == 0 && preSpeed != 0)
            {
                preSpeed -= 2.0f * Time.deltaTime;
                anim.SetFloat("Speed", preSpeed);
                anim.SetFloat("VelocityVertical", preSpeed);
                if (preSpeed < 1e-3)
                    preSpeed = 0;
            }
            else
            {
                anim.SetFloat("Speed", currentSpeed);
                anim.SetFloat("VelocityVertical", currentSpeed);
                preSpeed = currentSpeed;
            }

            // Set horizontal speed
            if(angleToTurn == 0 && preAngletoTurn != 0)
            {
                if(preAngletoTurn > 0)
                {
                    preAngletoTurn -= 180.0f * Time.deltaTime;
                }
                else
                {
                    preAngletoTurn += 180.0f * Time.deltaTime;
                }
                angleToTurn = preAngletoTurn;
                if(Mathf.Abs(preAngletoTurn) < 180.0f * Time.deltaTime)
                    preAngletoTurn = 0;
            }
            else
            {
                preAngletoTurn = angleToTurn;
            }

            if (Mathf.Abs(angleToTurn) > 1e-3)
            {
                float convertedHorizontalSpeed = angleSpeedCurve.Evaluate(Mathf.Abs(angleToTurn));
                anim.SetFloat("VelocityHorizontal", -convertedHorizontalSpeed * angleToTurn / Mathf.Abs(angleToTurn));
            }
            else
            {
                anim.SetFloat("VelocityHorizontal", 0);
            }

        }

        // Set action trigger
        animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        animTrigger(Input.GetKeyDown(KeyCode.Space) || TCKInput.GetAction("ButtonJump", EActionEvent.Down),
                    "Jump",
                    "StandJump");
        if (animatorStateInfo.IsName("RunJump"))
            anim.ResetTrigger("Jump");
        animTrigger(Input.GetKeyDown(KeyCode.Alpha1) || TCKInput.GetAction("Button0", EActionEvent.Down),
                    "Attack1",
                    "Attack1-2");
        animTrigger(Input.GetKeyDown(KeyCode.Alpha2) || TCKInput.GetAction("Button1", EActionEvent.Down),
                    "Attack2",
                    "Attack2");
        animTrigger(Input.GetKeyDown(KeyCode.Alpha3) || TCKInput.GetAction("Button2", EActionEvent.Down),
                    "Attack3",
                    "Attack3");
    }

    void animTrigger(bool conditions, string animTriggerName, string animResetName)
    {
        if (conditions)
        {
            anim.SetTrigger(animTriggerName);
        }
        else if (animatorStateInfo.IsName(animResetName))
        {
            anim.ResetTrigger(animTriggerName);
        }
    }
}
