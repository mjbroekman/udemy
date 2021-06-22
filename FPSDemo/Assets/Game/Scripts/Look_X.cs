using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Look_X : MonoBehaviour
{
    private GameObject _thePlayer;
    private Player _playerScript;

    // Start is called before the first frame update
    void Start()
    {
        _thePlayer = GameObject.Find("Player");
        _playerScript = _thePlayer.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse input left/right
        float _mouseX = Input.GetAxis("Mouse X");

        // Apply sensitivity (probably set to a game option)
        _mouseX *= _playerScript.GetSensitivity();

        // Rebuild the Vector3 for the localEulerAngles
        Vector3 newFacing = transform.localEulerAngles;
        newFacing.y += _mouseX;
        transform.localEulerAngles = newFacing;
    }
}
