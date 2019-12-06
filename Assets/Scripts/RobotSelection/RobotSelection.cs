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
    enum PlayMode { onlineMode, ARMode, PVEMode };
    PlayMode currentMode;

    Button onlineBtn, ARBtn, PVEBtn;

    // Start is called before the first frame update
    void Start()
    {
        currentIndex = 0;
        InitializeModel();
        StartCoroutine(RobotComeIn());
        currentMode = PlayMode.onlineMode;
        onlineBtn = GameObject.Find("OnlineButton").GetComponent<Button>();
        ARBtn = GameObject.Find("ARButton").GetComponent<Button>();
        PVEBtn = GameObject.Find("PVEButton").GetComponent<Button>();
        onlineBtn.Select();
    }

    void Update()
    {
        SelectBtn();
    }


    IEnumerator RotateRobot()
    {
        while (true)
        {
            presentsObj[currentIndex].transform.Rotate(Vector3.up, 1.0f, Space.World);
            yield return null;
        }
    }

    void InitializeModel()
    {
        presentsObj = new GameObject[prefabList.Length];
        for (int i = 0; i < prefabList.Length; i++)
        {
            presentsObj[i] = GameObject.Instantiate(prefabList[i],
                                                    new Vector3(-1.446266f, 0.0f, 0.0f),
                                                    Quaternion.Euler(0.0f, 90.0f, 0.0f));
            presentsObj[i].SetActive(false);
        }
    }

    IEnumerator RobotComeIn()
    {
        presentsObj[currentIndex].transform.position = new Vector3(-1.446266f, 0.0f, 0.0f);
        presentsObj[currentIndex].transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        presentsObj[currentIndex].SetActive(true);
        Animator anim = presentsObj[currentIndex].GetComponent<Animator>();
        while (presentsObj[currentIndex].transform.position.x < 0)
        {
            anim.SetFloat("Speed", (-presentsObj[currentIndex].transform.position.x + 1.446266f) / (1.446266f + 1.446266f));
            yield return null;
        }
        anim.SetFloat("Speed", 0);
        anim.SetBool("Attack1", true);
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("Attack1", false);
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(RotateRobot());
    }

    public void Next()
    {
        presentsObj[currentIndex].transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        presentsObj[currentIndex].SetActive(false);
        currentIndex = (++currentIndex) % prefabList.Length;
        StopAllCoroutines();
        StartCoroutine(RobotComeIn());
    }

    public void Prev()
    {
        presentsObj[currentIndex].transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        presentsObj[currentIndex].SetActive(false);
        currentIndex = (--currentIndex < 0) ? prefabList.Length - 1 : currentIndex;
        StopAllCoroutines();
        StartCoroutine(RobotComeIn());
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
        else if (currentMode == PlayMode.PVEMode)
        {
            SceneManager.LoadScene("PVEMap1", LoadSceneMode.Single);
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

    public void EnablePVEMode()
    {
        currentMode = PlayMode.PVEMode;
    }

    void SelectBtn()
    {
        if (currentMode == PlayMode.onlineMode)
        {
            onlineBtn.Select();
        }
        else if (currentMode == PlayMode.ARMode)
        {
            ARBtn.Select();
        }
        else if (currentMode == PlayMode.PVEMode)
        {
            PVEBtn.Select();
        }
    }
}
