using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurderHole : MonoBehaviour
{
    [SerializeField]
    private Transform _spawn;

    void Start()
    {
        _spawn = GameObject.Find("Spawn_Point").GetComponent<Transform>();
        if (_spawn == null)
        {
            Debug.LogError("_spawn is NULL");
        }
        Debug.LogError(_spawn.position);
    }

    // If the player lands on this non-object, kill and respawn.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.LogError("Player has entered the MurderHole");
            Player p = other.GetComponent<Player>();
            if (p != null)
            {
                p.Damage();
                p.Respawn();
            }
        }
    }
}
