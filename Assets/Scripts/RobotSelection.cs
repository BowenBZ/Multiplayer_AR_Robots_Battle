using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RobotSelection : MonoBehaviour
{
    public GameObject[] objects;

    [HideInInspector]
    public int currentIndex;
    GameObject currentPresentObj;

    // Start is called before the first frame update
    void Start()
    {
        currentIndex = 0;
        UpdateModel();
    }

    public void Next()
    {
        currentIndex++;
        if (currentIndex >= objects.Length)
            currentIndex = 0;
        UpdateModel();
    }

    public void Prev()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = objects.Length - 1;
        UpdateModel();
    }

    void UpdateModel()
    {
        if (currentPresentObj)
            DestroyImmediate(currentPresentObj);

        currentPresentObj = GameObject.Instantiate(objects[currentIndex],
                                                    new Vector3(0.0f, 0.0f, 0.0f),
                                                    Quaternion.Euler(0.0f, 180.0f, 0.0f));
    }

    public void LoadMainScene()
    {
        SceneBridge.clientRobotIndex = currentIndex;
        SceneManager.LoadScene("ARFighting", LoadSceneMode.Single);
    }
}
