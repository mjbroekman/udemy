using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_90 : MonoBehaviour
{
    // Start is called before the first frame update
    private float _speed = 8.0f;

    void Start()
    {

    }

    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        Destroy(this.gameObject, 2.0f);
    }
}
