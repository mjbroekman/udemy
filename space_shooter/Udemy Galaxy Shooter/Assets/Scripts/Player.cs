using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Screen bounds
    private float _maxH;
    private float _maxV;
    private float _minV;

    // Player speed
    private float _curSpd;

    // ActiveEffects
    private Dictionary<string, GameObject> _activeEffects;

    // Weapon info
    private GameObject _pf_mainWeapon;
    private float _laserCoolDown;
    private float _curCoolDown;
    private float _coolDownMult;

    // PowerUp info
    private bool _tShotEnabled;
    private float _tShotTime;
    private float _tShotDuration;

    private bool _shieldEnabled;
    private float _shieldStrength;

    private bool _boostEnabled;
    private float _boostTime;
    private float _boostDuration;

    // Health
    private float _maxHealth;
    private float _curHealth;
    private int _lives;

    // References to other gameObjects
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private GameManager _gameManager;

    private SpriteRenderer _e_spriteRenderer;
    private SpriteRenderer _p_spriteRenderer;
    private AudioSource _p_sounds;
    private AudioManager _audioManager;
    private Animator _p_animator;

    private int _score;

    private bool _newLife;
    private bool _flyUp;
    private bool _flyDown;

    void Start()
    {
        SetStartingPosition();
        // Set base speed as well as max/min speed
        _curSpd = 5.0f;

        // Set the weapon stats
        _laserCoolDown = 0.2f;
        _curCoolDown = 0f;
        _coolDownMult = 1f;
        _pf_mainWeapon = Resources.Load<GameObject>("Prefabs/Weapons/Laser");

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
        if (_spawnManager == null) Debug.LogError("Player::Start() :: Houston, we have a problem. There is no Spawn_Manager in the scene.");

        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (_uiManager == null) Debug.LogError("Player::Start() :: Houston, we have a problem. There is no UIManager in the scene.");

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null) Debug.LogError("Player::Start() :: We have a problem. The gameManager is null");

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

        _p_animator = gameObject.GetComponent<Animator>();
        if (_p_animator == null) { Debug.LogError("Player::Start() :: Unable to get animator component"); }
        else
        {
            _p_animator.SetBool("Turn_Left", false);
            _p_animator.SetBool("Turn_Right", false);
        }

        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (_audioManager == null) Debug.LogError("Player::Start() :: We have a problem. The audioManager is null");
        else
        {
            this.gameObject.AddComponent<AudioSource>();
            _p_sounds = this.GetComponent<AudioSource>();
            _p_sounds.clip = _audioManager.GetEffectSound("Explosion");
            if (_p_sounds == null || _p_sounds.clip == null) { Debug.LogError("Player::Start() :: Something went wrong and the AudioSource or clip are null"); }
            _p_sounds.loop = false;
            _p_sounds.playOnAwake = false;
            _p_sounds.volume = 0.5f;
            _p_sounds.pitch = 0.35f;
        }

        _p_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!_activeEffects.ContainsKey("Thruster")) { SetEffect("Thruster"); }
        if (_newLife && _lives > 0)
        {
            if (!_flyUp && transform.position.y < -1f) { transform.Translate(Vector3.up * _curSpd * Time.deltaTime); }
            else if (!_flyUp && transform.position.y >= -1f) { _flyUp = true; }

            if (_flyUp && transform.position.y > -3.5f) { transform.Translate(Vector3.down * _curSpd * Time.deltaTime); }
            else if (_flyUp && transform.position.y <= -3.5f) { _flyDown = true; }

            if (_flyUp && _flyDown) { _newLife = false; }
        }
        else if (_lives > 0)
        {
            CheckPowerUp();
            CalculateMovement();
            if (Input.GetKey(KeyCode.Space) && Time.time > _curCoolDown) { FireLaser(); }
        }
    }

    private void SetStartingPosition()
    {
        transform.position = new Vector3(0f, -6.5f, 0f);
        _newLife = true;
        _flyUp = false;
        _flyDown = false;
    }

    void CheckPowerUp()
    {
        float curTime = Time.time;

        if (_tShotEnabled && (curTime - _tShotTime) > _tShotDuration) { DisableTripleShot(); }
        if (_boostEnabled && (curTime - _boostTime) > _boostDuration) { DisableSpeedBoost(); }
    }

    void CalculateMovement()
    {
        // Get player input wasd/arrows
        float hInput = 0f;
        float vInput = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { hInput = -1f; }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { hInput = 1f; }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { vInput = 1f; }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { vInput = -1f; }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) { _p_animator.SetBool("Turn_Left", true); _p_animator.SetBool("Turn_Right", false); }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow)) { _p_animator.SetBool("Turn_Left", false); _p_animator.SetBool("Turn_Right", false); }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) { _p_animator.SetBool("Turn_Left", false); _p_animator.SetBool("Turn_Right", true); }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) { _p_animator.SetBool("Turn_Left", false); _p_animator.SetBool("Turn_Right", false); }

        // Move the Player
        transform.Translate(new Vector3(hInput, vInput, 0) * _curSpd * Time.deltaTime);

        // Check to see if the new player position is 'outside' the bounds and warp to other side if they are
        if (transform.position.y > _maxV) transform.position = new Vector3(transform.position.x, _maxV, 0f);
        else if (transform.position.y < _minV) transform.position = new Vector3(transform.position.x, _minV, 0f);

        if (transform.position.x > _maxH) transform.position = new Vector3(-_maxH, transform.position.y, 0f);
        else if (transform.position.x < -_maxH) transform.position = new Vector3(_maxH, transform.position.y, 0f);
    }

    void FireLaser()
    {
        if (_pf_mainWeapon != null)
        {
            _curCoolDown = Time.time + (_laserCoolDown * _coolDownMult);
            GameObject laser = Instantiate(_pf_mainWeapon, transform.position + new Vector3(0, 0.75f, 0), Quaternion.identity, transform);
            if (laser == null) { Debug.LogError("Player::FireLaser() :: I fired a NULL!"); }
        }
        else
        {
            Debug.LogError("Player::FireLaser() :: Unable to find weapon!");
        }
    }

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

                AudioSource.PlayClipAtPoint(_p_sounds.clip, transform.position);

                DisablePowerUps();
                RemoveAllEffects();
                _lives--;
                _uiManager.UpdateLives(_lives);
                _ = Instantiate(_spawnManager.GetEffect("Explosion"), transform.position, Quaternion.identity);
                SetStartingPosition();
                _spawnManager.OnPlayerDeath(_lives);

                if (_lives > 0)
                {
                    Debug.Log("Player::TakeDamage() :: ...but another life begins!");
                    _curHealth = _maxHealth;
                }
                else
                {
                    Destroy(this.gameObject, 1f);
                }
            }
            else
            {
                int countFires = NumFireEffects();
                if (_curHealth < 7.5f && countFires < 1) { AddFireEffect(); }
                if (_curHealth < 5f && countFires < 2) { AddFireEffect(); }
                if (_curHealth < 2.5f && countFires < 3) { AddFireEffect(); }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string _what = other.tag;
        switch (_what)
        {
            case "Laser":
                Laser laser = other.transform.GetComponent<Laser>();
                if (!laser.IsHostile("Player")) { break; }

                float damage = 0;

                if (laser != null) { damage = laser.GetPower(); }

                Destroy(other.gameObject);
                TakeDamage(damage);
                break;
            case "Player":
                break;
            case "Asteroid":
                break;
            case "PowerUp":
                break;
            case "Enemy":
                break;
            default:
                Debug.Log("Enemy_base::OnTriggerEnter2D() :: Hit something undefined");
                break;
        }
    }

    private int NumFireEffects()
    {
        int num = 0;
        for (int i = 1; i <= 3; i++)
        {
            if (_activeEffects.ContainsKey("Fire_Effect_" + i)) num++;
        }
        return num;
    }

    private void AddFireEffect()
    {
        List<string> _fireEffects = new List<string> { "Fire_Effect_1", "Fire_Effect_2", "Fire_Effect_3" };
        for (int i = 1; i <= 3; i++)
        {
            if (_activeEffects.ContainsKey("Fire_Effect_" + i)) _fireEffects.Remove("Fire_Effect_" + i);
        }
        if (_fireEffects.Count > 0)
        {
            string fEffect = _fireEffects[Random.Range(0, _fireEffects.Count)];
            SetEffect(fEffect);
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
            Debug.Log("Player::SetEffect() :: Setting " + what + " effect on the player");
            Vector3 _effectPos = _spawnManager.GetEffectLocation(what);
            GameObject newEffect = Instantiate(_effect, transform.position, Quaternion.identity, transform);
            newEffect.transform.localPosition += _effectPos;
            _activeEffects.Add(what, newEffect);
        }
    }

    private void RemoveAllEffects()
    {
        string[] keys = new string[(_activeEffects.Count - 1)];
        int i = 0;
        foreach (string item in _activeEffects.Keys)
        {
            if (item != "Thruster")
            {
                keys[i] = item;
                i++;
            }
        }
        for (int j = 0; j < i; j++)
        {
            RemoveEffect(keys[j]);
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
        else { UpdateShield(); }
    }

    private void UpdateShield()
    {
        _e_spriteRenderer = _activeEffects["Shield_PowerUp_Effect"].GetComponent<SpriteRenderer>();
        // Change color and Alpha to reflect condition of the shield
        if (_shieldStrength > 20f) { _e_spriteRenderer.color = Color.cyan; }
        else { _e_spriteRenderer.color = new Color(1f - (_shieldStrength / 20f), 0, _shieldStrength / 20f, (_shieldStrength / 40f) + 0.5f); }
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
        _tShotDuration += strength + (_gameManager.GetLevel() / 2);
        _coolDownMult = 0.75f;
        _pf_mainWeapon = Resources.Load<GameObject>("Prefabs/Weapons/TripleShot");
    }

    private void DisableTripleShot()
    {
        _tShotEnabled = false;
        _tShotDuration = 0f;
        _coolDownMult = 1f;
        _pf_mainWeapon = Resources.Load<GameObject>("Prefabs/Weapons/Laser");
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
