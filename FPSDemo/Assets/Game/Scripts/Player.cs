using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _controller;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _gravity = 9.81f;

    [SerializeField]
    private float _jump;

    [SerializeField]
    private bool _isJumping;

    [SerializeField]
    private Vector3 _velocity;

    [SerializeField]
    private float _sensitivity;
    
    // Start is called before the first frame update
    void Start()
    {
        _speed = 3.5f;
        _jump = 15f;
        _isJumping = false;

        _sensitivity = 0.2f;

        _controller = GetComponent<CharacterController>();
        _velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        float eastwest = Input.GetAxis("Horizontal");
        float northsouth = Input.GetAxis("Vertical");
        bool jump = Input.GetKeyDown(KeyCode.Space);

        Vector3 direction = new Vector3(eastwest,0f, northsouth);

        // Apply speed to direction to get velocity
        _velocity = direction * _speed;

        if (jump && !_isJumping)
        {
            direction.y = _jump;
            _isJumping = true;
        }
        else if ((_controller.collisionFlags & CollisionFlags.Below) != 0)
        {
            _isJumping = false;
            direction.y = 0.0f;
        }
        else
        {
            _velocity.y -= _gravity;
        }

        // convert from localspace to worldspace
        _velocity = transform.transform.TransformDirection(_velocity);

        // Move the player based on how much time has passed since the last update
        _controller.Move(_velocity * Time.deltaTime);
    }

    public float GetSensitivity()
    {
        return _sensitivity;
    }

}
