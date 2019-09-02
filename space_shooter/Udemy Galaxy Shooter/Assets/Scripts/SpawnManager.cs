using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private IEnumerator spawnroutine;
    [SerializeField]
    private float _maxDelay;
    [SerializeField]
    private float _minDelay;
    [SerializeField]
    private GameObject _spawnObj;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerUpContainer;

    private bool _stopSpawning;

    // Start is called before the first frame update
    void Start()
    {
        _maxDelay = 2.0f;
        _minDelay = 0.1f;
        spawnroutine = SpawnRoutine();
        StartCoroutine(spawnroutine);
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
            float _randChance = Random.Range(0f, 100f);
            if (_randChance <= 95f)
            {
                Debug.Log("Spawn a regular enemy");
            }
            else if (_randChance <= 98f)
            {
                Debug.Log("Instantiat a 'boss' enemy");
            }
            else
            {
                Debug.Log("Instantiate a powerup");
            }
            if (_spawnObj != null)
            {
                GameObject newSpawn = Instantiate(_spawnObj);
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
            yield return new WaitForSeconds(5.0f);
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
