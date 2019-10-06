using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private string _variant;
    private float _strength;
    private float _speed = 2f;

    // Screen boundaries and positioning
    private GameManager _gameManager;
    private float _maxH;
    private float _maxV;
    private float _minV;
    private float _randomX;

    private AudioSource _e_sounds;
    private AudioManager _audioManager;

    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("PowerUp::Start() :: We have a problem. The gameManager is null");
        }

        float[] _bounds = _gameManager.GetScreenBoundaries(this.gameObject);
        _maxH = _bounds[1];
        _minV = _bounds[2];
        _maxV = _bounds[3];
        _speed += _gameManager.GetLevel() / 2f;

        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (_audioManager == null) Debug.LogError("PowerUp::Start() :: We have a problem. The audioManager is null");
        else
        {
            this.gameObject.AddComponent<AudioSource>();
            _e_sounds = this.GetComponent<AudioSource>();
            _e_sounds.clip = _audioManager.GetEffectSound("PowerUp");
            _e_sounds.loop = false;
        }

        if (name.Contains("Triple_Shot")) { _variant = "TripleShot"; _strength = 5f; }
        if (name.Contains("Shield")) { _variant = "Shield"; _strength = 20f; }
        if (name.Contains("Speed_Boost")) { _variant = "Boost"; _strength = 5f; }

        _randomX = Random.Range(-_maxH, _maxH);
        transform.position = new Vector3(_randomX, _maxV, 0.0f);
    }

    void Update()
    {
        if (gameObject.GetComponent<SpriteRenderer>().enabled)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            if (transform.position.y < _minV * 1.1) { Destroy(this.gameObject); }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string _what = other.tag;
        if (_what == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null) { player.CollectPowerUp(_variant, _strength); }

            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            AudioSource.PlayClipAtPoint(_e_sounds.clip, transform.position);

            Destroy(gameObject);
        }
    }
}
