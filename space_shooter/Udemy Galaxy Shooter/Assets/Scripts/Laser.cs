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

    private void Start()
    {
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        _power = 2f; // base power is 2 damage
        _speed = 8f; // base speed is 8 units
        Transform _parent = transform.parent;

        if (_parent != null)
        {
            if (_parent.name.Contains("TripleShot"))
            {
                // The tripleshot is weaker per laser, but if you hit an enemy with 2 or 3 of them, you do more damage.
                _power = 1.5f;
                _speed = 9.0f;
                // Change the color to green because we're super powerful
                _l_spriteRenderer = GetComponent<SpriteRenderer>();
                if (_l_spriteRenderer != null) { _l_spriteRenderer.color = Color.green; }
                if (_audioManager == null) Debug.LogError("Laser::Start() :: We have a problem. The audioManager is null");
                else
                {
                    _parent.gameObject.AddComponent<AudioSource>();
                    _l_sounds = _parent.gameObject.GetComponent<AudioSource>();
                    _l_sounds.clip = _audioManager.getWeaponSound();
                    _l_sounds.loop = false;
                    _l_sounds.volume = 0.75f;
                    _l_sounds.pitch = 0.5f;
                }
                _l_sounds.volume = 0.5f;
                _l_sounds.pitch = 0.25f;
            }
            // add other variations of the laser here as elseif comparisons to the parent name
        }
        else
        {
            if (_audioManager == null) Debug.LogError("Laser::Start() :: We have a problem. The audioManager is null");
            else
            {
                this.gameObject.AddComponent<AudioSource>();
                _l_sounds = this.GetComponent<AudioSource>();
                _l_sounds.clip = _audioManager.getWeaponSound();
                _l_sounds.loop = false;
                _l_sounds.volume = 0.75f;
                _l_sounds.pitch = 0.5f;
            }
        }
        _l_sounds.Play();
    }

    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.parent != null) { Destroy(transform.parent.gameObject, 1.9f); }
        else { Destroy(this.gameObject, 1.9f); }
    }

    public float GetPower()
    {
        return _power;
    }
}
