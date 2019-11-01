using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _p_control;

    [SerializeField]
    private float sideMove;

    [SerializeField]
    private float _walkSpd;

    [SerializeField]
    private float _gravity;

    [SerializeField]
    private Vector3 _velocity;

    [SerializeField]
    private float _jumpHeight;

    [SerializeField]
    private bool _can2Jump;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private int _coins;

    [SerializeField]
    private UIManager _uiManager;

    void Start()
    {
        _p_control = GetComponent<CharacterController>();
        _walkSpd = 5.0f;
        _gravity = 1.0f;
        _jumpHeight = 15.0f;
        _velocity = Vector3.down;
        _can2Jump = false;
        _coins = 0;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        sideMove = Input.GetAxis("Horizontal");
        bool jump = Input.GetKeyDown(KeyCode.Space);

        // If the LeftShift is 'pressed', double speed for sprinting
        if (Input.GetKey(KeyCode.LeftShift)) { _speed = _walkSpd * 2.0f; }
        // Otherwise use our walking speed
        else { _speed = _walkSpd; }

        // Check if Space was pressed for a jump attempt
        if (jump)
        {
            // Player is on the ground, so let's jump
            if (_p_control.isGrounded)
            {
                _velocity = new Vector3(sideMove * _speed, _jumpHeight, 0.0f);
                _can2Jump = true;
            }
            // Player is moving UP, so let's double jump
            else if (_can2Jump && _velocity.y > 0.0f)
            {
                _velocity = new Vector3(sideMove * _speed, _velocity.y - _gravity + (_jumpHeight * 1.5f), 0.0f);
                _can2Jump = false;
            }
            // Player is not on the ground and not moving up, so apply gravity
            else { _velocity = new Vector3(sideMove * _speed, _velocity.y - _gravity, 0.0f); }
        }
        // Check if we're on the ground
        else if (_p_control.isGrounded)
        {
            _velocity = new Vector3(sideMove * _speed, -_gravity, 0.0f);
        }
        // Check to see if we bonked our head
        else if ((_p_control.collisionFlags & CollisionFlags.Above) != 0)
        {
            // If we did bonk our head, start our initial downward fall
            if (_velocity.y > 0.0f) { _velocity = new Vector3(sideMove * _speed, -0.1f, 0.0f); }
            // Otherwise, just add gravity to our falling speed
            else { _velocity = new Vector3(sideMove * _speed, _velocity.y - _gravity, 0.0f); }
        }
        // We got here because we are falling (a.k.a. not on the ground)
        else { _velocity = new Vector3(sideMove * _speed, _velocity.y - _gravity, 0.0f); }

        // Now let's actually _move_
        _p_control.Move(_velocity * Time.deltaTime);
    }

    public void AddCoins(int count)
    {
        _coins += count;
        _uiManager.SetCoins(_coins);
    }

    public int GetCoins()
    {
        return _coins;
    }
}
