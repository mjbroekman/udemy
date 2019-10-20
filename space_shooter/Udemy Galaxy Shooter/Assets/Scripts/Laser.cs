using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Base attributes
    private float _power;
    private float _speed;

    // SpriteRenderer to be able to modify the laser depending on powerups
    private SpriteRenderer _l_spriteRenderer;
    private AudioSource _l_sounds;
    private AudioManager _audioManager;
    private Transform _parent;
    private Vector3 _direction;
    private string _owner;

    private void Start()
    {
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (_audioManager == null) { Debug.LogError("Laser::Start() :: We have a problem. The audioManager is null"); }

        _l_spriteRenderer = GetComponent<SpriteRenderer>();
        if (_l_spriteRenderer == null) { Debug.LogError("Laser::Start() :: Unable to grab the SpriteRenderer."); }
        _l_spriteRenderer.sortingOrder = 0;

        this.gameObject.AddComponent<AudioSource>();
        _l_sounds = this.GetComponent<AudioSource>();
        _l_sounds.loop = false;
        _l_sounds.volume = 0.75f;
        _l_sounds.pitch = 0.5f;

        _power = 2f; // base power is 2 damage
        _speed = 8f; // base speed is 8 units

        _parent = transform.parent;
        _direction = Vector3.up;
        _owner = "";

        if (_parent != null)
        {
            //Debug.Log("Laser::Start() :: I was fired by " + _parent.name);
            if (_parent.name.Contains("TripleShot"))
            {
                _parent.transform.parent = null;
                // The tripleshot is weaker per laser, but if you hit an enemy with 2 or 3 of them, you do more damage.
                _power = 1.5f;
                _speed = 9.0f;
                _l_sounds.volume = 0.5f;
                _l_sounds.pitch = 0.25f;
                _l_spriteRenderer.color = Color.green;
                _parent.transform.localScale = new Vector3(1f, 1f, 0f);
                transform.localScale = new Vector3(0.35f, 0.15f, 0f);

                _owner = "Player";
                Destroy(_parent.gameObject, 1.5f);
                Destroy(this.gameObject, 1.5f);
            }
            else if (_parent.name.Contains("Enemy"))
            {
                transform.parent = null;
                _owner = "Enemy";
                _power = 2f;
                _speed = 7f;

                _l_sounds.volume = 0.5f;

                _l_spriteRenderer.color = Color.yellow;
                _direction = Vector3.down;
                transform.localScale = new Vector3(0.25f, 0.25f, 0f);

                Destroy(this.gameObject, 1.9f);
            }
            else if (_parent.name.Contains("Boss"))
            {
                transform.parent = null;
                _owner = "Boss";
                _power = 5f;
                _speed = -3f;

                _l_sounds.volume = 1f;
                _l_sounds.pitch = 1f;

                _l_spriteRenderer.color = Color.cyan;
                transform.localScale = new Vector3(0.75f, 0.25f, 0f);

                Destroy(this.gameObject, 5f);
            }
            else if (_parent.name.Contains("Player"))
            {
                transform.parent = null;
                _owner = "Player";
                _l_spriteRenderer.color = Color.red;
                transform.localScale = new Vector3(0.15f, 0.15f, 0f);

                Destroy(this.gameObject, 1.9f);
            }
            else
            {
                Debug.Log("Laser::Start :: Unknown parent " + _parent.name + ". No lasers for you until Laser.cs is coded for you.");
                Destroy(this.gameObject);
            }
            // add other variations of the laser here as elseif comparisons to the parent name
        }
        else
        {
            Debug.Log("Laser::Start() :: Parent is NULL!!! This shouldn't happen. No lasers for you.");
            Destroy(this.gameObject);
        }

        //Debug.Log("Laser::ConfigureLaser :: Finding audio clip for " + _owner + " weapon.");
        _l_sounds.clip = _audioManager.GetWeaponSound(_owner);
        //Debug.Log("Laser::ConfigureLaser :: Found audio clip " + _audioManager.GetWeaponSound(_owner));
        AudioSource.PlayClipAtPoint(_l_sounds.clip, transform.position);
    }

    void Update()
    {
        _direction -= new Vector3(0f, 0f, _direction.z);
        transform.Translate(_direction * _speed * Time.deltaTime);
    }

    public bool IsHostile(string who)
    {
        if (_owner == who) { return false; }
        return true;
    }

    public float GetPower() { return _power; }
}
