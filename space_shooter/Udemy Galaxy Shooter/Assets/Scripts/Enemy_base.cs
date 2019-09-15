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

    // Start is called before the first frame update
    void Start()
    {
        _randomX = Random.Range(-_maxH, _maxH);
        transform.position = new Vector3(_randomX, _maxV, 0.0f);
        SetSpeed();
        _curLife = 1 + (int)((Time.time - _gameStartTime) / 36);

        if (_curLife > _maxLife) { _curLife = _maxLife; }
        _baseLife = _curLife;
        if (GameObject.Find("Player") != null)
        {
            _player = GameObject.Find("Player").GetComponent<Player>();
        }
    }

    /// <summary>
    /// Updates per frame. Move down and warp back to the top of the screen.
    /// </summary>
    void Update()
    {
        transform.Translate(Vector3.down * _curSpd * Time.deltaTime);
        if (transform.position.y < _maxV * -1.1)
        {
            _randomX = Random.Range(-_maxH, _maxH);
            transform.position = new Vector3(_randomX, _maxV, 0.0f);
            SetSpeed();
        }
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
            Destroy(this.gameObject);
        }
    }
}
