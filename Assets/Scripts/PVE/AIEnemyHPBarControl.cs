using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIEnemyHPBarControl : MonoBehaviour
{
    AIEnemyControl AIEnemyControl;
    protected Slider HPSlider;
    protected Transform robotHips;
    protected Vector3 offset = new Vector3(0f, 1.0f, -0.002244293f);


    // Start is called before the first frame update
    void Start()
    {
        AIEnemyControl = transform.parent.GetComponent<AIEnemyControl>();
        robotHips = transform.parent.Find("Base HumanPelvis");
        HPSlider = transform.Find("HP").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = robotHips.position + offset;
        transform.eulerAngles = new Vector3(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0.0f);
        HPSlider.value = AIEnemyControl.HP;
    }
}
