using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sharkman : MonoBehaviour
{
    [SerializeField]
    private AudioClip _purchase;

    public int Value { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _purchase = Resources.Load<AudioClip>("Sounds/You_Win");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player _p = other.GetComponent<Player>();
            if (_p == null) return;

            Debug.Log("Player collided! Press E for everything good!");
            if (Input.GetKey(KeyCode.E) )
            {
                if (_p.Money > 0)
                {
                    Debug.Log("Look out! Player has a gun!");
                    _p.AddCash(_p.Money * -1);
                    _p.EnableWeapons();
                    AudioSource.PlayClipAtPoint(_purchase, transform.position, 1f);
                }
                else
                {
                    Debug.Log("Get outta here, cheapskate!");
                }
            }
        }
    }
}
