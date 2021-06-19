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
    private Vector3 _velocity;

    // Start is called before the first frame update
    void Start()
    {
        _speed = 3.5f;
        _controller = GetComponent<CharacterController>();
        _velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Get user input for the direction of movement
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(kcode)) Debug.Log("KeyCode down: " + kcode);
        }
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        
        // Apply speed to direction to get velocity
        _velocity = direction * _speed;

        // Apply gravity to y-component of velocity
        if (!_controller.isGrounded) _velocity.y -= _gravity;

        // Move the player based on how much time has passed since the last update
        _controller.Move(_velocity * Time.deltaTime);
    }
}
