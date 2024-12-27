using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores and handles all money
public class MoneyManager : MonoBehaviour
{
    [SerializeField]
    private float startingFunds = 0f;
    private float currentBalance;

    // Set up default member variables
    void Start()
    {
        currentBalance = startingFunds;
        GameManager.instance.interfaceManager.UpdateBalanceText(currentBalance);
    }

    // Adds money to the total balance
    public void AddMoney(float money)
    {
        currentBalance += money;
        GameManager.instance.interfaceManager.UpdateBalanceText(currentBalance);
    }

    // Returns the float of the current balance
    public float GetMoney()
    {
        return currentBalance;
    }
}
