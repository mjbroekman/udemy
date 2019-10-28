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

    [SerializeField]
    private float _gravity;

    [SerializeField]
    private Vector3 _direction;

    [SerializeField]
    private Vector3 _velocity;

    void Start()
    {
        _p_control = GetComponent<CharacterController>();
        _walkSpd = 1.5f;
        _gravity = -1.0f;
    }

    void Update()
    {
        _hInput = Input.GetAxis("Horizontal");
        float _speed = _walkSpd;

        // sprint
        if (Input.GetKey(KeyCode.LeftShift)) { _speed *= 2.0f; }

        _direction = new Vector3(_hInput, _velocity.y, 0.0f);
        _velocity = _direction * _speed;

        // check to see if the character is touching the ground (isGrounded)
        // if grounded, zero out vertical velocity 
        if (_p_control.isGrounded) { _velocity.y = 0.0f; }
        // if not, apply gravity
        else { _velocity.y += _gravity; }

        _p_control.Move(_velocity * Time.deltaTime);
    }
}
