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

    [SerializeField]
    private float _jumpHeight;

    void Start()
    {
        _p_control = GetComponent<CharacterController>();
        _walkSpd = 5.0f;
        _gravity = 1.0f;
        _jumpHeight = 15.0f;
        _velocity = Vector3.down;
    }

    void Update()
    {
        _hInput = Input.GetAxis("Horizontal");
        float _speed = _walkSpd;

        // sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _hInput *= 2.0f;
        }

        Debug.Log("Vertical velocity is " + _velocity.y);

        // check to see if the character is touching the ground (isGrounded)
        // if grounded, zero out vertical velocity 
        if (_p_control.isGrounded)
        {
            Debug.Log("Player::Update() :: We are on solid ground.");
            _direction = new Vector3(_hInput, 0.0f, 0.0f);
            _velocity = _direction * _speed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _velocity.y = _jumpHeight;
            }
        }
        // if not, apply gravity
        else
        {
            Debug.Log("Player.Update() :: Apply gravity " + _gravity + " to vertical velocity " + _velocity.y);
            _direction = new Vector3(_hInput, _velocity.y, 0.0f);
            _velocity.y -= _gravity;
            if (_velocity.y < 0f)
            {
                Debug.Log("Player::Update() :: We are falling (negative Y velocity)");
            }
        }

        if (_velocity != Vector3.zero) { _p_control.Move(_velocity * Time.deltaTime); }
    }
}
