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


    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("Enemy_base::Start() :: Houston, we have a problem. There is no Spawn_Manager in the scene.");

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null) Debug.LogError("Enemy_base::Start() :: We have a problem. The gameManager is null");

        _bounds = _gameManager.GetScreenBoundaries(this.gameObject);
        _maxH = _bounds[1];
        _maxV = _bounds[3];

        _pf_mainWeapon = Resources.Load<GameObject>("Prefabs/Weapons/Laser");

        _randomX = Random.Range(-_maxH, _maxH);
        _lastMove = Vector3.down;
        transform.position = new Vector3(_randomX, _maxV, 0.0f);
        SetSpeed();
        _curLife = 1f + (_gameManager.GetLevel() / 3f);

        if (_curLife > _maxLife) { _curLife = _maxLife; }
        _baseLife = _curLife;

        if (GameObject.Find("Player") != null) _player = GameObject.Find("Player").GetComponent<Player>();

        _e_animator = gameObject.GetComponent<Animator>();
        if (_e_animator != null) _e_animator.ResetTrigger("OnEnemyDeath");
        else Debug.LogError("Enemy_base::Start() :: Unable to find Animator component");
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
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
        SetColor();
        _firingCycle = FiringCycle();
        StartCoroutine(_firingCycle);
    }

    void Update()
    {
        if (gameObject.GetComponent<SpriteRenderer>().enabled)
        {
            MoveEnemy();
        }
    }

    private void MoveEnemy()
    {
        float _randomH = Random.Range(0f, 100f);
        transform.Translate(Vector3.down * _curSpd * Time.deltaTime);
        if (_randomH > 90f || (_randomH > 25f && _lastMove != Vector3.down))
        {
            float _randomDir = Random.Range(0f, 1f);
            if (_lastMove == Vector3.down)
            {
                if (_randomDir > 0.5f)
                {
                    transform.Translate(Vector3.left * _curSpd * Time.deltaTime);
                    if (transform.position.x < -_maxH) transform.position = new Vector3(_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.left;
                }
                else
                {
                    transform.Translate(Vector3.right * _curSpd * Time.deltaTime);
                    if (transform.position.x > _maxH) transform.position = new Vector3(-_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.right;
                }
            }
            else if (_lastMove == Vector3.left)
            {
                if (_randomDir > 0.05f)
                {
                    transform.Translate(Vector3.left * _curSpd * Time.deltaTime);
                    if (transform.position.x < -_maxH) transform.position = new Vector3(_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.left;
                }
                else
                {
                    transform.Translate(Vector3.right * _curSpd * Time.deltaTime);
                    if (transform.position.x > _maxH) transform.position = new Vector3(-_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.right;
                }
            }
            else if (_lastMove == Vector3.right)
            {
                if (_randomDir > 0.95f)
                {
                    transform.Translate(Vector3.left * _curSpd * Time.deltaTime);
                    if (transform.position.x < -_maxH) transform.position = new Vector3(_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.left;
                }
                else
                {
                    transform.Translate(Vector3.right * _curSpd * Time.deltaTime);
                    if (transform.position.x > _maxH) transform.position = new Vector3(-_maxH, transform.position.y, 0.0f);
                    _lastMove = Vector3.right;
                }
            }
        }
        else
        {
            _lastMove = Vector3.down;
        }

        if (transform.position.y < _maxV * -1.1)
        {
            _randomX = Random.Range(-_maxH, _maxH);
            transform.position = new Vector3(_randomX, _maxV, 0.0f);
            _lastMove = Vector3.down;
            SetSpeed();
        }
    }

    IEnumerator FiringCycle()
    {
        float _delay = Random.Range(3f, 6f);
        _delay -= (_curLife / 2f);
        if (_delay < 0.5f) { _delay = 0.5f; }

        while (gameObject.GetComponent<BoxCollider2D>().enabled)
        {
            yield return new WaitForSeconds(_delay);

            if (_pf_mainWeapon != null)
            {
                GameObject laserA = Instantiate(_pf_mainWeapon, transform.position + new Vector3(0.1f, -0.75f, 0), Quaternion.identity);
                GameObject laserB = Instantiate(_pf_mainWeapon, transform.position + new Vector3(-0.1f, -0.75f, 0), Quaternion.identity);
                if (laserA != null) { laserA.GetComponent<Laser>().ConfigureLaser("Enemy"); }
                if (laserB != null) { laserB.GetComponent<Laser>().ConfigureLaser("Enemy"); }
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
        //Debug.Log("Enemy_base::SetColor() :: Enemy has " + _curLife + " health. Color set to " + colorize.color);
    }

    private void SetSpeed()
    {
        _curSpd = 2.0f + ((_gameManager.GetLevel() - 1f) / 2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string _what = other.tag;
        switch (_what)
        {
            case "Player":
                if (_player != null) { _player.TakeDamage(_curSpd); }
                TakeDamage(_maxLife, true);
                break;
            case "Laser":
                Laser laser = other.transform.GetComponent<Laser>();
                if (!laser.IsHostile("Enemy")) { break; }

                float damage = 0;

                if (laser != null) { damage = laser.GetPower(); }

                Destroy(other.gameObject);
                TakeDamage(damage, true);
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
            StopCoroutine(_firingCycle);
            _firingCycle = null;
            AudioSource.PlayClipAtPoint(_e_sounds.clip, transform.position);
            Destroy(this.gameObject, 1f);
        }
    }
}
