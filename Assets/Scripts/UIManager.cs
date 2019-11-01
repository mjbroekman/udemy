using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _coinCounter;

    void Start()
    {
        InitCoins();
    }

    private void Update()
    {
    }

    private void InitCoins()
    {
        SetCoins(0);
    }

    public void SetCoins(int coins)
    {
        _coinCounter.text = "Coins: " + coins;
    }
}
