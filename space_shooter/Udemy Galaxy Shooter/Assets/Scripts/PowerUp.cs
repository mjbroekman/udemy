﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // This is the type of powerUp (TripleShot, Shield, Boost, etc)
    private string _variant;
    // This is how long the powerUp lasts (duration in seconds or damage absorbed)
    private float _strength;
    // This is how fast the powerUp 'falls'
    private readonly float _speed = 3f;

    // Screen boundaries and positioning
    private readonly float _maxH = 9.5f;
    private readonly float _maxV = 6.5f;
    private float _randomX;

    // Start is called before the first frame update
    void Start()
    {
        if (name.Contains("Triple_Shot")) { _variant = "TripleShot"; _strength = 5f; }
        if (name.Contains("Shield")) { _variant = "Shield"; _strength = 20f; }
        if (name.Contains("Speed_Boost")) { _variant = "Boost"; _strength = 5f; }
        _randomX = Random.Range(-_maxH, _maxH);
        transform.position = new Vector3(_randomX, _maxV, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        // If the powerup falls off the screen, say goodbye Gracie
        if (transform.position.y < _maxV * -1.1) { Destroy(this.gameObject); }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string _what = other.tag;
        if (_what == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            Debug.Log("PowerUp::OnTriggerEnter2D() :: Attempting to get Player to CollectPowerUp(" + _variant + ", " + _strength + ")");
            if (player != null) { player.CollectPowerUp(_variant, _strength); }
            Destroy(gameObject);
        }
    }
}
