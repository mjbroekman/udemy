using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private AudioClip _pickup;
    [SerializeField]
    private bool _isDestroyed;

    public int Value { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Value = (int)Random.Range(50, 100);
        _pickup = Resources.Load<AudioClip>("Sounds/Pickup");
        _isDestroyed = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided! Press E for everything good!");
            if (Input.GetKey(KeyCode.E) && !_isDestroyed)
            {
                Debug.Log("Player pressed E! Woo hoo! They're RICH!");
                _isDestroyed = true;
                Player _p = other.GetComponent<Player>();
                _p.AddCash(Value);
                AudioSource.PlayClipAtPoint(_pickup, transform.position, 1f);
                Destroy(this.gameObject, 0.1f);
            }
        }
    }
}
