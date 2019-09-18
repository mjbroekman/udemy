using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_base : MonoBehaviour
{
    // Set base enemy life
    private readonly float _maxLife = 10f;

    // Set screen boundaries
    private readonly float _maxH = 9.5f;
    private readonly float _maxV = 6.5f;

    private float _curSpd;
    private float _randomX;
    private float _curLife;
    private float _baseLife;

    private Player _player;
    private float _gameStartTime;
    private Vector3 _lastMove;

    private Animator _e_animator;

    // Start is called before the first frame update
    void Start()
    {
        _randomX = Random.Range(-_maxH, _maxH);
        _lastMove = Vector3.down;
        transform.position = new Vector3(_randomX, _maxV, 0.0f);
        SetSpeed();
        _curLife = 1 + (int)((Time.time - _gameStartTime) / 36);

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
        if (_randomH > 95f || (_randomH > 50f && _lastMove != Vector3.down))
        {
            float _randomDir = Random.Range(0f, 1f);
            if (_lastMove == Vector3.down)
            {
                Debug.Log("Enemy_base::Update() :: Moving horizontally! _lastMove == " + _lastMove + " random direction == " + _randomDir);
                if (_randomDir > 0.5f)
                {
                    transform.Translate(Vector3.left * _curSpd * Time.deltaTime);
                    _lastMove = Vector3.left;
                }
                else
                {
                    transform.Translate(Vector3.right * _curSpd * Time.deltaTime);
                    _lastMove = Vector3.right;
                }
            }
            else if (_lastMove == Vector3.left)
            {
                Debug.Log("Enemy_base::Update() :: Moving horizontally! _lastMove == " + _lastMove + " random direction == " + _randomDir);
                if (_randomDir > 0.1f)
                {
                    transform.Translate(Vector3.left * _curSpd * Time.deltaTime);
                    _lastMove = Vector3.left;
                }
                else
                {
                    transform.Translate(Vector3.right * _curSpd * Time.deltaTime);
                    _lastMove = Vector3.right;
                }
            }
            else if (_lastMove == Vector3.right)
            {
                Debug.Log("Enemy_base::Update() :: Moving horizontally! _lastMove == " + _lastMove + " random direction == " + _randomDir);
                if (_randomDir > 0.9f)
                {
                    transform.Translate(Vector3.left * _curSpd * Time.deltaTime);
                    _lastMove = Vector3.left;
                }
                else
                {
                    transform.Translate(Vector3.right * _curSpd * Time.deltaTime);
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

    public void SetGameStart(float _startTime)
    {
        // Used to mitigate restarts causing enemies to spawn as faster speeds to begin with
        Debug.Log("Enemy_base::SetGameStart() :: Setting game start time to " + _startTime);
        _gameStartTime = _startTime;
    }

    private void SetSpeed()
    {
        _curSpd = 2.0f + ((Time.time - _gameStartTime) / 36);
        Debug.Log("Enemy_base::SetSpeed() :: Speed has changed to " + _curSpd);
    }

    /// <summary>
    /// Handle collisions
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        string _what = other.tag;

        if (_what == "Player")
        {
            if (_player != null) { _player.TakeDamage(_curSpd); }
            TakeDamage(_maxLife);
        }
        else if (_what == "Laser")
        {
            Laser laser = other.transform.GetComponent<Laser>();
            float damage = 0;

            if (laser != null) { damage = laser.GetPower(); }

            Destroy(other.gameObject);
            TakeDamage(damage);
        }
    }

    /// <summary>
    /// Apply damage to the enemy.
    /// </summary>
    /// <param name="damage"></param>
    void TakeDamage(float damage)
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
                _player.IncreaseScore((int)_baseLife + (int)_curSpd);
            }
            _e_animator.SetTrigger("OnEnemyDeath");
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            Destroy(this.gameObject, 3f);
        }
    }
}
