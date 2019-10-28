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
        Vector3 _direction = new Vector3(_hInput, 0f, 0f);
        Vector3 _velocity = _direction * _speed;
        _p_control.Move(_velocity * Time.deltaTime);
    }
}
