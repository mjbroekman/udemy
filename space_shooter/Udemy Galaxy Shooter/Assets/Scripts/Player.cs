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

    // ActiveEffects
    private Dictionary<string, GameObject> _activeEffects;

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
    private UIManager _uiManager;
    private GameManager _gameManager;

    private SpriteRenderer _e_spriteRenderer;
    private SpriteRenderer _p_spriteRenderer;

    private int _score;

    void Start()
    {
        // Set starting position to near the bottom of the screen
        transform.position = new Vector3(0, -3, 0);

        // Set base speed as well as max/min speed
        _curSpd = 5.0f;

        // Set the weapon stats
        _laserCoolDown = 0.2f;
        _curCoolDown = 0f;
        _coolDownMult = 1f;
        _pf_mainWeapon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/Laser.prefab");

        // Initialize the active effects
        _activeEffects = new Dictionary<string, GameObject>();

        // PowerUp info
        _tShotDuration = 0f;
        _tShotEnabled = false;
        _boostDuration = 0f;
        _boostEnabled = false;
        _shieldStrength = 0f;
        _shieldEnabled = false;

        // Set player health
        _maxHealth = 10f;
        _curHealth = _maxHealth;
        _lives = 3;

        // Get the spawn Manager and UI Manager
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Player::Start() :: Houston, we have a problem. There is no Spawn_Manager in the scene.");
        }

        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("Player::Start() :: Houston, we have a problem. There is no UIManager in the scene.");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Player::Start() :: We have a problem. The gameManager is null");
        }

        float[] _bounds = _gameManager.GetScreenBoundaries(this.gameObject);
        _maxH = _bounds[1];
        _minV = _bounds[2];
        _maxV = _bounds[3];
        _score = 0;

        if (_uiManager._uiLoaded)
        {
            _uiManager.UpdateScore(_score);
            _uiManager.UpdateLives(_lives);
        }

        _p_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Cycle through the possible actions in a frame.
    /// </summary>
    void Update()
    {
        CheckPowerUp();
        CalculateMovement();
        if (Input.GetKey(KeyCode.Space) && Time.time > _curCoolDown) { FireLaser(); }
    }

    /// <summary>
    /// Check the state of timed powerups
    /// </summary>
    void CheckPowerUp()
    {
        float curTime = Time.time;
        if (_tShotEnabled)
        {
            //Debug.Log("Player::CheckPowerUp() :: TripleShot: current time: " + curTime + " tshottime: " + _tShotTime + " duration: " + _tShotDuration);
        }
        if (_boostEnabled)
        {
            //Debug.Log("Player::CheckPowerUp() :: Boost: current time: " + curTime + " boosttime: " + _boostTime + " duration: " + _boostDuration);
        }
        if (_tShotEnabled && (curTime - _tShotTime) > _tShotDuration)
        {
            Debug.Log("Player::CheckPowerUp() :: TripleShot powerup expired");
            DisableTripleShot();
        }
        if (_boostEnabled && (curTime - _boostTime) > _boostDuration)
        {
            Debug.Log("Player::CheckPowerUp() :: Boost powerup expired");
            DisableSpeedBoost();
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
        if (transform.position.y > _maxV) transform.position = new Vector3(transform.position.x, _maxV, 0f);
        else if (transform.position.y < _minV) transform.position = new Vector3(transform.position.x, _minV, 0f);

        if (transform.position.x > _maxH) transform.position = new Vector3(-_maxH, transform.position.y, 0f);
        else if (transform.position.x < -_maxH) transform.position = new Vector3(_maxH, transform.position.y, 0f);
    }

    /// <summary>
    /// Fire the main laser.
    /// </summary>
    void FireLaser()
    {
        if (_pf_mainWeapon != null)
        {
            _curCoolDown = Time.time + (_laserCoolDown * _coolDownMult);
            Instantiate(_pf_mainWeapon, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Player::FireLaser() :: Unable to find weapon!");
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
            UpdateShield();
        }
        else
        {
            if (_shieldEnabled && _shieldStrength > 0f)
            {
                damage -= _shieldStrength;
                Debug.Log("Player::TakeDamage() :: Shield absorbed " + _shieldStrength + " damage before giving out.");
                DisableShield();
            }
            Debug.Log("Player::TakeDamage() :: Player took " + damage + " damage from that hit.");
            _curHealth -= damage;
            if (_curHealth <= 0)
            {
                Debug.Log("Player::TakeDamage() :: This life is over!");
                _uiManager.ResetBackground();
                if (_lives > 1)
                {
                    Debug.Log("Player::TakeDamage() :: ...but another life begins!");
                    _curHealth = _maxHealth;
                    _spawnManager.OnPlayerDeath(_lives);
                    DisablePowerUps();
                    RemoveAllEffects();
                    _lives--;
                    _uiManager.UpdateLives(_lives);
                }
                else
                {
                    _lives--;
                    _uiManager.UpdateLives(_lives);
                    _spawnManager.OnPlayerDeath(_lives);
                    Debug.Log("Player::TakeDamage() :: I'm going down! I'm hit! It's all over for me!");
                    Destroy(this.gameObject, 0.1f);
                }
            }
        }
    }

    public void CollectPowerUp(string powerUp, float strength)
    {
        switch (powerUp)
        {
            case "TripleShot": EnableTripleShot(strength); break;
            case "Shield": EnableShield(strength); break;
            case "Boost": EnableSpeedBoost(strength); break;
            default: break;
        }
    }

    private void SetEffect(string what)
    {
        GameObject _effect = _spawnManager.GetEffect(what);
        if (_effect != null)
        {
            GameObject newEffect = Instantiate(_effect, transform.position, Quaternion.identity);
            newEffect.transform.parent = transform;
            _activeEffects.Add(what, newEffect);
        }
    }

    private void RemoveAllEffects()
    {
        foreach (string effect in _activeEffects.Keys)
        {
            RemoveEffect(effect);
        }
    }

    private void RemoveEffect(string what)
    {
        if (_activeEffects.ContainsKey(what))
        {
            GameObject _effect = _activeEffects[what];
            _activeEffects.Remove(what);
            Destroy(_effect);
        }
    }

    private void DisablePowerUps()
    {
        if (_tShotEnabled) { DisableTripleShot(); }
        if (_boostEnabled) { DisableSpeedBoost(); }
        if (_shieldEnabled) { DisableShield(); }
    }

    private void EnableShield(float strength)
    {
        _shieldEnabled = true;
        _shieldStrength += strength;
        if (!_activeEffects.ContainsKey("Shield_PowerUp_Effect")) { SetEffect("Shield_PowerUp_Effect"); }
        else
        {
            UpdateShield();
            Debug.Log("Player::EnableShield() :: Increasing current shield strength.");
        }
    }

    private void UpdateShield()
    {
        _e_spriteRenderer = _activeEffects["Shield_PowerUp_Effect"].GetComponent<SpriteRenderer>();
        // Change color and Alpha to reflect condition of the shield
        if (_shieldStrength > 20f) { _e_spriteRenderer.color = Color.cyan; }
        else { _e_spriteRenderer.color = new Color(1f - (_shieldStrength / 20f), 0, _shieldStrength / 20f, (_shieldStrength / 40f) + 0.5f); }
        Debug.Log("Player::UpdateShield() :: Shield can still absorb " + _shieldStrength + " damage before giving out.");
    }

    private void DisableShield()
    {
        _shieldEnabled = false;
        _shieldStrength = 0f;
        RemoveEffect("Shield_PowerUp_Effect");
    }

    private void EnableTripleShot(float strength)
    {
        _tShotEnabled = true;
        _tShotTime = Time.time;
        _tShotDuration += strength;
        _pf_mainWeapon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/TripleShot.prefab");
    }

    private void DisableTripleShot()
    {
        _tShotEnabled = false;
        _tShotDuration = 0f;
        _pf_mainWeapon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Weapons/Laser.prefab");
    }

    private void EnableSpeedBoost(float strength)
    {
        _boostEnabled = true;
        _boostTime = Time.time;
        _boostDuration += strength;
        _curSpd *= 2f;
    }

    private void DisableSpeedBoost()
    {
        _boostEnabled = false;
        _boostDuration = 0f;
        _curSpd /= 2f;
    }

    public void IncreaseScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
