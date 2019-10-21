using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_boss : MonoBehaviour
{
    // Set max enemy life
    private readonly float _maxLife = 20f;
    [SerializeField]
    private float _curSpd;
    private float _curLife;
    private float _curShield;

    // Set screen boundary variables
    [SerializeField]
    private float _maxH;
    [SerializeField]
    private float _maxV;
    [SerializeField]
    private float[] _bounds;

    // The last direction we moved
    [SerializeField]
    private Vector3 _direction;
    [SerializeField]
    private bool _reverse;
    [SerializeField]
    private float _upDown = -1f;

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
    private GameObject _e_Shield;

    void Start()
    {
        SetupGameObjects();
        SetInitialPosition();

        _curLife = _maxLife;
        _curSpd = 2.5f;

        _firingCycle = FiringCycle();
        StartCoroutine(_firingCycle);

        _e_Thruster = new GameObject[4];
        SetEffects("Thruster", _e_Thruster.Length);
        SetEffects("Shield");

        _direction = Vector3.zero;
        _reverse = false;
    }

    private void SetEffects(string effect) { SetEffects(effect, 1); }

    private void SetEffects(string effect, int count)
    {
        switch (effect)
        {
            case "Thruster":
                for (int i = 0; i < count; i++)
                {
                    _e_Thruster[i] = Instantiate(_spawnManager.GetEffect("Thruster"), transform.position, Quaternion.identity, transform);
                    if (_e_Thruster[i] == null) { Debug.LogError("Unable to get Thruster effect from _spawnManager"); }

                    if (i % 2 == 0) { _e_Thruster[i].transform.localPosition += new Vector3(-1.03f + (1.03f * i), 0.48f, 0f); }
                    else { _e_Thruster[i].transform.localPosition += new Vector3(-0.25f + (0.25f * (i - 1)), 1.78f, 0f); }

                    _e_Thruster[i].transform.localScale = new Vector3(0.1f, 0.2f, 0f);
                    _e_Thruster[i].transform.Rotate(new Vector3(-180f, 0f, 0f));
                }
                break;
            case "Shield":
                _e_Shield = Instantiate(_spawnManager.GetEffect("Shield_PowerUp_Effect"), transform.position, Quaternion.identity, transform);
                if (_e_Shield == null) { Debug.LogError("Unable to get Shield effect from _spawnManager"); }

                _e_Shield.transform.localPosition += new Vector3(0f, -0.35f, 0f);
                _e_Shield.transform.localScale = new Vector3(1.25f, 1.25f, 1f);
                _e_Shield.GetComponent<SpriteRenderer>().color -= new Color(0f, 0f, 0f, 0.6f);

                _curShield = _gameManager.GetLevel() * 5f;
                Debug.Log("Enemy_boss::SetEffects() :: Shield has " + _curShield + " points");
                break;
        }
    }

    void Update()
    {
        if (_spawnManager.IsPlayerDead()) { Destroy(gameObject); }
        if (gameObject.GetComponent<SpriteRenderer>().enabled && !_spawnManager.IsPlayerDead()) { MoveEnemy(); }
    }

    private void SetInitialPosition()
    {
        _bounds = _gameManager.GetScreenBoundaries(this.gameObject);
        _maxH = _bounds[1];
        _maxV = _bounds[3];
        float _rand = Random.Range(-1f, 1f);
        transform.position = new Vector3(_rand, _maxV * 2f, 0.0f);
    }

    private void SetupGameObjects()
    {
        if (GameObject.Find("Player") != null) _player = GameObject.Find("Player").GetComponent<Player>();

        _pf_mainWeapon = Resources.Load<GameObject>("Prefabs/Weapons/Laser");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("Enemy_boss::Start() :: Houston, we have a problem. There is no Spawn_Manager in the scene.");

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null) Debug.LogError("Enemy_boss::Start() :: We have a problem. The gameManager is null");

        _e_animator = gameObject.GetComponent<Animator>();
        if (_e_animator != null) _e_animator.ResetTrigger("OnEnemyDeath");
        else Debug.LogError("Enemy_boss::Start() :: Unable to find Animator component");

        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        SetupAudioEffect();
    }

    private void SetupAudioEffect()
    {
        if (_audioManager == null) { Debug.LogError("Enemy_boss::Start() :: We have a problem. The audioManager is null"); }
        else
        {
            this.gameObject.AddComponent<AudioSource>();
            _e_sounds = this.GetComponent<AudioSource>();
            _e_sounds.clip = _audioManager.GetEffectSound("Explosion");
            if (_e_sounds == null || _e_sounds.clip == null) { Debug.LogError("Enemy_boss::Start() :: Something went wrong and the AudioSource or clip are null"); }
            _e_sounds.loop = false;
            _e_sounds.playOnAwake = false;
            _e_sounds.volume = 0.75f;
            _e_sounds.pitch = 0.35f;
        }
    }

    private void MoveEnemy()
    {
        // If we haven't started moving in a direction, pick one randomly.
        if (_direction == Vector3.zero)
        {
            int _rand = Random.Range(0, 2);
            switch (_rand)
            {
                case 0: _direction = Vector3.left; break;
                case 1: _direction = Vector3.right; break;
            }
        }

        // If we're on screen, move around a bit.
        if (transform.position.y < _maxV)
        {
            // Figure out how close we are to the sides of the screen
            Vector2 _l_vector = new Vector3(-_maxH, transform.position.y, 0f) - transform.position;
            Vector2 _r_vector = transform.position - new Vector3(_maxH, transform.position.y, 0f);
            Vector3 _move;

            float _l_distance = _l_vector.magnitude;
            float _r_distance = _r_vector.magnitude;

            // As we approach the borders, add some downward vector and reduce the horizontal vector
            if (_l_distance < 2f && _direction == Vector3.left) { _move = new Vector3(-_l_distance / 2f, _upDown * (2f - _l_distance) / 2f, 0f); }
            else if (_r_distance < 2f && _direction == Vector3.right) { _move = new Vector3(_r_distance / 2f, _upDown * (2f - _r_distance) / 2f, 0f); }
            else { _move = _direction; }

            // When we get really close to the border, start moving in the other direction.
            if (Mathf.Abs(_move.x) < 0.25f && !_reverse)
            {
                if (_direction == Vector3.left) { _direction = Vector3.right; _reverse = true; }
                else if (_direction == Vector3.right) { _direction = Vector3.left; _reverse = true; }
            }
            else if (Mathf.Abs(_move.x) > 0.25f && _reverse) { _reverse = false; }

            transform.Translate(_move * _curSpd * Time.deltaTime);

            if (transform.position.y < -3f) { _upDown = 1f; }
            if (transform.position.y > _maxV * 0.9f) { _upDown = -1f; }
        }
        else { transform.Translate(Vector3.down * _curSpd * Time.deltaTime); }
    }

    private IEnumerator FiringCycle()
    {
        while (gameObject.GetComponent<CapsuleCollider2D>().enabled)
        {
            float _delay = Random.Range(2f, 6f);
            _delay -= (_curLife / 10f);
            if (_delay < 0.25f) { _delay = 0.25f; }

            yield return new WaitForSeconds(_delay);

            if (_pf_mainWeapon != null)
            {
                _ = Instantiate(_pf_mainWeapon, transform.position + new Vector3(0.2f, -2f, 0), transform.rotation, transform);
                _ = Instantiate(_pf_mainWeapon, transform.position + new Vector3(-0.2f, -2f, 0), transform.rotation, transform);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string _what = other.tag;
        switch (_what)
        {
            case "Player":
                _player.TakeDamage(_curLife);
                TakeDamage(_curLife, false);
                break;
            case "Laser":
                Laser laser = other.transform.GetComponent<Laser>();
                if (laser == null) { Debug.LogError("Enemy_boss::OnTriggerEnter2D() :: Got a null laser component somehow"); }

                // Ignore lasers from small enemies
                if (!laser.IsHostile("Enemy")) { break; }
                // Ignore lasers that I fired
                if (!laser.IsHostile("Boss")) { break; }
                if (!laser.IsHostile("Player"))
                {
                    float damage = laser.GetPower();

                    Destroy(other.gameObject);
                    TakeDamage(damage, true);
                }

                break;
            case "Asteroid":
                // Asteroids and I ignore each other
                break;
            case "PowerUp":
                break;
            case "Enemy":
                break;
            default:
                Debug.Log("Enemy_boss::OnTriggerEnter2D() :: Hit something undefined");
                break;
        }
    }

    void TakeDamage(float damage, bool _byPlayer)
    {
        if (transform.position.y < (_maxV * 0.9))
        {
            if (_curShield > damage)
            {
                _curShield -= damage;
                damage = -1f;
            }
            else if (_curShield > 0f)
            {
                damage -= _curShield;
                _curShield = -1f;
                Destroy(_e_Shield);
            }
            if (damage > 0f)
            {
                Debug.Log("Enemy_boss::TakeDamage() :: Took " + damage + " points of damage.");
                _curLife -= damage;
                if (_curLife <= 0)
                {
                    if (_player == null) Debug.LogError("Enemy_boss::TakeDamage() :: Unable to find player object!");
                    else if (_byPlayer) { _player.IncreaseScore((int)_maxLife); }

                    // Trigger the explosion
                    _e_animator.SetTrigger("OnEnemyDeath");

                    // Turn off the Collider so it doesn't trigger collisions
                    gameObject.GetComponent<CapsuleCollider2D>().enabled = false;

                    // Remove the thruster effects
                    foreach (GameObject _effect in _e_Thruster) { Destroy(_effect); }

                    // Stop the firingCycle
                    if (_firingCycle != null) { StopCoroutine(_firingCycle); }
                    _firingCycle = null;

                    // Play the explosion sound
                    AudioSource.PlayClipAtPoint(_e_sounds.clip, transform.position);

                    // Destroy ourself
                    Destroy(this.gameObject, 1f);
                }
            }
        }
    }
}
