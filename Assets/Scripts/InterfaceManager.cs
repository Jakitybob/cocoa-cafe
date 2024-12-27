using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text moneyText;

    public void UpdateBalanceText(float money)
    {
        moneyText.text = "$" + money.ToString();
    }
}
