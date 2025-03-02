using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("Wall Sections")]
    [SerializeField] GameObject upperWall;
    [SerializeField] GameObject lowerWall;

    public void DisableUpperWall()
    {
        upperWall.GetComponent<MeshRenderer>().enabled = false;
    }

    public void EnableUpperWall()
    {
        upperWall.GetComponent<MeshRenderer>().enabled = true;
    }
}
