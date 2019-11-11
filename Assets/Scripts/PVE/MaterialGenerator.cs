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

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0;i<materialNumber;i++)
        {
            generatePos.x = Random.Range(minX, maxX);
            generatePos.y = mapY;
            generatePos.z = Random.Range(minZ, maxZ);
            GameObject.Instantiate(singleMaterial, generatePos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
