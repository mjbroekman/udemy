using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject _ammoCount;
    private Text _ammoCountText;
    private GameObject _cashCount;
    private Text _moneyTracker;
    private GameObject _coinIcon;

    public void InitUI()
    {
        _ammoCount = GameObject.FindGameObjectWithTag("AmmoCounter");
        _ammoCountText = _ammoCount.GetComponent<Text>();
        _cashCount = GameObject.FindGameObjectWithTag("CashCounter");
        _moneyTracker = _cashCount.GetComponent<Text>();
        _coinIcon = GameObject.FindGameObjectWithTag("Money");
        _coinIcon.SetActive(false);
    }

    public void SetAmmo(int newAmmo)
    {
        // If the _ammoCountText object is not null, update it with the new ammo count
        if (_ammoCountText != null) _ammoCountText.text = "Ammo:\n" + newAmmo.ToString();
    }

    public void SetCash(int newCash)
    {
        if (newCash == 0) { _coinIcon.SetActive(false); }
        else if (newCash > 0) { _coinIcon.SetActive(true); }
        else {
            _coinIcon.SetActive(false); 
            newCash = 0;
        }

        if (_moneyTracker != null) _moneyTracker.text = "Money:\n" + newCash.ToString();
    }

}
