using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_base : MonoBehaviour
{
    // Set base enemy life
    private readonly float _maxLife = 10f;

    // Set screen boundaries
    private float _maxH;
    private float _maxV;
    private float[] _bounds;
    [SerializeField]
    private float _curSpd;
    private float _randomX;
    private float _curLife;
    private float _baseLife;

    private Player _player;
    private Vector3 _lastMove;

    private Animator _e_animator;
    private SpawnManager _spawnManager;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Get the spawn Manager
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Enemy_base::Start() :: Houston, we have a problem. There is no Spawn_Manager in the scene.");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Enemy_base::Start() :: We have a problem. The gameManager is null");
        }

        _bounds = _gameManager.GetScreenBoundaries(this.gameObject);
        _maxH = _bounds[1];
        _maxV = _bounds[3];

        _randomX = Random.Range(-_maxH, _maxH);
        _lastMove = Vector3.down;
        transform.position = new Vector3(_randomX, _maxV, 0.0f);
        SetSpeed();
        _curLife = 1f + (_gameManager.GetLevel() / 4f);

        if (_curLife > _maxLife) { _curLife = _maxLife; }
        _baseLife = _curLife;
        if (GameObject.Find("Player") != null)
        {
            _player = GameObject.Find("Player").GetComponent<Player>();
        }
        SetColor();
        _e_animator = gameObject.GetComponent<Animator>();
        if (_e_animator != null)
        {
            _e_animator.ResetTrigger("OnEnemyDeath");
        }
        else
        {
            Debug.LogError("Enemy_base::Start() :: Unable to find Animator component");
        }
        //SetScale();
    }

    /// <summary>
    /// Updates per frame. Move down and warp back to the top of the screen.
    /// </summary>
    void Update()
    {
        MoveEnemy();
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
                //Debug.Log("Enemy_base::Update() :: Moving horizontally! _lastMove == " + _lastMove + " random direction == " + _randomDir);
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
                //Debug.Log("Enemy_base::Update() :: Moving horizontally! _lastMove == " + _lastMove + " random direction == " + _randomDir);
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
                //Debug.Log("Enemy_base::Update() :: Moving horizontally! _lastMove == " + _lastMove + " random direction == " + _randomDir);
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

    private void SetSpeed()
    {
        _curSpd = 2.0f + ((_gameManager.GetLevel() - 1f) / 2f);
        Debug.Log("Enemy_base::SetSpeed() :: Speed has changed to " + _curSpd);
    }

    /// <summary>
    /// Handle collisions
    /// </summary>
    /// <param name="other"></param>
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

    /// <summary>
    /// Apply damage to the enemy.
    /// </summary>
    /// <param name="damage"></param>
    void TakeDamage(float damage, bool _byPlayer)
    {
        _curLife -= damage;
        if (_curLife <= 0)
        {
            if (_player == null)
            {
                Debug.LogError("Enemy_base::TakeDamage() :: Unable to find player object!");
            }
            else
            {
                if (_byPlayer) { _player.IncreaseScore((int)_baseLife + (int)_curSpd); }
            }
            _e_animator.SetTrigger("OnEnemyDeath");
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            Destroy(this.gameObject, 3f);
        }
    }
}
