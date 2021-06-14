using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _coinCounter;

    [SerializeField]
    private Text _livesCounter;

    [SerializeField]
    private float _worldSpeed;

    void Start()
    {
        _coinCounter = GameObject.Find("Canvas/Coin_text").GetComponent<Text>();
        if (_coinCounter != null)
        {
            _coinCounter.text = "Coins: ";
        }
        _livesCounter = GameObject.Find("Canvas/Lives_text").GetComponent<Text>();
        if (_livesCounter != null)
        {
            _livesCounter.text = "Lives: ";
        }
        _worldSpeed = 1f;
    }

    private void Update()
    {
    }

    public void SetCoins(int coins)
    {
        _coinCounter.text = "Coins: " + coins.ToString();
    }

    public void SetLives(int lives)
    {
        _livesCounter.text = "Lives: " + lives.ToString();
    }

    public void SetWorldSpeed(float newSpeed)
    {
        _worldSpeed = newSpeed;
    }

    public float GetWorldSpeed()
    {
        return _worldSpeed;
    }
}
