using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawnManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject customerPrefab;
    [SerializeField] Vector3 spawnLocation = new Vector3(0, 1, 0);
    [SerializeField] float spawnDelay = 5f;

    private bool isSpawning = false;
    private int numCustomers = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawning && numCustomers < GameManager.instance.customerLimit)
            StartCoroutine(SpawnCustomer());
    }

    IEnumerator SpawnCustomer()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnDelay);

        Instantiate(customerPrefab, spawnLocation, Quaternion.identity);
        numCustomers++;
        isSpawning = false;
    }
}
