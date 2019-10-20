using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private GameManager _gameManager;

    private Vector3 _direction;

    private float _health;

    private float _rotationSpd;
    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;

    private AudioSource _e_sounds;
    private AudioManager _audioManager;

    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("Asteroid::Start() :: Houston, we have a problem. There is no Spawn_Manager in the scene.");

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null) Debug.LogError("Asteroid::Start() :: We have a problem. The gameManager is null");

        _health = 20f;

        _rotationSpd = 0.5f;

        PickDirection();

        float[] _bounds = _gameManager.GetScreenBoundaries(this.gameObject);
        _minX = _bounds[0];
        _maxX = _bounds[1];
        _minY = _bounds[2];
        _maxY = _bounds[3];

        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (_audioManager == null) Debug.LogError("Asteroid::Start() :: We have a problem. The audioManager is null");
        else
        {
            this.gameObject.AddComponent<AudioSource>();
            _e_sounds = this.GetComponent<AudioSource>();
            _e_sounds.clip = _audioManager.GetEffectSound("Explosion");
            if (_e_sounds == null || _e_sounds.clip == null) { Debug.LogError("Asteroid::Start() :: Something went wrong and the AudioSource or clip are null"); }

            _e_sounds.loop = false;
            _e_sounds.playOnAwake = false;
            _e_sounds.volume = 1f;
            _e_sounds.pitch = 0.1f;
        }
    }

    void Update()
    {
        if (_spawnManager.IsPlayerDead())
        {
            Debug.Log("Asteroid::Update() :: Player is dead. Time for me to die.");
            _spawnManager.EnableEnemySpawn();
            _spawnManager.EnablePowerUpSpawn();
            Destroy(gameObject);
        }

        if (gameObject.GetComponent<SpriteRenderer>().enabled)
        {
            _spawnManager.DisableEnemySpawn();
            _spawnManager.DisablePowerUpSpawn();
            PickDirection();
            MoveAsteroid();
        }
    }

    private void PickDirection()
    {
        if (_direction == new Vector3(0f, 0f, 0f))
        {
            int _dirPick = Random.Range(0, 4);

            switch (_dirPick)
            {
                case 0: _direction = Vector3.left + Vector3.down; break;
                case 1: _direction = Vector3.left + Vector3.up; break;
                case 2: _direction = Vector3.right + Vector3.down; break;
                case 3: _direction = Vector3.right + Vector3.up; break;
            }
        }
    }

    private void MoveAsteroid()
    {
        transform.Translate(_direction * Time.deltaTime, Space.World);

        if (transform.position.y < _minY) transform.position = new Vector3(transform.position.x, _maxY, 0f);
        else if (transform.position.y > _maxY) transform.position = new Vector3(transform.position.x, _minY, 0f);

        if (transform.position.x < _minX) transform.position = new Vector3(_maxX, transform.position.y, 0f);
        else if (transform.position.x > _maxX) transform.position = new Vector3(_minX, transform.position.y, 0f);

        Vector3 _rotationAxis = new Vector3(0f, 0f, 1f);
        float _rotationAngle = (_direction - Vector3.left == Vector3.up || _direction - Vector3.left == Vector3.down) ? -_rotationSpd : _rotationSpd;
        transform.Rotate(_rotationAxis, _rotationAngle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string _what = other.tag;

        if (_what == "Laser")
        {
            Laser laser = other.transform.GetComponent<Laser>();
            if (laser == null) { Debug.LogError("Asteroid::OnTriggerEnter2D() :: Got a null laser component somehow"); }

            float damage = laser.GetPower();
            Destroy(other.gameObject);
            TakeDamage(damage);
        }
    }

    void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            _ = Instantiate(_spawnManager.GetEffect("Explosion"), transform.position, Quaternion.identity);

            AudioSource.PlayClipAtPoint(_e_sounds.clip, transform.position);
            _gameManager.IncreaseLevel();
            _spawnManager.EnableEnemySpawn();
            _spawnManager.EnablePowerUpSpawn();
            Destroy(this.gameObject);
        }
    }
}

