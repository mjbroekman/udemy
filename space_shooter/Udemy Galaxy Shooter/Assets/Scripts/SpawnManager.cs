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

    // Spawn management
    private IEnumerator spawnEnemyRoutine;   // Enemy Spawn Coroutine
    private IEnumerator spawnPowerUpRoutine; // Enemy Spawn Coroutine
    private bool _stopEnemySpawning;         // Should we stop spawning new enemy objects
    private bool _stopPowerUpSpawning;         // Should we stop spawning new enemy objects
    private float _bossRate;                 // How often should 'boss' enemies appear
    private float _maxEDelay;                // Maximum delay between enemy spawns
    private float _minEDelay;                // Minimum delay between enemy spawns
    private float _maxPDelay;                // Maximum delay between powerup spawns
    private float _minPDelay;                // Minimum delay between powerup spawns

    // Screen boundaries and positioning
    private readonly float _maxH = 9.5f;
    private readonly float _maxV = 6.5f;
    private readonly float _minV = -5f;
    private float _randomX;

    void Start()
    {
        // Set up scene containers and spawnables
        _enemyContainer = new GameObject("Enemy Container");
        _enemyContainer.transform.parent = this.transform;
        _enemySpawns = new Dictionary<string, GameObject>();
        LoadAssets("Enemies");

        _powerUpContainer = new GameObject("PowerUp Container");
        _powerUpContainer.transform.parent = this.transform;
        _powerUpSpawns = new List<GameObject>();
        LoadAssets("PowerUps");

        EnableEnemySpawn();
        EnablePowerUpSpawn();

        // Set up spawn coroutines
        spawnEnemyRoutine = SpawnEnemyRoutine();
        StartCoroutine(spawnEnemyRoutine);
        spawnPowerUpRoutine = SpawnPowerUpRoutine();
        StartCoroutine(spawnPowerUpRoutine);
    }

    /// <summary>
    /// Load up the assets that the SpawnManager will control
    /// </summary>
    /// <param name="asset"></param>
    void LoadAssets(string asset)
    {
        string prefabPath = "Assets/Prefabs/" + asset;
        Debug.Log("Attempting to load from " + prefabPath);
        foreach (var guid in AssetDatabase.FindAssets("t:GameObject", new[] { prefabPath }))
        {
            GameObject newObj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            Debug.Log("Loading " + newObj.name);
            if (asset == "Enemies") { _enemySpawns.Add(newObj.name, newObj); }
            if (asset == "PowerUps") { _powerUpSpawns.Add(newObj); }
        }
    }


    // Control the spawning of Enemies
    public void DisableEnemySpawn()
    {
        _stopEnemySpawning = true;
    }

    public void EnableEnemySpawn()
    {
        _stopEnemySpawning = false;
    }

    // Control the spawning of PowerUps
    public void DisablePowerUpSpawn()
    {
        _stopPowerUpSpawning = true;
    }

    public void EnablePowerUpSpawn()
    {
        _stopPowerUpSpawning = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Spawn the enemies.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnEnemyRoutine()
    {
        // Set up delays
        _maxEDelay = 2f;
        _minEDelay = 0.75f;
        _bossRate = 10f * _maxEDelay;

        GameObject _spawnObj = null;     // Object to use in the Instantiate() call

        while (!_stopEnemySpawning)
        {
            Debug.Log("Next enemy spawn between " + _minEDelay + " and " + _maxEDelay + " seconds from now.");
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
            }
        }
    }

    /// <summary>
    /// Spawn the powerups
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnPowerUpRoutine()
    {
        // Set up delays
        _maxPDelay = 30f;
        _minPDelay = 15f;

        GameObject _spawnObj;     // Object to use in the Instantiate() call

        while (!_stopPowerUpSpawning)
        {
            Debug.Log("Next powerup spawn between " + _minPDelay + " and " + _maxPDelay + " seconds from now.");
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
            Debug.Log("Spawning a " + _spawnObj);

            if (_spawnObj != null)
            {
                _randomX = Random.Range(-_maxH, _maxH);
                GameObject newSpawn = Instantiate(_spawnObj, new Vector3(_randomX, _spawnV, 0.0f), Quaternion.identity);

                newSpawn.transform.parent = _powerUpContainer.transform;
            }
        }
    }

    public void OnPlayerDeath()
    {
        // Disable spawning and stop our coroutines
        DisableEnemySpawn();
        DisablePowerUpSpawn();
        StopAllCoroutines();

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
    }
}
