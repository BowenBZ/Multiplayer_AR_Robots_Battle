using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Random generate materials in map
/// </summary>
public class MaterialGenerator : MonoBehaviour
{

    public int materialNumber;
    public GameObject singleMaterial;
    float minX = -36.031f, maxX = 36.61f;
    float minZ = -14.16f, maxZ = 13.76f;
    float mapY = 4.11f;

    Vector3 generatePos = new Vector3(0, 0, 0);
    public Transform materialUpdatePos;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < materialUpdatePos.childCount; i++)
        {
            generatePos = materialUpdatePos.GetChild(i).position;
            generatePos.y -= 2.0f;
            for (int j = 0; j < materialNumber / materialUpdatePos.childCount; j++)
            {
                Vector2 randomOffset = Random.insideUnitCircle * 2;
                generatePos.x += randomOffset.x;
                generatePos.z += randomOffset.y;
                GameObject.Instantiate(singleMaterial, generatePos, Quaternion.identity);
                generatePos.x -= randomOffset.x;
                generatePos.z -= randomOffset.y;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
