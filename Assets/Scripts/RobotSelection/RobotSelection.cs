using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handle the logic of the selection scene in the robot selection scene
/// </summary>
public class RobotSelection : MonoBehaviour
{
    // List of Initialize the prefabs
    public GameObject[] prefabList;
    GameObject[] presentsObj;

    // Current Index
    int currentIndex;

    // Mode parameters
    enum PlayMode { onlineMode, ARMode };
    PlayMode currentMode;

    Button onlineBtn, ARBtn;


    // Start is called before the first frame update
    void Start()
    {
        currentIndex = 0;
        InitializeModel();
        presentsObj[currentIndex].SetActive(true);
        currentMode = PlayMode.onlineMode;
        onlineBtn = GameObject.Find("OnlineButton").GetComponent<Button>();
        ARBtn = GameObject.Find("ARButton").GetComponent<Button>();
        onlineBtn.Select();
    }

    void Update()
    {
        DetectTouchInput();
        SelectBtn();
    }


    Vector2 touchDelPos;
    Vector3 rotateAngle = new Vector3(0, 0, 0);
    /// <Summary>
    /// Detect the touch input 
    /// </Summary>
    void DetectTouchInput()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            touchDelPos = Input.GetTouch(0).deltaPosition;
            rotateAngle.y = -touchDelPos.x * 0.25f;
            presentsObj[currentIndex].transform.Rotate(rotateAngle);
        }
    }

    void InitializeModel()
    {
        presentsObj = new GameObject[prefabList.Length];
        for (int i = 0; i < prefabList.Length; i++)
        {
            presentsObj[i] = GameObject.Instantiate(prefabList[i],
                                                    new Vector3(0.0f, 0.0f, 0.0f),
                                                    Quaternion.Euler(0.0f, 180.0f, 0.0f));
            presentsObj[i].SetActive(false);
        }
    }

    public void Next()
    {
        presentsObj[currentIndex].transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        presentsObj[currentIndex].SetActive(false);
        currentIndex = (++currentIndex) % prefabList.Length;
        presentsObj[currentIndex].SetActive(true);
    }

    public void Prev()
    {
        presentsObj[currentIndex].transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        presentsObj[currentIndex].SetActive(false);
        currentIndex = (--currentIndex < 0) ? prefabList.Length - 1 : currentIndex;
        presentsObj[currentIndex].SetActive(true);
    }

    public void LoadMainScene()
    {
        SceneBridge.clientRobotIndex = currentIndex;
        SceneBridge.playMode = (SceneBridge.PlayMode)currentMode;
        
        if (currentMode == PlayMode.onlineMode)
        {
            SceneManager.LoadScene("OnlineFighting", LoadSceneMode.Single);
        }
        else if (currentMode == PlayMode.ARMode)
        {
            SceneManager.LoadScene("ARFighting", LoadSceneMode.Single);
        }
    }

    public void EnableOnlineMode()
    {
        currentMode = PlayMode.onlineMode;
    }

    public void EnableARMode()
    {
        currentMode = PlayMode.ARMode;
    }

    void SelectBtn()
    {
        if(currentMode == PlayMode.onlineMode)
        {
            onlineBtn.Select();
        }
        else if(currentMode == PlayMode.ARMode)
        {
            ARBtn.Select();
        }
    }
}
