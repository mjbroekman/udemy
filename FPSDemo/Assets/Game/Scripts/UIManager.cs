using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject _ammoCount;
    private Text _ammoCountText;

    public void InitUI()
    {
        _ammoCount = GameObject.FindGameObjectWithTag("AmmoCounter");
        _ammoCountText = _ammoCount.GetComponent<Text>();
    }

    public void SetAmmo(int newAmmo)
    {
        // If the _ammoCountText object is not null, update it with the new ammo count
        if (_ammoCountText != null) _ammoCountText.text = "Ammo: " + newAmmo.ToString();
    }
}
