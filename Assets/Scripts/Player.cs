using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _p_control;
    [SerializeField]
    private float _hInput;
    [SerializeField]
    private float _walkSpd;

    void Start()
    {
        _p_control = GetComponent<CharacterController>();
        _walkSpd = 1.5f;
    }

    void Update()
    {
        _hInput = Input.GetAxis("Horizontal");
        float _speed = _walkSpd;
        if (Input.GetKey(KeyCode.LeftShift)) { _speed *= 2f; }
        if (_hInput < 0 || _hInput > 0)
        {
            _p_control.Move(new Vector3(_hInput * _speed * Time.deltaTime, 0f, 0f));
        }
        // get horizontal input
        // define direction based on input
        // move based on input
        // get space for jump

    }
}
