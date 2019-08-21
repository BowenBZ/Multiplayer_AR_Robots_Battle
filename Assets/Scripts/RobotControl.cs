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

    NetworkDataShare networkDataControl;
    NetworkDataShare.RobotMessage msg;

    void Start()
    {
        anim = GetComponent<Animator>();
        networkDataControl = GameObject.Find("Manager").GetComponent<NetworkDataShare>();
        msg = new NetworkDataShare.RobotMessage();
    }


    void Update()
    {
        if (gameObject.name != networkDataControl.clientID)
            return;

        // For Keyboard Input
        if (inputType == InputType.keyboardInput)
        {
            velocityVertical = Input.GetAxis("Vertical");
            velocityHorizontal = Input.GetAxis("Horizontal");
            anim.SetFloat("Speed", velocityVertical);
            anim.SetFloat("VelocityVertical", velocityVertical);
            anim.SetFloat("VelocityHorizontal", velocityHorizontal);

            // Send data
            msg.Speed = velocityVertical;
            msg.VelocityVertical = velocityVertical;
            msg.VelocityHorizontal = velocityHorizontal;
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

                // Set data for send
                msg.Speed = preSpeed;
                msg.VelocityVertical = preSpeed;

                if (preSpeed < 1e-3)
                    preSpeed = 0;
            }
            else
            {
                anim.SetFloat("Speed", currentSpeed);
                anim.SetFloat("VelocityVertical", currentSpeed);

                // Set data for send
                msg.Speed = currentSpeed;
                msg.VelocityVertical = currentSpeed;

                preSpeed = currentSpeed;
            }

            // Set horizontal speed
            if (angleToTurn == 0 && preAngletoTurn != 0)
            {
                if (preAngletoTurn > 0)
                {
                    preAngletoTurn -= 180.0f * Time.deltaTime;
                }
                else
                {
                    preAngletoTurn += 180.0f * Time.deltaTime;
                }
                angleToTurn = preAngletoTurn;
                if (Mathf.Abs(preAngletoTurn) < 180.0f * Time.deltaTime)
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

                // Set data to send
                msg.VelocityHorizontal = -convertedHorizontalSpeed * angleToTurn / Mathf.Abs(angleToTurn);
            }
            else
            {
                anim.SetFloat("VelocityHorizontal", 0);

                // Set data to send
                msg.VelocityHorizontal = 0;
            }
        }

        // Set action trigger
        animatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (Input.GetKeyDown(KeyCode.Space) || TCKInput.GetAction("ButtonJump", EActionEvent.Down))
        {
            anim.SetBool("Jump", true);
            msg.Jump = true;
        }
        else if (animatorStateInfo.IsName("RunJump") || animatorStateInfo.IsName("StandJump"))
        {
            anim.SetBool("Jump", false);
            msg.Jump = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || TCKInput.GetAction("Button0", EActionEvent.Down))
        {
            anim.SetBool("Attack1", true);
            msg.Attack1 = true;
        }
        else if (animatorStateInfo.IsName("Attack1") || animatorStateInfo.IsName("Attack1-1") || animatorStateInfo.IsName("Attack1-2"))
        {
            anim.SetBool("Attack1", false);
            msg.Attack1 = false;
        }

        if (animatorStateInfo.IsName("Attack1") && TCKInput.GetAction("Button0", EActionEvent.Down))
        {
            anim.SetBool("Attack1-1", true);
            msg.Attack1_1 = true;
        }
        else if (animatorStateInfo.IsName("Attack1-1") || animatorStateInfo.IsName("Attack1-2"))
        {
            anim.SetBool("Attack1-1", false);
            msg.Attack1_1 = false;
        }

        if (animatorStateInfo.IsName("Attack1-1") && TCKInput.GetAction("Button0", EActionEvent.Down))
        {
            anim.SetBool("Attack1-2", true);
            msg.Attack1_2 = true;
        }
        else if (animatorStateInfo.IsName("Attack1-2"))
        {
            anim.SetBool("Attack1-2", false);
            msg.Attack1_2 = false;
        }


        if (Input.GetKeyDown(KeyCode.Alpha2) || TCKInput.GetAction("Button1", EActionEvent.Down))
        {
            anim.SetBool("Attack2", true);
            msg.Attack2 = true;
        }
        else if (animatorStateInfo.IsName("Attack2"))
        {
            anim.SetBool("Attack2", false);
            msg.Attack2 = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) || TCKInput.GetAction("Button2", EActionEvent.Down))
        {
            anim.SetBool("Attack3", true);
            msg.Attack3 = true;
        }
        else if (animatorStateInfo.IsName("Attack3") || animatorStateInfo.IsName("Attack3-1"))
        {
            anim.SetBool("Attack3", false);
            msg.Attack3 = false;
        }

        // Send the data
        msg.localPos = transform.localPosition;
        msg.localRot = transform.localRotation;
        networkDataControl.SendMessagetoServer(msg);
    }
}
