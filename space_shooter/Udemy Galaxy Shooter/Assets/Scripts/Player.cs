using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour
{
    // Screen bounds
    private float _maxH;
    private float _maxV;
    private float _minV;

    // Player speed
    [SerializeField]
    private float _curSpd;
    private float _maxDefSpd;
    private float _minSpd;

    // Weapon info
    private GameObject _pf_mainWeapon;
    private float _laserCoolDown;
    private float _curCoolDown;
    private float _coolDownMult;

    // PowerUp info
    [SerializeField]
    private bool _tShotEnabled;
    private float _tShotTime;
    private float _tShotDuration;

    [SerializeField]
    private bool _shieldEnabled;
    private float _shieldStrength;

    [SerializeField]
    private bool _boostEnabled;
    private float _boostTime;
    private float _boostDuration;

    // Health
    private float _maxHealth;
    private float _curHealth;
    private int _lives;

    private SpawnManager _spawnManager;

    void Start()
    {
        // Set starting position to near the bottom of the screen
        transform.position = new Vector3(0, -3, 0);

        // Set base speed as well as max/min speed
        _curSpd = 5.0f;
        _maxDefSpd = 10.0f;
        _minSpd = 0.0f;

        // Set screen boundaries
        _maxH = 10.0f;
        _maxV = 4.0f;
        _minV = -4.0f;

        // Set the weapon stats
        _laserCoolDown = 0.2f;
        _curCoolDown = 0f;
        _coolDownMult = 1.0f;
        _pf_mainWeapon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/Laser.prefab");

        // PowerUp info
        _tShotEnabled = false;
        _tShotDuration = 0f;
        _shieldEnabled = false;
        _boostDuration = 0f;
        _boostEnabled = false;
        _shieldStrength = 0f;

        // Set player health
        _maxHealth = 10f;
        _curHealth = _maxHealth;
        _lives = 3;

        // Get the spawn Manager
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Houston, we have a problem. There is no Spawn_Manager in the scene.");
        }
    }

    /// <summary>
    /// Cycle through the possible actions in a frame.
    /// </summary>
    void Update()
    {
        CheckPowerUp();
        CheckGameSpeed();
        CalculateMovement();
        if (Input.GetKey(KeyCode.Space) && Time.time > _curCoolDown) { FireLaser(); }
    }

    /// <summary>
    /// Check the state of timed powerups
    /// </summary>
    void CheckPowerUp()
    {
        float curTime = Time.time;
        if (_tShotEnabled && (curTime - _tShotTime) > _tShotDuration)
        {
            //Debug.Log("TripleShot powerup expired");
            //_tShotEnabled = false;
            //_pf_mainWeapon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/Laser.prefab");
        }
        if (_boostEnabled && (curTime - _boostTime) > _boostDuration)
        {
            //Debug.Log("Boost powerup expired");
            //_boostEnabled = false;
        }
    }

    /// <summary>
    /// Take user input to change the speed of the game
    /// </summary>
    void CheckGameSpeed()
    {
        bool sInputUp = Input.GetKey(KeyCode.RightBracket);
        bool sInputDown = Input.GetKey(KeyCode.LeftBracket);
        float _maxSpd = _maxDefSpd;
        float _spdMod = 20f;

        if (_boostEnabled)
        {
            _maxSpd = _maxDefSpd * 2f;
            _spdMod = 40f;
        }
        if (sInputUp)
        {
            _curSpd += _spdMod * Time.deltaTime;
            if (_curSpd > _maxSpd) { _curSpd = _maxSpd; }
        }

        if (sInputDown)
        {
            _curSpd -= _spdMod * Time.deltaTime;
            if (_curSpd < _minSpd) { _curSpd = _minSpd; }
        }
    }

    /// <summary>
    /// Take user input to move the player around the 2.5d space
    /// </summary>
    void CalculateMovement()
    {
        // Get player input wasd/arrows
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        // Move the Player
        transform.Translate(new Vector3(hInput, vInput, 0) * _curSpd * Time.deltaTime);

        // Check to see if the new player position is 'outside' the bounds and warp to other side if they are
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

    /// <summary>
    /// Fire the main laser. _pf_laser is assigned in the Inspector.
    /// </summary>
    void FireLaser()
    {
        if (_pf_mainWeapon != null)
        {
            if (_tShotEnabled) { Debug.Log("Firing triple shot!"); }

            _curCoolDown = Time.time + (_laserCoolDown * _coolDownMult);
            Instantiate(_pf_mainWeapon, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Unable to find weapon!");
        }
    }

    /// <summary>
    /// Apply damage from hitting something, like an enemy.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        if (_shieldEnabled && _shieldStrength > 0f && _shieldStrength >= damage)
        {
            _shieldStrength -= damage;
            Debug.Log("Shield absorbed " + damage + " damage from that hit.");
        }
        else
        {
            if (_shieldEnabled && _shieldStrength > 0f)
            {
                _shieldEnabled = false;
                damage -= _shieldStrength;
                Debug.Log("Shield absorbed " + _shieldStrength + " damage before giving out.");
                _shieldStrength = 0f;
            }
            Debug.Log("Player took " + damage + " damage from that hit.");
            _curHealth -= damage;
            if (_curHealth <= 0)
            {
                Debug.Log("This life is over!");
                if (_lives > 0)
                {
                    Debug.Log("...but another life begins!");
                    _curHealth = _maxHealth;
                    _lives--;
                }
                else
                {
                    _spawnManager.OnPlayerDeath();
                    Debug.Log("I'm going down! I'm hit! It's all over for me!");
                    Destroy(this.gameObject, 0.5f);
                }
            }
        }
    }

    public void CollectPowerUp(string powerUp, float strength)
    {
        if (powerUp == "TripleShot")
        {
            _tShotEnabled = true;
            _tShotTime = Time.time;
            _tShotDuration += strength;
            _pf_mainWeapon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/TripleShot.prefab");
            StartCoroutine(TripleShotCooldownRoutine());
        }
        if (powerUp == "Shield")
        {
            _shieldEnabled = true;
            _shieldStrength += strength;
        }
        if (powerUp == "Boost")
        {
            _boostEnabled = true;
            _boostTime = Time.time;
            _boostDuration += strength;
            _curSpd *= 2f;
            Debug.Log("Collected Speed Boost: Current Speed = " + _curSpd);
            StartCoroutine(BoostCooldownRoutine());
        }
    }

    IEnumerator TripleShotCooldownRoutine()
    {
        yield return new WaitForSeconds(_tShotDuration);
        Debug.Log("TripleShot powerup expired");
        _tShotEnabled = false;
        _pf_mainWeapon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/Laser.prefab");
    }

    IEnumerator BoostCooldownRoutine()
    {
        yield return new WaitForSeconds(_boostDuration);
        Debug.Log("Boost powerup expired");
        _boostEnabled = false;
        _curSpd /= 2f;
    }
}
