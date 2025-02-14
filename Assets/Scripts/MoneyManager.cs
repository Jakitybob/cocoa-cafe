using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores and handles all money
public class MoneyManager : MonoBehaviour, IGameData
{
    private float currentBalance = 1000.0f;

    // Set up default member variables
    void Start()
    {
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

    public void LoadData(GameData data)
    {
        this.currentBalance = data.money;
    }

    public void SaveData(ref GameData data)
    {
        data.money = this.currentBalance;
    }
}
