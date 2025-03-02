using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BaseAI : MonoBehaviour
{
    [SerializeField] public NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        if (navMeshAgent == null)
        {
            Debug.LogError($"NavMeshAgent on {gameObject.name} is null!");
        }
    }
}
