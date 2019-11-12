using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kill the object beyond the boundary
/// </summary>
public class KillZone : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        Destroy(other.gameObject);
    }
}
