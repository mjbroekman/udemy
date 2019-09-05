using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Base attributes
    [SerializeField]
    private float _power;
    private float _speed;

    // SpriteRenderer to be able to modify the laser depending on powerups
    private SpriteRenderer _l_spriteRenderer;

    // Determine if the laser is part of a bigger object
    private bool _hasParent;

    /// <summary>
    /// Set up the basic stats of this laser instance.
    /// </summary>
    private void Start()
    {
        if (this.transform.parent == null)
        {
            Debug.Log("I am a laser. Just a plain boring laser.");
            _power = 2f;
            _speed = 8f;
            _hasParent = false;
        }
        else if (ParentIs(this.transform.parent.name, "TripleShot"))
        {
            Debug.Log("I am a laser. My parent is " + this.transform.parent.name);
            // The tripleshot is weaker per laser, but if you hit an enemy with 2 or 3 of them, you do more damage.
            _power = 1.5f;
            _speed = 9.0f;
            // Change the color to green because we're super powerful
            _l_spriteRenderer = GetComponent<SpriteRenderer>();
            _l_spriteRenderer.color = Color.green;
            _hasParent = true;
        }
        else
        {
            _power = 2f;
            _speed = 8.0f;
            _hasParent = true;
        }
    }

    /// <summary>
    /// Move up from the player to the top of the screen
    /// Destroy after 2 seconds
    /// </summary>
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (_hasParent)
        {
            Destroy(this.transform.parent.gameObject, 1.9f);
        }
        Destroy(this.gameObject, 1.9f);
    }

    /// <summary>
    /// Return the strength of the laser
    /// </summary>
    /// <returns></returns>
    public float GetPower()
    {
        return _power;
    }

    private static bool ParentIs(string parent, string what)
    {
        int pLen = parent.Length;
        int wLen = what.Length;
        int pPos = 0; int wPos = 0;

        while (pPos < pLen && wPos < wLen && parent[pPos] == what[wPos])
        {
            pPos++;
            wPos++;
        }

        return (wPos == wLen && pLen >= wLen) ||

                (pPos == pLen && wLen >= pLen);
    }

}
