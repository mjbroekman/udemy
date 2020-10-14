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
            Player p = other.GetComponent<Player>();
            if (p != null)
            {
                Debug.Log("Player collected a coin worth " + _value.ToString() + " monetary units.");
                p.AddCoins(_value);
            }
            Destroy(this.gameObject);
        }
    }
}
