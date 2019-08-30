using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // [SerializeField] exposes private variables in the inspector but keeps them hidden from other game objects
    // public vs private
    // data type (int, float, bool, string)
    // variable name
    // optional value assignment
    [SerializeField]
    private float _curSpd;
    [SerializeField]
    private GameObject _pf_laser;

    private float _maxSpd;
    private float _minSpd;
    private float _maxH;
    private float _maxV;
    private float _minV;
    private float _laserCoolDown;
    private float _curCoolDown;
    private float _coolDownMult;

    // Start is called before the first frame update
    void Start()
    {
        // Set starting position to (0,0,0)
        transform.position = new Vector3(0, 0, 0);

        // Set base speed as well as max/min speed
        _curSpd = 1.0f;
        _maxSpd = 10.0f;
        _minSpd = 0.0f;

        // Set screen boundaries
        _maxH = 11.0f;
        _maxV = 1.0f;
        _minV = -5.0f;

        // Set the weapon cooldown
        _laserCoolDown = 0.2f;
        _curCoolDown = 0f;
        _coolDownMult = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameSpeed();
        CalculateMovement();
        if (Input.GetKey(KeyCode.Space) && Time.time > _curCoolDown) { FireLaser(); }
    }

    // Calculate the new speed
    void CheckGameSpeed()
    {
        bool sInputUp = Input.GetKey(KeyCode.RightBracket);
        bool sInputDown = Input.GetKey(KeyCode.LeftBracket);

        if (sInputUp)
        {
            _curSpd += 20.0f * Time.deltaTime;
            if (_curSpd > _maxSpd) { _curSpd = _maxSpd; }
        }

        if (sInputDown)
        {
            _curSpd -= 20.0f * Time.deltaTime;
            if (_curSpd < _minSpd) { _curSpd = _minSpd; }
        }
    }

    // All movement stuff is handled in here
    void CalculateMovement()
    {
        // Get player input wasd/arrows
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        // Move the Player
        transform.Translate(new Vector3(hInput, vInput, 0) * _curSpd * Time.deltaTime);

        // Check to see if the new player position is 'outside' the bounds and warp to other side if they are
        //if (transform.position.y > _maxV)
        //{
        //    transform.position = new Vector3(transform.position.x, -_maxV, 0);
        //}
        //else if (transform.position.y < -_maxV)
        //{
        //    transform.position = new Vector3(transform.position.x, _maxV, 0);
        //}
        if (transform.position.y > _maxV)
        {
            transform.position = new Vector3(transform.position.x, _maxV, 0);
        }
        else if (transform.position.y < _minV)
        {
            transform.position = new Vector3(transform.position.x, _minV, 0);
        }
        if (transform.position.x > _maxH)
        {
            transform.position = new Vector3(-_maxH, transform.position.y, 0);
        }
        else if (transform.position.x < -_maxH)
        {
            transform.position = new Vector3(_maxH, transform.position.y, 0);
        }
    }

    // Fire the laser
    void FireLaser()
    {
        Debug.Log("Firing laser");
        _curCoolDown = Time.time + (_laserCoolDown * _coolDownMult);
        Instantiate(_pf_laser, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
    }
}
