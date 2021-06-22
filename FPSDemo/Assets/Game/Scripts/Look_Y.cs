using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look_Y : MonoBehaviour
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
        float _mouseY = Input.GetAxis("Mouse Y");

        // Apply sensitivity (probably set to a game option)
        _mouseY *= _playerScript.GetSensitivity();

        // Rebuild the Vector3 for the localEulerAngles
        Vector3 newFacing = transform.localEulerAngles;
        newFacing.x -= _mouseY;
        transform.localEulerAngles = newFacing;
    }
}
