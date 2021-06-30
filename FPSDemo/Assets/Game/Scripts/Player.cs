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

    [SerializeField]
    private GameObject _weapon;
    
    // Start is called before the first frame update
    void Start()
    {
        _speed = 3.5f;
        _jump = 15f;
        _isJumping = false;

        // user setting
        _sensitivity = 1f;

        _controller = GetComponent<CharacterController>();
        _weapon = GameObject.Find("Weapon");

        _velocity = Vector3.zero;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        bool hitEscape = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace);

        if (Cursor.lockState == CursorLockMode.Locked) {
            if (Input.GetMouseButton(0)) ShootWeapon();
            else StopShooting();

            CalculateMovement();
            if (hitEscape)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (hitEscape) { 
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void ShootWeapon()
    {
        //if left click
        // cast ray from center of main camera
        Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitInfo;
        ParticleSystem flash = _weapon.GetComponentInChildren<ParticleSystem>(true);
        if (! flash.isEmitting)
        {
            flash.playOnAwake = true;
            flash.Play();
        }
        if (Physics.Raycast(rayOrigin, out hitInfo))
        {
            // Do this if we're in the Unity Editor
            Debug.Log("Raycast collision detected with " + hitInfo.transform.name);
        }
    }

    void StopShooting()
    {
        ParticleSystem flash = _weapon.GetComponentInChildren<ParticleSystem>(true);
        if (flash.isEmitting || flash.isPlaying)
        {
            flash.playOnAwake = false;
            flash.Stop();
        }
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
