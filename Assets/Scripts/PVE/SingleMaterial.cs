using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control the movement of single material box
/// </summary>
public class SingleMaterial : MonoBehaviour
{

    // Material list
    [SerializeField] private Material[] materials;
    // Category
    int category;
    public int Category { get { return category; } }
    // Scale
    Vector2 scaleModifier = new Vector2(0.1f, 0.5f);

    // Start is called before the first frame update
    void Start()
    {
        category = Random.Range(0, materials.Length);
        GetComponent<MeshRenderer>().material = materials[category];
        float scale = Random.Range(scaleModifier.x, scaleModifier.y);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    float allowPickUpDistance = 1.5f;
    public bool DetectDistance(Transform player)
    {
        if(Vector3.Magnitude(transform.position - player.position) < allowPickUpDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
