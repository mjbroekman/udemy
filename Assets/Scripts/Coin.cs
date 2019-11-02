using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private int _value;

    // Start is called before the first frame update
    void Start()
    {
        _value = Random.Range(1, 5);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetComponent<Player>().AddCoins(_value);
            Destroy(this);
        }
    }
}
