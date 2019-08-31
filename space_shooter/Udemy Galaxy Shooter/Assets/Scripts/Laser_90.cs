using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_90 : MonoBehaviour
{
    // Start is called before the first frame update
    private float _speed = 8.0f;
    public float power = 1.0f;

    /// <summary>
    /// Move up from the player to the top of the screen
    /// Destroy after 2 seconds
    /// </summary>
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        Destroy(this.gameObject, 2.0f);
    }

    /// <summary>
    /// Return the strength of the laser
    /// </summary>
    /// <returns></returns>
    float GetPower()
    {
        return power;
    }
}
