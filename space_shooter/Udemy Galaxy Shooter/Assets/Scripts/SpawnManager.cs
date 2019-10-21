using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Spawn containers and collections
    private GameObject _enemyContainer;
    private GameObject _powerUpContainer;

    private Dictionary<string, GameObject> _enemySpawns;
    private List<GameObject> _powerUpSpawns;

    private Dictionary<string, GameObject> _effectsManager;
    private Dictionary<string, Vector3> _effectsLocation;

    private GameObject[] _objectsInGame;
    private Dictionary<string, GameObject> _objectSpawns;

    // Spawn management
    private IEnumerator spawnEnemyRoutine;   // Enemy Spawn Coroutine
    private IEnumerator spawnPowerUpRoutine; // PowerUp Spawn Coroutine
    private IEnumerator spawnObjectRoutine;  // Object (Asteroid) Spawn Coroutine
    private bool _stopEnemySpawning;         // Should we stop spawning new enemy objects
    private bool _stopPowerUpSpawning;       // Should we stop spawning new enemy objects
    private bool _stopObjectSpawning;        // Should we stop spawning misc objects
    private bool _playerDead;
    private float _bossRate;                 // How often should 'boss' enemies appear
    private float _maxEDelay;                // Maximum delay between enemy spawns
    private float _minEDelay;                // Minimum delay between enemy spawns
    private float _maxPDelay;                // Maximum delay between powerup spawns
    private float _minPDelay;                // Minimum delay between powerup spawns

    // Tags to cycle through on Player Death 
    protected internal readonly string[] _tagsToKill = { "Enemy", "PowerUp", "Laser" };

    // Screen boundaries and positioning
    private float _maxV;
    private float _maxH;
    private float _gameTime;
    private float _randomX;
    private float[] _bounds;

    //Game Control
    private bool _isStarted;
    private GameManager _gameManager;

    void Start()
    {
        _isStarted = false;
        _gameTime = Time.time;

        _maxPDelay = 30f;
        _minPDelay = 15f;

        _maxEDelay = 2f;
        _minEDelay = 0.75f;

        _enemyContainer = new GameObject("Enemy Container");
        _enemyContainer.transform.parent = this.transform;
        _enemySpawns = new Dictionary<string, GameObject>();
        LoadAssets("Enemies");

        _powerUpContainer = new GameObject("PowerUp Container");
        _powerUpContainer.transform.parent = this.transform;
        _powerUpSpawns = new List<GameObject>();
        LoadAssets("PowerUps");

        _objectsInGame = new GameObject[] { };
        _objectSpawns = new Dictionary<string, GameObject>();
        LoadAssets("Objects");

        _effectsLocation = new Dictionary<string, Vector3>();
        _effectsManager = new Dictionary<string, GameObject>();
        LoadAssets("Effects");

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null) Debug.LogError("SpawnManager::Start() :: We have a problem. The gameManager is null");

        EnableEnemySpawn();
        EnablePowerUpSpawn();
        EnableObjectSpawn();

        _isStarted = true;
        _playerDead = true;
    }

    public bool IsStarted() { return _isStarted; }

    void LoadAssets(string asset)
    {
        string aPath = "Prefabs/" + asset;
        Debug.Log("SpawnManager::LoadAssets() :: Attempting to load from " + aPath);
        GameObject[] obj = Resources.LoadAll<GameObject>(aPath);
        Debug.Log("Found " + obj.Length + " objects in " + aPath);
        foreach (GameObject newObj in obj)
        {
            Debug.Log("SpawnManager::LoadAssets() :: Loading " + newObj.name);
            switch (asset)
            {
                case "Enemies":
                    _enemySpawns.Add(newObj.name, newObj);
                    break;
                case "PowerUps":
                    _powerUpSpawns.Add(newObj);
                    break;
                case "Effects":
                    _effectsManager.Add(newObj.name, newObj);
                    switch (newObj.name)
                    {
                        case "Thruster": _effectsLocation.Add(newObj.name, new Vector3(0f, -3.3f, 0f)); break;
                        case "Fire_Effect_1": _effectsLocation.Add(newObj.name, new Vector3(1.45f, -3.77f, 0f)); break;
                        case "Fire_Effect_2": _effectsLocation.Add(newObj.name, new Vector3(-1f, -3.5f, 0f)); break;
                        case "Fire_Effect_3": _effectsLocation.Add(newObj.name, new Vector3(0.5f, -2.5f, 0f)); break;
                    }
                    break;
                case "Objects":
                    _objectSpawns.Add(newObj.name, newObj);
                    break;
            }
        }
    }

    public void RestartGame()
    {
        _gameTime = Time.time;
        ResetSpawners();
    }

    private void ResetSpawners()
    {
        DisableEnemySpawn();
        DisablePowerUpSpawn();
        DisableObjectSpawn();
        EnableEnemySpawn();
        EnablePowerUpSpawn();
        EnableObjectSpawn();
    }

    public void DisableEnemySpawn()
    {
        _stopEnemySpawning = true;
        if (spawnEnemyRoutine != null) { StopCoroutine(spawnEnemyRoutine); spawnEnemyRoutine = null; }
    }

    public void EnableEnemySpawn()
    {
        _stopEnemySpawning = false;
        if (spawnEnemyRoutine != null) { StartCoroutine(spawnEnemyRoutine); }
        else { spawnEnemyRoutine = SpawnEnemyRoutine(); StartCoroutine(spawnEnemyRoutine); }
    }

    public void DisablePowerUpSpawn()
    {
        _stopPowerUpSpawning = true;
        if (spawnPowerUpRoutine != null) { StopCoroutine(spawnPowerUpRoutine); spawnPowerUpRoutine = null; }
    }

    public void EnablePowerUpSpawn()
    {
        _stopPowerUpSpawning = false;
        if (spawnPowerUpRoutine != null) { StartCoroutine(spawnPowerUpRoutine); }
        else { spawnPowerUpRoutine = SpawnPowerUpRoutine(); StartCoroutine(spawnPowerUpRoutine); }
    }

    public void DisableObjectSpawn()
    {
        _stopObjectSpawning = true;
        if (spawnObjectRoutine != null) { StopCoroutine(spawnObjectRoutine); spawnObjectRoutine = null; }
    }

    public void EnableObjectSpawn()
    {
        _stopObjectSpawning = false;
        if (spawnObjectRoutine != null) { StartCoroutine(spawnObjectRoutine); }
        else { spawnObjectRoutine = SpawnObjectRoutine(); StartCoroutine(spawnObjectRoutine); }
    }

    public GameObject GetEffect(string what) { return _effectsManager.ContainsKey(what) ? _effectsManager[what] : null; }

    public Vector3 GetEffectLocation(string what) { return _effectsLocation.ContainsKey(what) ? _effectsLocation[what] : new Vector3(0f, 0f, 0f); }

    void Update() { }

    IEnumerator SpawnEnemyRoutine()
    {
        _bossRate = 10f * _maxEDelay;

        GameObject _spawnObj = null;
        _bounds = _gameManager.GetScreenBoundaries("Enemy");
        _maxH = _bounds[1];
        _maxV = _bounds[3];

        while (!_stopEnemySpawning)
        {
            float _delay = Random.Range(_minEDelay, _maxEDelay);

            // If the player is dead, wait _delay seconds before checking again.
            if (_playerDead) { yield return new WaitForSeconds(_delay); _playerDead = false; }
            // If there is a boss spawned, wait _delay seconds before trying to spawn again.
            else if (_enemyContainer.GetComponentInChildren<Enemy_boss>() != null) { yield return new WaitForSeconds(_delay); }
            // If there are more than 8+level enemies in the _enemyContainer, wait _delay seconds before trying to spawn again
            else if (_enemyContainer.transform.childCount > (3 + _gameManager.GetLevel())) { yield return new WaitForSeconds(_delay); }
            // SPAWN!!!
            else
            {
                yield return new WaitForSeconds(_delay);

                if (_minEDelay > 0.1f) { _minEDelay -= Time.deltaTime / 1200f; }
                if (_maxEDelay > 1f) { _maxEDelay -= Time.deltaTime / 1800f; }

                float _randSpawn = Random.Range(0f, 100f);

                if (_randSpawn <= 90f && !_playerDead)
                {
                    float _randType = Random.Range(0f, 100f);
                    if (_randType > 90f && (Time.time % _bossRate) > 15f) { _spawnObj = _enemySpawns.ContainsKey("Boss") ? _enemySpawns["Boss"] : null; }
                    else { _spawnObj = _enemySpawns.ContainsKey("Enemy") ? _enemySpawns["Enemy"] : null; }
                }

                if (_spawnObj != null)
                {
                    GameObject newSpawn = Instantiate(_spawnObj, new Vector3(_maxH, _maxV, 0.0f), Quaternion.identity);
                    newSpawn.transform.parent = _enemyContainer.transform;
                }
            }
        }
    }

    IEnumerator SpawnObjectRoutine()
    {
        GameObject _spawnObj;
        _bounds = _gameManager.GetScreenBoundaries("Objects");
        _maxH = _bounds[1];
        _maxV = _bounds[3];
        float _spawnV;

        while (!_stopObjectSpawning)
        {
            yield return new WaitForSeconds(30f);
            _spawnObj = null;

            if (_objectsInGame.Length == 0 && ((Time.time - _gameTime) / 30f) > _gameManager.GetLevel() && !_playerDead)
            {
                if (Random.Range(0f, 1f) > 0.1f)
                {
                    float _randType = Random.Range(0f, 100f);
                    if (_randType < (100f / (float)_objectSpawns.Count)) { _spawnObj = _objectSpawns.ContainsKey("Asteroid") ? _objectSpawns["Asteroid"] : null; }
                }

                if (_spawnObj != null)
                {
                    _randomX = Random.Range(-_maxH, _maxH);
                    _spawnV = (Random.Range(0, 2) == 0) ? -_maxV : _maxV;
                    GameObject newSpawn = Instantiate(_spawnObj, new Vector3(_randomX, _spawnV, 0.0f), Quaternion.identity);
                    _objectsInGame = new GameObject[] { newSpawn };
                }
            }
            else if (_objectsInGame.Length == 1)
            {
                if (_objectsInGame[0] == null)
                {
                    EnableEnemySpawn();
                    EnablePowerUpSpawn();
                    _objectsInGame = new GameObject[] { };
                }
                else if (!_objectsInGame[0].activeInHierarchy)
                {
                    EnableEnemySpawn();
                    EnablePowerUpSpawn();
                    Destroy(_objectsInGame[0]);
                    _objectsInGame = new GameObject[] { };
                }
            }
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        // Set up delays
        GameObject _spawnObj;
        _bounds = _gameManager.GetScreenBoundaries("PowerUp");
        _maxH = _bounds[1];
        _maxV = _bounds[3];

        yield return new WaitForSeconds(2f);

        while (!_stopPowerUpSpawning)
        {
            float _delay = Random.Range(_minPDelay, _maxPDelay);

            yield return new WaitForSeconds(_delay);

            // If the player isn't dead and there's no powerup already spawned, spawn another powerup.
            if (!_playerDead && _powerUpContainer.transform.childCount < 1)
            {
                if (_minPDelay < _maxPDelay) { _minPDelay += Time.time / 1200f; }
                if (_maxPDelay < 300f) { _maxPDelay += Time.time / 900f; }

                float _spawnV = _maxV;

                int _randType = Random.Range(0, _powerUpSpawns.Count);
                _spawnObj = _powerUpSpawns[_randType];

                if (_spawnObj != null)
                {
                    _randomX = Random.Range(-_maxH, _maxH);
                    GameObject newSpawn = Instantiate(_spawnObj, new Vector3(_randomX, _spawnV, 0.0f), Quaternion.identity);

                    newSpawn.transform.parent = _powerUpContainer.transform;
                }
            }
        }
    }

    public void OnPlayerDeath(int lives)
    {
        _gameTime = Time.time / 2;
        Debug.Log("SpawnManager::OnPlayerDeath() :: Resetting game time to " + _gameTime);

        _playerDead = true;

        DisableEnemySpawn();
        DisablePowerUpSpawn();
        DisableObjectSpawn();

        if (lives > 0)
        {
            EnableEnemySpawn();
            EnablePowerUpSpawn();
            EnableObjectSpawn();
        }
    }

    public bool IsPlayerDead() { return _playerDead; }
}
