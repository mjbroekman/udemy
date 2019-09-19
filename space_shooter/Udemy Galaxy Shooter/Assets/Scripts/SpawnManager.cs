using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnManager : MonoBehaviour
{
    // Spawn containers and collections
    private GameObject _enemyContainer;
    private GameObject _powerUpContainer;

    private Dictionary<string, GameObject> _enemySpawns;
    private List<GameObject> _powerUpSpawns;

    private Dictionary<string, GameObject> _effectsManager;

    private GameObject[] _objectsInGame;
    private Dictionary<string, GameObject> _objectSpawns;

    // Spawn management
    private IEnumerator spawnEnemyRoutine;   // Enemy Spawn Coroutine
    private IEnumerator spawnPowerUpRoutine; // PowerUp Spawn Coroutine
    private IEnumerator spawnObjectRoutine;  // Object (Asteroid) Spawn Coroutine
    private bool _stopEnemySpawning;         // Should we stop spawning new enemy objects
    private bool _stopPowerUpSpawning;       // Should we stop spawning new enemy objects
    private bool _stopObjectSpawning;        // Should we stop spawning misc objects
    private float _bossRate;                 // How often should 'boss' enemies appear
    private float _maxEDelay;                // Maximum delay between enemy spawns
    private float _minEDelay;                // Minimum delay between enemy spawns
    private float _maxPDelay;                // Maximum delay between powerup spawns
    private float _minPDelay;                // Minimum delay between powerup spawns

    // Screen boundaries and positioning
    private float _minV;
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
        // Set up scene containers and spawnables
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

        _effectsManager = new Dictionary<string, GameObject>();
        LoadAssets("Effects");

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("SpawnManager::Start() :: We have a problem. The gameManager is null");
        }

        // Set up spawn coroutines
        spawnEnemyRoutine = SpawnEnemyRoutine();
        EnableEnemySpawn();
        spawnPowerUpRoutine = SpawnPowerUpRoutine();
        EnablePowerUpSpawn();
        spawnObjectRoutine = SpawnObjectRoutine();
        EnableObjectSpawn();
        _isStarted = true;
    }

    public bool IsStarted()
    {
        return _isStarted;
    }

    /// <summary>
    /// Load up the assets that the SpawnManager will control
    /// </summary>
    /// <param name="asset"></param>
    void LoadAssets(string asset)
    {
        string prefabPath = "Assets/Prefabs/" + asset;
        Debug.Log("SpawnManager::LoadAssets() :: Attempting to load from " + prefabPath);
        foreach (var guid in AssetDatabase.FindAssets("t:GameObject", new[] { prefabPath }))
        {
            GameObject newObj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
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
        EnableEnemySpawn();
        EnablePowerUpSpawn();
    }

    // Control the spawning of Enemies
    public void DisableEnemySpawn()
    {
        _stopEnemySpawning = true;
        if (spawnEnemyRoutine != null) StopCoroutine(spawnEnemyRoutine);
    }

    public void EnableEnemySpawn()
    {
        _stopEnemySpawning = false;
        if (spawnEnemyRoutine != null) StartCoroutine(spawnEnemyRoutine);
    }

    // Control the spawning of PowerUps
    public void DisablePowerUpSpawn()
    {
        _stopPowerUpSpawning = true;
        if (spawnPowerUpRoutine != null) StopCoroutine(spawnPowerUpRoutine);
    }

    public void EnablePowerUpSpawn()
    {
        _stopPowerUpSpawning = false;
        if (spawnPowerUpRoutine != null) StartCoroutine(spawnPowerUpRoutine);
    }

    // Control the spawning of misc objects
    public void DisableObjectSpawn()
    {
        _stopObjectSpawning = true;
        if (spawnObjectRoutine != null) StopCoroutine(spawnObjectRoutine);
    }

    public void EnableObjectSpawn()
    {
        _stopObjectSpawning = false;
        if (spawnObjectRoutine != null) StartCoroutine(spawnObjectRoutine);
    }

    public GameObject GetEffect(string what)
    {
        return _effectsManager.ContainsKey(what) ? _effectsManager[what] : null;
    }

    void Update() { }

    IEnumerator SpawnEnemyRoutine()
    {
        // Set up delays
        _maxEDelay = 2f;
        _minEDelay = 0.75f;
        _bossRate = 10f * _maxEDelay;

        GameObject _spawnObj = null;     // Object to use in the Instantiate() call
        _bounds = _gameManager.GetScreenBoundaries("Enemy");
        _maxH = _bounds[1];
        _minV = _bounds[2];
        _maxV = _bounds[3];

        while (!_stopEnemySpawning)
        {
            //Debug.Log("SpawnManager::SpawnEnemyRoutine() :: Next enemy spawn between " + _minEDelay + " and " + _maxEDelay + " seconds from now.");
            float _delay = Random.Range(_minEDelay, _maxEDelay);
            yield return new WaitForSeconds(_delay);

            // Lower the delays with each spawn
            if (_minEDelay > 0.1f) { _minEDelay -= Time.deltaTime / 1200f; }
            if (_maxEDelay > 1f) { _maxEDelay -= Time.deltaTime / 1800f; }

            // All powerups start from the top of the screen
            float _spawnV = _maxV;

            // Figure out what exactly to spawn
            float _randSpawn = Random.Range(0f, 100f);

            if (_randSpawn <= 90f)
            {
                float _randType = Random.Range(0f, 100f);
                if (_randType > 90f && (Time.time % _bossRate) > 15f)
                {
                    _spawnV = _minV;
                    _spawnObj = _enemySpawns.ContainsKey("Boss") ? _enemySpawns["Boss"] : null;
                }
                else
                {
                    _spawnObj = _enemySpawns.ContainsKey("Enemy") ? _enemySpawns["Enemy"] : null;
                }
            }

            if (_spawnObj != null)
            {
                _randomX = Random.Range(-_maxH, _maxH);
                GameObject newSpawn = Instantiate(_spawnObj, new Vector3(_randomX, _spawnV, 0.0f), Quaternion.identity);
                newSpawn.transform.parent = _enemyContainer.transform;
                var newEnemy = newSpawn.GetComponent<Enemy_base>();
                if (newEnemy != null)
                {
                    newEnemy.SetGameStart(_gameTime);
                }
            }
        }
    }

    IEnumerator SpawnObjectRoutine()
    {
        GameObject _spawnObj;     // Object to use in the Instantiate() call
        _bounds = _gameManager.GetScreenBoundaries("Objects");
        _maxH = _bounds[1];
        _minV = _bounds[2];
        _maxV = _bounds[3];
        float _spawnV;

        while (!_stopObjectSpawning)
        {
            yield return new WaitForSeconds(30f);
            _spawnObj = null;
            Debug.Log("SpawnManager::SpawnObjectRoutine() :: There are " + _objectsInGame.Length + " misc objects in the game right now and " + (Time.time - _gameTime) + " seconds have elapsed in the game.");
            if (_objectsInGame.Length == 0 && ((Time.time - _gameTime) / 30f) > _gameManager.GetLevel())
            {
                Debug.Log("SpawnManager::SpawnObjectRoutine() :: Trying to spawn a misc object. Passed time check and objectsInGame is empty");
                if (Random.Range(0f, 1f) > 0.1f)
                {
                    Debug.Log("SpawnManager::SpawnObjectRoutine() :: Trying to spawn a misc object. Passed random chance check 1.");
                    float _randType = Random.Range(0f, 100f);
                    if (_randType < (100f / (float)_objectSpawns.Count))
                    {
                        Debug.Log("SpawnManager::SpawnObjectRoutine() :: Picked an object from _objectSpawns.");
                        _spawnObj = _objectSpawns.ContainsKey("Asteroid") ? _objectSpawns["Asteroid"] : null;
                    }
                }
            }
            if (_spawnObj != null)
            {
                Debug.Log("SpawnManager::SpawnObjectRoutine() :: I spawned a thing!");
                _randomX = Random.Range(-_maxH, _maxH);
                _spawnV = (Random.Range(0, 2) == 0) ? -_maxV : _maxV;
                GameObject newSpawn = Instantiate(_spawnObj, new Vector3(_randomX, _spawnV, 0.0f), Quaternion.identity);
                _objectsInGame = new GameObject[] { newSpawn };
            }
            if (_objectsInGame.Length == 1)
            {
                Debug.Log("SpawnManager::SpawnObjectRoutine() :: There is a misc object in game.");
                if (_objectsInGame[0] == null)
                {
                    Debug.Log("SpawnManager::SpawnObjectRoutine() :: The misc object is actually null. Resetting array.");
                    _objectsInGame = new GameObject[] { };
                }
                else if (!_objectsInGame[0].activeInHierarchy)
                {
                    Debug.Log("SpawnManager::SpawnObjectRoutine() :: The misc object is not active. Destroying object and resetting array.");
                    Destroy(_objectsInGame[0]);
                    _objectsInGame = new GameObject[] { };
                }
            }
        }
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        // Set up delays
        _maxPDelay = 30f;
        _minPDelay = 15f;

        GameObject _spawnObj;     // Object to use in the Instantiate() call
        _bounds = _gameManager.GetScreenBoundaries("PowerUp");
        _maxH = _bounds[1];
        _minV = _bounds[2];
        _maxV = _bounds[3];

        while (!_stopPowerUpSpawning)
        {
            //Debug.Log("SpawnManager::SpawnPowerUpRoutine() :: Next powerup spawn between " + _minPDelay + " and " + _maxPDelay + " seconds from now.");
            float _delay = Random.Range(_minPDelay, _maxPDelay);
            yield return new WaitForSeconds(_delay);

            // Increase the delays with each spawn
            if (_minPDelay < _maxPDelay) { _minPDelay += Time.deltaTime / 1200f; }
            if (_maxPDelay < 300f) { _maxPDelay += Time.deltaTime / 900f; }

            // All powerups start from the top of the screen
            float _spawnV = _maxV;

            // Pick a random powerup from the loaded powerUp objects
            int _randType = Random.Range(0, _powerUpSpawns.Count);
            _spawnObj = _powerUpSpawns[_randType];
            Debug.Log("SpawnManager::SpawnPowerUpRoutine() :: Spawning a " + _spawnObj);

            if (_spawnObj != null)
            {
                _randomX = Random.Range(-_maxH, _maxH);
                GameObject newSpawn = Instantiate(_spawnObj, new Vector3(_randomX, _spawnV, 0.0f), Quaternion.identity);

                newSpawn.transform.parent = _powerUpContainer.transform;
            }
        }
    }

    public void OnPlayerDeath(int lives)
    {
        _gameTime = Time.time / 2;
        Debug.Log("SpawnManager::OnPlayerDeath() :: Resetting game time to " + _gameTime);
        // Disable enemy/powerup spawning and stop coroutines if that was the last life
        if (lives == 0)
        {
            // Destroy any lingering powerups and enemies
            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < objectsToDestroy.Length; i++)
            {
                Destroy(objectsToDestroy[i]);
            }
            objectsToDestroy = GameObject.FindGameObjectsWithTag("PowerUp");
            for (int i = 0; i < objectsToDestroy.Length; i++)
            {
                Destroy(objectsToDestroy[i]);
            }

            DisableEnemySpawn();
            DisablePowerUpSpawn();
            DisableObjectSpawn();
        }
    }
}
