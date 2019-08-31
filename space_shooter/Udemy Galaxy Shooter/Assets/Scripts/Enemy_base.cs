using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_base : MonoBehaviour
{
    [SerializeField]
    private float _curSpd;

    [SerializeField]
    private float _maxLife = 10.0f;

    private float _maxH = 9.5f;
    private float _maxV = 6.5f;
    private float _randomX;
    private float _curLife;

    // Start is called before the first frame update
    void Start()
    {
        _randomX = Random.Range(-_maxH, _maxH);
        transform.position = new Vector3(_randomX, _maxV, 0.0f);
        _curSpd = 2.0f + Time.time / 36;
        _curLife = 0.1f + Time.time / 36;
        if (_curLife > _maxLife) { _curLife = _maxLife; }
    }

    /// <summary>
    /// Updates per frame. Move down and warp back to the top of the screen.
    /// </summary>
    void Update()
    {
        transform.Translate(Vector3.down * _curSpd * Time.deltaTime);
        if (transform.position.y < _maxV * -1.1)
        {
            _randomX = Random.Range(-_maxH, _maxH);
            transform.position = new Vector3(_randomX, _maxV, 0.0f);
            _curSpd = 2.0f + Time.time / 36;
        }
    }

    /// <summary>
    /// Handle collisions
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        string _what = other.tag;
        Debug.Log("Hit " + _what);
        if (_what == "Player")
        {
            other.transform.SendMessage("TakeDamage", _curSpd);
            TakeDamage(_maxLife);
        }
        else if (_what == "Laser")
        {
            // change this to get the strength of the laser
            float _damage = 1f;
            Destroy(other.gameObject);
            TakeDamage(_damage);
        }
        // if we hit the player, damage player, destroy us
        // if we hit the laser, destroy the laser, then do something to us
    }

    /// <summary>
    /// Apply damage to the enemy.
    /// </summary>
    /// <param name="damage"></param>
    void TakeDamage(float damage)
    {
        _curLife -= damage;
        Debug.Log(this.transform.name + " took " + damage + " damage from hit.");
        if (_curLife <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
