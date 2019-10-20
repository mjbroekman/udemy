using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_base : MonoBehaviour
{
    // Set max enemy life
    private readonly float _maxLife = 10f;
    private float _curSpd;
    private float _curLife;
    private float _baseLife;

    // Set screen boundary variables
    private float _maxH;
    private float _maxV;
    private float[] _bounds;
    private float _randomX;
    // The last direction we moved
    private Vector3 _lastMove;

    // Object references to other game objects
    private Player _player;
    private Animator _e_animator;
    private SpawnManager _spawnManager;
    private GameManager _gameManager;
    private AudioSource _e_sounds;
    private AudioManager _audioManager;
    private IEnumerator _firingCycle;
    private GameObject _pf_mainWeapon;
    private GameObject[] _e_Thruster;

    void Start()
    {
        SetupGameObjects();
        SetInitialPosition();

        _lastMove = Vector3.down;

        SetSpeed();
        SetLife();
        SetColor();

        _firingCycle = FiringCycle();
        StartCoroutine(_firingCycle);

        _e_Thruster = new GameObject[2];
        SetEffects("Thruster", _e_Thruster.Length);
    }

    void Update()
    {
        if (_spawnManager.IsPlayerDead()) { Debug.Log("Enemy_base::Update() :: Player is dead. Time for me to die."); Destroy(gameObject); }
        if (gameObject.GetComponent<SpriteRenderer>().enabled) { MoveEnemy(); }
    }

    private void SetLife()
    {
        _curLife = 1f + (_gameManager.GetLevel() / 3f);

        if (_curLife > _maxLife) { _curLife = _maxLife; }
        _baseLife = _curLife;
    }

    private void SetInitialPosition()
    {
        _bounds = _gameManager.GetScreenBoundaries(this.gameObject);
        _maxH = _bounds[1];
        _maxV = _bounds[3];

        _randomX = Random.Range(-_maxH, _maxH);
        transform.position = new Vector3(_randomX, _maxV, 0.0f);
    }

    private void SetupGameObjects()
    {
        if (GameObject.Find("Player") != null) _player = GameObject.Find("Player").GetComponent<Player>();

        _pf_mainWeapon = Resources.Load<GameObject>("Prefabs/Weapons/Laser");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("Enemy_base::Start() :: Houston, we have a problem. There is no Spawn_Manager in the scene.");

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null) Debug.LogError("Enemy_base::Start() :: We have a problem. The gameManager is null");

        _e_animator = gameObject.GetComponent<Animator>();
        if (_e_animator != null) _e_animator.ResetTrigger("OnEnemyDeath");
        else Debug.LogError("Enemy_base::Start() :: Unable to find Animator component");

        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        SetupAudioEffect();
    }

    private void SetupAudioEffect()
    {
        if (_audioManager == null) Debug.LogError("Enemy_base::Start() :: We have a problem. The audioManager is null");
        else
        {
            this.gameObject.AddComponent<AudioSource>();
            _e_sounds = this.GetComponent<AudioSource>();
            _e_sounds.clip = _audioManager.GetEffectSound("Explosion");
            if (_e_sounds == null || _e_sounds.clip == null) { Debug.LogError("Enemy_base::Start() :: Something went wrong and the AudioSource or clip are null"); }
            _e_sounds.loop = false;
            _e_sounds.playOnAwake = false;
            _e_sounds.volume = 0.75f;
            _e_sounds.pitch = 0.35f;
        }
    }

    private void SetEffects(string effect, int count)
    {
        switch (effect)
        {
            case "Thruster":
                for (int i = 0; i < count; i++)
                {
                    _e_Thruster[i] = Instantiate(_spawnManager.GetEffect("Thruster"), transform.position, Quaternion.identity, transform);
                    _e_Thruster[i].transform.localPosition += new Vector3(-0.375f + (i * 0.375f), 2.5f, 0f);
                    _e_Thruster[i].transform.localScale = new Vector3(0.1f, 0.5f, 0f);
                    _e_Thruster[i].transform.Rotate(new Vector3(-180f, 0f, 0f));
                }
                break;
        }

    }

    private void MoveEnemy()
    {
        float _randomH = Random.Range(0f, 100f);
        if (_randomH > 90f || (_randomH > 25f && _lastMove != Vector3.down))
        {
            float _randomDir = Random.Range(0f, 1f);
            if (_lastMove == Vector3.down)
            {
                if (_randomDir > 0.5f)
                {
                    transform.Translate((Vector3.left + Vector3.down) * _curSpd * Time.deltaTime);
                    if (transform.position.x < -_maxH) transform.position = new Vector3(_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.left;
                }
                else
                {
                    transform.Translate((Vector3.right + Vector3.down) * _curSpd * Time.deltaTime);
                    if (transform.position.x > _maxH) transform.position = new Vector3(-_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.right;
                }
            }
            else if (_lastMove == Vector3.left)
            {
                if (_randomDir > 0.05f)
                {
                    transform.Translate((Vector3.left + Vector3.down) * _curSpd * Time.deltaTime);
                    if (transform.position.x < -_maxH) transform.position = new Vector3(_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.left;
                }
                else
                {
                    transform.Translate((Vector3.right + Vector3.down) * _curSpd * Time.deltaTime);
                    if (transform.position.x > _maxH) transform.position = new Vector3(-_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.right;
                }
            }
            else if (_lastMove == Vector3.right)
            {
                if (_randomDir > 0.95f)
                {
                    transform.Translate((Vector3.left + Vector3.down) * _curSpd * Time.deltaTime);
                    if (transform.position.x < -_maxH) transform.position = new Vector3(_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.left;
                }
                else
                {
                    transform.Translate((Vector3.right + Vector3.down) * _curSpd * Time.deltaTime);
                    if (transform.position.x > _maxH) transform.position = new Vector3(-_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.right;
                }
            }
        }
        else
        {
            transform.Translate(Vector3.down * _curSpd * Time.deltaTime);
            _lastMove = Vector3.down;
        }

        if (transform.position.y < _maxV * -1.1 && gameObject.GetComponent<BoxCollider2D>().enabled)
        {
            _randomX = Random.Range(-_maxH, _maxH);
            transform.position = new Vector3(_randomX, _maxV, 0.0f);
            _lastMove = Vector3.down;
            SetSpeed();
        }
    }

    private IEnumerator FiringCycle()
    {
        float _delay = Random.Range(3f, 6f);
        _delay -= (_curLife / 2f);
        if (_delay < 0.5f) { _delay = 0.5f; }

        while (gameObject.GetComponent<BoxCollider2D>().enabled)
        {
            yield return new WaitForSeconds(_delay);

            if (_pf_mainWeapon != null)
            {
                _ = Instantiate(_pf_mainWeapon, transform.position + new Vector3(0.1f, -0.75f, 0), Quaternion.identity, transform);
                _ = Instantiate(_pf_mainWeapon, transform.position + new Vector3(-0.1f, -0.75f, 0), Quaternion.identity, transform);
            }
        }
    }

    private void SetColor()
    {
        SpriteRenderer colorize = transform.GetComponent<SpriteRenderer>();
        if (_curLife < 2f) colorize.color = new Color(1f, 0.6f, 1f, 1f);
        else if (_curLife < 3f) colorize.color = new Color(1f, 0.1f, 1f, 1f);
        else if (_curLife < 4f) colorize.color = new Color(1f, 0.6f, 0.3f, 1f);
        else if (_curLife < 5f) colorize.color = new Color(0f, 1f, 0.3f, 1f);
        else if (_curLife < 6f) colorize.color = new Color(0f, 1f, 1f, 1f);
        else if (_curLife < 7f) colorize.color = new Color(0f, 0.65f, 1f, 1f);
        else if (_curLife < 8f) colorize.color = new Color(0.65f, 1f, 0f, 1f);
        else if (_curLife < 9f) colorize.color = new Color(1f, 1f, 0f, 1f);
        else if (_curLife <= 10f) colorize.color = new Color(1f, 0.666f, 0f, 1f);
    }

    private void SetSpeed() { _curSpd = 2.0f + ((_gameManager.GetLevel() - 1f) / 3f); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string _what = other.tag;
        switch (_what)
        {
            case "Player":
                TakeDamage(_maxLife, true);
                if (_player != null) { _player.TakeDamage(_curSpd); }
                break;
            case "Laser":
                Laser laser = other.transform.GetComponent<Laser>();
                if (laser == null) { Debug.LogError("Enemy_base::OnTriggerEnter2D() :: Got a null laser component somehow"); }
                float damage = laser.GetPower();

                // Ignore other base enemy lasers
                if (!laser.IsHostile("Enemy")) { break; }

                // Absorb all other lasers
                Destroy(other.gameObject);

                // Take a hit from the boss laser
                if (!laser.IsHostile("Boss")) { TakeDamage(damage, false); }
                // Take a hit from the player laser
                else { TakeDamage(damage, true); }

                break;
            case "Asteroid":
                TakeDamage(_maxLife, false);
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

    void TakeDamage(float damage, bool _byPlayer)
    {
        _curLife -= damage;
        if (_curLife <= 0)
        {
            if (_player == null) Debug.LogError("Enemy_base::TakeDamage() :: Unable to find player object!");
            else if (_byPlayer) { _player.IncreaseScore((int)_baseLife + (int)_curSpd); }

            _e_animator.SetTrigger("OnEnemyDeath");
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            foreach (GameObject _effect in _e_Thruster) { Destroy(_effect); }

            if (_firingCycle != null) { StopCoroutine(_firingCycle); }
            _firingCycle = null;

            AudioSource.PlayClipAtPoint(_e_sounds.clip, transform.position);
            Destroy(this.gameObject, 1f);
        }
    }
}
