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
    private GameObject _spawnObj;     // Object to use in the Instantiate() call
    private IEnumerator spawnroutine; // Spawn Coroutine
    private bool _stopSpawning;       // Should we stop spawning new objects
    private float _maxDelay;          // Maximum delay between spawns
    private float _minDelay;          // Minimum delay between spawns
    [SerializeField]
    private float _bossRate;          // How often should 'boss' enemies appear

    // Screen boundaries and positioning
    private readonly float _maxH = 9.5f;
    private readonly float _maxV = 6.5f;
    private readonly float _minV = -5f;
    private float _randomX;

    void Start()
    {
        // Set up delays
        _maxDelay = 2f;
        _minDelay = 0.5f;
        _bossRate = 10f * _maxDelay;

        // Set up scene containers and spawnables
        _enemyContainer = new GameObject("Enemy Container");
        _enemyContainer.transform.parent = this.transform;
        _enemySpawns = new Dictionary<string, GameObject>();
        LoadAssets("Enemies");

        _powerUpContainer = new GameObject("PowerUp Container");
        _powerUpContainer.transform.parent = this.transform;
        _powerUpSpawns = new List<GameObject>();
        LoadAssets("PowerUps");

        spawnroutine = SpawnRoutine();
        StartCoroutine(spawnroutine);
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
            if (asset == "Enemies")
            {
                _enemySpawns.Add(newObj.name, newObj);
            }
            if (asset == "PowerUps")
            {
                _powerUpSpawns.Add(newObj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Spawn the appropriate stuff here.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnRoutine()
    {
        while (!_stopSpawning)
        {
            float _randSpawn = Random.Range(0f, 100f);
            float _spawnV;

            if (_randSpawn <= 90f)
            {
                float _randType = Random.Range(0f, 100f);
                if (_randType > 90f && (Time.time % _bossRate) > 15f)
                {
                    _spawnV = _minV;
                    if (_enemySpawns.ContainsKey("Boss"))
                    {
                        Debug.Log("Found a Boss mob to spawn.");
                        _spawnObj = _enemySpawns["Boss"];
                    }
                    else
                    {
                        Debug.Log("Couldn't find a 'Boss' in the _enemySpawns dictionary");
                        _spawnObj = null;
                    }
                }
                else
                {
                    _spawnV = _maxV;
                    if (_enemySpawns.ContainsKey("Enemy"))
                    {
                        Debug.Log("Found a basic enemy to spawn");
                        _spawnObj = _enemySpawns["Enemy"];
                    }
                    else
                    {
                        Debug.Log("Couldn't find an 'Enemy' in the _enemySpawns dictionary");
                        _spawnObj = null;
                    }
                }
            }
            else
            {
                // All powerups start from the top of the screen
                _spawnV = _maxV;
                // Pick a random powerup from the loaded powerUp objects
                int _randType = Random.Range(0, _powerUpSpawns.Count);
                _spawnObj = _powerUpSpawns[_randType];
                Debug.Log("Spawning a " + _spawnObj);
            }

            if (_spawnObj != null)
            {
                _randomX = Random.Range(-_maxH, _maxH);
                GameObject newSpawn = Instantiate(_spawnObj, new Vector3(_randomX, _spawnV, 0.0f), Quaternion.identity);

                if (newSpawn.tag == "Enemy")
                {
                    newSpawn.transform.parent = _enemyContainer.transform;
                }
                else if (newSpawn.tag == "PowerUp")
                {
                    newSpawn.transform.parent = _powerUpContainer.transform;
                }
            }

            float _delay = Random.Range(_minDelay, _maxDelay);
            yield return new WaitForSeconds(_delay);
        }
        Debug.Log("Player is dead. Stopped spawning.");
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("Enemy");
        for (var i = 0; i < objectsToDestroy.Length; i++)
        {
            Destroy(objectsToDestroy[i]);
        }
        objectsToDestroy = GameObject.FindGameObjectsWithTag("PowerUp");
        for (var i = 0; i < objectsToDestroy.Length; i++)
        {
            Destroy(objectsToDestroy[i]);
        }
    }
}
