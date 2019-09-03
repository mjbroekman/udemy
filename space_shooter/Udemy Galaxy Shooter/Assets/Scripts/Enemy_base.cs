using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_base : MonoBehaviour
{
    [SerializeField]
    private float _curSpd;

    [SerializeField]
    private int _maxLife = 10;

    private float _maxH = 9.5f;
    private float _maxV = 6.5f;
    private float _randomX;
    private int _curLife;

    // Start is called before the first frame update
    void Start()
    {
        _randomX = Random.Range(-_maxH, _maxH);
        transform.position = new Vector3(_randomX, _maxV, 0.0f);
        _curSpd = 2.0f + Time.time / 36;
        _curLife = 1 + (int)(Time.time / 36);
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        string _what = other.tag;
        Debug.Log("Hit " + _what);
        if (_what == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null) { player.TakeDamage((int)_curSpd); }
            // apply the maximum amount of damage I will ever be able to take.
            TakeDamage(_maxLife);
        }
        else if (_what == "Laser")
        {
            Laser laser = other.transform.GetComponent<Laser>();
            int damage = 0;

            if (laser != null) { damage = laser.GetPower(); }

            Destroy(other.gameObject);
            TakeDamage(damage);
        }
    }

    /// <summary>
    /// Apply damage to the enemy.
    /// </summary>
    /// <param name="damage"></param>
    void TakeDamage(int damage)
    {
        _curLife -= damage;
        Debug.Log(this.transform.name + " took " + damage + " damage from hit.");
        if (_curLife <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
